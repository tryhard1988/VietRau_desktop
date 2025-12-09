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
        private DataTable mEmployeeDeduction_dt;
        private DataView mDeductionLogDV;
        private const string DeductionTypeCode = "CEP";
        private string mDeductionName = "";
        bool isNewState = false;
        public EmployeeDeduction_CEP()
        {
            InitializeComponent();
            this.KeyPreview = true;
            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            month_cbb.TabIndex = countTab++; month_cbb.TabStop = true;
            amount_tb.TabIndex = countTab++; amount_tb.TabStop = true;
            note_tb.TabIndex = countTab++; note_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            month_cbb.Items.Clear();
            for (int m = 1; m <= 12; m++)
            {
                month_cbb.Items.Add(m);
            }

            month_cbb.SelectedItem = DateTime.Now.Month;
            year_tb.Text = DateTime.Now.Year.ToString();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            employeeDeductionGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            employeeDeductionGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            
            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            load_btn.Click += Load_btn_Click;
            month_cbb.SelectedIndexChanged += Month_cbb_SelectedIndexChanged;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
            this.KeyDown += EmployeeDeduction_CEP_KeyDown;
        }

        private void EmployeeDeduction_CEP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int year = Convert.ToInt32(year_tb.Text);

                SQLStore.Instance.removeDeduction(year);
                ShowData();
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
                int month = Convert.ToInt32(month_cbb.SelectedItem);
                int year = Convert.ToInt32(year_tb.Text);
                string[] keepColumns = { "EmployeeCode", "FullName", "PositionName", "ContractTypeName", };
                var employeesTask = SQLStore.Instance.GetEmployeesAsync(keepColumns);
                var employeeDeductionAsync = SQLStore.Instance.GetDeductionAsync(year, DeductionTypeCode);
                var deductionNameAsync = SQLStore.Instance.GetDeductionNameAsync(DeductionTypeCode);
                var EmployeeDeductionLogTask = SQLStore.Instance.GetEmployeeDeductionLogAsync(year, DeductionTypeCode);
                await Task.WhenAll(employeesTask, employeeDeductionAsync, deductionNameAsync, EmployeeDeductionLogTask);
                DataTable employee_dt = employeesTask.Result;
                mEmployeeDeduction_dt = employeeDeductionAsync.Result;
                mDeductionName = deductionNameAsync.Result;
                mDeductionLogDV = new DataView(EmployeeDeductionLogTask.Result);
                foreach (DataRow dr in employee_dt.Rows)
                {
                    if (dr["PositionName"] == DBNull.Value) dr["PositionName"] = "";
                    if (dr["ContractTypeName"] == DBNull.Value) dr["ContractTypeName"] = "";
                }

                employeeDeductionGV.DataSource = mEmployeeDeduction_dt;

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = employee_dt;
                log_GV.DataSource = mDeductionLogDV;

                employeeDeductionGV.Columns["EmployeeDeductionID"].Visible = false;
                employeeDeductionGV.Columns["EmployeeCode"].Visible = false;
                employeeDeductionGV.Columns["DeductionTypeCode"].Visible = false;
                employeeDeductionGV.Columns["DeductionTypeName"].Visible = false;

                log_GV.Columns["LogID"].Visible = false;
                log_GV.Columns["EmployeeCode"].Visible = false;
                log_GV.Columns["DeductionTypeCode"].Visible = false;

                int count = 0;
                mEmployeeDeduction_dt.Columns["DeductionDate"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["Amount"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["Note"].SetOrdinal(count++);


                employeeDeductionGV.Columns["DeductionDate"].HeaderText = "Ngày Chi";
                employeeDeductionGV.Columns["Amount"].HeaderText = "Số Tiền";
                employeeDeductionGV.Columns["Note"].HeaderText = "Ghi Chú";

                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";

                employeeDeductionGV.Columns["DeductionDate"].Width = 70;
                employeeDeductionGV.Columns["Amount"].Width = 60;
                employeeDeductionGV.Columns["Note"].Width = 200;

                employeeDeductionGV.SelectionChanged -= this.allowanceGV_CellClick;
                employeeDeductionGV.SelectionChanged += this.allowanceGV_CellClick;
                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                 //   UpdateAllowancetUI(0);
                }
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

            }
            catch (Exception ex)
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

        private async void Load_btn_Click(object sender, EventArgs e)
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            int year = Convert.ToInt32(year_tb.Text);

            var employeeDeductionAsync = SQLStore.Instance.GetDeductionAsync(year, DeductionTypeCode);
            var EmployeeDeductionLogTask = SQLStore.Instance.GetEmployeeDeductionLogAsync(year, DeductionTypeCode);
            await Task.WhenAll(employeeDeductionAsync);
            mEmployeeDeduction_dt = employeeDeductionAsync.Result;
            mDeductionLogDV = new DataView(EmployeeDeductionLogTask.Result);
            log_GV.DataSource = mDeductionLogDV;
            if (dataGV.CurrentRow != null)
            {
                int selectedIndex = dataGV.CurrentRow?.Index ?? -1;
                UpdateEmployeeDeductionUI(selectedIndex);
            }

            await Task.Delay(100);
            loadingOverlay.Hide();
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


            UpdateRightUI(rowIndex);
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateEmployeeDeductionUI(rowIndex);
        }

        private void UpdateEmployeeDeductionUI(int index)
        {
            amount_tb.Text = "0";

            employeeDeductionGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mEmployeeDeduction_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            employeeDeductionGV.DataSource = dv;

            mDeductionLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
        }
        private void UpdateRightUI(int index)
        {
            if (isNewState) return;

            var cells = employeeDeductionGV.Rows[index].Cells;
            int employeeDeductionID = Convert.ToInt32(cells["EmployeeDeductionID"].Value);
            DateTime deductionDate = Convert.ToDateTime(cells["DeductionDate"].Value);
            int amount = Convert.ToInt32(cells["Amount"].Value);
            string note = cells["Note"].Value.ToString();

            this.employeeDeductionID_tb.Text = employeeDeductionID.ToString();
            month_cbb.SelectedItem = deductionDate.Month;
            year_tb.Text = deductionDate.Year.ToString();
            amount_tb.Text = amount.ToString();
            note_tb.Text = note;

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
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
                            bool isScussess = await SQLManager.Instance.updateEmployeeDeductionAsync(employeeDeductionID, employeeCode, deductionDate, amount, note);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit {row.Cells["DeductionDate"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value}: Success", deductionDate.Date, amount, note);

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["DeductionDate"].Value = deductionDate.Date;
                                row.Cells["Amount"].Value = amount;
                                row.Cells["Note"].Value = note;

                                

                                DataRowView drv = row.DataBoundItem as DataRowView;
                                if (drv != null)
                                {
                                    DataRow dr = drv.Row;
                                    SQLStore.Instance.addOrUpdateDeduction(deductionDate.Year, dr);
                                }
                            }
                            else
                            {
                                _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit {row.Cells["DeductionDate"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value}: Fail", deductionDate.Date, amount, note);
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, $"Edit {row.Cells["DeductionDate"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value}: Fail Exception: {ex.Message}", deductionDate.Date, amount, note);
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
                    int employeeDeductionID = await SQLManager.Instance.insertEmployeeDeductionAsync(employeeCode, DeductionTypeCode, deductionDate, amount, note);
                    if (employeeDeductionID > 0)
                    {
                        DataRow drToAdd = mEmployeeDeduction_dt.NewRow();

                        drToAdd["EmployeeDeductionID"] = employeeDeductionID;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["DeductionTypeCode"] = DeductionTypeCode;
                        drToAdd["DeductionTypeName"] = mDeductionName;
                        drToAdd["DeductionDate"] = deductionDate.Date;
                        drToAdd["Amount"] = amount;
                        drToAdd["Note"] = note;

                        mEmployeeDeduction_dt.Rows.Add(drToAdd);
                        mEmployeeDeduction_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Success", deductionDate.Date, amount, note);
                        SQLStore.Instance.addOrUpdateDeduction(deductionDate.Year, drToAdd);
                        newCustomerBtn_Click(null, null);
                    }
                    else
                    {
                        _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Fail", deductionDate.Date, amount, note);
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Fail Exception: " + ex.Message, deductionDate.Date, amount, note);
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
                        
            int year = Convert.ToInt32(year_tb.Text);
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            DateTime deductionDate = new DateTime(year, month, 15);

            string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
            int amount = Convert.ToInt32(amount_tb.Text);
            string note = note_tb.Text;

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
            if (isLock)
            {
                MessageBox.Show("Tháng " + month + "/" + year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (IsDuplicateDeductionMonth(employeeDeductionGV, employeeCode, month, year, employeeDeductionID_tb.Text.Length != 0 ? false : true))
            {
                MessageBox.Show("Tháng " + month + "/" + year + " Đã Có Data", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (employeeDeductionID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(employeeDeductionID_tb.Text), employeeCode, deductionDate, amount, note);
            else
                createNew(employeeCode, deductionDate, amount, note);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string employeeDeductionID = employeeDeductionID_tb.Text;

            foreach (DataGridViewRow row in employeeDeductionGV.Rows)
            {
                string id = row.Cells["EmployeeDeductionID"].Value.ToString();
                string employeeCode = row.Cells["EmployeeCode"].Value.ToString();
                int amount = Convert.ToInt32(row.Cells["Amount"].Value);
                if (id.CompareTo(employeeDeductionID) == 0)
                {
                    DateTime deductionDate = Convert.ToDateTime(row.Cells["DeductionDate"].Value);
                    bool isLock = await SQLStore.Instance.IsSalaryLockAsync(deductionDate.Month, deductionDate.Year);
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
                            bool isScussess = await SQLManager.Instance.deleteEmployeeDeductionAsync(Convert.ToInt32(employeeDeductionID));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Success", deductionDate.Date, amount, "Delete");

                                int delRowInd = row.Index;
                                employeeDeductionGV.Rows.Remove(row);
                            }
                            else
                            {
                                _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Fail", deductionDate.Date, amount, "Delete");
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager.Instance.InsertEmployeeDeductionLogAsync(employeeCode, DeductionTypeCode, "Create: Fail Exception: " + ex.Message, deductionDate.Date, amount, "Delete");
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
            month_cbb.Focus();
            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = false;
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
            delete_btn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            SetUIReadOnly(true);
            allowanceGV_CellClick(null, null);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = true;
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

        private async void Month_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);
            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
            LuuThayDoiBtn.Visible = !isLock;
            delete_btn.Visible = !isLock;
            edit_btn.Visible = !isLock;            
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            month_cbb.Enabled = !isReadOnly;
            amount_tb.ReadOnly = isReadOnly;
            note_tb.ReadOnly = isReadOnly;
        }
    }
}
