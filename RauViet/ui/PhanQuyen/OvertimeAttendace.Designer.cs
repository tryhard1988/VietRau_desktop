
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.monthYearLabel = new System.Windows.Forms.Label();
            this.departmentGV = new System.Windows.Forms.DataGridView();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.inPreview_btn = new System.Windows.Forms.Button();
            this.attendanceMonth_CB = new System.Windows.Forms.CheckBox();
            this.in_DS_TCa_btn = new System.Windows.Forms.Button();
            this.monthYearDtp = new System.Windows.Forms.DateTimePicker();
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.edit_btn = new System.Windows.Forms.Button();
            this.overtimeAttendaceID_tb = new System.Windows.Forms.TextBox();
            this.newBtn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.endTime_dtp = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.startTime_dtp = new System.Windows.Forms.DateTimePicker();
            this.overtimeType_cbb = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.note_tb = new System.Windows.Forms.TextBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.log_GV = new System.Windows.Forms.DataGridView();
            this.panel4 = new System.Windows.Forms.Panel();
            this.attendanceGV = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.count_label = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.totalHour_label = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.departmentGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attendanceGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SaddleBrown;
            this.panel1.Controls.Add(this.monthYearLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1309, 56);
            this.panel1.TabIndex = 15;
            // 
            // monthYearLabel
            // 
            this.monthYearLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monthYearLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthYearLabel.ForeColor = System.Drawing.Color.Bisque;
            this.monthYearLabel.Location = new System.Drawing.Point(0, 0);
            this.monthYearLabel.Name = "monthYearLabel";
            this.monthYearLabel.Size = new System.Drawing.Size(1309, 56);
            this.monthYearLabel.TabIndex = 0;
            this.monthYearLabel.Text = "Tháng 11";
            this.monthYearLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // departmentGV
            // 
            this.departmentGV.AllowUserToAddRows = false;
            this.departmentGV.AllowUserToDeleteRows = false;
            this.departmentGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.departmentGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.departmentGV.Location = new System.Drawing.Point(0, 56);
            this.departmentGV.Name = "departmentGV";
            this.departmentGV.ReadOnly = true;
            this.departmentGV.Size = new System.Drawing.Size(170, 748);
            this.departmentGV.TabIndex = 58;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGV.Location = new System.Drawing.Point(170, 56);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(253, 748);
            this.dataGV.TabIndex = 59;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.attendanceGV);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.log_GV);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(423, 56);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(886, 748);
            this.panel2.TabIndex = 60;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.inPreview_btn);
            this.panel3.Controls.Add(this.attendanceMonth_CB);
            this.panel3.Controls.Add(this.in_DS_TCa_btn);
            this.panel3.Controls.Add(this.monthYearDtp);
            this.panel3.Controls.Add(this.readOnly_btn);
            this.panel3.Controls.Add(this.edit_btn);
            this.panel3.Controls.Add(this.overtimeAttendaceID_tb);
            this.panel3.Controls.Add(this.newBtn);
            this.panel3.Controls.Add(this.info_gb);
            this.panel3.Controls.Add(this.status_lb);
            this.panel3.Controls.Add(this.LuuThayDoiBtn);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(543, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(343, 496);
            this.panel3.TabIndex = 66;
            // 
            // inPreview_btn
            // 
            this.inPreview_btn.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.inPreview_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inPreview_btn.Location = new System.Drawing.Point(149, 403);
            this.inPreview_btn.Name = "inPreview_btn";
            this.inPreview_btn.Size = new System.Drawing.Size(110, 47);
            this.inPreview_btn.TabIndex = 40;
            this.inPreview_btn.Text = "Xem Bảng In";
            this.inPreview_btn.UseVisualStyleBackColor = false;
            // 
            // attendanceMonth_CB
            // 
            this.attendanceMonth_CB.AutoSize = true;
            this.attendanceMonth_CB.BackColor = System.Drawing.Color.Bisque;
            this.attendanceMonth_CB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.attendanceMonth_CB.ForeColor = System.Drawing.Color.ForestGreen;
            this.attendanceMonth_CB.Location = new System.Drawing.Point(7, 6);
            this.attendanceMonth_CB.Name = "attendanceMonth_CB";
            this.attendanceMonth_CB.Size = new System.Drawing.Size(274, 20);
            this.attendanceMonth_CB.TabIndex = 39;
            this.attendanceMonth_CB.Text = "Hiện Thì Dữ Liệu Chấm Công Theo Tháng";
            this.attendanceMonth_CB.UseVisualStyleBackColor = false;
            // 
            // in_DS_TCa_btn
            // 
            this.in_DS_TCa_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.in_DS_TCa_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.in_DS_TCa_btn.Location = new System.Drawing.Point(13, 128);
            this.in_DS_TCa_btn.Name = "in_DS_TCa_btn";
            this.in_DS_TCa_btn.Size = new System.Drawing.Size(110, 33);
            this.in_DS_TCa_btn.TabIndex = 38;
            this.in_DS_TCa_btn.Text = "In DS T.Ca";
            this.in_DS_TCa_btn.UseVisualStyleBackColor = false;
            // 
            // monthYearDtp
            // 
            this.monthYearDtp.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthYearDtp.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthYearDtp.Location = new System.Drawing.Point(103, 57);
            this.monthYearDtp.Name = "monthYearDtp";
            this.monthYearDtp.Size = new System.Drawing.Size(150, 31);
            this.monthYearDtp.TabIndex = 37;
            // 
            // readOnly_btn
            // 
            this.readOnly_btn.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.readOnly_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readOnly_btn.Location = new System.Drawing.Point(295, 129);
            this.readOnly_btn.Name = "readOnly_btn";
            this.readOnly_btn.Size = new System.Drawing.Size(42, 32);
            this.readOnly_btn.TabIndex = 34;
            this.readOnly_btn.Text = "X";
            this.readOnly_btn.UseVisualStyleBackColor = false;
            // 
            // edit_btn
            // 
            this.edit_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.edit_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.Location = new System.Drawing.Point(159, 129);
            this.edit_btn.Name = "edit_btn";
            this.edit_btn.Size = new System.Drawing.Size(94, 32);
            this.edit_btn.TabIndex = 33;
            this.edit_btn.Text = "Chỉnh sửa";
            this.edit_btn.UseVisualStyleBackColor = false;
            // 
            // overtimeAttendaceID_tb
            // 
            this.overtimeAttendaceID_tb.Location = new System.Drawing.Point(260, 464);
            this.overtimeAttendaceID_tb.Name = "overtimeAttendaceID_tb";
            this.overtimeAttendaceID_tb.ReadOnly = true;
            this.overtimeAttendaceID_tb.Size = new System.Drawing.Size(55, 20);
            this.overtimeAttendaceID_tb.TabIndex = 31;
            this.overtimeAttendaceID_tb.Visible = false;
            // 
            // newBtn
            // 
            this.newBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newBtn.Location = new System.Drawing.Point(254, 129);
            this.newBtn.Name = "newBtn";
            this.newBtn.Size = new System.Drawing.Size(83, 32);
            this.newBtn.TabIndex = 30;
            this.newBtn.Text = "Tạo mới";
            this.newBtn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.label6);
            this.info_gb.Controls.Add(this.endTime_dtp);
            this.info_gb.Controls.Add(this.label8);
            this.info_gb.Controls.Add(this.startTime_dtp);
            this.info_gb.Controls.Add(this.overtimeType_cbb);
            this.info_gb.Controls.Add(this.label4);
            this.info_gb.Controls.Add(this.label15);
            this.info_gb.Controls.Add(this.note_tb);
            this.info_gb.Location = new System.Drawing.Point(13, 159);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(324, 188);
            this.info_gb.TabIndex = 29;
            this.info_gb.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 16);
            this.label6.TabIndex = 45;
            this.label6.Text = "Giờ Kết Thúc:";
            // 
            // endTime_dtp
            // 
            this.endTime_dtp.Location = new System.Drawing.Point(107, 49);
            this.endTime_dtp.Name = "endTime_dtp";
            this.endTime_dtp.Size = new System.Drawing.Size(113, 20);
            this.endTime_dtp.TabIndex = 44;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(17, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 16);
            this.label8.TabIndex = 31;
            this.label8.Text = "Giờ bắt Đầu:";
            // 
            // startTime_dtp
            // 
            this.startTime_dtp.Location = new System.Drawing.Point(107, 18);
            this.startTime_dtp.Name = "startTime_dtp";
            this.startTime_dtp.Size = new System.Drawing.Size(113, 20);
            this.startTime_dtp.TabIndex = 30;
            // 
            // overtimeType_cbb
            // 
            this.overtimeType_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.overtimeType_cbb.FormattingEnabled = true;
            this.overtimeType_cbb.Location = new System.Drawing.Point(107, 77);
            this.overtimeType_cbb.Name = "overtimeType_cbb";
            this.overtimeType_cbb.Size = new System.Drawing.Size(170, 21);
            this.overtimeType_cbb.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 16);
            this.label4.TabIndex = 19;
            this.label4.Text = "Loại Tăng Ca:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(40, 124);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 16);
            this.label15.TabIndex = 11;
            this.label15.Text = "Ghi Chú:";
            // 
            // note_tb
            // 
            this.note_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.note_tb.Location = new System.Drawing.Point(107, 106);
            this.note_tb.Multiline = true;
            this.note_tb.Name = "note_tb";
            this.note_tb.Size = new System.Drawing.Size(195, 67);
            this.note_tb.TabIndex = 18;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(19, 361);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(149, 350);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(110, 47);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.PeachPuff;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label1.Location = new System.Drawing.Point(0, 496);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(886, 23);
            this.label1.TabIndex = 64;
            this.label1.Text = "Lịch sử thay đổi";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // log_GV
            // 
            this.log_GV.AllowUserToAddRows = false;
            this.log_GV.AllowUserToDeleteRows = false;
            this.log_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.log_GV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.log_GV.Location = new System.Drawing.Point(0, 519);
            this.log_GV.Name = "log_GV";
            this.log_GV.ReadOnly = true;
            this.log_GV.Size = new System.Drawing.Size(886, 229);
            this.log_GV.TabIndex = 63;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.totalHour_label);
            this.panel4.Controls.Add(this.label10);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Controls.Add(this.count_label);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 470);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(543, 26);
            this.panel4.TabIndex = 67;
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
            this.attendanceGV.Size = new System.Drawing.Size(543, 470);
            this.attendanceGV.TabIndex = 68;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 16);
            this.label2.TabIndex = 32;
            this.label2.Text = "Số Lượng:";
            // 
            // count_label
            // 
            this.count_label.AutoSize = true;
            this.count_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.count_label.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.count_label.ForeColor = System.Drawing.Color.Firebrick;
            this.count_label.Location = new System.Drawing.Point(66, 0);
            this.count_label.Name = "count_label";
            this.count_label.Size = new System.Drawing.Size(72, 16);
            this.count_label.TabIndex = 33;
            this.count_label.Text = "Số Lượng:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Left;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(138, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 16);
            this.label7.TabIndex = 34;
            this.label7.Text = " | ";
            // 
            // totalHour_label
            // 
            this.totalHour_label.AutoSize = true;
            this.totalHour_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.totalHour_label.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalHour_label.ForeColor = System.Drawing.Color.Firebrick;
            this.totalHour_label.Location = new System.Drawing.Point(253, 0);
            this.totalHour_label.Name = "totalHour_label";
            this.totalHour_label.Size = new System.Drawing.Size(72, 16);
            this.totalHour_label.TabIndex = 37;
            this.totalHour_label.Text = "Số Lượng:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Left;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(159, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 16);
            this.label10.TabIndex = 36;
            this.label10.Text = "Tổng Giờ T.Ca:";
            // 
            // OvertimeAttendace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1309, 804);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.departmentGV);
            this.Controls.Add(this.panel1);
            this.Name = "OvertimeAttendace";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.departmentGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attendanceGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label monthYearLabel;
        private System.Windows.Forms.DataGridView departmentGV;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button readOnly_btn;
        private System.Windows.Forms.Button edit_btn;
        private System.Windows.Forms.TextBox overtimeAttendaceID_tb;
        private System.Windows.Forms.Button newBtn;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker endTime_dtp;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker startTime_dtp;
        private System.Windows.Forms.ComboBox overtimeType_cbb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox note_tb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.DateTimePicker monthYearDtp;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView log_GV;
        private System.Windows.Forms.Button in_DS_TCa_btn;
        private System.Windows.Forms.CheckBox attendanceMonth_CB;
        private System.Windows.Forms.Button inPreview_btn;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView attendanceGV;
        private System.Windows.Forms.Label totalHour_label;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label count_label;
        private System.Windows.Forms.Label label2;
    }
}