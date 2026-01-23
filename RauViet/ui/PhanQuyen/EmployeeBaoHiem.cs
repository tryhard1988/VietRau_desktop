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
            this.KeyDown += EmployeeBaoHiem_KeyDown;

            search_tb.TextChanged += Search_tb_TextChanged;
        }

        private void EmployeeBaoHiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_QLNS.Instance.removeEmployees();
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
                string[] keepColumns = { "EmployeeCode", "FullName", "SocialInsuranceNumber", "HealthInsuranceNumber", "EmployessName_NoSign" };
                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
                var employeeInsuranceLogTask = SQLStore_QLNS.Instance.GetEmployeeInsuranceLogAsync();
                await Task.WhenAll(employeesTask, employeeInsuranceLogTask);
                DataTable employee_dt = employeesTask.Result;
                mLogDV = new DataView(employeeInsuranceLogTask.Result);
                dataGV.DataSource = employee_dt;
                log_GV.DataSource = mLogDV;
                Utils.HideColumns(log_GV, new[] { "LogID", "EmployeeCode" });
                Utils.HideColumns(dataGV, new[] { "EmployessName_NoSign" });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"EmployeeCode", "Mã NV" },
                    {"FullName", "Tên NV" },
                    {"SocialInsuranceNumber", "BHXH" },
                    {"HealthInsuranceNumber", "BHYT" }
                });

                Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                    {"CreatedAt", "Ngày thay đổi" },
                    {"ACtionBy", "Người thay đổi" },
                    {"OldValue", "Giá trị cũ" },
                    {"NewValue", "Giá trị mới" }
                });

                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                    {"EmployeeCode", 60},
                    {"FullName", 160},
                    {"SocialInsuranceNumber", 160},
                    {"HealthInsuranceNumber", 160},
                });

                Utils.SetGridWidths(log_GV, new System.Collections.Generic.Dictionary<string, int> {
                    {"CreatedAt", 120},
                    {"ACtionBy", 150}
                });

                log_GV.Columns["OldValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["NewValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    UpdateRightUI(0);
                }

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
                            bool isScussess = await SQLManager_QLNS.Instance.updateEmployeeBHAsync(EmployeeCode, bhxh, bhyt);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                _ = SQLManager_QLNS.Instance.InsertEmployeesInsuranceLogAsync(EmployeeCode, $"{row.Cells["SocialInsuranceNumber"].Value} - {row.Cells["HealthInsuranceNumber"].Value}",
                                    $"Success: {bhxh} - {bhyt}");

                                row.Cells["SocialInsuranceNumber"].Value = bhxh;
                                row.Cells["HealthInsuranceNumber"].Value = bhyt;

                                var parameters = new Dictionary<string, object>
                                {
                                    ["SocialInsuranceNumber"] = bhxh,
                                    ["HealthInsuranceNumber"] = bhyt
                                };
                                SQLStore_QLNS.Instance.updateEmploy(EmployeeCode, parameters);
                            }
                            else
                            {
                                _ = SQLManager_QLNS.Instance.InsertEmployeesInsuranceLogAsync(EmployeeCode, $"{row.Cells["SocialInsuranceNumber"].Value} - {row.Cells["HealthInsuranceNumber"].Value}",
                                    $"Fail: {bhxh} - {bhyt}");
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = SQLManager_QLNS.Instance.InsertEmployeesInsuranceLogAsync(EmployeeCode, $"{row.Cells["SocialInsuranceNumber"].Value} - {row.Cells["HealthInsuranceNumber"].Value}",
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
            bhxh_tb.ReadOnly = isReadOnly;
            bhyt_tb.ReadOnly = isReadOnly;
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
    }
}
