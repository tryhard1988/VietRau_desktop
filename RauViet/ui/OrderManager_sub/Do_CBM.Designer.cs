
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.exportCode_cbb = new System.Windows.Forms.ComboBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cartonSizeGroupGV = new System.Windows.Forms.DataGridView();
            this.cusGroupGV = new System.Windows.Forms.DataGridView();
            this.productGroup_GV = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cartonSizeGroupGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cusGroupGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productGroup_GV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.exportCode_cbb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1089, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(175, 681);
            this.panel1.TabIndex = 9;
            // 
            // exportCode_cbb
            // 
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
            this.cartonSizeGroupGV.Size = new System.Drawing.Size(448, 681);
            this.cartonSizeGroupGV.TabIndex = 11;
            // 
            // cusGroupGV
            // 
            this.cusGroupGV.AllowUserToAddRows = false;
            this.cusGroupGV.AllowUserToDeleteRows = false;
            this.cusGroupGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cusGroupGV.Dock = System.Windows.Forms.DockStyle.Left;
            this.cusGroupGV.Location = new System.Drawing.Point(448, 0);
            this.cusGroupGV.Name = "cusGroupGV";
            this.cusGroupGV.ReadOnly = true;
            this.cusGroupGV.Size = new System.Drawing.Size(454, 681);
            this.cusGroupGV.TabIndex = 12;
            // 
            // productGroup_GV
            // 
            this.productGroup_GV.AllowUserToAddRows = false;
            this.productGroup_GV.AllowUserToDeleteRows = false;
            this.productGroup_GV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.productGroup_GV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productGroup_GV.Location = new System.Drawing.Point(902, 0);
            this.productGroup_GV.Name = "productGroup_GV";
            this.productGroup_GV.ReadOnly = true;
            this.productGroup_GV.Size = new System.Drawing.Size(187, 681);
            this.productGroup_GV.TabIndex = 13;
            // 
            // Do_CBM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.productGroup_GV);
            this.Controls.Add(this.cusGroupGV);
            this.Controls.Add(this.cartonSizeGroupGV);
            this.Controls.Add(this.panel1);
            this.Name = "Do_CBM";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cartonSizeGroupGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cusGroupGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productGroup_GV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ComboBox exportCode_cbb;
        private System.Windows.Forms.DataGridView cartonSizeGroupGV;
        private System.Windows.Forms.DataGridView cusGroupGV;
        private System.Windows.Forms.DataGridView productGroup_GV;
    }
}