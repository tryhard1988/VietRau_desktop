using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class WooCommerceUI : Form
    {
        public WooCommerceUI()
        {
            InitializeComponent();
        }

        private async  void button1_Click(object sender, EventArgs e)
        {
            WooCommerceApi api = new WooCommerceApi();
            var products = await api.GetProducts();
            dataGridView1.DataSource = products;
        }
    }
}
