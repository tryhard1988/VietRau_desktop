using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RauViet.ui
{
    public partial class QL_kho : Form
    {
        public QL_kho()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            KhachHang_menuItem.Click += khachhang_btn_Click;
            productMain_menuitem.Click += Products_SKU_btn_Click;
            productPacking_meniitem.Click += productPacking_btn_Click;
            exportCode_menuItem.Click += exportCode_btn_Click;
            order_menuitem.Click += others_btn_Click;
            dsDongThung_menuitem.Click += orderpackingList_btn_Click;
            do417_menuitem.Click += orderTotal_btn_Click;
            lotCode_menuitem.Click += lotCode_btn_Click;
            dkkd_menuitem.Click += dkkd_btn_Click;
            phyto_menuitem.Click += phyto_btn_Click;
            invoice_menuitem.Click += invoice_btn_Click;
            packingTotal_menuitem.Click += packingTotal_btn_Click;
            customerDetailPacking_mi.Click += customerDetailPacking_mi_Click;

            title_lb.Text = "";
        }

        private void SwitchChildForm<T>(string title) where T : Form, new()
        {
            // Lưu form hiện tại nếu có
            if (this.content_panel.Controls.Count > 0)
            {
                if (this.content_panel.Controls[0] is ICanSave currentForm)
                    currentForm.SaveData();

                this.content_panel.Controls.Clear();
            }

            // Tạo form mới
            var form = new T();
            title_lb.Text = title;
            form.TopLevel = false;
            form.Parent = this.content_panel;
            form.Dock = DockStyle.Fill;

            // Nếu form con có method ShowData, gọi bằng reflection hoặc interface
            var showDataMethod = form.GetType().GetMethod("ShowData");
            showDataMethod?.Invoke(form, null);

            form.Show();
        }


        private void customerDetailPacking_mi_Click(object sender, EventArgs e)
        {
            SwitchChildForm<CustomerDetailPackingTotal>("Customer Detail Packing");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "Customer Detail Packing";
            //var form = new CustomerDetailPackingTotal();
            //form.ShowData();

            //form.TopLevel = false;
            //form.Parent = this.content_panel;
            //form.Show();
        }

        private void packingTotal_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<DetailPackingTotal>("Packing Total");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "Packing Total";
            //var form = new DetailPackingTotal();
            //form.ShowData();

            //form.TopLevel = false;
            //form.Parent = this.content_panel;
            //form.Show();
        }

        private void lotCode_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<LOTCode>("Nhập Mã LOT");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "Nhập Mã LOT";
            //var form = new LOTCode();
            //form.ShowData();

            //form.TopLevel = false;
            //form.Parent = this.content_panel;
            //form.Show();
        }

        private void invoice_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<INVOICE>("INVOICE");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "INVOICE";
            //var form = new INVOICE();
            //form.ShowData();

            //form.TopLevel = false;
            //form.Parent = this.content_panel;
            //form.Show();
        }

        private void phyto_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<Phyto>("PHYTO");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "PHYTO";
            //var form = new Phyto();
            //form.ShowData();

            //form.TopLevel = false;
            //form.Parent = this.content_panel;
            //form.Show();
        }

        private void dkkd_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<DangKyKiemDinh>("Đăng Ký Kiểm Dịch");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "Đăng Ký Kiểm Định";
            //var form = new DangKyKiemDinh();
            //form.ShowData();

            //form.TopLevel = false;
            //form.Parent = this.content_panel;
            //form.Show();
        }

        private void orderTotal_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<Do417>("Dò 417");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "Dò 417";
            //var form = new Do417();
            //form.ShowData();

            //form.TopLevel = false;
            //form.Parent = this.content_panel;
            //form.Show();
        }

        private void orderpackingList_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<OrderPackingList>("Danh Sách Đóng Thùng");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "Danh Sách Đóng Thùng";
            //var form = new OrderPackingList();
            //form.ShowData();

            //form.TopLevel = false;
            //form.Parent = this.content_panel;
            //form.Show();
        }

        private void exportCode_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<ExportCodes>("Danh Sách Mã Xuất Cảng");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "Danh Sách Mã Xuất Cảng";
            //var productSKU = new ExportCodes();
            //productSKU.ShowData();

            //productSKU.TopLevel = false;
            //productSKU.Parent = this.content_panel;
            //productSKU.Show();
        }

        private void Products_SKU_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<ProductSKU>("Danh Sách Sản Phẩm Chính");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "Danh Sách Sản Phẩm Chính";
            //var productSKU = new ProductSKU();
            //productSKU.ShowData();

            //productSKU.TopLevel = false;
            //productSKU.Parent = this.content_panel;
            //productSKU.Show();
        }

        private void productPacking_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<ProductList>("Danh Sách Sản Phẩm Quy Cách");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "Danh Sách Sản Phẩm Quy Cách";
            //var productList = new ProductList();
            //productList.ShowData();

            //productList.TopLevel = false;
            //productList.Parent = this.content_panel;
            //productList.Show();
        }

        private void khachhang_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<Customers>("Danh Sách Khách Hàng");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "Danh Sách Khách Hàng";

            //var customers = new Customers();
            //customers.ShowData();

            //customers.TopLevel = false;
            //customers.Parent = this.content_panel;
            //customers.Show();
        }


        private void others_btn_Click(object sender, EventArgs e)
        {
            SwitchChildForm<OrdersList>("Danh Sách Đơn Hàng");
            //this.content_panel.Controls.Clear();
            //title_lb.Text = "Danh Sách Đơn Hàng";
            //var others = new OrdersList();
            //others.ShowData();

            //others.TopLevel = false;
            //others.Parent = this.content_panel;
            //others.Show();
        }

    }
}
