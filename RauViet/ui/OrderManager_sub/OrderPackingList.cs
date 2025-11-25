
using Microsoft.VisualBasic;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class OrderPackingList : Form
    {
        private LoadingOverlay loadingOverlay;
        DataTable mExportCode_dt, mOrders_dt, mCartonSize_dt, mProductSKU_dt, mProductPacking_dt;
        DataView mOrderPackingLog_dv;
        private Timer debounceTimer = new Timer { Interval = 300 };
        object oldValue;
        int mCurrentExportID = -1;
        public OrderPackingList()
        {
            InitializeComponent();

            Utils.SetTabStopRecursive(this, false);
            this.KeyPreview = true;
            int countTab = 0;
            cartonNo_tb.TabIndex = countTab++; cartonNo_tb.TabStop = true;
            cartonSize_cbb.TabIndex = countTab++; cartonSize_cbb.TabStop = true;

            setUIReadOnly(true);

            dataGV.ScrollBars = ScrollBars.Both;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGV.MultiSelect = false;

            carton_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            carton_GV.MultiSelect = false;
            carton_GV.RowHeadersVisible = false;
            carton_GV.AllowUserToAddRows = false;
            carton_GV.AllowUserToResizeRows = false;

            status_lb.Text = "";

            fillter_btn.Click += fillter_btn_Click;
            previewPrint_PT_btn.Click += previewPrint_PT_btn_Click;
            InPhieuGiaoHang_btn.Click += InPhieuGiaoHang_btn_Click;
            previewPrint_PGH_btn.Click += previewPrint_PGH_btn_Click;
            assignCustomerCarton_btn.Click += assignCustomerCarton_btn_Click;
            dataGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGV_EditingControlShowing);


            dataGV.DataBindingComplete += dataGV_DataBindingComplete;
            dataGV.CellFormatting += dataGV_CellFormatting;
            dataGV.CellEndEdit += dataGV_CellEndEdit;
            dataGV.KeyDown += dataGV_KeyDown;
            dataGV.CellBeginEdit += dataGV_CellBeginEdit;
            dataGV.SelectionChanged += DataGV_SelectionChanged;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += closeEdit_btn_Click;
            chiadon_btn.Click += Chiadon_btn_Click;

            cartonNo_tb.KeyPress += Tb_KeyPress_OnlyNumber1;
            

            ToolTipHelper.SetToolTip(assignCustomerCarton_btn, "Tự động dựa vào\nCarton.No và Khách Hàng \nĐể tạo mã thùng");
            ToolTipHelper.SetToolTip(chiadon_btn, "Dùng khi 1 đơn hàng cần chia ra 2 thùng \nthì cần tạo ra thêm 1 mã đơn hàng mới\ntừ mã hiện tại");

            search_tb.TextChanged += search_txt_TextChanged;
            debounceTimer.Tick += DebounceTimer_Tick;
            cartonSize_cbb.KeyPress += CBB_KeyPress_CartonSize;
            cartonSize_cbb.TextUpdate += cartonSize_cbb_TextUpdate;

            cartonSize_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //cartonSize_cbb.AutoCompleteSource = AutoCompleteSource.ListItems;

            this.KeyDown += OrderPackingList_KeyDown;
            carton_GV.CellDoubleClick += Carton_GV_CellDoubleClick; ;

            phieuCanHang_preview_btn.Click += PhieuCanHang_preview_btn_Click;
            phieuCanHang_btn.Click += PhieuCanHang_btn_Click;
            tem_preview_btn.Click += Tem_preview_btn_Click;
            inTem_btn.Click += InTem_btn_Click;
        }

        public async void ShowData()
        {
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(50);
            try
            {
                var cartonSizeTask = SQLStore.Instance.GetCartonSize();                
                string[] keepColumns = { "ExportCodeID", "ExportCode", "ExportDate", "InputByName_NoSign" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                var exportCodeTask = SQLStore.Instance.getExportCodesAsync(keepColumns, parameters);
                var productSKUATask = SQLStore.Instance.getProductSKUAsync();
                var productPackingTask = SQLStore.Instance.getProductpackingAsync();

                await Task.WhenAll(exportCodeTask, cartonSizeTask, productSKUATask, productPackingTask);

                mExportCode_dt = exportCodeTask.Result;
                mCartonSize_dt = cartonSizeTask.Result;
                mProductSKU_dt = productSKUATask.Result;
                mProductPacking_dt = productPackingTask.Result;

                if (mCurrentExportID <= 0 && mExportCode_dt.Rows.Count > 0)
                {
                    mCurrentExportID = Convert.ToInt32(mExportCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("ExportCodeID")));
                }

                var ordersTask = SQLStore.Instance.getOrdersAsync(mCurrentExportID);
                var ordersPackingTask = SQLStore.Instance.GetOrderPackingLogAsync(mCurrentExportID);

                await Task.WhenAll(ordersTask, ordersPackingTask);
                mOrders_dt = ordersTask.Result;
                mOrderPackingLog_dv = new DataView(ordersPackingTask.Result);
                logGV.DataSource = mOrderPackingLog_dv;

                // Chạy truy vấn trên thread riêng
                dataGV.DataSource = mOrders_dt;
                dataGV.Columns["Search_NoSign"].Visible = false;
                dataGV.Columns["ExportCodeID"].Visible = false;
                dataGV.Columns["Amount"].Visible = false;
                dataGV.Columns["packing"].Visible = false;
                dataGV.Columns["ProductNameEN"].Visible = false;
                dataGV.Columns["Package"].Visible = false;
                dataGV.Columns["ExportDate"].Visible = false;
                dataGV.Columns["ExportCode"].Visible = false;
                dataGV.Columns["CustomerCode"].Visible = false;
                dataGV.Columns["CustomerID"].Visible = false;
                dataGV.Columns["ProductPackingID"].Visible = false;
                dataGV.Columns["OrderPackingPriceCNF"].Visible = false;
                dataGV.Columns["LOTCode"].Visible = false;
                dataGV.Columns["LOTCodeComplete"].Visible = false;
                logGV.Columns["LogID"].Visible = false;
                logGV.Columns["ExportCodeID"].Visible = false;
                logGV.Columns["OrderID"].Visible = false;
                dataGV.Columns["SKU"].Visible = false;

                dataGV.ReadOnly = false;
                dataGV.Columns["CartonNo"].ReadOnly = false;
                dataGV.Columns["PCSReal"].ReadOnly = false;
                dataGV.Columns["CartonSize"].ReadOnly = false;
                dataGV.Columns["NWReal"].ReadOnly = true;
                dataGV.Columns["Priority"].ReadOnly = true;
                dataGV.Columns["CustomerName"].ReadOnly = true;
                dataGV.Columns["ProductNameVN"].ReadOnly = true;
                dataGV.Columns["CustomerCarton"].ReadOnly = false;

                dataGV.Columns["OrderId"].HeaderText = "ID";
                dataGV.Columns["PCSReal"].HeaderText = "PCS\nĐóng Thùng";
                dataGV.Columns["NWReal"].HeaderText = "NW\nTĐóng Thùng";
                dataGV.Columns["PCSOther"].HeaderText = "PCS\nĐặt Hàng";
                dataGV.Columns["NWOther"].HeaderText = "NW\nĐặt Hàng";
                dataGV.Columns["CartonNo"].HeaderText = "Carton.No";
                dataGV.Columns["CartonSize"].HeaderText = "Carton Size";
                dataGV.Columns["Priority"].HeaderText = "Ưu\nTiên";
                dataGV.Columns["Customername"].HeaderText = "Khách Hàng";
                dataGV.Columns["ProductNameVN"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["CustomerCarton"].HeaderText = "Mã Thùng";

                dataGV.Columns["OrderId"].Width = 50;
                dataGV.Columns["Priority"].Width = 40;
                dataGV.Columns["PCSReal"].Width = 50;
                dataGV.Columns["NWReal"].Width = 50;
                dataGV.Columns["PCSOther"].Width = 50;
                dataGV.Columns["NWOther"].Width = 50;
                dataGV.Columns["CartonNo"].Width = 60;
                dataGV.Columns["CartonSize"].Width = 80;
                dataGV.Columns["CustomerCarton"].Width = 80;
                //   dataGV.Columns["Priority"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["CustomerName"].Width = 80;
                dataGV.Columns["ProductNameVN"].Width = 220;

                dataGV.Columns["CartonNo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["PCSReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NWReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["OrderId"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["PCSOther"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NWOther"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.EnableHeadersVisualStyles = false; // Tắt theme hệ thống
                dataGV.Columns["PCSReal"].HeaderCell.Style.BackColor = System.Drawing.Color.Coral;
                dataGV.Columns["NWReal"].HeaderCell.Style.BackColor = System.Drawing.Color.Coral;
                //_dataChanged = false;

                exportCode_cbb.SelectedIndexChanged -= exportCode_search_cbb_SelectedIndexChanged;
                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";                
                exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
                exportCode_cbb.SelectedValue = mCurrentExportID;



                cartonSize_cbb.DataSource = mCartonSize_dt;
                cartonSize_cbb.DisplayMember = "CartonSize";
                cartonSize_cbb.ValueMember = "CartonID";

            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                await Task.Delay(100);
                loadingOverlay.Hide();
            }
        }

        private void Chiadon_btn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(
                this,
                "Tạo ra 1 bảng y hệt bảng hiện tại",
                "Chia Đơn",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dialogResult == DialogResult.No) return;

            _ = Chiadon();
        }
        private async Task Chiadon()
        {
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;

            int orderId = -1;
            int newId = -1;
            try
            {
                if (dataGV.CurrentRow == null) return;
                
                // Lấy dòng được chọn cuối cùng (theo thứ tự hiển thị)
                DataGridViewRow lastSelectedRow = dataGV.CurrentRow;

                // Ví dụ: lấy giá trị trong cột ExportCodeID
                orderId = Convert.ToInt32(lastSelectedRow.Cells["OrderId"].Value);

                newId = await SQLManager.Instance.CloneOrder_SplitHalfAsync(orderId);
                DataRow newRow = mOrders_dt.NewRow();
                foreach (DataGridViewCell cell in lastSelectedRow.Cells)
                {
                    string colName = cell.OwningColumn.DataPropertyName;
                    if (string.IsNullOrEmpty(colName)) continue;
                    if (mOrders_dt.Columns.Contains(colName))
                        newRow[colName] = cell.Value ?? DBNull.Value;
                }

                var packageVal = newRow["Package"]?.ToString();

                newRow["OrderId"] = newId;
                newRow["PCSOther"] = 0;
                newRow["NWOther"] = 0;
                newRow["PCSReal"] = DBNull.Value;
                newRow["NWReal"] = DBNull.Value;
                newRow["CartonNo"] = DBNull.Value;
                newRow["CartonSize"] = DBNull.Value;
                newRow["CustomerCarton"] = DBNull.Value;
                if (string.Equals(packageVal, "weight", StringComparison.OrdinalIgnoreCase))
                    newRow["PCSReal"] = 0;

                DataRowView currentView = dataGV.CurrentRow.DataBoundItem as DataRowView;
                DataRow currentRow = currentView.Row;
                int insertInd = mOrders_dt.Rows.IndexOf(currentRow) + 1;

                mOrders_dt.Rows.InsertAt(newRow, insertInd);
                mOrders_dt.AcceptChanges();

                dataGV.ClearSelection();
                var newRowView = mOrders_dt.DefaultView[insertInd];
                int gridIndex = dataGV.Rows.Cast<DataGridViewRow>()
                    .First(r => ((DataRowView)r.DataBoundItem).Row == newRowView.Row).Index;

                if (string.Equals(packageVal, "weight", StringComparison.OrdinalIgnoreCase))
                    dataGV.CurrentCell = dataGV.Rows[gridIndex].Cells["NWReal"];
                else
                    dataGV.CurrentCell = dataGV.Rows[gridIndex].Cells["PCSReal"];

                _ = SQLManager.Instance.InsertOrderPackingLogAsync(exportCodeId, newId, "chia Đơn Từ: " + orderId + " thành công", null, null, null, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi chia đơn: {ex.Message}");
                MessageBox.Show($"Lỗi khi chia đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ = SQLManager.Instance.InsertOrderPackingLogAsync(exportCodeId, newId, "chia Đơn Từ: " + orderId + " Thất Bại do Exception: " + ex, null, null, null, "");
            }
        }

        private async void DeleteOrder()
        {
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;

            if (dataGV.CurrentRow == null) return;
            var currentRow = dataGV.CurrentRow;
            var pcsVal = currentRow.Cells["PCSOther"].Value;
            var nwVal = currentRow.Cells["NWOther"].Value;

            int PCSOther = 0;
            double NWOther = 0;
            int.TryParse(pcsVal?.ToString() ?? "0", out PCSOther);
            double.TryParse(nwVal?.ToString() ?? "0", out NWOther);

            if ((PCSOther > 0 || NWOther > 0)) {
                MessageBox.Show("Chỉ được xóa những dòng có \nPCS(đặt hàng) <= 0 và NW(đặt hàng) <= 0");
                return; 
            }
            DialogResult dialogResult = MessageBox.Show("Xóa Đơn Đang Chọn", "Xóa",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question
                                       );

            if (dialogResult == DialogResult.No) return;

            int id = -1;

            try
            {
                if (dataGV.CurrentRow == null) return;

                // Lấy dòng được chọn cuối cùng (theo thứ tự hiển thị)
                DataGridViewRow lastSelectedRow = dataGV.CurrentRow;

                // Ví dụ: lấy giá trị trong cột ExportCodeID
                id = Convert.ToInt32(lastSelectedRow.Cells["OrderId"].Value);

                bool isSuccess = await SQLManager.Instance.deleteOrderAsync(id);

                if (isSuccess)
                {
                    status_lb.Text = "Thành công.";
                    status_lb.ForeColor = System.Drawing.Color.Green;

                    // Xóa row khỏi DataTable
                    for (int i = mOrders_dt.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow row = mOrders_dt.Rows[i];
                        int orderId = Convert.ToInt32(row["OrderId"]);
                        if (id.CompareTo(orderId) == 0)
                        {
                            mOrders_dt.Rows.Remove(row);
                            mOrders_dt.AcceptChanges();
                        }
                    }

                    _ = SQLManager.Instance.InsertOrderPackingLogAsync(exportCodeId, id, "Delete Thành Công", 0, 0, 0, "");
                }
                else
                {
                    MessageBox.Show("Thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _ = SQLManager.Instance.InsertOrderPackingLogAsync(exportCodeId, id, "Delete Thất Bại", 0, 0, 0, "");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi Xóa Đơn: {ex.Message}");
                MessageBox.Show($"Lỗi khi Xóa Đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ = SQLManager.Instance.InsertOrderPackingLogAsync(exportCodeId, id, "Delete Thất Bại Do Exception: " + ex.Message, 0, 0, 0, "");
            }
        }

        private async void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;
            mCurrentExportID = exportCodeId;

            var ordersTask = SQLStore.Instance.getOrdersAsync(mCurrentExportID);
            var ordersPackingTask = SQLStore.Instance.GetOrderPackingLogAsync(mCurrentExportID);
            await Task.WhenAll(ordersTask, ordersPackingTask);
            mOrders_dt = ordersTask.Result;
            mOrderPackingLog_dv = new DataView(ordersPackingTask.Result);
            logGV.DataSource = mOrderPackingLog_dv;

            // Tạo DataView để filter
            DataView dv = new DataView(mOrders_dt);
            dv.RowFilter = $"ExportCodeID = {exportCodeId}";

            // Gán lại cho DataGridView
            dataGV.DataSource = dv;                


            setUIReadOnly(true);
            fillter_btn_Click(null, null);
        }


        private async void dataGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (exportCode_cbb.SelectedValue == null || !int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
                return;
            
            int orderId = -1;
            int? pcsReal = null;
            decimal? nwReal = null;
            int? cartonNo = null;
            string cartonSize = "";
            var columnName = dataGV.Columns[e.ColumnIndex].Name;
            try
            {
                if (dataGV.CurrentRow == null || e.ColumnIndex < 0) return;

                object newValue = dataGV.CurrentCell?.Value;//.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";

                if (object.Equals(oldValue, newValue)) return;

                var row = dataGV.CurrentRow;
                if (row?.Cells["OrderId"].Value == null || row.Cells["OrderId"].Value == DBNull.Value)
                    return;

                orderId = Convert.ToInt32(row.Cells["OrderId"].Value);
                DataRow orderRow = mOrders_dt.Select($"OrderId = {orderId}").FirstOrDefault();
                if (orderRow == null) return;

                if (columnName == "PCSReal")
                    UpdateNWReal(orderRow);

                if (columnName == "CartonNo")
                    orderRow["CustomerCarton"] = DBNull.Value;

                if (columnName == "PCSReal" || columnName == "NWReal")
                {
                    int PCSReal = int.TryParse(orderRow["PCSReal"]?.ToString(), out var p) ? p : 0;
                    decimal NWReal = decimal.TryParse(orderRow["NWReal"]?.ToString(), out var w) ? w : 0;

                    if (PCSReal == 0 && NWReal == 0)
                    {
                        orderRow["CartonNo"] = DBNull.Value;
                        orderRow["CartonSize"] = DBNull.Value;
                        orderRow["CustomerCarton"] = DBNull.Value;
                        orderRow["NWReal"] = DBNull.Value;

                        if (columnName == "PCSReal")
                            orderRow["PCSReal"] = DBNull.Value;
                        else
                            orderRow["PCSReal"] = 0;
                    }
                    else
                    {
                        object cartonObj = orderRow["CartonNo"];
                        bool isEmptyCarton = cartonObj == null || cartonObj == DBNull.Value ||
                                     string.IsNullOrWhiteSpace(cartonObj.ToString());

                        if (isEmptyCarton)
                        {
                            orderRow["CustomerCarton"] = DBNull.Value;
                            if (int.TryParse(cartonNo_tb.Text, out int cartonNo1))
                                orderRow["CartonNo"] = cartonNo1;
                            else
                                orderRow["CartonNo"] = DBNull.Value;
                        }

                        if (orderRow["CartonSize"] == DBNull.Value || string.IsNullOrWhiteSpace(orderRow["CartonSize"].ToString()))
                        {
                            orderRow["CartonSize"] = cartonSize_cbb.Text;
                        }


                    }
                }

                pcsReal = int.TryParse(orderRow["PCSReal"]?.ToString(), out int pcs) ? pcs : (int?)null;
                nwReal = decimal.TryParse(orderRow["NWReal"]?.ToString(), out decimal nw) ? nw : (decimal?)null;
                cartonNo = int.TryParse(orderRow["CartonNo"]?.ToString(), out int carton) ? carton : (int?)null;
                cartonSize = orderRow["CartonSize"]?.ToString();
                string customerCarton = orderRow["CustomerCarton"]?.ToString();
                var orders = new List<(int, int?, decimal?, int?, string, string)>{(orderId, pcsReal, nwReal, cartonNo, cartonSize, customerCarton)};
               
                bool isScussess = await SQLManager.Instance.UpdatePackOrdersBulkAsync(orders);
                if (!isScussess)
                {
                    MessageBox.Show($"Cập Nhật Thất Bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _ = SQLManager.Instance.InsertOrderPackingLogAsync(exportCodeId, orderId, $"{columnName} {oldValue}: Update Thất Bại", pcsReal, nwReal, cartonNo, cartonSize);
                }
                else
                {
                    _ = SQLManager.Instance.InsertOrderPackingLogAsync(exportCodeId, orderId, $"{columnName} {oldValue}: Update Thành Công", pcsReal, nwReal, cartonNo, cartonSize);
                    status_lb.Text = "Thành công.";
                    status_lb.ForeColor = System.Drawing.Color.Green;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý thay đổi dữ liệu:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ = SQLManager.Instance.InsertOrderPackingLogAsync(exportCodeId, orderId, $"{columnName} {oldValue}: Update Thất Bại Do Exception: " + ex.Message, pcsReal, nwReal, cartonNo, cartonSize);
            }
        }

        private void UpdateNWReal(DataRow row)
        {
            if (row == null) return;
            var cellPCS = row["PCSReal"];
            if (cellPCS != null && int.TryParse(cellPCS.ToString(), out int pcs))
            {
                var amount = Convert.ToInt32(row["Amount"]);
                var packing = Convert.ToString(row["packing"]);

                row["NWReal"] = Utils.calNetWeight(pcs, amount, packing);
            }
            else
            {
                row["NWReal"] = DBNull.Value;
            }
        }

        private Dictionary<string, System.Drawing.Color> colorMap = new Dictionary<string, System.Drawing.Color>();
        private System.Drawing.Color[] colors = new System.Drawing.Color[] {
            System.Drawing.Color.LightBlue, 
            System.Drawing.Color.LightGreen, 
            System.Drawing.Color.LightPink, 
            System.Drawing.Color.LightCyan, 
            System.Drawing.Color .LightSeaGreen,
            System.Drawing.Color .LightSalmon, 
            System.Drawing.Color .Bisque};

        private void dataGV_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            colorMap.Clear();
            int colorIndex = 0;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.Cells["CustomerName"].Value == null) continue;

                string customer = row.Cells["CustomerName"].Value.ToString();

                if (!colorMap.ContainsKey(customer))
                {
                    colorMap[customer] = colors[colorIndex % colors.Length];
                    colorIndex++;
                }
            }
        }

        private void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            var row = dataGV.Rows[e.RowIndex];
            var packageVal = row.Cells["Package"].Value?.ToString();
            if (dataGV.Columns[e.ColumnIndex].Name == "CustomerName")
            {
                string customer = e.Value?.ToString();
                if (!string.IsNullOrEmpty(customer) && colorMap.ContainsKey(customer))
                {
                    // Luôn giữ màu CustomerName, kể cả khi row được chọn
                    e.CellStyle.BackColor = colorMap[customer];
                    e.CellStyle.SelectionBackColor = colorMap[customer];
                    e.CellStyle.SelectionForeColor = dataGV.DefaultCellStyle.ForeColor;
                }
            }
            else if (dataGV.Columns[e.ColumnIndex].Name == "CartonNo" ||
                    dataGV.Columns[e.ColumnIndex].Name == "CartonSize" ||
                    dataGV.Columns[e.ColumnIndex].Name == "CustomerCarton")
            {
                e.CellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (dataGV.Columns[e.ColumnIndex].Name == "PCSReal")
            {
                if (!string.Equals(packageVal, "weight", StringComparison.OrdinalIgnoreCase))
                {
                    row.Cells["PCSReal"].ReadOnly = false;
                    e.CellStyle.BackColor = System.Drawing.Color.LightGray;
                }
                else
                {
                    row.Cells["PCSReal"].ReadOnly = true;
                  //  row.Cells["PCSReal"].Value = 0;
                }
            }
            else if (dataGV.Columns[e.ColumnIndex].Name == "NWReal")
            {
                
                if (string.Equals(packageVal, "weight", StringComparison.OrdinalIgnoreCase))
                {
                    row.Cells["NWReal"].ReadOnly = false;
                    e.CellStyle.BackColor = System.Drawing.Color.LightGray;
                }
                else
                {
                    row.Cells["NWReal"].ReadOnly = true;
                }
            }
            else if (dataGV.Columns[e.ColumnIndex].Name == "PCSOther" || dataGV.Columns[e.ColumnIndex].Name == "NWOther")
            {
                int PCSOther = int.TryParse(row.Cells["PCSOther"].Value?.ToString(), out var pcs) ? pcs : 0;
                decimal NWOther = decimal.TryParse(row.Cells["NWOther"].Value?.ToString(), out var nw) ? nw : 0;


                if (PCSOther <= 0 && NWOther <= 0)
                {
                    e.CellStyle.BackColor = System.Drawing.Color.PaleVioletRed;
                }
            }
        }      

        private void dataGV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = e.Control as TextBox;
            if (tb == null) return;

            // Gỡ toàn bộ trước để tránh add trùng
            tb.KeyPress -= Tb_KeyPress_OnlyNumber;
            tb.KeyPress -= Tb_KeyPress_CartonSize;
            tb.KeyPress -= Tb_KeyPress_OnlyNumber1;

            var colName = dataGV.CurrentCell.OwningColumn.Name;

            if ( colName == "NWReal")
            {
                tb.KeyPress += Tb_KeyPress_OnlyNumber;
            }
            else if (colName == "PCSReal" || colName == "CartonNo")
            {
                tb.KeyPress += Tb_KeyPress_OnlyNumber1;
            }
            else if (colName == "CartonSize")
            {
                tb.KeyPress += Tb_KeyPress_CartonSize;
            }
        }


        private void dataGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            oldValue = dataGV.CurrentCell?.Value;
            if (exportCode_cbb.SelectedItem != null)
            {
                DataRowView dataR = (DataRowView)exportCode_cbb.SelectedItem; 
                string staff = dataR["InputByName_NoSign"].ToString();
                if (UserManager.Instance.fullName_NoSign.CompareTo(staff) != 0)
                {
                    e.Cancel = true;
                    return;
                }
            }
            e.Cancel = edit_btn.Visible;

            // dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            // Chỉ cho nhập số, phím điều khiển hoặc dấu chấm
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true; // chặn ký tự không hợp lệ
            }

            // Không cho nhập nhiều dấu chấm
            if (e.KeyChar == '.' && tb.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void Tb_KeyPress_OnlyNumber1(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            // Chỉ cho nhập số, phím điều khiển hoặc dấu chấm
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // chặn ký tự không hợp lệ
            }

        }

        private void Tb_KeyPress_CartonSize(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;

            // Cho phép phím điều khiển (Backspace, Delete, mũi tên...)
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            // Nếu không phải số hoặc 'x' thì chặn
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 'x' && e.KeyChar != 'X')
            {
                e.Handled = true;
                return;
            }

            string text = tb.Text;
            int selectionStart = tb.SelectionStart; // vị trí con trỏ

            // Nếu ký tự nhập là 'x'
            if (e.KeyChar == 'x' || e.KeyChar == 'X')
            {
                // 1. Không cho 'x' ở đầu
                if (selectionStart == 0)
                {
                    e.Handled = true;
                    return;
                }

                // 2. Không cho nhập 2 'x' liên tiếp
                if (selectionStart > 0 &&
                    (text[selectionStart - 1] == 'x' || text[selectionStart - 1] == 'X'))
                {
                    e.Handled = true;
                    return;
                }

                // 3. Không cho phép có nhiều hơn 2 'x' trong chuỗi
                int countX = text.Count(c => c == 'x' || c == 'X');
                if (countX >= 2)
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void CBB_KeyPress_CartonSize(object sender, KeyPressEventArgs e)
        {
            ComboBox tb = sender as ComboBox;
            if (tb == null) return;

            // Cho phép phím điều khiển (Backspace, Delete, mũi tên...)
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            // Nếu không phải số hoặc 'x' thì chặn
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 'x' && e.KeyChar != 'X')
            {
                e.Handled = true;
                return;
            }

            string text = tb.Text;
            int selectionStart = tb.SelectionStart; // vị trí con trỏ

            // Nếu ký tự nhập là 'x'
            if (e.KeyChar == 'x' || e.KeyChar == 'X')
            {
                // 1. Không cho 'x' ở đầu
                if (selectionStart == 0)
                {
                    e.Handled = true;
                    return;
                }

                // 2. Không cho nhập 2 'x' liên tiếp
                if (selectionStart > 0 &&
                    (text[selectionStart - 1] == 'x' || text[selectionStart - 1] == 'X'))
                {
                    e.Handled = true;
                    return;
                }

                // 3. Không cho phép có nhiều hơn 2 'x' trong chuỗi
                int countX = text.Count(c => c == 'x' || c == 'X');
                if (countX >= 2)
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void dataGV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // chặn xuống dòng mặc định

                int row = dataGV.CurrentCell.RowIndex;
                int col = dataGV.CurrentCell.ColumnIndex;

                // Nếu chưa phải dòng cuối thì nhảy xuống
                if (row < dataGV.Rows.Count - 1)
                {
                    dataGV.CurrentCell = dataGV.Rows[row + 1].Cells[col];
                    dataGV.BeginEdit(true); // mở chế độ nhập luôn
                }
            }
            else if (e.KeyCode == Keys.F3)
            {
                // Ngăn mặc định xảy ra
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

        }

        private void assignCustomerCarton_btn_Click(object sender, EventArgs e)
        {

            _ =autoCreateCustomverCarton();
        }

        private async Task autoCreateCustomverCarton( bool isAutoPrint = false)
        {
            if (exportCode_cbb.SelectedValue == null) return;

            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return;
            }
            // search_tb.Text = "";
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(200);

            var rows = mOrders_dt.AsEnumerable()
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                .ToList();

            var changedRows = new List<DataRow>();
            // Nhóm theo CustomerCode
            var groups = rows.GroupBy(r => (r.Field<string>("CustomerCode") ?? "").Trim());

            foreach (var grp in groups)
            {
                string customer = grp.Key;
                if (string.IsNullOrEmpty(customer)) continue;

                // Bước 1: gom theo CartonNo
                var cartonMap = new Dictionary<int, List<DataRow>>();
                foreach (var row in grp)
                {
                    if (row.IsNull("CartonNo")) continue;
                    int cartonKey = row.Field<int>("CartonNo");

                    if (!cartonMap.TryGetValue(cartonKey, out var list))
                    {
                        list = new List<DataRow>();
                        cartonMap[cartonKey] = list;
                    }
                    list.Add(row);
                }

                // Bước 2: tách các carton đã có CustomerCarton và chưa có
                var existingCartonNos = new Dictionary<int, string>(); // CartonNo -> CustomerCarton
                foreach (var kv in cartonMap)
                {
                    // Lấy CustomerCarton đầu tiên (nếu có)
                    string existing = kv.Value
                        .Select(r => (r["CustomerCarton"] ?? "").ToString().Trim())
                        .FirstOrDefault(v => !string.IsNullOrEmpty(v));
                    if (!string.IsNullOrEmpty(existing))
                        existingCartonNos[kv.Key] = existing;
                }

                // Bước 3: tạo danh sách CartonNo cần gán mới (chưa có CustomerCarton)
                var distinctKeysToAssign = cartonMap.Keys
                    .Where(k => !existingCartonNos.ContainsKey(k))
                    .OrderBy(k => k)
                    .ToList();

                // Bước 4: lấy danh sách CustomerCarton đã tồn tại (để tránh trùng)
                var usedCustomerCartons = new HashSet<string>(
                    grp.Select(r => (r["CustomerCarton"]?? "").ToString().Trim())
                       .Where(s => !string.IsNullOrEmpty(s))
                );

                // Tìm max index đang có (ví dụ CUST.3 → 3)
                int maxIdx = 0;
                foreach (var val in usedCustomerCartons)
                {
                    if (val.StartsWith(customer + "."))
                    {
                        string part = val.Substring(customer.Length + 1);
                        if (int.TryParse(part, out int num))
                            maxIdx = Math.Max(maxIdx, num);
                    }
                }

                // Bước 5: gán CustomerCarton cho các CartonNo mới
                // Lấy tất cả số đang dùng (đã có)
                var usedNumbers = new HashSet<int>();
                foreach (var val in usedCustomerCartons)
                {
                    if (val.StartsWith(customer + "."))
                    {
                        string part = val.Substring(customer.Length + 1);
                        if (int.TryParse(part, out int num))
                            usedNumbers.Add(num);
                    }
                }

                foreach (var key in distinctKeysToAssign)
                {
                    // Tìm số nhỏ nhất chưa dùng
                    int nextNum = 1;
                    while (usedNumbers.Contains(nextNum))
                        nextNum++;

                    string newCCarton = $"{customer}.{nextNum:D2}";
                    existingCartonNos[key] = newCCarton;
                    usedNumbers.Add(nextNum);
                }

                // Bước 6: gán CustomerCarton cho toàn bộ row
                foreach (var kv in cartonMap)
                {
                    int cartonNo = kv.Key;
                    string custCarton = existingCartonNos.ContainsKey(cartonNo)
                        ? existingCartonNos[cartonNo]
                        : "";

                    foreach (var row in kv.Value)
                    {
                        if ((row["CustomerCarton"] ?? "").ToString() != custCarton)
                        {
                            row["CustomerCarton"] = custCarton;
                            changedRows.Add(row);
                        }
                    }
                }
            }

            await Task.Delay(200);
            fillter_btn_Click(null, null);
            SaveData(changedRows);
            loadingOverlay.Hide();

            if (!checkCartonNo() || !CheckCustomerCartonSequence())
                return;

            if (isAutoPrint)
            {
                if (int.TryParse(cartonNo_tb.Text, out int cartonNo))
                {
                    var customerCarton = "";
                    foreach (var row in rows)
                    {
                        // Lấy CartonNo theo cách an toàn
                        object val = row["CartonNo"];
                        if (val == null || val == DBNull.Value)
                            continue;

                        int cartonNo1;
                        if (!int.TryParse(val.ToString(), out cartonNo1))
                            continue;

                        if (cartonNo1 == cartonNo)
                        {
                            customerCarton = row["CustomerCarton"].ToString();
                            break;
                        }
                    }
                    if(!string.IsNullOrEmpty(customerCarton))
                        _ = PrintPhieuThung(false, customerCarton);
                                        
                    cartonNo_tb.Text = (cartonNo + 1).ToString();
                }
                
            }
        }

        private void fillter_btn_Click(object sender, EventArgs e)
        {
            if (exportCode_cbb.SelectedValue == null) return;
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return;
            }
            // Lấy danh sách CustomerCarton kèm theo các CartonNo
            var customerCartons = mOrders_dt.AsEnumerable()
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId)
                .Select(r => new
                {
                    CustomerCarton = r["CustomerCarton"]?.ToString(),
                    CartonNo = r["CartonNo"]?.ToString()
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.CustomerCarton) && !string.IsNullOrWhiteSpace(x.CartonNo))
                .GroupBy(x => x.CustomerCarton)
                .Select(g => new
                {
                    CustomerCarton = g.Key,
                    CartonNos = string.Join(", ", g.Select(x => x.CartonNo).Distinct().OrderBy(n => n))
                })
                .ToList();

            // Xóa dữ liệu cũ trong carton_GV
            carton_GV.Rows.Clear();

            if (!carton_GV.Columns.Contains("CustomerCarton"))
            {
                carton_GV.Columns.Add("CustomerCarton", "Mã Thùng");
                carton_GV.Columns["CustomerCarton"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                carton_GV.Columns["CustomerCarton"].ReadOnly = true;
            }

            if (!carton_GV.Columns.Contains("CartonNo"))
            {
                carton_GV.Columns.Add("CartonNo", "C.No");
                carton_GV.Columns["CartonNo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                carton_GV.Columns["CartonNo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                carton_GV.Columns["CartonNo"].ReadOnly = true;
            }
            
            // Đổ dữ liệu mới vào
            foreach (var item in customerCartons)
            {
                carton_GV.Rows.Add(item.CustomerCarton, item.CartonNos);
            }

            fillter_btn.Text = "Tổng Số Thùng: " + carton_GV.Rows.Count.ToString();
        }

        private void previewPrint_PT_btn_Click(object sender, EventArgs e)
        {
            _= PrintPhieuThung(true);
        }
        private void print_btn_Click(object sender, EventArgs e)
        {
            _ = PrintPhieuThung(false);
        }

        private async Task PrintPhieuThung(bool isPreview, string customerCarton = "")
        {
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);
            // Giả sử dataGV là DataGridView của bạn
            List<string> selectedCartons = new List<string>();
            if (string.IsNullOrEmpty(customerCarton))
            {
                foreach (DataGridViewCell cell in carton_GV.SelectedCells)
                {
                    // chỉ lấy ô ở cột CustomerCarton
                    if (carton_GV.Columns[cell.ColumnIndex].Name == "CustomerCarton")
                    {
                        if (cell.Value != null && !string.IsNullOrWhiteSpace(cell.Value.ToString()))
                        {
                            selectedCartons.Add(cell.Value.ToString());
                        }
                    }
                }
            }
            else
            {
                selectedCartons.Add(customerCarton);
            }

            if (selectedCartons.Count <= 0)
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
                return;
            }

            if (exportCode_cbb.SelectedValue == null) return;
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return;
            }

            DataTable resultTable = mOrders_dt.Clone();

            var rows = mOrders_dt.AsEnumerable()
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                .ToList();

            foreach (var g in rows)
            {
                resultTable.Rows.Add(g.ItemArray);
            }

            PackingListPrinter printer = new PackingListPrinter(resultTable, "Asiaway AG", "Schwamendingenstrasse 10, 8050 Zürich, Switzerland", selectedCartons);

            // Gọi phương thức in
            if (!isPreview)
            {
                if(!string.IsNullOrEmpty(customerCarton))
                    printer.Print(customerCarton);
                else
                    printer.Print(selectedCartons.Count);
            }
            else
                printer.PrintPreview(this);

            await Task.Delay(200);
            loadingOverlay.Hide();
        }

        private void checkCarton_btn_click(object sender, EventArgs e)
        {

            if (checkCartonNo() && CheckCustomerCartonSequence())
            {
                MessageBox.Show("Data Không Vẫn Đề Gì", "CartonNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private bool CheckCustomerCartonSequence()
        {
            if (exportCode_cbb.SelectedValue == null) return false;
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return false;
            }
            var rows = mOrders_dt.AsEnumerable()
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                .ToList();

            if (rows.Count == 0)
                return true; 

            var groups = rows.GroupBy(r => (r["CustomerCode"] ?? "").ToString().Trim());

            var missingList = new List<string>(); // chứa danh sách lỗi

            foreach (var grp in groups)
            {
                string customer = grp.Key;
                if (string.IsNullOrEmpty(customer)) continue;

                // Lấy danh sách CustomerCarton dạng "Customer.X"
                var numbers = grp.Select(r => (r["CustomerCarton"] ?? "").ToString().Trim())
                                 .Where(v => v.StartsWith(customer + "."))
                                 .Select(v =>
                                 {
                                     string numPart = v.Substring(customer.Length + 1);
                                     return int.TryParse(numPart, out int n) ? n : -1;
                                 })
                                 .Where(n => n > 0)
                                 .Distinct()
                                 .OrderBy(n => n)
                                 .ToList();

                if (numbers.Count == 0) continue;

                // Kiểm tra dãy có liên tục không
                int expected = numbers.First();
                foreach (int actual in numbers)
                {
                    while (expected < actual)
                    {
                        // Phát hiện số bị thiếu
                        missingList.Add($"{customer}: thiếu {expected}");
                        expected++;
                    }
                    expected = actual + 1;
                }
            }

            if (missingList.Count > 0)
            {
                string msg = "⚠️ Phát hiện mã thùng bị nhảy cóc:\n" +
                             string.Join("\n", missingList);
                MessageBox.Show(msg, "Kiểm tra Mã Thùng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }


        private bool checkCartonNo()
        {
            if (exportCode_cbb.SelectedValue == null) return false;
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return false;
            }

            string cartonColName = "CartonNo";
            string customerColName = "Customername";
            string sizeColName = "CartonSize";
            string PCSReal = "PCSReal";
            string NWReal = "NWReal";

            var rows = mOrders_dt.AsEnumerable()
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                .ToList();

            List<int> cartonNumbers = new List<int>();
            foreach (var row in rows)
            {
                if (int.TryParse(row[cartonColName]?.ToString(), out int carton))
                {
                    string customerCarton = row["CustomerCarton"]?.ToString();
                    if (string.IsNullOrEmpty(customerCarton))
                    {
                        MessageBox.Show(
                            $"CartonNo '{carton}' có nhưng CustomerCarton bị trống!",
                            "Thiếu CustomerCarton",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return false;
                    }

                    cartonNumbers.Add(carton);
                }
            }

            if (cartonNumbers.Count == 0) return false;

            // Lấy min và max
            int min = 1;
            int max = cartonNumbers.Max();

            // Tạo set duy nhất
            HashSet<int> uniqueNumbers = new HashSet<int>(cartonNumbers);

            // Kiểm tra số liên tục
            for (int i = min; i <= max; i++)
            {
                if (!uniqueNumbers.Contains(i))
                {
                    MessageBox.Show($"CartonNo bị thiếu số {i}!", "CartonNo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            foreach (DataRow row in rows)
            {
                int pcs = 0;
                decimal nw = 0;

                string carton = row[cartonColName]?.ToString();
                string customer = row[customerColName]?.ToString();
                string cartonSize = row[sizeColName]?.ToString();               

                int.TryParse(row["PCSReal"]?.ToString(), out pcs);
                decimal.TryParse(row["NWReal"]?.ToString(), out nw);

                if (pcs > 0 || nw > 0) {
                    if (string.IsNullOrEmpty(carton))
                    {
                        MessageBox.Show($"Khách Hàng: {customer}\n CartonNo: Có Ô Chưa Nhập Số Liệu", "Thiếu Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    if (string.IsNullOrEmpty(cartonSize))
                    {
                        MessageBox.Show($"Khách Hàng: {customer}\n Carton Size: Có Ô Chưa Nhập Số Liệu", "Thiếu Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    
                    if(!Regex.IsMatch(cartonSize, @"^\d+x\d+x\d+$", RegexOptions.IgnoreCase))
                    {
                        MessageBox.Show($"Thùng Số: {carton}\n Carton Size:{cartonSize} sai định dạng", "Thiếu Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                }
            }


            var cartonDict = new Dictionary<string, HashSet<string>>();
            List<string> errorCartons = new List<string>();

            foreach (DataRow row in rows)
            {
                string carton = row[cartonColName]?.ToString();
                string customer = row[customerColName]?.ToString();

                if (string.IsNullOrEmpty(carton) || string.IsNullOrEmpty(customer))
                    continue;

                if (!cartonDict.ContainsKey(carton))
                    cartonDict[carton] = new HashSet<string>();

                cartonDict[carton].Add(customer);

                if (cartonDict[carton].Count > 1)
                {
                    if (!errorCartons.Contains(carton))
                        errorCartons.Add(carton);
                }
            }

            // Sau khi duyệt hết mới báo
            if (errorCartons.Count > 0)
            {
                string cartons = string.Join(", ", errorCartons);
                MessageBox.Show(
                    $"Các CartonNo sau chứa nhiều Customer khác nhau: {cartons}",
                    "CartonNo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            Dictionary<string, HashSet<string>> cartonSizeDict = new Dictionary<string, HashSet<string>>();
            errorCartons.Clear();

            foreach (DataRow row in rows)
            {
                string carton = row[cartonColName]?.ToString();
                string size = row[sizeColName]?.ToString();

                if (string.IsNullOrEmpty(carton) || string.IsNullOrEmpty(size))
                    continue;

                if (!cartonSizeDict.ContainsKey(carton))
                    cartonSizeDict[carton] = new HashSet<string>();

                cartonSizeDict[carton].Add(size);

                if (cartonSizeDict[carton].Count > 1)
                {
                    if (!errorCartons.Contains(carton))
                        errorCartons.Add(carton);
                }
            }

            if (errorCartons.Count > 0)
            {
                string cartons = string.Join(", ", errorCartons);
                MessageBox.Show(
                    $"Các CartonNo sau có nhiều CartonSize khác nhau: {cartons}",
                    "CartonSize",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            Dictionary<string, HashSet<string>> cartonCustomerCartonDict = new Dictionary<string, HashSet<string>>();
            errorCartons.Clear();

            foreach (DataRow row in rows)
            {
                string carton = row[cartonColName]?.ToString();
                string customerCarton = row["CustomerCarton"]?.ToString();

                if (string.IsNullOrEmpty(carton) || string.IsNullOrEmpty(customerCarton))
                    continue;

                if (!cartonCustomerCartonDict.ContainsKey(carton))
                    cartonCustomerCartonDict[carton] = new HashSet<string>();

                cartonCustomerCartonDict[carton].Add(customerCarton);

                if (cartonCustomerCartonDict[carton].Count > 1)
                {
                    if (!errorCartons.Contains(carton))
                        errorCartons.Add(carton);
                }
            }

            if (errorCartons.Count > 0)
            {
                string cartons = string.Join(", ", errorCartons);
                MessageBox.Show(
                    $"Các CartonNo sau có nhiều Mã Thùng khác nhau: {cartons}",
                    "CustomerCarton",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }


            return true;
        }

        public async void SaveData(List<DataRow> rows)
        {
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return;
            }

            var orders = new List<(int, int?, decimal?, int?, string, string)>();
            var logsSuccess = new List<(int, int, string, int?, decimal?, int?, string, string)>();
            var logsFail = new List<(int, int, string, int?, decimal?, int?, string, string)>();
            try
            {                
                foreach (DataRow row in rows)
                {
                    int orderId = Convert.ToInt32(row["OrderId"]);

                    int? pcsReal = int.TryParse(row["PCSReal"]?.ToString(), out int pcs) ? pcs : (int?)null;
                    decimal? nwReal = decimal.TryParse(row["NWReal"]?.ToString(), out decimal nw) ? nw : (decimal?)null;
                    int? cartonNo = int.TryParse(row["CartonNo"]?.ToString(), out int carton) ? carton : (int?)null;
                    string cartonSize = row["CartonSize"]?.ToString();
                    string customerCarton = row["CustomerCarton"]?.ToString();

                    orders.Add((orderId, pcsReal, nwReal, cartonNo, cartonSize, customerCarton));
                    logsSuccess.Add((exportCodeId, orderId, "customerCarton: Update Thành Công", pcsReal, nwReal, cartonNo, cartonSize, customerCarton));
                    logsFail.Add((exportCodeId, orderId, "customerCarton: Update Thất Bại", pcsReal, nwReal, cartonNo, cartonSize, customerCarton));
                }

                bool isScussess = await SQLManager.Instance.UpdatePackOrdersBulkAsync(orders);
                if (isScussess)
                {
                    _ = SQLManager.Instance.InsertOrderPackingLogBulkAsync(logsSuccess);
                    status_lb.Text = "Thành công.";
                    status_lb.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    _ = SQLManager.Instance.InsertOrderPackingLogBulkAsync(logsFail);
                    MessageBox.Show("Cập nhật thất bại!");
                }

            }
            catch (Exception ex)
            {
                for (int i = 0; i < logsFail.Count; i++)
                {
                    var log = logsFail[i];
                    logsFail[i] = (
                        log.Item1,log.Item2,log.Item3 + " Exception: " + ex.Message,
                        log.Item4,log.Item5,log.Item6,log.Item7, log.Item8
                    );
                }
                _ = SQLManager.Instance.InsertOrderPackingLogBulkAsync(logsFail);
                MessageBox.Show("Cập nhật thất bại!");
            }
        }

        private void closeEdit_btn_Click(object sender, EventArgs e)
        {
            setUIReadOnly(true);
        }
        private void Edit_btn_Click(object sender, EventArgs e)
        {
            setUIReadOnly(false);
        }

        private void setUIReadOnly(bool isReadOnly)
        {
            if (exportCode_cbb.SelectedItem != null)
            {
                DataRowView dataR = (DataRowView)exportCode_cbb.SelectedItem;

                string staff = dataR["InputByName_NoSign"].ToString();
                if (UserManager.Instance.fullName_NoSign.CompareTo(staff) != 0)
                {
                    edit_btn.Visible = false;
                    readOnly_btn.Visible = false;
                    rightInfo_gb.Visible = false;
                    InPhieuGiaoHang_btn.Visible = true;
                    previewPrint_PGH_btn.Visible = true;
                    previewPrint_PT_btn.Visible = true;
                    return;
                }
            }
            rightInfo_gb.Visible = !isReadOnly;
            edit_btn.Visible = isReadOnly;
            readOnly_btn.Visible = !isReadOnly;
            phieuCanHang_gb.Visible = isReadOnly;
            phieuGiaoHang_gb.Visible = isReadOnly;
        }
      
        private void DataGV_SelectionChanged(object sender, EventArgs e)
        {
            status_lb.Text = "";
            if (dataGV.CurrentRow == null) return;

            if (exportCode_cbb.SelectedValue == null) return;
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return;
            }

            var rows = mOrders_dt.AsEnumerable()
                .Where(r => r.RowState != DataRowState.Deleted)
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                .ToList();

            var currentRow = dataGV.CurrentRow;

            int orderID = Convert.ToInt32(currentRow.Cells["OrderID"].Value);
            mOrderPackingLog_dv.RowFilter = $"OrderID = {orderID}";
            mOrderPackingLog_dv.Sort = "LogID DESC";

            object val = currentRow.Cells["CartonNo"].Value;
            var pcsVal = currentRow.Cells["PCSOther"].Value;
            var nwVal = currentRow.Cells["NWOther"].Value;

            int PCSOther = 0;
            double NWOther = 0;            
            int.TryParse(pcsVal?.ToString() ?? "0", out PCSOther);
            double.TryParse(nwVal?.ToString() ?? "0", out NWOther);

            if (!int.TryParse(val?.ToString(), out int result))
                return;

            int CustomerID = Convert.ToInt32(currentRow.Cells["CustomerID"].Value);            
            int CartonNo = val != DBNull.Value && val != null ? Convert.ToInt32(val) : -1;
            decimal sumNW_cus = 0;
            int sumPCS_cus = 0;
            decimal sumNW_cus_tt = 0;
            int sumPCS_cus_tt = 0;
            decimal sumNW = 0;
            int sumPCS = 0;
            decimal sumNW_tt = 0;
            int sumPCS_tt = 0;
            decimal sumNW_cn = 0;
            int sumPCS_cn = 0;
            decimal sumNW_tt_cn = 0;
            int sumPCS_tt_cn = 0;
            int countCartonNo = 0;
            // Duyệt toàn bộ các dòng trong DataGridView
            foreach (DataRow row in rows)
            {

                decimal nw = 0;
                int pcs = 0;
                decimal nwtt = 0;
                int pcstt = 0;
                
                decimal.TryParse(row["NWOther"]?.ToString(), out nw);
                int.TryParse(row["PCSOther"]?.ToString(), out pcs);
                decimal.TryParse(row["NWReal"]?.ToString(), out nwtt);
                int.TryParse(row["PCSReal"]?.ToString(), out pcstt);
                int cus = Convert.ToInt32(row["CustomerID"]);

                object val1 = row["CartonNo"];
                int cn = val1 != DBNull.Value && val1 != null ? Convert.ToInt32(val1) : -1;
                if (cus == CustomerID)
                {
                    sumNW_cus += nw;
                    sumPCS_cus += pcs;
                    sumNW_cus_tt += nwtt;
                    sumPCS_cus_tt += pcstt;
                }
                if(CartonNo >= 0 && cn == CartonNo)
                {
                    sumNW_cn += nw;
                    sumPCS_cn += pcs;
                    sumNW_tt_cn += nwtt;
                    sumPCS_tt_cn += pcstt;
                    countCartonNo ++;
                }
                sumNW += nw;
                sumPCS += pcs;
                sumNW_tt += nwtt;
                sumPCS_tt += pcstt;
            }
            tongdathang_label.Text = "[" + sumPCS + " pcs, " + sumNW + "kg" + "]";
            tongdongthung_lable.Text = "[" + sumPCS_tt + " pcs, " + sumNW_tt + " kg" + "]";
            dathang_cus_lable.Text = "[" + sumPCS_cus + " pcs, " + sumNW_cus + " kg" + "]";
            dongthung_cus_lable.Text = "[" + sumPCS_cus_tt + " pcs, " + sumNW_cus_tt + " kg" + "]";
            if (CartonNo >= 0)
            {
                dathang_cn_name_lable.Text = "Thùng " + CartonNo + "[" + countCartonNo + "]" + " Đặt:";
                dathang_cn_lable.Text = "[" + sumPCS_cn + " pcs, " + sumNW_cn + " kg" + "]";
                dongthung_cn_name_lable.Text = "Thùng " + CartonNo + " Nhận:";
                dongthung_cn_lable.Text = "[" + sumPCS_tt_cn + " pcs, " + sumNW_tt_cn + " kg" + "]";
            }
            else
            {
                dathang_cn_lable.Text = "[null]";
                dongthung_cn_lable.Text = "[null]";
            }
        }

        private void search_txt_TextChanged(object sender, EventArgs e)
        {
            string selectedExportCode = exportCode_cbb.Text;

            if (string.IsNullOrEmpty(selectedExportCode))
                return;

            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower()).Replace("'", "''"); // tránh lỗi cú pháp '

            DataView currentView = dataGV.DataSource is DataView dv ? dv : new DataView(mOrders_dt);
            string filter = "";
            if (!string.IsNullOrEmpty(keyword))
                filter += $"[Search_NoSign] LIKE '%{keyword.Replace("'", "''")}%'";  // escape dấu nháy đơn

            if (!string.IsNullOrEmpty(selectedExportCode))
            {
                if (filter.Length > 0) filter += " AND ";
                filter += $"ExportCode = '{selectedExportCode.Replace("'", "''")}'";
            }
            currentView.RowFilter = filter;
            dataGV.DataSource = currentView;
        }

        private void cartonSize_cbb_TextUpdate(object sender, EventArgs e)
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

                string typed = cartonSize_cbb.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mCartonSize_dt.AsEnumerable()
                    .Where(r => r["CartonSize"].ToString()
                    .Contains(plain));

                DataTable temp;
                if (filtered.Any())
                {
                    temp = filtered.CopyToDataTable();
                    cartonSize_cbb.DataSource = temp;
                    cartonSize_cbb.DisplayMember = "CartonSize";
                    cartonSize_cbb.ValueMember = "CartonID";

                    cartonSize_cbb.DroppedDown = true;
                    cartonSize_cbb.Text = typed;
                    cartonSize_cbb.SelectionStart = typed.Length;
                    cartonSize_cbb.SelectionLength = 0;
                }
                else
                {
                    temp = mCartonSize_dt.Clone();                
                    cartonSize_cbb.DroppedDown = false;
                    cartonSize_cbb.DataSource = temp;
                    cartonSize_cbb.Text = typed;
                    cartonSize_cbb.SelectionStart = typed.Length;
                }

                // Gán lại DataSource
                
            }
            catch { }
        }

        private async void OrderPackingList_KeyDown(object sender, KeyEventArgs e)
        {
            DataRowView dataR = (DataRowView)exportCode_cbb.SelectedItem;
            string staff = dataR["InputByName_NoSign"].ToString();

            if (e.KeyCode == Keys.F1)
            {
                dataGV.EndEdit();
                await Task.Delay(100);
                search_tb.Focus();
            }
            else if (e.KeyCode == Keys.F4)
            {
                dataGV.EndEdit();
                await Task.Delay(100);
                checkCarton_btn_click(null, null);
            }
            else if(e.KeyCode == Keys.F5)
            {
                if (mCurrentExportID <= 0)
                {
                    return;
                }
                SQLStore.Instance.removeOrders(mCurrentExportID);
                ShowData();
            }
            else if (edit_btn.Visible == false && UserManager.Instance.fullName_NoSign.CompareTo(staff) == 0)
            {
                if (e.KeyCode == Keys.F2)
                {
                    dataGV.EndEdit();
                    await Task.Delay(100);
                    _ = autoCreateCustomverCarton(true);
                }
                else if (e.KeyCode == Keys.F3)
                {
                    dataGV.EndEdit();
                    await Task.Delay(100);
                    Chiadon_btn_Click(null, null);
                }
                else if (e.KeyCode == Keys.Add)
                {
                    if (int.TryParse(cartonNo_tb.Text, out int cartonNo1))
                    {
                        cartonNo_tb.Text = (cartonNo1 + 1).ToString();
                    }
                }
                else if (e.KeyCode == Keys.Subtract)
                {
                    if (int.TryParse(cartonNo_tb.Text, out int cartonNo1))
                    {
                        if (cartonNo1 - 1 > 0)
                            cartonNo_tb.Text = (cartonNo1 - 1).ToString();
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    Control ctrl = this.ActiveControl;

                    if (ctrl is TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }
                    DeleteOrder();
                }
            }
        }

        private void previewPrint_PGH_btn_Click(object sender, EventArgs e)
        {
            InPhieuGiaoHang(true);
        }

        private void InPhieuGiaoHang_btn_Click(object sender, EventArgs e)
        {
            InPhieuGiaoHang(false);
        }

        private async void InPhieuGiaoHang(bool preview = false)
        {
            

            string packerName = Interaction.InputBox(
                "Ghi Chú:", // Prompt
                "Thông tin Ghi chú", // Title
                " " // Default value
            );

            if (packerName == "")
            {
                return; 
            }


            if (exportCode_cbb.SelectedValue == null) return;
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return;
            }

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            // Tạo DataTable kết quả
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("CartonSize", typeof(string));
            resultTable.Columns.Add("Count", typeof(int));

            var rows = mOrders_dt.AsEnumerable()
                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId) // chỉ lấy ExportCode cần xử lý
                .ToList();
            // Group theo ProductPackingName
            var groups = rows
                        .Where(r => r["CartonSize"] != null && r["CartonSize"].ToString().Trim() != "" &&
                                    r["CartonNo"] != null && r["CartonNo"].ToString().Trim() != "")
                        .GroupBy(r => r["CartonSize"].ToString().Trim())
                        .Select(g => new
                        {
                            CartonSize = g.Key,
                            Count = g.Select(r => r["CartonNo"].ToString().Trim()).Distinct().Count()
                        })
                        .ToList();

            foreach (var g in groups)
            {
                resultTable.Rows.Add(g.CartonSize, g.Count);
            }

            DataRowView exportCodeItem = (DataRowView)exportCode_cbb.SelectedItem;

            string exportCode = exportCodeItem["ExportCode"].ToString();
            DateTime exportDate = Convert.ToDateTime(exportCodeItem["ExportDate"]);

            DeliveryPrinter deliveryPrinter = new DeliveryPrinter(resultTable, exportCode, exportDate, packerName);
            if (!preview)
                deliveryPrinter.PrintDirect();
            else
                deliveryPrinter.PrintPreview(this);

            await Task.Delay(200);
            loadingOverlay.Hide();
        }


        private void PhieuCanHang_btn_Click(object sender, EventArgs e)
        {
            InPhieuCanHang(false);
        }

        private void PhieuCanHang_preview_btn_Click(object sender, EventArgs e)
        {
            InPhieuCanHang(true);
        }

        private async void InPhieuCanHang(bool preview = false)
        {
            if (exportCode_cbb.SelectedValue == null) return;
            if (!int.TryParse(exportCode_cbb.SelectedValue.ToString(), out int exportCodeId))
            {
                return;
            }

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            var list = mOrders_dt.AsEnumerable()
                                .Where(r => r.Field<int>("ExportCodeID") == exportCodeId)
                                .Where(r => r.Field<int?>("CartonNo").HasValue)
                                .GroupBy(r => r.Field<int?>("CartonNo"))
                                .Select(g => g.First())
                                .Select(r => new
                                {
                                    CartonNo = r.Field<int?>("CartonNo"),
                                    CustomerName = r.Field<string>("CustomerName"),
                                    CartonSize = r.Field<string>("CartonSize"),
                                    GEL = 5
                                })
                                .OrderBy(x => x.CartonNo)
                                .ToList();

            DataTable dt = list.ToDataTable();

            DataRowView exportCodeItem = (DataRowView)exportCode_cbb.SelectedItem;

            string exportCode = exportCodeItem["ExportCode"].ToString();
            DateTime exportDate = Convert.ToDateTime(exportCodeItem["ExportDate"]);

            PhieuCanHangPrinter deliveryPrinter = new PhieuCanHangPrinter(dt, exportCode, exportDate);
            if (!preview)
                deliveryPrinter.PrintDirect();
            else
                deliveryPrinter.PrintPreview(this);

            await Task.Delay(200);
            loadingOverlay.Hide();
        }

        private void Carton_GV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (carton_GV.CurrentRow == null)
                return; // Ignore header

            // Lấy giá trị cell
            var value = carton_GV.CurrentRow.Cells["CustomerCarton"].Value;

            _ = PrintPhieuThung(false, value.ToString());
        }

        private async void InTem(bool isPreview)
        {
            if (dataGV.CurrentRow == null) return;

            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            var currentRow = dataGV.CurrentRow;
            int pPackingID = Convert.ToInt32(currentRow.Cells["ProductPackingID"].Value);

            var rows = mOrders_dt.AsEnumerable().Where(r => r.Field<int>("ProductPackingID") == pPackingID &&
                !string.IsNullOrWhiteSpace(r.Field<string>("LOTCodeComplete"))).ToList();

            if (rows.Count <= 0)
            {
                MessageBox.Show("Chưa có Mã LOTCodeComplete");

                await Task.Delay(200);
                loadingOverlay.Hide();

                return;
            }

            PickDateDialog dlg = new PickDateDialog();

            if (dlg.ShowDialog(this) != DialogResult.OK)
            {
                loadingOverlay.Hide();
                return;   // Người dùng bấm Cancel → không in
            }

            string packedDate = dlg.SelectedDate.ToString("dd/MM/yyyy");

            var row = rows[0];
            int SKU = Convert.ToInt32(row["SKU"]);
            string LOTCodeComplete = Convert.ToString(row["LOTCodeComplete"]);
            DataRow[] SKURows = mProductSKU_dt.Select($"SKU = {SKU}");
            DataRow[] packingRows = mProductPacking_dt.Select($"ProductPackingID = {pPackingID}");
            if (SKURows.Length <= 0 || packingRows.Length <= 0) return;

            string packing = row["packing"].ToString();
            int Amount = Convert.ToInt32(row["Amount"]);

            var printer = new LabelPrinter(
                SKURows[0]["ProductNameEN"].ToString(),
                SKURows[0]["BotanicalName"].ToString(),
                $"{Amount} {packing}",
                packingRows[0]["BarCodeEAN13"].ToString(),
                SKU.ToString(),
                LOTCodeComplete,
                packedDate);
            if (isPreview)
                printer.PrintPreview(this);
            else
                printer.Print();

            await Task.Delay(200);
            loadingOverlay.Hide();
        }
        private void Tem_preview_btn_Click(object sender, EventArgs e)
        {
            InTem(true);
        }


        private void InTem_btn_Click(object sender, EventArgs e)
        {
            InTem(false);
        }
    }
}
