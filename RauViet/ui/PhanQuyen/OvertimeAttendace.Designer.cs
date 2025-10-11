
namespace RauViet.ui
{
    partial class OvertimeAttendace
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
            this.load_gb = new System.Windows.Forms.GroupBox();
            this.loadAttandance_btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.month_cbb = new System.Windows.Forms.ComboBox();
            this.year_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.loading_lb = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.attendanceGV = new System.Windows.Forms.DataGridView();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.endTime_dtp = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.startTime_dtp = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.workDate_dtp = new System.Windows.Forms.DateTimePicker();
            this.overtimeType_cbb = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.note_tb = new System.Windows.Forms.TextBox();
            this.newBtn = new System.Windows.Forms.Button();
            this.overtimeAttendaceID_tb = new System.Windows.Forms.TextBox();
            this.updatedHistory_tb = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel1.SuspendLayout();
            this.load_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attendanceGV)).BeginInit();
            this.info_gb.SuspendLayout();
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
            this.panel1.Controls.Add(this.updatedHistory_tb);
            this.panel1.Controls.Add(this.overtimeAttendaceID_tb);
            this.panel1.Controls.Add(this.newBtn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.load_gb);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(966, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(343, 681);
            this.panel1.TabIndex = 11;
            // 
            // load_gb
            // 
            this.load_gb.Controls.Add(this.loadAttandance_btn);
            this.load_gb.Controls.Add(this.label1);
            this.load_gb.Controls.Add(this.month_cbb);
            this.load_gb.Controls.Add(this.year_tb);
            this.load_gb.Controls.Add(this.label2);
            this.load_gb.Location = new System.Drawing.Point(53, 64);
            this.load_gb.Name = "load_gb";
            this.load_gb.Size = new System.Drawing.Size(241, 136);
            this.load_gb.TabIndex = 28;
            this.load_gb.TabStop = false;
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
            this.status_lb.Location = new System.Drawing.Point(134, 29);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(40, 499);
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
            this.attendanceGV.Size = new System.Drawing.Size(563, 681);
            this.attendanceGV.TabIndex = 12;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.label6);
            this.info_gb.Controls.Add(this.endTime_dtp);
            this.info_gb.Controls.Add(this.label8);
            this.info_gb.Controls.Add(this.startTime_dtp);
            this.info_gb.Controls.Add(this.label7);
            this.info_gb.Controls.Add(this.workDate_dtp);
            this.info_gb.Controls.Add(this.overtimeType_cbb);
            this.info_gb.Controls.Add(this.label4);
            this.info_gb.Controls.Add(this.label15);
            this.info_gb.Controls.Add(this.note_tb);
            this.info_gb.Location = new System.Drawing.Point(6, 256);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(324, 228);
            this.info_gb.TabIndex = 29;
            this.info_gb.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 16);
            this.label6.TabIndex = 45;
            this.label6.Text = "Giờ Kết Thúc:";
            // 
            // endTime_dtp
            // 
            this.endTime_dtp.Location = new System.Drawing.Point(107, 79);
            this.endTime_dtp.Name = "endTime_dtp";
            this.endTime_dtp.Size = new System.Drawing.Size(113, 20);
            this.endTime_dtp.TabIndex = 44;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(17, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 16);
            this.label8.TabIndex = 31;
            this.label8.Text = "Giờ bắt Đầu:";
            // 
            // startTime_dtp
            // 
            this.startTime_dtp.Location = new System.Drawing.Point(107, 48);
            this.startTime_dtp.Name = "startTime_dtp";
            this.startTime_dtp.Size = new System.Drawing.Size(113, 20);
            this.startTime_dtp.TabIndex = 30;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(4, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 16);
            this.label7.TabIndex = 29;
            this.label7.Text = "Ngày Tăng Ca:";
            // 
            // workDate_dtp
            // 
            this.workDate_dtp.Location = new System.Drawing.Point(107, 19);
            this.workDate_dtp.Name = "workDate_dtp";
            this.workDate_dtp.Size = new System.Drawing.Size(113, 20);
            this.workDate_dtp.TabIndex = 28;
            // 
            // overtimeType_cbb
            // 
            this.overtimeType_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.overtimeType_cbb.FormattingEnabled = true;
            this.overtimeType_cbb.Location = new System.Drawing.Point(107, 107);
            this.overtimeType_cbb.Name = "overtimeType_cbb";
            this.overtimeType_cbb.Size = new System.Drawing.Size(170, 21);
            this.overtimeType_cbb.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 16);
            this.label4.TabIndex = 19;
            this.label4.Text = "Loại Tăng Ca:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(40, 154);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 16);
            this.label15.TabIndex = 11;
            this.label15.Text = "Ghi Chú:";
            // 
            // note_tb
            // 
            this.note_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.note_tb.Location = new System.Drawing.Point(107, 136);
            this.note_tb.Multiline = true;
            this.note_tb.Name = "note_tb";
            this.note_tb.Size = new System.Drawing.Size(195, 67);
            this.note_tb.TabIndex = 18;
            // 
            // newBtn
            // 
            this.newBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newBtn.Location = new System.Drawing.Point(167, 499);
            this.newBtn.Name = "newBtn";
            this.newBtn.Size = new System.Drawing.Size(113, 47);
            this.newBtn.TabIndex = 30;
            this.newBtn.Text = "Tạo mới";
            this.newBtn.UseVisualStyleBackColor = false;
            // 
            // overtimeAttendaceID_tb
            // 
            this.overtimeAttendaceID_tb.Location = new System.Drawing.Point(276, 230);
            this.overtimeAttendaceID_tb.Name = "overtimeAttendaceID_tb";
            this.overtimeAttendaceID_tb.ReadOnly = true;
            this.overtimeAttendaceID_tb.Size = new System.Drawing.Size(55, 20);
            this.overtimeAttendaceID_tb.TabIndex = 31;
            // 
            // updatedHistory_tb
            // 
            this.updatedHistory_tb.Location = new System.Drawing.Point(195, 230);
            this.updatedHistory_tb.Name = "updatedHistory_tb";
            this.updatedHistory_tb.ReadOnly = true;
            this.updatedHistory_tb.Size = new System.Drawing.Size(55, 20);
            this.updatedHistory_tb.TabIndex = 32;
            // 
            // OvertimeAttendace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1309, 681);
            this.Controls.Add(this.attendanceGV);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGV);
            this.Name = "OvertimeAttendace";
            this.Text = "FormTableData";
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.load_gb.ResumeLayout(false);
            this.load_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attendanceGV)).EndInit();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox load_gb;
        private System.Windows.Forms.TextBox year_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.ComboBox month_cbb;
        private System.Windows.Forms.Button loadAttandance_btn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView attendanceGV;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker endTime_dtp;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker startTime_dtp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker workDate_dtp;
        private System.Windows.Forms.ComboBox overtimeType_cbb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox note_tb;
        private System.Windows.Forms.Button newBtn;
        private System.Windows.Forms.TextBox overtimeAttendaceID_tb;
        private System.Windows.Forms.TextBox updatedHistory_tb;
    }
}