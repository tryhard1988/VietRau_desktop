using RauViet.ui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
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

        public async Task<DataTable> getSellerssAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                string query = "SELECT * FROM Sellers;";
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

        public async Task<int> insertSellerAsync(string name, string CCCD, string phone, string address, string bankAccount, string bankName)
        {
            int newId = -1;
            string query = @"INSERT INTO Sellers (SellerName, CitizenID, Phone, Address, BankAccount, BankName) 
                             OUTPUT INSERTED.SellerID
                            VALUES (@SellerName, @CitizenID, @Phone, @Address, @BankAccount, @BankName)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SellerName", name);
                        cmd.Parameters.AddWithValue("@CitizenID", CCCD);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@BankAccount", bankAccount);
                        cmd.Parameters.AddWithValue("@BankName", bankName);
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

        public async Task<bool> updateSellerAsync(int sellerID, string name, string CCCD, string phone, string address, string bankAccount, string bankName)
        {
            string query = "UPDATE Sellers SET SellerName=@SellerName, CitizenID=@CitizenID, Phone=@Phone, Address=@Address, BankAccount=@BankAccount, BankName=@BankName WHERE SellerID=@SellerID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SellerID", sellerID);
                        cmd.Parameters.AddWithValue("@SellerName", name);
                        cmd.Parameters.AddWithValue("@CitizenID", CCCD);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@BankAccount", bankAccount);
                        cmd.Parameters.AddWithValue("@BankName", bankName);
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
                using (SqlCommand cmd = new SqlCommand("sp_GetProductSKU", con))
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

        public async Task<bool> updateProductSKUAsync(int SKU, string ProductNameVN, string ProductNameEN, string PackingType, string Package,
            string PackingList, string BotanicalName, decimal PriceCNF, int priority, string plantingareaCode, string LOTCodeHeader, bool isActive, int? supplierID)
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
                                Priority=@Priority,
                                SupplierID=@SupplierID
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
                        cmd.Parameters.AddWithValue("@SupplierID", (object)supplierID ?? DBNull.Value);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> updatePriceProductSKUAsync(int SKU, decimal PriceCNF)
        {
            string query = @"UPDATE ProductSKU SET 
                                PriceCNF=@PriceCNF
                             WHERE SKU=@SKU";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@PriceCNF", PriceCNF);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertProductSKUAsync(string ProductNameVN, string ProductNameEN, string PackingType, string Package, 
            string PackingList, string BotanicalName, decimal PriceCNF, int priority, string plantingareaCode, string LOTCodeHeader, int? supplierID)
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
                        cmd.Parameters.AddWithValue("@SupplierID", (object)supplierID ?? DBNull.Value);
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
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_DeleteProductDomesticPrice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PriceID", priceID);

                        int rows = await cmd.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> deleteProductpackingAsync(int productPackingID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_DeleteProductPacking", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductPackingID", productPackingID);

                        int rows = await cmd.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        //==============================ExportCodes==============================
        public async Task<DataTable> getExportCodesAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetExportCodes", con))
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

        public async Task<bool> updateExportCodeAsync(int exportCodeID, string exportCode, int exportCodeIndex, DateTime exportDate, decimal? exRate, decimal? shippingCost, int inputBy, int packingBy, bool complete)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateExportCode", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

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
            catch
            {
                return false;
            }
        }

        public async Task<int> insertExportCodeAsync(string exportCode, int exportCodeIndex, DateTime exportDate, decimal? exRate, decimal? shippingCost, int inputBy, int packingBy)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertExportCode", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

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
            catch
            {
                return -1;
            }
        }

        //==================================================Others================================

        public async Task<DataTable> getOrdersAsync(int exportCodeID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetOrdersByExportCode", con))
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

        public async Task<bool> updateOrdersAsync(int orderId, int exportCodeId, int customerId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateOrder", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

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
            catch
            {
                return false;
            }
        }

        public async Task<int> insertOrderAsync(int customerId, int exportCodeId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertOrder", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@CustomerID", customerId);
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeId);
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
            catch
            {
                return -1;
            }
        }

        public async Task<bool> deleteOrderAsync(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_DeleteOrder", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderId", id);

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

        public async Task<DataTable> Clean_OrdersTotal_ByExportCodeAsyn(int exportCodeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SP_Clean_OrdersTotal_ByExportCode", con))
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

                using (SqlCommand cmd = new SqlCommand("sp_GetLOTCodeByExportCode", con))
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
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateNewPriceInOrderListWithExportCode", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);

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
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_AutoUpdateCompleteExportCode", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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

                using (SqlCommand cmd = new SqlCommand("sp_Get3LatestOrders", con))
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

                using (SqlCommand cmd = new SqlCommand("sp_GetProductDomesticPrices", con))
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

        public async Task<DataTable> getOrderDomesticCodeAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetOrderDomesticCode", con))
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

        public async Task<int> insertOrderDomesticCodeAsync(int orderDomesticIndex, int customerID, DateTime deliveryDate, int inputBy, int packingBy)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertOrderDomesticCode", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@OrderDomesticIndex", orderDomesticIndex);
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.Parameters.AddWithValue("@DeliveryDate", deliveryDate);
                        cmd.Parameters.AddWithValue("@InputBy", inputBy);
                        cmd.Parameters.AddWithValue("@PackingBy", packingBy);

                        // Tham số OUTPUT
                        var outputParam = new SqlParameter("@NewOrderDomesticCodeID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);

                        await cmd.ExecuteNonQueryAsync();

                        newId = (outputParam.Value != DBNull.Value) ? Convert.ToInt32(outputParam.Value) : -1;
                    }
                }

                return newId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<bool> updateOrderDomesticCodeAsync(int orderDomesticCodeID, int orderDomesticIndex, int customerID, DateTime deliveryDate, int inputBy, int packingBy, bool completed)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateOrderDomesticCode", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

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
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteOrderDomesticCodeAsync(int orderDomesticCodeID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_DeleteOrderDomesticCode", con, tran))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
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

                using (SqlCommand cmd = new SqlCommand("sp_GetOrderDomesticDetail", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateOrderDomesticDetail", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

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
            catch
            {
                return false;
            }
        }

        public async Task<bool> updateOrderDomesticDetail_PackingAsync(int orderDomesticDetailID, int PCSReal, decimal NWReal, string Note)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateOrderDomesticDetail_Packing", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@OrderDomesticDetailID", orderDomesticDetailID);
                        cmd.Parameters.AddWithValue("@PCSReal", PCSReal);
                        cmd.Parameters.AddWithValue("@NWReal", NWReal);
                        cmd.Parameters.AddWithValue("@Note", Note);

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

        public async Task<int> insertOrderDomesticDetailAsync(int orderDomesticCodeID, int packingId, string productType, int PCSOrder, decimal NWOrder, decimal price)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertOrderDomesticDetail", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@OrderDomesticCodeID", orderDomesticCodeID);
                        cmd.Parameters.AddWithValue("@ProductPackingID", packingId);
                        cmd.Parameters.AddWithValue("@CustomerProductTypesCode", productType);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@PCSOrder", PCSOrder);
                        cmd.Parameters.AddWithValue("@NWOrder", NWOrder);

                        // Tham số OUTPUT
                        SqlParameter outputParam = new SqlParameter("@NewOrderDomesticDetailID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);

                        await cmd.ExecuteNonQueryAsync();

                        if (outputParam.Value != DBNull.Value)
                            newId = Convert.ToInt32(outputParam.Value);
                    }
                }

                return newId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<bool> deleteOrderDomesticDetailAsync(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_DeleteOrderDomesticDetail", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderDomesticDetailID", id);

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

        public async Task<DataTable> getProductTypeAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetCustomerProductTypes", con))
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
                using (SqlCommand cmd = new SqlCommand("sp_GetInventoryTransaction", con))
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

        public async Task<int> insertInventoryTransactionAsync(int SKU, string TransactionType, int Quantity, DateTime TransactionDate, string note)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertInventoryTransaction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@TransactionType", TransactionType);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@TransactionDate", TransactionDate);
                        cmd.Parameters.AddWithValue("@Note", note);

                        // Tham số OUTPUT
                        SqlParameter outputParam = new SqlParameter("@NewTransactionID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);

                        await cmd.ExecuteNonQueryAsync();

                        if (outputParam.Value != DBNull.Value)
                            newId = Convert.ToInt32(outputParam.Value);
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
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateInventoryTransaction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TransactionID", ID);
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
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_DeleteInventoryTransaction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TransactionID", tranID);

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

                using (SqlCommand cmd = new SqlCommand("sp_GetDomesticLiquidationPrice", con))
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

        public async Task<bool> updateDomesticLiquidationPriceAsync(int DomesticLiquidationPriceID, int SalePrice)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateDomesticLiquidationPrice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@DomesticLiquidationPriceID", DomesticLiquidationPriceID);
                        cmd.Parameters.AddWithValue("@SalePrice", SalePrice);

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
        public async Task<int> insertDomesticLiquidationPriceAsync(int SKU, int SalePrice)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertDomesticLiquidationPrice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@SKU", SKU);
                        cmd.Parameters.AddWithValue("@SalePrice", SalePrice);

                        // Tham số OUTPUT
                        SqlParameter outputParam = new SqlParameter("@NewID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);

                        await cmd.ExecuteNonQueryAsync();

                        if (outputParam.Value != DBNull.Value)
                            newId = Convert.ToInt32(outputParam.Value);
                    }
                }

                return newId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<bool> deletetDomesticLiquidationPriceAsync(int domesticLiquidationPriceID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_DeleteDomesticLiquidationPrice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DomesticLiquidationPriceID", domesticLiquidationPriceID);

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

                using (SqlCommand cmd = new SqlCommand("sp_GetDomesticLiquidationImport", con))
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

            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertDomesticLiquidationImport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ImportDate", importDate);
                        cmd.Parameters.AddWithValue("@DomesticLiquidationPriceID", domesticLiquidationPriceID);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@ReportedByID", reportedByID);

                        // Tham số OUTPUT
                        SqlParameter outputParam = new SqlParameter("@NewImportID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);

                        await cmd.ExecuteNonQueryAsync();

                        if (outputParam.Value != DBNull.Value)
                            newId = Convert.ToInt32(outputParam.Value);
                    }
                }

                return newId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<bool> updateDDomesticLiquidationImportAsync(int importID, DateTime importDate, int domesticLiquidationPriceID, decimal quantity, int price, int reportedByID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateDomesticLiquidationImport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ImportID", importID);
                        cmd.Parameters.AddWithValue("@ImportDate", importDate);
                        cmd.Parameters.AddWithValue("@DomesticLiquidationPriceID", domesticLiquidationPriceID);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@ReportedByID", reportedByID);

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
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_DeleteDomesticLiquidationImport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ImportID", importID);

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

        public async Task<DataTable> getDomesticLiquidationExportAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetAllDomesticLiquidationExport", con))
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

        public async Task<DataTable> getDomesticLiquidationExportAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetDomesticLiquidationExportByMonth", con))
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

        public async Task<DataTable> GetDomesticLiquidationExportLogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetDomesticLiquidationExportLog", con))
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

        public async Task<int> insertDomesticLiquidationExportAsync(DateTime exportDate, int domesticLiquidationPriceID, decimal quantity, int price, int? employeeBuyID, bool isCanceled)
        {
            int newId = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertDomesticLiquidationExport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ExportDate", SqlDbType.DateTime).Value = exportDate;
                        cmd.Parameters.Add("@DomesticLiquidationPriceID", SqlDbType.Int).Value = domesticLiquidationPriceID;
                        cmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = quantity;
                        cmd.Parameters.Add("@Price", SqlDbType.Int).Value = price;
                        cmd.Parameters.Add("@EmployeeBuyID", SqlDbType.Int).Value = employeeBuyID ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@IsCanceled", SqlDbType.Bit).Value = isCanceled;

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

        public async Task<bool> updateDDomesticLiquidationExportAsync(int exportID, DateTime exportDate, int domesticLiquidationPriceID, decimal quantity, int price, int? employeeBuyID, bool isCanceled)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateDomesticLiquidationExport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ExportID", SqlDbType.Int).Value = exportID;
                        cmd.Parameters.Add("@ExportDate", SqlDbType.DateTime).Value = exportDate;
                        cmd.Parameters.Add("@DomesticLiquidationPriceID", SqlDbType.Int).Value = domesticLiquidationPriceID;
                        cmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = quantity;
                        cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = price;
                        cmd.Parameters.Add("@EmployeeBuyID", SqlDbType.Int).Value = employeeBuyID;
                        cmd.Parameters.Add("@IsCanceled", SqlDbType.Bit).Value = isCanceled;

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

        public async Task insertDomesticLiquidationExportLogAsync(int domesticLiquidationPriceID, string oldValue, string newValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertDomesticLiquidationExportLog", con))
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

        public async Task<bool> deletetDomesticLiquidationExportAsync(int exportID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteDomesticLiquidationExport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExportID", exportID);
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

        public async Task<DataTable> getVegetableWarehouseTransactionSync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetVegetableWarehouseTransaction_Last2Months", con))
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

        public async Task<int> insertVegetableWarehouseTransactionAsync(int SKU, string TransactionType,decimal Quantity, string FarmSourceCode, DateTime TransactionDate, string Note, int? sellerID, int price)
        {
            int newId = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertVegetableWarehouseTransaction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@SKU", SqlDbType.Int).Value = SKU;
                        cmd.Parameters.Add("@TransactionType", SqlDbType.NVarChar, 50).Value = TransactionType;
                        cmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = Quantity;
                        cmd.Parameters.Add("@FarmSourceCode", SqlDbType.NVarChar, 50).Value = FarmSourceCode;
                        cmd.Parameters.Add("@TransactionDate", SqlDbType.DateTime).Value = TransactionDate;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar, 250).Value = Note;
                        cmd.Parameters.Add("@SellerID", SqlDbType.Int).Value = sellerID ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Price", SqlDbType.Int).Value = price;

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

        public async Task<bool> updateVegetableWarehouseTransactionAsync(int ID, int SKU, string TransactionType, decimal Quantity, string FarmSourceCode, DateTime TransactionDate, string Note, int? sellerID, int price)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateVegetableWarehouseTransaction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID;
                        cmd.Parameters.Add("@SKU", SqlDbType.Int).Value = SKU;
                        cmd.Parameters.Add("@TransactionType", SqlDbType.NVarChar, 50).Value = TransactionType;
                        cmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = Quantity;
                        cmd.Parameters.Add("@FarmSourceCode", SqlDbType.NVarChar, 50).Value = FarmSourceCode;
                        cmd.Parameters.Add("@TransactionDate", SqlDbType.DateTime).Value = TransactionDate;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar, 250).Value = Note;
                        cmd.Parameters.Add("@SellerID", SqlDbType.Int).Value = sellerID ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@Price", SqlDbType.Int).Value = price;

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

        public async Task<bool> deleteVegetableWarehouseTransactionAsync(int tranID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_DeleteVegetableWarehouseTransaction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@TransactionID", SqlDbType.Int).Value = tranID;

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

        public async Task<int> insertVegetableWarehouseTransactionLOGAsync(string farmSourceCode, string oldValue, string newValue)
        {
            int newId = -1;
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_InsertVegetableWarehouseTransactionLOG", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FarmSourceCode", farmSourceCode);
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

        public async Task<DataTable> getVegetableWarehouseTransactionLOGAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetVegetableWarehouseTransactionLOG", con))
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

        public async Task<DataTable> GetSupplierAsybc()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetSupplier", con))
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

        public async Task<bool> updateSupplierAsync(int SupplierID, string SupplierName, string Phone, string citizen, string address, string bankName, string bankAccount)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateSupplier", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = SupplierID;
                        cmd.Parameters.Add("@SupplierName", SqlDbType.NVarChar, 200).Value = SupplierName;
                        cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 50).Value = Phone;
                        cmd.Parameters.Add("@Citizen", SqlDbType.NVarChar, 50).Value = citizen;
                        cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 250).Value = address;
                        cmd.Parameters.Add("@BankName", SqlDbType.NVarChar, 100).Value = bankName;
                        cmd.Parameters.Add("@BankAccount", SqlDbType.NVarChar, 50).Value = bankAccount;

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

        public async Task<int> insertSupplierAsync(string A, string B, string C, string D, string E, string F)
        {
            int newId = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertSupplier", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@SupplierName", SqlDbType.NVarChar, 200).Value = A;
                        cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 50).Value = B;
                        cmd.Parameters.Add("@Citizen", SqlDbType.NVarChar, 50).Value = C;
                        cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 250).Value = D;
                        cmd.Parameters.Add("@BankName", SqlDbType.NVarChar, 100).Value = E;
                        cmd.Parameters.Add("@BankAccount", SqlDbType.NVarChar, 50).Value = F;

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

        public async Task<bool> deleteSupplierAsync(int A)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_DeleteSupplier", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@SupplierID", SqlDbType.Int).Value = A;

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

        public async Task CopyLotCodeAsync(int A, int B)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_CopyLOTCode", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromExportCodeID ", (object)A);
                        cmd.Parameters.AddWithValue("@ToExportCodeID ", (object)B);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Auto Update ExportCode: {ex.Message}");
            }
        }

        public async Task<DataTable> getPurchasePricesASync()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_GetPurchasePrices", con))
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

        public async Task<int> insertPurchasePricesAsync(int A, int B, int C)
        {
            int newId = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertPurchasePrices", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@SKU", SqlDbType.Int).Value = B;
                        cmd.Parameters.Add("@SellerID", SqlDbType.Int).Value = A;
                        cmd.Parameters.Add("@Price", SqlDbType.Int).Value = C;

                        // Lấy ID vừa insert
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

        public async Task<bool> updatePurchasePricesAsync(int A, int B, int C, int D)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdatePurchasePrices", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = A;
                        cmd.Parameters.Add("@SKU", SqlDbType.Int).Value = C;
                        cmd.Parameters.Add("@SellerID", SqlDbType.Int).Value = B;
                        cmd.Parameters.Add("@Price", SqlDbType.Int).Value = D;

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

        public async Task<bool> deletePurchasePricesAsync(int A)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_DeletePurchasePrices", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PurchasePricesID", A);

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

        public async Task<DataTable> GetPurchasePricesLOGAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_GetPurchasePricesLogs", con))
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

        public async Task<int> insertPurchasePricesLOGAsync(int A, int B, string C, string D)
        {
            int newId = -1;
            using (SqlConnection con = new SqlConnection(ql_kho_Log_conStr()))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("sp_InsertPurchasePricesLog", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@SKU", A);
                    cmd.Parameters.AddWithValue("@SellerID", B);
                    cmd.Parameters.AddWithValue("@OldValue", D);
                    cmd.Parameters.AddWithValue("@NewValue", C);
                    cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);

                    object result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                        newId = Convert.ToInt32(result);
                }
            }
            return newId;
        }

        public async Task<bool> updateVegetableWarehouseTransaction_ThanhToanAsync(int A, bool B)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_kho_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpdateVegetableWarehouseTransaction_Payment", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@TransactionID", SqlDbType.Int).Value = A;
                        cmd.Parameters.Add("@IsPaid", SqlDbType.Bit).Value = B;

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
    }
}

