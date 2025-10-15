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
    public partial class AllowanceType : Form
    {
        private DataTable mDepartment_dt, mPosition_dt, mApplyScope_dt;
        public AllowanceType()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;
            delete_btn.Enabled = false;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
        }

        public async void ShowData()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                // Chạy truy vấn trên thread riêng
                var allowanceTypeTask = SQLManager.Instance.GetAllowanceTypeAsync();
                var applyScopeAsync = SQLManager.Instance.GetApplyScopeAsync();

                await Task.WhenAll(allowanceTypeTask, applyScopeAsync);
                DataTable allowanceType_dt = allowanceTypeTask.Result;
                mApplyScope_dt = applyScopeAsync.Result;

                ScopeName_cbb.DataSource = mApplyScope_dt;
                ScopeName_cbb.DisplayMember = "ScopeName";
                ScopeName_cbb.ValueMember = "ApplyScopeID";

                allowanceType_dt.Columns.Add(new DataColumn("ScopeName", typeof(string)));

                foreach (DataRow dr in allowanceType_dt.Rows)
                {
                    int applyScopeID = Convert.ToInt32(dr["ApplyScopeID"]);

                    string departmentName = "";
                    DataRow[] applyScopeRows = mApplyScope_dt.Select($"ApplyScopeID = {applyScopeID}");
                    if (applyScopeRows.Length > 0)
                        dr["ScopeName"] = applyScopeRows[0]["ScopeName"].ToString();
                }

                int count = 0;
                allowanceType_dt.Columns["AllowanceName"].SetOrdinal(count++);
                allowanceType_dt.Columns["ScopeName"].SetOrdinal(count++);
                allowanceType_dt.Columns["IsInsuranceIncluded"].SetOrdinal(count++);
                allowanceType_dt.Columns["IsActive"].SetOrdinal(count++);
               
                                
                dataGV.DataSource = allowanceType_dt;

                dataGV.Columns["AllowanceName"].HeaderText = "Loại Phụ Cấp";
                dataGV.Columns["ScopeName"].HeaderText = "Nhóm Áp dụng";
                dataGV.Columns["IsInsuranceIncluded"].HeaderText = "Đóng Bảo Hiểm Không";
                dataGV.Columns["IsActive"].HeaderText = "Đang Hoạt Động";
              
                dataGV.Columns["AllowanceTypeID"].Visible = false;
                dataGV.Columns["ApplyScopeID"].Visible = false;

                dataGV.Columns["AllowanceName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["ScopeName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["IsInsuranceIncluded"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["IsActive"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

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
            int allowanceTypeID = Convert.ToInt32(cells["AllowanceTypeID"].Value);
            int applyScopeID = Convert.ToInt32(cells["ApplyScopeID"].Value);
            string allowanceName = cells["AllowanceName"].Value.ToString();
            Boolean isActive = Convert.ToBoolean(cells["IsActive"].Value);
            Boolean isInsuranceIncluded = Convert.ToBoolean(cells["IsInsuranceIncluded"].Value);

            this.allowanceTypeID_tb.Text = allowanceTypeID.ToString();
            allowanceName_tb.Text = allowanceName;
            isInsuranceIncluded_cb.Checked = isInsuranceIncluded;
            this.isActive_cb.Checked = isActive;
            ScopeName_cbb.SelectedValue = applyScopeID;
            delete_btn.Enabled = true;

            info_gb.BackColor = Color.DarkGray;
            status_lb.Text = "";
        }
        
        private async void updateData(int allowanceTypeID, string allowanceName, int applyScopeID, bool isActive,bool isInsuranceIncluded)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["AllowanceTypeID"].Value);
                if (id.CompareTo(allowanceTypeID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateAllowanceTypeAsync(allowanceTypeID, allowanceName, applyScopeID, isActive, isInsuranceIncluded);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["AllowanceName"].Value = allowanceName;
                                row.Cells["IsInsuranceIncluded"].Value = isInsuranceIncluded;
                                row.Cells["ApplyScopeID"].Value = applyScopeID;
                                row.Cells["IsActive"].Value = isActive;
                                row.Cells["ScopeName"].Value = mApplyScope_dt.Select($"ApplyScopeID = {applyScopeID}")[0]["ScopeName"].ToString();
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

        private async void createNew(string allowanceName, int applyScopeID, bool isActive, bool isInsuranceIncluded)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int allowanceTypeID = await SQLManager.Instance.insertAllowanceTypeAsync(allowanceName, applyScopeID, isActive, isInsuranceIncluded);
                    if (allowanceTypeID > 0)
                    {
                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();


                        allowanceTypeID_tb.Text = allowanceTypeID.ToString();
                        drToAdd["AllowanceTypeID"] = allowanceTypeID;
                        drToAdd["AllowanceName"] = allowanceName;
                        drToAdd["IsInsuranceIncluded"] = isInsuranceIncluded;
                        drToAdd["ApplyScopeID"] = applyScopeID;
                        drToAdd["IsActive"] = isActive;
                        drToAdd["ScopeName"] = mApplyScope_dt.Select($"ApplyScopeID = {applyScopeID}")[0]["ScopeName"].ToString();

                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

                        dataGV.ClearSelection(); // bỏ chọn row cũ
                        int rowIndex = dataGV.Rows.Count - 1;
                        dataGV.Rows[rowIndex].Selected = true;
                        UpdateRightUI(dataGV.Rows.Count - 1);


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
            if (ScopeName_cbb.SelectedValue == null) 
                return;

            string allowanceName = allowanceName_tb.Text;
            int applyScopeID = Convert.ToInt32(ScopeName_cbb.SelectedValue);
            bool isInsuranceIncluded = isInsuranceIncluded_cb.Checked;
            bool isActive = this.isActive_cb.Checked;

            if (allowanceName.CompareTo("") == 0)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (allowanceTypeID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(allowanceTypeID_tb.Text), allowanceName, applyScopeID, isActive, isInsuranceIncluded);
            else
                createNew(allowanceName, applyScopeID, isActive, isInsuranceIncluded);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string employeeID = allowanceTypeID_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string id = row.Cells["EmployeeID"].Value.ToString();
                if (id.CompareTo(employeeID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteEmployeeAsync(Convert.ToInt32(employeeID));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                int delRowInd = row.Index;
                                dataGV.Rows.Remove(row);
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
            allowanceTypeID_tb.Text = "";
            allowanceName_tb.Text = "";
            isInsuranceIncluded_cb.Checked = false;
            isActive_cb.Checked = true;

            status_lb.Text = "";
            delete_btn.Enabled = false;
            info_gb.BackColor = Color.Green;
         //   dataGV.ClearSelection();
            return;            
        }
    }
}
