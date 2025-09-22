using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlX.XDevAPI.Common;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class OrderPackingList : Form
    {
        DataTable mExportCode_dt, mOrders_dt;
        public OrderPackingList()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = true;
            carton_GV.ColumnHeadersVisible = false;
            carton_GV.RowHeadersVisible = false;
            carton_GV.AllowUserToAddRows = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;


            fillter_btn.Click += fillter_btn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            print_btn.Click += print_btn_Click;
            assignCartonNoBtn.Click += AssignCartonNoBtn_Click;
            autoEditCartonNo_btn.Click += autoEditCartonNo_btn_Click;
            assignPCSReal_btn.Click += AssignPCSRealBtn_Click;
            assignCartonSize_btn.Click += assignCartonSizeBtn_Click;
            assignCustomerCarton_btn.Click += assignCustomerCarton_btn_Click;
            dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            dataGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGV_EditingControlShowing);

            dataGV.DataBindingComplete += dataGV_DataBindingComplete;
            dataGV.CellFormatting += dataGV_CellFormatting;
            dataGV.CellValueChanged += dataGV_CellValueChanged;
            dataGV.KeyDown += dataGV_KeyDown;
            dataGV.CellBeginEdit += dataGV_CellBeginEdit;
            //dataGV.CellEndEdit += dataGV_CellEndEdit;

            cartonSize_tb.KeyPress += Tb_KeyPress_CartonSize;
            PCSReal_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            cartonNo_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            exportCode_cbb.SelectedIndexChanged += exportCode_search_cbb_SelectedIndexChanged;
        }

        public async void ShowData()
        {
            

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                var ordersPackingTask = SQLManager.Instance.getOrdersPackingAsync();
                var exportCodeTask = SQLManager.Instance.getExportCodes_Incomplete();

                await Task.WhenAll(ordersPackingTask, exportCodeTask);

                mExportCode_dt = exportCodeTask.Result;
                mOrders_dt = ordersPackingTask.Result;
                // Chạy truy vấn trên thread riêng
                dataGV.DataSource = mOrders_dt;
                dataGV.Columns["OrderId"].Visible = false;
                dataGV.Columns["ExportCodeID"].Visible = false;
                dataGV.Columns["Amount"].Visible = false;
                dataGV.Columns["packing"].Visible = false;
                dataGV.Columns["ProductnameEN"].Visible = false;
                dataGV.Columns["Package"].Visible = false;
                dataGV.Columns["ExportDate"].Visible = false;
                dataGV.Columns["ExportCode"].Visible = false;
                dataGV.Columns["CustomerCode"].Visible = false;

                dataGV.ReadOnly = false;
                dataGV.Columns["CartonNo"].ReadOnly = false;
                dataGV.Columns["PCSReal"].ReadOnly = false;
                dataGV.Columns["CartonSize"].ReadOnly = false;
                
                dataGV.Columns["Priority"].Width = 30;

                dataGV.Columns["NWReal"].ReadOnly = true;
                dataGV.Columns["Priority"].ReadOnly = true;
                dataGV.Columns["CustomerName"].ReadOnly = true;
                dataGV.Columns["ProductPackingName"].ReadOnly = true;
                dataGV.Columns["CustomerCarton"].ReadOnly = true;
                
                dataGV.Columns["PCSReal"].HeaderText = "PCS\nThực Tế";
                dataGV.Columns["NWReal"].HeaderText = "NW\nThực Tế";
                dataGV.Columns["CartonNo"].HeaderText = "Carton.No";
                dataGV.Columns["CartonSize"].HeaderText = "Carton Size";
                dataGV.Columns["Priority"].HeaderText = "Ưu\nTiên";
                dataGV.Columns["Customername"].HeaderText = "Khách Hàng";
                dataGV.Columns["ProductPackingName"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["CustomerCarton"].HeaderText = "Mã Thùng";

                dataGV.Columns["PCSReal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["NWReal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["CartonNo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["CartonSize"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //   dataGV.Columns["Priority"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["CustomerName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["ProductPackingName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dataGV.Columns["CartonNo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["PCSReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["NWReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["Priority"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                exportCode_cbb.DataSource = mExportCode_dt;
                exportCode_cbb.DisplayMember = "ExportCode";  // hiển thị tên
                exportCode_cbb.ValueMember = "ExportCodeID";

                //   dataGV.AutoResizeColumns();

                fillter_btn_Click(null, null);
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

            fillter_btn_Click(null, null);
        }

        private void dataGV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var columnName = dataGV.Columns[e.ColumnIndex].Name;

                if (columnName == "PCSReal")
                {
                    UpdateNWReal(dataGV.Rows[e.RowIndex]);
                }
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

        private Dictionary<string, Color> colorMap = new Dictionary<string, Color>();
        private Color[] colors = new Color[] {Color.LightBlue, Color.LightGreen, Color.LightPink, Color.LightCyan};

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
                    dataGV.Columns[e.ColumnIndex].Name == "PCSReal")
            {
                e.CellStyle.BackColor = Color.LightGray;
            }
        }
        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
            }
        }        

        private void dataGV_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tb = e.Control as TextBox;
            if (tb == null) return;

            // Gỡ toàn bộ trước để tránh add trùng
            tb.KeyPress -= Tb_KeyPress_OnlyNumber;
            tb.KeyPress -= Tb_KeyPress_CartonSize;

            var colName = dataGV.CurrentCell.OwningColumn.Name;

            if (colName == "PCSReal" || colName == "CartonNo")
            {
                tb.KeyPress += Tb_KeyPress_OnlyNumber;
            }
            else if (colName == "CartonSize")
            {
                tb.KeyPress += Tb_KeyPress_CartonSize;
            }
        }


        private void dataGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Chỉ áp dụng cho cột CartonNo
            if (dataGV.Columns[e.ColumnIndex].Name == "NWReal")
            {
                var cell = dataGV.Rows[e.RowIndex].Cells[e.ColumnIndex];

                // Ví dụ: nếu ô đó NULL hoặc giá trị nào đó thì khóa
                if (cell.Value == null || string.IsNullOrEmpty(cell.Value.ToString()))
                {
                    // Cho phép nhập
                    e.Cancel = false;
                }
                else
                {
                    // Khóa không cho nhập
                    e.Cancel = true;
                }
            }
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho nhập số hoặc phím điều khiển (Backspace, Delete, Enter…)
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
            // Lấy tất cả hàng (bỏ hàng mới)
            var rows = dataGV.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();

            // Nhóm theo CustomerName (trim)
            var groups = rows.GroupBy(r => (r.Cells["CustomerCode"].Value ?? "")
                                              .ToString().Trim());

            foreach (var grp in groups)
            {
                string customer = grp.Key; // tên khách (đã trim)

                // Tạo map: cartonKey (string) -> list các hàng có carton đó
                var cartonMap = new Dictionary<string, List<DataGridViewRow>>();
                foreach (var row in grp)
                {
                    string cartonKey = (row.Cells["CartonNo"].Value ?? "").ToString().Trim();
                    if (!cartonMap.TryGetValue(cartonKey, out var list))
                    {
                        list = new List<DataGridViewRow>();
                        cartonMap[cartonKey] = list;
                    }
                    list.Add(row);
                }

                // Lấy danh sách cartonKey độc nhất và sắp xếp:
                // - nếu cả hai đều parse được thành int thì sort theo số
                // - số sẽ đứng trước chuỗi không phải số
                var distinctKeys = cartonMap.Keys.ToList();
                distinctKeys.Sort((a, b) =>
                {
                    bool aNum = int.TryParse(a, out int ai);
                    bool bNum = int.TryParse(b, out int bi);
                    if (aNum && bNum) return ai.CompareTo(bi);
                    if (aNum) return -1;   // số trước chữ
                    if (bNum) return 1;
                    return string.Compare(a, b, StringComparison.Ordinal);
                });

                // Gán index cho từng cartonKey và set cùng giá trị cho tất cả hàng trong nhóm đó
                int idx = 1;
                foreach (var key in distinctKeys)
                {
                    foreach (var row in cartonMap[key])
                    {
                        DataRow rowOrder = mOrders_dt.AsEnumerable()
                                       .FirstOrDefault(r => r.Field<int>("OrderId") == Convert.ToInt32(row.Cells["OrderId"].Value));

                        row.Cells["CustomerCarton"].Value = $"{customer}-{idx}";
                        rowOrder["CustomerCarton"] = $"{customer}-{idx}";
                    }
                    idx++;
                }
            }

            // refresh DataGridView (nếu cần)
            dataGV.Refresh();
            fillter_btn_Click(null, null);
        }

        private void assignCartonSizeBtn_Click(object sender, EventArgs e)
        {
            string cartonSize = cartonSize_tb.Text;

            foreach (DataGridViewRow row in dataGV.SelectedRows)
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
            }
        }
        private void AssignPCSRealBtn_Click(object sender, EventArgs e)
        {
            string cartonNo = PCSReal_tb.Text;

            foreach (DataGridViewRow row in dataGV.SelectedRows)
            {
                DataRow rowOrder = mOrders_dt.AsEnumerable()
                                        .FirstOrDefault(r => r.Field<int>("OrderId") == Convert.ToInt32(row.Cells["OrderId"].Value));
                if (string.IsNullOrWhiteSpace(cartonNo))
                {
                    // Xóa dữ liệu ô CartonNo
                    row.Cells["PCSReal"].Value = DBNull.Value;
                    rowOrder["PCSReal"] = DBNull.Value;
                }
                else
                {
                    // Gán giá trị mới
                    row.Cells["PCSReal"].Value = cartonNo;
                    rowOrder["PCSReal"] = cartonNo;
                }
            }
            mOrders_dt.AcceptChanges();
        }
        private void AssignCartonNoBtn_Click(object sender, EventArgs e)
        {
            string cartonNo = cartonNo_tb.Text;

            foreach (DataGridViewRow row in dataGV.SelectedRows)
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
            }
        }

        private void autoEditCartonNo_btn_Click(object sender, EventArgs e)
        {
            bool result2 = CheckCartonCustomerConsistency(dataGV, "CartonNo", "Customername");
            if (!result2)
            {
                MessageBox.Show("Một CartonNo hiện đang chứa nhiều Customer khác nhau!", "CartonNo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FixCartonNoContinuous(dataGV, "CartonNo");
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            bool result = CheckCartonNoContinuous(dataGV, "CartonNo");
            if (!result) {
                MessageBox.Show("CartonNo bị nhảy số!", "CartonNo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool result2 = CheckCartonCustomerConsistency(dataGV, "CartonNo", "Customername");
            if (!result2)
            {
                MessageBox.Show("Một CartonNo đang chứa nhiều Customer khác nhau!", "CartonNo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


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

                bool isScussess = await SQLManager.Instance.updatePackOrdersBulkAsync(orders);
                if (isScussess)
                {
                    MessageBox.Show("Cập nhật thành công!");
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!");
                }
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }

        }

        private void fillter_btn_Click(object sender, EventArgs e)
        {
            // Lấy danh sách CustomerCarton duy nhất (Distinct)
            var customerCartons = dataGV.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow) // bỏ dòng trắng cuối
                .Select(r => r.Cells["CustomerCarton"].Value?.ToString())
                .Where(s => !string.IsNullOrWhiteSpace(s)) // bỏ trống
                .Distinct() // loại trùng
                .ToList();

            // Xóa dữ liệu cũ trong carton_GV
            carton_GV.Rows.Clear();

            if (!carton_GV.Columns.Contains("CustomerCarton"))
            {
                carton_GV.Columns.Add("CustomerCarton", "CustomerCarton");
                carton_GV.Columns["CustomerCarton"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                carton_GV.Columns["CustomerCarton"].ReadOnly = true;
            }

            // Đổ dữ liệu mới vào
            foreach (var c in customerCartons)
            {
                carton_GV.Rows.Add(c);
            }

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

            PackingListPrinter printer = new PackingListPrinter(dataGV, "Asiaway AG", "Schwamendingenstrasse 10, 8050 Zürich, Switzerland", selectedCartons);

            // Gọi phương thức in
             printer.Print(selectedCartons.Count);

            // printer.PrintPreview();
        }

        private bool CheckCartonNoContinuous(DataGridView dgv, string cartonColName)
        {
            List<int> cartonNumbers = new List<int>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    if (int.TryParse(row.Cells[cartonColName].Value?.ToString(), out int carton))
                    {
                        cartonNumbers.Add(carton);
                    }
                }
            }

            if (cartonNumbers.Count == 0) return true;

            // Lấy min và max
            int min = cartonNumbers.Min();
            int max = cartonNumbers.Max();

            // Tạo set duy nhất
            HashSet<int> uniqueNumbers = new HashSet<int>(cartonNumbers);

            // Kiểm tra số liên tục
            for (int i = min; i <= max; i++)
            {
                if (!uniqueNumbers.Contains(i))
                    return false; // Có số bị nhảy
            }

            return true; // Không nhảy số
        }

        private bool CheckCartonCustomerConsistency(DataGridView dgv, string cartonColName, string customerColName)
        {
            var cartonDict = new Dictionary<string, HashSet<string>>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    string carton = row.Cells[cartonColName].Value?.ToString();
                    string customer = row.Cells[customerColName].Value?.ToString();

                    if (string.IsNullOrEmpty(carton) || string.IsNullOrEmpty(customer))
                        continue;

                    if (!cartonDict.ContainsKey(carton))
                        cartonDict[carton] = new HashSet<string>();

                    cartonDict[carton].Add(customer);

                    if (cartonDict[carton].Count > 1)
                        return false; // Một carton chứa >1 Customer
                }
            }

            return true; // Tất cả ok
        }

        private void FixCartonNoContinuous(DataGridView dgv, string cartonColName)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Chỉnh sữa nội dung cột CartonNo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int currentNumber = 1; // Bắt đầu từ 1
                    string prevCartonValue = null;

                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            string cartonValue = row.Cells[cartonColName].Value?.ToString();

                            if (cartonValue != prevCartonValue)
                            {
                                prevCartonValue = cartonValue;
                                row.Cells[cartonColName].Value = currentNumber.ToString();
                                currentNumber++;
                            }
                            else
                            {
                                row.Cells[cartonColName].Value = (currentNumber - 1).ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }




            }

            
        }


    }
}
