
namespace RauViet.ui
{
    partial class KhoVatTu_GiamSatTiepNhanRau
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
            this.exportCode_cbb = new System.Windows.Forms.ComboBox();
            this.exportExcel_btn = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ngPhuTrach_cbb = new System.Windows.Forms.ComboBox();
            this.ngThuHoach_cbb = new System.Windows.Forms.ComboBox();
            this.saveGlobalGAP_btn = new System.Windows.Forms.Button();
            this.noiNhan_cbb = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.info_gb.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.exportCode_cbb);
            this.panel1.Controls.Add(this.exportExcel_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1393, 43);
            this.panel1.TabIndex = 0;
            // 
            // exportCode_cbb
            // 
            this.exportCode_cbb.BackColor = System.Drawing.Color.White;
            this.exportCode_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exportCode_cbb.FormattingEnabled = true;
            this.exportCode_cbb.Location = new System.Drawing.Point(23, 11);
            this.exportCode_cbb.Name = "exportCode_cbb";
            this.exportCode_cbb.Size = new System.Drawing.Size(168, 21);
            this.exportCode_cbb.TabIndex = 38;
            // 
            // exportExcel_btn
            // 
            this.exportExcel_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.exportExcel_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportExcel_btn.Location = new System.Drawing.Point(197, 6);
            this.exportExcel_btn.Name = "exportExcel_btn";
            this.exportExcel_btn.Size = new System.Drawing.Size(94, 32);
            this.exportExcel_btn.TabIndex = 42;
            this.exportExcel_btn.Text = "Xuất Excel";
            this.exportExcel_btn.UseVisualStyleBackColor = false;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.info_gb);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(1025, 43);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(368, 715);
            this.panel3.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.panel2.Controls.Add(this.dataGV);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 43);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1025, 715);
            this.panel2.TabIndex = 3;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 0);
            this.dataGV.MultiSelect = false;
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGV.Size = new System.Drawing.Size(1025, 715);
            this.dataGV.TabIndex = 73;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.noiNhan_cbb);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Controls.Add(this.saveGlobalGAP_btn);
            this.info_gb.Controls.Add(this.ngThuHoach_cbb);
            this.info_gb.Controls.Add(this.ngPhuTrach_cbb);
            this.info_gb.Controls.Add(this.label8);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Location = new System.Drawing.Point(17, 6);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(322, 170);
            this.info_gb.TabIndex = 29;
            this.info_gb.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Người Thu Hoạch:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(17, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 16);
            this.label8.TabIndex = 33;
            this.label8.Text = "Người Phụ Trách:";
            // 
            // ngPhuTrach_cbb
            // 
            this.ngPhuTrach_cbb.BackColor = System.Drawing.Color.White;
            this.ngPhuTrach_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ngPhuTrach_cbb.FormattingEnabled = true;
            this.ngPhuTrach_cbb.Location = new System.Drawing.Point(148, 21);
            this.ngPhuTrach_cbb.Name = "ngPhuTrach_cbb";
            this.ngPhuTrach_cbb.Size = new System.Drawing.Size(168, 21);
            this.ngPhuTrach_cbb.TabIndex = 39;
            // 
            // ngThuHoach_cbb
            // 
            this.ngThuHoach_cbb.BackColor = System.Drawing.Color.White;
            this.ngThuHoach_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ngThuHoach_cbb.FormattingEnabled = true;
            this.ngThuHoach_cbb.Location = new System.Drawing.Point(148, 55);
            this.ngThuHoach_cbb.Name = "ngThuHoach_cbb";
            this.ngThuHoach_cbb.Size = new System.Drawing.Size(168, 21);
            this.ngThuHoach_cbb.TabIndex = 40;
            // 
            // saveGlobalGAP_btn
            // 
            this.saveGlobalGAP_btn.BackColor = System.Drawing.Color.YellowGreen;
            this.saveGlobalGAP_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveGlobalGAP_btn.Location = new System.Drawing.Point(79, 125);
            this.saveGlobalGAP_btn.Name = "saveGlobalGAP_btn";
            this.saveGlobalGAP_btn.Size = new System.Drawing.Size(157, 38);
            this.saveGlobalGAP_btn.TabIndex = 43;
            this.saveGlobalGAP_btn.Text = "Lưu Global GAP";
            this.saveGlobalGAP_btn.UseVisualStyleBackColor = false;
            // 
            // noiNhan_cbb
            // 
            this.noiNhan_cbb.BackColor = System.Drawing.Color.White;
            this.noiNhan_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.noiNhan_cbb.FormattingEnabled = true;
            this.noiNhan_cbb.Location = new System.Drawing.Point(148, 85);
            this.noiNhan_cbb.Name = "noiNhan_cbb";
            this.noiNhan_cbb.Size = new System.Drawing.Size(168, 21);
            this.noiNhan_cbb.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 16);
            this.label1.TabIndex = 44;
            this.label1.Text = "Nơi Nhận:";
            // 
            // KhoVatTu_GiamSatTiepNhanRau
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1393, 758);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Name = "KhoVatTu_GiamSatTiepNhanRau";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button exportExcel_btn;
        private System.Windows.Forms.ComboBox exportCode_cbb;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.Button saveGlobalGAP_btn;
        private System.Windows.Forms.ComboBox ngThuHoach_cbb;
        private System.Windows.Forms.ComboBox ngPhuTrach_cbb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox noiNhan_cbb;
        private System.Windows.Forms.Label label1;
    }
}