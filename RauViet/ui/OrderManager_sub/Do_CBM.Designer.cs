
namespace RauViet.ui
{
    partial class Do_CBM
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cartonSizeGroupGV = new System.Windows.Forms.DataGridView();
            this.cusGroupGV = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.totalCBM_tb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.totalFreightCharge_tb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.totalAmount_tb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.netWeight_tb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.totalCarton_tb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.totalChargeWeight_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.exportCode_cbb = new System.Windows.Forms.ComboBox();
            this.status_lb = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cartonSizeGroupGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cusGroupGV)).BeginInit();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // cartonSizeGroupGV
            // 
            this.cartonSizeGroupGV.AllowUserToAddRows = false;
            this.cartonSizeGroupGV.AllowUserToDeleteRows = false;
            this.cartonSizeGroupGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cartonSizeGroupGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.cartonSizeGroupGV.Location = new System.Drawing.Point(0, 0);
            this.cartonSizeGroupGV.Name = "cartonSizeGroupGV";
            this.cartonSizeGroupGV.ReadOnly = true;
            this.cartonSizeGroupGV.Size = new System.Drawing.Size(508, 681);
            this.cartonSizeGroupGV.TabIndex = 11;
            // 
            // cusGroupGV
            // 
            this.cusGroupGV.AllowUserToAddRows = false;
            this.cusGroupGV.AllowUserToDeleteRows = false;
            this.cusGroupGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cusGroupGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.cusGroupGV.Location = new System.Drawing.Point(508, 0);
            this.cusGroupGV.Name = "cusGroupGV";
            this.cusGroupGV.ReadOnly = true;
            this.cusGroupGV.Size = new System.Drawing.Size(541, 681);
            this.cusGroupGV.TabIndex = 12;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.exportCode_cbb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1049, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(407, 681);
            this.panel1.TabIndex = 13;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.totalCBM_tb);
            this.info_gb.Controls.Add(this.label6);
            this.info_gb.Controls.Add(this.totalFreightCharge_tb);
            this.info_gb.Controls.Add(this.label5);
            this.info_gb.Controls.Add(this.totalAmount_tb);
            this.info_gb.Controls.Add(this.label4);
            this.info_gb.Controls.Add(this.netWeight_tb);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.totalCarton_tb);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Controls.Add(this.totalChargeWeight_tb);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info_gb.Location = new System.Drawing.Point(0, 191);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(346, 298);
            this.info_gb.TabIndex = 55;
            this.info_gb.TabStop = false;
            // 
            // totalCBM_tb
            // 
            this.totalCBM_tb.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.totalCBM_tb.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalCBM_tb.Location = new System.Drawing.Point(157, 231);
            this.totalCBM_tb.Name = "totalCBM_tb";
            this.totalCBM_tb.ReadOnly = true;
            this.totalCBM_tb.Size = new System.Drawing.Size(157, 27);
            this.totalCBM_tb.TabIndex = 42;
            this.totalCBM_tb.Text = "vxcv";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(80, 236);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 16);
            this.label6.TabIndex = 41;
            this.label6.Text = "Tổng CBM:";
            // 
            // totalFreightCharge_tb
            // 
            this.totalFreightCharge_tb.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.totalFreightCharge_tb.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalFreightCharge_tb.Location = new System.Drawing.Point(157, 115);
            this.totalFreightCharge_tb.Name = "totalFreightCharge_tb";
            this.totalFreightCharge_tb.ReadOnly = true;
            this.totalFreightCharge_tb.Size = new System.Drawing.Size(157, 27);
            this.totalFreightCharge_tb.TabIndex = 40;
            this.totalFreightCharge_tb.Text = "vxcv";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 121);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 16);
            this.label5.TabIndex = 39;
            this.label5.Text = "Tổng Freight Charge:";
            // 
            // totalAmount_tb
            // 
            this.totalAmount_tb.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.totalAmount_tb.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalAmount_tb.Location = new System.Drawing.Point(157, 38);
            this.totalAmount_tb.Name = "totalAmount_tb";
            this.totalAmount_tb.ReadOnly = true;
            this.totalAmount_tb.Size = new System.Drawing.Size(157, 27);
            this.totalAmount_tb.TabIndex = 38;
            this.totalAmount_tb.Text = "vxcv";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 16);
            this.label4.TabIndex = 37;
            this.label4.Text = "Tổng Tiền Hàng:";
            // 
            // netWeight_tb
            // 
            this.netWeight_tb.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.netWeight_tb.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.netWeight_tb.Location = new System.Drawing.Point(157, 194);
            this.netWeight_tb.Name = "netWeight_tb";
            this.netWeight_tb.ReadOnly = true;
            this.netWeight_tb.Size = new System.Drawing.Size(157, 27);
            this.netWeight_tb.TabIndex = 36;
            this.netWeight_tb.Text = "vxcv";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(76, 199);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 16);
            this.label3.TabIndex = 35;
            this.label3.Text = "Net Weight:";
            // 
            // totalCarton_tb
            // 
            this.totalCarton_tb.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.totalCarton_tb.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalCarton_tb.Location = new System.Drawing.Point(157, 152);
            this.totalCarton_tb.Name = "totalCarton_tb";
            this.totalCarton_tb.ReadOnly = true;
            this.totalCarton_tb.Size = new System.Drawing.Size(157, 27);
            this.totalCarton_tb.TabIndex = 34;
            this.totalCarton_tb.Text = "vxcv";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 33;
            this.label1.Text = "Tổng Số Thùng:";
            // 
            // totalChargeWeight_tb
            // 
            this.totalChargeWeight_tb.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.totalChargeWeight_tb.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalChargeWeight_tb.Location = new System.Drawing.Point(157, 77);
            this.totalChargeWeight_tb.Name = "totalChargeWeight_tb";
            this.totalChargeWeight_tb.ReadOnly = true;
            this.totalChargeWeight_tb.Size = new System.Drawing.Size(157, 27);
            this.totalChargeWeight_tb.TabIndex = 32;
            this.totalChargeWeight_tb.Text = "vxcv";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 16);
            this.label2.TabIndex = 30;
            this.label2.Text = "Tổng Charge Weight:";
            // 
            // exportCode_cbb
            // 
            this.exportCode_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exportCode_cbb.FormattingEnabled = true;
            this.exportCode_cbb.Location = new System.Drawing.Point(0, 0);
            this.exportCode_cbb.Name = "exportCode_cbb";
            this.exportCode_cbb.Size = new System.Drawing.Size(160, 21);
            this.exportCode_cbb.TabIndex = 52;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(3, 24);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // Do_CBM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1456, 681);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cusGroupGV);
            this.Controls.Add(this.cartonSizeGroupGV);
            this.Name = "Do_CBM";
            this.Text = "FormTableData";
            ((System.ComponentModel.ISupportInitialize)(this.cartonSizeGroupGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cusGroupGV)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.DataGridView cartonSizeGroupGV;
        private System.Windows.Forms.DataGridView cusGroupGV;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox exportCode_cbb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.TextBox totalAmount_tb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox netWeight_tb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox totalCarton_tb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox totalChargeWeight_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox totalFreightCharge_tb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox totalCBM_tb;
        private System.Windows.Forms.Label label6;
    }
}