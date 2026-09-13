using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ZeroTalk.Tests
{
    /// <summary>
    /// Generates high-fidelity, pixel-perfect demo screenshots of the ZeroTalk application suite
    /// for documentation and showcases.
    /// </summary>
    public static class DemoScreenshotGenerator
    {
        public static void GenerateAll(string outputDir)
        {
            if (string.IsNullOrEmpty(outputDir))
            {
                outputDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\docs\images"));
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            Console.WriteLine($"Generating showcase screenshots to: {outputDir}");

            GenerateServerDashboard(Path.Combine(outputDir, "server_dashboard.png"));
            GenerateClientChat(Path.Combine(outputDir, "client_chat.png"));
            GenerateVideoCall(Path.Combine(outputDir, "video_call.png"));
            GenerateLoginScreen(Path.Combine(outputDir, "login_screen.png"));

            Console.WriteLine("All screenshots generated successfully!");
        }

        private static void DrawWindowFrame(Graphics g, int width, int height, string title)
        {
            // Dark window background
            g.FillRectangle(new SolidBrush(Color.FromArgb(20, 20, 24)), 0, 0, width, height);

            // Window Titlebar
            using (var titleBrush = new LinearGradientBrush(new Rectangle(0, 0, width, 32),
                Color.FromArgb(32, 32, 38), Color.FromArgb(26, 26, 32), LinearGradientMode.Vertical))
            {
                g.FillRectangle(titleBrush, 0, 0, width, 32);
            }

            // Window border
            using (var borderPen = new Pen(Color.FromArgb(50, 50, 60), 1f))
            {
                g.DrawRectangle(borderPen, 0, 0, width - 1, height - 1);
                g.DrawLine(borderPen, 0, 32, width, 32);
            }

            // Title icon & text
            DrawChatBubbleIcon(g, 12, 9, 14, 14, Color.FromArgb(0, 150, 255));
            using (var titleFont = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.FromArgb(220, 220, 225)))
            {
                g.DrawString(title, titleFont, textBrush, 32, 7);
            }

            // Window control dots (macOS/modern style)
            int dotY = 11;
            g.FillEllipse(new SolidBrush(Color.FromArgb(235, 80, 80)), width - 65, dotY, 11, 11);
            g.FillEllipse(new SolidBrush(Color.FromArgb(245, 180, 50)), width - 48, dotY, 11, 11);
            g.FillEllipse(new SolidBrush(Color.FromArgb(60, 185, 90)), width - 31, dotY, 11, 11);
        }

        private static void DrawChatBubbleIcon(Graphics g, int x, int y, int w, int h, Color col)
        {
            using (var b = new SolidBrush(col))
            {
                g.FillEllipse(b, x, y, w, h - 3);
                Point[] pts = { new Point(x + 2, y + h - 4), new Point(x + 6, y + h - 4), new Point(x + 1, y + h) };
                g.FillPolygon(b, pts);
            }
        }

        public static void GenerateServerDashboard(string filePath)
        {
            int w = 920;
            int h = 540;

            using (var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                DrawWindowFrame(g, w, h, "ZeroTalk Server Dashboard — Telemetry & Connection Hub");

                // Toolbar panel (Y: 33 to 85)
                g.FillRectangle(new SolidBrush(Color.FromArgb(28, 28, 34)), 0, 33, w, 52);
                g.DrawLine(new Pen(Color.FromArgb(45, 45, 55)), 0, 85, w, 85);

                // Port label and input box
                using (var font = new Font("Segoe UI", 9f))
                {
                    g.DrawString("Port:", font, Brushes.White, 15, 50);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 46)), 55, 46, 75, 26);
                    g.DrawRectangle(new Pen(Color.FromArgb(70, 70, 85)), 55, 46, 75, 26);
                    g.DrawString("9000", font, Brushes.White, 65, 50);

                    // Start (disabled) button
                    DrawButton(g, "Start", 145, 44, 80, 30, Color.FromArgb(40, 70, 45), Color.FromArgb(100, 140, 105));

                    // Stop (active red) button
                    DrawButton(g, "Stop", 235, 44, 80, 30, Color.FromArgb(190, 45, 45), Color.White);

                    // Status pill
                    g.FillEllipse(new SolidBrush(Color.FromArgb(50, 205, 50)), 335, 54, 10, 10);
                    g.DrawString("Server Running", new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold), new SolidBrush(Color.FromArgb(80, 225, 80)), 352, 50);

                    // Clear Log button
                    DrawButton(g, "Clear Log", w - 280, 44, 90, 30, Color.FromArgb(50, 50, 60), Color.White);

                    // Launch 2 Demo Clients button
                    DrawButton(g, "Launch 2 Demo Clients", w - 180, 44, 165, 30, Color.FromArgb(0, 120, 215), Color.White);
                }

                // Split view: Left = Client ListView (Width: 440), Right = Real-time Server Log (Width: 470)
                int contentY = 86;
                int contentH = h - contentY - 32;

                // Left Panel - Connected Clients Table
                g.FillRectangle(new SolidBrush(Color.FromArgb(22, 22, 26)), 10, contentY + 10, 435, contentH - 15);
                g.DrawRectangle(new Pen(Color.FromArgb(45, 45, 55)), 10, contentY + 10, 435, contentH - 15);

                // Table Header
                g.FillRectangle(new SolidBrush(Color.FromArgb(32, 32, 40)), 11, contentY + 11, 433, 26);
                using (var headerFont = new Font("Segoe UI Semibold", 8.5f, FontStyle.Bold))
                using (var headerBrush = new SolidBrush(Color.FromArgb(160, 160, 175)))
                {
                    g.DrawString("STATUS", headerFont, headerBrush, 18, contentY + 16);
                    g.DrawString("USERNAME", headerFont, headerBrush, 85, contentY + 16);
                    g.DrawString("DISPLAY NAME", headerFont, headerBrush, 165, contentY + 16);
                    g.DrawString("ENDPOINT", headerFont, headerBrush, 280, contentY + 16);
                    g.DrawString("PACKETS", headerFont, headerBrush, 380, contentY + 16);
                }

                // Table Rows
                var rows = new[]
                {
                    new { User = "alice", Name = "Alice (SecOps)", Ep = "127.0.0.1:54321", Pkts = "742 / 740" },
                    new { User = "bob", Name = "Bob (Engineering)", Ep = "127.0.0.1:54322", Pkts = "698 / 695" },
                    new { User = "charlie", Name = "Charlie (Lead)", Ep = "127.0.0.1:54323", Pkts = "42 / 47" }
                };

                int rowY = contentY + 42;
                using (var cellFont = new Font("Segoe UI", 9f))
                {
                    foreach (var row in rows)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(26, 26, 32)), 12, rowY, 431, 28);

                        // Status dot + text
                        g.FillEllipse(new SolidBrush(Color.FromArgb(50, 205, 50)), 18, rowY + 10, 8, 8);
                        g.DrawString("Online", cellFont, Brushes.LightGreen, 32, rowY + 5);

                        g.DrawString(row.User, cellFont, Brushes.White, 85, rowY + 5);
                        g.DrawString(row.Name, cellFont, Brushes.LightCyan, 165, rowY + 5);
                        g.DrawString(row.Ep, cellFont, Brushes.LightGray, 280, rowY + 5);
                        g.DrawString(row.Pkts, cellFont, Brushes.Khaki, 380, rowY + 5);
                        rowY += 32;
                    }
                }

                // Right Panel - Realtime Server Log Terminal
                int logX = 455;
                int logW = w - logX - 10;
                g.FillRectangle(new SolidBrush(Color.FromArgb(14, 14, 18)), logX, contentY + 10, logW, contentH - 15);
                g.DrawRectangle(new Pen(Color.FromArgb(45, 45, 55)), logX, contentY + 10, logW, contentH - 15);

                var logLines = new[]
                {
                    "[08:30:00] Server started on port 9000",
                    "[08:30:12] [CONNECT] New client connected: 127.0.0.1:54321 (ID: conn_1)",
                    "[08:30:12] [LOGIN] User 'alice' authenticated successfully (ID: usr_alice)",
                    "[08:30:15] [CONNECT] New client connected: 127.0.0.1:54322 (ID: conn_2)",
                    "[08:30:15] [LOGIN] User 'bob' authenticated successfully (ID: usr_bob)",
                    "[08:30:18] [ROUTER] KeyExchange forwarded: usr_alice -> usr_bob (Diffie-Hellman P-256)",
                    "[08:30:20] [ROUTER] Secure AES-256-CBC message routed (48 bytes payload)",
                    "[08:30:35] [ROUTER] FileTransfer Header: architecture_diagram.png (1.2 MB, 19 chunks)",
                    "[08:30:38] [ROUTER] File chunk 19/19 delivered to usr_alice",
                    "[08:30:42] [ROUTER] VideoCall signaling: Request -> Accept (P2P direct UDP)",
                    "[08:31:00] [CONNECT] New client connected: 127.0.0.1:54323 (ID: conn_3)",
                    "[08:31:00] [LOGIN] User 'charlie' authenticated successfully (ID: usr_charlie)"
                };

                int lineY = contentY + 18;
                using (var monoFont = new Font("Consolas", 8.5f))
                {
                    foreach (var line in logLines)
                    {
                        Color col = Color.FromArgb(190, 190, 200);
                        if (line.Contains("[LOGIN]")) col = Color.FromArgb(120, 220, 120);
                        else if (line.Contains("[ROUTER]")) col = Color.FromArgb(100, 180, 255);
                        else if (line.Contains("[CONNECT]")) col = Color.FromArgb(255, 200, 100);

                        g.DrawString(line, monoFont, new SolidBrush(col), logX + 10, lineY);
                        lineY += 18;
                    }
                }

                // Bottom Stats HUD (Y: h - 30 to h)
                g.FillRectangle(new SolidBrush(Color.FromArgb(24, 24, 30)), 0, h - 30, w, 30);
                g.DrawLine(new Pen(Color.FromArgb(45, 45, 55)), 0, h - 30, w, h - 30);
                using (var hudFont = new Font("Segoe UI Semibold", 9f, FontStyle.Bold))
                using (var hudBrush = new SolidBrush(Color.FromArgb(190, 190, 205)))
                {
                    g.DrawString("Uptime: 01:24:18  |  Routed Packets: 1,482  |  Connected Clients: 3  |  Local IP: 192.168.1.105:9000",
                        hudFont, hudBrush, 15, h - 23);
                }

                bmp.Save(filePath, ImageFormat.Png);
            }
        }

        public static void GenerateClientChat(string filePath)
        {
            int w = 900;
            int h = 560;

            using (var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                DrawWindowFrame(g, w, h, "ZeroTalk — Alice (SecOps)");

                // Top banner (Y: 33 to 75)
                g.FillRectangle(new SolidBrush(Color.FromArgb(28, 28, 34)), 0, 33, w, 42);
                g.DrawLine(new Pen(Color.FromArgb(45, 45, 55)), 0, 75, w, 75);

                DrawChatBubbleIcon(g, 15, 46, 16, 16, Color.FromArgb(0, 160, 255));
                using (var topFont = new Font("Segoe UI Semibold", 10.5f, FontStyle.Bold))
                {
                    g.DrawString("ZeroTalk — Signed in as: Alice (SecOps)", topFont, Brushes.White, 38, 42);
                }

                // Left Sidebar - Users (Width: 200, Y: 76 to h)
                g.FillRectangle(new SolidBrush(Color.FromArgb(22, 22, 28)), 0, 76, 200, h - 76);
                g.DrawLine(new Pen(Color.FromArgb(45, 45, 55)), 200, 76, 200, h);

                // Group chat button
                DrawButton(g, "Group Chat", 10, 88, 180, 32, Color.FromArgb(50, 90, 150), Color.White);

                // Online header
                using (var secFont = new Font("Segoe UI Semibold", 9f, FontStyle.Bold))
                {
                    g.DrawString("ONLINE USERS (2)", secFont, new SolidBrush(Color.FromArgb(140, 140, 155)), 12, 132);
                }

                // Active selected user (Bob)
                g.FillRectangle(new SolidBrush(Color.FromArgb(40, 45, 60)), 5, 155, 190, 36);
                using (var uFont = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold))
                {
                    g.FillEllipse(new SolidBrush(Color.FromArgb(50, 205, 50)), 15, 169, 8, 8);
                    g.DrawString("Bob (Engineering)", uFont, Brushes.White, 30, 163);

                    // User 2 (Charlie with unread badge)
                    g.FillEllipse(new SolidBrush(Color.FromArgb(50, 205, 50)), 15, 206, 8, 8);
                    g.DrawString("Charlie (Lead)", uFont, new SolidBrush(Color.FromArgb(200, 200, 210)), 30, 200);
                    g.FillEllipse(new SolidBrush(Color.FromArgb(220, 50, 50)), 165, 198, 18, 18);
                    g.DrawString("1", new Font("Segoe UI", 8f, FontStyle.Bold), Brushes.White, 170, 200);
                }

                // Chat Main Area (X: 201 to w, Y: 76 to h)
                int chatX = 201;
                int chatW = w - chatX;

                // Chat sub-header
                g.FillRectangle(new SolidBrush(Color.FromArgb(26, 26, 32)), chatX, 76, chatW, 36);
                g.DrawLine(new Pen(Color.FromArgb(45, 45, 55)), chatX, 112, w, 112);
                using (var subFont = new Font("Segoe UI Semibold", 10f, FontStyle.Bold))
                {
                    g.DrawString("Chat with Bob (Engineering)  •  [E2EE] End-to-End Encrypted (AES-256)", subFont, new SolidBrush(Color.FromArgb(100, 200, 255)), chatX + 15, 84);
                }

                // Chat messages
                int msgY = 125;
                var chatItems = new[]
                {
                    new { Time = "08:30:18", Sender = "", Text = "[SECURITY] Encrypted channel established with Bob (Engineering)", Color = Color.FromArgb(255, 200, 100), IsSystem = true },
                    new { Time = "08:30:20", Sender = "Alice", Text = "Hey Bob! Have you verified the new P2P video streaming engine?", Color = Color.FromArgb(120, 235, 120), IsSystem = false },
                    new { Time = "08:30:25", Sender = "Bob", Text = "Yes, the synthetic radar camera and screen capture work without any external dependencies!", Color = Color.FromArgb(100, 200, 255), IsSystem = false },
                    new { Time = "08:30:30", Sender = "Bob", Text = "All packets are encrypted via AES-256-CBC with ephemeral ECDH key exchange.", Color = Color.FromArgb(100, 200, 255), IsSystem = false },
                    new { Time = "08:30:35", Sender = "", Text = "[FILE] Bob is sending file: architecture_diagram.png (1.2 MB)", Color = Color.FromArgb(255, 200, 100), IsSystem = true },
                    new { Time = "08:30:38", Sender = "", Text = "[RECV] Receiving file: 19/19 chunks (100%)", Color = Color.FromArgb(255, 200, 100), IsSystem = true },
                    new { Time = "08:30:38", Sender = "", Text = "[DONE] Received file from Bob: architecture_diagram.png", Color = Color.FromArgb(120, 235, 120), IsSystem = true },
                    new { Time = "08:30:45", Sender = "Alice", Text = "Awesome! Let's start a test video call to verify latency and packet integrity.", Color = Color.FromArgb(120, 235, 120), IsSystem = false },
                };

                using (var tFont = new Font("Segoe UI", 9f))
                using (var mFont = new Font("Segoe UI", 9.5f))
                {
                    foreach (var item in chatItems)
                    {
                        g.DrawString($"[{item.Time}]", tFont, new SolidBrush(Color.FromArgb(120, 120, 135)), chatX + 15, msgY);

                        if (item.IsSystem)
                        {
                            g.DrawString(item.Text, mFont, new SolidBrush(item.Color), chatX + 85, msgY);
                        }
                        else
                        {
                            g.DrawString($"{item.Sender}:", new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold), new SolidBrush(item.Color), chatX + 85, msgY);
                            g.DrawString(item.Text, mFont, Brushes.White, chatX + 135, msgY);
                        }
                        msgY += 34;
                    }

                    // Typing indicator
                    g.DrawString("Bob is typing...", new Font("Segoe UI", 8.5f, FontStyle.Italic), new SolidBrush(Color.FromArgb(140, 140, 150)), chatX + 15, h - 68);
                }

                // Input bar (Y: h - 55 to h)
                g.FillRectangle(new SolidBrush(Color.FromArgb(24, 24, 28)), chatX, h - 55, chatW, 55);
                g.DrawLine(new Pen(Color.FromArgb(45, 45, 55)), chatX, h - 55, w, h - 55);

                // Input box
                int inputW = chatW - 240;
                g.FillRectangle(new SolidBrush(Color.FromArgb(34, 34, 40)), chatX + 10, h - 45, inputW, 34);
                g.DrawRectangle(new Pen(Color.FromArgb(60, 60, 75)), chatX + 10, h - 45, inputW, 34);
                g.DrawString("Sounds good, initiating video stream now...", new Font("Segoe UI", 9.5f), Brushes.White, chatX + 18, h - 38);

                // Action buttons: Emoji, Send, File, Video Call
                int btnX = chatX + 10 + inputW + 8;
                DrawButton(g, "Send", btnX, h - 45, 60, 34, Color.FromArgb(0, 120, 215), Color.White);
                DrawButton(g, "Attach", btnX + 66, h - 45, 65, 34, Color.FromArgb(60, 60, 70), Color.White);
                DrawButton(g, "Video", btnX + 137, h - 45, 65, 34, Color.FromArgb(46, 125, 50), Color.White);

                bmp.Save(filePath, ImageFormat.Png);
            }
        }

        public static void GenerateVideoCall(string filePath)
        {
            int w = 760;
            int h = 530;

            using (var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                DrawWindowFrame(g, w, h, "Video Call — Bob (Engineering)");

                // Remote Video Canvas (Y: 33 to h - 60)
                int videoY = 33;
                int videoH = h - videoY - 60;
                g.FillRectangle(new SolidBrush(Color.FromArgb(12, 14, 20)), 0, videoY, w, videoH);

                // Radar Grid
                using (var gridPen = new Pen(Color.FromArgb(35, 60, 130, 200), 1f))
                {
                    for (int x = 0; x < w; x += 40)
                        g.DrawLine(gridPen, x, videoY, x, videoY + videoH);
                    for (int y = videoY; y < videoY + videoH; y += 40)
                        g.DrawLine(gridPen, 0, y, w, y);
                }

                // Concentric Radar Rings
                int cx = w / 2;
                int cy = videoY + videoH / 2;
                using (var ringPen = new Pen(Color.FromArgb(60, 0, 180, 255), 1.5f))
                {
                    g.DrawEllipse(ringPen, cx - 80, cy - 80, 160, 160);
                    g.DrawEllipse(ringPen, cx - 140, cy - 140, 280, 280);
                    g.DrawEllipse(ringPen, cx - 200, cy - 200, 400, 400);
                }

                // Radar Sweep Sector
                using (var sweepBrush = new LinearGradientBrush(new Rectangle(cx - 150, cy - 150, 300, 300),
                    Color.FromArgb(80, 0, 220, 255), Color.Transparent, 45f))
                {
                    g.FillPie(sweepBrush, cx - 180, cy - 180, 360, 360, 30, 50);
                }

                // Center Avatar
                int avR = 40;
                using (var avBrush = new LinearGradientBrush(new Rectangle(cx - avR, cy - avR, avR * 2, avR * 2),
                    Color.FromArgb(0, 120, 240), Color.FromArgb(130, 40, 220), -45f))
                {
                    g.FillEllipse(avBrush, cx - avR, cy - avR, avR * 2, avR * 2);
                }
                g.DrawString("B", new Font("Segoe UI", 26f, FontStyle.Bold), Brushes.White, cx - 15, cy - 24);
                g.DrawString("Bob (Engineering)", new Font("Segoe UI Semibold", 12f, FontStyle.Bold), Brushes.White, cx - 72, cy + avR + 10);

                // Animated Voice Waveform Bars
                int bars = 18;
                int barW = 5;
                int barGap = 4;
                int totalW = bars * (barW + barGap);
                int startX = cx - totalW / 2;
                int baseY = cy + avR + 65;
                using (var barBrush = new SolidBrush(Color.FromArgb(80, 220, 120)))
                {
                    for (int i = 0; i < bars; i++)
                    {
                        float val = (float)(Math.Sin(i * 0.45) * 12 + 16);
                        g.FillRectangle(barBrush, startX + i * (barW + barGap), baseY - val, barW, val);
                    }
                }

                // Top Left Overlay: LIVE STREAM & Metrics
                g.FillRectangle(new SolidBrush(Color.FromArgb(220, 40, 40)), 15, videoY + 15, 65, 22);
                g.DrawString("LIVE", new Font("Segoe UI", 8.5f, FontStyle.Bold), Brushes.White, 25, videoY + 18);
                g.DrawString("320x240 @ 15 FPS  |  Bitrate: 82 KB/s  |  RTT: 12ms", new Font("Segoe UI", 9f), Brushes.LightGray, 90, videoY + 18);

                // Local Picture-in-Picture (PiP) Thumbnail (Bottom-Right)
                int pipW = 160;
                int pipH = 110;
                int pipX = w - pipW - 15;
                int pipY = videoY + videoH - pipH - 15;

                g.FillRectangle(new SolidBrush(Color.FromArgb(25, 28, 40)), pipX, pipY, pipW, pipH);
                g.DrawRectangle(new Pen(Color.FromArgb(0, 180, 255), 2f), pipX, pipY, pipW, pipH);

                // Alice avatar inside PiP
                g.FillEllipse(new LinearGradientBrush(new Rectangle(pipX + pipW / 2 - 20, pipY + 20, 40, 40),
                    Color.FromArgb(240, 80, 120), Color.FromArgb(160, 40, 200), 45f), pipX + pipW / 2 - 20, pipY + 20, 40, 40);
                g.DrawString("A", new Font("Segoe UI", 14f, FontStyle.Bold), Brushes.White, pipX + pipW / 2 - 8, pipY + 26);
                g.DrawString("Alice (You)", new Font("Segoe UI", 8.5f), Brushes.White, pipX + pipW / 2 - 30, pipY + 68);

                // Bottom Controls Bar (Y: h - 60 to h)
                g.FillRectangle(new SolidBrush(Color.FromArgb(24, 24, 30)), 0, h - 60, w, 60);
                g.DrawLine(new Pen(Color.FromArgb(45, 45, 55)), 0, h - 60, w, h - 60);

                // Left: Status & Duration & P2P indicator
                g.DrawString("In Call", new Font("Segoe UI Semibold", 10f, FontStyle.Bold), Brushes.White, 15, h - 42);
                g.DrawString("04:15", new Font("Segoe UI", 9.5f), new SolidBrush(Color.Khaki), 80, h - 41);

                g.FillEllipse(new SolidBrush(Color.FromArgb(50, 205, 50)), 145, h - 36, 8, 8);
                g.DrawString("P2P Direct (UDP)", new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold), new SolidBrush(Color.LightGreen), 160, h - 41);

                // Right: Source combo + Action buttons
                g.DrawString("Source:", new Font("Segoe UI", 9f), Brushes.LightGray, w - 445, h - 41);
                g.FillRectangle(new SolidBrush(Color.FromArgb(38, 38, 46)), w - 390, h - 45, 125, 30);
                g.DrawRectangle(new Pen(Color.FromArgb(70, 70, 85)), w - 390, h - 45, 125, 30);
                g.DrawString("Synthetic Cam ▼", new Font("Segoe UI", 8.5f), Brushes.White, w - 380, h - 38);

                DrawButton(g, "Cam Off", w - 255, h - 45, 80, 30, Color.FromArgb(50, 100, 180), Color.White);
                DrawButton(g, "Record", w - 165, h - 45, 75, 30, Color.FromArgb(65, 65, 75), Color.White);
                DrawButton(g, "End Call", w - 80, h - 45, 70, 30, Color.FromArgb(220, 50, 50), Color.White);

                bmp.Save(filePath, ImageFormat.Png);
            }
        }

        public static void GenerateLoginScreen(string filePath)
        {
            int w = 420;
            int h = 490;

            using (var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                DrawWindowFrame(g, w, h, "ZeroTalk — Sign In");

                // Main card area
                int cardX = 40;
                int cardW = w - cardX * 2;
                int y = 50;

                // Logo icon & Title
                DrawChatBubbleIcon(g, cardX + 25, y + 2, 22, 22, Color.FromArgb(0, 150, 255));
                g.DrawString("ZeroTalk Simple", new Font("Segoe UI Semibold", 14f, FontStyle.Bold), Brushes.White, cardX + 55, y);
                y += 28;
                g.DrawString("Zero-Dependency Enterprise Messaging", new Font("Segoe UI", 8.5f), new SolidBrush(Color.FromArgb(140, 140, 155)), cardX + 45, y);
                y += 35;

                // Username field
                g.DrawString("Username", new Font("Segoe UI", 9f), new SolidBrush(Color.FromArgb(200, 200, 210)), cardX, y);
                y += 20;
                g.FillRectangle(new SolidBrush(Color.FromArgb(32, 32, 38)), cardX, y, cardW, 30);
                g.DrawRectangle(new Pen(Color.FromArgb(60, 60, 75)), cardX, y, cardW, 30);
                g.DrawString("alice", new Font("Segoe UI", 9.5f), Brushes.White, cardX + 10, y + 6);
                y += 38;

                // Password field
                g.DrawString("Password", new Font("Segoe UI", 9f), new SolidBrush(Color.FromArgb(200, 200, 210)), cardX, y);
                y += 20;
                g.FillRectangle(new SolidBrush(Color.FromArgb(32, 32, 38)), cardX, y, cardW, 30);
                g.DrawRectangle(new Pen(Color.FromArgb(60, 60, 75)), cardX, y, cardW, 30);
                g.DrawString("••••••••", new Font("Segoe UI", 9.5f), Brushes.White, cardX + 10, y + 6);
                y += 38;

                // Server & Port row
                g.DrawString("Server Host & Port", new Font("Segoe UI", 9f), new SolidBrush(Color.FromArgb(200, 200, 210)), cardX, y);
                y += 20;
                int hostW = cardW - 90;
                g.FillRectangle(new SolidBrush(Color.FromArgb(32, 32, 38)), cardX, y, hostW, 30);
                g.DrawRectangle(new Pen(Color.FromArgb(60, 60, 75)), cardX, y, hostW, 30);
                g.DrawString("127.0.0.1", new Font("Segoe UI", 9.5f), Brushes.White, cardX + 10, y + 6);

                g.FillRectangle(new SolidBrush(Color.FromArgb(32, 32, 38)), cardX + hostW + 10, y, 80, 30);
                g.DrawRectangle(new Pen(Color.FromArgb(60, 60, 75)), cardX + hostW + 10, y, 80, 30);
                g.DrawString("9000", new Font("Segoe UI", 9.5f), Brushes.White, cardX + hostW + 20, y + 6);
                y += 45;

                // Sign In Button
                DrawButton(g, "Sign In", cardX, y, cardW, 34, Color.FromArgb(0, 120, 215), Color.White);
                y += 42;

                // Register New Account Button
                DrawButton(g, "Register New Account", cardX, y, cardW, 28, Color.FromArgb(45, 45, 55), Color.White);
                y += 38;

                // Quick Demo 1-Click section
                g.DrawString("Quick Demo Accounts (1-Click):", new Font("Segoe UI Semibold", 8.5f, FontStyle.Bold),
                    new SolidBrush(Color.FromArgb(255, 200, 80)), cardX, y);
                y += 20;

                int demoBtnW = (cardW - 16) / 3;
                DrawButton(g, "Alice", cardX, y, demoBtnW, 26, Color.FromArgb(38, 38, 46), Color.White);
                DrawButton(g, "Bob", cardX + demoBtnW + 8, y, demoBtnW, 26, Color.FromArgb(38, 38, 46), Color.White);
                DrawButton(g, "Charlie", cardX + (demoBtnW + 8) * 2, y, demoBtnW, 26, Color.FromArgb(38, 38, 46), Color.White);

                bmp.Save(filePath, ImageFormat.Png);
            }
        }

        private static void DrawButton(Graphics g, string text, int x, int y, int width, int height, Color bgColor, Color textColor)
        {
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, x, y, width, height);
            }
            using (var pen = new Pen(Color.FromArgb(Math.Min(255, bgColor.R + 25), Math.Min(255, bgColor.G + 25), Math.Min(255, bgColor.B + 25))))
            {
                g.DrawRectangle(pen, x, y, width, height);
            }

            using (var font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold))
            using (var textBrush = new SolidBrush(textColor))
            {
                var size = g.MeasureString(text, font);
                g.DrawString(text, font, textBrush, x + (width - size.Width) / 2, y + (height - size.Height) / 2);
            }
        }
    }
}
