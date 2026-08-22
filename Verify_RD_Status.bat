@echo off
setlocal
cd /d "%~dp0"
title Morpho L1 RD Service - Real-Time Status Verifier
color 0B
cls

echo ====================================================================
echo         MORPHO L1 RD SERVICE - LIVE STATUS VERIFIER
echo ====================================================================
echo.

powershell -NoProfile -Command "& {
    Write-Host '1. HARDWARE STATUS:' -ForegroundColor Cyan
    $dev = Get-PnpDevice -PresentOnly | Where-Object { $_.FriendlyName -match 'Morpho|MSO' -or $_.InstanceId -match 'VID_225D' }
    if ($dev) {
        Write-Host '   [OK] Connected: ' $dev.FriendlyName -ForegroundColor Green
    } else {
        Write-Host '   [X] Morpho Device Not Detected. Please check USB cable.' -ForegroundColor Red
    }
    
    Write-Host '`n2. WINDOWS SERVICES:' -ForegroundColor Cyan
    $usbSvc = Get-Service -Name 'MSO_SpUsb_Service' -ErrorAction SilentlyContinue
    $rdSvc  = Get-Service -Name 'Idemia_L1RD_Service' -ErrorAction SilentlyContinue
    
    if ($usbSvc) {
        Write-Host ('   - MorphoSmart USB Service : ' + $usbSvc.Status) -ForegroundColor ($usbSvc.Status -eq 'Running' ? 'Green' : 'Red')
    } else {
        Write-Host '   - MorphoSmart USB Service : NOT INSTALLED' -ForegroundColor Red
    }
    
    if ($rdSvc) {
        Write-Host ('   - Idemia L1 RD Service    : ' + $rdSvc.Status) -ForegroundColor ($rdSvc.Status -eq 'Running' ? 'Green' : 'Red')
    } else {
        Write-Host '   - Idemia L1 RD Service    : NOT INSTALLED' -ForegroundColor Red
    }
    
    Write-Host '`n3. COMMUNICATION PORT (11100):' -ForegroundColor Cyan
    $port = Get-NetTCPConnection -State Listen | Where-Object { $_.LocalPort -ge 11100 -and $_.LocalPort -le 11120 }
    if ($port) {
        Write-Host ('   [OK] Listening on Port: ' + ($port | Select-Object -ExpandProperty LocalPort -First 1)) -ForegroundColor Green
    } else {
        Write-Host '   [!] Port 11100 not open yet (waiting for device initialization).' -ForegroundColor Yellow
    }
}"

echo.
echo ====================================================================
echo Opening Test Page...
start "" "%~dp0Test_Page\RD_Test_Page.html"
echo.
pause
