
namespace RauViet.ui
{
    partial class AnnualLeaveBalance
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.capphep_btn = new System.Windows.Forms.Button();
            this.status_lb = new System.Windows.Forms.Label();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.ResetPhep_btn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(900, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(364, 681);
            this.panel1.TabIndex = 11;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.ResetPhep_btn);
            this.info_gb.Controls.Add(this.capphep_btn);
            this.info_gb.Location = new System.Drawing.Point(35, 156);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(241, 166);
            this.info_gb.TabIndex = 28;
            this.info_gb.TabStop = false;
            // 
            // capphep_btn
            // 
            this.capphep_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.capphep_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.capphep_btn.Location = new System.Drawing.Point(39, 19);
            this.capphep_btn.Name = "capphep_btn";
            this.capphep_btn.Size = new System.Drawing.Size(169, 58);
            this.capphep_btn.TabIndex = 32;
            this.capphep_btn.Text = "Cấp 1 Phép";
            this.capphep_btn.UseVisualStyleBackColor = false;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(12, 357);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 0);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(900, 681);
            this.dataGV.TabIndex = 12;
            // 
            // ResetPhep_btn
            // 
            this.ResetPhep_btn.BackColor = System.Drawing.Color.OrangeRed;
            this.ResetPhep_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResetPhep_btn.Location = new System.Drawing.Point(39, 83);
            this.ResetPhep_btn.Name = "ResetPhep_btn";
            this.ResetPhep_btn.Size = new System.Drawing.Size(169, 54);
            this.ResetPhep_btn.TabIndex = 33;
            this.ResetPhep_btn.Text = "Reset Phép";
            this.ResetPhep_btn.UseVisualStyleBackColor = false;
            // 
            // AnnualLeaveBalance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Name = "AnnualLeaveBalance";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.info_gb.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Button capphep_btn;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Button ResetPhep_btn;
    }
}