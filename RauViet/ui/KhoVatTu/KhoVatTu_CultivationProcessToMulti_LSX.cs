using DocumentFormat.OpenXml.Bibliography;
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
    public partial class KhoVatTu_CultivationProcessToMulti_LSX : Form
    {
        DataView mPlantingRowDV;
        DataTable mMaterial_dt, mWorkType_dt, mCultivationProcessTemplate_dt, mEmployee_dt, mDepartment_dt;
        private Timer FertilizationTypeDebounceTimer = new Timer { Interval = 300 };
        private Timer WorkTypeDebounceTimer = new Timer { Interval = 300 };
        private Timer MaterialDebounceTimer = new Timer { Interval = 300 };
        private Timer EmployeeDebounceTimer = new Timer { Interval = 300 };
        private Timer departmentDebounceTimer = new Timer { Interval = 300 };

        bool isNewState = false;
        int mDepartmentID;
        int mYear;

        private LoadingOverlay loadingOverlay;        
        public KhoVatTu_CultivationProcessToMulti_LSX(int departmentID, int year)
        {
            InitializeComponent();
            this.KeyPreview = true;
            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            processDate_dtp.Format = DateTimePickerFormat.Custom;
            processDate_dtp.CustomFormat = "dd/MM/yyyy";

            status_lb.Text = "";

            mDepartmentID = departmentID;
            mYear = year;


            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;

            LoadDefaultData_btn.Click += LoadDefaultData_btn_Click;
            this.KeyDown += form_KeyDown;

            FertilizationTypeDebounceTimer.Tick += FertilizationTypeDebounceTimer_Tick;
            hinhThucBon_CBB.TextUpdate += HinhThucBon_CBB_TextUpdate;

            WorkTypeDebounceTimer.Tick += WokTypeDebounceTimer_Tick;
            congViec_CBB.TextUpdate += CongViec_CBB_TextUpdate;
            departmentDebounceTimer.Tick += departmentDebounceTimer_Tick;

            MaterialDebounceTimer.Tick += MaterialDebounceTimer_Tick;
            vatTu_CB.TextUpdate += VatTu_CB_TextUpdate;
            department_CBB.TextUpdate += Department_CBB_TextUpdate;
            vatTu_CB.SelectedIndexChanged += VatTu_CB_SelectedIndexChanged;

            EmployeeDebounceTimer.Tick += EmployeeDebounceTimer_Tick;
            employee_CBB.TextUpdate += Employee_CBB_TextUpdate;

            materialQuantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            waterAmount_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            isolationDays_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            search_tb.TextChanged += Search_tb_TextChanged;
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                ShowData();
            }
            else if ((!isNewState && !edit_btn.Visible))
            {
                if (e.KeyCode == Keys.Delete)
                {
                    System.Windows.Forms.Control ctrl = this.ActiveControl;

                    if (ctrl is System.Windows.Controls.TextBox || ctrl is RichTextBox || (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    if((!isNewState && !edit_btn.Visible))
                        nktd_deleteProduct(id_tb.Text);
                }
            }
        }

        public async void ShowData()
        {
            dataGV.SelectionChanged -= this.dataGV_CellClick;
            plantingGV.SelectionChanged -= PlantingGV_SelectionChanged;
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            await loadingOverlay.Show();

            try
            {
                var plantingManagement_dt = await SQLStore_KhoVatTu.Instance.getPlantingManagementAsync(mYear);
                mPlantingRowDV = new DataView(plantingManagement_dt);
                mPlantingRowDV.RowFilter = $"Department = {mDepartmentID} AND IsCompleted = false";
                DataTable cultivationProcess_dt = null;

                List<int> plantingIDs = new List<int>();
                foreach (DataRowView rowView in mPlantingRowDV)
                {
                    DataRow row = rowView.Row;
                    int plantingID = Convert.ToInt32(row["PlantingID"]);
                    plantingIDs.Add(plantingID);
                    if (cultivationProcess_dt == null)
                        cultivationProcess_dt = await SQLStore_KhoVatTu.Instance.GetCultivationProcessAsync(plantingID);
                }
                await SQLStore_KhoVatTu.Instance.GetCultivationProcessAsync(plantingIDs);


                string[] empKeepColumns = { "EmployeeCode", "FullName", "EmployessName_NoSign", "DepartmentID", "PositionID" };   
                var cultivationProcessTemplateTask = SQLStore_KhoVatTu.Instance.GetCultivationProcessTemplateAsync();
                var materialTask = SQLStore_KhoVatTu.Instance.getMaterialAsync();
                var workTypeTask = SQLStore_KhoVatTu.Instance.GetWorkTypeAsync();
                var departmentTask = SQLStore_QLNS.Instance.GetDepartmentAsync();
                var employeeTask = SQLStore_QLNS.Instance.GetEmployeesAsync(empKeepColumns);

                await Task.WhenAll( materialTask, workTypeTask, cultivationProcessTemplateTask, employeeTask, departmentTask);
                                                
                mMaterial_dt = materialTask.Result;
                mWorkType_dt = workTypeTask.Result;
                mCultivationProcessTemplate_dt = cultivationProcessTemplateTask.Result;
                mEmployee_dt = employeeTask.Result;
                mDepartment_dt = departmentTask.Result;
                                
                vatTu_CB.DataSource = mMaterial_dt;
                vatTu_CB.DisplayMember = "MaterialName";  // hiển thị tên
                vatTu_CB.ValueMember = "MaterialID";
                vatTu_CB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                hinhThucBon_CBB.DataSource = mWorkType_dt.Copy();
                hinhThucBon_CBB.DisplayMember = "WorkTypeName";  // hiển thị tên
                hinhThucBon_CBB.ValueMember = "WorkTypeID";
                hinhThucBon_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                congViec_CBB.DataSource = mWorkType_dt;
                congViec_CBB.DisplayMember = "WorkTypeName";  // hiển thị tên
                congViec_CBB.ValueMember = "WorkTypeID";
                congViec_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                employee_CBB.DataSource = mEmployee_dt;
                employee_CBB.DisplayMember = "FullName";  // hiển thị tên
                employee_CBB.ValueMember = "EmployeeCode";
                employee_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                department_CBB.DataSource = mDepartment_dt;
                department_CBB.DisplayMember = "DepartmentName";  // hiển thị tên
                department_CBB.ValueMember = "DepartmentID";
                department_CBB.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

               
                dataGV.DataSource = cultivationProcess_dt;
                plantingGV.DataSource = mPlantingRowDV;

                LoadDefaultData_btn.Visible = cultivationProcess_dt.Rows.Count <= 0;
                
                Utils.HideColumns(plantingGV, new[] { "IsCompleted", "FarmID", "CultivationTypeID", "Department", "HarvestDate", "PlantingDate", "NurseryDate", "Quantity", "SKU", "Area", "Supervisor", "FarmName",
                    "HarvestQuantity", "SupervisorName", "PlantLocation", "Note", "PlantingID", "search_nosign", "DepartmentName", "CultivationTypeName" });
                Utils.SetGridHeaders(plantingGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"ProductionOrder", "LSX"},
                        {"PlantName", "Cây Trồng" },
                    });
                Utils.SetGridWidths(plantingGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"ProductionOrder", 70 },
                        {"MaterialQuantity", 100 },
                    });

                Utils.HideColumns(dataGV, new[] { "CultivationProcessID", "PlantingID", "MaterialID", "WorkTypeID", "WorkTypeID", "MaterialID" , "EmployeeCode" , "DepartmentID", "FertilizationWorkTypeID", "CategoryCode" });
                Utils.SetGridOrdinal(cultivationProcess_dt, new[] { "ProcessDate", "ProcessDate_Week", "FertilizationWorkTypeName", "WorkTypeName", "MaterialName", "MaterialQuantity", "MaterialUnit", "Dosage", "PlantStatus", "EmployeeName", "IsolationDays", "IsolationEndDate", "DepartmentName", "PlantLocation", "WaterAmount", "MaterialPrice", "TotalMaterialCost", "Note" });
                Utils.SetGridHeaders(dataGV, new System.Collections.Generic.Dictionary<string, string> {
                        {"ProcessDate", "Ngày Xử Lý" },
                        {"WorkTypeName", "Công Việc" },
                        {"MaterialName", "Vật Tư" },
                        {"MaterialQuantity", "SL Vật Tư" },
                        {"Dosage", "Liều Lượng" },
                        {"PlantStatus", "Tình Trạng Cây" },
                        {"ActiveIngredient", "Thành Phần" },
                        {"EmployeeName", "Người Thực Hiện" },
                        {"IsolationDays", "T.Gian C.Ly" },
                        {"IsolationEndDate", "Ngày Hết C.Ly" },
                        {"DepartmentName", "Tổ Phụ Trách" },
                        {"PlantLocation", "Vị Trí Trồng" },
                        {"WaterAmount", "Lượng Nước" },
                        {"Note", "Ghi Chú" },
                        {"FertilizationWorkTypeName", "Hình Thức Bón" },
                        {"MaterialUnit", "Đơn Vị" },
                        {"MaterialPrice", "Giá V.tư" },
                        {"TotalMaterialCost", "Thành Tiền" },
                        {"ProcessDate_Week", "Thứ" }
                    });
                Utils.SetGridWidths(dataGV, new System.Collections.Generic.Dictionary<string, int> {
                        {"ProcessDate", 70 },
                        {"MaterialQuantity", 70 },
                        {"WaterAmount", 70 },
                        {"IsolationDays", 60 },
                        {"EmployeeName", 150 },
                        {"MaterialName", 150 },
                        {"IsolationEndDate", 70 },
                        {"MaterialPrice", 60 },
                        {"MaterialUnit", 50 },
                        {"ProcessDate_Week", 50 }
                    });

                Utils.SetTabStopRecursive(this, false);
                int countTab = 0;
                processDate_dtp.TabIndex = countTab++; employee_CBB.TabStop = true;
                hinhThucBon_CBB.TabIndex = countTab++; hinhThucBon_CBB.TabStop = true;
                congViec_CBB.TabIndex = countTab++; congViec_CBB.TabStop = true;
                vatTu_CB.TabIndex = countTab++; vatTu_CB.TabStop = true;
                materialQuantity_tb.TabIndex = countTab++; materialQuantity_tb.TabStop = true;
                dosage_tb.TabIndex = countTab++; dosage_tb.TabStop = true;
                plantStatus_tb.TabIndex = countTab++; plantStatus_tb.TabStop = true;
                employee_CBB.TabIndex = countTab++; employee_CBB.TabStop = true;
                isolationDays_tb.TabIndex = countTab++; isolationDays_tb.TabStop = true;
                department_CBB.TabIndex = countTab++; department_CBB.TabStop = true;
                plantLocation_tb.TabIndex = countTab++; plantLocation_tb.TabStop = true;
                waterAmount_tb.TabIndex = countTab++; waterAmount_tb.TabStop = true;
                note_tb.TabIndex = countTab++; note_tb.TabStop = true;
                LuuThayDoiBtn.TabIndex = countTab++; LuuThayDoiBtn.TabStop = true;
                               
                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                ReadOnly_btn_Click(null, null);
                dataGV.SelectionChanged += this.dataGV_CellClick;
                plantingGV.SelectionChanged += PlantingGV_SelectionChanged;
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


        private void CongViec_CBB_TextUpdate(object sender, EventArgs e)
        {
            WorkTypeDebounceTimer.Stop();
            WorkTypeDebounceTimer.Start();
        }

        private void WokTypeDebounceTimer_Tick(object sender, EventArgs e)
        {
            WorkTypeDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(congViec_CBB, mWorkType_dt, "search_nosign");            
        }

        private void HinhThucBon_CBB_TextUpdate(object sender, EventArgs e)
        {
            FertilizationTypeDebounceTimer.Stop();
            FertilizationTypeDebounceTimer.Start();
        }
        private void FertilizationTypeDebounceTimer_Tick(object sender, EventArgs e)
        {
            FertilizationTypeDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(hinhThucBon_CBB, mWorkType_dt, "search_nosign");            
        }

        private void Department_CBB_TextUpdate(object sender, EventArgs e)
        {
            // Restart timer mỗi khi gõ
            departmentDebounceTimer.Stop();
            departmentDebounceTimer.Start();
        }
        private void departmentDebounceTimer_Tick(object sender, EventArgs e)
        {
            departmentDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(department_CBB, mDepartment_dt, "DepartmentName", false);            
        }

        private void VatTu_CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isNewState) return;

            int? materialID = (string.IsNullOrEmpty(vatTu_CB.Text) || vatTu_CB.SelectedValue == null || vatTu_CB.SelectedValue == DBNull.Value) ? (int?)null : Convert.ToInt32(vatTu_CB.SelectedValue);
            if (!materialID.HasValue)
            {
                dosage_tb.Text = "";
            }
            else
            {
                DataRow[] matiralRows = mMaterial_dt.Select($"MaterialID = {materialID}");

                string composition = matiralRows.Length > 0 ? matiralRows[0]["Composition"].ToString() : "";
                dosage_tb.Text = composition.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                    .FirstOrDefault(l => Utils.RemoveVietnameseSigns(l).ToLower().Contains("lieu luong"))
                                    ?.Split(':')[1]
                                    .Trim();

                isolationDays_tb.Text = composition.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                            .FirstOrDefault(l => Utils.RemoveVietnameseSigns(l).ToLower().Contains("thoi gian canh ly"))
                                            ?.Split(':')[1]
                                            .Trim()
                                            .TrimEnd('.');

                
            }
        }
                
        private void VatTu_CB_TextUpdate(object sender, EventArgs e)
        {
            MaterialDebounceTimer.Stop();
            MaterialDebounceTimer.Start();
        }

        private void MaterialDebounceTimer_Tick(object sender, EventArgs e)
        {
            MaterialDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(vatTu_CB, mMaterial_dt, "MaterialName_nosign");
        }

        private void Employee_CBB_TextUpdate(object sender, EventArgs e)
        {
            EmployeeDebounceTimer.Stop();
            EmployeeDebounceTimer.Start();
        }
        private void EmployeeDebounceTimer_Tick(object sender, EventArgs e)
        {
            EmployeeDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(employee_CBB, mEmployee_dt, "EmployessName_NoSign");
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }
      
        private async void nktd_updateItem(
            int cultivationProcessID,
            DateTime processDate,
            int fertilizationWorkTypeID,
            int workTypeID,
            int? materialID,
            decimal materialQuantity,
            string dosage,
            string plantStatus,
            string employeeCode,
            int isolationDays,
            int? departmentID,
            string plantLocation,
            decimal waterAmount,
            string note)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            DataRow[] fertilizationWorkRows = mWorkType_dt.Select($"WorkTypeID = {fertilizationWorkTypeID}");
            DataRow[] workTypeRows = mWorkType_dt.Select($"WorkTypeID = {workTypeID}");
            DataRow[] matiralRows = Array.Empty<DataRow>();
            if (materialID.HasValue)
                matiralRows = mMaterial_dt.Select($"MaterialID = {materialID}");

            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");

            DataRow[] departmentRows = Array.Empty<DataRow>();
            if (departmentID.HasValue)
                departmentRows = mDepartment_dt.Select($"DepartmentID = {departmentID}");

            if (plantingGV.SelectedRows.Count != 1)
            {
                MessageBox.Show("Chỉ chọn 1 lệnh sản xuất", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int plantingID = Convert.ToInt32(plantingGV.SelectedRows[0].Cells["PlantingID"].Value);
            var cultivationProcess_dt = await SQLStore_KhoVatTu.Instance.GetCultivationProcessAsync(plantingID);
            foreach (DataRow row in cultivationProcess_dt.Rows)
            {
                int ID = Convert.ToInt32(row["CultivationProcessID"]);
                if (ID.CompareTo(cultivationProcessID) == 0)
                {

                    string workTypeName = workTypeRows.Length > 0 ? workTypeRows[0]["WorkTypeName"].ToString() : "";
                    string fertilizationWorkTypeName = fertilizationWorkRows.Length > 0 ? workTypeRows[0]["WorkTypeName"].ToString() : "";
                    string employeeName = employeeRows.Length > 0 ? employeeRows[0]["FullName"].ToString() : "";
                    string departmentName = departmentRows.Length > 0 ? departmentRows[0]["DepartmentName"].ToString() : "";
                    string materialName = matiralRows.Length > 0 ? matiralRows[0]["MaterialName"].ToString() : "";
                    string unitName = matiralRows.Length > 0 ? matiralRows[0]["UnitName"].ToString() : "";
                    if (unitName.CompareTo("Bao") == 0)
                        unitName = "Kg";
                    string composition = matiralRows.Length > 0 ? matiralRows[0]["Composition"].ToString() : "";

                    string activeIngredient = composition?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                                .FirstOrDefault(l => Utils.RemoveVietnameseSigns(l).ToLower().Contains("hoat chat"))
                                                ?.Split(':')
                                                .Skip(1)
                                                .FirstOrDefault()
                                                ?.Trim() ?? "";

                    string actionType = "Update ";
                    string oldValue = $"Ngày: {row["ProcessDate"]}; Hình Thức: {row["FertilizationWorkTypeName"]}; Công Việc: {row["WorkTypeName"]}; Vật Tư: {row["MaterialName"]}; S.Lượng VT: {row["MaterialQuantity"]}; Liều Lượng: {row["Dosage"]}; Tình Trạng Cây: {row["PlantStatus"]}; Hoạt Chất: {row["ActiveIngredient"]}; Người TH: {row["EmployeeName"]}; S.Ngày C.Li: {row["IsolationDays"]}; Tổ P.Trách: {row["DepartmentName"]}; V.Trí Trồng: {row["PlantLocation"]}; Lượng Nước: {row["WaterAmount"]}, Ghi Chú: {row["Note"]}";
                    string newValue = $"Ngày: {processDate}; Hình Thức: {fertilizationWorkTypeName}; Công Việc: {workTypeName}; Vật Tư: {materialName}; S.Lượng VT: {materialQuantity}; Liều Lượng: {dosage}; Tình Trạng Cây: {plantStatus}; Hoạt Chất: {activeIngredient}; Người TH: {employeeName}; S.Ngày C.Li: {isolationDays}; Tổ P.Trách: {departmentName}; V.Trí Trồng: {plantLocation}; Lượng Nước: {waterAmount}, Ghi Chú: {note}";

                    int? materialPrice = null;
                    if (materialID.HasValue)
                    {
                        DataRow materiaRow = mMaterial_dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["MaterialID"]) == materialID);
                        if (materiaRow != null)
                            materialPrice = Convert.ToInt32(materiaRow["MaterialPrice"]);
                    }
                    try
                    {
                        bool isScussess = await SQLManager_KhoVatTu.Instance.updateCultivationProcessAsync(cultivationProcessID, plantingID, processDate, fertilizationWorkTypeID, workTypeID, materialID, materialPrice,
                                                                                                         materialQuantity, dosage, plantStatus, activeIngredient, employeeCode, isolationDays, departmentID, plantLocation, waterAmount, note);

                        if (isScussess == true)
                        {
                            row["MaterialID"] = materialID.HasValue ? (object)materialID.Value : DBNull.Value;
                            row["PlantingID"] = plantingID;
                            row["WorkTypeID"] = workTypeID;
                            row["ProcessDate"] = processDate;
                            row["MaterialQuantity"] = materialQuantity;
                            row["WaterAmount"] = waterAmount;
                            row["Dosage"] = dosage;
                            row["PlantStatus"] = plantStatus;
                            row["ActiveIngredient"] = activeIngredient;
                            row["EmployeeCode"] = employeeCode;
                            row["IsolationDays"] = isolationDays;
                            row["DepartmentID"] = departmentID.HasValue ? (object)departmentID.Value : DBNull.Value;
                            row["PlantLocation"] = plantLocation;
                            row["Note"] = note;
                            row["FertilizationWorkTypeID"] = fertilizationWorkTypeID;
                            row["MaterialPrice"] = materialPrice.HasValue ? (object)materialPrice.Value : DBNull.Value;
                            row["WorkTypeName"] = workTypeName;
                            row["FertilizationWorkTypeName"] = fertilizationWorkTypeName;
                            row["EmployeeName"] = employeeName;
                            row["DepartmentName"] = departmentName;
                            row["MaterialName"] = materialName;
                            row["MaterialUnit"] = unitName;
                            row["ProcessDate_Week"] = Utils.GetThu_Viet(processDate);

                            actionType += "Success";
                            status_lb.Text = "Thành công.";
                            status_lb.ForeColor = Color.Green;
                        }
                        else
                        {
                            actionType += "Fail";
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                    }
                    catch
                    {
                        actionType += "Exception";
                        status_lb.Text = "Thất bại.";
                        status_lb.ForeColor = Color.Red;
                    }
                    finally
                    {
                        _ = SQLManager_KhoVatTu.Instance.insertCultivationProcessLogAsync(plantingID, actionType, oldValue, newValue);
                    }

                    break;
                }
            }
        }

        private async void nktd_createItem(
            DateTime processDate,
            int fertilizationWorkTypeID,
            int workTypeID,
            int? materialID,
            decimal materialQuantity,
            string dosage,
            string plantStatus,
            string employeeCode,
            int isolationDays,
            int? departmentID,
            string plantLocation,
            decimal waterAmount,
            string note)
        {
            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            DataRow[] fertilizationWorkRows = mWorkType_dt.Select($"WorkTypeID = {fertilizationWorkTypeID}");
            DataRow[] workTypeRows = mWorkType_dt.Select($"WorkTypeID = {workTypeID}");
            DataRow[] matiralRows = Array.Empty<DataRow>();
            if (materialID.HasValue)
                matiralRows = mMaterial_dt.Select($"MaterialID = {materialID}");

            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");

            DataRow[] departmentRows = Array.Empty<DataRow>();
            if (departmentID.HasValue)
                departmentRows = mDepartment_dt.Select($"DepartmentID = {departmentID}");

            string workTypeName = workTypeRows.Length > 0 ? workTypeRows[0]["WorkTypeName"].ToString() : "";
            string fertilizationWorkTypeName = fertilizationWorkRows.Length > 0 ? workTypeRows[0]["WorkTypeName"].ToString() : "";
            string employeeName = employeeRows.Length > 0 ? employeeRows[0]["FullName"].ToString() : "";
            string departmentName = departmentRows.Length > 0 ? departmentRows[0]["DepartmentName"].ToString() : "";
            string materialName = matiralRows.Length > 0 ? matiralRows[0]["MaterialName"].ToString() : "";
            string composition = matiralRows.Length > 0 ? matiralRows[0]["Composition"].ToString() : "";
            string unitName = matiralRows.Length > 0 ? matiralRows[0]["UnitName"].ToString() : "";
            if (unitName.CompareTo("Bao") == 0)
                unitName = "Kg";
            string activeIngredient = composition?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                                .FirstOrDefault(l => Utils.RemoveVietnameseSigns(l).ToLower().Contains("hoat chat"))
                                                ?.Split(':')
                                                .Skip(1)
                                                .FirstOrDefault()
                                                ?.Trim() ?? "";

            string actionType = "Create ";
            string oldValue = "";
            string newValue = $"Ngày: {processDate}; Hình Thức: {fertilizationWorkTypeName}; Công Việc: {workTypeName}; Vật Tư: {materialName}; S.Lượng VT: {materialQuantity}; Liều Lượng: {dosage}; Tình Trạng Cây: {plantStatus}; Hoạt Chất: {activeIngredient}; Người TH: {employeeName}; S.Ngày C.Li: {isolationDays}; Tổ P.Trách: {departmentName}; V.Trí Trồng: {plantLocation}; Lượng Nước: {waterAmount}, Ghi Chú: {note}";

            foreach (DataGridViewRow plantingRow in plantingGV.SelectedRows)
            {
                int plantingID = Convert.ToInt32(plantingRow.Cells["PlantingID"].Value);
                int? materialPrice = null;
                if (materialID.HasValue)
                {
                    DataRow materiaRow = mMaterial_dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["MaterialID"]) == materialID);
                    if (materiaRow != null)
                        materialPrice = Convert.ToInt32(materiaRow["MaterialPrice"]);
                }

                try
                {
                    int newId = await SQLManager_KhoVatTu.Instance.insertCultivationProcessAsync(plantingID, processDate, fertilizationWorkTypeID, workTypeID, materialID, materialPrice, materialQuantity, dosage, plantStatus,
                                                                                                activeIngredient, employeeCode, isolationDays, departmentID, plantLocation, waterAmount, note);
                    if (newId > 0)
                    {
                        var cultivationProcess_dt = await SQLStore_KhoVatTu.Instance.GetCultivationProcessAsync(plantingID);
                        DataRow drToAdd = cultivationProcess_dt.NewRow();

                        drToAdd["CultivationProcessID"] = newId;
                        drToAdd["MaterialID"] = materialID.HasValue ? (object)materialID.Value : DBNull.Value;
                        drToAdd["PlantingID"] = plantingID;
                        drToAdd["WorkTypeID"] = workTypeID;
                        drToAdd["ProcessDate"] = processDate;
                        drToAdd["MaterialQuantity"] = materialQuantity;
                        drToAdd["WaterAmount"] = waterAmount;
                        drToAdd["Dosage"] = dosage;
                        drToAdd["PlantStatus"] = plantStatus;
                        drToAdd["ActiveIngredient"] = activeIngredient;
                        drToAdd["EmployeeCode"] = employeeCode;
                        drToAdd["IsolationDays"] = isolationDays;
                        drToAdd["DepartmentID"] = departmentID.HasValue ? (object)departmentID.Value : DBNull.Value;
                        drToAdd["PlantLocation"] = plantLocation;
                        drToAdd["Note"] = note;
                        drToAdd["FertilizationWorkTypeID"] = fertilizationWorkTypeID;
                        drToAdd["MaterialPrice"] = materialPrice.HasValue ? (object)materialPrice.Value : DBNull.Value;
                        drToAdd["EmployeeName"] = employeeName;
                        drToAdd["DepartmentName"] = departmentName;
                        drToAdd["WorkTypeName"] = workTypeName;
                        drToAdd["FertilizationWorkTypeName"] = fertilizationWorkTypeName;
                        drToAdd["MaterialName"] = materialName;
                        drToAdd["MaterialUnit"] = unitName;
                        drToAdd["ProcessDate_Week"] = Utils.GetThu_Viet(processDate);

                        cultivationProcess_dt.Rows.Add(drToAdd);
                        cultivationProcess_dt.AcceptChanges();

                        actionType += "Success";
                        status_lb.Text = "Thành công";
                        status_lb.ForeColor = Color.Green;

                        newBtn_Click(null, null);
                    }
                    else
                    {
                        actionType += "Fail";
                        status_lb.Text = "Thất bại";
                        status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    actionType += "Exception";
                    Console.WriteLine("ERROR: " + ex.Message);
                    status_lb.Text = "Thất bại.";
                    status_lb.ForeColor = Color.Red;
                }
                finally
                {
                    await SQLManager_KhoVatTu.Instance.insertCultivationProcessLogAsync(plantingID, actionType, oldValue, newValue);
                }
            }
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (hinhThucBon_CBB.SelectedValue == null || string.IsNullOrEmpty(hinhThucBon_CBB.Text.Trim()) ||
                congViec_CBB.SelectedValue == null || string.IsNullOrEmpty(congViec_CBB.Text.Trim()) ||
                string.IsNullOrEmpty(materialQuantity_tb.Text.Trim()) ||
                string.IsNullOrEmpty(waterAmount_tb.Text.Trim()) ||
                string.IsNullOrEmpty(isolationDays_tb.Text.Trim())
                )
                return;

            DateTime processDate = processDate_dtp.Value;
            int fertilizationWorkTypeID = Convert.ToInt32(hinhThucBon_CBB.SelectedValue);
            int workTypeID = Convert.ToInt32(congViec_CBB.SelectedValue);
            int? materialID = (string.IsNullOrEmpty(vatTu_CB.Text) || vatTu_CB.SelectedValue == null || vatTu_CB.SelectedValue == DBNull.Value) ? (int?)null : Convert.ToInt32(vatTu_CB.SelectedValue);
            decimal materialQuantity = Utils.ParseDecimalSmart(materialQuantity_tb.Text);
            string dosage = dosage_tb.Text;
            string plantStatus= plantStatus_tb.Text;
            string employeeCode = Convert.ToString(employee_CBB.SelectedValue);
            int isolationDays = Convert.ToInt32(isolationDays_tb.Text);
            int? departmentID = (string.IsNullOrEmpty(department_CBB.Text) || department_CBB.SelectedValue == null || department_CBB.SelectedValue == DBNull.Value) ? (int?)null : Convert.ToInt32(department_CBB.SelectedValue);
            string plantLocation = plantLocation_tb.Text;
            decimal waterAmount = Utils.ParseDecimalSmart(waterAmount_tb.Text);
            string note = note_tb.Text;

            if (id_tb.Text.Length != 0)
                nktd_updateItem(Convert.ToInt32(id_tb.Text), processDate, fertilizationWorkTypeID, workTypeID, materialID, materialQuantity, dosage, plantStatus,
                            employeeCode, isolationDays, departmentID, plantLocation, waterAmount, note);
            else
                nktd_createItem(processDate, fertilizationWorkTypeID, workTypeID, materialID, materialQuantity, dosage, plantStatus, 
                            employeeCode, isolationDays, departmentID, plantLocation, waterAmount, note);

        }

        private async void nktd_deleteProduct(string id)
        {
            DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
                return;

            if (plantingGV.SelectedRows.Count != 1)
            {
                MessageBox.Show("Chỉ chọn 1 lệnh sản xuất", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int plantingID = Convert.ToInt32(plantingGV.SelectedRows[0].Cells["PlantingID"].Value);
            var cultivationProcess_dt = await SQLStore_KhoVatTu.Instance.GetCultivationProcessAsync(plantingID);

            foreach (DataRow row in cultivationProcess_dt.Rows)
            {
                string cultivationProcessID = row["CultivationProcessID"].ToString();
                if (cultivationProcessID.CompareTo(id) == 0)
                {
                    string actionType = "Detele ";
                    string oldValue = $"Ngày: {row["ProcessDate"]}; Hình Thức: {row["FertilizationWorkTypeName"]}; Công Việc: {row["WorkTypeName"]}; Vật Tư: {row["MaterialName"]}; S.Lượng VT: {row["MaterialQuantity"]}; Liều Lượng: {row["Dosage"]}; Tình Trạng Cây: {row["PlantStatus"]}; Hoạt Chất: {row["ActiveIngredient"]}; Người TH: {row["EmployeeName"]}; S.Ngày C.Li: {row["IsolationDays"]}; Tổ P.Trách: {row["DepartmentName"]}; V.Trí Trồng: {row["PlantLocation"]}; Lượng Nước: {row["WaterAmount"]}, Ghi Chú: {row["Note"]}";
                    try
                    {
                        bool isScussess = await SQLManager_KhoVatTu.Instance.deletetCultivationProcessAsync(Convert.ToInt32(id));

                        if (isScussess == true)
                        {
                            status_lb.Text = "Thành công.";
                            status_lb.ForeColor = Color.Green;

                            cultivationProcess_dt.Rows.Remove(row);
                            cultivationProcess_dt.AcceptChanges();
                            actionType += "Success";

                        }
                        else
                        {
                            actionType += "Fail";
                            status_lb.Text = "Thất bại.";
                            status_lb.ForeColor = Color.Red;
                        }
                    }
                    catch (Exception ex)
                    {
                        actionType += "Exception";
                        status_lb.Text = "Thất bại.";
                        status_lb.ForeColor = Color.Red;
                    }
                    finally
                    {
                        _ = SQLManager_KhoVatTu.Instance.insertCultivationProcessLogAsync(plantingID, actionType, oldValue, "");
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
            materialQuantity_tb.Enabled = true;
            vatTu_CB.Enabled = true;
            department_CBB.Enabled = true;
            congViec_CBB.Enabled = true;
            processDate_dtp.Enabled = true;
            dosage_tb.Enabled = true;
            plantStatus_tb.Enabled = true;
            employee_CBB.Enabled = true;
            isolationDays_tb.Enabled = true;
            plantLocation_tb.Enabled = true;
            waterAmount_tb.Enabled = true;
            note_tb.Enabled = true;
            hinhThucBon_CBB.Enabled = true;
            processDate_dtp.Focus();

            VatTu_CB_SelectedIndexChanged(null, null);
        }

        private void ReadOnly_btn_Click(object sender, EventArgs e)
        {
            edit_btn.Visible = true;
            newCustomerBtn.Visible = true;
            readOnly_btn.Visible = false;
            LuuThayDoiBtn.Visible = false;
            info_gb.BackColor = Color.DarkGray;
            isNewState = false;
            materialQuantity_tb.Enabled = false;
            vatTu_CB.Enabled = false;
            department_CBB.Enabled = false;
            congViec_CBB.Enabled = false;
            processDate_dtp.Enabled = false;
            dosage_tb.Enabled = false;
            plantStatus_tb.Enabled = false;
            employee_CBB.Enabled = false;
            isolationDays_tb.Enabled = false;
            plantLocation_tb.Enabled = false;
            waterAmount_tb.Enabled = false;
            note_tb.Enabled = false;
            hinhThucBon_CBB.Enabled = false;
            if (dataGV.SelectedRows.Count > 0)
                updateRightUI();
        }

        private void Edit_btn_Click(object sender, EventArgs e)
        {
            materialQuantity_tb.Enabled = true;
            vatTu_CB.Enabled = true;
            edit_btn.Visible = false;
            newCustomerBtn.Visible = false;
            readOnly_btn.Visible = true;
            LuuThayDoiBtn.Visible = true;
            department_CBB.Enabled = true;
            congViec_CBB.Enabled = true;
            info_gb.BackColor = edit_btn.BackColor;
            isNewState = false;
            processDate_dtp.Enabled = true;
            dosage_tb.Enabled = true;
            plantStatus_tb.Enabled = true;
            employee_CBB.Enabled = true;
            isolationDays_tb.Enabled = true;
            plantLocation_tb.Enabled = true;
            waterAmount_tb.Enabled = true;
            note_tb.Enabled = true;
            hinhThucBon_CBB.Enabled = true;
            LuuThayDoiBtn.Text = "Lưu C.Sửa";
        }

        private void updateRightUI()
        {
            congViec_CBB.SelectionLength = 0;
            vatTu_CB.SelectionLength = 0;
            department_CBB.SelectionLength = 0;
            try
            {
                if (isNewState) return;

                if (dataGV.SelectedRows.Count > 0)
                {
                    var cells = dataGV.SelectedRows[0].Cells;

                    DateTime processDate = Convert.ToDateTime(cells["ProcessDate"].Value);
                    int fertilizationWorkTypeID = int.TryParse(cells["FertilizationWorkTypeID"].Value?.ToString(), out int fertilizationWorkType) ? fertilizationWorkType : -1;
                    int workTypeID = int.TryParse(cells["WorkTypeID"].Value?.ToString(), out int workType) ? workType : -1;
                    int materialID = int.TryParse(cells["MaterialID"].Value?.ToString(), out int material) ? material : -1;
                    int departmentID = int.TryParse(cells["DepartmentID"].Value?.ToString(), out int department) ? department : -1;
                    string employeeCode = cells["EmployeeCode"].Value.ToString();

                    if (!vatTu_CB.Items.Cast<object>().Any(i => ((DataRowView)i)["MaterialID"].ToString() == materialID.ToString()))
                    {
                        vatTu_CB.DataSource = mMaterial_dt;
                    }

                    if (!congViec_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["WorkTypeID"].ToString() == workTypeID.ToString()))
                    {
                        congViec_CBB.DataSource = mWorkType_dt;
                    }

                    if (!hinhThucBon_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["WorkTypeID"].ToString() == fertilizationWorkTypeID.ToString()))
                    {
                        hinhThucBon_CBB.DataSource = mWorkType_dt.Copy();
                    }

                    if (!employee_CBB.Items.Cast<object>().Any(i => ((DataRowView)i)["EmployeeCode"].ToString() == employeeCode))
                    {
                        employee_CBB.DataSource = mEmployee_dt;
                    }

                    id_tb.Text = cells["CultivationProcessID"].Value.ToString();
                    processDate_dtp.Value = processDate;
                    hinhThucBon_CBB.SelectedValue = fertilizationWorkTypeID;
                    congViec_CBB.SelectedValue = workTypeID;
                    vatTu_CB.SelectedValue = materialID;
                    materialQuantity_tb.Text = Convert.ToDecimal(cells["MaterialQuantity"].Value).ToString("G29", CultureInfo.InvariantCulture);
                    dosage_tb.Text = cells["Dosage"].Value.ToString();
                    plantStatus_tb.Text = cells["PlantStatus"].Value.ToString();
                    employee_CBB.SelectedValue = employeeCode;
                    isolationDays_tb.Text = cells["IsolationDays"].Value.ToString();
                    department_CBB.SelectedValue = departmentID;
                    plantLocation_tb.Text = cells["PlantLocation"].Value.ToString();
                    waterAmount_tb.Text = Convert.ToDecimal(cells["WaterAmount"].Value).ToString("G29", CultureInfo.InvariantCulture);
                    note_tb.Text = cells["Note"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
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

        private async void LoadDefaultData_btn_Click(object sender, EventArgs e)
        {

            if (plantingGV.SelectedRows.Count != 1)
            {
                MessageBox.Show("Chỉ chọn 1 lệnh sản xuất", "Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cells = plantingGV.SelectedRows[0].Cells;

            DialogResult dialogResult = MessageBox.Show("Tạo Dữ Liệu Mặc Định, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
                return;

            int plantingID = Convert.ToInt32(cells["PlantingID"].Value);
            DateTime plantingDate = Convert.ToDateTime(cells["PlantingDate"].Value);
            int sku = Convert.ToInt32(cells["SKU"].Value);
            int cultivationTypeID = Convert.ToInt32(cells["CultivationTypeID"].Value);
            int? departmentId = cells["Department"].Value == DBNull.Value ? (int?)null : Convert.ToInt32(cells["Department"].Value);
            string employeeCode = cells["Supervisor"].Value.ToString();
            decimal area = Convert.ToDecimal(cells["Area"].Value);
            var filteredRows = mCultivationProcessTemplate_dt.AsEnumerable().Where(r =>r.Field<int>("SKU") == sku &&  r.Field<int>("CultivationTypeID") == cultivationTypeID);

            List<(int PlantingID, DateTime ProcessDate, int? FertilizationWorkTypeID, int WorkTypeID, int? MaterialID, int? MaterialPrice, decimal? MaterialQuantity, decimal? WaterAmount, int? DepartmentID, string EmployeeCode, string dosage, string activeIngredientStr, int? isolationDays)> data = 
                new List<(int, DateTime, int?, int, int?, int?, decimal?, decimal?, int?, string, string, string, int?)>();
           
            foreach(DataRow rowItem in filteredRows)
            {
                string baseDateType = rowItem["BaseDateType"].ToString();
                int daysAfter = Convert.ToInt32(rowItem["DaysAfter"]);
                int? fertilizationWorkTypeID = rowItem["FertilizationWorkTypeID"] == DBNull.Value ? (int?)null : Convert.ToInt32(rowItem["FertilizationWorkTypeID"]);
                int workTypeID = Convert.ToInt32(rowItem["WorkTypeID"]);                
                int ? materialID = rowItem["MaterialID"] == DBNull.Value ? (int?)null : Convert.ToInt32(rowItem["MaterialID"]);
                decimal materialQuantity = Convert.ToDecimal(rowItem["MaterialQuantity"]);
                decimal waterAmount = Convert.ToDecimal(rowItem["WaterAmount"]);
                bool IsMultiplyArea = Convert.ToBoolean(rowItem["IsMultiplyArea"]);

                DateTime processDate = Convert.ToDateTime(cells[baseDateType].Value).AddDays(daysAfter);
                int? departmentIdTemp = departmentId;
                string employeeCodeTemp = employeeCode;
                if (workTypeID == 18) //chuẩn bị giá thể
                {
                    departmentIdTemp = 27;
                    employeeCodeTemp = "VR0359";
                }

                int? materialPrice = null;
                string dosageStr = "";
                string activeIngredientStr = "";
                int? isolationDays = null;
                if (materialID.HasValue)
                {
                    DataRow materiaRow = mMaterial_dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["MaterialID"]) == materialID);
                    if (materiaRow != null)
                    {
                        materialPrice = Convert.ToInt32(materiaRow["MaterialPrice"]);
                        string composition = materiaRow["Composition"] != DBNull.Value ? materiaRow["Composition"].ToString() : "";

                        dosageStr = composition.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                            .FirstOrDefault(l => Utils.RemoveVietnameseSigns(l).ToLower().Contains("lieu luong"))
                                            ?.Split(':')[1]
                                            .Trim();

                        activeIngredientStr = composition?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                                    .FirstOrDefault(l => Utils.RemoveVietnameseSigns(l).ToLower().Contains("hoat chat"))
                                                    ?.Split(':')
                                                    .Skip(1)
                                                    .FirstOrDefault()
                                                    ?.Trim() ?? "";

                        string isolationDaysStr = composition.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                                    .FirstOrDefault(l => Utils.RemoveVietnameseSigns(l).ToLower().Contains("thoi gian canh ly"))
                                                    ?.Split(':')[1]
                                                    .Trim()
                                                    .TrimEnd('.');

                        if (int.TryParse(isolationDaysStr, out int temp))
                            isolationDays = temp;
                    }

                }

                decimal materialQuantity_Erea = materialQuantity * area;
                if (!IsMultiplyArea)
                    materialQuantity_Erea = materialQuantity;

                data.Add((plantingID, processDate, fertilizationWorkTypeID, workTypeID, materialID, materialPrice, materialQuantity_Erea, waterAmount * area, departmentIdTemp, employeeCodeTemp, dosageStr, activeIngredientStr, isolationDays));
            }

            if(data.Count <= 0)
            {
                MessageBox.Show("Không có dữ liệu mẫu", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isSuccess = await SQLManager_KhoVatTu.Instance.InsertCultivationProcessListAsync(data);
            if (!isSuccess)
            {
                MessageBox.Show("Có Lỗi Xảy Ra", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SQLStore_KhoVatTu.Instance.removeCultivationProcess(plantingID);
            ShowData();
        }

        private async void PlantingGV_SelectionChanged(object sender, EventArgs e)
        {
            if (plantingGV.SelectedRows.Count != 1)
            {
                return;
            }

            int plantingID = Convert.ToInt32(plantingGV.SelectedRows[0].Cells["PlantingID"].Value);

            var data = await SQLStore_KhoVatTu.Instance.GetCultivationProcessAsync(plantingID);
            dataGV.DataSource = data;

            LoadDefaultData_btn.Visible = data.Rows.Count <= 0;
        }

        private void Search_tb_TextChanged(object sender, EventArgs e)
        {
            string keyword = Utils.RemoveVietnameseSigns(search_tb.Text.Trim().ToLower()).Replace("'", "''");
            mPlantingRowDV.RowFilter = $"Department = {mDepartmentID} AND IsCompleted = false AND [search_nosign] LIKE '%{keyword}%'";
        }
    }
}
