using DocumentFormat.OpenXml.Bibliography;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{    
    public partial class EmployeeDeduction_VEG : Form
    {
        private Timer _monthYearDebounceTimer;
        private DataTable mEmployeeDeduction_dt, mEmployee_dt;
        private DataView mDeductionLogDV;
        private const string DeductionTypeCode = "VEG";
        private string mDeductionName = "";
        bool isNewState = false;
        int currMonth = -1;
        int currYear = -1;
        LoadingOverlay loadingOverlay;
        public EmployeeDeduction_VEG()
        {
            InitializeComponent();
            this.KeyPreview = true;
            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            deductionDate_dtp.TabIndex = countTab++; deductionDate_dtp.TabStop = true;
            amount_tb.TabIndex = countTab++; amount_tb.TabStop = true;
            note_tb.TabIndex = countTab++; note_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            monthYearDtp.Format = DateTimePickerFormat.Custom;
            monthYearDtp.CustomFormat = "MM/yyyy";
            monthYearDtp.ShowUpDown = true;
            monthYearDtp.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

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
            
            this.KeyDown += EmployeeDeduction_VEG_KeyDown;

            search_tb.TextChanged += Search_tb_TextChanged;
            this.Load += OvertimeAttendace_Load;

            loadFromExcel_btn.Click += LoadFromExcel_btn_Click;
        }


        private void OvertimeAttendace_Load(object sender, EventArgs e)
        {
            _monthYearDebounceTimer = new Timer();
            _monthYearDebounceTimer.Interval = 500;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
        }

        private void EmployeeDeduction_VEG_KeyDown(object sender, KeyEventArgs e)
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
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);
            try
            {
                dataGV.SelectionChanged -= this.dataGV_CellClick;
                employeeDeductionGV.SelectionChanged -= this.allowanceGV_CellClick;
                monthYearDtp.ValueChanged -= monthYearDtp_ValueChanged;
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
                mDeductionName = deductionNameAsync.Result;
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

                employeeDeductionGV.Columns["EmployeeDeductionID"].Visible = false;
               // employeeDeductionGV.Columns["EmployeeCode"].Visible = false;
                employeeDeductionGV.Columns["DeductionTypeCode"].Visible = false;
                employeeDeductionGV.Columns["DeductionTypeName"].Visible = false;

                log_GV.Columns["LogID"].Visible = false;
                log_GV.Columns["EmployeeCode"].Visible = false;
                log_GV.Columns["DeductionTypeCode"].Visible = false;

                int count = 0;
                mEmployeeDeduction_dt.Columns["EmployeeCode"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["EmployeeName"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["DeductionDate"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["Amount"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["Note"].SetOrdinal(count++);


                employeeDeductionGV.Columns["DeductionDate"].HeaderText = "Ngày Chi";
                employeeDeductionGV.Columns["Amount"].HeaderText = "Số Tiền";
                employeeDeductionGV.Columns["Note"].HeaderText = "Ghi Chú";
                employeeDeductionGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                employeeDeductionGV.Columns["EmployeeName"].HeaderText = "Tên NV";

                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";
                //dataGV.Columns["IsActive"].HeaderText = "Đang Hoạt Động";

                //dataGV.Columns["AllowanceTypeID"].Visible = false;
                //dataGV.Columns["ApplyScopeID"].Visible = false;
                dataGV.Columns["EmployessName_NoSign"].Visible = false;
                dataGV.Columns["EmployeeCode"].Width = 60;
                dataGV.Columns["FullName"].Width = 160;
                employeeDeductionGV.Columns["DeductionDate"].Width = 70;
                employeeDeductionGV.Columns["Amount"].Width = 60;
                employeeDeductionGV.Columns["Note"].Width = 200;
                employeeDeductionGV.Columns["EmployeeCode"].Width = 60;
                employeeDeductionGV.Columns["EmployeeName"].Width = 160;
                employeeDeductionGV.Columns["Amount"].DefaultCellStyle.Format = "N0";

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                }

                bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
                newCustomerBtn.Visible = !isLock;
                edit_btn.Visible = !isLock;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                employeeDeductionGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                
                

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

                ReadOnly_btn_Click(null, null);
                employeeDeductionGV.SelectionChanged += this.allowanceGV_CellClick;
                monthYearDtp.ValueChanged += monthYearDtp_ValueChanged;
                dataGV.SelectionChanged += this.dataGV_CellClick;

                UpdateInfo();
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
            //amount_tb.Text = "0";

            employeeDeductionGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            //DataView dv = new DataView(mEmployeeDeduction_dt);
            //dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            //employeeDeductionGV.DataSource = dv;

            
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
                                
                                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit: {row.Cells["DeductionDate"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value} Success", deductionDate.Date, amount, note);

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["DeductionDate"].Value = deductionDate.Date;
                                row.Cells["Amount"].Value = amount;
                                row.Cells["Note"].Value = note;

                                UpdateInfo();

                                DataRowView drv = row.DataBoundItem as DataRowView;
                                if (drv != null)
                                {
                                    DataRow dr = drv.Row;
                                    SQLStore_QLNS.Instance.addOrUpdateDeduction(deductionDate.Year, dr);                                    
                                }
                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit: {row.Cells["DeductionDate"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value} Fail", deductionDate.Date, amount, note);
                                status_lb.ForeColor = Color.Red;
                                status_lb.Text = "Thất bại.";
                            }
                        }
                        catch
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                            _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit: {row.Cells["DeductionDate"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value} Fail", deductionDate.Date, amount, note);
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

                        _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "New: Success", deductionDate.Date, amount, note);
                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        SQLStore_QLNS.Instance.addOrUpdateDeduction(deductionDate.Year, drToAdd);
                        newCustomerBtn_Click(null, null);
                        UpdateInfo();
                    }
                    else
                    {
                        _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "New: Fail", deductionDate.Date, amount, note);
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"New: Fail Exception: {ex.Message}", deductionDate.Date, amount, note);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

            }
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            int amount;
            if (!int.TryParse(amount_tb.Text, out amount) || dataGV.CurrentRow == null)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int month = monthYearDtp.Value.Month;
            int year = monthYearDtp.Value.Year;
            DateTime deductionDate = new DateTime(year, month, deductionDate_dtp.Value.Day);

            //if (deductionDate.Year != year || month != deductionDate.Month)
            //{
            //    MessageBox.Show("Tháng hoặc Năm có vẫn đề", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(deductionDate.Month, deductionDate.Year);
            if (isLock)
            {
                MessageBox.Show("Tháng " + deductionDate.Month + "/" + deductionDate.Year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //
            string note = note_tb.Text;

            if (employeeDeductionID_tb.Text.Length != 0)
            {
                string employeeCode = Convert.ToString(employeeDeductionGV.CurrentRow.Cells["EmployeeCode"].Value);                
                updateData(Convert.ToInt32(employeeDeductionID_tb.Text), employeeCode, deductionDate, amount, note);
            }
            else {
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
                string id = row.Cells["EmployeeDeductionID"].Value.ToString();
                string employeeCode = row.Cells["EmployeeCode"].Value.ToString();
                string deductionTypeCode = row.Cells["DeductionTypeCode"].Value.ToString();
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

                        UpdateInfo();
                    }
                    break;
                }
            }
        }

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            info_gb.Enabled = false;
            LuuThayDoiBtn.Visible = false;
            //int month = monthYearDtp.Value.Month;
            //int year = monthYearDtp.Value.Year;
            //employeeDeductionID_tb.Text = "";
            //amount_tb.Text = "";
            //deductionDate_dtp.Value = new DateTime(year, month, deductionDate_dtp.Value.Day);
            //status_lb.Text = "";
            //info_gb.BackColor = Color.Green;

            //deductionDate_dtp.Focus();
            //info_gb.BackColor = newCustomerBtn.BackColor;
            //edit_btn.Visible = false;
            //newCustomerBtn.Visible = false;
            //readOnly_btn.Visible = false;// true;
            //LuuThayDoiBtn.Visible = true;
            isNewState = true;
            //LuuThayDoiBtn.Text = "Lưu Mới";
            //SetUIReadOnly(false);
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
            info_gb.Enabled = true;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = false;//true;
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
            employeeDeductionGV.DataSource = mEmployeeDeduction_dt;
            if (dataGV.CurrentRow != null)
            {
                int selectedIndex = dataGV.CurrentRow?.Index ?? -1;
                UpdateEmployeeDeductionUI(selectedIndex);
            }

            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
            newCustomerBtn.Visible = !isLock;
            edit_btn.Visible = !isLock;

            monthYearLabel.Text = $"Tháng {month}/{year}";
            UpdateInfo();
            await Task.Delay(100);
            loadingOverlay.Hide();
        }

        private async void LoadFromExcel_btn_Click(object sender, EventArgs e)
        {
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(200);

            DateTime updateDate = new DateTime(monthYearDtp.Value.Year, monthYearDtp.Value.Month,
                                                            DateTime.DaysInMonth(monthYearDtp.Value.Year, monthYearDtp.Value.Month)
                                                        );
            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(updateDate.Month, updateDate.Year);
            if (isLock)
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
                MessageBox.Show($"Tháng {updateDate.Month}/{updateDate.Year} Đã Bị Khóa Rồi ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dialogResult = MessageBox.Show($"Sẽ cập nhật dữ liệu mua rau vào tháng {monthYearDtp.Value.Month}/{monthYearDtp.Value.Year}?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
                return;

            try
            {
                using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.Title = "Chọn file Excel";
                    ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
                    ofd.Multiselect = false; // chỉ cho chọn 1 file

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = ofd.FileName;
                        System.Data.DataTable excelData = Utils.LoadExcel_NoHeader(filePath, 2);
                        List<(string employeeCode, string DeductionTypeCode, DateTime deductionDate, int amount, string note)> newData = new List<(string employeeCode, string DeductionTypeCodev, DateTime deductionDate, int amount, string note)>();
                        List<(string employeeCode, string deductionTypeCode, string description, DateTime deductionDate, int amount, string note)> logs = new List<(string, string, string, DateTime, int, string)>();

                        
                        HashSet<string> employeeSet = mEmployee_dt.AsEnumerable().Select(r => r.Field<string>("EmployeeCode")).ToHashSet();
                        foreach (DataRow edr in excelData.Rows)
                        {
                            string employeeCode = edr["Column2"].ToString();
                            int amount = Convert.ToInt32(edr["Column4"]);
                            if (string.IsNullOrWhiteSpace(employeeCode) || !employeeSet.Contains(employeeCode) || amount <= 0)
                                continue;

                            newData.Add((employeeCode, DeductionTypeCode, updateDate, amount, $"Excel: Tiền mua rau T{monthYearDtp.Value.Month}/{monthYearDtp.Value.Year}"));
                            logs.Add((employeeCode, DeductionTypeCode, "Tải từ Excel T{monthYearDtp.Value.Month}/{monthYearDtp.Value.Year}", updateDate, amount, ""));
                        }

                        try
                        {
                            bool isSuccess = await SQLManager_QLNS.Instance.InsertEmployeeDeduction_ListAsync(newData);
                            if (isSuccess)
                            {
                                var updatedLogs = logs
                                        .Select(item =>
                                            (
                                                item.employeeCode,
                                                item.deductionTypeCode,
                                                Description: item.description + " Success",   // <— sửa ở đây
                                                item.deductionDate,
                                                item.amount,
                                                item.note
                                            )
                                        ).ToList();

                                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLog_ListAsync(updatedLogs);

                                await Task.Delay(200);
                                loadingOverlay.Hide();
                                SQLStore_QLNS.Instance.removeDeduction(updateDate.Year);
                                ShowData();
                            }
                            else
                            {
                                var updatedLogs = logs
                                       .Select(item =>
                                           (
                                               item.employeeCode,
                                               item.deductionTypeCode,
                                               Description: item.description + " Fail",   // <— sửa ở đây
                                               item.deductionDate,
                                               item.amount,
                                               item.note
                                           )
                                       ).ToList();

                                _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLog_ListAsync(updatedLogs);

                                MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                await Task.Delay(200);
                                loadingOverlay.Hide();
                            }
                        }
                        catch (Exception ex)
                        {
                            await Task.Delay(200);
                            loadingOverlay.Hide();
                            Console.WriteLine("ERROR: " + ex.ToString());

                            var updatedLogs = logs
                               .Select(item =>
                                   (
                                       item.employeeCode,
                                       item.deductionTypeCode,
                                       Description: item.description + " Fail Exception: " + ex.Message,   // <— sửa ở đây
                                       item.deductionDate,
                                       item.amount,
                                       item.note
                                   )
                               ).ToList();

                            _ = SQLManager_QLNS.Instance.InsertEmployeeDeductionLog_ListAsync(updatedLogs);

                            MessageBox.Show("Thất Bại ?" + ex.Message, "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }                            
                    }

                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
                MessageBox.Show("Thất Bại ?", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }

        private void UpdateInfo()
        {
            int rowCount = mEmployeeDeduction_dt.Rows.Count;
            decimal totalAmount = mEmployeeDeduction_dt.AsEnumerable().Where(r => r["Amount"] != DBNull.Value).Sum(r => r.Field<int>("Amount"));
            count_label.Text = rowCount.ToString();
            totalAmount_label.Text = totalAmount.ToString("N0");
        }
    }
}
