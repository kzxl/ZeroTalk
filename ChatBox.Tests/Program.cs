using System;

namespace ChatBox.Tests
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("==================================================");
            Console.WriteLine("   ChatBoxSimple Automated Test Suite (.NET 4.8)  ");
            Console.WriteLine("==================================================");

            int passed = 0;
            int failed = 0;

            void RunTest(string testName, Action action)
            {
                Console.Write($"Running: {testName} ... ");
                try
                {
                    action();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[PASS]");
                    Console.ResetColor();
                    passed++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[FAIL]");
                    Console.ResetColor();
                    Console.WriteLine($"   Error: {ex.Message}");
                    failed++;
                }
            }

            RunTest("Protocol: Packet Framing & Deserialization", ProtocolTests.RunAll);
            RunTest("Crypto: AES-256 & Diffie-Hellman Key Exchange", CryptoTests.RunAll);
            RunTest("Storage: UserStore & MessageStore Persistence", StorageTests.RunAll);
            RunTest("Video: Synthetic Camera Frame Generation & JPEG Compression", VideoSourceTests.RunAll);

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Summary: {passed} passed, {failed} failed.");
            Console.WriteLine("==================================================");

            return failed == 0 ? 0 : 1;
        }
    }
}
