
namespace RauViet.ui
{
    partial class TienRauCuaNhanVienTrongThang
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
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.monthYear_dtp = new System.Windows.Forms.DateTimePicker();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.totalAmount_label = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.dataGV);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(873, 681);
            this.panel2.TabIndex = 70;
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
            this.dataGV.Size = new System.Drawing.Size(873, 681);
            this.dataGV.TabIndex = 66;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.totalAmount_label);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 651);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(873, 30);
            this.panel3.TabIndex = 67;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.monthYear_dtp);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(873, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 681);
            this.panel1.TabIndex = 71;
            // 
            // monthYear_dtp
            // 
            this.monthYear_dtp.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthYear_dtp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthYear_dtp.Location = new System.Drawing.Point(138, 255);
            this.monthYear_dtp.Name = "monthYear_dtp";
            this.monthYear_dtp.Size = new System.Drawing.Size(113, 26);
            this.monthYear_dtp.TabIndex = 26;
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.Color.Tomato;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(117, 313);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(148, 43);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Chốt Tháng";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // totalAmount_label
            // 
            this.totalAmount_label.AutoSize = true;
            this.totalAmount_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.totalAmount_label.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalAmount_label.ForeColor = System.Drawing.Color.Firebrick;
            this.totalAmount_label.Location = new System.Drawing.Point(120, 0);
            this.totalAmount_label.Name = "totalAmount_label";
            this.totalAmount_label.Size = new System.Drawing.Size(90, 19);
            this.totalAmount_label.TabIndex = 49;
            this.totalAmount_label.Text = "Số Lượng:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Left;
            this.label10.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(0, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(120, 19);
            this.label10.TabIndex = 48;
            this.label10.Text = "Tổng Tiền Rau:";
            // 
            // TienRauCuaNhanVienTrongThang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1349, 681);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "TienRauCuaNhanVienTrongThang";
            this.Text = "FormTableData";
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DateTimePicker monthYear_dtp;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Label totalAmount_label;
        private System.Windows.Forms.Label label10;
    }
}