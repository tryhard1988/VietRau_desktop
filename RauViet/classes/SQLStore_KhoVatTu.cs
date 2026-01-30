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
        DataTable mUnit_dt = null;
        DataTable mMaterialCategory_dt = null;
        DataTable mMaterial_dt = null;
        DataTable mPlantingManagement_dt = null;

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

                var unitTask = SQLManager_KhoVatTu.Instance.GetUnitAsync();
                var MaterialCategoryTask = SQLManager_KhoVatTu.Instance.GetMaterialCategoryAsync();
                var MaterialTask = SQLManager_KhoVatTu.Instance.GetMaterialAsync();

                await Task.WhenAll(unitTask, MaterialCategoryTask, MaterialTask);

                if (unitTask.Status == TaskStatus.RanToCompletion && unitTask.Result != null) mUnit_dt = unitTask.Result;
                if (MaterialCategoryTask.Status == TaskStatus.RanToCompletion && MaterialCategoryTask.Result != null) mMaterialCategory_dt = MaterialCategoryTask.Result;
                if (MaterialTask.Status == TaskStatus.RanToCompletion && MaterialTask.Result != null) mMaterial_dt = MaterialTask.Result;

                editMaterial(mMaterial_dt);
                editCategory(mMaterialCategory_dt);
                editUnit(mUnit_dt);
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

                row["MaterialName_nosign"] = Utils.RemoveVietnameseSigns(row["MaterialName"].ToString());
                if (unitRow != null)
                {
                    row["UnitName"] = unitRow["UnitName"].ToString();
                }

                if (categoryRow != null)
                {
                    row["CategoryName"] = categoryRow["CategoryName"].ToString();
                }
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

        public async Task<DataTable> getPlantingManagementAsync()
        {
            if (mPlantingManagement_dt == null)
            {
                try
                {
                    mPlantingManagement_dt = await SQLManager_KhoVatTu.Instance.getPlantingManagementAsync();
                    editPlantingManagement(mPlantingManagement_dt);
                }
                catch
                {
                    Console.WriteLine("error getMaterialAsync SQLStore");
                    return null;
                }
            }

            return mPlantingManagement_dt;
        }

        private async void editPlantingManagement(DataTable data)
        {
            data.Columns.Add("DepartmentName", typeof(string));
            data.Columns.Add("PlantName", typeof(string));
            data.Columns.Add("SupervisorName", typeof(string));

            DataTable employee_dt = await SQLStore_QLNS.Instance.GetEmployeesAsync();
            DataTable department_dt = await SQLStore_QLNS.Instance.GetDepartmentAsync();
            DataTable product_dt = await SQLStore_Kho.Instance.getProductSKUAsync();
            foreach (DataRow row in data.Rows)
            {
                int sku = Convert.ToInt32(row["SKU"]);
                int department = Convert.ToInt32(row["Department"]);
                string Supervisor = row["Supervisor"].ToString();
                
                DataRow productRow = product_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("SKU") == sku);
                if (productRow != null)
                    row["PlantName"] = productRow["ProductNameVN"];

                DataRow employeeRow = employee_dt.AsEnumerable().FirstOrDefault(r => r.Field<string>("EmployeeCode") == Supervisor);
                if(employeeRow != null)
                    row["SupervisorName"] = employeeRow["FullName"];

                DataRow departmentRow = department_dt.AsEnumerable().FirstOrDefault(r => r.Field<int>("DepartmentID") == department);
                if (departmentRow != null)
                    row["DepartmentName"] = departmentRow["DepartmentName"];
            }
        }
    }
    
}
