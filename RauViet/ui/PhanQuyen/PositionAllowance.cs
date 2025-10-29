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
    public partial class PositionAllowance : Form
    {
        bool isNewState = false;
        private DataTable  mPositionAllowance_dt, mAllowanceType_dt;
        public PositionAllowance()
        {
            InitializeComponent();

            Utils.SetTabStopRecursive(this, false);

            int countTab = 0;
            allowanceType_cbb.TabIndex = countTab++; allowanceType_cbb.TabStop = true;
            amount_tb.TabIndex = countTab++; amount_tb.TabStop = true;
            note_tb.TabIndex = countTab++; note_tb.TabStop = true;
            LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            allowanceGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            allowanceGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;

            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
            allowanceGV.SelectionChanged += this.allowanceGV_CellClick;

            amount_tb.KeyPress += Tb_KeyPress_OnlyNumber;

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
                var postionTask = SQLStore.Instance.GetActivePositionAsync();
                var positionAllowanceAsync = SQLManager.Instance.GetPositionAllowanceAsybc();
                var allowanceTypeAsync = SQLManager.Instance.GetAllowanceTypeAsync("POS");

                await Task.WhenAll(postionTask, positionAllowanceAsync, allowanceTypeAsync);
                DataTable position_dt = postionTask.Result;
                mPositionAllowance_dt = positionAllowanceAsync.Result;
                mAllowanceType_dt = allowanceTypeAsync.Result;

                position_dt.Columns.Remove("IsActive");
                position_dt.Columns.Remove("CreatedAt");
                position_dt.Columns.Remove("PositionCode");

                mPositionAllowance_dt.Columns.Add(new DataColumn("AllowanceName", typeof(string)));
                foreach (DataRow dr in mPositionAllowance_dt.Rows)
                {
                    int allowanceTypeID = Convert.ToInt32(dr["AllowanceTypeID"]);
                    DataRow[] applyScopeRows = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}");

                    if (applyScopeRows.Length > 0)
                        dr["AllowanceName"] = applyScopeRows[0]["AllowanceName"].ToString();
                }


                DataView dv = new DataView(mAllowanceType_dt);
                dv.RowFilter = "IsActive = 1";
                allowanceType_cbb.DataSource = dv;
                allowanceType_cbb.DisplayMember = "AllowanceName";
                allowanceType_cbb.ValueMember = "AllowanceTypeID";

                allowanceGV.DataSource = mPositionAllowance_dt;

                dataGV.DataSource = position_dt;

                dataGV.Columns["PositionID"].Visible = false;
                allowanceGV.Columns["PositionAllowanceID"].Visible = false;
                allowanceGV.Columns["PositionID"].Visible = false;
                allowanceGV.Columns["AllowanceTypeID"].Visible = false;

                int count = 0;
                mPositionAllowance_dt.Columns["AllowanceName"].SetOrdinal(count++);
                mPositionAllowance_dt.Columns["Amount"].SetOrdinal(count++);
                mPositionAllowance_dt.Columns["Note"].SetOrdinal(count++);


                allowanceGV.Columns["AllowanceName"].HeaderText = "Loại Phụ Cấp";
                allowanceGV.Columns["Amount"].HeaderText = "Số Tiền";
                allowanceGV.Columns["Note"].HeaderText = "Ghi Chú";

                dataGV.Columns["PositionName"].HeaderText = "Tên Chức Vụ";
                dataGV.Columns["Description"].HeaderText = "Diễn Giải";
                //dataGV.Columns["IsActive"].HeaderText = "Đang Hoạt Động";

                //dataGV.Columns["AllowanceTypeID"].Visible = false;
                //dataGV.Columns["ApplyScopeID"].Visible = false;

                dataGV.Columns["PositionName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dataGV.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                allowanceGV.Columns["AllowanceName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                allowanceGV.Columns["Amount"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                allowanceGV.Columns["Note"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                if (dataGV.Rows.Count > 0)
                {
                    dataGV.ClearSelection();
                    dataGV.Rows[0].Selected = true;
                    UpdateAllowancetUI(0);
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
            int positionID = Convert.ToInt32(cells["PositionID"].Value);

            DataView dv = new DataView(mPositionAllowance_dt);
            dv.RowFilter = $"PositionID = '{positionID}'";

            allowanceGV.DataSource = dv;
        }
        private void UpdateRightUI(int index)
        {
            if (isNewState) return;
            var cells = allowanceGV.Rows[index].Cells;
            int positionAllowanceID = Convert.ToInt32(cells["PositionAllowanceID"].Value);
            int allowanceTypeID = Convert.ToInt32(cells["AllowanceTypeID"].Value);
            int amount = Convert.ToInt32(cells["Amount"].Value);
            string note = cells["Note"].Value.ToString();

            this.positionAllowanceID_tb.Text = positionAllowanceID.ToString();
            amount_tb.Text = amount.ToString();
            note_tb.Text = note;
            allowanceType_cbb.SelectedValue = allowanceTypeID;
            status_lb.Text = "";
        }
        
        private async void updateData(int positionAllowanceID, int positionID, int amount, int allowanceTypeID, string note)
        {
            foreach (DataGridViewRow row in allowanceGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["PositionAllowanceID"].Value);
                if (id.CompareTo(positionAllowanceID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updatePositionAllowanceAsync(positionAllowanceID, positionID, amount, allowanceTypeID, note);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                row.Cells["PositionID"].Value = positionID;
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

        private async void createNew(int positionID, int amount, int allowanceTypeID, string note)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int positionAllowanceID = await SQLManager.Instance.insertPositionAllowanceAsync(positionID, amount, allowanceTypeID, note);
                    if (allowanceTypeID > 0)
                    {
                        DataRow drToAdd = mPositionAllowance_dt.NewRow();

                        drToAdd["PositionAllowanceID"] = positionAllowanceID;
                        drToAdd["PositionID"] = positionID;
                        drToAdd["AllowanceTypeID"] = allowanceTypeID;
                        drToAdd["Amount"] = amount;
                        drToAdd["Note"] = note;
                        drToAdd["AllowanceName"] = mAllowanceType_dt.Select($"AllowanceTypeID = {allowanceTypeID}")[0]["AllowanceName"].ToString();

                        mPositionAllowance_dt.Rows.Add(drToAdd);
                        mPositionAllowance_dt.AcceptChanges();

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

            int positionID = Convert.ToInt32(dataGV.CurrentRow.Cells["PositionID"].Value);
            int amount = Convert.ToInt32(amount_tb.Text);
            int allowanceTypeID = Convert.ToInt32(allowanceType_cbb.SelectedValue);
            string note= note_tb.Text;

            if (positionAllowanceID_tb.Text.Length != 0)
                updateData(Convert.ToInt32(positionAllowanceID_tb.Text), positionID, amount, allowanceTypeID, note);
            else
                createNew(positionID, amount, allowanceTypeID, note);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            if (dataGV.SelectedRows.Count == 0) return;

            string positionAllowanceID = positionAllowanceID_tb.Text;

            foreach (DataGridViewRow row in allowanceGV.Rows)
            {
                string id = row.Cells["PositionAllowanceID"].Value.ToString();
                if (id.CompareTo(positionAllowanceID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("XÓA THÔNG TIN \n Chắc chắn chưa ?", " Xóa Thông Tin", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.deletePositionAllowanceAsync(Convert.ToInt32(positionAllowanceID));

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
            positionAllowanceID_tb.Text = "";
            amount_tb.Text = "";

            status_lb.Text = "";
            delete_btn.Enabled = false;

            allowanceType_cbb.Focus();
            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = false;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
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
        }
    }
}
