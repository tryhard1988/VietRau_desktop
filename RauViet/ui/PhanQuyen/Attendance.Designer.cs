
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
            this.monthYearDtp = new System.Windows.Forms.DateTimePicker();
            this.loadAttandance_btn = new System.Windows.Forms.Button();
            this.status_lb = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.attendanceGV = new System.Windows.Forms.DataGridView();
            this.log_GV = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.attendanceGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).BeginInit();
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
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(995, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(309, 681);
            this.panel1.TabIndex = 11;
            // 
            // excelAttendance_btn
            // 
            this.excelAttendance_btn.BackColor = System.Drawing.Color.LimeGreen;
            this.excelAttendance_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.excelAttendance_btn.Location = new System.Drawing.Point(60, 289);
            this.excelAttendance_btn.Name = "excelAttendance_btn";
            this.excelAttendance_btn.Size = new System.Drawing.Size(192, 40);
            this.excelAttendance_btn.TabIndex = 29;
            this.excelAttendance_btn.Text = "Excel Chấm Công";
            this.excelAttendance_btn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.monthYearDtp);
            this.info_gb.Controls.Add(this.loadAttandance_btn);
            this.info_gb.Location = new System.Drawing.Point(60, 335);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(192, 52);
            this.info_gb.TabIndex = 28;
            this.info_gb.TabStop = false;
            // 
            // monthYearDtp
            // 
            this.monthYearDtp.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthYearDtp.Location = new System.Drawing.Point(5, 19);
            this.monthYearDtp.Name = "monthYearDtp";
            this.monthYearDtp.Size = new System.Drawing.Size(99, 20);
            this.monthYearDtp.TabIndex = 36;
            // 
            // loadAttandance_btn
            // 
            this.loadAttandance_btn.BackColor = System.Drawing.SystemColors.Highlight;
            this.loadAttandance_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadAttandance_btn.Location = new System.Drawing.Point(123, 10);
            this.loadAttandance_btn.Name = "loadAttandance_btn";
            this.loadAttandance_btn.Size = new System.Drawing.Size(64, 37);
            this.loadAttandance_btn.TabIndex = 30;
            this.loadAttandance_btn.Text = "Load";
            this.loadAttandance_btn.UseVisualStyleBackColor = false;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(9, 392);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.log_GV);
            this.panel2.Controls.Add(this.attendanceGV);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(403, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(592, 681);
            this.panel2.TabIndex = 12;
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
            this.attendanceGV.Size = new System.Drawing.Size(592, 681);
            this.attendanceGV.TabIndex = 77;
            // 
            // log_GV
            // 
            this.log_GV.AllowUserToAddRows = false;
            this.log_GV.AllowUserToDeleteRows = false;
            this.log_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.log_GV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.log_GV.Location = new System.Drawing.Point(0, 357);
            this.log_GV.Name = "log_GV";
            this.log_GV.ReadOnly = true;
            this.log_GV.Size = new System.Drawing.Size(592, 324);
            this.log_GV.TabIndex = 78;
            // 
            // Attendance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1304, 681);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGV);
            this.Name = "Attendance";
            this.Text = "FormTableData";
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.attendanceGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button excelAttendance_btn;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Button loadAttandance_btn;
        private System.Windows.Forms.DateTimePicker monthYearDtp;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView attendanceGV;
        private System.Windows.Forms.DataGridView log_GV;
    }
}