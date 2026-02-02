
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class KhoVatTu_PlantingManagement : Form
    {
        System.Data.DataTable mPlantingManagement_dt, mDepartment_dt, mProductSKU_dt, mEmployee_dt;
        private Timer productSKUDebounceTimer = new Timer { Interval = 300 };
        private Timer employeeDebounceTimer = new Timer { Interval = 300 };
        private Timer departmentDebounceTimer = new Timer { Interval = 300 };
        private Timer _monthYearDebounceTimer = new Timer { Interval = 500 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_PlantingManagement()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            monthYear_dtp.Format = DateTimePickerFormat.Custom;
            monthYear_dtp.CustomFormat = "yyyy";
            monthYear_dtp.ShowUpDown = true;
            monthYear_dtp.Value = DateTime.Now;

            ngayUom_dtp.Format = DateTimePickerFormat.Custom;
            ngayUom_dtp.CustomFormat = "dd/MM/yyyy";
            ngaytrong_dtp.Format = DateTimePickerFormat.Custom;
            ngaytrong_dtp.CustomFormat = "dd/MM/yyyy";
            ngaythu_dtp.Format = DateTimePickerFormat.Custom;
            ngaythu_dtp.CustomFormat = "dd/MM/yyyy";

            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            status_lb.Text = "";

            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click; 
            this.KeyDown += Kho_Materials_KeyDown;
            productSKUDebounceTimer.Tick += productSKUDebounceTimer_Tick;
            employeeDebounceTimer.Tick += employeeDebounceTimer_Tick;
            departmentDebounceTimer.Tick += departmentDebounceTimer_Tick;
            _monthYearDebounceTimer.Tick += MonthYearDebounceTimer_Tick;
            caytrong_CB.TextUpdate += productSKU_cbb_TextUpdate;
            nguoiPhuTrach_CB.TextUpdate += Employee_CBB_TextUpdate;
            deparment_CBB.TextUpdate += Department_CBB_TextUpdate;

            dientich_tb.KeyPress += Tb_KeyPress_OnlyNumber_decimal;
            soLuong_tb.KeyPress += Tb_KeyPress_OnlyNumber_int;
            dataGV.CellFormatting += dataGV_CellFormatting;
        }

        private void Kho_Materials_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SQLStore_Kho.Instance.removeDomesticLiquidationImport();
                ShowData();
            }
            else if (!isNewState && !edit_btn.Visible)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    System.Windows.Forms.Control ctrl = this.ActiveControl;

                    if (ctrl is System.Windows.Forms.TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    deleteProduct();
                }
            }
        }

        public async void ShowData()
        {
            dataGV.SelectionChanged -= this.dataGV_CellClick;
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                monthYear_dtp.ValueChanged -= monthYearDtp_ValueChanged;
                var parameters = new Dictionary<string, object> { { "IsActive", true } };
                string[] keepColumns = { "EmployeeCode", "FullName", "EmployessName_NoSign" };

                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
                var departmentTask = SQLStore_QLNS.Instance.GetDepartmentAsync();
                var productSKUTask = SQLStore_Kho.Instance.getProductSKUAsync(parameters);
                var plantingManagementTask = SQLStore_KhoVatTu.Instance.getPlantingManagementAsync(monthYear_dtp.Value.Year);
                await Task.WhenAll(departmentTask, productSKUTask, plantingManagementTask, employeesTask);
                mDepartment_dt = departmentTask.Result;
                mProductSKU_dt = productSKUTask.Result;
                mPlantingManagement_dt = plantingManagementTask.Result;
                mEmployee_dt = employeesTask.Result;

                deparment_CBB.DataSource = mDepartment_dt;
                deparment_CBB.DisplayMember = "DepartmentName";  // hiển thị tên
                deparment_CBB.ValueMember = "DepartmentID";
                deparment_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                caytrong_CB.DataSource = mProductSKU_dt;
                caytrong_CB.DisplayMember = "ProductNameVN";  // hiển thị tên
                caytrong_CB.ValueMember = "SKU";
                caytrong_CB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                nguoiPhuTrach_CB.DataSource = mEmployee_dt;
                nguoiPhuTrach_CB.DisplayMember = "FullName";  // hiển thị tên
                nguoiPhuTrach_CB.ValueMember = "EmployeeCode";
                nguoiPhuTrach_CB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                dataGV.DataSource = mPlantingManagement_dt;
                //    log_GV.DataSource = mLogDV;
                Utils.HideColumns(dataGV, new[] { "PlantingID", "SKU", "Department", "Department", "Supervisor", "CreatedAt" });
                Utils.SetGridOrdinal(mPlantingManagement_dt, new[] { "ProductionOrder", "PlantName", "Quantity", "NurseryDate", "PlantingDate", "Area", "HarvestDate", "DepartmentName", "SupervisorName", "Note", "IsCompleted" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"ProductionOrder", "Lệnh\nSản Xuất" },
                        {"Area", "Diện\nTích" },
                        {"Quantity", "S.Lượng\nCây" },
                        {"NurseryDate", "Ngày\nƯơm Hạt" },
                        {"PlantingDate", "Ngày\nTrồng" },
                        {"HarvestDate", "Ngày\nThu Hoạch" },
                        {"DepartmentName", "Tổ Phụ\nTrách Trồng" },
                        {"SupervisorName", "Giám Sát" },
                        {"PlantName", "Tên\nCây Trồng" },
                        {"IsCompleted", "Đóng Lệnh" },
                        {"Note", "Ghi Chú" }
                    });

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


                Utils.SetTabStopRecursive(this, false);

                int countTab = 0;
                lenhSX_tb.TabIndex = countTab++; lenhSX_tb.TabStop = true;
                caytrong_CB.TabIndex = countTab++; caytrong_CB.TabStop = true;
                soLuong_tb.TabIndex = countTab++; soLuong_tb.TabStop = true;
                ngayUom_dtp.TabIndex = countTab++; ngayUom_dtp.TabStop = true;
                ngaytrong_dtp.TabIndex = countTab++; ngaytrong_dtp.TabStop = true;
                ngaythu_dtp.TabIndex = countTab++; ngaythu_dtp.TabStop = true;
                dientich_tb.TabIndex = countTab++; dientich_tb.TabStop = true;
                deparment_CBB.TabIndex = countTab++; deparment_CBB.TabStop = true;
                nguoiPhuTrach_CB.TabIndex = countTab++; nguoiPhuTrach_CB.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;

                ReadOnly_btn_Click(null, null);
                dataGV.SelectionChanged += this.dataGV_CellClick;
                monthYear_dtp.ValueChanged += monthYearDtp_ValueChanged;
            }
            catch
            {
                status_lb.Text = "Thất bại.";
                status_lb.ForeColor = Color.Red;
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }

        
        private void productSKU_cbb_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            productSKUDebounceTimer.Stop();
            productSKUDebounceTimer.Start();
        }

        private void productSKUDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                productSKUDebounceTimer.Stop();

                string typed = caytrong_CB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mProductSKU_dt.AsEnumerable()
                    .Where(r => r["ProductNameVN_NoSign"].ToString().ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mProductSKU_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                caytrong_CB.DataSource = temp;
                caytrong_CB.DisplayMember = "ProductNameVN";  // hiển thị tên
                caytrong_CB.ValueMember = "SKU";

                // Giữ lại text người đang gõ
                caytrong_CB.DroppedDown = true;
                caytrong_CB.Text = typed;
                caytrong_CB.SelectionStart = typed.Length;
                caytrong_CB.SelectionLength = 0;
            }
            catch { }
        }

        private void Employee_CBB_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            employeeDebounceTimer.Stop();
            employeeDebounceTimer.Start();
        }
        
        private void employeeDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                employeeDebounceTimer.Stop();

                string typed = nguoiPhuTrach_CB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mEmployee_dt.AsEnumerable()
                    .Where(r => r["EmployessName_NoSign"].ToString().ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mEmployee_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                nguoiPhuTrach_CB.DataSource = temp;
                nguoiPhuTrach_CB.DisplayMember = "FullName";
                nguoiPhuTrach_CB.ValueMember = "EmployeeCode";

                // Giữ lại text người đang gõ
                nguoiPhuTrach_CB.DroppedDown = true;
                nguoiPhuTrach_CB.Text = typed;
                nguoiPhuTrach_CB.SelectionStart = typed.Length;
                nguoiPhuTrach_CB.SelectionLength = 0;
            }
            catch { }
        }

        private void Department_CBB_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            departmentDebounceTimer.Stop();
            departmentDebounceTimer.Start();
        }
        private void departmentDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                departmentDebounceTimer.Stop();

                string typed = deparment_CBB.Text ?? "";
                string plain = Utils.RemoveVietnameseSigns(typed).ToLower();

                // Filter bằng LINQ
                var filtered = mDepartment_dt.AsEnumerable()
                    .Where(r => Utils.RemoveVietnameseSigns(r["DepartmentName"].ToString()).ToLower()
                    .Contains(plain));

                System.Data.DataTable temp;
                if (filtered.Any())
                    temp = filtered.CopyToDataTable();
                else
                    temp = mDepartment_dt.Clone(); // nếu không có kết quả thì trả về table rỗng

                // Gán lại DataSource
                deparment_CBB.DataSource = temp;
                deparment_CBB.DisplayMember = "DepartmentName";  // hiển thị tên
                deparment_CBB.ValueMember = "DepartmentID";

                // Giữ lại text người đang gõ
                deparment_CBB.DroppedDown = true;
                deparment_CBB.Text = typed;
                deparment_CBB.SelectionStart = typed.Length;
                deparment_CBB.SelectionLength = 0;
            }
            catch { }
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }

        private async void updateItem(int plantingID, string ProductionOrder, int SKU, decimal Area, int Quantity, DateTime NurseryDate, DateTime PlantingDate, DateTime HarvestDate, int Department, string Supervisor, string Note, bool IsCompleted)
        {
            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{Supervisor}'");
            DataRow[] departmentRows = mDepartment_dt.Select($"DepartmentID = {Department}");
            DataRow[] productRows = mProductSKU_dt.Select($"SKU = {SKU}");
            if (employeeRows.Length <= 0 || departmentRows.Length <= 0 || productRows.Length <= 0) return;

            foreach (DataRow row in mPlantingManagement_dt.Rows)
            {
                int ID = Convert.ToInt32(row["PlantingID"]);
                if (ID.CompareTo(plantingID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.updatePlantingManagementAsync(plantingID, ProductionOrder, SKU, Area, Quantity, NurseryDate, PlantingDate, HarvestDate, Department, Supervisor, Note, IsCompleted);

                            if (isScussess == true)
                            {
                                row["ProductionOrder"] = ProductionOrder;
                                row["SKU"] = SKU;
                                row["Area"] = Area;
                                row["Quantity"] = Quantity;
                                row["NurseryDate"] = NurseryDate;
                                row["PlantingDate"] = PlantingDate;
                                row["HarvestDate"] = HarvestDate;
                                row["Department"] = Department;
                                row["Supervisor"] = Supervisor;
                                row["Note"] = Note;
                                row["DepartmentName"] = departmentRows[0]["DepartmentName"].ToString();
                                row["PlantName"] = productRows[0]["ProductNameVN"].ToString();
                                row["SupervisorName"] = employeeRows[0]["FullName"].ToString();
                                row["IsCompleted"] = IsCompleted;

                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;
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

        private async void createItem(string ProductionOrder, int SKU, decimal Area, int Quantity, DateTime NurseryDate, DateTime PlantingDate, DateTime HarvestDate, int Department, string Supervisor, string Note)
        {
            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{Supervisor}'");
            DataRow[] departmentRows = mDepartment_dt.Select($"DepartmentID = {Department}");
            DataRow[] productRows = mProductSKU_dt.Select($"SKU = {SKU}");
            if (employeeRows.Length <= 0 || departmentRows.Length <= 0 || productRows.Length <= 0) return;

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    int newId = await SQLManager_KhoVatTu.Instance.insertPlantingManagementAsync(ProductionOrder, SKU, Area, Quantity, NurseryDate, PlantingDate, HarvestDate, Department, Supervisor, Note);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mPlantingManagement_dt.NewRow();
                        drToAdd["PlantingID"] = newId;
                        drToAdd["ProductionOrder"] = ProductionOrder;
                        drToAdd["SKU"] = SKU;
                        drToAdd["Area"] = Area;
                        drToAdd["Quantity"] = Quantity;
                        drToAdd["NurseryDate"] = NurseryDate;
                        drToAdd["PlantingDate"] = PlantingDate;
                        drToAdd["HarvestDate"] = HarvestDate;
                        drToAdd["Department"] = Department;
                        drToAdd["Supervisor"] = Supervisor;
                        drToAdd["Note"] = Note;
                        drToAdd["DepartmentName"] = departmentRows[0]["DepartmentName"].ToString();
                        drToAdd["PlantName"] = productRows[0]["ProductNameVN"].ToString();
                        drToAdd["SupervisorName"] = employeeRows[0]["FullName"].ToString();
                        drToAdd["IsCompleted"] = false;

                        drToAdd["CreatedAt"] = DateTime.Now;

                        mPlantingManagement_dt.Rows.Add(drToAdd);
                        mPlantingManagement_dt.AcceptChanges();

                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        newBtn_Click(null, null);
                    }
                    else
                    {
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }

            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (deparment_CBB.SelectedValue == null || caytrong_CB.SelectedValue == null || string.IsNullOrEmpty(lenhSX_tb.Text.Trim()) || string.IsNullOrEmpty(dientich_tb.Text.Trim()) || string.IsNullOrEmpty(soLuong_tb.Text.Trim())) return;

            string productionOrder = lenhSX_tb.Text;
            int sku = Convert.ToInt32(caytrong_CB.SelectedValue);
            decimal area = Utils.ParseDecimalSmart(dientich_tb.Text);
            int quantity = Convert.ToInt32(soLuong_tb.Text);
            DateTime nurseryDate = ngayUom_dtp.Value.Date;
            DateTime plantingDate = ngaytrong_dtp.Value.Date;
            DateTime harvestDate = ngaythu_dtp.Value.Date;
            int dep = Convert.ToInt32(deparment_CBB.SelectedValue);
            string empCode = Convert.ToString(nguoiPhuTrach_CB.SelectedValue);
            string note = note_tb.Text;
            if (id_tb.Text.Length != 0)
                updateItem(Convert.ToInt32(id_tb.Text), productionOrder, sku, area, quantity, nurseryDate, plantingDate, harvestDate, dep, empCode, note, complete_cb.Checked);
            else
                createItem(productionOrder, sku, area, quantity, nurseryDate, plantingDate, harvestDate, dep, empCode, note);

        }
        private async void deleteProduct()
        {
            string id = id_tb.Text;

            foreach (DataRow row in mPlantingManagement_dt.Rows)
            {
                string materialID = row["MaterialID"].ToString();
                if (materialID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.deletetMaterialsAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mPlantingManagement_dt.Rows.Remove(row);
                                mPlantingManagement_dt.AcceptChanges();
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

        private void newBtn_Click(object sender, EventArgs e)
        {
            id_tb.Text = "";
            status_lb.Text = "";     

            info_gb.BackColor = newCustomerBtn.BackColor;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            isNewState = true;
            LuuThayDoiBtn.Text = "Lưu Mới";
            lenhSX_tb.Enabled = true;
            deparment_CBB.Enabled = true;
            caytrong_CB.Enabled = true;
            complete_cb.Visible = false;
            ngayUom_dtp.Enabled = true;
            ngaytrong_dtp.Enabled = true;
            ngaythu_dtp.Enabled = true;
            dientich_tb.Enabled = true;
            note_tb.Enabled = true;
            soLuong_tb.Enabled = true;
            nguoiPhuTrach_CB.Enabled = true;
            lenhSX_tb.Focus();
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            lenhSX_tb.Enabled = false;
            deparment_CBB.Enabled = false;
            caytrong_CB.Enabled = false;
            complete_cb.Visible = true;
            ngayUom_dtp.Enabled = false;
            ngaytrong_dtp.Enabled = false;
            ngaythu_dtp.Enabled = false;
            dientich_tb.Enabled = false;
            soLuong_tb.Enabled = false;
            nguoiPhuTrach_CB.Enabled = false;
            note_tb.Enabled = false;
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            lenhSX_tb.Enabled = true;
            deparment_CBB.Enabled = true;
            caytrong_CB.Enabled = true;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            ngayUom_dtp.Enabled = true;
            ngaytrong_dtp.Enabled = true;
            ngaythu_dtp.Enabled = true;
            dientich_tb.Enabled = true;
            note_tb.Enabled = true;
            soLuong_tb.Enabled = true;
            nguoiPhuTrach_CB.Enabled = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
        }

        private void updateRightUI()
        {
            try
            {
                if (isNewState) return;

                if (dataGV.SelectedRows.Count > 0)
                {
                    var cells = dataGV.SelectedRows[0].Cells;

                    int? department = cells["Department"].Value == null || cells["Department"].Value == DBNull.Value ? (int?)null : Convert.ToInt32(cells["Department"].Value);
                    int sku = Convert.ToInt32(cells["SKU"].Value);
                    string Supervisor = Convert.ToString(cells["Supervisor"].Value);

                    if (!deparment_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["DepartmentID"].ToString() == department.ToString()))
                    {
                        deparment_CBB.DataSource = mDepartment_dt;
                    }                    

                    if (!caytrong_CB.Items.Cast<object>().Any(i => ((DataRowView)i)["SKU"].ToString() == sku.ToString()))
                    {
                        caytrong_CB.DataSource = mProductSKU_dt;
                    }

                    if (!nguoiPhuTrach_CB.Items.Cast<object>().Any(i => ((DataRowView)i)["EmployeeCode"].ToString() == Supervisor))
                    {
                        nguoiPhuTrach_CB.DataSource = mEmployee_dt;
                    }

                    id_tb.Text = cells["PlantingID"].Value.ToString();
                    lenhSX_tb.Text = cells["ProductionOrder"].Value.ToString();
                    soLuong_tb.Text = cells["Quantity"].Value.ToString();
                    dientich_tb.Text = Convert.ToDecimal(cells["Area"].Value).ToString("0.##", CultureInfo.InvariantCulture);
                    ngayUom_dtp.Value = Convert.ToDateTime(cells["NurseryDate"].Value);
                    ngaytrong_dtp.Value = Convert.ToDateTime(cells["PlantingDate"].Value);
                    ngaythu_dtp.Value = Convert.ToDateTime(cells["HarvestDate"].Value);
                    
                    if (department.HasValue)
                        deparment_CBB.SelectedValue = department.Value;
                    else
                        deparment_CBB.SelectedIndex = -1;

                    nguoiPhuTrach_CB.SelectedValue = Supervisor;
                    caytrong_CB.SelectedValue = sku;
                    note_tb.Text = cells["Note"].Value.ToString();
                    complete_cb.Checked = Convert.ToBoolean(cells["IsCompleted"].Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        private void Tb_KeyPress_OnlyNumber_decimal(object sender, KeyPressEventArgs e)
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

        private void Tb_KeyPress_OnlyNumber_int(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            // Cho phép số
            if (char.IsDigit(e.KeyChar))
                return;

            // Còn lại thì chặn
            e.Handled = true;
        }

        private void dataGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGV.Columns[e.ColumnIndex].Name == "Area" && e.Value != null)
            {
                if (e.Value is decimal dec)
                {
                    e.Value = dec.ToString("0.00");
                    e.FormattingApplied = true;
                }
            }
        }

        private void monthYearDtp_ValueChanged(object sender, EventArgs e)
        {
            // Mỗi lần thay đổi thì reset timer
            _monthYearDebounceTimer.Stop();
            _monthYearDebounceTimer.Start();
        }

        private void MonthYearDebounceTimer_Tick(object sender, EventArgs e)
        {
            _monthYearDebounceTimer.Stop();
            ShowData();
        }


        //System.Data.DataTable excelData;
        //private async void button1_Click(object sender, EventArgs e)
        //{

        //    var unit_dt = await SQLManager_KhoVatTu.Instance.GetUnitAsync();
        //    var materialCategory_dt = await SQLManager_KhoVatTu.Instance.GetMaterialCategoryAsync();

        //    using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
        //    {
        //        ofd.Title = "Chọn file Excel";
        //        ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
        //        ofd.Multiselect = false; // chỉ cho chọn 1 file

        //        if (ofd.ShowDialog() == DialogResult.OK)
        //        {
        //            string filePath = ofd.FileName;
        //            excelData = Utils.LoadExcel_NoHeader(filePath);

        //            var employees = await SQLStore_QLNS.Instance.GetEmployeesAsync();
        //            var departments = await SQLStore_QLNS.Instance.GetDepartmentAsync();
        //            var products = await SQLStore_Kho.Instance.getProductSKUAsync();

        //            Utils.AddColumnIfNotExists(excelData, "Quantity", typeof(int));
        //            Utils.AddColumnIfNotExists(excelData, "Area", typeof(decimal));
        //            Utils.AddColumnIfNotExists(excelData, "Supervisor", typeof(string));
        //            Utils.AddColumnIfNotExists(excelData, "Department", typeof(int));
        //            Utils.AddColumnIfNotExists(excelData, "SKU", typeof(int));
        //            foreach (DataRow row in excelData.Rows)
        //            {
        //                string raw = row["Column3"].ToString(); 
        //                string numberPart = raw.Replace("cây", "").Trim();
        //                numberPart = numberPart.Replace(".", "");
        //                row["Quantity"] = int.Parse(numberPart);

        //                raw = row["Column6"].ToString().ToLower();
        //                numberPart = raw.Replace("m2", "").Trim();
        //                numberPart = numberPart.Replace("m3", "").Trim();
        //                numberPart = numberPart.Replace(",", ".");
        //                if (string.IsNullOrWhiteSpace(numberPart))
        //                    numberPart = "0";
        //                row["Area"] = decimal.Parse(numberPart, System.Globalization.CultureInfo.InvariantCulture);

        //                raw = row["Column9"].ToString().ToLower();
        //                DataRow emp = employees.AsEnumerable().FirstOrDefault(r => r.Field<string>("FullName").Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if(emp != null)
        //                    row["Supervisor"] = emp["EmployeeCode"];

        //                raw = row["Column8"].ToString().ToLower();
        //                if (raw.CompareTo("farm") == 0)
        //                    raw = "farm vr";
        //                DataRow dep = departments.AsEnumerable().FirstOrDefault(r => r.Field<string>("DepartmentName").Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if (dep != null)
        //                    row["Department"] = dep["DepartmentID"];

        //                raw = row["Column2"].ToString().ToLower();
        //                if (raw.CompareTo("cải cúc") == 0)
        //                    raw = "tần ô (cải cúc)";
        //                DataRow product = products.AsEnumerable().FirstOrDefault(r => r.Field<string>("ProductNameVN").Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if (product != null)
        //                    row["SKU"] = product["SKU"];
        //            }

        //            dataGV.DataSource = excelData;
        //        }
        //    }
        //}

        //private async void button2_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        foreach (DataRow edr in excelData.Rows)
        //        {

        //            string ProductionOrder = edr["Column1"].ToString();
        //            int SKU = Convert.ToInt32(edr["SKU"]);
        //            decimal Area = Convert.ToDecimal(edr["Area"]);
        //            int Quantity = Convert.ToInt32(edr["Quantity"]);
        //            DateTime NurseryDate = Convert.ToDateTime(edr["Column4"]);
        //            DateTime PlantingDate = Convert.ToDateTime(edr["Column5"]);
        //            DateTime HarvestDate = Convert.ToDateTime(edr["Column7"]);
        //            int? department = edr["Department"] == DBNull.Value ? (int?)null : Convert.ToInt32(edr["Department"]);
        //            string Supervisor = Convert.ToString(edr["Supervisor"]);

        //            string Note = "";
        //            if (excelData.Columns.Contains("Column10"))
        //                Note = Convert.ToString(edr["Column10"]);

        //            int salaryInfoID = await SQLManager_KhoVatTu.Instance.insertPlantingManagementAsync(ProductionOrder, SKU, Area, Quantity, NurseryDate, PlantingDate, HarvestDate, department, Supervisor, Note);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("có lỗi " + ex.Message);
        //    }
        //    MessageBox.Show("xong");
        //}
    }
}
