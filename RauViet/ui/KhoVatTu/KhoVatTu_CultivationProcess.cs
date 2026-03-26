using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using RauViet.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Color = System.Drawing.Color;

namespace RauViet.ui
{
    public partial class KhoVatTu_CultivationProcess : Form
    {
        DataRow mPlantingRow;
        System.Data.DataTable mMaterial_dt, mWorkType_dt, mCultivationProcess_dt, mCultivationProcessTemplate_dt, mEmployee_dt, mDepartment_dt, mPestDiseaseMonitoring_dt, 
            mGrowthStage_dt, mPestDisease_dt, mHarvestSchedule_dt;
        private DataView mLogDV, mQLSBLogDV, mQLTHLog_DV;
        private Timer FertilizationTypeDebounceTimer = new Timer { Interval = 300 };
        private Timer WorkTypeDebounceTimer = new Timer { Interval = 300 };
        private Timer MaterialDebounceTimer = new Timer { Interval = 300 };
        private Timer EmployeeDebounceTimer = new Timer { Interval = 300 };
        private Timer departmentDebounceTimer = new Timer { Interval = 300 };
        private Timer qlsb_PestDiseaseDebounceTimer = new Timer { Interval = 300 };
        private Timer qlsb_ObserverDebounceTimer = new Timer { Interval = 300 };
        private Timer qlsb_DecisionMakerDebounceTimer = new Timer { Interval = 300 };
        private Timer qlsb_GrowthStageDebounceTimer = new Timer { Interval = 300 };
        private Timer qlth_HarvestEmployeeDebounceTimer = new Timer { Interval = 300 };
        private Timer qlth_ReceiveDepartmentDebounceTimer = new Timer { Interval = 300 };
        bool isNewState = false;
        bool qlsb_isNewState = false;
        bool qlth_isNewState = false;
        bool mIsGlobalGap = false;

        private LoadingOverlay loadingOverlay;        
        public KhoVatTu_CultivationProcess(DataRow plantingRow, bool isGlobalGap = false)
        {
            InitializeComponent();
            this.KeyPreview = true;
            mIsGlobalGap = isGlobalGap;
            dataGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGV.MultiSelect = false;

            qlsb_gv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            qlsb_gv.MultiSelect = false;

            qlth_gv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            qlth_gv.MultiSelect = false;

            processDate_dtp.Format = DateTimePickerFormat.Custom;
            processDate_dtp.CustomFormat = "dd/MM/yyyy";

            qlsb_date_dtp.Format = DateTimePickerFormat.Custom;
            qlsb_date_dtp.CustomFormat = "dd/MM/yyyy";

            qlth_date_dtp.Format = DateTimePickerFormat.Custom;
            qlth_date_dtp.CustomFormat = "dd/MM/yyyy";

            status_lb.Text = "";

            string completeStr = Convert.ToBoolean(plantingRow["IsCompleted"]) ? "Đã Đóng" : "Đang Hoạt Động";
            string globalGAPStr = mIsGlobalGap ? "Global GAP" : "";
            plant_lb.Text = $"[{globalGAPStr}]{plantingRow["ProductionOrder"].ToString()} - {plantingRow["PlantName"].ToString()}({plantingRow["CultivationTypeName"].ToString()}) ==> {completeStr}";
            mPlantingRow = plantingRow;


            newCustomerBtn.Click += newBtn_Click;
            LuuThayDoiBtn.Click += saveBtn_Click;
            edit_btn.Click += Edit_btn_Click;
            readOnly_btn.Click += ReadOnly_btn_Click;

            qlsb_create_btn.Click += Qlsb_create_btn_Click; 
            qlsb_Save_btn.Click += Qlsb_Save_btn_Click; 
            qlsb_edit_btn.Click += Qlsb_edit_btn_Click; 
            qlsb_readOnly_btn.Click += Qlsb_readOnly_btn_Click; 

            qlth_create_btn.Click += Qlth_create_btn_Click;
            qlth_Save_btn.Click += Qlth_Save_btn_Click;
            qlth_edit_btn.Click += Qlth_edit_btn_Click; ;
            qlth_readOnly_btn.Click += Qlth_readOnly_btn_Click;

            LoadDefaultData_btn.Click += LoadDefaultData_btn_Click;
            exportExcel_btn.Click += ExportExcel_btn_Click;
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

            qlsb_PestDiseaseDebounceTimer.Tick += Qlsb_PestDiseaseDebounceTimer_Tick;
            qlsb_pestDisease_cbb.TextUpdate += Qlsb_pestDisease_cbb_TextUpdate;

            qlsb_GrowthStageDebounceTimer.Tick += Qlsb_GrowthStageDebounceTimer_Tick;
            qlsb_growthStatus_cbb.TextUpdate += Qlsb_growthStatus_cbb_TextUpdate;

            qlsb_ObserverDebounceTimer.Tick += Qlsb_ObserverDebounceTimer_Tick;
            qlsb_observer_cbb.TextUpdate += Qlsb_observer_cbb_TextUpdate;

            qlsb_DecisionMakerDebounceTimer.Tick += Qlsb_DecisionMakerDebounceTimer_Tick;
            qlsb_decisionMaker_cbb.TextUpdate += Qlsb_decisionMaker_cbb_TextUpdate;

            EmployeeDebounceTimer.Tick += EmployeeDebounceTimer_Tick;
            employee_CBB.TextUpdate += Employee_CBB_TextUpdate;

            qlth_HarvestEmployeeDebounceTimer.Tick += Qlth_HarvestEmployeeDebounceTimer_Tick;
            qlth_HarvestEmployee_cbb.TextUpdate += Qlth_HarvestEmployee_cbb_TextUpdate;

            qlth_ReceiveDepartmentDebounceTimer.Tick += Qlth_ReceiveDepartmentDebounceTimer_Tick;
            qlth_ReceiveDepartment_cbb.TextUpdate += Qlth_ReceiveDepartment_cbb_TextUpdate;

            materialQuantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            waterAmount_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            isolationDays_tb.KeyPress += Tb_KeyPress_OnlyNumber;
            qlth_Quantity_tb.KeyPress += Tb_KeyPress_OnlyNumber;

            if(mIsGlobalGap)
            {
                panel1.Visible = false;
                panel6.Visible = false;
                panel8.Visible = false;
                qlsb_LOG_gv.Visible = false;
                qlth_Log_gv.Visible = false;
                log_GV.Visible = false;
            }
        }

        private void form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);
                if(data_tc.SelectedTab == nhatKyTheoDoi_tp)
                    SQLStore_KhoVatTu.Instance.removeCultivationProcess(plantingID);
                else if(data_tc.SelectedTab == quanLySauBenh_tp)
                    SQLStore_KhoVatTu.Instance.removePestDiseaseMonitoring(plantingID);
                else if (data_tc.SelectedTab == thuHoach_tp)
                    SQLStore_KhoVatTu.Instance.removeHarvestSchedule(plantingID);

