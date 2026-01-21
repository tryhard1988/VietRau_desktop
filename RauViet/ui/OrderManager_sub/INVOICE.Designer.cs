
namespace RauViet.ui
{
    partial class INVOICE
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.customerHomeBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.exportCode_cbb = new System.Windows.Forms.ComboBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel5 = new System.Windows.Forms.Panel();
            this.TotalAmount_label = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.CNTSTotal_label = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.NWTotal_label = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.PCSTotal_label = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cartonSizeGV = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cusOrderGV = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cartonSizeGV)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cusOrderGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.customerHomeBtn);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.exportCode_cbb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1084, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(180, 681);
            this.panel1.TabIndex = 9;
            // 
            // customerHomeBtn
            // 
            this.customerHomeBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.customerHomeBtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customerHomeBtn.Location = new System.Drawing.Point(6, 402);
            this.customerHomeBtn.Name = "customerHomeBtn";
            this.customerHomeBtn.Size = new System.Drawing.Size(162, 52);
            this.customerHomeBtn.TabIndex = 54;
            this.customerHomeBtn.Text = "Xuất File Tổng Hợp Theo Nhà";
            this.customerHomeBtn.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(11, 293);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 32);
            this.label3.TabIndex = 53;
            this.label3.Text = "Nhớ Bấm xuất file excel \r\nđể lưu báo cáo đơn hàng";
            // 
            // exportCode_cbb
            // 
            this.exportCode_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exportCode_cbb.FormattingEnabled = true;
            this.exportCode_cbb.Location = new System.Drawing.Point(0, 3);
            this.exportCode_cbb.Name = "exportCode_cbb";
            this.exportCode_cbb.Size = new System.Drawing.Size(160, 21);
            this.exportCode_cbb.TabIndex = 52;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(6, 49);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(6, 336);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(162, 52);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Xuất Invoice";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.TotalAmount_label);
            this.panel5.Controls.Add(this.label12);
            this.panel5.Controls.Add(this.CNTSTotal_label);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.NWTotal_label);
            this.panel5.Controls.Add(this.label11);
            this.panel5.Controls.Add(this.PCSTotal_label);
            this.panel5.Controls.Add(this.label4);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 659);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1084, 22);
            this.panel5.TabIndex = 16;
            // 
            // TotalAmount_label
            // 
            this.TotalAmount_label.AutoSize = true;
            this.TotalAmount_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.TotalAmount_label.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalAmount_label.ForeColor = System.Drawing.Color.MediumPurple;
            this.TotalAmount_label.Location = new System.Drawing.Point(434, 0);
            this.TotalAmount_label.Name = "TotalAmount_label";
            this.TotalAmount_label.Size = new System.Drawing.Size(35, 16);
            this.TotalAmount_label.TabIndex = 96;
            this.TotalAmount_label.Text = "PCS:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Dock = System.Windows.Forms.DockStyle.Left;
            this.label12.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label12.Location = new System.Drawing.Point(322, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(112, 16);
            this.label12.TabIndex = 95;
            this.label12.Text = "Tổng Tiền (CHF):";
            // 
            // CNTSTotal_label
            // 
            this.CNTSTotal_label.AutoSize = true;
            this.CNTSTotal_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.CNTSTotal_label.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CNTSTotal_label.ForeColor = System.Drawing.Color.MediumPurple;
            this.CNTSTotal_label.Location = new System.Drawing.Point(287, 0);
            this.CNTSTotal_label.Name = "CNTSTotal_label";
            this.CNTSTotal_label.Size = new System.Drawing.Size(35, 16);
            this.CNTSTotal_label.TabIndex = 94;
            this.CNTSTotal_label.Text = "PCS:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Left;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(209, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 16);
            this.label6.TabIndex = 93;
            this.label6.Text = "Tổng CNTS:";
            // 
            // NWTotal_label
            // 
            this.NWTotal_label.AutoSize = true;
            this.NWTotal_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.NWTotal_label.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NWTotal_label.ForeColor = System.Drawing.Color.Brown;
            this.NWTotal_label.Location = new System.Drawing.Point(174, 0);
            this.NWTotal_label.Name = "NWTotal_label";
            this.NWTotal_label.Size = new System.Drawing.Size(35, 16);
            this.NWTotal_label.TabIndex = 88;
            this.NWTotal_label.Text = "PCS:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Dock = System.Windows.Forms.DockStyle.Left;
            this.label11.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label11.Location = new System.Drawing.Point(106, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 16);
            this.label11.TabIndex = 87;
            this.label11.Text = "Tổng NW:";
            // 
            // PCSTotal_label
            // 
            this.PCSTotal_label.AutoSize = true;
            this.PCSTotal_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.PCSTotal_label.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PCSTotal_label.ForeColor = System.Drawing.SystemColors.Highlight;
            this.PCSTotal_label.Location = new System.Drawing.Point(71, 0);
            this.PCSTotal_label.Name = "PCSTotal_label";
            this.PCSTotal_label.Size = new System.Drawing.Size(35, 16);
            this.PCSTotal_label.TabIndex = 86;
            this.PCSTotal_label.Text = "PCS:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Left;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 16);
            this.label4.TabIndex = 85;
            this.label4.Text = "Tổng PCS:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(677, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(407, 659);
            this.panel2.TabIndex = 18;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.cartonSizeGV);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 306);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(407, 353);
            this.panel4.TabIndex = 15;
            // 
            // cartonSizeGV
            // 
            this.cartonSizeGV.AllowUserToAddRows = false;
            this.cartonSizeGV.AllowUserToDeleteRows = false;
            this.cartonSizeGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cartonSizeGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cartonSizeGV.Location = new System.Drawing.Point(0, 16);
            this.cartonSizeGV.Name = "cartonSizeGV";
            this.cartonSizeGV.ReadOnly = true;
            this.cartonSizeGV.Size = new System.Drawing.Size(407, 337);
            this.cartonSizeGV.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(203, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "THỐNG KÊ SỐ THÙNG THEO SIZE ";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.cusOrderGV);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(407, 659);
            this.panel3.TabIndex = 14;
            // 
            // cusOrderGV
            // 
            this.cusOrderGV.AllowUserToAddRows = false;
            this.cusOrderGV.AllowUserToDeleteRows = false;
            this.cusOrderGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cusOrderGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cusOrderGV.Location = new System.Drawing.Point(0, 16);
            this.cusOrderGV.Name = "cusOrderGV";
            this.cusOrderGV.ReadOnly = true;
            this.cusOrderGV.Size = new System.Drawing.Size(407, 643);
            this.cusOrderGV.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(255, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "THỐNG KÊ SỐ THÙNG THEO KHÁCH HÀNG";
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
            this.dataGV.Size = new System.Drawing.Size(677, 659);
            this.dataGV.TabIndex = 19;
            // 
            // INVOICE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Name = "INVOICE";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cartonSizeGV)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cusOrderGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ComboBox exportCode_cbb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button customerHomeBtn;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView cartonSizeGV;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView cusOrderGV;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Label NWTotal_label;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label PCSTotal_label;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label TotalAmount_label;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label CNTSTotal_label;
        private System.Windows.Forms.Label label6;
    }
}