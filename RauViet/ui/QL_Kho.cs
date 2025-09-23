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

            khachhang_btn.Click += khachhang_btn_Click;
            products_SKU_btn.Click += Products_SKU_btn_Click;
            productpacking_btn.Click += productPacking_btn_Click;
            others_btn.Click += others_btn_Click;
            exportCode_btn.Click += exportCode_btn_Click;
            orderpackingList_btn.Click += orderpackingList_btn_Click;
            orderTotal_btn.Click += orderTotal_btn_Click;
            dkkd_btn.Click += dkkd_btn_Click;
            phyto_btn.Click += phyto_btn_Click;
            invoice_btn.Click += invoice_btn_Click;
        }

        private void invoice_btn_Click(object sender, EventArgs e)
        {
            this.content_panel.Controls.Clear();

            var form = new INVOICE();
            form.ShowData();

            form.TopLevel = false;
            form.Parent = this.content_panel;
            form.Show();
        }

        private void phyto_btn_Click(object sender, EventArgs e)
        {
            this.content_panel.Controls.Clear();

            var form = new Phyto();
            form.ShowData();

            form.TopLevel = false;
            form.Parent = this.content_panel;
            form.Show();
        }

        private void dkkd_btn_Click(object sender, EventArgs e)
        {
            this.content_panel.Controls.Clear();

            var form = new DangKyKiemDinh();
            form.ShowData();

            form.TopLevel = false;
            form.Parent = this.content_panel;
            form.Show();
        }

        private void orderTotal_btn_Click(object sender, EventArgs e)
        {
            this.content_panel.Controls.Clear();

            var form = new OrdersTotal();
            form.ShowData();

            form.TopLevel = false;
            form.Parent = this.content_panel;
            form.Show();
        }

        private void orderpackingList_btn_Click(object sender, EventArgs e)
        {
            this.content_panel.Controls.Clear();

            var form = new OrderPackingList();
            form.ShowData();

            form.TopLevel = false;
            form.Parent = this.content_panel;
            form.Show();
        }

        private void exportCode_btn_Click(object sender, EventArgs e)
        {
            this.content_panel.Controls.Clear();

            var productSKU = new ExportCodes();
            productSKU.ShowData();

            productSKU.TopLevel = false;
            productSKU.Parent = this.content_panel;
            productSKU.Show();
        }

        private void Products_SKU_btn_Click(object sender, EventArgs e)
        {
            this.content_panel.Controls.Clear();

            var productSKU = new ProductSKU();
            productSKU.ShowData();

            productSKU.TopLevel = false;
            productSKU.Parent = this.content_panel;
            productSKU.Show();
        }

        private void productPacking_btn_Click(object sender, EventArgs e)
        {
            this.content_panel.Controls.Clear();

            var productList = new ProductList();
            productList.ShowData();

            productList.TopLevel = false;
            productList.Parent = this.content_panel;
            productList.Show();
        }

        private void khachhang_btn_Click(object sender, EventArgs e)
        {
            this.content_panel.Controls.Clear();

            var customers = new Customers();
            customers.ShowData();

            customers.TopLevel = false;
            customers.Parent = this.content_panel;
            customers.Show();
        }


        private void others_btn_Click(object sender, EventArgs e)
        {
            this.content_panel.Controls.Clear();

            var others = new OrdersList();
            others.ShowData();

            others.TopLevel = false;
            others.Parent = this.content_panel;
            others.Show();
        }
    }
}
