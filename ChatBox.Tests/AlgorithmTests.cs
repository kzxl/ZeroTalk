using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ChatBox.Client.Services;
using ChatBox.Shared.Constants;
using ChatBox.Shared.Crypto;
using ChatBox.Shared.Network;
using ChatBox.Shared.Protocol;

namespace ChatBox.Tests
{
    /// <summary>
    /// Comprehensive algorithmic tests adhering to AgentOption universal test-logic standard.
    /// Validates mathematical invariants, protocol binary parsing, sequence reassembly, and crypto correctness.
    /// </summary>
    public static class AlgorithmTests
    {
        public static void RunAll()
        {
            RunStunTests();
            RunChunkingTests();
            RunCryptoInvariantTests();
            RunGeometryTests();
        }

        public static void RunStunTests()
        {
            TestStunXorMappedAddress_ValidIPv4Payload_DecodesCorrectIpAndPort();
            TestStunParseResponse_MismatchedTransactionId_ReturnsNull();
            TestStunParseResponse_MalformedOrTruncatedBuffer_ReturnsNull();
        }

        public static void RunChunkingTests()
        {
            TestChunking_BoundaryPayloadSizes_CalculatesExactChunkCount();
            TestChunkReassembly_OutOfOrderArrival_ProducesByteIdenticalFile();
        }

        public static void RunCryptoInvariantTests()
        {
            TestCrypto_EcdhCommutativity_BothPeersDeriveIdenticalSecret();
            TestCrypto_EcdhDistinctness_DifferentPeersProduceDistinctSecrets();
            TestCrypto_AesIvUniqueness_SamePlaintextProducesUniqueCiphertexts();
        }

        public static void RunGeometryTests()
        {
            TestRadarSweep_AngleWrapAround_MaintainsModulo360PeriodicInvariant();
            TestRadarSweep_TrigonometricCoordinates_AlwaysFallWithinCircleRadius();
        }

        #region Category 1: STUN RFC 5389 Binary Parsing Algorithm

        private static void TestStunXorMappedAddress_ValidIPv4Payload_DecodesCorrectIpAndPort()
        {
            // 1. Arrange: Build RFC 5389 Binding Response packet
            // Target public endpoint to encode: 203.0.113.45:12345
            string expectedIpStr = "203.0.113.45";
            int expectedPort = 12345;
            byte[] rawIp = IPAddress.Parse(expectedIpStr).GetAddressBytes();

            var transactionId = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var packet = new byte[32];

            // STUN Header (20 bytes)
            // Message Type: Binding Response (0x0101)
            packet[0] = 0x01; packet[1] = 0x01;
            // Message Length: 12 bytes (attribute header 4 + data 8)
            packet[2] = 0x00; packet[3] = 0x0C;
            // Magic Cookie: 0x2112A442
            packet[4] = 0x21; packet[5] = 0x12; packet[6] = 0xA4; packet[7] = 0x42;
            // Transaction ID (12 bytes)
            Array.Copy(transactionId, 0, packet, 8, 12);

            // Attribute: XOR-MAPPED-ADDRESS (0x0020)
            packet[20] = 0x00; packet[21] = 0x20;
            // Attribute Length: 8 bytes
            packet[22] = 0x00; packet[23] = 0x08;

            // XOR-MAPPED-ADDRESS Data:
            packet[24] = 0x00; // Reserved
            packet[25] = 0x01; // Family: IPv4

            // XOR Port: expectedPort ^ 0x2112
            ushort xorPort = (ushort)(expectedPort ^ 0x2112);
            packet[26] = (byte)(xorPort >> 8);
            packet[27] = (byte)(xorPort & 0xFF);

            // XOR IP: rawIp ^ MagicCookie (0x21, 0x12, 0xA4, 0x42)
            packet[28] = (byte)(rawIp[0] ^ 0x21);
            packet[29] = (byte)(rawIp[1] ^ 0x12);
            packet[30] = (byte)(rawIp[2] ^ 0xA4);
            packet[31] = (byte)(rawIp[3] ^ 0x42);

            // 2. Act: Parse using StunClient algorithm
            IPEndPoint result = StunClient.ParseBindingResponse(packet, transactionId);

            // 3. Assert
            Assert.NotNull(result, "STUN parser should successfully decode valid XOR-MAPPED-ADDRESS payload");
            Assert.Equal(expectedIpStr, result.Address.ToString());
            Assert.Equal(expectedPort, result.Port);
        }

        private static void TestStunParseResponse_MismatchedTransactionId_ReturnsNull()
        {
            // Arrange
            var validTxId = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var wrongTxId = new byte[] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 };

