using System;

namespace ZeroTalk.Client.Services
{
    public class AudioFrameHeader
    {
        public uint SequenceNumber { get; set; }
        public uint TimestampMs { get; set; }
        public int SampleRate { get; set; } = 48000;
        public int Channels { get; set; } = 1;
        public int PayloadSize { get; set; }
        public bool IsVoiceDetected { get; set; }

        public byte[] Serialize()
        {
            var header = new byte[18];
            Buffer.BlockCopy(BitConverter.GetBytes(SequenceNumber), 0, header, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(TimestampMs), 0, header, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(SampleRate), 0, header, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Channels), 0, header, 12, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(PayloadSize), 0, header, 14, 2);
            header[16] = (byte)(IsVoiceDetected ? 1 : 0);
            return header;
        }

        public static AudioFrameHeader Deserialize(byte[] data, int offset = 0)
        {
            if (data == null || data.Length - offset < 17)
            {
                throw new ArgumentException("Invalid audio frame header buffer size.");
            }

            return new AudioFrameHeader
            {
                SequenceNumber = BitConverter.ToUInt32(data, offset),
                TimestampMs = BitConverter.ToUInt32(data, offset + 4),
                SampleRate = BitConverter.ToInt32(data, offset + 8),
                Channels = BitConverter.ToInt16(data, offset + 12),
                PayloadSize = BitConverter.ToInt16(data, offset + 14),
                IsVoiceDetected = data[offset + 16] == 1
            };
        }
    }

    public class VoiceCallSessionService
    {
        private uint _nextSeq = 1;
        private uint _expectedIncomingSeq = 1;
        private readonly DateTime _startTime = DateTime.UtcNow;

        public bool IsMuted { get; set; }
        public bool IsDeafened { get; set; }
        public float VadThreshold { get; set; } = 0.015f; // RMS amplitude silence suppression

        public long PacketsSent { get; private set; }
        public long PacketsReceived { get; private set; }
        public long PacketsLost { get; private set; }
        public long SilenceFramesSuppressed { get; private set; }

        public event Action<byte[]> OnOutgoingAudioPacketReady;
        public event Action<byte[], AudioFrameHeader> OnIncomingAudioFrame;

        /// <summary>
        /// Processes raw or encoded audio buffer from microphone.
        /// Applies VAD to suppress silence, applies Mute, builds packet and triggers outgoing send.
        /// </summary>
        public bool ProcessOutgoingAudio(byte[] audioData, int sampleRate = 48000, int channels = 1)
        {
            if (audioData == null || audioData.Length == 0 || IsMuted)
            {
                return false;
            }

            // Calculate RMS energy (assuming 16-bit PCM if uncompressed)
            float rms = CalculateRms(audioData);
            bool isVoice = rms >= VadThreshold;

            if (!isVoice)
            {
                SilenceFramesSuppressed++;
                return false; // Silence suppressed to preserve bandwidth
            }

            var elapsedMs = (uint)(DateTime.UtcNow - _startTime).TotalMilliseconds;
            var header = new AudioFrameHeader
            {
                SequenceNumber = _nextSeq++,
                TimestampMs = elapsedMs,
                SampleRate = sampleRate,
                Channels = channels,
                PayloadSize = audioData.Length,
                IsVoiceDetected = true
            };

            var headerBytes = header.Serialize();
            var packet = new byte[headerBytes.Length + audioData.Length];
            Buffer.BlockCopy(headerBytes, 0, packet, 0, headerBytes.Length);
            Buffer.BlockCopy(audioData, 0, packet, headerBytes.Length, audioData.Length);

            PacketsSent++;
            OnOutgoingAudioPacketReady?.Invoke(packet);
            return true;
        }

        /// <summary>
        /// Processes received network packet and dispatches to audio output.
        /// </summary>
        public bool ProcessIncomingAudioPacket(byte[] packetBytes)
        {
            if (packetBytes == null || packetBytes.Length < 17 || IsDeafened)
            {
                return false;
            }

            var header = AudioFrameHeader.Deserialize(packetBytes, 0);

            // Check packet loss
            if (header.SequenceNumber > _expectedIncomingSeq)
            {
                PacketsLost += (header.SequenceNumber - _expectedIncomingSeq);
            }
            _expectedIncomingSeq = header.SequenceNumber + 1;
            PacketsReceived++;

            int payloadLen = packetBytes.Length - 18;
            if (payloadLen > 0)
            {
                var payload = new byte[payloadLen];
                Buffer.BlockCopy(packetBytes, 18, payload, 0, payloadLen);
                OnIncomingAudioFrame?.Invoke(payload, header);
                return true;
            }

            return false;
        }

        private static float CalculateRms(byte[] buffer)
        {
            if (buffer.Length < 2) return 0f;

            double sumSquares = 0;
            int samples = buffer.Length / 2;

            for (int i = 0; i < buffer.Length - 1; i += 2)
            {
                short sample = (short)(buffer[i] | (buffer[i + 1] << 8));
                float normalized = sample / 32768.0f;
                sumSquares += normalized * normalized;
            }

            return (float)Math.Sqrt(sumSquares / samples);
        }
    }
}
