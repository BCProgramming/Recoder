using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Markup;
using Un4seen.Bass;
using Un4seen.Bass.Misc;

namespace Recoder
{
    public class ReEncoder
    {
        //represents a single Work Item that can be queued up.
        public class EncoderWorkItem
        {
            private String InputFile;
            private String OutputFile;

            public EncoderWorkItem(String pInput, String pOutput)
            {
                InputFile = pInput;
                OutputFile = pOutput;
            }
        }

        public class ProgressEventArgs : EventArgs
        {
            //there are a few progress-based things we track.
            //first, this only actually works on a per-file basis.

            public long BytesProcessed;
            public long BytesTotal;


            public float Percentage
            {
                get { return (float) BytesProcessed/(float) BytesTotal; }
            }

            public ProgressEventArgs(long pProcessed, long pTotal)
            {
                BytesProcessed = pProcessed;
                BytesTotal = pTotal;
            }
        }

        public class ReencodeEventArgs : EventArgs
        {
            public String SourceFile;
            public String TargetFile;

            public ReencodeEventArgs(String pSourceFile, String pTargetFile)
            {
                SourceFile = pSourceFile;
                TargetFile = pTargetFile;
            }
        }
        public enum TargetFormatConstants
        {
            Target_MP3,
            Target_FLAC,
            Target_OGG
        }

        public event EventHandler<ProgressEventArgs> OnProgress;
        public event EventHandler<ReencodeEventArgs> OnBeginReencode;
        public event EventHandler<ReencodeEventArgs> OnFinishReencode;
        private static readonly String[] DefaultExtensions = {".flac", ".wma"};
        private String[] InputExtensions = null;
        private BaseEncoder.BITRATE _BitRate = BaseEncoder.BITRATE.kbps_320;
        private BaseEncoder.SAMPLERATE _SampleRate = BaseEncoder.SAMPLERATE.Hz_44100;
        private TargetFormatConstants _TargetFormat = TargetFormatConstants.Target_MP3;
        private static Dictionary<TargetFormatConstants, String> TargetFormatExtension = new Dictionary<TargetFormatConstants, string>()
        {
            {TargetFormatConstants.Target_MP3, ".mp3"},
            {TargetFormatConstants.Target_FLAC, ".flac"},
            {TargetFormatConstants.Target_OGG, ".ogg"}
        };
        public TargetFormatConstants TargetFormat { get { return _TargetFormat; } set { _TargetFormat = value; } }
        public BaseEncoder.BITRATE BitRate
        {
            get { return _BitRate; }
            set { _BitRate = value; }
        }

        public BaseEncoder.SAMPLERATE SampleRate
        {
            get { return _SampleRate; }
            set { _SampleRate = value; }
        }

        private void InvokeProgress(ProgressEventArgs args)
        {
            var copied = OnProgress;
            if (copied == null) return;
            copied(this, args);
        }

        private void InvokeBeginReencode(ReencodeEventArgs args)
        {
            var copied = OnBeginReencode;
            if (copied == null) return;
            copied(this, args);
        }

        private void InvokeFinishReencode(ReencodeEventArgs args)
        {
            var copied = OnFinishReencode;
            if (copied == null) return;
            copied(this, args);
        }
        public static long GetDecodedLength(String sFilename)
        {
            int hStream = Bass.BASS_StreamCreateFile(sFilename, 0, 0, BASSFlag.BASS_DEFAULT);
            try
            {
                long result = Bass.BASS_ChannelGetLength(hStream, BASSMode.BASS_POS_BYTES);
                return result;
            }
            finally
            {
                Bass.BASS_StreamFree(hStream);
            }
        }
        private void EncodeFileProc(long bytesTotal, long bytesDone)
        {
            if (hStream != 0)
            {
                
                
                InvokeProgress(new ProgressEventArgs(bytesDone, bytesTotal));
            }
        }

        public ReEncoder(String[] FileExtensions)
        {
            InputExtensions = FileExtensions;
        }
        int hStream = 0;
        FileInfo ProcessingFile = null;
        public bool ConvertFile(String pSource, String pTarget)
        {
            
            InvokeBeginReencode(new ReencodeEventArgs(pSource, pTarget));
            hStream = Bass.BASS_StreamCreateFile(pSource, 0, 0, BASSFlag.BASS_DEFAULT);
            ProcessingFile = new FileInfo(pSource);
            //Bass.BASS_ChannelPlay(hStream, false);
            BaseEncoder useEncoder = null;
            if (TargetFormat == TargetFormatConstants.Target_MP3)
            {
                var l = new EncoderLAME(hStream);
                l.InputFile = null; //STDIN (recording)
                l.OutputFile = pTarget;
                l.LAME_Bitrate = (int)_BitRate;
                l.LAME_Mode = EncoderLAME.LAMEMode.Stereo;
                l.LAME_TargetSampleRate = (int)_SampleRate;
                l.LAME_Quality = EncoderLAME.LAMEQuality.Quality;
                useEncoder = l;
            }
            else if(TargetFormat == TargetFormatConstants.Target_FLAC)
            {
                var flacencode = new EncoderFLAC(hStream);
                flacencode.InputFile = null;
                flacencode.OutputFile = pTarget;
                useEncoder = flacencode;
            }
            else if(TargetFormat == TargetFormatConstants.Target_OGG)
            {
                var oggencode = new EncoderOGG(hStream);
                oggencode.InputFile = null;
                oggencode.OutputFile = pTarget;
                oggencode.OGG_Bitrate = (int)_SampleRate;
                
                useEncoder = oggencode;
            }
            bool runresult = BaseEncoder.EncodeFile(pSource, pTarget, useEncoder, EncodeFileProc, true, false);
            InvokeFinishReencode(new ReencodeEventArgs(pSource, pTarget));
            Bass.BASS_StreamFree(hStream);
            return true;
        }

