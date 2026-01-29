
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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
    }
}

