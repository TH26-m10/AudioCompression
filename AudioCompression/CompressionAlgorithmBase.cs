using NAudio.Lame;
using NAudio.Flac;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading;
using NAudio.MediaFoundation;
using System.Windows.Forms;

namespace AudioCompression
{
    public abstract class CompressionAlgorithmBase
    {
        protected readonly CompressionSettings _settings;

        protected CancellationToken CancellationToken { get; private set; }

        // عملنا ايفينت اي كلاس بيقدر يشترك في 
        public event Action<CompressionProgress> ProgressChanged;

        protected CompressionAlgorithmBase(CompressionSettings settings)
        {
            _settings = settings;
        }

        // هاد التابع اللي بيستدعو الكلاسات ليتم تحديث الواجهة
        protected void ReportProgress(CompressionProgress progress)
        {
            ProgressChanged?.Invoke(progress);
        }

        public void SetCancellationToken(CancellationToken token)
        {
            CancellationToken = token;
        }

        public abstract string Compress(string inputFile);

        public abstract string Decompress(string compressedFile);

        protected void WriteWavFile(string path, float[] samples, int sampleRate, int channels, int bitDepth)
        {
            WaveFormat format;

            switch (bitDepth)
            {
                case 8:
                    format = new WaveFormat(sampleRate, 8, channels);
                    break;
                case 16:
                case 24:
                    format = new WaveFormat(sampleRate, bitDepth, channels);
                    break;
                case 32:
                    format = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
                    break;
                default:
                    format = new WaveFormat(sampleRate, 16, channels);
                    break;
            }

            using (WaveFileWriter writer = new WaveFileWriter(path, format))
            {
                if (bitDepth == 8)
                {
                    byte[] pcm8 = new byte[samples.Length];
                    for (int i = 0; i < samples.Length; i++)
                    {
                        float clamped = Math.Max(-1f, Math.Min(1f, samples[i]));
                        pcm8[i] = (byte)((clamped * 127.5f) + 128f);
                    }
                    writer.Write(pcm8, 0, pcm8.Length);
                }
                else
                {
                    writer.WriteSamples(samples, 0, samples.Length);
                }
            }
        }

        public static string ConvertFromWav(string wavFile, string targetExtension, int bitDepth = 16, int bitRateKbps = 128)
        {
            string outputFile = Path.ChangeExtension(wavFile, targetExtension);

            switch (targetExtension.ToLowerInvariant())
            {
                case ".wav":
                    return wavFile;

                case ".mp3":
                case ".mpa":
                    ConvertWavToMp3(wavFile, outputFile);
                    break;

                case ".flac":
                    ConvertWavToFlac(wavFile, outputFile);
                    break;

                case ".aif":
                case ".aiff":
                    ConvertWavToAiff(wavFile, outputFile, bitDepth);
                    break;

                case ".wma":
                    ConvertWavToWma(wavFile, outputFile, bitRateKbps);
                    break;

                case ".aac":
                case ".m4a":
                    ConvertWavToAac(wavFile, outputFile, bitRateKbps);
                    break;

                default:
                    return FallbackToWav(wavFile, targetExtension);
            }

            return outputFile;
        }

        private static void ConvertWavToMp3(string wavFile, string outputFile)
        {
            // Requires NuGet: NAudio.Lame
            using (var reader = new AudioFileReader(wavFile))
            using (var writer = new LameMP3FileWriter(outputFile, reader.WaveFormat, LAMEPreset.STANDARD))
                reader.CopyTo(writer);
        }

        private static void ConvertWavToAiff(string wavFile, string outputFile, int targetBitDepth = 16)
        {
            using (var reader = new AudioFileReader(wavFile))
            {
                // Create a WaveFormat that matches the target bit depth
                WaveFormat targetFormat;

                /*if (targetBitDepth == 24)
                {
                    targetFormat = new WaveFormat(reader.WaveFormat.SampleRate, 24, reader.WaveFormat.Channels);
                }
                else
                {
                    targetFormat = new WaveFormat(reader.WaveFormat.SampleRate, 16, reader.WaveFormat.Channels);
                }*/

                targetFormat = new WaveFormat(reader.WaveFormat.SampleRate, targetBitDepth, reader.WaveFormat.Channels);
                // Read all samples into memory
                float[] allSamples = new float[(int)reader.Length];
                int samplesRead = reader.Read(allSamples, 0, allSamples.Length);

                // Write directly to AIFF without conversion stream
                using (var writer = new AiffFileWriter(outputFile, targetFormat))
                {
                    writer.WriteSamples(allSamples, 0, samplesRead);
                }
            }
        }

        private static void ConvertWavToWma(string wavFile, string outputFile, int bitRateKbps = 128)
        {
            FFmpegConverter.Convert(wavFile, outputFile, $"-c:a wmav2 -b:a {bitRateKbps}k");
        }

        private static void ConvertWavToFlac(string wavFile, string outputFile)
        {
            FFmpegConverter.Convert(wavFile, outputFile);
        }

        private static void ConvertWavToAac(string wavFile, string outputFile, int bitRateKbps = 128)
        {
            FFmpegConverter.Convert(wavFile, outputFile, $"-c:a aac -b:a {bitRateKbps}k");
        }

        private static string FallbackToWav(string wavFile, string targetExtension)
        {
            MessageBox.Show($"Warning: {targetExtension} encoding not supported. Returning WAV.");
            return wavFile;
        }
    }
}