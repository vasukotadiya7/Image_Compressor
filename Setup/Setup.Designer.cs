
namespace Setup
{
    partial class Setup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Panel pnlTop;
        private Panel pnlContent;
        private Panel pnlBottom;

        private Label lblTitle;
        private PictureBox picLogo;

        private Button btnBack;
        private Button btnNext;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setup));
            pnlTop = new Panel();
            lblTitle = new Label();
            picLogo = new PictureBox();
            pnlContent = new Panel();
            pnlBottom = new Panel();
            btnBack = new Button();
            btnNext = new Button();
            btnCancel = new Button();
            pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            pnlBottom.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.WhiteSmoke;
            pnlTop.Controls.Add(lblTitle);
            pnlTop.Controls.Add(picLogo);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(650, 80);
            pnlTop.TabIndex = 1;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 25);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(431, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "ImageCompressor Setup Wizard";
            // 
            // picLogo
            // 
            picLogo.Image = (Image)resources.GetObject("picLogo.Image");
            picLogo.Location = new Point(560, 10);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(60, 60);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 1;
            picLogo.TabStop = false;
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.White;
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 80);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(650, 340);
            pnlContent.TabIndex = 0;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.WhiteSmoke;
            pnlBottom.Controls.Add(btnBack);
            pnlBottom.Controls.Add(btnNext);
            pnlBottom.Controls.Add(btnCancel);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 420);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(650, 60);
            pnlBottom.TabIndex = 2;
            // 
            // btnBack
            // 
            btnBack.BackColor = Color.White;
            btnBack.Location = new Point(310, 15);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(100, 30);
            btnBack.TabIndex = 0;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += btnBack_Click;
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.White;
            btnNext.Location = new Point(420, 15);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(100, 30);
            btnNext.TabIndex = 1;
            btnNext.Text = "Next";
            btnNext.UseVisualStyleBackColor = false;
            btnNext.Click += btnNext_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.White;
            btnCancel.Location = new Point(530, 15);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 30);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // Setup
            // 
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(650, 480);
            Controls.Add(pnlContent);
            Controls.Add(pnlTop);
            Controls.Add(pnlBottom);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Setup";
            Text = "ImageCompressor Setup";
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            pnlBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

    }
}