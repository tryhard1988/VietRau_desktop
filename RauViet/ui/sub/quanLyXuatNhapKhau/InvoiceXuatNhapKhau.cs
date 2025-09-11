using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui.sub
{
    public partial class InvoiceXuatNhapKhau : Form
    {
        string formNameStr = "";
        public InvoiceXuatNhapKhau(string _formName)
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            formNameStr = _formName;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;
        }

        public void ShowData()
        {
            dataGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGV_CellClick);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            DataTable dt = SQLManager.Instance.getInvoiceXuatNhapKhau();

            dataGV.DataSource = dt;

           /* dataGV.Columns["MaNSX"].HeaderText = "Mã Nhà Sản Xuất";
            dataGV.Columns["TenNSX"].HeaderText = "Tên Nhà Sản Xuất";
            dataGV.Columns["GhiChu"].HeaderText = "Ghi Chú";*/
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            //  dataGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }

        private void dataGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
        }
    }
}
