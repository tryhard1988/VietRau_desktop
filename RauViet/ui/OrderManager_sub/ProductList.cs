﻿using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class ProductList : Form
    {
        DataTable mSKU_dt;
        private Timer debounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public ProductList()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
        //    this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            priceCNF_tb.TextChanged += priceCNF_tb_TextChanged;
            search_tb.TextChanged += search_txt_TextChanged;
            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            debounceTimer.Tick += DebounceTimer_Tick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);

        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                var skuTask = SQLStore.Instance.getProductSKUAsync();
                var packingTask = SQLStore.Instance.getProductpackingAsync();

                await Task.WhenAll(skuTask, packingTask);

                mSKU_dt = skuTask.Result;
                DataTable packing_dt = packingTask.Result;

                foreach (DataColumn col in packing_dt.Columns)
                    col.ReadOnly = false;

                if (!packing_dt.Columns.Contains("ProductNameVN_NoSign"))
                    packing_dt.Columns.Add("ProductNameVN_NoSign", typeof(string));

                sku_cbb.DataSource = mSKU_dt;
                sku_cbb.DisplayMember = "ProductNameVN";  // hiển thị tên
                sku_cbb.ValueMember = "SKU";
                sku_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                sku_cbb.AutoCompleteSource = AutoCompleteSource.ListItems;
                sku_cbb.SelectedIndexChanged += sku_cbb_SelectedIndexChanged;
                sku_cbb.TextUpdate += sku_cbb_TextUpdate;

                // Thêm cột vào DataTable
                packing_dt.Columns.Add(new DataColumn("PriceCNF", typeof(string)));
                packing_dt.Columns.Add(new DataColumn("Name_VN", typeof(string)));
                packing_dt.Columns.Add(new DataColumn("Name_EN", typeof(string)));
               // packing_dt.Columns.Add(new DataColumn("Package", typeof(string)));

                // Chuyển cột vào vị trí mong muốn

                packing_dt.Columns["Name_VN"].SetOrdinal(3); 
                packing_dt.Columns["Name_EN"].SetOrdinal(4);
                packing_dt.Columns["Amount"].SetOrdinal(5);
                // PriceCNF.SetOrdinal(5); // chèn vào vị trí index 3
                // Gán giá trị cho từng dòng
                foreach (DataRow dr in packing_dt.Rows)
                {
                    int sku = Convert.ToInt32(dr["SKU"]);
                    DataRow row = mSKU_dt.Select($"SKU = '{sku}'")[0];

                    string package = row["Package"].ToString();
                    string nameVN = row["ProductNameVN"].ToString();
                    string nameEN = row["ProductNameEN"].ToString();
                    string packingType = row["PackingType"].ToString();
                    decimal amount = dr["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Amount"]);
                    string packing = dr["Packing"].ToString();
                    dr["Name_EN"] = nameEN;

                    string resultAmount = amount.ToString("0.##");
                    dr["Amount"] = resultAmount;

                    if (package.CompareTo("kg") == 0 && packing.CompareTo("") != 0 && amount > 0)
                    {
                        
                        dr["Name_VN"] = nameVN + " " + packingType + " " + resultAmount + " " + packing;
                        dr["Name_EN"] = nameEN + " " + packingType + " " + resultAmount + " " + packing;
                    }
                    else
                    {
                        dr["Name_VN"] = nameVN;
                        dr["Name_EN"] = nameEN;
                    }

                    dr["ProductNameVN_NoSign"] = Utils.RemoveVietnameseSigns(nameVN + " " + sku).ToLower();

                    // dr["Package"] = package;
                    dr["PriceCNF"] = row["PriceCNF"].ToString();
                }

                dataGV.DataSource = packing_dt;
                dataGV.Columns["ProductPackingID"].Visible = false;
               // dataGV.Columns["Amount"].Visible = false;
               // dataGV.Columns["Packing"].Visible = false;
                dataGV.Columns["PriceCNF"].Visible = false;
                dataGV.Columns["SKU"].Visible = false; 
                dataGV.Columns["ProductNameVN_NoSign"].Visible = false;


                //  dataGV.Columns["SKU"].Visible = false;

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
                dataGV.Columns["Packing"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["Amount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dataGV.Columns["PriceCNF"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dataGV.AutoResizeColumns();

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

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            dataGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Dock = DockStyle.Fill;

            
        }

        
        private void sku_cbb_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            debounceTimer.Stop();
            debounceTimer.Start();
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            debounceTimer.Stop();

            string typed = sku_cbb.Text ?? "";
            string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

            // Filter bằng LINQ
            var filtered = mSKU_dt.AsEnumerable()
                .Where(r => Utils.RemoveVietnameseSigns(r["ProductNameVN"].ToString().ToLower())
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

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }
        
        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateDataTextBoxFlowSKU();            
        }

       

        private async void updateProductPacking(int ID, int SKU, string BarCode, string PLU, int? Amount, string packing, string barCodeEAN13, string artNr, string GGN)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int productPackingID = Convert.ToInt32(row.Cells["ProductPackingID"].Value);
                if (productPackingID.CompareTo(ID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {      
                            bool isScussess = await SQLManager.Instance.updateProductpackingAsync(ID, SKU, BarCode, PLU, Amount, packing, barCodeEAN13, artNr, GGN);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                DataRowView productSKUData = (DataRowView)sku_cbb.SelectedItem;

                                string package = productSKUData["Package"].ToString();
                                decimal amount = Amount ?? 0;

                                string resultAmount = amount.ToString("0.##");



                                row.Cells["SKU"].Value = SKU;
                                row.Cells["BarCode"].Value = BarCode;
                                row.Cells["PLU"].Value = PLU;
                                if (package.CompareTo("kg") == 0 && packing.CompareTo("") != 0 && amount > 0)
                                {
                                    row.Cells["Name_VN"].Value = productSKUData["ProductNameVN"] + " " + productSKUData["PackingType"] + " " + resultAmount + " " + packing; 
                                    row.Cells["Name_EN"].Value = productSKUData["ProductNameEN"] + " " + productSKUData["PackingType"] + " " + resultAmount + " " + packing; 
                                }
                                else
                                {
                                    row.Cells["Name_VN"].Value = productSKUData["ProductNameVN"];
                                    row.Cells["Name_EN"].Value = productSKUData["ProductNameEN"];
                                }
                                    row.Cells["Packing"].Value = packing;
                                row.Cells["PriceCNF"].Value = productSKUData["PriceCNF"];
                                row.Cells["BarCodeEAN13"].Value = barCodeEAN13;
                                row.Cells["ArtNr"].Value = artNr;
                                row.Cells["GGN"].Value = GGN;
                                if(Amount != null)
                                    row.Cells["Amount"].Value = Amount;
                                else
                                    row.Cells["Amount"].Value = DBNull.Value;
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
                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

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
                        drToAdd["Packing"] = packing;
                        drToAdd["PriceCNF"] = productSKUData["PriceCNF"]; ;
                        drToAdd["BarCodeEAN13"] = barCodeEAN13;
                        drToAdd["ArtNr"] = artNr;
                        drToAdd["GGN"] = GGN;
                        if (Amount != null)
                            drToAdd["Amount"] = Amount;
                        else
                            drToAdd["Amount"] = DBNull.Value;


                        // SKU_tb.Text = newCustomerID.ToString();

                        id_tb.Text = newId.ToString();
                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

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
                updateProductPacking(int.Parse(id_tb.Text), sku, barCode_tb.Text, PLU_tb.Text, amount, packingStr, barCodeEAN13_tb.Text, artNr_tb.Text, GGN_tb.Text);
            else
                createNewProducrpacking(sku, barCode_tb.Text, PLU_tb.Text, amount, packingStr, barCodeEAN13_tb.Text, artNr_tb.Text, GGN_tb.Text);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            string id = id_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string productPackingID = row.Cells["ProductPackingID"].Value.ToString();
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
            delete_btn.Visible = false;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            delete_btn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
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

            DataTable dt = dataGV.DataSource as DataTable;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            dv.RowFilter = $"[ProductNameVN_NoSign] LIKE '%{keyword}%'";

        }
    }
}
