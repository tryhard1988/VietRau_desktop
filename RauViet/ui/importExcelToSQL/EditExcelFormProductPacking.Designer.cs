namespace RauViet.ui
{
    partial class EditExcelFormProductPacking
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openFileExcel_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.save_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.importToSQL_ProductPacking_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chuanhoa_packingList_btn = new System.Windows.Forms.Button();
            this.DeleteSelectedRows_btn = new System.Windows.Forms.Button();
            this.Xoa_SP_Khong_Cần = new System.Windows.Forms.Button();
            this.ThemCotAmountVaboPhanSocuaPackingList_btn = new System.Windows.Forms.Button();
            this.khongGia_khongPLU_btn = new System.Windows.Forms.Button();
            this.checkSKU_btn = new System.Windows.Forms.Button();
            this.editpackage_btn = new System.Windows.Forms.Button();
            this.dataGV = new System.Windows.Forms.DataGridView();
            this.CountRowTotal_btn = new System.Windows.Forms.Button();
            this.changeSKU_diff_kg_btn = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileExcel_mi,
            this.save_mi,
            this.importToSQL_ProductPacking_mi});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1268, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openFileExcel_mi
            // 
            this.openFileExcel_mi.Name = "openFileExcel_mi";
            this.openFileExcel_mi.Size = new System.Drawing.Size(48, 20);
            this.openFileExcel_mi.Text = "Open";
            // 
            // save_mi
            // 
            this.save_mi.Name = "save_mi";
            this.save_mi.Size = new System.Drawing.Size(43, 20);
            this.save_mi.Text = "Save";
            // 
            // importToSQL_ProductPacking_mi
            // 
            this.importToSQL_ProductPacking_mi.Name = "importToSQL_ProductPacking_mi";
            this.importToSQL_ProductPacking_mi.Size = new System.Drawing.Size(204, 20);
            this.importToSQL_ProductPacking_mi.Text = "Import To SQL => Product Packing";
            this.importToSQL_ProductPacking_mi.Click += new System.EventHandler(this.importToSQL_ProductPacking_mi_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.changeSKU_diff_kg_btn);
            this.panel1.Controls.Add(this.CountRowTotal_btn);
            this.panel1.Controls.Add(this.chuanhoa_packingList_btn);
            this.panel1.Controls.Add(this.DeleteSelectedRows_btn);
            this.panel1.Controls.Add(this.Xoa_SP_Khong_Cần);
            this.panel1.Controls.Add(this.ThemCotAmountVaboPhanSocuaPackingList_btn);
            this.panel1.Controls.Add(this.khongGia_khongPLU_btn);
            this.panel1.Controls.Add(this.checkSKU_btn);
            this.panel1.Controls.Add(this.editpackage_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(953, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(315, 623);
            this.panel1.TabIndex = 2;
            // 
            // chuanhoa_packingList_btn
            // 
            this.chuanhoa_packingList_btn.Location = new System.Drawing.Point(153, 60);
            this.chuanhoa_packingList_btn.Name = "chuanhoa_packingList_btn";
            this.chuanhoa_packingList_btn.Size = new System.Drawing.Size(135, 47);
            this.chuanhoa_packingList_btn.TabIndex = 11;
            this.chuanhoa_packingList_btn.Text = "Chuẩn Hóa PackingList";
            this.chuanhoa_packingList_btn.UseVisualStyleBackColor = true;
            this.chuanhoa_packingList_btn.Click += new System.EventHandler(this.chuanhoa_packingList_btn_Click);
            // 
            // DeleteSelectedRows_btn
            // 
            this.DeleteSelectedRows_btn.Location = new System.Drawing.Point(6, 192);
            this.DeleteSelectedRows_btn.Name = "DeleteSelectedRows_btn";
            this.DeleteSelectedRows_btn.Size = new System.Drawing.Size(273, 47);
            this.DeleteSelectedRows_btn.TabIndex = 10;
            this.DeleteSelectedRows_btn.Text = "Xoa cột Product Name";
            this.DeleteSelectedRows_btn.UseVisualStyleBackColor = true;
            this.DeleteSelectedRows_btn.Click += new System.EventHandler(this.DeleteSelectedRows_btn_Click);
            // 
            // Xoa_SP_Khong_Cần
            // 
            this.Xoa_SP_Khong_Cần.Location = new System.Drawing.Point(6, 127);
            this.Xoa_SP_Khong_Cần.Name = "Xoa_SP_Khong_Cần";
            this.Xoa_SP_Khong_Cần.Size = new System.Drawing.Size(273, 47);
            this.Xoa_SP_Khong_Cần.TabIndex = 6;
            this.Xoa_SP_Khong_Cần.Text = "Xóa Những Sản Phẩm Không Dùng";
            this.Xoa_SP_Khong_Cần.UseVisualStyleBackColor = true;
            this.Xoa_SP_Khong_Cần.Click += new System.EventHandler(this.Xoa_SP_Khong_Cần_Click);
            // 
            // ThemCotAmountVaboPhanSocuaPackingList_btn
            // 
            this.ThemCotAmountVaboPhanSocuaPackingList_btn.Location = new System.Drawing.Point(6, 7);
            this.ThemCotAmountVaboPhanSocuaPackingList_btn.Name = "ThemCotAmountVaboPhanSocuaPackingList_btn";
            this.ThemCotAmountVaboPhanSocuaPackingList_btn.Size = new System.Drawing.Size(141, 47);
            this.ThemCotAmountVaboPhanSocuaPackingList_btn.TabIndex = 5;
            this.ThemCotAmountVaboPhanSocuaPackingList_btn.Text = "Cắt Phần Số của PakingList Đưa vào Amount";
            this.ThemCotAmountVaboPhanSocuaPackingList_btn.UseVisualStyleBackColor = true;
            this.ThemCotAmountVaboPhanSocuaPackingList_btn.Click += new System.EventHandler(this.ThemCotAmountVaboPhanSocuaPackingList_btn_Click);
            // 
            // khongGia_khongPLU_btn
            // 
            this.khongGia_khongPLU_btn.Location = new System.Drawing.Point(6, 245);
            this.khongGia_khongPLU_btn.Name = "khongGia_khongPLU_btn";
            this.khongGia_khongPLU_btn.Size = new System.Drawing.Size(273, 47);
            this.khongGia_khongPLU_btn.TabIndex = 4;
            this.khongGia_khongPLU_btn.Text = "SKU không phải kiểu int      Không có Giá, Không Có PLU => XÓA";
            this.khongGia_khongPLU_btn.UseVisualStyleBackColor = true;
            this.khongGia_khongPLU_btn.Click += new System.EventHandler(this.khongGia_khongPLU_btn_Click);
            // 
            // checkSKU_btn
            // 
            this.checkSKU_btn.Location = new System.Drawing.Point(153, 7);
            this.checkSKU_btn.Name = "checkSKU_btn";
            this.checkSKU_btn.Size = new System.Drawing.Size(135, 47);
            this.checkSKU_btn.TabIndex = 3;
            this.checkSKU_btn.Text = "Check SKU";
            this.checkSKU_btn.UseVisualStyleBackColor = true;
            this.checkSKU_btn.Click += new System.EventHandler(this.checkSKU_btn_Click);
            // 
            // editpackage_btn
            // 
            this.editpackage_btn.Location = new System.Drawing.Point(6, 60);
            this.editpackage_btn.Name = "editpackage_btn";
            this.editpackage_btn.Size = new System.Drawing.Size(141, 47);
            this.editpackage_btn.TabIndex = 2;
            this.editpackage_btn.Text = "Chuẩn Hóa Package";
            this.editpackage_btn.UseVisualStyleBackColor = true;
            // 
            // dataGV
            // 
            this.dataGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGV.Location = new System.Drawing.Point(0, 24);
            this.dataGV.Name = "dataGV";
            this.dataGV.Size = new System.Drawing.Size(953, 623);
            this.dataGV.TabIndex = 3;
            // 
            // CountRowTotal_btn
            // 
            this.CountRowTotal_btn.Location = new System.Drawing.Point(6, 318);
            this.CountRowTotal_btn.Name = "CountRowTotal_btn";
            this.CountRowTotal_btn.Size = new System.Drawing.Size(273, 47);
            this.CountRowTotal_btn.TabIndex = 12;
            this.CountRowTotal_btn.Text = "Đếm Số Dòng";
            this.CountRowTotal_btn.UseVisualStyleBackColor = true;
            this.CountRowTotal_btn.Click += new System.EventHandler(this.CountRowTotal_btn_Click);
            // 
            // changeSKU_diff_kg_btn
            // 
            this.changeSKU_diff_kg_btn.Location = new System.Drawing.Point(6, 382);
            this.changeSKU_diff_kg_btn.Name = "changeSKU_diff_kg_btn";
            this.changeSKU_diff_kg_btn.Size = new System.Drawing.Size(273, 47);
            this.changeSKU_diff_kg_btn.TabIndex = 13;
            this.changeSKU_diff_kg_btn.Text = "Đổi Tên SKU nếu không phải Kg";
            this.changeSKU_diff_kg_btn.UseVisualStyleBackColor = true;
            this.changeSKU_diff_kg_btn.Click += new System.EventHandler(this.changeSKU_diff_kg_btn_Click);
            // 
            // EditExcelFormProductPacking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1268, 647);
            this.Controls.Add(this.dataGV);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "EditExcelFormProductPacking";
            this.Text = "EditExcelForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openFileExcel_mi;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGV;
        private System.Windows.Forms.ToolStripMenuItem save_mi;
        private System.Windows.Forms.Button editpackage_btn;
        private System.Windows.Forms.Button checkSKU_btn;
        private System.Windows.Forms.Button khongGia_khongPLU_btn;
        private System.Windows.Forms.Button ThemCotAmountVaboPhanSocuaPackingList_btn;
        private System.Windows.Forms.Button Xoa_SP_Khong_Cần;
        private System.Windows.Forms.Button DeleteSelectedRows_btn;
        private System.Windows.Forms.Button chuanhoa_packingList_btn;
        private System.Windows.Forms.ToolStripMenuItem importToSQL_ProductPacking_mi;
        private System.Windows.Forms.Button CountRowTotal_btn;
        private System.Windows.Forms.Button changeSKU_diff_kg_btn;
    }
}