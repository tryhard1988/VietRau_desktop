using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Vml.Office;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.ui
{
    public partial class AnnualLeaveBalance : Form
    {
        DataTable mEmployee_dt;
        object oldValue;
        // DataTable mShift_dt;
        public AnnualLeaveBalance()
        {
            InitializeComponent();
            this.KeyPreview = true;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = true;

            status_lb.Text = "";

            dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGV.CellFormatting += DataGV_CellFormatting;

            capphep_btn.Click += Capphep_btn_Click;
            ResetPhep_btn.Click += ResetPhep_btn_Click;

            dataGV.CellBeginEdit += dataGV_CellBeginEdit;
            dataGV.CellEndEdit += DataGV_CellEndEdit;
            this.KeyDown += AnnualLeaveBalance_KeyDown;
        }

        private void AnnualLeaveBalance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_QLNS.Instance.removeAnnualLeaveBalance();
                ShowData();
            }
        }

        public async void ShowData()
        {
            await Task.Delay(50);
            LoadingOverlay loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);

            try
            {
                var employeeALBTask = SQLStore_QLNS.Instance.GetAnnualLeaveBalanceAsync();
                await Task.WhenAll(employeeALBTask);
                mEmployee_dt = employeeALBTask.Result;

                DefineEmployeeGV();
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

        private void DefineEmployeeGV()
        {
            int count = 0;
            mEmployee_dt.Columns["EmployeeCode"].SetOrdinal(count++);
            mEmployee_dt.Columns["FullName"].SetOrdinal(count++);
            mEmployee_dt.Columns["PositionName"].SetOrdinal(count++);

            dataGV.ReadOnly = false;
            
            mEmployee_dt.Columns["RemainingLeaveDays"].ReadOnly = false;

            dataGV.DataSource = mEmployee_dt;

            foreach (DataGridViewColumn col in dataGV.Columns)
            {
                col.ReadOnly = true;
            }
            dataGV.Columns["RemainingLeaveDays_1"].ReadOnly = false;
            dataGV.Columns["EmployessName_NoSign"].Visible = false;
            dataGV.Columns["RemainingLeaveDays"].Visible = false;
            dataGV.Columns["LeaveCount"].Visible = false;

            dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
            dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
            dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
            dataGV.Columns["Month"].HeaderText = "Tháng Có phép";
            dataGV.Columns["RemainingLeaveDays_1"].HeaderText = "Còn Lại";

            dataGV.Columns["EmployeeCode"].Width = 70;
            dataGV.Columns["FullName"].Width = 200;
            dataGV.Columns["Month"].Width = 200;
            dataGV.Columns["PositionName"].Width = 90;
            dataGV.Columns["RemainingLeaveDays_1"].Width = 70;

            if (dataGV.Rows.Count > 0)
            {
                dataGV.ClearSelection();
                dataGV.Rows[0].Selected = true;
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

        private void Capphep_btn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show($"Cấp thêm 1 Phép cho nhân viên đang chọn ?", "Cập Nhật Tồn Phép", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;


            foreach (DataGridViewRow dr in dataGV.SelectedRows)
            {
                int remainingLeaveDays = 0;

                if (dr.Cells["RemainingLeaveDays"].Value != DBNull.Value)
                    remainingLeaveDays = Convert.ToInt32(dr.Cells["RemainingLeaveDays"].Value);

                dr.Cells["RemainingLeaveDays"].Value = remainingLeaveDays + 1;
                dr.Cells["RemainingLeaveDays_1"].Value = remainingLeaveDays + 1 - Convert.ToInt32(dr.Cells["LeaveCount"].Value);
            }

              SaveData(false);
        }

        private void ResetPhep_btn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show($"Reset Phép Năm cho tất cả nhân viên?", "Cập Nhật Tồn Phép", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            int prevMonth = DateTime.Now.Month;
            foreach (DataRow dr in mEmployee_dt.Rows)
            {
                int remainingLeaveDays = 0;

                if (dr["RemainingLeaveDays_1"] != DBNull.Value)
                    remainingLeaveDays = Convert.ToInt32(dr["RemainingLeaveDays_1"]);

                int remainingLeaveDays_new = Math.Min(remainingLeaveDays, prevMonth);

                dr["RemainingLeaveDays_1"] = remainingLeaveDays_new;
                dr["RemainingLeaveDays"] = remainingLeaveDays_new;// Convert.ToInt32(dr.Cells["RemainingLeaveDays"].Value) - (remainingLeaveDays - remainingLeaveDays_new);
            }

            SaveData(true);
        }


        public async void SaveData(bool isResetPhep)
        {
            List<(string EmployeeCode, int RemainingLeaveDays)> albData = new List<(string, int)>();

            foreach (DataRow dr in mEmployee_dt.Rows)
            {
                string employeeCode = Convert.ToString(dr["EmployeeCode"]);
                int remainingLeaveDays = Convert.ToInt32(dr["RemainingLeaveDays"]);

                albData.Add((employeeCode, remainingLeaveDays));

            }

            try
            {
                int prevMonth = DateTime.Now.AddMonths(-1).Month;

                Boolean iSuccess = await SQLManager_QLNS.Instance.UpsertAnnualLeaveBalanceBatchAsync(albData, isResetPhep, prevMonth, DateTime.Now.Year);
                if (iSuccess)
                {
                    status_lb.Text = "Thành Công.";
                    status_lb.ForeColor = Color.Green;
                    if (isResetPhep)
                    {
                        SQLStore_QLNS.Instance.removeLeaveAttendances(DateTime.Now.Year);
                        SQLStore_QLNS.Instance.removeLeaveAttendances(DateTime.Now.AddYears(-1).Year);
                        await SQLStore_QLNS.Instance.GetLeaveAttendancesAsyn(DateTime.Now.Year);
                    }
                }
                else
                {
                    status_lb.Text = "";
                    MessageBox.Show("Thất Bại", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                status_lb.Text = "";
                MessageBox.Show("Thất Bại", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void DataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGV.Columns[e.ColumnIndex].Name == "RemainingLeaveDays_1")
            {
                e.CellStyle.BackColor = System.Drawing.Color.LightGray;
            }
        }

        private void dataGV_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            oldValue = dataGV.CurrentCell?.Value;
        }
        private async void DataGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                object newValue = dataGV.CurrentCell?.Value;
                if (object.Equals(oldValue, newValue)) return;

                var row = dataGV.CurrentRow;

                int newValueInt = 0;
                int oldValueInt = 0;

                if (newValue != null && int.TryParse(newValue.ToString(), out int tmpNew))
                    newValueInt = tmpNew;

                if (oldValue != null && int.TryParse(oldValue.ToString(), out int tmpOld))
                    oldValueInt = tmpOld;

                List<(string EmployeeCode, int RemainingLeaveDays)> albData = new List<(string, int)>();
                string employeeCode = Convert.ToString(row.Cells["EmployeeCode"].Value);
                int remainingLeaveDays = Convert.ToInt32(row.Cells["RemainingLeaveDays"].Value) - (oldValueInt - newValueInt);

                albData.Add((employeeCode, remainingLeaveDays));
                row.Cells["RemainingLeaveDays"].Value = remainingLeaveDays;
                try
                {
                    int prevMonth = DateTime.Now.AddMonths(-1).Month;
                    Boolean iSuccess = await SQLManager_QLNS.Instance.UpsertAnnualLeaveBalanceBatchAsync(albData , false, prevMonth, DateTime.Now.Year);
                    if (iSuccess)
                    {
                        status_lb.Text = "Thành Công.";
                        status_lb.ForeColor = Color.Green;
                    }
                    else
                    {
                        status_lb.Text = "";
                        MessageBox.Show("Thất Bại", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    status_lb.Text = "";
                    MessageBox.Show("Thất Bại", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Title = "Chọn file Excel";
                ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
                ofd.Multiselect = false; // chỉ cho chọn 1 file

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    System.Data.DataTable excelData = Utils.LoadExcel_NoHeader(filePath);

                    List<(string EmployeeCode, int RemainingLeaveDays)> albData = new List<(string, int)>();

                    foreach (DataRow dr in excelData.Rows)
                    {
                        string employeeCode = Convert.ToString(dr["Column1"]);
                        int remainingLeaveDays = 0;

                        if (dr["Column2"] != DBNull.Value && int.TryParse(dr["Column2"].ToString().Trim(), out int value))
                        {
                            remainingLeaveDays = value;
                        }

                        albData.Add((employeeCode, remainingLeaveDays));

                    }

                    try
                    {
                        int prevMonth = DateTime.Now.AddMonths(-1).Month;

                        Boolean iSuccess = await SQLManager_QLNS.Instance.UpsertAnnualLeaveBalanceBatchAsync(albData, false, prevMonth, 2025);
                        if (iSuccess)
                        {
                            status_lb.Text = "Thành Công.";
                            status_lb.ForeColor = Color.Green;                            
                        }
                        else
                        {
                            status_lb.Text = "";
                            MessageBox.Show("Thất Bại", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch
                    {
                        status_lb.Text = "";
                        MessageBox.Show("Thất Bại", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        //private async void button1_Click_1(object sender, EventArgs e)
        //{
        //    using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
        //    {
        //        ofd.Title = "Chọn file Excel";
        //        ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
        //        ofd.Multiselect = false; // chỉ cho chọn 1 file

        //        if (ofd.ShowDialog() == DialogResult.OK)
        //        {
        //            string filePath = ofd.FileName;
        //            System.Data.DataTable excelData = Utils.LoadExcel_NoHeader(filePath);
        //            try
        //            {
        //                foreach (DataRow edr in excelData.Rows)
        //                {
        //                    string empCode = edr["Column1"].ToString();
        //                    bool exists = mEmployee_dt.AsEnumerable().Any(r => r.Field<string>("EmployeeCode") == empCode);
        //                    if (!exists) continue;


        //                    int phep = Convert.ToInt32(edr["Column2"]);

        //                    await SQLManager_QLNS.Instance.PhepAsync(empCode, phep);

        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("có lỗi " + ex.Message);
        //            }
        //            MessageBox.Show("xong");
        //        }
        //    }
        //}
    }
}
