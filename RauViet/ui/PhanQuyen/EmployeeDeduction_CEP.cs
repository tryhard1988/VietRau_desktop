using RauViet.classes;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{    
    public partial class EmployeeDeduction_CEP : Form
    {
        private Timer _monthYearDebounceTimer;
        private DataTable mEmployeeDeduction_dt, mEmployee_dt;
        private DataView mDeductionLogDV;
        private const string DeductionTypeCode = "CEP";
        private string mDeductionName = "";
        bool isNewState = false;
        private bool _isUpdatingUI = false;
        int mCurYear;
        public EmployeeDeduction_CEP()
        {
            InitializeComponent();
            this.KeyPreview = true;
            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            amount_tb.TabIndex = countTab++; amount_tb.TabStop = true;
            note_tb.TabIndex = countTab++; note_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "MM/yyyy";
            monthYearDtp.ShowUpDown = true;
            monthYearDtp.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            monthYearDtp.ValueChanged += monthYearDtp_ValueChanged;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            employeeDeductionGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            employeeDeductionGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            dataGV.Click += this.dataGV_CellClick;

            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;            

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
            this.KeyDown += EmployeeDeduction_CEP_KeyDown;

            search_tb.TextChanged += Search_tb_TextChanged;
            this.Load += OvertimeAttendace_Load;
            CEP_In_Month_cb.CheckedChanged += CEP_In_Month_cb_CheckedChanged; ;
        }


        private void OvertimeAttendace_Load(object sender, EventArgs e)
        {
            _monthYearDebounceTimer = new Timer();
            _monthYearDebounceTimer.Interval = 500;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
        }

        private void EmployeeDeduction_CEP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_QLNS.Instance.removeDeduction(monthYearDtp.Value.Year);
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

                    deleteBtn_Click(null, null);
                }
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);
            try
            {
                employeeDeductionGV.SelectionChanged -= this.allowanceGV_CellClick;
                employeeDeductionGV.Click -= this.allowanceGV_CellClick;
                DateTime selectDate = monthYearDtp.Value;
                string[] keepColumns = { "EmployeeCode", "FullName", "PositionName", "ContractTypeName", "EmployessName_NoSign" };
                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
                var employeeDeductionAsync = SQLStore_QLNS.Instance.GetDeductionAsync(selectDate.Year, DeductionTypeCode);
                var deductionNameAsync = SQLStore_QLNS.Instance.GetDeductionNameAsync(DeductionTypeCode);
                var EmployeeDeductionLogTask = SQLStore_QLNS.Instance.GetEmployeeDeductionLogAsync(selectDate.Year, DeductionTypeCode);
                await Task.WhenAll(employeesTask, employeeDeductionAsync, deductionNameAsync, EmployeeDeductionLogTask);
                mEmployee_dt = employeesTask.Result;
                mEmployeeDeduction_dt = employeeDeductionAsync.Result;
                mDeductionName = deductionNameAsync.Result;
                mDeductionLogDV = new DataView(EmployeeDeductionLogTask.Result);

                mCurYear = selectDate.Year;

                foreach (DataRow dr in mEmployee_dt.Rows)
                {
                    if (dr["PositionName"] == DBNull.Value) dr["PositionName"] = "";
                    if (dr["ContractTypeName"] == DBNull.Value) dr["ContractTypeName"] = "";
                }

                mEmployeeDeduction_dt.Columns.Add("EmployeeName", typeof(string));
                foreach (DataRow dr in mEmployeeDeduction_dt.Rows)
                {
                    string employeeCode = dr["EmployeeCode"].ToString();
                    DataRow[] rows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");
                    if (rows.Length > 0)
                        dr["EmployeeName"] = rows[0]["FullName"];
                }

                monthYearLabel.Text = $"Năm {selectDate.Year}";

                employeeDeductionGV.DataSource = mEmployeeDeduction_dt;

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = mEmployee_dt;
                log_GV.DataSource = mDeductionLogDV;

                Utils.HideColumns(log_GV, new[] { "LogID", "EmployeeCode", "DeductionTypeCode" });
                Utils.HideColumns(employeeDeductionGV, new[] { "EmployeeDeductionID", "DeductionTypeCode", "DeductionTypeName" });
                Utils.HideColumns(dataGV, new[] { "EmployessName_NoSign"});

                Utils.SetGridHeaders(employeeDeductionGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"EmployeeCode", "Mã NV" },
                    {"EmployeeName", "Tên NV" },
                    {"DeductionDate", "Ngày Chi" },
                    {"Amount", "Số Tiền" },
                    {"Note", "Ghi Chú" }
                });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"FullName", "Tên Nhân Viên" },
                    {"EmployeeCode", "Mã NV" },
                    {"PositionName", "Chức Vụ" },
                    {"ContractTypeName", "Loại Hợp Đồng" }
                });

                Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                    {"CreateAt", "Ngày thay đổi" },
                    {"ACtionBy", "Người thay đổi" },
                    {"Description", "Hành động" },
                    {"DeductionDate", "Ngày" },
                    {"Amount", "Số Tiền" }
                });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"EmployeeCode", 60},
                    {"FullName", 160}
                });

                Utils.SetGridWidths(employeeDeductionGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"EmployeeCode", 60},
                    {"EmployeeName", 160},
                    {"DeductionDate", 70},
                    {"Amount", 60},
                    {"Note", 200}
                });

                Utils.SetGridWidths(log_GV, new System.Collections.Generic.Dictionary<string, int> {
                    {"CreateAt", 120},
                    {"ActionBy", 150},
                    {"DeductionDate", 80},
                    {"Amount", 80},
                });

                int count = 0;
                mEmployeeDeduction_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["EmployeeName"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["DeductionDate"].SetOrdinal(count++);                
                mEmployeeDeduction_dt.Columns["Amount"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["Note"].SetOrdinal(count++);

                

                
                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                 //   UpdateAllowancetUI(0);
                }
                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                employeeDeductionGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                log_GV.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                employeeDeductionGV.SelectionChanged += this.allowanceGV_CellClick;
                employeeDeductionGV.Click += this.allowanceGV_CellClick;

            }
            catch
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                await Task.Delay(100);
                loadingOverlay.Hide();
            } 
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

        private void allowanceGV_CellClick(object sender, EventArgs e)
        {
            if (employeeDeductionGV.CurrentRow == null|| _isUpdatingUI) return;
            int rowIndex = employeeDeductionGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            _isUpdatingUI = true;
            dataGV.ClearSelection();
            Edit_btn_Click(null, null);
            UpdateRightUI(rowIndex);
            _isUpdatingUI = false;
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null || _isUpdatingUI) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;
            _isUpdatingUI = true;
            newCustomerBtn_Click(null, null);
            UpdateEmployeeDeductionUI(rowIndex);
            UpdateRightUI(rowIndex);
            _isUpdatingUI = false;
        }

        private void UpdateEmployeeDeductionUI(int index)
        {
            _isUpdatingUI = true;
            employeeDeductionGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mEmployeeDeduction_dt);
            string filterStr = "";
            if (CEP_In_Month_cb.Checked)
            {
                DateTime dt = monthYearDtp.Value;
                DateTime fromDate = new DateTime(dt.Year, dt.Month, 1);
                DateTime toDate = fromDate.AddMonths(1);

                filterStr = $"DeductionDate >= #{fromDate:MM/dd/yyyy}# AND DeductionDate < #{toDate:MM/dd/yyyy}#";


            }
            else
            {                
                filterStr = $"EmployeeCode = '{employeeCode}'";
            }
            dv.RowFilter = filterStr;
            employeeDeductionGV.DataSource = dv;

            int rowCount = dv.Count;

            // sum cột Amount
            decimal totalAmount = 0;
            foreach (DataRowView row in dv)
            {
                if (row["Amount"] != DBNull.Value)
                    totalAmount += Convert.ToDecimal(row["Amount"]);
            }

            count_label.Text = rowCount.ToString();
            totalCEP_label.Text = totalAmount.ToString("N0");

            mDeductionLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
            _isUpdatingUI = false;
        }
        private void UpdateRightUI(int index)
        {
            if (!isNewState)
            {
                var cells = employeeDeductionGV.Rows[index].Cells;
                int employeeDeductionID = Convert.ToInt32(cells["EmployeeDeductionID"].Value);
                DateTime deductionDate = Convert.ToDateTime(cells["DeductionDate"].Value);
                int amount = Convert.ToInt32(cells["Amount"].Value);
                string note = cells["Note"].Value.ToString();

                this.employeeDeductionID_tb.Text = employeeDeductionID.ToString();
                // month_cbb.SelectedItem = deductionDate.Month;
                // year_tb.Text = deductionDate.Year.ToString();
                amount_tb.Text = amount.ToString();
                note_tb.Text = note;
                employeeName_tb.Text = cells["EmployeeName"].Value.ToString();
                status_lb.Text = "";
            }
            else
            {
                var cells = dataGV.Rows[index].Cells;
                employeeName_tb.Text = cells["FullName"].Value.ToString();
                status_lb.Text = "";
            }
        }
        
        private async void updateData(int employeeDeductionID, string employeeCode, DateTime deductionDate, int amount, string note)
        {
            foreach (DataGridViewRow row in employeeDeductionGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["EmployeeDeductionID"].Value);
                if (id.CompareTo(employeeDeductionID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.updateEmployeeDeductionAsync(employeeDeductionID, employeeCode, deductionDate, amount, note);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit {row.Cells["DeductionDate"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value}: Success", deductionDate.Date, amount, note);

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["DeductionDate"].Value = deductionDate.Date;
                                row.Cells["Amount"].Value = amount;
                                row.Cells["Note"].Value = note;

                                

                                DataRowView drv = row.DataBoundItem as DataRowView;
                                if (drv != null)
                                {
                                    DataRow dr = drv.Row;
                                    SQLStore_QLNS.Instance.addOrUpdateDeduction(deductionDate.Year, dr);
                                }
                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit {row.Cells["DeductionDate"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value}: Fail", deductionDate.Date, amount, note);
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit {row.Cells["DeductionDate"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value}: Fail Exception: {ex.Message}", deductionDate.Date, amount, note);
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                    }
                    break;
                }
            }
        }

        private async void createNew(string employeeCode, DateTime deductionDate, int amount, string note)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int employeeDeductionID = await SQLManager_QLNS.Instance.insertEmployeeDeductionAsync(employeeCode, DeductionTypeCode, deductionDate, amount, note);
                    if (employeeDeductionID > 0)
                    {
                        DataRow[] rows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");
                        DataRow drToAdd = mEmployeeDeduction_dt.NewRow();

                        drToAdd["EmployeeDeductionID"] = employeeDeductionID;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["DeductionTypeCode"] = DeductionTypeCode;
                        drToAdd["DeductionTypeName"] = mDeductionName;
                        drToAdd["DeductionDate"] = deductionDate.Date;
                        drToAdd["Amount"] = amount;
                        drToAdd["Note"] = note;
                        drToAdd["EmployeeName"] = rows[0]["FullName"];

                        mEmployeeDeduction_dt.Rows.Add(drToAdd);
                        mEmployeeDeduction_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Success", deductionDate.Date, amount, note);
                        SQLStore_QLNS.Instance.addOrUpdateDeduction(deductionDate.Year, drToAdd);
                        newCustomerBtn_Click(null, null);
                    }
                    else
                    {
                        _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Fail", deductionDate.Date, amount, note);
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Fail Exception: " + ex.Message, deductionDate.Date, amount, note);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

            }
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(amount_tb.Text) || dataGV.CurrentRow == null)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime selectDate = monthYearDtp.Value;
            DateTime deductionDate = new DateTime(selectDate.Year, selectDate.Month, 15);
                        
            int amount = Convert.ToInt32(amount_tb.Text);
            string note = note_tb.Text;

            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(selectDate.Month, selectDate.Year);
            if (isLock)
            {
                MessageBox.Show("Tháng " + selectDate.Month + "/" + selectDate.Year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string employeeCode = "";
            if (employeeDeductionID_tb.Text.Length != 0)
                employeeCode = Convert.ToString(employeeDeductionGV.CurrentRow.Cells["EmployeeCode"].Value);
            else
                employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);

            if (IsDuplicateDeductionMonth(employeeDeductionGV, employeeCode, selectDate.Month , selectDate.Year, employeeDeductionID_tb.Text.Length != 0 ? false : true))
            {
                MessageBox.Show("Tháng " + selectDate.Month + "/" + selectDate.Year + " Đã Có Data", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (employeeDeductionID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(employeeDeductionID_tb.Text), employeeCode, deductionDate, amount, note);
            else
                createNew(employeeCode, deductionDate, amount, note);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (employeeDeductionGV.SelectedRows.Count == 0) return;

            string employeeDeductionID = employeeDeductionID_tb.Text;

            foreach (DataGridViewRow row in employeeDeductionGV.Rows)
            {
                string id = row.Cells["EmployeeDeductionID"].Value.ToString();
                string employeeCode = row.Cells["EmployeeCode"].Value.ToString();
                int amount = Convert.ToInt32(row.Cells["Amount"].Value);
                if (id.CompareTo(employeeDeductionID) == 0)
                {
                    DateTime deductionDate = Convert.ToDateTime(row.Cells["DeductionDate"].Value);
                    bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(deductionDate.Month, deductionDate.Year);
                    if (isLock)
                    {
                        MessageBox.Show("Tháng " + deductionDate.Month + "/" + deductionDate.Year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.deleteEmployeeDeductionAsync(Convert.ToInt32(employeeDeductionID));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Success", deductionDate.Date, amount, "Delete");

                                int delRowInd = row.Index;
                                employeeDeductionGV.Rows.Remove(row);
                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Fail", deductionDate.Date, amount, "Delete");
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Fail Exception: " + ex.Message, deductionDate.Date, amount, "Delete");
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                        break;
                    }
                }
            }
        }

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            employeeDeductionID_tb.Text = "";

            status_lb.Text = "";
            if(!search_tb.Focused)
                amount_tb.Focus();

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = false;// true;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            SetUIReadOnly(false);
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            SetUIReadOnly(true);
            allowanceGV_CellClick(null, null);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = false;// true;
            LuuThayDoiBtn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            SetUIReadOnly(false);
        }

        private bool IsDuplicateDeductionMonth(DataGridView grid, string employeeCode, int month, int year, bool isNew)
        {
            if (grid == null || grid.Rows.Count == 0) return false;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                string code = Convert.ToString(row.Cells["EmployeeCode"].Value);
                if (code != employeeCode)
                    continue;

                if (row.Cells["DeductionDate"].Value == null)
                    continue;

                if (DateTime.TryParse(row.Cells["DeductionDate"].Value.ToString(), out DateTime existingDate))
                {
                    if (existingDate.Month == month && existingDate.Year == year)
                    {
                        // Nếu đang thêm mới → báo trùng ngay
                        if (isNew) return true;

                        // Nếu đang cập nhật → bỏ qua dòng đang chọn
                        if (grid.CurrentRow != null && row.Index == grid.CurrentRow.Index)
                            continue;

                        return true;
                    }
                }
            }

            return false;
        }

        private async void monthYearDtp_ValueChanged(object sender, EventArgs e)
        {
            if (!isNewState)
            {
                DateTime curTime = monthYearDtp.Value;
                bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(curTime.Month, curTime.Year);
                LuuThayDoiBtn.Visible = !isLock;
                edit_btn.Visible = !isLock;
            }
            _monthYearDebounceTimer.Stop();
            _monthYearDebounceTimer.Start();
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            amount_tb.ReadOnly = isReadOnly;
            note_tb.ReadOnly = isReadOnly;
        }

        private void Search_tb_TextChanged(object sender, EventArgs e)
        {
            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower())
                     .Replace("'", "''"); // tránh lỗi cú pháp '

            DataTable dt = dataGV.DataSource as DataTable;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            dv.RowFilter = $"[EmployessName_NoSign] LIKE '%{keyword}%'";
        }

        private void MonthYearDebounceTimer_Tick(object sender, EventArgs e)
        {
            _monthYearDebounceTimer.Stop();
            HandleMonthYearChanged();
        }

        private async void HandleMonthYearChanged()
        {
            int year = monthYearDtp.Value.Year;

            if (year != mCurYear)
            {
                await Task.Delay(50);
                LoadingOverlay loadingOverlay = new LoadingOverlay(this);
                loadingOverlay.Show();
                await Task.Delay(200);

                var employeeDeductionAsync = SQLStore_QLNS.Instance.GetDeductionAsync(year, DeductionTypeCode);
                var EmployeeDeductionLogTask = SQLStore_QLNS.Instance.GetEmployeeDeductionLogAsync(year, DeductionTypeCode);
                await Task.WhenAll(employeeDeductionAsync, EmployeeDeductionLogTask);
                mEmployeeDeduction_dt = employeeDeductionAsync.Result;
                mDeductionLogDV = new DataView(EmployeeDeductionLogTask.Result);

                mEmployeeDeduction_dt.Columns.Add("EmployeeName", typeof(string));
                foreach (DataRow dr in mEmployeeDeduction_dt.Rows)
                {
                    string employeeCode = dr["EmployeeCode"].ToString();
                    DataRow[] rows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");
                    if (rows.Length > 0)
                        dr["EmployeeName"] = rows[0]["FullName"];
                }

                log_GV.DataSource = mDeductionLogDV;
                employeeDeductionGV.DataSource = mEmployeeDeduction_dt;
                mCurYear = year;

                if (dataGV.CurrentRow != null)
                {
                    int selectedIndex = dataGV.CurrentRow?.Index ?? -1;
                    UpdateEmployeeDeductionUI(selectedIndex);
                }

                monthYearLabel.Text = $"Năm {year}";

                await Task.Delay(100);
                loadingOverlay.Hide();
            }

            dataGV_CellClick(null, null);
        }

        private void CEP_In_Month_cb_CheckedChanged(object sender, EventArgs e)
        {
            dataGV_CellClick(null, null);            
        }
    }
}
