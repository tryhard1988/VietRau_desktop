using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{    
    public partial class EmployeeAllowance : Form
    {
        private DataTable mEmployeeAllowance_dt, mAllowanceType_dt, _employee_dt;
        private DataView mLogDV;
        bool isNewState = false;
        public EmployeeAllowance()
        {
            InitializeComponent();
            this.KeyPreview = true;
            Utils.SetTabStopRecursive(this, false);

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
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;            
            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
            this.KeyDown += EmployeeAllowance_KeyDown;
            search_tb.TextChanged += Search_tb_TextChanged;
        }

        private void EmployeeAllowance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                ShowData();
            }
        }

        public async void ShowData()
        {
            allowanceGV.SelectionChanged -= this.allowanceGV_CellClick;

            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);
            try
            {
                string[] keepColumns = { "EmployeeCode", "FullName", "DepartmentName", "PositionName", "ContractTypeName", "EmployessName_NoSign" };
                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
                var employeeAllowanceAsync = SQLManager_QLNS.Instance.GetEmployeeAllowanceAsybc();
                var allowanceTypeAsync = SQLManager_QLNS.Instance.GetAllowanceTypeAsync("EMP");
                var EmployeeAllowanceLogTask = SQLStore_QLNS.Instance.GetEmployeeAllowanceLogAsync();

                await Task.WhenAll(employeesTask, employeeAllowanceAsync, allowanceTypeAsync, EmployeeAllowanceLogTask);
                _employee_dt = employeesTask.Result;
                mEmployeeAllowance_dt = employeeAllowanceAsync.Result;
                mAllowanceType_dt = allowanceTypeAsync.Result;
                mLogDV = new DataView(EmployeeAllowanceLogTask.Result);

                mEmployeeAllowance_dt.Columns.Add(new DataColumn("AllowanceName", typeof(string)));
                foreach (DataRow dr in mEmployeeAllowance_dt.Rows)
                {
                    int allowanceTypeID = Convert.ToInt32(dr["AllowanceTypeID"]);
                    DataRow[] applyScopeRows = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}");

                    if (applyScopeRows.Length > 0)
                        dr["AllowanceName"] = applyScopeRows[0]["AllowanceName"].ToString();
                }

                DataView dv = new DataView(mAllowanceType_dt);
                dv.RowFilter = "IsActive = 1";
                allowanceType_cbb.DataSource = dv;
                allowanceType_cbb.DisplayMember = "AllowanceName";
                allowanceType_cbb.ValueMember = "AllowanceTypeID";

                allowanceGV.DataSource = mEmployeeAllowance_dt;

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = _employee_dt;
                log_GV.DataSource = mLogDV;

                Utils.HideColumns(dataGV, new[] { "EmployessName_NoSign"});
                Utils.HideColumns(allowanceGV, new[] { "AllowanceTypeID", "EmployeeCode", "EmployeeAllowanceID" });

                int count = 0;
                mEmployeeAllowance_dt.Columns["AllowanceName"].SetOrdinal(count++);
                mEmployeeAllowance_dt.Columns["Amount"].SetOrdinal(count++);
                mEmployeeAllowance_dt.Columns["Note"].SetOrdinal(count++);

                allowanceGV.Columns["Amount"].DefaultCellStyle.Format = "N0";

                allowanceGV.Columns["AllowanceName"].HeaderText = "Loại Phụ Cấp";
                allowanceGV.Columns["Amount"].HeaderText = "Số Tiền";
                allowanceGV.Columns["Note"].HeaderText = "Ghi Chú";

                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";
                dataGV.Columns["DepartmentName"].HeaderText = "Phòng Ban";

                allowanceGV.Columns["AllowanceName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                allowanceGV.Columns["Amount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                allowanceGV.Columns["Note"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                log_GV.Columns["LogID"].Visible = false;
                log_GV.Columns["EmployeeCode"].Visible = false;

                
                allowanceGV.SelectionChanged += this.allowanceGV_CellClick;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                }


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                allowanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                log_GV.Columns["CreatedAt"].Width = 120;
                log_GV.Columns["ActionBy"].Width = 150;
                log_GV.Columns["AllowanceName"].Width = 100;
                log_GV.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["Amount"].Width = 80;
                log_GV.Columns["CreatedAt"].HeaderText = "Thời điểm thay đổi";
                log_GV.Columns["ACtionBy"].HeaderText = "Người thay đổi";
                log_GV.Columns["AllowanceName"].HeaderText = "Loại Phụ Cấp";
                log_GV.Columns["Description"].HeaderText = "Hành động";
                log_GV.Columns["Amount"].HeaderText = "Số Tiền";

                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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

            UpdateRightUI(rowIndex);
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateAllowancetUI(rowIndex);
        }

        private void UpdateAllowancetUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mEmployeeAllowance_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            allowanceGV.DataSource = dv;

            mLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
        }
        private void UpdateRightUI(int index)
        {
            if (isNewState) return;

            var cells = allowanceGV.Rows[index].Cells;
            int employeeAllowanceID = Convert.ToInt32(cells["EmployeeAllowanceID"].Value);
            int allowanceTypeID = Convert.ToInt32(cells["AllowanceTypeID"].Value);
            int amount = Convert.ToInt32(cells["Amount"].Value);
            string note = cells["Note"].Value.ToString();

            this.employeeAllowanceID_tb.Text = employeeAllowanceID.ToString();
            amount_tb.Text = amount.ToString();
            note_tb.Text = note;
            allowanceType_cbb.SelectedValue = allowanceTypeID;

            status_lb.Text = "";
        }
        
        private async void updateData(int employeeAllowanceID, string employeeCode, int amount, int allowanceTypeID, string note)
        {
            foreach (DataGridViewRow row in allowanceGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["EmployeeAllowanceID"].Value);
                if (id.CompareTo(employeeAllowanceID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        string allowanceName = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}")[0]["AllowanceName"].ToString();
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.updateEmployeeAllowanceAsync(employeeAllowanceID, employeeCode, amount, allowanceTypeID, note);

                            if (isScussess == true)
                            {
                                _ = SQLManager_QLNS.Instance.InsertEmployeeAllowanceLogAsync(employeeCode, allowanceName, $"Create Success: {row.Cells["AllowanceName"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value}", amount, note);
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["AllowanceTypeID"].Value = allowanceTypeID;
                                row.Cells["Amount"].Value = amount;
                                row.Cells["Note"].Value = note;
                                row.Cells["AllowanceName"].Value = allowanceName;
                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertEmployeeAllowanceLogAsync(employeeCode, allowanceName, $"Create Fail: {row.Cells["AllowanceName"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value}", amount, note);
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager_QLNS.Instance.InsertEmployeeAllowanceLogAsync(employeeCode, allowanceName, $"Create Fail Exception {ex.Message}: {row.Cells["AllowanceName"].Value} - {row.Cells["Amount"].Value} - {row.Cells["Note"].Value}", amount, note);
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                    }
                    break;
                }
            }
        }

        private async void createNew(string employeeCode, int amount, int allowanceTypeID, string note)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string allowanceName = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}")[0]["AllowanceName"].ToString();
                try
                {
                    int employeeAllowanceID = await SQLManager_QLNS.Instance.insertEmployeeAllowanceAsync(employeeCode, amount, allowanceTypeID, note);

                    if (allowanceTypeID > 0)
                    {
                        _ = SQLManager_QLNS.Instance.InsertEmployeeAllowanceLogAsync(employeeCode, allowanceName, $"Create Success", amount, note);

                        DataRow drToAdd = mEmployeeAllowance_dt.NewRow();

                        drToAdd["EmployeeAllowanceID"] = employeeAllowanceID;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["AllowanceTypeID"] = allowanceTypeID;
                        drToAdd["Amount"] = amount;
                        drToAdd["Note"] = note;
                        drToAdd["AllowanceName"] = allowanceName;

                        mEmployeeAllowance_dt.Rows.Add(drToAdd);
                        mEmployeeAllowance_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        newCustomerBtn_Click(null, null);
                    }
                    else
                    {
                        _ = SQLManager_QLNS.Instance.InsertEmployeeAllowanceLogAsync(employeeCode, allowanceName, $"Create Fail", amount, note);
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager_QLNS.Instance.InsertEmployeeAllowanceLogAsync(employeeCode, allowanceName, $"Create Fail Exception: {ex.Message}", amount, note);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (allowanceType_cbb.SelectedValue == null || string.IsNullOrEmpty(amount_tb.Text) || dataGV.CurrentRow == null)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
            int amount = Convert.ToInt32(amount_tb.Text);
            int allowanceTypeID = Convert.ToInt32(allowanceType_cbb.SelectedValue);
            string note= note_tb.Text;

            if (employeeAllowanceID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(employeeAllowanceID_tb.Text), employeeCode, amount, allowanceTypeID, note);
            else
                createNew(employeeCode, amount, allowanceTypeID, note);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string employeeAllowanceID = employeeAllowanceID_tb.Text;

            foreach (DataGridViewRow row in allowanceGV.Rows)
            {
                string id = row.Cells["EmployeeAllowanceID"].Value.ToString();
                string employeeCode = row.Cells["EmployeeCode"].Value.ToString();
                string allowanceName = row.Cells["AllowanceName"].Value.ToString();
                if (id.CompareTo(employeeAllowanceID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.deleteEmployeeAllowanceAsync(Convert.ToInt32(employeeAllowanceID));

                            if (isScussess == true)
                            {
                                _ = SQLManager_QLNS.Instance.InsertEmployeeAllowanceLogAsync(employeeCode, allowanceName, $"Delete Success", 0, "Delete");
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                int delRowInd = row.Index;
                                allowanceGV.Rows.Remove(row);
                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertEmployeeAllowanceLogAsync(employeeCode, allowanceName, $"Delete Fail", 0, "Delete");
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager_QLNS.Instance.InsertEmployeeAllowanceLogAsync(employeeCode, allowanceName, $"Delete Fail Exception: {ex.Message}", 0, "Delete");
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
            employeeAllowanceID_tb.Text = "";
            amount_tb.Text = "";

            status_lb.Text = "";
            info_gb.BackColor = Color.Green;

            allowanceType_cbb.Focus();
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

        private void SetUIReadOnly(bool isReadOnly)
        {
            allowanceType_cbb.Enabled = !isReadOnly;
            amount_tb.ReadOnly = isReadOnly;
            note_tb.ReadOnly = isReadOnly;
        }

        //private async void button1_Click(object sender, EventArgs e)
        //{
        //    using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
        //    {
        //        ofd.Title = "Chọn file Excel";
        //        ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
        //        ofd.Multiselect = false; // chỉ cho chọn 1 file

        //        if (ofd.ShowDialog() == DialogResult.OK)
        //        {
        //            string filePath = ofd.FileName;
        //            System.Data.DataTable excelData = Utils.LoadExcel_NoHeader(filePath);
        //            try
        //            {
        //                foreach (DataRow edr in excelData.Rows)
        //                {
        //                    string empCode = edr["Column1"].ToString();
        //                    bool exists = _employee_dt.AsEnumerable().Any(r => r.Field<string>("EmployeeCode") == empCode);
        //                    if (!exists) continue;

        //                    int money = Convert.ToInt32(edr["Column2"]);

        //                    int employeeAllowanceID = mEmployeeAllowance_dt.AsEnumerable().Where(r => r.Field<string>("EmployeeCode") == empCode && r.Field<int>("AllowanceTypeID") == 3).Select(r => r.Field<int?>("EmployeeAllowanceID")).FirstOrDefault() ?? -1;

        //                    if (employeeAllowanceID > 0)
        //                    {
        //                        await SQLManager_QLNS.Instance.updateEmployeeAllowanceAsync(employeeAllowanceID, empCode, money, 3, "từ Excel");
        //                    }
        //                    else
        //                    {
        //                        await SQLManager_QLNS.Instance.insertEmployeeAllowanceAsync(empCode, money, 3, "từ Excel");
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("có lỗi " + ex.Message);
        //            }
        //            MessageBox.Show("xong");
        //        }
        //    }
        //}

        private void Search_tb_TextChanged(object sender, EventArgs e)
        {
            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower())
                     .Replace("'", "''"); // tránh lỗi cú pháp '

            DataTable dt = dataGV.DataSource as DataTable;
            if (dt == null) return;

            DataView dv = dt.DefaultView;
            dv.RowFilter = $"[EmployessName_NoSign] LIKE '%{keyword}%'";
        }
    }
}
