using DocumentFormat.OpenXml.Bibliography;
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

namespace RauViet.ui
{
    public partial class EmployeeNganHang : Form
    { 
        public EmployeeNganHang()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;

            LuuThayDoiBtn.Click += saveBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                string[] keepColumns = { "EmployeeCode", "FullName", "BankName", "BankAccountHolder", "BankAccountNumber", "BankBranch" };
                var employeesTask = SQLStore.Instance.GetEmployeesAsync(keepColumns);

                await Task.WhenAll(employeesTask);
                DataTable employee_dt = employeesTask.Result;

                dataGV.DataSource = employee_dt;

                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Tên NV";
                dataGV.Columns["BankName"].HeaderText = "Tên Ngân Hàng";
                dataGV.Columns["BankBranch"].HeaderText = "Chi Nhánh";
                dataGV.Columns["BankAccountNumber"].HeaderText = "Số Tài Khoản";
                dataGV.Columns["BankAccountHolder"].HeaderText = "Chủ Tài Khoản";

                dataGV.Columns["EmployeeCode"].Width = 60;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["BankName"].Width = 150;
                dataGV.Columns["BankBranch"].Width = 320;
                dataGV.Columns["BankAccountNumber"].Width = 160;
                dataGV.Columns["BankAccountHolder"].Width = 160;

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
                loading_lb.Visible = false; // ẩn loading
                loading_lb.Enabled = true; // enable lại button
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

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
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
    }
}
