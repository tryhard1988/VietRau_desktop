namespace RauViet.ui
{
    partial class EditExcelFormEmployee
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openFileExcel_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.save_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.importToSQL_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.position_btn = new System.Windows.Forms.Button();
            this.department_btn = new System.Windows.Forms.Button();
            this.Contract_btn = new System.Windows.Forms.Button();
            this.gender_btn = new System.Windows.Forms.Button();
            this.EmployeeID_btn = new System.Windows.Forms.Button();
            this.BirthDate_btn = new System.Windows.Forms.Button();
            this.HireDate_btn = new System.Windows.Forms.Button();
            this.IssueDate_btn = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileExcel_mi,
            this.save_mi,
            this.importToSQL_mi});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1268, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openFileExcel_mi
            // 
            this.openFileExcel_mi.Name = "openFileExcel_mi";
            this.openFileExcel_mi.Size = new System.Drawing.Size(48, 20);
            this.openFileExcel_mi.Text = "Open";
            // 
            // save_mi
            // 
            this.save_mi.Name = "save_mi";
            this.save_mi.Size = new System.Drawing.Size(43, 20);
            this.save_mi.Text = "Save";
            // 
            // importToSQL_mi
            // 
            this.importToSQL_mi.Name = "importToSQL_mi";
            this.importToSQL_mi.Size = new System.Drawing.Size(95, 20);
            this.importToSQL_mi.Text = "Import To SQL";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.IssueDate_btn);
            this.panel1.Controls.Add(this.HireDate_btn);
            this.panel1.Controls.Add(this.BirthDate_btn);
            this.panel1.Controls.Add(this.EmployeeID_btn);
            this.panel1.Controls.Add(this.gender_btn);
            this.panel1.Controls.Add(this.Contract_btn);
            this.panel1.Controls.Add(this.department_btn);
            this.panel1.Controls.Add(this.position_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(953, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(315, 623);
            this.panel1.TabIndex = 2;
            // 
            // dataGV
            // 
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 24);
            this.dataGV.Name = "dataGV";
            this.dataGV.Size = new System.Drawing.Size(953, 623);
            this.dataGV.TabIndex = 3;
            // 
            // position_btn
            // 
            this.position_btn.Location = new System.Drawing.Point(27, 3);
            this.position_btn.Name = "position_btn";
            this.position_btn.Size = new System.Drawing.Size(215, 23);
            this.position_btn.TabIndex = 0;
            this.position_btn.Text = "Position";
            this.position_btn.UseVisualStyleBackColor = true;
            // 
            // department_btn
            // 
            this.department_btn.Location = new System.Drawing.Point(27, 42);
            this.department_btn.Name = "department_btn";
            this.department_btn.Size = new System.Drawing.Size(215, 23);
            this.department_btn.TabIndex = 1;
            this.department_btn.Text = "department";
            this.department_btn.UseVisualStyleBackColor = true;
            // 
            // Contract_btn
            // 
            this.Contract_btn.Location = new System.Drawing.Point(27, 86);
            this.Contract_btn.Name = "Contract_btn";
            this.Contract_btn.Size = new System.Drawing.Size(215, 23);
            this.Contract_btn.TabIndex = 2;
            this.Contract_btn.Text = "Contract";
            this.Contract_btn.UseVisualStyleBackColor = true;
            // 
            // gender_btn
            // 
            this.gender_btn.Location = new System.Drawing.Point(27, 127);
            this.gender_btn.Name = "gender_btn";
            this.gender_btn.Size = new System.Drawing.Size(215, 23);
            this.gender_btn.TabIndex = 3;
            this.gender_btn.Text = "Gender";
            this.gender_btn.UseVisualStyleBackColor = true;
            // 
            // EmployeeID_btn
            // 
            this.EmployeeID_btn.Location = new System.Drawing.Point(27, 173);
            this.EmployeeID_btn.Name = "EmployeeID_btn";
            this.EmployeeID_btn.Size = new System.Drawing.Size(215, 23);
            this.EmployeeID_btn.TabIndex = 4;
            this.EmployeeID_btn.Text = "Employee ID";
            this.EmployeeID_btn.UseVisualStyleBackColor = true;
            this.EmployeeID_btn.Click += new System.EventHandler(this.EmployeeID_btn_Click);
            // 
            // BirthDate_btn
            // 
            this.BirthDate_btn.Location = new System.Drawing.Point(27, 214);
            this.BirthDate_btn.Name = "BirthDate_btn";
            this.BirthDate_btn.Size = new System.Drawing.Size(215, 23);
            this.BirthDate_btn.TabIndex = 5;
            this.BirthDate_btn.Text = "BirthDate";
            this.BirthDate_btn.UseVisualStyleBackColor = true;
            this.BirthDate_btn.Click += new System.EventHandler(this.BirthDate_btn_Click);
            // 
            // HireDate_btn
            // 
            this.HireDate_btn.Location = new System.Drawing.Point(27, 252);
            this.HireDate_btn.Name = "HireDate_btn";
            this.HireDate_btn.Size = new System.Drawing.Size(215, 23);
            this.HireDate_btn.TabIndex = 6;
            this.HireDate_btn.Text = "HireDate";
            this.HireDate_btn.UseVisualStyleBackColor = true;
            this.HireDate_btn.Click += new System.EventHandler(this.HireDate_btn_Click);
            // 
            // IssueDate_btn
            // 
            this.IssueDate_btn.Location = new System.Drawing.Point(27, 281);
            this.IssueDate_btn.Name = "IssueDate_btn";
            this.IssueDate_btn.Size = new System.Drawing.Size(215, 23);
            this.IssueDate_btn.TabIndex = 7;
            this.IssueDate_btn.Text = "IssueDate";
            this.IssueDate_btn.UseVisualStyleBackColor = true;
            this.IssueDate_btn.Click += new System.EventHandler(this.IssueDate_btn_Click);
            // 
            // EditExcelFormEmployee
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1268, 647);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "EditExcelFormEmployee";
            this.Text = "EditExcelForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openFileExcel_mi;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.ToolStripMenuItem save_mi;
        private System.Windows.Forms.ToolStripMenuItem importToSQL_mi;
        private System.Windows.Forms.Button position_btn;
        private System.Windows.Forms.Button department_btn;
        private System.Windows.Forms.Button Contract_btn;
        private System.Windows.Forms.Button gender_btn;
        private System.Windows.Forms.Button EmployeeID_btn;
        private System.Windows.Forms.Button HireDate_btn;
        private System.Windows.Forms.Button BirthDate_btn;
        private System.Windows.Forms.Button IssueDate_btn;
    }
}