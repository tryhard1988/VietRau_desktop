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
    public partial class Customers : Form
    {
        string formNameStr = "";
        public Customers(string _formName)
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

            DataTable dt = SQLManager.Instance.getCustomers();

            dataGV.DataSource = dt;

            dataGV.Columns["MaKhachHang"].Frozen = true;
            dataGV.Columns["Name"].Frozen = true;

            this.dataGV.RowPrePaint
    += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(
        this.dataGV_RowPrePaint);
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
            var cells = dataGV.Rows[e.RowIndex].Cells;
            string maKH = cells["MaKhachHang"].Value.ToString();
            string name = cells["Name"].Value.ToString();
            string address = cells["Address"].Value.ToString();
            string phone = cells["Phone"].Value.ToString();
            string email = cells["Email"].Value.ToString();
            int b2BID = int.Parse(cells["B2BID"].Value.ToString());
            string note = cells["Note"].Value.ToString();
            bool uuTien = bool.Parse(cells["UuTien"].Value.ToString());

            maKH_tb.Text = maKH;
            name_tb.Text = name;
            address_tb.Text = address;
            phone_tb.Text = phone;
            email_tb.Text = email;
            b2bId_tb.Text = b2BID.ToString();
            note_tb.Text = note;
            uuTien_cb.Checked = uuTien;
        }

        private void LuuThayDoiBtn_Click(object sender, EventArgs e)
        {
            if (maKH_tb.Text.CompareTo("") == 0)
                return;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string maKH = row.Cells["MaKhachHang"].Value.ToString();
                if(maKH.CompareTo(maKH_tb.Text) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Sure ?", " Edit Mode", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        bool isScussess = SQLManager.Instance.updateCustomer(
                        maKH,
                        name_tb.Text,
                        address_tb.Text,
                        phone_tb.Text,
                        email_tb.Text,
                       int.Parse(b2bId_tb.Text),
                        note_tb.Text,
                        uuTien_cb.Checked
                        );

                        if (isScussess == true)
                        {
                            row.Cells["Name"].Value = name_tb.Text;
                            row.Cells["Address"].Value = address_tb.Text;
                            row.Cells["Phone"].Value = phone_tb.Text;
                            row.Cells["Email"].Value = email_tb.Text;
                            row.Cells["B2BID"].Value = int.Parse(b2bId_tb.Text);
                            row.Cells["Note"].Value = note_tb.Text;
                            row.Cells["UuTien"].Value = uuTien_cb.Checked;
                        }
                    }
                    break;
                }
            }
        }

        private void saveNewBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Sure ?", " Save Mode", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                bool isScussess = SQLManager.Instance.insertCustomer(
                        maKH_tb.Text,
                        name_tb.Text,
                        address_tb.Text,
                        phone_tb.Text,
                        email_tb.Text,
                       int.Parse(b2bId_tb.Text),
                        note_tb.Text,
                        uuTien_cb.Checked
                        );

                if (isScussess == true)
                {

                    DataTable dataTable = (DataTable)dataGV.DataSource;
                    DataRow drToAdd = dataTable.NewRow();

                    drToAdd["MaKhachHang"] = maKH_tb.Text;
                    drToAdd["Name"] = name_tb.Text;
                    drToAdd["Address"] = address_tb.Text;
                    drToAdd["Phone"] = phone_tb.Text;
                    drToAdd["Email"] = email_tb.Text;
                    drToAdd["B2BID"] = int.Parse(b2bId_tb.Text);
                    drToAdd["Note"] = note_tb.Text;
                    drToAdd["UuTien"] = uuTien_cb.Checked;


                    dataTable.Rows.Add(drToAdd);
                    dataTable.AcceptChanges();
                }
                else
                {
                    MessageBox.Show("Fail");
                }
            }
        }
    }
}
