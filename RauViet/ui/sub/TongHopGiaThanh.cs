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
    public partial class TongHopGiaThanh : Form
    {
        public TongHopGiaThanh()
        {
            InitializeComponent();
            dataGV.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGV_CellDoubleClick);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

           /* DataTable dt = SQLManager.Instance.getNames();
            dt.Columns["FirstName"].ColumnName = "First Name";

            dataGV.DataSource = dt;
            dataGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;*/
        }

        private void dataGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataIndexNo = dataGV.Rows[e.RowIndex].Index.ToString();
            string cellValue = dataGV.Rows[e.RowIndex].Cells[1].Value.ToString();

            MessageBox.Show("The row index = " + dataIndexNo.ToString() + " and the row data in second column is: "
                + cellValue.ToString());
        }
    }
}
