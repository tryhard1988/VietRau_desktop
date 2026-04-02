using DocumentFormat.OpenXml.Bibliography;
using RauViet.classes;
using System;
using System.Data;
using System.Drawing;
using System.IO.Packaging;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class UpdatePriceProductSKU : Form
    {
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        DataTable mPoductSKUHistory_dt, mProductSKU_dt;
        public UpdatePriceProductSKU()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loai_cbb.DisplayMember = "Text";
            loai_cbb.ValueMember = "Value";
            loai_cbb.DataSource = new[]
            {
                new { Text = "Tăng.", Value = "T" },
                new { Text = "Giảm.", Value = "G" }
            };

            hinhthuc_cbb.DisplayMember = "Text";
            hinhthuc_cbb.ValueMember = "Value";
            hinhthuc_cbb.DataSource = new[]
            {
                new { Text = "+/-", Value = "ct" },
                new { Text = "%", Value = "pt" }
                
            };

            Utils.SetTabStopRecursive(this, false);
            int countTab = 0;
            loai_cbb.TabIndex = countTab++; loai_cbb.TabStop = true;
            thaydoi_tb.TabIndex = countTab++; thaydoi_tb.TabStop = true;            
            hinhthuc_cbb.TabIndex = countTab++; hinhthuc_cbb.TabStop = true;
            luuBtn.TabIndex = countTab++; luuBtn.TabStop = true;


            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = true;

            status_lb.Text = "";
                        
            thaydoi_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            this.KeyDown += ProductSKU_KeyDown;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            luuBtn.Click += saveBtn_Click;
        }

        private void ProductSKU_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeProductSKUHistory();
                SQLStore_Kho.Instance.removeProductSKU();
                ShowData();
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);
            try
            {
                // Chạy truy vấn trên thread riêng
                var productSKUATask = SQLStore_Kho.Instance.getProductSKUAsync();
                var productSKUHistoryTask = SQLStore_Kho.Instance.GetProductSKUHistoryAsync();

                await Task.WhenAll(productSKUATask, productSKUHistoryTask);
                mProductSKU_dt = productSKUATask.Result;
                mPoductSKUHistory_dt = productSKUHistoryTask.Result;

                if(mProductSKU_dt.Columns.Contains("DaThem") == false)
                    mProductSKU_dt.Columns.Add("DaThem", typeof(string));

                Utils.SetGridOrdinal(mProductSKU_dt, new[] { "SKU", "ProductNameVN", "ProductNameEN", "PackingType", "Package", "PackingList", "BotanicalName", "PriceCNF", "PlantingAreaCode", "LOTCodeHeader", "Priority" });
               
                priceCNFHisGV.DataSource = mPoductSKUHistory_dt;
                dataGV.DataSource = mProductSKU_dt;


                Utils.HideColumns(dataGV, new[] { "ProductSKU", "ProductNameVN_NoSign", "PackingType", "GroupProduct", "SupplierID", "ProductNameEN", "BotanicalName", "PackingList", "PackingType", "PlantingAreaCode", "LOTCodeHeader", "SupplierName" });
                Utils.HideColumns(priceCNFHisGV, new[] { "id", "SKU" });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"ProductNameVN", "Tên Sản Phẩm (VN)" },
                    {"PriceCNF", "Giá CNF\n(CHF/Kg)" },
                    {"Priority", "Ưu\nTiên" },
                    {"IsActive", "H.Động" },
                    {"DaThem", "Đã Thêm" }
                });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"ProductNameVN", 270},
                    {"PriceCNF", 100},
                    {"Priority", 100},  
                    {"IsActive", 60}
                });

                dataGV.Columns["PriceCNF"].Visible = !UserManager.Instance.hasRole_AnGiaSanPham();
                priceCNFHisGV.Visible = !UserManager.Instance.hasRole_AnGiaSanPham();

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                Utils.SetGridFormat_Alignment(dataGV, "PriceCNF", DataGridViewContentAlignment.MiddleCenter);
                Utils.SetGridFormat_Alignment(dataGV, "package", DataGridViewContentAlignment.MiddleCenter);
                Utils.SetGridFormat_Alignment(dataGV, "PackingType", DataGridViewContentAlignment.MiddleCenter);
                Utils.SetGridFormat_Alignment(dataGV, "Priority", DataGridViewContentAlignment.MiddleCenter);
                Utils.SetGridFormat_Alignment(dataGV, "PackingList", DataGridViewContentAlignment.MiddleCenter);
                Utils.SetGridFormat_Alignment(dataGV, "SKU", DataGridViewContentAlignment.MiddleCenter);

                search_txt_TextChanged(null, null);
            }
            catch
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
            
        private async Task<bool> updateProductSKU(int SKU, decimal OldpriceCNF, decimal priceCNF, string productName)
        {
            foreach (DataRow row in mProductSKU_dt.Rows)
            {
                int _SKU =Convert.ToInt32(row["SKU"]);
                if (_SKU.CompareTo(SKU) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show($"Cập Nhật Cho {productName} {OldpriceCNF} => {priceCNF}", "Thông Tin", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.updatePriceProductSKUAsync(SKU, priceCNF);

                            if (isScussess == true)
                            {
                                SQLStore_Kho.Instance.removeProductpacking();

                                await SQLManager_Kho.Instance.SaveProductSKUHistoryAsync(SKU, priceCNF);

                                row["PriceCNF"] = priceCNF;
                                row["DaThem"] = "V";

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                                MessageBox.Show("Có Lỗi");
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;

                            MessageBox.Show("Có Lỗi " + ex.Message);
                        }

                        return true;
                    }
                    else if (dialogResult == DialogResult.No)
                        return true;
                    else
                        return false;
                }
            }

            return true;
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if (thaydoi_tb.Text.CompareTo("") == 0)
                return;

            DialogResult dialogResult = MessageBox.Show($"Bắt Đầu Thay Đổi", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
                return;

            string loai = loai_cbb.SelectedValue.ToString();
            string hinhthuc = hinhthuc_cbb.SelectedValue.ToString();
            decimal thayDoi = Utils.ParseDecimalSmart(thaydoi_tb.Text);

            loai_cbb.ValueMember = "Value";
            loai_cbb.DataSource = new[]
            {
                new { Text = "Tăng.", Value = "T" },
                new { Text = "Giảm.", Value = "G" }
            };

            hinhthuc_cbb.DisplayMember = "Text";
            hinhthuc_cbb.ValueMember = "Value";
            hinhthuc_cbb.DataSource = new[]
            {
                new { Text = "+/-", Value = "ct" },
                new { Text = "%", Value = "pt" }

            };

            foreach (DataGridViewRow row in dataGV.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    var SKU = Convert.ToInt32(row.Cells["SKU"].Value);
                    var name = row.Cells["ProductNameVN"].Value.ToString();
                    var PriceCNF = Convert.ToDecimal(row.Cells["PriceCNF"].Value);
                    decimal thayDoiConvert = 0;
                    if (hinhthuc.CompareTo("ct") == 0)
                        thayDoiConvert = thayDoi;
                    else if (hinhthuc.CompareTo("pt") == 0)
                        thayDoiConvert = (PriceCNF * thayDoi)/ 100.0m;

                    decimal newPrice = 0;
                    if (loai.CompareTo("T") == 0)
                        newPrice = PriceCNF + thayDoiConvert;
                    else if (loai.CompareTo("G") == 0)
                        newPrice = PriceCNF - thayDoiConvert;

                    bool isContinue = await updateProductSKU(SKU, PriceCNF, newPrice, name);

                    if (!isContinue)
                        break;
                }
            }

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

        private void search_txt_TextChanged(object sender, EventArgs e)
        {
            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower())
                     .Replace("'", "''"); // tránh lỗi cú pháp '

            DataTable dt = dataGV.DataSource as DataTable;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            dv.RowFilter = $"[ProductNameVN_NoSign] LIKE '%{keyword}%'";
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null)
                return;

            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            string SKU = dataGV.CurrentRow.Cells["SKU"].Value.ToString();
            DataView dv = new DataView(mPoductSKUHistory_dt);
            dv.RowFilter = $"SKU = '{SKU}'";
            priceCNFHisGV.DataSource = dv;
        }
    }
}
