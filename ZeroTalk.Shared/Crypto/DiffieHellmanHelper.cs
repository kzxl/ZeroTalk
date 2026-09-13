using System;
using System.Security.Cryptography;

namespace ZeroTalk.Shared.Crypto
{
    /// <summary>
    /// Helper thực hiện trao đổi khoá Diffie-Hellman.
    /// Sử dụng ECDiffieHellmanCng (.NET built-in).
    /// 
    /// Flow:
    /// 1. Mỗi bên gọi GenerateKeyPair() → lấy PublicKey
    /// 2. Gửi PublicKey cho bên kia qua server (KeyExchange packet)
    /// 3. Mỗi bên gọi DeriveSharedSecret(otherPublicKey) → cùng 1 SharedSecret
    /// 4. SharedSecret được hash SHA256 → làm AES Key (32 bytes)
    /// </summary>
    public class DiffieHellmanHelper : IDisposable
    {
        private ECDiffieHellmanCng _ecdh;

        public DiffieHellmanHelper()
        {
            _ecdh = new ECDiffieHellmanCng(256);
            _ecdh.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            _ecdh.HashAlgorithm = CngAlgorithm.Sha256;
        }

        /// <summary>
        /// Lấy public key dạng Base64 để gửi cho bên kia
        /// </summary>
        public string GetPublicKey()
        {
            return Convert.ToBase64String(_ecdh.PublicKey.ToByteArray());
        }

        /// <summary>
        /// Tạo shared secret từ public key của đối phương.
        /// Kết quả là 32 bytes (SHA256 hash) → dùng làm AES-256 key.
        /// </summary>
        public byte[] DeriveSharedSecret(string otherPublicKeyBase64)
        {
            var otherKeyBytes = Convert.FromBase64String(otherPublicKeyBase64);
            var otherKey = CngKey.Import(otherKeyBytes, CngKeyBlobFormat.EccPublicBlob);
            return _ecdh.DeriveKeyMaterial(otherKey);
        }

        /// <summary>
        /// Tạo shared secret từ public key bytes
        /// </summary>
        public byte[] DeriveSharedSecret(byte[] otherPublicKeyBytes)
        {
            var otherKey = CngKey.Import(otherPublicKeyBytes, CngKeyBlobFormat.EccPublicBlob);
            return _ecdh.DeriveKeyMaterial(otherKey);
        }

        public void Dispose()
        {
            _ecdh?.Dispose();
        }
    }
}
