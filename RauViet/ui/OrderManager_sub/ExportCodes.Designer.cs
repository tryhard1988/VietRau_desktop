﻿
namespace RauViet.ui
{
    partial class ExportCodes
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
            this.updatePrice_btn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.exRate_btn = new System.Windows.Forms.Button();
            this.shippingCost_tb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.exRate_tb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.autoCreateExportId_btn = new System.Windows.Forms.Button();
            this.complete_cb = new System.Windows.Forms.CheckBox();
            this.exportdate_dtp = new System.Windows.Forms.DateTimePicker();
            this.exportCode_tb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.delete_btn = new System.Windows.Forms.Button();
            this.loading_lb = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.exportCodeId_tb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.inputBy_cbb = new System.Windows.Forms.ComboBox();
            this.packingBy_cbb = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.updatePrice_btn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.delete_btn);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.exportCodeId_tb);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(833, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(431, 681);
            this.panel1.TabIndex = 9;
            // 
            // updatePrice_btn
            // 
            this.updatePrice_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.updatePrice_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updatePrice_btn.Location = new System.Drawing.Point(132, 502);
            this.updatePrice_btn.Name = "updatePrice_btn";
            this.updatePrice_btn.Size = new System.Drawing.Size(178, 39);
            this.updatePrice_btn.TabIndex = 29;
            this.updatePrice_btn.Text = "Cập Nhật Lại Giá Mới";
            this.updatePrice_btn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.packingBy_cbb);
            this.info_gb.Controls.Add(this.inputBy_cbb);
            this.info_gb.Controls.Add(this.label7);
            this.info_gb.Controls.Add(this.label6);
            this.info_gb.Controls.Add(this.exRate_btn);
            this.info_gb.Controls.Add(this.shippingCost_tb);
            this.info_gb.Controls.Add(this.label5);
            this.info_gb.Controls.Add(this.exRate_tb);
            this.info_gb.Controls.Add(this.label4);
            this.info_gb.Controls.Add(this.autoCreateExportId_btn);
            this.info_gb.Controls.Add(this.complete_cb);
            this.info_gb.Controls.Add(this.exportdate_dtp);
            this.info_gb.Controls.Add(this.exportCode_tb);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info_gb.Location = new System.Drawing.Point(16, 143);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(399, 298);
            this.info_gb.TabIndex = 28;
            this.info_gb.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 217);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 16);
            this.label7.TabIndex = 42;
            this.label7.Text = "NV Đóng Gói:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 180);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 16);
            this.label6.TabIndex = 40;
            this.label6.Text = "NV Nhập Số Liệu:";
            // 
            // exRate_btn
            // 
            this.exRate_btn.BackColor = System.Drawing.SystemColors.Highlight;
            this.exRate_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exRate_btn.Location = new System.Drawing.Point(275, 94);
            this.exRate_btn.Name = "exRate_btn";
            this.exRate_btn.Size = new System.Drawing.Size(117, 38);
            this.exRate_btn.TabIndex = 39;
            this.exRate_btn.Text = "Tỉ Giá Hiện Tại";
            this.exRate_btn.UseVisualStyleBackColor = false;
            // 
            // shippingCost_tb
            // 
            this.shippingCost_tb.Location = new System.Drawing.Point(178, 140);
            this.shippingCost_tb.Name = "shippingCost_tb";
            this.shippingCost_tb.Size = new System.Drawing.Size(89, 23);
            this.shippingCost_tb.TabIndex = 38;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 16);
            this.label5.TabIndex = 37;
            this.label5.Text = "Giá Cước:";
            // 
            // exRate_tb
            // 
            this.exRate_tb.Location = new System.Drawing.Point(178, 103);
            this.exRate_tb.Name = "exRate_tb";
            this.exRate_tb.Size = new System.Drawing.Size(89, 23);
            this.exRate_tb.TabIndex = 36;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 16);
            this.label4.TabIndex = 35;
            this.label4.Text = "Tỉ Giá:";
            // 
            // autoCreateExportId_btn
            // 
            this.autoCreateExportId_btn.BackColor = System.Drawing.SystemColors.Highlight;
            this.autoCreateExportId_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autoCreateExportId_btn.Location = new System.Drawing.Point(273, 21);
            this.autoCreateExportId_btn.Name = "autoCreateExportId_btn";
            this.autoCreateExportId_btn.Size = new System.Drawing.Size(88, 38);
            this.autoCreateExportId_btn.TabIndex = 29;
            this.autoCreateExportId_btn.Text = "Tự Tạo ID";
            this.autoCreateExportId_btn.UseVisualStyleBackColor = false;
            // 
            // complete_cb
            // 
            this.complete_cb.AutoSize = true;
            this.complete_cb.Location = new System.Drawing.Point(17, 267);
            this.complete_cb.Name = "complete_cb";
            this.complete_cb.Size = new System.Drawing.Size(115, 20);
            this.complete_cb.TabIndex = 34;
            this.complete_cb.Text = "Đã Hoàn Thành";
            this.complete_cb.UseVisualStyleBackColor = true;
            // 
            // exportdate_dtp
            // 
            this.exportdate_dtp.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.exportdate_dtp.Location = new System.Drawing.Point(178, 66);
            this.exportdate_dtp.Name = "exportdate_dtp";
            this.exportdate_dtp.Size = new System.Drawing.Size(142, 23);
            this.exportdate_dtp.TabIndex = 33;
            // 
            // exportCode_tb
            // 
            this.exportCode_tb.Location = new System.Drawing.Point(178, 29);
            this.exportCode_tb.Name = "exportCode_tb";
            this.exportCode_tb.Size = new System.Drawing.Size(89, 23);
            this.exportCode_tb.TabIndex = 32;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 16);
            this.label3.TabIndex = 31;
            this.label3.Text = "Ngày Xuất Cảng:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 16);
            this.label2.TabIndex = 30;
            this.label2.Text = "Mã Xuất Cảng:";
            // 
            // delete_btn
            // 
            this.delete_btn.BackColor = System.Drawing.Color.Red;
            this.delete_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delete_btn.Location = new System.Drawing.Point(277, 447);
            this.delete_btn.Name = "delete_btn";
            this.delete_btn.Size = new System.Drawing.Size(100, 39);
            this.delete_btn.TabIndex = 27;
            this.delete_btn.Text = "Xóa";
            this.delete_btn.UseVisualStyleBackColor = false;
            // 
            // loading_lb
            // 
            this.loading_lb.AutoSize = true;
            this.loading_lb.Dock = System.Windows.Forms.DockStyle.Top;
            this.loading_lb.Location = new System.Drawing.Point(0, 0);
            this.loading_lb.Name = "loading_lb";
            this.loading_lb.Size = new System.Drawing.Size(55, 13);
            this.loading_lb.TabIndex = 10;
            this.loading_lb.Text = "loading_lb";
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(190, 116);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(57, 24);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // newCustomerBtn
            // 
            this.newCustomerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newCustomerBtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCustomerBtn.Location = new System.Drawing.Point(169, 447);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(100, 39);
            this.newCustomerBtn.TabIndex = 25;
            this.newCustomerBtn.Text = "Tạo mới";
            this.newCustomerBtn.UseVisualStyleBackColor = false;
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(61, 447);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(100, 39);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // exportCodeId_tb
            // 
            this.exportCodeId_tb.Location = new System.Drawing.Point(124, 630);
            this.exportCodeId_tb.Name = "exportCodeId_tb";
            this.exportCodeId_tb.ReadOnly = true;
            this.exportCodeId_tb.Size = new System.Drawing.Size(24, 20);
            this.exportCodeId_tb.TabIndex = 16;
            this.exportCodeId_tb.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 633);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Mã Xuất Cảng ID";
            this.label1.Visible = false;
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
            this.dataGV.Size = new System.Drawing.Size(833, 681);
            this.dataGV.TabIndex = 10;
            // 
            // inputBy_cbb
            // 
            this.inputBy_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputBy_cbb.FormattingEnabled = true;
            this.inputBy_cbb.Location = new System.Drawing.Point(178, 177);
            this.inputBy_cbb.Name = "inputBy_cbb";
            this.inputBy_cbb.Size = new System.Drawing.Size(212, 24);
            this.inputBy_cbb.TabIndex = 44;
            // 
            // packingBy_cbb
            // 
            this.packingBy_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.packingBy_cbb.FormattingEnabled = true;
            this.packingBy_cbb.Location = new System.Drawing.Point(178, 215);
            this.packingBy_cbb.Name = "packingBy_cbb";
            this.packingBy_cbb.Size = new System.Drawing.Size(212, 24);
            this.packingBy_cbb.TabIndex = 45;
            // 
            // ExportCodes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Name = "ExportCodes";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox exportCodeId_tb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.Button delete_btn;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.CheckBox complete_cb;
        private System.Windows.Forms.DateTimePicker exportdate_dtp;
        private System.Windows.Forms.TextBox exportCode_tb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button autoCreateExportId_btn;
        private System.Windows.Forms.TextBox shippingCost_tb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox exRate_tb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button updatePrice_btn;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Button exRate_btn;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox packingBy_cbb;
        private System.Windows.Forms.ComboBox inputBy_cbb;
    }
}