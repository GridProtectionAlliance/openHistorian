namespace System.Security.Cryptography
{
    public interface IHashAlgorithm
    {
        void ComputeHash(byte[] data, byte[] hash);
        int OutputSize { get; }
        int BlockSize { get; }
    }
}