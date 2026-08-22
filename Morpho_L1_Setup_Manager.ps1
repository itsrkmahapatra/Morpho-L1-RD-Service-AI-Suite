Add-Type -AssemblyName PresentationFramework, PresentationCore, WindowsBase, System.Drawing, System.Windows.Forms

# Ensure elevated administrator privileges
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Start-Process powershell.exe -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs
    exit
}

$scriptDir = Split-Path -Parent $PSCommandPath
if (-not $scriptDir) { $scriptDir = "C:\Users\rajki\OneDrive\Desktop\installer L1" }

[xml]$xaml = @"
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="IDEMIA Morpho L1 RD Service - Control Center" Height="680" Width="880"
        WindowStartupLocation="CenterScreen" Background="#0F172A" ResizeMode="CanMinimize">
    <Window.Resources>
        <Style TargetType="Button">
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="FontWeight" Value="SemiBold"/>
            <Setter Property="FontSize" Value="14"/>
            <Setter Property="Padding" Value="15,10"/>
            <Setter Property="Cursor" Value="Hand"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border Background="{TemplateBinding Background}" CornerRadius="8" Padding="{TemplateBinding Padding}">
                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
    </Window.Resources>

    <Grid Margin="24">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Header -->
        <Border Grid.Row="0" Background="#1E293B" CornerRadius="12" Padding="20,16" Margin="0,0,0,16">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>
                <StackPanel Grid.Column="0">
                    <TextBlock Text="IDEMIA Morpho MSO 1300 E3 L1" FontSize="22" FontWeight="Bold" Foreground="#F8FAFC"/>
                    <TextBlock Text="RD Service Setup Manager &amp; Diagnostic Utility" FontSize="13" Foreground="#94A3B8" Margin="0,4,0,0"/>
                </StackPanel>
                <Border Grid.Column="1" Background="#334155" CornerRadius="20" Padding="12,6" VerticalAlignment="Center">
                    <TextBlock Text="v5.0.0.10 HTTPS" FontSize="12" FontWeight="Bold" Foreground="#38BDF8"/>
                </Border>
            </Grid>
        </Border>

        <!-- Status Cards -->
        <Grid Grid.Row="1" Margin="0,0,0,16">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <!-- Card 1: Hardware -->
            <Border Grid.Column="0" Background="#1E293B" CornerRadius="10" Padding="14" Margin="0,0,8,0">
                <StackPanel>
                    <TextBlock Text="USB HARDWARE" FontSize="11" FontWeight="Bold" Foreground="#64748B"/>
                    <TextBlock Name="txtHwStatus" Text="Checking..." FontSize="14" FontWeight="Bold" Foreground="#F59E0B" Margin="0,6,0,0"/>
                    <TextBlock Name="txtHwName" Text="Scanning USB ports" FontSize="11" Foreground="#94A3B8" TextTrimming="CharacterEllipsis"/>
                </StackPanel>
            </Border>

            <!-- Card 2: VC++ Runtimes -->
            <Border Grid.Column="1" Background="#1E293B" CornerRadius="10" Padding="14" Margin="4,0,4,0">
                <StackPanel>
                    <TextBlock Text="VC++ RUNTIMES" FontSize="11" FontWeight="Bold" Foreground="#64748B"/>
                    <TextBlock Name="txtVcStatus" Text="Checking..." FontSize="14" FontWeight="Bold" Foreground="#F59E0B" Margin="0,6,0,0"/>
                    <TextBlock Text="32-bit &amp; 64-bit CRT" FontSize="11" Foreground="#94A3B8"/>
                </StackPanel>
            </Border>

            <!-- Card 3: Windows Services -->
            <Border Grid.Column="2" Background="#1E293B" CornerRadius="10" Padding="14" Margin="4,0,4,0">
                <StackPanel>
                    <TextBlock Text="RD SERVICES" FontSize="11" FontWeight="Bold" Foreground="#64748B"/>
                    <TextBlock Name="txtSvcStatus" Text="Checking..." FontSize="14" FontWeight="Bold" Foreground="#F59E0B" Margin="0,6,0,0"/>
                    <TextBlock Text="Idemia + MorphoUSB" FontSize="11" Foreground="#94A3B8"/>
                </StackPanel>
            </Border>

            <!-- Card 4: Port 11100 -->
            <Border Grid.Column="3" Background="#1E293B" CornerRadius="10" Padding="14" Margin="8,0,0,0">
                <StackPanel>
                    <TextBlock Text="HTTPS PORT" FontSize="11" FontWeight="Bold" Foreground="#64748B"/>
                    <TextBlock Name="txtPortStatus" Text="Checking..." FontSize="14" FontWeight="Bold" Foreground="#F59E0B" Margin="0,6,0,0"/>
                    <TextBlock Text="127.0.0.1:11100" FontSize="11" Foreground="#94A3B8"/>
                </StackPanel>
            </Border>
        </Grid>

        <!-- Action Buttons -->
        <Grid Grid.Row="2" Margin="0,0,0,16">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="2*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <Button Name="btnAutoInstall" Grid.Column="0" Content="🚀  1-Click Full Install &amp; Fix" Background="#4F46E5" Margin="0,0,8,0"/>
            <Button Name="btnRestartSvc" Grid.Column="1" Content="🔄 Restart Services" Background="#0284C7" Margin="4,0,4,0"/>
            <Button Name="btnFixDll" Grid.Column="2" Content="🛠️ Fix DLL Errors" Background="#059669" Margin="4,0,4,0"/>
            <Button Name="btnTestPage" Grid.Column="3" Content="🧪 Open Test Page" Background="#D97706" Margin="8,0,0,0"/>
        </Grid>

        <!-- Log Output Box -->
        <Border Grid.Row="3" Background="#020617" CornerRadius="10" BorderBrush="#334155" BorderThickness="1" Padding="14">
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="*"/>
                </Grid.RowDefinitions>
                <TextBlock Grid.Row="0" Text="REAL-TIME INSTALLATION &amp; DIAGNOSTIC LOG" FontSize="11" FontWeight="Bold" Foreground="#64748B" Margin="0,0,0,8"/>
                <TextBox Name="txtLogs" Grid.Row="1" Background="Transparent" Foreground="#38BDF8" FontFamily="Consolas" FontSize="12"
                         BorderThickness="0" TextWrapping="Wrap" VerticalScrollBarVisibility="Auto" IsReadOnly="True"/>
            </Grid>
        </Border>

        <!-- Footer -->
        <Grid Grid.Row="4" Margin="0,12,0,0">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            <TextBlock Grid.Column="0" Text="UIDAI Aadhaar L1 Registered Device Compliant" FontSize="11" Foreground="#64748B" VerticalAlignment="Center"/>
            <Button Name="btnRefresh" Grid.Column="1" Content="🔄 Refresh Status" Background="#334155" FontSize="12" Padding="12,6"/>
        </Grid>
    </Grid>
