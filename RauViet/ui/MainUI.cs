using RauViet.classes;
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

namespace RauViet.ui
{
    public partial class MainUI : Form
    {
        private string treeViewSelectNodeStr = "";
        public MainUI()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.IsMdiContainer = true;

            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);

        }

        private void showtonghopgiathanh()
        {
            this.panel1.Controls.Clear();

            Form f = new sub.TongHopGiaThanh();
            f.TopLevel = false;
            f.Parent = this.panel1;
            f.Show();
        }
        private sub.Customers QuanLyXuatNhapKhau_Customers(string _formName)
        {
            this.panel1.Controls.Clear();

            var f = new sub.Customers(_formName);
            f.TopLevel = false;
            f.Parent = this.panel1;
            f.Show();

            return f;
        }

        private sub.DanhSachNSXKhac QuanLyXuatNhapKhau_DanhSachNSXKhac(string _formName)
        {
            this.panel1.Controls.Clear();

            var f = new sub.DanhSachNSXKhac(_formName);
            f.TopLevel = false;
            f.Parent = this.panel1;
            f.Show();

            return f;
        }

        private sub.DanhSachXuatCang QuanLyXuatNhapKhau_DSXuatCang(string _formName)
        {
            this.panel1.Controls.Clear();

            var f = new sub.DanhSachXuatCang(_formName);
            f.TopLevel = false;
            f.Parent = this.panel1;
            f.Show();

            return f;
        }

        private sub.HoaDonXuatKhau QuanLyXuatNhapKhau_HoaDonXuatKhau(string _formName)
        {
            this.panel1.Controls.Clear();

            var f = new sub.HoaDonXuatKhau(_formName);
            f.TopLevel = false;
            f.Parent = this.panel1;
            f.Show();

            return f;
        }

        private sub.InvoiceXuatNhapKhau QuanLyXuatNhapKhau_InvoiceXuatNhapKhau(string _formName)
        {
            this.panel1.Controls.Clear();

            var f = new sub.InvoiceXuatNhapKhau(_formName);
            f.TopLevel = false;
            f.Parent = this.panel1;
            f.Show();

            return f;
        }

        private sub.LichThuHoach QuanLyXuatNhapKhau_LichThuHoach(string _formName)
        {
            this.panel1.Controls.Clear();

            var f = new sub.LichThuHoach(_formName);
            f.TopLevel = false;
            f.Parent = this.panel1;
            f.Show();

            return f;
        }

        private sub.DonDatHang QuanLyXuatNhapKhau_DonDatHang(string _formName)
        {
            this.panel1.Controls.Clear();

            var f = new sub.DonDatHang(_formName);
            f.TopLevel = false;
            f.Parent = this.panel1;
            f.Show();

            return f;
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;

            string objName = treeView1.SelectedNode.Name;
            if (treeViewSelectNodeStr.CompareTo(objName) == 0)
            {
                return;
            }

            treeViewSelectNodeStr = objName;
            
            switch (objName)
            {
                case "tonghopgiathanh":
                    showtonghopgiathanh();
                    break;
                case "LenhSanXuat":
                    sub.Customers lenhSanXuatform = QuanLyXuatNhapKhau_Customers(objName);
                    break;
                case "Customers":
                    sub.Customers customerForm = QuanLyXuatNhapKhau_Customers(objName);
                    customerForm.ShowData();
                    break;
                case "DanhSachNSXKhac":
                    sub.DanhSachNSXKhac danhSachNSXKhacForm = QuanLyXuatNhapKhau_DanhSachNSXKhac(objName);
                    danhSachNSXKhacForm.ShowData();
                    break;
                case "DSXuatCang":
                    sub.DanhSachXuatCang danhSachXuatCangForm = QuanLyXuatNhapKhau_DSXuatCang(objName);
                    danhSachXuatCangForm.ShowData();
                    break;
                case "HoaDonXuatKhau":
                    sub.HoaDonXuatKhau hoaDonXuatKhauForm = QuanLyXuatNhapKhau_HoaDonXuatKhau(objName);
                    hoaDonXuatKhauForm.ShowData();
                    break;
                case "InvoiceXuatNhapKhau":
                    sub.InvoiceXuatNhapKhau invoiceXuatNhapKhauForm = QuanLyXuatNhapKhau_InvoiceXuatNhapKhau(objName);
                    invoiceXuatNhapKhauForm.ShowData();
                    break;
                case "LichThuHoach":
                    sub.LichThuHoach lichThuHoachForm = QuanLyXuatNhapKhau_LichThuHoach(objName);
                    lichThuHoachForm.ShowData();
                    break;
                case "DonDatHang":
                    sub.DonDatHang donDatHangForm = QuanLyXuatNhapKhau_DonDatHang(objName);
                    donDatHangForm.ShowData();
                    break;

            }
        }
        /*
private void tabControl1_MouseDown(object sender, MouseEventArgs e)
{
// tabControl1.TabPages.Remove(tabControl1.SelectedTab);
}

private void tabPage_Click(object sender, EventArgs e)
{

}*/
    }
}
