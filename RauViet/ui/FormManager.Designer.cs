namespace RauViet.ui
{
    partial class FormManager
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
            this.title_lb = new System.Windows.Forms.Label();
            this.LOT_menuitem = new System.Windows.Forms.MenuStrip();
            this.nhansu_group_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.employee_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.department_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.position_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.KhachHang_menuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sanpham_group_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.productMain_menuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.productPacking_meniitem = new System.Windows.Forms.ToolStripMenuItem();
            this.donhang_group_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCode_menuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.order_menuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.dsDongThung_menuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.do417_menuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.lotCode_menuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.xuatExcelGuiKH_Group_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.dkkd_menuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.phyto_menuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.invoice_menuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.packingTotal_menuitem = new System.Windows.Forms.ToolStripMenuItem();
            this.customerDetailPacking_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.content_panel = new System.Windows.Forms.Panel();
            this.NVLogin_lb = new System.Windows.Forms.Label();
            this.user_mi = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.LOT_menuitem.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.NVLogin_lb);
            this.panel1.Controls.Add(this.title_lb);
            this.panel1.Controls.Add(this.LOT_menuitem);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1388, 56);
            this.panel1.TabIndex = 0;
            // 
            // title_lb
            // 
            this.title_lb.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.title_lb.AutoSize = true;
            this.title_lb.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title_lb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.title_lb.Location = new System.Drawing.Point(595, 30);
            this.title_lb.Name = "title_lb";
            this.title_lb.Size = new System.Drawing.Size(68, 23);
            this.title_lb.TabIndex = 19;
            this.title_lb.Text = "label1";
            // 
            // LOT_menuitem
            // 
            this.LOT_menuitem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LOT_menuitem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.user_mi,
            this.nhansu_group_mi,
            this.KhachHang_menuItem,
            this.sanpham_group_mi,
            this.donhang_group_mi,
            this.xuatExcelGuiKH_Group_mi});
            this.LOT_menuitem.Location = new System.Drawing.Point(0, 0);
            this.LOT_menuitem.Name = "LOT_menuitem";
            this.LOT_menuitem.Size = new System.Drawing.Size(1388, 25);
            this.LOT_menuitem.TabIndex = 18;
            this.LOT_menuitem.Text = "topMenu_ms";
            // 
            // nhansu_group_mi
            // 
            this.nhansu_group_mi.Checked = true;
            this.nhansu_group_mi.CheckState = System.Windows.Forms.CheckState.Checked;
            this.nhansu_group_mi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.employee_mi,
            this.department_mi,
            this.position_mi});
            this.nhansu_group_mi.Name = "nhansu_group_mi";
            this.nhansu_group_mi.Size = new System.Drawing.Size(70, 21);
            this.nhansu_group_mi.Text = "Nhân Sự";
            // 
            // employee_mi
            // 
            this.employee_mi.Name = "employee_mi";
            this.employee_mi.Size = new System.Drawing.Size(201, 22);
            this.employee_mi.Text = "Danh Sách Nhân Viên";
            // 
            // department_mi
            // 
            this.department_mi.Name = "department_mi";
            this.department_mi.Size = new System.Drawing.Size(201, 22);
            this.department_mi.Text = "Phòng Ban";
            // 
            // position_mi
            // 
            this.position_mi.Name = "position_mi";
            this.position_mi.Size = new System.Drawing.Size(201, 22);
            this.position_mi.Text = "Chức Vụ";
            // 
            // KhachHang_menuItem
            // 
            this.KhachHang_menuItem.Name = "KhachHang_menuItem";
            this.KhachHang_menuItem.Size = new System.Drawing.Size(90, 21);
            this.KhachHang_menuItem.Text = "Khách Hàng";
            // 
            // sanpham_group_mi
            // 
            this.sanpham_group_mi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.productMain_menuitem,
            this.productPacking_meniitem});
            this.sanpham_group_mi.Name = "sanpham_group_mi";
            this.sanpham_group_mi.Size = new System.Drawing.Size(142, 21);
            this.sanpham_group_mi.Text = "Danh Sách Sản Phẩm";
            // 
            // productMain_menuitem
            // 
            this.productMain_menuitem.Name = "productMain_menuitem";
            this.productMain_menuitem.Size = new System.Drawing.Size(192, 22);
            this.productMain_menuitem.Text = "Sản Phẩm Chính";
            // 
            // productPacking_meniitem
            // 
            this.productPacking_meniitem.Name = "productPacking_meniitem";
            this.productPacking_meniitem.Size = new System.Drawing.Size(192, 22);
            this.productPacking_meniitem.Text = "Sản Phẩm Quy Cách";
            // 
            // donhang_group_mi
            // 
            this.donhang_group_mi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportCode_menuItem,
            this.order_menuitem,
            this.dsDongThung_menuitem,
            this.do417_menuitem,
            this.lotCode_menuitem});
            this.donhang_group_mi.Name = "donhang_group_mi";
            this.donhang_group_mi.Size = new System.Drawing.Size(79, 21);
            this.donhang_group_mi.Text = "Đơn Hàng";
            // 
            // exportCode_menuItem
            // 
            this.exportCode_menuItem.Name = "exportCode_menuItem";
            this.exportCode_menuItem.Size = new System.Drawing.Size(180, 22);
            this.exportCode_menuItem.Text = "Mã Xuất Cảng";
            // 
            // order_menuitem
            // 
            this.order_menuitem.Name = "order_menuitem";
            this.order_menuitem.Size = new System.Drawing.Size(180, 22);
            this.order_menuitem.Text = "Đơn hàng";
            // 
            // dsDongThung_menuitem
            // 
            this.dsDongThung_menuitem.Name = "dsDongThung_menuitem";
            this.dsDongThung_menuitem.Size = new System.Drawing.Size(180, 22);
            this.dsDongThung_menuitem.Text = "DS Đóng Thùng";
            // 
            // do417_menuitem
            // 
            this.do417_menuitem.Name = "do417_menuitem";
            this.do417_menuitem.Size = new System.Drawing.Size(180, 22);
            this.do417_menuitem.Text = "Dò 417";
            // 
            // lotCode_menuitem
            // 
            this.lotCode_menuitem.Name = "lotCode_menuitem";
            this.lotCode_menuitem.Size = new System.Drawing.Size(180, 22);
            this.lotCode_menuitem.Text = "Nhập Mã LOT";
            // 
            // xuatExcelGuiKH_Group_mi
            // 
            this.xuatExcelGuiKH_Group_mi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dkkd_menuitem,
            this.phyto_menuitem,
            this.invoice_menuitem,
            this.packingTotal_menuitem,
            this.customerDetailPacking_mi});
            this.xuatExcelGuiKH_Group_mi.Name = "xuatExcelGuiKH_Group_mi";
            this.xuatExcelGuiKH_Group_mi.Size = new System.Drawing.Size(79, 21);
            this.xuatExcelGuiKH_Group_mi.Text = "Xuất Excel";
            // 
            // dkkd_menuitem
            // 
            this.dkkd_menuitem.Name = "dkkd_menuitem";
            this.dkkd_menuitem.Size = new System.Drawing.Size(217, 22);
            this.dkkd_menuitem.Text = "Đăng Ký Kiểm Dịch";
            // 
            // phyto_menuitem
            // 
            this.phyto_menuitem.Name = "phyto_menuitem";
            this.phyto_menuitem.Size = new System.Drawing.Size(217, 22);
            this.phyto_menuitem.Text = "PHYTO";
            // 
            // invoice_menuitem
            // 
            this.invoice_menuitem.Name = "invoice_menuitem";
            this.invoice_menuitem.Size = new System.Drawing.Size(217, 22);
            this.invoice_menuitem.Text = "INVOICE";
            // 
            // packingTotal_menuitem
            // 
            this.packingTotal_menuitem.Name = "packingTotal_menuitem";
            this.packingTotal_menuitem.Size = new System.Drawing.Size(217, 22);
            this.packingTotal_menuitem.Text = "Packing Total";
            // 
            // customerDetailPacking_mi
            // 
            this.customerDetailPacking_mi.Name = "customerDetailPacking_mi";
            this.customerDetailPacking_mi.Size = new System.Drawing.Size(217, 22);
            this.customerDetailPacking_mi.Text = "Customer Detail Packing";
            // 
            // content_panel
            // 
            this.content_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content_panel.Location = new System.Drawing.Point(0, 56);
            this.content_panel.Name = "content_panel";
            this.content_panel.Size = new System.Drawing.Size(1388, 491);
            this.content_panel.TabIndex = 1;
            // 
            // NVLogin_lb
            // 
            this.NVLogin_lb.AutoSize = true;
            this.NVLogin_lb.Dock = System.Windows.Forms.DockStyle.Right;
            this.NVLogin_lb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NVLogin_lb.ForeColor = System.Drawing.Color.Blue;
            this.NVLogin_lb.Location = new System.Drawing.Point(1347, 25);
            this.NVLogin_lb.Name = "NVLogin_lb";
            this.NVLogin_lb.Size = new System.Drawing.Size(41, 16);
            this.NVLogin_lb.TabIndex = 20;
            this.NVLogin_lb.Text = "label1";
            // 
            // user_mi
            // 
            this.user_mi.Name = "user_mi";
            this.user_mi.Size = new System.Drawing.Size(127, 21);
            this.user_mi.Text = "Quản Lý Tài Khoản";
            // 
            // FormManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1388, 547);
            this.Controls.Add(this.content_panel);
            this.Controls.Add(this.panel1);
            this.MainMenuStrip = this.LOT_menuitem;
            this.Name = "FormManager";
            this.Text = "Việt Rau - Quản Lý";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.LOT_menuitem.ResumeLayout(false);
            this.LOT_menuitem.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel content_panel;
        private System.Windows.Forms.MenuStrip LOT_menuitem;
        private System.Windows.Forms.ToolStripMenuItem KhachHang_menuItem;
        private System.Windows.Forms.ToolStripMenuItem sanpham_group_mi;
        private System.Windows.Forms.ToolStripMenuItem productMain_menuitem;
        private System.Windows.Forms.ToolStripMenuItem productPacking_meniitem;
        private System.Windows.Forms.ToolStripMenuItem donhang_group_mi;
        private System.Windows.Forms.ToolStripMenuItem exportCode_menuItem;
        private System.Windows.Forms.ToolStripMenuItem order_menuitem;
        private System.Windows.Forms.ToolStripMenuItem dsDongThung_menuitem;
        private System.Windows.Forms.ToolStripMenuItem do417_menuitem;
        private System.Windows.Forms.ToolStripMenuItem xuatExcelGuiKH_Group_mi;
        private System.Windows.Forms.ToolStripMenuItem dkkd_menuitem;
        private System.Windows.Forms.ToolStripMenuItem phyto_menuitem;
        private System.Windows.Forms.ToolStripMenuItem invoice_menuitem;
        private System.Windows.Forms.ToolStripMenuItem packingTotal_menuitem;
        private System.Windows.Forms.ToolStripMenuItem lotCode_menuitem;
        private System.Windows.Forms.Label title_lb;
        private System.Windows.Forms.ToolStripMenuItem customerDetailPacking_mi;
        private System.Windows.Forms.ToolStripMenuItem nhansu_group_mi;
        private System.Windows.Forms.ToolStripMenuItem employee_mi;
        private System.Windows.Forms.ToolStripMenuItem department_mi;
        private System.Windows.Forms.ToolStripMenuItem position_mi;
        private System.Windows.Forms.Label NVLogin_lb;
        private System.Windows.Forms.ToolStripMenuItem user_mi;
    }
}