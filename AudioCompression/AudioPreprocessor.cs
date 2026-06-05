using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Flac;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AudioCompression
{
    public class ProcessedAudio
    {
        public float[] Samples { get; set; }
        public int SampleRate { get; set; }
        public int BitDepth { get; set; }
        public int Channels { get; set; }
    }

    public class AudioPreprocessor
    {
        private readonly CompressionSettings _settings;
        private readonly Random _rng = new Random();

        public AudioPreprocessor(CompressionSettings settings)
        {
            _settings = settings;
        }

        public ProcessedAudio Process(string filePath)
        {
            List<float> samples = new List<float>();

            int finalSampleRate;
            int finalChannels;
            int finalBitDepth = GetSourceBitDepth(filePath);

            using (var reader = new AudioFileReader(filePath))
            {
                ISampleProvider provider = reader;

                finalSampleRate = reader.WaveFormat.SampleRate;
                finalChannels = reader.WaveFormat.Channels;

                // Sample Rate
                if (_settings.SampleRate.HasValue &&
                    _settings.SampleRate.Value != reader.WaveFormat.SampleRate)
                {
                    provider = new WdlResamplingSampleProvider(provider, _settings.SampleRate.Value);
                    finalSampleRate = _settings.SampleRate.Value;
                }
                
                // Channel Conversion
                int sourceChannels = provider.WaveFormat.Channels;
                int targetChannels = _settings.Channels ?? sourceChannels;

                if (sourceChannels != targetChannels)
                {
                    if (sourceChannels == 2 && targetChannels == 1)
                    {
                        provider = new StereoToMonoSampleProvider(provider);
                    }
                    else if (sourceChannels == 1 && targetChannels == 2)
                    {
                        provider = new MonoToStereoSampleProvider(provider)
                        {
                            LeftVolume = 1.0f,
                            RightVolume = 1.0f
                        };
                    }
                    else
                    {
                        throw new NotSupportedException(
                            $"Channel conversion {sourceChannels} -> {targetChannels} is not supported.");
                    }
                }

                finalChannels = targetChannels;
                float[] buffer = new float[8192];
                int samplesRead;
                while ((samplesRead = provider.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < samplesRead; i++)
                        samples.Add(buffer[i]);
                }
            }

            float[] result = samples.ToArray();

            // Bit Depth Quantization
            if (_settings.BitDepth.HasValue && _settings.BitDepth.Value != finalBitDepth)
            {
                result = QuantizeBitDepth(result, _settings.BitDepth.Value);
                finalBitDepth = _settings.BitDepth.Value;
            }

            return new ProcessedAudio
            {
                Samples = result,
                SampleRate = finalSampleRate,
                BitDepth = finalBitDepth,
                Channels = finalChannels
            };
        }

        private static int GetSourceBitDepth(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();

            switch (ext)
            {
                case ".wav":
                    using (var inspector = new WaveFileReader(filePath))
                        return inspector.WaveFormat.BitsPerSample;

                case ".aif":
                case ".aiff":
                    using (var inspector = new AiffFileReader(filePath))
                        return inspector.WaveFormat.BitsPerSample;

                /*case ".flac":
                    using (var inspector = new FlacReader(filePath))
                        return inspector.WaveFormat.BitsPerSample;*/

                case ".mp3":
                case ".aac":
                case ".m4a":
                case ".wma":
                case ".ogg":
                    return 16;

                default:
                    using (var reader = new AudioFileReader(filePath))
                        return reader.WaveFormat.BitsPerSample;
            }
        }

        private float[] QuantizeBitDepth(float[] samples, int bitDepth)
        {
            int levels = 1 << bitDepth;
            float[] output = new float[samples.Length];

            for (int i = 0; i < samples.Length; i++)
            {
                float dither = (float)(_rng.NextDouble() - _rng.NextDouble()) / levels;

                float normalized = (samples[i] + 1f) * 0.5f;
                normalized = Math.Max(0f, Math.Min(1f, normalized + dither));

                int quantized = (int)Math.Round(normalized * (levels - 1));
                float restored = (float)quantized / (levels - 1);

                output[i] = restored * 2f - 1f;
            }

            return output;
        }
    }
}