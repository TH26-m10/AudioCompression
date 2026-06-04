using System;

namespace AudioCompression
{
    public static class CompressionFactory
    {
        public static CompressionAlgorithmBase Create(CompressionSettings settings)
        {
            switch (settings.Algorithm)
            {
                case CompressionAlgorithm.DeltaModulation:
                    return new DeltaModulation(settings);

                case CompressionAlgorithm.DPCM:
                    throw new NotImplementedException("DPCM not implemented yet.");

                case CompressionAlgorithm.AdaptiveDeltaModulation:
                    throw new NotImplementedException("Adaptive Delta Modulation not implemented yet.");

                case CompressionAlgorithm.PredictiveDifferentialCoding:
                    throw new NotImplementedException("Predictive Differential Coding not implemented yet.");

                case CompressionAlgorithm.NonlinearQuantization:
                    throw new NotImplementedException("Nonlinear Quantization not implemented yet.");

                default:
                    throw new NotSupportedException($"Unknown algorithm: {settings.Algorithm}");
            }
        }
    }
}
