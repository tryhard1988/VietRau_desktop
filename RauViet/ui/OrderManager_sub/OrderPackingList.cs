using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Mysqlx.Session;
using MySqlX.XDevAPI.Common;
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
    public partial class OrderPackingList : Form
    {
        private LoadingOverlay loadingOverlay;
        DataTable mExportCode_dt, mOrders_dt, mCartonSize_dt;
        private int sortMode = 0;
        private Timer debounceTimer = new Timer { Interval = 300 };

        public OrderPackingList()
        {
            InitializeComponent();

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            cartonNo_tb.TabIndex = countTab++; cartonNo_tb.TabStop = true;
            cartonSize_cbb.TabIndex = countTab++; cartonSize_cbb.TabStop = true;

            setUIReadOnly(true);

            dataGV.ScrollBars = ScrollBars.Both;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGV.MultiSelect = true;

            carton_GV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            carton_GV.MultiSelect = true;
           // carton_GV.ColumnHeadersVisible = false;
            carton_GV.RowHeadersVisible = false;
            carton_GV.AllowUserToAddRows = false;
            carton_GV.AllowUserToResizeRows = false;

            status_lb.Text = "";

           // cartonSize_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            checkCarton_btn.Click += checkCarton_btn_click;
            fillter_btn.Click += fillter_btn_Click;
           // LuuThayDoiBtn.Click += saveBtn_Click;
            print_btn.Click += print_btn_Click;
            previewPrint_PT_btn.Click += previewPrint_PT_btn_Click;
            InPhieuGiaoHang_btn.Click += InPhieuGiaoHang_btn_Click;
            previewPrint_PGH_btn.Click += previewPrint_PGH_btn_Click;
            assignCartonNoBtn.Click += AssignCartonNoBtn_Click;
            assignCartonSize_btn.Click += assignCartonSizeBtn_Click;
            assignCustomerCarton_btn.Click += assignCustomerCarton_btn_Click;
           // autoFillCartonSize_btn.Click += btnFillCartonSize_Click;
         //   dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            dataGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGV_EditingControlShowing);

            carton_GV.SelectionChanged += carton_GV_SelectionChanged;

            dataGV.DataBindingComplete += dataGV_DataBindingComplete;
            dataGV.CellFormatting += dataGV_CellFormatting;
            dataGV.CellEndEdit += dataGV_CellValueChanged;
            dataGV.KeyDown += dataGV_KeyDown;
            dataGV.CellBeginEdit += dataGV_CellBeginEdit;
            dataGV.SelectionChanged += DataGV_SelectionChanged;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += closeEdit_btn_Click;
            chiadon_btn.Click += Chiadon_btn_Click;
            delOrder_btn.Click += DelOrder_btn_Click;
            cartonNo_tang1_btn.Click += CartonNo_tang1_btn_Click;

            cartonNo_tb.KeyPress += Tb_KeyPress_OnlyNumber1;
            exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;

           // ToolTipHelper.SetToolTip(autoFillCartonSize_btn, "Tự động tìm ô có cùng Carton.No và \nđã có Carton Size\nSau đó thêm vào các ô chưa có Carton Size");
           // ToolTipHelper.SetToolTip(autoEditCartonNo_btn, "Tự động chỉnh sửa lại Carton.No \nNếu thứ tự không đúng");
            ToolTipHelper.SetToolTip(assignCustomerCarton_btn, "Tự động dựa vào\nCarton.No và Khách Hàng \nĐể tạo mã thùng");
            ToolTipHelper.SetToolTip(chiadon_btn, "Dùng khi 1 đơn hàng cần chia ra 2 thùng \nthì cần tạo ra thêm 1 mã đơn hàng mới\ntừ mã hiện tại");
            ToolTipHelper.SetToolTip(checkCarton_btn, "Chỉ Kiểm tra dữ liệu nhập vào có đúng không \nkhông thay đổi gì cả");

            search_tb.TextChanged += search_txt_TextChanged;
            debounceTimer.Tick += DebounceTimer_Tick;
            cartonSize_cbb.KeyPress += CBB_KeyPress_CartonSize;
            cartonSize_cbb.TextUpdate += cartonSize_cbb_TextUpdate;

            cartonSize_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //cartonSize_cbb.AutoCompleteSource = AutoCompleteSource.ListItems;
        }
        private void previewPrint_PGH_btn_Click(object sender, EventArgs e)
        {
            InPhieuGiaoHang(true);
        }

        private void InPhieuGiaoHang_btn_Click(object sender, EventArgs e)
        {
            InPhieuGiaoHang(false);
        }

        private void InPhieuGiaoHang(bool preview = false)
        {
            if (exportCode_cbb.SelectedItem == null) return;
            // Tạo DataTable kết quả
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("CartonSize", typeof(string));
            resultTable.Columns.Add("Count", typeof(int));

            // Group theo ProductPackingName
            var groups = dataGV.Rows.Cast<DataGridViewRow>().Where(r =>
                                                            !r.IsNewRow &&
                                                            r.Cells["CartonSize"].Value != null &&
                                                            r.Cells["CartonSize"].Value.ToString().Trim() != "" &&
                                                            r.Cells["CartonNo"].Value != null &&
                                                            r.Cells["CartonNo"].Value.ToString().Trim() != "")
                                                        .GroupBy(r => r.Cells["CartonSize"].Value.ToString())
                                                        .Select(g => new
                                                        {
                                                            CartonSize = g.Key,
                                                            Count = g
                                                                .Select(r => r.Cells["CartonNo"].Value.ToString())
                                                                .Distinct()
                                                                .Count()
                                                        });

            foreach (var g in groups)
            {
                resultTable.Rows.Add(g.CartonSize, g.Count);
            }

            DataRowView exportCodeItem = (DataRowView)exportCode_cbb.SelectedItem;

            string exportCode = exportCodeItem["ExportCode"].ToString();
            DateTime exportDate = Convert.ToDateTime(exportCodeItem["ExportDate"]);

            DeliveryPrinter deliveryPrinter = new DeliveryPrinter(resultTable, exportCode, exportDate);
            if(!preview)
                deliveryPrinter.PrintDirect();
            else
                deliveryPrinter.PrintPreview();
        }

        private void carton_GV_SelectionChanged(object sender, EventArgs e)
        {
            // Nếu không muốn reset liên tục thì clear selection trước
            dataGV.ClearSelection();

            // Lấy tất cả các ProductPackingName đang được chọn bên carton_GV
            var selectedNames = new HashSet<string>();
            foreach (DataGridViewRow row in carton_GV.SelectedRows)
            {
                if (row.Cells["CustomerCarton"].Value != null)
                {
                    selectedNames.Add(row.Cells["CustomerCarton"].Value.ToString());
                }
            }

            // Duyệt dataGV, nếu ProductPackingName có trong list thì select
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                var name = row.Cells["CustomerCarton"].Value?.ToString();
                if (name != null && selectedNames.Contains(name))
                {
                    row.Selected = true;
                }
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
                var cartonSizeTask = SQLStore.Instance.GetCartonSize();
                var ordersPackingTask = SQLStore.Instance.getOrdersAsync();
                string[] keepColumns = { "ExportCodeID", "ExportCode", "ExportDate", "InputByName_NoSign" };
                var parameters = new Dictionary<string, object> { { "Complete", false } };
                var exportCodeTask = SQLStore.Instance.getExportCodesAsync(keepColumns, parameters);

                await Task.WhenAll(ordersPackingTask, exportCodeTask, cartonSizeTask);

                mExportCode_dt = exportCodeTask.Result;
                mOrders_dt = ordersPackingTask.Result;
                mCartonSize_dt = cartonSizeTask.Result;

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

                int count = 0;
                mOrders_dt.Columns["OrderId"].SetOrdinal(count++);
                mOrders_dt.Columns["Customername"].SetOrdinal(count++);
                mOrders_dt.Columns["ProductNameVN"].SetOrdinal(count++);
                mOrders_dt.Columns["CustomerCarton"].SetOrdinal(count++);
                mOrders_dt.Columns["CartonNo"].SetOrdinal(count++);
                mOrders_dt.Columns["CartonSize"].SetOrdinal(count++);
                mOrders_dt.Columns["PCSReal"].SetOrdinal(count++);
                mOrders_dt.Columns["NWReal"].SetOrdinal(count++);
                mOrders_dt.Columns["PCSOther"].SetOrdinal(count++);
                mOrders_dt.Columns["NWOther"].SetOrdinal(count++);
                

                dataGV.ReadOnly = false;
                dataGV.Columns["CartonNo"].ReadOnly = false;
                dataGV.Columns["PCSReal"].ReadOnly = false;
                dataGV.Columns["CartonSize"].ReadOnly = false;
                dataGV.Columns["NWReal"].ReadOnly = true;
                dataGV.Columns["Priority"].ReadOnly = true;
                dataGV.Columns["CustomerName"].ReadOnly = true;
                dataGV.Columns["ProductNameVN"].ReadOnly = true;
                dataGV.Columns["CustomerCarton"].ReadOnly = true;

                dataGV.Columns["OrderId"].HeaderText = "ID";
                dataGV.Columns["PCSReal"].HeaderText = "PCS\nThực Tế";
                dataGV.Columns["NWReal"].HeaderText = "NW\nThực Tế";
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

                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";

                if (mExportCode_dt.Rows.Count > 0)
                {
                    int maxID = Convert.ToInt32(mExportCode_dt.AsEnumerable()
                                   .Max(r => r.Field<int>("ExportCodeID")));
                    exportCode_cbb.SelectedValue = maxID;
                }


                cartonSize_cbb.DataSource = mCartonSize_dt;
                cartonSize_cbb.DisplayMember = "CartonSize";
                cartonSize_cbb.ValueMember = "CartonID";

                fillter_btn_Click(null, null);
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                await Task.Delay(200);
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
            try
            {
                if (dataGV.CurrentCell == null) return;
                
                // Lấy dòng được chọn cuối cùng (theo thứ tự hiển thị)
                DataGridViewRow lastSelectedRow = dataGV.Rows[dataGV.CurrentCell.RowIndex];

                // Ví dụ: lấy giá trị trong cột ExportCodeID
                int orderId = Convert.ToInt32(lastSelectedRow.Cells["OrderId"].Value);

                int newId = await SQLManager.Instance.CloneOrder_SplitHalfAsync(orderId);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi chia đơn: {ex.Message}");
                MessageBox.Show($"Lỗi khi chia đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DelOrder_btn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Xóa Đơn Đang Chọn", "Xóa",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question
                                       );

            if (dialogResult == DialogResult.No) return;

            try
            {
                if (dataGV.CurrentCell == null) return;

                // Lấy dòng được chọn cuối cùng (theo thứ tự hiển thị)
                DataGridViewRow lastSelectedRow = dataGV.Rows[dataGV.CurrentCell.RowIndex];

                // Ví dụ: lấy giá trị trong cột ExportCodeID
                int id = Convert.ToInt32(lastSelectedRow.Cells["OrderId"].Value);

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
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi Xóa Đơn: {ex.Message}");
                MessageBox.Show($"Lỗi khi Xóa Đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportCode_search_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mOrders_dt == null || mOrders_dt.Rows.Count == 0)
                return;

            string selectedExportCode = exportCode_cbb.Text;

            if (!string.IsNullOrEmpty(selectedExportCode))
            {
                // Tạo DataView để filter
                DataView dv = new DataView(mOrders_dt);
                dv.RowFilter = $"ExportCode = '{selectedExportCode}'";

                // Gán lại cho DataGridView
                dataGV.DataSource = dv;                
            }
            else
            {
                // Nếu chưa chọn gì thì hiển thị toàn bộ
                dataGV.DataSource = mOrders_dt;
            }

            setUIReadOnly(true);
            fillter_btn_Click(null, null);
        }

        private void CartonNo_tang1_btn_Click(object sender, EventArgs e)
        {
            if (int.TryParse(cartonNo_tb.Text, out int cartonNo1))
            {
                cartonNo_tb.Text = (cartonNo1 + 1).ToString();
            }
        }

        private async void dataGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var row = dataGV.Rows[e.RowIndex];
                    var columnName = dataGV.Columns[e.ColumnIndex].Name;

                    if (columnName == "CustomerCarton")
                    {
                        return;
                    }

                    if (columnName == "PCSReal")
                    {
                        UpdateNWReal(row);
                    }

                    if (columnName == "CartonNo")
                    {
                        row.Cells["CustomerCarton"].Value = DBNull.Value;
                    }

                    if (columnName == "PCSReal" || columnName == "NWReal")
                    {
                        if (!int.TryParse(row.Cells["PCSReal"].Value.ToString(), out int PCSReal) ||
                            !decimal.TryParse(row.Cells["NWReal"].Value.ToString(), out decimal NWReal))
                        {
                            row.Cells["CartonNo"].Value = DBNull.Value;
                            row.Cells["CartonSize"].Value = DBNull.Value;
                            row.Cells["CustomerCarton"].Value = DBNull.Value;
                        }
                        else if (int.TryParse(cartonNo_tb.Text, out int cartonNo1))
                        {
                            if (int.TryParse(row.Cells["CartonNo"].Value.ToString(), out int cartonNo2)) {
                                if (cartonNo2 != cartonNo1)
                                    row.Cells["CustomerCarton"].Value = DBNull.Value;
                            }
                            else
                                row.Cells["CustomerCarton"].Value = DBNull.Value;

                            row.Cells["CartonNo"].Value = cartonNo1;
                            row.Cells["CartonSize"].Value = cartonSize_cbb.Text;
                            
                            if (cartonNoTuTang_cb.Checked)
                                cartonNo_tb.Text = (cartonNo1 + 1).ToString();
                        }
                    }

                    int orderId = Convert.ToInt32(row.Cells["OrderId"].Value);
                    int? pcsReal = int.TryParse(row.Cells["PCSReal"]?.Value?.ToString(), out int pcs) ? pcs : (int?)null;
                    decimal? nwReal = decimal.TryParse(row.Cells["NWReal"]?.Value?.ToString(), out decimal nw) ? nw : (decimal?)null;
                    int? cartonNo = int.TryParse(row.Cells["CartonNo"]?.Value?.ToString(), out int carton) ? carton : (int?)null;
                    string cartonSize = row.Cells["CartonSize"]?.Value?.ToString();
                    string customerCarton = row.Cells["CustomerCarton"]?.Value?.ToString();
                    var orders = new List<(int, int?, decimal?, int?, string, string)>();
                    {
                        orders.Add((orderId, pcsReal, nwReal, cartonNo, cartonSize, customerCarton));
                    }
                    ;
                    bool isScussess = await SQLManager.Instance.UpdatePackOrdersBulkAsync(orders);
                    if (!isScussess)
                        MessageBox.Show($"Cập Nhật Thất Bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        status_lb.Text = "Thành công.";
                        status_lb.ForeColor = System.Drawing.Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý thay đổi dữ liệu:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateNWReal(DataGridViewRow row)
        {
            var cellPCS = row.Cells["PCSReal"].Value;
            if (cellPCS != null && int.TryParse(cellPCS.ToString(), out int pcs))
            {
                var amount = Convert.ToInt32(row.Cells["Amount"].Value);
                var packing = Convert.ToString(row.Cells["packing"].Value);

                row.Cells["NWReal"].Value = Utils.calNetWeight(pcs, amount, packing);
            }
            else
            {
                row.Cells["NWReal"].Value = DBNull.Value;
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
            if (e.RowIndex < 0) return;
            
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
                    dataGV.Columns[e.ColumnIndex].Name == "CartonSize")
            {
                e.CellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (dataGV.Columns[e.ColumnIndex].Name == "PCSReal")
            {
                var row = dataGV.Rows[e.RowIndex];
                var packageVal = row.Cells["Package"].Value?.ToString();

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
                var row = dataGV.Rows[e.RowIndex];
                var packageVal = row.Cells["Package"].Value?.ToString();

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
                var row = dataGV.Rows[e.RowIndex];
                int PCSOther = int.TryParse(row.Cells["PCSOther"].Value?.ToString(), out var pcs) ? pcs : 0;
                decimal NWOther = decimal.TryParse(row.Cells["NWOther"].Value?.ToString(), out var nw) ? nw : 0;


                if (PCSOther <= 0 && NWOther <= 0)
                {
                    e.CellStyle.BackColor = System.Drawing.Color.PaleVioletRed;
                }
            }
        }
        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = System.Drawing.Color.Beige;
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

            if ( colName == "CartonNo" || colName == "NWReal")
            {
                tb.KeyPress += Tb_KeyPress_OnlyNumber;
            }
            else if (colName == "PCSReal" )
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
        }

        private void assignCustomerCarton_btn_Click(object sender, EventArgs e)
        {

            _ =autoCreateCustomverCarton();
        }

        private async Task autoCreateCustomverCarton()
        {
            search_tb.Text = "";
            await Task.Delay(200);
            DataTable dt = null;

            if (dataGV.DataSource is DataTable)
                dt = (DataTable)dataGV.DataSource;
            else if (dataGV.DataSource is DataView)
                dt = ((DataView)dataGV.DataSource).ToTable();

            dt.DefaultView.Sort = "";

            // Lấy tất cả hàng (bỏ hàng mới)
            var rows = dataGV.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();

            // Nhóm theo CustomerCode
            var groups = rows.GroupBy(r => (r.Cells["CustomerCode"].Value ?? "").ToString().Trim());

            foreach (var grp in groups)
            {
                string customer = grp.Key;
                if (string.IsNullOrEmpty(customer)) continue;

                // Bước 1: gom theo CartonNo
                var cartonMap = new Dictionary<string, List<DataGridViewRow>>();
                foreach (var row in grp)
                {
                    string cartonKey = (row.Cells["CartonNo"].Value ?? "").ToString().Trim();
                    if (string.IsNullOrEmpty(cartonKey)) continue;

                    if (!cartonMap.TryGetValue(cartonKey, out var list))
                    {
                        list = new List<DataGridViewRow>();
                        cartonMap[cartonKey] = list;
                    }
                    list.Add(row);
                }

                // Bước 2: tách các carton đã có CustomerCarton và chưa có
                var existingCartonNos = new Dictionary<string, string>(); // CartonNo -> CustomerCarton
                foreach (var kv in cartonMap)
                {
                    // Lấy CustomerCarton đầu tiên (nếu có)
                    string existing = kv.Value
                        .Select(r => (r.Cells["CustomerCarton"].Value ?? "").ToString().Trim())
                        .FirstOrDefault(v => !string.IsNullOrEmpty(v));
                    if (!string.IsNullOrEmpty(existing))
                        existingCartonNos[kv.Key] = existing;
                }

                // Bước 3: tạo danh sách CartonNo cần gán mới (chưa có CustomerCarton)
                var distinctKeysToAssign = cartonMap.Keys
                    .Where(k => !existingCartonNos.ContainsKey(k))
                    .ToList();

                // Sắp xếp theo số học hoặc chữ
                distinctKeysToAssign.Sort((a, b) =>
                {
                    bool aNum = int.TryParse(a, out int ai);
                    bool bNum = int.TryParse(b, out int bi);
                    if (aNum && bNum) return ai.CompareTo(bi);
                    if (aNum) return -1;
                    if (bNum) return 1;
                    return string.Compare(a, b, StringComparison.Ordinal);
                });

                // Bước 4: lấy danh sách CustomerCarton đã tồn tại (để tránh trùng)
                var usedCustomerCartons = new HashSet<string>(
                    grp.Select(r => (r.Cells["CustomerCarton"].Value ?? "").ToString().Trim())
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
                foreach (var key in distinctKeysToAssign)
                {
                    maxIdx++;
                    string newCCarton = $"{customer}.{maxIdx}";
                    existingCartonNos[key] = newCCarton;
                }

                // Bước 6: gán CustomerCarton cho toàn bộ row
                foreach (var kv in cartonMap)
                {
                    string cartonNo = kv.Key;
                    string custCarton = existingCartonNos.ContainsKey(cartonNo)
                        ? existingCartonNos[cartonNo]
                        : "";

                    foreach (var row in kv.Value)
                    {
                        row.Cells["CustomerCarton"].Value = custCarton;

                        DataRow rowOrder = mOrders_dt.AsEnumerable()
                            .FirstOrDefault(r => r.Field<int>("OrderId") == Convert.ToInt32(row.Cells["OrderId"].Value));
                        if (rowOrder != null)
                            rowOrder["CustomerCarton"] = custCarton;
                    }
                }
            }

            dataGV.Refresh();
            fillter_btn_Click(null, null);
            await Task.Delay(300);
            SaveData();
        }


        private async void assignCartonSizeBtn_Click(object sender, EventArgs e)
        {
            DataTable dt = null;

            if (dataGV.DataSource is DataTable)
            {
                dt = (DataTable)dataGV.DataSource;
            }
            else if (dataGV.DataSource is DataView)
            {
                dt = ((DataView)dataGV.DataSource).ToTable();
            }
            dt.DefaultView.Sort = "";
            string cartonSize = cartonSize_cbb.Text;

            var selectedRows = dataGV.SelectedCells.Cast<DataGridViewCell>().Select(c => c.OwningRow).Distinct().ToList();
            var orders = new List<(int, int?, decimal?, int?, string, string)>();
            foreach (DataGridViewRow row in selectedRows)
            {
                DataRow rowOrder = mOrders_dt.AsEnumerable()
                                        .FirstOrDefault(r => r.Field<int>("OrderId") == Convert.ToInt32(row.Cells["OrderId"].Value));
                if (string.IsNullOrWhiteSpace(cartonSize))
                {
                    // Xóa dữ liệu ô CartonNo
                    row.Cells["CartonSize"].Value = DBNull.Value;
                    rowOrder["CartonSize"] = DBNull.Value;
                }
                else
                {
                    // Gán giá trị mới
                    row.Cells["CartonSize"].Value = cartonSize;
                    rowOrder["CartonSize"] = cartonSize;
                }

                int orderId = Convert.ToInt32(row.Cells["OrderId"].Value);
                int? pcsReal = int.TryParse(row.Cells["PCSReal"]?.Value?.ToString(), out int pcs) ? pcs : (int?)null;
                decimal? nwReal = decimal.TryParse(row.Cells["NWReal"]?.Value?.ToString(), out decimal nw) ? nw : (decimal?)null;
                int? cartonNo1 = int.TryParse(row.Cells["CartonNo"]?.Value?.ToString(), out int carton) ? carton : (int?)null;
                string cartonSize1 = row.Cells["CartonSize"]?.Value?.ToString();
                string customerCarton = row.Cells["CustomerCarton"]?.Value?.ToString();

                orders.Add((orderId, pcsReal, nwReal, cartonNo1, cartonSize1, customerCarton));
            }

            bool isScussess = await SQLManager.Instance.UpdatePackOrdersBulkAsync(orders);
            if (!isScussess)
                MessageBox.Show($"Cập Nhật Thất Bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                status_lb.Text = "Thành công.";
                status_lb.ForeColor = System.Drawing.Color.Green;
            }
        }
        
        private async void AssignCartonNoBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedCells.Count <= 0) return;
            DataTable dt = null;

            if (dataGV.DataSource is DataTable)
            {
                dt = (DataTable)dataGV.DataSource;
            }
            else if (dataGV.DataSource is DataView)
            {
                dt = ((DataView)dataGV.DataSource).ToTable();
            }
            dt.DefaultView.Sort = "";
            string cartonNo = cartonNo_tb.Text;

            var selectedRows = dataGV.SelectedCells.Cast<DataGridViewCell>().Select(c => c.OwningRow).Distinct().ToList();
            var orders = new List<(int, int?, decimal?, int?, string, string)>();
            foreach (DataGridViewRow row in selectedRows)
            {
                DataRow rowOrder = mOrders_dt.AsEnumerable()
                                        .FirstOrDefault(r => r.Field<int>("OrderId") == Convert.ToInt32(row.Cells["OrderId"].Value));
                if (string.IsNullOrWhiteSpace(cartonNo))
                {
                    // Xóa dữ liệu ô CartonNo
                    row.Cells["CartonNo"].Value = DBNull.Value;
                    rowOrder["CartonNo"] = DBNull.Value;
                    
                }
                else
                {
                    // Gán giá trị mới
                    row.Cells["CartonNo"].Value = cartonNo;
                    rowOrder["CartonNo"] = cartonNo;
                }

                row.Cells["CustomerCarton"].Value = DBNull.Value;
                rowOrder["CustomerCarton"] = DBNull.Value;

                int orderId = Convert.ToInt32(row.Cells["OrderId"].Value);
                int? pcsReal = int.TryParse(row.Cells["PCSReal"]?.Value?.ToString(), out int pcs) ? pcs : (int?)null;
                decimal? nwReal = decimal.TryParse(row.Cells["NWReal"]?.Value?.ToString(), out decimal nw) ? nw : (decimal?)null;
                int? cartonNo1 = int.TryParse(row.Cells["CartonNo"]?.Value?.ToString(), out int carton) ? carton : (int?)null;
                string cartonSize = row.Cells["CartonSize"]?.Value?.ToString();
                string customerCarton = row.Cells["CustomerCarton"]?.Value?.ToString();

                orders.Add((orderId, pcsReal, nwReal, cartonNo1, cartonSize, customerCarton));
            }

            
            bool isScussess = await SQLManager.Instance.UpdatePackOrdersBulkAsync(orders);
            if (!isScussess)
                MessageBox.Show($"Cập Nhật Thất Bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                status_lb.Text = "Thành công.";
                status_lb.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void fillter_btn_Click(object sender, EventArgs e)
        {
            // Lấy danh sách CustomerCarton kèm theo các CartonNo
            var customerCartons = dataGV.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow) // bỏ dòng trắng cuối
                .Select(r => new
                {
                    CustomerCarton = r.Cells["CustomerCarton"].Value?.ToString(),
                    CartonNo = r.Cells["CartonNo"].Value?.ToString()
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.CustomerCarton) && !string.IsNullOrWhiteSpace(x.CartonNo))
                .GroupBy(x => x.CustomerCarton) // gom theo CustomerCarton
                .Select(g => new
                {
                    CustomerCarton = g.Key,
                    CartonNos = string.Join(", ", g.Select(x => x.CartonNo).Distinct().OrderBy(n => n))
                })
                .ToList();

            // Xóa dữ liệu cũ trong carton_GV
            carton_GV.Rows.Clear();

            // Thêm cột nếu chưa có
            if (!carton_GV.Columns.Contains("CustomerCarton"))
            {
                carton_GV.Columns.Add("CustomerCarton", "Customer\nCarton");
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

            CountCarton_tb.Text = carton_GV.Rows.Count.ToString();
        }

        private void previewPrint_PT_btn_Click(object sender, EventArgs e)
        {
            // Giả sử dataGV là DataGridView của bạn
            List<string> selectedCartons = new List<string>();
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

            if (selectedCartons.Count <= 0) return;

            PackingListPrinter printer = new PackingListPrinter(dataGV, "Asiaway AG", "Schwamendingenstrasse 10, 8050 Zürich, Switzerland", selectedCartons);

            // Gọi phương thức in
            // printer.Print(selectedCartons.Count);

            printer.PrintPreview();
        }
        private void print_btn_Click(object sender, EventArgs e)
        {

            // Giả sử dataGV là DataGridView của bạn
            List<string> selectedCartons = new List<string>();
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

            if (selectedCartons.Count <= 0) return;

            PackingListPrinter printer = new PackingListPrinter(dataGV, "Asiaway AG", "Schwamendingenstrasse 10, 8050 Zürich, Switzerland", selectedCartons);

            // Gọi phương thức in
             printer.Print(selectedCartons.Count);

        }

        private void checkCarton_btn_click(object sender, EventArgs e)
        {

            if (checkCartonNo())
            {
                MessageBox.Show("Data Không Vẫn Đề Gì", "CartonNo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private bool checkCartonNo()
        {
            string cartonColName = "CartonNo";
            string customerColName = "Customername";
            string sizeColName = "CartonSize";
            string PCSReal = "PCSReal";
            string NWReal = "NWReal";

            List<int> cartonNumbers = new List<int>();
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (!row.IsNewRow)
                {
                    if (int.TryParse(row.Cells[cartonColName].Value?.ToString(), out int carton))
                    {
                        cartonNumbers.Add(carton);
                    }
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

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                int pcs = 0;
                decimal nw = 0;

                string carton = row.Cells[cartonColName].Value?.ToString();
                string customer = row.Cells[customerColName].Value?.ToString();
                string cartonSize = row.Cells[sizeColName].Value?.ToString();               

                int.TryParse(row.Cells["PCSReal"].Value?.ToString(), out pcs);
                decimal.TryParse(row.Cells["NWReal"].Value?.ToString(), out nw);

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
                }
            }


            var cartonDict = new Dictionary<string, HashSet<string>>();
            List<string> errorCartons = new List<string>();

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                string carton = row.Cells[cartonColName].Value?.ToString();
                string customer = row.Cells[customerColName].Value?.ToString();

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

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                string carton = row.Cells[cartonColName].Value?.ToString();
                string size = row.Cells[sizeColName].Value?.ToString();

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

            return true;
        }

        public async void SaveData(bool ask = true)
        {
          //  if (!_dataChanged && ask) return;

            try
            {
                var orders = new List<(int, int?, decimal?, int?, string, string)>();

                foreach (DataGridViewRow row in dataGV.Rows)
                {
                    if (row.IsNewRow) continue;

                    int orderId = Convert.ToInt32(row.Cells["OrderId"].Value);

                    int? pcsReal = int.TryParse(row.Cells["PCSReal"]?.Value?.ToString(), out int pcs) ? pcs : (int?)null;
                    decimal? nwReal = decimal.TryParse(row.Cells["NWReal"]?.Value?.ToString(), out decimal nw) ? nw : (decimal?)null;
                    int? cartonNo = int.TryParse(row.Cells["CartonNo"]?.Value?.ToString(), out int carton) ? carton : (int?)null;
                    string cartonSize = row.Cells["CartonSize"]?.Value?.ToString();
                    string customerCarton = row.Cells["CustomerCarton"]?.Value?.ToString();

                    orders.Add((orderId, pcsReal, nwReal, cartonNo, cartonSize, customerCarton));
                }

                bool isScussess = await SQLManager.Instance.UpdatePackOrdersBulkAsync(orders);
                if (isScussess)
                {
                    status_lb.Text = "Thành công.";
                    status_lb.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!");
                }

                setUIReadOnly(true);
            }
            catch (Exception ex)
            {
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
                    print_btn.Visible = true;
                    previewPrint_PT_btn.Visible = true;
                    return;
                }
            }
            rightInfo_gb.Visible = !isReadOnly;
            edit_btn.Visible = isReadOnly;
            readOnly_btn.Visible = !isReadOnly;
            InPhieuGiaoHang_btn.Visible = isReadOnly;
            previewPrint_PGH_btn.Visible = isReadOnly;
            print_btn.Visible = isReadOnly;
            previewPrint_PT_btn.Visible = isReadOnly;
        }
      
        private void DataGV_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGV.CurrentCell == null) return;


            int rowIndex = dataGV.CurrentCell.RowIndex;
            object val = dataGV.Rows[rowIndex].Cells["CartonNo"].Value;
            var pcsVal = dataGV.Rows[rowIndex].Cells["PCSOther"].Value;
            var nwVal = dataGV.Rows[rowIndex].Cells["NWOther"].Value;

            int PCSOther = 0;
            double NWOther = 0;            
            int.TryParse(pcsVal?.ToString() ?? "0", out PCSOther);
            double.TryParse(nwVal?.ToString() ?? "0", out NWOther);

            delOrder_btn.Visible = (PCSOther <= 0 && NWOther <= 0);

            if (!int.TryParse(val?.ToString(), out int result))
                return;

            int CustomerID = Convert.ToInt32(dataGV.Rows[rowIndex].Cells["CustomerID"].Value);            
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

            // Duyệt toàn bộ các dòng trong DataGridView
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue; // bỏ qua dòng trống cuối

                decimal nw = 0;
                int pcs = 0;
                decimal nwtt = 0;
                int pcstt = 0;
                decimal.TryParse(row.Cells["NWOther"].Value?.ToString(), out nw);
                int.TryParse(row.Cells["PCSOther"].Value?.ToString(), out pcs);
                decimal.TryParse(row.Cells["NWReal"].Value?.ToString(), out nwtt);
                int.TryParse(row.Cells["PCSReal"].Value?.ToString(), out pcstt);
                int cus = Convert.ToInt32(row.Cells["CustomerID"].Value);

                object val1 = row.Cells["CartonNo"].Value;
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
                }
                sumNW += nw;
                sumPCS += pcs;
                sumNW_tt += nwtt;
                sumPCS_tt += pcstt;
            }
            tongdathang_name_label.Text = "T.Đặt hàng:";
            tongdathang_label.Text =  sumPCS + " pcs, " + sumNW + "kg";
            tongdongthung_name_lable.Text = "T.Đóng thùng:";
            tongdongthung_lable.Text =  sumPCS_tt + " pcs, " + sumNW_tt + " kg";
            dathang_cus_name_lable.Text = "KH Đặt:";
            dathang_cus_lable.Text = sumPCS_cus + " pcs, " + sumNW_cus + " kg";
            dongthung_cus_name_lable.Text = "KH Nhận:";
            dongthung_cus_lable.Text = sumPCS_cus_tt + " pcs, " + sumNW_cus_tt + " kg";
            if (CartonNo >= 0)
            {
                dathang_cn_name_lable.Text = "Thùng " + CartonNo + " Đặt:";
                dathang_cn_lable.Text = sumPCS_cn + " pcs, " + sumNW_cn + " kg";
                dongthung_cn_name_lable.Text = "Thùng " + CartonNo + " Nhận:";
                dongthung_cn_lable.Text = sumPCS_tt_cn + " pcs, " + sumNW_tt_cn + " kg";
            }
            else
            {
                dathang_cn_lable.Text = "null";
                dongthung_cn_lable.Text = "null";
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

    }
}
