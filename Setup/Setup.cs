namespace Setup
{
    public enum SetupScreen
    {
        Welcome,
        AlreadyInstalled,
        Destination,
        Progress,
        Finish
    }
    public partial class Setup : Form
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private SetupScreen currentScreen = SetupScreen.Welcome;
        private readonly RegistryManager registryManager;
        private readonly ServiceManager serviceManager;
        public string finalLabel;
        public string installDir;
        public Setup()
        {
            registryManager = new RegistryManager();
            serviceManager = new ServiceManager();
            InitializeComponent();
            LoadScreen(registryManager.GetInstalledPath() == string.Empty ? SetupScreen.Welcome : SetupScreen.AlreadyInstalled);
        }
        private void LoadScreen(SetupScreen screen)
        {
            currentScreen = screen;

            pnlContent.Controls.Clear();

            switch (screen)
            {
                case SetupScreen.Welcome:
                    LoadWelcomeScreen();
                    btnBack.Enabled = false;
                    btnNext.Enabled = true;
                    break;

                case SetupScreen.AlreadyInstalled:
                    LoadAlreadyInstalledScreen();
                    btnBack.Enabled = true;
                    btnNext.Enabled = true;
                    break;

                case SetupScreen.Destination:
                    LoadDestinationScreen();
                    btnBack.Enabled = true;
                    btnNext.Enabled = true;
                    break;

                case SetupScreen.Progress:
                    LoadProgressScreen();
                    btnBack.Enabled = false;
                    btnNext.Enabled = false;
                    break;

                case SetupScreen.Finish:
                    LoadFinishScreen();
                    btnBack.Enabled = false;
                    btnNext.Enabled = false;
                    btnCancel.Text = "Close";
                    break;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentScreen == SetupScreen.Welcome)
            {
                LoadScreen(SetupScreen.Destination);
            }
            else if (currentScreen == SetupScreen.AlreadyInstalled)
            {
                LoadScreen(SetupScreen.Progress);
                if (rbRemove.Checked)
                {
                    StartUninstallation();
                }
                else
                {
                    StartMaintanance(cts.Token);
                }
            }
            else if (currentScreen == SetupScreen.Destination)
            {
                installDir = txtPath.Text;
                LoadScreen(SetupScreen.Progress);
                StartInstallation(cts.Token);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (currentScreen == SetupScreen.Destination)
                LoadScreen(SetupScreen.Welcome);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (currentScreen == SetupScreen.Progress)
            {
                var res = MessageBox.Show(
                    "Setup is performing important operations.\nDo you really want to cancel?",
                    "Cancel Installation",
                    MessageBoxButtons.YesNo);

                if (res == DialogResult.No) return;

                cts.Cancel();

                lblStage.Text = "Cancelling...";
                Application.DoEvents();

                LoadScreen(SetupScreen.Finish);
                lblFinish.Text = "Setup was cancelled.";
                return;
            }
            else if (currentScreen == SetupScreen.Finish)
            {
                if (chkManual.Checked && chkManual.Visible)
                {
                    string manualPath = Path.Combine(installDir, "ImageCompressor_Manual.html");
                    ProcessUtils.RunProcess("cmd.exe", $"/C start \"\" \"{manualPath}\"");
                }
            }
            Close();
        }

        private void LoadWelcomeScreen()
        {
            btnBack.Visible = false;
            btnNext.Text = "Next";

            Label lbl = new Label()
            {
                Text = "Welcome to the Image Compressor Setup Wizard\nThis wizard will guide you through installation.",
                AutoSize = false,
                Size = new Size(500, 100),
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 10)
            };
            pnlContent.Controls.Add(lbl);
        }
        RadioButton rbRepair;
        RadioButton rbRemove;
        private void LoadAlreadyInstalledScreen()
        {
            btnBack.Visible = false;

            Label lbl = new Label()
            {
                Text = "Image Compressor is already installed.\nChoose an option:",
                AutoSize = false,
                Size = new Size(500, 70),
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 10)
            };

            rbRepair = new RadioButton()
            {
                Text = "Repair Installation",
                Name = "rdbRepair",
                Location = new Point(30, 110),
                Font = new Font("Segoe UI", 9),
                Checked = true
            };

            rbRemove = new RadioButton()
            {
                Text = "Remove Installation",
                Name = "rdbRemove",
                Location = new Point(30, 150),
                Font = new Font("Segoe UI", 9)
            };

            pnlContent.Controls.Add(lbl);
            pnlContent.Controls.Add(rbRepair);
            pnlContent.Controls.Add(rbRemove);

            installDir = registryManager.GetInstalledPath();
        }

        TextBox txtPath;

        private void LoadDestinationScreen()
        {
            btnBack.Visible = true;
            btnNext.Text = "Install";

            Label lbl = new Label()
            {
                Text = "Choose the destination folder:",
                AutoSize = true,
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 11)
            };

            txtPath = new TextBox()
            {
                Text = GetInstallDir(),
                Location = new Point(20, 60),
                Width = 450
            };

            Button btnBrowse = new Button()
            {
                Text = "Browse...",
                Location = new Point(480, 58),
                Size = new Size(100, 30),
                BackColor = Color.White,
            };
            btnBrowse.Click += (s, e) =>
            {
                using FolderBrowserDialog f = new FolderBrowserDialog();
                if (f.ShowDialog() == DialogResult.OK)
                    txtPath.Text = f.SelectedPath;
            };

            Label lblSpace = new Label()
            {
                Text = "Required disk space: 30 MB",
                Location = new Point(20, 120),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
            };

            pnlContent.Controls.Add(lbl);
            pnlContent.Controls.Add(txtPath);
            pnlContent.Controls.Add(btnBrowse);
            pnlContent.Controls.Add(lblSpace);
        }

        ProgressBar progress;
        Label lblStage;

        private void LoadProgressScreen()
        {
            lblStage = new Label()
            {
                Text = "Preparing...",
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 10),
                AutoSize = true
            };

            progress = new ProgressBar()
            {
                Location = new Point(20, 60),
                Width = 550,
                Height = 20,
                Style = ProgressBarStyle.Continuous
            };

            pnlContent.Controls.Add(lblStage);
            pnlContent.Controls.Add(progress);
        }

        Label lblFinish;
        CheckBox chkManual;

        private void LoadFinishScreen()
        {
            btnBack.Visible = false;
            btnNext.Visible = false;

            lblFinish = new Label()
            {
                Text = finalLabel,
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 12),
                AutoSize = true
            };

            chkManual = new CheckBox()
            {
                Text = "Open User Manual",
                Location = new Point(20, 70),
                AutoSize = true,
                Checked = true
            };

            pnlContent.Controls.Add(lblFinish);
            pnlContent.Controls.Add(chkManual);
        }

        private string GetInstallDir()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Vasu Kotadiya", "ImageCompressor");
        }

        private async void StartInstallation(CancellationToken token)
        {
            await Task.Delay(10, token);
            lblStage.Text = "Copying files...";
            if (!ProcessUtils.ExtractAllResources(installDir))
                Rollback();
            progress.Value = 20;

            await Task.Delay(10, token);
            lblStage.Text = "Registering service...";
            if (!serviceManager.RegisterService(installDir))
                Rollback();
            progress.Value = 40;

            await Task.Delay(10, token);
            lblStage.Text = "Starting service...";
            if (!serviceManager.StartService())
                Rollback();
            progress.Value = 60;

            await Task.Delay(10, token);
            lblStage.Text = "Writing registry entries...";
            if (!registryManager.Register(installDir))
                Rollback();
            progress.Value = 80;

            await Task.Delay(100, token);
            lblStage.Text = "Finalizing...";
            progress.Value = 100;

            await Task.Delay(10, token);
            finalLabel = "Setup Completed Successfully.";
            LoadScreen(SetupScreen.Finish);
        }
        private async void StartUninstallation()
        {
            await Task.Delay(1000);
            lblStage.Text = "Stopping service...";
            if (!serviceManager.StopService())
                Rollback();
            progress.Value = 20;

            await Task.Delay(1500);
            lblStage.Text = "Removing service...";
            if (!serviceManager.RemoveService())
                Rollback();
            progress.Value = 40;

            await Task.Delay(1000);
            lblStage.Text = "Deleting registry entries...";
            if (!registryManager.Unregister())
                Rollback();
            progress.Value = 60;

            await Task.Delay(1000);
            lblStage.Text = "Deleting files...";
            if (!ProcessUtils.DeleteAppFiles(installDir))
                Rollback();
            progress.Value = 80;

            await Task.Delay(100);
            lblStage.Text = "Finalizing...";
            progress.Value = 100;

            await Task.Delay(1000);
            finalLabel = "Image Compressor Uninstalled.";

            LoadScreen(SetupScreen.Finish);
            chkManual.Visible = false;
        }
        private async void StartMaintanance(CancellationToken token)
        {
            await Task.Delay(10, token);
            lblStage.Text = "Stopping service...";
            if (!serviceManager.StopService())
                Rollback();
            progress.Value = 20;

            await Task.Delay(10, token);
            lblStage.Text = "Copying files...";
            if (!ProcessUtils.ExtractAllResources(installDir))
                Rollback();
            progress.Value = 40;

            await Task.Delay(10, token);
            lblStage.Text = "Starting service...";
            if (!serviceManager.StartService())
                Rollback();
            progress.Value = 60;

            await Task.Delay(10, token);
            lblStage.Text = "Writing registry entries...";
            if (!registryManager.Register(installDir))
                Rollback();
            progress.Value = 80;

            await Task.Delay(100, token);
            lblStage.Text = "Finalizing...";
            progress.Value = 100;

            await Task.Delay(10, token);
            finalLabel = "Setup Completed Successfully.";
            LoadScreen(SetupScreen.Finish);
        }
        private void Rollback()
        {
            btnCancel.Enabled = false;
            lblStage.Text = "Rolling Back...";
            finalLabel = "Error Ouccured!!!";
            cts.Cancel();
            LoadScreen(SetupScreen.Finish);
            btnCancel.Text = "Close";
            btnCancel.Enabled = true;
        }
    }
}
