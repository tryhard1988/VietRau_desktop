﻿
namespace RauViet.ui
{
    partial class EmployeeDeduction_ATT
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
            this.load_gb = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.load_btn = new System.Windows.Forms.Button();
            this.month_cbb = new System.Windows.Forms.ComboBox();
            this.year_tb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.amount_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.employeeDeductionID_tb = new System.Windows.Forms.TextBox();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.employeeDeductionGV = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.load_gb.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.employeeDeductionGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.load_gb);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.employeeDeductionID_tb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(976, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(288, 724);
            this.panel1.TabIndex = 9;
            // 
            // load_gb
            // 
            this.load_gb.Controls.Add(this.label3);
            this.load_gb.Controls.Add(this.load_btn);
            this.load_gb.Controls.Add(this.month_cbb);
            this.load_gb.Controls.Add(this.year_tb);
            this.load_gb.Controls.Add(this.label6);
            this.load_gb.Location = new System.Drawing.Point(23, 16);
            this.load_gb.Name = "load_gb";
            this.load_gb.Size = new System.Drawing.Size(253, 136);
            this.load_gb.TabIndex = 29;
            this.load_gb.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(136, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 16);
            this.label3.TabIndex = 31;
            this.label3.Text = "/";
            // 
            // load_btn
            // 
            this.load_btn.BackColor = System.Drawing.SystemColors.Highlight;
            this.load_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.load_btn.Location = new System.Drawing.Point(51, 58);
            this.load_btn.Name = "load_btn";
            this.load_btn.Size = new System.Drawing.Size(145, 46);
            this.load_btn.TabIndex = 30;
            this.load_btn.Text = "Load";
            this.load_btn.UseVisualStyleBackColor = false;
            // 
            // month_cbb
            // 
            this.month_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.month_cbb.FormattingEnabled = true;
            this.month_cbb.Location = new System.Drawing.Point(91, 29);
            this.month_cbb.Name = "month_cbb";
            this.month_cbb.Size = new System.Drawing.Size(39, 21);
            this.month_cbb.TabIndex = 21;
            // 
            // year_tb
            // 
            this.year_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.year_tb.Location = new System.Drawing.Point(154, 29);
            this.year_tb.Name = "year_tb";
            this.year_tb.Size = new System.Drawing.Size(69, 23);
            this.year_tb.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(36, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 16);
            this.label6.TabIndex = 10;
            this.label6.Text = "Tháng:";
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.amount_tb);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Location = new System.Drawing.Point(23, 282);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(253, 70);
            this.info_gb.TabIndex = 28;
            this.info_gb.TabStop = false;
            // 
            // amount_tb
            // 
            this.amount_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.amount_tb.Location = new System.Drawing.Point(91, 19);
            this.amount_tb.Name = "amount_tb";
            this.amount_tb.Size = new System.Drawing.Size(137, 23);
            this.amount_tb.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(25, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Số Tiền:";
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(6, 355);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(106, 370);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(113, 47);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // employeeDeductionID_tb
            // 
            this.employeeDeductionID_tb.Location = new System.Drawing.Point(144, 635);
            this.employeeDeductionID_tb.Name = "employeeDeductionID_tb";
            this.employeeDeductionID_tb.ReadOnly = true;
            this.employeeDeductionID_tb.Size = new System.Drawing.Size(32, 20);
            this.employeeDeductionID_tb.TabIndex = 16;
            this.employeeDeductionID_tb.Visible = false;
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
            this.dataGV.Size = new System.Drawing.Size(656, 724);
            this.dataGV.TabIndex = 10;
            // 
            // employeeDeductionGV
            // 
            this.employeeDeductionGV.AllowUserToAddRows = false;
            this.employeeDeductionGV.AllowUserToDeleteRows = false;
            this.employeeDeductionGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.employeeDeductionGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.employeeDeductionGV.Location = new System.Drawing.Point(656, 0);
            this.employeeDeductionGV.Name = "employeeDeductionGV";
            this.employeeDeductionGV.ReadOnly = true;
            this.employeeDeductionGV.Size = new System.Drawing.Size(320, 724);
            this.employeeDeductionGV.TabIndex = 11;
            // 
            // EmployeeDeduction_ATT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 724);
            this.Controls.Add(this.employeeDeductionGV);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Name = "EmployeeDeduction_ATT";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.load_gb.ResumeLayout(false);
            this.load_gb.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.employeeDeductionGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox amount_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.TextBox employeeDeductionID_tb;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.DataGridView employeeDeductionGV;
        private System.Windows.Forms.ComboBox month_cbb;
        private System.Windows.Forms.TextBox year_tb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox load_gb;
        private System.Windows.Forms.Button load_btn;
        private System.Windows.Forms.Label label3;
    }
}