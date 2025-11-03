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
    public partial class EmployeeDeduction_ADV : Form
    {
        private DataTable mEmployeeDeduction_dt;
        private const string DeductionTypeCode = "ADV";
        private string mDeductionName = "";
        bool isNewState = false;
        int currMonth = -1;
        int currYear = -1;
        public EmployeeDeduction_ADV()
        {
            InitializeComponent();

            month_cbb.Items.Clear();
            for (int m = 1; m <= 12; m++)
            {
                month_cbb.Items.Add(m);
            }

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            deductionDate_dtp.TabIndex = countTab++; deductionDate_dtp.TabStop = true;
            amount_tb.TabIndex = countTab++; amount_tb.TabStop = true;
            note_tb.TabIndex = countTab++; note_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;


            month_cbb.SelectedItem = DateTime.Now.Month;
            year_tb.Text = DateTime.Now.Year.ToString();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            employeeDeductionGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            employeeDeductionGV.MultiSelect = false;

            status_lb.Text = "";

            deductionDate_dtp.Format = DateTimePickerFormat.Custom;
            deductionDate_dtp.CustomFormat = "dd/MM/yyyy";

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            employeeDeductionGV.SelectionChanged += this.allowanceGV_CellClick;
            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            load_btn.Click += Load_btn_Click;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                int month = Convert.ToInt32(month_cbb.SelectedItem);
                int year = Convert.ToInt32(year_tb.Text);
                string[] keepColumns = { "EmployeeCode", "FullName", "PositionName", "ContractTypeName", };
                var employeesTask = SQLStore.Instance.GetEmployeesAsync(keepColumns);
                var employeeDeductionAsync = SQLStore.Instance.GetDeductionAsync(month, year, DeductionTypeCode);
                var deductionNameAsync = SQLStore.Instance.GetDeductionNameAsync(DeductionTypeCode);
                await Task.WhenAll(employeesTask, employeeDeductionAsync, deductionNameAsync);
                DataTable employee_dt = employeesTask.Result;
                mEmployeeDeduction_dt = employeeDeductionAsync.Result;
                mDeductionName = deductionNameAsync.Result;

                currMonth = month;
                currYear = year;

                foreach (DataRow dr in employee_dt.Rows)
                {
                    if (dr["PositionName"] == DBNull.Value) dr["PositionName"] = "";
                    if (dr["ContractTypeName"] == DBNull.Value) dr["ContractTypeName"] = "";
                }

                employeeDeductionGV.DataSource = mEmployeeDeduction_dt;

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = employee_dt;

                employeeDeductionGV.Columns["EmployeeDeductionID"].Visible = false;
                employeeDeductionGV.Columns["EmployeeCode"].Visible = false;
                employeeDeductionGV.Columns["DeductionTypeCode"].Visible = false;
                employeeDeductionGV.Columns["DeductionTypeName"].Visible = false;

                int count = 0;
                mEmployeeDeduction_dt.Columns["DeductionDate"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["Amount"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["Note"].SetOrdinal(count++);
                mEmployeeDeduction_dt.Columns["UpdateHistory"].SetOrdinal(count++);


                employeeDeductionGV.Columns["DeductionDate"].HeaderText = "Ngày Chi";
                employeeDeductionGV.Columns["Amount"].HeaderText = "Số Tiền";
                employeeDeductionGV.Columns["Note"].HeaderText = "Ghi Chú";
                employeeDeductionGV.Columns["UpdateHistory"].HeaderText = "Log";

                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";
                //dataGV.Columns["IsActive"].HeaderText = "Đang Hoạt Động";

                //dataGV.Columns["AllowanceTypeID"].Visible = false;
                //dataGV.Columns["ApplyScopeID"].Visible = false;

                employeeDeductionGV.Columns["DeductionDate"].Width = 70;
                employeeDeductionGV.Columns["Amount"].Width = 60;
                employeeDeductionGV.Columns["Note"].Width = 200;
                employeeDeductionGV.Columns["UpdateHistory"].Width = 150;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                 //   UpdateAllowancetUI(0);
                }


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                employeeDeductionGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
                LuuThayDoiBtn.Visible = !isLock;
                newCustomerBtn.Visible = !isLock;
                delete_btn.Visible = !isLock;
                readOnly_btn.Visible = !isLock;
                edit_btn.Visible = !isLock;
                isNewState = false;
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
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);

            var employeeDeductionAsync = SQLStore.Instance.GetDeductionAsync(month, year, DeductionTypeCode);

            await Task.WhenAll(employeeDeductionAsync);
            mEmployeeDeduction_dt = employeeDeductionAsync.Result;

            currMonth = month;
            currYear = year;

            if (dataGV.CurrentRow != null)
            {
                int selectedIndex = dataGV.CurrentRow?.Index ?? -1;
                UpdateEmployeeDeductionUI(selectedIndex);
            }

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
            LuuThayDoiBtn.Visible = !isLock;
            newCustomerBtn.Visible = !isLock;
            delete_btn.Visible = !isLock;
            readOnly_btn.Visible = !isLock;
            edit_btn.Visible = !isLock;
            isNewState = false;

            if (!isLock) ReadOnly_btn_Click(null, null);

            await Task.Delay(50);
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
            updateHistory_tb.Text = "";

            employeeDeductionGV.ClearSelection();

            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mEmployeeDeduction_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            employeeDeductionGV.DataSource = dv;
        }
        private void UpdateRightUI(int index)
        {
            if (isNewState) return;

            var cells = employeeDeductionGV.Rows[index].Cells;
            int employeeDeductionID = Convert.ToInt32(cells["EmployeeDeductionID"].Value);
            DateTime deductionDate = Convert.ToDateTime(cells["DeductionDate"].Value);
            int amount = Convert.ToInt32(cells["Amount"].Value);
            string note = cells["Note"].Value.ToString();
            string updateHistory = cells["UpdateHistory"].Value.ToString();

            this.employeeDeductionID_tb.Text = employeeDeductionID.ToString();
            deductionDate_dtp.Value = deductionDate;
            amount_tb.Text = amount.ToString();
            note_tb.Text = note;
            updateHistory_tb.Text = updateHistory;

            status_lb.Text = "";
        }
        
        private async void updateData(int employeeDeductionID, string employeeCode, DateTime deductionDate, int amount, string note, string updateHistory)
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
                            bool isScussess = await SQLManager.Instance.updateEmployeeDeductionAsync(employeeDeductionID, employeeCode, deductionDate, amount, note, updateHistory);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["DeductionDate"].Value = deductionDate.Date;
                                row.Cells["Amount"].Value = amount;
                                row.Cells["Note"].Value = note;
                                row.Cells["UpdateHistory"].Value = updateHistory;

                                DataRowView drv = row.DataBoundItem as DataRowView;
                                if (drv != null)
                                {
                                    DataRow dr = drv.Row;
                                    SQLStore.Instance.addOrUpdateDeduction(deductionDate.Year, dr);
                                }
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

        private async void createNew(string employeeCode, DateTime deductionDate, int amount, string note, string updateHistory)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int employeeDeductionID = await SQLManager.Instance.insertEmployeeDeductionAsync(employeeCode, DeductionTypeCode, deductionDate, amount, note, updateHistory);
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
                        drToAdd["UpdateHistory"] = updateHistory;

                        mEmployeeDeduction_dt.Rows.Add(drToAdd);
                        mEmployeeDeduction_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        SQLStore.Instance.addOrUpdateDeduction(deductionDate.Year, drToAdd);
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
            if (string.IsNullOrEmpty(amount_tb.Text) || dataGV.CurrentRow == null)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime deductionDate = deductionDate_dtp.Value;
            int year = Convert.ToInt32(year_tb.Text);
            int month = Convert.ToInt32(month_cbb.SelectedItem);

            if (deductionDate.Year != year || month != deductionDate.Month || month != currMonth || year != currYear)
            {
                MessageBox.Show("Tháng hoặc Năm có vẫn đề", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
            if (isLock)
            {
                MessageBox.Show("Tháng " + month + "/" + year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
            int amount = Convert.ToInt32(amount_tb.Text);
            string note = note_tb.Text;
            string updateHistory = updateHistory_tb.Text + DateTime.Now.ToString("MM/yyyy") + ":" + UserManager.Instance.employeeCode + ";";

            if (employeeDeductionID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(employeeDeductionID_tb.Text), employeeCode, deductionDate, amount, note, updateHistory);
            else
                createNew(employeeCode, deductionDate, amount, note, updateHistory);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string employeeDeductionID = employeeDeductionID_tb.Text;

            foreach (DataGridViewRow row in employeeDeductionGV.Rows)
            {
                string id = row.Cells["EmployeeDeductionID"].Value.ToString();
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

                                int delRowInd = row.Index;
                                employeeDeductionGV.Rows.Remove(row);
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
            employeeDeductionID_tb.Text = "";
            amount_tb.Text = "";
            updateHistory_tb.Text = null;
            deductionDate_dtp.Value = new DateTime(Convert.ToInt32(year_tb.Text), Convert.ToInt32(month_cbb.SelectedItem), deductionDate_dtp.Value.Day);

            status_lb.Text = "";
            info_gb.BackColor = Color.Green;

            deductionDate_dtp.Focus();
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
            deductionDate_dtp.Enabled = !isReadOnly;
            amount_tb.ReadOnly = isReadOnly;
            note_tb.ReadOnly = isReadOnly;
        }
    }
}
