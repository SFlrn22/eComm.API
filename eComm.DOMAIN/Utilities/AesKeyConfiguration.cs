namespace eComm.DOMAIN.Utilities
{
    public static class AesKeyConfiguration
    {
        public static byte[] Key { get; } = [100, 22, 138, 148, 201, 233, 150, 156, 141, 117, 94, 39, 206, 151, 122, 137];
        public static byte[] IV { get; } = [7, 127, 165, 237, 21, 147, 252, 189, 184, 255, 192, 216, 54, 5, 110, 200];
    }
}
