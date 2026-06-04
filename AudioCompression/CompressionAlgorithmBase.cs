using NAudio.Wave;
using System;
using System.Threading;

namespace AudioCompression
{
    public abstract class CompressionAlgorithmBase
    {
        protected readonly CompressionSettings _settings;
        
        protected CancellationToken CancellationToken { get; private set; }

        public event Action<CompressionProgress> ProgressChanged;

        protected CompressionAlgorithmBase(CompressionSettings settings)
        {
            _settings = settings;
        }

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
    }
}