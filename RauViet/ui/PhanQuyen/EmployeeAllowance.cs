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
    public partial class EmployeeAllowance : Form
    {
        private DataTable mEmployeeAllowance_dt, mAllowanceType_dt;
        public EmployeeAllowance()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            allowanceGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            allowanceGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;
            delete_btn.Enabled = false;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            allowanceGV.SelectionChanged += this.allowanceGV_CellClick;
            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                var employeeTask = SQLManager.Instance.GetActiveEmployeeAsync();
                var employeeAllowanceAsync = SQLManager.Instance.GetEmployeeAllowanceAsybc();
                var allowanceTypeAsync = SQLManager.Instance.GetAllowanceTypeAsync("EMP");

                await Task.WhenAll(employeeTask, employeeAllowanceAsync, allowanceTypeAsync);
                DataTable employee_dt = employeeTask.Result;
                mEmployeeAllowance_dt = employeeAllowanceAsync.Result;
                mAllowanceType_dt = allowanceTypeAsync.Result;

                mEmployeeAllowance_dt.Columns.Add(new DataColumn("AllowanceName", typeof(string)));
                foreach (DataRow dr in mEmployeeAllowance_dt.Rows)
                {
                    int allowanceTypeID = Convert.ToInt32(dr["AllowanceTypeID"]);
                    DataRow[] applyScopeRows = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}");

                    if (applyScopeRows.Length > 0)
                        dr["AllowanceName"] = applyScopeRows[0]["AllowanceName"].ToString();
                }

                foreach (DataRow dr in employee_dt.Rows)
                {
                    if (dr["PositionName"] == DBNull.Value) dr["PositionName"] = "";
                    if (dr["ContractTypeName"] == DBNull.Value) dr["ContractTypeName"] = "";
                }


                DataView dv = new DataView(mAllowanceType_dt);
                dv.RowFilter = "IsActive = 1";
                allowanceType_cbb.DataSource = dv;
                allowanceType_cbb.DisplayMember = "AllowanceName";
                allowanceType_cbb.ValueMember = "AllowanceTypeID";

                allowanceGV.DataSource = mEmployeeAllowance_dt;

                dataGV.AutoGenerateColumns = true;
                dataGV.DataSource = employee_dt;

                allowanceGV.Columns["EmployeeAllowanceID"].Visible = false;
                allowanceGV.Columns["EmployeeCode"].Visible = false;
                allowanceGV.Columns["AllowanceTypeID"].Visible = false;

                int count = 0;
                mEmployeeAllowance_dt.Columns["AllowanceName"].SetOrdinal(count++);
                mEmployeeAllowance_dt.Columns["Amount"].SetOrdinal(count++);
                mEmployeeAllowance_dt.Columns["Note"].SetOrdinal(count++);


                allowanceGV.Columns["AllowanceName"].HeaderText = "Loại Phụ Cấp";
                allowanceGV.Columns["Amount"].HeaderText = "Số Tiền";
                allowanceGV.Columns["Note"].HeaderText = "Ghi Chú";

                dataGV.Columns["FullName"].HeaderText = "Tên Nhân Viên";
                dataGV.Columns["EmployeeCode"].HeaderText = "Mã NV";
                dataGV.Columns["PositionName"].HeaderText = "Chức Vụ";
                dataGV.Columns["ContractTypeName"].HeaderText = "Loại Hợp Đồng";
                //dataGV.Columns["IsActive"].HeaderText = "Đang Hoạt Động";

                //dataGV.Columns["AllowanceTypeID"].Visible = false;
                //dataGV.Columns["ApplyScopeID"].Visible = false;

                allowanceGV.Columns["AllowanceName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                allowanceGV.Columns["Amount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                allowanceGV.Columns["Note"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                 //   UpdateAllowancetUI(0);
                }


                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                allowanceGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
            if (allowanceGV.CurrentRow == null) return;
            int rowIndex = allowanceGV.CurrentRow.Index;
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


            UpdateAllowancetUI(rowIndex);
        }

        private void UpdateAllowancetUI(int index)
        {
            var cells = dataGV.Rows[index].Cells;
            string employeeCode = Convert.ToString(cells["EmployeeCode"].Value);

            DataView dv = new DataView(mEmployeeAllowance_dt);
            dv.RowFilter = $"EmployeeCode = '{employeeCode}'";

            allowanceGV.DataSource = dv;
        }
        private void UpdateRightUI(int index)
        {
            var cells = allowanceGV.Rows[index].Cells;
            int employeeAllowanceID = Convert.ToInt32(cells["EmployeeAllowanceID"].Value);
            int allowanceTypeID = Convert.ToInt32(cells["AllowanceTypeID"].Value);
            int amount = Convert.ToInt32(cells["Amount"].Value);
            string note = cells["Note"].Value.ToString();

            this.employeeAllowanceID_tb.Text = employeeAllowanceID.ToString();
            amount_tb.Text = amount.ToString();
            note_tb.Text = note;
            allowanceType_cbb.SelectedValue = allowanceTypeID;

            delete_btn.Enabled = true;

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }
        
        private async void updateData(int employeeAllowanceID, string employeeCode, int amount, int allowanceTypeID, string note)
        {
            foreach (DataGridViewRow row in allowanceGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["EmployeeAllowanceID"].Value);
                if (id.CompareTo(employeeAllowanceID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateEmployeeAllowanceAsync(employeeAllowanceID, employeeCode, amount, allowanceTypeID, note);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["EmployeeCode"].Value = employeeCode;
                                row.Cells["AllowanceTypeID"].Value = allowanceTypeID;
                                row.Cells["Amount"].Value = amount;
                                row.Cells["Note"].Value = note;
                                row.Cells["AllowanceName"].Value = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}")[0]["AllowanceName"].ToString();
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

        private async void createNew(string employeeCode, int amount, int allowanceTypeID, string note)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int employeeAllowanceID = await SQLManager.Instance.insertEmployeeAllowanceAsync(employeeCode, amount, allowanceTypeID, note);
                    if (allowanceTypeID > 0)
                    {
                        DataRow drToAdd = mEmployeeAllowance_dt.NewRow();

                        drToAdd["EmployeeAllowanceID"] = employeeAllowanceID;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["AllowanceTypeID"] = allowanceTypeID;
                        drToAdd["Amount"] = amount;
                        drToAdd["Note"] = note;
                        drToAdd["AllowanceName"] = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}")[0]["AllowanceName"].ToString();

                        mEmployeeAllowance_dt.Rows.Add(drToAdd);
                        mEmployeeAllowance_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

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

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (allowanceType_cbb.SelectedValue == null || string.IsNullOrEmpty(amount_tb.Text) || dataGV.CurrentRow == null)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string employeeCode = Convert.ToString(dataGV.CurrentRow.Cells["EmployeeCode"].Value);
            int amount = Convert.ToInt32(amount_tb.Text);
            int allowanceTypeID = Convert.ToInt32(allowanceType_cbb.SelectedValue);
            string note= note_tb.Text;

            if (employeeAllowanceID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(employeeAllowanceID_tb.Text), employeeCode, amount, allowanceTypeID, note);
            else
                createNew(employeeCode, amount, allowanceTypeID, note);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string employeeAllowanceID = employeeAllowanceID_tb.Text;

            foreach (DataGridViewRow row in allowanceGV.Rows)
            {
                string id = row.Cells["EmployeeAllowanceID"].Value.ToString();
                if (id.CompareTo(employeeAllowanceID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteEmployeeAllowanceAsync(Convert.ToInt32(employeeAllowanceID));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                int delRowInd = row.Index;
                                allowanceGV.Rows.Remove(row);
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
            employeeAllowanceID_tb.Text = "";
            amount_tb.Text = "";

            status_lb.Text = "";
            delete_btn.Enabled = false;
            info_gb.BackColor = Color.Green;
         //   dataGV.ClearSelection();
            return;            
        }
    }
}
