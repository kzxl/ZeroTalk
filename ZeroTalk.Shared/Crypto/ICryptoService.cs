namespace ZeroTalk.Shared.Crypto
{
    /// <summary>
    /// Interface cho dịch vụ mã hoá
    /// </summary>
    public interface ICryptoService
    {
        /// <summary>Mã hoá text bằng AES với key cho trước</summary>
        string Encrypt(string plainText, byte[] key);

        /// <summary>Giải mã text bằng AES với key cho trước</summary>
        string Decrypt(string cipherText, byte[] key);

        /// <summary>Mã hoá byte array bằng AES</summary>
        byte[] EncryptBytes(byte[] data, byte[] key);

        /// <summary>Giải mã byte array bằng AES</summary>
        byte[] DecryptBytes(byte[] data, byte[] key);
    }
}
