using ImageCompressorDemo;
using System.ServiceProcess;
using System.Text.Json;

namespace ImageCompressor.UI
{
    public partial class Form1 : Form
    {
        private PictureBox loadingGif;
        private TextBox txtPath;
        private TextBox txtWidth;
        private TextBox txtHeight;
        private TextBox txtSize;
        private ComboBox cmbUnitW;
        private ComboBox cmbUnitH;
        private ComboBox cmbUnitS;
        private Button btnBrowse;
        private Button btnCompress;
        private Button btnCancel;

        private string selectedImagePath = "";
        private int imgHeight = 0;
        private int imgWidth = 0;

        public Form1(string path)
        {
            InitializeComponents(path);
        }

        private void InitializeComponents(string argPath)
        {
            Text = "Image Compressor";
            Size = new Size(500, 350);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            Label lblPath = new Label() { Text = "File Path:", Left = 20, Top = 30, Width = 100 };
            txtPath = new TextBox() { Left = 120, Top = 25, Width = 260 };
            btnBrowse = new Button() { Text = "Browse", Left = 390, Top = 25, Width = 70, Height = 30 };
            btnBrowse.Click += BtnBrowse_Click;

            Label lblWidth = new Label() { Text = "Width:", Left = 20, Top = 70, Width = 100 };
            txtWidth = new TextBox() { Left = 120, Top = 65, Width = 100 };
            cmbUnitW = new ComboBox() { Left = 230, Top = 65, Width = 70 };
            cmbUnitW.Items.AddRange(new string[] { "px", "cm" });
            cmbUnitW.SelectedIndex = 0;

            Label lblHeight = new Label() { Text = "Height:", Left = 20, Top = 110, Width = 100 };
            txtHeight = new TextBox() { Left = 120, Top = 105, Width = 100 };
            cmbUnitH = new ComboBox() { Left = 230, Top = 105, Width = 70 };
            cmbUnitH.Items.AddRange(new string[] { "px", "cm" });
            cmbUnitH.SelectedIndex = 0;

            Label lblSize = new Label() { Text = "Size:", Left = 20, Top = 150, Width = 100 };
            txtSize = new TextBox() { Left = 120, Top = 145, Width = 70 };
            cmbUnitS = new ComboBox() { Left = 230, Top = 145, Width = 70 };
            cmbUnitS.Items.AddRange(new string[] { "KB", "MB" });
            cmbUnitS.SelectedIndex = 0;

            btnCompress = new Button() { Text = "Compress", Left = 120, Top = 190, Width = 100, Height = 30 };
            btnCompress.Click += BtnCompress_Click;

            btnCancel = new Button() { Text = "Cancel", Left = 230, Top = 190, Width = 100, Height = 30 };
            btnCancel.Click += (s, e) => Close();

            loadingGif = new PictureBox()
            {
                Left = 200,
                Top = 230,
                Width = 80,
                Height = 80,
                Visible = false,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            selectedImagePath = argPath;

            Controls.AddRange(new Control[]
            {
                lblPath, txtPath, btnBrowse,
                lblWidth, txtWidth, cmbUnitW,
                lblHeight, txtHeight, cmbUnitH,
                lblSize, txtSize,cmbUnitS,
                btnCompress, btnCancel,
                loadingGif
            });

            if (!string.IsNullOrEmpty(selectedImagePath))
                LoadImageData();
        }

        private void BtnBrowse_Click(object? sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                selectedImagePath = ofd.FileName;
                LoadImageData();
            }
        }

        private void LoadImageData()
        {
            txtPath.Text = selectedImagePath;
            FileInfo fi = new FileInfo(selectedImagePath);
            using (var img = Image.FromFile(selectedImagePath))
            {
                imgHeight = img.Height;
                imgWidth = img.Width;
                txtHeight.Text = imgHeight.ToString();
                txtWidth.Text = imgWidth.ToString();
                txtSize.Text = (fi.Length / 1024).ToString();
            }
        }

        private async void BtnCompress_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPath.Text) || !File.Exists(txtPath.Text))
            {
                MessageBox.Show("Please select a valid image file.");
                return;
            }

            if (!int.TryParse(txtWidth.Text, out int width) ||
                !int.TryParse(txtHeight.Text, out int height))
            {
                MessageBox.Show("Invalid width or height.");
                return;
            }

            if (!int.TryParse(txtSize.Text, out int size))
            {
                MessageBox.Show("Invalid size.");
                return;
            }

            loadingGif.Visible = true;
            btnCompress.Enabled = false;
            btnCancel.Enabled = false;

            GetInputData(ref height, ref width, ref size);

            try
            {
                ImageRequest requestData = new ImageRequest
                {
                    InputPath = txtPath.Text,
                    Height = height,
                    Width = width,
                    MaxSizeKB = size
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;

                try
                {
                    ServiceController sc = new ServiceController("ImageCompressorWorkerService");
                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    btnCompress.Enabled = true;
                    btnCancel.Enabled = true;
                    return;
                }
                //TODO : Make API Call If Service Not Exists
                using (var client = new HttpClient())
                {
                    response = await client.PostAsync("http://localhost:21922/api/ImageCompression/Compress", content);
                }

                if (!response.IsSuccessStatusCode)
                {
                    Result? result = JsonSerializer.Deserialize<Result>(await response.Content.ReadAsStringAsync());
                    MessageBox.Show("Error Occured : " + result?.Message);
                }

                loadingGif.Visible = false;
                Close();
            }
            catch (Exception ex)
            {
                loadingGif.Visible = false;
                MessageBox.Show("Error: " + ex.Message);
                btnCompress.Enabled = true;
                btnCancel.Enabled = true;
            }
        }

        private void GetInputData(ref int height, ref int width, ref int size)
        {
            if (cmbUnitH.SelectedIndex == 1)
                height *= 37;
            else
                height = (imgHeight == height) ? 0 : height;

            if (cmbUnitW.SelectedIndex == 1)
                width *= 37;
            else
                width = (imgWidth == width) ? 0 : width;

            if (cmbUnitS.SelectedIndex == 1)
                size /= 1024;
        }

    }
}
