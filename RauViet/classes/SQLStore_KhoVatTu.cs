using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataTable = System.Data.DataTable;

namespace RauViet.classes
{
    public sealed class SQLStore_KhoVatTu
    {
        private static SQLStore_KhoVatTu ins = null;
        private static readonly object padlock = new object();

        //suong
        DataTable mWorkType_dt = null;
        DataTable mCultivationType_dt = null;
        DataTable mUnit_dt = null;
        DataTable mMaterialCategory_dt = null;
        DataTable mMaterial_dt = null;
        DataTable mCultivationProcessTemplate_dt = null;

        Dictionary<int, DataTable> mCultivationProcesses = null;
        Dictionary<int, DataTable> mPlantingManagements = null;
        Dictionary<string, DataTable> mMaterialExports = null;
        Dictionary<string, DataTable> mMaterialImports = null;

        Dictionary<string, DataTable> mMaterialExportLogs = null;
        Dictionary<string, DataTable> mMaterialImportLogs = null;
        Dictionary<int, DataTable> mPlantingManagementLogs = null;

        private SQLStore_KhoVatTu() { }

        public static SQLStore_KhoVatTu Instance
        {
            get
            {
                lock (padlock)
                {
                    if (ins == null)
                        ins = new SQLStore_KhoVatTu();
                    return ins;
                }
            }
        }

        public async Task preload()
        {
            try
            {
                int year = DateTime.Now.Year;

                mPlantingManagements = new Dictionary<int, DataTable>();
                mMaterialExports = new Dictionary<string, DataTable>();
                mMaterialImports = new Dictionary<string, DataTable>();
                mMaterialExportLogs = new Dictionary<string, DataTable>();
                mMaterialImportLogs = new Dictionary<string, DataTable>();
                mPlantingManagementLogs = new Dictionary<int, DataTable>();
                mCultivationProcesses = new Dictionary<int, DataTable>();

                var unitTask = SQLManager_KhoVatTu.Instance.GetUnitAsync();
                var MaterialCategoryTask = SQLManager_KhoVatTu.Instance.GetMaterialCategoryAsync();
                var MaterialTask = SQLManager_KhoVatTu.Instance.GetMaterialAsync();
                var WorkTypeTask = SQLManager_KhoVatTu.Instance.GetWorkTypeAsync();
                var CultivationTypeTask = SQLManager_KhoVatTu.Instance.GetCultivationTypeAsync();
                var PlantingManagementTask = SQLManager_KhoVatTu.Instance.getPlantingManagementAsync(year);
                await Task.WhenAll(unitTask, MaterialCategoryTask, MaterialTask, WorkTypeTask, PlantingManagementTask, CultivationTypeTask);

                if (unitTask.Status == TaskStatus.RanToCompletion && unitTask.Result != null) mUnit_dt = unitTask.Result;
                if (MaterialCategoryTask.Status == TaskStatus.RanToCompletion && MaterialCategoryTask.Result != null) mMaterialCategory_dt = MaterialCategoryTask.Result;
                if (MaterialTask.Status == TaskStatus.RanToCompletion && MaterialTask.Result != null) mMaterial_dt = MaterialTask.Result;
                if (WorkTypeTask.Status == TaskStatus.RanToCompletion && WorkTypeTask.Result != null) mWorkType_dt = WorkTypeTask.Result;
                if (PlantingManagementTask.Status == TaskStatus.RanToCompletion && PlantingManagementTask.Result != null) mPlantingManagements[year] = PlantingManagementTask.Result;
                if (CultivationTypeTask.Status == TaskStatus.RanToCompletion && CultivationTypeTask.Result != null) mCultivationType_dt = CultivationTypeTask.Result;

                editMaterial(mMaterial_dt);
                editCategory(mMaterialCategory_dt);
                editUnit(mUnit_dt);
                editWorkType(mWorkType_dt);
                editCultivationType(mCultivationType_dt);
                editPlantingManagement(PlantingManagementTask.Result);
            }
            catch
            {
                Console.WriteLine("preload in SQLStore errror");
            }
        }

