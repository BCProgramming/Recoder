using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.WindowsAPICodePack.Taskbar;
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
            FolderBrowserDialog fbd = new FolderBrowserDialog()
            {
                Description = "Select " + (isSource ? "Source" : "Target") + " Folder.",
                SelectedPath = InitialFolder,
                ShowNewFolderButton = true
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                strFolder = fbd.SelectedPath;
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
                    StartReencoding(StrSource, StrTarget, txtSourceExtensions.Text.Split(','), cancelSource.Token);
                }
                catch (TaskCanceledException tce)
                {
                }
                cmdBegin.Text = "Begin";
                cmdBegin.Enabled = true;
                pBarCurrentFile.Value = 0;
                pBarTask.Value = 0;
                lblCurrentFile.Text = "Cancelled.";
                lblEntireTask.Text = "Cancelled.";
                lblStatistics.Text = "Operation Cancelled.";
            }
        }

        private CancellationTokenSource cancelSource;
        private CancellationToken ActiveToken;

        private long SizeFolder(String strFolder, ref int FileCount, String[] testextensions = null)
        {
            long Runner = 0;
            DirectoryInfo di = new DirectoryInfo(strFolder);
            foreach (FileInfo fi in di.GetFiles())
            {
                if (testextensions == null || testextensions.Any((String a) => a.Equals(fi.Extension, StringComparison.OrdinalIgnoreCase)))
                {
                    Runner += fi.Length;
                    FileCount++;
                }
            }
            foreach (DirectoryInfo subdir in di.GetDirectories())
            {
                Runner += SizeFolder(subdir.FullName, ref FileCount, testextensions);
            }
            return Runner;
        }

        private ReEncoder ActiveReCoder = null;

        private async void StartReencoding(String strSource, String strTarget, String[] sSourceExtensions, CancellationToken canceltoken)
        {
            ActiveToken = canceltoken;
            //change begin button into Cancel button.
            Reencoding = true;
            cmdBegin.Text = "Cancel";

            await Task.Run
                (() =>
                {
                    //calculate the total size of all processable files.
                    String[] inputextensions = sSourceExtensions;
                    Invoke((MethodInvoker) (() => { lblStatistics.Text = " Calculating Total Size..."; }));
                    TotalInputSize = SizeFolder(strSource, ref TotalFiles, inputextensions);

                    Invoke((MethodInvoker) (() => { lblStatistics.Text = " Processing " + TotalFiles + " Total Files, Size:" + ByteSizeFormatter.FormatSize(TotalInputSize); }));

                    ReEncoder re=null;
                    Invoke((MethodInvoker)(()=>{
                        re = new ReEncoder(inputextensions, ".mp3");

                    re.BitRate = getRate((String) cboBitRate.SelectedItem);
                    ActiveReCoder = re;
                    re.OnBeginReencode += re_OnBeginReencode;
                    re.OnFinishReencode += re_OnFinishReencode;
                    re.OnProgress += re_OnProgress;
                    }));
                    var EncodeResults = re.EncodeFolder(strSource, strTarget);
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
            String strtext =
                @"Processed {0} Files.
                 Input Size: {1}
                 Output Size: {2}";

            String inputfmt = ByteSizeFormatter.FormatSize(rresults.OriginalBytes);
            String outputfmt = ByteSizeFormatter.FormatSize(rresults.TargetBytes);
            lblStatistics.Text = String.Format(strtext, rresults.FileCount, inputfmt, outputfmt);
        }

        private void re_OnProgress(object sender, ReEncoder.ProgressEventArgs e)
        {
            Invoke
                ((MethodInvoker) (() =>
                {
                    ActiveToken.ThrowIfCancellationRequested();
                    var Range = pBarCurrentFile.Maximum - pBarCurrentFile.Minimum;
                    var useValue = (Range*e.Percentage) + pBarCurrentFile.Minimum;
                    pBarCurrentFile.Value = (int) useValue;

                    long totalFinishedBytes = finishedBytes + e.BytesProcessed;
                    float totalPercent = (float) ((double) totalFinishedBytes/(double) TotalInputSize);

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
                }
                    ));
        }

        private void re_OnFinishReencode(object sender, ReEncoder.ReencodeEventArgs e)
        {
            finishedFiles++;
            finishedBytes += new FileInfo(e.SourceFile).Length;
        }

        private long finishedBytes = 0; //all finished bytes, incremented when files are finished being re-encoded.
        private long finishedFiles = 0;
        private int TotalFiles = 0;
        private long TotalInputSize = 0;
        private String CurrentSourceFile;
        private String CurrentTargetFile;

        private void re_OnBeginReencode(object sender, ReEncoder.ReencodeEventArgs e)
        {
            CurrentSourceFile = e.SourceFile;
            CurrentTargetFile = e.TargetFile;
            Invoke((MethodInvoker) (() => { lblCurrentFile.Text = "Processing:" + Path.GetFileName(e.SourceFile); }));
            //throw new NotImplementedException();
        }

        private void frmRecoder_Load(object sender, EventArgs e)
        {
            cboBitRate.Items.AddRange(new String[] {"128", "160", "192", "224", "256", "320"});
            cboBitRate.SelectedIndex = 5;
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