
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using DataTable = System.Data.DataTable;

namespace RauViet.classes
{
    public sealed class SQLManager_KhoVatTu
    {
        private static SQLManager_KhoVatTu ins = null;
        private static readonly object padlock = new object();

        private SQLManager_KhoVatTu() { }

        public static SQLManager_KhoVatTu Instance
        {
            get
            {
                lock (padlock)
                {
                    if (ins == null)
                        ins = new SQLManager_KhoVatTu();
                    return ins;
                }
            }
        }

        public string ql_khoVatTu_conStr() { return "Server=192.168.1.8,1433;Database=QL_KhoVatTu;User Id=ql_kho;Password=A7t#kP2x;"; }

        //==============================Customers==============================

        public async Task<int> insertMaterialCategoryAsync(string name, string code)
        {
            int newId = -1;

            string insertQuery = @"INSERT INTO MaterialCategory (CategoryCode, CategoryName)
                                    OUTPUT INSERTED.CategoryID
                                    VALUES (@CategoryCode, @CategoryName)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@CategoryCode", code);
                        cmd.Parameters.AddWithValue("@CategoryName", name);

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }

                return newId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> insertWorkTypeAsync(string name)
        {
            int newId = -1;

            string insertQuery = @"INSERT INTO WorkType (WorkTypeName)
                                    OUTPUT INSERTED.WorkTypeID
                                    VALUES (@WorkTypeName)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@WorkTypeName", name);

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }

                return newId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> insertCropAsync(string name)
        {
            int newId = -1;

            string insertQuery = @"INSERT INTO Crop (CropName)
                                    OUTPUT INSERTED.CropID
                                    VALUES (@CropName)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@CropName", name);

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }

                return newId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> insertUnitAsync(string name)
        {
            int newId = -1;

            string insertQuery = @"INSERT INTO Units (UnitName)
                                    OUTPUT INSERTED.UnitID
                                    VALUES (@UnitName)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UnitName", name);

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }

                return newId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> insertMaterialsAsync(string MaterialName, int CategoryID, int UnitID)
        {
            int newId = -1;

            string insertQuery = @"INSERT INTO Materials (MaterialName, CategoryID, UnitID)
                                    OUTPUT INSERTED.MaterialID
                                    VALUES (@MaterialName, @CategoryID, @UnitID)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@MaterialName", MaterialName);
                        cmd.Parameters.AddWithValue("@CategoryID", CategoryID);
                        cmd.Parameters.AddWithValue("@UnitID", UnitID);

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }

