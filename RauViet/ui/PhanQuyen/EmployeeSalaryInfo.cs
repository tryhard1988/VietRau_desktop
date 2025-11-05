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
    public partial class EmployeeSalaryInfo : Form
    {
        private DataTable mEmployeeSalaryInfo_dt;
        bool isNewState = false;
        public EmployeeSalaryInfo()
        {
            InitializeComponent();

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

            salaryInfoGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            salaryInfoGV.MultiSelect = false;

            status_lb.Text = "";
            delete_btn.Enabled = false;

            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            salaryInfoGV.SelectionChanged += this.allowanceGV_CellClick;
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            baseSalary_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            insuranceBaseSalary_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            month_cbb.SelectedIndexChanged += Month_cbb_SelectedIndexChanged;

            new_btn.Click += New_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
        }        

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                string[] keepColumns = { "EmployeeCode", "FullName", "PositionName", "ContractTypeName", "GradeName", "SalaryGrade" };
                var employeesTask = SQLStore.Instance.GetEmployeesAsync(keepColumns);
                var employeeSalaryInfoAsync = SQLStore.Instance.GetEmployeeSalaryInfoAsync();

                await Task.WhenAll(employeesTask, employeeSalaryInfoAsync);
                DataTable employee_dt = employeesTask.Result;
                mEmployeeSalaryInfo_dt = employeeSalaryInfoAsync.Result;

                foreach (DataRow dr in employee_dt.Rows)
                {
                    if (dr["PositionName"] == DBNull.Value) dr["PositionName"] = "";
                    if (dr["ContractTypeName"] == DBNull.Value) dr["ContractTypeName"] = "";
                }


                salaryInfoGV.DataSource = mEmployeeSalaryInfo_dt;

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = employee_dt;

                dataGV.Columns["SalaryGrade"].Visible = false;
                salaryInfoGV.Columns["Month"].Visible = false;
                salaryInfoGV.Columns["Year"].Visible = false;
                salaryInfoGV.Columns["EmployeeCode"].Visible = false;
                salaryInfoGV.Columns["SalaryInfoID"].Visible = false;

                int count = 0;
                mEmployeeSalaryInfo_dt.Columns["Date"].SetOrdinal(count++);
                mEmployeeSalaryInfo_dt.Columns["BaseSalary"].SetOrdinal(count++);
                mEmployeeSalaryInfo_dt.Columns["InsuranceBaseSalary"].SetOrdinal(count++);
                mEmployeeSalaryInfo_dt.Columns["Note"].SetOrdinal(count++);
                mEmployeeSalaryInfo_dt.Columns["CreatedAt"].SetOrdinal(count++);

                salaryInfoGV.Columns["Date"].HeaderText = "Tháng/Năm";
                salaryInfoGV.Columns["BaseSalary"].HeaderText = "Lương CB";
                salaryInfoGV.Columns["Note"].HeaderText = "Ghi Chú";
                salaryInfoGV.Columns["CreatedAt"].HeaderText = "Ngày Tạo";
                salaryInfoGV.Columns["InsuranceBaseSalary"].HeaderText = "Lương CS Đóng BHXH";

                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";
                dataGV.Columns["GradeName"].HeaderText = "Bậc Lương";

                dataGV.Columns["FullName"].Width = 150;
                dataGV.Columns["EmployeeCode"].Width = 60;
                dataGV.Columns["PositionName"].Width = 80;
                dataGV.Columns["ContractTypeName"].Width = 80;
                dataGV.Columns["GradeName"].Width = 70;

                salaryInfoGV.Columns["Date"].Width = 80;
                salaryInfoGV.Columns["BaseSalary"].Width = 80;
                salaryInfoGV.Columns["InsuranceBaseSalary"].Width = 80;
                salaryInfoGV.Columns["Note"].Width = 80;
                salaryInfoGV.Columns["CreatedAt"].Width = 80;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                 //   UpdateAllowancetUI(0);
                }


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                salaryInfoGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
            if (salaryInfoGV.CurrentRow == null) return;
            int rowIndex = salaryInfoGV.CurrentRow.Index;
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


            UpdateSalaryInfoUI(rowIndex);
        }

        private void UpdateSalaryInfoUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);
            string salaryGrade = Convert.ToString(cells["SalaryGrade"].Value);

            salaryGrade_tb.Text = salaryGrade.ToString();

            DataView dv = new DataView(mEmployeeSalaryInfo_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            salaryInfoGV.DataSource = dv;
        }
        private void UpdateRightUI(int index)
        {
            if (isNewState) return;
            var cells = salaryInfoGV.Rows[index].Cells;
            int month = Convert.ToInt32(cells["Month"].Value);
            int year = Convert.ToInt32(cells["Year"].Value);
            int baseSalary = Convert.ToInt32(cells["BaseSalary"].Value);
            int insuranceBaseSalary = Convert.ToInt32(cells["InsuranceBaseSalary"].Value);
            string note = cells["Note"].Value.ToString();

            baseSalary_tb.Text = baseSalary.ToString();
            insuranceBaseSalary_tb.Text = insuranceBaseSalary.ToString();
            year_tb.Text = year.ToString();
            month_cbb.SelectedItem = month;
            note_tb.Text = note;

            delete_btn.Enabled = true;
            status_lb.Text = "";
        }
        
        private async void createNew(string employeeCode, int month, int year, int baseSalary, int insuranceBaseSalary, string note)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int salaryInfoID = await SQLManager.Instance.insertEmployeeSalaryInfoAsync(employeeCode, month, year, baseSalary, insuranceBaseSalary,note);
                    if (salaryInfoID > 0)
                    {
                        DataRow drToAdd = mEmployeeSalaryInfo_dt.NewRow();

                        drToAdd["SalaryInfoID"] = salaryInfoID;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["Month"] = month;
                        drToAdd["Year"] = year;
                        drToAdd["Date"] = month + "/" + year;
                        drToAdd["BaseSalary"] = baseSalary;
                        drToAdd["InsuranceBaseSalary"] = insuranceBaseSalary;
                        drToAdd["Note"] = note;
                        drToAdd["CreatedAt"] = DateTime.Now;
                        mEmployeeSalaryInfo_dt.Rows.Add(drToAdd);
                        mEmployeeSalaryInfo_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;
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
            if (string.IsNullOrEmpty(baseSalary_tb.Text) || 
                dataGV.CurrentRow == null || month_cbb.SelectedItem == null || string.IsNullOrEmpty(year_tb.Text))
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
            int baseSalary = Convert.ToInt32(baseSalary_tb.Text);
            int insuranceBaseSalary = Convert.ToInt32(insuranceBaseSalary_tb.Text);
            int newMonth = Convert.ToInt32(month_cbb.SelectedItem);
            int newYear = Convert.ToInt32(year_tb.Text);
            string note= note_tb.Text;

            int maxYear = 0, maxMonth = 0;
            foreach (DataGridViewRow row in salaryInfoGV.Rows)
            {
                if (row.IsNewRow) continue; // bỏ dòng trống
                int year1 = Convert.ToInt32(row.Cells["Year"].Value);
                int month1 = Convert.ToInt32(row.Cells["Month"].Value);

                if (year1 > maxYear || (year1 == maxYear && month1 > maxMonth))
                {
                    maxYear = year1;
                    maxMonth = month1;
                }
            }

            // So sánh
            bool isValid = (newYear > maxYear) || (newYear == maxYear && newMonth > maxMonth);

            if (!isValid)
            {
                MessageBox.Show($"Dữ liệu mới ({newMonth}/{newYear}) phải lớn hơn dữ liệu hiện có ({maxMonth}/{maxYear}).");
                return;
            }

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(newMonth, newYear);
            if (isLock)
            {
                MessageBox.Show("Tháng " + newMonth + "/" + newYear + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            createNew(employeeCode, newMonth, newYear, baseSalary, insuranceBaseSalary, note);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (salaryInfoGV.CurrentRow == null) return;

            var currentRow = salaryInfoGV.CurrentRow;
            if (currentRow.Cells["SalaryInfoID"].Value == null) return;

            int  month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);
            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
            if (isLock)
            {
                MessageBox.Show("Tháng " + month + "/" + year + " đã bị khóa.", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int salaryInfoID = Convert.ToInt32(currentRow.Cells["SalaryInfoID"].Value);

            DialogResult dialogResult = MessageBox.Show(
                "Chắc chắn chưa?",
                "Xóa Thông Tin",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (dialogResult != DialogResult.Yes) return;

            try
            {
                bool isSuccess = await SQLManager.Instance.deleteEmployeeSalaryInfoAsync(salaryInfoID);

                if (isSuccess)
                {
                    salaryInfoGV.Rows.Remove(currentRow);

                    status_lb.Text = "Xóa thành công.";
                    status_lb.ForeColor = Color.Green;
                }
                else
                {
                    status_lb.Text = "Xóa thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                status_lb.Text = "Lỗi khi xóa: " + ex.Message;
                status_lb.ForeColor = Color.Red;
            }
        }

        private async void Month_cbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newMonth = Convert.ToInt32(month_cbb.SelectedItem);
            int newYear = Convert.ToInt32(year_tb.Text);
            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(newMonth, newYear);

            if (!isLock)
            {
                delete_btn.Visible = !isLock;

                int maxYear = 0, maxMonth = 0;
                foreach (DataGridViewRow row in salaryInfoGV.Rows)
                {
                    if (row.IsNewRow) continue; // bỏ dòng trống
                    int year1 = Convert.ToInt32(row.Cells["Year"].Value);
                    int month1 = Convert.ToInt32(row.Cells["Month"].Value);

                    if (year1 > maxYear || (year1 == maxYear && month1 > maxMonth))
                    {
                        maxYear = year1;
                        maxMonth = month1;
                    }
                }

                isLock = !((newYear > maxYear) || (newYear == maxYear && newMonth > maxMonth));
                LuuThayDoiBtn.Visible = !isLock;
            }
            else
            {
                LuuThayDoiBtn.Visible = !isLock;
                delete_btn.Visible = !isLock;
            } 
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            new_btn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            delete_btn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            SetUIReadOnly(true);
        }

        private void New_btn_Click(object sender, EventArgs e)
        {
            new_btn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = true;
            isNewState = true;
            info_gb.BackColor = new_btn.BackColor;
            SetUIReadOnly(false);
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            month_cbb.Enabled = !isReadOnly;
            year_tb.Enabled = !isReadOnly;
            baseSalary_tb.ReadOnly = isReadOnly;
            insuranceBaseSalary_tb.ReadOnly = isReadOnly;
            note_tb.ReadOnly = isReadOnly;
        }
    }
}
