
namespace RauViet.ui
{
    partial class MainUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainUI));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Lệnh Sản Xuất", 1, 1);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Workplace", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Tổng Hợp Giá Thành", 1, 1);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Tài Khoản", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Customers");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Danh Sách NSX Khác");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Danh Sách Xuất Cảng");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Dự Báo Sản Lượng(*)");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Hóa Đơn Xuất Khẩu");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Invoice");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Lịch Thu Hoạch");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Đơn Đặt Hàng");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Quản Lý Xuất Nhập Khẩu", 0, 0, new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12});
            this.image_icon_list = new System.Windows.Forms.ImageList(this.components);
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.topMenu_tc = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.newBtn = new System.Windows.Forms.Button();
            this.topMenu_tc.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // image_icon_list
            // 
            this.image_icon_list.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("image_icon_list.ImageStream")));
            this.image_icon_list.TransparentColor = System.Drawing.Color.Transparent;
            this.image_icon_list.Images.SetKeyName(0, "folder_icon.ico");
            this.image_icon_list.Images.SetKeyName(1, "notes_icon.ico");
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(132, 313);
            // 
            // topMenu_tc
            // 
            this.topMenu_tc.Controls.Add(this.tabPage1);
            this.topMenu_tc.Dock = System.Windows.Forms.DockStyle.Top;
            this.topMenu_tc.Location = new System.Drawing.Point(0, 0);
            this.topMenu_tc.Name = "topMenu_tc";
            this.topMenu_tc.SelectedIndex = 0;
            this.topMenu_tc.Size = new System.Drawing.Size(1264, 117);
            this.topMenu_tc.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.newBtn);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1256, 91);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Home";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 117);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel1);
            this.splitContainer2.Size = new System.Drawing.Size(1264, 564);
            this.splitContainer2.SplitterDistance = 205;
            this.splitContainer2.TabIndex = 3;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.image_icon_list;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode1.ImageIndex = 1;
            treeNode1.Name = "LenhSanXuat";
            treeNode1.SelectedImageIndex = 1;
            treeNode1.Text = "Lệnh Sản Xuất";
            treeNode2.ImageIndex = 0;
            treeNode2.Name = "workplace";
            treeNode2.SelectedImageIndex = 0;
            treeNode2.Text = "Workplace";
            treeNode3.ImageIndex = 1;
            treeNode3.Name = "tonghopgiathanh";
            treeNode3.SelectedImageIndex = 1;
            treeNode3.Text = "Tổng Hợp Giá Thành";
            treeNode4.ImageIndex = 0;
            treeNode4.Name = "taikhoan";
            treeNode4.SelectedImageIndex = 0;
            treeNode4.Text = "Tài Khoản";
            treeNode5.Name = "Customers";
            treeNode5.Text = "Customers";
            treeNode6.Name = "DanhSachNSXKhac";
            treeNode6.Text = "Danh Sách NSX Khác";
            treeNode7.Name = "DSXuatCang";
            treeNode7.Text = "Danh Sách Xuất Cảng";
            treeNode8.Name = "Node3";
            treeNode8.Text = "Dự Báo Sản Lượng(*)";
            treeNode9.Name = "HoaDonXuatKhau";
            treeNode9.Text = "Hóa Đơn Xuất Khẩu";
            treeNode10.Name = "InvoiceXuatNhapKhau";
            treeNode10.Text = "Invoice";
            treeNode11.Name = "LichThuHoach";
            treeNode11.Text = "Lịch Thu Hoạch";
            treeNode12.Name = "DonDatHang";
            treeNode12.Text = "Đơn Đặt Hàng";
            treeNode13.ImageIndex = 0;
            treeNode13.Name = "Node1";
            treeNode13.SelectedImageIndex = 0;
            treeNode13.Text = "Quản Lý Xuất Nhập Khẩu";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode4,
            treeNode13});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(205, 564);
            this.treeView1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1055, 564);
            this.panel1.TabIndex = 3;
            // 
            // newBtn
            // 
            this.newBtn.BackColor = System.Drawing.Color.Transparent;
            this.newBtn.Image = global::RauViet.Properties.Resources._new;
            this.newBtn.Location = new System.Drawing.Point(8, 6);
            this.newBtn.Name = "newBtn";
            this.newBtn.Size = new System.Drawing.Size(68, 76);
            this.newBtn.TabIndex = 0;
            this.newBtn.UseVisualStyleBackColor = false;
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.topMenu_tc);
            this.Name = "MainUI";
            this.Text = "VietRau";
            this.topMenu_tc.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList image_icon_list;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.TabControl topMenu_tc;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button newBtn;
    }
}