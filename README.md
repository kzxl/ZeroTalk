# SimpleChatBox

[![.NET Framework 4.8](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)](https://dotnet.microsoft.com/)
[![Dependencies](https://img.shields.io/badge/NuGet-Zero%20Dependencies-brightgreen.svg)]()
[![Platform](https://img.shields.io/badge/Platform-Windows%20Forms-blueviolet.svg)]()
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%7C%20P2P%20%2B%20Relay-orange.svg)]()

**SimpleChatBox** is a high-performance, enterprise-grade real-time chat and multimedia communication platform built on C# Windows Forms and .NET Framework 4.8. It operates with **zero external NuGet package dependencies**, relying solely on native .NET Base Class Libraries (BCL).

The application demonstrates real-world software engineering best practices, including length-prefixed TCP binary framing, ECDH key negotiation, AES-256 end-to-end encryption, STUN NAT traversal with UDP hole punching, server relay fallback, pluggable synthetic video capture, desktop screen sharing, real-time server telemetry HUD, and an automated regression test suite.

---

## 🌟 Key Capabilities

### 1. Pluggable Video Streaming Pipeline (P2P & Relay)
- **Zero Webcam Hardware Required**: Features an embedded high-performance **Synthetic Camera** (`SyntheticVideoSource`) that generates animated radar sweeps, live milliseconds clocks, randomized audio frequency bars, and user avatar identifiers at 15–20 FPS.
- **Desktop Screen Sharing**: Features `ScreenCaptureVideoSource` to stream active desktop displays in real-time.
- **Picture-in-Picture (PIP) & Camera Toggle**: Local preview window rendered simultaneously with remote incoming streams with memory-safe GDI bitmap recycling.
- **STUN NAT Traversal (RFC 5389)**: Discovers public IP endpoints and performs UDP hole punching for direct P2P streaming.
- **Automatic Server Relay Fallback**: Automatically falls back to TCP server relay if UDP hole punching is blocked by symmetric NATs.

### 2. End-to-End Cryptography & Security
- **ECDH Key Exchange**: Implements Diffie-Hellman Elliptic Curve Key Exchange (`DiffieHellmanHelper`) for per-session shared secret negotiation.
- **AES-256-CBC Encryption**: End-to-end encrypted messaging with dynamically generated initialization vectors (IVs).
- **Secure Authentication**: SHA-256 hashed password storage (`UserStore`) with pre-seeded demo credentials.

### 3. Server Management & Real-Time Telemetry Dashboard
- **Live Client Directory**: `ListView` displaying client IDs, usernames, display names, IP endpoints, connection timestamps, and packet telemetry.
- **Administrative Controls**: Right-click context menu to disconnect/kick connected clients.
- **Real-Time HUD**: Live uptime clock, total routed message counters, and host IP address display for LAN discovery.
- **One-Click Multi-Client Launch**: Button to automatically spin up 2 demo client instances (`alice` and `bob`).

### 4. Client UX & Quick Demo Workflow
- **1-Click Demo Login**: Quick-access buttons for pre-seeded users (`👤 Alice`, `👤 Bob`, `👤 Charlie`).
- **CLI Automation**: Supports command-line auto-login arguments (e.g., `ChatBox.Client.exe demo_alice`).
- **File Transfer Progress**: Real-time chunk transmission progress (`cur/total chunks` and percentage).
- **Dedicated Image Previewer**: Automatic modal dialog (`frmImagePreview`) for viewing received `.jpg`, `.png`, `.gif`, and `.bmp` files with dimensions, file size, and explorer shortcuts.
- **Group & Direct Messaging**: Support for private 1-to-1 conversations and broadcast rooms.

### 5. Automated Regression Test Suite (`ChatBox.Tests`)
- Custom lightweight test runner with zero third-party dependencies verifying:
  1. Packet serialization, length-prefixed framing, special characters, and Unicode text.
  2. AES-256 encryption/decryption roundtrips and ECDH shared key equivalence.
  3. User authentication, password hashing, and message history persistence.
  4. Video generator frame capture and JPEG SOI header validity.

---

## 🏛️ System Architecture

```
ChatBoxSimple.sln
│
├── ChatBox.Shared/               # Core BCL Library (Shared across Server & Client)
│   ├── Constants/AppConstants.cs # Protocol ports, buffer limits, chunk sizes
│   ├── Crypto/                   # AES-256-CBC & Diffie-Hellman (ECDH) helpers
│   ├── DTOs/                     # Network data transfer contracts
│   ├── Network/StunClient.cs     # RFC 5389 STUN NAT traversal engine
│   └── Protocol/                 # Packet framing, types, and JSON serialization
│
├── ChatBox.Server/               # WinForms Server Management Host
│   ├── Data/                     # UserStore (JSON) & MessageStore (JSON history)
│   ├── Models/                   # ConnectedClient & UserAccount
│   ├── Services/                 # TcpServerService, AuthService, MessageRouter
│   └── Forms/frmServer.cs        # Telemetry HUD, Client list, and 1-click launcher
│
├── ChatBox.Client/               # WinForms End-User Client
│   ├── Forms/                    # frmLogin, frmChat, frmVideoCall, frmImagePreview
│   ├── Helpers/VideoRecorder.cs  # Video call recording hooks
│   └── Services/                 # Pluggable IVideoSource, VideoCallService,
│                                 # FileTransferService, FileReceiveService, UdpPeerService
│
└── ChatBox.Tests/                # Standalone Automated Test Runner (.NET 4.8)
    ├── Program.cs                # Test harness entry point (exit code 0/1)
    ├── ProtocolTests.cs          # Serialization and packet integrity tests
    ├── CryptoTests.cs            # Cryptographic roundtrip tests
    ├── StorageTests.cs           # Database and persistence tests
    └── VideoSourceTests.cs       # Video frame generation and compression tests
```

---

## 📹 Video Call Flow Diagram

```
┌──────────────────────────────────────────────────────────┐
│  Client A                  Server               Client B │
│      │                        │                    │     │
│      ├──STUN Discovery───────→│                    │     │
│      │  (Public IP/Port)      │                    │     │
│      │                        │                    │     │
│      ├──VideoCallRequest─────→│───Forward Signal──→│     │
│      │                        │     ←──Accept──────┤     │
│      │   ←──Forward Signal────┤                    │     │
│      │                                             │     │
│      ├════════════ UDP Hole Punching ══════════════┤     │
│      │                                             │     │
│      │  [Success] → Direct P2P UDP Streaming       │     │
│      │  [Failure] → Fallback TCP Server Relay      │     │
│      │                                             │     │
│      └──Display Local PIP (Synthetic/Screen Share)─┘     │
└──────────────────────────────────────────────────────────┘
```

---

## 👥 Default Demo Accounts

All demo accounts are pre-seeded in `UserStore` with password `123`:

| Username | Password | Display Name | Role |
| :--- | :--- | :--- | :--- |
| `alice` | `123` | Alice Johnson | Demo User A |
| `bob` | `123` | Bob Williams | Demo User B |
| `charlie` | `123` | Charlie Davis | Demo User C |

---

## 🚀 Quick Start Guide

### Prerequisites
- **Operating System**: Windows 10 / 11 / Windows Server 2016+
- **Runtime**: .NET Framework 4.8 runtime (built into modern Windows)
- **Build Tools**: .NET SDK (`dotnet build`) or Visual Studio 2022 / MSBuild

### Automated 1-Command Demo Launcher
Run the PowerShell orchestrator script from the repository root:
```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\run-demo.ps1
```
This script automatically:
1. Compiles the solution in `Debug` configuration.
2. Executes the regression test suite (`ChatBox.Tests.exe`).
3. Starts the Server Dashboard and binds to port `9000`.
4. Spawns two pre-configured demo clients (`Alice` and `Bob`) with automatic login.

---

### Manual Execution Steps

#### 1. Compile the Solution
```bash
dotnet build ChatBoxSimple.sln -c Debug
```

#### 2. Run the Test Suite
```bash
.\ChatBox.Tests\bin\Debug\ChatBox.Tests.exe
```

#### 3. Start the Server
```bash
.\ChatBox.Server\bin\Debug\ChatBox.Server.exe
```
- Click **▶ Khởi động** to open port 9000.
- Click **⚡ Mở 2 Client Demo** to launch both Alice and Bob simultaneously.

#### 4. Start the Clients
```bash
.\ChatBox.Client\bin\Debug\ChatBox.Client.exe demo_alice
.\ChatBox.Client\bin\Debug\ChatBox.Client.exe demo_bob
```

---

## 📦 Build & Packaging (`scripts/publish-app.ps1`)

In adherence with enterprise deployment standards, the project supports both distribution modes:

```powershell
# Full release package:
powershell -ExecutionPolicy Bypass -File .\scripts\publish-app.ps1 -Mode Full

# Lite release package (binaries only, excludes .pdb symbols):
powershell -ExecutionPolicy Bypass -File .\scripts\publish-app.ps1 -Mode Lite
```

---

## 📄 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
