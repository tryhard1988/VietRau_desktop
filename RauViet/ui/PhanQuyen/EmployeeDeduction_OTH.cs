using RauViet.classes;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{    
    public partial class EmployeeDeduction_OTH : Form
    {
        private Timer _monthYearDebounceTimer;
        private DataTable mEmployeeDeduction_dt, mEmployee_dt;
        private DataView mDeductionLogDV;
        private const string DeductionTypeCode = "OTH";
        private string mDeductionname = "";
        bool isNewState = false;
        int currMonth = -1;
        int currYear = -1;
        public EmployeeDeduction_OTH()
        {
            InitializeComponent();
            this.KeyPreview = true;
            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "MM/yyyy";
            monthYearDtp.ShowUpDown = true;
            monthYearDtp.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            int countTab = 0;
            deductionDate_dtp.TabIndex = countTab++; deductionDate_dtp.TabStop = true;
            amount_tb.TabIndex = countTab++; amount_tb.TabStop = true;
            note_tb.TabIndex = countTab++; note_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            employeeDeductionGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            employeeDeductionGV.MultiSelect = false;

            status_lb.Text = "";

            deductionDate_dtp.Format = DateTimePickerFormat.Custom;
            deductionDate_dtp.CustomFormat = "dd";
            deductionDate_dtp.ShowUpDown = true;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            
            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);

            this.KeyDown += EmployeeDeduction_OTH_KeyDown;

            search_tb.TextChanged += Search_tb_TextChanged;
            this.Load += OvertimeAttendace_Load;
        }

        private void OvertimeAttendace_Load(object sender, EventArgs e)
        {
            _monthYearDebounceTimer = new Timer();
            _monthYearDebounceTimer.Interval = 500;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
        }

        private void EmployeeDeduction_OTH_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int year = monthYearDtp.Value.Year;

                SQLStore_QLNS.Instance.removeDeduction(year);
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
                monthYearDtp.ValueChanged -= monthYearDtp_ValueChanged;
                employeeDeductionGV.SelectionChanged -= this.allowanceGV_CellClick;
                dataGV.SelectionChanged -= this.dataGV_CellClick;
                int month = monthYearDtp.Value.Month;
                int year = monthYearDtp.Value.Year;
                string[] keepColumns = { "EmployeeCode", "FullName", "PositionName", "ContractTypeName", "EmployessName_NoSign" };
                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
                var employeeDeductionAsync = SQLStore_QLNS.Instance.GetDeductionAsync(month, year, DeductionTypeCode);
                var deductionNameAsync = SQLStore_QLNS.Instance.GetDeductionNameAsync(DeductionTypeCode);
                var EmployeeDeductionLogTask = SQLStore_QLNS.Instance.GetEmployeeDeductionLogAsync(month, year, DeductionTypeCode);
                await Task.WhenAll(employeesTask, employeeDeductionAsync, deductionNameAsync, EmployeeDeductionLogTask);
                mEmployee_dt = employeesTask.Result;
                mEmployeeDeduction_dt = employeeDeductionAsync.Result;
                mDeductionname = deductionNameAsync.Result;
                mDeductionLogDV = new DataView(EmployeeDeductionLogTask.Result);
                currMonth = month;
                currYear = year;

                monthYearLabel.Text = $"Tháng {month}/{year}";

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

                employeeDeductionGV.DataSource = mEmployeeDeduction_dt;

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = mEmployee_dt;
                log_GV.DataSource = mDeductionLogDV;

                Utils.HideColumns(employeeDeductionGV, new[] { "EmployeeDeductionID", "DeductionTypeCode", "DeductionTypeName" });
                Utils.HideColumns(log_GV, new[] { "LogID", "EmployeeCode", "DeductionTypeCode" });
                Utils.HideColumns(dataGV, new[] { "EmployessName_NoSign"});

                int count = 0;
                mEmployeeDeduction_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["EmployeeName"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["DeductionDate"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["Amount"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["Note"].SetOrdinal(count++);

                employeeDeductionGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                employeeDeductionGV.Columns["EmployeeName"].HeaderText = "Tên NV";
                employeeDeductionGV.Columns["DeductionDate"].HeaderText = "Ngày Chi";
                employeeDeductionGV.Columns["Amount"].HeaderText = "Số Tiền";
                employeeDeductionGV.Columns["Note"].HeaderText = "Ghi Chú";

                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";

                dataGV.Columns["EmployeeCode"].Width = 60;
                dataGV.Columns["FullName"].Width = 160;

                employeeDeductionGV.Columns["DeductionDate"].Width = 70;
                employeeDeductionGV.Columns["Amount"].Width = 60;
                employeeDeductionGV.Columns["Note"].Width = 200;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                 //   UpdateAllowancetUI(0);
                }


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                employeeDeductionGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                monthYearDtp.ValueChanged += monthYearDtp_ValueChanged;

                bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
                newCustomerBtn.Visible = !isLock;
                edit_btn.Visible = !isLock;

                log_GV.Columns["CreateAt"].Width = 120;
                log_GV.Columns["ActionBy"].Width = 150;
                log_GV.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["DeductionDate"].Width = 80;
                log_GV.Columns["Amount"].Width = 80;
                log_GV.Columns["CreateAt"].HeaderText = "Thời điểm thay đổi";
                log_GV.Columns["ACtionBy"].HeaderText = "Người thay đổi";
                log_GV.Columns["Description"].HeaderText = "Hành động";
                log_GV.Columns["DeductionDate"].HeaderText = "Ngày";
                log_GV.Columns["Amount"].HeaderText = "Số Tiền";

                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGV.SelectionChanged += this.dataGV_CellClick;
                employeeDeductionGV.SelectionChanged += this.allowanceGV_CellClick;
                allowanceGV_CellClick(null, null);
            }
            catch (Exception ex)
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
                Console.WriteLine("Err" + ex.Message);
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
            if (employeeDeductionGV.CurrentRow == null) return;
            int rowIndex = employeeDeductionGV.CurrentRow.Index;
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


            employeeDeductionGV.ClearSelection();
            newCustomerBtn_Click(null, null);
            UpdateEmployeeDeductionUI(rowIndex);
            UpdateRightUI(rowIndex);
        }

        private void UpdateEmployeeDeductionUI(int index)
        {
            amount_tb.Text = "0";

            employeeDeductionGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            //DataView dv = new DataView(mEmployeeDeduction_dt);
            //dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            //employeeDeductionGV.DataSource = dv;
            mDeductionLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
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
                deductionDate_dtp.Value = deductionDate;
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

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["DeductionDate"].Value = deductionDate.Date;
                                row.Cells["Amount"].Value = amount;
                                row.Cells["Note"].Value = note;

                                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit {row.Cells["DeductionDate"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value}: Success", deductionDate.Date, amount, note);

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
                            _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit {row.Cells["DeductionDate"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value}: Fail Exception {ex.Message}", deductionDate.Date, amount, note);
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
                        drToAdd["DeductionTypeCode"] = DeductionTypeCode;
                        drToAdd["DeductionTypeName"] = mDeductionname;
                        drToAdd["EmployeeCode"] = employeeCode;                        
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
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                        _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Fail", deductionDate.Date, amount, note);
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Fail Exception: " + ex.Message, deductionDate.Date, amount, note);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                    Console.WriteLine("err" + ex.Message);
                }

            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(amount_tb.Text) || dataGV.CurrentRow == null)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;
            DateTime deductionDate = new DateTime(year, month, deductionDate_dtp.Value.Day);

            //if (deductionDate.Year != year || month != deductionDate.Month || month != currMonth || year != currYear)
            //{
            //    MessageBox.Show("Tháng hoặc Năm có vẫn đề", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            
            int amount = Convert.ToInt32(amount_tb.Text);
            string note = note_tb.Text;

            if (employeeDeductionID_tb.Text.Length != 0)
            {
                string employeeCode = Convert.ToString(employeeDeductionGV.CurrentRow.Cells["EmployeeCode"].Value);
                updateData(Convert.ToInt32(employeeDeductionID_tb.Text), employeeCode, deductionDate, amount, note);
            }
            else
            {
                string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
                createNew(employeeCode, deductionDate, amount, note);
            }

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (employeeDeductionGV.SelectedRows.Count == 0) return;

            string employeeDeductionID = employeeDeductionID_tb.Text;

            foreach (DataGridViewRow row in employeeDeductionGV.Rows)
            {
                string employeeCode = row.Cells["EmployeeCode"].Value.ToString();
                string id = row.Cells["EmployeeDeductionID"].Value.ToString();
                DateTime deductionDate = Convert.ToDateTime(row.Cells["DeductionDate"].Value);
                int amount = Convert.ToInt32(row.Cells["Amount"].Value);
                if (id.CompareTo(employeeDeductionID) == 0)
                {
                    
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

                                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Delete: Success", deductionDate.Date, amount, "Delete");

                                int delRowInd = row.Index;
                                employeeDeductionGV.Rows.Remove(row);
                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Delete: Fail", deductionDate.Date, amount, "Delete");
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Delete: Fail Exception: " + ex.Message, deductionDate.Date, amount, "Delete");
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
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;
            employeeDeductionID_tb.Text = "";
            deductionDate_dtp.Value = new DateTime(year, month, deductionDate_dtp.Value.Day);

            status_lb.Text = "";
            info_gb.BackColor = Color.Green;

            if(!search_tb.Focused)
                deductionDate_dtp.Focus();

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

        private void SetUIReadOnly(bool isReadOnly)
        {
            deductionDate_dtp.Enabled = !isReadOnly;
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
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);
            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;

            var employeeDeductionAsync = SQLStore_QLNS.Instance.GetDeductionAsync(month, year, DeductionTypeCode);
            var EmployeeDeductionLogTask = SQLStore_QLNS.Instance.GetEmployeeDeductionLogAsync(month, year, DeductionTypeCode);

            await Task.WhenAll(employeeDeductionAsync, EmployeeDeductionLogTask);
            mEmployeeDeduction_dt = employeeDeductionAsync.Result;
            mDeductionLogDV = new DataView(EmployeeDeductionLogTask.Result);
            log_GV.DataSource = mDeductionLogDV;
            currMonth = month;
            currYear = year;

            if (dataGV.CurrentRow != null)
            {
                int selectedIndex = dataGV.CurrentRow?.Index ?? -1;
                UpdateEmployeeDeductionUI(selectedIndex);
            }

            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
            newCustomerBtn.Visible = !isLock;
            edit_btn.Visible = !isLock;

            monthYearLabel.Text = $"Tháng {month}/{year}";

            await Task.Delay(100);
            loadingOverlay.Hide();
        }
    }
}
