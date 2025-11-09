using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class ExportCodes : Form
    {
        System.Data.DataTable _employeesInDongGoi_dt;
        bool isNewState = false;
        public ExportCodes()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";
            loading_lb.Text = "Đang tải dữ liệu, vui lòng chờ...";
            loading_lb.Visible = false;

            complete_cb.Visible = UserManager.Instance.hasRole_HoanThanhDonHang();

            autoCreateExportId_btn.Enabled = false;

            updatePrice_btn.Click += updatePrice_btn_click;
            autoCreateExportId_btn.Click += autoCreateExportId_btn_Click;
            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            exRate_btn.Click += exRate_btn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
      //      this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            complete_cb.CheckedChanged += completeCB_CheckedChanged;

            exportCode_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            exRate_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            shippingCost_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;
            ReadOnly_btn_Click(null, null);

            ToolTipHelper.SetToolTip(updatePrice_btn, "nếu mã xuất cảng đã tạo rồi\nmà giá có thay đổi\nthì cần update lại giá SP");
        }

        private async void updatePrice_btn_click(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow != null && !dataGV.CurrentRow.IsNewRow)
            {
                DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Thay Đổi Giá", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        int exportCodeID = Convert.ToInt32(dataGV.CurrentRow.Cells["ExportCodeID"].Value);
                        bool isScussess = await SQLManager.Instance.updateNewPriceInOrderListWithExportCodeAsync(exportCodeID);
                        if(isScussess == true)
                        {
                            _= SQLStore.Instance.getOrdersAsync(true);
                            MessageBox.Show("Thành Công", " Thay Đổi Giá", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Thất Bại", " Thay Đổi Giá", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
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
            }            
        }

        private void autoCreateExportId_btn_Click(object sender, EventArgs e)
        {
            int maxExportIndex = 0;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue; // bỏ dòng mới chưa nhập dữ liệu

                if (row.Cells["exportCodeIndex"].Value != null &&
                    int.TryParse(row.Cells["exportCodeIndex"].Value.ToString(), out int value))
                {
                    if (value > maxExportIndex)
                        maxExportIndex = value;
                }
            }

            exportCode_tb.Text = (maxExportIndex + 1).ToString();
        }

        private void Tb_KeyPress_OnlyNumber(object sender, KeyPressEventArgs e)
        {
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;

            // Chỉ cho nhập số, dấu chấm hoặc ký tự điều khiển
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Nếu đã có 1 dấu chấm, không cho nhập thêm
            if (e.KeyChar == '.' && tb.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        public async void ShowData()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            
            loading_lb.Visible = true;            

            try
            {
                // Chạy truy vấn trên thread riêng
                var exportCodeTask = SQLStore.Instance.getExportCodesAsync();
                var employeesInDongGoiTask = SQLStore.Instance.GetActiveEmployeesIn_DongGoiAsync();

                await Task.WhenAll(exportCodeTask, employeesInDongGoiTask);

                System.Data.DataTable exportCode_dt = exportCodeTask.Result;
                _employeesInDongGoi_dt = employeesInDongGoiTask.Result;

                dataGV.DataSource = exportCode_dt;

                dataGV.Columns["ExportCode"].HeaderText = "Mã Xuất Cảng";
                dataGV.Columns["ExportDate"].HeaderText = "Ngày Xuất Cảng";                
                dataGV.Columns["Complete"].HeaderText = "Hoàn Thành";
                dataGV.Columns["ModifiedAt"].HeaderText = "Ngày Thay đổi";
                dataGV.Columns["InputByName"].HeaderText = "NV Nhập S.Liệu";
                dataGV.Columns["PackingByName"].HeaderText = "NV Đóng Gói";

                dataGV.Columns["ExportCode"].Width = 120;
                dataGV.Columns["ExportDate"].Width = 100;
                dataGV.Columns["Complete"].Width = 90;
                dataGV.Columns["InputByName"].Width = 150;
                dataGV.Columns["PackingByName"].Width = 150;

                dataGV.Columns["ModifiedAt"].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

                dataGV.Columns["InputByName_NoSign"].Visible = false;
                dataGV.Columns["ExportCodeID"].Visible = false;
                dataGV.Columns["ExportCodeIndex"].Visible = false;
                dataGV.Columns["InputBy"].Visible = false;
                dataGV.Columns["PackingBy"].Visible = false;

                inputBy_cbb.DataSource = _employeesInDongGoi_dt;
                inputBy_cbb.DisplayMember = "FullName";  // hiển thị tên
                inputBy_cbb.ValueMember = "EmployeeID";

                packingBy_cbb.BindingContext = new BindingContext();
                packingBy_cbb.DataSource = _employeesInDongGoi_dt;
                packingBy_cbb.DisplayMember = "FullName";  // hiển thị tên
                packingBy_cbb.ValueMember = "EmployeeID";


                if (dataGV.SelectedRows.Count > 0)
                {
                    updateRightUI(0);
                }


                
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

            updateRightUI(rowIndex);
        }

        private void updateRightUI(int index)
        {
            if (isNewState) return;

            var cells = dataGV.Rows[index].Cells;
            int exportCodeID = Convert.ToInt32(cells["ExportCodeID"].Value);
            int exportCodeIndex = Convert.ToInt32(cells["ExportCodeIndex"].Value);
            decimal exRate = Convert.ToDecimal(cells["ExchangeRate"].Value);
            decimal shippingCost = Convert.ToDecimal(cells["ShippingCost"].Value);
            DateTime exportDate = Convert.ToDateTime(cells["ExportDate"].Value.ToString());
            bool complete = Convert.ToBoolean(cells["Complete"].Value);
            int? inputBy = cells["InputBy"].Value == DBNull.Value? (int?) null : Convert.ToInt32(cells["InputBy"].Value);
            int? packingBy = cells["PackingBy"].Value == DBNull.Value? (int?) null : Convert.ToInt32(cells["PackingBy"].Value);

            exportCodeId_tb.Text = exportCodeID.ToString();
            exportCode_tb.Text = exportCodeIndex.ToString();
            exRate_tb.Text = exRate.ToString();
            shippingCost_tb.Text = shippingCost.ToString();
            exportdate_dtp.Value = exportDate;

            if (inputBy != null)
                inputBy_cbb.SelectedValue = inputBy;

            if (packingBy != null)
                packingBy_cbb.SelectedValue = packingBy;

            complete_cb.Checked = complete;
            complete_cb.AutoCheck = !complete;
            updatePrice_btn.Enabled = !complete;
            LuuThayDoiBtn.Enabled = !complete;
            exRate_btn.Enabled = !complete;
            exRate_tb.Enabled = !complete;
            shippingCost_tb.Enabled = !complete;
            inputBy_cbb.Enabled = !complete;
            packingBy_cbb.Enabled = !complete;
            delete_btn.Enabled = !complete;
            if (UserManager.Instance.hasRole_HoanThanhDonHang())
                complete_cb.Visible = true;
            else
                complete_cb.Visible = false;

            autoCreateExportId_btn.Enabled = false;

            if (complete)
            {
                complete_cb.ForeColor = Color.Red; // chỉ tác dụng khi Enabled = true trên nhiều hệ thống
                complete_cb.BackColor = Color.Yellow; // background
            }
            else
            {
                complete_cb.ForeColor = Color.Black; // chỉ tác dụng khi Enabled = true trên nhiều hệ thống
                complete_cb.BackColor = Color.DarkGray; // background
            }

            completeCB_CheckedChanged(null, null);
        }

        private async void updateExportCode(int exportCodeID, string exportCode, int exportCodeIndex, DateTime exportDate, decimal? exRate, decimal? shippingCost, int inputBy, int packingBy, bool complete)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                int id = Convert.ToInt32(row.Cells["ExportCodeID"].Value);
                if (id.CompareTo(exportCodeID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Thay Đổi Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.updateExportCodeAsync(exportCodeID, exportCode, exportCodeIndex, exportDate, exRate, shippingCost, inputBy, packingBy, complete);

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                DataRow[] inputByRow = _employeesInDongGoi_dt.Select($"EmployeeID = {inputBy}");
                                DataRow[] packingByRow = _employeesInDongGoi_dt.Select($"EmployeeID = {packingBy}");


                                row.Cells["ExportCodeIndex"].Value = exportCodeIndex;
                                row.Cells["ExportCode"].Value = exportCode;
                                row.Cells["ExportDate"].Value = exportDate;
                                row.Cells["ModifiedAt"].Value = DateTime.Now;
                                row.Cells["Complete"].Value = complete;
                                row.Cells["ExchangeRate"].Value = exRate ?? (object)DBNull.Value;
                                row.Cells["ShippingCost"].Value = shippingCost ?? (object)DBNull.Value;
                                row.Cells["InputBy"].Value = inputBy;
                                row.Cells["PackingBy"].Value = packingBy;
                                row.Cells["InputByName"].Value = inputByRow[0]["FullName"].ToString(); ;
                                row.Cells["PackingByName"].Value = packingByRow[0]["FullName"].ToString();
                                row.Cells["InputByName_NoSign"].Value = Utils.RemoveVietnameseSigns(inputByRow[0]["FullName"].ToString()).Replace(" ", "");

                                if (complete)
                                {
                                    updatePrice_btn.Enabled = false;
                                    LuuThayDoiBtn.Enabled = false;
                                    delete_btn.Enabled = false;
                                }
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

        private async void createExportCode(string exportCode, int exportCodeIndex, DateTime exportDate, decimal? exRate, decimal? shippingCost, int inputBy, int packingBy)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Tạo Mới Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newId = await SQLManager.Instance.insertExportCodeAsync(exportCode, exportCodeIndex, exportDate, exRate, shippingCost, inputBy, packingBy);
                    if (newId > 0)
                    {
                        DataRow[] inputByRow = _employeesInDongGoi_dt.Select($"EmployeeID = {inputBy}");
                        DataRow[] packingByRow = _employeesInDongGoi_dt.Select($"EmployeeID = {packingBy}");

                        System.Data.DataTable dataTable = (System.Data.DataTable)dataGV.DataSource;
                        DataRow drToAdd = dataTable.NewRow();

                        drToAdd["ExportCodeID"] = newId;
                        drToAdd["ExportCode"] = exportCode;
                        drToAdd["ExportCodeIndex"] = exportCodeIndex;
                        drToAdd["ExportDate"] = exportDate;
                        drToAdd["ModifiedAt"] = DateTime.Now;
                        drToAdd["Complete"] = false;
                        drToAdd["ExchangeRate"] = exRate ?? (object)DBNull.Value;
                        drToAdd["ShippingCost"] = shippingCost ?? (object)DBNull.Value;
                        drToAdd["InputBy"] = inputBy;
                        drToAdd["PackingBy"] = packingBy;
                        drToAdd["InputByName"] = inputByRow[0]["FullName"].ToString(); ;
                        drToAdd["PackingByName"] = packingByRow[0]["FullName"].ToString();
                        drToAdd["InputByName_NoSign"] = Utils.RemoveVietnameseSigns(inputByRow[0]["FullName"].ToString()).Replace(" ", "");
                        

                        exportCodeId_tb.Text = newId.ToString();


                        dataTable.Rows.Add(drToAdd);
                        dataTable.AcceptChanges();

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
        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if(exportCode_tb.Text.CompareTo("") == 0 || inputBy_cbb.SelectedValue == null || packingBy_cbb.SelectedValue == null)
            {
                MessageBox.Show("Dữ Liệu Không hợp Lệ, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           if (complete_cb.AutoCheck == false) return;

            DateTime exportDate = exportdate_dtp.Value.Date;
            int exportCodeIndex = Convert.ToInt32(exportCode_tb.Text);
            string exportCode = "MXC" + exportCodeIndex + "_"+ exportDate.ToString("dd") + exportDate.ToString("MM") + exportDate.Year;
            int inputBy = Convert.ToInt32(inputBy_cbb.SelectedValue);
            int packingBy = Convert.ToInt32(packingBy_cbb.SelectedValue);

            bool isAdded = await SQLStore.Instance.ExportHistoryIsAddedExportCode(exportCode, exportDate.Year);
            if (complete_cb.Checked && !isAdded)
            {
                MessageBox.Show("Chưa xuất invoice, Vui lòng xuất Invoice trước khi Khóa!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal exRate = 0;
            decimal shippingCost = 0;

            if (!string.IsNullOrWhiteSpace(exRate_tb.Text))
            {
                exRate = Convert.ToDecimal(exRate_tb.Text.Trim());
            }

            if (!string.IsNullOrWhiteSpace(shippingCost_tb.Text))
            {
                shippingCost = Convert.ToDecimal(shippingCost_tb.Text.Trim());
            }

            if (exportCodeId_tb.Text.Length != 0)
                updateExportCode(Convert.ToInt32(exportCodeId_tb.Text), exportCode, exportCodeIndex, exportDate, exRate, shippingCost, inputBy, packingBy, complete_cb.Checked);
            else
                createExportCode(exportCode, exportCodeIndex, exportDate, exRate, shippingCost, inputBy, packingBy);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            string id = exportCodeId_tb.Text;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                string exportCodeID = row.Cells["ExportCodeID"].Value.ToString();
                if (exportCodeID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show(
                        "XÓA THÔNG ĐÓ NHA\nChắc chắn chưa?",
                        "Thông Báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning  // Thêm icon cảnh báo
                    ); 
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager.Instance.DeleteExportCodeWithOrdersAsync(Convert.ToInt32(exportCodeID));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                System.Data.DataTable dataTable = (System.Data.DataTable)dataGV.DataSource;
                                dataGV.Rows.Remove(row);
                                dataTable.AcceptChanges();

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
            exportCodeId_tb.Text = "";
            exportCode_tb.Text = "";
            exRate_tb.Text = "";
            exportdate_dtp.Value = DateTime.Now;
            complete_cb.Visible = false;
            autoCreateExportId_btn.Enabled = true;

            decimal maxShippingCost = 0;
            int maxId = -1;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.IsNewRow) continue;

                object exportCodeID = row.Cells["ExportCodeID"].Value;
                object shippingCostStr = row.Cells["ShippingCost"].Value;
                if (exportCodeID != null && int.TryParse(exportCodeID.ToString(), out int id) &&
                    shippingCostStr != null && decimal.TryParse(shippingCostStr.ToString(), out decimal shippingCost))
                {
                    if (id > maxId)
                    {
                        maxId = id;
                        maxShippingCost = shippingCost;
                    }
                }
            }

            shippingCost_tb.Text = maxShippingCost.ToString();

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            delete_btn.Visible = false;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            rightUIReadOnly(false);
            updatePrice_btn.Visible = false;
            exRate_btn.Visible = true;
            autoCreateExportId_btn.Visible = true;
            exRate_btn.Enabled = true;
            LuuThayDoiBtn.Enabled = true;
            exRate_tb.Enabled = true;
            shippingCost_tb.Enabled = true;
            complete_cb.Checked = false;
            complete_cb.AutoCheck = true;
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
            rightUIReadOnly(true);
            updatePrice_btn.Visible = false;
            exRate_btn.Visible = false;
            autoCreateExportId_btn.Visible = false;
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
            rightUIReadOnly(false);
            updatePrice_btn.Visible = true;
            exRate_btn.Visible = true;
            autoCreateExportId_btn.Visible = false;
        }

        private void completeCB_CheckedChanged(object sender, EventArgs e)
        {
            exportCode_tb.Enabled = !complete_cb.Checked;
            exportdate_dtp.Enabled = !complete_cb.Checked;
        }

        private async void exRate_btn_Click(object sender, EventArgs e)
        {
            try
            {
                decimal rate = await CurrencyHelper.GetUSDtoCHF_FreeAsync();
                exRate_tb.Text = rate.ToString("F4");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy tỷ giá: " + ex.Message);
            }
        }

        private void rightUIReadOnly(bool isReadOnly)
        {
            exportCode_tb.ReadOnly = isReadOnly;
            exportdate_dtp.Enabled = !isReadOnly;
            exRate_tb.ReadOnly = isReadOnly;            
            shippingCost_tb.ReadOnly = isReadOnly;
            inputBy_cbb.Enabled = !isReadOnly;
            packingBy_cbb.Enabled = !isReadOnly;
        }
    }
}
