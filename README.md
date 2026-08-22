# IDEMIA Morpho L1 RD Service AI Suite 🖐️⚡

[![Windows 10/11 Compatible](https://img.shields.io/badge/Windows-10%20%7C%2011-0078D6?style=for-the-badge&logo=windows&logoColor=white)](https://itsrkmahapatra.qzz.io/)
[![UIDAI L1 Certified](https://img.shields.io/badge/UIDAI-L1%20Registered%20Device-FF9933?style=for-the-badge&logo=shield&logoColor=white)](https://uidai.gov.in/)
[![.NET Framework 4.8](https://img.shields.io/badge/.NET-Framework%204.8%20%7C%20WPF-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![License: End-User Restricted License](https://img.shields.io/badge/License-EULA%20%7C%20Usage%20Only-EF4444?style=for-the-badge)](LICENSE)
[![Developer: Raj Kishor Mahapatra](https://img.shields.io/badge/Developer-Raj%20Kishor%20Mahapatra-4F46E5?style=for-the-badge)](https://itsrkmahapatra.qzz.io/)

> **Enterprise Grade IDEMIA Morpho MSO 1300 E3 L1 RD Service Automation, Autonomous AI Self-Healing Engine, Memory Integrity (Core Isolation) Resolver, and In-App Native Biometric Studio.**

---

## 📥 Direct Downloads

- 🚀 **[Download Morpho L1 RD Service Pro Suite (.exe)](https://github.com/itsrkmahapatra/Morpho-L1-RD-Service-AI-Suite/raw/main/MorphoL1RDServicePro.exe)** (~63 MB, All-in-One Standalone)
- ⚡ **[Download Morpho L1 Service Manager (.exe)](https://github.com/itsrkmahapatra/Morpho-L1-RD-Service-AI-Suite/raw/main/MorphoL1RDServiceManager.exe)** (~76 KB, Ultra-Lightweight)
- 🌐 **[Live Showcase Website & Interactive Simulator](https://itsrkmahapatra.github.io/Morpho-L1-RD-Service-AI-Suite/)**
- 🔬 **[System Architecture Documentation](ARCHITECTURE.md)**

---

## 🌟 Key Highlights

- 🚀 **1-Click All-in-One Automated Setup**: Automatically extracts embedded packages, syncs NTP system time, installs Visual C++ 2010 & 2015-2022 runtimes (x86 & x64), installs IDEMIA v5.0.0.10 HTTPS RD Service, and registers Windows Services.
- 🖐️ **In-App Native Biometric Studio**: Complete built-in biometric capture and testing suite without needing a web browser or external test page. Directly test `RDSERVICE`, `DEVICEINFO`, and trigger optical `CAPTURE` inside the app with real-time Quality Score and encrypted `PidData` XML generation.
- 🤖 **Autonomous AI Self-Healing Doctor**: Continuously monitors hardware connection, driver health, 64-bit DLL collisions, and automatically starts stopped services.
- 🛡️ **Memory Integrity (Code 39) Resolver**: 1-click detection and guide to fix Windows 11 Core Isolation / Hypervisor Code Integrity (HVCI) driver blockage (`Code 39`).
- ⚡ **Zero-Dependency Resilient Transport**: Custom high-speed TLS/SSL stream socket client that solves IDEMIA HTTP daemon Unix newline (`\n`) protocol violations, supporting custom UIDAI verbs (`RDSERVICE`, `DEVICEINFO`, `CAPTURE`).

---

## 📸 In-App Native Biometric Studio

The Native Biometric Studio communicates directly with the local IDEMIA RD Service HTTPS daemon (`https://127.0.0.1:11100`) via low-level TLS socket transport:

1. **⚡ 1. RDSERVICE**: Discovers RD Service readiness, status (`READY` / `NOTREADY`), and service info.
2. **📋 2. DEVICE INFO**: Retrieves device metadata including Serial Number (e.g., `2438I050674`), RDS ID, Model, Certificate Expiry, and Modality.
3. **🔴 3. CAPTURE FINGERPRINT**: Sends custom `<PidOptions>` XML, activates optical scanner sensor, and captures encrypted `PidData` with real-time biometric Quality Score (`qScore`).
4. **📑 Copy XML & 💾 Save XML**: One-click clipboard copy or save payload directly to disk for Aadhaar authentication pipelines.

---

## 📁 Repository Structure

```
├── Source/
│   ├── Program.cs             # C# WPF Complete Application Source Code
│   ├── app.ico                # Multi-Resolution Vector Application Icon
│   ├── logo.png               # High-Resolution Application Emblem
│   └── app.manifest           # Windows UAC Elevation & OS Compatibility Manifest
├── Prerequisites/
│   ├── VC_redist.x86.exe      # Visual C++ 2015-2022 Redistributable (32-bit)
│   ├── VC_redist.x64.exe      # Visual C++ 2015-2022 Redistributable (64-bit)
│   └── vcredist_2010_x86.exe  # Visual C++ 2010 SP1 Redistributable (32-bit)
├── Setup/
│   └── Windows_L1RDService-HTTPs-LocalHost.exe  # Official IDEMIA L1 Setup
├── Test_Page/
│   └── RD_Test_Page.html      # Browser-based test page (optional fallback)
├── website/                   # Modern Showcase Website with Simulator & Full SEO
│   ├── index.html
│   ├── style.css
│   ├── app.js
│   ├── favicon.ico
│   └── logo.png
├── docs/                      # GitHub Pages documentation & live demo
│   ├── index.html
│   ├── style.css
│   ├── app.js
│   ├── favicon.ico
│   └── logo.png
├── MorphoL1RDServiceManager.exe   # Lightweight Executable (~76 KB)
├── MorphoL1RDServicePro.exe       # Self-Contained All-In-One Pro Suite (~63 MB)
├── CLICK_ME_TO_INSTALL.bat       # Silent batch installation runner
├── ARCHITECTURE.md            # In-depth architectural & protocol design
├── API_SPECIFICATION.md       # UIDAI RD Service v2.0 API Reference
├── LICENSE                    # End-User Restricted License Agreement (EULA)
└── README.md                  # Main Documentation
```

---

## 🛠️ Installation & Quick Start

### Option A: Self-Contained Pro Executable (Recommended)
1. Download or launch [`MorphoL1RDServicePro.exe`](https://github.com/itsrkmahapatra/Morpho-L1-RD-Service-AI-Suite/raw/main/MorphoL1RDServicePro.exe).
2. Click **"🚀 START 1-CLICK ALL-IN-ONE INSTALLATION & REPAIR"**.
3. The application will silently extract dependencies, install runtimes, configure services, and open the Native Biometric Studio.

### Option B: Command Line / Batch Script
Right-click [`CLICK_ME_TO_INSTALL.bat`](file:///c:/Users/rajki/OneDrive/Desktop/installer%20L1/CLICK_ME_TO_INSTALL.bat) and select **"Run as Administrator"**.

---

## 🔬 Compilation Instructions

To compile from source with embedded application icon using Microsoft .NET Framework C# Compiler (`csc.exe`):

### Compile Lightweight Manager:
```cmd
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /win32manifest:Source\app.manifest /win32icon:Source\app.ico /out:MorphoL1RDServiceManager.exe /reference:System.dll,System.Core.dll,System.Data.dll,System.Drawing.dll,System.Management.dll,System.ServiceProcess.dll,System.Xml.dll,System.Xml.Linq.dll,System.Xaml.dll,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\WindowsBase.dll","C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationCore.dll","C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationFramework.dll" Source\Program.cs
```

### Compile Self-Contained Pro Suite:
```cmd
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /win32manifest:Source\app.manifest /win32icon:Source\app.ico /out:MorphoL1RDServicePro.exe /resource:Setup\Windows_L1RDService-HTTPs-LocalHost.exe /resource:Prerequisites\VC_redist.x86.exe /resource:Prerequisites\VC_redist.x64.exe /resource:Prerequisites\vcredist_2010_x86.exe /reference:System.dll,System.Core.dll,System.Data.dll,System.Drawing.dll,System.Management.dll,System.ServiceProcess.dll,System.Xml.dll,System.Xml.Linq.dll,System.Xaml.dll,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\WindowsBase.dll","C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationCore.dll","C:\Windows\Microsoft.NET\Framework64\v4.0.30319\WPF\PresentationFramework.dll" Source\Program.cs
```

---

## 🩺 Troubleshooting Common Issues

| Error / Symptom | Root Cause | AI Suite Automatic Solution |
| :--- | :--- | :--- |
| **Section=ResponseStatusLine Protocol Violation** | IDEMIA HTTP Daemon uses Unix `\n` line endings instead of `\r\n`. | Custom `SendRdServiceRequestAsync` socket transport handles raw byte streams and both line-ending formats. |
| **BadImageFormatException (0x8007000B)** | 64-bit Visual C++ DLLs copied into 32-bit RD Service folder. | `FixDllArchitecture()` cleans conflicting DLLs and deploys 32-bit `ucrtbase.dll`. |
| **Driver Error Code 39** | Windows 11 Core Isolation (HVCI) blocks unsigned/legacy kernel driver. | Memory Integrity tab provides 1-click registry patch or direct Core Isolation settings shortcut. |
| **Port 11100 Offline** | Services stopped or certificate missing. | Autonomous AI Doctor auto-restarts services and re-registers localhost root certificates. |

---

## 📜 Specifications & Compliance

- **UIDAI Registered Device Specification**: v2.0 (L1 Compliance)
- **Supported Hardware**: IDEMIA / MorphoSmart MSO 1300 E3 L1 USB Fingerprint Scanner
- **Target OS**: Windows 10 (32-bit & 64-bit), Windows 11 (all builds)
- **Protocol**: HTTPS / TLS 1.2 on `127.0.0.1:11100` (dynamic port fallback 11100..11120)

---

## 👨‍💻 Author & Maintainer

**Raj Kishor Mahapatra**  
- 🌐 Website: [https://itsrkmahapatra.qzz.io/](https://itsrkmahapatra.qzz.io/)  
- 📧 Email: [dev@itsrkmahapatra.qzz.io](mailto:dev@itsrkmahapatra.qzz.io)  
- 🐙 GitHub: [@itsrkmahapatra](https://github.com/itsrkmahapatra)

---

## 📄 License & Terms of Use

Copyright © 2026 **Raj Kishor Mahapatra**. All Rights Reserved.

This software is governed by the **End-User Restricted License Agreement (EULA)**:
- **Usage Permitted**: Users and organizations are permitted to download, install, and execute the Software for personal, business, banking, CSP kiosk, and biometric authentication operations.
- **Restrictions**: Modifying, decompiling, reverse engineering, copying, cloning, redistributing, or selling the Software or its source code without prior express written authorization from Raj Kishor Mahapatra is strictly prohibited.

See the full [LICENSE](LICENSE) file for complete legal terms.

---

## ⚖️ Third-Party Notices & Trademarks

- **IDEMIA**: `IDEMIA`, `Morpho`, `MorphoSmart`, and `MSO 1300 E3` are trademarks of **IDEMIA Identity & Security France**. The underlying driver installers and firmware are proprietary to IDEMIA.
- **Microsoft**: `Microsoft`, `Visual C++`, `Windows`, and `.NET Framework` are registered trademarks of **Microsoft Corporation**.
- **UIDAI**: `UIDAI` and `Aadhaar` are registered trademarks and statutory authorities of the **Unique Identification Authority of India (Government of India)**.
- **Non-Affiliation Disclaimer**: This project is an independent diagnostic and self-healing suite authored by Raj Kishor Mahapatra and is not affiliated with, endorsed by, or sponsored by IDEMIA, Microsoft, or UIDAI.

For complete third-party licensing terms and attributions, see [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
