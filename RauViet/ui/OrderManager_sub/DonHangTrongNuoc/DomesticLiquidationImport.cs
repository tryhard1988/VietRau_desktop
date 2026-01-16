using RauViet.classes;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class DomesticLiquidationImport : Form
    {
        System.Data.DataTable mDomesticLiquidationPrice_dt, mDomesticLiquidationImport_dt, mEmployee_dt;
        private DataView mLogDV;
        private Timer debounceTimer = new Timer { Interval = 300 };
        private Timer empDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public DomesticLiquidationImport()
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
            reportedBy_CBB.TextUpdate += reportedBy_CBB_TextUpdate;
            quantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;
        }

        private void ProductList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeDomesticLiquidationImport();
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
                var domesticLiquidationImportTask = SQLStore_Kho.Instance.getDomesticLiquidationImportAsync();
                var empTask = SQLStore_QLNS.Instance.GetEmployeesAsync();
                var logDataTask = SQLStore_Kho.Instance.GetDomesticLiquidationImportLogAsync();
                await Task.WhenAll(domesticLiquidationPriceTask, domesticLiquidationImportTask, empTask, logDataTask);

                mDomesticLiquidationPrice_dt = domesticLiquidationPriceTask.Result;
                mDomesticLiquidationImport_dt = domesticLiquidationImportTask.Result;
                mEmployee_dt = empTask.Result;
                mLogDV = new DataView(logDataTask.Result);
                

                domesticLiquidationPrice_cbb.DataSource = mDomesticLiquidationPrice_dt;
                domesticLiquidationPrice_cbb.DisplayMember = "Name_VN";  // hiển thị tên
                domesticLiquidationPrice_cbb.ValueMember = "DomesticLiquidationPriceID";
                domesticLiquidationPrice_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
               
                reportedBy_CBB.DataSource = mEmployee_dt;
                reportedBy_CBB.DisplayMember = "FullName";  // hiển thị tên
                reportedBy_CBB.ValueMember = "EmployeeID";
                reportedBy_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                

                dataGV.DataSource = mDomesticLiquidationImport_dt;

                log_GV.DataSource = mLogDV;

                dataGV.Columns["DomesticLiquidationPriceID"].Visible = false;
                dataGV.Columns["ImportID"].Visible = false;
                dataGV.Columns["ReportedByID"].Visible = false;

                dataGV.Columns["Name_VN"].HeaderText = "Tên Sản Phẩm";
                dataGV.Columns["Package"].HeaderText = "Đ.Vị";
                dataGV.Columns["Quantity"].HeaderText = "S.Lượng";
                dataGV.Columns["EmployeeReported"].HeaderText = "Người Báo TL";
                dataGV.Columns["ImportDate"].HeaderText = "Ngày Nhập";
                dataGV.Columns["Price"].HeaderText = "Giá";
                dataGV.Columns["TotalMoney"].HeaderText = "Thành Tiền";

                dataGV.Columns["Price"].DefaultCellStyle.Format = "N0";
                dataGV.Columns["TotalMoney"].DefaultCellStyle.Format = "N0";
                dataGV.Columns["Quantity"].DefaultCellStyle.Format = "F1";

                dataGV.Columns["ImportDate"].Width = 70;
                dataGV.Columns["Name_VN"].Width = 150;
                dataGV.Columns["Package"].Width = 60;
                dataGV.Columns["Quantity"].Width = 70;
                dataGV.Columns["Price"].Width = 70;
                dataGV.Columns["TotalMoney"].Width = 70;
                dataGV.Columns["EmployeeReported"].Width = 150;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                updateRightUI();

                Utils.SetTabStopRecursive(this, false);

                int countTab = 0;
                importDate_dtp.TabIndex = countTab++; importDate_dtp.TabStop = true;
                domesticLiquidationPrice_cbb.TabIndex = countTab++; domesticLiquidationPrice_cbb.TabStop = true;
                quantity_tb.TabIndex = countTab++; quantity_tb.TabStop = true;
                reportedBy_CBB.TabIndex = countTab++; reportedBy_CBB.TabStop = true;
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

                string typed = reportedBy_CBB.Text ?? "";
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
                reportedBy_CBB.DataSource = temp;
                reportedBy_CBB.DisplayMember = "FullName";
                reportedBy_CBB.ValueMember = "EmployeeID";

                // Giữ lại text người đang gõ
                reportedBy_CBB.DroppedDown = true;
                reportedBy_CBB.Text = typed;
                reportedBy_CBB.SelectionStart = typed.Length;
                reportedBy_CBB.SelectionLength = 0;
            }
            catch { }
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }

        private async void updateItem(int ImportID, DateTime importDate, int domesticLiquidationPriceID, decimal quantity, int price, int reportedByID)
        {
            DataRow[] rows = mDomesticLiquidationPrice_dt.Select($"DomesticLiquidationPriceID = {domesticLiquidationPriceID}");
            DataRow[] empRows = mEmployee_dt.Select($"EmployeeID = {reportedByID}");
            if (empRows.Length <= 0 || rows.Length <= 0) return;

            foreach (DataRow row in mDomesticLiquidationImport_dt.Rows)
            {
                int ID = Convert.ToInt32(row["ImportID"]);
                if (ID.CompareTo(ImportID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    string oldValue = $"ngày: {row["ImportDate"]} - số lượng: {row["Quantity"]} - giá: {row["Price"]}, người báo thanh lý: {row["EmployeeReported"]}";
                    string newValue = $"ngày: {importDate} - số lượng: {quantity} - giá: {price}, người báo thanh lý: {empRows[0]["FullName"]}";
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.updateDDomesticLiquidationImportAsync(ImportID, importDate, domesticLiquidationPriceID, quantity, price, reportedByID);

                            if (isScussess == true)
                            {
                                string package = rows[0]["Package"].ToString();                                

                                row["ImportDate"] = importDate.Date;
                                row["DomesticLiquidationPriceID"] = domesticLiquidationPriceID;
                                row["Quantity"] = quantity;
                                row["Price"] = price;
                                row["ReportedByID"] = reportedByID;

                                row["Name_VN"] = rows[0]["Name_VN"];
                                row["EmployeeReported"] = empRows[0]["FullName"];
                                row["TotalMoney"] = Convert.ToInt32(quantity * price);
                                row["Package"] = package;

                                _ = SQLManager_Kho.Instance.insertDomesticLiquidationImportLogAsync(domesticLiquidationPriceID, "Update Success: " + oldValue, newValue);

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;

                                _ = SQLManager_Kho.Instance.insertDomesticLiquidationImportLogAsync(domesticLiquidationPriceID, "Update Fail: " + oldValue, newValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;

                            _ = SQLManager_Kho.Instance.insertDomesticLiquidationImportLogAsync(domesticLiquidationPriceID, "Update Exception: " + ex.Message + oldValue, newValue);
                        }
                    }
                    break;
                }
            }
        }

        private async void createItem(DateTime importDate, int domesticLiquidationPriceID, decimal quantity, int price, int reportedByID)
        {
            DataRow[] rows = mDomesticLiquidationPrice_dt.Select($"DomesticLiquidationPriceID = {domesticLiquidationPriceID}");
            DataRow[] empRows = mEmployee_dt.Select($"EmployeeID = {reportedByID}");
            if (empRows.Length <= 0 || rows.Length <= 0) return;

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {

                string newValue = $"ngày: {importDate} - số lượng: {quantity} - giá: {price}, người báo thanh lý: {empRows[0]["FullName"]}";
                try
                {
                    int newId = await SQLManager_Kho.Instance.insertDomesticLiquidationImportAsync(importDate, domesticLiquidationPriceID, quantity, price, reportedByID);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mDomesticLiquidationImport_dt.NewRow();
                        string package = rows[0]["Package"].ToString();

                        drToAdd["ImportID"] = newId;
                        drToAdd["ImportDate"] = importDate.Date;
                        drToAdd["DomesticLiquidationPriceID"] = domesticLiquidationPriceID;
                        drToAdd["Quantity"] = quantity;
                        drToAdd["Price"] = price;
                        drToAdd["ReportedByID"] = reportedByID;

                        drToAdd["Name_VN"] = rows[0]["Name_VN"];
                        drToAdd["EmployeeReported"] = empRows[0]["FullName"];
                        drToAdd["TotalMoney"] = Convert.ToInt32(quantity * price);
                        drToAdd["Package"] = package;

                        mDomesticLiquidationImport_dt.Rows.Add(drToAdd);
                        mDomesticLiquidationImport_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        _ = SQLManager_Kho.Instance.insertDomesticLiquidationImportLogAsync(domesticLiquidationPriceID, "Create Success: ", newValue);

                        newBtn_Click(null, null);
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                        _ = SQLManager_Kho.Instance.insertDomesticLiquidationImportLogAsync(domesticLiquidationPriceID, "Create Fail: ", newValue);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;

                    _ = SQLManager_Kho.Instance.insertDomesticLiquidationImportLogAsync(domesticLiquidationPriceID, "Create Exception: " + ex.Message, newValue);
                }

            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (domesticLiquidationPrice_cbb.SelectedValue == null) return;
                        
            DataRowView productSKUData = (DataRowView)domesticLiquidationPrice_cbb.SelectedItem;

            DateTime importDate = importDate_dtp.Value.Date;
            int domesticLiquidationPriceID = Convert.ToInt32(domesticLiquidationPrice_cbb.SelectedValue);
            int price = Convert.ToInt32(productSKUData["SalePrice"]);
            int reportedB = Convert.ToInt32(reportedBy_CBB.SelectedValue);

            decimal quantity = string.IsNullOrWhiteSpace(quantity_tb.Text) ? 0 : decimal.Parse(quantity_tb.Text, CultureInfo.InvariantCulture);


            if (id_tb.Text.Length != 0)
                updateItem(Convert.ToInt32(id_tb.Text), importDate, domesticLiquidationPriceID, quantity, price, reportedB);
            else
                createItem(importDate,domesticLiquidationPriceID, quantity, price, reportedB);

        }
        private async void deleteProduct()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mDomesticLiquidationImport_dt.Rows)
            {
                string importID = row["ImportID"].ToString();
                if (importID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        int domesticLiquidationPriceID = Convert.ToInt32(row["DomesticLiquidationPriceID"]);
                        string oldValue = $"ngày: {row["ImportDate"]} - số lượng: {row["Quantity"]} - giá: {row["Price"]}, người báo thanh lý: {row["EmployeeReported"]}";
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.deletetDomesticLiquidationImportAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mDomesticLiquidationImport_dt.Rows.Remove(row);
                                mDomesticLiquidationImport_dt.AcceptChanges();

                                _ = SQLManager_Kho.Instance.insertDomesticLiquidationImportLogAsync(domesticLiquidationPriceID, "Delete Success: " + oldValue, "");
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                                _ = SQLManager_Kho.Instance.insertDomesticLiquidationImportLogAsync(domesticLiquidationPriceID, "Delete Fail: " + oldValue, "");
                            }
                        }
                        catch (Exception ex)
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                            _ = SQLManager_Kho.Instance.insertDomesticLiquidationImportLogAsync(domesticLiquidationPriceID, "Delete Fail: " + oldValue, "Exception: " + ex.Message);
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
            reportedBy_CBB.Enabled = true;
            importDate_dtp.Enabled = true;
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
            reportedBy_CBB.Enabled = false;
            importDate_dtp.Enabled = false;
            RightUiReadOnly(true);
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();

            RightUiEnable(true);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            domesticLiquidationPrice_cbb.Enabled = false;
            reportedBy_CBB.Enabled = true;
            importDate_dtp.Enabled = true;
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

                    string ID = cells["ImportID"].Value.ToString();
                    int domesticLiquidationPriceID = Convert.ToInt32(cells["DomesticLiquidationPriceID"].Value);
                    decimal quantity = Convert.ToDecimal(cells["Quantity"].Value);
                    int reportedByID = Convert.ToInt32(cells["ReportedByID"].Value);
                    DateTime importDate = Convert.ToDateTime(cells["ImportDate"].Value);

                    if (!domesticLiquidationPrice_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["DomesticLiquidationPriceID"].ToString() == domesticLiquidationPriceID.ToString()))
                    {
                        domesticLiquidationPrice_cbb.DataSource = mDomesticLiquidationPrice_dt;
                    }
                    domesticLiquidationPrice_cbb.SelectedValue = domesticLiquidationPriceID;

                    id_tb.Text = ID;
                    quantity_tb.Text = quantity.ToString("F1", CultureInfo.InvariantCulture);
                    reportedBy_CBB.SelectedValue = reportedByID;
                    importDate_dtp.Value = importDate;
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
