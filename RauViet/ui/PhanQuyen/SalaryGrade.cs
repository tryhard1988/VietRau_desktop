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
    public partial class SalaryGrade : Form
    {
        DataView mLogDV;
        bool isNewState = false;
        public SalaryGrade()
        {
            InitializeComponent();

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            salaryName_tb.TabIndex = countTab++; salaryName_tb.TabStop = true;
            salary_tb.TabIndex = countTab++; salary_tb.TabStop = true;
            description_tb.TabIndex = countTab++; description_tb.TabStop = true;
            isActive_cb.TabIndex = countTab++; isActive_cb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);
        }

        public async void ShowData()
        {
            loading_lb.Visible = true;            

            try
            {
                // Chạy truy vấn trên thread riêng
                var salaryGradeTask = SQLStore.Instance.GetSalaryGradeAsync();
                var salaryGradeLogTask = SQLManager.Instance.GetSalaryGrade_LogAsync();
                await Task.WhenAll(salaryGradeTask, salaryGradeLogTask);
                DataTable salaryGrade_dt = salaryGradeTask.Result;
                mLogDV = new DataView(salaryGradeLogTask.Result);

                dataGV.DataSource = salaryGrade_dt;
                log_GV.DataSource = mLogDV;

                dataGV.Columns["GradeName"].HeaderText = "Tên bậc lương";
                dataGV.Columns["Salary"].HeaderText = "Mức lương";
                dataGV.Columns["Note"].HeaderText = "Diễn Giải";
                dataGV.Columns["IsActive"].HeaderText = "Đang Hoạt Động";

                dataGV.Columns["SalaryGradeID"].Visible = false;
                log_GV.Columns["LogID"].Visible = false;
                log_GV.Columns["GradeID"].Visible = false;

                dataGV.Columns["GradeName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["Salary"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["Note"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["IsActive"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

               // dataGV.Columns["UserID"].Visible = false;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
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
            if (isNewState) return;
            var cells = dataGV.Rows[index].Cells;
            int salaryGradeID = Convert.ToInt32(cells["SalaryGradeID"].Value);
            string gradeName = cells["GradeName"].Value.ToString();
            string salary = cells["Salary"].Value.ToString();
            string note = cells["Note"].Value.ToString();
            Boolean isActive = Convert.ToBoolean(cells["IsActive"].Value);

            salaryGradeID_tb.Text = salaryGradeID.ToString();
            salaryName_tb.Text = gradeName;
            salary_tb.Text = salary;
            description_tb.Text = note;      
            isActive_cb.Checked = isActive;

            status_lb.Text = "";

            mLogDV.RowFilter = $"GradeID = {salaryGradeID}";
        }

        private async void updateData(int salaryGradeID, string gradeName, int salary, string note, bool isActive)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["SalaryGradeID"].Value);
                if (id.CompareTo(salaryGradeID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        string oldValue = $"{row.Cells["GradeName"].Value} - {row.Cells["Salary"].Value} - {row.Cells["Note"].Value} - {row.Cells["IsActive"].Value}";
                        string newValue = $"{gradeName} - {salary} - {note} - {isActive}";
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateSalaryGradeAsync(salaryGradeID, gradeName, salary, note, isActive);

                            if (isScussess == true)
                            {
                                newValue = "edit Success " + newValue;
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["GradeName"].Value = gradeName;
                                row.Cells["Salary"].Value = salary;
                                row.Cells["Note"].Value = note;
                                row.Cells["IsActive"].Value = isActive;
                            }
                            else
                            {
                                newValue = "edit Fail " + newValue;
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            newValue = "edit Fail Exception: " + ex.Message + ": " + newValue;
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }


                        _ = SQLManager.Instance.InsertSalaryGrade_LogAsync(salaryGradeID, oldValue, newValue);
                        
                    }
                    break;
                }
            }
        }

        private async void createNew(string gradeName, int salary, string note, bool isActive)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string newValue = $"{gradeName} - {salary} - {note} - {isActive}";
                int newId = -1;
                try
                {
                    newId = await SQLManager.Instance.insertSalaryGradeAsync(gradeName, salary, note, isActive);
                    if (newId > 0)
                    {
                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["SalaryGradeID"] = newId;
                        drToAdd["GradeName"] = gradeName;
                        drToAdd["Salary"] = salary;
                        drToAdd["Note"] = note;
                        drToAdd["IsActive"] = isActive;

                        salaryGradeID_tb.Text = newId.ToString();


                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

                        dataGV.ClearSelection(); // bỏ chọn row cũ
                        int rowIndex = dataGV.Rows.Count - 1;
                        dataGV.Rows[rowIndex].Selected = true;
                        UpdateRightUI(dataGV.Rows.Count - 1);

                        newValue += "Create Success " + newValue;
                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;
                    }
                    else
                    {
                        newValue += "Create Fail " + newValue;
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    newValue += "Create Fail Exception: " + ex.Message + ": " + newValue;
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

                _ = SQLManager.Instance.InsertSalaryGrade_LogAsync(newId, "New", newValue);
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (salary_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            string gradeName = salaryName_tb.Text;
            int salary = Convert.ToInt32(salary_tb.Text);
            string note = description_tb.Text;
            bool isActive = isActive_cb.Checked;

            if (salaryGradeID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(salaryGradeID_tb.Text), gradeName, salary, note, isActive);
            else
                createNew(gradeName, salary, note, isActive);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string id = salaryGradeID_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string salaryGradeID = row.Cells["SalaryGradeID"].Value.ToString();
                if (salaryGradeID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        string oldValue = $"{row.Cells["GradeName"].Value} - {row.Cells["Salary"].Value} - {row.Cells["Note"].Value} - {row.Cells["IsActive"].Value}";
                        string newValue = "";
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deletePositionAsync(Convert.ToInt32(salaryGradeID));

                            if (isScussess == true)
                            {
                                newValue = "Delete Success";
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                int delRowInd = row.Index;
                                dataGV.Rows.Remove(row);
                            }
                            else
                            {
                                newValue = "Delete Fail";
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            newValue = "Delete Fail Exception: " + ex.Message;
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }

                        
                        _ = SQLManager.Instance.InsertSalaryGrade_LogAsync(Convert.ToInt32(salaryGradeID), "Delete " + oldValue, newValue);

                    }
                    break;
                }
            }
        }

        private void newCustomerBtn_Click(object sender, EventArgs e)
        {
            salaryName_tb.Text = "";
            salaryGradeID_tb.Text = "";
            salary_tb.Text = "";
            description_tb.Text = "";
            isActive_cb.Checked = true;

            status_lb.Text = "";
            salaryName_tb.Focus();
            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = false;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            SetUIReadOnly(false);
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            delete_btn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            SetUIReadOnly(true);
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
            SetUIReadOnly(false);
        }

        private void SetUIReadOnly(bool isReadOnly)
        {
            isActive_cb.Enabled = !isReadOnly;
            salaryName_tb.ReadOnly = isReadOnly;
            salary_tb.ReadOnly = isReadOnly;
            description_tb.Enabled = !isReadOnly;
        }
    }
}
