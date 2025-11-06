# SQL Server 2022 RDLC PDF Renderer (Digitally Signed & Password Protected)

A custom **SQL Server Reporting Services (SSRS) 2022 rendering extension** that allows exporting RDLC reports to **digitally signed** and/or **password-protected PDFs**, powered by **iTextSharp** and .NET Framework 4.8.

---

## ✳️ Requirements

* **SQL Server Standard/Enterprise 2022**
* **SQL Server Reporting Services (SSRS) 2022**
* **.NET Framework 4.8**
  👉 [Download .NET 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)
* **Digital Signature Certificate** (local or from DigiCert / GlobalSign)

---

## 🔐 Digital Certificate Setup

To enable **digitally signed PDFs**, obtain or create a digital certificate:

* Recommended key length: **2048 bits**
* Import the certificate into the **Windows Certificate Store**
* Note: Self-signed/local certificates **won’t be trusted by PDF readers**

---

## ⚙️ Deployment Guide

### 1. Copy Assemblies

Place the following DLLs in your SSRS **ReportServer\bin** directory:

``` 
SSRS2022PDFRenderer.dll  
```

Default path:

```
%ProgramFiles%\Microsoft SQL Server Reporting Services\SSRS\ReportServer\bin
```

---

### 2. Configure Security Policy

Edit:

```
rssrvpolicy.config
```

Location:

```
%ProgramFiles%\Microsoft SQL Server Reporting Services\SSRS\ReportServer\
```

Add these entries **above the second-last** `</CodeGroup>`:

#### ➤ Password Protected PDF

```xml
<CodeGroup class="UnionCodeGroup" version="1" PermissionSetName="FullTrust"
           Name="PPPDF" Description="Code group for Password Protected PDF">
  <IMembershipCondition class="UrlMembershipCondition" version="1"
    Url="C:\Program Files\Microsoft SQL Server Reporting Services\SSRS\ReportServer\bin\SSRS2022PDFRenderer.dll" />
</CodeGroup>
```

#### ➤ Digitally Signed PDF

```xml
<CodeGroup class="UnionCodeGroup" version="1" PermissionSetName="FullTrust"
           Name="DSPDF" Description="Code group for Digitally Signed PDF">
  <IMembershipCondition class="UrlMembershipCondition" version="1"
    Url="C:\Program Files\Microsoft SQL Server Reporting Services\SSRS\ReportServer\bin\SSRS2022PDFRenderer.dll" />
</CodeGroup>
```

---

### 3. Configure Render Extensions

Edit:

```
rsreportserver.config
```

Add **above `</Render>`**:

#### ➤ Password Protected PDF

```xml
<Extension Name="PPPDF" Type="PDFCustom.CustomPDFProvider, SSRS2022PDFRenderer">
  <OverrideNames>
    <Name Language="en-US">Password Protected PDF</Name>
  </OverrideNames>
  <Configuration>
    <DeviceInfo>
      <PasswordProtected>True</PasswordProtected>
      <CertificatePath>E:/Certificates/Password Protected/signature.pfx</CertificatePath>
      <CertificatePassword>yourcertpassword</CertificatePassword>
      <PDFOwnerPassword>yourpdfpassword</PDFOwnerPassword>
      <AllowPrint>True</AllowPrint>
      <AllowDocumentAssembly>False</AllowDocumentAssembly>
      <AllowContentCopying>True</AllowContentCopying>
      <LoggingFileLocation>E:/Password Protected PDF/Logs/Password_Protected.txt</LoggingFileLocation>
    </DeviceInfo>
  </Configuration>
</Extension>
```

#### ➤ Digitally Signed PDF

```xml
<Extension Name="DSPDF" Type="PDFCustom.CustomPDFProvider, SSRS2022PDFRenderer">
  <OverrideNames>
    <Name Language="en-US">Digitally Signed PDF</Name>
  </OverrideNames>
  <Configuration>
    <DeviceInfo>
      <PasswordProtected>False</PasswordProtected>
      <CertificatePath>E:/Certificates/Digitally Signed/signature.pfx</CertificatePath>
      <CertificatePassword>yourcertpassword</CertificatePassword>
      <PDFOwnerPassword>yourpdfpassword</PDFOwnerPassword>
      <AllowPrint>True</AllowPrint>
      <AllowContentCopying>True</AllowContentCopying>
      <LoggingFileLocation>E:/Digitally Signed PDF/Logs/Digitally_Signed.txt</LoggingFileLocation>
    </DeviceInfo>
  </Configuration>
</Extension>
```

---

### 4. Web Config

Edit:

```
web.config
```

Replace:

```xml
<trust level="RosettaSrv" originUrl="" />
```

With:

```xml
<trust level="Full" originUrl="" />
```

Restart **Reporting Services** after saving changes.

---

### 5. Global Assembly Cache (GAC)

If required, register assemblies using:

```bash
gacutil.exe /if "C:\path\to\SSRS2022PDFRenderer.dll"
```

---

## 🔧 Assembly Configuration

Use **ILSpy** or a similar tool to confirm:

```xml
<AssemblyName>Microsoft.ReportingServices.ImageRendering, Version=2022.1.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91</AssemblyName>
```

---

## 🧾 Default PDF Restrictions

| Feature               | Password Protected | Digitally Signed |
| --------------------- | ------------------ | ---------------- |
| Security Method       | Password           | None             |
| Printing              | ✅ Allowed          | ✅ Allowed        |
| Document Assembly     | ❌ Not Allowed      | ❌ Not Allowed    |
| Content Copying       | ✅ Allowed          | ✅ Allowed        |
| Accessibility Copying | ✅ Allowed          | ✅ Allowed        |
| Commenting            | ✅ Allowed          | ✅ Allowed        |
| Form Filling          | ✅ Allowed          | ✅ Allowed        |

---

## 🧑‍💻 Author

**Uzma Ashraf**

---

## ⚖️ License

Licensed under the **GNU Affero General Public License v3 (AGPLv3)**.
See the [LICENSE](./LICENSE) file for details.

---

Would you like me to make this same README in **GitHub Markdown with table formatting and badges** (e.g., “Built for SSRS 2022”, “AGPLv3 License”, “.NET Framework 4.8”)?
It will make your GitHub page look professional and visually appealing.
