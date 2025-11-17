using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using RauViet.ui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using DataTable = System.Data.DataTable;

namespace RauViet.classes
{
    public sealed class SQLManager
    {
        private static SQLManager ins = null;
        private static readonly object padlock = new object();
        public readonly string ql_User_conStr = "Server=192.168.1.8,1433;Database=QL_User;User Id=ql_user;Password=A7t#kP2x;";
        public readonly string ql_kho_conStr = "Server=192.168.1.8,1433;Database=QL_Kho;User Id=ql_kho;Password=A7t#kP2x;";
        public readonly string ql_khoHis_conStr = "Server=192.168.1.8,1433;Database=QL_Kho_History;User Id=ql_kho_history;Password=A7t#kP2x;";
        public readonly string salaryLock_conStr = "Server=192.168.1.8,1433;Database=SalaryLock;User Id=salary_lock;Password=A7t#kP2x;";
        public readonly string qlnvHis_conStr = "Server=192.168.1.8,1433;Database=QLNV_VR_History;User Id=qlnv_vr_history;Password=A7t#kP2x;";
        public readonly string conStr = "Server=192.168.1.8,1433;Database=QLNV;User Id=vietrau;Password=A7t#kP2x;";

        private SQLManager() { }

        public static SQLManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (ins == null)
                        ins = new SQLManager();
                    return ins;
                }
            }
        }

        public async Task<string> GetPasswordHashFromDatabase(string username)
        {
            string hash = "";

            using (SqlConnection con = new SqlConnection(ql_User_conStr))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand("SELECT PasswordHash FROM Users WHERE Username = @Username AND IsActive = 1", con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    var result = await cmd.ExecuteScalarAsync();

                    if (result != DBNull.Value && result != null)
                        hash = result.ToString();
                }
            }
            

            return hash;
        }

        //==============================Customers==============================
        public async Task<DataTable> getCustomersAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();
                string query = "SELECT * FROM Customers ORDER BY Priority ASC;";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateCustomerAsync(int customerID, string name, string code, int Priority)
        {
            string query = "UPDATE Customers SET FullName=@FullName, CustomerCode=@CustomerCode, Priority=@Priority WHERE CustomerID=@CustomerID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.Parameters.AddWithValue("@FullName", name);
                        cmd.Parameters.AddWithValue("@CustomerCode", code);
                        cmd.Parameters.AddWithValue("@Priority", Priority);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertCustomerAsync(string name, string code, int priority)
        {
            int newId = -1;
            string query = @"INSERT INTO Customers (FullName, CustomerCode, Priority ) 
                             OUTPUT INSERTED.CustomerID
                            VALUES (@FullName, @CustomerCode, @CustomerCode)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FullName", name);
                        cmd.Parameters.AddWithValue("@CustomerCode", code);
                        cmd.Parameters.AddWithValue("@Priority", priority);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> deleteCustomerAsync(int customerID)
        {
            string query = "DELETE FROM Customers WHERE CustomerID=@CustomerID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        //==============================ProductSKU==============================
        public async Task<DataTable> getProductSKUAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM ProductSKU ORDER BY ProductNameVN ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateProductSKUAsync(int SKU, string ProductNameVN, string ProductNameEN, string PackingType, string Package,
            string PackingList, string BotanicalName, decimal PriceCNF, int priority, string plantingareaCode, string LOTCodeHeader, bool isActive)
        {
            string query = @"UPDATE ProductSKU SET 
                                ProductNameVN=@ProductNameVN, 
                                ProductNameEN=@ProductNameEN, 
                                PackingType=@PackingType, 
                                Package=@Package, 
                                PackingList=@PackingList, 
                                BotanicalName=@BotanicalName, 
                                PriceCNF=@PriceCNF,
                                PlantingAreaCode=@PlantingAreaCode,
                                LOTCodeHeader=@LOTCodeHeader,
                                IsActive=@IsActive,
                                Priority=@Priority
                             WHERE SKU=@SKU";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@ProductNameVN", ProductNameVN);
                        cmd.Parameters.AddWithValue("@ProductNameEN", ProductNameEN);
                        cmd.Parameters.AddWithValue("@PackingType", PackingType);
                        cmd.Parameters.AddWithValue("@Package", Package);
                        cmd.Parameters.AddWithValue("@PackingList", PackingList);
                        cmd.Parameters.AddWithValue("@BotanicalName", BotanicalName);
                        cmd.Parameters.AddWithValue("@PriceCNF", PriceCNF);
                        cmd.Parameters.AddWithValue("@PlantingAreaCode", plantingareaCode);
                        cmd.Parameters.AddWithValue("@LOTCodeHeader", LOTCodeHeader);
                        cmd.Parameters.AddWithValue("@Priority", priority);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertProductSKUAsync(string ProductNameVN, string ProductNameEN, string PackingType, string Package, 
            string PackingList, string BotanicalName, decimal PriceCNF, int priority, string plantingareaCode, string LOTCodeHeader)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("SP_InsertProductSKU", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ProductNameVN", ProductNameVN);
                        cmd.Parameters.AddWithValue("@ProductNameEN", ProductNameEN);
                        cmd.Parameters.AddWithValue("@PackingType", PackingType);
                        cmd.Parameters.AddWithValue("@Package", Package);
                        cmd.Parameters.AddWithValue("@PackingList", PackingList);
                        cmd.Parameters.AddWithValue("@BotanicalName", BotanicalName);
                        cmd.Parameters.AddWithValue("@PriceCNF", PriceCNF);
                        cmd.Parameters.AddWithValue("@Priority", priority);
                        cmd.Parameters.AddWithValue("@PlantingAreaCode", plantingareaCode);
                        cmd.Parameters.AddWithValue("@LOTCodeHeader", LOTCodeHeader);

                        var outParam = new SqlParameter("@NewSKU", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outParam);

                        await cmd.ExecuteNonQueryAsync();
                        newId = Convert.ToInt32(outParam.Value);
                    }
                }
            }
            catch
            {
                newId = -1;
            }

            return newId;
        }

        public async Task<bool> deleteProductSKUAsync(int SKU)
        {
            string query = "DELETE FROM ProductSKU WHERE SKU=@SKU";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        //==============================ProductPacking==============================
        public async Task<DataTable> getProductpackingAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();
                string query = @"
                    SELECT 
                        p.SKU,
                        pk.BarCode,   
                        pk.PLU,        
                        pk.Packing,    
                        pk.BarCodeEAN13,
                        pk.ArtNr,
                        pk.GGN,
                        pk.Amount,
                        pk.IsActive,
                        pk.ProductPackingID
                    FROM ProductSKU p
                    JOIN ProductPacking pk ON p.SKU = pk.SKU;";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateProductpackingAsync(int ID, int SKU, string BarCode, string PLU, int? Amount, string packing, string barCodeEAN13, string artNr, string GGN, bool isActive)
        {
            string query = @"UPDATE ProductPacking SET
                                SKU=@SKU, 
                                BarCode=@BarCode, 
                                PLU=@PLU, 
                                Amount=@Amount, 
                                packing=@packing, 
                                BarCodeEAN13=@BarCodeEAN13, 
                                ArtNr=@ArtNr, 
                                IsActive=@IsActive, 
                                GGN=@GGN
                             WHERE ProductPackingID=@ID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@BarCode", BarCode);
                        cmd.Parameters.AddWithValue("@PLU", PLU);
                        cmd.Parameters.AddWithValue("@Amount", (object)Amount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@packing", packing);
                        cmd.Parameters.AddWithValue("@BarCodeEAN13", barCodeEAN13);
                        cmd.Parameters.AddWithValue("@ArtNr", artNr);
                        cmd.Parameters.AddWithValue("@GGN", GGN);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertProductpackingAsync(int SKU, string BarCode, string PLU, int? Amount, string packing, string barCodeEAN13, string artNr, string GGN)
        {
            int newId = -1;
            string query = @"INSERT INTO ProductPacking (SKU, BarCode, PLU, Amount, packing, BarCodeEAN13, ArtNr, GGN)
                            OUTPUT INSERTED.ProductPackingID
                             VALUES (@SKU, @BarCode, @PLU, @Amount, @packing, @BarCodeEAN13, @ArtNr, @GGN)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@BarCode", BarCode);
                        cmd.Parameters.AddWithValue("@PLU", PLU);
                        cmd.Parameters.AddWithValue("@Amount", (object)Amount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@packing", packing);
                        cmd.Parameters.AddWithValue("@BarCodeEAN13", barCodeEAN13);
                        cmd.Parameters.AddWithValue("@ArtNr", artNr);
                        cmd.Parameters.AddWithValue("@GGN", GGN);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> deleteProductpackingAsync(int productPackingID)
        {
            string query = "DELETE FROM ProductPacking WHERE ProductPackingID=@ProductPackingID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ProductPackingID", productPackingID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        //==============================ExportCodes==============================
        public async Task<DataTable> getExportCodesAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();
                string query = "SELECT * FROM ExportCodes";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateExportCodeAsync(int exportCodeID, string exportCode, int exportCodeIndex, DateTime exportDate, decimal? exRate, decimal? shippingCost, int inputBy, int packingBy, bool complete)
        {
            string query = @"UPDATE ExportCodes SET 
                                ExportCode=@ExportCode, 
                                ExportCodeIndex=@ExportCodeIndex, 
                                ExportDate=@ExportDate, 
                                ExchangeRate=@ExchangeRate, 
                                ShippingCost=@ShippingCost, 
                                ModifiedAt=@ModifiedAt, 
                                InputBy=@InputBy, 
                                PackingBy=@PackingBy, 
                                Complete=@Complete 
                            WHERE ExportCodeID=@ExportCodeID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                        cmd.Parameters.AddWithValue("@ExportCode", exportCode);
                        cmd.Parameters.AddWithValue("@ExportCodeIndex", exportCodeIndex);
                        cmd.Parameters.AddWithValue("@ExportDate", exportDate);
                        cmd.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Complete", complete);
                        cmd.Parameters.AddWithValue("@InputBy", inputBy);
                        cmd.Parameters.AddWithValue("@PackingBy", packingBy);
                        cmd.Parameters.AddWithValue("@ExchangeRate", exRate.HasValue ? (object)exRate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ShippingCost", shippingCost.HasValue ? (object)shippingCost.Value : DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertExportCodeAsync(string exportCode, int exportCodeIndex, DateTime exportDate, decimal? exRate, decimal? shippingCost, int inputBy, int packingBy)
        {
            int newId = -1;
            string query = @"INSERT INTO ExportCodes (ExportCode, ExportCodeIndex, ExportDate, ExchangeRate, ShippingCost, InputBy, PackingBy) 
                                OUTPUT INSERTED.ExportCodeID
                                VALUES (@ExportCode, @ExportCodeIndex, @ExportDate, @ExchangeRate, @ShippingCost, @InputBy, @PackingBy)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExportCode", exportCode);
                        cmd.Parameters.AddWithValue("@ExportCodeIndex", exportCodeIndex);
                        cmd.Parameters.AddWithValue("@ExportDate", exportDate);
                        cmd.Parameters.AddWithValue("@InputBy", inputBy);
                        cmd.Parameters.AddWithValue("@PackingBy", packingBy);
                        cmd.Parameters.AddWithValue("@ExchangeRate", exRate.HasValue ? (object)exRate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@ShippingCost", shippingCost.HasValue ? (object)shippingCost.Value : DBNull.Value);

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> DeleteExportCodeWithOrdersAsync(int exportCodeID)
        {
            string deleteOrdersTotalQuery = "DELETE FROM OrdersTotal WHERE ExportCodeID = @ExportCodeID";
            string deleteOrdersQuery = "DELETE FROM Orders WHERE ExportCodeID = @ExportCodeID";
            string deleteExportCodeQuery = "DELETE FROM ExportCodes WHERE ExportCodeID = @ExportCodeID";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();

                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        // Xóa OrdersTotal
                        using (SqlCommand cmd = new SqlCommand(deleteOrdersTotalQuery, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Xóa Orders
                        using (SqlCommand cmd = new SqlCommand(deleteOrdersQuery, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        // Xóa ExportCode
                        using (SqlCommand cmd = new SqlCommand(deleteExportCodeQuery, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        tran.Commit();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        //==================================================Others================================

        public async Task<DataTable> getOrdersAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT o.* FROM Orders o
                                        INNER JOIN ExportCodes ec ON o.ExportCodeID = ec.ExportCodeID
                                        WHERE o.ExportCodeID = @ExportCodeID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<bool> updateOrdersAsync(int orderId, int exportCodeId, int customerId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF)
        {
            string query = @"UPDATE Orders SET 
                                ExportCodeID=@ExportCodeID, 
                                CustomerID=@CustomerID, 
                                ProductPackingID=@ProductPackingID, 
                                OrderPackingPriceCNF=@OrderPackingPriceCNF, 
                                PCSOther=@PCSOther, 
                                NWOther=@NWOther
                             WHERE OrderId=@OrderId";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeId);
                        cmd.Parameters.AddWithValue("@CustomerID", customerId);
                        cmd.Parameters.AddWithValue("@ProductPackingID", packingId);
                        cmd.Parameters.AddWithValue("@OrderPackingPriceCNF", priceCNF);
                        cmd.Parameters.AddWithValue("@PCSOther", PCSOther);
                        cmd.Parameters.AddWithValue("@NWOther", NWOther);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertOrderAsync(int customerId, int exportCodeId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF)
        {
            int newId = -1;
            string query = @"INSERT INTO Orders (CustomerID, ExportCodeID, ProductPackingID, OrderPackingPriceCNF, PCSOther, NWOther)
                            OUTPUT INSERTED.OrderId
                             VALUES (@CustomerID, @ExportCodeID, @ProductPackingID, @OrderPackingPriceCNF, @PCSOther, @NWOther)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeId);
                        cmd.Parameters.AddWithValue("@CustomerID", customerId);
                        cmd.Parameters.AddWithValue("@ProductPackingID", packingId);
                        cmd.Parameters.AddWithValue("@OrderPackingPriceCNF", priceCNF);
                        cmd.Parameters.AddWithValue("@PCSOther", PCSOther);
                        cmd.Parameters.AddWithValue("@NWOther", NWOther);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> deleteOrderAsync(int id)
        {
            string query = "DELETE FROM Orders WHERE OrderId=@OrderId";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> getPendingOrderSummary(int exportCodeID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SP_PendingOrder", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Thêm tham số cho stored procedure
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }


        public async Task<DataTable> getOrdersPackingAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();
                string query = @"
                                SELECT 
                                    o.OrderId,
                                    c.FullName AS CustomerName,   
                                    s.ProductNameVN AS ProductPackingName,
                                    s.PackingType,  
                                    o.CustomerCarton,
                                    o.CartonNo,
                                    o.CartonSize,
                                    o.PCSReal,
                                    o.NWReal, 
                                    o.PCSOther,
                                    o.NWOther,
                                    s.Priority,
                                    s.ProductNameEN,
                                    s.Package,
                                    ec.ExportCode,
                                    ec.ExportDate,
                                    p.Amount,
                                    p.packing,                
                                    c.CustomerCode,
                                    o.ExportCodeID
                                FROM Orders o
                                INNER JOIN Customers c ON o.CustomerID = c.CustomerID
                                INNER JOIN ProductPacking p ON o.ProductPackingID = p.ProductPackingID
                                INNER JOIN ProductSKU s ON p.SKU = s.SKU
                                INNER JOIN ExportCodes ec ON o.ExportCodeID = ec.ExportCodeID
                                WHERE ec.Complete = 0
                                ORDER BY o.ExportCodeID;
                            ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> UpdatePackOrdersBulkAsync(List<(int orderId, int? pcsReal, decimal? nwReal, int? cartonNo, string cartonSize, string customerCarton)> orders)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();

                    using (var dt = new DataTable())
                    {
                        dt.Columns.Add("OrderId", typeof(int));
                        dt.Columns.Add("PCSReal", typeof(int));
                        dt.Columns.Add("NWReal", typeof(decimal));
                        dt.Columns.Add("CartonNo", typeof(int));
                        dt.Columns.Add("CartonSize", typeof(string));
                        dt.Columns.Add("CustomerCarton", typeof(string));

                        foreach (var o in orders)
                        {
                            dt.Rows.Add(o.orderId, (object)o.pcsReal ?? DBNull.Value, (object)o.nwReal ?? DBNull.Value,
                                        (object)o.cartonNo ?? DBNull.Value, (object)o.cartonSize ?? DBNull.Value,
                                        (object)o.customerCarton ?? DBNull.Value);
                        }

                        using (var cmd = new SqlCommand("dbo.UpdateOrdersBulk", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            var param = cmd.Parameters.AddWithValue("@OrderUpdates", dt);
                            param.SqlDbType = SqlDbType.Structured;
                            param.TypeName = "dbo.OrderUpdateType";

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdatePackOrdersBulkAsync] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async Task<DataTable> getOrdersTotalAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SP_OrdersTotal", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<bool> UpsertOrdersTotalListAsync(List<(int ExportCodeID, int ProductPackingID, decimal? NetWeightFinal)> list)
        {
            try
            {
                using (var con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();

                    // 1️⃣ Chuẩn bị DataTable khớp với kiểu TVP
                    var dt = new DataTable();
                    dt.Columns.Add("ExportCodeID", typeof(int));
                    dt.Columns.Add("ProductPackingID", typeof(int));
                    dt.Columns.Add("NetWeightFinal", typeof(decimal));

                    foreach (var item in list)
                    {
                        dt.Rows.Add(item.ExportCodeID, item.ProductPackingID,
                                    (object)item.NetWeightFinal ?? DBNull.Value);
                    }

                    // 2️⃣ Gọi SP
                    using (var cmd = new SqlCommand("dbo.UpsertOrdersTotalList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@Updates", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "dbo.OrderTotalUpdateType";

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpsertOrdersTotalListAsync] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async Task<DataTable> getOrdersDKKDAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetOrdersDKKD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // <--- Gọi SP
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> getOrdersChotPhytosync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetOrdersChotPhyto", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // <--- Gọi SP
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }


        public async Task<DataTable> getOrdersPhytoAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SP_GetOrdersPhyto", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> getOrdersINVOICEAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SP_OrdersINVOICE", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetCustomersOrdersAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SP_CustomersOrders", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<DataTable> GetExportCartonCountsAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SP_GetCartonCountByExportCode", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetLOTCodeByExportCodeAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT DISTINCT
                                        p.SKU,
                                        o.ExportCodeID,
                                        e.ExportCode,
                                        s.ProductNameVN,
                                        s.LOTCodeHeader,
                                        o.LOTCode,
                                        o.LOTCodeComplete,
                                        s.Priority 
                                    FROM Orders o
                                    INNER JOIN ProductPacking p ON o.ProductPackingID = p.ProductPackingID
                                    INNER JOIN ProductSKU s ON p.SKU = s.SKU
                                    INNER JOIN ExportCodes e ON o.ExportCodeID = e.ExportCodeID
                                    WHERE e.ExportCodeID = @ExportCodeID;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<bool> UpsertOrdersLotCodesBySKUAsync(List<(int ExportCodeID, int SKU, string LOTCode, string LOTCodeComplete)> list)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();

                    // Tạo DataTable khớp với kiểu SQL Type
                    var dt = new DataTable();
                    dt.Columns.Add("ExportCodeID", typeof(int));
                    dt.Columns.Add("SKU", typeof(int));
                    dt.Columns.Add("LOTCode", typeof(string));
                    dt.Columns.Add("LOTCodeComplete", typeof(string));

                    foreach (var item in list)
                        dt.Rows.Add(item.ExportCodeID, item.SKU,
                                    (object)item.LOTCode ?? DBNull.Value,
                                    (object)item.LOTCodeComplete ?? DBNull.Value);

                    using (SqlCommand cmd = new SqlCommand("dbo.UpsertOrdersLotCodesBySKU", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@Updates", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "dbo.OrderLotUpdateType";

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpsertOrdersLotCodesBySKUAsync] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async Task<DataTable> GetDetailPackingTotalAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("SP_DetailPackingTotalByExportCode_incomplete", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }


        public async Task<DataTable> GetCustomerDetailPackingAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SP_CustomerDetailPacking_incomplete", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }



        public async Task<bool> updateNewPriceInOrderListWithExportCodeAsync(int exportCodeID)
        {
            string query = @"UPDATE o
                            SET o.OrderPackingPriceCNF = ps.PriceCNF
                            FROM Orders o
                            INNER JOIN ProductPacking pp ON o.ProductPackingID = pp.ProductPackingID
                            INNER JOIN ProductSKU ps ON pp.SKU = ps.SKU
                            INNER JOIN ExportCodes ec ON o.ExportCodeID = ec.ExportCodeID
                            WHERE ec.Complete = 0
                                AND o.ExportCodeID = @ExportCodeID;";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetActiveEmployeesIn_DongGoiAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT EmployeeID, FullName FROM Employee WHERE DepartmentID = 29 AND IsActive = 1;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetActiveEmployees_For_CreateUserAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT EmployeeID, FullName, EmployeeCode FROM Employee WHERE canCreateUserName = 1 AND IsActive = 1;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetRolesAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_User_conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT RoleID, RoleName FROM Roles;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetEmployeesAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Employee";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateEmployeesAsync(int employeeId, string maNV, string tenNV, DateTime birthDate, DateTime hireDate,
                                    bool isMale, string homeTown, string address, string citizenID, DateTime? issueDate, string issuePlace, 
                                    bool isActive, bool canCreateUserName, decimal probationSalaryPercent, string phone, string noteResign, bool isInsuranceRefund, int salaryGradeID)
        {
            string query = @"UPDATE Employee SET 
                                EmployeeCode=@EmployeeCode, 
                                FullName=@FullName,
                                BirthDate=@BirthDate, 
                                HireDate=@HireDate,
                                Gender=@Gender, 
                                Hometown=@Hometown,
                                Address=@Address, 
                                CitizenID=@CitizenID,
                                IssueDate=@IssueDate, 
                                IssuePlace=@IssuePlace,                                
                                IsActive=@IsActive,
                                canCreateUserName=@canCreateUserName,
                                ProbationSalaryPercent=@ProbationSalaryPercent,
                                PhoneNumber=@PhoneNumber,
                                IsInsuranceRefund=@IsInsuranceRefund,
                                SalaryGradeID=@SalaryGradeID,
                                NoteResign=@NoteResign
                            WHERE EmployeeID=@EmployeeID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        cmd.Parameters.AddWithValue("@EmployeeCode", maNV);
                        cmd.Parameters.AddWithValue("@FullName", tenNV);
                        cmd.Parameters.AddWithValue("@BirthDate", birthDate);
                        cmd.Parameters.AddWithValue("@HireDate", hireDate);
                        cmd.Parameters.AddWithValue("@Gender", isMale);
                        cmd.Parameters.AddWithValue("@Hometown", homeTown);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@CitizenID", citizenID);
                        cmd.Parameters.AddWithValue("@IssueDate", (object)(issueDate ?? (object)DBNull.Value));
                        cmd.Parameters.AddWithValue("@IssuePlace", issuePlace);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@canCreateUserName", canCreateUserName);
                        cmd.Parameters.AddWithValue("@ProbationSalaryPercent", probationSalaryPercent);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phone);
                        cmd.Parameters.AddWithValue("@NoteResign", noteResign);
                        cmd.Parameters.AddWithValue("@IsInsuranceRefund", isInsuranceRefund);
                        cmd.Parameters.AddWithValue("@SalaryGradeID", salaryGradeID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> deleteEmployeeAsync(int employeeID)
        {
            string query = "DELETE FROM Employee WHERE EmployeeID=@EmployeeID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }


        public async Task<int> insertEmployeeAsync(string maNV_temp, string tenNV, DateTime birthDate, DateTime hireDate, bool isMale, string homeTown,
                                                    string address, string citizenID, DateTime? issueDate, string issuePlace, bool isActive, bool canCreateUserName, 
                                                    decimal probationSalaryPercent, string phone, string noteResign, bool isInsuranceRefund, int salaryGradeID)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertEmployee", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@EmployeeCode", maNV_temp);
                        cmd.Parameters.AddWithValue("@FullName", tenNV);
                        cmd.Parameters.AddWithValue("@BirthDate", birthDate);
                        cmd.Parameters.AddWithValue("@HireDate", hireDate);
                        cmd.Parameters.AddWithValue("@Gender", isMale);
                        cmd.Parameters.AddWithValue("@Hometown", homeTown);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@CitizenID", citizenID);
                        cmd.Parameters.AddWithValue("@IssueDate", (object)issueDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IssuePlace", issuePlace);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@CanCreateUserName", canCreateUserName);
                        cmd.Parameters.AddWithValue("@ProbationSalaryPercent", probationSalaryPercent);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phone);
                        cmd.Parameters.AddWithValue("@NoteResign", noteResign);
                        cmd.Parameters.AddWithValue("@IsInsuranceRefund", isInsuranceRefund);
                        cmd.Parameters.AddWithValue("@SalaryGradeID", salaryGradeID);

                        // Output params
                        SqlParameter idParam = new SqlParameter("@NewEmployeeID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        SqlParameter codeParam = new SqlParameter("@NewEmployeeCode", SqlDbType.NVarChar, 50)
                        {
                            Direction = ParameterDirection.Output
                        };

                        cmd.Parameters.Add(idParam);
                        cmd.Parameters.Add(codeParam);

                        await cmd.ExecuteNonQueryAsync();

                        newId = Convert.ToInt32(idParam.Value);
                        string newCode = codeParam.Value?.ToString();

                        // 👉 bạn có thể return code nếu muốn
                        Console.WriteLine($"Employee created: {newCode}");
                    }
                }

                return newId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting employee: {ex.Message}");
                return -1;
            }
        }

        public async Task<DataTable> GetActiveEmployee_DeductionATT_Async(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeAllowance_ChuyenCan", con))
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

        public async Task<DataTable> GetEmployeeLeave_PT_KP_Async(int month, int year)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeLeave_PT_KP", con))
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


        public async Task<DataTable> GetContractTypeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM ContractType";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetApplyScopeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM ApplyScope";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetUserDataAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_User_conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT 
                                    u.UserID,
                                    u.Username,
                                    u.PasswordHash,
                                    u.EmployeeCode,
                                    u.IsActive,
                                    STRING_AGG(r.RoleID, ',') AS RoleIDs
                                FROM Users u
                                LEFT JOIN UserRoles ur ON u.UserID = ur.UserID
                                LEFT JOIN Roles r ON ur.RoleID = r.RoleID
                                GROUP BY 
                                    u.UserID, u.Username, u.PasswordHash, u.EmployeeCode, 
                                    u.IsActive
                                ORDER BY u.UserID;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateUserAsync(int userID, string userName, string password, string employeeCode, Boolean isActive, List<int> roleIDs)
        {
            string query = @"UPDATE Users SET 
                                Username=@Username, PasswordHash=@PasswordHash, EmployeeCode=@EmployeeCode, IsActive=@IsActive 
                            WHERE UserID=@UserID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Username", userName);
                        cmd.Parameters.AddWithValue("@PasswordHash", password);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    string roleIDsString = string.Join(",", roleIDs);
                    using (SqlCommand cmdRole = new SqlCommand("sp_UpdateUserRoles", con))
                    {
                        cmdRole.CommandType = CommandType.StoredProcedure;
                        cmdRole.Parameters.AddWithValue("@UserID", userID);
                        cmdRole.Parameters.AddWithValue("@RoleIDs", roleIDsString);
                        await cmdRole.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi updateUserAsync: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> updateUser_notPasswordAsync(int userID, string userName, string employeeCode, Boolean isActive, List<int> roleIDs)
        {
            string query = @"UPDATE Users SET 
                                Username=@Username, EmployeeCode=@EmployeeCode, IsActive=@IsActive 
                            WHERE UserID=@UserID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Username", userName);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    string roleIDsString = string.Join(",", roleIDs);
                    using (SqlCommand cmdRole = new SqlCommand("sp_UpdateUserRoles", con))
                    {
                        cmdRole.CommandType = CommandType.StoredProcedure;
                        cmdRole.Parameters.AddWithValue("@UserID", userID);
                        cmdRole.Parameters.AddWithValue("@RoleIDs", roleIDsString);
                        await cmdRole.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi updateUserAsync: " + ex.Message);
                return false;
            }
        }

        public async Task<int> insertUserDataAsync(string userName, string password, string employeeCode, Boolean isActive, List<int> roleIDs)
        {
            int newId = -1;
            string query = @"INSERT INTO Users (Username, PasswordHash, EmployeeCode, IsActive) 
                             OUTPUT INSERTED.UserID
                            VALUES (@Username, @PasswordHash, @EmployeeCode, @IsActive)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", userName);
                        cmd.Parameters.AddWithValue("@PasswordHash", password);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                        {
                            newId = Convert.ToInt32(result);

                            string roleIDsString = string.Join(",", roleIDs);
                            using (SqlCommand cmdRole = new SqlCommand("sp_UpdateUserRoles", con))
                            {
                                cmdRole.CommandType = CommandType.StoredProcedure;
                                cmdRole.Parameters.AddWithValue("@UserID", newId);
                                cmdRole.Parameters.AddWithValue("@RoleIDs", roleIDsString);
                                await cmdRole.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
                return newId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi updateUserAsync: " + ex.Message);
                return -1;
            }
        }

        public async Task<bool> deleteUserAsync(int userID)
        {
            string query = "DELETE FROM Users WHERE UserID=@UserID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetDepartmentAsybc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Department";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateDepartmentAsync(int departmentID, string departmentName, string description, bool isActive)
        {
            string query = @"UPDATE Department SET DepartmentName=@DepartmentName, Description=@Description, IsActive=@IsActive 
                            WHERE DepartmentID=@DepartmentID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertDepartmentAsync(string departmentName, string description, bool isActive)
        {
            int newId = -1;

            string query = @"INSERT INTO Department (DepartmentName, Description, IsActive) 
                                OUTPUT INSERTED.DepartmentID
                                VALUES (@DepartmentName, @Description, @IsActive)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentName", departmentName);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1;}
        }

        public async Task<bool> deleteDepartmentAsync(int ID)
        {
            string query = "DELETE FROM Department WHERE DepartmentID=@DepartmentID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", ID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetPositionAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Position";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updatePositionAsync(int PositionID, string positionCode, string PositionName, string description, bool isActive)
        {
            string query = @"UPDATE Position SET PositionCode=@PositionCode, PositionName=@PositionName, Description=@Description, IsActive=@IsActive 
                            WHERE PositionID=@PositionID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PositionID", PositionID);
                        cmd.Parameters.AddWithValue("@PositionCode", positionCode);
                        cmd.Parameters.AddWithValue("@PositionName", PositionName);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertPositionAsync(string positionCode, string PositionName, string description, bool isActive)
        {
            int newId = -1;

            string query = @"INSERT INTO Position (PositionCode, PositionName, Description, IsActive) 
                                OUTPUT INSERTED.PositionID
                                VALUES (@PositionCode, @PositionName, @Description, @IsActive)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PositionCode", positionCode);
                        cmd.Parameters.AddWithValue("@PositionName", PositionName);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> deletePositionAsync(int ID)
        {
            string query = "DELETE FROM Position WHERE PositionID=@PositionID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PositionID", ID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataRow> GetUserRoleAsync(string userName)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_User_conStr))
            {
                await con.OpenAsync();

                string query = @"SELECT 
                                    u.EmployeeCode,
                                    STRING_AGG(r.RoleCode, ',') AS RoleCodes
                                FROM Users u
                                LEFT JOIN UserRoles ur ON u.UserID = ur.UserID
                                LEFT JOIN Roles r ON ur.RoleID = r.RoleID
                                WHERE u.Username = @Username
                                    AND u.IsActive = 1
                                GROUP BY u.EmployeeCode;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", userName);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            // Trả về dòng đầu tiên nếu có
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public async Task<DataTable> GetUserInfoAsync(string employeeCode)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                string query = @"SELECT EmployeeID, EmployeeCode, FullName FROM Employee WHERE EmployeeCode = @EmployeeCode AND IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            // Trả về dòng đầu tiên nếu có
            return dt;
        }

        public async Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            try
            {
                string oldHash = await GetPasswordHashFromDatabase(username);
                if (string.IsNullOrEmpty(oldHash))
                    return false;

                if (!Utils.VerifyPassword(oldPassword, oldHash))
                    return false;

                string newHash = Utils.HashPassword(newPassword);

                using (SqlConnection con = new SqlConnection(ql_User_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_ChangeUserPassword", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@NewPassword", newHash);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true; // thành công
            }
            catch (SqlException ex)
            {
                // Lỗi từ SQL Server (ví dụ: SP không tồn tại, constraint, timeout...)
                MessageBox.Show("Lỗi SQL: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                // Lỗi khác (mất kết nối, null reference, ...)
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public async Task<DataTable> GetShiftAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT ShiftID, ShiftCode, ShiftName FROM Shift";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> UpsertAttendanceBatchAsync(List<(string EmployeeCode, DateTime WorkDate, double WorkingHours, string Note, string AttendanceLog)> attendanceData)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("EmployeeCode", typeof(string));
                dt.Columns.Add("WorkDate", typeof(DateTime));
                dt.Columns.Add("WorkingHours", typeof(double));
                dt.Columns.Add("Note", typeof(string));
                dt.Columns.Add("AttendanceLog", typeof(string));

                foreach (var item in attendanceData)
                {
                    dt.Rows.Add(item.EmployeeCode, item.WorkDate.Date, item.WorkingHours, item.Note, item.AttendanceLog);
                }

                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("dbo.UpsertAttendanceBatch", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@AttendanceList", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "dbo.AttendanceTableType";

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật batch chấm công: " + ex.Message);
                return false;
            }
        }

        public async Task<DataTable> GetGetRemainingLeaveAsync(int year)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetRemainingLeave", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<DataTable> GetAttendamceAsync(int month, int year)
        {
            var dt = new DataTable();

            try
            {
                using (var con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    using (var cmd = new SqlCommand("sp_GetAttendanceSummary", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Thêm tham số
                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;

                        // Đọc dữ liệu
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy dữ liệu chấm công: " + ex.Message, ex);
            }

            return dt;
        }

        public async Task<DataTable> GetHolidayAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Holiday";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetHolidayAsync(int month, int year)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                string query = @"SELECT *
                                FROM Holiday
                                WHERE MONTH(HolidayDate) = @Month AND YEAR(HolidayDate) = @Year";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }


        public async Task<bool> insertHolidayAsync(DateTime holidayDate, string holidayName)
        {
            string query = "sp_InsertHolidayAndAttendance";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HolidayDate", holidayDate.Date);
                        cmd.Parameters.AddWithValue("@HolidayName", holidayName);

                        // Dùng ReturnValue để nhận giá trị trả về
                        SqlParameter returnParam = new SqlParameter("@ReturnVal", SqlDbType.Int);
                        returnParam.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(returnParam);

                        await cmd.ExecuteNonQueryAsync();

                        int result = (int)returnParam.Value;
                        return result == 1;
                    }
                }
            }
            catch
            {
                return false;
            }
        }



        public async Task<bool> DeleteHolidayAsync(DateTime holidayDate)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                using (SqlCommand cmd = new SqlCommand("sp_DeleteHolidayAndAttendance", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@HolidayDate", holidayDate.Date);

                    await con.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();

                    // Vì SP có COMMIT TRANSACTION nên nếu không lỗi => xem như thành công
                    return true;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                MessageBox.Show($"General Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public async Task<DataTable> GetOvertimeTypeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM OvertimeType";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateOvertimeTypeAsync(int overtimeTypeID, string overtimeName, double salaryFactor, bool isActive)
        {
            string query = @"UPDATE OvertimeType SET 
                                OvertimeName=@OvertimeName, 
                                SalaryFactor=@SalaryFactor, 
                                IsActive=@IsActive 
                            WHERE OvertimeTypeID=@OvertimeTypeID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OvertimeTypeID", overtimeTypeID);
                        cmd.Parameters.AddWithValue("@OvertimeName", overtimeName);
                        cmd.Parameters.AddWithValue("@SalaryFactor", salaryFactor);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertOvertimeTypeAsync(string overtimeName, double salaryFactor, bool isActive)
        {
            int newId = -1;

            string query = @"INSERT INTO OvertimeType (OvertimeName, SalaryFactor, IsActive) 
                                OUTPUT INSERTED.OvertimeTypeID
                                VALUES (@OvertimeName, @SalaryFactor, @IsActive)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OvertimeName", overtimeName);
                        cmd.Parameters.AddWithValue("@SalaryFactor", salaryFactor);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> deleteOvertimeTypeAsync(int ID)
        {
            string query = "DELETE FROM OvertimeType WHERE OvertimeTypeID=@OvertimeTypeID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OvertimeTypeID", ID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetOvertimeAttendamceForSalaryAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT 
                                    oa.OvertimeTypeID,
                                    oa.EmployeeCode, oa.WorkDate,
                                    ot.OvertimeName, ot.SalaryFactor,
                                    CAST(DATEDIFF(MINUTE, oa.StartTime, oa.EndTime) / 60.0 AS DECIMAL(10,2)) AS HourWork                                    
                                FROM OvertimeAttendance oa
                                JOIN OvertimeType ot ON oa.OvertimeTypeID = ot.OvertimeTypeID
                                WHERE MONTH(oa.WorkDate) = @Month
                                  AND YEAR(oa.WorkDate) = @Year
                                ORDER BY oa.EmployeeCode, oa.WorkDate;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetAttendamceForSalaryAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT EmployeeCode, WorkDate, WorkingHours
                                FROM Attendance
                                WHERE MONTH(WorkDate) = @Month AND YEAR(WorkDate) = @Year
                                ORDER BY EmployeeCode, WorkDate;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetOvertimeAttendamceAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT OvertimeAttendanceID, EmployeeCode, WorkDate, StartTime, EndTime, OvertimeTypeID, Note, UpdatedHistory,
                                CAST(DATEDIFF(MINUTE, StartTime, EndTime) / 60.0 AS DECIMAL(10,2)) AS HourWork  
                                FROM OvertimeAttendance
                                WHERE MONTH(WorkDate) = @Month
                                  AND YEAR(WorkDate) = @Year
                                ORDER BY EmployeeCode, WorkDate;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<bool> updateOvertimeAttendanceAsync(int overtimeAttendanceID, string employeeCode, DateTime workDate,
                                        TimeSpan startTime, TimeSpan endTime, int overtimeTypeID, string note, string updatedHistory)
        {
            string query = @"UPDATE OvertimeAttendance SET 
                                EmployeeCode=@EmployeeCode, 
                                WorkDate=@WorkDate,
                                StartTime=@StartTime, 
                                EndTime=@EndTime,
                                OvertimeTypeID=@OvertimeTypeID, 
                                Note=@Note,
                                UpdatedHistory=@UpdatedHistory                                
                            WHERE OvertimeAttendanceID=@OvertimeAttendanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OvertimeAttendanceID", overtimeAttendanceID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@WorkDate", workDate.Date);
                        cmd.Parameters.AddWithValue("@StartTime", startTime);
                        cmd.Parameters.AddWithValue("@EndTime", endTime);
                        cmd.Parameters.AddWithValue("@OvertimeTypeID", overtimeTypeID);
                        cmd.Parameters.AddWithValue("@Note", note);
                        cmd.Parameters.AddWithValue("@UpdatedHistory", updatedHistory);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertOvertimeAttendanceAsync(string employeeCode, DateTime workDate, TimeSpan startTime, TimeSpan endTime, 
                                                                int overtimeTypeID, string note, string updatedHistory)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO OvertimeAttendance (
                                        EmployeeCode, WorkDate, StartTime, EndTime, OvertimeTypeID, Note, UpdatedHistory
                                    )
                                    OUTPUT INSERTED.OvertimeAttendanceID
                                    VALUES (
                                        @EmployeeCode, @WorkDate, @StartTime, @EndTime, @OvertimeTypeID, @Note, @UpdatedHistory
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@WorkDate", workDate.Date);
                        cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = startTime;
                        cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = endTime;
                        cmd.Parameters.AddWithValue("@OvertimeTypeID", overtimeTypeID);
                        cmd.Parameters.AddWithValue("@Note", note);
                        cmd.Parameters.AddWithValue("@UpdatedHistory", updatedHistory);

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
        public async Task<DataTable> GetAnnualLeaveBalanceAsync(int year)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetAnnualLeaveReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }


        public async Task<bool> UpsertAnnualLeaveBalanceBatchAsync(List<(string EmployeeCode, int Year, string Month)> albData)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("EmployeeCode", typeof(string));
                dt.Columns.Add("Year", typeof(int));
                dt.Columns.Add("Month", typeof(string));

                foreach (var item in albData)
                {
                    dt.Rows.Add(item.EmployeeCode, item.Year, item.Month);
                }

                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("Upsert_AnnualLeaveBalance_Batch", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@AnnualLeaveList", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "AnnualLeaveBalanceType";

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật: " + ex.Message);
                return false;
            }
        }

        public async Task AutoUpsertAnnualLeaveMonthListAsync()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("UpdateAnnualLeaveMonthList", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                Console.WriteLine("[AutoUpsertAnnualLeaveMonthList] ✅ Cập nhật thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AutoUpsertAnnualLeaveMonthList] ❌ Lỗi: {ex.Message}");
            }
        }

        public async Task<DataTable> GetLeaveAttendanceAsync(int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM LeaveAttendance WHERE YEAR(DateOff) = @Year ORDER BY EmployeeCode, DateOff;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetLeaveAttendanceAsync_Withour_NghiLe(int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT *
                                FROM LeaveAttendance
                                WHERE YEAR(DateOff) = @Year
                                  AND LeaveTypeCode != 'NL_1'
                                ORDER BY EmployeeCode, DateOff;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetLeaveAttendanceAsync_IsPaid(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetPaidLeaveByMonth", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@Month", month);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetEmployeeDeductions(int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeDeductions", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetLeaveTypeAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                string query = @"SELECT * FROM LeaveType ";

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

        public async Task<int> insertLeaveAttendanceAsync(string employeeCode, string leaveTypeCode, DateTime dateOff, string Note, string UpdatedHistory, decimal hourLeave)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO LeaveAttendance  (
                                EmployeeCode, LeaveTypeCode, DateOff, Note, UpdatedHistory, LeaveHours
                            )
                            OUTPUT INSERTED.LeaveID
                            VALUES (
                                @EmployeeCode, @LeaveTypeCode, @DateOff, @Note, @UpdatedHistory, @LeaveHours
                            )";


            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@LeaveTypeCode", leaveTypeCode);
                        cmd.Parameters.AddWithValue("@DateOff", dateOff.Date);
                        cmd.Parameters.AddWithValue("@Note", Note);
                        cmd.Parameters.AddWithValue("@UpdatedHistory", UpdatedHistory);
                        cmd.Parameters.AddWithValue("@LeaveHours", hourLeave);

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

        public async Task<bool> updateLeaveAttendanceAsync(int leaveID, string employeeCode, string leaveTypeCode, DateTime dateOff, string Note, string UpdatedHistory, decimal hourLeave)
        {
            string query = @"UPDATE LeaveAttendance SET 
                                EmployeeCode=@EmployeeCode, 
                                LeaveTypeCode=@LeaveTypeCode,
                                DateOff=@DateOff, 
                                Note=@Note,
                                LeaveHours=@LeaveHours,
                                UpdatedHistory=@UpdatedHistory                                
                            WHERE LeaveID=@LeaveID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@LeaveID", leaveID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@LeaveTypeCode", leaveTypeCode);
                        cmd.Parameters.AddWithValue("@DateOff", dateOff.Date);
                        cmd.Parameters.AddWithValue("@Note", Note);
                        cmd.Parameters.AddWithValue("@LeaveHours", hourLeave);
                        cmd.Parameters.AddWithValue("@UpdatedHistory", UpdatedHistory);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> deleteLeaveAttendanceAsync(int leaveID)
        {
            string query = "DELETE FROM LeaveAttendance WHERE LeaveID=@LeaveID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@LeaveID", leaveID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetAllowanceTypeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM AllowanceType";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetAllowanceTypeAsync(string scopeCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT at.AllowanceTypeID,
                                at.AllowanceName,
                                at.IsInsuranceIncluded,
                                at.ApplyScopeID,
                                at.IsActive
                         FROM AllowanceType at
                         INNER JOIN ApplyScope aps
                             ON at.ApplyScopeID = aps.ApplyScopeID
                         WHERE aps.ScopeCode = @ScopeCode;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ScopeCode", scopeCode);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }


        public async Task<int> insertAllowanceTypeAsync(string allowanceName, string allowanceCode, int applyScopeID, bool isActive, bool isInsuranceIncluded)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO AllowanceType (
                                        AllowanceName, AllowanceCode, ApplyScopeID, IsActive, IsInsuranceIncluded
                                    )
                                    OUTPUT INSERTED.AllowanceTypeID
                                    VALUES (
                                        @AllowanceName, @AllowanceCode, @ApplyScopeID, @IsActive, @IsInsuranceIncluded
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@AllowanceName", allowanceName);
                        cmd.Parameters.AddWithValue("@AllowanceCode", allowanceCode);
                        cmd.Parameters.AddWithValue("@ApplyScopeID", applyScopeID);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@IsInsuranceIncluded", isInsuranceIncluded);

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

        public async Task<bool> updateAllowanceTypeAsync(int allowanceTypeID, string allowanceName, string allowanceCode, int applyScopeID, bool isActive, bool isInsuranceIncluded)
        {
            string query = @"UPDATE AllowanceType SET 
                                AllowanceName=@AllowanceName, 
                                AllowanceCode=@AllowanceCode, 
                                IsInsuranceIncluded=@IsInsuranceIncluded,
                                ApplyScopeID=@ApplyScopeID, 
                                IsActive=@IsActive
                            WHERE AllowanceTypeID=@AllowanceTypeID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@AllowanceName", allowanceName);
                        cmd.Parameters.AddWithValue("@AllowanceCode", allowanceCode);
                        cmd.Parameters.AddWithValue("@ApplyScopeID", applyScopeID);
                        cmd.Parameters.AddWithValue("@IsInsuranceIncluded", isInsuranceIncluded);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> deleteAllowanceTypeAsync(int allowanceTypeID)
        {
            string query = "DELETE FROM AllowanceType WHERE AllowanceTypeID=@AllowanceTypeID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetDepartmentAllowanceAsybc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM DepartmentAllowance";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateDepartmentAllowanceAsync(int departmentAllowanceID, int departmentID, int amount, int allowanceTypeID, string note)
        {
            string query = @"UPDATE DepartmentAllowance SET 
                                DepartmentID=@DepartmentID, 
                                AllowanceTypeID=@AllowanceTypeID,
                                Amount=@Amount, 
                                Note=@Note
                            WHERE DepartmentAllowanceID=@DepartmentAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentAllowanceID", departmentAllowanceID);
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertDepartmentAllowanceAsync(int departmentID, int amount, int allowanceTypeID, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO DepartmentAllowance (
                                        DepartmentID, AllowanceTypeID, Amount, Note
                                    )
                                    OUTPUT INSERTED.DepartmentAllowanceID
                                    VALUES (
                                        @DepartmentID, @AllowanceTypeID, @Amount, @Note
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);

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

        public async Task<bool> deleteDepartmentAllowanceAsync(int departmentAllowanceID)
        {
            string query = "DELETE FROM DepartmentAllowance WHERE DepartmentAllowanceID=@DepartmentAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentAllowanceID", departmentAllowanceID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetPositionAllowanceAsybc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM PositionAllowance";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updatePositionAllowanceAsync(int positionAllowanceID, int positionID, int amount, int allowanceTypeID, string note)
        {
            string query = @"UPDATE PositionAllowance SET 
                                PositionID=@PositionID, 
                                AllowanceTypeID=@AllowanceTypeID,
                                Amount=@Amount, 
                                Note=@Note
                            WHERE PositionAllowanceID=@PositionAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PositionAllowanceID", positionAllowanceID);
                        cmd.Parameters.AddWithValue("@PositionID", positionID);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertPositionAllowanceAsync(int positionID, int amount, int allowanceTypeID, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO PositionAllowance (
                                        PositionID, AllowanceTypeID, Amount, Note
                                    )
                                    OUTPUT INSERTED.PositionAllowanceID
                                    VALUES (
                                        @PositionID, @AllowanceTypeID, @Amount, @Note
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@PositionID", positionID);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);

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

        public async Task<bool> deletePositionAllowanceAsync(int positionAllowanceID)
        {
            string query = "DELETE FROM PositionAllowance WHERE PositionAllowanceID=@PositionAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PositionAllowanceID", positionAllowanceID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetEmployeeAllowanceAsybc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM EmployeeAllowance";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateEmployeeAllowanceAsync(int employeeAllowanceID, string employeeCode, int amount, int allowanceTypeID, string note)
        {
            string query = @"UPDATE EmployeeAllowance SET 
                                EmployeeCode=@EmployeeCode, 
                                AllowanceTypeID=@AllowanceTypeID,
                                Amount=@Amount, 
                                Note=@Note
                            WHERE EmployeeAllowanceID=@EmployeeAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeAllowanceID", employeeAllowanceID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertEmployeeAllowanceAsync(string employeeCode, int amount, int allowanceTypeID, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO EmployeeAllowance (
                                        EmployeeCode, AllowanceTypeID, Amount, Note
                                    )
                                    OUTPUT INSERTED.EmployeeAllowanceID
                                    VALUES (
                                        @EmployeeCode, @AllowanceTypeID, @Amount, @Note
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);

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

        public async Task<bool> deleteEmployeeAllowanceAsync(int employeeAllowanceID)
        {
            string query = "DELETE FROM EmployeeAllowance WHERE EmployeeAllowanceID=@EmployeeAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeAllowanceID", employeeAllowanceID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetMonthlyAllowanceAsybc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM MonthlyAllowance";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateMonthlyAllowanceAsync(int monthlyAllowanceID, string employeeCode, int month, int year, int amount, int allowanceTypeID, string note)
        {
            string query = @"UPDATE MonthlyAllowance SET 
                                EmployeeCode=@EmployeeCode, 
                                AllowanceTypeID=@AllowanceTypeID,
                                Month=@Month, 
                                Year=@Year, 
                                Amount=@Amount, 
                                Note=@Note
                            WHERE MonthlyAllowanceID=@MonthlyAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@MonthlyAllowanceID", monthlyAllowanceID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Month", month);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertMonthlyAllowanceAsync(string employeeCode, int month, int year, int amount, int allowanceTypeID, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO MonthlyAllowance (EmployeeCode, AllowanceTypeID, Month, Year, Amount, Note)
                                    OUTPUT INSERTED.MonthlyAllowanceID
                                    VALUES (@EmployeeCode, @AllowanceTypeID, @Month, @Year, @Amount, @Note)";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Month", month);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);

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

        public async Task<bool> deleteMonthlyAllowanceAsync(int monthlyAllowanceID)
        {
            string query = "DELETE FROM MonthlyAllowance WHERE MonthlyAllowanceID=@MonthlyAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeAllowaMonthlyAllowanceIDnceID", monthlyAllowanceID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetSalaryGradeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM SalaryGrade";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateSalaryGradeAsync(int salaryGradeID, string gradeName, int salary, string note, bool isActive)
        {
            string query = @"UPDATE SalaryGrade SET 
                                GradeName=@GradeName,
                                Salary=@Salary, 
                                Note=@Note, 
                                IsActive=@IsActive                             
                            WHERE SalaryGradeID=@SalaryGradeID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SalaryGradeID", salaryGradeID);
                        cmd.Parameters.AddWithValue("@GradeName", gradeName);
                        cmd.Parameters.AddWithValue("@Salary", salary);
                        cmd.Parameters.AddWithValue("@Note", note);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertSalaryGradeAsync(string gradeName, int salary, string note, bool isActive)
        {
            int newId = -1;

            string query = @"INSERT INTO SalaryGrade (GradeName, Salary, Note, IsActive) 
                                OUTPUT INSERTED.SalaryGradeID
                                VALUES (@GradeName, @Salary, @Note, @IsActive)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@GradeName", gradeName);
                        cmd.Parameters.AddWithValue("@Salary", salary);
                        cmd.Parameters.AddWithValue("@Note", note);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> deleteSalaryGradeAsync(int ID)
        {
            string query = "DELETE FROM SalaryGrade WHERE SalaryGradeID=@SalaryGradeID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SalaryGradeID", ID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetEmployeeSalaryInfoAsybc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM EmployeeSalaryInfo";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetSalaryHistoryAsyc(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(qlnvHis_conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM EmployeeSalaryHistory WHERE Month=@Month AND Year=@Year";
                using (SqlCommand cmd = new SqlCommand(query, con)) {
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<int> insertEmployeeSalaryInfoAsync(string employeeCode, int month, int year, int baseSalary, int insuranceBaseSalary, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO EmployeeSalaryInfo (EmployeeCode, Month, Year, BaseSalary, InsuranceBaseSalary, Note)
                                    OUTPUT INSERTED.SalaryInfoID
                                    VALUES (@EmployeeCode, @Month, @Year, @BaseSalary, @InsuranceBaseSalary, @Note)";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@Month", month);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@BaseSalary", baseSalary);
                        cmd.Parameters.AddWithValue("@InsuranceBaseSalary", insuranceBaseSalary);
                        cmd.Parameters.AddWithValue("@Note", note);

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

        public async Task<bool> deleteEmployeeSalaryInfoAsync(int salaryInfoID)
        {
            string query = "DELETE FROM EmployeeSalaryInfo WHERE SalaryInfoID=@SalaryInfoID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SalaryInfoID", salaryInfoID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> updateEmployeeBHAsync(string EmployeeCode, string SocialInsuranceNumber, string HealthInsuranceNumber)
        {
            string query = @"UPDATE Employee SET 
                                SocialInsuranceNumber=@SocialInsuranceNumber, HealthInsuranceNumber=@HealthInsuranceNumber
                            WHERE EmployeeCode=@EmployeeCode";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SocialInsuranceNumber", SocialInsuranceNumber);
                        cmd.Parameters.AddWithValue("@HealthInsuranceNumber", HealthInsuranceNumber);
                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> updateEmployeeBankAsync(string EmployeeCode, string BankName, string BankBranch, string BankAccountNumber, string BankAccountHolder)
        {
            string query = @"UPDATE Employee SET 
                                BankName=@BankName, BankBranch=@BankBranch, BankAccountNumber=@BankAccountNumber, BankAccountHolder=@BankAccountHolder
                            WHERE EmployeeCode=@EmployeeCode";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BankName", BankName);
                        cmd.Parameters.AddWithValue("@BankBranch", BankBranch);
                        cmd.Parameters.AddWithValue("@BankAccountNumber", BankAccountNumber);
                        cmd.Parameters.AddWithValue("@BankAccountHolder", BankAccountHolder);
                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> updateEmployeeWorkAsync(string EmployeeCode, int PositionID, int DepartmentID, int ContractTypeID)
        {
            string query = @"UPDATE Employee SET 
                                PositionID=@PositionID, DepartmentID=@DepartmentID, ContractTypeID=@ContractTypeID
                            WHERE EmployeeCode=@EmployeeCode";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PositionID", PositionID);
                        cmd.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                        cmd.Parameters.AddWithValue("@ContractTypeID", ContractTypeID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetDeductionTypeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT DeductionTypeCode, DeductionTypeName FROM DeductionType";

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

        public async Task<DataTable> GetEmployeeDeductionAsync(int month, int year, string deductionCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT EmployeeDeductionID, EmployeeCode, DeductionDate, Amount, Note, updateHistory
                         FROM EmployeeDeduction
                         WHERE DeductionMonth = @Month AND DeductionYear = @Year AND DeductionTypeCode = @DeductionTypeCode";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@DeductionTypeCode", deductionCode);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }
        public async Task<DataTable> GetEmployeeDeductionAsync(int year, string deductionCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT EmployeeDeductionID, EmployeeCode, DeductionDate, Amount, Note, updateHistory
                         FROM EmployeeDeduction
                         WHERE DeductionYear = @Year AND DeductionTypeCode = @DeductionTypeCode";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@DeductionTypeCode", deductionCode);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }


        public async Task<bool> updateEmployeeDeductionAsync(int EmployeeDeductionID, string EmployeeCode, DateTime DeductionDate, int amount, string note, string updateHistory)
        {
            string query = @"UPDATE EmployeeDeduction SET 
                                EmployeeCode=@EmployeeCode,
                                DeductionMonth=@DeductionMonth,
                                DeductionYear=@DeductionYear,
                                DeductionDate=@DeductionDate,
                                Amount=@Amount,
                                Note=@Note,
                                UpdateHistory=@UpdateHistory
                            WHERE EmployeeDeductionID=@EmployeeDeductionID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeDeductionID", EmployeeDeductionID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        cmd.Parameters.AddWithValue("@DeductionMonth", DeductionDate.Month);
                        cmd.Parameters.AddWithValue("@DeductionYear", DeductionDate.Year);
                        cmd.Parameters.AddWithValue("@DeductionDate", DeductionDate.Date);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);
                        cmd.Parameters.AddWithValue("@UpdateHistory", updateHistory);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertEmployeeDeductionAsync(string EmployeeCode, string DeductionTypeCode, DateTime DeductionDate, int amount, string note, string updateHistory)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO EmployeeDeduction (
                                        EmployeeCode, DeductionTypeCode, DeductionMonth, DeductionYear, DeductionDate, Amount, Note, UpdateHistory
                                    )
                                    OUTPUT INSERTED.EmployeeDeductionID
                                    VALUES (
                                        @EmployeeCode, @DeductionTypeCode, @DeductionMonth, @DeductionYear, @DeductionDate, @Amount, @Note, @UpdateHistory
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        cmd.Parameters.AddWithValue("@DeductionTypeCode", DeductionTypeCode);
                        cmd.Parameters.AddWithValue("@DeductionMonth", DeductionDate.Month);
                        cmd.Parameters.AddWithValue("@DeductionYear", DeductionDate.Year);
                        cmd.Parameters.AddWithValue("@DeductionDate", DeductionDate.Date);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);
                        cmd.Parameters.AddWithValue("@UpdateHistory", updateHistory);

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                // ✅ Tùy chọn: log lỗi ra console hoặc file
                Console.WriteLine($"[InsertEmployeeDeductionAsync] Error: {ex.Message}");
            }
            return newId;
        }

        public async Task<bool> UpsertEmployeeDeductionAsync(string EmployeeCode, string DeductionTypeCode, DateTime DeductionDate, int amount, string note, string updateHistory)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpsertEmployeeDeduction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        cmd.Parameters.AddWithValue("@DeductionTypeCode", DeductionTypeCode);
                        cmd.Parameters.AddWithValue("@DeductionMonth", DeductionDate.Month);
                        cmd.Parameters.AddWithValue("@DeductionYear", DeductionDate.Year);
                        cmd.Parameters.AddWithValue("@DeductionDate", DeductionDate.Date);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@UpdateHistory", updateHistory ?? (object)DBNull.Value);

                        // ExecuteScalarAsync chỉ để trigger SP, không cần lấy giá trị
                        await cmd.ExecuteScalarAsync();
                    }
                }

                return true; // Thành công
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpsertEmployeeDeductionAsync] Error: {ex.Message}");
                return false; // Lỗi
            }
        }


        public async Task<bool> deleteEmployeeDeductionAsync(int EmployeeDeductionID)
        {
            string query = "DELETE FROM EmployeeDeduction WHERE EmployeeDeductionID=@EmployeeDeductionID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeDeductionID", EmployeeDeductionID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetEmployeeAllowanceAsync(int month, int year)
        {
            DataTable dt = new DataTable();

            bool isLock = await SQLStore.Instance.IsSalaryLockAsync(month, year);
            using (SqlConnection con = new SqlConnection(isLock ? qlnvHis_conStr : conStr))
            {
                await con.OpenAsync();
                string spStr = isLock ? "sp_GetEmployeeAllowanceHistory_ByMonth" : "sp_GetEmployeeAllowance";
                using (SqlCommand cmd = new SqlCommand(spStr, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<bool> UpsertSaveEmployeeAllowanceHistoryBatchAsync(List<(string EmployeeCode, string scopeCode, int allowanceTypeID, string AllowanceName, bool IsInsuranceIncluded, int Amount, int Month, int Year)> data)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ScopeCode", typeof(string));
                dt.Columns.Add("AllowanceTypeID", typeof(int));
                dt.Columns.Add("EmployeeCode", typeof(string));                
                dt.Columns.Add("AllowanceName", typeof(string));
                dt.Columns.Add("IsInsuranceIncluded", typeof(bool));
                dt.Columns.Add("Amount", typeof(int));
                dt.Columns.Add("Month", typeof(int));
                dt.Columns.Add("Year", typeof(int));

                foreach (var item in data)
                {
                    dt.Rows.Add(item.scopeCode, item.allowanceTypeID, item.EmployeeCode, item.AllowanceName, item.IsInsuranceIncluded, item.Amount, item.Month, item.Year);
                }

                using (SqlConnection conn = new SqlConnection(qlnvHis_conStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_SaveEmployeeAllowanceHistory_Batch", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@AllowanceData", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "EmployeeAllowanceHistoryTableType";

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("====Lỗi khi cập nhật: " + ex.Message);
                return false;
            }
        }

        public async Task<DataTable> GetSalaryLockAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(salaryLock_conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM SalaryLock";

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

        public async Task<bool> UpsertLockSalaryAsync(int Month, int Year, bool IsLocked)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(salaryLock_conStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_SetSalaryLock", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Month", Month);
                        cmd.Parameters.AddWithValue("@Year", Year);
                        cmd.Parameters.AddWithValue("@IsLocked", IsLocked);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật: " + ex.Message);
                return false;
            }
        }

        
        public async Task<bool> UpsertEmployeeSalaryHistoryAsync(List<(string EmployeeCode, string ContractTypeName, int Month, int Year, decimal BaseSalary, decimal NetSalary, decimal NetInsuranceSalary,
            decimal InsuranceAllowance, decimal NonInsuranceAllowance, decimal OvertimeSalary, decimal LeaveSalary, decimal DeductionAmount, bool IsInsuranceRefund)> data)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("EmployeeCode", typeof(string));
                dt.Columns.Add("ContractTypeName", typeof(string));
                dt.Columns.Add("Month", typeof(int));
                dt.Columns.Add("Year", typeof(int));
                dt.Columns.Add("BaseSalary", typeof(decimal));
                dt.Columns.Add("NetSalary", typeof(decimal));
                dt.Columns.Add("NetInsuranceSalary", typeof(decimal));
                dt.Columns.Add("InsuranceAllowance", typeof(decimal));
                dt.Columns.Add("NonInsuranceAllowance", typeof(decimal));
                dt.Columns.Add("OvertimeSalary", typeof(decimal));
                dt.Columns.Add("LeaveSalary", typeof(decimal));
                dt.Columns.Add("DeductionAmount", typeof(decimal));
                dt.Columns.Add("IsInsuranceRefund", typeof(bool));


                foreach (var item in data)
                {
                    dt.Rows.Add(item.EmployeeCode, item.ContractTypeName, item.Month, item.Year, item.BaseSalary, item.NetSalary, item.NetInsuranceSalary, item.InsuranceAllowance,
                         item.NonInsuranceAllowance, item.OvertimeSalary, item.LeaveSalary, item.DeductionAmount, item.IsInsuranceRefund);
                }

                using (SqlConnection conn = new SqlConnection(qlnvHis_conStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_UpsertEmployeeSalaryHistory_Batch", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@SalaryData", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "EmployeeSalaryHistoryTableType";

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật: " + ex.Message);
                return false;
            }
        }

        public async Task<DataTable> GetSalarySummaryByYearAsync(int year)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(qlnvHis_conStr))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetSalarySummaryByYear", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<bool> SaveProductSKUHistoryAsync(int SKU, decimal priceCNF)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ql_khoHis_conStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_SaveProductSKUHistory", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@PriceCNF", priceCNF);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật: " + ex.Message);
                return false;
            }
        }

        public async Task<DataTable> GetProductSKUHistoryAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoHis_conStr))
            {
                await con.OpenAsync().ConfigureAwait(false);

                string query = @"SELECT * FROM ProductSKUHistory ORDER BY CreateAt ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    dt.Load(reader);
                }
            }

            return dt;
        }

        public async Task<bool> UpsertExportHistoryAsync(string exportCode, DateTime exportDate, decimal TotalMoney, decimal totalNW, int numberCarton, decimal freightCharge)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ql_khoHis_conStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("SP_Upsert_ExportHistory", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExportCode", exportCode);
                        cmd.Parameters.AddWithValue("@ExportDate", exportDate);
                        cmd.Parameters.AddWithValue("@TotalMoney", TotalMoney);
                        cmd.Parameters.AddWithValue("@TotalNW", totalNW);
                        cmd.Parameters.AddWithValue("@NumberCarton", numberCarton);
                        cmd.Parameters.AddWithValue("@FreightCharge", freightCharge);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật: " + ex.Message);
                return false;
            }
        }

        public async Task<DataTable> GetExportHistoryAsync(int year)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_khoHis_conStr))
            {
                await con.OpenAsync().ConfigureAwait(false);

                string query = @"SELECT * FROM ExportHistory WHERE YEAR(ExportDate) = @Year ORDER BY ExportDate ASC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Year", year);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<int> CloneOrder_SplitHalfAsync(int orderID)
        {
            int newOrderId = -1;
            try
            {
                using (SqlConnection conn = new SqlConnection(ql_kho_conStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("CloneOrder_SplitHalf", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OldOrderId", orderID);

                        SqlParameter outputParam = new SqlParameter("@NewOrderId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);
                        await cmd.ExecuteNonQueryAsync();
                        newOrderId = Convert.ToInt32(outputParam.Value);
                    }
                }

                return newOrderId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật: " + ex.Message);
                return newOrderId;
            }
        }

        public async Task<DataTable> getCartonSizeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM CartonSize";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task updateMachineInfoWhenLoginAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return;

            string computerName = Environment.MachineName;
            string ipAddress = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())
                                              .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                                              .ToString();

            string machineInfo = $"{computerName} - {ipAddress}";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateUserMachineInfo", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", userName);
                        cmd.Parameters.AddWithValue("@MachineInfo", machineInfo);
                        await cmd.ExecuteNonQueryAsync();

                        UserManager.Instance.machineInfo = machineInfo;
                    }
                }
            }
            catch { }
        }

        public async Task<bool> HaveOtherComputerLoginAsync()
        {
            using (SqlConnection con = new SqlConnection(ql_User_conStr))
            {
                await con.OpenAsync();
                string query = "SELECT MachineInfo FROM Users WHERE UserName = @UserName";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", UserManager.Instance.userName);
                    var result = await cmd.ExecuteScalarAsync();

                    if (result != null && result.ToString() != UserManager.Instance.machineInfo)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task AutoDeleteLoginHistoryAsync()
        {
            string query = "DELETE FROM UserLoginHistory WHERE  LoginTime < DATEADD(MONTH, -6, GETDATE());";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting login history: {ex.Message}");
            }
        }

        public async Task AutoUpdateCompleteExportCodeAsync()
        {
            string query = "UPDATE ExportCodes SET Complete = 1 WHERE ExportCodeID IN (SELECT TOP 5 ExportCodeID FROM ExportCodes WHERE ExportDate <= DATEADD(DAY, -10, GETDATE()) AND Complete = 0 ORDER BY ExportCodeID DESC);";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Auto Update ExportCode: {ex.Message}");
            }
        }

        public async Task<bool> deleteOrderNotUseAsync(int exportCodeID)
        {
            string query = @"DELETE FROM Orders
                                    WHERE (PCSReal IS NULL OR PCSReal <= 0)
                                        AND (NWReal IS NULL OR NWReal <= 0)
                                        AND (CartonSize IS NULL OR LTRIM(RTRIM(CartonSize)) = '')
                                        AND ExportCodeID = @ExportCodeID;";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> get3LatestOrdersAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT DISTINCT
                                    o.CustomerID,
                                    o.ProductPackingID
                                FROM Orders o
                                WHERE o.ExportCodeID IN (
                                    SELECT TOP 3 ExportCodeID
                                    FROM ExportCodes
                                    ORDER BY ExportCodeID DESC
                                );";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }
    }
}
