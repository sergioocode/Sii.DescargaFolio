[![](https://img.shields.io/badge/License-GPLv3-blue.svg?style=for-the-badge)](LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?style=for-the-badge)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/w/sergioocode/Sii.DescargaFolio?style=for-the-badge)](https://github.com/sergioocode/Sii.DescargaFolio)
[![GitHub contributors](https://img.shields.io/github/contributors/sergioocode/Sii.DescargaFolio?style=for-the-badge)](https://github.com/sergioocode/Sii.DescargaFolio/graphs/contributors/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/sergioocode/Sii.DescargaFolio?style=for-the-badge)](https://github.com/sergioocode/Sii.DescargaFolio)

# Sii.DescargaFolio

Download CAF (Certificate of Authorization of Folios) files from Chile's Internal Revenue Service (SII) using authenticated HTTPS requests with a valid digital certificate.

It includes:

- Fluent API: `Authenticate → Prepare → Download`
- Session-aware: avoids redundant authentication
- Built with .NET 8 + ASP.NET Core + IHttpClientFactory
- Designed for backend automation and SII integration

> ⚠️ This project is not affiliated with the official SII. It is intended for private automation and educational use.

---

### 📦 Details

| Package Reference            | Version |
|-----------------------------|:-------:|
| Azure.Storage.Blobs         | 12.24.0 |
| Microsoft.Extensions.Azure  | 1.7.6   |
| AngleSharp.XPath            | 2.0.5   |

---

### 🚀 Usage

```bash
curl -X GET "https://localhost:7212/api/DescargaFolio/caf?rut=88777555-9&tipoDoc=33&folioInicio=2294&folioFin=2415&fecha=30-12-2024" \
     -H "Accept: application/xml"
```

Return XML from controller:
<p align="center">
  <img src="https://img001.prntscr.com/file/img001/qtjoxOjRRTWh9I6nFweOYw.png" width="80%" />
</p>


---
### ⚙️ Configuration

Use `appsettings.json` or environment variables to configure the certificate source:

```json
{
  "StorageConnection": "UseDevelopmentStorage=true",
  "StorageConnection:ContainerName": "certificados",
  "StorageConnection:BlobName": "certificado1.pfx",
  "StorageConnection:CertPassword": "<your-cert-password>"
}
```

You may also define these as [Azure App Settings](https://learn.microsoft.com/en-us/azure/app-service/configure-common) if you're deploying the API to the cloud.

---

### 📢 Have a question? Found a Bug?

Feel free to **file a new issue** with a respective title and description on the [Sii.DescargaFolio/issues](https://github.com/sergioocode/Sii.DescargaFolio/issues) repository.

---

### 💖 Community and Contributions

If this tool was useful, consider contributing with ideas or improving it further.

<p align="center">
    <a href="https://www.paypal.com/donate/?hosted_button_id=PTKX9BNY96SNJ" target="_blank">
        <img width="12%" src="https://img.shields.io/badge/PayPal-00457C?style=for-the-badge&logo=paypal&logoColor=white" alt="Support via PayPal">
    </a>
</p>

---

### 📘 License

This repository is released under the [GNU General Public License v3.0](LICENSE.txt).

