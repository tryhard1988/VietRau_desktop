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
    public partial class OvertimeType : Form
    {
        DataTable mOvertimeType_dt;
        bool isNewState = false;
        public OvertimeType()
        {
            InitializeComponent();

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            overtimeName_tb.TabIndex = countTab++; overtimeName_tb.TabStop = true;
            salaryFactor_tb.TabIndex = countTab++; salaryFactor_tb.TabStop = true;
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            loading_lb.Visible = true;            

            try
            {
                // Chạy truy vấn trên thread riêng
                var overtimeTypeTask = SQLStore.Instance.GetOvertimeTypeAsync();

                await Task.WhenAll(overtimeTypeTask);
                mOvertimeType_dt = overtimeTypeTask.Result;

                int count = 0;
                mOvertimeType_dt.Columns["OvertimeName"].SetOrdinal(count++);
                mOvertimeType_dt.Columns["SalaryFactor"].SetOrdinal(count++);
                mOvertimeType_dt.Columns["IsActive"].SetOrdinal(count++);

                dataGV.DataSource = mOvertimeType_dt;

                dataGV.Columns["OvertimeName"].HeaderText = "Tên Loại Tăng Ca";
                dataGV.Columns["SalaryFactor"].HeaderText = "Hệ Số Nhân";
                dataGV.Columns["IsActive"].HeaderText = "Còn Hoạt Động";

                dataGV.Columns["OvertimeTypeID"].Visible = false;

                dataGV.Columns["OvertimeName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["SalaryFactor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["IsActive"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

               // dataGV.Columns["UserID"].Visible = false;

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
            int positionID = Convert.ToInt32(cells["OvertimeTypeID"].Value);
            string positionName = cells["OvertimeName"].Value.ToString();
            double salaryFactor = Convert.ToDouble(cells["SalaryFactor"].Value);
            Boolean isActive = Convert.ToBoolean(cells["IsActive"].Value);

            overtimeTypeID_tb.Text = positionID.ToString();
            overtimeName_tb.Text = positionName;
            salaryFactor_tb.Text = salaryFactor.ToString(); 
            isActive_cb.Checked = isActive;

            status_lb.Text = "";
        }

        private async void updateData(int overtimeTypeID, string overtimeName, double salaryFactor, bool isActive)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["OvertimeTypeID"].Value);
                if (id.CompareTo(overtimeTypeID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateOvertimeTypeAsync(overtimeTypeID, overtimeName, salaryFactor, isActive);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["OvertimeName"].Value = overtimeName;
                                row.Cells["SalaryFactor"].Value = salaryFactor;
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

        private async void createNew(string overtimeName, double salaryFactor, bool isActive)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newId = await SQLManager.Instance.insertOvertimeTypeAsync(overtimeName, salaryFactor, isActive);
                    if (newId > 0)
                    {

                        DataTable dataTable = (DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["OvertimeTypeID"] = newId;
                        drToAdd["OvertimeName"] = overtimeName;
                        drToAdd["SalaryFactor"] = salaryFactor;
                        drToAdd["IsActive"] = isActive;

                        overtimeTypeID_tb.Text = newId.ToString();


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
            if (salaryFactor_tb.Text.CompareTo("") == 0)
            {
                MessageBox.Show(
                                "Thiếu Dữ Liệu, Kiểm Tra Lại!",
                                "Thông Báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                return;
            }

            string overtimeName = overtimeName_tb.Text;
            double salaryFactor = Convert.ToDouble(salaryFactor_tb.Text);
            Boolean isActive = isActive_cb.Checked;

            if (overtimeTypeID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(overtimeTypeID_tb.Text), overtimeName, salaryFactor, isActive);
            else
                createNew(overtimeName, salaryFactor, isActive);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string id = overtimeTypeID_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string overtimeTypeID = row.Cells["OvertimeTypeID"].Value.ToString();
                if (overtimeTypeID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA ĐÓ NHA \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deleteOvertimeTypeAsync(Convert.ToInt32(overtimeTypeID));

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
            overtimeTypeID_tb.Text = "";
            overtimeName_tb.Text = "";
            salaryFactor_tb.Text = "";
            isActive_cb.Checked = true;

            status_lb.Text = "";

            overtimeName_tb.Focus();
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
            overtimeName_tb.ReadOnly = isReadOnly;
            salaryFactor_tb.ReadOnly = isReadOnly;
        }
    }
}
