﻿
namespace RauViet.ui
{
    partial class Attendance
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
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.excelAttendance_btn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.loadAttandance_btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.month_cbb = new System.Windows.Forms.ComboBox();
            this.year_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.loading_lb = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.attendanceGV = new System.Windows.Forms.DataGridView();
            this.calWorkHour_btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attendanceGV)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGV.Location = new System.Drawing.Point(0, 0);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(403, 681);
            this.dataGV.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.excelAttendance_btn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(955, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(309, 681);
            this.panel1.TabIndex = 11;
            // 
            // excelAttendance_btn
            // 
            this.excelAttendance_btn.BackColor = System.Drawing.SystemColors.Highlight;
            this.excelAttendance_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.excelAttendance_btn.Location = new System.Drawing.Point(128, 388);
            this.excelAttendance_btn.Name = "excelAttendance_btn";
            this.excelAttendance_btn.Size = new System.Drawing.Size(177, 47);
            this.excelAttendance_btn.TabIndex = 29;
            this.excelAttendance_btn.Text = "Excel Chấm Công";
            this.excelAttendance_btn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.calWorkHour_btn);
            this.info_gb.Controls.Add(this.loadAttandance_btn);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Controls.Add(this.month_cbb);
            this.info_gb.Controls.Add(this.year_tb);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Location = new System.Drawing.Point(35, 156);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(241, 184);
            this.info_gb.TabIndex = 28;
            this.info_gb.TabStop = false;
            // 
            // loadAttandance_btn
            // 
            this.loadAttandance_btn.BackColor = System.Drawing.SystemColors.Highlight;
            this.loadAttandance_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadAttandance_btn.Location = new System.Drawing.Point(32, 73);
            this.loadAttandance_btn.Name = "loadAttandance_btn";
            this.loadAttandance_btn.Size = new System.Drawing.Size(145, 46);
            this.loadAttandance_btn.TabIndex = 30;
            this.loadAttandance_btn.Text = "Load";
            this.loadAttandance_btn.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(111, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 16);
            this.label1.TabIndex = 22;
            this.label1.Text = "/";
            // 
            // month_cbb
            // 
            this.month_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.month_cbb.FormattingEnabled = true;
            this.month_cbb.Location = new System.Drawing.Point(69, 32);
            this.month_cbb.Name = "month_cbb";
            this.month_cbb.Size = new System.Drawing.Size(39, 21);
            this.month_cbb.TabIndex = 21;
            // 
            // year_tb
            // 
            this.year_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.year_tb.Location = new System.Drawing.Point(129, 32);
            this.year_tb.Name = "year_tb";
            this.year_tb.Size = new System.Drawing.Size(48, 23);
            this.year_tb.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Tháng:";
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
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(116, 121);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(12, 388);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(110, 47);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // attendanceGV
            // 
            this.attendanceGV.AllowUserToAddRows = false;
            this.attendanceGV.AllowUserToDeleteRows = false;
            this.attendanceGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.attendanceGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attendanceGV.Location = new System.Drawing.Point(403, 0);
            this.attendanceGV.Name = "attendanceGV";
            this.attendanceGV.ReadOnly = true;
            this.attendanceGV.Size = new System.Drawing.Size(552, 681);
            this.attendanceGV.TabIndex = 12;
            // 
            // calWorkHour_btn
            // 
            this.calWorkHour_btn.BackColor = System.Drawing.SystemColors.Highlight;
            this.calWorkHour_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calWorkHour_btn.Location = new System.Drawing.Point(17, 125);
            this.calWorkHour_btn.Name = "calWorkHour_btn";
            this.calWorkHour_btn.Size = new System.Drawing.Size(177, 47);
            this.calWorkHour_btn.TabIndex = 30;
            this.calWorkHour_btn.Text = "Tính Lại Số Giờ làm";
            this.calWorkHour_btn.UseVisualStyleBackColor = false;
            // 
            // Attendance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.attendanceGV);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGV);
            this.Name = "Attendance";
            this.Text = "FormTableData";
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attendanceGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button excelAttendance_btn;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.TextBox year_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.ComboBox month_cbb;
        private System.Windows.Forms.Button loadAttandance_btn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView attendanceGV;
        private System.Windows.Forms.Button calWorkHour_btn;
    }
}