</Window>
"@

$reader = (New-Object System.Xml.XmlNodeReader $xaml)
$window = [Windows.Markup.XamlReader]::Load($reader)

# Controls
$txtHwStatus   = $window.FindName("txtHwStatus")
$txtHwName     = $window.FindName("txtHwName")
$txtVcStatus   = $window.FindName("txtVcStatus")
$txtSvcStatus  = $window.FindName("txtSvcStatus")
$txtPortStatus = $window.FindName("txtPortStatus")
$txtLogs       = $window.FindName("txtLogs")
$btnAutoInstall= $window.FindName("btnAutoInstall")
$btnRestartSvc = $window.FindName("btnRestartSvc")
$btnFixDll     = $window.FindName("btnFixDll")
$btnTestPage   = $window.FindName("btnTestPage")
$btnRefresh    = $window.FindName("btnRefresh")

function Log($msg) {
    $time = Get-Date -Format "HH:mm:ss"
    $txtLogs.AppendText("[$time] $msg`r`n")
    $txtLogs.ScrollToEnd()
    [System.Windows.Forms.Application]::DoEvents()
}

function Update-Status {
    # 1. Hardware Check
    $dev = Get-PnpDevice -PresentOnly -ErrorAction SilentlyContinue | Where-Object { $_.FriendlyName -match 'Morpho|MSO' -or $_.InstanceId -match 'VID_225D' }
    if ($dev) {
        $txtHwStatus.Text = "CONNECTED"
        $txtHwStatus.Foreground = [Windows.Media.Brushes]::LimeGreen
        $txtHwName.Text = $dev.FriendlyName
    } else {
        $txtHwStatus.Text = "NOT CONNECTED"
        $txtHwStatus.Foreground = [Windows.Media.Brushes]::Crimson
        $txtHwName.Text = "Plug in USB device"
    }

    # 2. VC++ Runtimes Check
    $vcInstalled = (Test-Path "C:\Windows\SysWOW64\msvcp140.dll") -or (Test-Path "C:\Windows\SysWOW64\ucrtbase.dll")
    if ($vcInstalled) {
        $txtVcStatus.Text = "READY"
        $txtVcStatus.Foreground = [Windows.Media.Brushes]::LimeGreen
    } else {
        $txtVcStatus.Text = "MISSING"
        $txtVcStatus.Foreground = [Windows.Media.Brushes]::Crimson
    }

    # 3. Services Check
    $s1 = Get-Service -Name "MSO_SpUsb_Service" -ErrorAction SilentlyContinue
    $s2 = Get-Service -Name "Idemia_L1RD_Service" -ErrorAction SilentlyContinue
    if ($s1 -and $s2 -and $s1.Status -eq "Running" -and $s2.Status -eq "Running") {
        $txtSvcStatus.Text = "RUNNING"
        $txtSvcStatus.Foreground = [Windows.Media.Brushes]::LimeGreen
    } elseif ($s1 -and $s1.Status -eq "Running") {
        $txtSvcStatus.Text = "PARTIAL"
        $txtSvcStatus.Foreground = [Windows.Media.Brushes]::Gold
    } else {
        $txtSvcStatus.Text = "STOPPED"
        $txtSvcStatus.Foreground = [Windows.Media.Brushes]::Crimson
    }

    # 4. Port 11100 Check
    $port = Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue | Where-Object { $_.LocalPort -ge 11100 -and $_.LocalPort -le 11120 }
    if ($port) {
        $txtPortStatus.Text = "ONLINE (" + ($port | Select-Object -ExpandProperty LocalPort -First 1) + ")"
        $txtPortStatus.Foreground = [Windows.Media.Brushes]::LimeGreen
    } else {
        $txtPortStatus.Text = "OFFLINE"
        $txtPortStatus.Foreground = [Windows.Media.Brushes]::SlateGray
    }
}

