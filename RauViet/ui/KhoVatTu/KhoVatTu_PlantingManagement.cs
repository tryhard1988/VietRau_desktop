using Microsoft.Office.Interop.Excel;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class KhoVatTu_PlantingManagement : Form
    {
        System.Data.DataTable mPlantingManagement_dt, mDepartment_dt, mProductSKU_dt, mEmployee_dt;
        private DataView mLogDV;
        private Timer productSKUDebounceTimer = new Timer { Interval = 300 };
        private Timer UnitDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        private LoadingOverlay loadingOverlay;
        public KhoVatTu_PlantingManagement()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

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
            UnitDebounceTimer.Tick += unitDebounceTimer_Tick;
            caytrong_CB.TextUpdate += productSKU_cbb_TextUpdate;
            deparment_CBB.TextUpdate += unit_CBB_TextUpdate;
            //quantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;
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
                var parameters = new Dictionary<string, object> { { "IsActive", true } };
                string[] keepColumns = { "EmployeeCode", "FullName"};

                var employeesTask = SQLStore_QLNS.Instance.GetEmployeesAsync(keepColumns);
                var departmentTask = SQLStore_QLNS.Instance.GetDepartmentAsync();
                var productSKUTask = SQLStore_Kho.Instance.getProductSKUAsync(parameters);
                var plantingManagementTask = SQLStore_KhoVatTu.Instance.getPlantingManagementAsync();
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

                //Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                //        {"MaterialName", 300},
                //        {"CategoryName", 150},
                //        {"UnitName",60}
                //    });

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                //    updateRightUI();

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

                //    await Task.Delay(100);
                //    loadingOverlay.Hide();

                //    Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                //        {"OldValue", "Giá Trị Cũ" },
                //        {"NewValue", "Giá Trị Mới" },
                //        {"ActionBy", "Người Thực Hiện" },
                //        {"CreatedAt", "Ngày Thực Hiện" }
                //    });
                //    Utils.SetGridWidths(log_GV, new System.Collections.Generic.Dictionary<string, int> {
                //        {"ActionBy", 150},
                //        {"CreatedAt", 120}
                //    });
                //    Utils.SetGridWidth(log_GV, "OldValue", DataGridViewAutoSizeColumnMode.Fill);
                //    Utils.SetGridWidth(log_GV, "NewValue", DataGridViewAutoSizeColumnMode.Fill);
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

        private void unit_CBB_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            UnitDebounceTimer.Stop();
            UnitDebounceTimer.Start();
        }
        private void unitDebounceTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                UnitDebounceTimer.Stop();

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
                nguoiPhuTrach_CB.ValueMember = "ExportCode";

                // Giữ lại text người đang gõ
                nguoiPhuTrach_CB.DroppedDown = true;
                nguoiPhuTrach_CB.Text = typed;
                nguoiPhuTrach_CB.SelectionStart = typed.Length;
                nguoiPhuTrach_CB.SelectionLength = 0;
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
            decimal area = Convert.ToDecimal(dientich_tb.Text);
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

                    int Department = Convert.ToInt32(cells["Department"].Value);
                    int sku = Convert.ToInt32(cells["SKU"].Value);
                    string Supervisor = Convert.ToString(cells["Supervisor"].Value);

                    if (!deparment_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["DepartmentID"].ToString() == Department.ToString()))
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
                    dientich_tb.Text = cells["Area"].Value.ToString();
                    ngayUom_dtp.Value = Convert.ToDateTime(cells["NurseryDate"].Value);
                    ngaytrong_dtp.Value = Convert.ToDateTime(cells["PlantingDate"].Value);
                    ngaythu_dtp.Value = Convert.ToDateTime(cells["HarvestDate"].Value);
                    deparment_CBB.SelectedValue = Department;
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
            ;
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
        //            System.Data.DataTable excelData = Utils.LoadExcel_NoHeader(filePath);

        //            excelData = excelData.AsEnumerable()
        //                                .Where(r => !string.IsNullOrWhiteSpace(r["Column1"]?.ToString()))
        //                                .Select(r => r["Column1"].ToString().Trim())   // chuẩn hoá
        //                                .Distinct(StringComparer.OrdinalIgnoreCase)    // bỏ trùng, ignore case
        //                                .OrderBy(x => x)                               // sort A-Z
        //                                .Select(x =>
        //                                {
        //                                    var row = excelData.NewRow();
        //                                    row["Column1"] = x;
        //                                    return row;
        //                                })
        //                                .CopyToDataTable();
        //            try
        //            {

        //                foreach (DataRow edr in excelData.Rows)
        //                {
        //                    string name = edr["Column1"].ToString();
        //                    if (string.IsNullOrWhiteSpace(name.Trim())) continue;

        //                    int salaryInfoID = await SQLManager_KhoVatTu.Instance.insertCropAsync(name);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("có lỗi " + ex.Message);
        //            }
        //            MessageBox.Show("xong");
        //        }
        //    }
        //}

    }
}
