
namespace RauViet.ui
{
    partial class Employee
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
            this.label13 = new System.Windows.Forms.Label();
            this.issuePlace_tb = new System.Windows.Forms.TextBox();
            this.issueDate_dtp = new System.Windows.Forms.DateTimePicker();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.citizenId_tb = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.address_tb = new System.Windows.Forms.TextBox();
            this.gender_cb = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.hireDate_dtp = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.birthdate_dtp = new System.Windows.Forms.DateTimePicker();
            this.nvCode_tb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.contractType_cbb = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.position_cbb = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.isActive_cb = new System.Windows.Forms.CheckBox();
            this.department_cbb = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tenNV_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.hometown_tb = new System.Windows.Forms.TextBox();
            this.delete_btn = new System.Windows.Forms.Button();
            this.loading_lb = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.employeeID_tb = new System.Windows.Forms.TextBox();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.canCreateUserName_cb = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.delete_btn);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.employeeID_tb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(811, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(453, 724);
            this.panel1.TabIndex = 9;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.canCreateUserName_cb);
            this.info_gb.Controls.Add(this.label13);
            this.info_gb.Controls.Add(this.issuePlace_tb);
            this.info_gb.Controls.Add(this.issueDate_dtp);
            this.info_gb.Controls.Add(this.label12);
            this.info_gb.Controls.Add(this.label11);
            this.info_gb.Controls.Add(this.citizenId_tb);
            this.info_gb.Controls.Add(this.label10);
            this.info_gb.Controls.Add(this.address_tb);
            this.info_gb.Controls.Add(this.gender_cb);
            this.info_gb.Controls.Add(this.label9);
            this.info_gb.Controls.Add(this.label8);
            this.info_gb.Controls.Add(this.hireDate_dtp);
            this.info_gb.Controls.Add(this.label7);
            this.info_gb.Controls.Add(this.birthdate_dtp);
            this.info_gb.Controls.Add(this.nvCode_tb);
            this.info_gb.Controls.Add(this.label6);
            this.info_gb.Controls.Add(this.contractType_cbb);
            this.info_gb.Controls.Add(this.label5);
            this.info_gb.Controls.Add(this.position_cbb);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Controls.Add(this.isActive_cb);
            this.info_gb.Controls.Add(this.department_cbb);
            this.info_gb.Controls.Add(this.label4);
            this.info_gb.Controls.Add(this.tenNV_tb);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.hometown_tb);
            this.info_gb.Location = new System.Drawing.Point(16, 56);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(422, 473);
            this.info_gb.TabIndex = 28;
            this.info_gb.TabStop = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(13, 299);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 16);
            this.label13.TabIndex = 41;
            this.label13.Text = "Nơi Cấp:";
            // 
            // issuePlace_tb
            // 
            this.issuePlace_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.issuePlace_tb.Location = new System.Drawing.Point(128, 290);
            this.issuePlace_tb.Name = "issuePlace_tb";
            this.issuePlace_tb.Size = new System.Drawing.Size(287, 23);
            this.issuePlace_tb.TabIndex = 42;
            // 
            // issueDate_dtp
            // 
            this.issueDate_dtp.Location = new System.Drawing.Point(128, 262);
            this.issueDate_dtp.Name = "issueDate_dtp";
            this.issueDate_dtp.Size = new System.Drawing.Size(113, 20);
            this.issueDate_dtp.TabIndex = 40;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(13, 269);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 16);
            this.label12.TabIndex = 38;
            this.label12.Text = "Ngày Cấp:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(13, 239);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(83, 16);
            this.label11.TabIndex = 36;
            this.label11.Text = "CCCD/CMND:";
            // 
            // citizenId_tb
            // 
            this.citizenId_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.citizenId_tb.Location = new System.Drawing.Point(128, 231);
            this.citizenId_tb.Name = "citizenId_tb";
            this.citizenId_tb.Size = new System.Drawing.Size(287, 23);
            this.citizenId_tb.TabIndex = 37;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(13, 209);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(104, 16);
            this.label10.TabIndex = 34;
            this.label10.Text = "Địa Chỉ Hiện Tại:";
            // 
            // address_tb
            // 
            this.address_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.address_tb.Location = new System.Drawing.Point(128, 200);
            this.address_tb.Name = "address_tb";
            this.address_tb.Size = new System.Drawing.Size(287, 23);
            this.address_tb.TabIndex = 35;
            // 
            // gender_cb
            // 
            this.gender_cb.AutoSize = true;
            this.gender_cb.Location = new System.Drawing.Point(128, 144);
            this.gender_cb.Name = "gender_cb";
            this.gender_cb.Size = new System.Drawing.Size(69, 17);
            this.gender_cb.TabIndex = 33;
            this.gender_cb.Text = "Nam Giới";
            this.gender_cb.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(13, 149);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 16);
            this.label9.TabIndex = 32;
            this.label9.Text = "Giới Tính:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(13, 119);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 16);
            this.label8.TabIndex = 31;
            this.label8.Text = "Ngày Vào làm:";
            // 
            // hireDate_dtp
            // 
            this.hireDate_dtp.Location = new System.Drawing.Point(128, 116);
            this.hireDate_dtp.Name = "hireDate_dtp";
            this.hireDate_dtp.Size = new System.Drawing.Size(113, 20);
            this.hireDate_dtp.TabIndex = 30;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(13, 89);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 16);
            this.label7.TabIndex = 29;
            this.label7.Text = "Ngày Sinh:";
            // 
            // birthdate_dtp
            // 
            this.birthdate_dtp.Location = new System.Drawing.Point(128, 88);
            this.birthdate_dtp.Name = "birthdate_dtp";
            this.birthdate_dtp.Size = new System.Drawing.Size(113, 20);
            this.birthdate_dtp.TabIndex = 28;
            // 
            // nvCode_tb
            // 
            this.nvCode_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nvCode_tb.Location = new System.Drawing.Point(128, 26);
            this.nvCode_tb.Name = "nvCode_tb";
            this.nvCode_tb.Size = new System.Drawing.Size(117, 23);
            this.nvCode_tb.TabIndex = 27;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(13, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 16);
            this.label6.TabIndex = 26;
            this.label6.Text = "mã NV:";
            // 
            // contractType_cbb
            // 
            this.contractType_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.contractType_cbb.FormattingEnabled = true;
            this.contractType_cbb.Location = new System.Drawing.Point(128, 391);
            this.contractType_cbb.Name = "contractType_cbb";
            this.contractType_cbb.Size = new System.Drawing.Size(246, 21);
            this.contractType_cbb.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(13, 389);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 16);
            this.label5.TabIndex = 24;
            this.label5.Text = "Loại Hợp Đồng:";
            // 
            // position_cbb
            // 
            this.position_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.position_cbb.FormattingEnabled = true;
            this.position_cbb.Location = new System.Drawing.Point(128, 350);
            this.position_cbb.Name = "position_cbb";
            this.position_cbb.Size = new System.Drawing.Size(246, 21);
            this.position_cbb.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 359);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 16);
            this.label1.TabIndex = 22;
            this.label1.Text = "Chức Vụ:";
            // 
            // isActive_cb
            // 
            this.isActive_cb.AutoSize = true;
            this.isActive_cb.Location = new System.Drawing.Point(17, 438);
            this.isActive_cb.Name = "isActive_cb";
            this.isActive_cb.Size = new System.Drawing.Size(107, 17);
            this.isActive_cb.TabIndex = 21;
            this.isActive_cb.Text = "Đang Hoạt Động";
            this.isActive_cb.UseVisualStyleBackColor = true;
            // 
            // department_cbb
            // 
            this.department_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.department_cbb.FormattingEnabled = true;
            this.department_cbb.Location = new System.Drawing.Point(128, 321);
            this.department_cbb.Name = "department_cbb";
            this.department_cbb.Size = new System.Drawing.Size(246, 21);
            this.department_cbb.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 329);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 19;
            this.label4.Text = "Phòng Ban:";
            // 
            // tenNV_tb
            // 
            this.tenNV_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tenNV_tb.Location = new System.Drawing.Point(128, 57);
            this.tenNV_tb.Name = "tenNV_tb";
            this.tenNV_tb.Size = new System.Drawing.Size(161, 23);
            this.tenNV_tb.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Tên NV:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Quê Quán:";
            // 
            // hometown_tb
            // 
            this.hometown_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hometown_tb.Location = new System.Drawing.Point(128, 169);
            this.hometown_tb.Name = "hometown_tb";
            this.hometown_tb.Size = new System.Drawing.Size(287, 23);
            this.hometown_tb.TabIndex = 18;
            // 
            // delete_btn
            // 
            this.delete_btn.BackColor = System.Drawing.Color.Red;
            this.delete_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delete_btn.Location = new System.Drawing.Point(300, 573);
            this.delete_btn.Name = "delete_btn";
            this.delete_btn.Size = new System.Drawing.Size(113, 47);
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
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(202, 30);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // newCustomerBtn
            // 
            this.newCustomerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newCustomerBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCustomerBtn.Location = new System.Drawing.Point(174, 573);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(113, 47);
            this.newCustomerBtn.TabIndex = 25;
            this.newCustomerBtn.Text = "Tạo mới";
            this.newCustomerBtn.UseVisualStyleBackColor = false;
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(48, 573);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(113, 47);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // employeeID_tb
            // 
            this.employeeID_tb.Location = new System.Drawing.Point(144, 635);
            this.employeeID_tb.Name = "employeeID_tb";
            this.employeeID_tb.ReadOnly = true;
            this.employeeID_tb.Size = new System.Drawing.Size(32, 20);
            this.employeeID_tb.TabIndex = 16;
            this.employeeID_tb.Visible = false;
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
            this.dataGV.Size = new System.Drawing.Size(811, 724);
            this.dataGV.TabIndex = 10;
            // 
            // canCreateUserName_cb
            // 
            this.canCreateUserName_cb.AutoSize = true;
            this.canCreateUserName_cb.Location = new System.Drawing.Point(164, 438);
            this.canCreateUserName_cb.Name = "canCreateUserName_cb";
            this.canCreateUserName_cb.Size = new System.Drawing.Size(92, 17);
            this.canCreateUserName_cb.TabIndex = 43;
            this.canCreateUserName_cb.Text = "Có UserName";
            this.canCreateUserName_cb.UseVisualStyleBackColor = true;
            // 
            // Employee
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 724);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Name = "Employee";
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
        private System.Windows.Forms.TextBox hometown_tb;
        private System.Windows.Forms.TextBox tenNV_tb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.Button delete_btn;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox isActive_cb;
        private System.Windows.Forms.ComboBox department_cbb;
        private System.Windows.Forms.TextBox employeeID_tb;
        private System.Windows.Forms.ComboBox contractType_cbb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox position_cbb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.TextBox nvCode_tb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker birthdate_dtp;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker hireDate_dtp;
        private System.Windows.Forms.CheckBox gender_cb;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox address_tb;
        private System.Windows.Forms.DateTimePicker issueDate_dtp;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox citizenId_tb;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox issuePlace_tb;
        private System.Windows.Forms.CheckBox canCreateUserName_cb;
    }
}