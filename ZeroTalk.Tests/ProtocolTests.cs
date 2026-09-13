using System.Collections.Generic;
using ZeroTalk.Shared.Protocol;

namespace ZeroTalk.Tests
{
    public static class ProtocolTests
    {
        public static void RunAll()
        {
            TestPacketFraming();
            TestJsonSerializationSpecialCharacters();
            TestPacketHeaderParsing();
        }

        private static void TestPacketFraming()
        {
            var packet = new Packet(PacketType.Message, "alice", "bob", "Hello world! 🚀");
            string json = PacketSerializer.Serialize(packet);

            Assert.NotNull(json);
            Assert.True(json.Length > 0);

            var deserialized = PacketSerializer.Deserialize(json);
            Assert.NotNull(deserialized);
            Assert.Equal(PacketType.Message, deserialized.Type);
            Assert.Equal("alice", deserialized.SenderId);
            Assert.Equal("bob", deserialized.ReceiverId);
            Assert.Equal("Hello world! 🚀", deserialized.Data);
        }

        private static void TestJsonSerializationSpecialCharacters()
        {
            var dict = new Dictionary<string, object>
            {
                { "Quote", "Text with \"quotes\" and 'single' and \\backslashes\\" },
                { "NewLine", "Line1\r\nLine2\nLine3" },
                { "Unicode", "Tiếng Việt có dấu: Xin chào Việt Nam! 🇻🇳" }
            };

            string json = PacketSerializer.ToJson(dict);
            Assert.NotNull(json);

            var restored = PacketSerializer.FromJson<Dictionary<string, object>>(json);
            Assert.NotNull(restored);
            Assert.Equal("Text with \"quotes\" and 'single' and \\backslashes\\", restored["Quote"].ToString());
            Assert.Equal("Line1\r\nLine2\nLine3", restored["NewLine"].ToString());
            Assert.Equal("Tiếng Việt có dấu: Xin chào Việt Nam! 🇻🇳", restored["Unicode"].ToString());
        }

        private static void TestPacketHeaderParsing()
        {
            var packet = new Packet(PacketType.Login, "testuser", null, "{\"Username\":\"test\"}");
            string json = PacketSerializer.Serialize(packet);

            var parsed = PacketSerializer.Deserialize(json);
            Assert.Equal(PacketType.Login, parsed.Type);
            Assert.Equal("testuser", parsed.SenderId);
            Assert.Equal("{\"Username\":\"test\"}", parsed.Data);
        }
    }
}
