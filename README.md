# ⭐ Image Compressor — Windows Image Compression Suite
*Right-click → Compress image. Converts, compresses and optimizes images using a Windows service.*

---

## 📌 Overview

**Image Compressor** is a Windows-only image compression system built with:

- **.NET 8**
- **Windows Service (Worker Service)**
- **WinForms UI Popup**
- **Custom EXE Setup Installer with MSI-style Wizard**
- **Bootstrapper Console Installer**
- **Registry-based Context Menu Integration (Right-Click → Compress Image)**

It allows users to right-click any supported image and compress/convert it into a JPEG with optional target size, quality and dimension settings.

---

## 🚀 Features

### 🖼️ Image Compression
- Supports: **JPG, JPEG, PNG, BMP, WEBP**
- Non-JPEG images are automatically **converted to JPEG**
- Removes metadata (EXIF) for smaller file sizes
- Intelligent **binary search** quality adjustment to match target size
- Stores output next to original image with `*_compressed.jpg`

---

### 🔧 Windows Service Backend
- Runs async compression jobs  
- Accepts file path + compression settings from the popup  
- Exposes a local **HTTP API**  
- Logs every operation for debugging  

---

### 📂 WinForms Popup UI
Triggered from context menu; includes:

- **File path** (autofilled)
- **Width / Height** with units (px/cm/inch)
- **Quality**
- **Target File Size (KB)**
- Progress indicator during compression

---

### 🖱️ Windows Context Menu Integration
Adds **Compress Image** to:

- Windows **Classic context menu**
- Windows **11 Modern context menu**

Supports:

- `.jpg`
- `.jpeg`
- `.png`
- `.bmp`
- `.webp`

---

### ⚙️ Custom Installer (Setup.UI)
A modern MSI-style wizard built in WinForms:

- Welcome screen  
- Detect existing installation → **Repair / Remove**  
- Choose installation directory  
- Installation progress (copy files, register service, create shortcuts, registry entries)  
- Finish screen with **Open Manual** checkbox  
- Auto-creates Desktop & Start Menu shortcuts  

---

### ⚙️ Bootstrapper (installer.exe)
A lightweight console installer that:

1. Checks .NET 8 Runtime  
2. Downloads & installs runtime if missing  
3. Extracts embedded Setup UI installer  
4. Launches the installer wizard  

---

## 🔒 Security & Safety
- All image processing is local only
- No external uploads
- Logs stored in `%ProgramData%\ImageCompressor\Logs`
- No telemetry, no tracking

