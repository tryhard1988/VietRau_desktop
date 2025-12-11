
namespace RauViet.ui
{
    partial class ProductDomesticPrices
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
            this.edit_btn = new System.Windows.Forms.Button();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.isActive_cb = new System.Windows.Forms.CheckBox();
            this.sku_cbb = new System.Windows.Forms.ComboBox();
            this.packedPrice_tb = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.refinedPrice_tb = new System.Windows.Forms.TextBox();
            this.rawPrice_tb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.id_tb = new System.Windows.Forms.TextBox();
            this.status_lb = new System.Windows.Forms.Label();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.readOnly_btn);
            this.panel1.Controls.Add(this.edit_btn);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.id_tb);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(861, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 681);
            this.panel1.TabIndex = 0;
            // 
            // edit_btn
            // 
            this.edit_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.edit_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.Location = new System.Drawing.Point(194, 75);
            this.edit_btn.Name = "edit_btn";
            this.edit_btn.Size = new System.Drawing.Size(94, 32);
            this.edit_btn.TabIndex = 47;
            this.edit_btn.Text = "Chỉnh sửa";
            this.edit_btn.UseVisualStyleBackColor = false;
            // 
            // newCustomerBtn
            // 
            this.newCustomerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newCustomerBtn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCustomerBtn.Location = new System.Drawing.Point(290, 75);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(74, 32);
            this.newCustomerBtn.TabIndex = 46;
            this.newCustomerBtn.Text = "Tạo mới";
            this.newCustomerBtn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.isActive_cb);
            this.info_gb.Controls.Add(this.sku_cbb);
            this.info_gb.Controls.Add(this.packedPrice_tb);
            this.info_gb.Controls.Add(this.label10);
            this.info_gb.Controls.Add(this.refinedPrice_tb);
            this.info_gb.Controls.Add(this.rawPrice_tb);
            this.info_gb.Controls.Add(this.label6);
            this.info_gb.Controls.Add(this.label7);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info_gb.Location = new System.Drawing.Point(17, 102);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(352, 205);
            this.info_gb.TabIndex = 43;
            this.info_gb.TabStop = false;
            // 
            // isActive_cb
            // 
            this.isActive_cb.AutoSize = true;
            this.isActive_cb.BackColor = System.Drawing.Color.SandyBrown;
            this.isActive_cb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.isActive_cb.ForeColor = System.Drawing.Color.Black;
            this.isActive_cb.Location = new System.Drawing.Point(143, 174);
            this.isActive_cb.Name = "isActive_cb";
            this.isActive_cb.Size = new System.Drawing.Size(121, 20);
            this.isActive_cb.TabIndex = 63;
            this.isActive_cb.Text = "Đang hoạt động";
            this.isActive_cb.UseVisualStyleBackColor = false;
            // 
            // sku_cbb
            // 
            this.sku_cbb.FormattingEnabled = true;
            this.sku_cbb.IntegralHeight = false;
            this.sku_cbb.Location = new System.Drawing.Point(143, 29);
            this.sku_cbb.Name = "sku_cbb";
            this.sku_cbb.Size = new System.Drawing.Size(193, 24);
            this.sku_cbb.TabIndex = 62;
            // 
            // packedPrice_tb
            // 
            this.packedPrice_tb.Location = new System.Drawing.Point(143, 135);
            this.packedPrice_tb.Name = "packedPrice_tb";
            this.packedPrice_tb.Size = new System.Drawing.Size(145, 23);
            this.packedPrice_tb.TabIndex = 58;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 138);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(119, 16);
            this.label10.TabIndex = 57;
            this.label10.Text = "Giá Hàng Đóng Gói:";
            // 
            // refinedPrice_tb
            // 
            this.refinedPrice_tb.Location = new System.Drawing.Point(143, 100);
            this.refinedPrice_tb.Name = "refinedPrice_tb";
            this.refinedPrice_tb.Size = new System.Drawing.Size(145, 23);
            this.refinedPrice_tb.TabIndex = 56;
            // 
            // rawPrice_tb
            // 
            this.rawPrice_tb.Location = new System.Drawing.Point(143, 65);
            this.rawPrice_tb.Name = "rawPrice_tb";
            this.rawPrice_tb.Size = new System.Drawing.Size(145, 23);
            this.rawPrice_tb.TabIndex = 55;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 16);
            this.label6.TabIndex = 54;
            this.label6.Text = "Giá Hàng Tinh:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 16);
            this.label7.TabIndex = 53;
            this.label7.Text = "Giá Hàng Xá:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 16);
            this.label1.TabIndex = 47;
            this.label1.Text = "Sản Phẩm:";
            // 
            // id_tb
            // 
            this.id_tb.Location = new System.Drawing.Point(40, 621);
            this.id_tb.Name = "id_tb";
            this.id_tb.ReadOnly = true;
            this.id_tb.Size = new System.Drawing.Size(31, 20);
            this.id_tb.TabIndex = 42;
            this.id_tb.Visible = false;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.Location = new System.Drawing.Point(6, 322);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(59, 23);
            this.status_lb.TabIndex = 40;
            this.status_lb.Text = "status";
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(194, 313);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(104, 43);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(861, 36);
            this.panel2.TabIndex = 63;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 36);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(861, 645);
            this.dataGV.TabIndex = 64;
            // 
            // readOnly_btn
            // 
            this.readOnly_btn.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.readOnly_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readOnly_btn.Location = new System.Drawing.Point(321, 75);
            this.readOnly_btn.Name = "readOnly_btn";
            this.readOnly_btn.Size = new System.Drawing.Size(42, 32);
            this.readOnly_btn.TabIndex = 48;
            this.readOnly_btn.Text = "X";
            this.readOnly_btn.UseVisualStyleBackColor = false;
            // 
            // ProductDomesticPrices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ProductDomesticPrices";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.TextBox id_tb;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.ComboBox sku_cbb;
        private System.Windows.Forms.TextBox packedPrice_tb;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox refinedPrice_tb;
        private System.Windows.Forms.TextBox rawPrice_tb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.CheckBox isActive_cb;
        private System.Windows.Forms.Button edit_btn;
        private System.Windows.Forms.Button readOnly_btn;
    }
}