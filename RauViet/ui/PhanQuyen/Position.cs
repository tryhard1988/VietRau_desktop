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
    public partial class Position : Form
    {
        bool isNewState = false;
        public Position()
        {
            InitializeComponent();

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            positionCode_tb.TabIndex = countTab++; positionCode_tb.TabStop = true;
            positionName_tb.TabIndex = countTab++; positionName_tb.TabStop = true;
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
            this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);

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
                var postionTask = SQLStore.Instance.GetPositionAsync();

                await Task.WhenAll(postionTask);
                DataTable postion_dt = postionTask.Result;

                int count = 0;
                postion_dt.Columns["PositionCode"].SetOrdinal(count++);
                postion_dt.Columns["PositionName"].SetOrdinal(count++);
                postion_dt.Columns["Description"].SetOrdinal(count++);
                postion_dt.Columns["IsActive"].SetOrdinal(count++);

                dataGV.DataSource = postion_dt;

                dataGV.Columns["PositionName"].HeaderText = "Tên Vị Trí";
                dataGV.Columns["Description"].HeaderText = "Diễn Giải";
                dataGV.Columns["IsActive"].HeaderText = "Còn Hoạt Động";
                dataGV.Columns["PositionCode"].HeaderText = "Mã Chức Vụ";

                dataGV.Columns["PositionID"].Visible = false;
                dataGV.Columns.Remove("CreatedAt");

                dataGV.Columns["PositionCode"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["PositionName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["IsActive"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

               // dataGV.Columns["UserID"].Visible = false;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    dataGV.CurrentCell = dataGV.Rows[0].Cells[0];
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
            if (isNewState) return;
            var cells = dataGV.Rows[index].Cells;
            int positionID = Convert.ToInt32(cells["PositionID"].Value);
            string positionName = cells["PositionName"].Value.ToString();
            string description = cells["Description"].Value.ToString();
            string positionCode = cells["PositionCode"].Value.ToString();
            Boolean isActive = Convert.ToBoolean(cells["IsActive"].Value);

            positionID_tb.Text = positionID.ToString();
            description_tb.Text = description;
            positionName_tb.Text = positionName;
            positionCode_tb.Text = positionCode;    
            isActive_cb.Checked = isActive;

            status_lb.Text = "";
        }

        private async void updateData(int positionId, string positionCode, string positionName, string description, bool isActive)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["PositionID"].Value);
                if (id.CompareTo(positionId) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updatePositionAsync(positionId, positionCode, positionName, description, isActive);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["PositionName"].Value = positionName;
                                row.Cells["Description"].Value = description;
                                row.Cells["PositionCode"].Value = positionCode;
                                row.Cells["IsActive"].Value = isActive;
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

        private async void createNew(string positionCode, string positionName, string description, bool isActive)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newId = await SQLManager.Instance.insertPositionAsync(positionCode, positionName, description, isActive);
                    if (newId > 0)
                    {

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["PositionID"] = newId;
                        drToAdd["PositionName"] = positionName;
                        drToAdd["PositionCode"] = positionCode;
                        drToAdd["Description"] = description;
                        drToAdd["IsActive"] = isActive;

                        positionID_tb.Text = newId.ToString();


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
                catch (Exception ex)
                {
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
                
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (positionName_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            string positionCode = positionCode_tb.Text.Replace(" ", "").ToLower();
            string positionName = positionName_tb.Text;
            string description = description_tb.Text;

            Boolean isActive = isActive_cb.Checked;

            if (positionID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(positionID_tb.Text), positionCode, positionName, description, isActive);
            else
                createNew(positionCode, positionName, description, isActive);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string id = positionID_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string positionID = row.Cells["PositionID"].Value.ToString();
                if (positionID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deletePositionAsync(Convert.ToInt32(positionID));

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
            positionID_tb.Text = "";
            positionName_tb.Text = "";
            description_tb.Text = "";
            isActive_cb.Checked = true;

            status_lb.Text = "";

            positionCode_tb.Focus();
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
            positionCode_tb.ReadOnly = isReadOnly;
            positionName_tb.ReadOnly = isReadOnly;
            description_tb.ReadOnly = isReadOnly;
        }
    }
}
