using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RauViet.ui
{
    public partial class EmployeeBaoHiem : Form
    { 
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

                await Task.WhenAll(employeesTask);
                DataTable employee_dt = employeesTask.Result;

                dataGV.DataSource = employee_dt;

                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["FullName"].HeaderText = "Tên NV";
                dataGV.Columns["SocialInsuranceNumber"].HeaderText = "BHXH";
                dataGV.Columns["HealthInsuranceNumber"].HeaderText = "BHYT";

                dataGV.Columns["EmployeeCode"].Width = 60;
                dataGV.Columns["FullName"].Width = 160;
                dataGV.Columns["SocialInsuranceNumber"].Width = 160;
                dataGV.Columns["HealthInsuranceNumber"].Width = 160;

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

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
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
            if (employeeCode_tb.Text.Length != 0)
                updateData(employeeCode_tb.Text, bhxh_tb.Text, bhyt_tb.Text);

        }
    }
}
