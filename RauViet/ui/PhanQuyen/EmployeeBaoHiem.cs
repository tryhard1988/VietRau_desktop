using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class EmployeeBaoHiem : Form
    {
        DataView mLogDV;
        public EmployeeBaoHiem()
        {
            InitializeComponent();

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
                string[] keepColumns = { "EmployeeCode", "FullName", "SocialInsuranceNumber", "HealthInsuranceNumber" };
                var employeesTask = SQLStore.Instance.GetEmployeesAsync(keepColumns);
                var employeeInsuranceLogTask = SQLStore.Instance.GetEmployeeInsuranceLogAsync();
                await Task.WhenAll(employeesTask, employeeInsuranceLogTask);
                DataTable employee_dt = employeesTask.Result;
                mLogDV = new DataView(employeeInsuranceLogTask.Result);
                dataGV.DataSource = employee_dt;
                log_GV.DataSource = mLogDV;

                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Tên NV";
                dataGV.Columns["SocialInsuranceNumber"].HeaderText = "BHXH";
                dataGV.Columns["HealthInsuranceNumber"].HeaderText = "BHYT";

                dataGV.Columns["EmployeeCode"].Width = 60;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["SocialInsuranceNumber"].Width = 160;
                dataGV.Columns["HealthInsuranceNumber"].Width = 160;

                log_GV.Columns["LogID"].Visible = false;
                log_GV.Columns["EmployeeCode"].Visible = false;

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
            string bhxh = cells["SocialInsuranceNumber"].Value.ToString();
            string bhyt = cells["HealthInsuranceNumber"].Value.ToString();

            employeeCode_tb.Text = employeeCode.ToString();
            bhyt_tb.Text = bhyt;
            bhxh_tb.Text = bhxh;

            status_lb.Text = "";
            mLogDV.RowFilter = $"EmployeeCode = '{employeeCode}'";
        }

        private async void updateData(string EmployeeCode, string bhxh, string bhyt)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string id = Convert.ToString(row.Cells["EmployeeCode"].Value);
                if (id.CompareTo(EmployeeCode) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateEmployeeBHAsync(EmployeeCode, bhxh, bhyt);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                _ = SQLManager.Instance.InsertEmployeesInsuranceLogAsync(EmployeeCode, $"{row.Cells["SocialInsuranceNumber"].Value} - {row.Cells["HealthInsuranceNumber"].Value}",
                                    $"Success: {bhxh} - {bhyt}");

                                row.Cells["SocialInsuranceNumber"].Value = bhxh;
                                row.Cells["HealthInsuranceNumber"].Value = bhyt;

                                var parameters = new Dictionary<string, object>
                                {
                                    ["SocialInsuranceNumber"] = bhxh,
                                    ["HealthInsuranceNumber"] = bhyt
                                };
                                SQLStore.Instance.updateEmploy(EmployeeCode, parameters);
                            }
                            else
                            {
                                _ = SQLManager.Instance.InsertEmployeesInsuranceLogAsync(EmployeeCode, $"{row.Cells["SocialInsuranceNumber"].Value} - {row.Cells["HealthInsuranceNumber"].Value}",
                                    $"Fail: {bhxh} - {bhyt}");
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager.Instance.InsertEmployeesInsuranceLogAsync(EmployeeCode, $"{row.Cells["SocialInsuranceNumber"].Value} - {row.Cells["HealthInsuranceNumber"].Value}",
                                    $" Fail Exception: {ex.Message}: {bhxh} - {bhyt}");
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
            if (employeeCode_tb.Text.Length != 0)
                updateData(employeeCode_tb.Text, bhxh_tb.Text, bhyt_tb.Text);

        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            SetUIReadOnly(true);
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
            bhxh_tb.ReadOnly = isReadOnly;
            bhyt_tb.ReadOnly = isReadOnly;
        }
    }
}
