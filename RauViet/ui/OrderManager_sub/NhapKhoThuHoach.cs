using Org.BouncyCastle.Tls;
using PdfSharp.Drawing;
using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class NhapKhoThuHoach : Form
    {
        DataTable mHarvest_dt, mInventoryTransaction_dt;

        private LoadingOverlay loadingOverlay;        
        public NhapKhoThuHoach(DataTable data)
        {
            InitializeComponent();
            this.KeyPreview = true;
            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;
            mInventoryTransaction_dt = data;
            LuuThayDoiBtn.Click += saveBtn_Click;

            this.KeyDown += form_KeyDown;
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                System.Windows.Forms.Control ctrl = this.ActiveControl;

                if (ctrl is System.Windows.Controls.TextBox || ctrl is RichTextBox || (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                {
                    return; // không xử lý Delete
                }

                nktd_deleteProduct();
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            await loadingOverlay.Show();

            try
            {
                mHarvest_dt = await SQLStore_KhoVatTu.Instance.GetHarvestScheduleLast3MonthsAsync();
                Utils.SetGridOrdinal(mHarvest_dt, new[] { "SellerName", "HarvestDate", "ProductName", "Quantity", "MaLenh", "Note" });

                mHarvest_dt.Columns.Remove("PlantingID");
                var resultTable = mHarvest_dt.Clone(); // tạo schema mới

                var rows = mHarvest_dt.AsEnumerable()
                    .GroupBy(r => new
                    {
                        MaLenh = r.Field<string>("MaLenh"),
                        SKU = r.Field<int?>("SKU")
                    })
                    .Select(g =>
                    {
                        var firstRow = g
                            .OrderByDescending(r => r.Field<DateTime>("HarvestDate"))
                            .First();

                        var newRow = resultTable.NewRow(); // ✅ đúng table

                        var note = string.Join(", ", g.Select(r => r.Field<string>("ProductionOrder")).Select(x => x).Distinct());

                        newRow["MaLenh"] = g.Key.MaLenh;
                        newRow["SKU"] = g.Key.SKU ?? 0;

                        newRow["SellerID"] = firstRow["SellerID"];
                        newRow["SellerName"] = firstRow["SellerName"];
                        newRow["ProductName"] = firstRow["ProductName"];
                        newRow["HarvestDate"] = firstRow["HarvestDate"];
                        newRow["Quantity"] = g.Sum(r => r.Field<decimal?>("Quantity") ?? 0);                        
                        newRow["Note"] = $"LSX: {note}";

                        return newRow;
                    });

                foreach (var row in rows)
                {
                    resultTable.Rows.Add(row);
                }

                // gán lại nếu cần
                mHarvest_dt = resultTable;

                var rowsToDelete = mHarvest_dt.AsEnumerable()
                                    .Where(h =>
                                    {
                                        var sku = h.Field<int?>("SKU");
                                        var farm = h.Field<string>("MaLenh");

                                        return mInventoryTransaction_dt.AsEnumerable()
                                            .Any(i =>
                                                i.Field<int?>("SKU") == sku &&
                                                i.Field<string>("FarmSourceCode") == farm
                                            );
                                    })
                                    .ToList(); // ⚠️ rất quan trọng

                // Xóa sau khi đã ToList()
                foreach (var row in rowsToDelete)
                {
                    row.Delete();
                }

                mHarvest_dt.AcceptChanges();

                mHarvest_dt.Columns.Remove("ProductionOrder");
                dataGV.DataSource = mHarvest_dt;
                Utils.HideColumns(dataGV, new[] { "SKU", "SellerID", "HarvestID" });
                
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"HarvestDate", "Ngày" },
                        {"ProductName", "Sản Phẩm" },
                        {"Quantity", "Số Lượng" },
                        {"MaLenh", "Mã Lệnh" },
                        {"SellerName", "Nguồn" },
                        {"Note", "Ghi Chú" }
                    });
                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"HarvestDate", 70 },
                        {"ProductName", 120 },
                        {"Quantity", 70 },
                        {"MaLenh", 100 },
                        {"Note", 250 }
                    });

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }

        }
        
        private async void saveBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            await loadingOverlay.Show();

            foreach (DataRow row in mHarvest_dt.Rows)
            {
                int sku = Convert.ToInt32(row["SKU"]);
                int sellerID = Convert.ToInt32(row["SellerID"]);
                string productName = row["ProductName"].ToString();
                decimal quantity = Convert.ToDecimal(row["Quantity"]);
                string maLenh = row["MaLenh"].ToString();
                DateTime harvestDate = Convert.ToDateTime(row["HarvestDate"]);
                string note = row["Note"].ToString();
                string sellerName = row["SellerName"].ToString();

                string newVallueStr = $"Tên SP: {productName}; Hành động: Nhập Hàng; số lượng: {quantity}; Khu Vực: {maLenh}; ngày: {harvestDate}; ghi chứ: {note}; Người Bán: {sellerName}";
                try
                {
                    int newId = await SQLManager_Kho.Instance.insertVegetableWarehouseTransactionAsync(sku, "N", quantity, maLenh, harvestDate, note, sellerID, 0, "kg");

                    if (newId > 0)
                    {
                        _ = SQLManager_Kho.Instance.insertVegetableWarehouseTransactionLOGAsync(maLenh, "", "New Sucess: " + newVallueStr);
                    }
                    else
                    {
                        _ = SQLManager_Kho.Instance.insertVegetableWarehouseTransactionLOGAsync(maLenh, "", "New Fail: " + newVallueStr);
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager_Kho.Instance.insertVegetableWarehouseTransactionLOGAsync(maLenh, "Exception: " + ex.Message, "New Fail: " + newVallueStr);
                }
            }

            await Task.Delay(200);
            loadingOverlay.Hide();
            this.Close();
        }

        private async void nktd_deleteProduct()
        {
            foreach (DataGridViewRow dRow in dataGV.SelectedRows)
            {
                string id = dRow.Cells["HarvestID"].Value.ToString();

                DataRow[] rows = mHarvest_dt.Select($"HarvestID = '{id}'");

                foreach (DataRow row in rows)
                {
                    row.Delete(); // đánh dấu xóa
                }
            }

            // cập nhật 1 lần
            mHarvest_dt.AcceptChanges();
        }
    }
}
