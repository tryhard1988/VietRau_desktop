using RauViet.classes;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{    
    public partial class AllowanceType : Form
    {
        private DataTable mApplyScope_dt;
        bool isNewState = false;
        public AllowanceType()
        {
            InitializeComponent();
            this.KeyPreview = true;
            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            allowanceName_tb.TabIndex = countTab++; allowanceName_tb.TabStop = true;
            allowanceCode_tb.TabIndex = countTab++; allowanceCode_tb.TabStop = true;
            ScopeName_cbb.TabIndex = countTab++; ScopeName_cbb.TabStop = true;
            isActive_cb.TabIndex = countTab++; isActive_cb.TabStop = true;
            isInsuranceIncluded_cb.TabIndex = countTab++; isInsuranceIncluded_cb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);

            this.KeyDown += AllowanceType_KeyDown;
        }

        private void AllowanceType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_QLNS.Instance.removeAllowanceType();
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
                // Chạy truy vấn trên thread riêng
                var allowanceTypeTask = SQLStore_QLNS.Instance.GetAllowanceTypeAsync();
                var applyScopeAsync = SQLStore_QLNS.Instance.GetApplyScopeAsync();

                await Task.WhenAll(allowanceTypeTask, applyScopeAsync);
                DataTable allowanceType_dt = allowanceTypeTask.Result;
                mApplyScope_dt = applyScopeAsync.Result;

                ScopeName_cbb.DataSource = mApplyScope_dt;
                ScopeName_cbb.DisplayMember = "ScopeName";
                ScopeName_cbb.ValueMember = "ApplyScopeID";

                

                int count = 0;
                allowanceType_dt.Columns["AllowanceName"].SetOrdinal(count++);
                allowanceType_dt.Columns["ScopeName"].SetOrdinal(count++);
                allowanceType_dt.Columns["IsInsuranceIncluded"].SetOrdinal(count++);
                allowanceType_dt.Columns["IsActive"].SetOrdinal(count++);
               
                                
                dataGV.DataSource = allowanceType_dt;
                Utils.HideColumns(dataGV, new[] { "AllowanceTypeID", "ApplyScopeID" });

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"AllowanceName", "Loại Phụ Cấp" },
                    {"ScopeName", "Nhóm Áp dụng" },
                    {"IsInsuranceIncluded", "Đóng Bảo Hiểm Không" },
                    {"IsActive", "Đang Hoạt Động" },
                    {"AllowanceCode", "Mã Phụ Cấp" }
                });

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
            if (isNewState == true) return;

            var cells = dataGV.Rows[index].Cells;
            int allowanceTypeID = Convert.ToInt32(cells["AllowanceTypeID"].Value);
            int applyScopeID = Convert.ToInt32(cells["ApplyScopeID"].Value);
            string allowanceName = cells["AllowanceName"].Value.ToString();
            string allowanceCode = cells["AllowanceCode"].Value.ToString();
            Boolean isActive = Convert.ToBoolean(cells["IsActive"].Value);
            Boolean isInsuranceIncluded = Convert.ToBoolean(cells["IsInsuranceIncluded"].Value);

            this.allowanceTypeID_tb.Text = allowanceTypeID.ToString();
            allowanceName_tb.Text = allowanceName;
            allowanceCode_tb.Text = allowanceCode;
            isInsuranceIncluded_cb.Checked = isInsuranceIncluded;
            this.isActive_cb.Checked = isActive;
            ScopeName_cbb.SelectedValue = applyScopeID;            
            status_lb.Text = "";
        }
        
        private async void updateData(int allowanceTypeID, string allowanceName, string allowanceCode, int applyScopeID, bool isActive,bool isInsuranceIncluded)
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
                            bool isScussess = await SQLManager_QLNS.Instance.updateAllowanceTypeAsync(allowanceTypeID, allowanceName, allowanceCode, applyScopeID, isActive, isInsuranceIncluded);
                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["AllowanceName"].Value = allowanceName;
                                row.Cells["AllowanceCode"].Value = allowanceCode;
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
                        catch
                        {
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }




                    }
                    break;
                }
            }
        }

        private async void createNew(string allowanceName, string allowanceCode, int applyScopeID, bool isActive, bool isInsuranceIncluded)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int allowanceTypeID = await SQLManager_QLNS.Instance.insertAllowanceTypeAsync(allowanceName, allowanceCode, applyScopeID, isActive, isInsuranceIncluded);
                    if (allowanceTypeID > 0)
                    {
                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();


                        allowanceTypeID_tb.Text = allowanceTypeID.ToString();
                        drToAdd["AllowanceTypeID"] = allowanceTypeID;
                        drToAdd["AllowanceName"] = allowanceName;
                        drToAdd["AllowanceCode"] = allowanceCode;
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
                catch
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
            string allowanceCode = allowanceCode_tb.Text;
            int applyScopeID = Convert.ToInt32(ScopeName_cbb.SelectedValue);
            bool isInsuranceIncluded = isInsuranceIncluded_cb.Checked;
            bool isActive = this.isActive_cb.Checked;

            if (allowanceName.CompareTo("") == 0)
            {
                MessageBox.Show("Sai Dữ Liệu, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (allowanceTypeID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(allowanceTypeID_tb.Text), allowanceName, allowanceCode, applyScopeID, isActive, isInsuranceIncluded);
            else
                createNew(allowanceName, allowanceCode, applyScopeID, isActive, isInsuranceIncluded);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string allowanceTypeID = allowanceTypeID_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string id = row.Cells["AllowanceTypeID"].Value.ToString();
                if (id.CompareTo(allowanceTypeID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.deleteAllowanceTypeAsync(Convert.ToInt32(allowanceTypeID));

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
                        catch
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
            allowanceCode_tb.Text = "";
            isInsuranceIncluded_cb.Checked = false;
            isActive_cb.Checked = true;

            status_lb.Text = "";

            allowanceName_tb.Focus();
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
            dataGV_CellClick(null, null);
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
            isInsuranceIncluded_cb.Enabled = !isReadOnly;
            isActive_cb.Enabled = !isReadOnly;
            allowanceCode_tb.ReadOnly = isReadOnly;
            allowanceName_tb.ReadOnly = isReadOnly;
            ScopeName_cbb.Enabled = !isReadOnly;
        }
    }
}
