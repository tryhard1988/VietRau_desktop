namespace RauViet.ui
{
    partial class QRForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtContent = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.picQR = new System.Windows.Forms.PictureBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.logoBtn = new System.Windows.Forms.Button();
            this.picLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // txtContent
            // 
            this.txtContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtContent.Location = new System.Drawing.Point(12, 12);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(360, 40);
            this.txtContent.TabIndex = 3;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnGenerate.Location = new System.Drawing.Point(380, 12);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(120, 40);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Tạo Mã QR";
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // picQR
            // 
            this.picQR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picQR.Location = new System.Drawing.Point(12, 70);
            this.picQR.Name = "picQR";
            this.picQR.Size = new System.Drawing.Size(360, 360);
            this.picQR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picQR.TabIndex = 1;
            this.picQR.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnSave.Location = new System.Drawing.Point(380, 124);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 40);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save QR";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // logoBtn
            // 
            this.logoBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.logoBtn.Location = new System.Drawing.Point(380, 70);
            this.logoBtn.Name = "logoBtn";
            this.logoBtn.Size = new System.Drawing.Size(120, 40);
            this.logoBtn.TabIndex = 4;
            this.logoBtn.Text = "Logo";
            this.logoBtn.Click += new System.EventHandler(this.logoBtn_Click);
            // 
            // picLogo
            // 
            this.picLogo.Location = new System.Drawing.Point(380, 170);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(38, 40);
            this.picLogo.TabIndex = 5;
            this.picLogo.TabStop = false;
            // 
            // QRForm
            // 
            this.ClientSize = new System.Drawing.Size(548, 531);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.logoBtn);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.picQR);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.txtContent);
            this.Name = "QRForm";
            this.Text = "QR Code Generator";
            ((System.ComponentModel.ISupportInitialize)(this.picQR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.PictureBox picQR;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button logoBtn;
        private System.Windows.Forms.PictureBox picLogo;
    }
}