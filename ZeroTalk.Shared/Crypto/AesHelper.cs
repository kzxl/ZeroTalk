using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ZeroTalk.Shared.Crypto
{
    /// <summary>
    /// Helper mã hoá / giải mã AES-256-CBC.
    /// Sử dụng System.Security.Cryptography built-in, không cần NuGet.
    /// </summary>
    public class AesHelper : ICryptoService
    {
        /// <summary>
        /// Mã hoá text bằng AES-256-CBC.
        /// Output: Base64(IV + CipherText)
        /// </summary>
        public string Encrypt(string plainText, byte[] key)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            var encrypted = EncryptBytes(Encoding.UTF8.GetBytes(plainText), key);
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Giải mã text từ Base64(IV + CipherText)
        /// </summary>
        public string Decrypt(string cipherText, byte[] key)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            var cipherBytes = Convert.FromBase64String(cipherText);
            var decrypted = DecryptBytes(cipherBytes, key);
            return Encoding.UTF8.GetString(decrypted);
        }

        /// <summary>
        /// Mã hoá byte array. Output = IV (16 bytes) + CipherData
        /// </summary>
        public byte[] EncryptBytes(byte[] data, byte[] key)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = EnsureKeySize(key, 32); // 256 bits
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    // Ghi IV vào đầu stream
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }

                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Giải mã byte array. Input = IV (16 bytes) + CipherData
        /// </summary>
        public byte[] DecryptBytes(byte[] data, byte[] key)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = EnsureKeySize(key, 32);

                // Tách IV từ 16 bytes đầu
                var iv = new byte[16];
                Array.Copy(data, 0, iv, 0, 16);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(data, 16, data.Length - 16))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var output = new MemoryStream())
                {
                    cs.CopyTo(output);
                    return output.ToArray();
                }
            }
        }

        /// <summary>
        /// Đảm bảo key đúng kích thước. Nếu ngắn hơn thì pad 0, dài hơn thì cắt.
        /// Thực tế key từ DH sẽ là 32 bytes (SHA256 hash).
        /// </summary>
        private static byte[] EnsureKeySize(byte[] key, int size)
        {
            if (key.Length == size)
                return key;

            var result = new byte[size];
            Array.Copy(key, result, Math.Min(key.Length, size));
            return result;
        }

        /// <summary>
        /// Tạo random AES key (256-bit) cho testing
        /// </summary>
        public static byte[] GenerateRandomKey()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                return aes.Key;
            }
        }
    }
}