        public ReencodeResults EncodeFolder(String SourceFolder, String TargetFolder, TargetFormatConstants TargetFormat = TargetFormatConstants.Target_MP3, String[] ValidExtensions = null)
        {
            int Count = 0;
            long SourceBytes = 0;
            long TargetBytes = 0;
            String TargetExtension = TargetFormatExtension[TargetFormat];
            if (ValidExtensions == null) ValidExtensions = InputExtensions;
            //loop through each file, and convert each one.
            if (!Directory.Exists(TargetFolder))
            {
                Directory.CreateDirectory(TargetFolder);
            }
            var di = new DirectoryInfo(SourceFolder);
            foreach (FileInfo fi in di.GetFiles())
            {
                if (ValidExtensions.Any(a => a.Equals(fi.Extension, StringComparison.OrdinalIgnoreCase)))
                {
                    String sFilePart = Path.GetFileNameWithoutExtension(fi.FullName);

                    String TargetFile = Path.Combine(TargetFolder, sFilePart + TargetExtension);
                    //Console.WriteLine("Re-Encoding " + Path.GetFileNameWithoutExtension(fi.FullName));
                    //Console.WriteLine("Target:" + Path.GetFileNameWithoutExtension(TargetFile));

                    ConvertFile(fi.FullName, TargetFile);
                    Count++;
                    SourceBytes += fi.Length;
                    TargetBytes += new FileInfo(TargetFile).Length;
                }
            }
            //now subfolders.
            foreach (DirectoryInfo subdir in di.GetDirectories())
            {
                String TargetName = Path.Combine(TargetFolder, subdir.Name);
                ReencodeResults EncodeResults = EncodeFolder(subdir.FullName, TargetName, TargetFormat, ValidExtensions);
                Count += EncodeResults.FileCount;
                SourceBytes += EncodeResults.OriginalBytes;
                TargetBytes += EncodeResults.TargetBytes;
            }
            return new ReencodeResults(Count, SourceBytes, TargetBytes);
        }

        public static long SizeFolder(String strFolder, ref int FileCount, out FileSizeData[] FileResults,String[] testextensions = null)
        {
            List<FileSizeData> BuildResult = new List<FileSizeData>();
            long Runner = 0;
            DirectoryInfo di = new DirectoryInfo(strFolder);
            foreach (FileInfo fi in di.GetFiles())
            {
                if (testextensions == null || testextensions.Any((String a) => a.Equals(fi.Extension, StringComparison.OrdinalIgnoreCase)))
                {
                    //Runner += fi.Length;
                    Runner += ReEncoder.GetDecodedLength(fi.FullName);
                    BuildResult.Add(new FileSizeData(fi.FullName,fi.Length,Runner));
                    FileCount++;
                }
            }
            foreach (DirectoryInfo subdir in di.GetDirectories())
            {
                FileSizeData[] dirresults;
                Runner += SizeFolder(subdir.FullName, ref FileCount, out dirresults,testextensions);
                foreach (FileSizeData addresult in dirresults) BuildResult.Add(addresult); 
            }
            List<FileSizeData> resultData = new List<FileSizeData>();
            long accumulated = 0;
            foreach(FileSizeData loopfile in BuildResult)
            {
                accumulated += loopfile.FileSize;
                
                resultData.Add(loopfile);
            }
            FileResults = resultData.ToArray();
            return Runner;
        }

    }
    public class FileSizeData
    {
        public String FullName;
        public long FileSize;
        public long AccumulatedSize;
        public FileSizeData(String pFullName,long pFileSize,long pAccumulatedSize)
        {
            FullName = pFullName;
            FileSize = pFileSize;
            AccumulatedSize = pAccumulatedSize;
        }
    }
    public class ReencodeResults
    {
        public ReencodeResults(int pFileCount, long pOriginal, long pTarget)
        {
            FileCount = pFileCount;
            OriginalBytes = pOriginal;
            TargetBytes = pTarget;
        }

        public int FileCount { get; private set; }
        public long OriginalBytes { get; private set; }
        public long TargetBytes { get; private set; }
    }
}