        private void editMaterial(DataTable data)
        {
            data.Columns.Add("CategoryName", typeof(string));
            data.Columns.Add("UnitName", typeof(string));
            data.Columns.Add("MaterialName_nosign", typeof(string));
            foreach (DataRow row in data.Rows)
            {
                int CategoryID = Convert.ToInt32(row["CategoryID"]);
                int UnitID = Convert.ToInt32(row["UnitID"]);

                DataRow unitRow = mUnit_dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["UnitID"]) == UnitID);
                DataRow categoryRow = mMaterialCategory_dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["CategoryID"]) == CategoryID);


                if (unitRow != null)
                {
                    row["UnitName"] = unitRow["UnitName"].ToString();
                }

                if (categoryRow != null)
                {
                    row["CategoryName"] = categoryRow["CategoryName"].ToString();
                }

                row["MaterialName_nosign"] = Utils.RemoveVietnameseSigns($"{row["CategoryName"].ToString()} {row["MaterialName"].ToString()}");
            }
        }

        public void removeMaterial() { mMaterial_dt = null; }

        public async Task<DataTable> getMaterialAsync()
        {
            if (mMaterial_dt == null)
            {
                try
                {
                    await GetUnitAsync();
                    await GetMaterialCategoryAsync();
                    mMaterial_dt = await SQLManager_KhoVatTu.Instance.GetMaterialAsync();
                    editMaterial(mMaterial_dt);
                }
                catch
                {
                    Console.WriteLine("error getMaterialAsync SQLStore");
                    return null;
                }
            }

            return mMaterial_dt;
        }

        public async Task<DataTable> GetUnitAsync()
        {
            if (mUnit_dt == null)
            {
                try
                {
                    mUnit_dt = await SQLManager_KhoVatTu.Instance.GetUnitAsync();
                    editUnit(mUnit_dt);
                }
                catch
                {
                    Console.WriteLine("error getMaterialAsync SQLStore");
                    return null;
                }
            }

            return mUnit_dt;
        }

        private void editUnit(DataTable data)
        {
            data.Columns.Add("UnitName_nosign", typeof(string));
            foreach (DataRow row in data.Rows)
                row["UnitName_nosign"] = Utils.RemoveVietnameseSigns(row["UnitName"].ToString());
        }

        public async Task<DataTable> GetMaterialCategoryAsync()
        {
            if (mMaterialCategory_dt == null)
            {
                try
                {
                    mMaterialCategory_dt = await SQLManager_KhoVatTu.Instance.GetMaterialCategoryAsync();
                    editCategory(mMaterialCategory_dt);
                }
                catch
                {
                    Console.WriteLine("error GetMaterialCategoryAsync SQLStore");
                    return null;
                }
            }

            return mMaterialCategory_dt;
        }

        private void editCategory(DataTable data)
        {
            data.Columns.Add("CategoryName_nosign", typeof(string));
            foreach (DataRow row in data.Rows)
                row["CategoryName_nosign"] = Utils.RemoveVietnameseSigns(row["CategoryName"].ToString());
        }

        public void removePlantingManagementAsync(int year) { mPlantingManagements.Remove(year); }
        public async Task<DataTable> getPlantingManagementAsync(int year)
        {
            if (mPlantingManagements.ContainsKey(year) == false)
            {
                try
                {
                    DataTable data = await SQLManager_KhoVatTu.Instance.getPlantingManagementAsync(year);
                    editPlantingManagement(data);
                    mPlantingManagements[year] = data;
                }
                catch
                {
                    Console.WriteLine("error getMaterialAsync SQLStore");
                    return null;
                }
            }

            return mPlantingManagements[year];
        }

        public async Task<DataTable> getPlantingManagementAsync(bool isComplete)
        {
            try
            {
                DataTable data = await SQLManager_KhoVatTu.Instance.getPlantingManagementAsync(isComplete);
                editPlantingManagement(data);

                data.Columns.Add("LenhSX_CayTrong", typeof(string));

                foreach (DataRow row in data.Rows)
                {
                    row["LenhSX_CayTrong"] = $"{row["ProductionOrder"]} - {row["PlantName"]}";
                }

                return data;
            }
            catch
            {
                Console.WriteLine("error getMaterialAsync SQLStore");
                return null;
            }
        }

        private async void editPlantingManagement(DataTable data)
        {
            data.Columns.Add("DepartmentName", typeof(string));
            data.Columns.Add("PlantName", typeof(string));
            data.Columns.Add("SupervisorName", typeof(string));
            data.Columns.Add("CultivationTypeName", typeof(string));
            data.Columns.Add("search_nosign", typeof(string));

            DataTable employee_dt = await SQLStore_QLNS.Instance.GetEmployeesAsync();
            DataTable department_dt = await SQLStore_QLNS.Instance.GetDepartmentAsync();
            DataTable product_dt = await SQLStore_Kho.Instance.getProductSKUAsync();
            await SQLStore_KhoVatTu.Instance.GetCultivationTypeAsync();
            foreach (DataRow row in data.Rows)
            {
                int sku = Convert.ToInt32(row["SKU"]);
                int? department = row.Field<int?>("Department");
                int? cultivationTypeID = row.Field<int?>("CultivationTypeID");
                string Supervisor = row["Supervisor"].ToString();

                DataRow productRow = product_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("ProductSKU") == sku);
                if (productRow != null)
                    row["PlantName"] = productRow["ProductNameVN"];

                DataRow employeeRow = employee_dt.AsEnumerable().FirstOrDefault(r => r.Field<string>("EmployeeCode") == Supervisor);
                if (employeeRow != null)
                    row["SupervisorName"] = employeeRow["FullName"];

                DataRow departmentRow = department_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("DepartmentID") == department);
                if (departmentRow != null)
                    row["DepartmentName"] = departmentRow["DepartmentName"];

                if (cultivationTypeID.HasValue)
                {
                    DataRow cultivationRow = mCultivationType_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("CultivationTypeID") == cultivationTypeID);
                    if (cultivationRow != null)
                        row["CultivationTypeName"] = cultivationRow["CultivationTypeName"];
                }
                row["search_nosign"] = Utils.RemoveVietnameseSigns($"{row["ProductionOrder"]} {row["PlantName"]}");
            }
        }

        public async Task<DataTable> GetWorkTypeAsync()
        {
            if (mWorkType_dt == null)
            {
                try
                {
                    mWorkType_dt = await SQLManager_KhoVatTu.Instance.GetWorkTypeAsync();
                    editWorkType(mWorkType_dt);
                }
                catch
                {
                    Console.WriteLine("error GetWorkTypeAsync SQLStore");
                    return null;
                }
            }

            return mWorkType_dt;
        }

        public async Task<DataTable> GetCultivationTypeAsync()
        {
            if (mCultivationType_dt == null)
            {
                try
                {
                    mCultivationType_dt = await SQLManager_KhoVatTu.Instance.GetCultivationTypeAsync();
                    editCultivationType(mCultivationType_dt);
                }
                catch
                {
                    Console.WriteLine("error GetCultivationTypeAsync SQLStore");
                    return null;
                }
            }

            return mCultivationType_dt;
        }

        public DataTable GetBaseDateType()
        {
            DataTable baseDateType_dt = new DataTable();

            baseDateType_dt.Columns.Add("Value", typeof(string));
            baseDateType_dt.Columns.Add("Text", typeof(string));

            baseDateType_dt.Rows.Add("NurseryDate", "Ngày Ươm");
            baseDateType_dt.Rows.Add("PlantingDate", "Ngày Trồng");
            baseDateType_dt.Rows.Add("HarvestDate", "Ngày Thu");

            return baseDateType_dt;
        }

        void editWorkType(DataTable data)
        {
            data.Columns.Add("search_nosign", typeof(string));
            foreach (DataRow row in data.Rows)
            {
                row["search_nosign"] = Utils.RemoveVietnameseSigns(row["WorkTypeName"].ToString());
            }
        }

        void editCultivationType(DataTable data)
        {
            data.Columns.Add("search_nosign", typeof(string));
            foreach (DataRow row in data.Rows)
            {
                row["search_nosign"] = Utils.RemoveVietnameseSigns(row["CultivationTypeName"].ToString());
            }
        }

        public void removeMaterialExport(int month, int year)
        {
            string key = $"{month}_{year}";
            mMaterialExports.Remove(key);
        }

        public async Task<DataTable> GetMaterialExportAsync(int month, int year)
        {
            string key = $"{month}_{year}";
            if (!mMaterialExports.ContainsKey(key))
            {
                try
                {
                    var data = await SQLManager_KhoVatTu.Instance.GetMaterialExportAsync(month, year);
                    editMaterialExport(data, month, year);
                    mMaterialExports[key] = data;
                }
                catch
                {
                    Console.WriteLine("error GetMaterialExportAsync SQLStore");
                    return null;
                }
            }

            return mMaterialExports[key];
        }

        private async void editMaterialExport(DataTable data, int month, int year)
        {
            data.Columns.Add("MaterialName", typeof(string));
            data.Columns.Add("PlantName", typeof(string));
            data.Columns.Add("WorkTypeName", typeof(string));
            data.Columns.Add("RecieverName", typeof(string));
            data.Columns.Add("UnitName", typeof(string));

            DataTable employee_dt = await SQLStore_QLNS.Instance.GetEmployeesAsync();
            DataTable workType_dt = await SQLStore_KhoVatTu.Instance.GetWorkTypeAsync();
            DataTable material_dt = await SQLStore_KhoVatTu.Instance.getMaterialAsync();
            foreach (DataRow row in data.Rows)
            {
                int? plantingID = row.Field<int?>("PlantingID");
                int? workTypeID = row.Field<int?>("WorkTypeID");
                int? materialID = row.Field<int?>("MaterialID");
                string Receiver = row["Receiver"].ToString();

                if (plantingID.HasValue)
                {
                    DateTime NurseryDate = Convert.ToDateTime(row["NurseryDate"]);
                    DataTable planting_dt = await SQLStore_KhoVatTu.Instance.getPlantingManagementAsync(NurseryDate.Year);
                    DataRow plantingRow = planting_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("PlantingID") == plantingID);
                    if (plantingRow != null)
                        row["PlantName"] = plantingRow["PlantName"];
                }

                DataRow employeeRow = employee_dt.AsEnumerable().FirstOrDefault(r => r.Field<string>("EmployeeCode") == Receiver);
                if (employeeRow != null)
                    row["RecieverName"] = employeeRow["FullName"];

                if (workTypeID.HasValue)
                {
                    DataRow workTypeRow = workType_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("WorkTypeID") == workTypeID);
                    if (workTypeRow != null)
                        row["WorkTypeName"] = workTypeRow["WorkTypeName"];
                }

                if (materialID.HasValue)
                {
                    DataRow materialRow = material_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("MaterialID") == materialID);
                    if (materialRow != null)
                    {
                        row["UnitName"] = materialRow["UnitName"];
                        row["MaterialName"] = materialRow["MaterialName"];
                    }
                }
            }

            Utils.SetGridOrdinal(data, new[] { "ExportDate", "MaterialName", "UnitName", "Amount", "ProductionOrder", "PlantName", "WorkTypeName", "RecieverName", "IsCompleted", "Note" });
        }

        public void removeMaterialImport(int month, int year)
        {
            string key = $"{month}_{year}";
            mMaterialImports.Remove(key);
        }

        public async Task<DataTable> GetMaterialImportAsync(int month, int year)
        {
            string key = $"{month}_{year}";
            if (!mMaterialImports.ContainsKey(key))
            {
                try
                {
                    var data = await SQLManager_KhoVatTu.Instance.GetMaterialImportAsync(month, year);
                    editMaterialImport(data, month, year);
                    mMaterialImports[key] = data;
                }
                catch
                {
                    Console.WriteLine("error GetMaterialExportAsync SQLStore");
                    return null;
                }
            }

            return mMaterialImports[key];
        }

        private async void editMaterialImport(DataTable data, int month, int year)
        {
            data.Columns.Add("MaterialName", typeof(string));
            data.Columns.Add("UnitName", typeof(string));
            data.Columns.Add("TotalMoney", typeof(int));

            DataTable material_dt = await SQLStore_KhoVatTu.Instance.getMaterialAsync();
            foreach (DataRow row in data.Rows)
            {
                int? materialID = row.Field<int?>("MaterialID");

                if (materialID.HasValue)
                {
                    DataRow materialRow = material_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("MaterialID") == materialID);
                    if (materialRow != null)
                    {
                        row["UnitName"] = materialRow["UnitName"];
                        row["MaterialName"] = materialRow["MaterialName"];
                    }

                    decimal amount = Convert.ToDecimal(row["Amount"]);
                    int? price = row.Field<int?>("Price");
                    if (price.HasValue)
                    {
                        row["TotalMoney"] = Convert.ToInt32(amount * price);
                    }
                }
            }

            Utils.SetGridOrdinal(data, new[] { "ImportDate", "MaterialName", "UnitName", "Amount", "Price", "TotalMoney", "Note" });
        }

        public async Task<DataTable> GetMaterialExportLogAsync(int month, int year)
        {
            string key = $"{month}_{year}";
            if (!mMaterialExportLogs.ContainsKey(key))
            {
                try
                {
                    var data = await SQLManager_KhoVatTu.Instance.GetMaterialExportLogAsync(month, year);
                    mMaterialExportLogs[key] = data;
                }
                catch
                {
                    Console.WriteLine("error GetMaterialExportLogAsync SQLStore");
                    return null;
                }
            }

            return mMaterialExportLogs[key];
        }

        public async Task<DataTable> GetMaterialImportLogAsync(int month, int year)
        {
            string key = $"{month}_{year}";
            if (!mMaterialImportLogs.ContainsKey(key))
            {
                try
                {
                    var data = await SQLManager_KhoVatTu.Instance.GetMaterialImportLogAsync(month, year);
                    mMaterialImportLogs[key] = data;
                }
                catch
                {
                    Console.WriteLine("error GetMaterialExportLogAsync SQLStore");
                    return null;
                }
            }

            return mMaterialImportLogs[key];
        }

        public async Task<DataTable> getPlantingManagementLogAsync(int year)
        {
            if (!mPlantingManagementLogs.ContainsKey(year))
            {
                try
                {
                    var data = await SQLManager_KhoVatTu.Instance.getPlantingManagementLogAsync(year);
                    mPlantingManagementLogs[year] = data;
                }
                catch
                {
                    Console.WriteLine("error GetMaterialExportLogAsync SQLStore");
                    return null;
                }
            }

            return mPlantingManagementLogs[year];
        }

        public async Task<DataTable> GetCultivationProcessTemplateAsync()
        {
            if (mCultivationProcessTemplate_dt == null)
            {
                try
                {
                    mCultivationProcessTemplate_dt = await SQLManager_KhoVatTu.Instance.GetCultivationProcessTemplateAsync();
                    editCultivationProcessTemplate(mCultivationProcessTemplate_dt);
                }
                catch
                {
                    Console.WriteLine("error GetMaterialCategoryAsync SQLStore");
                    return null;
                }
            }

            return mCultivationProcessTemplate_dt;
        }

        private async void editCultivationProcessTemplate(DataTable data)
        {
            data.Columns.Add("ProductNameVN", typeof(string));
            data.Columns.Add("CultivationTypeName", typeof(string));
            data.Columns.Add("BaseDateTypeName", typeof(string));
            data.Columns.Add("WorkTypeName", typeof(string));
            data.Columns.Add("MaterialName", typeof(string));

            DataTable baseDateType_dt = GetBaseDateType();
            DataTable productSKU_dt = await SQLStore_Kho.Instance.getProductSKUAsync();
            DataTable cultivationType_dt = await GetCultivationTypeAsync();
            DataTable workType_dt = await GetWorkTypeAsync();
            DataTable material_dt = await getMaterialAsync();
            foreach (DataRow row in data.Rows)
            {
                int sku = Convert.ToInt32(row["SKU"]);
                int cultivationTypeID = Convert.ToInt32(row["CultivationTypeID"]);
                string baseDateType = Convert.ToString(row["BaseDateType"]);
                int workTypeID = Convert.ToInt32(row["WorkTypeID"]);
                int? materialID = row["MaterialID"] != DBNull.Value ? Convert.ToInt32(row["MaterialID"]) : (int?)null;

                DataRow baseDateTypeRow = baseDateType_dt.AsEnumerable().FirstOrDefault(r => r.Field<string>("Value") == baseDateType);
                if (baseDateTypeRow != null)
                    row["BaseDateTypeName"] = baseDateTypeRow["Text"].ToString();

                DataRow skuRow = productSKU_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("ProductSKU") == sku);
                if (skuRow != null)
                    row["ProductNameVN"] = skuRow["ProductNameVN"].ToString();

                DataRow cultivationTypeRow = cultivationType_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("CultivationTypeID") == cultivationTypeID);
                if (cultivationTypeRow != null)
                    row["CultivationTypeName"] = cultivationTypeRow["CultivationTypeName"].ToString();

                DataRow workTypeRow = workType_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("WorkTypeID") == workTypeID);
                if (workTypeRow != null)
                    row["WorkTypeName"] = workTypeRow["WorkTypeName"].ToString();

                if (materialID.HasValue)
                {
                    DataRow materialRow = material_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("MaterialID") == materialID);
                    if (materialRow != null)
                        row["MaterialName"] = materialRow["MaterialName"].ToString();
                }
            }
        }

        public async Task<DataTable> GetCultivationProcessAsync(int plantingID)
        {
            if (!mCultivationProcesses.ContainsKey(plantingID))
            {
                try
                {
                    DataTable data = await SQLManager_KhoVatTu.Instance.GetCultivationProcessAsync(plantingID);
                    editCultivationProcess(data);

                    mCultivationProcesses[plantingID] = data;
                }
                catch
                {
                    Console.WriteLine("error GetMaterialCategoryAsync SQLStore");
                    return null;
                }
            }

            return mCultivationProcesses[plantingID];
        }

        private async void editCultivationProcess(DataTable data)
        {
            data.Columns.Add("WorkTypeName", typeof(string));
            data.Columns.Add("MaterialName", typeof(string));
            data.Columns.Add("DepartmentName", typeof(string));
            data.Columns.Add("EmployeeName", typeof(string));
            data.Columns.Add("IsolationEndDate", typeof(string));
            foreach (DataRow row in data.Rows) 
            {
                
            }
        }
    }
}
