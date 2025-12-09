using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class EmployeeNganHang : Form
    {
        private DataView mLogDV;
        public EmployeeNganHang()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
            this.KeyDown += EmployeeNganHang_KeyDown;
        }

        private void EmployeeNganHang_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore.Instance.removeEmployees();
                ShowData();
            }
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);
            try
            {
                string[] keepColumns = { "EmployeeCode", "FullName", "BankName", "BankAccountHolder", "BankAccountNumber", "BankBranch" };
                var employeesTask = SQLStore.Instance.GetEmployeesAsync(keepColumns);
                var employeeBankLogTask = SQLStore.Instance.GetEmployeeBankLogAsync();
                await Task.WhenAll(employeesTask, employeeBankLogTask);
                DataTable employee_dt = employeesTask.Result;
                mLogDV = new DataView(employeeBankLogTask.Result);
                dataGV.DataSource = employee_dt;
                log_GV.DataSource = mLogDV;

                log_GV.Columns["LogID"].Visible = false;
                log_GV.Columns["EmployeeCode"].Visible = false;

                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Tên NV";
                dataGV.Columns["BankName"].HeaderText = "Tên Ngân Hàng";
                dataGV.Columns["BankBranch"].HeaderText = "Chi Nhánh";
                dataGV.Columns["BankAccountNumber"].HeaderText = "Số Tài Khoản";
                dataGV.Columns["BankAccountHolder"].HeaderText = "Chủ Tài Khoản";

                dataGV.Columns["EmployeeCode"].Width = 60;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["BankName"].Width = 130;
                dataGV.Columns["BankBranch"].Width = 200;
                dataGV.Columns["BankAccountNumber"].Width = 100;
                dataGV.Columns["BankAccountHolder"].Width = 160;

                dataGV.Columns["BankName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGV.Columns["BankAccountNumber"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                log_GV.Columns["CreatedAt"].Width = 120;
                log_GV.Columns["ACtionBy"].Width = 150;
                log_GV.Columns["OldValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["NewValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["CreatedAt"].HeaderText = "Thời điểm thay đổi";
                log_GV.Columns["ACtionBy"].HeaderText = "Người thay đổi";
                log_GV.Columns["OldValue"].HeaderText = "Giá trị cũ";
                log_GV.Columns["NewValue"].HeaderText = "Giá trị mới";
                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    UpdateRightUI(0);
                }

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
        
        private void dataGV_CellClick(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow == null) return;
            int rowIndex = dataGV.CurrentRow.Index;
            if (rowIndex < 0)
                return;


            UpdateRightUI(rowIndex);
        }

        private void UpdateRightUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);
            string bankName = cells["BankName"].Value.ToString();
            string bankBranch = cells["BankBranch"].Value.ToString();
            string bankAccountNumber = cells["BankAccountNumber"].Value.ToString();
            string bankAccountHolder = cells["BankAccountHolder"].Value.ToString();

            employeeCode_tb.Text = employeeCode.ToString();            
            bankName_tb.Text = bankName;
            bankBranch_tb.Text = bankBranch;
            bankAccountNumber_tb.Text = bankAccountNumber;
            bankAccountHolder_tb.Text = bankAccountHolder;

            status_lb.Text = "";
            mLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
        }

        private async void updateData(string employeeCode, string bankName, string bankBranch, string bankAccountNumber, string bankAccountHolder)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string id = Convert.ToString(row.Cells["EmployeeCode"].Value);
                if (id.CompareTo(employeeCode) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateEmployeeBankAsync(employeeCode, bankName, bankBranch, bankAccountNumber, bankAccountHolder);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                _ = SQLManager.Instance.InsertEmployeesBankLogAsync(employeeCode, $"{row.Cells["BankName"].Value} - {row.Cells["BankBranch"].Value} - {row.Cells["BankAccountNumber"].Value} - {row.Cells["BankAccountHolder"].Value}",
                                    $"Success: {bankName} - {bankBranch} - {bankAccountNumber} - {bankAccountHolder}");
                                row.Cells["BankName"].Value = bankName;
                                row.Cells["BankBranch"].Value = bankBranch;
                                row.Cells["BankAccountNumber"].Value = bankAccountNumber;
                                row.Cells["BankAccountHolder"].Value = bankAccountHolder;

                                var parameters = new Dictionary<string, object>
                                {
                                    ["BankName"] = bankName,
                                    ["BankBranch"] = bankBranch,
                                    ["BankAccountNumber"] = bankAccountNumber,
                                    ["BankAccountHolder"] = bankAccountHolder
                                };
                                SQLStore.Instance.updateEmploy(employeeCode, parameters);
                            }
                            else
                            {
                                _ = SQLManager.Instance.InsertEmployeesBankLogAsync(employeeCode, $"{row.Cells["BankName"].Value} - {row.Cells["BankBranch"].Value} - {row.Cells["BankAccountNumber"].Value} - {row.Cells["BankAccountHolder"].Value}",
                                    $"Fail: {bankName} - {bankBranch} - {bankAccountNumber} - {bankAccountHolder}");
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager.Instance.InsertEmployeesBankLogAsync(employeeCode, $"{row.Cells["BankName"].Value} - {row.Cells["BankBranch"].Value} - {row.Cells["BankAccountNumber"].Value} - {row.Cells["BankAccountHolder"].Value}",
                                    $"Fail Exception: {ex.Message} - {bankName} - {bankBranch} - {bankAccountNumber} - {bankAccountHolder}");
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }  
                    }
                    break;
                }
            }
        }
        
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(bankName_tb.Text) && string.IsNullOrEmpty(bankBranch_tb.Text) &&
                string.IsNullOrEmpty(bankAccountNumber_tb.Text) && string.IsNullOrEmpty(bankAccountHolder_tb.Text))
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            if (employeeCode_tb.Text.Length != 0)
                updateData(employeeCode_tb.Text, bankName_tb.Text, bankBranch_tb.Text, bankAccountNumber_tb.Text, bankAccountHolder_tb.Text);

        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            SetUIReadOnly(true);
            dataGV_CellClick(null, null);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            SetUIReadOnly(false);
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            bankName_tb.ReadOnly = isReadOnly;
            bankAccountHolder_tb.ReadOnly = isReadOnly;
            bankAccountNumber_tb.ReadOnly = isReadOnly;
            bankBranch_tb.ReadOnly = isReadOnly;
        }
    }
}