                return newId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<bool> updateMaterialsAsync(int MaterialID, string MaterialName, int CategoryID, int UnitID)
        {
            string query = @"UPDATE Materials SET
                                MaterialName=@MaterialName,
                                CategoryID=@CategoryID,
                                UnitID=@UnitID
                             WHERE MaterialID=@MaterialID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@MaterialID", MaterialID);
                        cmd.Parameters.AddWithValue("@MaterialName", MaterialName);
                        cmd.Parameters.AddWithValue("@CategoryID", CategoryID);
                        cmd.Parameters.AddWithValue("@UnitID", UnitID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> deletetMaterialsAsync(int MaterialID)
        {
            string query = "DELETE FROM Materials WHERE MaterialID=@MaterialID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@MaterialID", MaterialID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetMaterialAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Materials";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetUnitAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Units";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }
        public async Task<DataTable> GetWorkTypeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM WorkType";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetMaterialCategoryAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM MaterialCategory";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> getPlantingManagementAsync(int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM PlantingManagement WHERE YEAR(NurseryDate) = @Year;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> getPlantingManagementAsync(bool isComplete)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM PlantingManagement WHERE IsCompleted = @IsCompleted;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@IsCompleted", SqlDbType.Bit).Value = isComplete;
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<int> insertPlantingManagementAsync(string ProductionOrder, int SKU, decimal Area, int Quantity, DateTime NurseryDate, DateTime PlantingDate, DateTime HarvestDate, int? Department, string Supervisor, string Note)
        {
            int newId = -1;

            string insertQuery = @"INSERT INTO PlantingManagement (ProductionOrder, SKU, Area, Quantity, NurseryDate, PlantingDate, HarvestDate, Department, Supervisor, Note)
                                    OUTPUT INSERTED.PlantingID
                                    VALUES (@ProductionOrder, @SKU, @Area, @Quantity, @NurseryDate, @PlantingDate, @HarvestDate, @Department, @Supervisor, @Note)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.Add("@ProductionOrder", SqlDbType.NVarChar).Value = ProductionOrder;
                        cmd.Parameters.Add("@SKU", SqlDbType.Int).Value = SKU;
                        cmd.Parameters.Add("@Area", SqlDbType.Decimal).Value = Area;
                        cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = Quantity;
                        cmd.Parameters.Add("@NurseryDate", SqlDbType.Date).Value = NurseryDate;
                        cmd.Parameters.Add("@PlantingDate", SqlDbType.Date).Value = PlantingDate;
                        cmd.Parameters.Add("@HarvestDate", SqlDbType.Date).Value = HarvestDate;
                        cmd.Parameters.Add("@Department", SqlDbType.Int).Value = Department.HasValue ? (object)Department.Value : DBNull.Value;
                        cmd.Parameters.Add("@Supervisor", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(Supervisor) ? DBNull.Value : (object)Supervisor;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Note;


                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }

                return newId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<bool> updatePlantingManagementAsync(int PlantingID, string ProductionOrder, int SKU, decimal Area, int Quantity, DateTime NurseryDate, DateTime PlantingDate, DateTime HarvestDate, int Department, string Supervisor, string Note,  bool IsCompleted)
        {
            string query = @"UPDATE PlantingManagement SET
                                ProductionOrder=@ProductionOrder,
                                SKU=@SKU,
                                Area=@Area,
                                Quantity=@Quantity,
                                NurseryDate=@NurseryDate,
                                PlantingDate=@PlantingDate,
                                HarvestDate=@HarvestDate,
                                Department=@Department,
                                Supervisor=@Supervisor,
                                Note=@Note,
                                IsCompleted=@IsCompleted
                             WHERE PlantingID=@PlantingID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PlantingID", PlantingID);
                        cmd.Parameters.AddWithValue("@ProductionOrder", ProductionOrder);
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@Area", Area);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@NurseryDate", NurseryDate);
                        cmd.Parameters.AddWithValue("@PlantingDate", PlantingDate);
                        cmd.Parameters.AddWithValue("@HarvestDate", HarvestDate);
                        cmd.Parameters.AddWithValue("@Department", Department);
                        cmd.Parameters.AddWithValue("@Supervisor", Supervisor);
                        cmd.Parameters.AddWithValue("@Note", Note);
                        cmd.Parameters.AddWithValue("@IsCompleted", IsCompleted);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> deletetPlantingManagementAsync(int PlantingID)
        {
            string query = "DELETE FROM PlantingManagement WHERE PlantingID=@PlantingID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PlantingID", PlantingID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertMaterialExportAsync(DateTime ExportDate, int MaterialID, decimal Amount, int? PlantingID, int? WorkTypeID, string Receiver, string Note)
        {
            int newId = -1;

            string insertQuery = @"INSERT INTO MaterialExport (ExportDate, MaterialID, Amount, PlantingID, WorkTypeID, Receiver, Note)
                                    OUTPUT INSERTED.ExportID
                                    VALUES (@ExportDate, @MaterialID, @Amount, @PlantingID, @WorkTypeID, @Receiver, @Note)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.Add("@ExportDate", SqlDbType.DateTime).Value = ExportDate;
                        cmd.Parameters.Add("@MaterialID", SqlDbType.Int).Value = MaterialID;
                        cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = PlantingID.HasValue ? (object)PlantingID.Value : DBNull.Value;
                        cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = WorkTypeID.HasValue ? (object)WorkTypeID.Value : DBNull.Value;
                        cmd.Parameters.Add("@Receiver", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(Receiver) ? (object)DBNull.Value : Receiver;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Note;

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }

                return newId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return -1;
            }
        }

        public async Task<bool> updateMaterialExportAsync(int ExportID, DateTime ExportDate, int MaterialID, decimal Amount, int? PlantingID, int? WorkTypeID, string Receiver, string Note)
        {
            string query = @"UPDATE MaterialExport SET
                                ExportDate=@ExportDate,
                                MaterialID=@MaterialID,
                                Amount=@Amount,
                                PlantingID=@PlantingID,
                                WorkTypeID=@WorkTypeID,
                                Receiver=@Receiver,
                                Note=@Note
                             WHERE ExportID=@ExportID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@ExportID", SqlDbType.Int).Value = ExportID;
                        cmd.Parameters.Add("@ExportDate", SqlDbType.DateTime).Value = ExportDate;
                        cmd.Parameters.Add("@MaterialID", SqlDbType.Int).Value = MaterialID;
                        cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = PlantingID.HasValue ? (object)PlantingID.Value : DBNull.Value;
                        cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = WorkTypeID.HasValue ? (object)WorkTypeID.Value : DBNull.Value;
                        cmd.Parameters.Add("@Receiver", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(Receiver) ? (object)DBNull.Value : Receiver;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Note;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetMaterialExportAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT me.*, pm.NurseryDate, pm.ProductionOrder, pm.IsCompleted FROM MaterialExport me
                                                            LEFT JOIN PlantingManagement pm ON me.PlantingID = pm.PlantingID
                                                            WHERE me.ExportDate >= DATEFROMPARTS(@Year, @Month, 1) AND me.ExportDate <  DATEADD(MONTH, 1, DATEFROMPARTS(@Year, @Month, 1))";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<bool> deletetMaterialExportAsync(int ExportID)
        {
            string query = "DELETE FROM MaterialExport WHERE ExportID=@ExportID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExportID", ExportID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }
    }
}

