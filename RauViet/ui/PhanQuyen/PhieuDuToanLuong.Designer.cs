
namespace RauViet.ui
{
    partial class PhieuDuToanLuong
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.search_tb = new System.Windows.Forms.TextBox();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.employeeAllowances_GV = new System.Windows.Forms.DataGridView();
            this.preview_btn = new System.Windows.Forms.Button();
            this.Print_btn = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.employeeAllowances_GV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.search_tb);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1452, 34);
            this.panel2.TabIndex = 13;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(7, 8);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(66, 16);
            this.label16.TabIndex = 20;
            this.label16.Text = "Tìm Kiếm:";
            // 
            // search_tb
            // 
            this.search_tb.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.search_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.search_tb.Location = new System.Drawing.Point(78, 5);
            this.search_tb.Name = "search_tb";
            this.search_tb.Size = new System.Drawing.Size(287, 23);
            this.search_tb.TabIndex = 19;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGV.Location = new System.Drawing.Point(0, 34);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(946, 690);
            this.dataGV.TabIndex = 14;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.preview_btn);
            this.panel1.Controls.Add(this.Print_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1230, 34);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(222, 690);
            this.panel1.TabIndex = 16;
            // 
            // employeeAllowances_GV
            // 
            this.employeeAllowances_GV.AllowUserToAddRows = false;
            this.employeeAllowances_GV.AllowUserToDeleteRows = false;
            this.employeeAllowances_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.employeeAllowances_GV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.employeeAllowances_GV.Location = new System.Drawing.Point(946, 34);
            this.employeeAllowances_GV.Name = "employeeAllowances_GV";
            this.employeeAllowances_GV.ReadOnly = true;
            this.employeeAllowances_GV.Size = new System.Drawing.Size(284, 690);
            this.employeeAllowances_GV.TabIndex = 17;
            // 
            // preview_btn
            // 
            this.preview_btn.BackColor = System.Drawing.Color.MediumAquamarine;
            this.preview_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.preview_btn.Location = new System.Drawing.Point(53, 361);
            this.preview_btn.Name = "preview_btn";
            this.preview_btn.Size = new System.Drawing.Size(105, 34);
            this.preview_btn.TabIndex = 40;
            this.preview_btn.Text = "Xem Trước";
            this.preview_btn.UseVisualStyleBackColor = false;
            // 
            // Print_btn
            // 
            this.Print_btn.BackColor = System.Drawing.Color.MediumAquamarine;
            this.Print_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Print_btn.Location = new System.Drawing.Point(11, 312);
            this.Print_btn.Name = "Print_btn";
            this.Print_btn.Size = new System.Drawing.Size(189, 34);
            this.Print_btn.TabIndex = 39;
            this.Print_btn.Text = "In Phiếu Dự Toán";
            this.Print_btn.UseVisualStyleBackColor = false;
            // 
            // PhieuDuToanLuong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1452, 724);
            this.Controls.Add(this.employeeAllowances_GV);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel2);
            this.Name = "PhieuDuToanLuong";
            this.Text = "FormTableData";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.employeeAllowances_GV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox search_tb;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView employeeAllowances_GV;
        private System.Windows.Forms.Button preview_btn;
        private System.Windows.Forms.Button Print_btn;
    }
}