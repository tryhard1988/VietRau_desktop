namespace RauViet.ui
{
    partial class LoadExcelOrderReport
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
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.AddExportCode_btn = new System.Windows.Forms.Button();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.year_tb = new System.Windows.Forms.TextBox();
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
            this.importToSQL_mi.Size = new System.Drawing.Size(94, 20);
            this.importToSQL_mi.Text = "Import To SQL";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.year_tb);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.AddExportCode_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(953, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(315, 623);
            this.panel1.TabIndex = 2;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(87, 57);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(173, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Edit Package";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(87, 171);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(173, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Thu Gọn Trong Tháng";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(87, 142);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(173, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Gọn Sản Phẩm Cùng Tên";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AddExportCode_btn
            // 
            this.AddExportCode_btn.Location = new System.Drawing.Point(87, 97);
            this.AddExportCode_btn.Name = "AddExportCode_btn";
            this.AddExportCode_btn.Size = new System.Drawing.Size(173, 23);
            this.AddExportCode_btn.TabIndex = 0;
            this.AddExportCode_btn.Text = "Add ExportCode";
            this.AddExportCode_btn.UseVisualStyleBackColor = true;
            this.AddExportCode_btn.Click += new System.EventHandler(this.AddExportCode_btn_Click);
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
            // year_tb
            // 
            this.year_tb.Location = new System.Drawing.Point(77, 252);
            this.year_tb.Name = "year_tb";
            this.year_tb.Size = new System.Drawing.Size(121, 20);
            this.year_tb.TabIndex = 4;
            // 
            // LoadExcelOrderReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1268, 647);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "LoadExcelOrderReport";
            this.Text = "EditExcelForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.Button AddExportCode_btn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox year_tb;
    }
}