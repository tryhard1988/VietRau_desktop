
namespace RauViet.ui
{
    partial class KhoVatTu_MaterialExport
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
            this.monthYear_dtp = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.log_GV = new System.Windows.Forms.DataGridView();
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.edit_btn = new System.Windows.Forms.Button();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.note_tb = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.nguoiNhan_CBB = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.congViec_CBB = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ngayXuat_dtp = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.soLuong_tb = new System.Windows.Forms.TextBox();
            this.vatTu_CB = new System.Windows.Forms.ComboBox();
            this.LenhSX_CBB = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.id_tb = new System.Windows.Forms.TextBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).BeginInit();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.monthYear_dtp);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.log_GV);
            this.panel1.Controls.Add(this.readOnly_btn);
            this.panel1.Controls.Add(this.edit_btn);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.id_tb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(816, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(533, 758);
            this.panel1.TabIndex = 69;
            // 
            // monthYear_dtp
            // 
            this.monthYear_dtp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthYear_dtp.Location = new System.Drawing.Point(212, 21);
            this.monthYear_dtp.Name = "monthYear_dtp";
            this.monthYear_dtp.Size = new System.Drawing.Size(98, 26);
            this.monthYear_dtp.TabIndex = 73;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.PeachPuff;
            this.label4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label4.Location = new System.Drawing.Point(0, 541);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(533, 23);
            this.label4.TabIndex = 70;
            this.label4.Text = "Lịch sử thay đổi";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // log_GV
            // 
            this.log_GV.AllowUserToAddRows = false;
            this.log_GV.AllowUserToDeleteRows = false;
            this.log_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.log_GV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.log_GV.Location = new System.Drawing.Point(0, 564);
            this.log_GV.Name = "log_GV";
            this.log_GV.ReadOnly = true;
            this.log_GV.Size = new System.Drawing.Size(533, 194);
            this.log_GV.TabIndex = 69;
            // 
            // readOnly_btn
            // 
            this.readOnly_btn.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.readOnly_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readOnly_btn.Location = new System.Drawing.Point(490, 74);
            this.readOnly_btn.Name = "readOnly_btn";
            this.readOnly_btn.Size = new System.Drawing.Size(42, 32);
            this.readOnly_btn.TabIndex = 48;
            this.readOnly_btn.Text = "X";
            this.readOnly_btn.UseVisualStyleBackColor = false;
            // 
            // edit_btn
            // 
            this.edit_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.edit_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.Location = new System.Drawing.Point(363, 74);
            this.edit_btn.Name = "edit_btn";
            this.edit_btn.Size = new System.Drawing.Size(94, 32);
            this.edit_btn.TabIndex = 47;
            this.edit_btn.Text = "Chỉnh sửa";
            this.edit_btn.UseVisualStyleBackColor = false;
            // 
            // newCustomerBtn
            // 
            this.newCustomerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newCustomerBtn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCustomerBtn.Location = new System.Drawing.Point(459, 74);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(74, 32);
            this.newCustomerBtn.TabIndex = 46;
            this.newCustomerBtn.Text = "Tạo mới";
            this.newCustomerBtn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.note_tb);
            this.info_gb.Controls.Add(this.label8);
            this.info_gb.Controls.Add(this.nguoiNhan_CBB);
            this.info_gb.Controls.Add(this.label6);
            this.info_gb.Controls.Add(this.congViec_CBB);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.ngayXuat_dtp);
            this.info_gb.Controls.Add(this.label5);
            this.info_gb.Controls.Add(this.soLuong_tb);
            this.info_gb.Controls.Add(this.vatTu_CB);
            this.info_gb.Controls.Add(this.LenhSX_CBB);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Controls.Add(this.label7);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info_gb.Location = new System.Drawing.Point(17, 102);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(513, 294);
            this.info_gb.TabIndex = 43;
            this.info_gb.TabStop = false;
            // 
            // note_tb
            // 
            this.note_tb.Location = new System.Drawing.Point(115, 218);
            this.note_tb.Multiline = true;
            this.note_tb.Name = "note_tb";
            this.note_tb.Size = new System.Drawing.Size(245, 54);
            this.note_tb.TabIndex = 80;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 237);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 16);
            this.label8.TabIndex = 79;
            this.label8.Text = "Ghi Chú:";
            // 
            // nguoiNhan_CBB
            // 
            this.nguoiNhan_CBB.FormattingEnabled = true;
            this.nguoiNhan_CBB.IntegralHeight = false;
            this.nguoiNhan_CBB.Location = new System.Drawing.Point(115, 185);
            this.nguoiNhan_CBB.Name = "nguoiNhan_CBB";
            this.nguoiNhan_CBB.Size = new System.Drawing.Size(214, 24);
            this.nguoiNhan_CBB.TabIndex = 78;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(27, 189);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 16);
            this.label6.TabIndex = 77;
            this.label6.Text = "Người Nhận:";
            // 
            // congViec_CBB
            // 
            this.congViec_CBB.FormattingEnabled = true;
            this.congViec_CBB.IntegralHeight = false;
            this.congViec_CBB.Location = new System.Drawing.Point(115, 152);
            this.congViec_CBB.Name = "congViec_CBB";
            this.congViec_CBB.Size = new System.Drawing.Size(157, 24);
            this.congViec_CBB.TabIndex = 76;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 16);
            this.label3.TabIndex = 75;
            this.label3.Text = "Công Việc:";
            // 
            // ngayXuat_dtp
            // 
            this.ngayXuat_dtp.CalendarTitleBackColor = System.Drawing.SystemColors.ControlText;
            this.ngayXuat_dtp.CalendarTitleForeColor = System.Drawing.SystemColors.Highlight;
            this.ngayXuat_dtp.Location = new System.Drawing.Point(115, 22);
            this.ngayXuat_dtp.Name = "ngayXuat_dtp";
            this.ngayXuat_dtp.Size = new System.Drawing.Size(104, 23);
            this.ngayXuat_dtp.TabIndex = 74;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 16);
            this.label5.TabIndex = 73;
            this.label5.Text = "Ngày Xuất:";
            // 
            // soLuong_tb
            // 
            this.soLuong_tb.Location = new System.Drawing.Point(115, 87);
            this.soLuong_tb.Name = "soLuong_tb";
            this.soLuong_tb.Size = new System.Drawing.Size(98, 23);
            this.soLuong_tb.TabIndex = 68;
            // 
            // vatTu_CB
            // 
            this.vatTu_CB.FormattingEnabled = true;
            this.vatTu_CB.IntegralHeight = false;
            this.vatTu_CB.Location = new System.Drawing.Point(115, 54);
            this.vatTu_CB.Name = "vatTu_CB";
            this.vatTu_CB.Size = new System.Drawing.Size(293, 24);
            this.vatTu_CB.TabIndex = 67;
            // 
            // LenhSX_CBB
            // 
            this.LenhSX_CBB.FormattingEnabled = true;
            this.LenhSX_CBB.IntegralHeight = false;
            this.LenhSX_CBB.Location = new System.Drawing.Point(115, 119);
            this.LenhSX_CBB.Name = "LenhSX_CBB";
            this.LenhSX_CBB.Size = new System.Drawing.Size(191, 24);
            this.LenhSX_CBB.TabIndex = 66;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 16);
            this.label2.TabIndex = 65;
            this.label2.Text = "Lệnh SX:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 16);
            this.label7.TabIndex = 53;
            this.label7.Text = "Vật Tư:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 16);
            this.label1.TabIndex = 47;
            this.label1.Text = "Số Lượng:";
            // 
            // id_tb
            // 
            this.id_tb.Location = new System.Drawing.Point(372, 0);
            this.id_tb.Name = "id_tb";
            this.id_tb.ReadOnly = true;
            this.id_tb.Size = new System.Drawing.Size(31, 20);
            this.id_tb.TabIndex = 42;
            this.id_tb.Visible = false;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.Location = new System.Drawing.Point(4, 411);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(59, 23);
            this.status_lb.TabIndex = 40;
            this.status_lb.Text = "status";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(254, 402);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(104, 43);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
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
            this.dataGV.Size = new System.Drawing.Size(816, 758);
            this.dataGV.TabIndex = 70;
            // 
            // KhoVatTu_MaterialExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1349, 758);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Name = "KhoVatTu_MaterialExport";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.log_GV)).EndInit();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button readOnly_btn;
        private System.Windows.Forms.Button edit_btn;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox id_tb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.ComboBox LenhSX_CBB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox vatTu_CB;
        private System.Windows.Forms.TextBox soLuong_tb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView log_GV;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.DateTimePicker monthYear_dtp;
        private System.Windows.Forms.DateTimePicker ngayXuat_dtp;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox note_tb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox nguoiNhan_CBB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox congViec_CBB;
        private System.Windows.Forms.Label label3;
    }
}