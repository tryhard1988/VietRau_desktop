
namespace RauViet.ui
{
    partial class MonthlyReportForYear
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel4 = new System.Windows.Forms.Panel();
            this.product_GV = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.load_gb = new System.Windows.Forms.GroupBox();
            this.timeReport_dtp = new System.Windows.Forms.DateTimePicker();
            this.load_btn = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.exportToExcel_btn = new System.Windows.Forms.Button();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.product_GV)).BeginInit();
            this.panel1.SuspendLayout();
            this.load_gb.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.product_GV);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1430, 724);
            this.panel4.TabIndex = 20;
            // 
            // product_GV
            // 
            this.product_GV.AllowUserToAddRows = false;
            this.product_GV.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.product_GV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.product_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.product_GV.DefaultCellStyle = dataGridViewCellStyle2;
            this.product_GV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.product_GV.Location = new System.Drawing.Point(0, 0);
            this.product_GV.Name = "product_GV";
            this.product_GV.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.product_GV.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.product_GV.Size = new System.Drawing.Size(1430, 724);
            this.product_GV.TabIndex = 37;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.exportToExcel_btn);
            this.panel1.Controls.Add(this.load_gb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1430, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(456, 724);
            this.panel1.TabIndex = 21;
            // 
            // load_gb
            // 
            this.load_gb.Controls.Add(this.timeReport_dtp);
            this.load_gb.Controls.Add(this.load_btn);
            this.load_gb.Controls.Add(this.label6);
            this.load_gb.Location = new System.Drawing.Point(10, 51);
            this.load_gb.Name = "load_gb";
            this.load_gb.Size = new System.Drawing.Size(241, 136);
            this.load_gb.TabIndex = 29;
            this.load_gb.TabStop = false;
            // 
            // timeReport_dtp
            // 
            this.timeReport_dtp.Location = new System.Drawing.Point(106, 36);
            this.timeReport_dtp.Name = "timeReport_dtp";
            this.timeReport_dtp.Size = new System.Drawing.Size(63, 20);
            this.timeReport_dtp.TabIndex = 31;
            // 
            // load_btn
            // 
            this.load_btn.BackColor = System.Drawing.SystemColors.Highlight;
            this.load_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.load_btn.Location = new System.Drawing.Point(45, 73);
            this.load_btn.Name = "load_btn";
            this.load_btn.Size = new System.Drawing.Size(145, 46);
            this.load_btn.TabIndex = 30;
            this.load_btn.Text = "Load";
            this.load_btn.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(62, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 16);
            this.label6.TabIndex = 10;
            this.label6.Text = "Năm:";
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(6, 9);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // exportToExcel_btn
            // 
            this.exportToExcel_btn.BackColor = System.Drawing.SystemColors.Highlight;
            this.exportToExcel_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportToExcel_btn.Location = new System.Drawing.Point(71, 405);
            this.exportToExcel_btn.Name = "exportToExcel_btn";
            this.exportToExcel_btn.Size = new System.Drawing.Size(108, 41);
            this.exportToExcel_btn.TabIndex = 30;
            this.exportToExcel_btn.Text = "Xuất File Excel";
            this.exportToExcel_btn.UseVisualStyleBackColor = false;
            // 
            // MonthlyReportForYear
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1886, 724);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.Name = "MonthlyReportForYear";
            this.Text = "FormTableData";
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.product_GV)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.load_gb.ResumeLayout(false);
            this.load_gb.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView product_GV;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox load_gb;
        private System.Windows.Forms.DateTimePicker timeReport_dtp;
        private System.Windows.Forms.Button load_btn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.Button exportToExcel_btn;
    }
}