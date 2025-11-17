using Microsoft.VisualBasic.Devices;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class ProductList : Form
    {
        DataTable mSKU_dt, packing_dt;
        private Timer debounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public ProductList()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            priceCNF_tb.TextChanged += priceCNF_tb_TextChanged;
            search_tb.TextChanged += search_txt_TextChanged;
            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            debounceTimer.Tick += DebounceTimer_Tick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            this.KeyDown += ProductList_KeyDown;

        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore.Instance.removeProductpacking();
                SQLStore.Instance.removeProductSKU();
                ShowData();
            }
            else if (!isNewState && !edit_btn.Visible)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    Control ctrl = this.ActiveControl;

                    if (ctrl is TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    deletePackingProduct();
                }
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                var parameters = new Dictionary<string, object> { { "IsActive", true } };
                mSKU_dt = await SQLStore.Instance.getProductSKUAsync(parameters);
                 packing_dt = await SQLStore.Instance.getProductpackingAsync();

                foreach (DataColumn col in packing_dt.Columns)
                    col.ReadOnly = false;

                sku_cbb.DataSource = mSKU_dt;
                sku_cbb.DisplayMember = "ProductNameVN";  // hiển thị tên
                sku_cbb.ValueMember = "SKU";
                sku_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
               // sku_cbb.AutoCompleteSource = AutoCompleteSource.ListItems;
                sku_cbb.SelectedIndexChanged += sku_cbb_SelectedIndexChanged;
                sku_cbb.TextUpdate += sku_cbb_TextUpdate;

                packing_dt.Columns["Name_VN"].SetOrdinal(3); 
                packing_dt.Columns["Name_EN"].SetOrdinal(4);
                packing_dt.Columns["Amount"].SetOrdinal(5);
                

                dataGV.DataSource = packing_dt;
                DataView dv = packing_dt.DefaultView;
                dv.RowFilter = $"IsActive_SKU = true";

                dataGV.Columns["ProductPackingID"].Visible = false;
                dataGV.Columns["IsActive_SKU"].Visible = false;
                // dataGV.Columns["Amount"].Visible = false;
                // dataGV.Columns["Packing"].Visible = false;
                dataGV.Columns["PriceCNF"].Visible = false;
                dataGV.Columns["SKU"].Visible = false; 
                dataGV.Columns["ProductNameVN_NoSign"].Visible = false;
                dataGV.Columns["PackingName"].Visible = false;
                dataGV.Columns["Package"].Visible = false;
                dataGV.Columns["Amount"].Visible = false;
                dataGV.Columns["Packing"].Visible = false;

                dataGV.Columns["Name_VN"].HeaderText = "Tên Tiếng Việt";
                dataGV.Columns["Name_EN"].HeaderText = "Tên Tiếng Anh";
                dataGV.Columns["PriceCNF"].HeaderText = "Giá CNF (CHF/Kg)";
                dataGV.Columns["BarCodeEAN13"].HeaderText = "Bar code EAN13";
                dataGV.Columns["ArtNr"].HeaderText = "Art.Nr";
                dataGV.Columns["Priority"].HeaderText = "Ưu Tiên";
                dataGV.Columns["IsActive"].HeaderText = "";


                dataGV.Columns["PLU"].Width = 50;
                dataGV.Columns["BarCode"].Width = 90;
                dataGV.Columns["BarCodeEAN13"].Width = 90;
                dataGV.Columns["ArtNr"].Width = 50;
                dataGV.Columns["GGN"].Width = 90;
                dataGV.Columns["Name_VN"].Width = 200;
                dataGV.Columns["Name_EN"].Width = 200;
                dataGV.Columns["Priority"].Width = 50;
                dataGV.Columns["IsActive"].Width = 30;


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.Columns["Packing"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                updateDataTextBoxFlowSKU();

                Utils.SetTabStopRecursive(this, false);

                int countTab = 0;
                barCode_tb.TabIndex = countTab++; barCode_tb.TabStop = true;
                PLU_tb.TabIndex = countTab++; PLU_tb.TabStop = true;
                sku_cbb.TabIndex = countTab++; sku_cbb.TabStop = true;
                packing_panel.TabIndex = countTab++; packing_panel.TabStop = true;
                foreach (Control ctrl in packing_panel.Controls)
                {
                    if (ctrl is RadioButton rb && rb.Checked)
                    {
                        rb.TabIndex = countTab++; rb.TabStop = true;
                    }
                }
                amount_tb.TabIndex = countTab++; amount_tb.TabStop = true;
                barCodeEAN13_tb.TabIndex = countTab++; barCodeEAN13_tb.TabStop = true;
                artNr_tb.TabIndex = countTab++; artNr_tb.TabStop = true;
                GGN_tb.TabIndex = countTab++; GGN_tb.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

                ReadOnly_btn_Click(null, null);
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
 
        }

        
        private void sku_cbb_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                debounceTimer.Stop();

                string typed = sku_cbb.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mSKU_dt.AsEnumerable()
                    .Where(r => r["ProductNameVN_NoSign"].ToString()
                    .Contains(plain));

                DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mSKU_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                sku_cbb.DataSource = temp;
                sku_cbb.DisplayMember = "ProductNameVN";
                sku_cbb.ValueMember = "SKU";

                // Giữ lại text người đang gõ
                sku_cbb.DroppedDown = true;
                sku_cbb.Text = typed;
                sku_cbb.SelectionStart = typed.Length;
                sku_cbb.SelectionLength = 0;
            }
            catch { }
        }
                
        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateDataTextBoxFlowSKU();            
        }

        private async void updateProductPacking(int ID, int SKU, string BarCode, string PLU, int? Amount, string packing, string barCodeEAN13, string artNr, string GGN, bool isActive)
        {
            foreach (DataRow row in packing_dt.Rows)
            {
                int productPackingID = Convert.ToInt32(row["ProductPackingID"]);
                if (productPackingID.CompareTo(ID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {      
                            bool isScussess = await SQLManager.Instance.updateProductpackingAsync(ID, SKU, BarCode, PLU, Amount, packing, barCodeEAN13, artNr, GGN, isActive);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;

                                string package = productSKUData["Package"].ToString();
                                decimal amount = Amount ?? 0;

                                string resultAmount = amount.ToString("0.##");



                                row["SKU"] = SKU;
                                row["BarCode"] = BarCode;
                                row["PLU"] = PLU;
                                if (package.CompareTo("kg") == 0 && packing.CompareTo("") != 0 && amount > 0)
                                {
                                    row["Name_VN"] = productSKUData["ProductNameVN"] + " " + productSKUData["PackingType"] + " " + resultAmount + " " + packing; 
                                    row["Name_EN"] = productSKUData["ProductNameEN"] + " " + productSKUData["PackingType"] + " " + resultAmount + " " + packing; 
                                }
                                else
                                {
                                    row["Name_VN"] = productSKUData["ProductNameVN"];
                                    row["Name_EN"] = productSKUData["ProductNameEN"];
                                }

                                row["ProductNameVN_NoSign"] = Utils.RemoveVietnameseSigns(productSKUData["ProductNameVN"] + " " + SKU).ToLower();
                                row["Packing"] = packing;
                                row["PriceCNF"] = productSKUData["PriceCNF"];
                                row["BarCodeEAN13"] = barCodeEAN13;
                                row["ArtNr"] = artNr;
                                row["GGN"] = GGN;
                                row["PackingName"] = $"{resultAmount} {packing}";
                                row["IsActive"] = isActive;
                                if (Amount != null)
                                    row["Amount"] = Amount;
                                else
                                    row["Amount"] = DBNull.Value;
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

        private async void createNewProducrpacking(int SKU, string BarCode, string PLU, int? Amount, string packing, string barCodeEAN13, string artNr, string GGN)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newId = await SQLManager.Instance.insertProductpackingAsync(SKU, BarCode, PLU, Amount, packing, barCodeEAN13, artNr, GGN);
                    if (newId > 0)
                    {
                        DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
                        DataRow drToAdd = packing_dt.NewRow();

                        string package = productSKUData["Package"].ToString();
                        decimal amount = Amount ?? 0;
                        string resultAmount = amount.ToString("0.##");

                        drToAdd["ProductPackingID"] = newId;
                        drToAdd["SKU"] = SKU;
                        drToAdd["BarCode"] = BarCode;
                        drToAdd["PLU"] = PLU;
                        if (package.CompareTo("kg") == 0 && packing.CompareTo("") != 0 && amount > 0)
                        {
                            drToAdd["Name_VN"] = productSKUData["ProductNameVN"] + " " + productSKUData["PackingType"] + " " + Amount + " " + packing;
                            drToAdd["Name_EN"] = productSKUData["ProductNameEN"] + " " + productSKUData["PackingType"] + " " + Amount + " " + packing;
                        }
                        else
                        {
                            drToAdd["Name_VN"] = productSKUData["ProductNameVN"];
                            drToAdd["Name_EN"] = productSKUData["ProductNameEN"];
                        }
                        drToAdd["ProductNameVN_NoSign"] = Utils.RemoveVietnameseSigns(productSKUData["ProductNameVN"] + " " + SKU).ToLower();

                        drToAdd["Packing"] = packing;
                        drToAdd["PriceCNF"] = productSKUData["PriceCNF"]; ;
                        drToAdd["BarCodeEAN13"] = barCodeEAN13;
                        drToAdd["ArtNr"] = artNr;
                        drToAdd["GGN"] = GGN;
                        drToAdd["IsActive"] = true;
                        drToAdd["IsActive_SKU"] = true;

                        drToAdd["PackingName"] = $"{resultAmount} {packing}";

                        if (Amount != null)
                            drToAdd["Amount"] = Amount;
                        else
                            drToAdd["Amount"] = DBNull.Value;


                        // SKU_tb.Text = newCustomerID.ToString();

                        id_tb.Text = newId.ToString();
                        packing_dt.Rows.Add(drToAdd);
                        packing_dt.AcceptChanges();

                        // chuyển selected vào row mới
                        dataGV.ClearSelection(); // bỏ chọn row cũ
                        int rowIndex = dataGV.Rows.Count - 1;
                        dataGV.Rows[rowIndex].Selected = true;


                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;


                        newBtn_Click(null, null);
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

            if (sku_cbb.Text.CompareTo("") == 0 || 
                (packingStr.CompareTo("") != 0 && amount_tb.Text.CompareTo("") == 0) ||
                (packing_panel.Controls.Count > 0 && packingStr.CompareTo("") == 0)) {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return; 
            }

            int sku = Convert.ToInt32(sku_cbb.SelectedValue);
            int? amount = string.IsNullOrWhiteSpace(amount_tb.Text)? (int?)null : int.Parse(amount_tb.Text);

            if (id_tb.Text.Length != 0)
                updateProductPacking(int.Parse(id_tb.Text), sku, barCode_tb.Text, PLU_tb.Text, amount, packingStr, barCodeEAN13_tb.Text, artNr_tb.Text, GGN_tb.Text, isActive_cb.Checked);
            else
                createNewProducrpacking(sku, barCode_tb.Text, PLU_tb.Text, amount, packingStr, barCodeEAN13_tb.Text, artNr_tb.Text, GGN_tb.Text);

        }
        private async void deletePackingProduct()
        {
            string id = id_tb.Text;

            foreach (DataRow row in packing_dt.Rows)
            {
                string productPackingID = row["ProductPackingID"].ToString();
                if (productPackingID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteProductpackingAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                packing_dt.Rows.Remove(row);
                                packing_dt.AcceptChanges();
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

        private void newBtn_Click(object sender, EventArgs e)
        {
            sku_cbb.SelectedIndex = -1;
            id_tb.Text = "";
            barCode_tb.Text = "";
            PLU_tb.Text = "";
            nameVN_tb.Text = "";
            nameEN_tb.Text = "";
            status_lb.Text = "";
            amount_tb.Text = "";
            priceCNF_tb.Text = "";
            barCodeEAN13_tb.Text = "";
            artNr_tb.Text = "";
            GGN_tb.Text = "";
            barCode_tb.Focus();           

            sku_cbb_SelectedIndexChanged(sender, e);

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            sku_cbb.Enabled = true;
            RightUiReadOnly(false);
            isActive_cb.Visible = false;
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            sku_cbb.Enabled = false;
            RightUiReadOnly(true);
            isActive_cb.Visible = true;
            if (dataGV.SelectedRows.Count > 0)
                updateDataTextBoxFlowSKU();
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            sku_cbb.Enabled = false;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            isActive_cb.Enabled = true;
        }

        private void sku_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sku_cbb.SelectedItem != null)
            {
                DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;

        //        botanicalName_tb.Text = productSKUData["BotanicalName"].ToString();
                nameVN_tb.Text = productSKUData["ProductNameVN"].ToString();// + " " + amount + " " + packing;
                nameEN_tb.Text = productSKUData["ProductNameEN"].ToString();// + " " + amount + " " + packing;
                priceCNF_tb.Text = productSKUData["PriceCNF"].ToString();
                packing_panel.Controls.Clear();
                if (productSKUData["PackingList"].ToString().CompareTo("") != 0)
                {
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
                        rb.Checked = true;
                     //   rb.TabIndex = sku_cbb.TabIndex + 1;
                    }

                    if (dataGV.SelectedRows.Count > 0)
                    {
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
        }

        private void updateDataTextBoxFlowSKU()
        {
            try
            {
                if (isNewState) return;

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
                    bool isActive = Convert.ToBoolean(cells["IsActive"].Value);

                    if (!sku_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["SKU"].ToString() == SKU))
                    {
                        sku_cbb.DataSource = mSKU_dt;
                    }
                    sku_cbb.SelectedValue = SKU;
                    DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
                    if (productSKUData == null) return;

                    id_tb.Text = ID;
                    barCode_tb.Text = barcode;
                    PLU_tb.Text = PLU;
                    //        botanicalName_tb.Text = productSKUData["BotanicalName"].ToString();
                    nameVN_tb.Text = productSKUData["ProductNameVN"].ToString();
                    nameEN_tb.Text = productSKUData["ProductNameEN"].ToString();
                    //    packing_tb.Text = packing;
                    amount_tb.Text = amount;
                    priceCNF_tb.Text = priceCNF;
                    barCodeEAN13_tb.Text = barCodeEAN13;
                    artNr_tb.Text = artNr;
                    GGN_tb.Text = GGN;
                    isActive_cb.Checked = isActive;

                    packing_panel.Controls.Clear();

                    if (productSKUData["PackingList"].ToString().CompareTo("") != 0)
                    {
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
                    }

                    status_lb.Text = "";
                }
            }
            catch { };
        }

        private void priceCNF_tb_TextChanged(object sender, EventArgs e)
        {
           if(priceCNF_tb.Text.Length == 0 && sku_cbb.SelectedItem != null)
            {
                DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;
                priceCNF_tb.Text = productSKUData["PriceCNF"].ToString();
            }
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;

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

        private void search_txt_TextChanged(object sender, EventArgs e)
        {
            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower())
                     .Replace("'", "''"); // tránh lỗi cú pháp '

           // DataTable dt = dataGV.DataSource as DataTable;
           // if (dt == null) return;

            DataView dv = packing_dt.DefaultView;
            dv.RowFilter = $"[ProductNameVN_NoSign] LIKE '%{keyword}%' AND IsActive_SKU = true";

        }

        private void RightUiReadOnly(bool isReadOnly)
        {
            barCodeEAN13_tb.ReadOnly = isReadOnly;
            PLU_tb.ReadOnly = isReadOnly;            
            packing_panel.Enabled = !isReadOnly;
            amount_tb.ReadOnly = isReadOnly;
            barCode_tb.ReadOnly = isReadOnly;
            artNr_tb.ReadOnly = isReadOnly;
            GGN_tb.ReadOnly = isReadOnly;
            isActive_cb.Enabled = !isReadOnly;
        }
    }
}
