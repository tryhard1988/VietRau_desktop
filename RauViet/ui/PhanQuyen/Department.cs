using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class Department : Form
    {
        DataTable mDepartment_dt;
        bool isNewState = false;
        public Department()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            departmentName_tb.TabIndex = countTab++; departmentName_tb.TabStop = true;
            description_tb.TabIndex = countTab++; description_tb.TabStop = true;
            isActive_cb.TabIndex = countTab++; isActive_cb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);

            this.KeyDown += Department_KeyDown; ;
        }

        private void Department_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_QLNS.Instance.removeDepartment();
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
                var departmentTask = SQLStore_QLNS.Instance.GetDepartmentAsync();

                await Task.WhenAll(departmentTask);
                mDepartment_dt = departmentTask.Result;

                if (mDepartment_dt.Columns.Contains("CreatedAt"))
                    mDepartment_dt.Columns.Remove("CreatedAt");

                dataGV.DataSource = mDepartment_dt;

                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                    {"DepartmentID", "Mã Phòng Ban" },
                    {"DepartmentName", "Tên Phòng Ban" },
                    {"Description", "Diễn Giải" },
                    {"IsActive", "Còn Hoạt Động" }
                });

                Utils.SetGridWidth(dataGV, "DepartmentID", DataGridViewAutoSizeColumnMode.AllCells);
                Utils.SetGridWidth(dataGV, "DepartmentName", DataGridViewAutoSizeColumnMode.AllCells);
                Utils.SetGridWidth(dataGV, "Description", DataGridViewAutoSizeColumnMode.AllCells);
                Utils.SetGridWidth(dataGV, "IsActive", DataGridViewAutoSizeColumnMode.AllCells);

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    dataGV.CurrentCell = dataGV.Rows[0].Cells[0];
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

        private void dataGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Beige;
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
            int departmentID = Convert.ToInt32(cells["DepartmentID"].Value);
            string departmentName = cells["DepartmentName"].Value.ToString();
            string description = cells["Description"].Value.ToString();
            Boolean isActive = Convert.ToBoolean(cells["IsActive"].Value);

            departmentID_tb.Text = departmentID.ToString();
            description_tb.Text = description;
            departmentName_tb.Text = departmentName;
            isActive_cb.Checked = isActive;
            status_lb.Text = "";
        }

        private async void updateData(int departmentID, string departmentName, string description, Boolean isActive)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["DepartmentID"].Value);
                if (id.CompareTo(departmentID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.updateDepartmentAsync(departmentID, departmentName, description, isActive);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["DepartmentName"].Value = departmentName;
                                row.Cells["Description"].Value = description;
                                row.Cells["IsActive"].Value = isActive;
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

        private async void createNew(string departmentName, string description, bool isActive)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int deparmetnID = await SQLManager_QLNS.Instance.insertDepartmentAsync(departmentName, description, isActive);
                    if (deparmetnID > 0)
                    {
                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["DepartmentID"] = deparmetnID;
                        drToAdd["DepartmentName"] = departmentName;
                        drToAdd["Description"] = description;
                        drToAdd["IsActive"] = isActive;

                        departmentID_tb.Text = deparmetnID.ToString();


                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

                        dataGV.ClearSelection(); // bỏ chọn row cũ
                        int rowIndex = dataGV.Rows.Count - 1;
                        dataGV.Rows[rowIndex].Selected = true;
                        UpdateRightUI(dataGV.Rows.Count - 1);

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;
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
            if (departmentName_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            string departmentName = departmentName_tb.Text;
            string description = description_tb.Text;
            Boolean isActive = isActive_cb.Checked;

            if (departmentID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(departmentID_tb.Text), departmentName, description, isActive);
            else
                createNew(departmentName, description, isActive);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string departmentID = departmentID_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string id = row.Cells["DepartmentID"].Value.ToString();
                if (id.CompareTo(departmentID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_QLNS.Instance.deleteDepartmentAsync(Convert.ToInt32(id));

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
            departmentID_tb.Text = "";
            departmentName_tb.Text = "";
            description_tb.Text = "";
            isActive_cb.Checked = true;

            status_lb.Text = "";

            departmentName_tb.Focus();
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
            isActive_cb.Enabled = !isReadOnly;
            departmentName_tb.ReadOnly = isReadOnly;
            description_tb.ReadOnly = isReadOnly;
        }
    }
}
