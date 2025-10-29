using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Color = System.Drawing.Color;

namespace RauViet.ui
{    
    public partial class MonthlyAllowance : Form
    {
        private DataTable mMonthlyAllowance_dt, mAllowanceType_dt;
        bool isNewState = false;
        public MonthlyAllowance()
        {
            InitializeComponent();

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            month_cbb.TabIndex = countTab++; month_cbb.TabStop = true;
            year_tb.TabIndex = countTab++; year_tb.TabStop = true;
            allowanceType_cbb.TabIndex = countTab++; allowanceType_cbb.TabStop = true;
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

            allowanceGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            allowanceGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            allowanceGV.SelectionChanged += this.allowanceGV_CellClick;
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            month_cbb.SelectedIndexChanged += Month_cbb_SelectedIndexChanged;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
        }

        public async void ShowData()
        {

            loading_lb.Visible = true;            

            try
            {
                string[] keepColumns = { "EmployeeCode", "FullName", "DepartmentName", "PositionName", "ContractTypeName", };
                var employeesTask = SQLStore.Instance.GetEmployeesAsync(keepColumns);
                var monthlyAllowanceAsync = SQLManager.Instance.GetMonthlyAllowanceAsybc();
                var allowanceTypeAsync = SQLManager.Instance.GetAllowanceTypeAsync("ONCE");

                await Task.WhenAll(employeesTask, monthlyAllowanceAsync, allowanceTypeAsync);
                DataTable employee_dt = employeesTask.Result;
                mMonthlyAllowance_dt = monthlyAllowanceAsync.Result;
                mAllowanceType_dt = allowanceTypeAsync.Result;

                mMonthlyAllowance_dt.Columns.Add(new DataColumn("AllowanceName", typeof(string)));
                mMonthlyAllowance_dt.Columns.Add(new DataColumn("Date", typeof(string)));
                foreach (DataRow dr in mMonthlyAllowance_dt.Rows)
                {
                    int allowanceTypeID = Convert.ToInt32(dr["AllowanceTypeID"]);
                    DataRow[] applyScopeRows = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}");

                    if (applyScopeRows.Length > 0)
                        dr["AllowanceName"] = applyScopeRows[0]["AllowanceName"].ToString();

                    dr["Date"] = dr["Month"] + "/" + dr["Year"];
                }

                foreach (DataRow dr in employee_dt.Rows)
                {
                    if (dr["PositionName"] == DBNull.Value) dr["PositionName"] = "";
                    if (dr["ContractTypeName"] == DBNull.Value) dr["ContractTypeName"] = "";
                }


                DataView dv = new DataView(mAllowanceType_dt);
                dv.RowFilter = "IsActive = 1";
                allowanceType_cbb.DataSource = dv;
                allowanceType_cbb.DisplayMember = "AllowanceName";
                allowanceType_cbb.ValueMember = "AllowanceTypeID";

                allowanceGV.DataSource = mMonthlyAllowance_dt;

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = employee_dt;

                allowanceGV.Columns["Month"].Visible = false;
                allowanceGV.Columns["Year"].Visible = false;
                allowanceGV.Columns["EmployeeCode"].Visible = false;
                allowanceGV.Columns["AllowanceTypeID"].Visible = false;

                int count = 0;
                mMonthlyAllowance_dt.Columns["Date"].SetOrdinal(count++);
                mMonthlyAllowance_dt.Columns["AllowanceName"].SetOrdinal(count++);
                mMonthlyAllowance_dt.Columns["Amount"].SetOrdinal(count++);
                mMonthlyAllowance_dt.Columns["Note"].SetOrdinal(count++);

                allowanceGV.Columns["MonthlyAllowanceID"].HeaderText = "ID";
                allowanceGV.Columns["Date"].HeaderText = "Tháng P.C";
                allowanceGV.Columns["AllowanceName"].HeaderText = "Loại Phụ Cấp";
                allowanceGV.Columns["Amount"].HeaderText = "Số Tiền";
                allowanceGV.Columns["Note"].HeaderText = "Ghi Chú";
                

                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";
                dataGV.Columns["DepartmentName"].HeaderText = "Phòng Ban";
                //dataGV.Columns["IsActive"].HeaderText = "Đang Hoạt Động";

                //dataGV.Columns["AllowanceTypeID"].Visible = false;
                //dataGV.Columns["ApplyScopeID"].Visible = false;

                allowanceGV.Columns["MonthlyAllowanceID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                allowanceGV.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                allowanceGV.Columns["AllowanceName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                allowanceGV.Columns["Amount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                allowanceGV.Columns["Note"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                 //   UpdateAllowancetUI(0);
                }

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                allowanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                Month_cbb_SelectedIndexChanged(null, null);
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

            DataView dv = new DataView(mMonthlyAllowance_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            allowanceGV.DataSource = dv;
        }
        private void UpdateRightUI(int index)
        {
            if (isNewState) return;

            var cells = allowanceGV.Rows[index].Cells;
            int monthlyAllowanceID = Convert.ToInt32(cells["MonthlyAllowanceID"].Value);
            int allowanceTypeID = Convert.ToInt32(cells["AllowanceTypeID"].Value);
            int month = Convert.ToInt32(cells["Month"].Value);
            int year = Convert.ToInt32(cells["Year"].Value);
            int amount = Convert.ToInt32(cells["Amount"].Value);
            string note = cells["Note"].Value.ToString();

            this.monthlyAllowanceID_tb.Text = monthlyAllowanceID.ToString();
            amount_tb.Text = amount.ToString();
            year_tb.Text = year.ToString();
            month_cbb.SelectedItem = month;
            note_tb.Text = note;
            allowanceType_cbb.SelectedValue = allowanceTypeID;

            info_gb.BackColor = Color.DarkGray;
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
                            bool isScussess = await SQLManager.Instance.updateMonthlyAllowanceAsync(monthlyAllowanceID, employeeCode, month, year, amount, allowanceTypeID, note);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["AllowanceTypeID"].Value = allowanceTypeID;
                                row.Cells["Amount"].Value = amount;
                                row.Cells["Month"].Value = month;
                                row.Cells["Year"].Value = year;
                                row.Cells["Date"].Value = month + "/" + year;
                                row.Cells["Note"].Value = note;
                                row.Cells["AllowanceName"].Value = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}")[0]["AllowanceName"].ToString();
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
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
                    int monthlyAllowanceID = await SQLManager.Instance.insertMonthlyAllowanceAsync(employeeCode, month, year, amount, allowanceTypeID, note);
                    if (allowanceTypeID > 0)
                    {
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

                        mMonthlyAllowance_dt.Rows.Add(drToAdd);
                        mMonthlyAllowance_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        newCustomerBtn_Click(null, null);
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

            }
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if (allowanceType_cbb.SelectedValue == null || string.IsNullOrEmpty(amount_tb.Text) || 
                dataGV.CurrentRow == null || month_cbb.SelectedItem == null || string.IsNullOrEmpty(year_tb.Text))
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
            int amount = Convert.ToInt32(amount_tb.Text);
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);
            int allowanceTypeID = Convert.ToInt32(allowanceType_cbb.SelectedValue);
            string note= note_tb.Text;

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
            if (isLock)
            {
                MessageBox.Show("Tháng " + month + "/" + year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (monthlyAllowanceID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(monthlyAllowanceID_tb.Text), employeeCode, month, year, amount, allowanceTypeID, note);
            else
                createNew(employeeCode, month, year, amount, allowanceTypeID, note);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string monthlyAllowanceID = monthlyAllowanceID_tb.Text;

            foreach (DataGridViewRow row in allowanceGV.Rows)
            {
                string id = row.Cells["MonthlyAllowanceID"].Value.ToString();
                if (id.CompareTo(monthlyAllowanceID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteMonthlyAllowanceAsync(Convert.ToInt32(monthlyAllowanceID));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                int delRowInd = row.Index;
                                allowanceGV.Rows.Remove(row);
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
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

            month_cbb.Focus();
            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = false;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
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
        }

        private async void Month_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);
            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
            if (readOnly_btn.Visible)
            {
                LuuThayDoiBtn.Visible = !isLock;

                delete_btn.Visible = !isLock;
            }
            newCustomerBtn.Visible = !isLock;
            edit_btn.Visible = !isLock;

        }
    }
}
