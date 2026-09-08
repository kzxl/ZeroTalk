using System;
using ChatBox.Shared.Crypto;

namespace ChatBox.Tests
{
    public static class CryptoTests
    {
        public static void RunAll()
        {
            TestAesEncryptionDecryption();
            TestDiffieHellmanKeyExchange();
        }

        private static void TestAesEncryptionDecryption()
        {
            var aes = new AesHelper();
            byte[] key = AesHelper.GenerateRandomKey();

            string secretText = "Top secret enterprise message: mật khẩu bí mật 123! 🔑";

            string cipherText = aes.Encrypt(secretText, key);
            Assert.NotNull(cipherText);
            Assert.False(secretText == cipherText, "Cipher text must differ from plain text");

            string decryptedText = aes.Decrypt(cipherText, key);
            Assert.Equal(secretText, decryptedText, "Decrypted text must match original plain text");
        }

        private static void TestDiffieHellmanKeyExchange()
        {
            using (var aliceDh = new DiffieHellmanHelper())
            using (var bobDh = new DiffieHellmanHelper())
            {
                string alicePublicKey = aliceDh.GetPublicKey();
                string bobPublicKey = bobDh.GetPublicKey();

                Assert.NotNull(alicePublicKey);
                Assert.NotNull(bobPublicKey);

                byte[] aliceShared = aliceDh.DeriveSharedSecret(bobPublicKey);
                byte[] bobShared = bobDh.DeriveSharedSecret(alicePublicKey);

                Assert.NotNull(aliceShared);
                Assert.NotNull(bobShared);
                Assert.Equal(aliceShared.Length, bobShared.Length);

                string aliceSharedB64 = Convert.ToBase64String(aliceShared);
                string bobSharedB64 = Convert.ToBase64String(bobShared);

                Assert.Equal(aliceSharedB64, bobSharedB64, "Both peers must compute the exact same ECDH shared key!");
            }
        }
    }
}
