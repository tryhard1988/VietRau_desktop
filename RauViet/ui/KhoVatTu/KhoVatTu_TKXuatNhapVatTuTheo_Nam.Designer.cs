
namespace RauViet.ui
{
    partial class KhoVatTu_TKXuatNhapVatTuTheo_Nam
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
            this.exportExcel_btn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.search_tb = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.exportExcel_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1393, 41);
            this.panel1.TabIndex = 0;
            // 
            // exportExcel_btn
            // 
            this.exportExcel_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.exportExcel_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportExcel_btn.Location = new System.Drawing.Point(12, 6);
            this.exportExcel_btn.Name = "exportExcel_btn";
            this.exportExcel_btn.Size = new System.Drawing.Size(94, 32);
            this.exportExcel_btn.TabIndex = 42;
            this.exportExcel_btn.Text = "Xuất Excel";
            this.exportExcel_btn.UseVisualStyleBackColor = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.panel2.Controls.Add(this.dataGV);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 41);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1393, 717);
            this.panel2.TabIndex = 1;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 0);
            this.dataGV.MultiSelect = false;
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGV.Size = new System.Drawing.Size(1393, 717);
            this.dataGV.TabIndex = 73;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.search_tb);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(1033, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(360, 41);
            this.panel3.TabIndex = 45;
            // 
            // search_tb
            // 
            this.search_tb.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.search_tb.Location = new System.Drawing.Point(24, 9);
            this.search_tb.Name = "search_tb";
            this.search_tb.Size = new System.Drawing.Size(324, 24);
            this.search_tb.TabIndex = 44;
            // 
            // KhoVatTu_TKXuatNhapVatTuTheo_Nam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1393, 758);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "KhoVatTu_TKXuatNhapVatTuTheo_Nam";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Button exportExcel_btn;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox search_tb;
    }
}