
namespace RauViet.ui
{
    partial class MonthlyAllowance_TienAnCaDem
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.monthYearDtp = new System.Windows.Forms.DateTimePicker();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.allowanceGV = new System.Windows.Forms.DataGridView();
            this.department_GV = new System.Windows.Forms.DataGridView();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowanceGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.department_GV)).BeginInit();
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
            this.panel4.Size = new System.Drawing.Size(1264, 55);
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
            this.monthYearLabel.Size = new System.Drawing.Size(882, 55);
            this.monthYearLabel.TabIndex = 25;
            this.monthYearLabel.Text = "Tháng 11";
            this.monthYearLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel5
            // 
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(382, 55);
            this.panel5.TabIndex = 24;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.monthYearDtp);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(925, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(339, 669);
            this.panel1.TabIndex = 50;
            // 
            // monthYearDtp
            // 
            this.monthYearDtp.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthYearDtp.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthYearDtp.Location = new System.Drawing.Point(21, 17);
            this.monthYearDtp.Name = "monthYearDtp";
            this.monthYearDtp.Size = new System.Drawing.Size(122, 31);
            this.monthYearDtp.TabIndex = 39;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(3, 171);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(99, 233);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(113, 47);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGV);
            this.panel2.Controls.Add(this.allowanceGV);
            this.panel2.Controls.Add(this.department_GV);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 55);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1264, 669);
            this.panel2.TabIndex = 41;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(672, 0);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(253, 669);
            this.dataGV.TabIndex = 55;
            // 
            // allowanceGV
            // 
            this.allowanceGV.AllowUserToAddRows = false;
            this.allowanceGV.AllowUserToDeleteRows = false;
            this.allowanceGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.allowanceGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.allowanceGV.Location = new System.Drawing.Point(212, 0);
            this.allowanceGV.Name = "allowanceGV";
            this.allowanceGV.ReadOnly = true;
            this.allowanceGV.Size = new System.Drawing.Size(460, 669);
            this.allowanceGV.TabIndex = 54;
            // 
            // department_GV
            // 
            this.department_GV.AllowUserToAddRows = false;
            this.department_GV.AllowUserToDeleteRows = false;
            this.department_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.department_GV.Dock = System.Windows.Forms.DockStyle.Left;
            this.department_GV.Location = new System.Drawing.Point(0, 0);
            this.department_GV.Name = "department_GV";
            this.department_GV.ReadOnly = true;
            this.department_GV.Size = new System.Drawing.Size(212, 669);
            this.department_GV.TabIndex = 52;
            // 
            // MonthlyAllowance_TienAnCaDem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 724);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.Name = "MonthlyAllowance_TienAnCaDem";
            this.Text = "FormTableData";
            this.panel4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowanceGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.department_GV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label monthYearLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DateTimePicker monthYearDtp;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView department_GV;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.DataGridView allowanceGV;
    }
}