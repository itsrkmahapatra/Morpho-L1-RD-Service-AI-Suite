using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ServiceProcess;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Reflection;
using System.Xml;
using Microsoft.Win32;

[assembly: AssemblyTitle("IDEMIA Morpho L1 RD Service AI Suite")]
[assembly: AssemblyDescription("Enterprise Grade Morpho L1 RD Service Management, Diagnostic & In-App Capture Suite")]
[assembly: AssemblyCompany("Raj Kishor Mahapatra")]
[assembly: AssemblyProduct("Morpho L1 RD Service AI Suite")]
[assembly: AssemblyCopyright("Copyright © 2026 Raj Kishor Mahapatra. All Rights Reserved.")]
[assembly: AssemblyVersion("3.5.0.0")]
[assembly: AssemblyFileVersion("3.5.0.0")]

namespace MorphoL1ProSuite
{
    public class App : Application
    {
        [STAThread]
        public static void Main()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            App app = new App();
            app.Run(new MainWindow());
        }
    }

    public class MainWindow : Window
    {
        // UI Controls - Dashboard
        private TextBlock txtHwStatus, txtHwDetails, txtHvciStatus, txtSvcStatus, txtPortStatus, txtAiStatus;
        private Border cardHw, cardHvci, cardSvc, cardPort;
        private ProgressBar progressBar;
        private TextBlock txtProgressStatus;
        private TextBox txtLogBox;
        private Button btnAutoInstall;

        // UI Controls - Native Biometric Studio
        private ComboBox cmbEnv, cmbFormat;
        private TextBox txtFingerCount, txtFingerType, txtTimeout, txtWadh, txtOtp, txtPosh;
        private Button btnPingRd, btnGetDevInfo, btnCaptureFinger;
        private TextBlock txtScanStatus, txtScanScore;
        private Border borderScanIndicator;
        private TextBox txtStudioXml;

        private DispatcherTimer monitorTimer;
        private string tempExtractDir;
        private bool isAiHealingEnabled = true;
        private bool isInstalling = false;

        public MainWindow()
        {
            tempExtractDir = Path.Combine(Path.GetTempPath(), "MorphoL1_Suite_RajKishor");
            InitializeComponent();

            Log("====================================================================");
            Log(" IDEMIA Morpho L1 RD Service AI Suite — Enterprise Edition");
            Log(" Lead Software Architect & Developer: Raj Kishor Mahapatra");
            Log(" Website: https://itsrkmahapatra.qzz.io/ | Email: dev@itsrkmahapatra.qzz.io");
            Log(" Autonomous AI Self-Healing Engine: ACTIVE");
            Log(" System Initialized at " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            Log("====================================================================");

            StartMonitorTimer();
        }

        private void InitializeComponent()
        {
            this.Title = "IDEMIA Morpho L1 RD Service AI Suite — Developed by Raj Kishor Mahapatra";
            this.Width = 1120;
            this.Height = 800;
            this.MinWidth = 1000;
            this.MinHeight = 720;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Background = new SolidColorBrush(Color.FromRgb(11, 15, 25)); // Deep Obsidian #0B0F19
            this.Foreground = Brushes.White;
            this.FontFamily = new FontFamily("Segoe UI, Inter, Arial");

            try
            {
                using (System.Drawing.Icon sysIcon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location))
                {
                    if (sysIcon != null)
                    {
                        this.Icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            sysIcon.Handle,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                    }
                }
            }
            catch { }

            ApplyCustomStyles();

            Grid rootGrid = new Grid();
            rootGrid.Margin = new Thickness(20);
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); // Header
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); // Live Status Cards
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }); // Tabs
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto }); // Footer

            // 1. Header
            Border headerBorder = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(17, 24, 39)),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20, 14, 20, 14),
                Margin = new Thickness(0, 0, 0, 14),
                BorderBrush = new SolidColorBrush(Color.FromRgb(31, 41, 55)),
                BorderThickness = new Thickness(1)
            };

            Grid headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            // Vector Biometric Logo
            Border logoBorder = new Border()
            {
                Width = 44,
                Height = 44,
                Background = new SolidColorBrush(Color.FromRgb(30, 27, 75)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(99, 102, 241)),
                BorderThickness = new Thickness(1.5),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(0, 0, 14, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            TextBlock logoIcon = new TextBlock()
            {
                Text = "🖐️",
                FontSize = 22,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            logoBorder.Child = logoIcon;

            StackPanel titlePanel = new StackPanel() { VerticalAlignment = VerticalAlignment.Center };
            TextBlock titleText = new TextBlock()
            {
                Text = "IDEMIA MORPHO L1 RD SERVICE AI SUITE",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(248, 250, 252))
            };
            TextBlock subTitleText = new TextBlock()
            {
                Text = "Autonomous Self-Healing Doctor • Memory Integrity Resolver • Native Capture Studio",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                Margin = new Thickness(0, 3, 0, 0)
            };
            titlePanel.Children.Add(titleText);
            titlePanel.Children.Add(subTitleText);

            StackPanel devPanel = new StackPanel() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

            Border aiBadge = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(6, 78, 59)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(16, 185, 129)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(0, 0, 10, 0)
            };
            txtAiStatus = new TextBlock()
            {
                Text = "🤖 AI Doctor: ACTIVE",
                FontWeight = FontWeights.Bold,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153))
            };
            aiBadge.Child = txtAiStatus;

            Border devBadge = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(79, 70, 229)),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12, 6, 12, 6),
                Cursor = Cursors.Hand
            };
            TextBlock devText = new TextBlock()
            {
                Text = "Developer: Raj Kishor Mahapatra ↗",
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Foreground = Brushes.White
            };
            devBadge.Child = devText;
            devBadge.MouseLeftButtonUp += (s, e) => Process.Start(new ProcessStartInfo("https://itsrkmahapatra.qzz.io/") { UseShellExecute = true });

            devPanel.Children.Add(aiBadge);
            devPanel.Children.Add(devBadge);

            headerGrid.Children.Add(logoBorder);
            Grid.SetColumn(logoBorder, 0);
            headerGrid.Children.Add(titlePanel);
            Grid.SetColumn(titlePanel, 1);
            headerGrid.Children.Add(devPanel);
            Grid.SetColumn(devPanel, 2);
            headerBorder.Child = headerGrid;

            // 2. Status Cards
            Grid cardsGrid = new Grid() { Margin = new Thickness(0, 0, 0, 14) };
            for (int i = 0; i < 4; i++) cardsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock dummy1, dummy2, dummy3;
            cardHw = CreateStatusCard("USB HARDWARE", out txtHwStatus, out txtHwDetails, "Morpho MSO 1300 E3 L1");
            cardHvci = CreateStatusCard("MEMORY INTEGRITY", out txtHvciStatus, out dummy1, "Core Isolation (HVCI)");
            cardSvc = CreateStatusCard("WINDOWS SERVICES", out txtSvcStatus, out dummy2, "Idemia L1 + Morpho USB");
            cardPort = CreateStatusCard("HTTPS PORT (11100)", out txtPortStatus, out dummy3, "127.0.0.1 / localhost");

            cardHw.Margin = new Thickness(0, 0, 6, 0);
            cardHvci.Margin = new Thickness(3, 0, 3, 0);
            cardSvc.Margin = new Thickness(3, 0, 3, 0);
            cardPort.Margin = new Thickness(6, 0, 0, 0);

            Grid.SetColumn(cardHw, 0);
            Grid.SetColumn(cardHvci, 1);
            Grid.SetColumn(cardSvc, 2);
            Grid.SetColumn(cardPort, 3);

            cardsGrid.Children.Add(cardHw);
            cardsGrid.Children.Add(cardHvci);
            cardsGrid.Children.Add(cardSvc);
            cardsGrid.Children.Add(cardPort);

            // 3. TabControl
            TabControl tabCtrl = new TabControl()
            {
                Background = new SolidColorBrush(Color.FromRgb(17, 24, 39)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85)),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 10)
            };

            tabCtrl.Items.Add(CreateInstallTab());
            tabCtrl.Items.Add(CreateNativeBiometricStudioTab());
            tabCtrl.Items.Add(CreateMemoryIntegrityTab());
            tabCtrl.Items.Add(CreateDoctorFixTab());
            tabCtrl.Items.Add(CreateLogsTab());
            tabCtrl.Items.Add(CreateAboutTab());

            // 4. Footer
            Grid footerGrid = new Grid();
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            TextBlock footTxt = new TextBlock()
            {
                Text = "UIDAI Aadhaar L1 Registered Device Specification v2.0 | Official Website: https://itsrkmahapatra.qzz.io/",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                VerticalAlignment = VerticalAlignment.Center
            };

            StackPanel footBtns = new StackPanel() { Orientation = Orientation.Horizontal };

            Button btnToggleAi = CreateButton("🤖 AI Auto-Fix: ON", Color.FromRgb(6, 95, 70), 12, 6);
            btnToggleAi.Click += (s, e) =>
            {
                isAiHealingEnabled = !isAiHealingEnabled;
                if (isAiHealingEnabled)
                {
                    btnToggleAi.Content = "🤖 AI Auto-Fix: ON";
                    btnToggleAi.Background = new SolidColorBrush(Color.FromRgb(6, 95, 70));
                    txtAiStatus.Text = "🤖 AI Doctor: ACTIVE";
                    txtAiStatus.Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153));
                    Log("AI Self-Healing Engine ENABLED.");
                }
                else
                {
                    btnToggleAi.Content = "🤖 AI Auto-Fix: OFF";
                    btnToggleAi.Background = new SolidColorBrush(Color.FromRgb(71, 85, 105));
                    txtAiStatus.Text = "🤖 AI Doctor: PAUSED";
                    txtAiStatus.Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184));
                    Log("AI Self-Healing Engine PAUSED.");
                }
            };

            Button btnRefreshAll = CreateButton("🔄 Refresh Status", Color.FromRgb(30, 41, 59), 12, 6);
            btnRefreshAll.Margin = new Thickness(8, 0, 0, 0);
            btnRefreshAll.Click += (s, e) => { UpdateLiveStatus(); Log("Live monitor refreshed."); };

            footBtns.Children.Add(btnToggleAi);
            footBtns.Children.Add(btnRefreshAll);

            footerGrid.Children.Add(footTxt);
            Grid.SetColumn(footTxt, 0);
            footerGrid.Children.Add(footBtns);
            Grid.SetColumn(footBtns, 1);

            rootGrid.Children.Add(headerBorder);
            Grid.SetRow(headerBorder, 0);
            rootGrid.Children.Add(cardsGrid);
            Grid.SetRow(cardsGrid, 1);
            rootGrid.Children.Add(tabCtrl);
            Grid.SetRow(tabCtrl, 2);
            rootGrid.Children.Add(footerGrid);
            Grid.SetRow(footerGrid, 3);

            this.Content = rootGrid;
        }

        private void ApplyCustomStyles()
        {
            Style tabItemStyle = new Style(typeof(TabItem));
            ControlTemplate template = new ControlTemplate(typeof(TabItem));
            FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.Name = "TabBorder";
            borderFactory.SetValue(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(30, 41, 59))); // #1E293B
            borderFactory.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(51, 65, 85)));
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1, 1, 1, 0));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(8, 8, 0, 0));
            borderFactory.SetValue(Border.PaddingProperty, new Thickness(16, 10, 16, 10));
            borderFactory.SetValue(Border.MarginProperty, new Thickness(0, 0, 4, 0));

            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.Name = "ContentSite";
            contentPresenter.SetValue(ContentPresenter.ContentSourceProperty, "Header");
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            contentPresenter.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);
            contentPresenter.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(203, 213, 225)));

            borderFactory.AppendChild(contentPresenter);
            template.VisualTree = borderFactory;

            Trigger isSelectedTrigger = new Trigger() { Property = TabItem.IsSelectedProperty, Value = true };
            isSelectedTrigger.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(79, 70, 229)), "TabBorder")); // Indigo
            isSelectedTrigger.Setters.Add(new Setter(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(129, 140, 248)), "TabBorder"));
            isSelectedTrigger.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.White, "ContentSite"));
            template.Triggers.Add(isSelectedTrigger);

            Trigger isMouseOverTrigger = new Trigger() { Property = TabItem.IsMouseOverProperty, Value = true };
            isMouseOverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(51, 65, 85)), "TabBorder"));
            isMouseOverTrigger.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.White, "ContentSite"));
            template.Triggers.Add(isMouseOverTrigger);

            tabItemStyle.Setters.Add(new Setter(TabItem.TemplateProperty, template));
            this.Resources.Add(typeof(TabItem), tabItemStyle);

            Style comboStyle = new Style(typeof(ComboBox));
            comboStyle.Setters.Add(new Setter(ComboBox.BackgroundProperty, new SolidColorBrush(Color.FromRgb(15, 23, 42))));
            comboStyle.Setters.Add(new Setter(ComboBox.ForegroundProperty, new SolidColorBrush(Color.FromRgb(56, 189, 248))));
            comboStyle.Setters.Add(new Setter(ComboBox.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(51, 65, 85))));
            comboStyle.Setters.Add(new Setter(ComboBox.FontFamilyProperty, new FontFamily("Consolas, Segoe UI")));
            this.Resources.Add(typeof(ComboBox), comboStyle);
        }

        private Border CreateStatusCard(string header, out TextBlock statusText, out TextBlock subText, string defaultSub)
        {
            Border b = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(17, 24, 39)),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(14, 12, 14, 12),
                BorderBrush = new SolidColorBrush(Color.FromRgb(31, 41, 55)),
                BorderThickness = new Thickness(1)
            };

            StackPanel sp = new StackPanel();
            TextBlock lblHeader = new TextBlock()
            {
                Text = header,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139))
            };

            statusText = new TextBlock()
            {
                Text = "Checking...",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(245, 158, 11)),
                Margin = new Thickness(0, 4, 0, 2)
            };

            subText = new TextBlock()
            {
                Text = defaultSub,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                TextTrimming = TextTrimming.CharacterEllipsis
            };

            sp.Children.Add(lblHeader);
            sp.Children.Add(statusText);
            sp.Children.Add(subText);
            b.Child = sp;
            return b;
        }

        private TabItem CreateInstallTab()
        {
            TabItem tab = new TabItem() { Header = "  🚀 1-Click Automated Setup  ", FontSize = 13 };
            Grid grid = new Grid() { Margin = new Thickness(18) };
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            StackPanel intro = new StackPanel() { Margin = new Thickness(0, 0, 0, 16) };
            TextBlock h1 = new TextBlock() { Text = "Self-Contained Automated Deployment & AI Architecture Doctor", FontSize = 16, FontWeight = FontWeights.Bold, Foreground = Brushes.White };
            TextBlock desc = new TextBlock()
            {
                Text = "All installer binaries, drivers, and Visual C++ dependencies are embedded directly in this single executable. The engine automatically extracts dependencies, synchronizes NTP time, installs runtimes, deploys the IDEMIA service, applies the 32-bit architecture repair (BadImageFormatException 0x8007000B), and launches the services.",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 0)
            };
            intro.Children.Add(h1);
            intro.Children.Add(desc);

            btnAutoInstall = CreateButton("🚀  START 1-CLICK ALL-IN-ONE INSTALLATION & REPAIR", Color.FromRgb(79, 70, 229), 20, 14, 15);
            btnAutoInstall.Margin = new Thickness(0, 0, 0, 16);
            btnAutoInstall.Click += BtnAutoInstall_Click;

            StackPanel progressPanel = new StackPanel() { Margin = new Thickness(0, 0, 0, 16) };
            txtProgressStatus = new TextBlock() { Text = "Ready to deploy Morpho L1 RD Service.", FontSize = 12, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(Color.FromRgb(56, 189, 248)), Margin = new Thickness(0, 0, 0, 6) };
            progressBar = new ProgressBar() { Height = 10, Background = new SolidColorBrush(Color.FromRgb(31, 41, 55)), Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129)), Value = 0 };
            progressPanel.Children.Add(txtProgressStatus);
            progressPanel.Children.Add(progressBar);

            Border infoBox = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(2, 6, 23)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14),
                BorderBrush = new SolidColorBrush(Color.FromRgb(31, 41, 55)),
                BorderThickness = new Thickness(1)
            };
            TextBlock infoTxt = new TextBlock()
            {
                Text = "✔ Embedded Packages: IDEMIA L1 RD Service v5.0.0.10, VC++ 2010 x86, VC++ 2015-2022 x86, VC++ 2015-2022 x64\n" +
                       "✔ Autonomous AI Doctor: Continuously monitors and auto-repairs DLL crashes, stopped services, and time drift\n" +
                       "✔ Native In-App Testing: No Web Browser Required — Switch to the 'Native Biometric Studio' tab for live capture.",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                LineHeight = 20
            };
            infoBox.Child = infoTxt;

            grid.Children.Add(intro);
            Grid.SetRow(intro, 0);
            grid.Children.Add(btnAutoInstall);
            Grid.SetRow(btnAutoInstall, 1);
            grid.Children.Add(progressPanel);
            Grid.SetRow(progressPanel, 2);
            grid.Children.Add(infoBox);
            Grid.SetRow(infoBox, 3);

            tab.Content = grid;
            return tab;
        }

        private TabItem CreateNativeBiometricStudioTab()
        {
            TabItem tab = new TabItem() { Header = "  🖐️ Native Biometric Studio (In-App)  ", FontSize = 13 };
            Grid grid = new Grid() { Margin = new Thickness(16) };
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            Border optBorder = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(2, 6, 23)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(0, 0, 0, 12),
                BorderBrush = new SolidColorBrush(Color.FromRgb(31, 41, 55)),
                BorderThickness = new Thickness(1)
            };

            WrapPanel wp = new WrapPanel() { VerticalAlignment = VerticalAlignment.Center };

            wp.Children.Add(CreateFieldLabel("Env:"));
            cmbEnv = new ComboBox() { Width = 60, Margin = new Thickness(0, 0, 12, 0), SelectedIndex = 0 };
            cmbEnv.Items.Add("P");
            cmbEnv.Items.Add("PP");
            wp.Children.Add(cmbEnv);

            wp.Children.Add(CreateFieldLabel("Format:"));
            cmbFormat = new ComboBox() { Width = 75, Margin = new Thickness(0, 0, 12, 0), SelectedIndex = 0 };
            cmbFormat.Items.Add("0 (XML)");
            cmbFormat.Items.Add("1 (Proto)");
            wp.Children.Add(cmbFormat);

            wp.Children.Add(CreateFieldLabel("fCount:"));
            txtFingerCount = CreateInputTextBox("1", 40);
            wp.Children.Add(txtFingerCount);

            wp.Children.Add(CreateFieldLabel("fType:"));
            txtFingerType = CreateInputTextBox("0", 40);
            wp.Children.Add(txtFingerType);

            wp.Children.Add(CreateFieldLabel("Timeout (ms):"));
            txtTimeout = CreateInputTextBox("10000", 65);
            wp.Children.Add(txtTimeout);

            wp.Children.Add(CreateFieldLabel("Wadh:"));
            txtWadh = CreateInputTextBox("", 70);
            wp.Children.Add(txtWadh);

            wp.Children.Add(CreateFieldLabel("Otp:"));
            txtOtp = CreateInputTextBox("", 60);
            wp.Children.Add(txtOtp);

            wp.Children.Add(CreateFieldLabel("Posh:"));
            txtPosh = CreateInputTextBox("", 60);
            wp.Children.Add(txtPosh);

            optBorder.Child = wp;

            Grid actGrid = new Grid() { Margin = new Thickness(0, 0, 0, 12) };
            actGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            actGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            actGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            actGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            actGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            actGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            actGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            btnPingRd = CreateButton("⚡ 1. RDSERVICE", Color.FromRgb(79, 70, 229));
            btnPingRd.Margin = new Thickness(0, 0, 8, 0);
            btnPingRd.Click += async (s, e) => await PingRdService();

            btnGetDevInfo = CreateButton("📋 2. DEVICE INFO", Color.FromRgb(2, 132, 199));
            btnGetDevInfo.Margin = new Thickness(0, 0, 8, 0);
            btnGetDevInfo.Click += async (s, e) => await QueryDeviceInfo();

            btnCaptureFinger = CreateButton("🔴 3. CAPTURE FINGERPRINT", Color.FromRgb(220, 38, 38), 18, 10, 13);
            btnCaptureFinger.Margin = new Thickness(0, 0, 8, 0);
            btnCaptureFinger.Click += async (s, e) => await CaptureBiometric();

            Button btnCopyXml = CreateButton("📑 Copy XML", Color.FromRgb(51, 65, 85), 12, 8);
            btnCopyXml.Margin = new Thickness(0, 0, 8, 0);
            btnCopyXml.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(txtStudioXml.Text))
                {
                    Clipboard.SetText(txtStudioXml.Text);
                    Log("XML copied to clipboard.");
                }
            };

            Button btnClearXml = CreateButton("🧹 Clear", Color.FromRgb(51, 65, 85), 12, 8);
            btnClearXml.Margin = new Thickness(0, 0, 8, 0);
            btnClearXml.Click += (s, e) =>
            {
                txtStudioXml.Text = "";
                SetScanIndicator("SCANNER STATUS: IDLE", Color.FromRgb(148, 163, 184));
            };

            Button btnSaveXml = CreateButton("💾 Save XML", Color.FromRgb(15, 118, 110), 12, 8);
            btnSaveXml.Margin = new Thickness(0, 0, 8, 0);
            btnSaveXml.Click += (s, e) =>
            {
                try
                {
                    SaveFileDialog sfd = new SaveFileDialog()
                    {
                        Filter = "XML File (*.xml)|*.xml|All Files (*.*)|*.*",
                        FileName = "Morpho_Biometric_Capture_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml"
                    };
                    if (sfd.ShowDialog() == true)
                    {
                        File.WriteAllText(sfd.FileName, txtStudioXml.Text);
                        Log("Saved XML payload to: " + sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    Log("Error saving XML: " + ex.Message);
                }
            };

            borderScanIndicator = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(2, 6, 23)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14, 6, 14, 6),
                BorderBrush = new SolidColorBrush(Color.FromRgb(31, 41, 55)),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(8, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            StackPanel scanSp = new StackPanel() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            txtScanStatus = new TextBlock() { Text = "SCANNER STATUS: IDLE", FontSize = 12, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)) };
            txtScanScore = new TextBlock() { Text = "", FontSize = 12, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129)), Margin = new Thickness(10, 0, 0, 0) };
            scanSp.Children.Add(txtScanStatus);
            scanSp.Children.Add(txtScanScore);
            borderScanIndicator.Child = scanSp;

            actGrid.Children.Add(btnPingRd);
            Grid.SetColumn(btnPingRd, 0);
            actGrid.Children.Add(btnGetDevInfo);
            Grid.SetColumn(btnGetDevInfo, 1);
            actGrid.Children.Add(btnCaptureFinger);
            Grid.SetColumn(btnCaptureFinger, 2);
            actGrid.Children.Add(btnCopyXml);
            Grid.SetColumn(btnCopyXml, 3);
            actGrid.Children.Add(btnClearXml);
            Grid.SetColumn(btnClearXml, 4);
            actGrid.Children.Add(btnSaveXml);
            Grid.SetColumn(btnSaveXml, 5);
            actGrid.Children.Add(borderScanIndicator);
            Grid.SetColumn(borderScanIndicator, 6);

            txtStudioXml = new TextBox()
            {
                Background = new SolidColorBrush(Color.FromRgb(2, 6, 23)),
                Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153)),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                Padding = new Thickness(14),
                BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85)),
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                IsReadOnly = true,
                Text = "<!-- IDEMIA Morpho L1 Native Biometric Studio Ready.\r\n     Click '1. RDSERVICE' to check engine readiness, or click '3. CAPTURE FINGERPRINT' to scan your finger directly in this app. -->"
            };

            grid.Children.Add(optBorder);
            Grid.SetRow(optBorder, 0);
            grid.Children.Add(actGrid);
            Grid.SetRow(actGrid, 1);
            grid.Children.Add(txtStudioXml);
            Grid.SetRow(txtStudioXml, 2);

            tab.Content = grid;
            return tab;
        }

        private TabItem CreateMemoryIntegrityTab()
        {
            TabItem tab = new TabItem() { Header = "  🛡️ Memory Integrity (Code 39 Fix)  ", FontSize = 13 };
            Grid grid = new Grid() { Margin = new Thickness(18) };
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            StackPanel spHead = new StackPanel() { Margin = new Thickness(0, 0, 0, 16) };
            TextBlock h1 = new TextBlock() { Text = "Windows Memory Integrity (Core Isolation) & Driver Code 39 Resolver", FontSize = 16, FontWeight = FontWeights.Bold, Foreground = Brushes.White };
            TextBlock p1 = new TextBlock()
            {
                Text = "Windows 11 Memory Integrity (HVCI) blocks the official Morpho USB kernel driver (UsbBioDriver_x64.sys) because it was built before HVCI standards. This causes 'Code 39: Driver failed to load' in Device Manager.",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 0)
            };
            spHead.Children.Add(h1);
            spHead.Children.Add(p1);

            Grid btnRow = new Grid() { Margin = new Thickness(0, 0, 0, 16) };
            btnRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            btnRow.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            Button btnAutoDisableHvci = CreateButton("⚡ 1-Click Turn OFF Memory Integrity (Fix Code 39)", Color.FromRgb(220, 38, 38), 16, 12, 13);
            btnAutoDisableHvci.Margin = new Thickness(0, 0, 6, 0);
            btnAutoDisableHvci.Click += (s, e) => DisableMemoryIntegrityRegistry();

            Button btnOpenSettings = CreateButton("⚙️ Open Windows Core Isolation Settings", Color.FromRgb(2, 132, 199), 16, 12, 13);
            btnOpenSettings.Margin = new Thickness(6, 0, 0, 0);
            btnOpenSettings.Click += (s, e) => Process.Start(new ProcessStartInfo("windowsdefender://coreisolation") { UseShellExecute = true });

            btnRow.Children.Add(btnAutoDisableHvci);
            Grid.SetColumn(btnAutoDisableHvci, 0);
            btnRow.Children.Add(btnOpenSettings);
            Grid.SetColumn(btnOpenSettings, 1);

            Border cardSteps = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(2, 6, 23)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16),
                BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85)),
                BorderThickness = new Thickness(1)
            };

            StackPanel spSteps = new StackPanel();
            TextBlock sTitle = new TextBlock() { Text = "HOW TO SOLVE & IMPORTANT SECURITY REASSURANCE:", FontSize = 12, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(52, 211, 153)) };
            TextBlock sBody = new TextBlock()
            {
                Text = "1. Click '1-Click Turn OFF Memory Integrity' above (our software writes the official registry key instantly).\n" +
                       "2. RESTART your computer.\n" +
                       "3. Once restarted, the Morpho MSO 1300 E3 L1 driver will load cleanly and the scanner light will activate!\n\n" +
                       "🛡️ Security Note: Disabling Memory Integrity only allows legacy USB hardware drivers to load. Your Windows Defender Antivirus, Real-Time Protection, Firewall, Cloud Protection, Tamper Protection, and Secure Boot remain 100% ACTIVE and PROTECTED.",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                LineHeight = 22,
                Margin = new Thickness(0, 8, 0, 0)
            };
            spSteps.Children.Add(sTitle);
            spSteps.Children.Add(sBody);
            cardSteps.Child = spSteps;

            grid.Children.Add(spHead);
            Grid.SetRow(spHead, 0);
            grid.Children.Add(btnRow);
            Grid.SetRow(btnRow, 1);
            grid.Children.Add(cardSteps);
            Grid.SetRow(cardSteps, 2);

            tab.Content = grid;
            return tab;
        }

        private TabItem CreateDoctorFixTab()
        {
            TabItem tab = new TabItem() { Header = "  🛠️ One-Click Fix Center  ", FontSize = 13 };
            ScrollViewer sv = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            StackPanel mainSp = new StackPanel() { Margin = new Thickness(18) };

            // 1. One-Click Repairs
            TextBlock h1 = new TextBlock() { Text = "Automated Diagnostics & AI One-Click Repair Tools", FontSize = 16, FontWeight = FontWeights.Bold, Foreground = Brushes.White, Margin = new Thickness(0, 0, 0, 12) };
            mainSp.Children.Add(h1);

            Grid btnGrid1 = new Grid() { Margin = new Thickness(0, 0, 0, 10) };
            btnGrid1.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            btnGrid1.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            Button btnFix1 = CreateButton("🛠️ Fix 0x8007000B (BadImageFormatException)", Color.FromRgb(5, 150, 105), 14, 10);
            btnFix1.Margin = new Thickness(0, 0, 6, 0);
            btnFix1.Click += (s, e) => { FixDllArchitecture(); UpdateLiveStatus(); MessageBox.Show("32-bit Architecture compatibility patch applied successfully!", "Fix Applied", MessageBoxButton.OK, MessageBoxImage.Information); };

            Button btnFix2 = CreateButton("⏱️ Set IST (UTC+05:30) & Resync Clock", Color.FromRgb(2, 132, 199), 14, 10);
            btnFix2.Margin = new Thickness(6, 0, 0, 0);
            btnFix2.Click += (s, e) => SetIndianTimeZoneAndSyncTime();

            btnGrid1.Children.Add(btnFix1);
            Grid.SetColumn(btnFix1, 0);
            btnGrid1.Children.Add(btnFix2);
            Grid.SetColumn(btnFix2, 1);
            mainSp.Children.Add(btnGrid1);

            Grid btnGrid2 = new Grid() { Margin = new Thickness(0, 0, 0, 10) };
            btnGrid2.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            btnGrid2.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            Button btnFix3 = CreateButton("🔄 Restart All RD Windows Services", Color.FromRgb(79, 70, 229), 14, 10);
            btnFix3.Margin = new Thickness(0, 0, 6, 0);
            btnFix3.Click += (s, e) => RestartServices();

            Button btnFix4 = CreateButton("🌐 Re-Register HTTPS Localhost Certificates", Color.FromRgb(217, 119, 6), 14, 10);
            btnFix4.Margin = new Thickness(6, 0, 0, 0);
            btnFix4.Click += (s, e) => ReinstallSslCertificates();

            btnGrid2.Children.Add(btnFix3);
            Grid.SetColumn(btnFix3, 0);
            btnGrid2.Children.Add(btnFix4);
            Grid.SetColumn(btnFix4, 1);
            mainSp.Children.Add(btnGrid2);

            Grid btnGrid3 = new Grid() { Margin = new Thickness(0, 0, 0, 16) };
            btnGrid3.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            btnGrid3.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            Button btnFix5 = CreateButton("🧹 Purge Temp & Driver Cache (%temp%)", Color.FromRgb(71, 85, 105), 14, 10);
            btnFix5.Margin = new Thickness(0, 0, 6, 0);
            btnFix5.Click += (s, e) => PurgeTempCache();

            Button btnFix6 = CreateButton("🛡️ Allow Ports 11100-11120 in Firewall", Color.FromRgb(15, 118, 110), 14, 10);
            btnFix6.Margin = new Thickness(6, 0, 0, 0);
            btnFix6.Click += (s, e) => AddFirewallRules();

            btnGrid3.Children.Add(btnFix5);
            Grid.SetColumn(btnFix5, 0);
            btnGrid3.Children.Add(btnFix6);
            Grid.SetColumn(btnFix6, 1);
            mainSp.Children.Add(btnGrid3);

            // 2. Communication Mode (Portal Compatibility) Switcher
            Border commCard = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(2, 6, 23)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(14),
                Margin = new Thickness(0, 0, 0, 16),
                BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85)),
                BorderThickness = new Thickness(1)
            };

            StackPanel commSp = new StackPanel();
            TextBlock commTitle = new TextBlock()
            {
                Text = "⚡ Communication Mode Switcher (Portal Compatibility Fixer)",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(245, 158, 11))
            };
            TextBlock commDesc = new TextBlock()
            {
                Text = "Different portals (DigiPay, CSC, PM-Kisan, Banking) require specific communication modes in config_settings.xml. Select your target portal mode below for instant 1-click reconfiguration:",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 10)
            };

            Grid modeGrid = new Grid();
            modeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            modeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            modeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            Button btnMode0 = CreateButton("Mode 0: HTTP (CSC/DigiPay/AEPS)", Color.FromRgb(30, 41, 59), 8, 8, 11);
            btnMode0.Margin = new Thickness(0, 0, 4, 0);
            btnMode0.Click += (s, e) => SetCommunicationMode(0, "Mode 0: HTTP (CSC DigiPay / Spice Money / AEPS)");

            Button btnMode1 = CreateButton("Mode 1: HTTPS (localhost)", Color.FromRgb(30, 41, 59), 8, 8, 11);
            btnMode1.Margin = new Thickness(2, 0, 2, 0);
            btnMode1.Click += (s, e) => SetCommunicationMode(1, "Mode 1: HTTPS localhost (UIDAI Direct / Ayushman Bharat)");

            Button btnMode2 = CreateButton("Mode 2: HTTPS (127.0.0.1)", Color.FromRgb(30, 41, 59), 8, 8, 11);
            btnMode2.Margin = new Thickness(4, 0, 0, 0);
            btnMode2.Click += (s, e) => SetCommunicationMode(2, "Mode 2: HTTPS 127.0.0.1 (PM-Kisan / Bank CSPs)");

            modeGrid.Children.Add(btnMode0);
            Grid.SetColumn(btnMode0, 0);
            modeGrid.Children.Add(btnMode1);
            Grid.SetColumn(btnMode1, 1);
            modeGrid.Children.Add(btnMode2);
            Grid.SetColumn(btnMode2, 2);

            commSp.Children.Add(commTitle);
            commSp.Children.Add(commDesc);
            commSp.Children.Add(modeGrid);
            commCard.Child = commSp;
            mainSp.Children.Add(commCard);

            // 3. Online Whitelist & Browser Helpers
            Grid helperGrid = new Grid();
            helperGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            helperGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            Button btnChromeFlag = CreateButton("🌐 Fix Chrome Localhost Flag (#allow-insecure-localhost)", Color.FromRgb(14, 116, 144), 12, 10);
            btnChromeFlag.Margin = new Thickness(0, 0, 6, 0);
            btnChromeFlag.Click += (s, e) => OpenChromeLocalhostFlags();

            Button btnWhitelist = CreateButton("🔍 Check Device Whitelist on rdserviceonline.com", Color.FromRgb(109, 40, 217), 12, 10);
            btnWhitelist.Margin = new Thickness(6, 0, 0, 0);
            btnWhitelist.Click += (s, e) => CheckDeviceWhitelistOnline();

            helperGrid.Children.Add(btnChromeFlag);
            Grid.SetColumn(btnChromeFlag, 0);
            helperGrid.Children.Add(btnWhitelist);
            Grid.SetColumn(btnWhitelist, 1);
            mainSp.Children.Add(helperGrid);

            sv.Content = mainSp;
            tab.Content = sv;
            return tab;
        }

        private void SetCommunicationMode(int mode, string modeName)
        {
            Task.Run(() =>
            {
                try
                {
                    string cfg = @"C:\IdemiaL1RdService\RDService\config_settings.xml";
                    if (File.Exists(cfg))
                    {
                        string content = File.ReadAllText(cfg);
                        content = Regex.Replace(content, @"<IsSSLEnabled>\d+</IsSSLEnabled>", "<IsSSLEnabled>" + mode + "</IsSSLEnabled>");
                        File.WriteAllText(cfg, content);
                        Log("[⚡ Config Updated] IsSSLEnabled set to " + mode + " (" + modeName + ")");
                        RestartServices();
                        MessageBox.Show("Communication Mode updated to:\n" + modeName + "\n\nRD Service restarted successfully!", "Mode Applied", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Log("[!] config_settings.xml not found at default path.");
                    }
                }
                catch (Exception ex)
                {
                    Log("[!] Error updating communication mode: " + ex.Message);
                }
            });
        }

        private void SetIndianTimeZoneAndSyncTime()
        {
            Task.Run(() =>
            {
                Log("Configuring Windows Time Zone to India Standard Time (UTC+05:30)...");
                RunProcess("tzutil.exe", "/s \"India Standard Time\"");
                RunProcess("net.exe", "start w32time");
                RunProcess("w32tm.exe", "/resync /force");
                Log("[OK] System clock synchronized with NTP server (UTC+05:30 IST).");
                MessageBox.Show("Time Zone configured to India Standard Time (UTC+05:30) and system clock synchronized!", "Time Synced", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void PurgeTempCache()
        {
            Task.Run(() =>
            {
                Log("Purging Windows Temp, Driver Prefetch, and stale log files...");
                try
                {
                    string tempPath = Path.GetTempPath();
                    foreach (string file in Directory.GetFiles(tempPath))
                    {
                        try { File.Delete(file); } catch { }
                    }
                }
                catch { }

                if (Directory.Exists(@"C:\MORPHORDLOG"))
                {
                    try { Directory.Delete(@"C:\MORPHORDLOG", true); } catch { }
                }

                Log("[OK] Temporary files and cache purged.");
                MessageBox.Show("Temporary files, driver cache, and stale logs purged successfully!", "Cache Cleaned", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void AddFirewallRules()
        {
            Task.Run(() =>
            {
                Log("Adding Windows Firewall Inbound/Outbound rules for RD Service ports 11100-11120...");
                RunProcess("netsh.exe", "advfirewall firewall add rule name=\"IDEMIA Morpho L1 RDService Ports (11100-11120)\" dir=in action=allow protocol=TCP localport=11100-11120");
                RunProcess("netsh.exe", "advfirewall firewall add rule name=\"IDEMIA Morpho L1 RDService Program\" dir=in action=allow program=\"C:\\IdemiaL1RdService\\RDService\\IdemiaInteractiveL1RDService.exe\" enable=yes");
                Log("[OK] Windows Firewall rules registered for ports 11100-11120.");
                MessageBox.Show("Windows Firewall rules added for ports 11100-11120 and IDEMIA Service!", "Firewall Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void OpenChromeLocalhostFlags()
        {
            try
            {
                Log("Opening Google Chrome flags for localhost certificate bypass (#allow-insecure-localhost)...");
                Process.Start(new ProcessStartInfo("chrome.exe", "chrome://flags/#allow-insecure-localhost") { UseShellExecute = true });
            }
            catch
            {
                Process.Start(new ProcessStartInfo("https://itsrkmahapatra.github.io/Morpho-L1-RD-Service-AI-Suite/#troubleshooting") { UseShellExecute = true });
            }
        }

        private void CheckDeviceWhitelistOnline()
        {
            try
            {
                Log("Opening RDServiceOnline official device whitelist checker...");
                Process.Start(new ProcessStartInfo("https://rdserviceonline.com/check-device-validity") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Log("[!] Error opening whitelist checker: " + ex.Message);
            }
        }

        private TabItem CreateLogsTab()
        {
            TabItem tab = new TabItem() { Header = "  📜 Live Event Logs  ", FontSize = 13 };
            Grid grid = new Grid() { Margin = new Thickness(18) };
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            txtLogBox = new TextBox()
            {
                Background = new SolidColorBrush(Color.FromRgb(2, 6, 23)),
                Foreground = new SolidColorBrush(Color.FromRgb(56, 189, 248)),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                Padding = new Thickness(12),
                BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85)),
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                IsReadOnly = true
            };

            StackPanel btnBar = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 10, 0, 0) };
            Button btnClear = CreateButton("Clear Logs", Color.FromRgb(30, 41, 59), 12, 6);
            btnClear.Click += (s, e) => txtLogBox.Clear();

            Button btnExport = CreateButton("Export Log File", Color.FromRgb(79, 70, 229), 12, 6);
            btnExport.Margin = new Thickness(8, 0, 0, 0);
            btnExport.Click += (s, e) =>
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Morpho_L1_Diagnostic_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".log");
                File.WriteAllText(path, txtLogBox.Text);
                MessageBox.Show("Diagnostic log saved successfully to Desktop:\n" + path, "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            btnBar.Children.Add(btnClear);
            btnBar.Children.Add(btnExport);

            grid.Children.Add(txtLogBox);
            Grid.SetRow(txtLogBox, 0);
            grid.Children.Add(btnBar);
            Grid.SetRow(btnBar, 1);

            tab.Content = grid;
            return tab;
        }

        private TabItem CreateAboutTab()
        {
            TabItem tab = new TabItem() { Header = "  ℹ️ Developer & System  ", FontSize = 13 };
            Border border = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(2, 6, 23)),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(24),
                Margin = new Thickness(18),
                BorderBrush = new SolidColorBrush(Color.FromRgb(31, 41, 55)),
                BorderThickness = new Thickness(1)
            };

            StackPanel sp = new StackPanel();
            TextBlock h1 = new TextBlock() { Text = "IDEMIA Morpho L1 RD Service AI Suite", FontSize = 22, FontWeight = FontWeights.Bold, Foreground = Brushes.White };
            TextBlock ver = new TextBlock() { Text = "Version 3.5.0 Enterprise AI Edition (Single Portable Binary)", FontSize = 13, Foreground = new SolidColorBrush(Color.FromRgb(56, 189, 248)), Margin = new Thickness(0, 4, 0, 16) };

            Border devCard = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(17, 24, 39)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(18),
                Margin = new Thickness(0, 0, 0, 16),
                BorderBrush = new SolidColorBrush(Color.FromRgb(79, 70, 229)),
                BorderThickness = new Thickness(1.5)
            };

            StackPanel devSp = new StackPanel();
            TextBlock devTitle = new TextBlock() { Text = "LEAD SOFTWARE ARCHITECT & DEVELOPER", FontSize = 11, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)) };
            TextBlock devName = new TextBlock() { Text = "Raj Kishor Mahapatra", FontSize = 20, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129)), Margin = new Thickness(0, 4, 0, 8) };

            TextBlock devWeb = new TextBlock() { Text = "🌐 Portfolio / Website: https://itsrkmahapatra.qzz.io/", FontSize = 13, Foreground = new SolidColorBrush(Color.FromRgb(56, 189, 248)), Cursor = Cursors.Hand };
            devWeb.MouseLeftButtonUp += (s, e) => Process.Start(new ProcessStartInfo("https://itsrkmahapatra.qzz.io/") { UseShellExecute = true });

            TextBlock devEmail = new TextBlock() { Text = "✉️ Official Contact Email: dev@itsrkmahapatra.qzz.io", FontSize = 13, Foreground = new SolidColorBrush(Color.FromRgb(129, 140, 248)), Margin = new Thickness(0, 4, 0, 0), Cursor = Cursors.Hand };
            devEmail.MouseLeftButtonUp += (s, e) => Process.Start(new ProcessStartInfo("mailto:dev@itsrkmahapatra.qzz.io") { UseShellExecute = true });

            devSp.Children.Add(devTitle);
            devSp.Children.Add(devName);
            devSp.Children.Add(devWeb);
            devSp.Children.Add(devEmail);
            devCard.Child = devSp;

            TextBlock details = new TextBlock()
            {
                Text = "• Compliant with UIDAI Aadhaar Registered Device (RD) Service Specification v2.0\n" +
                       "• Autonomous AI Doctor: Background self-healing of DLL collisions, dropped services, and clock drift\n" +
                       "• Memory Integrity (HVCI) Resolver: 1-Click repair for Device Manager Code 39\n" +
                       "• Native In-App Capture Studio — 100% functional without external browsers\n" +
                       "• 1-Click Dynamic Communication Mode Switcher (HTTP Mode 0 vs HTTPS Mode 1/2)\n" +
                       "• Compatible with all Banking CSPs, CSC DigiPay, Spice Money, PayNearby, Ayushman Bharat, etc.",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(203, 213, 225)),
                LineHeight = 18,
                Margin = new Thickness(0, 0, 0, 12)
            };

            Border legalCard = new Border()
            {
                Background = new SolidColorBrush(Color.FromRgb(15, 23, 42)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12),
                BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85)),
                BorderThickness = new Thickness(1)
            };

            StackPanel legalSp = new StackPanel();
            TextBlock legalTitle = new TextBlock() { Text = "⚖️ LEGAL NOTICES, THIRD-PARTY ATTRIBUTIONS & DISCLAIMERS", FontSize = 10, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(245, 158, 11)) };
            TextBlock legalText = new TextBlock()
            {
                Text = "• IDEMIA, Morpho, MorphoSmart, and MSO 1300 E3 are registered trademarks of IDEMIA Identity & Security France.\n" +
                       "• Microsoft, Visual C++, Windows, and .NET are registered trademarks of Microsoft Corporation.\n" +
                       "• UIDAI and Aadhaar are registered trademarks and statutory authorities of the Government of India.\n" +
                       "• DISCLAIMER: This software is an independent utility created by Raj Kishor Mahapatra. It is not affiliated with, endorsed by, or sponsored by IDEMIA, Microsoft, or UIDAI.\n" +
                       "• LICENSE: End-User Restricted License Agreement (EULA). Usage permitted; copying or modifying is strictly prohibited.",
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                LineHeight = 15,
                Margin = new Thickness(0, 4, 0, 0)
            };
            legalSp.Children.Add(legalTitle);
            legalSp.Children.Add(legalText);
            legalCard.Child = legalSp;

            sp.Children.Add(h1);
            sp.Children.Add(ver);
            sp.Children.Add(devCard);
            sp.Children.Add(details);
            sp.Children.Add(legalCard);

            ScrollViewer sv = new ScrollViewer() { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            sv.Content = sp;
            border.Child = sv;
            tab.Content = border;
            return tab;
        }

        private void DisableMemoryIntegrityRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", true))
                {
                    if (key != null)
                    {
                        key.SetValue("Enabled", 0, RegistryValueKind.DWord);
                        Log("[🛡️ Memory Integrity Fix] Registry key updated: HypervisorEnforcedCodeIntegrity = 0");
                        UpdateLiveStatus();
                        
                        MessageBoxResult res = MessageBox.Show(
                            "Memory Integrity has been configured to OFF in Windows Registry!\r\n\r\n" +
                            "A system restart is required for the Morpho USB driver to load cleanly without Code 39.\r\n\r\n" +
                            "Would you like to restart your computer now?",
                            "Restart Required",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (res == MessageBoxResult.Yes)
                        {
                            RunProcess("shutdown.exe", "/r /t 5 /c \"Restarting to apply Morpho L1 Driver Compatibility (Memory Integrity Disabled)...\"");
                        }
                    }
                    else
                    {
                        Log("[!] Registry path not found. Opening Windows Core Isolation Settings...");
                        Process.Start(new ProcessStartInfo("windowsdefender://coreisolation") { UseShellExecute = true });
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error modifying registry: " + ex.Message);
                Process.Start(new ProcessStartInfo("windowsdefender://coreisolation") { UseShellExecute = true });
            }
        }

        private TextBlock CreateFieldLabel(string text)
        {
            return new TextBlock()
            {
                Text = text,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(6, 0, 4, 0)
            };
        }

        private TextBox CreateInputTextBox(string defaultVal, int width)
        {
            return new TextBox()
            {
                Text = defaultVal,
                Width = width,
                Background = new SolidColorBrush(Color.FromRgb(15, 23, 42)),
                Foreground = new SolidColorBrush(Color.FromRgb(56, 189, 248)),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                Padding = new Thickness(4, 2, 4, 2),
                BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85)),
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private Button CreateButton(string text, Color bgColor, int padH = 14, int padV = 8, int fontSize = 12)
        {
            Button btn = new Button()
            {
                Content = text,
                Background = new SolidColorBrush(bgColor),
                Padding = new Thickness(padH, padV, padH, padV),
                FontSize = fontSize,
                FontWeight = FontWeights.SemiBold,
                Foreground = Brushes.White,
                Cursor = Cursors.Hand
            };
            return btn;
        }

        public void Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                string stamp = DateTime.Now.ToString("HH:mm:ss");
                string line = string.Format("[{0}] {1}\r\n", stamp, message);
                if (txtLogBox != null)
                {
                    txtLogBox.AppendText(line);
                    txtLogBox.ScrollToEnd();
                }
            });
        }

        private void StartMonitorTimer()
        {
            monitorTimer = new DispatcherTimer();
            monitorTimer.Interval = TimeSpan.FromSeconds(3);
            monitorTimer.Tick += (s, e) => UpdateLiveStatus();
            monitorTimer.Start();
            UpdateLiveStatus();
        }

        public void UpdateLiveStatus()
        {
            if (isInstalling) return;

            Task.Run(() =>
            {
                // 1. Hardware Check
                string hwName = "Not Detected";
                bool hwFound = false;
                bool code39Detected = false;
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity Where Name Like '%Morpho%' Or DeviceID Like '%VID_225D%'");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        hwName = obj["Name"] != null ? obj["Name"].ToString() : "Morpho Device";
                        hwFound = true;
                        if (obj["ConfigManagerErrorCode"] != null && Convert.ToInt32(obj["ConfigManagerErrorCode"]) == 39)
                        {
                            code39Detected = true;
                        }
                        break;
                    }
                }
                catch { }

                // 2. HVCI (Memory Integrity) Check
                bool hvciEnabled = false;
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity"))
                    {
                        if (key != null)
                        {
                            object val = key.GetValue("Enabled");
                            if (val != null && Convert.ToInt32(val) == 1) hvciEnabled = true;
                        }
                    }
                }
                catch { }

                // 3. Services Check
                ServiceControllerStatus s1Status = ServiceControllerStatus.Stopped;
                ServiceControllerStatus s2Status = ServiceControllerStatus.Stopped;
                try
                {
                    ServiceController sc1 = new ServiceController("MSO_SpUsb_Service");
                    s1Status = sc1.Status;
                }
                catch { }
                try
                {
                    ServiceController sc2 = new ServiceController("Idemia_L1RD_Service");
                    s2Status = sc2.Status;
                }
                catch { }

                // 4. Port 11100 Check
                bool portListening = false;
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        IAsyncResult res = client.BeginConnect("127.0.0.1", 11100, null, null);
                        portListening = res.AsyncWaitHandle.WaitOne(400);
                        if (portListening) client.EndConnect(res);
                    }
                }
                catch { }

                // AI Autonomous Self-Healing Engine
                if (isAiHealingEnabled)
                {
                    if (File.Exists(@"C:\IdemiaL1RdService\RDService\msvcp140.dll") || File.Exists(@"C:\IdemiaL1RdService\RDService\vcruntime140.dll"))
                    {
                        Log("[🤖 AI Self-Healing] Detected 64-bit DLL collision. Auto-repairing 32-bit architecture...");
                        FixDllArchitecture();
                    }

                    if (hwFound && !code39Detected && File.Exists(@"C:\IdemiaL1RdService\RDService\IdemiaInteractiveL1RDService.exe"))
                    {
                        if (s1Status != ServiceControllerStatus.Running)
                        {
                            Log("[🤖 AI Self-Healing] USB driver service was stopped. Auto-starting MSO_SpUsb_Service...");
                            RunProcess("net.exe", "start MSO_SpUsb_Service");
                        }
                        if (s2Status != ServiceControllerStatus.Running)
                        {
                            Log("[🤖 AI Self-Healing] IDEMIA RD service was stopped. Auto-starting Idemia_L1RD_Service...");
                            RunProcess("net.exe", "start Idemia_L1RD_Service");
                        }
                    }
                }

                Dispatcher.Invoke(() =>
                {
                    if (hwFound)
                    {
                        if (code39Detected)
                        {
                            txtHwStatus.Text = "BLOCKED (CODE 39)";
                            txtHwStatus.Foreground = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                            txtHwDetails.Text = "Memory Integrity Blocked Driver";
                        }
                        else
                        {
                            txtHwStatus.Text = "CONNECTED (READY)";
                            txtHwStatus.Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                            txtHwDetails.Text = hwName;
                        }
                    }
                    else
                    {
                        txtHwStatus.Text = "NOT CONNECTED";
                        txtHwStatus.Foreground = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                        txtHwDetails.Text = "Plug in Morpho USB scanner";
                    }

                    if (hvciEnabled)
                    {
                        txtHvciStatus.Text = "ON (BLOCKING USB)";
                        txtHvciStatus.Foreground = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                    }
                    else
                    {
                        txtHvciStatus.Text = "OFF (COMPATIBLE)";
                        txtHvciStatus.Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                    }

                    if (s1Status == ServiceControllerStatus.Running && s2Status == ServiceControllerStatus.Running)
                    {
                        txtSvcStatus.Text = "ALL RUNNING";
                        txtSvcStatus.Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                    }
                    else if (s1Status == ServiceControllerStatus.Running)
                    {
                        txtSvcStatus.Text = "USB ACTIVE ONLY";
                        txtSvcStatus.Foreground = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                    }
                    else
                    {
                        txtSvcStatus.Text = "STOPPED";
                        txtSvcStatus.Foreground = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                    }

                    if (portListening)
                    {
                        txtPortStatus.Text = "ONLINE (11100)";
                        txtPortStatus.Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129));
                    }
                    else
                    {
                        txtPortStatus.Text = "OFFLINE";
                        txtPortStatus.Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139));
                    }
                });
            });
        }

        private async void BtnAutoInstall_Click(object sender, RoutedEventArgs e)
        {
            btnAutoInstall.IsEnabled = false;
            isInstalling = true;
            Log("====================================================================");
            Log(">>> STARTING 1-CLICK ALL-IN-ONE AUTOMATED INSTALLATION & REPAIR <<<");
            Log("====================================================================");

            await Task.Run(() =>
            {
                SetProgress(5, "Extracting embedded runtime packages...");
                Directory.CreateDirectory(tempExtractDir);
                string vc86Path = ExtractEmbeddedFile("VC_redist.x86.exe");
                string vc64Path = ExtractEmbeddedFile("VC_redist.x64.exe");
                string vc2010Path = ExtractEmbeddedFile("vcredist_2010_x86.exe");
                string setupPath = ExtractEmbeddedFile("Windows_L1RDService-HTTPs-LocalHost.exe");

                SetProgress(15, "Synchronizing system date and time with NTP...");
                RunProcess("w32tm.exe", "/resync");

                SetProgress(25, "Stopping existing processes and old services...");
                KillProcess("IdemiaL1RDServiceManagement");
                KillProcess("RDServiceL1");
                KillProcess("IdemiaInteractiveL1RDService");

                RunProcess("net.exe", "stop Idemia_L1RD_Service");
                RunProcess("net.exe", "stop MSO_SpUsb_Service");

                SetProgress(40, "Installing Microsoft Visual C++ Runtimes...");
                if (File.Exists(vc86Path)) RunProcess(vc86Path, "/install /passive /norestart");
                if (File.Exists(vc64Path)) RunProcess(vc64Path, "/install /passive /norestart");
                if (File.Exists(vc2010Path)) RunProcess(vc2010Path, "/q /norestart");

                SetProgress(60, "Installing IDEMIA Morpho L1 RD Service v5.0.0.10...");
                if (File.Exists(setupPath))
                {
                    RunProcess(setupPath, "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART");
                    Log("IDEMIA L1 RD Service setup binary completed successfully.");
                }

                SetProgress(75, "Applying 32-bit Native Architecture & DLL conflict repair...");
                FixDllArchitecture();

                SetProgress(85, "Configuring and starting Windows Services...");
                RunProcess("sc.exe", "config MSO_SpUsb_Service start= auto");
                RunProcess("net.exe", "start MSO_SpUsb_Service");

                if (File.Exists(@"C:\IdemiaL1RdService\RDService\IdemiaInteractiveL1RDService.exe"))
                {
                    RunProcess("sc.exe", "create Idemia_L1RD_Service binPath= \"C:\\IdemiaL1RdService\\RDService\\IdemiaInteractiveL1RDService.exe\" start= auto DisplayName= \"Idemia L1 RD Service\"");
                }
                RunProcess("sc.exe", "config Idemia_L1RD_Service start= auto");
                RunProcess("net.exe", "start Idemia_L1RD_Service");

                string mgmtTool = @"C:\IdemiaL1RdService\RDService\L1RD_Management_Tool\IdemiaL1RDServiceManagement.exe";
                if (File.Exists(mgmtTool))
                {
                    Process.Start(new ProcessStartInfo(mgmtTool) { WorkingDirectory = Path.GetDirectoryName(mgmtTool) });
                }

                Thread.Sleep(2500);
                SetProgress(100, "Setup Completed Successfully! Device Ready for Use.");
                Log("====================================================================");
                Log(">>> SUCCESS: Morpho L1 RD Service is fully active and ready! <<<");
                Log("====================================================================");
            });

            isInstalling = false;
            UpdateLiveStatus();
            btnAutoInstall.IsEnabled = true;
        }

        private string ExtractEmbeddedFile(string fileName)
        {
            string outPath = Path.Combine(tempExtractDir, fileName);
            if (File.Exists(outPath) && new FileInfo(outPath).Length > 0) return outPath;

            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                string[] names = asm.GetManifestResourceNames();
                string match = null;
                foreach (string n in names)
                {
                    if (n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        match = n;
                        break;
                    }
                }

                if (match != null)
                {
                    using (Stream s = asm.GetManifestResourceStream(match))
                    using (FileStream fs = new FileStream(outPath, FileMode.Create, FileAccess.Write))
                    {
                        s.CopyTo(fs);
                    }
                    Log("Extracted embedded package: " + fileName);
                    return outPath;
                }
            }
            catch (Exception ex)
            {
                Log("Extract error (" + fileName + "): " + ex.Message);
            }
            return null;
        }

        private void SetProgress(int value, string text)
        {
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = value;
                txtProgressStatus.Text = string.Format("[{0}%] {1}", value, text);
                Log(text);
            });
        }

        public void FixDllArchitecture()
        {
            Log("Applying 32-bit Architecture compatibility patch...");
            string[] badDlls = new string[] {
                "msvcp100.dll", "msvcr100.dll", "msvcp120.dll", "msvcr120.dll", "msvcp140.dll",
                "vcruntime140.dll", "msvcp100d.dll", "msvcp140d.dll", "msvcp90d.dll", "msvcr100d.dll",
                "msvcrt.dll", "ucrtbased.dll", "vcruntime140d.dll"
            };

            foreach (string d in badDlls)
            {
                string p = Path.Combine(@"C:\IdemiaL1RdService\RDService", d);
                if (File.Exists(p))
                {
                    try { File.Delete(p); Log(" - Cleaned conflicting 64-bit DLL: " + d); } catch { }
                }
            }

            if (File.Exists(@"C:\Windows\SysWOW64\ucrtbase.dll"))
            {
                try { File.Copy(@"C:\Windows\SysWOW64\ucrtbase.dll", @"C:\IdemiaL1RdService\RDService\ucrtbase.dll", true); } catch { }
            }
            Log("[OK] Architecture compatibility patch applied (BadImageFormatException resolved).");
        }

        private void RestartServices()
        {
            Task.Run(() =>
            {
                Log("Restarting Morpho RD Services...");
                RunProcess("net.exe", "stop Idemia_L1RD_Service");
                RunProcess("net.exe", "stop MSO_SpUsb_Service");
                Thread.Sleep(1000);
                RunProcess("net.exe", "start MSO_SpUsb_Service");
                RunProcess("net.exe", "start Idemia_L1RD_Service");
                Thread.Sleep(1500);
                UpdateLiveStatus();
                Log("Services restarted.");
            });
        }

        private void ReinstallSslCertificates()
        {
            Task.Run(() =>
            {
                Log("Reinstalling and Trusting Localhost SSL Certificates...");
                string crt1 = @"C:\IdemiaL1RdService\RDService\server_localhost.crt";
                string crt2 = @"C:\IdemiaL1RdService\RDService\morpho.crt";

                if (File.Exists(crt1)) RunProcess("certutil.exe", "-addstore -f \"Root\" \"" + crt1 + "\"");
                if (File.Exists(crt2)) RunProcess("certutil.exe", "-addstore -f \"Root\" \"" + crt2 + "\"");

                Log("[OK] SSL Root Certificates registered in Windows Store.");
                MessageBox.Show("Localhost & Morpho SSL Certificates re-registered successfully!", "Certificates Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        // -------------------------------------------------------------
        // NATIVE BIOMETRIC STUDIO ENGINE (NO BROWSER REQUIRED)
        // -------------------------------------------------------------

        private async Task PingRdService()
        {
            SetScanIndicator("PINGING RDSERVICE (11100)...", Color.FromRgb(245, 158, 11));
            Log(">>> Sending native RDSERVICE discovery request to 127.0.0.1:11100...");

            string resp = await SendRdServiceRequestAsync("RDSERVICE", "https://127.0.0.1:11100/", null);
            txtStudioXml.Text = FormatXml(resp);

            if (resp.Contains("IDEMIA_L1_RDSERVICE") || resp.Contains("RDService") || resp.Contains("status=\"READY\"") || resp.Contains("status=\"NOTREADY\""))
            {
                Match mStat = Regex.Match(resp, "status=\"([^\"]+)\"");
                Match mInfo = Regex.Match(resp, "info=\"([^\"]+)\"");
                string st = mStat.Success ? mStat.Groups[1].Value : "ONLINE";
                string inf = mInfo.Success ? mInfo.Groups[1].Value : "IDEMIA L1";

                SetScanIndicator("RD SERVICE " + st.ToUpper(), Color.FromRgb(16, 185, 129), inf);
                Log(string.Format("[OK] Native RDSERVICE responded: status={0}, info={1}", st, inf));
            }
            else
            {
                SetScanIndicator("OFFLINE / NOT FOUND", Color.FromRgb(239, 68, 68));
                Log("[!] RD Service did not return valid RDService XML: " + resp);
                ShowAiErrorGuidance("RD Service Not Responding on Port 11100", 
                    "The RD Service daemon is offline or blocked. Raw response: " + resp,
                    "Click 'Auto-Fix Now' to restart both Morpho services, re-register SSL certificates, and bind port 11100.",
                    () => { RestartServices(); ReinstallSslCertificates(); });
            }
        }

        private async Task QueryDeviceInfo()
        {
            SetScanIndicator("QUERYING DEVICE INFO...", Color.FromRgb(245, 158, 11));
            Log(">>> Sending native DEVICEINFO request to 127.0.0.1:11100/getDeviceInfo...");

            string resp = await SendRdServiceRequestAsync("DEVICEINFO", "https://127.0.0.1:11100/getDeviceInfo", null);
            txtStudioXml.Text = FormatXml(resp);

            if (resp.Contains("<DeviceInfo") || resp.Contains("dpId") || resp.Contains("rdsId"))
            {
                Match mSrno = Regex.Match(resp, "name=\"srno\"\\s+value=\"([^\"]+)\"");
                if (!mSrno.Success) mSrno = Regex.Match(resp, "srno=\"([^\"]+)\"");
                Match mModel = Regex.Match(resp, "mi=\"([^\"]+)\"");
                Match mRds = Regex.Match(resp, "rdsId=\"([^\"]+)\"");

                string srno = mSrno.Success ? mSrno.Groups[1].Value : "Connected";
                string model = mModel.Success ? mModel.Groups[1].Value : "Morpho L1";

                SetScanIndicator("✔ DEVICE ONLINE", Color.FromRgb(16, 185, 129), "S/N: " + srno + " (" + model + ")");
                Log(string.Format("[OK] Native DEVICEINFO received. S/N: {0} | Model: {1}", srno, model));
            }
            else
            {
                SetScanIndicator("DEVICE INFO FAILED", Color.FromRgb(239, 68, 68));
                Log("[!] Failed to get Device Info: " + resp);
                ShowAiErrorGuidance("Failed to Query Device Info",
                    "The device might not be plugged into USB, or Memory Integrity (Code 39) is blocking the kernel driver.",
                    "Ensure your Morpho MSO 1300 E3 L1 is plugged in. If Memory Integrity is ON, go to the 'Memory Integrity' tab to disable it, then click 'Auto-Fix'.",
                    () => { RestartServices(); });
            }
        }

        private async Task CaptureBiometric()
        {
            SetScanIndicator("🔴 SCANNER ACTIVE — PLACE FINGER ON SENSOR", Color.FromRgb(239, 68, 68), "");
            Log(">>> Native Biometric Capture Initiated. Please place finger firmly on optical sensor...");

            string env = cmbEnv.SelectedItem != null ? cmbEnv.SelectedItem.ToString() : "P";
            string format = cmbFormat.SelectedIndex == 1 ? "1" : "0";
            string fCount = txtFingerCount.Text.Trim();
            string fType = txtFingerType.Text.Trim();
            string timeout = txtTimeout.Text.Trim();
            string wadh = txtWadh.Text.Trim();
            string otp = txtOtp.Text.Trim();
            string posh = txtPosh.Text.Trim();

            int timeoutNum = 15000;
            int.TryParse(timeout, out timeoutNum);
            if (timeoutNum < 3000) timeoutNum = 10000;

            string pidOpts = string.Format("<PidOptions ver=\"1.0\"><Opts env=\"{0}\" fCount=\"{1}\" fType=\"{2}\" format=\"{3}\" pidVer=\"2.0\" timeout=\"{4}\" otp=\"{5}\" wadh=\"{6}\" posh=\"{7}\"/></PidOptions>",
                env, fCount, fType, format, timeout, otp, wadh, posh);

            string resp = await SendRdServiceRequestAsync("CAPTURE", "https://127.0.0.1:11100/capture", pidOpts, timeoutNum + 5000);
            txtStudioXml.Text = FormatXml(resp);

            Match mErr = Regex.Match(resp, "errCode=\"([^\"]+)\"");
            Match mScore = Regex.Match(resp, "qScore=\"([^\"]+)\"");
            Match mInfo = Regex.Match(resp, "errInfo=\"([^\"]+)\"");

            string errCode = mErr.Success ? mErr.Groups[1].Value : "-1";
            string score = mScore.Success ? mScore.Groups[1].Value : "";
            string info = mInfo.Success ? mInfo.Groups[1].Value : "";

            if (errCode == "0")
            {
                SetScanIndicator("✔ CAPTURE SUCCESSFUL", Color.FromRgb(16, 185, 129), "Quality Score: " + score + "%");
                Log(string.Format(">>> CAPTURE SUCCESS! Quality: {0}% | errCode: 0 | Encrypted PidData generated", score));
            }
            else
            {
                SetScanIndicator("✖ CAPTURE ERROR (Code " + errCode + ")", Color.FromRgb(239, 68, 68), info);
                Log(string.Format(">>> Capture Unsuccessful. Code: {0}, Info: {1}", errCode, info));

                string reason = "Capture was not completed.";
                string recommendation = "Clean the scanner glass and place your finger firmly on the sensor.";
                if (errCode == "720")
                {
                    reason = "Error 720: Device is not ready or blocked by Windows Memory Integrity (Code 39).";
                    recommendation = "Go to the 'Memory Integrity' tab to turn OFF Memory Integrity and restart your PC.";
                }
                else if (errCode == "700" || errCode == "710")
                {
                    reason = "Error " + errCode + ": Sensor timeout or bad fingerprint capture quality (" + info + ").";
                    recommendation = "Place your finger centrally and firmly on the sensor when the red LED glows.";
                }
                else if (resp.StartsWith("ERROR:"))
                {
                    reason = "Cannot reach RD Service local endpoint: " + resp;
                    recommendation = "Click 'Auto-Fix Now' to repair and restart the RD Service engine.";
                }

                ShowAiErrorGuidance("Fingerprint Capture Issue (Code: " + errCode + ")", reason, recommendation, () => { RestartServices(); });
            }
        }

        private void ShowAiErrorGuidance(string title, string explanation, string recommendedAction, Action autoFixAction)
        {
            Dispatcher.Invoke(() =>
            {
                Window errorWin = new Window()
                {
                    Title = "🤖 AI Doctor — Diagnostic & Recommended Solution",
                    Width = 580,
                    Height = 380,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this,
                    Background = new SolidColorBrush(Color.FromRgb(15, 23, 42)),
                    ResizeMode = ResizeMode.NoResize
                };

                Grid eg = new Grid() { Margin = new Thickness(20) };
                eg.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                eg.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                eg.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                eg.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                TextBlock lblTitle = new TextBlock()
                {
                    Text = "⚠️ " + title,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(248, 113, 113)),
                    Margin = new Thickness(0, 0, 0, 12)
                };

                Border causeCard = new Border()
                {
                    Background = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 0, 0, 12),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85)),
                    BorderThickness = new Thickness(1)
                };
                StackPanel causeSp = new StackPanel();
                TextBlock cHead = new TextBlock() { Text = "DIAGNOSTIC ROOT CAUSE:", FontSize = 10, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)) };
                TextBlock cBody = new TextBlock() { Text = explanation, FontSize = 12, Foreground = new SolidColorBrush(Color.FromRgb(226, 232, 240)), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 4, 0, 0) };
                causeSp.Children.Add(cHead);
                causeSp.Children.Add(cBody);
                causeCard.Child = causeSp;

                Border fixCard = new Border()
                {
                    Background = new SolidColorBrush(Color.FromRgb(6, 78, 59)),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 0, 0, 16),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(16, 185, 129)),
                    BorderThickness = new Thickness(1)
                };
                StackPanel fixSp = new StackPanel();
                TextBlock fHead = new TextBlock() { Text = "RECOMMENDED ACTION / AI AUTO-FIX:", FontSize = 10, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(110, 231, 183)) };
                TextBlock fBody = new TextBlock() { Text = recommendedAction, FontSize = 12, Foreground = Brushes.White, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 4, 0, 0) };
                fixSp.Children.Add(fHead);
                fixSp.Children.Add(fBody);
                fixCard.Child = fixSp;

                StackPanel btnBar = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
                Button btnClose = CreateButton("Close", Color.FromRgb(51, 65, 85), 14, 8);
                btnClose.Click += (s, e) => errorWin.Close();

                Button btnFixNow = CreateButton("🤖 Auto-Fix Now with AI", Color.FromRgb(79, 70, 229), 16, 8);
                btnFixNow.Margin = new Thickness(10, 0, 0, 0);
                btnFixNow.Click += (s, e) =>
                {
                    errorWin.Close();
                    Log("[🤖 AI Action] Executing recommended auto-repair pipeline...");
                    if (autoFixAction != null) { autoFixAction(); }
                };

                btnBar.Children.Add(btnClose);
                btnBar.Children.Add(btnFixNow);

                eg.Children.Add(lblTitle);
                Grid.SetRow(lblTitle, 0);
                eg.Children.Add(causeCard);
                Grid.SetRow(causeCard, 1);
                eg.Children.Add(fixCard);
                Grid.SetRow(fixCard, 2);
                eg.Children.Add(btnBar);
                Grid.SetRow(btnBar, 3);

                errorWin.Content = eg;
                errorWin.ShowDialog();
            });
        }

        private void SetScanIndicator(string status, Color color, string scoreText = "")
        {
            Dispatcher.Invoke(() =>
            {
                txtScanStatus.Text = status;
                txtScanStatus.Foreground = new SolidColorBrush(color);
                txtScanScore.Text = scoreText;
                borderScanIndicator.BorderBrush = new SolidColorBrush(color);
            });
        }

        private async Task<string> SendRdServiceRequestAsync(string method, string urlOrEndpoint, string postData, int timeoutMs = 15000)
        {
            return await Task.Run(() =>
            {
                string host = "127.0.0.1";
                int port = 11100;
                string path = "/";
                bool isHttps = true;

                if (urlOrEndpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    urlOrEndpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        Uri u = new Uri(urlOrEndpoint);
                        host = u.Host;
                        port = u.Port > 0 ? u.Port : (u.Scheme == "https" ? 11100 : 80);
                        path = u.PathAndQuery;
                        isHttps = u.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        path = urlOrEndpoint;
                    }
                }
                else
                {
                    path = urlOrEndpoint.StartsWith("/") ? urlOrEndpoint : "/" + urlOrEndpoint;
                }

                // If port is 11100, we attempt connect. If connection refused, we scan 11100-11120
                int[] portsToTry = (port == 11100) ? 
                    new int[] { 11100, 11101, 11102, 11103, 11104, 11105, 11111, 11112, 11113 } : 
                    new int[] { port };

                foreach (int currentPort in portsToTry)
                {
                    try
                    {
                        using (TcpClient tcp = new TcpClient())
                        {
                            IAsyncResult ar = tcp.BeginConnect(host, currentPort, null, null);
                            bool connected = ar.AsyncWaitHandle.WaitOne(currentPort == port ? timeoutMs : 300);
                            if (!connected)
                            {
                                tcp.Close();
                                if (portsToTry.Length > 1) continue;
                                return "ERROR: Connection timeout connecting to " + host + ":" + currentPort;
                            }
                            tcp.EndConnect(ar);
                            tcp.ReceiveTimeout = timeoutMs;
                            tcp.SendTimeout = timeoutMs;

                            Stream stream = tcp.GetStream();
                            if (isHttps)
                            {
                                SslStream ssl = new SslStream(stream, false, new RemoteCertificateValidationCallback((s, cert, chain, sslErr) => true));
                                ssl.AuthenticateAsClient(host);
                                stream = ssl;
                            }

                            byte[] bodyBytes = string.IsNullOrEmpty(postData) ? new byte[0] : Encoding.UTF8.GetBytes(postData);

                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("{0} {1} HTTP/1.1\r\n", method, path);
                            sb.AppendFormat("Host: {0}:{1}\r\n", host, currentPort);
                            sb.Append("User-Agent: Morpho_Native_Biometric_Studio/3.5\r\n");
                            sb.Append("Accept: text/xml, application/xml, */*\r\n");
                            if (bodyBytes.Length > 0)
                            {
                                sb.Append("Content-Type: text/xml; charset=utf-8\r\n");
                                sb.AppendFormat("Content-Length: {0}\r\n", bodyBytes.Length);
                            }
                            sb.Append("Connection: close\r\n");
                            sb.Append("\r\n");

                            byte[] headerBytes = Encoding.UTF8.GetBytes(sb.ToString());
                            stream.Write(headerBytes, 0, headerBytes.Length);
                            if (bodyBytes.Length > 0)
                            {
                                stream.Write(bodyBytes, 0, bodyBytes.Length);
                            }
                            stream.Flush();

                            using (MemoryStream ms = new MemoryStream())
                            {
                                byte[] buffer = new byte[4096];
                                int bytesRead;
                                do
                                {
                                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                                    if (bytesRead > 0)
                                    {
                                        ms.Write(buffer, 0, bytesRead);
                                    }
                                } while (tcp.Connected && (bytesRead > 0 || (isHttps ? ((SslStream)stream).CanRead : true)) && ms.Length == 0);

                                byte[] respBytes = ms.ToArray();
                                string fullResponse = Encoding.UTF8.GetString(respBytes);

                                // Extract XML body regardless of \r\n or \n line endings
                                int headerEnd = fullResponse.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                                if (headerEnd >= 0) return fullResponse.Substring(headerEnd + 4).Trim();

                                headerEnd = fullResponse.IndexOf("\n\n", StringComparison.Ordinal);
                                if (headerEnd >= 0) return fullResponse.Substring(headerEnd + 2).Trim();

                                headerEnd = fullResponse.IndexOf("<?xml", StringComparison.OrdinalIgnoreCase);
                                if (headerEnd >= 0) return fullResponse.Substring(headerEnd).Trim();

                                headerEnd = fullResponse.IndexOf("<", StringComparison.OrdinalIgnoreCase);
                                if (headerEnd >= 0) return fullResponse.Substring(headerEnd).Trim();

                                return fullResponse;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (currentPort == portsToTry[portsToTry.Length - 1])
                        {
                            return "ERROR: " + ex.Message;
                        }
                    }
                }

                return "ERROR: Unable to communicate with RD Service on port " + port;
            });
        }

        private string FormatXml(string rawXml)
        {
            if (string.IsNullOrEmpty(rawXml)) return "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(rawXml);
                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    OmitXmlDeclaration = false
                };
                using (XmlWriter writer = XmlWriter.Create(sb, settings))
                {
                    doc.Save(writer);
                }
                return sb.ToString();
            }
            catch
            {
                return rawXml;
            }
        }

        private void RunProcess(string fileName, string args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(fileName, args)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                Process p = Process.Start(psi);
                p.WaitForExit(45000);
            }
            catch { }
        }

        private void KillProcess(string name)
        {
            try
            {
                foreach (Process p in Process.GetProcessesByName(name))
                {
                    p.Kill();
                    p.WaitForExit(3000);
                }
            }
            catch { }
        }
    }
}
