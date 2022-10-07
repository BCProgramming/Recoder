using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Taskbar;
using Un4seen.Bass;
using Un4seen.Bass.Misc;

namespace Recoder
{
    public partial class frmRecoder : Form
    {
        public frmRecoder()
        {
            InitializeComponent();
        }

        private void cmdBrowseSource_Click(object sender, EventArgs e)
        {
            String strFolder;
            bool isSource = sender == cmdBrowseSource;
            String InitialFolder = isSource ? txtSourceFolder.Text : txtTargetFolder.Text;
            String PromptText = "Select " + (isSource ? "Source" : "Target") + " Folder.";
            CommonOpenFileDialog cofd = new CommonOpenFileDialog(PromptText);
            cofd.IsFolderPicker = true;
            cofd.InitialDirectory = InitialFolder;
            cofd.AllowNonFileSystemItems = false;
            cofd.EnsurePathExists = true;
            cofd.Multiselect = false;
            if(cofd.ShowDialog()==CommonFileDialogResult.Ok)
            {
                strFolder = cofd.FileName;
            }
            else
            {
                return;
            }
            if (isSource)
            {
                txtSourceFolder.Text = strFolder;
            }
            else
            {
                txtTargetFolder.Text = strFolder;
            }
        }

        private bool Reencoding = false;
        private Task RunningTask = null;