function Fix-DllArchitectureMismatch {
    Log "Scanning and repairing 32-bit native libraries..."
    $bad = @('msvcp100.dll', 'msvcr100.dll', 'msvcp120.dll', 'msvcr120.dll', 'msvcp140.dll', 'vcruntime140.dll', 'msvcp100d.dll', 'msvcp140d.dll', 'msvcp90d.dll', 'msvcr100d.dll', 'msvcrt.dll', 'ucrtbased.dll', 'vcruntime140d.dll')
    foreach ($d in $bad) {
        $p = "C:\IdemiaL1RdService\RDService\$d"
        if (Test-Path $p) {
            Remove-Item $p -Force -ErrorAction SilentlyContinue
            Log " - Removed conflicting 64-bit library: $d"
        }
    }
    # Copy clean 32-bit ucrtbase if needed
    if (Test-Path "C:\Windows\SysWOW64\ucrtbase.dll") {
        Copy-Item "C:\Windows\SysWOW64\ucrtbase.dll" "C:\IdemiaL1RdService\RDService\ucrtbase.dll" -Force -ErrorAction SilentlyContinue
    }
    Log "[OK] Architecture compatibility fix applied (BadImageFormatException resolved)."
}

# Button Click: 1-Click Auto Install
$btnAutoInstall.Add_Click({
    $btnAutoInstall.IsEnabled = $false
    Log "=========================================================="
    Log "STARTING AUTOMATIC SETUP & CONFIGURATION FOR MORPHO L1..."
    Log "=========================================================="
    
    # 1. Sync Time
    Log "[1/6] Synchronizing Windows Clock with Internet Time..."
    Start-Service w32time -ErrorAction SilentlyContinue
    w32tm /resync | Out-Null
    
    # 2. Cleanup old tasks
    Log "[2/6] Terminating lingering processes and stopping services..."
    taskkill /F /T /IM IdemiaL1RDServiceManagement.exe 2>$null | Out-Null
    taskkill /F /T /IM RDServiceL1.exe 2>$null | Out-Null
    taskkill /F /T /IM IdemiaInteractiveL1RDService.exe 2>$null | Out-Null
    net stop "Idemia_L1RD_Service" 2>$null | Out-Null
    net stop "MSO_SpUsb_Service" 2>$null | Out-Null
    
    # 3. Install Runtimes
    Log "[3/6] Installing Microsoft Visual C++ Runtimes..."
    $vcX86 = "$scriptDir\Prerequisites\VC_redist.x86.exe"
    $vcX64 = "$scriptDir\Prerequisites\VC_redist.x64.exe"
    $vc2010 = "$scriptDir\Prerequisites\vcredist_2010_x86.exe"
    
    if (Test-Path $vcX86) {
        Log " - Installing Visual C++ 2015-2022 (x86)..."
        Start-Process -FilePath $vcX86 -ArgumentList "/install /passive /norestart" -Wait
    }
    if (Test-Path $vcX64) {
        Log " - Installing Visual C++ 2015-2022 (x64)..."
        Start-Process -FilePath $vcX64 -ArgumentList "/install /passive /norestart" -Wait
    }
    if (Test-Path $vc2010) {
        Log " - Installing Visual C++ 2010 (x86)..."
        Start-Process -FilePath $vc2010 -ArgumentList "/q /norestart" -Wait -ErrorAction SilentlyContinue
    }
    Log "[OK] Visual C++ runtimes installed."
    
    # 4. Install RD Service
    Log "[4/6] Installing IDEMIA Morpho L1 RD Service & Driver Setup..."
    $setupExe = "$scriptDir\Setup\Windows_L1RDService-HTTPs-LocalHost.exe"
    if (Test-Path $setupExe) {
        Start-Process -FilePath $setupExe -ArgumentList "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART" -Wait
        Log "[OK] RD Service Setup completed."
    } else {
        Log "[ERROR] Setup file not found in Setup directory!"
    }
    
    # 5. Fix DLL conflicts
    Log "[5/6] Applying DLL Fixes & Enabling Diagnostics..."
    Fix-DllArchitectureMismatch
    
    if (Test-Path "C:\IdemiaL1RdService\RDService\config_settings.xml") {
        (Get-Content "C:\IdemiaL1RdService\RDService\config_settings.xml") -replace '<RDEnableLog>0</RDEnableLog>', '<RDEnableLog>1</RDEnableLog>' -replace '<MCEnableLog>0</MCEnableLog>', '<MCEnableLog>1</MCEnableLog>' | Set-Content "C:\IdemiaL1RdService\RDService\config_settings.xml"
    }
    
    # 6. Start Services
    Log "[6/6] Starting Windows Services & Launching IDEMIA Tool..."
    sc.exe config "MSO_SpUsb_Service" start= auto | Out-Null
    net start "MSO_SpUsb_Service" 2>$null | Out-Null
    
    $check = sc.exe query "Idemia_L1RD_Service"
    if ($LASTEXITCODE -ne 0 -and (Test-Path "C:\IdemiaL1RdService\RDService\IdemiaInteractiveL1RDService.exe")) {
        sc.exe create "Idemia_L1RD_Service" binPath= "C:\IdemiaL1RdService\RDService\IdemiaInteractiveL1RDService.exe" start= auto DisplayName= "Idemia L1 RD Service" | Out-Null
    }
    sc.exe config "Idemia_L1RD_Service" start= auto | Out-Null
    net start "Idemia_L1RD_Service" 2>$null | Out-Null
    
    if (Test-Path "C:\IdemiaL1RdService\RDService\L1RD_Management_Tool\IdemiaL1RDServiceManagement.exe") {
        Start-Process -FilePath "C:\IdemiaL1RdService\RDService\L1RD_Management_Tool\IdemiaL1RDServiceManagement.exe" -WorkingDirectory "C:\IdemiaL1RdService\RDService\L1RD_Management_Tool"
    }
    
    Start-Sleep -Seconds 3
    Update-Status
    Log "=========================================================="
    Log "SUCCESS: Morpho L1 RD Service is fully configured!"
    Log "Opening Test Page in your browser..."
    Log "=========================================================="
    
    $testPage = "$scriptDir\Test_Page\RD_Test_Page.html"
    if (Test-Path $testPage) {
        Start-Process $testPage
    }
    
    $btnAutoInstall.IsEnabled = $true
})

# Button: Restart Services
$btnRestartSvc.Add_Click({
    Log "Restarting Morpho RD Services..."
    net stop "Idemia_L1RD_Service" 2>$null | Out-Null
    net stop "MSO_SpUsb_Service" 2>$null | Out-Null
    Start-Sleep -Seconds 1
    net start "MSO_SpUsb_Service" 2>$null | Out-Null
    net start "Idemia_L1RD_Service" 2>$null | Out-Null
    Start-Sleep -Seconds 2
    Update-Status
    Log "[OK] Services restarted successfully."
})

# Button: Fix DLL
$btnFixDll.Add_Click({
    Fix-DllArchitectureMismatch
    Update-Status
})

# Button: Test Page
$btnTestPage.Add_Click({
    $testPage = "$scriptDir\Test_Page\RD_Test_Page.html"
    if (Test-Path $testPage) {
        Start-Process $testPage
        Log "Opened RD Test Page."
    }
})

# Button: Refresh
$btnRefresh.Add_Click({
    Update-Status
    Log "Status refreshed."
})

# Initialize UI
Log "Morpho L1 RD Service Control Center loaded."
Update-Status
Log "Ready. Click '1-Click Full Install & Fix' to begin."

$window.ShowDialog() | Out-Null
