using DocumentFormat.OpenXml.Bibliography;
using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class DomesticLiquidationExport : Form
    {
        System.Data.DataTable mDomesticLiquidationPrice_dt, mDomesticLiquidationExport_dt, mEmployee_dt;
        private DataView mLogDV;
        private Timer debounceTimer = new Timer { Interval = 300 };
        private Timer empDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public DomesticLiquidationExport()
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
            

            debounceTimer.Tick += DebounceTimer_Tick;
            empDebounceTimer.Tick += EmpDebounceTimer_Tick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            this.KeyDown += ProductList_KeyDown;

            domesticLiquidationPrice_cbb.TextUpdate += sku_cbb_TextUpdate;
            employeeBuy_CBB.TextUpdate += reportedBy_CBB_TextUpdate;
            quantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;
        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeDomesticLiquidationExport();
                ShowData();
            }
            else if (!isNewState && !edit_btn.Visible)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    System.Windows.Forms.Control ctrl = this.ActiveControl;

                    if (ctrl is TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    deleteProduct();
                }
            }
        }

        public async void ShowData()
        {
            dataGV.SelectionChanged -= this.dataGV_CellClick;
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                var domesticLiquidationPriceTask = SQLStore_Kho.Instance.getDomesticLiquidationPriceAsync();
                var domesticLiquidationExportTask = SQLStore_Kho.Instance.getDomesticLiquidationExportAsync();
                var empTask = SQLStore_QLNS.Instance.GetEmployeesAsync();
                var logDataTask = SQLStore_Kho.Instance.GetDomesticLiquidationExportLogAsync();
                await Task.WhenAll(domesticLiquidationPriceTask, domesticLiquidationExportTask, empTask, logDataTask);

                mDomesticLiquidationPrice_dt = domesticLiquidationPriceTask.Result;
                mDomesticLiquidationExport_dt = domesticLiquidationExportTask.Result;
                mEmployee_dt = empTask.Result;
                mLogDV = new DataView(logDataTask.Result);
                

                domesticLiquidationPrice_cbb.DataSource = mDomesticLiquidationPrice_dt;
                domesticLiquidationPrice_cbb.DisplayMember = "Name_VN";  // hiển thị tên
                domesticLiquidationPrice_cbb.ValueMember = "DomesticLiquidationPriceID";
                domesticLiquidationPrice_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
               
                employeeBuy_CBB.DataSource = mEmployee_dt;
                employeeBuy_CBB.DisplayMember = "FullName";  // hiển thị tên
                employeeBuy_CBB.ValueMember = "EmployeeID";
                employeeBuy_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                

                dataGV.DataSource = mDomesticLiquidationExport_dt;

                log_GV.DataSource = mLogDV;

                dataGV.Columns["DomesticLiquidationPriceID"].Visible = false;
                dataGV.Columns["ExportID"].Visible = false;
                dataGV.Columns["EmployeeBuyID"].Visible = false;


                dataGV.Columns["Name_VN"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["Package"].HeaderText = "Đ.Vị";
                dataGV.Columns["Quantity"].HeaderText = "S.Lượng";
                dataGV.Columns["EmployeeBuy"].HeaderText = "Người Mua";
                dataGV.Columns["ExportDate"].HeaderText = "Ngày Bán";
                dataGV.Columns["Price"].HeaderText = "Giá";
                dataGV.Columns["TotalMoney"].HeaderText = "Thành Tiền";

                dataGV.Columns["Price"].DefaultCellStyle.Format = "N0";
                dataGV.Columns["TotalMoney"].DefaultCellStyle.Format = "N0";
                dataGV.Columns["Quantity"].DefaultCellStyle.Format = "F1";

                dataGV.Columns["ExportDate"].Width = 70;
                dataGV.Columns["Name_VN"].Width = 150;
                dataGV.Columns["Package"].Width = 60;
                dataGV.Columns["Quantity"].Width = 70;
                dataGV.Columns["Price"].Width = 70;
                dataGV.Columns["TotalMoney"].Width = 70;
                dataGV.Columns["EmployeeBuy"].Width = 150;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                updateRightUI();

                Utils.SetTabStopRecursive(this, false);

                int countTab = 0;
                exportDate_dtp.TabIndex = countTab++; exportDate_dtp.TabStop = true;
                domesticLiquidationPrice_cbb.TabIndex = countTab++; domesticLiquidationPrice_cbb.TabStop = true;
                quantity_tb.TabIndex = countTab++; quantity_tb.TabStop = true;
                employeeBuy_CBB.TabIndex = countTab++; employeeBuy_CBB.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

                ReadOnly_btn_Click(null, null);
                dataGV.SelectionChanged += this.dataGV_CellClick;

                await Task.Delay(100);
                loadingOverlay.Hide();


                log_GV.Columns["LogID"].Visible = false;
                log_GV.Columns["DomesticLiquidationPriceID"].Visible = false;
                log_GV.Columns["OldValue"].HeaderText = "Giá Trị Cũ";
                log_GV.Columns["NewValue"].HeaderText = "Giá Trị Mới";
                log_GV.Columns["ActionBy"].HeaderText = "Người Thực Hiện";
                log_GV.Columns["CreatedAt"].HeaderText = "Ngày Thực Hiện";

                log_GV.Columns["OldValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["NewValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["ActionBy"].Width = 150;
                log_GV.Columns["CreatedAt"].Width = 120;

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

                string typed = domesticLiquidationPrice_cbb.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mDomesticLiquidationPrice_dt.AsEnumerable()
                    .Where(r => r["ProductNameVN_NoSign"].ToString()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mDomesticLiquidationPrice_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                domesticLiquidationPrice_cbb.DataSource = temp;
                domesticLiquidationPrice_cbb.DisplayMember = "Name_VN";
                domesticLiquidationPrice_cbb.ValueMember = "DomesticLiquidationPriceID";

                // Giữ lại text người đang gõ
                domesticLiquidationPrice_cbb.DroppedDown = true;
                domesticLiquidationPrice_cbb.Text = typed;
                domesticLiquidationPrice_cbb.SelectionStart = typed.Length;
                domesticLiquidationPrice_cbb.SelectionLength = 0;
            }
            catch { }
        }

        private void reportedBy_CBB_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            empDebounceTimer.Stop();
            empDebounceTimer.Start();
        }
        private void EmpDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                empDebounceTimer.Stop();

                string typed = employeeBuy_CBB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mEmployee_dt.AsEnumerable()
                    .Where(r => r["EmployessName_NoSign"].ToString()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mDomesticLiquidationPrice_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                employeeBuy_CBB.DataSource = temp;
                employeeBuy_CBB.DisplayMember = "FullName";
                employeeBuy_CBB.ValueMember = "EmployeeID";

                // Giữ lại text người đang gõ
                employeeBuy_CBB.DroppedDown = true;
                employeeBuy_CBB.Text = typed;
                employeeBuy_CBB.SelectionStart = typed.Length;
                employeeBuy_CBB.SelectionLength = 0;
            }
            catch { }
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }

        private async void updateItem(int exportID, DateTime exportDate, int domesticLiquidationPriceID, decimal quantity, int price, int employeeBuyID)
        {
            DataRow[] rows = mDomesticLiquidationPrice_dt.Select($"DomesticLiquidationPriceID = {domesticLiquidationPriceID}");
            DataRow[] empRows = mEmployee_dt.Select($"EmployeeID = {employeeBuyID}");
            if (empRows.Length <= 0 || rows.Length <= 0) return;

            foreach (DataRow row in mDomesticLiquidationExport_dt.Rows)
            {
                int ID = Convert.ToInt32(row["ExportID"]);
                if (ID.CompareTo(exportID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    string oldValue = $"ngày: {row["ExportDate"]} - số lượng: {row["Quantity"]} - giá: {row["Price"]}, người báo thanh lý: {row["EmployeeBuy"]}";
                    string newValue = $"ngày: {exportDate} - số lượng: {quantity} - giá: {price}, người báo thanh lý: {empRows[0]["FullName"]}";
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.updateDDomesticLiquidationExportAsync(exportID, exportDate, domesticLiquidationPriceID, quantity, price, employeeBuyID);

                            if (isScussess == true)
                            {
                                string package = rows[0]["Package"].ToString();                                

                                row["ExportDate"] = exportDate.Date;
                                row["DomesticLiquidationPriceID"] = domesticLiquidationPriceID;
                                row["Quantity"] = quantity;
                                row["Price"] = price;
                                row["EmployeeBuyID"] = employeeBuyID;

                                row["Name_VN"] = rows[0]["Name_VN"];
                                row["EmployeeBuy"] = empRows[0]["FullName"];
                                row["TotalMoney"] = Convert.ToInt32(quantity * price);
                                row["Package"] = package;

                                _ = SQLManager_Kho.Instance.insertDomesticLiquidationExportLogAsync(domesticLiquidationPriceID, "Update Success: " + oldValue, newValue);

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;

                                _ = SQLManager_Kho.Instance.insertDomesticLiquidationExportLogAsync(domesticLiquidationPriceID, "Update Fail: " + oldValue, newValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;

                            _ = SQLManager_Kho.Instance.insertDomesticLiquidationExportLogAsync(domesticLiquidationPriceID, "Update Exception: " + ex.Message + oldValue, newValue);
                        }
                    }
                    break;
                }
            }
        }

        private async void createItem(DateTime exportDate, int domesticLiquidationPriceID, decimal quantity, int price, int employeeBuyID)
        {
            DataRow[] rows = mDomesticLiquidationPrice_dt.Select($"DomesticLiquidationPriceID = {domesticLiquidationPriceID}");
            DataRow[] empRows = mEmployee_dt.Select($"EmployeeID = {employeeBuyID}");
            if (empRows.Length <= 0 || rows.Length <= 0) return;

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {

                string newValue = $"ngày: {exportDate} - số lượng: {quantity} - giá: {price}, người báo thanh lý: {empRows[0]["FullName"]}";
                try
                {
                    int newId = await SQLManager_Kho.Instance.insertDomesticLiquidationExportAsync(exportDate, domesticLiquidationPriceID, quantity, price, employeeBuyID);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mDomesticLiquidationExport_dt.NewRow();
                        string package = rows[0]["Package"].ToString();

                        drToAdd["ExportID"] = newId;
                        drToAdd["ExportDate"] = exportDate.Date;
                        drToAdd["DomesticLiquidationPriceID"] = domesticLiquidationPriceID;
                        drToAdd["Quantity"] = quantity;
                        drToAdd["Price"] = price;
                        drToAdd["EmployeeBuyID"] = employeeBuyID;

                        drToAdd["Name_VN"] = rows[0]["Name_VN"];
                        drToAdd["EmployeeBuy"] = empRows[0]["FullName"];
                        drToAdd["TotalMoney"] = Convert.ToInt32(quantity * price);
                        drToAdd["Package"] = package;

                        mDomesticLiquidationExport_dt.Rows.Add(drToAdd);
                        mDomesticLiquidationExport_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        _ = SQLManager_Kho.Instance.insertDomesticLiquidationExportLogAsync(domesticLiquidationPriceID, "Create Success: ", newValue);

                        newBtn_Click(null, null);
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                        _ = SQLManager_Kho.Instance.insertDomesticLiquidationExportLogAsync(domesticLiquidationPriceID, "Create Fail: ", newValue);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;

                    _ = SQLManager_Kho.Instance.insertDomesticLiquidationExportLogAsync(domesticLiquidationPriceID, "Create Exception: " + ex.Message, newValue);
                }

            }
        }
        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if (domesticLiquidationPrice_cbb.SelectedValue == null) return;
                        
            DataRowView productSKUData = (DataRowView)domesticLiquidationPrice_cbb.SelectedItem;

            DateTime exportDate = exportDate_dtp.Value.Date;
            int domesticLiquidationPriceID = Convert.ToInt32(domesticLiquidationPrice_cbb.SelectedValue);
            int price = Convert.ToInt32(productSKUData["SalePrice"]);
            int reportedB = Convert.ToInt32(employeeBuy_CBB.SelectedValue);
            decimal quantity = string.IsNullOrWhiteSpace(quantity_tb.Text) ? 0 : decimal.Parse(quantity_tb.Text);

            var isLocked = await SQLStore_QLNS.Instance.IsSalaryLockAsync(exportDate.Month, exportDate.Year);
            if (isLocked)
            {
                MessageBox.Show($"Tháng {exportDate.Month}/{exportDate.Year} Đã Bị Khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (id_tb.Text.Length != 0)
                updateItem(Convert.ToInt32(id_tb.Text), exportDate, domesticLiquidationPriceID, quantity, price, reportedB);
            else
                createItem(exportDate,domesticLiquidationPriceID, quantity, price, reportedB);

        }
        private async void deleteProduct()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mDomesticLiquidationExport_dt.Rows)
            {
                string exportID = row["ExportID"].ToString();
                if (exportID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        int domesticLiquidationPriceID = Convert.ToInt32(row["DomesticLiquidationPriceID"]);
                        string oldValue = $"ngày: {row["ExportDate"]} - số lượng: {row["Quantity"]} - giá: {row["Price"]}, người báo thanh lý: {row["EmployeeBuy"]}";
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.deletetDomesticLiquidationExportAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mDomesticLiquidationExport_dt.Rows.Remove(row);
                                mDomesticLiquidationExport_dt.AcceptChanges();

                                _ = SQLManager_Kho.Instance.insertDomesticLiquidationExportLogAsync(domesticLiquidationPriceID, "Delete Success: " + oldValue, "");
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                                _ = SQLManager_Kho.Instance.insertDomesticLiquidationExportLogAsync(domesticLiquidationPriceID, "Delete Fail: " + oldValue, "");
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                            _ = SQLManager_Kho.Instance.insertDomesticLiquidationExportLogAsync(domesticLiquidationPriceID, "Delete Fail: " + oldValue, "Exception: " + ex.Message);
                        }
                    }
                    break;
                }
            }

        }

        private void newBtn_Click(object sender, EventArgs e)
        {
            id_tb.Text = "";
            status_lb.Text = "";
            quantity_tb.Text = "";        

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            domesticLiquidationPrice_cbb.Enabled = true;
            employeeBuy_CBB.Enabled = true;
            exportDate_dtp.Enabled = true;
            RightUiReadOnly(false);

            domesticLiquidationPrice_cbb.Focus();

            RightUiEnable(true);
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            domesticLiquidationPrice_cbb.Enabled = false;
            employeeBuy_CBB.Enabled = false;
            exportDate_dtp.Enabled = false;
            RightUiReadOnly(true);
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();

            RightUiEnable(true);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            domesticLiquidationPrice_cbb.Enabled = false;
            employeeBuy_CBB.Enabled = true;
            exportDate_dtp.Enabled = true;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";

            RightUiReadOnly(false);
            RightUiEnable(false);
        }

        private void updateRightUI()
        {
            try
            {
                if (isNewState) return;

                if (dataGV.SelectedRows.Count > 0)
                {
                    var cells = dataGV.SelectedRows[0].Cells;

                    string ID = cells["ExportID"].Value.ToString();
                    int domesticLiquidationPriceID = Convert.ToInt32(cells["DomesticLiquidationPriceID"].Value);
                    int quantity = Convert.ToInt32(cells["Quantity"].Value);
                    int employeeBuyID = Convert.ToInt32(cells["EmployeeBuyID"].Value);

                    if (!domesticLiquidationPrice_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["DomesticLiquidationPriceID"].ToString() == domesticLiquidationPriceID.ToString()))
                    {
                        domesticLiquidationPrice_cbb.DataSource = mDomesticLiquidationPrice_dt;
                    }
                    domesticLiquidationPrice_cbb.SelectedValue = domesticLiquidationPriceID;

                    id_tb.Text = ID;
                    quantity_tb.Text = quantity.ToString("F1");
                    employeeBuy_CBB.SelectedValue = employeeBuyID;

                    status_lb.Text = "";

                    mLogDV.RowFilter = $"DomesticLiquidationPriceID = {domesticLiquidationPriceID}";
                    mLogDV.Sort = "LogID DESC";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            };
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

        private void RightUiReadOnly(bool isReadOnly)
        {
            quantity_tb.ReadOnly = isReadOnly;
        }

        private void RightUiEnable(bool enable)
        {
        }
    }
}
