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
            this.changeSKU_diff_kg_btn = new System.Windows.Forms.Button();
            this.edit_price_btn = new System.Windows.Forms.Button();
            this.chuanhoa_packingList_btn = new System.Windows.Forms.Button();
            this.DeleteSelectedRows_btn = new System.Windows.Forms.Button();
            this.normalizeBotanicalName_btn = new System.Windows.Forms.Button();
            this.normalizeProductNameEN_btn = new System.Windows.Forms.Button();
            this.checkDuplicateProductNameEN_btn = new System.Windows.Forms.Button();
            this.Xoa_SP_Khong_Cần = new System.Windows.Forms.Button();
            this.DeleteSKUDuplicate_btn = new System.Windows.Forms.Button();
            this.khongGia_khongPLU_btn = new System.Windows.Forms.Button();
            this.checkSKU_btn = new System.Windows.Forms.Button();
            this.editpackage_btn = new System.Windows.Forms.Button();
            this.EditProductName_btn = new System.Windows.Forms.Button();
            this.dataGV = new System.Windows.Forms.DataGridView();
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
            this.panel1.Controls.Add(this.edit_price_btn);
            this.panel1.Controls.Add(this.chuanhoa_packingList_btn);
            this.panel1.Controls.Add(this.DeleteSelectedRows_btn);
            this.panel1.Controls.Add(this.normalizeBotanicalName_btn);
            this.panel1.Controls.Add(this.normalizeProductNameEN_btn);
            this.panel1.Controls.Add(this.checkDuplicateProductNameEN_btn);
            this.panel1.Controls.Add(this.Xoa_SP_Khong_Cần);
            this.panel1.Controls.Add(this.DeleteSKUDuplicate_btn);
            this.panel1.Controls.Add(this.khongGia_khongPLU_btn);
            this.panel1.Controls.Add(this.checkSKU_btn);
            this.panel1.Controls.Add(this.editpackage_btn);
            this.panel1.Controls.Add(this.EditProductName_btn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(953, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(315, 623);
            this.panel1.TabIndex = 2;
            // 
            // changeSKU_diff_kg_btn
            // 
            this.changeSKU_diff_kg_btn.Location = new System.Drawing.Point(15, 484);
            this.changeSKU_diff_kg_btn.Name = "changeSKU_diff_kg_btn";
            this.changeSKU_diff_kg_btn.Size = new System.Drawing.Size(273, 47);
            this.changeSKU_diff_kg_btn.TabIndex = 13;
            this.changeSKU_diff_kg_btn.Text = "Đổi SKU nếu package không phải là kg";
            this.changeSKU_diff_kg_btn.UseVisualStyleBackColor = true;
            this.changeSKU_diff_kg_btn.Click += new System.EventHandler(this.changeSKU_diff_kg_btn_Click);
            // 
            // edit_price_btn
            // 
            this.edit_price_btn.Location = new System.Drawing.Point(6, 325);
            this.edit_price_btn.Name = "edit_price_btn";
            this.edit_price_btn.Size = new System.Drawing.Size(270, 47);
            this.edit_price_btn.TabIndex = 12;
            this.edit_price_btn.Text = "Chuẩn Hóa Price CHF";
            this.edit_price_btn.UseVisualStyleBackColor = true;
            this.edit_price_btn.Click += new System.EventHandler(this.edit_price_btn_Click);
            // 
            // chuanhoa_packingList_btn
            // 
            this.chuanhoa_packingList_btn.Location = new System.Drawing.Point(153, 113);
            this.chuanhoa_packingList_btn.Name = "chuanhoa_packingList_btn";
            this.chuanhoa_packingList_btn.Size = new System.Drawing.Size(135, 47);
            this.chuanhoa_packingList_btn.TabIndex = 11;
            this.chuanhoa_packingList_btn.Text = "Chuẩn Hóa PackingList";
            this.chuanhoa_packingList_btn.UseVisualStyleBackColor = true;
            this.chuanhoa_packingList_btn.Click += new System.EventHandler(this.chuanhoa_packingList_btn_Click);
            // 
            // DeleteSelectedRows_btn
            // 
            this.DeleteSelectedRows_btn.Location = new System.Drawing.Point(6, 378);
            this.DeleteSelectedRows_btn.Name = "DeleteSelectedRows_btn";
            this.DeleteSelectedRows_btn.Size = new System.Drawing.Size(273, 47);
            this.DeleteSelectedRows_btn.TabIndex = 10;
            this.DeleteSelectedRows_btn.Text = "Xoa cột Product Name";
            this.DeleteSelectedRows_btn.UseVisualStyleBackColor = true;
            this.DeleteSelectedRows_btn.Click += new System.EventHandler(this.DeleteSelectedRows_btn_Click);
            // 
            // normalizeBotanicalName_btn
            // 
            this.normalizeBotanicalName_btn.Location = new System.Drawing.Point(153, 60);
            this.normalizeBotanicalName_btn.Name = "normalizeBotanicalName_btn";
            this.normalizeBotanicalName_btn.Size = new System.Drawing.Size(135, 47);
            this.normalizeBotanicalName_btn.TabIndex = 9;
            this.normalizeBotanicalName_btn.Text = "Chuẩn Hóa Botanical Name";
            this.normalizeBotanicalName_btn.UseVisualStyleBackColor = true;
            this.normalizeBotanicalName_btn.Click += new System.EventHandler(this.normalizeBotanicalName_btn_Click);
            // 
            // normalizeProductNameEN_btn
            // 
            this.normalizeProductNameEN_btn.Location = new System.Drawing.Point(6, 275);
            this.normalizeProductNameEN_btn.Name = "normalizeProductNameEN_btn";
            this.normalizeProductNameEN_btn.Size = new System.Drawing.Size(273, 47);
            this.normalizeProductNameEN_btn.TabIndex = 8;
            this.normalizeProductNameEN_btn.Text = "Đổi Tên ProduceName EN";
            this.normalizeProductNameEN_btn.UseVisualStyleBackColor = true;
            this.normalizeProductNameEN_btn.Click += new System.EventHandler(this.normalizeProductNameEN_btn_Click);
            // 
            // checkDuplicateProductNameEN_btn
            // 
            this.checkDuplicateProductNameEN_btn.Location = new System.Drawing.Point(3, 222);
            this.checkDuplicateProductNameEN_btn.Name = "checkDuplicateProductNameEN_btn";
            this.checkDuplicateProductNameEN_btn.Size = new System.Drawing.Size(273, 47);
            this.checkDuplicateProductNameEN_btn.TabIndex = 7;
            this.checkDuplicateProductNameEN_btn.Text = "Kiểm Tra Produce EN có trùng tên không";
            this.checkDuplicateProductNameEN_btn.UseVisualStyleBackColor = true;
            this.checkDuplicateProductNameEN_btn.Click += new System.EventHandler(this.checkDuplicateProductNameEN_btn_Click);
            // 
            // Xoa_SP_Khong_Cần
            // 
            this.Xoa_SP_Khong_Cần.Location = new System.Drawing.Point(6, 169);
            this.Xoa_SP_Khong_Cần.Name = "Xoa_SP_Khong_Cần";
            this.Xoa_SP_Khong_Cần.Size = new System.Drawing.Size(273, 47);
            this.Xoa_SP_Khong_Cần.TabIndex = 6;
            this.Xoa_SP_Khong_Cần.Text = "Xóa Những Sản Phẩm Không Dùng";
            this.Xoa_SP_Khong_Cần.UseVisualStyleBackColor = true;
            this.Xoa_SP_Khong_Cần.Click += new System.EventHandler(this.Xoa_SP_Khong_Cần_Click);
            // 
            // DeleteSKUDuplicate_btn
            // 
            this.DeleteSKUDuplicate_btn.Location = new System.Drawing.Point(6, 7);
            this.DeleteSKUDuplicate_btn.Name = "DeleteSKUDuplicate_btn";
            this.DeleteSKUDuplicate_btn.Size = new System.Drawing.Size(141, 47);
            this.DeleteSKUDuplicate_btn.TabIndex = 5;
            this.DeleteSKUDuplicate_btn.Text = "Xóa SKU Trùng";
            this.DeleteSKUDuplicate_btn.UseVisualStyleBackColor = true;
            this.DeleteSKUDuplicate_btn.Click += new System.EventHandler(this.DeleteSKUDuplicate_btn_Click);
            // 
            // khongGia_khongPLU_btn
            // 
            this.khongGia_khongPLU_btn.Location = new System.Drawing.Point(6, 431);
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
            // EditProductName_btn
            // 
            this.EditProductName_btn.Location = new System.Drawing.Point(6, 113);
            this.EditProductName_btn.Name = "EditProductName_btn";
            this.EditProductName_btn.Size = new System.Drawing.Size(141, 47);
            this.EditProductName_btn.TabIndex = 0;
            this.EditProductName_btn.Text = "Chuẩn Hóa Tên Sản Phẩm";
            this.EditProductName_btn.UseVisualStyleBackColor = true;
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
        private System.Windows.Forms.Button EditProductName_btn;
        private System.Windows.Forms.Button editpackage_btn;
        private System.Windows.Forms.Button checkSKU_btn;
        private System.Windows.Forms.Button khongGia_khongPLU_btn;
        private System.Windows.Forms.Button DeleteSKUDuplicate_btn;
        private System.Windows.Forms.Button Xoa_SP_Khong_Cần;
        private System.Windows.Forms.Button checkDuplicateProductNameEN_btn;
        private System.Windows.Forms.Button normalizeProductNameEN_btn;
        private System.Windows.Forms.Button normalizeBotanicalName_btn;
        private System.Windows.Forms.Button DeleteSelectedRows_btn;
        private System.Windows.Forms.Button chuanhoa_packingList_btn;
        private System.Windows.Forms.Button edit_price_btn;
        private System.Windows.Forms.Button changeSKU_diff_kg_btn;
        private System.Windows.Forms.ToolStripMenuItem importToSQL_ProductPacking_mi;
    }
}