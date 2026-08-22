# Third-Party Notices & Legal Disclaimers

This document contains mandatory third-party software attributions, copyright notices, and legal disclaimers for third-party components utilized, referenced, or bundled alongside the **IDEMIA Morpho L1 RD Service AI Suite**.

---

## 1. IDEMIA Morpho RD Service & Hardware Drivers

- **Component**: IDEMIA Windows L1 RD Service (v5.0.0.10) & USB Drivers (`UsbBioDriver_x64.sys` / `RDServiceL1.exe`)
- **Copyright Owner**: **IDEMIA Identity & Security France** (formerly Morpho / Safran Identity & Security)
- **Official Portal**: [https://rdserviceonline.com/](https://rdserviceonline.com/) | [https://www.idemia.com/](https://www.idemia.com/)
- **Attribution Notice**:
  - `IDEMIA`, `Morpho`, `MorphoSmart`, and `MSO 1300 E3` are registered trademarks of IDEMIA Identity & Security France.
  - The underlying RD Service binaries, driver installers, and cryptographic device firmware are the proprietary intellectual property of IDEMIA.
  - The standalone installer `Windows_L1RDService-HTTPs-LocalHost.exe` bundled for automated offline setup is provided in its original, unmodified binary format to assist system administrators and end users in automated deployment.

---

## 2. Microsoft Visual C++ Redistributables & Windows Runtime

- **Components**:
  - Microsoft Visual C++ 2010 SP1 Redistributable (x86)
  - Microsoft Visual C++ 2015–2022 Redistributable (x86 and x64)
  - Universal C Runtime (`ucrtbase.dll`)
- **Copyright Owner**: **Microsoft Corporation**
- **Official Portal**: [https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist)
- **Attribution Notice**:
  - `Microsoft`, `Windows`, `Visual C++`, and `.NET Framework` are registered trademarks of Microsoft Corporation in the United States and other countries.
  - The Visual C++ Redistributable packages are distributed pursuant to Microsoft Software License Terms for Microsoft Visual Studio.

---

## 3. UIDAI (Unique Identification Authority of India) Conformance

- **Specification**: UIDAI Aadhaar Registered Device (RD) Service Technical Specification v2.0 (L1 Compliance)
- **Authority**: **Unique Identification Authority of India (UIDAI), Government of India**
- **Official Portal**: [https://uidai.gov.in/](https://uidai.gov.in/)
- **Disclaimer**:
  - `Aadhaar` and `UIDAI` are registered trademarks and statutory authorities of the Government of India.
  - This AI Suite is an independent diagnostic, self-healing, and testing utility designed to test and verify conformance with UIDAI public API specifications.
  - This software is **not** created, sponsored, certified, or endorsed by UIDAI or the Government of India.

---

## 4. Proprietary Core Software & Author Attributions

- **Components**:
  - IDEMIA Morpho L1 RD Service AI Suite Desktop Application
  - Autonomous AI Self-Healing Doctor Engine
  - In-App Native Biometric Studio
  - Zero-Violation Direct SslStream Socket Transport
  - Windows 11 Memory Integrity (HVCI Code 39) Resolver
- **Author & Copyright Holder**: **Raj Kishor Mahapatra**
  - Website: [https://itsrkmahapatra.qzz.io/](https://itsrkmahapatra.qzz.io/)
  - Email: [dev@itsrkmahapatra.qzz.io](mailto:dev@itsrkmahapatra.qzz.io)
  - GitHub: [@itsrkmahapatra](https://github.com/itsrkmahapatra)
- **License**: **End-User Restricted License Agreement (EULA)** — Usage permitted. Modification, reverse-engineering, copying, cloning, redistribution, or sale strictly prohibited without prior written permission.

---

## 5. Summary Table

| Component | Author / Owner | Role / Purpose | License / Terms |
| :--- | :--- | :--- | :--- |
| **Morpho L1 AI Suite & Native Studio** | Raj Kishor Mahapatra | Diagnostic, Transport, AI Self-Healing & In-App Testing | Proprietary EULA (Restricted Use) |
| **IDEMIA L1 RD Service v5.0.0.10** | IDEMIA Identity & Security | Hardware daemon, L1 Key Rotation & Capture | IDEMIA Proprietary End-User License |
| **Visual C++ 2010 / 2015-2022** | Microsoft Corporation | Native C/C++ runtime libraries (`msvcr100.dll`, etc.) | Microsoft Redistributable EULA |
| **UIDAI RD Specification v2.0** | UIDAI (Govt. of India) | Technical standard for biometric PID block | Public Technical Specification |
