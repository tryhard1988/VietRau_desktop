
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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
        public string ql_khoVatTuLog_conStr() { return "Server=192.168.1.8,1433;Database=QL_KhoVatTu_Log;User Id=ql_kho;Password=A7t#kP2x;"; }
                
        public async Task<int> insertMaterialsAsync(string MaterialName, int CategoryID, int UnitID, string composition)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertMaterials", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@MaterialName", SqlDbType.NVarChar).Value = MaterialName;
                        cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = CategoryID;
                        cmd.Parameters.Add("@UnitID", SqlDbType.Int).Value = UnitID;
                        cmd.Parameters.Add("@Composition", SqlDbType.NVarChar).Value =
                            string.IsNullOrWhiteSpace(composition) ? (object)DBNull.Value : composition;

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

        public async Task<bool> updateMaterialsAsync(int MaterialID, string MaterialName, int CategoryID, int UnitID, string composition)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateMaterials", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@MaterialID", SqlDbType.Int).Value = MaterialID;
                        cmd.Parameters.Add("@MaterialName", SqlDbType.NVarChar).Value = MaterialName;
                        cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = CategoryID;
                        cmd.Parameters.Add("@UnitID", SqlDbType.Int).Value = UnitID;
                        cmd.Parameters.Add("@Composition", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(composition) ? (object)DBNull.Value : composition;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
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

                using (SqlCommand cmd = new SqlCommand("sp_GetMaterials", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

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

        public async Task<DataTable> GetMaterialDepartmentAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM MaterialDepartment";

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

        public async Task<DataTable> GetGrowthStageAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM GrowthStage";

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

        public async Task<DataTable> GetPestDiseaseAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM PestDisease";

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

        public async Task<DataTable> GetCultivationTypeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM CultivationType";

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

        public async Task<DataTable> GetFarmsAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Farms";

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
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_GetPlantingManagementWithHarvest", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Truyền parameter từ C#
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                        cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = Utils.WorkType_ThuHoach();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            return dt;
        }

        public async Task<DataTable> getPlantingManagementAsync_ProductionOrder(List<string> productionOrders)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();

                var paramNames = productionOrders.Select((x, i) => $"@p{i}").ToList();
                string query = $@"SELECT * FROM PlantingManagement WHERE ProductionOrder IN ({string.Join(",", paramNames)})";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    for (int i = 0; i < productionOrders.Count; i++)
                    {
                        cmd.Parameters.AddWithValue(paramNames[i], productionOrders[i]);
                    }

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<DataTable> getPlantingManagementAsync_HarvestQuantityMonth(int year)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_GetPlantingHarvestQuantityByYear", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = Utils.WorkType_ThuHoach();
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            return dt;
        }

        public async Task<DataTable> getPlantingManagementAsync_HarvestQuantity()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_GetPlantingHarvestQuantity", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Truyền parameter WorkTypeID (Thu Hoach)
                        cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = Utils.WorkType_ThuHoach();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            return dt;
        }


        public async Task<DataTable> getPlantingManagementAsync(bool isComplete)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetPlantingManagement", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@IsCompleted", SqlDbType.Bit).Value = isComplete;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<int> insertPlantingManagementAsync(string ProductionOrder, int SKU, decimal Area, int Quantity, DateTime NurseryDate, DateTime PlantingDate, DateTime HarvestDate, int? Department, string Supervisor, string Note, int cultivationTypeID, int farmId)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertPlantingManagement", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ProductionOrder", SqlDbType.NVarChar).Value = ProductionOrder;
                        cmd.Parameters.Add("@SKU", SqlDbType.Int).Value = SKU;
                        cmd.Parameters.Add("@Area", SqlDbType.Decimal).Value = Area;
                        cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = Quantity;
                        cmd.Parameters.Add("@NurseryDate", SqlDbType.Date).Value = NurseryDate;
                        cmd.Parameters.Add("@PlantingDate", SqlDbType.Date).Value = PlantingDate;
                        cmd.Parameters.Add("@HarvestDate", SqlDbType.Date).Value = HarvestDate;
                        cmd.Parameters.Add("@Department", SqlDbType.Int).Value = (object)Department ?? DBNull.Value;
                        cmd.Parameters.Add("@CultivationTypeID", SqlDbType.Int).Value = cultivationTypeID;
                        cmd.Parameters.Add("@Supervisor", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(Supervisor) ? (object)DBNull.Value : Supervisor;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Note;
                        cmd.Parameters.Add("@FarmID", SqlDbType.Int).Value = farmId;

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

        public async Task<bool> updatePlantingManagementAsync(int PlantingID, string ProductionOrder, int SKU, decimal Area, int Quantity, DateTime NurseryDate, DateTime PlantingDate, DateTime HarvestDate, int Department, string Supervisor, string Note,  bool IsCompleted, int cultivationTypeID, int farmId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdatePlantingManagement", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = PlantingID;
                        cmd.Parameters.Add("@ProductionOrder", SqlDbType.NVarChar).Value = ProductionOrder;
                        cmd.Parameters.Add("@SKU", SqlDbType.Int).Value = SKU;
                        cmd.Parameters.Add("@Area", SqlDbType.Decimal).Value = Area;
                        cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = Quantity;
                        cmd.Parameters.Add("@NurseryDate", SqlDbType.Date).Value = NurseryDate;
                        cmd.Parameters.Add("@PlantingDate", SqlDbType.Date).Value = PlantingDate;
                        cmd.Parameters.Add("@HarvestDate", SqlDbType.Date).Value = HarvestDate;
                        cmd.Parameters.Add("@Department", SqlDbType.Int).Value = Department;
                        cmd.Parameters.Add("@CultivationTypeID", SqlDbType.Int).Value = cultivationTypeID;
                        cmd.Parameters.Add("@Supervisor", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(Supervisor) ? (object)DBNull.Value : Supervisor;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Note;
                        cmd.Parameters.Add("@IsCompleted", SqlDbType.Bit).Value = IsCompleted;
                        cmd.Parameters.Add("@FarmID", SqlDbType.Int).Value = farmId;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
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

        public async Task<int> insertMaterialExportAsync(DateTime ExportDate, int MaterialID, decimal Amount, int? PlantingID, int? WorkTypeID, string Receiver, string Note, int MaterialDepartmentID)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertMaterialExport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ExportDate", SqlDbType.DateTime).Value = ExportDate;
                        cmd.Parameters.Add("@MaterialID", SqlDbType.Int).Value = MaterialID;
                        cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
                        cmd.Parameters.Add("@MaterialDepartmentID", SqlDbType.Int).Value = MaterialDepartmentID;
                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = (object)PlantingID ?? DBNull.Value;
                        cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = (object)WorkTypeID ?? DBNull.Value;
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

        public async Task<bool> InsertMaterialExportBatchAsync(List<(DateTime ExportDate, int MaterialID, decimal Amount, int? PlantingID, int? WorkTypeID, string Receiver, string Note, int MaterialDepartmentID)> list)
        {
            if (list == null || list.Count == 0)
                return false;

            bool isSuccess = false;

            try
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("ExportDate", typeof(DateTime));
                dt.Columns.Add("MaterialID", typeof(int));
                dt.Columns.Add("Amount", typeof(decimal));
                dt.Columns.Add("PlantingID", typeof(int));
                dt.Columns.Add("WorkTypeID", typeof(int));
                dt.Columns.Add("Receiver", typeof(string));
                dt.Columns.Add("Note", typeof(string));
                dt.Columns.Add("MaterialDepartmentID", typeof(int));

                foreach (var item in list)
                {
                    dt.Rows.Add(
                        item.ExportDate,
                        item.MaterialID,
                        item.Amount,
                        item.PlantingID ?? (object)DBNull.Value,
                        item.WorkTypeID ?? (object)DBNull.Value,
                        string.IsNullOrWhiteSpace(item.Receiver) ? (object)DBNull.Value : item.Receiver,
                        item.Note ?? (object)DBNull.Value,
                        item.MaterialDepartmentID
                    );
                }

                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertMaterialExport_Batch", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter param = cmd.Parameters.AddWithValue("@MaterialExports", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "MaterialExportType";

                        object result = await cmd.ExecuteScalarAsync();

                        if (result != null)
                            isSuccess = Convert.ToInt32(result) == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            return isSuccess;
        }

        public async Task<bool> updateMaterialExportAsync(int ExportID, DateTime ExportDate, int MaterialID, decimal Amount, int? PlantingID, int? WorkTypeID, string Receiver, string Note, int MaterialDepartmentID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateMaterialExport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ExportID", SqlDbType.Int).Value = ExportID;
                        cmd.Parameters.Add("@ExportDate", SqlDbType.DateTime).Value = ExportDate;
                        cmd.Parameters.Add("@MaterialID", SqlDbType.Int).Value = MaterialID;
                        cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
                        cmd.Parameters.Add("@MaterialDepartmentID", SqlDbType.Int).Value = MaterialDepartmentID;
                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = (object)PlantingID ?? DBNull.Value;
                        cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = (object)WorkTypeID ?? DBNull.Value;
                        cmd.Parameters.Add("@Receiver", SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(Receiver) ? (object)DBNull.Value : Receiver;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Note;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DataTable> GetMaterialExportAsync(int month, int year)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetMaterialExport_ByMonth", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

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

        public async Task<int> insertMaterialImportAsync(DateTime ImportDate, int MaterialID, decimal Amount, int? Price, string Note)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertMaterialImport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ImportDate", SqlDbType.DateTime).Value = ImportDate;
                        cmd.Parameters.Add("@MaterialID", SqlDbType.Int).Value = MaterialID;
                        cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
                        cmd.Parameters.Add("@Price", SqlDbType.Int).Value = (object)Price ?? DBNull.Value;
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

        public async Task<bool> updateMaterialImportAsync(int ImportID, DateTime ImportDate, int MaterialID, decimal Amount, int? Price, string Note)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateMaterialImport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ImportID", SqlDbType.Int).Value = ImportID;
                        cmd.Parameters.Add("@ImportDate", SqlDbType.DateTime).Value = ImportDate;
                        cmd.Parameters.Add("@MaterialID", SqlDbType.Int).Value = MaterialID;
                        cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Amount;
                        cmd.Parameters.Add("@Price", SqlDbType.Int).Value = (object)Price ?? DBNull.Value;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar).Value = Note;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DataTable> GetMaterialImportAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_GetMaterialImportByMonth", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            return dt;
        }

        public async Task<DataTable> GetMaterialPriceAsync()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_GetMaterialPrice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            return dt;
        }

        public async Task<bool> deletetMaterialImportAsync(int ImportID)
        {
            string query = "DELETE FROM MaterialImport WHERE ImportID=@ImportID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ImportID", ImportID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetSumaryMaterialImportAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetSummaryMaterialImport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<DataTable> GetSumaryMaterialExportAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetSummaryMaterialExport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<DataTable> GetMaterialExportLogAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM MaterialExportLog WHERE YEAR(ExportDate) = @Year AND MONTH(ExportDate) = @Month;";

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

        public async Task<int> insertMaterialExportLogAsync(DateTime ExportDate, string oldValue, string newValue)
        {
            int newId = -1;
            string insertQuery = @"INSERT INTO MaterialExportLog (ExportDate, OldValue, NewValue, ActionBy)
                                    OUTPUT INSERTED.LogID
                                    VALUES (@ExportDate, @OldValue, @NewValue, @ActionBy)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.Add("@ExportDate", SqlDbType.DateTime).Value = ExportDate;
                        cmd.Parameters.Add("@OldValue", SqlDbType.NChar).Value = oldValue;
                        cmd.Parameters.Add("@NewValue", SqlDbType.NChar).Value = newValue;
                        cmd.Parameters.Add("@ActionBy", SqlDbType.NChar).Value = UserManager.Instance.fullName;
                       
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

        public async Task<DataTable> GetMaterialImportLogAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM MaterialImportLog WHERE YEAR(ImportDate) = @Year AND MONTH(ImportDate) = @Month;";

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

        public async Task<int> insertMaterialImportLogAsync(DateTime ImportDate, string oldValue, string newValue)
        {
            int newId = -1;
            string insertQuery = @"INSERT INTO MaterialImportLog (ImportDate, OldValue, NewValue, ActionBy)
                                    OUTPUT INSERTED.LogID
                                    VALUES (@ImportDate, @OldValue, @NewValue, @ActionBy)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.Add("@ImportDate", SqlDbType.DateTime).Value = ImportDate;
                        cmd.Parameters.Add("@OldValue", SqlDbType.NChar).Value = oldValue;
                        cmd.Parameters.Add("@NewValue", SqlDbType.NChar).Value = newValue;
                        cmd.Parameters.Add("@ActionBy", SqlDbType.NChar).Value = UserManager.Instance.fullName;

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

        public async Task<DataTable> getPlantingManagementLogAsync(int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM PlantingManagement_Log WHERE YEAR(NurseryDate) = @Year";

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
        public async Task<int> insertCultivationProcessTemplateAsync(int sku, int cultivationTypeID, string baseDateType, int daysAfter, int? fertilizationWorkTypeID, int workTypeID, int? materialID, decimal materialQuantity, decimal waterAmount, bool isMultiplyArea)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertCultivationProcessTemplate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@SKU", SqlDbType.Int).Value = sku;
                        cmd.Parameters.Add("@CultivationTypeID", SqlDbType.Int).Value = cultivationTypeID;
                        cmd.Parameters.Add("@BaseDateType", SqlDbType.NVarChar).Value = baseDateType;
                        cmd.Parameters.Add("@DaysAfter", SqlDbType.Int).Value = daysAfter;
                        cmd.Parameters.Add("@FertilizationWorkTypeID", SqlDbType.Int).Value = (object)fertilizationWorkTypeID ?? DBNull.Value;
                        cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID;
                        cmd.Parameters.Add("@MaterialID", SqlDbType.Int).Value = (object)materialID ?? DBNull.Value;
                        cmd.Parameters.Add("@MaterialQuantity", SqlDbType.Decimal).Value = materialQuantity;
                        cmd.Parameters.Add("@WaterAmount", SqlDbType.Decimal).Value = waterAmount;
                        cmd.Parameters.Add("@IsMultiplyArea", SqlDbType.Bit).Value = isMultiplyArea;

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

        public async Task<bool> updateCultivationProcessTemplateAsync(int processTemplateID, int sku, int cultivationTypeID, string baseDateType, int daysAfter, int? fertilizationWorkTypeID, int workTypeID, int? materialID, decimal materialQuantity, decimal waterAmount, bool isMultiplyArea)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateCultivationProcessTemplate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ProcessTemplateID", SqlDbType.Int).Value = processTemplateID;
                        cmd.Parameters.Add("@SKU", SqlDbType.Int).Value = sku;
                        cmd.Parameters.Add("@CultivationTypeID", SqlDbType.Int).Value = cultivationTypeID;
                        cmd.Parameters.Add("@BaseDateType", SqlDbType.NVarChar).Value = baseDateType;
                        cmd.Parameters.Add("@DaysAfter", SqlDbType.Int).Value = daysAfter;
                        cmd.Parameters.Add("@FertilizationWorkTypeID", SqlDbType.Int).Value = (object)fertilizationWorkTypeID ?? DBNull.Value;
                        cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID;
                        cmd.Parameters.Add("@MaterialID", SqlDbType.Int).Value = (object)materialID ?? DBNull.Value;
                        cmd.Parameters.Add("@MaterialQuantity", SqlDbType.Decimal).Value = materialQuantity;
                        cmd.Parameters.Add("@WaterAmount", SqlDbType.Decimal).Value = waterAmount;
                        cmd.Parameters.Add("@IsMultiplyArea", SqlDbType.Bit).Value = isMultiplyArea;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> deletetCultivationProcessTemplateAsync(int ProcessTemplateID)
        {
            string query = "DELETE FROM CultivationProcessTemplate WHERE ProcessTemplateID=@ProcessTemplateID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProcessTemplateID", ProcessTemplateID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetCultivationProcessTemplateAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetCultivationProcessTemplate", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<DataTable> GetCultivationProcessAsync(int plantingID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetCultivationProcess", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<DataTable> GetCultivationProcessAsync(int month, int year, int departmentID)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_GetCultivationProcess1", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                        cmd.Parameters.Add("@Department", SqlDbType.Int).Value = departmentID;

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }

            return dt;
        }

        public async Task<DataTable> GetCultivationProcessAsync(DateTime date)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetCultivationProcess_ByDate", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Date", SqlDbType.Date).Value = date.Date;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<bool> InsertCultivationProcessListAsync(List<(int PlantingID, DateTime ProcessDate, int? FertilizationWorkTypeID, int WorkTypeID, int? MaterialID, int? MaterialPrice, 
            decimal? MaterialQuantity, decimal? WaterAmount, int? DepartmentID, string EmployeeCode, string dosage, string activeIngredientStr, int? isolationDays)> list)
        {
            try
            {
                using (var con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    // 1️⃣ Chuẩn bị DataTable khớp với kiểu TVP
                    var dt = new DataTable();
                    dt.Columns.Add("PlantingID", typeof(int));
                    dt.Columns.Add("ProcessDate", typeof(DateTime));
                    dt.Columns.Add("FertilizationWorkTypeID", typeof(int));
                    dt.Columns.Add("WorkTypeID", typeof(int));
                    dt.Columns.Add("MaterialID", typeof(int));
                    dt.Columns.Add("MaterialPrice", typeof(int));
                    dt.Columns.Add("MaterialQuantity", typeof(decimal));
                    dt.Columns.Add("WaterAmount", typeof(decimal));
                    dt.Columns.Add("DepartmentID", typeof(int));
                    dt.Columns.Add("EmployeeCode", typeof(string));
                    dt.Columns.Add("Dosage", typeof(string));
                    dt.Columns.Add("ActiveIngredient", typeof(string));
                    dt.Columns.Add("IsolationDays", typeof(int));

                    foreach (var item in list)
                    {
                        dt.Rows.Add(item.PlantingID, 
                                    item.ProcessDate,
                                    (object)item.FertilizationWorkTypeID ?? DBNull.Value,
                                    item.WorkTypeID, 
                                    (object)item.MaterialID ?? DBNull.Value,
                                    (object)item.MaterialPrice ?? DBNull.Value,
                                    (object)item.MaterialQuantity ?? DBNull.Value,
                                    (object)item.WaterAmount ?? DBNull.Value,
                                    (object)item.DepartmentID ?? DBNull.Value,
                                    item.EmployeeCode,
                                    item.dosage,
                                    item.activeIngredientStr,
                                    (object)item.isolationDays ?? DBNull.Value);
                    }

                    // 2️⃣ Gọi SP
                    using (var cmd = new SqlCommand("dbo.sp_InsertCultivationProcessList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@ProcessList", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "dbo.CultivationProcessType";

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InsertCultivationProcessListAsync] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async Task<int> insertCultivationProcessAsync(
            int PlantingID,
            DateTime processDate,
            int fertilizationWorkTypeID,
            int workTypeID,
            int? materialID,
            int? MaterialPrice,
            decimal materialQuantity,
            string dosage,
            string plantStatus,
            string activeIngredient,
            string employeeCode,
            int isolationDays,
            int? departmentID,
            string plantLocation,
            decimal waterAmount,
            string note)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertCultivationProcess", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = PlantingID;
                        cmd.Parameters.Add("@MaterialID", SqlDbType.Int).Value = materialID.HasValue ? (object)materialID.Value : DBNull.Value;
                        cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID;
                        cmd.Parameters.Add("@ProcessDate", SqlDbType.Date).Value = processDate.Date;
                        cmd.Parameters.Add("@MaterialQuantity", SqlDbType.Decimal).Value = materialQuantity;
                        cmd.Parameters.Add("@WaterAmount", SqlDbType.Decimal).Value = waterAmount;
                        cmd.Parameters.Add("@Dosage", SqlDbType.NVarChar).Value = dosage;
                        cmd.Parameters.Add("@PlantStatus", SqlDbType.NVarChar).Value = plantStatus;
                        cmd.Parameters.Add("@ActiveIngredient", SqlDbType.NVarChar).Value = activeIngredient;
                        cmd.Parameters.Add("@EmployeeCode", SqlDbType.NVarChar).Value = employeeCode;
                        cmd.Parameters.Add("@IsolationDays", SqlDbType.Int).Value = isolationDays;
                        cmd.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = departmentID.HasValue ? (object)departmentID.Value : DBNull.Value;
                        cmd.Parameters.Add("@PlantLocation", SqlDbType.NVarChar).Value = plantLocation;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar).Value = note;
                        cmd.Parameters.Add("@FertilizationWorkTypeID", SqlDbType.Int).Value = fertilizationWorkTypeID;
                        cmd.Parameters.Add("@MaterialPrice", SqlDbType.Int).Value = MaterialPrice.HasValue ? (object)MaterialPrice.Value : DBNull.Value;

                        // OUTPUT parameter để lấy ID mới
                        var outputId = new SqlParameter("@NewID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputId);

                        await cmd.ExecuteNonQueryAsync();

                        if (outputId.Value != DBNull.Value)
                            newId = Convert.ToInt32(outputId.Value);
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

        public async Task<bool> updateCultivationProcessAsync(
            int CultivationProcessID,
            int PlantingID,
            DateTime processDate,
            int fertilizationWorkTypeID,
            int workTypeID,
            int? materialID,
            int? MaterialPrice,
            decimal materialQuantity,
            string dosage,
            string plantStatus,
            string activeIngredient,
            string employeeCode,
            int isolationDays,
            int? departmentID,
            string plantLocation,
            decimal waterAmount,
            string note)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateCultivationProcess", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@CultivationProcessID", SqlDbType.Int).Value = CultivationProcessID;
                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = PlantingID;
                        cmd.Parameters.Add("@MaterialID", SqlDbType.Int).Value = materialID.HasValue ? (object)materialID.Value : DBNull.Value;
                        cmd.Parameters.Add("@WorkTypeID", SqlDbType.Int).Value = workTypeID;
                        cmd.Parameters.Add("@ProcessDate", SqlDbType.Date).Value = processDate.Date;
                        cmd.Parameters.Add("@MaterialQuantity", SqlDbType.Decimal).Value = materialQuantity;
                        cmd.Parameters.Add("@WaterAmount", SqlDbType.Decimal).Value = waterAmount;
                        cmd.Parameters.Add("@Dosage", SqlDbType.NVarChar).Value = dosage;
                        cmd.Parameters.Add("@PlantStatus", SqlDbType.NVarChar).Value = plantStatus;
                        cmd.Parameters.Add("@ActiveIngredient", SqlDbType.NVarChar).Value = activeIngredient;
                        cmd.Parameters.Add("@EmployeeCode", SqlDbType.NVarChar).Value = employeeCode;
                        cmd.Parameters.Add("@IsolationDays", SqlDbType.Int).Value = isolationDays;
                        cmd.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = departmentID.HasValue ? (object)departmentID.Value : DBNull.Value;
                        cmd.Parameters.Add("@PlantLocation", SqlDbType.NVarChar).Value = plantLocation;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar).Value = note;
                        cmd.Parameters.Add("@FertilizationWorkTypeID", SqlDbType.Int).Value = fertilizationWorkTypeID;
                        cmd.Parameters.Add("@MaterialPrice", SqlDbType.Int).Value = MaterialPrice.HasValue ? (object)MaterialPrice.Value : DBNull.Value;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> updateCultivationProcessAsync_ThuHoach(int cultivationProcessID, decimal quantity)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateCultivationProcess_ThuHoach", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@CultivationProcessID", SqlDbType.Int).Value = cultivationProcessID;
                        cmd.Parameters.Add("@MaterialQuantity", SqlDbType.Decimal).Value = quantity;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> deletetCultivationProcessAsync(int ProcessID)
        {
            string query = "DELETE FROM CultivationProcess WHERE CultivationProcessID=@CultivationProcessID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CultivationProcessID", ProcessID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertPlantingManagementLogAsync(DateTime nurseryDate, DateTime plantingDate, DateTime harvestDate, string oldValue, string newValue)
        {
            int newId = -1;
            string insertQuery = @"INSERT INTO PlantingManagement_Log (NurseryDate, PlantingDate, HarvestDate, OldValue, NewValue, ActionBy)
                                    OUTPUT INSERTED.LogID
                                    VALUES (@NurseryDate, @PlantingDate, @HarvestDate, @OldValue, @NewValue, @ActionBy)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.Add("@NurseryDate", SqlDbType.DateTime).Value = nurseryDate;
                        cmd.Parameters.Add("@PlantingDate", SqlDbType.DateTime).Value = plantingDate;
                        cmd.Parameters.Add("@HarvestDate", SqlDbType.DateTime).Value = harvestDate;
                        cmd.Parameters.Add("@OldValue", SqlDbType.NVarChar).Value = oldValue;
                        cmd.Parameters.Add("@NewValue", SqlDbType.NVarChar).Value = newValue;
                        cmd.Parameters.Add("@ActionBy", SqlDbType.NVarChar).Value = UserManager.Instance.fullName;

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

        public async Task<DataTable> GetCultivationProcessLogAsync(int plantingID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM CultivationProcess_Log WHERE PlantingID = @PlantingID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<int> insertCultivationProcessLogAsync(int plantingID, string actionType, string oldValue, string newValue)
        {
            int newId = -1;
            string insertQuery = @"INSERT INTO CultivationProcess_Log (PlantingID, ActionType, OldValue, NewValue, ActionBy)
                                    OUTPUT INSERTED.LogID
                                    VALUES (@PlantingID, @ActionType, @OldValue, @NewValue, @ActionBy)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;
                        cmd.Parameters.Add("@ActionType", SqlDbType.NVarChar).Value = actionType;
                        cmd.Parameters.Add("@OldValue", SqlDbType.NVarChar).Value = oldValue;
                        cmd.Parameters.Add("@NewValue", SqlDbType.NVarChar).Value = newValue;
                        cmd.Parameters.Add("@ActionBy", SqlDbType.NVarChar).Value = UserManager.Instance.fullName;

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

        public async Task<DataTable> GetCultivationProcessTemplateLogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM CultivationProcessTemplate_Log";

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

        public async Task<int> insertCultivationProcessTemplateLogAsync(int SKU, int CultivationTypeID, string actionType, string oldValue, string newValue)
        {
            int newId = -1;
            string insertQuery = @"INSERT INTO CultivationProcessTemplate_Log (SKU, CultivationTypeID, ActionType, OldValue, NewValue, ActionBy)
                                    OUTPUT INSERTED.LogID
                                    VALUES (@SKU, @CultivationTypeID, @ActionType, @OldValue, @NewValue, @ActionBy)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.Add("@SKU", SqlDbType.Int).Value = SKU;
                        cmd.Parameters.Add("@CultivationTypeID", SqlDbType.Int).Value = CultivationTypeID;
                        cmd.Parameters.Add("@ActionType", SqlDbType.NVarChar).Value = actionType;
                        cmd.Parameters.Add("@OldValue", SqlDbType.NVarChar).Value = oldValue;
                        cmd.Parameters.Add("@NewValue", SqlDbType.NVarChar).Value = newValue;
                        cmd.Parameters.Add("@ActionBy", SqlDbType.NVarChar).Value = UserManager.Instance.fullName;

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

        public async Task<DataTable> GetPestDiseaseMonitoringAsync(int plantingID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetPestDiseaseMonitoring", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<bool> updatePestDiseaseMonitoringAsync(int plantingID, int monitoringID, DateTime processDate, string location, int? growwthStatusID, int? pestDiseaseID, string currentStatus, string observerCode, string treatmentPlan, string decisionMakerCode)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdatePestDiseaseMonitoring", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@MonitoringID", SqlDbType.Int).Value = monitoringID;
                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;
                        cmd.Parameters.Add("@MonitoringDate", SqlDbType.Date).Value = processDate.Date;
                        cmd.Parameters.Add("@Location", SqlDbType.NVarChar).Value = location;
                        cmd.Parameters.Add("@GrowthStageID", SqlDbType.Int).Value =
                            (object)growwthStatusID ?? DBNull.Value;
                        cmd.Parameters.Add("@PestDiseaseID", SqlDbType.Int).Value =
                            (object)pestDiseaseID ?? DBNull.Value;
                        cmd.Parameters.Add("@CurrentStatus", SqlDbType.NVarChar).Value = currentStatus;
                        cmd.Parameters.Add("@Observer", SqlDbType.NVarChar).Value = observerCode;
                        cmd.Parameters.Add("@TreatmentPlan", SqlDbType.NVarChar).Value = treatmentPlan;
                        cmd.Parameters.Add("@DecisionMaker", SqlDbType.NVarChar).Value = decisionMakerCode;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> insertPestDiseaseMonitoringAsync(int plantingID, DateTime processDate, string location, int? growwthStatusID, int? pestDiseaseID, string currentStatus, string observerCode, string treatmentPlan, string decisionMakerCode)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertPestDiseaseMonitoring", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;
                        cmd.Parameters.Add("@MonitoringDate", SqlDbType.Date).Value = processDate.Date;
                        cmd.Parameters.Add("@Location", SqlDbType.NVarChar).Value = location;
                        cmd.Parameters.Add("@GrowthStageID", SqlDbType.Int).Value =
                            (object)growwthStatusID ?? DBNull.Value;
                        cmd.Parameters.Add("@PestDiseaseID", SqlDbType.Int).Value =
                            (object)pestDiseaseID ?? DBNull.Value;
                        cmd.Parameters.Add("@CurrentStatus", SqlDbType.NVarChar).Value = currentStatus;
                        cmd.Parameters.Add("@Observer", SqlDbType.NVarChar).Value = observerCode;
                        cmd.Parameters.Add("@TreatmentPlan", SqlDbType.NVarChar).Value = treatmentPlan;
                        cmd.Parameters.Add("@DecisionMaker", SqlDbType.NVarChar).Value = decisionMakerCode;

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

        public async Task<int> insertPestDiseaseMonitoringLogAsync(int plantingID, string actionType, string oldValue, string newValue)
        {
            int newId = -1;
            string insertQuery = @"INSERT INTO PestDiseaseMonitoring_Log (PlantingID, ActionType, OldValue, NewValue, ActionBy)
                                    OUTPUT INSERTED.LogID
                                    VALUES (@PlantingID, @ActionType, @OldValue, @NewValue, @ActionBy)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;
                        cmd.Parameters.Add("@ActionType", SqlDbType.NVarChar).Value = actionType;
                        cmd.Parameters.Add("@OldValue", SqlDbType.NVarChar).Value = oldValue;
                        cmd.Parameters.Add("@NewValue", SqlDbType.NVarChar).Value = newValue;
                        cmd.Parameters.Add("@ActionBy", SqlDbType.NVarChar).Value = UserManager.Instance.fullName;

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

        public async Task<bool> deletetPestDiseaseMonitoringAsync(int ProcessID)
        {
            string query = "DELETE FROM PestDiseaseMonitoring WHERE MonitoringID=@MonitoringID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@MonitoringID", ProcessID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetPestDiseaseMonitoringLogAsync(int plantingID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM PestDiseaseMonitoring_Log WHERE PlantingID = @PlantingID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetHarvestScheduleAsync(int plantingID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetHarvestSchedule", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<DataTable> GetHarvestScheduleLogAsync(int plantingID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM HarvestScheduleLog WHERE PlantingID = @PlantingID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<int> insertHarvestScheduleLogAsync(int plantingID, string actionType, string oldValue, string newValue)
        {
            int newId = -1;
            string insertQuery = @"INSERT INTO HarvestScheduleLog (PlantingID, ActionType, OldValue, NewValue, ActionBy)
                                    OUTPUT INSERTED.LogID
                                    VALUES (@PlantingID, @ActionType, @OldValue, @NewValue, @ActionBy)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTuLog_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;
                        cmd.Parameters.Add("@ActionType", SqlDbType.NVarChar).Value = actionType;
                        cmd.Parameters.Add("@OldValue", SqlDbType.NVarChar).Value = oldValue;
                        cmd.Parameters.Add("@NewValue", SqlDbType.NVarChar).Value = newValue;
                        cmd.Parameters.Add("@ActionBy", SqlDbType.NVarChar).Value = UserManager.Instance.fullName;

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

        public async Task<int> insertHarvestScheduleAsync(int plantingID, DateTime harvestDate, decimal quantity, string productLotCode, string harvestEmployeeCode, int? receiveDepartmentID)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertHarvestSchedule", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;
                        cmd.Parameters.Add("@HarvestDate", SqlDbType.Date).Value = harvestDate.Date;
                        cmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = quantity;
                        cmd.Parameters.Add("@ProductLotCode", SqlDbType.NVarChar).Value = productLotCode;
                        cmd.Parameters.Add("@HarvestEmployee", SqlDbType.NVarChar).Value = harvestEmployeeCode;
                        cmd.Parameters.Add("@ReceiveDepartmentID", SqlDbType.Int).Value =
                            (object)receiveDepartmentID ?? DBNull.Value;

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

        public async Task<bool> updatePestDiseaseMonitoringAsync(int harvestID, int plantingID, DateTime harvestDate, decimal quantity, string productLotCode, string harvestEmployeeCode, int? receiveDepartmentID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateHarvestSchedule", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@HarvestID", SqlDbType.Int).Value = harvestID;
                        cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;
                        cmd.Parameters.Add("@HarvestDate", SqlDbType.Date).Value = harvestDate.Date;
                        cmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = quantity;
                        cmd.Parameters.Add("@ProductLotCode", SqlDbType.NVarChar).Value = productLotCode;
                        cmd.Parameters.Add("@HarvestEmployee", SqlDbType.NVarChar).Value = harvestEmployeeCode;
                        cmd.Parameters.Add("@ReceiveDepartmentID", SqlDbType.Int).Value =
                            (object)receiveDepartmentID ?? DBNull.Value;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> deletetHarvestScheduleAsync(int harvestID)
        {
            string query = "DELETE FROM HarvestSchedule WHERE HarvestID=@HarvestID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@HarvestID", harvestID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetPlantTrayDensityAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM PlantTrayDensity";

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

        public async Task<DataTable> getThongKeCapPhanAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetThongKeCapPhan", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<DataTable> getThongKeVatTuTheoNamAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetThongKeVatTuTheoNam", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<DataTable> getGlobalGapProductsAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetGlobalGapProducts", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<DataTable> GetHarvestScheduleGlobalGAPAsync(int plantingID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoVatTu_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetHarvestSchedule_GlobalGAP", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@PlantingID", SqlDbType.Int).Value = plantingID;

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<bool> InsertHarvestScheduleGlobalGapListAsync(List<(int PlantingID, DateTime HarvestDate, decimal Quantity, string ProductLotCode, string HarvestEmployee, string SupervisorEmployee, int ReceiveDepartmentID)> list)
        {
            try
            {
                using (var con = new SqlConnection(ql_khoVatTu_conStr()))
                {
                    await con.OpenAsync();

                    // 1️⃣ Chuẩn bị DataTable khớp với kiểu TVP
                    var dt = new DataTable();
                    dt.Columns.Add("PlantingID", typeof(int));
                    dt.Columns.Add("HarvestDate", typeof(DateTime));
                    dt.Columns.Add("Quantity", typeof(decimal));
                    dt.Columns.Add("ProductLotCode", typeof(string));
                    dt.Columns.Add("HarvestEmployee", typeof(string));
                    dt.Columns.Add("SupervisorEmployee", typeof(string));
                    dt.Columns.Add("ReceiveDepartmentID", typeof(int));

                    foreach (var item in list)
                    {
                        dt.Rows.Add(item.PlantingID,
                                    item.HarvestDate,
                                    item.Quantity,
                                    item.ProductLotCode,
                                    item.HarvestEmployee,
                                    item.SupervisorEmployee,
                                    item.ReceiveDepartmentID);
                    }

                    // 2️⃣ Gọi SP
                    using (var cmd = new SqlCommand("dbo.sp_Upsert_HarvestSchedule_GlobalGap", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@Data", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "dbo.HarvestSchedule_GlobalGap_Type";

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InsertCultivationProcessListAsync] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}

