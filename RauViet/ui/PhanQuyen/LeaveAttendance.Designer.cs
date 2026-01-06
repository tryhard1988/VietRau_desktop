
namespace RauViet.ui
{
    partial class LeaveAttendance
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
            this.panel4 = new System.Windows.Forms.Panel();
            this.monthYearLabel = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.search_tb = new System.Windows.Forms.TextBox();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.attendanceGV = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.year_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.edit_btn = new System.Windows.Forms.Button();
            this.leaveID_tb = new System.Windows.Forms.TextBox();
            this.newBtn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.linkStartEnd_cb = new System.Windows.Forms.CheckBox();
            this.dateOffEnd_dtp = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.hourLeave_tb = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.dateOffStart_dtp = new System.Windows.Forms.DateTimePicker();
            this.leaveType_cbb = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.note_tb = new System.Windows.Forms.TextBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.log_GV = new System.Windows.Forms.DataGridView();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attendanceGV)).BeginInit();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Brown;
            this.panel4.Controls.Add(this.monthYearLabel);
            this.panel4.Controls.Add(this.panel5);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1370, 55);
            this.panel4.TabIndex = 39;
            // 
            // monthYearLabel
            // 
            this.monthYearLabel.BackColor = System.Drawing.Color.Transparent;
            this.monthYearLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monthYearLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthYearLabel.ForeColor = System.Drawing.Color.Bisque;
            this.monthYearLabel.Location = new System.Drawing.Point(382, 0);
            this.monthYearLabel.Name = "monthYearLabel";
            this.monthYearLabel.Size = new System.Drawing.Size(988, 55);
            this.monthYearLabel.TabIndex = 25;
            this.monthYearLabel.Text = "Tháng 11";
            this.monthYearLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label16);
            this.panel5.Controls.Add(this.search_tb);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(382, 55);
            this.panel5.TabIndex = 24;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.PeachPuff;
            this.label16.Location = new System.Drawing.Point(12, 17);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(66, 16);
            this.label16.TabIndex = 22;
            this.label16.Text = "Tìm Kiếm:";
            // 
            // search_tb
            // 
            this.search_tb.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.search_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.search_tb.Location = new System.Drawing.Point(83, 14);
            this.search_tb.Name = "search_tb";
            this.search_tb.Size = new System.Drawing.Size(287, 23);
            this.search_tb.TabIndex = 21;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGV.Location = new System.Drawing.Point(0, 55);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(403, 626);
            this.dataGV.TabIndex = 40;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.attendanceGV);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.log_GV);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(403, 55);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(967, 626);
            this.panel2.TabIndex = 41;
            // 
            // attendanceGV
            // 
            this.attendanceGV.AllowUserToAddRows = false;
            this.attendanceGV.AllowUserToDeleteRows = false;
            this.attendanceGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.attendanceGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attendanceGV.Location = new System.Drawing.Point(0, 0);
            this.attendanceGV.Name = "attendanceGV";
            this.attendanceGV.ReadOnly = true;
            this.attendanceGV.Size = new System.Drawing.Size(579, 403);
            this.attendanceGV.TabIndex = 53;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.year_tb);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.readOnly_btn);
            this.panel1.Controls.Add(this.edit_btn);
            this.panel1.Controls.Add(this.leaveID_tb);
            this.panel1.Controls.Add(this.newBtn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(579, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(388, 403);
            this.panel1.TabIndex = 52;
            // 
            // year_tb
            // 
            this.year_tb.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.year_tb.Location = new System.Drawing.Point(156, 38);
            this.year_tb.Name = "year_tb";
            this.year_tb.Size = new System.Drawing.Size(61, 33);
            this.year_tb.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(95, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 25);
            this.label2.TabIndex = 10;
            this.label2.Text = "Năm:";
            // 
            // readOnly_btn
            // 
            this.readOnly_btn.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.readOnly_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readOnly_btn.Location = new System.Drawing.Point(339, 119);
            this.readOnly_btn.Name = "readOnly_btn";
            this.readOnly_btn.Size = new System.Drawing.Size(42, 32);
            this.readOnly_btn.TabIndex = 35;
            this.readOnly_btn.Text = "X";
            this.readOnly_btn.UseVisualStyleBackColor = false;
            // 
            // edit_btn
            // 
            this.edit_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.edit_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.Location = new System.Drawing.Point(203, 119);
            this.edit_btn.Name = "edit_btn";
            this.edit_btn.Size = new System.Drawing.Size(94, 32);
            this.edit_btn.TabIndex = 34;
            this.edit_btn.Text = "Chỉnh sửa";
            this.edit_btn.UseVisualStyleBackColor = false;
            // 
            // leaveID_tb
            // 
            this.leaveID_tb.Location = new System.Drawing.Point(65, 119);
            this.leaveID_tb.Name = "leaveID_tb";
            this.leaveID_tb.ReadOnly = true;
            this.leaveID_tb.Size = new System.Drawing.Size(55, 20);
            this.leaveID_tb.TabIndex = 31;
            this.leaveID_tb.Visible = false;
            // 
            // newBtn
            // 
            this.newBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newBtn.Location = new System.Drawing.Point(298, 119);
            this.newBtn.Name = "newBtn";
            this.newBtn.Size = new System.Drawing.Size(84, 32);
            this.newBtn.TabIndex = 30;
            this.newBtn.Text = "Tạo mới";
            this.newBtn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.linkStartEnd_cb);
            this.info_gb.Controls.Add(this.dateOffEnd_dtp);
            this.info_gb.Controls.Add(this.label5);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Controls.Add(this.hourLeave_tb);
            this.info_gb.Controls.Add(this.label8);
            this.info_gb.Controls.Add(this.dateOffStart_dtp);
            this.info_gb.Controls.Add(this.leaveType_cbb);
            this.info_gb.Controls.Add(this.label4);
            this.info_gb.Controls.Add(this.label15);
            this.info_gb.Controls.Add(this.note_tb);
            this.info_gb.Location = new System.Drawing.Point(11, 147);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(370, 228);
            this.info_gb.TabIndex = 29;
            this.info_gb.TabStop = false;
            // 
            // linkStartEnd_cb
            // 
            this.linkStartEnd_cb.AutoSize = true;
            this.linkStartEnd_cb.Checked = true;
            this.linkStartEnd_cb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.linkStartEnd_cb.Location = new System.Drawing.Point(216, 83);
            this.linkStartEnd_cb.Name = "linkStartEnd_cb";
            this.linkStartEnd_cb.Size = new System.Drawing.Size(15, 14);
            this.linkStartEnd_cb.TabIndex = 37;
            this.linkStartEnd_cb.UseVisualStyleBackColor = true;
            // 
            // dateOffEnd_dtp
            // 
            this.dateOffEnd_dtp.Location = new System.Drawing.Point(266, 81);
            this.dateOffEnd_dtp.Name = "dateOffEnd_dtp";
            this.dateOffEnd_dtp.Size = new System.Drawing.Size(98, 20);
            this.dateOffEnd_dtp.TabIndex = 36;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(237, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 16);
            this.label5.TabIndex = 35;
            this.label5.Text = "=>";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(187, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "(1 ngày)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(31, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 16);
            this.label1.TabIndex = 32;
            this.label1.Text = "Số Giờ Nghỉ:";
            // 
            // hourLeave_tb
            // 
            this.hourLeave_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hourLeave_tb.Location = new System.Drawing.Point(117, 107);
            this.hourLeave_tb.Name = "hourLeave_tb";
            this.hourLeave_tb.Size = new System.Drawing.Size(66, 23);
            this.hourLeave_tb.TabIndex = 33;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(38, 81);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 16);
            this.label8.TabIndex = 31;
            this.label8.Text = "Ngày Nghỉ:";
            // 
            // dateOffStart_dtp
            // 
            this.dateOffStart_dtp.Location = new System.Drawing.Point(115, 79);
            this.dateOffStart_dtp.Name = "dateOffStart_dtp";
            this.dateOffStart_dtp.Size = new System.Drawing.Size(98, 20);
            this.dateOffStart_dtp.TabIndex = 30;
            // 
            // leaveType_cbb
            // 
            this.leaveType_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.leaveType_cbb.FormattingEnabled = true;
            this.leaveType_cbb.Location = new System.Drawing.Point(115, 49);
            this.leaveType_cbb.Name = "leaveType_cbb";
            this.leaveType_cbb.Size = new System.Drawing.Size(170, 21);
            this.leaveType_cbb.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(11, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 16);
            this.label4.TabIndex = 19;
            this.label4.Text = "Loại Nghỉ Phép:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(51, 160);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 16);
            this.label15.TabIndex = 11;
            this.label15.Text = "Ghi Chú:";
            // 
            // note_tb
            // 
            this.note_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.note_tb.Location = new System.Drawing.Point(115, 142);
            this.note_tb.Multiline = true;
            this.note_tb.Name = "note_tb";
            this.note_tb.Size = new System.Drawing.Size(195, 67);
            this.note_tb.TabIndex = 18;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(11, 381);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(48, 19);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(128, 381);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(110, 47);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.PeachPuff;
            this.label6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label6.Location = new System.Drawing.Point(0, 403);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(967, 23);
            this.label6.TabIndex = 50;
            this.label6.Text = "Lịch sử thay đổi";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // log_GV
            // 
            this.log_GV.AllowUserToAddRows = false;
            this.log_GV.AllowUserToDeleteRows = false;
            this.log_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.log_GV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.log_GV.Location = new System.Drawing.Point(0, 426);
            this.log_GV.Name = "log_GV";
            this.log_GV.ReadOnly = true;
            this.log_GV.Size = new System.Drawing.Size(967, 200);
            this.log_GV.TabIndex = 41;
            // 
            // LeaveAttendance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 681);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel4);
            this.Name = "LeaveAttendance";
            this.Text = "FormTableData";
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.attendanceGV)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView attendanceGV;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button readOnly_btn;
        private System.Windows.Forms.Button edit_btn;
        private System.Windows.Forms.TextBox leaveID_tb;
        private System.Windows.Forms.Button newBtn;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.CheckBox linkStartEnd_cb;
        private System.Windows.Forms.DateTimePicker dateOffEnd_dtp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox hourLeave_tb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dateOffStart_dtp;
        private System.Windows.Forms.ComboBox leaveType_cbb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox note_tb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.TextBox year_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView log_GV;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox search_tb;
        private System.Windows.Forms.Label monthYearLabel;
    }
}