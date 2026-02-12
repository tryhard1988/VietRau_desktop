
namespace RauViet.ui
{
    partial class SumaryAttendanceReport
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
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.exportExcel_btn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.exportExcel_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1264, 48);
            this.panel1.TabIndex = 12;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 48);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1264, 676);
            this.panel3.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(987, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(277, 676);
            this.panel2.TabIndex = 14;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Right;
            this.dataGridView1.Location = new System.Drawing.Point(6, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(271, 676);
            this.dataGridView1.TabIndex = 14;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dataGV);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(987, 676);
            this.panel4.TabIndex = 15;
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
            this.dataGV.Size = new System.Drawing.Size(987, 676);
            this.dataGV.TabIndex = 13;
            // 
            // exportExcel_btn
            // 
            this.exportExcel_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.exportExcel_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportExcel_btn.Location = new System.Drawing.Point(10, 9);
            this.exportExcel_btn.Name = "exportExcel_btn";
            this.exportExcel_btn.Size = new System.Drawing.Size(94, 32);
            this.exportExcel_btn.TabIndex = 32;
            this.exportExcel_btn.Text = "Xuất Excel";
            this.exportExcel_btn.UseVisualStyleBackColor = false;
            // 
            // SumaryAttendanceReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 724);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Name = "SumaryAttendanceReport";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button exportExcel_btn;
    }
}