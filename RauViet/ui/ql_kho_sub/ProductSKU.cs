using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
            dataGV.SelectionChanged += this.dataGV_CellClick;
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            priceCNF_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            priority_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            lotCodeHeader_tb.KeyPress += Tb_KeyPress_OnlyNumber;


            sku_tb.Visible = false;
            label1.Visible = false;
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
                

                int count = 0;
                dt.Columns["SKU"].SetOrdinal(count++);
                dt.Columns["ProductNameVN"].SetOrdinal(count++);
                dt.Columns["ProductNameEN"].SetOrdinal(count++);
                dt.Columns["PackingType"].SetOrdinal(count++);
                dt.Columns["Package"].SetOrdinal(count++);
                dt.Columns["PackingList"].SetOrdinal(count++);
                dt.Columns["BotanicalName"].SetOrdinal(count++);
                dt.Columns["PriceCNF"].SetOrdinal(count++);
                dt.Columns["PlantingAreaCode"].SetOrdinal(count++);
                dt.Columns["LOTCodeHeader"].SetOrdinal(count++);
                dt.Columns["Priority"].SetOrdinal(count++);                
                dataGV.DataSource = dt;


                dataGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm (VN)";
                dataGV.Columns["ProductNameEN"].HeaderText = "Tên Sản Phẩm (EN)";
                dataGV.Columns["BotanicalName"].HeaderText = "Botanical Name";
                dataGV.Columns["PriceCNF"].HeaderText = "Giá CNF\n(CHF/Kg)";
                dataGV.Columns["Priority"].HeaderText = "Ưu\nTiên";
                dataGV.Columns["PackingList"].HeaderText = "Packing\nList";
                dataGV.Columns["PackingType"].HeaderText = "Packing\nType";
                dataGV.Columns["PlantingAreaCode"].HeaderText = "Mã\nVùng Trồng";
                dataGV.Columns["LOTCodeHeader"].HeaderText = "Mã LOT\n3 số đầu";

                dataGV.Columns["ProductNameVN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGV.Columns["ProductNameEN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGV.Columns["BotanicalName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;   
                dataGV.Columns["PackingList"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["PlantingAreaCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dataGV.Columns["PriceCNF"].Width = 60;
                dataGV.Columns["Priority"].Width = 35;
                dataGV.Columns["package"].Width = 60;
                dataGV.Columns["PackingType"].Width = 60;
                dataGV.Columns["SKU"].Width = 40;

                dataGV.Columns["SKU"].Visible = false;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["PriceCNF"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["package"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["PackingType"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["PackingList"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["SKU"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //dataGV.AutoResizeColumns();

                

                if (dataGV.SelectedRows.Count > 0)
                    UpdateRightUI(0);
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

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) 
                return;

            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateRightUI(rowIndex);
        }

        private void UpdateRightUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;

            string SKU = cells["SKU"].Value.ToString();
            string productNameVN = cells["ProductNameVN"].Value.ToString();
            string productNameEN = cells["ProductNameEN"].Value.ToString();
            string packingType = cells["PackingType"].Value.ToString();
            string package = cells["Package"].Value.ToString();
            string packingList = cells["PackingList"].Value.ToString();
            string botanicalName = cells["BotanicalName"].Value.ToString();
            string PriceCNF = cells["PriceCNF"].Value.ToString();
            string priority = cells["Priority"].Value.ToString();
            string plantingAreaCode = cells["PlantingAreaCode"].Value.ToString();
            string lotCodeHeader = cells["LOTCodeHeader"].Value.ToString();

            sku_tb.Text = SKU;
            product_VN_tb.Text = productNameVN;
            product_EN_tb.Text = productNameEN;
            package_tb.Text = package;
            packingType_tb.Text = packingType;
            packing_tb.Text = packingList;
            botanicalName_tb.Text = botanicalName;
            priceCNF_tb.Text = PriceCNF;
            priority_tb.Text = priority;
            plantingareaCode_tb.Text = plantingAreaCode;
            lotCodeHeader_tb.Text = lotCodeHeader;
            delete_btn.Enabled = true;

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }


       

        private async void updateProductSKU(int SKU, string productNameVN, string productNameEN, string packingType, string package, 
            string packingList, string botanicalName, decimal priceCNF, int priority, string plantingareaCode, string LOTCodeHeader)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int _SKU =Convert.ToInt32(row.Cells["SKU"].Value);
                if (_SKU.CompareTo(SKU) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateProductSKUAsync(SKU, productNameVN, productNameEN, packingType, package, 
                                packingList, botanicalName, priceCNF, priority, plantingareaCode, LOTCodeHeader);

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
                                row.Cells["Priority"].Value = priority;
                                row.Cells["PlantingAreaCode"].Value = plantingareaCode;
                                row.Cells["LOTCodeHeader"].Value = LOTCodeHeader;
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

        private async void createNewProductSKU(string productNameVN, string productNameEN, string packingType, string package, 
            string packingList, string botanicalName, decimal priceCNF, int priority, string plantingareaCode, string LOTCodeHeader)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    bool isScussess = await SQLManager.Instance.insertProductSKUAsync(productNameVN, productNameEN, packingType, package, 
                        packingList, botanicalName, priceCNF, priority, plantingareaCode, LOTCodeHeader);
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
                        drToAdd["Priority"] = priority;
                        drToAdd["PlantingAreaCode"] = plantingareaCode;
                        drToAdd["LOTCodeHeader"] = LOTCodeHeader;
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
            if(package_tb.Text.CompareTo("") == 0 ||
                product_EN_tb.Text.CompareTo("") == 0 || product_VN_tb.Text.CompareTo("") == 0 || 
                priceCNF_tb.Text.CompareTo("") == 0 || priority_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }
                
            String packingList = packing_tb.Text;
            packingList = packingList.Replace(" ", "").ToLower();
            if (packingList.EndsWith("-"))
            {
                packingList = packingList.Substring(0, packingList.Length - 1);
            }
            string productNameVN = product_VN_tb.Text;
            string productNameEN = product_VN_tb.Text;
            string packingType = packingType_tb.Text;
            string plantingareaCode = plantingareaCode_tb.Text;
            string lotCode = lotCodeHeader_tb.Text;
            string package = package_tb.Text.Replace(" ", "").ToLower();

            if (sku_tb.Text.Length != 0)
                updateProductSKU(Convert.ToInt32(sku_tb.Text), productNameVN, productNameEN, packingType, package, 
                    packingList, botanicalName_tb.Text,Convert.ToDecimal(priceCNF_tb.Text),Convert.ToInt32(priority_tb.Text), plantingareaCode, lotCode);
            else
                createNewProductSKU(productNameVN, productNameEN, packingType, package, 
                    packingList, botanicalName_tb.Text, Convert.ToDecimal(priceCNF_tb.Text), Convert.ToInt32(priority_tb.Text), plantingareaCode, lotCode);
            info_gb.BackColor = Color.Gray;

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

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
            plantingareaCode_tb.Text = "";
            delete_btn.Enabled = false;
            info_gb.BackColor = Color.Green;
            return;            
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            // Chỉ cho nhập số, dấu chấm hoặc ký tự điều khiển
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Nếu đã có 1 dấu chấm, không cho nhập thêm
            if (e.KeyChar == '.' && tb.Text.Contains("."))
            {
                e.Handled = true;
            }
        }
    }
}
