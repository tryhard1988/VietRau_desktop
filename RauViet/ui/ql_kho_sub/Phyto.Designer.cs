
namespace RauViet.ui
{
    partial class Phyto
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
            this.loading_lb = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.exportCode_cbb);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(798, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(466, 681);
            this.panel1.TabIndex = 9;
            // 
            // exportCode_cbb
            // 
            this.exportCode_cbb.FormattingEnabled = true;
            this.exportCode_cbb.Location = new System.Drawing.Point(-3, 12);
            this.exportCode_cbb.Name = "exportCode_cbb";
            this.exportCode_cbb.Size = new System.Drawing.Size(160, 21);
            this.exportCode_cbb.TabIndex = 52;
            // 
            // loading_lb
            // 
            this.loading_lb.AutoSize = true;
            this.loading_lb.Dock = System.Windows.Forms.DockStyle.Top;
            this.loading_lb.Location = new System.Drawing.Point(0, 0);
            this.loading_lb.Name = "loading_lb";
            this.loading_lb.Size = new System.Drawing.Size(55, 13);
            this.loading_lb.TabIndex = 10;
            this.loading_lb.Text = "loading_lb";
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.status_lb.Location = new System.Drawing.Point(125, 253);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(55, 23);
            this.status_lb.TabIndex = 26;
            this.status_lb.Text = "Email";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(67, 290);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(162, 52);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Xuất File Excel";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
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
            this.dataGV.Size = new System.Drawing.Size(798, 681);
            this.dataGV.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // Phyto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGV);
            this.Name = "Phyto";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ComboBox exportCode_cbb;
        private System.Windows.Forms.Label status_lb;
    }
}