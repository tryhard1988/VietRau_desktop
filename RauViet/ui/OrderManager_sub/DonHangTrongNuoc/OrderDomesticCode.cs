using DocumentFormat.OpenXml.Vml.Office;
using RauViet.classes;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class OrderDomesticCode : Form
    {
        System.Data.DataTable _employeesInDongGoi_dt, mOrderDomesticCode_dt, mCustomer_dt;
        DataView mLogDV;
        private LoadingOverlay loadingOverlay;
        bool isNewState = false;
        public OrderDomesticCode()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            complete_cb.Visible = UserManager.Instance.hasRole_HoanThanhDonHang();

            autoCreateExportId_btn.Enabled = false;

            updatePrice_btn.Click += updatePrice_btn_click;
            autoCreateExportId_btn.Click += autoCreateExportId_btn_Click;
            newCustomerBtn.Click += newCustomerBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            delete_btn.Click += deleteBtn_Click;
            dataGV.SelectionChanged += this.dataGV_CellClick;
      //      this.dataGV.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGV_RowPrePaint);
            complete_cb.CheckedChanged += completeCB_CheckedChanged;

            orderDomesticIndex_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;            

            ToolTipHelper.SetToolTip(updatePrice_btn, "nếu mã xuất cảng đã tạo rồi\nmà giá có thay đổi\nthì cần update lại giá SP");
        }

        private async void updatePrice_btn_click(object sender, EventArgs e)
        {
            if (dataGV.CurrentRow != null && !dataGV.CurrentRow.IsNewRow)
            {
                DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Thay Đổi Giá", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    int OrderDomesticCodeID = Convert.ToInt32(dataGV.CurrentRow.Cells["OrderDomesticCodeID"].Value);
                    int OrderDomesticIndex = Convert.ToInt32(dataGV.CurrentRow.Cells["OrderDomesticIndex"].Value);
                    try
                    {
                        await Task.Delay(50);
                        loadingOverlay = new LoadingOverlay(this);
                        loadingOverlay.Show();
                        await Task.Delay(200);
                        
                        bool isScussess = await SQLManager_Kho.Instance.updateNewPriceInOrderDomesticDetailAsync(OrderDomesticCodeID);
                        if (isScussess == true)
                        {
                            SQLStore_Kho.Instance.removeOrderDomesticDetail(OrderDomesticCodeID);
                            MessageBox.Show("Thành Công", " Thay Đổi Giá", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _ = SQLManager_Kho.Instance.InsertOrderDomesticCodeLogAsync(OrderDomesticIndex, "Cập Nhật Giá Thành Công","");
                        }
                        else
                        {
                            MessageBox.Show("Thất Bại", " Thay Đổi Giá", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _ = SQLManager_Kho.Instance.InsertOrderDomesticCodeLogAsync(OrderDomesticIndex, "Cập Nhật Giá Thất Bại", "");
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = SQLManager_Kho.Instance.InsertOrderDomesticCodeLogAsync(OrderDomesticIndex, "Cập Nhật Giá Thất Bại", "Exception: " + ex.Message);
                        status_lb.Text = "Thất bại.";
                        status_lb.ForeColor = Color.Red;
                    }
                    finally
                    {
                        await Task.Delay(100);
                        loadingOverlay.Hide();
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

                if (row.Cells["OrderDomesticIndex"].Value != null &&
                    int.TryParse(row.Cells["OrderDomesticIndex"].Value.ToString(), out int value))
                {
                    if (value > maxExportIndex)
                        maxExportIndex = value;
                }
            }

            orderDomesticIndex_tb.Text = (maxExportIndex + 1).ToString();
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

            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();
            await Task.Delay(200);

            try
            {
                // Chạy truy vấn trên thread riêng
                var orderDomesticCodeTask = SQLStore_Kho.Instance.getOrderDomesticCodeAsync();
                var orderDomesticCodeLogTask = SQLStore_Kho.Instance.GetOrderDomesticCodeLogAsync();
                var customerTask = SQLStore_Kho.Instance.getCustomersAsync();
                var employeesInDongGoiTask = SQLStore_Kho.Instance.GetActiveEmployeesIn_DongGoiAsync();

                await Task.WhenAll(orderDomesticCodeTask, employeesInDongGoiTask, customerTask, orderDomesticCodeLogTask);

                mOrderDomesticCode_dt = orderDomesticCodeTask.Result;
                _employeesInDongGoi_dt = employeesInDongGoiTask.Result;
                mLogDV = new DataView(orderDomesticCodeLogTask.Result);


                dataGV.DataSource = mOrderDomesticCode_dt;
                log_GV.DataSource = mLogDV;

                dataGV.Columns["OrderDomesticIndex"].HeaderText = "Mã Đơn Hàng";
                dataGV.Columns["DeliveryDate"].HeaderText = "Ngày Giao";                
                dataGV.Columns["Complete"].HeaderText = "Hoàn Thành";
                dataGV.Columns["InputByName"].HeaderText = "NV Nhập S.Liệu";
                dataGV.Columns["PackingByName"].HeaderText = "NV Đóng Gói";
                dataGV.Columns["CustomerName"].HeaderText = "Khách Hàng";
                dataGV.Columns["Company"].HeaderText = "Khách Hàng";

                dataGV.Columns["OrderDomesticIndex"].Width = 70;
                dataGV.Columns["DeliveryDate"].Width = 70;
                dataGV.Columns["Complete"].Width = 60;
                dataGV.Columns["InputByName"].Width = 140;
                dataGV.Columns["PackingByName"].Width = 140;
                dataGV.Columns["CustomerName"].Width = 100;
                dataGV.Columns["Company"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dataGV.Columns["InputByName_NoSign"].Visible = false;
                dataGV.Columns["OrderDomesticCodeID"].Visible = false;
                dataGV.Columns["InputBy"].Visible = false;
                dataGV.Columns["PackingBy"].Visible = false;
                dataGV.Columns["CustomerID"].Visible = false;
                dataGV.Columns["CustomerCode"].Visible = false;
                dataGV.Columns["Address"].Visible = false;
                dataGV.Columns["CustomerName"].Visible = false;

                inputBy_cbb.DataSource = _employeesInDongGoi_dt;
                inputBy_cbb.DisplayMember = "FullName";  // hiển thị tên
                inputBy_cbb.ValueMember = "EmployeeID";

                packingBy_cbb.BindingContext = new BindingContext();
                packingBy_cbb.DataSource = _employeesInDongGoi_dt;
                packingBy_cbb.DisplayMember = "FullName";  // hiển thị tên
                packingBy_cbb.ValueMember = "EmployeeID";

                var rows = customerTask.Result.AsEnumerable().Where(r => !string.IsNullOrWhiteSpace(r.Field<string>("Company")));
                if (rows.Any())
                    mCustomer_dt = rows.CopyToDataTable();
                else
                    mCustomer_dt = customerTask.Result.Clone(); // bảng rỗng

                customer_cb.DataSource = mCustomer_dt;
                customer_cb.DisplayMember = "Company";  // hiển thị tên
                customer_cb.ValueMember = "CustomerID";

                log_GV.Columns["LogID"].Visible = false;
                log_GV.Columns["OrderDomesticIndex"].Visible = false;
                log_GV.Columns["OldValue"].HeaderText = "Giá Trị Cũ";
                log_GV.Columns["NewValue"].HeaderText = "Giá Trị Mới";
                log_GV.Columns["ActionBy"].HeaderText = "Người Thực Hiện";
                log_GV.Columns["CreatedAt"].HeaderText = "Ngày Thực Hiện";

                log_GV.Columns["OldValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["NewValue"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                log_GV.Columns["ActionBy"].Width = 150;
                log_GV.Columns["CreatedAt"].Width = 120;

                ReadOnly_btn_Click(null, null);

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

            updateRightUI(rowIndex); 
        }

        private void updateRightUI(int index)
        {
            if (isNewState) return;

            var cells = dataGV.Rows[index].Cells;
            int orderDomesticCodeID = Convert.ToInt32(cells["OrderDomesticCodeID"].Value);
            int orderDomesticIndex = Convert.ToInt32(cells["OrderDomesticIndex"].Value);
            DateTime deliveryDate = Convert.ToDateTime(cells["DeliveryDate"].Value.ToString());
            bool complete = Convert.ToBoolean(cells["Complete"].Value);
            int? inputBy = cells["InputBy"].Value == DBNull.Value? (int?) null : Convert.ToInt32(cells["InputBy"].Value);
            int? packingBy = cells["PackingBy"].Value == DBNull.Value? (int?) null : Convert.ToInt32(cells["PackingBy"].Value);
            int? customerID = cells["CustomerID"].Value == DBNull.Value ? (int?)null : Convert.ToInt32(cells["CustomerID"].Value);

            orderDomesticCodeID_tb.Text = orderDomesticCodeID.ToString();
            orderDomesticIndex_tb.Text = orderDomesticIndex.ToString();
            deliveryDate_dtp.Value = deliveryDate;

            if (inputBy != null)
                inputBy_cbb.SelectedValue = inputBy;

            if (packingBy != null)
                packingBy_cbb.SelectedValue = packingBy;

            if (customerID != null)
                customer_cb.SelectedValue = customerID;

            complete_cb.Checked = complete;

            if (readOnly_btn.Visible == true)
            {
                complete_cb.AutoCheck = !complete;
                updatePrice_btn.Enabled = !complete;
                LuuThayDoiBtn.Enabled = !complete;
                inputBy_cbb.Enabled = !complete;
                packingBy_cbb.Enabled = !complete;
                delete_btn.Enabled = !complete;
            }
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

            mLogDV.RowFilter = $"OrderDomesticIndex = {orderDomesticIndex}";
            mLogDV.Sort = "LogID DESC";

            completeCB_CheckedChanged(null, null);
        }

        private async void updateOrderDomesticCode(int orderDomesticCodeID, int orderDomesticIndex, int customerID, DateTime deliveryDate, int inputBy, int packingBy, bool completed)
        {
            foreach (DataRow row in mOrderDomesticCode_dt.Rows)
            {
                int id = Convert.ToInt32(row["OrderDomesticCodeID"]);
                if (id.CompareTo(orderDomesticCodeID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Thay Đổi Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        DataRow[] inputByRow = _employeesInDongGoi_dt.Select($"EmployeeID = {inputBy}");
                        DataRow[] packingByRow = _employeesInDongGoi_dt.Select($"EmployeeID = {packingBy}");
                        DataRow[] customerRow = mCustomer_dt.Select($"CustomerID = {customerID}");

                        string oldValue = $"{row["DeliveryDate"]} - {row["InputByName"]} - {row["PackingByName"]} - {row["CustomerName"]}";
                        string newValue = $"{deliveryDate} - {inputByRow[0]["FullName"]} - {packingByRow[0]["FullName"]} - {customerRow[0]["Company"]}";
                        try
                        {
                            bool isScussess = await SQLManager_Kho.Instance.updateOrderDomesticCodeAsync(orderDomesticCodeID, orderDomesticIndex, customerID, deliveryDate, inputBy, packingBy, completed);
                            

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                _ = SQLManager_Kho.Instance.InsertOrderDomesticCodeLogAsync(orderDomesticIndex, "Update Success: " + oldValue, newValue);

                                row["OrderDomesticIndex"] = orderDomesticIndex;
                                row["DeliveryDate"] = deliveryDate;
                                row["InputBy"] = inputBy;
                                row["PackingBy"] = packingBy;
                                row["CustomerID"] = customerID;
                                row["Complete"] = completed;
                                row["InputByName"] = inputByRow[0]["FullName"].ToString();
                                row["PackingByName"] = packingByRow[0]["FullName"].ToString();
                                row["CustomerName"] = customerRow[0]["Company"].ToString();
                                row["InputByName_NoSign"] = Utils.RemoveVietnameseSigns(inputByRow[0]["FullName"].ToString()).Replace(" ", "");

                                if (completed)
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
                                _ = SQLManager_Kho.Instance.InsertOrderDomesticCodeLogAsync(orderDomesticIndex, "Update Fail: " + oldValue, newValue);
                            }
                        }
                        catch (Exception ex)
                        {                         
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                            _ = SQLManager_Kho.Instance.InsertOrderDomesticCodeLogAsync(orderDomesticIndex, "Update Exception: " + ex.Message + oldValue, newValue);
                        }
                    }
                    break;
                }
            }
        }

        private async void createExportCode(int orderDomesticIndex, int customerID, DateTime deliveryDate, int inputBy, int packingBy)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", " Tạo Mới Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                DataRow[] inputByRow = _employeesInDongGoi_dt.Select($"EmployeeID = {inputBy}");
                DataRow[] packingByRow = _employeesInDongGoi_dt.Select($"EmployeeID = {packingBy}");
                DataRow[] customerRow = mCustomer_dt.Select($"CustomerID = {customerID}");

                string newValue = $"{deliveryDate} - {inputByRow[0]["FullName"]} - {packingByRow[0]["FullName"]} - {customerRow[0]["Company"]}";
                try
                {
                    
                    int newId = await SQLManager_Kho.Instance.insertOrderDomesticCodeAsync(orderDomesticIndex, customerID, deliveryDate, inputBy, packingBy);
                    if (newId > 0)
                    {

                        _ = SQLManager_Kho.Instance.InsertOrderDomesticCodeLogAsync(orderDomesticIndex, "Create Success: ", newValue);

                        DataRow drToAdd = mOrderDomesticCode_dt.NewRow();

                        drToAdd["OrderDomesticCodeID"] = newId;
                        drToAdd["OrderDomesticIndex"] = orderDomesticIndex;
                        drToAdd["DeliveryDate"] = deliveryDate;
                        drToAdd["Complete"] = false;
                        drToAdd["InputBy"] = inputBy;
                        drToAdd["PackingBy"] = packingBy;
                        drToAdd["CustomerID"] = customerID;
                        drToAdd["InputByName"] = inputByRow[0]["FullName"].ToString(); ;
                        drToAdd["PackingByName"] = packingByRow[0]["FullName"].ToString();
                        drToAdd["CustomerName"] = customerRow[0]["Company"].ToString();
                        drToAdd["InputByName_NoSign"] = Utils.RemoveVietnameseSigns(inputByRow[0]["FullName"].ToString()).Replace(" ", "");
                        

                        orderDomesticCodeID_tb.Text = newId.ToString();


                        mOrderDomesticCode_dt.Rows.Add(drToAdd);
                        mOrderDomesticCode_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;
                    }
                    else
                    {
                        _ = SQLManager_Kho.Instance.InsertOrderDomesticCodeLogAsync(orderDomesticIndex, "Create Fail: ", newValue);
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    _ = SQLManager_Kho.Instance.InsertOrderDomesticCodeLogAsync(orderDomesticIndex, "Create Exception: " + ex.Message, newValue);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

            }
        }
        private async void saveBtn_Click(object sender, EventArgs e)
        {
            if(orderDomesticIndex_tb.Text.CompareTo("") == 0 || inputBy_cbb.SelectedValue == null || packingBy_cbb.SelectedValue == null || customer_cb.SelectedValue == null)
            {
                MessageBox.Show("Dữ Liệu Không hợp Lệ, Kiểm Tra Lại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           if (complete_cb.AutoCheck == false) return;

            DateTime deliveryDate = deliveryDate_dtp.Value.Date;
            int orderDomesticIndex = Convert.ToInt32(orderDomesticIndex_tb.Text);
            int inputBy = Convert.ToInt32(inputBy_cbb.SelectedValue);
            int packingBy = Convert.ToInt32(packingBy_cbb.SelectedValue);
            int customerID = Convert.ToInt32(customer_cb.SelectedValue);

            if (orderDomesticCodeID_tb.Text.Length != 0)
                updateOrderDomesticCode(Convert.ToInt32(orderDomesticCodeID_tb.Text), orderDomesticIndex, customerID, deliveryDate, inputBy, packingBy, complete_cb.Checked);
            else
                createExportCode(orderDomesticIndex, customerID, deliveryDate, inputBy, packingBy);

        }
        private async void deleteBtn_Click(object sender, EventArgs e)
        {
            string id = orderDomesticCodeID_tb.Text;

            foreach (DataRow row in mOrderDomesticCode_dt.Rows)
            {
                string orderDomesticCodeID = row["OrderDomesticCodeID"].ToString();
                if (orderDomesticCodeID.CompareTo(id) == 0)
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
                            bool isScussess = await SQLManager_Kho.Instance.DeleteOrderDomesticCodeAsync(Convert.ToInt32(orderDomesticCodeID));

                            if (isScussess == true)
                            {
                                // _ = SQLManager.Instance.InsertExportCodeLogAsync(exportCode, "Xóa Thành Công", null, null, null, "", "", false);
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mOrderDomesticCode_dt.Rows.Remove(row);
                                mOrderDomesticCode_dt.AcceptChanges();
                            }
                            else
                            {
                                status_lb.Text = "Thất bại.";
                                status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ERROR Exception " + ex.Message);
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
            orderDomesticCodeID_tb.Text = "";
            orderDomesticIndex_tb.Text = "";
            deliveryDate_dtp.Value = DateTime.Now;
            complete_cb.Visible = false;
            autoCreateExportId_btn.Enabled = true;
            orderDomesticIndex_tb.Enabled = true;
            
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
            autoCreateExportId_btn.Visible = true;
            LuuThayDoiBtn.Enabled = true;
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
            orderDomesticIndex_tb.Enabled = false;
            rightUIReadOnly(true);
            updatePrice_btn.Visible = false;
            autoCreateExportId_btn.Visible = false;

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
            rightUIReadOnly(false);
            updatePrice_btn.Visible = true;
            autoCreateExportId_btn.Visible = false;
        }

        private void completeCB_CheckedChanged(object sender, EventArgs e)
        {
            if(readOnly_btn.Visible == true)
                deliveryDate_dtp.Enabled = !complete_cb.Checked;
        }

        private void rightUIReadOnly(bool isReadOnly)
        {
            orderDomesticIndex_tb.ReadOnly = isReadOnly;       
            inputBy_cbb.Enabled = !isReadOnly;
            packingBy_cbb.Enabled = !isReadOnly;
            deliveryDate_dtp.Enabled = !isReadOnly;
            complete_cb.Enabled = !isReadOnly;
            customer_cb.Enabled = !isReadOnly;
        }
    }
}