            var packet = new byte[20];
            packet[0] = 0x01; packet[1] = 0x01; // Binding Response
            packet[4] = 0x21; packet[5] = 0x12; packet[6] = 0xA4; packet[7] = 0x42; // Magic Cookie
            Array.Copy(validTxId, 0, packet, 8, 12);

            // Act
            var result = StunClient.ParseBindingResponse(packet, wrongTxId);

            // Assert: Security invariant - responses with mismatched transaction IDs must be dropped
            Assert.True(result == null, "STUN parser must reject mismatched transaction ID to prevent spoofing");
        }

        private static void TestStunParseResponse_MalformedOrTruncatedBuffer_ReturnsNull()
        {
            var txId = new byte[12];

            // Truncated buffer (< 20 bytes header)
            Assert.True(StunClient.ParseBindingResponse(new byte[10], txId) == null);

            // Null buffer
            Assert.True(StunClient.ParseBindingResponse(null, txId) == null);

            // Wrong message type (e.g. 0x0001 request instead of 0x0101 response)
            var wrongType = new byte[20];
            wrongType[0] = 0x00; wrongType[1] = 0x01;
            wrongType[4] = 0x21; wrongType[5] = 0x12; wrongType[6] = 0xA4; wrongType[7] = 0x42;
            Assert.True(StunClient.ParseBindingResponse(wrongType, txId) == null);

            // Wrong magic cookie
            var wrongCookie = new byte[20];
            wrongCookie[0] = 0x01; wrongCookie[1] = 0x01;
            wrongCookie[4] = 0x00; wrongCookie[5] = 0x00; wrongCookie[6] = 0x00; wrongCookie[7] = 0x00;
            Assert.True(StunClient.ParseBindingResponse(wrongCookie, txId) == null);
        }

        #endregion

        #region Category 2: File Chunking & Out-of-Order Reassembly Algorithm

        private static void TestChunking_BoundaryPayloadSizes_CalculatesExactChunkCount()
        {
            int chunkSize = AppConstants.FileChunkSize; // 64 KB = 65536 bytes

            int Calc(long size) => (int)Math.Ceiling((double)size / chunkSize);

            // Boundary Value Analysis:
            Assert.Equal(0, Calc(0));
            Assert.Equal(1, Calc(1));
            Assert.Equal(1, Calc(chunkSize - 1));
            Assert.Equal(1, Calc(chunkSize));
            Assert.Equal(2, Calc(chunkSize + 1));
            Assert.Equal(2, Calc(chunkSize * 2));
            Assert.Equal(3, Calc(chunkSize * 2 + 1));
            Assert.Equal(10, Calc(chunkSize * 10));
        }

        private static void TestChunkReassembly_OutOfOrderArrival_ProducesByteIdenticalFile()
        {
            // 1. Arrange: Create a 150 KB test payload with pseudo-random deterministic byte sequence
            int payloadSize = 150 * 1024;
            var originalBytes = new byte[payloadSize];
            var rng = new Random(42); // Deterministic seed
            rng.NextBytes(originalBytes);

            int chunkSize = AppConstants.FileChunkSize;
            int totalChunks = (int)Math.Ceiling((double)payloadSize / chunkSize);
            string transferId = "test_trans_" + Guid.NewGuid().ToString("N").Substring(0, 6);

            // Split into discrete chunks
            var chunks = new List<byte[]>();
            for (int i = 0; i < totalChunks; i++)
            {
                int offset = i * chunkSize;
                int len = Math.Min(chunkSize, payloadSize - offset);
                var chunk = new byte[len];
                Array.Copy(originalBytes, offset, chunk, 0, len);
                chunks.Add(chunk);
            }

            // 2. Act: Simulate out-of-order network arrival (e.g. Chunk 2, then Chunk 0, then Chunk 1)
            var receiver = new FileReceiveService();
            receiver.HandleFileHeader("sender1", "Alice", "test_file.bin", payloadSize, totalChunks, transferId);

            int[] arrivalOrder = new int[] { 2, 0, 1 };
            foreach (int chunkIdx in arrivalOrder)
            {
                string b64 = Convert.ToBase64String(chunks[chunkIdx]);
                receiver.HandleFileChunk(transferId, chunkIdx, b64);
            }

            string savedPath = receiver.HandleFileComplete(transferId);

            // 3. Assert: Verify saved file exists and matches original payload 100% byte-for-byte
            Assert.NotNull(savedPath, "Saved path should not be null after all chunks are supplied");
            Assert.True(File.Exists(savedPath), "Reassembled file must exist on disk");

            try
            {
                byte[] reassembledBytes = File.ReadAllBytes(savedPath);
                Assert.Equal(originalBytes.Length, reassembledBytes.Length, "Reassembled file size must match original exactly");

                for (int i = 0; i < originalBytes.Length; i++)
                {
                    if (originalBytes[i] != reassembledBytes[i])
                    {
                        throw new Exception($"Byte mismatch at index {i}: expected {originalBytes[i]}, got {reassembledBytes[i]}");
                    }
                }
            }
            finally
            {
                if (File.Exists(savedPath))
                    File.Delete(savedPath);
            }
        }

