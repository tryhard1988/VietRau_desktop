using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
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
                var employeeSalaryInfoAsync = SQLStore_QLNS.Instance.GetEmployeeSalaryInfoAsync();
                var employeeAllowanceAsync = SQLManager_QLNS.Instance.GetEmployeeAllowanceAsybc();
                var allowanceTypeTask = SQLStore_QLNS.Instance.GetAllowanceTypeAsync();

                await Task.WhenAll(employeesTask, employeeInsuranceLogTask, employeeSalaryInfoAsync, employeeAllowanceAsync, allowanceTypeTask);
                DataTable employee_dt = employeesTask.Result;
                mLogDV = new DataView(employeeInsuranceLogTask.Result);
                dataGV.DataSource = employee_dt;
                log_GV.DataSource = mLogDV;


                CalInsuranceSalary(employee_dt, employeeSalaryInfoAsync.Result, employeeAllowanceAsync.Result, allowanceTypeTask.Result);


                Utils.SetGridFormat_NO(dataGV, "NLD_InsuranceContribution");
                Utils.SetGridFormat_NO(dataGV, "CTY_InsuranceContribution");
                Utils.SetGridFormat_NO(dataGV, "Tong_InsuranceContribution");
                Utils.SetGridFormat_NO(dataGV, "InsuranceSalary");
                Utils.HideColumns(log_GV, new[] { "LogID", "EmployeeCode" });
                Utils.HideColumns(dataGV, new[] { "EmployessName_NoSign" });

                Utils.SetGridFormat_Alignment(dataGV, "InsuranceSalary", DataGridViewContentAlignment.MiddleRight);
                Utils.SetGridFormat_Alignment(dataGV, "NLD_InsuranceContribution", DataGridViewContentAlignment.MiddleRight);
                Utils.SetGridFormat_Alignment(dataGV, "CTY_InsuranceContribution", DataGridViewContentAlignment.MiddleRight);
                Utils.SetGridFormat_Alignment(dataGV, "Tong_InsuranceContribution", DataGridViewContentAlignment.MiddleRight);

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"EmployeeCode", "Mã NV" },
                    {"FullName", "Tên NV" },
                    {"SocialInsuranceNumber", "BHXH" },
                    {"HealthInsuranceNumber", "BHYT" },
                    {"InsuranceSalary", "Lương Đóng BH" },
                    {"Tong_InsuranceContribution", "Tiền Nộp BH\n(32% Lương)" },
                    {"NLD_InsuranceContribution", "NLĐ Nộp BH\n(10.5% Lương)" },
                    {"CTY_InsuranceContribution", "Cty Nộp BH\n(21.5% Lương)" }
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

                Utils.SetGridWidth(log_GV, "OldValue", DataGridViewAutoSizeColumnMode.Fill);
                Utils.SetGridWidth(log_GV, "NewValue", DataGridViewAutoSizeColumnMode.Fill);
                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    UpdateRightUI(0);
                }

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);

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

        void CalInsuranceSalary(DataTable employee_dt, DataTable SalaryInfo_dt, DataTable employeeAllowance_dt, DataTable allowanceType_dt)
        {
            // 1. Add IsInsuranceIncluded
            Utils.AddColumnIfNotExists(employeeAllowance_dt, "IsInsuranceIncluded", typeof(bool));

            var allowanceTypeDict = allowanceType_dt.AsEnumerable()
                .ToDictionary(
                    r => r.Field<int>("AllowanceTypeID"),
                    r => r.Field<bool>("IsInsuranceIncluded")
                );

            foreach (DataRow row in employeeAllowance_dt.Rows)
            {
                int allowanceTypeID = row.Field<int>("AllowanceTypeID");
                row["IsInsuranceIncluded"] = allowanceTypeDict.ContainsKey(allowanceTypeID)? allowanceTypeDict[allowanceTypeID]: false;
            }

            // 2. Lấy lương mới nhất theo EmployeeCode
            var latestSalaryDict = SalaryInfo_dt.AsEnumerable().GroupBy(r => r.Field<string>("EmployeeCode"))
                                                .Select(g => g.OrderByDescending(r => r.Field<int>("Year"))
                                                .ThenByDescending(r => r.Field<int>("Month"))
                                                .First())
                                                .ToDictionary(
                                                    r => r.Field<string>("EmployeeCode"),
                                                    r => r.Field<int>("InsuranceBaseSalary")
                                                );

            // 3. Sum phụ cấp được tính BH theo EmployeeCode
            var allowanceSumDict = employeeAllowance_dt.AsEnumerable()
                                                        .Where(r => r.Field<bool>("IsInsuranceIncluded") == true)
                                                        .GroupBy(r => r.Field<string>("EmployeeCode"))
                                                        .ToDictionary(
                                                            g => g.Key,
                                                            g => g.Sum(r => r.Field<decimal>("Amount"))
                                                        );

            // 4. Add cột InsuranceSalary
            Utils.AddColumnIfNotExists(employee_dt, "InsuranceSalary", typeof(int));
            Utils.AddColumnIfNotExists(employee_dt, "Tong_InsuranceContribution", typeof(int));
            Utils.AddColumnIfNotExists(employee_dt, "NLD_InsuranceContribution", typeof(int));
            Utils.AddColumnIfNotExists(employee_dt, "CTY_InsuranceContribution", typeof(int));

            int totalLuong = 0;
            int totalNop = 0;
            int NLDNop = 0;
            int CtyNop = 0;
            foreach (DataRow row in employee_dt.Rows)
            {
                string employeeCode = row.Field<string>("EmployeeCode");

                int baseSalary = latestSalaryDict.ContainsKey(employeeCode)? latestSalaryDict[employeeCode]: 0;
                int allowanceSum = Convert.ToInt32(allowanceSumDict.ContainsKey(employeeCode)? allowanceSumDict[employeeCode]: 0);

                row["InsuranceSalary"] = baseSalary + allowanceSum;
                row["Tong_InsuranceContribution"] = (baseSalary + allowanceSum) * 0.32m;
                row["NLD_InsuranceContribution"] = (baseSalary + allowanceSum) * 0.105m;
                row["CTY_InsuranceContribution"] = (baseSalary + allowanceSum) * 0.215m;

                totalLuong += (baseSalary + allowanceSum);
                totalNop += Convert.ToInt32((baseSalary + allowanceSum) * 0.32m);
                NLDNop += Convert.ToInt32((baseSalary + allowanceSum) * 0.105m);
                CtyNop += Convert.ToInt32((baseSalary + allowanceSum) * 0.215m);
            }

            tongLuong_lb.Text = totalLuong.ToString("N0");
            tongNop_lb.Text = totalNop.ToString("N0");
            nld_lb.Text = NLDNop.ToString("N0");
            cty_lb.Text = CtyNop.ToString("N0");
        }
    }
}
