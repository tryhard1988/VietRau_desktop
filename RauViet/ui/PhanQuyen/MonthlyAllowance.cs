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
    public partial class MonthlyAllowance : Form
    {
        private Timer _monthYearDebounceTimer;
        private DataTable mMonthlyAllowance_dt, mAllowanceType_dt, mEmployee_dt;
        private DataView mlog_DV;
        bool isNewState = false;
        int mCurrentMonth = -1;
        int mCurrentYear = -1;
        
        public MonthlyAllowance()
        {
            InitializeComponent();
            this.KeyPreview = true;
            Utils.SetTabStopRecursive(this, false);

            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "MM/yyyy";
            monthYearDtp.ShowUpDown = true;
            monthYearDtp.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            int countTab = 0;
            allowanceType_cbb.TabIndex = countTab++; allowanceType_cbb.TabStop = true;
            amount_tb.TabIndex = countTab++; amount_tb.TabStop = true;
            note_tb.TabIndex = countTab++; note_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            allowanceGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            allowanceGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            
            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            monthYearDtp.ValueChanged += MonthYearDtp_ValueChanged;            
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
            this.KeyDown += MonthlyAllowance_KeyDown;

            search_tb.TextChanged += Search_tb_TextChanged;
            this.Load += OvertimeAttendace_Load;
        }

        private void OvertimeAttendace_Load(object sender, EventArgs e)
        {
            _monthYearDebounceTimer = new Timer();
            _monthYearDebounceTimer.Interval = 500;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
        }

        private void MonthlyAllowance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
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
                dataGV.SelectionChanged -= this.dataGV_CellClick;
                allowanceGV.SelectionChanged -= this.allowanceGV_CellClick;
                monthYearDtp.ValueChanged -= monthYearDtp_ValueChanged;
                allowanceType_cbb.SelectedIndexChanged -= AllowanceType_cbb_SelectedIndexChanged;
                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;

                string[] keepColumns = { "EmployeeCode", "FullName", "DepartmentName", "PositionName", "ContractTypeName", "EmployessName_NoSign" };
                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
                var monthlyAllowanceAsync = SQLManager_QLNS.Instance.GetMonthlyAllowanceAsybc(month, year);
                var allowanceTypeAsync = SQLManager_QLNS.Instance.GetAllowanceTypeAsync("ONCE");                
                var monthlyAllowanceLogTask = SQLManager_QLNS.Instance.GetMonthlyAllowanceLogAsync(month, year);

                await Task.WhenAll(employeesTask, monthlyAllowanceAsync, allowanceTypeAsync, monthlyAllowanceLogTask);
                mEmployee_dt = employeesTask.Result;
                mMonthlyAllowance_dt = monthlyAllowanceAsync.Result;
                mAllowanceType_dt = allowanceTypeAsync.Result;
                mlog_DV = new DataView(monthlyAllowanceLogTask.Result);
                mCurrentMonth = month;
                mCurrentYear = year;

                monthYearLabel.Text = $"Tháng {month}/{year}";

                mMonthlyAllowance_dt.Columns.Add(new DataColumn("EmployeeName", typeof(string)));
                mMonthlyAllowance_dt.Columns.Add(new DataColumn("AllowanceName", typeof(string)));
                mMonthlyAllowance_dt.Columns.Add(new DataColumn("Date", typeof(string)));
                foreach (DataRow dr in mMonthlyAllowance_dt.Rows)
                {
                    int allowanceTypeID = Convert.ToInt32(dr["AllowanceTypeID"]);
                    string employeeCode = Convert.ToString(dr["EmployeeCode"]);
                    DataRow[] applyScopeRows = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}");
                    DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");

                    if (applyScopeRows.Length > 0)
                        dr["AllowanceName"] = applyScopeRows[0]["AllowanceName"].ToString();
                    if(employeeRows.Length > 0)
                        dr["EmployeeName"] = employeeRows[0]["FullName"].ToString();

                    dr["Date"] = dr["Month"] + "/" + dr["Year"];
                }

                foreach (DataRow dr in mEmployee_dt.Rows)
                {
                    if (dr["PositionName"] == DBNull.Value) dr["PositionName"] = "";
                    if (dr["ContractTypeName"] == DBNull.Value) dr["ContractTypeName"] = "";
                }


                DataView dv = new DataView(mAllowanceType_dt);
                dv.RowFilter = "IsActive = 1";
                allowanceType_cbb.DataSource = dv;
                allowanceType_cbb.DisplayMember = "AllowanceName";
                allowanceType_cbb.ValueMember = "AllowanceTypeID";

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = mEmployee_dt;
                allowanceGV.DataSource = mMonthlyAllowance_dt;
                log_GV.DataSource = mlog_DV;

                Utils.HideColumns(dataGV, new[] { "EmployessName_NoSign"});
                Utils.HideColumns(allowanceGV, new[] { "MonthlyAllowanceID", "AllowanceTypeID", "Month", "Year" });

                Utils.SetGridHeaders(allowanceGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"MonthlyAllowanceID", "ID" },
                    {"Date", "Tháng P.C" },
                    {"AllowanceName", "Loại Phụ Cấp" },
                    {"Amount", "Số Tiền" },
                    {"Note", "Ghi Chú" },
                    {"EmployeeCode", "Mã NV" },
                    {"EmployeeName", "Tên NV" }
                });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"FullName", "Tên Nhân Viên" },
                    {"EmployeeCode", "Mã NV" },
                    {"PositionName", "Chức Vụ" },
                    {"ContractTypeName", "Loại Hợp Đồng" },
                    {"DepartmentName", "Phòng Ban" }
                 });

                Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                    {"CreatedAt", "Ngày thay đổi" },
                    {"ACtionBy", "Người thay đổi" },
                    {"AllowanceName", "Loại Phụ Cấp" },
                    {"Description", "Hành động" },
                    {"Amount", "Số Tiền" }
                });

                Utils.SetGridOrdinal(mMonthlyAllowance_dt, new[] { "EmployeeCode", "EmployeeName", "Date", "AllowanceName", "Amount", "Note"});

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"EmployeeCode", 60},
                    {"FullName", 160}
                });

                Utils.SetGridWidths(allowanceGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"EmployeeCode", 60},
                    {"EmployeeName", 160},
                    {"Date", 60},
                    {"Amount", 70},
                    {"Note", 120}
                });

                Utils.SetGridWidths(log_GV, new System.Collections.Generic.Dictionary<string, int> {
                    {"CreatedAt", 120},
                    {"ActionBy", 150},
                    {"AllowanceName", 100},
                    {"Amount", 80}
                });

                Utils.SetGridFormat_NO(allowanceGV, "Amount");
                Utils.SetGridWidth(allowanceGV, "AllowanceName", DataGridViewAutoSizeColumnMode.AllCells);
                Utils.SetGridWidth(log_GV, "Description", DataGridViewAutoSizeColumnMode.Fill);

                dataGV.SelectionChanged += this.dataGV_CellClick;
                allowanceGV.SelectionChanged += this.allowanceGV_CellClick;
                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                }

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                allowanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                MonthYearDtp_ValueChanged(null, null);

                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                monthYearDtp.ValueChanged += monthYearDtp_ValueChanged;
                allowanceType_cbb.SelectedIndexChanged += AllowanceType_cbb_SelectedIndexChanged;
                AllowanceType_cbb_SelectedIndexChanged(null, null);
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
            if (allowanceGV.CurrentRow == null) return;
            int rowIndex = allowanceGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            dataGV.ClearSelection();
            Edit_btn_Click(null, null);
            UpdateRightUI(rowIndex);
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;

            allowanceGV.ClearSelection();
            newCustomerBtn_Click(null, null);
            UpdateAllowancetUI(rowIndex);
        }

        private void UpdateAllowancetUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            //DataView dv = new DataView(mMonthlyAllowance_dt);
            //dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            // allowanceGV.DataSource = dv;
            UpdateRightUI(index);
            mlog_DV.RowFilter = $"EmployeeCode = '{employeeCode}'";
        }
        private void UpdateRightUI(int index)
        {
            if (isNewState)
            {
                var cells = dataGV.Rows[index].Cells;
                employeeName_tb.Text = cells["FullName"].Value.ToString();
            }
            else
            {
                var cells = allowanceGV.Rows[index].Cells;
                string monthlyAllowanceID = cells["MonthlyAllowanceID"].Value.ToString();
                string allowanceTypeID = Convert.ToString(cells["AllowanceTypeID"].Value);
                int month = Convert.ToInt32(cells["Month"].Value);
                int year = Convert.ToInt32(cells["Year"].Value);
                decimal amount = Convert.ToDecimal(cells["Amount"].Value);
                string note = cells["Note"].Value.ToString();

                DataRow row = mAllowanceType_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("AllowanceTypeID") == Convert.ToInt32(allowanceTypeID));
                if (row == null) return;

                int giaPhuCap = row.Field<int?>("AllowancePrice") ?? 1;

                employeeName_tb.Text = cells["EmployeeName"].Value.ToString();
                this.monthlyAllowanceID_tb.Text = monthlyAllowanceID;
                amount_tb.Text = (amount / giaPhuCap).ToString("0.##", CultureInfo.InvariantCulture);
                note_tb.Text = note;
                allowanceType_cbb.SelectedValue = allowanceTypeID;
            }
            status_lb.Text = "";
        }
        
        private async void updateData(int monthlyAllowanceID, string employeeCode, int month, int year, int amount, int allowanceTypeID, string note)
        {
            foreach (DataGridViewRow row in allowanceGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["MonthlyAllowanceID"].Value);
                if (id.CompareTo(monthlyAllowanceID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {                        
                        try
                        {
                            string allowanceName = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}")[0]["AllowanceName"].ToString();
                            bool isScussess = await SQLManager_QLNS.Instance.updateMonthlyAllowanceAsync(monthlyAllowanceID, employeeCode, month, year, amount, allowanceTypeID, note);
                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;                                
                                _ = SQLManager_QLNS.Instance.InsertMonthlyAllowanceLogAsync(employeeCode, allowanceName,
                                    $"Edit Success {row.Cells["AllowanceName"].Value} - {row.Cells["Month"].Value}/{row.Cells["Year"].Value} - {row.Cells["Note"].Value} - {row.Cells["Amount"].Value}",
                                    month, year, amount, note);

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["AllowanceTypeID"].Value = allowanceTypeID;
                                row.Cells["Amount"].Value = amount;
                                row.Cells["Month"].Value = month;
                                row.Cells["Year"].Value = year;
                                row.Cells["Date"].Value = month + "/" + year;
                                row.Cells["Note"].Value = note;
                                row.Cells["AllowanceName"].Value = allowanceName;
                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertMonthlyAllowanceLogAsync(employeeCode, allowanceName,
                                    $"Edit Fail {row.Cells["AllowanceName"].Value} - {row.Cells["Month"].Value}/{row.Cells["Year"].Value} - {row.Cells["Note"].Value} - {row.Cells["Amount"].Value}",
                                    month, year, amount, note);
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager_QLNS.Instance.InsertMonthlyAllowanceLogAsync(employeeCode, allowanceTypeID.ToString(),$"Edit Fail Exception: {ex.Message}",
                                    month, year, amount, note);
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                    }
                    break;
                }
            }
        }

        private async void createNew(string employeeCode, int month, int year, int amount, int allowanceTypeID, string note)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int monthlyAllowanceID = await SQLManager_QLNS.Instance.insertMonthlyAllowanceAsync(employeeCode, month, year, amount, allowanceTypeID, note);
                    string allowanceName = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}")[0]["AllowanceName"].ToString();
                    if (monthlyAllowanceID > 0)
                    {
                        DataRow[] rows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");
                        DataRow drToAdd = mMonthlyAllowance_dt.NewRow();

                        drToAdd["MonthlyAllowanceID"] = monthlyAllowanceID;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["AllowanceTypeID"] = allowanceTypeID;
                        drToAdd["Amount"] = amount;
                        drToAdd["Month"] = month;
                        drToAdd["Year"] = year;
                        drToAdd["Date"] = month + "/" + year;
                        drToAdd["Note"] = note;
                        drToAdd["AllowanceName"] = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}")[0]["AllowanceName"].ToString();
                        drToAdd["EmployeeName"] = rows[0]["FullName"];
                        mMonthlyAllowance_dt.Rows.Add(drToAdd);
                        mMonthlyAllowance_dt.AcceptChanges();

                        _ = SQLManager_QLNS.Instance.InsertMonthlyAllowanceLogAsync(employeeCode, allowanceName, $"Create Success", month, year, amount, note);

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        newCustomerBtn_Click(null, null);
                    }
                    else
                    {
                        _ = SQLManager_QLNS.Instance.InsertMonthlyAllowanceLogAsync(employeeCode, allowanceName, $"Create Fail", month, year, amount, note);
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager_QLNS.Instance.InsertMonthlyAllowanceLogAsync(employeeCode, allowanceTypeID.ToString(), $"Create Fail Exception: {ex.Message}", month, year, amount, note);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

            }
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if (allowanceType_cbb.SelectedValue == null || string.IsNullOrEmpty(amount_tb.Text) || 
                dataGV.CurrentRow == null)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
            decimal amount = Utils.ParseDecimalSmart(amount_tb.Text);
            int giaPhuCap = Convert.ToInt32(giaPhuCap_tb.Text);
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;
            int allowanceTypeID = Convert.ToInt32(allowanceType_cbb.SelectedValue);
            string note= note_tb.Text;

            if (mCurrentMonth != month || mCurrentYear != year)
            {
                MessageBox.Show("Tháng " + month + "/" + year + " có vẫn đề.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
            if (isLock)
            {
                MessageBox.Show("Tháng " + month + "/" + year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (monthlyAllowanceID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(monthlyAllowanceID_tb.Text), employeeCode, month, year, Convert.ToInt32(amount * giaPhuCap), allowanceTypeID, note);
            else
                createNew(employeeCode, month, year, Convert.ToInt32(amount * giaPhuCap), allowanceTypeID, note);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (allowanceGV.SelectedRows.Count == 0) return;

            string monthlyAllowanceID = monthlyAllowanceID_tb.Text;

            foreach (DataGridViewRow row in allowanceGV.Rows)
            {
                string id = row.Cells["MonthlyAllowanceID"].Value.ToString();
                string employeeCode = row.Cells["EmployeeCode"].Value.ToString();
                string allowanceName = row.Cells["AllowanceName"].Value.ToString();
                int month = Convert.ToInt32(row.Cells["Month"].Value);
                int year = Convert.ToInt32(row.Cells["Year"].Value);
                int amount = Convert.ToInt32(row.Cells["Amount"].Value);

                if (id.CompareTo(monthlyAllowanceID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.deleteMonthlyAllowanceAsync(Convert.ToInt32(monthlyAllowanceID));

                            if (isScussess == true)
                            {
                                _ = SQLManager_QLNS.Instance.InsertMonthlyAllowanceLogAsync(employeeCode, allowanceName, $"Delete Success", month, year, amount, "Delete");
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                int delRowInd = row.Index;
                                allowanceGV.Rows.Remove(row);
                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertMonthlyAllowanceLogAsync(employeeCode, allowanceName, $"Delete Fail", month, year, amount, "Delete");
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager_QLNS.Instance.InsertMonthlyAllowanceLogAsync(employeeCode, allowanceName, $"Delete Fail Exception: {ex.Message}", month, year, amount, "Delete");
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }




                    }
                    break;
                }
            }
        }

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            monthlyAllowanceID_tb.Text = "";
            amount_tb.Text = "";

            status_lb.Text = "";

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

        private async void MonthYearDtp_ValueChanged(object sender, EventArgs e)
        {
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;
            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
            if (readOnly_btn.Visible)
            {
                LuuThayDoiBtn.Visible = !isLock;
            }
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            allowanceType_cbb.Enabled = !isReadOnly;
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


        private void monthYearDtp_ValueChanged(object sender, EventArgs e)
        {
            // Mỗi lần thay đổi thì reset timer
            _monthYearDebounceTimer.Stop();
            _monthYearDebounceTimer.Start();
        }

        private void MonthYearDebounceTimer_Tick(object sender, EventArgs e)
        {
            _monthYearDebounceTimer.Stop();
            HandleMonthYearChanged();
        }

        private async void HandleMonthYearChanged()
        {
            ShowData();
        }

        private void AllowanceType_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allowanceType_cbb.SelectedValue == null) return;

            int allowanceTypeID = Convert.ToInt32(allowanceType_cbb.SelectedValue);

            DataRow row = mAllowanceType_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("AllowanceTypeID") == allowanceTypeID);

            if (row == null) return;

            int? allowancePrice = row.Field<int?>("AllowancePrice");
            giaPhuCap_tb.Text = (allowancePrice.HasValue ? allowancePrice : 1).ToString();
        }
    }
}
