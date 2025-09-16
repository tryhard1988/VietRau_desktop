
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
            this.delete_btn = new System.Windows.Forms.Button();
            this.loading_lb = new System.Windows.Forms.Label();
            this.newCustomerBtn = new System.Windows.Forms.Button();
            this.LuuThayDoiBtn = new System.Windows.Forms.Button();
            this.nameEN_tb = new System.Windows.Forms.TextBox();
            this.nameVN_tb = new System.Windows.Forms.TextBox();
            this.PLU_tb = new System.Windows.Forms.TextBox();
            this.barCode_tb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.artNr_tb = new System.Windows.Forms.TextBox();
            this.barCodeEAN13_tb = new System.Windows.Forms.TextBox();
            this.priceCNF_tb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.packing_lb = new System.Windows.Forms.Label();
            this.GGN_tb = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.status_lb = new System.Windows.Forms.Label();
            this.id_tb = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.botanicalName_tb = new System.Windows.Forms.TextBox();
            this.amount_tb = new System.Windows.Forms.TextBox();
            this.amount_lb = new System.Windows.Forms.Label();
            this.packing_panel = new System.Windows.Forms.Panel();
            this.sku_cbb = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.sku_cbb);
            this.panel1.Controls.Add(this.packing_panel);
            this.panel1.Controls.Add(this.amount_tb);
            this.panel1.Controls.Add(this.amount_lb);
            this.panel1.Controls.Add(this.id_tb);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.status_lb);
            this.panel1.Controls.Add(this.GGN_tb);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.artNr_tb);
            this.panel1.Controls.Add(this.barCodeEAN13_tb);
            this.panel1.Controls.Add(this.priceCNF_tb);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.packing_lb);
            this.panel1.Controls.Add(this.delete_btn);
            this.panel1.Controls.Add(this.loading_lb);
            this.panel1.Controls.Add(this.newCustomerBtn);
            this.panel1.Controls.Add(this.LuuThayDoiBtn);
            this.panel1.Controls.Add(this.nameEN_tb);
            this.panel1.Controls.Add(this.nameVN_tb);
            this.panel1.Controls.Add(this.botanicalName_tb);
            this.panel1.Controls.Add(this.PLU_tb);
            this.panel1.Controls.Add(this.barCode_tb);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(861, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 681);
            this.panel1.TabIndex = 0;
            // 
            // delete_btn
            // 
            this.delete_btn.Location = new System.Drawing.Point(272, 475);
            this.delete_btn.Name = "delete_btn";
            this.delete_btn.Size = new System.Drawing.Size(124, 23);
            this.delete_btn.TabIndex = 27;
            this.delete_btn.Text = "Xóa";
            this.delete_btn.UseVisualStyleBackColor = true;
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
            // newCustomerBtn
            // 
            this.newCustomerBtn.Location = new System.Drawing.Point(267, 36);
            this.newCustomerBtn.Name = "newCustomerBtn";
            this.newCustomerBtn.Size = new System.Drawing.Size(124, 23);
            this.newCustomerBtn.TabIndex = 25;
            this.newCustomerBtn.Text = "Tạo mới khách hàng";
            this.newCustomerBtn.UseVisualStyleBackColor = true;
            // 
            // LuuThayDoiBtn
            // 
            this.LuuThayDoiBtn.Location = new System.Drawing.Point(50, 475);
            this.LuuThayDoiBtn.Name = "LuuThayDoiBtn";
            this.LuuThayDoiBtn.Size = new System.Drawing.Size(124, 23);
            this.LuuThayDoiBtn.TabIndex = 25;
            this.LuuThayDoiBtn.Text = "Lưu Thay Đổi";
            this.LuuThayDoiBtn.UseVisualStyleBackColor = true;
            // 
            // nameEN_tb
            // 
            this.nameEN_tb.Location = new System.Drawing.Point(129, 238);
            this.nameEN_tb.Name = "nameEN_tb";
            this.nameEN_tb.ReadOnly = true;
            this.nameEN_tb.Size = new System.Drawing.Size(267, 20);
            this.nameEN_tb.TabIndex = 20;
            // 
            // nameVN_tb
            // 
            this.nameVN_tb.Location = new System.Drawing.Point(129, 212);
            this.nameVN_tb.Name = "nameVN_tb";
            this.nameVN_tb.ReadOnly = true;
            this.nameVN_tb.Size = new System.Drawing.Size(267, 20);
            this.nameVN_tb.TabIndex = 19;
            // 
            // PLU_tb
            // 
            this.PLU_tb.Location = new System.Drawing.Point(133, 133);
            this.PLU_tb.Name = "PLU_tb";
            this.PLU_tb.Size = new System.Drawing.Size(267, 20);
            this.PLU_tb.TabIndex = 18;
            // 
            // barCode_tb
            // 
            this.barCode_tb.Location = new System.Drawing.Point(133, 107);
            this.barCode_tb.Name = "barCode_tb";
            this.barCode_tb.Size = new System.Drawing.Size(267, 20);
            this.barCode_tb.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 242);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Tên tiếng anh";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 216);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Tên tiếng việt";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "PLU";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Bar Code";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Sản Phẩm";
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
            this.dataGV.Size = new System.Drawing.Size(861, 681);
            this.dataGV.TabIndex = 2;
            // 
            // artNr_tb
            // 
            this.artNr_tb.Location = new System.Drawing.Point(129, 373);
            this.artNr_tb.Name = "artNr_tb";
            this.artNr_tb.Size = new System.Drawing.Size(267, 20);
            this.artNr_tb.TabIndex = 37;
            // 
            // barCodeEAN13_tb
            // 
            this.barCodeEAN13_tb.Location = new System.Drawing.Point(129, 347);
            this.barCodeEAN13_tb.Name = "barCodeEAN13_tb";
            this.barCodeEAN13_tb.Size = new System.Drawing.Size(267, 20);
            this.barCodeEAN13_tb.TabIndex = 36;
            // 
            // priceCNF_tb
            // 
            this.priceCNF_tb.Location = new System.Drawing.Point(129, 321);
            this.priceCNF_tb.Name = "priceCNF_tb";
            this.priceCNF_tb.ReadOnly = true;
            this.priceCNF_tb.Size = new System.Drawing.Size(267, 20);
            this.priceCNF_tb.TabIndex = 35;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 377);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 32;
            this.label6.Text = "Art.Nr";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 351);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Bar Code EAN13";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(28, 325);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 13);
            this.label8.TabIndex = 30;
            this.label8.Text = "Giá CNF (CHF/Kg)";
            // 
            // packing_lb
            // 
            this.packing_lb.AutoSize = true;
            this.packing_lb.Location = new System.Drawing.Point(28, 268);
            this.packing_lb.Name = "packing_lb";
            this.packing_lb.Size = new System.Drawing.Size(40, 13);
            this.packing_lb.TabIndex = 29;
            this.packing_lb.Text = "Paking";
            // 
            // GGN_tb
            // 
            this.GGN_tb.Location = new System.Drawing.Point(129, 399);
            this.GGN_tb.Name = "GGN_tb";
            this.GGN_tb.Size = new System.Drawing.Size(267, 20);
            this.GGN_tb.TabIndex = 39;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(28, 403);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 13);
            this.label10.TabIndex = 38;
            this.label10.Text = "GGN";
            // 
            // status_lb
            // 
            this.status_lb.AutoSize = true;
            this.status_lb.Location = new System.Drawing.Point(33, 459);
            this.status_lb.Name = "status_lb";
            this.status_lb.Size = new System.Drawing.Size(35, 13);
            this.status_lb.TabIndex = 40;
            this.status_lb.Text = "status";
            // 
            // id_tb
            // 
            this.id_tb.Location = new System.Drawing.Point(129, 82);
            this.id_tb.Name = "id_tb";
            this.id_tb.ReadOnly = true;
            this.id_tb.Size = new System.Drawing.Size(267, 20);
            this.id_tb.TabIndex = 42;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(28, 86);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(18, 13);
            this.label11.TabIndex = 41;
            this.label11.Text = "ID";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(32, 190);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(82, 13);
            this.label12.TabIndex = 11;
            this.label12.Text = "Botanical Name";
            // 
            // botanicalName_tb
            // 
            this.botanicalName_tb.Location = new System.Drawing.Point(133, 186);
            this.botanicalName_tb.Name = "botanicalName_tb";
            this.botanicalName_tb.ReadOnly = true;
            this.botanicalName_tb.Size = new System.Drawing.Size(267, 20);
            this.botanicalName_tb.TabIndex = 18;
            // 
            // amount_tb
            // 
            this.amount_tb.Location = new System.Drawing.Point(129, 295);
            this.amount_tb.Name = "amount_tb";
            this.amount_tb.Size = new System.Drawing.Size(267, 20);
            this.amount_tb.TabIndex = 44;
            // 
            // amount_lb
            // 
            this.amount_lb.AutoSize = true;
            this.amount_lb.Location = new System.Drawing.Point(28, 299);
            this.amount_lb.Name = "amount_lb";
            this.amount_lb.Size = new System.Drawing.Size(43, 13);
            this.amount_lb.TabIndex = 43;
            this.amount_lb.Text = "Amount";
            // 
            // packing_panel
            // 
            this.packing_panel.Location = new System.Drawing.Point(129, 264);
            this.packing_panel.Name = "packing_panel";
            this.packing_panel.Size = new System.Drawing.Size(262, 25);
            this.packing_panel.TabIndex = 45;
            // 
            // sku_cbb
            // 
            this.sku_cbb.FormattingEnabled = true;
            this.sku_cbb.Location = new System.Drawing.Point(129, 159);
            this.sku_cbb.Name = "sku_cbb";
            this.sku_cbb.Size = new System.Drawing.Size(262, 21);
            this.sku_cbb.TabIndex = 46;
            // 
            // ProductList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Name = "ProductList";
            this.Text = "FormTableData";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox nameEN_tb;
        private System.Windows.Forms.TextBox nameVN_tb;
        private System.Windows.Forms.TextBox PLU_tb;
        private System.Windows.Forms.TextBox barCode_tb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button LuuThayDoiBtn;
        private System.Windows.Forms.Button newCustomerBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label loading_lb;
        private System.Windows.Forms.Button delete_btn;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.TextBox artNr_tb;
        private System.Windows.Forms.TextBox barCodeEAN13_tb;
        private System.Windows.Forms.TextBox priceCNF_tb;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label packing_lb;
        private System.Windows.Forms.TextBox GGN_tb;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label status_lb;
        private System.Windows.Forms.TextBox id_tb;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox botanicalName_tb;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox amount_tb;
        private System.Windows.Forms.Label amount_lb;
        private System.Windows.Forms.Panel packing_panel;
        private System.Windows.Forms.ComboBox sku_cbb;
    }
}