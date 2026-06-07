using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioCompression
{
    public enum CompressionAlgorithm
    {
        NonlinearQuantization,
        DPCM,
        PredictiveDifferentialCoding,
        DeltaModulation,
        AdaptiveDeltaModulation
    }

    // for Nonlinear Quantization
    /*public enum CompandingLaw
    {
        MuLaw,
        ALaw
    }*/

    public class CompressionSettings
    {
        // إعدادات عامة لكل الخوارزميات
        public int? SampleRate { get; set; } = null;
        public int? BitDepth { get; set; } = null;
        public int? Channels { get; set; } = null;
        public CompressionAlgorithm Algorithm { get; set; } = CompressionAlgorithm.DeltaModulation;
        public int? TargetBitRate { get; set; } = null;

        // Nonlinear Quantization
        // public CompandingLaw CompandingLaw { get; set; } = CompandingLaw.MuLaw;

        // DPCM
        public int? QuantizationLevels { get; set; } = 16;


        // ADPCM
        //public double AdaptationSpeed { get; set; } = 0.5;

        // Delta Modulation
        public float FixedStepSize { get; set; } = 0.01f;

        //Adaptive Delta Modulation (CVSD)
        public int BitHistoryLength { get; set; } = 3;
        public float MinStepSize { get; set; } = 0.001f;
        public float MaxStepSize { get; set; } = 0.5f;
        public float StepMultiplier { get; set; } = 1.5f;
        public float StepDecay { get; set; } = 0.5f;
    }
}
