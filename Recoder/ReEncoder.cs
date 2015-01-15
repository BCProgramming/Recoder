using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        public event EventHandler<ProgressEventArgs> OnProgress;
        public event EventHandler<ReencodeEventArgs> OnBeginReencode;
        public event EventHandler<ReencodeEventArgs> OnFinishReencode;
        private static readonly String[] DefaultExtensions = {".flac", ".wma"};
        private String[] InputExtensions = null;
        private String OutputExtension = null;
        private BaseEncoder.BITRATE _BitRate = BaseEncoder.BITRATE.kbps_320;
        private BaseEncoder.SAMPLERATE _SampleRate = BaseEncoder.SAMPLERATE.Hz_44100;

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

        private void EncodeFileProc(long bytesTotal, long bytesDone)
        {
            InvokeProgress(new ProgressEventArgs(bytesDone, bytesTotal));
        }

        public ReEncoder(String[] FileExtensions, String pOutputExtension)
        {
            InputExtensions = FileExtensions;
            OutputExtension = pOutputExtension;
        }

        public bool ConvertFile(String pSource, String pTarget)
        {
            InvokeBeginReencode(new ReencodeEventArgs(pSource, pTarget));
            int hStream = Bass.BASS_StreamCreateFile(pSource, 0, 0, BASSFlag.BASS_DEFAULT);
            //Bass.BASS_ChannelPlay(hStream, false);
            var l = new EncoderLAME(hStream);
            l.InputFile = null; //STDIN (recording)
            l.OutputFile = pTarget;
            l.LAME_Bitrate = (int) _BitRate;
            l.LAME_Mode = EncoderLAME.LAMEMode.Stereo;
            l.LAME_TargetSampleRate = (int) _SampleRate;
            l.LAME_Quality = EncoderLAME.LAMEQuality.Quality;

            BaseEncoder.EncodeFile(pSource, pTarget, l, EncodeFileProc, true, false);
            InvokeFinishReencode(new ReencodeEventArgs(pSource, pTarget));
            return true;
        }

        public ReencodeResults EncodeFolder(String SourceFolder, String TargetFolder, String[] ValidExtensions = null)
        {
            int Count = 0;
            long SourceBytes = 0;
            long TargetBytes = 0;
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

                    String TargetFile = Path.Combine(TargetFolder, sFilePart + ".mp3");
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
                ReencodeResults EncodeResults = EncodeFolder(subdir.FullName, TargetName, ValidExtensions);
                Count += EncodeResults.FileCount;
                SourceBytes += EncodeResults.OriginalBytes;
                TargetBytes += EncodeResults.TargetBytes;
            }
            return new ReencodeResults(Count, SourceBytes, TargetBytes);
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