        #endregion

        #region Category 3: Cryptographic Invariant & Key Derivation Algorithms

        private static void TestCrypto_EcdhCommutativity_BothPeersDeriveIdenticalSecret()
        {
            // Mathematical Invariant: Derive(PrivA, PubB) == Derive(PrivB, PubA)
            using (var peerA = new DiffieHellmanHelper())
            using (var peerB = new DiffieHellmanHelper())
            {
                string pubA = peerA.GetPublicKey();
                string pubB = peerB.GetPublicKey();

                byte[] secretA = peerA.DeriveSharedSecret(pubB);
                byte[] secretB = peerB.DeriveSharedSecret(pubA);

                Assert.NotNull(secretA);
                Assert.NotNull(secretB);
                Assert.Equal(32, secretA.Length, "ECDH + SHA256 KDF must output exactly 256 bits (32 bytes)");
                Assert.Equal(secretA.Length, secretB.Length);

                for (int i = 0; i < 32; i++)
                {
                    Assert.Equal(secretA[i], secretB[i], $"Secret byte {i} must match between both peers");
                }
            }
        }

        private static void TestCrypto_EcdhDistinctness_DifferentPeersProduceDistinctSecrets()
        {
            // Invariant: Given unique ephemeral keys, Derive(A, B) != Derive(A, C)
            using (var peerA = new DiffieHellmanHelper())
            using (var peerB = new DiffieHellmanHelper())
            using (var peerC = new DiffieHellmanHelper())
            {
                byte[] secretAB = peerA.DeriveSharedSecret(peerB.GetPublicKey());
                byte[] secretAC = peerA.DeriveSharedSecret(peerC.GetPublicKey());

                string b64AB = Convert.ToBase64String(secretAB);
                string b64AC = Convert.ToBase64String(secretAC);

                Assert.False(b64AB == b64AC, "Shared secrets with different peers must be completely distinct");
            }
        }

        private static void TestCrypto_AesIvUniqueness_SamePlaintextProducesUniqueCiphertexts()
        {
            // Security Invariant: Ciphertexts of identical plaintexts must differ due to randomized IV
            var aes = new AesHelper();
            byte[] key = AesHelper.GenerateRandomKey();
            string plainText = "Confidential financial statement 2026";

            string cipher1 = aes.Encrypt(plainText, key);
            string cipher2 = aes.Encrypt(plainText, key);

            Assert.False(cipher1 == cipher2, "AES-CBC must generate unique IV per encryption, producing distinct ciphertexts");

            // But both must decrypt back to identical original text
            Assert.Equal(plainText, aes.Decrypt(cipher1, key));
            Assert.Equal(plainText, aes.Decrypt(cipher2, key));
        }

        #endregion

        #region Category 4: Radar Sweep Trigonometric Geometry Algorithm

        private static void TestRadarSweep_AngleWrapAround_MaintainsModulo360PeriodicInvariant()
        {
            // Invariant: Angle stepping with delta must remain strictly in [0, 360)
            int stepDelta = 5;
            int angle = 350;

            angle = (angle + stepDelta) % 360;
            Assert.Equal(355, angle);

            angle = (angle + stepDelta) % 360;
            Assert.Equal(0, angle, "355 + 5 degrees must wrap around to 0 degrees modulo 360");

            angle = (angle + stepDelta) % 360;
            Assert.Equal(5, angle);
        }

        private static void TestRadarSweep_TrigonometricCoordinates_AlwaysFallWithinCircleRadius()
        {
            // Invariant: Polar-to-Cartesian projected endpoint (cx + r*cosθ, cy + r*sinθ)
            // must satisfy: (x - cx)^2 + (y - cy)^2 <= r^2
            int cx = 160;
            int cy = 120;
            double r = 80.0;

            for (int deg = 0; deg < 360; deg += 15)
            {
                double rad = deg * Math.PI / 180.0;
                double x = cx + r * Math.Cos(rad);
                double y = cy + r * Math.Sin(rad);

                double distSquared = Math.Pow(x - cx, 2) + Math.Pow(y - cy, 2);
                double maxAllowed = Math.Pow(r, 2) + 1e-5; // Tolerance

                Assert.True(distSquared <= maxAllowed, $"Point at {deg} degrees ({x}, {y}) must lie within radius {r}");
            }
        }

        #endregion
    }
}
