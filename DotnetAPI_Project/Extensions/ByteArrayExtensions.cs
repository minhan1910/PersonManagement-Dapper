namespace DotnetAPI_Project.Extensions
{
    public static class ByteArrayExtensions
    {

        public static bool ComparedTo(this byte[] a, byte[] b)
        {
            return ByteArraysEqual(a, b);
        }

        private static bool ByteArraysEqual(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }

    }
}
