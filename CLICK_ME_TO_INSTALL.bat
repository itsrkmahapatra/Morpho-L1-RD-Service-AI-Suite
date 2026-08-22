@echo off
setlocal EnableDelayedExpansion
cd /d "%~dp0"

:: ----------------------------------------------------------------------
:: AUTO ELEVATION TO ADMINISTRATOR
:: ----------------------------------------------------------------------
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo.
    echo ====================================================================
    echo   [!] Administrator rights required. Requesting UAC elevation...
    echo ====================================================================
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

title Morpho IDEMIA L1 RD Service - 1-Click Auto Installer & Verifier
color 0A
cls

echo ====================================================================
echo    MORPHO IDEMIA L1 RD SERVICE - 1-CLICK ALL-IN-ONE INSTALLER
echo ====================================================================
echo.

:: ----------------------------------------------------------------------
:: STEP 1: HARDWARE & SYSTEM CHECK
:: ----------------------------------------------------------------------
echo [1/6] Running System & Hardware Diagnostics...

:: Sync Windows Date & Time (Crucial for UIDAI)
echo   - Syncing Windows Date and Time...
net start w32time >nul 2>&1
w32tm /resync >nul 2>&1

:: Check if Morpho hardware is connected
powershell -NoProfile -Command "$dev = Get-PnpDevice -PresentOnly | Where-Object { $_.FriendlyName -match 'Morpho|MSO' -or $_.InstanceId -match 'VID_225D' }; if ($dev) { Write-Host '  [+] Morpho USB Scanner Detected: ' $dev.FriendlyName -ForegroundColor Green } else { Write-Host '  [!] Warning: Morpho device not plugged in. Please plug it into a USB port.' -ForegroundColor Yellow }"
echo.

:: ----------------------------------------------------------------------
:: STEP 2: CLEAN UP OLD / CONFLICTING DRIVERS & SERVICES
:: ----------------------------------------------------------------------
echo [2/6] Cleaning up any old/conflicting RD Service files...
taskkill /F /T /IM IdemiaL1RDServiceManagement.exe >nul 2>&1
taskkill /F /T /IM RDServiceL1.exe >nul 2>&1
taskkill /F /T /IM IdemiaInteractiveL1RDService.exe >nul 2>&1

net stop "Idemia_L1RD_Service" >nul 2>&1
net stop "MSO_SpUsb_Service" >nul 2>&1

if exist "C:\IdemiaL1RdService\RDService\unins000.exe" (
    start /wait "" "C:\IdemiaL1RdService\RDService\unins000.exe" /VERYSILENT /SUPPRESSMSGBOXES /NORESTART
)

if exist "C:\MORPHORDLOG" rd /s /q "C:\MORPHORDLOG" >nul 2>&1
echo   - Cleanup complete.
echo.

:: ----------------------------------------------------------------------
:: STEP 3: INSTALL REQUIRED RUNTIMES (Visual C++ 2010 & 2015-2022)
:: ----------------------------------------------------------------------
echo [3/6] Installing Required Visual C++ Runtimes...
if exist "%~dp0Prerequisites\vcredist_2010_x86.exe" (
    echo   - Installing Visual C++ 2010 (x86)...
    start /wait "" "%~dp0Prerequisites\vcredist_2010_x86.exe" /q /norestart
)

if exist "%~dp0Prerequisites\VC_redist.x86.exe" (
    echo   - Installing Visual C++ 2015-2022 (x86)...
    start /wait "" "%~dp0Prerequisites\VC_redist.x86.exe" /install /passive /norestart
)

if exist "%~dp0Prerequisites\VC_redist.x64.exe" (
    echo   - Installing Visual C++ 2015-2022 (x64)...
    start /wait "" "%~dp0Prerequisites\VC_redist.x64.exe" /install /passive /norestart
)
echo   - Runtime packages installed.
echo.

:: ----------------------------------------------------------------------
:: STEP 4: INSTALL IDEMIA L1 RD SERVICE & FIX 32-BIT ARCHITECTURE
:: ----------------------------------------------------------------------
echo [4/6] Installing Official IDEMIA L1 RD Service & Driver Setup...
if exist "%~dp0Setup\Windows_L1RDService-HTTPs-LocalHost.exe" (
    start /wait "" "%~dp0Setup\Windows_L1RDService-HTTPs-LocalHost.exe" /VERYSILENT /SUPPRESSMSGBOXES /NORESTART
    echo   - IDEMIA L1 Service files installed.
) else (
    echo   - [ERROR] Setup file not found in Setup directory!
)

