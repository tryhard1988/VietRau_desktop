
namespace RauViet.ui
{
    partial class Employee_POS_DEP_CON
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
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.salaryGrade_ccb = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.contractType_cbb = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.position_cbb = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.department_cbb = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.loading_lb = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.employeeCode_tb = new System.Windows.Forms.TextBox();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.employeeCode_tb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(811, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(453, 724);
            this.panel1.TabIndex = 9;
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(168, 408);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(113, 47);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.salaryGrade_ccb);
            this.info_gb.Controls.Add(this.label14);
            this.info_gb.Controls.Add(this.contractType_cbb);
            this.info_gb.Controls.Add(this.status_lb);
            this.info_gb.Controls.Add(this.label5);
            this.info_gb.Controls.Add(this.position_cbb);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Controls.Add(this.department_cbb);
            this.info_gb.Controls.Add(this.label4);
            this.info_gb.Location = new System.Drawing.Point(19, 205);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(422, 176);
            this.info_gb.TabIndex = 28;
            this.info_gb.TabStop = false;
            // 
            // salaryGrade_ccb
            // 
            this.salaryGrade_ccb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.salaryGrade_ccb.FormattingEnabled = true;
            this.salaryGrade_ccb.Location = new System.Drawing.Point(127, 132);
            this.salaryGrade_ccb.Name = "salaryGrade_ccb";
            this.salaryGrade_ccb.Size = new System.Drawing.Size(200, 21);
            this.salaryGrade_ccb.TabIndex = 45;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(53, 134);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 16);
            this.label14.TabIndex = 44;
            this.label14.Text = "Bậc Lương:";
            // 
            // contractType_cbb
            // 
            this.contractType_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.contractType_cbb.FormattingEnabled = true;
            this.contractType_cbb.Location = new System.Drawing.Point(127, 102);
            this.contractType_cbb.Name = "contractType_cbb";
            this.contractType_cbb.Size = new System.Drawing.Size(246, 21);
            this.contractType_cbb.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(29, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 16);
            this.label5.TabIndex = 24;
            this.label5.Text = "Loại Hợp Đồng:";
            // 
            // position_cbb
            // 
            this.position_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.position_cbb.FormattingEnabled = true;
            this.position_cbb.Location = new System.Drawing.Point(127, 69);
            this.position_cbb.Name = "position_cbb";
            this.position_cbb.Size = new System.Drawing.Size(246, 21);
            this.position_cbb.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(64, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 16);
            this.label1.TabIndex = 22;
            this.label1.Text = "Chức Vụ:";
            // 
            // department_cbb
            // 
            this.department_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.department_cbb.FormattingEnabled = true;
            this.department_cbb.Location = new System.Drawing.Point(127, 35);
            this.department_cbb.Name = "department_cbb";
            this.department_cbb.Size = new System.Drawing.Size(215, 21);
            this.department_cbb.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(52, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 19;
            this.label4.Text = "Phòng Ban:";
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
            this.status_lb.Location = new System.Drawing.Point(195, 0);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // employeeCode_tb
            // 
            this.employeeCode_tb.Location = new System.Drawing.Point(358, 3);
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
            this.dataGV.Size = new System.Drawing.Size(811, 724);
            this.dataGV.TabIndex = 10;
            // 
            // Employee_POS_DEP_CON
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 724);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Name = "Employee_POS_DEP_CON";
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
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox department_cbb;
        private System.Windows.Forms.TextBox employeeCode_tb;
        private System.Windows.Forms.ComboBox contractType_cbb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox position_cbb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.ComboBox salaryGrade_ccb;
        private System.Windows.Forms.Label label14;
    }
}