﻿
namespace RauViet.ui
{
    partial class EmployeeNganHang
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
            this.loading_lb = new System.Windows.Forms.Label();
            this.employeeCode_tb = new System.Windows.Forms.TextBox();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.bankName_tb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bankAccountNumber_tb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bankAccountHolder_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bankBranch_tb = new System.Windows.Forms.TextBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.info_gb.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.employeeCode_tb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(778, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(486, 681);
            this.panel1.TabIndex = 9;
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
            // employeeCode_tb
            // 
            this.employeeCode_tb.Location = new System.Drawing.Point(144, 582);
            this.employeeCode_tb.Name = "employeeCode_tb";
            this.employeeCode_tb.ReadOnly = true;
            this.employeeCode_tb.Size = new System.Drawing.Size(32, 20);
            this.employeeCode_tb.TabIndex = 16;
            this.employeeCode_tb.Visible = false;
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
            this.dataGV.Size = new System.Drawing.Size(778, 681);
            this.dataGV.TabIndex = 10;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.bankName_tb);
            this.info_gb.Controls.Add(this.label4);
            this.info_gb.Controls.Add(this.bankAccountNumber_tb);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Controls.Add(this.bankAccountHolder_tb);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.bankBranch_tb);
            this.info_gb.Location = new System.Drawing.Point(9, 170);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(465, 236);
            this.info_gb.TabIndex = 31;
            this.info_gb.TabStop = false;
            // 
            // bankName_tb
            // 
            this.bankName_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bankName_tb.Location = new System.Drawing.Point(135, 54);
            this.bankName_tb.Name = "bankName_tb";
            this.bankName_tb.Size = new System.Drawing.Size(182, 23);
            this.bankName_tb.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(26, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 16);
            this.label4.TabIndex = 22;
            this.label4.Text = "Số Tài Khoản:";
            // 
            // bankAccountNumber_tb
            // 
            this.bankAccountNumber_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bankAccountNumber_tb.Location = new System.Drawing.Point(135, 129);
            this.bankAccountNumber_tb.Name = "bankAccountNumber_tb";
            this.bankAccountNumber_tb.Size = new System.Drawing.Size(182, 23);
            this.bankAccountNumber_tb.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(26, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 16);
            this.label1.TabIndex = 20;
            this.label1.Text = "Chủ Tài Khoản:";
            // 
            // bankAccountHolder_tb
            // 
            this.bankAccountHolder_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bankAccountHolder_tb.Location = new System.Drawing.Point(135, 91);
            this.bankAccountHolder_tb.Name = "bankAccountHolder_tb";
            this.bankAccountHolder_tb.Size = new System.Drawing.Size(270, 23);
            this.bankAccountHolder_tb.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(29, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Tên Ngân Hàng:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(26, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Chi Nhánh:";
            // 
            // bankBranch_tb
            // 
            this.bankBranch_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bankBranch_tb.Location = new System.Drawing.Point(135, 165);
            this.bankBranch_tb.Name = "bankBranch_tb";
            this.bankBranch_tb.Size = new System.Drawing.Size(294, 23);
            this.bankBranch_tb.TabIndex = 18;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(238, 96);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 30;
            this.status_lb.Text = "Email";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(164, 412);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(156, 47);
            this.LuuThayDoiBtn.TabIndex = 29;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // EmployeeNganHang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Name = "EmployeeNganHang";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.TextBox employeeCode_tb;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.TextBox bankName_tb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox bankAccountNumber_tb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox bankAccountHolder_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox bankBranch_tb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Button LuuThayDoiBtn;
    }
}