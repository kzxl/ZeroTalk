using System.Threading;
using ZeroTalk.Client.Services;

namespace ZeroTalk.Tests
{
    public static class VideoSourceTests
    {
        public static void RunAll()
        {
            TestSyntheticVideoSourceFrameGeneration();
        }

        private static void TestSyntheticVideoSourceFrameGeneration()
        {
            byte[] capturedFrame = null;
            var resetEvent = new ManualResetEvent(false);

            using (var source = new SyntheticVideoSource("TestPilot"))
            {
                source.OnFrameCaptured += frame =>
                {
                    capturedFrame = frame;
                    resetEvent.Set();
                };

                source.Start(320, 240, 15);
                bool signalReceived = resetEvent.WaitOne(2000);
                source.Stop();

                Assert.True(signalReceived, "SyntheticVideoSource should capture frames within 2s");
                Assert.NotNull(capturedFrame, "Frame data should not be null");
                Assert.True(capturedFrame.Length > 100, "Frame data should be reasonable JPEG size");

                // Verify JPEG SOI marker (0xFF, 0xD8)
                Assert.Equal((byte)0xFF, capturedFrame[0]);
                Assert.Equal((byte)0xD8, capturedFrame[1]);
            }
        }
    }
}