:: Fix 64-bit DLL mismatch for 32-bit loader (Fixes BadImageFormatException 0x8007000B)
echo   - Applying 32-bit DLL compatibility fix...
powershell -NoProfile -Command "$bad = @('msvcp100.dll', 'msvcr100.dll', 'msvcp120.dll', 'msvcr120.dll', 'msvcp140.dll', 'vcruntime140.dll', 'msvcp100d.dll', 'msvcp140d.dll', 'msvcp90d.dll', 'msvcr100d.dll', 'msvcrt.dll', 'ucrtbased.dll', 'vcruntime140d.dll'); foreach ($d in $bad) { $p = 'C:\IdemiaL1RdService\RDService\' + $d; if (Test-Path $p) { Remove-Item $p -Force -ErrorAction SilentlyContinue } }"
echo.

:: ----------------------------------------------------------------------
:: STEP 5: CONFIGURE & START SERVICES
:: ----------------------------------------------------------------------
echo [5/6] Starting Morpho & IDEMIA Windows Services...

:: Enable Logging for Diagnostics
if exist "C:\IdemiaL1RdService\RDService\config_settings.xml" (
    powershell -NoProfile -Command "(Get-Content 'C:\IdemiaL1RdService\RDService\config_settings.xml') -replace '<RDEnableLog>0</RDEnableLog>', '<RDEnableLog>1</RDEnableLog>' -replace '<MCEnableLog>0</MCEnableLog>', '<MCEnableLog>1</MCEnableLog>' | Set-Content 'C:\IdemiaL1RdService\RDService\config_settings.xml'"
)

:: Start MorphoSmart USB Service
sc config "MSO_SpUsb_Service" start= auto >nul 2>&1
net start "MSO_SpUsb_Service" >nul 2>&1

:: Create service if needed and start
sc query "Idemia_L1RD_Service" >nul 2>&1
if %errorlevel% neq 0 (
    if exist "C:\IdemiaL1RdService\RDService\IdemiaInteractiveL1RDService.exe" (
        sc create "Idemia_L1RD_Service" binPath= "C:\IdemiaL1RdService\RDService\IdemiaInteractiveL1RDService.exe" start= auto DisplayName= "Idemia L1 RD Service" >nul 2>&1
    )
)
sc config "Idemia_L1RD_Service" start= auto >nul 2>&1
net start "Idemia_L1RD_Service" >nul 2>&1

:: Launch IDEMIA Management Client
if exist "C:\IdemiaL1RdService\RDService\L1RD_Management_Tool\IdemiaL1RDServiceManagement.exe" (
    start "" "C:\IdemiaL1RdService\RDService\L1RD_Management_Tool\IdemiaL1RDServiceManagement.exe"
)

:: Wait 3 seconds for service socket binding
timeout /t 3 /nobreak >nul
echo.

:: ----------------------------------------------------------------------
:: STEP 6: VERIFY INSTALLATION AND TEST
:: ----------------------------------------------------------------------
echo [6/6] Verifying Active RD Service Status...
echo.
echo ====================================================================
powershell -NoProfile -Command "& {
    $usbSvc = Get-Service -Name 'MSO_SpUsb_Service' -ErrorAction SilentlyContinue
    $rdSvc  = Get-Service -Name 'Idemia_L1RD_Service' -ErrorAction SilentlyContinue
    
    if ($usbSvc -and $usbSvc.Status -eq 'Running') {
        Write-Host '  [PASS] MorphoSmart USB Service : RUNNING' -ForegroundColor Green
    } else {
        Write-Host '  [WARN] MorphoSmart USB Service : NOT RUNNING' -ForegroundColor Red
    }
    
    if ($rdSvc -and $rdSvc.Status -eq 'Running') {
        Write-Host '  [PASS] Idemia L1 RD Service    : RUNNING' -ForegroundColor Green
    } else {
        Write-Host '  [WARN] Idemia L1 RD Service    : NOT RUNNING' -ForegroundColor Red
    }
    
    $port = Get-NetTCPConnection -State Listen -LocalPort 11100 -ErrorAction SilentlyContinue
    if ($port) {
        Write-Host '  [PASS] HTTPS Port 11100        : LISTENING' -ForegroundColor Green
    } else {
        Write-Host '  [INFO] HTTPS Port 11100 will activate as soon as Morpho L1 device is initialized.' -ForegroundColor Yellow
    }
}"
echo ====================================================================
echo.
echo  ALL DONE! Opening pre-configured RD Service Test Page...
start "" "%~dp0Test_Page\RD_Test_Page.html"
echo.
echo  Press any key to exit.
pause >nul
