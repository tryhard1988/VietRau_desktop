
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
    public sealed class SQLManager_Kho
    {
        private static SQLManager_Kho ins = null;
        private static readonly object padlock = new object();

        private SQLManager_Kho() { }

        public static SQLManager_Kho Instance
        {
            get
            {
                lock (padlock)
                {
                    if (ins == null)
                        ins = new SQLManager_Kho();
                    return ins;
                }
            }
        }

        public string ql_kho_conStr() { return "Server=192.168.1.8,1433;Database=QL_Kho;User Id=ql_kho;Password=A7t#kP2x;"; }
        public string ql_kho_Log_conStr() { return "Server=192.168.1.8,1433;Database=QL_Kho_Log;User Id=ql_kho;Password=A7t#kP2x;"; }
        public string ql_khoHis_conStr() { return "Server=192.168.1.8,1433;Database=QL_Kho_History;User Id=ql_kho_history;Password=A7t#kP2x;"; }       

        //==============================Customers==============================
        public async Task<DataTable> getCustomersAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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

        public async Task<bool> updateCustomerAsync(int customerID, string name, string code, int Priority, string home, string company, string address, string email, string taxCode)
        {
            string query = "UPDATE Customers SET FullName=@FullName, CustomerCode=@CustomerCode, Priority=@Priority, Home=@Home, Company=@Company, Address=@Address, TaxCode=@TaxCode, Email=@Email WHERE CustomerID=@CustomerID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.Parameters.AddWithValue("@FullName", name);
                        cmd.Parameters.AddWithValue("@CustomerCode", code);
                        cmd.Parameters.AddWithValue("@Priority", Priority);
                        cmd.Parameters.AddWithValue("@Home", home);
                        cmd.Parameters.AddWithValue("@Company", company);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@TaxCode", taxCode);
                        cmd.Parameters.AddWithValue("@Email", email);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertCustomerAsync(string name, string code, int priority, string home, string company, string address, string email, string taxCode)
        {
            int newId = -1;
            string query = @"INSERT INTO Customers (FullName, CustomerCode, Priority, Home, Company, Address, TaxCode, Email) 
                             OUTPUT INSERTED.CustomerID
                            VALUES (@FullName, @CustomerCode, @Priority, @Home, @Company, @Address, @TaxCode, @Email)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FullName", name);
                        cmd.Parameters.AddWithValue("@CustomerCode", code);
                        cmd.Parameters.AddWithValue("@Priority", priority);
                        cmd.Parameters.AddWithValue("@Home", home);
                        cmd.Parameters.AddWithValue("@Company", company);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@TaxCode", taxCode);
                        cmd.Parameters.AddWithValue("@Email", email);
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

        public async Task<bool> deleteCustomerAsync(int customerID)
        {
            string query = "DELETE FROM Customers WHERE CustomerID=@CustomerID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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

        public async Task<bool> updateProductDomesticPriceAsync(int SKU, int rawPrice, int refinePrice, int packedPrice, bool isActive)
        {
            string query = @"UPDATE ProductDomesticPrices SET
                                RawPrice=@RawPrice, 
                                RefinedPrice=@RefinedPrice, 
                                PackedPrice=@PackedPrice, 
                                IsActive=@IsActive
                             WHERE SKU=@SKU";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@RawPrice", rawPrice);
                        cmd.Parameters.AddWithValue("@RefinedPrice", refinePrice);
                        cmd.Parameters.AddWithValue("@PackedPrice", packedPrice);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }
        public async Task<int> insertProductDomesticPriceAsync(int SKU, int rawPrice, int refinePrice, int packedPrice)
        {
            int newId = -1;
            string query = @"INSERT INTO ProductDomesticPrices (SKU, RawPrice, RefinedPrice, PackedPrice)
                            OUTPUT INSERTED.PriceID
                             VALUES (@SKU, @RawPrice, @RefinedPrice, @PackedPrice)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@RawPrice", rawPrice);
                        cmd.Parameters.AddWithValue("@RefinedPrice", refinePrice);
                        cmd.Parameters.AddWithValue("@PackedPrice", packedPrice);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> deleteProductDomesticPriceAsync(int priceID)
        {
            string query = "DELETE FROM ProductDomesticPrices WHERE PriceID=@PriceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PriceID", priceID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> deleteProductpackingAsync(int productPackingID)
        {
            string query = "DELETE FROM ProductPacking WHERE ProductPackingID=@ProductPackingID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                string query = "SELECT TOP 30 * FROM ExportCodes ORDER BY Complete ASC, ExportCodeID DESC";
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
                                InputBy=@InputBy, 
                                PackingBy=@PackingBy, 
                                Complete=@Complete 
                            WHERE ExportCodeID=@ExportCodeID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                        cmd.Parameters.AddWithValue("@ExportCode", exportCode);
                        cmd.Parameters.AddWithValue("@ExportCodeIndex", exportCodeIndex);
                        cmd.Parameters.AddWithValue("@ExportDate", exportDate);
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExportCode", exportCode);
                        cmd.Parameters.AddWithValue("@ExportCodeIndex", exportCodeIndex);
                        cmd.Parameters.AddWithValue("@ExportDate", exportDate.Date);
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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

            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (var con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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

            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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

            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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

            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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

        public async Task<bool> SaveProductSKUHistoryAsync(int SKU, decimal priceCNF)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ql_khoHis_conStr()))
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

            using (SqlConnection con = new SqlConnection(ql_khoHis_conStr()))
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
                using (SqlConnection conn = new SqlConnection(ql_khoHis_conStr()))
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

            using (SqlConnection con = new SqlConnection(ql_khoHis_conStr()))
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
                using (SqlConnection conn = new SqlConnection(ql_kho_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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

        public async Task AutoUpdateCompleteExportCodeAsync()
        {
            string query = "UPDATE ExportCodes SET Complete = 1 WHERE ExportCodeID IN (SELECT TOP 5 ExportCodeID FROM ExportCodes WHERE ExportDate <= DATEADD(DAY, -4, GETDATE()) AND Complete = 0 ORDER BY ExportCodeID DESC);";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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

        public async Task<DataTable> get3LatestOrdersAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
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

        public async Task<DataTable> GetExportCodeLogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM ExportCode_Log";

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

        public async Task InsertExportCodeLogAsync(string exportCode, string description, DateTime? exportDate, decimal? exRate, decimal? shippingCost, string inputBy, string packingBy, bool complete)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_Insert_ExportCode_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExportCode", exportCode);
                        cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExportDate", (object)exportDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExRate", (object)exRate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ShippingCost", (object)shippingCost ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@InputBy", (object)inputBy ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PackingBy", (object)packingBy ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Complete", complete);
                        cmd.Parameters.AddWithValue("@CreatedBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Auto Update ExportCode: {ex.Message}");
            }
        }

        public async Task AutoDeleteExportCodeLogAsync()
        {
            string query = "DELETE FROM ExportCode_Log WHERE  CreatedDate < DATEADD(MONTH, -4, GETDATE());";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
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
                Console.WriteLine($"Error deleting ExportCode Log: {ex.Message}");
            }
        }

        public async Task<DataTable> GetOrderLogAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Order_Log WHERE ExportCodeID = @ExportCodeID";

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
        public async Task InsertOrderLogAsync(int exportCodeID, int orderID, string description, string customer, string productName, int? OrderPCS, decimal? OrderNW)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_Insert_Order_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                        cmd.Parameters.AddWithValue("@OrderID", orderID);
                        cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Customer", (object)customer ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProductName", (object)productName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OrderPCS", (object)OrderPCS ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@OrderNW", (object)OrderNW ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Insert Order Log: {ex.Message}");
            }
        }

        public async Task AutoDeleteOrderLogAsync()
        {
            string query = "DELETE FROM Order_Log WHERE  CreateAt < DATEADD(MONTH, -4, GETDATE());";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
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
                Console.WriteLine($"Error deleting Order Log: {ex.Message}");
            }
        }

        public async Task<DataTable> GetOrderPackingLogAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM OrderPacking_Log WHERE ExportCodeID = @ExportCodeID";

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

        public async Task InsertOrderPackingLogAsync(int exportCodeID, int orderID, string description, int? PCSReal, decimal? NWReal, int? CartonNo, string CartonSize)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_Insert_OrderPacking_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                        cmd.Parameters.AddWithValue("@OrderID", orderID);
                        cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PCSReal", (object)PCSReal ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NWReal", (object)NWReal ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CartonNo", (object)CartonNo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CartonSize", (object)CartonSize ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Insert Order Log: {ex.Message}");
            }
        }

        public async Task InsertOrderPackingLogBulkAsync(List<(int exportCodeID, int orderID, string description, int? PCSReal, decimal? NWReal, int? CartonNo, string CartonSize, string CustomCarton)> logs)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ExportCodeID", typeof(int));
            dt.Columns.Add("OrderID", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("PCSReal", typeof(int));
            dt.Columns.Add("NWReal", typeof(decimal));
            dt.Columns.Add("CartonNo", typeof(int));
            dt.Columns.Add("CartonSize", typeof(string));
            dt.Columns.Add("ActionBy", typeof(string));
            dt.Columns.Add("CustomCarton", typeof(string));

            foreach (var log in logs)
            {
                dt.Rows.Add(
                    log.exportCodeID,
                    log.orderID,
                    log.description ?? (object)DBNull.Value,
                    log.PCSReal ?? (object)DBNull.Value,
                    log.NWReal ?? (object)DBNull.Value,
                    log.CartonNo ?? (object)DBNull.Value,
                    string.IsNullOrEmpty(log.CartonSize) ? (object)DBNull.Value : log.CartonSize,
                    UserManager.Instance.fullName,
                    log.CustomCarton
                );
            }

            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_Insert_OrderPacking_Log_Batch", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LogTable", dt);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task AutoDeleteOrderPackingLogAsync()
        {
            string query = "DELETE FROM OrderPacking_Log WHERE  CreateAt < DATEADD(MONTH, -4, GETDATE());";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
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
                Console.WriteLine($"Error deleting Order Packing Log: {ex.Message}");
            }
        }

        public async Task<DataTable> GetDo47LogAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Do47_Log WHERE ExportCodeID = @ExportCodeID";

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

        public async Task InsertDo47LogAsync(int exportCodeID, int productPackingID, string description, decimal? NWOrder, decimal? NWFinal, decimal? NWReal)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_Insert_Do47_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                        cmd.Parameters.AddWithValue("@ProductPackingID", productPackingID);
                        cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NWOrder", (object)NWOrder ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NWReal", (object)NWReal ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NetWeightFinal", (object)NWFinal ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Insert Do47 Log: {ex.Message}");
            }
        }

        public async Task AutoDeleteDo47LogAsync()
        {
            string query = "DELETE FROM Do47_Log WHERE  CreateAt < DATEADD(MONTH, -4, GETDATE());";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
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
                Console.WriteLine($"Error deleting Do 47 Log: {ex.Message}");
            }
        }

        public async Task<DataTable> GetLotCodeLogAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM LotCode_Log WHERE ExportCodeID = @ExportCodeID";

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

        public async Task InsertLotCodeLogAsync(int exportCodeID, int SKU, string description, string lotCode, string lotCodeHeader, string lotCodeComplete)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_LotCodeLog_Insert", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LotCode", (object)lotCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LotCodeHeader", (object)lotCodeHeader ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LotCodeComplete", (object)lotCodeComplete ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Insert Do47 Log: {ex.Message}");
            }
        }

        public async Task AutoDeletLotCodeLogAsync()
        {
            string query = "DELETE FROM LotCode_Log WHERE  CreateAt < DATEADD(MONTH, -4, GETDATE());";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
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
                Console.WriteLine($"Error deleting Do 47 Log: {ex.Message}");
            }
        }

        public async Task<bool> CustomerOrderDetailHistory_SaveListAsync(List<(string customerName, string exportCode, DateTime exportDate, string productVN, string productEN, string package, int PCS, decimal netWeight, decimal amount, int priority)> orders)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoHis_conStr()))
                {
                    await con.OpenAsync();

                    using (var dt = new DataTable())
                    {
                        dt.Columns.Add("CustomerName", typeof(string));
                        dt.Columns.Add("ExportCode", typeof(string));
                        dt.Columns.Add("ExportDate", typeof(DateTime));
                        dt.Columns.Add("ProductName_VN", typeof(string));
                        dt.Columns.Add("ProductName_EN", typeof(string));
                        dt.Columns.Add("Package", typeof(string));
                        dt.Columns.Add("NetWeight", typeof(decimal));
                        dt.Columns.Add("PCS", typeof(int));
                        dt.Columns.Add("AmountCHF", typeof(decimal));
                        dt.Columns.Add("Priority", typeof(int));

                        foreach (var o in orders)
                        {
                            dt.Rows.Add(o.customerName, o.exportCode, o.exportDate.Date, o.productVN, o.productEN, o.package, o.netWeight, o.PCS, o.amount, o.priority);
                        }

                        using (var cmd = new SqlCommand("dbo.sp_CustomerOrderDetailHistory_SaveList", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 60;

                            var tableParam = new SqlParameter("@Items", SqlDbType.Structured)
                            {
                                TypeName = "dbo.CustomerOrderDetailHistoryList",
                                Value = dt
                            };

                            cmd.Parameters.Add(tableParam);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CustomerOrderDetailHistory_SaveListAsync] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public async Task<DataTable> GetCustomerOrderDetailHistory_InYearAsync(int year)
        {
            var dt = new DataTable();

            try
            {
                using (var con = new SqlConnection(ql_khoHis_conStr()))
                {
                    await con.OpenAsync();

                    using (var cmd = new SqlCommand("sp_CustomerOrderDetailHistory_GetSumByMonthAndProduct_InYear", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 20;

                        // Tạo parameter rõ ràng thay vì AddWithValue
                        cmd.Parameters.Add(new SqlParameter("@Year", SqlDbType.Int) { Value = year });

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetCustomerOrderHistory_GetSumByMonthAndProduct_InYearAsync] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw; // hoặc xử lý theo logic của bạn
            }

            return dt;
        }

        public async Task<DataTable> GetCustomerOrderDetailHistoryAsync()
        {
            var dt = new DataTable();

            try
            {
                using (var con = new SqlConnection(ql_khoHis_conStr()))
                {
                    await con.OpenAsync();

                    using (var cmd = new SqlCommand("sp_CustomerOrderDetailHistory_GetSumByMonthAndProduct", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 20;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetCustomerOrderDetailHistoryAsync] Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw; // hoặc xử lý theo logic của bạn
            }

            return dt;
        }

        public async Task<DataTable> getProductDomesticPricesAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                string query = "SELECT * FROM ProductDomesticPrices";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> getOrderDomesticCodeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                string query = "SELECT * FROM OrderDomesticCode ORDER BY OrderDomesticCodeID DESC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<int> insertOrderDomesticCodeAsync(int orderDomesticIndex, int customerID, DateTime deliveryDate, int inputBy, int packingBy)
        {
            int newId = -1;
            string query = @"INSERT INTO OrderDomesticCode (OrderDomesticIndex, CustomerID, DeliveryDate, InputBy, PackingBy) 
                                OUTPUT INSERTED.OrderDomesticCodeID
                                VALUES (@OrderDomesticIndex, @CustomerID, @DeliveryDate, @InputBy, @PackingBy)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderDomesticIndex", orderDomesticIndex);
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.Parameters.AddWithValue("@DeliveryDate", deliveryDate);
                        cmd.Parameters.AddWithValue("@InputBy", inputBy);
                        cmd.Parameters.AddWithValue("@PackingBy", packingBy);

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> updateOrderDomesticCodeAsync(int orderDomesticCodeID, int orderDomesticIndex, int customerID, DateTime deliveryDate, int inputBy, int packingBy, bool completed)
        {
            string query = @"UPDATE OrderDomesticCode SET 
                                OrderDomesticIndex=@OrderDomesticIndex, 
                                CustomerID=@CustomerID,
                                DeliveryDate=@DeliveryDate, 
                                InputBy=@InputBy, 
                                PackingBy=@PackingBy, 
                                Complete=@Complete 
                            WHERE OrderDomesticCodeID=@OrderDomesticCodeID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderDomesticCodeID", orderDomesticCodeID);
                        cmd.Parameters.AddWithValue("@OrderDomesticIndex", orderDomesticIndex);
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.Parameters.AddWithValue("@DeliveryDate", deliveryDate);
                        cmd.Parameters.AddWithValue("@InputBy", inputBy);
                        cmd.Parameters.AddWithValue("@PackingBy", packingBy);
                        cmd.Parameters.AddWithValue("@Complete", completed);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> DeleteOrderDomesticCodeAsync(int orderDomesticCodeID)
        {
            string queryStr = "DELETE FROM OrderDomesticCode WHERE OrderDomesticCodeID = @OrderDomesticCodeID";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlTransaction tran = con.BeginTransaction())
                    {

                        // Xóa Orders
                        using (SqlCommand cmd = new SqlCommand(queryStr, con, tran))
                        {
                            cmd.Parameters.AddWithValue("@OrderDomesticCodeID", orderDomesticCodeID);
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

        public async Task<DataTable> getOrderDomesticDetailAsync(int OrderDomesticCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM OrderDomesticDetail  WHERE OrderDomesticCodeID = @OrderDomesticCodeID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@OrderDomesticCodeID", OrderDomesticCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<bool> updateOrderDomesticDetailAsync(int orderDomesticDetailID, int packingId, string productType, int PCSOrder, decimal NWOrder, decimal price)
        {
            string query = @"UPDATE OrderDomesticDetail SET  
                                ProductPackingID=@ProductPackingID, 
                                CustomerProductTypesCode=@CustomerProductTypesCode,
                                Price=@Price, 
                                PCSOrder=@PCSOrder, 
                                NWOrder=@NWOrder
                             WHERE OrderDomesticDetailID=@OrderDomesticDetailID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderDomesticDetailID", orderDomesticDetailID);
                        cmd.Parameters.AddWithValue("@ProductPackingID", packingId);
                        cmd.Parameters.AddWithValue("@CustomerProductTypesCode", productType);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@PCSOrder", PCSOrder);
                        cmd.Parameters.AddWithValue("@NWOrder", NWOrder);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> updateOrderDomesticDetail_PackingAsync(int orderDomesticDetailID, int PCSReal, decimal NWReal, string Note)
        {
            string query = @"UPDATE OrderDomesticDetail SET  
                                PCSReal=@PCSReal,
                                NWReal=@NWReal,
                                Note=@Note
                             WHERE OrderDomesticDetailID=@OrderDomesticDetailID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderDomesticDetailID", orderDomesticDetailID);
                        cmd.Parameters.AddWithValue("@PCSReal", PCSReal);
                        cmd.Parameters.AddWithValue("@NWReal", NWReal);
                        cmd.Parameters.AddWithValue("@Note", Note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertOrderDomesticDetailAsync(int orderDomesticCodeID, int packingId, string productType, int PCSOrder, decimal NWOrder, decimal price)
        {
            int newId = -1;
            string query = @"INSERT INTO OrderDomesticDetail (OrderDomesticCodeID, ProductPackingID, CustomerProductTypesCode, Price, PCSOrder, NWOrder)
                            OUTPUT INSERTED.OrderDomesticDetailID
                             VALUES (@OrderDomesticCodeID, @ProductPackingID, @CustomerProductTypesCode, @Price, @PCSOrder, @NWOrder)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderDomesticCodeID", orderDomesticCodeID);
                        cmd.Parameters.AddWithValue("@ProductPackingID", packingId);
                        cmd.Parameters.AddWithValue("@CustomerProductTypesCode", productType);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@PCSOrder", PCSOrder);
                        cmd.Parameters.AddWithValue("@NWOrder", NWOrder);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> deleteOrderDomesticDetailAsync(int id)
        {
            string query = "DELETE FROM OrderDomesticDetail WHERE OrderDomesticDetailID=@OrderDomesticDetailID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderDomesticDetailID", id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> getProductTypeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                string query = @" SELECT * FROM CustomerProductTypes ";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task InsertOrderDomesticCodeLogAsync(int OrderDomesticIndex, string OldValue, string NewValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertOrderDomesticCodeLog", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderDomesticIndex", OrderDomesticIndex);
                        cmd.Parameters.AddWithValue("@OldValue", (object)OldValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)NewValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Auto Update ExportCode: {ex.Message}");
            }
        }

        public async Task<DataTable> GetOrderDomesticCodeLogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM OrderDomesticCodeLog";

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

        public async Task InsertOrderDomesticDetailLogAsync(int OrderDomesticDetailID, int OrderDomesticCodeID, string OldValue, string NewValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertOrderDomesticDetailLog", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderDomesticDetailID", OrderDomesticDetailID);
                        cmd.Parameters.AddWithValue("@OrderDomesticCodeID", OrderDomesticCodeID);
                        cmd.Parameters.AddWithValue("@OldValue", (object)OldValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)NewValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Auto Update ExportCode: {ex.Message}");
            }
        }

        public async Task<DataTable> GetOrderDomesticDetailLogAsync(int orderDomesticCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM OrderDomesticDetailLog WHERE OrderDomesticCodeID = @OrderDomesticCodeID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@OrderDomesticCodeID", orderDomesticCodeID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task InsertProductDomesticPricesHistory(int SKU, string OldValue, string NewValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_khoHis_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertProductDomesticPricesHistory", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@OldValue", (object)OldValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)NewValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Auto Update ExportCode: {ex.Message}");
            }
        }

        public async Task<DataTable> GetProductDomesticPricesHistoryAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_khoHis_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM ProductDomesticPricesHistory";

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

        public async Task<DataTable> GetOrderDomesticByMonthYearAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetOrderDomesticByMonthYear", con))
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

        public async Task<DataTable> GetOrderDomesticByYearAsync(int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetOrderDomesticByYear", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }
                
        public async Task<bool> updateNewPriceInOrderDomesticDetailAsync(int OrderDomesticCodeID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateOrderDomesticDetailPrice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderDomesticCodeID", OrderDomesticCodeID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> getInventoryTransactionAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM InventoryTransaction";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<int> insertInventoryTransactionAsync(int SKU, string TransactionType, int Quantity, DateTime TransactionDate, string note)
        {
            int newId = -1;
            string query = @"INSERT INTO InventoryTransaction (SKU, TransactionType, Quantity, TransactionDate, Note)
                            OUTPUT INSERTED.TransactionID
                             VALUES (@SKU, @TransactionType, @Quantity, @TransactionDate, @Note)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@TransactionType", TransactionType);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@TransactionDate", TransactionDate);
                        cmd.Parameters.AddWithValue("@Note", note);
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

        public async Task<bool> updateInventoryTransactionAsync(int ID, int SKU, string TransactionType, int Quantity, DateTime TransactionDate, string note)
        {
            string query = @"UPDATE InventoryTransaction SET
                                SKU=@SKU, 
                                TransactionType=@TransactionType, 
                                Quantity=@Quantity, 
                                TransactionDate=@TransactionDate, 
                                Note=@Note                              
                             WHERE TransactionID=@ID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@TransactionType", TransactionType);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@TransactionDate", TransactionDate);
                        cmd.Parameters.AddWithValue("@Note", note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false; 
            }
        }

        public async Task<bool> deleteInventoryTransactionAsync(int tranID)
        {
            string query = "DELETE FROM InventoryTransaction WHERE TransactionID=@TransactionID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@TransactionID", tranID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertInventoryTransactionLOGAsync(int SKU, string oldValue, string newValue)
        {
            int newId = -1;
            string query = @"INSERT INTO InventoryTransaction_Log (SKU, OldValue, NewValue, ActionBy)
                            OUTPUT INSERTED.LogID
                             VALUES (@SKU, @OldValue, @NewValue, @ActionBy)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@OldValue", oldValue);
                        cmd.Parameters.AddWithValue("@NewValue", newValue);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
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

        public async Task<DataTable> getInventoryTransactionLOGAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"select * from InventoryTransaction_Log";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> getDomesticLiquidationPriceAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                string query = @"select * from DomesticLiquidationPrice";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateDomesticLiquidationPriceAsync(int DomesticLiquidationPriceID, int SalePrice)
        {
            string query = @"UPDATE DomesticLiquidationPrice SET
                                SalePrice=@SalePrice
                             WHERE DomesticLiquidationPriceID=@DomesticLiquidationPriceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SalePrice", SalePrice);
                        cmd.Parameters.AddWithValue("@DomesticLiquidationPriceID", DomesticLiquidationPriceID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }
        public async Task<int> insertDomesticLiquidationPriceAsync(int SKU, int SalePrice)
        {
            int newId = -1;
            string query = @"INSERT INTO DomesticLiquidationPrice (SKU, SalePrice)
                            OUTPUT INSERTED.DomesticLiquidationPriceID
                             VALUES (@SKU, @SalePrice)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@SalePrice", SalePrice);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> deletetDomesticLiquidationPriceAsync(int domesticLiquidationPriceID)
        {
            string query = "DELETE FROM DomesticLiquidationPrice WHERE DomesticLiquidationPriceID=@DomesticLiquidationPriceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DomesticLiquidationPriceID", domesticLiquidationPriceID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetDomesticLiquidationPriceLogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM DomesticLiquidationPriceLog";

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

        public async Task InsertDomesticLiquidationPriceLogAsnc(int SKU, string OldValue, string NewValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertDomesticLiquidationPriceLog", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@OldValue", (object)OldValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)NewValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Auto Update ExportCode: {ex.Message}");
            }
        }

        public async Task<DataTable> getDomesticLiquidationImportAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                string query = @"select * from DomesticLiquidationImport";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }


        public async Task<DataTable> GetDomesticLiquidationImportLogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM DomesticLiquidationImportLog";

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


        public async Task<int> insertDomesticLiquidationImportAsync(DateTime importDate, int domesticLiquidationPriceID, decimal quantity, int price, int reportedByID)
        {
            int newId = -1;
            string query = @"INSERT INTO DomesticLiquidationImport (ImportDate, DomesticLiquidationPriceID, Quantity, Price, ReportedByID)
                            OUTPUT INSERTED.ImportID
                             VALUES (@ImportDate, @DomesticLiquidationPriceID, @Quantity, @Price, @ReportedByID)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ImportDate", importDate);
                        cmd.Parameters.AddWithValue("@DomesticLiquidationPriceID", domesticLiquidationPriceID);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@ReportedByID", reportedByID);
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
        }

        public async Task<bool> updateDDomesticLiquidationImportAsync(int importID, DateTime importDate, int domesticLiquidationPriceID, decimal quantity, int price, int reportedByI)
        {
            string query = @"UPDATE DomesticLiquidationImport SET
                                ImportDate=@ImportDate,
                                DomesticLiquidationPriceID=@DomesticLiquidationPriceID,
                                Quantity=@Quantity,
                                Price=@Price,
                                ReportedByID=@ReportedByID
                             WHERE ImportID=@ImportID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ImportDate", importDate);
                        cmd.Parameters.AddWithValue("@DomesticLiquidationPriceID", domesticLiquidationPriceID);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@ReportedByID", reportedByI);
                        cmd.Parameters.AddWithValue("@ImportID", importID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task insertDomesticLiquidationImportLogAsync(int domesticLiquidationPriceID, string oldValue, string newValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertDomesticLiquidationImportLog", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DomesticLiquidationPriceID", domesticLiquidationPriceID);
                        cmd.Parameters.AddWithValue("@OldValue", (object)oldValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)newValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Auto Update ExportCode: {ex.Message}");
            }
        }

        public async Task<bool> deletetDomesticLiquidationImportAsync(int importID)
        {
            string query = "DELETE FROM DomesticLiquidationImport WHERE ImportID=@ImportID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ImportID", importID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }
    }
}