        private void cmdBegin_Click(object sender, EventArgs e)
        {
            if (Reencoding)
            {
                Reencoding = false;
                cancelSource.Cancel();
            }
            else
            {
                //Source path must exist.
                String StrSource, StrTarget;
                StrSource = txtSourceFolder.Text;
                StrTarget = txtTargetFolder.Text;
                if (!Directory.Exists(StrSource))
                {
                    MessageBox.Show("Source Folder does not exist.");
                    return;
                }
                else if (Directory.Exists(StrTarget))
                {
                    if (
                        MessageBox.Show
                            ("The Target folder is already present. Existing files will be overwritten. Continue?", "Continue", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.No)
                    {
                        return;
                    }
                }
                //it all checks out- start the re-encoding process.
                cancelSource = new CancellationTokenSource();
                try
                {
                    cmdBegin.Text = "Cancel";
                    
                    StartReencoding(StrSource, StrTarget, txtSourceExtensions.Text.Split(','),GetSelectedFormat(), chkCopyTarget.Checked,cancelSource.Token);
                }
                catch (TaskCanceledException tce)
                {
                
                cmdBegin.Text = "Begin";
                cmdBegin.Enabled = true;
                pBarCurrentFile.Value = 0;
                pBarTask.Value = 0;
                lblCurrentFile.Text = "Cancelled.";
                lblEntireTask.Text = "Cancelled.";
                lblStatistics.Text = "Operation Cancelled.";
                }
            }
        }

        private CancellationTokenSource cancelSource;
        private CancellationToken ActiveToken;

        private void GetFileDecompressedSize(String sFilename)
        {
            int fLength = (int)new FileInfo(sFilename).Length;
            byte[] FileContents = new byte[fLength];
            using (FileStream fs = new FileStream(sFilename,FileMode.Open))
            {
                fs.Read(FileContents, 0, fLength);
            }
            IntPtr unmanagedPointer = Marshal.AllocHGlobal(FileContents.Length);
            Marshal.Copy(FileContents, 0, unmanagedPointer, FileContents.Length);

            int loadedsample = Bass.BASS_StreamCreateFile(unmanagedPointer, 0L, 0L, BASSFlag.BASS_DEFAULT);
            
            
            Bass.BASS_StreamFree(loadedsample);

            Marshal.FreeHGlobal(unmanagedPointer);

        }

       
        
        private ReEncoder ActiveReCoder = null;
        private FileSizeData[] SizeResults = null;
        private Stopwatch ProcessTimer = null;
        int completedcount = 0;
        private async void StartReencoding(String strSource, String strTarget, String[] sSourceExtensions,ReEncoder.TargetFormatConstants TargetType,bool fCopyTargetFromSource, CancellationToken canceltoken)
        {
            ActiveToken = canceltoken;
            //change begin button into Cancel button.
            Reencoding = true;
            cmdBegin.Text = "Cancel";

            await Task.Run
                (() =>
                {
                    ProcessTimer = new Stopwatch();
                    
                    //calculate the total size of all processable files.
                    String[] inputextensions = sSourceExtensions;
                    Invoke((MethodInvoker) (() => { lblStatistics.Text = " Calculating Total Size..."; }));
                    TotalInputSize = ReEncoder.SizeFolder(strSource, ref TotalFiles, out SizeResults,inputextensions);
                    
                    Invoke((MethodInvoker) (() => { lblStatistics.Text = " Processing " + TotalFiles + " Total Files, Size:" + ByteSizeFormatter.FormatSize(TotalInputSize); }));

                    ReEncoder re=null;
                    Invoke((MethodInvoker)(()=>{
                        re = new ReEncoder(inputextensions);
                        re.TargetFormat = TargetType;

                    re.BitRate = getRate((String) cboBitRate.SelectedItem);
                    ActiveReCoder = re;
                    re.OnBeginReencode += re_OnBeginReencode;
                    re.OnFinishReencode += re_OnFinishReencode;
                    re.OnProgress += re_OnProgress;
                    }));
                    ProcessTimer.Start();
                    var EncodeResults = re.EncodeFolder(strSource, strTarget,TargetType,null,fCopyTargetFromSource);
                    Invoke
                        ((MethodInvoker) (() =>
                        {
                            Reencoding = false;
                            cmdBegin.Text = "Begin";
                            DisplayResults(EncodeResults);
                        }));
                });
        }

        private void DisplayResults(ReencodeResults rresults)
        {
            ProcessTimer.Stop();
            TimeSpan Elapsed = ProcessTimer.Elapsed;
            String TimeData = Elapsed.ToString();
            String strtext =
                @"Processed {0} Files in {3}
                 Input Size: {1} (Processed {4}/s)
                 Output Size: {2} (processed {5}/s)";

            String inputfmt = ByteSizeFormatter.FormatSize(rresults.OriginalBytes);
            String outputfmt = ByteSizeFormatter.FormatSize(rresults.TargetBytes);

            int inputbytespeed = (int)(rresults.OriginalBytes / Elapsed.TotalSeconds);
            int outputbytespeed = (int)(rresults.TargetBytes / Elapsed.TotalSeconds);
            String inputspeedfmt = ByteSizeFormatter.FormatSize(inputbytespeed, 1, true);
            String outputspeedfmt = ByteSizeFormatter.FormatSize(outputbytespeed, 1, true);
            

            lblStatistics.Text = String.Format(strtext, rresults.FileCount, inputfmt, outputfmt,TimeData,inputspeedfmt,outputspeedfmt);
            lblEntireTask.Text = "Processing Complete.";
            lblCurrentFile.Text = "";
            pBarTask.Value = pBarTask.Maximum;

        }
        long lasttotalfinished = 0;
        private void re_OnProgress(object sender, ReEncoder.ProgressEventArgs e)
        {
            
            Invoke
                ((MethodInvoker) (() =>
                {
                    
                    ActiveToken.ThrowIfCancellationRequested();
                    var Range = pBarCurrentFile.Maximum - pBarCurrentFile.Minimum;
                    var useValue = (Range*e.Percentage) + pBarCurrentFile.Minimum;
                    useValue = useValue > 100 ? 100 : useValue < 0 ? 0 : useValue;
                    pBarCurrentFile.Value = (int) useValue;
                    
                    //rework: total progress will be calculated based on a base percent,
                    //calculated by the number of completed files * range/total files
                    //from there we add in the percentage of the current file/TotalFiles.
                    float baseCompletion = (1 / (float)TotalFiles) * finishedFiles;
                    baseCompletion += (1 / (float)TotalFiles) * e.Percentage;

                    
                    
                    


                    long totalFinishedBytes = finishedBytes + e.BytesProcessed; 
                    if(lasttotalfinished> totalFinishedBytes)
                    {
                        Debug.Print("mysterious");
                    }
                    float totalPercent = baseCompletion;
                    Debug.Print("OnProgress:" + "Percent:" + e.Percentage + " Bytes Total:" + e.BytesTotal + " Bytes Processed:" + e.BytesProcessed + " Total finished:" + totalFinishedBytes + " Total size:" + TotalInputSize + " TotalPercent:" + totalPercent);

                    int useTotal = (int) ((pBarTask.Maximum - pBarTask.Minimum)*totalPercent) + pBarTask.Minimum;
                    if (useTotal > 100) useTotal = 100;
                    pBarTask.Value = useTotal;

                    lblCurrentFile.Text = " Re-encoding " + CurrentSourceFile + " " + String.Format("{0:%}", e.Percentage);
                    lblEntireTask.Text = (TotalFiles - finishedFiles).ToString() + " Files Left.";

                    if (TaskbarManager.IsPlatformSupported)
                    {
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                        TaskbarManager.Instance.SetProgressValue(pBarTask.Value, pBarTask.Maximum, Handle);
                    }
                    lasttotalfinished = totalFinishedBytes;
                }
                    ));
        }

        private void re_OnFinishReencode(object sender, ReEncoder.ReencodeEventArgs e)
        {
            
            finishedFiles++;
            finishedBytes += ReEncoder.GetDecodedLength(e.SourceFile);
        }
        private long fileBytes = 0;
        private long finishedBytes = 0; //all finished bytes, incremented when files are finished being re-encoded.
        private long finishedFiles = 0;
        private int TotalFiles = 0;
        private long TotalInputSize = 0;
        private String CurrentSourceFile;
        private String CurrentTargetFile;

        private void re_OnBeginReencode(object sender, ReEncoder.ReencodeEventArgs e)
        {
            fileBytes = 0;
            CurrentSourceFile = e.SourceFile;
            CurrentTargetFile = e.TargetFile;
            Invoke((MethodInvoker) (() => { lblCurrentFile.Text = "Processing:" + Path.GetFileName(e.SourceFile); }));
            //throw new NotImplementedException();
        }

        private void frmRecoder_Load(object sender, EventArgs e)
        {
            String ProgramVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Text = "BASeCamp Recoder (v" + ProgramVersion + ")";
            cboBitRate.Items.AddRange(new String[] {"128", "160", "192", "224", "256", "320"});
            cboBitRate.SelectedIndex = 5;
            cboTargetType.Items.Add("MPEG Layer-3 (MP3)");
            cboTargetType.Items.Add("OGG Theora (OGG)");
            cboTargetType.Items.Add("Free Lossless Audio Codec (FLAC)");
            cboTargetType.SelectedIndex = 0;
        }
        private ReEncoder.TargetFormatConstants GetSelectedFormat()
        {
            switch (cboTargetType.SelectedIndex)
            {
            case 0: 
                return ReEncoder.TargetFormatConstants.Target_MP3;
            case 1:
                return ReEncoder.TargetFormatConstants.Target_OGG;
            case 2:
                return ReEncoder.TargetFormatConstants.Target_FLAC;
            }
            return ReEncoder.TargetFormatConstants.Target_MP3;
            
        }
        private BaseEncoder.BITRATE getRate(String src)
        {
            if (src == "128")
            {
                return BaseEncoder.BITRATE.kbps_128;
            }
            if (src == "160")
            {
                return BaseEncoder.BITRATE.kbps_160;
            }
            if (src == "192")
            {
                return BaseEncoder.BITRATE.kbps_192;
            }
            if (src == "224")
            {
                return BaseEncoder.BITRATE.kbps_224;
            }
            if (src == "256")
            {
                return BaseEncoder.BITRATE.kbps_256;
            }
            if (src == "320")
            {
                return BaseEncoder.BITRATE.kbps_320;
            }
            return BaseEncoder.BITRATE.kbps_128;
        }
    }
}