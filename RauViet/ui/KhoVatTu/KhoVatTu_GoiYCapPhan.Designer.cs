
namespace RauViet.ui
{
    partial class KhoVatTu_GoiYCapPhan
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
            this.farm_cbb = new System.Windows.Forms.ComboBox();
            this.capPhan_btn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.quyDoi_btn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.xodua_tb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.phanbo_tb = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel1.Controls.Add(this.farm_cbb);
            this.panel1.Controls.Add(this.capPhan_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1393, 41);
            this.panel1.TabIndex = 0;
            // 
            // farm_cbb
            // 
            this.farm_cbb.BackColor = System.Drawing.Color.White;
            this.farm_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.farm_cbb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.farm_cbb.ForeColor = System.Drawing.Color.Red;
            this.farm_cbb.FormattingEnabled = true;
            this.farm_cbb.Location = new System.Drawing.Point(89, 8);
            this.farm_cbb.Name = "farm_cbb";
            this.farm_cbb.Size = new System.Drawing.Size(192, 24);
            this.farm_cbb.TabIndex = 44;
            // 
            // capPhan_btn
            // 
            this.capPhan_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.capPhan_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.capPhan_btn.Location = new System.Drawing.Point(300, 4);
            this.capPhan_btn.Name = "capPhan_btn";
            this.capPhan_btn.Size = new System.Drawing.Size(94, 32);
            this.capPhan_btn.TabIndex = 42;
            this.capPhan_btn.Text = "Cấp Phân";
            this.capPhan_btn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.info_gb.Controls.Add(this.quyDoi_btn);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Controls.Add(this.xodua_tb);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.phanbo_tb);
            this.info_gb.Controls.Add(this.label9);
            this.info_gb.Dock = System.Windows.Forms.DockStyle.Right;
            this.info_gb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info_gb.Location = new System.Drawing.Point(1093, 41);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(300, 717);
            this.info_gb.TabIndex = 75;
            this.info_gb.TabStop = false;
            // 
            // quyDoi_btn
            // 
            this.quyDoi_btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.quyDoi_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.quyDoi_btn.Location = new System.Drawing.Point(105, 254);
            this.quyDoi_btn.Name = "quyDoi_btn";
            this.quyDoi_btn.Size = new System.Drawing.Size(94, 47);
            this.quyDoi_btn.TabIndex = 91;
            this.quyDoi_btn.Text = "Quy Đổi";
            this.quyDoi_btn.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(216, 218);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 16);
            this.label2.TabIndex = 90;
            this.label2.Text = "Kg";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(216, 175);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 16);
            this.label1.TabIndex = 89;
            this.label1.Text = "Kg";
            // 
            // xodua_tb
            // 
            this.xodua_tb.Location = new System.Drawing.Point(179, 172);
            this.xodua_tb.Name = "xodua_tb";
            this.xodua_tb.Size = new System.Drawing.Size(33, 23);
            this.xodua_tb.TabIndex = 88;
            this.xodua_tb.Text = "12";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(73, 175);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 16);
            this.label3.TabIndex = 87;
            this.label3.Text = "1 Bao Xơ Dừa =>";
            // 
            // phanbo_tb
            // 
            this.phanbo_tb.Location = new System.Drawing.Point(182, 215);
            this.phanbo_tb.Name = "phanbo_tb";
            this.phanbo_tb.Size = new System.Drawing.Size(30, 23);
            this.phanbo_tb.TabIndex = 78;
            this.phanbo_tb.Text = "20";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(72, 217);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(111, 16);
            this.label9.TabIndex = 77;
            this.label9.Text = "1 Bao Phân Bò =>";
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 41);
            this.dataGV.Name = "dataGV";
            this.dataGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGV.Size = new System.Drawing.Size(1093, 717);
            this.dataGV.TabIndex = 76;
            // 
            // KhoVatTu_GoiYCapPhan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1393, 758);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.info_gb);
            this.Controls.Add(this.panel1);
            this.Name = "KhoVatTu_GoiYCapPhan";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button capPhan_btn;
        private System.Windows.Forms.ComboBox farm_cbb;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.TextBox xodua_tb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox phanbo_tb;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button quyDoi_btn;
    }
}