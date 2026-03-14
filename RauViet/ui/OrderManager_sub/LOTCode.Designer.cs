
namespace RauViet.ui
{
    partial class LOTCode
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
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.logGV = new System.Windows.Forms.DataGridView();
            this.exportCode_cbb = new System.Windows.Forms.ComboBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.copy_btn = new System.Windows.Forms.Button();
            this.fromExportCode_cbb = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logGV)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGV.Location = new System.Drawing.Point(0, 0);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(703, 681);
            this.dataGV.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.logGV);
            this.panel1.Controls.Add(this.exportCode_cbb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(703, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(561, 681);
            this.panel1.TabIndex = 11;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.PeachPuff;
            this.label11.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label11.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label11.Location = new System.Drawing.Point(0, 366);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(561, 23);
            this.label11.TabIndex = 90;
            this.label11.Text = "Lịch sử thay đổi";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logGV
            // 
            this.logGV.AllowUserToAddRows = false;
            this.logGV.AllowUserToDeleteRows = false;
            this.logGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.logGV.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logGV.Location = new System.Drawing.Point(0, 389);
            this.logGV.Name = "logGV";
            this.logGV.ReadOnly = true;
            this.logGV.Size = new System.Drawing.Size(561, 292);
            this.logGV.TabIndex = 53;
            // 
            // exportCode_cbb
            // 
            this.exportCode_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exportCode_cbb.FormattingEnabled = true;
            this.exportCode_cbb.Location = new System.Drawing.Point(0, 3);
            this.exportCode_cbb.Name = "exportCode_cbb";
            this.exportCode_cbb.Size = new System.Drawing.Size(160, 21);
            this.exportCode_cbb.TabIndex = 52;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(229, 70);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.LightSalmon;
            this.groupBox1.Controls.Add(this.copy_btn);
            this.groupBox1.Controls.Add(this.fromExportCode_cbb);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(62, 156);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(302, 138);
            this.groupBox1.TabIndex = 93;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Copy Mã Lot từ Đơn Hàng khác";
            // 
            // copy_btn
            // 
            this.copy_btn.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.copy_btn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copy_btn.Location = new System.Drawing.Point(126, 82);
            this.copy_btn.Name = "copy_btn";
            this.copy_btn.Size = new System.Drawing.Size(59, 38);
            this.copy_btn.TabIndex = 94;
            this.copy_btn.Text = "Copy";
            this.copy_btn.UseVisualStyleBackColor = false;
            // 
            // fromExportCode_cbb
            // 
            this.fromExportCode_cbb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fromExportCode_cbb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fromExportCode_cbb.FormattingEnabled = true;
            this.fromExportCode_cbb.Location = new System.Drawing.Point(69, 31);
            this.fromExportCode_cbb.Name = "fromExportCode_cbb";
            this.fromExportCode_cbb.Size = new System.Drawing.Size(177, 23);
            this.fromExportCode_cbb.TabIndex = 93;
            // 
            // LOTCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGV);
            this.Name = "LOTCode";
            this.Text = "FormTableData";
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logGV)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridView logGV;
        private System.Windows.Forms.ComboBox exportCode_cbb;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button copy_btn;
        private System.Windows.Forms.ComboBox fromExportCode_cbb;
    }
}