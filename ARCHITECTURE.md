# IDEMIA Morpho L1 RD Service AI Suite — Architecture & Technical Design

## 1. System Overview

The **IDEMIA Morpho L1 RD Service AI Suite** bridges the gap between hardware device drivers, the Windows kernel, the local UIDAI Registered Device daemon, and client applications.

```
┌─────────────────────────────────────────────────────────────┐
│                 Client Layer / User Space                   │
│  ┌───────────────────────────────────────────────────────┐  │
│  │   Morpho L1 RD Service AI Suite (WPF / C# .NET 4.8)   │  │
│  │   - 1-Click Automated Setup                           │  │
│  │   - In-App Native Biometric Studio                    │  │
│  │   - Autonomous AI Self-Healing Engine                 │  │
│  │   - Memory Integrity (Code 39) Resolver               │  │
│  └──────────────────────────┬────────────────────────────┘  │
│                             │ Low-Level TLS / Raw Socket    │
│                             ▼ (Supports Unix LF & CRLF)     │
│  ┌───────────────────────────────────────────────────────┐  │
│  │      IDEMIA Local Daemon (RDServiceL1.exe :11100)     │  │
│  │      - HTTPS / TLS 1.2 Endpoint                       │  │
│  │      - Verbs: RDSERVICE, DEVICEINFO, CAPTURE          │  │
│  └──────────────────────────┬────────────────────────────┘  │
│                             │ C++ Native Calls / SpUsb      │
└─────────────────────────────┼───────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                        Kernel Space                         │
│  ┌───────────────────────────────────────────────────────┐  │
│  │   Morpho Smart USB Driver (MSO_SpUsb_Service)         │  │
│  │   (Subject to Windows 11 HVCI Code Integrity Rules)   │  │
│  └──────────────────────────┬────────────────────────────┘  │
└─────────────────────────────┼───────────────────────────────┘
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                       Hardware Layer                        │
│      IDEMIA MorphoSmart MSO 1300 E3 L1 Optical Scanner      │
│      USB VID_225D / PID_0001 (Aadhaar L1 Certified)         │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. Deep Dive: Protocol Violation Resolver

### The Problem
The official IDEMIA L1 HTTP daemon creates an embedded micro-webserver on `https://127.0.0.1:11100`. In its HTTP response stream, it terminates the status line and headers using single Unix Line Feeds `\n` (`0x0A`) rather than standard Internet RFC CRLF `\r\n` (`0x0D 0x0A`).

When standard .NET classes (`HttpWebRequest`, `HttpClient`, `WebClient`) parse this response, they throw:
```
System.Net.WebException: The underlying connection was closed: The server committed a protocol violation. Section=ResponseStatusLine
```

### The Solution (`SendRdServiceRequestAsync`)
Our custom transport communicates directly using `System.Net.Sockets.TcpClient` and `System.Net.Security.SslStream` with custom certificate validation. It receives raw byte streams and dynamically extracts the XML body across both `\r\n\r\n` and `\n\n` boundary patterns.

```csharp
// Direct SslStream socket transport with Unix LF resilience
using (TcpClient tcp = new TcpClient())
{
    tcp.Connect(host, port);
    using (SslStream ssl = new SslStream(tcp.GetStream(), false, (s, c, ch, err) => true))
    {
        ssl.AuthenticateAsClient(host);
        byte[] req = BuildHttpBuffer(method, path, postData);
        ssl.Write(req, 0, req.Length);
        
        string raw = ReadToEnd(ssl);
        string xml = ExtractXmlBody(raw);
        return FormatXml(xml);
    }
}
```

---

## 3. 32-bit vs 64-bit Architecture Conflict (0x8007000B)

The IDEMIA service binary `RDServiceL1.exe` is a 32-bit (x86) PE executable. When users install generic 64-bit C++ runtime DLLs into the RD Service folder, the 32-bit service fails on launch with `BadImageFormatException` (HRESULT `0x8007000B`).

The AI Engine executes `FixDllArchitecture()`:
1. Cleans conflicting 64-bit runtime DLLs (`msvcp140.dll`, `vcruntime140.dll`, etc.) from `C:\IdemiaL1RdService\RDService\`.
2. Restores clean 32-bit `ucrtbase.dll` from `C:\Windows\SysWOW64\`.
3. Re-registers the service cleanly in the Windows Service Control Manager (SCM).

---

## 4. Hypervisor Code Integrity (HVCI) & Error Code 39

On Windows 11, **Core Isolation / Memory Integrity** enforces kernel-mode code integrity signatures. Older Morpho driver revisions trigger Windows Device Manager **Code 39** (*"Windows cannot load the device driver for this hardware. The driver may be corrupted or missing"*).

The suite provides:
- Automated WMI PnP Entity detection for `Code 39` on `VID_225D`.
- 1-Click Registry modifier for `HypervisorEnforcedCodeIntegrity\Enabled = 0`.
- Direct URI launch to `windowsdefender://coreisolation`.
