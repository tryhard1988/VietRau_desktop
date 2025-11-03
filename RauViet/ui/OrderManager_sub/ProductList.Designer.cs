
namespace RauViet.ui
{
    partial class ProductList
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
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.readOnly_btn = new System.Windows.Forms.Button();
            this.edit_btn = new System.Windows.Forms.Button();
            this.info_gb = new System.Windows.Forms.GroupBox();
            this.sku_cbb = new System.Windows.Forms.ComboBox();
            this.packing_panel = new System.Windows.Forms.Panel();
            this.amount_tb = new System.Windows.Forms.TextBox();
            this.amount_lb = new System.Windows.Forms.Label();
            this.GGN_tb = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.artNr_tb = new System.Windows.Forms.TextBox();
            this.barCodeEAN13_tb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.packing_lb = new System.Windows.Forms.Label();
            this.PLU_tb = new System.Windows.Forms.TextBox();
            this.barCode_tb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.id_tb = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.priceCNF_tb = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.delete_btn = new System.Windows.Forms.Button();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.nameEN_tb = new System.Windows.Forms.TextBox();
            this.nameVN_tb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.search_tb = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.info_gb.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.readOnly_btn);
            this.panel1.Controls.Add(this.edit_btn);
            this.panel1.Controls.Add(this.info_gb);
            this.panel1.Controls.Add(this.id_tb);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.priceCNF_tb);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.delete_btn);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.nameEN_tb);
            this.panel1.Controls.Add(this.nameVN_tb);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(861, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 681);
            this.panel1.TabIndex = 0;
            // 
            // newCustomerBtn
            // 
            this.newCustomerBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.newCustomerBtn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCustomerBtn.Location = new System.Drawing.Point(295, 77);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(74, 32);
            this.newCustomerBtn.TabIndex = 46;
            this.newCustomerBtn.Text = "Tạo mới";
            this.newCustomerBtn.UseVisualStyleBackColor = false;
            // 
            // readOnly_btn
            // 
            this.readOnly_btn.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.readOnly_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readOnly_btn.Location = new System.Drawing.Point(327, 77);
            this.readOnly_btn.Name = "readOnly_btn";
            this.readOnly_btn.Size = new System.Drawing.Size(42, 32);
            this.readOnly_btn.TabIndex = 45;
            this.readOnly_btn.Text = "X";
            this.readOnly_btn.UseVisualStyleBackColor = false;
            // 
            // edit_btn
            // 
            this.edit_btn.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.edit_btn.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edit_btn.Location = new System.Drawing.Point(199, 77);
            this.edit_btn.Name = "edit_btn";
            this.edit_btn.Size = new System.Drawing.Size(94, 32);
            this.edit_btn.TabIndex = 44;
            this.edit_btn.Text = "Chỉnh sửa";
            this.edit_btn.UseVisualStyleBackColor = false;
            // 
            // info_gb
            // 
            this.info_gb.Controls.Add(this.sku_cbb);
            this.info_gb.Controls.Add(this.packing_panel);
            this.info_gb.Controls.Add(this.amount_tb);
            this.info_gb.Controls.Add(this.amount_lb);
            this.info_gb.Controls.Add(this.GGN_tb);
            this.info_gb.Controls.Add(this.label10);
            this.info_gb.Controls.Add(this.artNr_tb);
            this.info_gb.Controls.Add(this.barCodeEAN13_tb);
            this.info_gb.Controls.Add(this.label6);
            this.info_gb.Controls.Add(this.label7);
            this.info_gb.Controls.Add(this.packing_lb);
            this.info_gb.Controls.Add(this.PLU_tb);
            this.info_gb.Controls.Add(this.barCode_tb);
            this.info_gb.Controls.Add(this.label3);
            this.info_gb.Controls.Add(this.label2);
            this.info_gb.Controls.Add(this.label1);
            this.info_gb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info_gb.Location = new System.Drawing.Point(17, 102);
            this.info_gb.Name = "info_gb";
            this.info_gb.Size = new System.Drawing.Size(352, 329);
            this.info_gb.TabIndex = 43;
            this.info_gb.TabStop = false;
            // 
            // sku_cbb
            // 
            this.sku_cbb.FormattingEnabled = true;
            this.sku_cbb.IntegralHeight = false;
            this.sku_cbb.Location = new System.Drawing.Point(135, 99);
            this.sku_cbb.Name = "sku_cbb";
            this.sku_cbb.Size = new System.Drawing.Size(193, 24);
            this.sku_cbb.TabIndex = 62;
            // 
            // packing_panel
            // 
            this.packing_panel.Location = new System.Drawing.Point(135, 137);
            this.packing_panel.Name = "packing_panel";
            this.packing_panel.Size = new System.Drawing.Size(193, 25);
            this.packing_panel.TabIndex = 61;
            // 
            // amount_tb
            // 
            this.amount_tb.Location = new System.Drawing.Point(135, 179);
            this.amount_tb.Name = "amount_tb";
            this.amount_tb.Size = new System.Drawing.Size(96, 23);
            this.amount_tb.TabIndex = 60;
            // 
            // amount_lb
            // 
            this.amount_lb.AutoSize = true;
            this.amount_lb.Location = new System.Drawing.Point(20, 181);
            this.amount_lb.Name = "amount_lb";
            this.amount_lb.Size = new System.Drawing.Size(107, 16);
            this.amount_lb.TabIndex = 59;
            this.amount_lb.Text = "Trọng Lượng Gói:";
            // 
            // GGN_tb
            // 
            this.GGN_tb.Location = new System.Drawing.Point(135, 290);
            this.GGN_tb.Name = "GGN_tb";
            this.GGN_tb.Size = new System.Drawing.Size(145, 23);
            this.GGN_tb.TabIndex = 58;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 295);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 16);
            this.label10.TabIndex = 57;
            this.label10.Text = "GGN:";
            // 
            // artNr_tb
            // 
            this.artNr_tb.Location = new System.Drawing.Point(135, 253);
            this.artNr_tb.Name = "artNr_tb";
            this.artNr_tb.Size = new System.Drawing.Size(145, 23);
            this.artNr_tb.TabIndex = 56;
            // 
            // barCodeEAN13_tb
            // 
            this.barCodeEAN13_tb.Location = new System.Drawing.Point(135, 216);
            this.barCodeEAN13_tb.Name = "barCodeEAN13_tb";
            this.barCodeEAN13_tb.Size = new System.Drawing.Size(145, 23);
            this.barCodeEAN13_tb.TabIndex = 55;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 257);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 16);
            this.label6.TabIndex = 54;
            this.label6.Text = "Art.Nr:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 219);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(105, 16);
            this.label7.TabIndex = 53;
            this.label7.Text = "Bar Code EAN13:";
            // 
            // packing_lb
            // 
            this.packing_lb.AutoSize = true;
            this.packing_lb.Location = new System.Drawing.Point(20, 143);
            this.packing_lb.Name = "packing_lb";
            this.packing_lb.Size = new System.Drawing.Size(49, 16);
            this.packing_lb.TabIndex = 52;
            this.packing_lb.Text = "Paking:";
            // 
            // PLU_tb
            // 
            this.PLU_tb.Location = new System.Drawing.Point(135, 62);
            this.PLU_tb.Name = "PLU_tb";
            this.PLU_tb.Size = new System.Drawing.Size(141, 23);
            this.PLU_tb.TabIndex = 51;
            // 
            // barCode_tb
            // 
            this.barCode_tb.Location = new System.Drawing.Point(135, 25);
            this.barCode_tb.Name = "barCode_tb";
            this.barCode_tb.Size = new System.Drawing.Size(141, 23);
            this.barCode_tb.TabIndex = 50;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 16);
            this.label3.TabIndex = 49;
            this.label3.Text = "PLU:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 16);
            this.label2.TabIndex = 48;
            this.label2.Text = "Bar Code:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 105);
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
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(24, 621);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(18, 13);
            this.label11.TabIndex = 41;
            this.label11.Text = "ID";
            this.label11.Visible = false;
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_lb.Location = new System.Drawing.Point(6, 446);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(59, 23);
            this.status_lb.TabIndex = 40;
            this.status_lb.Text = "status";
            // 
            // priceCNF_tb
            // 
            this.priceCNF_tb.Location = new System.Drawing.Point(201, 625);
            this.priceCNF_tb.Name = "priceCNF_tb";
            this.priceCNF_tb.ReadOnly = true;
            this.priceCNF_tb.Size = new System.Drawing.Size(24, 20);
            this.priceCNF_tb.TabIndex = 35;
            this.priceCNF_tb.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(130, 628);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 13);
            this.label8.TabIndex = 30;
            this.label8.Text = "Giá CNF (CHF/Kg)";
            this.label8.Visible = false;
            // 
            // delete_btn
            // 
            this.delete_btn.BackColor = System.Drawing.Color.Red;
            this.delete_btn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.delete_btn.Location = new System.Drawing.Point(231, 437);
            this.delete_btn.Name = "delete_btn";
            this.delete_btn.Size = new System.Drawing.Size(104, 43);
            this.delete_btn.TabIndex = 27;
            this.delete_btn.Text = "Xóa";
            this.delete_btn.UseVisualStyleBackColor = false;
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.BackColor = System.Drawing.SystemColors.Highlight;
            this.LuuThayDoiBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(121, 437);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(104, 43);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = false;
            // 
            // nameEN_tb
            // 
            this.nameEN_tb.Location = new System.Drawing.Point(85, 595);
            this.nameEN_tb.Name = "nameEN_tb";
            this.nameEN_tb.ReadOnly = true;
            this.nameEN_tb.Size = new System.Drawing.Size(50, 20);
            this.nameEN_tb.TabIndex = 20;
            this.nameEN_tb.Visible = false;
            // 
            // nameVN_tb
            // 
            this.nameVN_tb.Location = new System.Drawing.Point(267, 599);
            this.nameVN_tb.Name = "nameVN_tb";
            this.nameVN_tb.ReadOnly = true;
            this.nameVN_tb.Size = new System.Drawing.Size(50, 20);
            this.nameVN_tb.TabIndex = 19;
            this.nameVN_tb.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 598);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Tên tiếng anh";
            this.label5.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(202, 602);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Tên tiếng việt";
            this.label4.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(531, 13);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(66, 16);
            this.label15.TabIndex = 62;
            this.label15.Text = "Tìm Kiếm:";
            // 
            // search_tb
            // 
            this.search_tb.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.search_tb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.search_tb.Location = new System.Drawing.Point(603, 9);
            this.search_tb.Name = "search_tb";
            this.search_tb.Size = new System.Drawing.Size(252, 23);
            this.search_tb.TabIndex = 61;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel2.Controls.Add(this.search_tb);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(861, 42);
            this.panel2.TabIndex = 63;
            // 
            // dataGV
            // 
            this.dataGV.AllowUserToAddRows = false;
            this.dataGV.AllowUserToDeleteRows = false;
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 42);
            this.dataGV.Name = "dataGV";
            this.dataGV.ReadOnly = true;
            this.dataGV.Size = new System.Drawing.Size(861, 639);
            this.dataGV.TabIndex = 64;
            // 
            // ProductList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ProductList";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.info_gb.ResumeLayout(false);
            this.info_gb.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox nameEN_tb;
        private System.Windows.Forms.TextBox nameVN_tb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Button delete_btn;
        private System.Windows.Forms.TextBox priceCNF_tb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.TextBox id_tb;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox info_gb;
        private System.Windows.Forms.ComboBox sku_cbb;
        private System.Windows.Forms.Panel packing_panel;
        private System.Windows.Forms.TextBox amount_tb;
        private System.Windows.Forms.Label amount_lb;
        private System.Windows.Forms.TextBox GGN_tb;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox artNr_tb;
        private System.Windows.Forms.TextBox barCodeEAN13_tb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label packing_lb;
        private System.Windows.Forms.TextBox PLU_tb;
        private System.Windows.Forms.TextBox barCode_tb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox search_tb;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.Button readOnly_btn;
        private System.Windows.Forms.Button edit_btn;
        private System.Windows.Forms.Button newCustomerBtn;
    }
}