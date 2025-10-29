namespace RauViet.ui
{
    partial class EditExcelFormAllowance_POS
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
            this.Salary_btn = new System.Windows.Forms.Button();
            this.dataGV = new System.Windows.Forms.DataGridView();
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
            this.panel1.Controls.Add(this.Salary_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(953, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(315, 623);
            this.panel1.TabIndex = 2;
            // 
            // Salary_btn
            // 
            this.Salary_btn.Location = new System.Drawing.Point(42, 16);
            this.Salary_btn.Name = "Salary_btn";
            this.Salary_btn.Size = new System.Drawing.Size(215, 23);
            this.Salary_btn.TabIndex = 8;
            this.Salary_btn.Text = "Lương";
            this.Salary_btn.UseVisualStyleBackColor = true;
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
            // EditExcelFormSalary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1268, 647);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "EditExcelFormSalary";
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
        private System.Windows.Forms.Button Salary_btn;
    }
}