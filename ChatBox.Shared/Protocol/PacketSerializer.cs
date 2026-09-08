using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;

namespace ChatBox.Shared.Protocol
{
    /// <summary>
    /// Serialize/Deserialize Packet qua NetworkStream.
    /// Format: [4 bytes - payload length (BigEndian)][N bytes - JSON payload]
    /// </summary>
    public static class PacketSerializer
    {
        private static readonly JavaScriptSerializer _serializer = new JavaScriptSerializer
        {
            MaxJsonLength = int.MaxValue
        };

        /// <summary>
        /// Serialize packet thành JSON string
        /// </summary>
        public static string Serialize(Packet packet)
        {
            if (packet == null) return null;
            return _serializer.Serialize(packet);
        }

        /// <summary>
        /// Deserialize JSON string thành Packet
        /// </summary>
        public static Packet Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                return _serializer.Deserialize<Packet>(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Helper serialize đối tượng bất kỳ thành JSON string
        /// </summary>
        public static string ToJson<T>(T obj)
        {
            if (obj == null) return null;
            return _serializer.Serialize(obj);
        }

        /// <summary>
        /// Helper deserialize JSON string thành đối tượng kiểu T
        /// </summary>
        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default(T);
            try
            {
                return _serializer.Deserialize<T>(json);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Gửi packet qua NetworkStream (length-prefixed)
        /// </summary>
        public static void SendPacket(NetworkStream stream, Packet packet)
        {
            var json = Serialize(packet);
            var payload = Encoding.UTF8.GetBytes(json);
            var lengthBytes = BitConverter.GetBytes(payload.Length);

            // Ensure BigEndian
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBytes);

            stream.Write(lengthBytes, 0, 4);
            stream.Write(payload, 0, payload.Length);
            stream.Flush();
        }

        /// <summary>
        /// Đọc packet từ NetworkStream (length-prefixed)
        /// </summary>
        public static Packet ReceivePacket(NetworkStream stream)
        {
            // Read length prefix (4 bytes)
            var lengthBytes = ReadExact(stream, 4);
            if (lengthBytes == null)
                return null;

            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBytes);

            int length = BitConverter.ToInt32(lengthBytes, 0);

            if (length <= 0 || length > Constants.AppConstants.MaxPacketSize)
                return null;

            // Read payload
            var payloadBytes = ReadExact(stream, length);
            if (payloadBytes == null)
                return null;

            var json = Encoding.UTF8.GetString(payloadBytes);
            return Deserialize(json);
        }

        /// <summary>
        /// Đọc đúng N bytes từ stream
        /// </summary>
        private static byte[] ReadExact(NetworkStream stream, int count)
        {
            var buffer = new byte[count];
            int totalRead = 0;

            while (totalRead < count)
            {
                int bytesRead = stream.Read(buffer, totalRead, count - totalRead);
                if (bytesRead == 0)
                    return null; // Connection closed

                totalRead += bytesRead;
            }

            return buffer;
        }
    }
}
