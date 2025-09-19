using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using RauViet.classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class ProductSKU : Form
    {        public ProductSKU()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;
            delete_btn.Enabled = false;

            newBtn.Click += newBtn_Click;
            luuBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGV_CellClick);
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
        }

        public async void ShowData()
        {
            

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                // Chạy truy vấn trên thread riêng
                DataTable dt = await SQLManager.Instance.getProductSKUAsync();
                dataGV.DataSource = dt;

                dataGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm (VN)";
                dataGV.Columns["ProductNameEN"].HeaderText = "Tên Sản Phẩm (EN)";
                dataGV.Columns["BotanicalName"].HeaderText = "Botanical Name";
                dataGV.Columns["PriceCNF"].HeaderText = "Giá CNF (CHF/Kg)";
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                loading_lb.Visible = false; // ẩn loading
                loading_lb.Enabled = true; // enable lại button
            }

            
        }

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }
        
        private int createSKUID()
        {
            var existingIds = dataGV.Rows
            .Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow && r.Cells["SKU"].Value != null)
            .Select(r => Convert.ToInt32(r.Cells["SKU"].Value))
            .ToList();

            int newSKU_ID = existingIds.Count > 0 ? existingIds.Max() + 1 : 1;

            return newSKU_ID;
        }

        private void dataGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            

            var cells = dataGV.Rows[e.RowIndex].Cells;

            string SKU = cells["SKU"].Value.ToString();
            string productNameVN = cells["ProductNameVN"].Value.ToString();
            string productNameEN = cells["ProductNameEN"].Value.ToString();            
            string packingType = cells["PackingType"].Value.ToString();
            string package = cells["Package"].Value.ToString();
            string packingList = cells["PackingList"].Value.ToString();
            string botanicalName = cells["BotanicalName"].Value.ToString();
            string PriceCNF = cells["PriceCNF"].Value.ToString();

            sku_tb.Text = SKU;
            product_VN_tb.Text = productNameVN;
            product_EN_tb.Text = productNameEN;
            package_tb.Text = package;
            packingType_tb.Text = packingType;
            packing_tb.Text = packingList;
            botanicalName_tb.Text = botanicalName;
            priceCNF_tb.Text = PriceCNF;
            delete_btn.Enabled = true;
        }

       

        private async void updateProductSKU(int SKU, string productNameVN, string productNameEN, string packingType, string package, string packingList, string botanicalName, decimal priceCNF)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int _SKU =Convert.ToInt32(row.Cells["SKU"].Value);
                if (_SKU.CompareTo(SKU) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Thay Đổi Thông Tin Khách Hàng", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateProductSKUAsync(SKU, productNameVN, productNameEN, packingType, package, packingList, botanicalName, priceCNF);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["ProductNameVN"].Value = productNameVN;
                                row.Cells["ProductNameEN"].Value = productNameEN;
                                row.Cells["PackingType"].Value = packingType;
                                row.Cells["Package"].Value = package;
                                row.Cells["PackingList"].Value = packingList;
                                row.Cells["BotanicalName"].Value = botanicalName;
                                row.Cells["PriceCNF"].Value = priceCNF;
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }

                        

                        
                    }
                    break;
                }
            }
        }

        private async void createNewProductSKU(string productNameVN, string productNameEN, string packingType, string package, string packingList, string botanicalName, decimal priceCNF)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Tạo Mới Thông Tin Khách Hàng", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    bool isScussess = await SQLManager.Instance.insertProductSKUAsync(productNameVN, productNameEN, packingType, package, packingList, botanicalName, priceCNF);
                    if (isScussess == true)
                    {

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        int newCustomerID = createSKUID();
                        drToAdd["SKU"] = newCustomerID;
                        drToAdd["ProductNameVN"] = productNameVN;
                        drToAdd["ProductNameEN"] = productNameEN;
                        drToAdd["PackingType"] = packingType;
                        drToAdd["Package"] = package;
                        drToAdd["PackingList"] = packingList;
                        drToAdd["BotanicalName"] = botanicalName;
                        drToAdd["PriceCNF"] = priceCNF;
                        sku_tb.Text = newCustomerID.ToString();


                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
                
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            String packingType = packing_tb.Text;
            packingType = packingType.Replace(" ", "");
            if (packingType.EndsWith("-"))
            {
                packingType = packingType.Substring(0, packingType.Length - 1);
            }

            if (sku_tb.Text.Length != 0)
                updateProductSKU(Convert.ToInt32(sku_tb.Text), product_VN_tb.Text, product_EN_tb.Text, packingType_tb.Text, package_tb.Text, packingType, botanicalName_tb.Text,Convert.ToDecimal(priceCNF_tb.Text));
            else
                createNewProductSKU(product_VN_tb.Text, product_EN_tb.Text, packingType_tb.Text, package_tb.Text, packingType, botanicalName_tb.Text, Convert.ToDecimal(priceCNF_tb.Text));

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            string SKU = sku_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string maKH = row.Cells["SKU"].Value.ToString();
                if (maKH.CompareTo(SKU) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN KHÁCH HÀNG ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin Khách Hàng", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteProductSKUAsync(Convert.ToInt32(SKU));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                dataGV.Rows.Remove(row);
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }




                    }
                    break;
                }
            }




            delete_btn.Enabled = false;
        }

        private void newBtn_Click(object sender, EventArgs e)
        {
            sku_tb.Text = "";
            product_VN_tb.Text = "";
            product_EN_tb.Text = "";
            package_tb.Text = "";
            botanicalName_tb.Text = "";
            priceCNF_tb.Text = "";
            status_lb.Text = "";
            delete_btn.Enabled = false;
            return;            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
