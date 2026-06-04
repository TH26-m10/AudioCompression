namespace AudioCompression
{
    public class CompressionProgress
    {
        // 0.0 -> 1.0
        public float Percentage { get; set; }

        // samples processed per second
        public float ProcessingSpeed { get; set; }

        // bits out so far / bits in so far
        public float CompressionRatio { get; set; }

        // elapsed milliseconds
        public long ElapsedMs { get; set; }
    }
}