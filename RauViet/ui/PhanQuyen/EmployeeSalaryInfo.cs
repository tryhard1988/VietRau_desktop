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
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;
            delete_btn.Enabled = false;

            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            salaryInfoGV.SelectionChanged += this.allowanceGV_CellClick;
            year_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            baseSalary_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            insuranceBaseSalary_tb.KeyPress += Tb_KeyPress_OnlyNumber;
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                var employeeTask = SQLManager.Instance.GetActiveEmployeeAsync();
                var employeeSalaryInfoAsync = SQLManager.Instance.GetEmployeeSalaryInfoAsybc();

                await Task.WhenAll(employeeTask, employeeSalaryInfoAsync);
                DataTable employee_dt = employeeTask.Result;
                mEmployeeSalaryInfo_dt = employeeSalaryInfoAsync.Result;

                mEmployeeSalaryInfo_dt.Columns.Add(new DataColumn("Date", typeof(string)));
                foreach (DataRow dr in mEmployeeSalaryInfo_dt.Rows)
                {
                    dr["Date"] = dr["Month"] + "/" + dr["Year"];
                }

                foreach (DataRow dr in employee_dt.Rows)
                {
                    if (dr["PositionName"] == DBNull.Value) dr["PositionName"] = "";
                    if (dr["ContractTypeName"] == DBNull.Value) dr["ContractTypeName"] = "";
                }


                salaryInfoGV.DataSource = mEmployeeSalaryInfo_dt;

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = employee_dt;

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

                salaryInfoGV.Columns["Date"].HeaderText = "Tháng P.C";
                salaryInfoGV.Columns["BaseSalary"].HeaderText = "Lương CB";
                salaryInfoGV.Columns["Note"].HeaderText = "Ghi Chú";
                salaryInfoGV.Columns["CreatedAt"].HeaderText = "Ngày Tạo";
                salaryInfoGV.Columns["InsuranceBaseSalary"].HeaderText = "Lương CS Đóng BHXH";

                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";

                salaryInfoGV.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                salaryInfoGV.Columns["BaseSalary"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                salaryInfoGV.Columns["InsuranceBaseSalary"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                salaryInfoGV.Columns["Note"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                salaryInfoGV.Columns["CreatedAt"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

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

            DataView dv = new DataView(mEmployeeSalaryInfo_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            salaryInfoGV.DataSource = dv;
        }
        private void UpdateRightUI(int index)
        {
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

            info_gb.BackColor = Color.DarkGreen;
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

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(baseSalary_tb.Text) == null || 
                dataGV.CurrentRow == null || month_cbb.SelectedItem == null || string.IsNullOrEmpty(year_tb.Text))
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
            int baseSalary = Convert.ToInt32(baseSalary_tb.Text);
            int insuranceBaseSalary = Convert.ToInt32(insuranceBaseSalary_tb.Text);
            int month = Convert.ToInt32(month_cbb.SelectedItem);
            int year = Convert.ToInt32(year_tb.Text);
            string note= note_tb.Text;
            
            createNew(employeeCode, month, year, baseSalary, insuranceBaseSalary, note);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (salaryInfoGV.CurrentRow == null) return;

            var currentRow = salaryInfoGV.CurrentRow;
            if (currentRow.Cells["SalaryInfoID"].Value == null) return;

            int salaryInfoID = Convert.ToInt32(currentRow.Cells["SalaryInfoID"].Value);

            DialogResult dialogResult = MessageBox.Show(
                "XÓA THÔNG TIN \nChắc chắn chưa?",
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
    }
}
