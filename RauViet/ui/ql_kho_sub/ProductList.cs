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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class ProductList : Form
    {        public ProductList()
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
            sku_cbb.DropDownStyle = ComboBoxStyle.DropDownList;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGV_CellClick);
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            priceCNF_tb.TextChanged += priceCNF_tb_TextChanged;

        }

        public async void ShowData()
        {   
            loading_lb.Visible = true;

            try
            {
                DataTable sku_dt = await Task.Run(() => SQLManager.Instance.getProductSKU());
                sku_cbb.DataSource = sku_dt;
                sku_cbb.DisplayMember = "ProductNameVN";  // hiển thị tên
                sku_cbb.ValueMember = "SKU";
                sku_cbb.SelectedIndexChanged += sku_cbb_SelectedIndexChanged;


                // Chạy truy vấn trên thread riêng
                DataTable dt = await Task.Run(() => SQLManager.Instance.getProductpacking());


                // Tạo cột mới
                DataColumn PriceCNF = new DataColumn("PriceCNF", typeof(string));
                DataColumn colNameVN = new DataColumn("Name_VN", typeof(string));
                DataColumn colNameEN = new DataColumn("Name_EN", typeof(string));

                // Thêm cột vào DataTable
                dt.Columns.Add(PriceCNF);
                dt.Columns.Add(colNameVN);
                dt.Columns.Add(colNameEN);

                // Chuyển cột vào vị trí mong muốn
                
                colNameVN.SetOrdinal(3); // chèn vào vị trí index 3
                colNameEN.SetOrdinal(4); // chèn vào vị trí index 4
               // PriceCNF.SetOrdinal(5); // chèn vào vị trí index 3
                // Gán giá trị cho từng dòng
                foreach (DataRow dr in dt.Rows)
                {
                    int sku = Convert.ToInt32(dr["SKU"]);
                    DataRow row = sku_dt.Select($"SKU = '{sku}'")[0];

                    dr["Name_VN"] = row["ProductNameVN"].ToString() + " " + row["PackingType"].ToString() + " " + dr["Amount"] + " " + dr["Packing"].ToString();
                    dr["Name_EN"] = row["ProductNameEN"].ToString() + " " + row["PackingType"].ToString() + " " + dr["Amount"] + " " + dr["Packing"].ToString();
                    dr["PriceCNF"] = row["PriceCNF"].ToString();
                }

                dataGV.DataSource = dt;
                dataGV.Columns["ProductPackingID"].Visible = false;
                dataGV.Columns["Amount"].Visible = false;
                dataGV.Columns["Packing"].Visible = false;
                dataGV.Columns["PriceCNF"].Visible = false;
                dataGV.Columns["SKU"].Visible = false;

                dataGV.Columns["Name_VN"].HeaderText = "Tên Tiếng Việt";
                dataGV.Columns["Name_EN"].HeaderText = "Tên Tiếng Anh";
                dataGV.Columns["PriceCNF"].HeaderText = "Giá CNF (CHF/Kg)";
                dataGV.Columns["BarCodeEAN13"].HeaderText = "Bar code EAN13";
                dataGV.Columns["ArtNr"].HeaderText = "Art.Nr";


                dataGV.Columns["SKU"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["PLU"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["BarCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["BarCodeEAN13"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["ArtNr"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["GGN"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //dataGV.Columns["Packing"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dataGV.Columns["PriceCNF"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dataGV.AutoResizeColumns();


                updateDataTextBoxFlowSKU();

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

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            dataGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Dock = DockStyle.Fill;

            
        }

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }
        
        private int createID()
        {
            var existingIds = dataGV.Rows
            .Cast<DataGridViewRow>()
            .Where(r => !r.IsNewRow && r.Cells["ProductPackingID"].Value != null)
            .Select(r => Convert.ToInt32(r.Cells["ProductPackingID"].Value))
            .ToList();

            int newCustomerID = existingIds.Count > 0 ? existingIds.Max() + 1 : 1;

            return newCustomerID;
        }

        private void dataGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            updateDataTextBoxFlowSKU();

            
        }

       

        private async void updateProductPacking(int ID, int SKU, string BarCode, string PLU, string Amount, string packing, string barCodeEAN13, string artNr, string GGN)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int productPackingID = int.Parse(row.Cells["ProductPackingID"].Value.ToString());
                if (productPackingID.CompareTo(ID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Thay Đổi Thông Tin Khách Hàng", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {      
                            bool isScussess = await Task.Run(() => SQLManager.Instance.updateProductpacking(ID, SKU, BarCode, PLU, Amount, packing, barCodeEAN13, artNr, GGN));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;


                                row.Cells["SKU"].Value = SKU;
                                row.Cells["BarCode"].Value = BarCode;
                                row.Cells["PLU"].Value = PLU;
                                row.Cells["Name_VN"].Value = productSKUData["ProductNameVN"] + " " + productSKUData["PackingType"] + " " + Amount + " " + packing; ;
                                row.Cells["Name_EN"].Value = productSKUData["ProductNameEN"] + " " + productSKUData["PackingType"] + " " + Amount + " " + packing; ;
                                row.Cells["Packing"].Value = packing;
                                row.Cells["PriceCNF"].Value = productSKUData["PriceCNF"];
                                row.Cells["BarCodeEAN13"].Value = barCodeEAN13;
                                row.Cells["ArtNr"].Value = artNr;
                                row.Cells["GGN"].Value = GGN;
                                row.Cells["Amount"].Value = Amount;
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

        private async void createNewProducrpacking(int SKU, string BarCode, string PLU, string Amount, string packing, string barCodeEAN13, string artNr, string GGN)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Tạo Mới Thông Tin Khách Hàng", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    bool isScussess = await Task.Run(() => SQLManager.Instance.insertProductpacking(SKU, BarCode, PLU, Amount, packing, barCodeEAN13, artNr, GGN));
                    if (isScussess == true)
                    {
                        DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        int newID = createID();

                        drToAdd["ProductPackingID"] = newID;
                        drToAdd["SKU"] = SKU;
                        drToAdd["BarCode"] = BarCode;
                        drToAdd["PLU"] = PLU;
                        drToAdd["Name_VN"] = productSKUData["ProductNameVN"] + " " + productSKUData["PackingType"] + " " + Amount + " " + packing; ;
                        drToAdd["Name_EN"] = productSKUData["ProductNameEN"] + " " + productSKUData["PackingType"] + " " + Amount + " " + packing; ;
                        drToAdd["Packing"] = packing;
                        drToAdd["PriceCNF"] = productSKUData["PriceCNF"]; ;
                        drToAdd["BarCodeEAN13"] = barCodeEAN13;
                        drToAdd["ArtNr"] = artNr;
                        drToAdd["GGN"] = GGN;
                        drToAdd["Amount"] = Amount;


                        // SKU_tb.Text = newCustomerID.ToString();

                        id_tb.Text = newID.ToString();
                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

                        // chuyển selected vào row mới
                        dataGV.ClearSelection(); // bỏ chọn row cũ
                        int rowIndex = dataGV.Rows.Count - 1;
                        dataGV.Rows[rowIndex].Selected = true;


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
            string packingStr = "";

            foreach (Control ctrl in packing_panel.Controls)
            {
                if (ctrl is RadioButton rb && rb.Checked)
                {
                    packingStr = rb.Text;
                    break; // thoát vòng lặp ngay khi tìm thấy
                }
            }

            if (id_tb.Text.Length != 0)
                updateProductPacking(int.Parse(id_tb.Text), Convert.ToInt32(sku_cbb.SelectedValue), barCode_tb.Text, PLU_tb.Text, amount_tb.Text, packingStr, barCodeEAN13_tb.Text, artNr_tb.Text, GGN_tb.Text);
            else
                createNewProducrpacking(Convert.ToInt32(sku_cbb.SelectedValue), barCode_tb.Text, PLU_tb.Text, amount_tb.Text, packingStr, barCodeEAN13_tb.Text, artNr_tb.Text, GGN_tb.Text);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            string id = id_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string productPackingID = row.Cells["ProductPackingID"].Value.ToString();
                if (productPackingID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN KHÁCH HÀNG ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin Khách Hàng", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await Task.Run(() => SQLManager.Instance.deleteProductpacking(id));

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

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            sku_cbb.SelectedIndex = -1;
            id_tb.Text = "";
            barCode_tb.Text = "";
            PLU_tb.Text = "";
            botanicalName_tb.Text = "";
            nameVN_tb.Text = "";
            nameEN_tb.Text = "";
            status_lb.Text = "";
            amount_tb.Text = "";
            priceCNF_tb.Text = "";
            barCodeEAN13_tb.Text = "";
            artNr_tb.Text = "";
            GGN_tb.Text = "";
            delete_btn.Enabled = false;
            dataGV.ClearSelection();

            sku_cbb_SelectedIndexChanged(sender, e);
            return;            
        }

        private void sku_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sku_cbb.SelectedItem != null)
            {
                DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;

                botanicalName_tb.Text = productSKUData["BotanicalName"].ToString();
                nameVN_tb.Text = productSKUData["ProductNameVN"].ToString();// + " " + amount + " " + packing;
                nameEN_tb.Text = productSKUData["ProductNameEN"].ToString();// + " " + amount + " " + packing;
                priceCNF_tb.Text = productSKUData["PriceCNF"].ToString();
                packing_panel.Controls.Clear();
                string[] arr = productSKUData["PackingList"].ToString().Split('-');
                int top = 00;

                foreach (var item in arr)
                {
                    RadioButton rb = new RadioButton();
                    rb.Text = item;
                    rb.Name = item;
                    rb.Location = new Point(top, 5);
                    rb.AutoSize = true;
                    packing_panel.Controls.Add(rb);
                    top += 70;
                }

                if (dataGV.SelectedRows.Count > 0)
                {
                    var cells = dataGV.SelectedRows[0].Cells;                  

                    foreach (var item in arr)
                    {
                        foreach (Control ctrl in packing_panel.Controls)
                        {
                            if (ctrl is RadioButton rb)
                            {
                                var rb1 = (RadioButton)ctrl; //; ;.Checked = true;
                                if (item.Equals(rb1.Name, StringComparison.OrdinalIgnoreCase))
                                {
                                    rb.Checked = true;
                                    break;
                                }
                            }
                        }
                        
                    }
                }
            }
        }

        private void updateDataTextBoxFlowSKU()
        {
            if (dataGV.SelectedRows.Count > 0)
            {
                var cells = dataGV.SelectedRows[0].Cells;

                string ID = cells["ProductPackingID"].Value.ToString();
                string SKU = cells["SKU"].Value.ToString();
                string barcode = cells["BarCode"].Value.ToString();
                string PLU = cells["PLU"].Value.ToString();
                string nameVN = cells["Name_VN"].Value.ToString();
                string nameEN = cells["Name_EN"].Value.ToString();
                string amount = cells["Amount"].Value.ToString();
                string priceCNF = cells["PriceCNF"].Value.ToString();
                string barCodeEAN13 = cells["BarCodeEAN13"].Value.ToString();
                string artNr = cells["ArtNr"].Value.ToString();
                string GGN = cells["GGN"].Value.ToString();


                sku_cbb.SelectedValue = SKU;
                DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;

                id_tb.Text = ID;
                barCode_tb.Text = barcode;
                PLU_tb.Text = PLU;
                botanicalName_tb.Text = productSKUData["BotanicalName"].ToString();
                nameVN_tb.Text = productSKUData["ProductNameVN"].ToString();
                nameEN_tb.Text = productSKUData["ProductNameEN"].ToString();
                //    packing_tb.Text = packing;
                amount_tb.Text = amount;
                priceCNF_tb.Text = priceCNF;
                barCodeEAN13_tb.Text = barCodeEAN13;
                artNr_tb.Text = artNr;
                GGN_tb.Text = GGN;

                packing_panel.Controls.Clear();
                string[] arr = productSKUData["PackingList"].ToString().Split('-');
                int top = 00;

                foreach (var item in arr)
                {
                    RadioButton rb = new RadioButton();
                    rb.Text = item;
                    rb.Location = new Point(top, 5);
                    rb.AutoSize = true;
                    packing_panel.Controls.Add(rb);

                    if (item.Equals(cells["Packing"].Value.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        rb.Checked = true;
                    }

                    top += 70;
                }

                delete_btn.Enabled = true;
            }
        }

        private void priceCNF_tb_TextChanged(object sender, EventArgs e)
        {
           if(priceCNF_tb.Text.Length == 0 && sku_cbb.SelectedItem != null)
            {
                DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
                priceCNF_tb.Text = productSKUData["PriceCNF"].ToString();
            }
        }

    }
}