                ShowData();
            }
            else if ((!isNewState && !edit_btn.Visible && data_tc.SelectedTab == nhatKyTheoDoi_tp) || 
                (!qlsb_isNewState && !qlsb_edit_btn.Visible && data_tc.SelectedTab == quanLySauBenh_tp) ||
                (!qlth_isNewState && !qlth_edit_btn.Visible && data_tc.SelectedTab == thuHoach_tp))
            {
                if (e.KeyCode == Keys.Delete)
                {
                    System.Windows.Forms.Control ctrl = this.ActiveControl;

                    if (ctrl is System.Windows.Controls.TextBox || ctrl is RichTextBox ||
                        (ctrl is DataGridView dgv && dgv.CurrentCell != null && dgv.IsCurrentCellInEditMode))
                    {
                        return; // không xử lý Delete
                    }

                    if((!isNewState && !edit_btn.Visible && data_tc.SelectedTab == nhatKyTheoDoi_tp))
                        nktd_deleteProduct(id_tb.Text);
                    else if((!qlsb_isNewState && !qlsb_edit_btn.Visible && data_tc.SelectedTab == quanLySauBenh_tp))
                        qlsb_deleteProduct(qlsb_ID_tb.Text);
                    else if ((!qlth_isNewState && !qlth_edit_btn.Visible && data_tc.SelectedTab == thuHoach_tp))
                        qlth_deleteProduct(qlth_ID_tb.Text);
                }
            }
        }

        public async void ShowData()
        {
            dataGV.SelectionChanged -= this.dataGV_CellClick;
            qlsb_gv.SelectionChanged -= this.qlsb_gv_CellClick;
            qlth_gv.SelectionChanged -= this.qlth_gv_CellClick;
            await Task.Delay(50);
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Show();

            try
            {
                int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);

                if(!mIsGlobalGap)
                    mHarvestSchedule_dt = await SQLStore_KhoVatTu.Instance.GetHarvestScheduleAsync(plantingID);
                else
                    mHarvestSchedule_dt = await SQLStore_KhoVatTu.Instance.GetHarvestScheduleGlobalGAPAsync(plantingID);

                string[] empKeepColumns = { "EmployeeCode", "FullName", "EmployessName_NoSign", "DepartmentID", "PositionID" };
                var cultivationProcessTask = SQLStore_KhoVatTu.Instance.GetCultivationProcessAsync(plantingID);
                var pestDiseaseMonitoringTask = SQLStore_KhoVatTu.Instance.GetPestDiseaseMonitoringAsync(plantingID);
                
                var logDataTask = SQLStore_KhoVatTu.Instance.GetCultivationProcessLogAsync(plantingID);
                var QLSBlogDataTask = SQLStore_KhoVatTu.Instance.GetPestDiseaseMonitoringLogAsync(plantingID);
                var QLTHlogDataTask = SQLStore_KhoVatTu.Instance.GetHarvestScheduleLogAsync(plantingID);

                var cultivationProcessTemplateTask = SQLStore_KhoVatTu.Instance.GetCultivationProcessTemplateAsync();
                var materialTask = SQLStore_KhoVatTu.Instance.getMaterialAsync();
                var workTypeTask = SQLStore_KhoVatTu.Instance.GetWorkTypeAsync();
                var departmentTask = SQLStore_QLNS.Instance.GetDepartmentAsync();
                var growthStageTask = SQLStore_KhoVatTu.Instance.GetGrowthStageAsync();
                var pestDiseaseTask = SQLStore_KhoVatTu.Instance.GetPestDiseaseAsync();
                var employeeTask = SQLStore_QLNS.Instance.GetEmployeesAsync(empKeepColumns);

                await Task.WhenAll(cultivationProcessTask, pestDiseaseMonitoringTask, materialTask, workTypeTask, cultivationProcessTemplateTask, employeeTask, 
                    departmentTask, logDataTask, growthStageTask, pestDiseaseTask, QLSBlogDataTask, QLTHlogDataTask);

                if (mIsGlobalGap)
                {
                    mCultivationProcess_dt = cultivationProcessTask.Result.Copy();
                    mPestDiseaseMonitoring_dt = pestDiseaseMonitoringTask.Result.Copy();
                }
                else
                {
                    mCultivationProcess_dt = cultivationProcessTask.Result;
                    mPestDiseaseMonitoring_dt = pestDiseaseMonitoringTask.Result;
                }

                
                mMaterial_dt = materialTask.Result;
                mWorkType_dt = workTypeTask.Result;
                mCultivationProcessTemplate_dt = cultivationProcessTemplateTask.Result;
                mEmployee_dt = employeeTask.Result;
                mDepartment_dt = departmentTask.Result;
                mGrowthStage_dt = growthStageTask.Result;
                mPestDisease_dt = pestDiseaseTask.Result;
                mLogDV = new DataView(logDataTask.Result);
                mQLSBLogDV = new DataView(QLSBlogDataTask.Result);
                mQLTHLog_DV = new DataView(QLTHlogDataTask.Result);


                if(mIsGlobalGap)
                {
                    var thuHoachRows = mCultivationProcess_dt.AsEnumerable().Where(r => r.Field<int>("WorkTypeID") == 20).ToList();
                    if (thuHoachRows.Any())
                    {
                        foreach (DataRow rowItem in mHarvestSchedule_dt.Rows)
                        {
                            DataRow newRow = mCultivationProcess_dt.NewRow();
                            newRow.ItemArray = thuHoachRows[0].ItemArray;
                            newRow["MaterialQuantity"] = rowItem["Quantity"];

                            mCultivationProcess_dt.Rows.Add(newRow);
                        }

                        foreach (DataRow row in thuHoachRows)
                        {
                            mCultivationProcess_dt.Rows.Remove(row);
                        }
                    }

                    var tbRows = mCultivationProcess_dt.AsEnumerable().Where(r => r.Field<string>("CategoryCode") == "TB").ToList();
                    foreach (var row in tbRows)
                    {
                        mCultivationProcess_dt.Rows.Remove(row);
                    }
                }

                qlsb_growthStatus_cbb.DataSource = mGrowthStage_dt;
                qlsb_growthStatus_cbb.DisplayMember = "GrowthStageName";  // hiển thị tên
                qlsb_growthStatus_cbb.ValueMember = "GrowthStageID";
                qlsb_growthStatus_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                qlsb_pestDisease_cbb.DataSource = mPestDisease_dt;
                qlsb_pestDisease_cbb.DisplayMember = "PestDiseaseName";  // hiển thị tên
                qlsb_pestDisease_cbb.ValueMember = "PestDiseaseID";
                qlsb_pestDisease_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                qlsb_observer_cbb.DataSource = mEmployee_dt.Copy();
                qlsb_observer_cbb.DisplayMember = "FullName";  // hiển thị tên
                qlsb_observer_cbb.ValueMember = "EmployeeCode";
                qlsb_observer_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                qlsb_decisionMaker_cbb.DataSource = mEmployee_dt.Copy();
                qlsb_decisionMaker_cbb.DisplayMember = "FullName";  // hiển thị tên
                qlsb_decisionMaker_cbb.ValueMember = "EmployeeCode";
                qlsb_decisionMaker_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

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

                qlth_HarvestEmployee_cbb.DataSource = mEmployee_dt.Copy();
                qlth_HarvestEmployee_cbb.DisplayMember = "FullName";  // hiển thị tên
                qlth_HarvestEmployee_cbb.ValueMember = "EmployeeCode";
                qlth_HarvestEmployee_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                qlth_ReceiveDepartment_cbb.DataSource = mDepartment_dt.Copy();
                qlth_ReceiveDepartment_cbb.DisplayMember = "DepartmentName";  // hiển thị tên
                qlth_ReceiveDepartment_cbb.ValueMember = "DepartmentID";
                qlth_ReceiveDepartment_cbb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                dataGV.DataSource = mCultivationProcess_dt;
                qlsb_gv.DataSource = mPestDiseaseMonitoring_dt;
                qlth_gv.DataSource = mHarvestSchedule_dt;
                
                log_GV.DataSource = mLogDV;
                qlsb_LOG_gv.DataSource = mQLSBLogDV;
                qlth_Log_gv.DataSource = mQLTHLog_DV;

                LoadDefaultData_btn.Visible = mCultivationProcess_dt.Rows.Count <= 0;
                
                Utils.HideColumns(dataGV, new[] { "CultivationProcessID", "PlantingID", "MaterialID", "WorkTypeID", "WorkTypeID", "MaterialID" , "EmployeeCode" , "DepartmentID", "FertilizationWorkTypeID", "CategoryCode" });
                Utils.SetGridOrdinal(mCultivationProcess_dt, new[] { "ProcessDate", "ProcessDate_Week", "FertilizationWorkTypeName", "WorkTypeName", "MaterialName", "MaterialQuantity", "MaterialUnit", "Dosage", "PlantStatus", "EmployeeName", "IsolationDays", "IsolationEndDate", "DepartmentName", "PlantLocation", "WaterAmount", "MaterialPrice", "TotalMaterialCost", "Note" });
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


                Utils.HideColumns(qlsb_gv, new[] { "MonitoringID", "PlantingID", "GrowthStageID", "Observer", "DecisionMaker", "PestDiseaseID" });
                Utils.SetGridOrdinal(mPestDiseaseMonitoring_dt, new[] { "MonitoringDate", "MonitoringDate_Week", "Location", "GrowthStageName", "PestDiseaseName", "CurrentStatus", "ObserverName", "TreatmentPlan", "DecisionMakerName"});
                Utils.SetGridHeaders(qlsb_gv, new System.Collections.Generic.Dictionary<string, string> {
                        {"MonitoringDate", "Ngày" },
                        {"MonitoringDate_Week", "Thứ" },
                        {"Location", "Vị Trí Mã Lô" },
                        {"GrowthStageName", "Giai Đoạn Sinh Trưởng" },
                        {"PestDiseaseName", "Tên Sâu Bệnh" },
                        {"CurrentStatus", "Hiện Trạng và Mật Độ Sâu Bệnh" },
                        {"ObserverName", "Người Theo Dõi" },
                        {"TreatmentPlan", "Phương Án Xử Lý và sử dụng thuốc BVTV" },
                        {"DecisionMakerName", "Người Quyết Định Phương Án Xử Lý" }
                    });
                Utils.SetGridWidths(qlsb_gv, new System.Collections.Generic.Dictionary<string, int> {
                        {"MonitoringDate", 70 },
                        {"MonitoringDate_Week", 50 },
                        {"Location", 80 },
                        {"GrowthStageName", 200 },
                        {"PestDiseaseName", 120 },
                        {"CurrentStatus", 200 },
                        {"ObserverName", 150},
                        {"TreatmentPlan", 200 },
                        {"DecisionMakerName", 150 }
                    });

                Utils.HideColumns(qlth_gv, new[] { "HarvestGlobalGapID", "SupervisorEmployee", "HarvestID", "PlantingID", "HarvestEmployee", "ReceiveDepartmentID"});
                Utils.SetGridOrdinal(mHarvestSchedule_dt, new[] { "HarvestDate", "HarvestDate_Week", "Quantity", "ProductLotCode", "SupervisorName", "HarvestEmployeeName", "ReceiveDepartmentName" });
                Utils.SetGridHeaders(qlth_gv, new System.Collections.Generic.Dictionary<string, string> {
                        {"HarvestDate", "Ngày" },
                        {"HarvestDate_Week", "Thứ" },
                        {"Quantity", "Số Lượng (Kg)" },
                        {"ProductLotCode", "Mã Lô SP" },
                        {"HarvestEmployeeName", "Người Thu Hoạch" },
                        {"ReceiveDepartmentName", "Nơi Nhận SP" }
                    });
                Utils.SetGridWidths(qlth_gv, new System.Collections.Generic.Dictionary<string, int> {
                        {"HarvestDate", 70 },
                        {"HarvestDate_Week", 50 },
                        {"Quantity", 80 },
                        {"ProductLotCode", 100 },
                        {"HarvestEmployeeName", 180 },
                        {"ReceiveDepartmentName", 200 }
                    });

                Utils.HideColumns(log_GV, new[] { "LogID", "PlantingID" });
                Utils.SetGridHeaders(log_GV, new System.Collections.Generic.Dictionary<string, string> {
                        {"ActionType", "Hành Động" },
                        {"OldValue", "Cũ" },
                        {"NewValue", "Mới" },
                        {"CreatedDate", "Ngày tạo" },
                        {"ActionBy", "Người tạo" }
                    });
                Utils.SetGridWidths(log_GV, new System.Collections.Generic.Dictionary<string, int> {
                        {"OldValue", 350 },
                        {"NewValue", 350 }
                    });

                Utils.HideColumns(qlsb_LOG_gv, new[] { "LogID", "PlantingID" });
                Utils.SetGridHeaders(qlsb_LOG_gv, new System.Collections.Generic.Dictionary<string, string> {
                        {"ActionType", "Hành Động" },
                        {"OldValue", "Cũ" },
                        {"NewValue", "Mới" },
                        {"CreatedDate", "Ngày tạo" },
                        {"ActionBy", "Người tạo" }
                    });
                Utils.SetGridWidths(qlsb_LOG_gv, new System.Collections.Generic.Dictionary<string, int> {
                        {"OldValue", 350 },
                        {"NewValue", 350 }
                    });

                Utils.HideColumns(qlth_Log_gv, new[] { "LogID", "PlantingID" });
                Utils.SetGridHeaders(qlth_Log_gv, new System.Collections.Generic.Dictionary<string, string> {
                        {"ActionType", "Hành Động" },
                        {"OldValue", "Cũ" },
                        {"NewValue", "Mới" },
                        {"CreatedDate", "Ngày tạo" },
                        {"ActionBy", "Người tạo" }
                    });
                Utils.SetGridWidths(qlth_Log_gv, new System.Collections.Generic.Dictionary<string, int> {
                        {"OldValue", 350 },
                        {"NewValue", 350 }
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

                qlsb_date_dtp.TabIndex = countTab++; qlsb_date_dtp.TabStop = true;
                qlsb_location_tb.TabIndex = countTab++; qlsb_location_tb.TabStop = true;
                qlsb_growthStatus_cbb.TabIndex = countTab++; qlsb_growthStatus_cbb.TabStop = true;
                qlsb_pestDisease_cbb.TabIndex = countTab++; qlsb_pestDisease_cbb.TabStop = true;
                qlsb_currentStatus_tb.TabIndex = countTab++; qlsb_currentStatus_tb.TabStop = true;
                qlsb_observer_cbb.TabIndex = countTab++; qlsb_observer_cbb.TabStop = true;
                qlsb_treatmentPlan_tb.TabIndex = countTab++; qlsb_treatmentPlan_tb.TabStop = true;
                qlsb_decisionMaker_cbb.TabIndex = countTab++; qlsb_decisionMaker_cbb.TabStop = true;
                qlsb_Save_btn.TabIndex = countTab++; qlsb_Save_btn.TabStop = true;

                qlth_date_dtp.TabIndex = countTab++; qlth_date_dtp.TabStop = true;
                qlth_Quantity_tb.TabIndex = countTab++; qlth_Quantity_tb.TabStop = true;
                qlth_ProductLotCode_tb.TabIndex = countTab++; qlth_ProductLotCode_tb.TabStop = true;
                qlth_HarvestEmployee_cbb.TabIndex = countTab++; qlth_HarvestEmployee_cbb.TabStop = true;
                qlth_ReceiveDepartment_cbb.TabIndex = countTab++; qlth_ReceiveDepartment_cbb.TabStop = true;
                qlth_Save_btn.TabIndex = countTab++; qlth_Save_btn.TabStop = true;

                dataGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                log_GV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                qlsb_gv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                ReadOnly_btn_Click(null, null);
                Qlsb_readOnly_btn_Click(null, null);
                Qlth_readOnly_btn_Click(null, null);
                dataGV.SelectionChanged += this.dataGV_CellClick;
                qlsb_gv.SelectionChanged += this.qlsb_gv_CellClick;
                qlth_gv.SelectionChanged += this.qlth_gv_CellClick;
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

        private void Qlsb_observer_cbb_TextUpdate(object sender, EventArgs e)
        {
            qlsb_ObserverDebounceTimer.Stop();
            qlsb_ObserverDebounceTimer.Start();
        }

        private void Qlsb_ObserverDebounceTimer_Tick(object sender, EventArgs e)
        {
            qlsb_ObserverDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(qlsb_observer_cbb, mEmployee_dt, "EmployessName_NoSign");
        }

        private void Qlsb_decisionMaker_cbb_TextUpdate(object sender, EventArgs e)
        {
            qlsb_DecisionMakerDebounceTimer.Stop();
            qlsb_DecisionMakerDebounceTimer.Start();
        }

        private void Qlsb_DecisionMakerDebounceTimer_Tick(object sender, EventArgs e)
        {
            qlsb_DecisionMakerDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(qlsb_decisionMaker_cbb, mEmployee_dt, "EmployessName_NoSign");
        }

        private void Qlsb_pestDisease_cbb_TextUpdate(object sender, EventArgs e)
        {
            qlsb_PestDiseaseDebounceTimer.Stop();
            qlsb_PestDiseaseDebounceTimer.Start();
        }

        private void Qlsb_PestDiseaseDebounceTimer_Tick(object sender, EventArgs e)
        {
            qlsb_PestDiseaseDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(qlsb_pestDisease_cbb, mPestDisease_dt, "search_nosign");            
        }

        private void Qlsb_growthStatus_cbb_TextUpdate(object sender, EventArgs e)
        {
            qlsb_GrowthStageDebounceTimer.Stop();
            qlsb_GrowthStageDebounceTimer.Start();
        }

        private void Qlsb_GrowthStageDebounceTimer_Tick(object sender, EventArgs e)
        {
            qlsb_GrowthStageDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(qlsb_growthStatus_cbb, mGrowthStage_dt, "search_nosign");
        }

        private void Qlth_HarvestEmployee_cbb_TextUpdate(object sender, EventArgs e)
        {
            qlth_HarvestEmployeeDebounceTimer.Stop();
            qlth_HarvestEmployeeDebounceTimer.Start();
        }

        private void Qlth_HarvestEmployeeDebounceTimer_Tick(object sender, EventArgs e)
        {
            qlth_HarvestEmployeeDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(qlth_HarvestEmployee_cbb, mEmployee_dt, "EmployessName_NoSign");
        }

        private void Qlth_ReceiveDepartment_cbb_TextUpdate(object sender, EventArgs e)
        {
            qlth_ReceiveDepartmentDebounceTimer.Stop();
            qlth_ReceiveDepartmentDebounceTimer.Start();
        }

        private void Qlth_ReceiveDepartmentDebounceTimer_Tick(object sender, EventArgs e)
        {
            qlth_ReceiveDepartmentDebounceTimer.Stop();
            Utils.ComboBoxSearchResult(qlth_ReceiveDepartment_cbb, mDepartment_dt, "DepartmentName", false);
        }

        private void dataGV_CellClick(object sender, EventArgs e)
        {
            updateRightUI();            
        }
        private void qlsb_gv_CellClick(object sender, EventArgs e)
        {
            qlsb_updateRightUI();
        }

        private void qlth_gv_CellClick(object sender, EventArgs e)
        {
            qlth_updateRightUI();
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
            DataRow[] fertilizationWorkRows = mWorkType_dt.Select($"WorkTypeID = {fertilizationWorkTypeID}");
            DataRow[] workTypeRows = mWorkType_dt.Select($"WorkTypeID = {workTypeID}");
            DataRow[] matiralRows = Array.Empty<DataRow>();
            if (materialID.HasValue)
                matiralRows = mMaterial_dt.Select($"MaterialID = {materialID}");

            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");

            DataRow[] departmentRows = Array.Empty<DataRow>();
            if (departmentID.HasValue)
                departmentRows = mDepartment_dt.Select($"DepartmentID = {departmentID}");

            foreach (DataRow row in mCultivationProcess_dt.Rows)
            {
                int ID = Convert.ToInt32(row["CultivationProcessID"]);
                if (ID.CompareTo(cultivationProcessID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        string workTypeName = workTypeRows.Length > 0 ? workTypeRows[0]["WorkTypeName"].ToString() : "";
                        string fertilizationWorkTypeName = fertilizationWorkRows.Length > 0 ? workTypeRows[0]["WorkTypeName"].ToString() : "";
                        string employeeName = employeeRows.Length > 0 ? employeeRows[0]["FullName"].ToString() : "";
                        string departmentName = departmentRows.Length > 0 ? departmentRows[0]["DepartmentName"].ToString() : "";
                        string materialName = matiralRows.Length > 0 ? matiralRows[0]["MaterialName"].ToString() : "";
                        string unitName = matiralRows.Length > 0 ? matiralRows[0]["UnitName"].ToString() : "";
                        if(unitName.CompareTo("Bao") == 0)
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

                        int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);
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
            string note, bool isAskQuestion = true)
        {
            DataRow[] fertilizationWorkRows = mWorkType_dt.Select($"WorkTypeID = {fertilizationWorkTypeID}");
            DataRow[] workTypeRows = mWorkType_dt.Select($"WorkTypeID = {workTypeID}");
            DataRow[] matiralRows = Array.Empty<DataRow>();
            if (materialID.HasValue)
                matiralRows = mMaterial_dt.Select($"MaterialID = {materialID}");

            DataRow[] employeeRows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");

            DataRow[] departmentRows = Array.Empty<DataRow>();
            if (departmentID.HasValue)
                departmentRows = mDepartment_dt.Select($"DepartmentID = {departmentID}");

            DialogResult dialogResult = DialogResult.No;
            if (isAskQuestion)
                dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes || isAskQuestion == false)
            {
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

                int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);
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
                        DataRow drToAdd = mCultivationProcess_dt.NewRow();

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

                        mCultivationProcess_dt.Rows.Add(drToAdd);
                        mCultivationProcess_dt.AcceptChanges();

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
                    _ = SQLManager_KhoVatTu.Instance.insertCultivationProcessLogAsync(plantingID, actionType, oldValue, newValue);
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

        private async void qlsb_updateItem(int monitoringID, DateTime processDate, string location, int? growwthStatusID, int? pestDiseaseID, string currentStatus, string observerCode, string treatmentPlan, string decisionMakerCode)
        {
            DataRow[] observerRows = mEmployee_dt.Select($"EmployeeCode = '{observerCode}'");
            DataRow[] decisionMakerRows = mEmployee_dt.Select($"EmployeeCode = '{decisionMakerCode}'");

            DataRow[] growthStageRows = Array.Empty<DataRow>();
            if (growwthStatusID.HasValue)
                growthStageRows = mGrowthStage_dt.Select($"GrowthStageID = {growwthStatusID}");

            DataRow[] pestDiseaseRows = Array.Empty<DataRow>();
            if (pestDiseaseID.HasValue)
                pestDiseaseRows = mPestDisease_dt.Select($"PestDiseaseID = {pestDiseaseID}");

            foreach (DataRow row in mPestDiseaseMonitoring_dt.Rows)
            {
                int ID = Convert.ToInt32(row["MonitoringID"]);
                if (ID.CompareTo(monitoringID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        string growthStageName = growthStageRows.Length > 0 ? growthStageRows[0]["GrowthStageName"].ToString() : "";
                        string pestDiseaseName = pestDiseaseRows.Length > 0 ? pestDiseaseRows[0]["PestDiseaseName"].ToString() : "";
                        string observerName = observerRows.Length > 0 ? observerRows[0]["FullName"].ToString() : "";
                        string decisionMakerName = decisionMakerRows.Length > 0 ? decisionMakerRows[0]["FullName"].ToString() : "";

                        string actionType = "Update ";
                        string oldValue = $"Ngày: {row["MonitoringDate"]};Vị Trí Mã Lô: {row["Location"]}; Giai Đoạn Sinh Trưởng: {row["GrowthStageName"]}; Tên Sâu Bệnh: {row["PestDiseaseName"]}; Hiện Trạng: {row["CurrentStatus"]}; Người Theo Dõi: {row["ObserverName"]}; Phương Án Xử Lý: {row["PestDiseaseName"]}; Người Quyết Định: {row["DecisionMakerName"]};";
                        string newValue = $"Ngày: {processDate};Vị Trí Mã Lô: {location}; Giai Đoạn Sinh Trưởng: {growthStageName}; Tên Sâu Bệnh: {pestDiseaseName}; Hiện Trạng: {currentStatus}; Người Theo Dõi: {growthStageName}; Phương Án Xử Lý: {treatmentPlan}; Người Quyết Định: {pestDiseaseName};";

                        int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.updatePestDiseaseMonitoringAsync(plantingID, monitoringID, processDate, location, growwthStatusID, pestDiseaseID, currentStatus, observerCode, treatmentPlan, decisionMakerCode);

                            if (isScussess == true)
                            {
                                row["PlantingID"] = plantingID;
                                row["MonitoringDate"] = processDate;
                                row["Location"] = location;
                                row["GrowthStageID"] = growwthStatusID.HasValue ? (object)growwthStatusID.Value : DBNull.Value;
                                row["PestDiseaseID"] = pestDiseaseID.HasValue ? (object)pestDiseaseID.Value : DBNull.Value;
                                row["CurrentStatus"] = currentStatus;
                                row["Observer"] = observerCode;
                                row["TreatmentPlan"] = treatmentPlan;
                                row["DecisionMaker"] = decisionMakerCode;

                                row["ObserverName"] = observerName;
                                row["DecisionMakerName"] = decisionMakerName;
                                row["GrowthStageName"] = growthStageName;
                                row["MonitoringDate_Week"] = Utils.GetThu_Viet(processDate);
                                row["PestDiseaseName"] = pestDiseaseName;

                                actionType += "Success";
                                qlsb_status_lb.Text = "Thành công.";
                                qlsb_status_lb.ForeColor = Color.Green;
                            }
                            else
                            {
                                actionType += "Fail";
                                qlsb_status_lb.Text = "Thất bại.";
                                qlsb_status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch
                        {
                            actionType += "Exception";
                            qlsb_status_lb.Text = "Thất bại.";
                            qlsb_status_lb.ForeColor = Color.Red;
                        }
                        finally
                        {
                            _ = SQLManager_KhoVatTu.Instance.insertPestDiseaseMonitoringLogAsync(plantingID, actionType, oldValue, newValue);
                        }
                    }
                    break;
                }
            }
        }

        private async Task qlsb_createItem(DateTime processDate, string location, int? growwthStatusID, int? pestDiseaseID, string currentStatus, string observerCode, string treatmentPlan, string decisionMakerCode, bool isAskQuestion = true)
        {
            DataRow[] observerRows = mEmployee_dt.Select($"EmployeeCode = '{observerCode}'");
            DataRow[] decisionMakerRows = mEmployee_dt.Select($"EmployeeCode = '{decisionMakerCode}'");

            DataRow[] growthStageRows = Array.Empty<DataRow>();
            if (growwthStatusID.HasValue)
                growthStageRows = mGrowthStage_dt.Select($"GrowthStageID = {growwthStatusID}");

            DataRow[] pestDiseaseRows = Array.Empty<DataRow>();
            if (pestDiseaseID.HasValue)
                pestDiseaseRows = mPestDisease_dt.Select($"PestDiseaseID = {pestDiseaseID}");

            DialogResult dialogResult = DialogResult.Yes;
            if(isAskQuestion)
                dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string growthStageName = growthStageRows.Length > 0 ? growthStageRows[0]["GrowthStageName"].ToString() : "";
                string pestDiseaseName = pestDiseaseRows.Length > 0 ? pestDiseaseRows[0]["PestDiseaseName"].ToString() : "";
                string observerName = observerRows.Length > 0 ? observerRows[0]["FullName"].ToString() : "";
                string decisionMakerName = decisionMakerRows.Length > 0 ? decisionMakerRows[0]["FullName"].ToString() : "";

                string actionType = "Create ";
                string oldValue = "";
                string newValue = $"Ngày: {processDate};Vị Trí Mã Lô: {location}; Giai Đoạn Sinh Trưởng: {growthStageName}; Tên Sâu Bệnh: {pestDiseaseName}; Hiện Trạng: {currentStatus}; Người Theo Dõi: {growthStageName}; Phương Án Xử Lý: {treatmentPlan}; Người Quyết Định: {pestDiseaseName};";

                int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);

                try
                {
                    int newId = await SQLManager_KhoVatTu.Instance.insertPestDiseaseMonitoringAsync(plantingID, processDate, location, growwthStatusID, pestDiseaseID, currentStatus, observerCode, treatmentPlan, decisionMakerCode);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mPestDiseaseMonitoring_dt.NewRow();

                        drToAdd["MonitoringID"] = newId;
                        drToAdd["PlantingID"] = plantingID;
                        drToAdd["MonitoringDate"] = processDate;
                        drToAdd["Location"] = location;
                        drToAdd["GrowthStageID"] = growwthStatusID.HasValue ? (object)growwthStatusID.Value : DBNull.Value;
                        drToAdd["PestDiseaseID"] = pestDiseaseID.HasValue ? (object)pestDiseaseID.Value : DBNull.Value;
                        drToAdd["CurrentStatus"] = currentStatus;
                        drToAdd["Observer"] = observerCode;
                        drToAdd["TreatmentPlan"] = treatmentPlan;
                        drToAdd["DecisionMaker"] = decisionMakerCode;

                        drToAdd["ObserverName"] = observerName;
                        drToAdd["DecisionMakerName"] = decisionMakerName;
                        drToAdd["GrowthStageName"] = growthStageName;
                        drToAdd["MonitoringDate_Week"] = Utils.GetThu_Viet(processDate);
                        drToAdd["PestDiseaseName"] = pestDiseaseName;

                        mPestDiseaseMonitoring_dt.Rows.Add(drToAdd);
                        mPestDiseaseMonitoring_dt.AcceptChanges();

                        actionType += "Success";
                        qlsb_status_lb.Text = "Thành công";
                        qlsb_status_lb.ForeColor = Color.Green;

                        Qlsb_create_btn_Click(null, null);
                    }
                    else
                    {
                        actionType += "Fail";
                        qlsb_status_lb.Text = "Thất bại";
                        qlsb_status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    actionType += "Exception";
                    Console.WriteLine("ERROR: " + ex.Message);
                    qlsb_status_lb.Text = "Thất bại.";
                    qlsb_status_lb.ForeColor = Color.Red;
                }
                finally
                {
                    _ = SQLManager_KhoVatTu.Instance.insertPestDiseaseMonitoringLogAsync(plantingID, actionType, oldValue, newValue);
                }
            }
        }
        private void Qlsb_Save_btn_Click(object sender, EventArgs e)
        {
            DateTime processDate = qlsb_date_dtp.Value;
            string location = qlsb_location_tb.Text;
            int? growwthStatusID = (string.IsNullOrEmpty(qlsb_growthStatus_cbb.Text) || qlsb_growthStatus_cbb.SelectedValue == null || qlsb_growthStatus_cbb.SelectedValue == DBNull.Value) ? (int?)null : Convert.ToInt32(qlsb_growthStatus_cbb.SelectedValue);
            int? pestDiseaseID = (string.IsNullOrEmpty(qlsb_pestDisease_cbb.Text) || qlsb_pestDisease_cbb.SelectedValue == null || qlsb_pestDisease_cbb.SelectedValue == DBNull.Value) ? (int?)null : Convert.ToInt32(qlsb_pestDisease_cbb.SelectedValue);
            string currentStatus = qlsb_currentStatus_tb.Text;
            string observerCode = (string.IsNullOrEmpty(qlsb_observer_cbb.Text) || qlsb_observer_cbb.SelectedValue == null || qlsb_observer_cbb.SelectedValue == DBNull.Value) ? "" : Convert.ToString(qlsb_observer_cbb.SelectedValue);
            string treatmentPlan = qlsb_treatmentPlan_tb.Text;
            string decisionMakerCode = (string.IsNullOrEmpty(qlsb_decisionMaker_cbb.Text) || qlsb_decisionMaker_cbb.SelectedValue == null || qlsb_decisionMaker_cbb.SelectedValue == DBNull.Value) ? "" : Convert.ToString(qlsb_decisionMaker_cbb.SelectedValue);

            if (qlsb_ID_tb.Text.Length != 0)
                qlsb_updateItem(Convert.ToInt32(qlsb_ID_tb.Text), processDate, location, growwthStatusID, pestDiseaseID, currentStatus, observerCode, treatmentPlan, decisionMakerCode);
            else
                qlsb_createItem(processDate, location, growwthStatusID, pestDiseaseID, currentStatus, observerCode, treatmentPlan, decisionMakerCode);
        }
                
        private async void qlth_updateItem(int harvestID, DateTime harvestDate, decimal quantity, string productLotCode, string harvestEmployeeCode, int? receiveDepartmentID)
        {
            DataRow[] harvestEmployeeRows = mEmployee_dt.Select($"EmployeeCode = '{harvestEmployeeCode}'");

            DataRow[] departmentRows = Array.Empty<DataRow>();
            if (receiveDepartmentID.HasValue)
                departmentRows = mDepartment_dt.Select($"DepartmentID = {receiveDepartmentID}");

            foreach (DataRow row in mHarvestSchedule_dt.Rows)
            {
                int ID = Convert.ToInt32(row["HarvestID"]);
                if (ID.CompareTo(harvestID) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        string departmentName = departmentRows.Length > 0 ? departmentRows[0]["DepartmentName"].ToString() : "";
                        string harvestEmployeerName = harvestEmployeeRows.Length > 0 ? harvestEmployeeRows[0]["FullName"].ToString() : "";

                        string actionType = "Update ";
                        string oldValue = $"Ngày: {row["HarvestDate"]};Số Lượng Thu: {row["Quantity"]}; Mã Số Thu (LOT): {row["ProductLotCode"]}; Nhân Viên Thu: {row["HarvestEmployeeName"]}; Phòng Nhận: {row["ReceiveDepartmentName"]}";
                        string newValue = $"Ngày: {harvestDate};Số Lượng Thu: {quantity}; Mã Số Thu (LOT): {productLotCode}; Nhân Viên Thu: {harvestEmployeerName}; Phòng Nhận: {departmentName}";

                        int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.updatePestDiseaseMonitoringAsync(harvestID, plantingID, harvestDate, quantity, productLotCode, harvestEmployeeCode, receiveDepartmentID);

                            if (isScussess == true)
                            {
                                row["HarvestDate"] = harvestDate;
                                row["Quantity"] = quantity;
                                row["ProductLotCode"] = productLotCode;
                                row["HarvestEmployee"] = harvestEmployeeCode;
                                row["ReceiveDepartmentID"] = receiveDepartmentID.HasValue ? (object)receiveDepartmentID.Value : DBNull.Value;

                                row["HarvestDate_Week"] = Utils.GetThu_Viet(harvestDate);
                                row["HarvestEmployeeName"] = harvestEmployeerName;
                                row["ReceiveDepartmentName"] = departmentName;

                                updateTongThuHoach();

                                actionType += "Success";
                                qlth_status_lb.Text = "Thành công.";
                                qlth_status_lb.ForeColor = Color.Green;
                            }
                            else
                            {
                                actionType += "Fail";
                                qlth_status_lb.Text = "Thất bại.";
                                qlth_status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch
                        {
                            actionType += "Exception";
                            qlth_status_lb.Text = "Thất bại.";
                            qlth_status_lb.ForeColor = Color.Red;
                        }
                        finally
                        {
                            _ = SQLManager_KhoVatTu.Instance.insertHarvestScheduleLogAsync(plantingID, actionType, oldValue, newValue);
                        }
                    }
                    break;
                }
            }
        }

        private async void qlth_createItem(DateTime harvestDate, decimal quantity, string productLotCode, string harvestEmployeeCode, int? receiveDepartmentID)
        {
            DataRow[] harvestEmployeeRows = mEmployee_dt.Select($"EmployeeCode = '{harvestEmployeeCode}'");

            DataRow[] departmentRows = Array.Empty<DataRow>();
            if (receiveDepartmentID.HasValue)
                departmentRows = mDepartment_dt.Select($"DepartmentID = {receiveDepartmentID}");

            DialogResult dialogResult = MessageBox.Show("Chắc chắn chưa ?", "Thông Tin", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                string departmentName = departmentRows.Length > 0 ? departmentRows[0]["DepartmentName"].ToString() : "";
                string harvestEmployeerName = harvestEmployeeRows.Length > 0 ? harvestEmployeeRows[0]["FullName"].ToString() : "";

                string actionType = "Create ";
                string oldValue = "";
                string newValue = $"Ngày: {harvestDate};Số Lượng Thu: {quantity}; Mã Số Thu (LOT): {productLotCode}; Nhân Viên Thu: {harvestEmployeerName}; Phòng Nhận: {departmentName}";

                int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);

                try
                {
                    int newId = await SQLManager_KhoVatTu.Instance.insertHarvestScheduleAsync(plantingID, harvestDate, quantity, productLotCode, harvestEmployeeCode, receiveDepartmentID);
                    if (newId > 0)
                    {
                        DataRow drToAdd = mHarvestSchedule_dt.NewRow();

                        drToAdd["HarvestID"] = newId;
                        drToAdd["PlantingID"] = plantingID;
                        drToAdd["HarvestDate"] = harvestDate;
                        drToAdd["Quantity"] = quantity;
                        drToAdd["ProductLotCode"] = productLotCode;
                        drToAdd["HarvestEmployee"] = harvestEmployeeCode;
                        drToAdd["ReceiveDepartmentID"] = receiveDepartmentID.HasValue ? (object)receiveDepartmentID.Value : DBNull.Value;

                        drToAdd["HarvestDate_Week"] = Utils.GetThu_Viet(harvestDate);
                        drToAdd["HarvestEmployeeName"] = harvestEmployeerName;
                        drToAdd["ReceiveDepartmentName"] = departmentName;

                        mHarvestSchedule_dt.Rows.Add(drToAdd);
                        mHarvestSchedule_dt.AcceptChanges();

                        actionType += "Success";
                        qlth_status_lb.Text = "Thành công";
                        qlth_status_lb.ForeColor = Color.Green;
                        if(receiveDepartmentID.HasValue)
                            Properties.Settings.Default.ToNhanSPThuHoach = receiveDepartmentID.Value;

                        Properties.Settings.Default.MaToTruong_ThuHoach = harvestEmployeeCode;
                        Properties.Settings.Default.Save();

                        updateTongThuHoach();
                        Qlsb_create_btn_Click(null, null);
                    }
                    else
                    {
                        actionType += "Fail";
                        qlth_status_lb.Text = "Thất bại";
                        qlth_status_lb.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    actionType += "Exception";
                    Console.WriteLine("ERROR: " + ex.Message);
                    qlth_status_lb.Text = "Thất bại.";
                    qlth_status_lb.ForeColor = Color.Red;
                }
                finally
                {
                    _ = SQLManager_KhoVatTu.Instance.insertHarvestScheduleLogAsync(plantingID, actionType, oldValue, newValue);
                }
            }
        }

        private async void updateTongThuHoach()
        {
            var result = mHarvestSchedule_dt.AsEnumerable()
                .GroupBy(r => r.Field<DateTime>("HarvestDate"))
                .Select(g => new
                {
                    HarvestDate = g.Key,
                    TotalQuantity = g.Sum(r => r.Field<decimal>("Quantity"))
                })
                .OrderBy(r => r.HarvestDate)
                .ToList();

            DataRow cultivationProcessRowTemp = mCultivationProcess_dt.AsEnumerable().LastOrDefault(r => r.Field<int>("WorkTypeID") == Utils.WorkType_ThuHoach());

            foreach (var item in result)
            {
                DataRow cultivationProcessRow = mCultivationProcess_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("WorkTypeID") == Utils.WorkType_ThuHoach() && r.Field<DateTime>("ProcessDate").Date == item.HarvestDate.Date);
                if(cultivationProcessRow != null)
                {
                    await SQLManager_KhoVatTu.Instance.updateCultivationProcessAsync_ThuHoach(Convert.ToInt32(cultivationProcessRow["CultivationProcessID"]), item.TotalQuantity);
                    cultivationProcessRow["MaterialQuantity"] = item.TotalQuantity;
                }
                else if(cultivationProcessRowTemp != null)
                {
                    int FertilizationWorkTypeID = Convert.ToInt32(cultivationProcessRowTemp["FertilizationWorkTypeID"]);
                    int WorkTypeID = Convert.ToInt32(cultivationProcessRowTemp["WorkTypeID"]);
                    int? MaterialID = cultivationProcessRowTemp.Field<int?>("MaterialID");
                    
                    string Dosage = cultivationProcessRowTemp["Dosage"].ToString();
                    string PlantStatus =cultivationProcessRowTemp["PlantStatus"].ToString();
                    string EmployeeCode = cultivationProcessRowTemp["EmployeeCode"].ToString();
                    int isolationDays = cultivationProcessRowTemp.Field<int?>("IsolationDays") ?? 0;
                    int? DepartmentID = cultivationProcessRowTemp.Field<int?>("DepartmentID");
                    string PlantLocation = cultivationProcessRowTemp["PlantLocation"].ToString();
                    int WaterAmount = Convert.ToInt32(cultivationProcessRowTemp["WaterAmount"]);

                    if (cultivationProcessRowTemp != null) {
                        nktd_createItem(item.HarvestDate, FertilizationWorkTypeID, WorkTypeID, MaterialID, item.TotalQuantity, Dosage, PlantStatus, EmployeeCode, isolationDays,
                            DepartmentID, PlantLocation, WaterAmount, "", false);
                    }
                }
            }

        }

        private void Qlth_Save_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(qlth_Quantity_tb.Text))
                return;

            DateTime harvestDate = qlth_date_dtp.Value;
            decimal quantity = Utils.ParseDecimalSmart(qlth_Quantity_tb.Text);
            string productLotCode = qlth_ProductLotCode_tb.Text;
            string harvestEmployeeCode = qlth_HarvestEmployee_cbb.SelectedValue.ToString();
            int? receiveDepartmentID = (string.IsNullOrEmpty(qlth_ReceiveDepartment_cbb.Text) || qlth_ReceiveDepartment_cbb.SelectedValue == null || qlth_ReceiveDepartment_cbb.SelectedValue == DBNull.Value) ? (int?)null : Convert.ToInt32(qlth_ReceiveDepartment_cbb.SelectedValue);
            
            if (qlth_ID_tb.Text.Length != 0)
                qlth_updateItem(Convert.ToInt32(qlth_ID_tb.Text), harvestDate, quantity, productLotCode, harvestEmployeeCode, receiveDepartmentID);
            else
                qlth_createItem(harvestDate, quantity, productLotCode, harvestEmployeeCode, receiveDepartmentID);
        }
        private async void nktd_deleteProduct(string id, bool isAskQuest = true)
        {
            if (mIsGlobalGap) return;

            foreach (DataRow row in mCultivationProcess_dt.Rows)
            {
                string cultivationProcessID = row["CultivationProcessID"].ToString();
                if (cultivationProcessID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = DialogResult.Yes;

                    if(isAskQuest)
                        dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);
                        string actionType = "Detele ";
                        string oldValue = $"Ngày: {row["ProcessDate"]}; Hình Thức: {row["FertilizationWorkTypeName"]}; Công Việc: {row["WorkTypeName"]}; Vật Tư: {row["MaterialName"]}; S.Lượng VT: {row["MaterialQuantity"]}; Liều Lượng: {row["Dosage"]}; Tình Trạng Cây: {row["PlantStatus"]}; Hoạt Chất: {row["ActiveIngredient"]}; Người TH: {row["EmployeeName"]}; S.Ngày C.Li: {row["IsolationDays"]}; Tổ P.Trách: {row["DepartmentName"]}; V.Trí Trồng: {row["PlantLocation"]}; Lượng Nước: {row["WaterAmount"]}, Ghi Chú: {row["Note"]}";
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.deletetCultivationProcessAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                status_lb.Text = "Thành công.";
                                status_lb.ForeColor = Color.Green;

                                mCultivationProcess_dt.Rows.Remove(row);
                                mCultivationProcess_dt.AcceptChanges();
                                actionType += "Success";

                                updateTongThuHoach();
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
                    }
                    break;
                }
            }
        }

        private async void qlsb_deleteProduct(string id)
        {
            if (mIsGlobalGap) return;
            foreach (DataRow row in mPestDiseaseMonitoring_dt.Rows)
            {
                string monitoringID = row["MonitoringID"].ToString();
                if (monitoringID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);
                        string actionType = "Datele ";
                        string oldValue = $"Ngày: {row["MonitoringDate"]};Vị Trí Mã Lô: {row["Location"]}; Giai Đoạn Sinh Trưởng: {row["GrowthStageName"]}; Tên Sâu Bệnh: {row["PestDiseaseName"]}; Hiện Trạng: {row["CurrentStatus"]}; Người Theo Dõi: {row["ObserverName"]}; Phương Án Xử Lý: {row["PestDiseaseName"]}; Người Quyết Định: {row["DecisionMakerName"]};";
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.deletetPestDiseaseMonitoringAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                qlsb_status_lb.Text = "Thành công.";
                                qlsb_status_lb.ForeColor = Color.Green;

                                mPestDiseaseMonitoring_dt.Rows.Remove(row);
                                mPestDiseaseMonitoring_dt.AcceptChanges();
                                actionType += "Success";
                            }
                            else
                            {
                                actionType += "Fail";
                                qlsb_status_lb.Text = "Thất bại.";
                                qlsb_status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            actionType += "Exception";
                            qlsb_status_lb.Text = "Thất bại.";
                            qlsb_status_lb.ForeColor = Color.Red;
                        }
                        finally
                        {
                            _ = SQLManager_KhoVatTu.Instance.insertPestDiseaseMonitoringLogAsync(plantingID, actionType, oldValue, "");
                        }
                    }
                    break;
                }
            }
        }

        private async void qlth_deleteProduct(string id)
        {
            if (mIsGlobalGap) return;
            foreach (DataRow row in mHarvestSchedule_dt.Rows)
            {
                string harvestID = row["HarvestID"].ToString();
                if (harvestID.CompareTo(id) == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Xóa Nha, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {
                        int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);
                        DateTime harvestDate = Convert.ToDateTime(row["HarvestDate"]);
                        string actionType = "Delete ";
                        string oldValue = $"Ngày: {row["HarvestDate"]};Số Lượng Thu: {row["Quantity"]}; Mã Số Thu (LOT): {row["ProductLotCode"]}; Nhân Viên Thu: {row["HarvestEmployeeName"]}; Phòng Nhận: {row["ReceiveDepartmentName"]}";
                        try
                        {
                            bool isScussess = await SQLManager_KhoVatTu.Instance.deletetHarvestScheduleAsync(Convert.ToInt32(id));

                            if (isScussess == true)
                            {
                                qlth_status_lb.Text = "Thành công.";
                                qlth_status_lb.ForeColor = Color.Green;

                                mHarvestSchedule_dt.Rows.Remove(row);
                                mHarvestSchedule_dt.AcceptChanges();

                                DataRow cultivationProcessRow = mCultivationProcess_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("WorkTypeID") == Utils.WorkType_ThuHoach() && r.Field<DateTime>("ProcessDate").Date == harvestDate.Date);
                                if (cultivationProcessRow != null)
                                {
                                    nktd_deleteProduct(cultivationProcessRow["CultivationProcessID"].ToString(), false);

                                    
                                }

                                actionType += "Success";
                            }
                            else
                            {
                                actionType += "Fail";
                                qlth_status_lb.Text = "Thất bại.";
                                qlth_status_lb.ForeColor = Color.Red;
                            }
                        }
                        catch (Exception ex)
                        {
                            actionType += "Exception";
                            qlth_status_lb.Text = "Thất bại.";
                            qlth_status_lb.ForeColor = Color.Red;
                        }
                        finally
                        {
                            _ = SQLManager_KhoVatTu.Instance.insertHarvestScheduleLogAsync(plantingID, actionType, oldValue, "");
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
            materialQuantity_tb.Text = "";

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

            bool isCompleted = Convert.ToBoolean(mPlantingRow["IsCompleted"]);
            if (isCompleted)
            {
                edit_btn.Visible = false;
                newCustomerBtn.Visible = false;
                LoadDefaultData_btn.Visible = false;
            }
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

        private void Qlsb_readOnly_btn_Click(object sender, EventArgs e)
        {
            qlsb_edit_btn.Visible = true;
            qlsb_create_btn.Visible = true;
            qlsb_readOnly_btn.Visible = false;
            qlsb_Save_btn.Visible = false;
            qlsb_info_gb.BackColor = Color.DarkGray;
            qlsb_isNewState = false;
            qlsb_date_dtp.Enabled = false;
            qlsb_location_tb.Enabled = false;
            qlsb_growthStatus_cbb.Enabled = false;
            qlsb_pestDisease_cbb.Enabled = false;
            qlsb_currentStatus_tb.Enabled = false;
            qlsb_observer_cbb.Enabled = false;
            qlsb_treatmentPlan_tb.Enabled = false;
            qlsb_decisionMaker_cbb.Enabled = false;
            
            if (qlsb_gv.SelectedRows.Count > 0)
                qlsb_updateRightUI();

            bool isCompleted = Convert.ToBoolean(mPlantingRow["IsCompleted"]);
            if (isCompleted)
            {
                qlsb_edit_btn.Visible = false;
                qlsb_create_btn.Visible = false;
            }
        }

        private void Qlsb_edit_btn_Click(object sender, EventArgs e)
        {
            qlsb_edit_btn.Visible = false;
            qlsb_create_btn.Visible = false;
            qlsb_Save_btn.Visible = true;
            qlsb_readOnly_btn.Visible = true;
            qlsb_info_gb.BackColor = edit_btn.BackColor;
            qlsb_isNewState = false;
            qlsb_date_dtp.Enabled = true;
            qlsb_location_tb.Enabled = true;
            qlsb_growthStatus_cbb.Enabled = true;
            qlsb_pestDisease_cbb.Enabled = true;
            qlsb_currentStatus_tb.Enabled = true;
            qlsb_observer_cbb.Enabled = true;
            qlsb_treatmentPlan_tb.Enabled = true;
            qlsb_decisionMaker_cbb.Enabled = true;
            qlsb_Save_btn.Text = "Lưu C.Sửa";
        }

        private void Qlsb_create_btn_Click(object sender, EventArgs e)
        {
            qlsb_ID_tb.Text = "";
            qlsb_status_lb.Text = "";

            qlsb_edit_btn.Visible = false;
            qlsb_create_btn.Visible = false;
            qlsb_Save_btn.Visible = true;
            qlsb_readOnly_btn.Visible = true;
            qlsb_info_gb.BackColor = qlsb_create_btn.BackColor;
            qlsb_isNewState = false;
            qlsb_date_dtp.Enabled = true;
            qlsb_location_tb.Enabled = true;
            qlsb_growthStatus_cbb.Enabled = true;
            qlsb_pestDisease_cbb.Enabled = true;
            qlsb_currentStatus_tb.Enabled = true;
            qlsb_observer_cbb.Enabled = true;
            qlsb_treatmentPlan_tb.Enabled = true;
            qlsb_decisionMaker_cbb.Enabled = true;
            qlsb_Save_btn.Text = "Lưu Mới";
        }

        private void Qlth_readOnly_btn_Click(object sender, EventArgs e)
        {
            qlth_edit_btn.Visible = true;
            qlth_create_btn.Visible = true;
            qlth_readOnly_btn.Visible = false;
            qlth_Save_btn.Visible = false;
            qlth_info_gb.BackColor = Color.DarkGray;

            qlth_isNewState = false;
            qlth_date_dtp.Enabled = false;
            qlth_Quantity_tb.Enabled = false;
            qlth_ProductLotCode_tb.Enabled = false;
            qlth_HarvestEmployee_cbb.Enabled = false;
            qlth_ReceiveDepartment_cbb.Enabled = false;

            if (qlth_gv.SelectedRows.Count > 0)
                qlth_updateRightUI();

            bool isCompleted = Convert.ToBoolean(mPlantingRow["IsCompleted"]);
            if (isCompleted)
            {
                qlth_edit_btn.Visible = false;
                qlth_create_btn.Visible = false;
            }
        }

        private void Qlth_edit_btn_Click(object sender, EventArgs e)
        {
            qlth_edit_btn.Visible = false;
            qlth_create_btn.Visible = false;
            qlth_Save_btn.Visible = true;
            qlth_readOnly_btn.Visible = true;
            qlth_info_gb.BackColor = edit_btn.BackColor;

            qlth_isNewState = false;
            qlth_date_dtp.Enabled = false;
            qlth_Quantity_tb.Enabled = true;
            qlth_ProductLotCode_tb.Enabled = true;
            qlth_HarvestEmployee_cbb.Enabled = true;
            qlth_ReceiveDepartment_cbb.Enabled = true;
            qlth_Save_btn.Text = "Lưu C.Sửa";
        }

        private void Qlth_create_btn_Click(object sender, EventArgs e)
        {
            string productionOrder = mPlantingRow["ProductionOrder"].ToString();

            qlth_ID_tb.Text = "";
            qlth_status_lb.Text = "";
            qlth_ProductLotCode_tb.Text = productionOrder + (mHarvestSchedule_dt.Rows.Count + 1).ToString("D2");

            qlth_HarvestEmployee_cbb.SelectedValue = Properties.Settings.Default.MaToTruong_ThuHoach.ToString();
            qlth_ReceiveDepartment_cbb.SelectedValue = Properties.Settings.Default.ToNhanSPThuHoach;

            qlth_edit_btn.Visible = false;
            qlth_create_btn.Visible = false;
            qlth_Save_btn.Visible = true;
            qlth_readOnly_btn.Visible = true;
            qlth_info_gb.BackColor = qlsb_create_btn.BackColor;

            qlth_isNewState = true;
            qlth_date_dtp.Enabled = true;
            qlth_Quantity_tb.Enabled = true;
            qlth_ProductLotCode_tb.Enabled = true;
            qlth_HarvestEmployee_cbb.Enabled = true;
            qlth_ReceiveDepartment_cbb.Enabled = true;
            qlth_Save_btn.Text = "Lưu Mới";
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

        private void qlsb_updateRightUI()
        {
            try
            {
                if (qlsb_isNewState) return;

                if (qlsb_gv.SelectedRows.Count > 0)
                {
                    var cells = qlsb_gv.SelectedRows[0].Cells;

                    int growthStageID = int.TryParse(cells["GrowthStageID"].Value?.ToString(), out int growthStage) ? growthStage : -1;
                    int pestDiseaseID = int.TryParse(cells["PestDiseaseID"].Value?.ToString(), out int pestDisease) ? pestDisease : -1;
                    string observerCode = cells["Observer"].Value.ToString();
                    string decisionMakerCode = cells["DecisionMaker"].Value.ToString();

                    if (!qlsb_growthStatus_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["GrowthStageID"].ToString() == growthStageID.ToString()))
                    {
                        qlsb_growthStatus_cbb.DataSource = mGrowthStage_dt;
                    }

                    if (!qlsb_pestDisease_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["PestDiseaseID"].ToString() == pestDiseaseID.ToString()))
                    {
                        qlsb_pestDisease_cbb.DataSource = mPestDisease_dt;
                    }

                    if (!qlsb_observer_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["EmployeeCode"].ToString() == observerCode))
                    {
                        qlsb_observer_cbb.DataSource = mEmployee_dt.Copy();
                    }

                    if (!qlsb_decisionMaker_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["EmployeeCode"].ToString() == decisionMakerCode))
                    {
                        qlsb_decisionMaker_cbb.DataSource = mEmployee_dt.Copy();
                    }

                    qlsb_ID_tb.Text = cells["MonitoringID"].Value.ToString();
                    qlsb_date_dtp.Value = Convert.ToDateTime(cells["MonitoringDate"].Value);
                    qlsb_location_tb.Text = cells["Location"].Value.ToString();
                    qlsb_growthStatus_cbb.SelectedValue = growthStageID;
                    qlsb_pestDisease_cbb.SelectedValue = pestDiseaseID;
                    qlsb_currentStatus_tb.Text = cells["CurrentStatus"].Value.ToString();
                    qlsb_observer_cbb.SelectedValue = observerCode;
                    qlsb_treatmentPlan_tb.Text = cells["TreatmentPlan"].Value.ToString();
                    qlsb_decisionMaker_cbb.SelectedValue = decisionMakerCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        private void qlth_updateRightUI()
        {
            try
            {
                if (qlth_isNewState)
                    return;

                if (qlth_gv.SelectedRows.Count > 0)
                {
                    var cells = qlth_gv.SelectedRows[0].Cells;

                    int receiveDepartmentID = int.TryParse(cells["ReceiveDepartmentID"].Value?.ToString(), out int receiveDepartment) ? receiveDepartment : -1;
                    string harvestEmployeeCode = cells["HarvestEmployee"].Value.ToString();

                    if (!qlth_HarvestEmployee_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["EmployeeCode"].ToString() == harvestEmployeeCode))
                    {
                        qlth_HarvestEmployee_cbb.DataSource = mEmployee_dt.Copy();
                    }

                    if (!qlth_ReceiveDepartment_cbb.Items.Cast<object>().Any(i => ((DataRowView)i)["DepartmentID"].ToString() == receiveDepartmentID.ToString()))
                    {
                        qlth_ReceiveDepartment_cbb.DataSource = mDepartment_dt.Copy();
                    }

                    qlth_ID_tb.Text = cells["HarvestID"].Value.ToString();
                    qlth_date_dtp.Value = Convert.ToDateTime(cells["HarvestDate"].Value);
                    qlth_Quantity_tb.Text = Convert.ToDecimal(cells["Quantity"].Value).ToString("G29", CultureInfo.InvariantCulture);
                    qlth_ProductLotCode_tb.Text = cells["ProductLotCode"].Value.ToString();
                    qlth_HarvestEmployee_cbb.SelectedValue = harvestEmployeeCode;
                    qlth_ReceiveDepartment_cbb.SelectedValue = receiveDepartmentID;
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
            DialogResult dialogResult = MessageBox.Show("Tạo Dữ Liệu Mặc Định, Chắc Chắn Chưa!", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
                return;

            int plantingID = Convert.ToInt32(mPlantingRow["PlantingID"]);
            DateTime plantingDate = Convert.ToDateTime(mPlantingRow["PlantingDate"]);
            int sku = Convert.ToInt32(mPlantingRow["SKU"]);
            int cultivationTypeID = Convert.ToInt32(mPlantingRow["CultivationTypeID"]);
            int? departmentId = mPlantingRow["Department"] == DBNull.Value ? (int?)null : Convert.ToInt32(mPlantingRow["Department"]);
            string employeeCode = mPlantingRow["Supervisor"].ToString();
            decimal area = Convert.ToDecimal(mPlantingRow["Area"]);
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

                DateTime processDate = Convert.ToDateTime(mPlantingRow[baseDateType]).AddDays(daysAfter);
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

            await qlsb_createItem(plantingDate.AddDays(7), "", 1, 20, "", employeeCode, "", "", false);
            await qlsb_createItem(plantingDate.AddDays(14), "", 2, 20, "", employeeCode, "", "", false);
            await qlsb_createItem(plantingDate.AddDays(21), "", 2, 20, "", employeeCode, "", "", false);

            SQLStore_KhoVatTu.Instance.removeCultivationProcess(plantingID);
            ShowData();
        }

        private async void ExportExcel_btn_Click(object sender, EventArgs e)
        {
            loadingOverlay = new LoadingOverlay(this);
            loadingOverlay.Message = "Đang xử lý ...";
            loadingOverlay.Show();
            await Task.Delay(100);

            try
            {
                using (var wb = new XLWorkbook())
                {
                    var ws_LSX = wb.Worksheets.Add("Lệnh sản xuất");
                    ws_LSX.Style.Font.FontName = "Times New Roman";
                    ws_LSX.Style.Font.FontSize = 12;
                    ws_LSX.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    WS_LenhSanXuat(ws_LSX);

                    var ws_NKTD = wb.Worksheets.Add("Nhật ký theo dõi");
                    ws_NKTD.Style.Font.FontName = "Times New Roman";
                    ws_NKTD.Style.Font.FontSize = 12;
                    ws_NKTD.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    WS_NhatKyTheoDoi(ws_NKTD);

                    var ws_QLSB = wb.Worksheets.Add("Quản lý sâu bệnh");
                    ws_QLSB.Style.Font.FontName = "Times New Roman";
                    ws_QLSB.Style.Font.FontSize = 12;
                    ws_QLSB.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    WS_QLSauBenh(ws_QLSB);

                    var ws_TH = wb.Worksheets.Add("Thu Hoạch");
                    ws_TH.Style.Font.FontName = "Times New Roman";
                    ws_TH.Style.Font.FontSize = 12;
                    ws_TH.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    WS_QThuHoach(ws_TH);

                    //ws.Column(8).Width = 27;
                    // ===== Save file =====
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Excel Workbook|*.xlsx";
                        sfd.FileName = $"{(mIsGlobalGap? "GlobalGAP_":"")}{Utils.RemoveVietnameseSigns(mPlantingRow["PlantName"].ToString()).Replace(" ", "")}_{mPlantingRow["ProductionOrder"].ToString()}";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            wb.SaveAs(sfd.FileName);
                            DialogResult result = MessageBox.Show("Bạn có muốn mở file này không?", "Lưu file thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                                    {
                                        FileName = sfd.FileName,
                                        UseShellExecute = true
                                    });
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Không thể mở file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message);
            }
            finally
            {
                await Task.Delay(200);
                loadingOverlay.Hide();
            }
        }


        void WS_LenhSanXuat(IXLWorksheet ws)
        {
            int rowInd = 1;
            int columnInd = 1;

            var image = Properties.Resources.logo_vr_1;

            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                var picture = ws.AddPicture(ms)
                                .MoveTo(ws.Cell(rowInd, columnInd));
            }

            columnInd++;
            ws.Cell(rowInd, columnInd).Value = "LỆNH SẢN XUẤT";
            ws.Cell(rowInd, columnInd).Style.Font.Bold = true;
            ws.Cell(rowInd, columnInd).Style.Font.FontSize = 14;
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            columnInd++;
            ws.Cell(rowInd, columnInd).Value = "Số hiệu:\nNgày ban hành:\nLần ban hành:\nSố trang:";
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            columnInd++;
            ws.Cell(rowInd, columnInd).Value = Utils.VRF_GAP();
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;
            rowInd = 2;
            columnInd = 1;
            // ===== Header cấp 2 và cấp 3 =====
            string[] columnsName = new string[] { "Tên sản phẩm:", "Mã sản phẩm:", "Số lệnh sản xuất:", "Mã quy trình:", "Ngày gieo hạt:", "Số thứ tự trồng:", "Diện tích:", "Số lượng cây (cây):", "Ngày trồng:", "Phụ trách kỹ thuật:", "Ngày thu hoạch:", "Tổ phụ trách:", "Vị trí trồng:", "Người phụ trách:" };
            string[] exportColumns = new string[] { "PlantName", "SKU", "ProductionOrder", "MaQuyTrinh", "NurseryDate", "SoThuVuTrong", "Area", "Quantity", "PlantingDate", "SupervisorName", "HarvestDate", "DepartmentName", "PlantLocation", "NguoiPhuTrach" };
            string[] typeColumns = new string[] { "string", "int", "string", "string", "date", "string", "decimal", "decimal", "date", "string", "date", "string", "string", "string" };
            for (int i = 0; i < columnsName.Length; i++)
            {
                var titleCell = ws.Cell(rowInd, columnInd);
                titleCell.Value = columnsName[i];
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                titleCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                string valueStr = "";

                if (exportColumns[i].CompareTo("SoThuVuTrong") == 0)
                {
                    string productionOrder = mPlantingRow["ProductionOrder"].ToString();

                    string last3 = productionOrder.Length >= 3 ? productionOrder.Substring(productionOrder.Length - 3) : "";
                    string twoFromMinus3 = productionOrder.Length >= 5 ? productionOrder.Substring(productionOrder.Length - 5, 2) : "";

                    valueStr = last3 + "/" + twoFromMinus3;
                }
                else if (exportColumns[i].CompareTo("NguoiPhuTrach") == 0)
                {
                    int departmentID = Convert.ToInt32(mPlantingRow["Department"]);
                    DataRow[] rows = mEmployee_dt.Select($"DepartmentID = {departmentID} AND PositionID = 17");
                    if (rows.Length > 0)
                        valueStr = rows[0]["FullName"].ToString();
                }
                else if (mPlantingRow.Table.Columns.Contains(exportColumns[i]))
                    valueStr = mPlantingRow[exportColumns[i]].ToString();

                var valueCell = ws.Cell(rowInd, columnInd + 1);
                valueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                valueCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                if (string.IsNullOrEmpty(valueStr))
                    valueCell.Value = valueStr;
                else
                {
                    if (typeColumns[i].CompareTo("int") == 0)
                        valueCell.Value = Convert.ToInt32(valueStr);
                    else if (typeColumns[i].CompareTo("date") == 0)
                        valueCell.Value = Convert.ToDateTime(valueStr);
                    else if (typeColumns[i].CompareTo("decimal") == 0)
                        valueCell.Value = Convert.ToDecimal(valueStr);
                    else
                        valueCell.Value = valueStr;
                }

                columnInd += 2;
                if (columnInd > 3)
                {
                    columnInd = 1;
                    rowInd++;
                }

            }
            ws.Column(1).Width = 16;
            ws.Column(2).Width = 25;
            ws.Column(3).Width = 17.5;
            ws.Column(4).Width = 30;
            ws.Row(1).Height = 62;
        }

        void WS_NhatKyTheoDoi(IXLWorksheet ws)
        {
            // Danh sách cột muốn xuất theo Name

            int numColumn = 6;
            int rowInd = 1;
            int columnInd = 1;

            var image = Properties.Resources.logo_vr_1;

            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                var picture = ws.AddPicture(ms)
                                .MoveTo(ws.Cell(rowInd, columnInd));
            }

            columnInd+=2;
            ws.Range(rowInd, columnInd, rowInd, columnInd + 10).Merge();
            ws.Cell(rowInd, columnInd).Value = "BIỂU MẪU GIÁM SÁT NHẬT KÝ SẢN XUẤT";
            ws.Cell(rowInd, columnInd).Style.Font.Bold = true;
            ws.Cell(rowInd, columnInd).Style.Font.FontSize = 14;
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            columnInd+=11;
            ws.Range(rowInd, columnInd, rowInd, columnInd + 1).Merge();
            ws.Cell(rowInd, columnInd).Value = "Số hiệu:\nNgày ban hành:\nLần ban hành:\nSố trang:";
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            columnInd+=2;
            ws.Range(rowInd, columnInd, rowInd, columnInd + 1).Merge();
            ws.Cell(rowInd, columnInd).Value = Utils.VRF_GAP();
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            rowInd = 2;
            columnInd = 1;
            string[] columnsName = new string[] { "Ngày", "Thứ", "Loại Công Việc", "Tên Vật Tư", "Số Lượng", "Đơn Vị", "Mã LOT Vật Tư", "Liều Lượng", "Tình Trạng Cây Trồng", "Thành Phần/ Hoạt Chất", "Người Thực Hiện", "Thời Gian Cách Li (Ngày )", "Ngày Hết Cách Li", "Tổ Ph.Trách", "Vị Trí Trồng", "Lượng Nước Tưới (L/h))", "Ghi Chú" };
            string[] exportColumns = new string[] { "ProcessDate", "ProcessDate_Week", "WorkTypeName", "MaterialName", "MaterialQuantity", "MaterialUnit", "maLotVT", "Dosage", "PlantStatus", "ActiveIngredient", "EmployeeName", "IsolationDays", "IsolationEndDate", "DepartmentName", "PlantLocation", "WaterAmount", "Note1" };
            string[] typeColumns = new string[] { "date", "string", "string", "string", "decimal", "string", "string", "string", "string", "string", "string", "int", "date", "sring", "string", "decimal", "string" };
            float[] widthColumns = new float[] { 11, 5, 17, 45, 9.8f, 7.2f, 17f, 12, 22, 24, 30, 27, 18, 20, 13, 25, 12 };
            foreach (var tile in columnsName)
            {
                var titleCell = ws.Cell(rowInd, columnInd);
                titleCell.Style.Font.Bold = true;

                titleCell.Value = tile;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                titleCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung

                columnInd++;
            }

            rowInd++;

            foreach (DataRow row in mCultivationProcess_dt.Rows)
            {
                int? workTypeID = row.Field<int?>("WorkTypeID");

                for (int i = 0; i < exportColumns.Length; i++)
                {
                    columnInd = i + 1;
                    var valueCell = ws.Cell(rowInd, columnInd);
                    valueCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    if (!mCultivationProcess_dt.Columns.Contains(exportColumns[i]))
                        continue;

                    string valueStr = row[exportColumns[i]].ToString();
                    if (string.IsNullOrEmpty(valueStr))
                    {
                        if (workTypeID == Utils.WorkType_ThuHoach())
                        {
                            if (exportColumns[i].CompareTo("MaterialName") == 0)
                                valueCell.Value = mPlantingRow["PlantName"].ToString();
                            else if (exportColumns[i].CompareTo("MaterialUnit") == 0)
                                valueCell.Value = "Kg";
                            else
                                valueCell.Value = valueStr;
                        }
                        else
                            valueCell.Value = valueStr;
                    }
                    else
                    {
                        if (typeColumns[i].CompareTo("int") == 0)
                            valueCell.Value = Convert.ToInt32(valueStr);
                        else if (typeColumns[i].CompareTo("date") == 0)
                            valueCell.Value = Convert.ToDateTime(valueStr);
                        else if (typeColumns[i].CompareTo("decimal") == 0)
                            valueCell.Value = Convert.ToDecimal(valueStr);
                        else
                        {
                            if (workTypeID == Utils.WorkType_ThuHoach())
                            {
                                if (exportColumns[i].CompareTo("MaterialName") == 0)
                                    valueCell.Value = mPlantingRow["PlantName"].ToString();
                                else if (exportColumns[i].CompareTo("MaterialUnit") == 0)
                                    valueCell.Value = "Kg";
                                else
                                    valueCell.Value = valueStr;
                            }

                            else
                            {
                                valueCell.Value = valueStr;
                            }
                        }
                    }

                    valueCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }

                rowInd++;
            }

            for(int i =0; i < widthColumns.Length; i++)
            {
                ws.Column(i+1).Width = widthColumns[i];
            }
            ws.Row(1).Height = 63;
        }

        void WS_QLSauBenh(IXLWorksheet ws)
        {
            // Danh sách cột muốn xuất theo Name

            int numColumn = 6;
            int rowInd = 1;
            int columnInd = 1;

            var image = Properties.Resources.logo_vr_1;

            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                var picture = ws.AddPicture(ms)
                                .MoveTo(ws.Cell(rowInd, columnInd));
            }

            columnInd ++;
            ws.Range(rowInd, columnInd, rowInd, columnInd + 4).Merge();
            ws.Cell(rowInd, columnInd).Value = "NHẬT KÍ QUẢN LÝ SÂU BỆNH";
            ws.Cell(rowInd, columnInd).Style.Font.Bold = true;
            ws.Cell(rowInd, columnInd).Style.Font.FontSize = 14;
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            columnInd +=5;
            ws.Cell(rowInd, columnInd).Value = "Số hiệu:\nNgày ban hành:\nLần ban hành:\nSố trang:";
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            columnInd ++;
            ws.Cell(rowInd, columnInd).Value = Utils.VRF_GAP();
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            rowInd = 2;
            columnInd = 1;
            string[] columnsName = new string[] { "Ngày Theo Dõi", "Vị Trí Mã Lô", "Giai Đoạn Sinh Trưởng", "Tên Sâu Bệnh Hại", "Hiện Trạng Và Mật Độ Sâu Bệnh", "Người Theo Dõi", "Phương Án Xử Lý Và Sử Dụng Thuốc BVTV", "Người Quyết Định Phương Án Xử Lý"};
            string[] exportColumns = new string[] { "MonitoringDate", "Location", "GrowthStageName", "PestDiseaseName", "CurrentStatus", "ObserverName", "TreatmentPlan", "DecisionMakerName" };
            foreach (var tile in columnsName)
            {
                var titleCell = ws.Cell(rowInd, columnInd);
                titleCell.Style.Font.Bold = true;

                titleCell.Value = tile;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                titleCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                titleCell.Style.Alignment.WrapText = true;
                columnInd++;
            }

            rowInd++;
            
            foreach (DataRow row in mPestDiseaseMonitoring_dt.Rows)
            {
                columnInd = 1;
                foreach (var columnName in exportColumns)
                {
                    string valueStr = row[columnName].ToString();
                    var titleCell = ws.Cell(rowInd, columnInd);

                    if (columnName.CompareTo("MonitoringDate") == 0)
                        titleCell.Value = Convert.ToDateTime(valueStr).Date;
                    else
                        titleCell.Value = valueStr;

                    titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    titleCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                    columnInd++;
                }
                
                rowInd++;
            }

            

            ws.Column(1).Width = 16;
            ws.Column(2).Width = 14;
            ws.Column(3).Width = 18;
            ws.Column(4).Width = 19;
            ws.Column(5).Width = 17;
            ws.Column(6).Width = 24.2;
            ws.Column(7).Width = 20;
            ws.Column(8).Width = 23.75;
            ws.Row(1).Height = 63;
            ws.Row(2).Height = 32;
        }

        void WS_QThuHoach(IXLWorksheet ws)
        {
            // Danh sách cột muốn xuất theo Name

            int numColumn = 6;
            int rowInd = 1;
            int columnInd = 1;

            var image = Properties.Resources.logo_vr_1;

            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                var picture = ws.AddPicture(ms)
                                .MoveTo(ws.Cell(rowInd, columnInd));
            }

            columnInd++;
            ws.Range(rowInd, columnInd, rowInd, columnInd + 5).Merge();
            ws.Cell(rowInd, columnInd).Value = "BIỂU MẪU GIÁM SÁT THU HOẠCH";
            ws.Cell(rowInd, columnInd).Style.Font.Bold = true;
            ws.Cell(rowInd, columnInd).Style.Font.FontSize = 14;
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            columnInd += 6;
            ws.Cell(rowInd, columnInd).Value = "Số hiệu:\nNgày ban hành:\nLần ban hành:\nSố trang:";
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            columnInd++;
            ws.Cell(rowInd, columnInd).Value = Utils.VRF_GAP();
            ws.Cell(rowInd, columnInd).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(rowInd, columnInd).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell(rowInd, columnInd).Style.Alignment.WrapText = true;

            rowInd = 2;
            columnInd = 1;
            string[] columnsName = new string[] { "Ngày Thu Hoạch", "Thứ", "Loại Sản Phẩm", "Sản Lượng (kg)", "Vị Trí Thu Hoạch", "Mã Số Lô Sản Phẩm (LOT)", "Người Giám Sát", "Người Thu Hoạch", "Nơi Nhận Sản Phẩm" };
            string[] exportColumns = new string[] { "HarvestDate", "HarvestDate_Week", "PlantName", "Quantity", "PlantLocation", "ProductLotCode", "EmployeeName", "HarvestEmployeeName", "ReceiveDepartmentName" };

            DataRow thuHoachRow = mCultivationProcess_dt.Select($"WorkTypeID = {Utils.WorkType_ThuHoach()}").FirstOrDefault();
            var plantingLocalRow = mCultivationProcess_dt.AsEnumerable().FirstOrDefault(r =>!string.IsNullOrWhiteSpace(r.Field<string>("PlantLocation")));

            foreach (var tile in columnsName)
            {
                var titleCell = ws.Cell(rowInd, columnInd);
                titleCell.Style.Font.Bold = true;

                titleCell.Value = tile;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                titleCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                titleCell.Style.Alignment.WrapText = true;
                columnInd++;
            }
            rowInd++;

            foreach(DataRow row in mHarvestSchedule_dt.Rows)
            {
                columnInd = 1;
                foreach (var columnName in exportColumns)
                {
                    string valueStr = "";
                    if (columnName.CompareTo("PlantName") == 0)
                    {
                        valueStr = mPlantingRow[columnName].ToString();
                    }
                    else if (columnName.CompareTo("EmployeeName") == 0 && !mIsGlobalGap)
                    {
                        if (thuHoachRow != null)
                            valueStr = thuHoachRow[columnName].ToString();
                    }
                    else if (columnName.CompareTo("PlantLocation") == 0)
                    {
                        if (plantingLocalRow != null)
                            valueStr = plantingLocalRow[columnName].ToString();
                    }
                    else
                    {
                        if(columnName.CompareTo("EmployeeName") == 0)
                            valueStr = row["SupervisorName"].ToString();
                        else
                            valueStr = row[columnName].ToString();
                    }

                    var titleCell = ws.Cell(rowInd, columnInd);

                    if (columnName.CompareTo("HarvestDate") == 0)
                        titleCell.Value = Convert.ToDateTime(valueStr).Date;
                    else if(columnName.CompareTo("Quantity") == 0)
                        titleCell.Value = Convert.ToDecimal(valueStr);
                    else
                        titleCell.Value = valueStr;

                    titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    titleCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Đóng khung
                    columnInd++;
                }

                rowInd++;
            }

            ws.Column(1).Width = 16.72;
            ws.Column(2).Width = 5;
            ws.Column(3).Width = 18;
            ws.Column(4).Width = 14;
            ws.Column(5).Width = 18;
            ws.Column(6).Width = 22;
            ws.Column(7).Width = 30;
            ws.Column(8).Width = 20;
            ws.Column(9).Width = 21;
            ws.Row(1).Height = 63;
            ws.Row(2).Height = 32;
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

        //            Utils.AddColumnIfNotExists(excelData, "WorkTypeID", typeof(int));
        //            Utils.AddColumnIfNotExists(excelData, "MaterialID", typeof(int));
        //            Utils.AddColumnIfNotExists(excelData, "PlantingID", typeof(int));
        //            Utils.AddColumnIfNotExists(excelData, "EmployeeCode", typeof(string));
        //            Utils.AddColumnIfNotExists(excelData, "Note", typeof(string));
        //            foreach (DataRow row in excelData.Rows)
        //            {
        //                string raw = Utils.RemoveVietnameseSigns(row["Column2"].ToString());

        //                if(raw.CompareTo(Utils.RemoveVietnameseSigns("Magnesium sulphate (Green mag)")) == 0)
        //                    raw = Utils.RemoveVietnameseSigns("BitterMag (MgSO4)");
        //                else if (raw.CompareTo(Utils.RemoveVietnameseSigns("YaraMila Winner")) == 0)
        //                    raw = Utils.RemoveVietnameseSigns("YaraMila Winner (15-9-20)");
        //                else if (raw.CompareTo(Utils.RemoveVietnameseSigns("Solu-K 0-0-51")) == 0)
        //                    raw = Utils.RemoveVietnameseSigns("Solu-K (K2SO4 Hàn Quốc)");

        //                DataRow material = mMaterial_dt.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("MaterialName")).Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if(material != null)
        //                    row["MaterialID"] = material["MaterialID"];

        //                raw = Utils.RemoveVietnameseSigns(row["Column6"].ToString());
        //                DataRow employee = mEmployee_dt.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("FullName")).Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if (employee != null)
        //                    row["EmployeeCode"] = employee["EmployeeCode"];

        //                raw = Utils.RemoveVietnameseSigns(row["Column5"].ToString());
        //                DataRow workType = mWorkType_dt.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("WorkTypeName")).Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if (workType != null)
        //                    row["WorkTypeID"] = workType["WorkTypeID"];

        //                raw = Utils.RemoveVietnameseSigns(row["Column4"].ToString()).Trim();
        //                if (raw.CompareTo("28125020") == 0)
        //                    raw = "28126001";
        //                else if (raw.CompareTo("11126001") == 0)
        //                    raw = "11125005";
        //                else if (raw.CompareTo("143250100") == 0)
        //                    raw = "14325100";
        //                else if (raw.CompareTo("12126005") == 0) 
        //                    raw = "12125007";
        //                else if (raw.CompareTo("17225053") == 0)
        //                    raw = "17226001";
        //                else if (raw.StartsWith("MXC")) 
        //                {
        //                    row["Note"] = raw;
        //                    row["Column4"] = "";
        //                    raw = "";
        //                }
        //                else if (raw.CompareTo("33125001") == 0 || raw.CompareTo("32324002") == 0)
        //                {
        //                    row["Note"] = raw;
        //                    row["Column4"] = "";
        //                    raw = "";
        //                }

        //                DataRow plantingManagement = mPlantingManagement_dt.AsEnumerable().FirstOrDefault(r => Utils.RemoveVietnameseSigns(r.Field<string>("ProductionOrder")).Trim().ToLower().Equals(raw.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
        //                if (plantingManagement != null)
        //                    row["PlantingID"] = plantingManagement["PlantingID"];
        //            }

        //            dataGV.DataSource = excelData;
        //            dataGV.Columns["Column2"].Visible = false;
        //            dataGV.Columns["Column6"].Visible = false;
        //            dataGV.Columns["Column5"].Visible = false;
        //            }
        //    }
        //}

        //private async void button2_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        foreach (DataRow edr in excelData.Rows)
        //        {
        //            if (string.IsNullOrWhiteSpace(edr["Column3"].ToString())) 
        //                continue;

        //            DateTime ExportDate = Convert.ToDateTime(edr["Column1"]);
        //            int MaterialID = Convert.ToInt32(edr["MaterialID"]);
        //            decimal Amount = Convert.ToDecimal(edr["Column3"]);
        //            int? PlantingID = edr["PlantingID"] == DBNull.Value ? (int?)null : Convert.ToInt32(edr["PlantingID"]);
        //            int? WorkTypeID = edr["WorkTypeID"] == DBNull.Value ? (int?)null : Convert.ToInt32(edr["WorkTypeID"]);
        //            string EmployeeCode = Convert.ToString(edr["EmployeeCode"]);
        //            string Note = Convert.ToString(edr["Note"]);


        //            int salaryInfoID = await SQLManager_KhoVatTu.Instance.insertMaterialExportAsync(ExportDate, MaterialID, Amount, PlantingID, WorkTypeID, EmployeeCode, Note);
        //            if(salaryInfoID <= 0)
        //            {
        //                MessageBox.Show("có lỗi ");
        //                return;
        //            }

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
