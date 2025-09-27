using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RauViet.ui;

namespace RauViet.classes
{
    public sealed class SQLManager
    {
        private static SQLManager ins = null;
        private static readonly object padlock = new object();
        public readonly string conStr = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=RauVietDB;Integrated Security=true;";

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

        //==============================Customers==============================
        public async Task<DataTable> getCustomersAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = "SELECT * FROM Customers";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateCustomerAsync(int customerID, string name, string code)
        {
            string query = "UPDATE Customers SET FullName=@FullName, CustomerCode=@CustomerCode WHERE CustomerID=@CustomerID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.Parameters.AddWithValue("@FullName", name);
                        cmd.Parameters.AddWithValue("@CustomerCode", code);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> insertCustomerAsync(string name, string code)
        {
            string query = "INSERT INTO Customers (FullName, CustomerCode) VALUES (@FullName, @CustomerCode)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FullName", name);
                        cmd.Parameters.AddWithValue("@CustomerCode", code);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> deleteCustomerAsync(int customerID)
        {
            string query = "DELETE FROM Customers WHERE CustomerID=@CustomerID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
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
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = "SELECT * FROM ProductSKU";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateProductSKUAsync(int SKU, string ProductNameVN, string ProductNameEN, string PackingType, string Package,
            string PackingList, string BotanicalName, decimal PriceCNF, int priority, string plantingareaCode, string LOTCodeHeader)
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
                                Priority=@Priority
                             WHERE SKU=@SKU";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
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
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> insertProductSKUAsync(string ProductNameVN, string ProductNameEN, string PackingType, string Package, 
            string PackingList, string BotanicalName, decimal PriceCNF, int priority, string plantingareaCode, string LOTCodeHeader)
        {
            string query = @"INSERT INTO ProductSKU (ProductNameVN, ProductNameEN, PackingType, Package, PackingList, BotanicalName, PriceCNF, Priority, PlantingAreaCode, LOTCodeHeader)
                             VALUES (@ProductNameVN, @ProductNameEN, @PackingType, @Package, @PackingList, @BotanicalName, @PriceCNF, @Priority, @PlantingAreaCode, @LOTCodeHeader)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
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
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> deleteProductSKUAsync(int SKU)
        {
            string query = "DELETE FROM ProductSKU WHERE SKU=@SKU";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
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
            using (SqlConnection con = new SqlConnection(conStr))
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

        public async Task<bool> updateProductpackingAsync(int ID, int SKU, string BarCode, string PLU, int? Amount, string packing, string barCodeEAN13, string artNr, string GGN)
        {
            string query = @"UPDATE ProductPacking SET
                                SKU=@SKU, 
                                BarCode=@BarCode, 
                                PLU=@PLU, 
                                Amount=@Amount, 
                                packing=@packing, 
                                BarCodeEAN13=@BarCodeEAN13, 
                                ArtNr=@ArtNr, 
                                GGN=@GGN
                             WHERE ProductPackingID=@ID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
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
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> insertProductpackingAsync(int SKU, string BarCode, string PLU, int? Amount, string packing, string barCodeEAN13, string artNr, string GGN)
        {
            string query = @"INSERT INTO ProductPacking (SKU, BarCode, PLU, Amount, packing, BarCodeEAN13, ArtNr, GGN)
                             VALUES (@SKU, @BarCode, @PLU, @Amount, @packing, @BarCodeEAN13, @ArtNr, @GGN)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
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
                using (SqlConnection con = new SqlConnection(conStr))
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
            using (SqlConnection con = new SqlConnection(conStr))
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

        public async Task<DataTable> getExportCodes_Incomplete()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = "SELECT ExportCodeID, ExportCode, ExportDate, ExchangeRate, ShippingCost FROM ExportCodes WHERE Complete = 0 ORDER BY ExportDate DESC;";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateExportCodeAsync(int exportCodeID, string exportCode, int exportCodeIndex, DateTime exportDate, decimal exRate, decimal shippingCost, bool complete)
        {
            string query = "UPDATE ExportCodes SET ExportCode=@ExportCode, ExportCodeIndex=@ExportCodeIndex, ExportDate=@ExportDate, ExchangeRate=@ExchangeRate, ShippingCost=@ShippingCost, ModifiedAt=@ModifiedAt, Complete=@Complete WHERE ExportCodeID=@ExportCodeID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
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
                        cmd.Parameters.AddWithValue("@ExchangeRate", exRate);
                        cmd.Parameters.AddWithValue("@ShippingCost", shippingCost);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> insertExportCodeAsync(string exportCode, int exportCodeIndex, DateTime exportDate, decimal exRate, decimal shippingCost)
        {
            string query = "INSERT INTO ExportCodes (ExportCode, ExportCodeIndex, ExportDate, ExchangeRate, ShippingCost) VALUES (@ExportCode, @ExportCodeIndex, @ExportDate, @ExchangeRate, @ShippingCost)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExportCode", exportCode);
                        cmd.Parameters.AddWithValue("@ExportCodeIndex", exportCodeIndex);
                        cmd.Parameters.AddWithValue("@ExportDate", exportDate);
                        cmd.Parameters.AddWithValue("@ExchangeRate", exRate);
                        cmd.Parameters.AddWithValue("@ShippingCost", shippingCost);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> deleteExportCodeAsync(int exportCodeID)
        {
            string query = "DELETE FROM ExportCodes WHERE ExportCodeID=@ExportCodeID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
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
        //==================================================Others================================

        public async Task<DataTable> getOrdersAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT 
    o.OrderId,
    o.ExportCodeID,
    c.CustomerID,    
    p.ProductPackingID,
    o.PCSOther,
    o.NWOther,
    o.PCSReal,
    o.NWReal,
    o.OrderPackingPriceCNF,
    s.Priority

FROM Orders o
INNER JOIN Customers c ON o.CustomerID = c.CustomerID
INNER JOIN ProductPacking p ON o.ProductPackingID = p.ProductPackingID
INNER JOIN ProductSKU s ON p.SKU = s.SKU
INNER JOIN ExportCodes ec ON o.ExportCodeID = ec.ExportCodeID
WHERE ec.Complete = 0
ORDER BY o.ExportCodeID;";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
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
                using (SqlConnection con = new SqlConnection(conStr))
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

        public async Task<bool> insertOrderAsync(int customerId, int exportCodeId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF)
        {
            string query = @"INSERT INTO Orders (CustomerID, ExportCodeID, ProductPackingID, OrderPackingPriceCNF, PCSOther, NWOther)
                             VALUES (@CustomerID, @ExportCodeID, @ProductPackingID, @OrderPackingPriceCNF, @PCSOther, @NWOther)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
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
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> deleteOrderAsync(int id)
        {
            string query = "DELETE FROM Customers Orders OrderId=@OrderId";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
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

            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                string query = @"
            SELECT 
                e.ExportCode,
                e.ExportDate,
                CONCAT(sku.ProductNameVN, ' ', sku.PackingType, ' ', pp.Amount, ' ', pp.packing) AS ProductPackingName,
                SUM(o.PCSOther) AS TotalPCSOther,
                SUM(o.NWOther) AS TotalNWOther,
                sku.Priority
            FROM Orders o
            INNER JOIN ExportCodes e
                ON o.ExportCodeID = e.ExportCodeID
            INNER JOIN ProductPacking pp
                ON o.ProductPackingID = pp.ProductPackingID
            INNER JOIN ProductSKU sku
                ON pp.SKU = sku.SKU
            WHERE e.Complete = 0 AND e.ExportCodeID = @exportCodeID
            GROUP BY 
                pp.ProductPackingID,
                sku.ProductNameVN,
                sku.PackingType,
                pp.Amount,
                pp.packing,
                e.ExportCode,
                e.ExportDate,
                sku.Priority
            ORDER BY 
                e.ExportCode,
                sku.Priority;
        ";

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

        public async Task<DataTable> getOrdersPackingAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"
            SELECT                 
                c.FullName AS CustomerName,    
                CONCAT(
                    s.ProductNameVN, ' ',
                    CAST(s.PackingType AS NVARCHAR(50)), ' ',
                    CAST(p.Amount AS NVARCHAR(50)), ' ',
                    CAST(p.packing AS NVARCHAR(50))
                ) AS ProductPackingName,  
                o.CustomerCarton,
                o.CartonNo,
                o.CartonSize,
                o.PCSReal,
                o.NWReal,          
                s.Priority,
                s.ProductNameEN,
                s.Package,
                ec.ExportCode,
                ec.ExportDate,
                p.Amount,
                p.packing,
                o.OrderId,
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

        public async Task<bool> updatePackOrdersBulkAsync(List<(int orderId, int? pcsReal, decimal? nwReal, int? cartonNo, string cartonSize, string customerCarton)> orders)
        {
            string query = @"UPDATE Orders 
                        SET PCSReal = @PCSReal, 
                            NWReal = @NWReal, 
                            CartonNo = @CartonNo, 
                            CartonSize = @CartonSize,
                            CustomerCarton = @CustomerCarton
                        WHERE OrderId = @OrderId";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    foreach (var o in orders)
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@OrderId", o.orderId);
                            cmd.Parameters.AddWithValue("@PCSReal", (object)o.pcsReal ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@NWReal", (object)o.nwReal ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CartonNo", (object)o.cartonNo ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CartonSize", (object)o.cartonSize ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CustomerCarton", (object)o.customerCarton ?? DBNull.Value);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> getOrdersTotalAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT 
    sku.ProductNameVN + ' ' + CAST(pp.Amount AS NVARCHAR(10)) + ' ' + pp.packing AS ProductNameVN,
          
    SUM(op.NWOther) AS TotalNWOther,
    SUM(op.NWReal) AS TotalNWReal,
    ot.NetWeightFinal,
    sku.Priority,
    op.ProductPackingID,  
    op.ExportCodeID
FROM Orders op
LEFT JOIN OrdersTotal ot 
    ON op.ProductPackingID = ot.ProductPackingID 
    AND op.ExportCodeID = ot.ExportCodeID
INNER JOIN ProductPacking pp 
    ON op.ProductPackingID = pp.ProductPackingID
INNER JOIN ProductSKU sku
    ON pp.SKU = sku.SKU
INNER JOIN ExportCodes ec
    ON op.ExportCodeID = ec.ExportCodeID
WHERE ec.Complete = 0
GROUP BY 
    sku.ProductNameVN,
    pp.Amount,
    pp.packing,
    sku.Priority,
    op.ProductPackingID,
    op.ExportCodeID,
    ot.NetWeightFinal,
    ec.ExportCode
ORDER BY 
    sku.Priority ASC,
    op.ExportCodeID,
    op.ProductPackingID;";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> UpsertOrdersTotalListAsync(List<(int ExportCodeID, int ProductPackingID, decimal? NetWeightFinal)> list)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    foreach (var item in list)
                    {
                        string checkQuery = @"SELECT COUNT(1) 
                                      FROM OrdersTotal
                                      WHERE ExportCodeID = @ExportCodeID
                                        AND ProductPackingID = @ProductPackingID";

                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@ExportCodeID", item.ExportCodeID);
                            checkCmd.Parameters.AddWithValue("@ProductPackingID", item.ProductPackingID);

                            int count = (int)await checkCmd.ExecuteScalarAsync();

                            if (count > 0)
                            {
                                string updateQuery = @"UPDATE OrdersTotal
                                               SET NetWeightFinal = @NetWeightFinal
                                               WHERE ExportCodeID = @ExportCodeID
                                                 AND ProductPackingID = @ProductPackingID";

                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                                {
                                    updateCmd.Parameters.AddWithValue("@NetWeightFinal", item.NetWeightFinal ?? (object)DBNull.Value);
                                    updateCmd.Parameters.AddWithValue("@ExportCodeID", item.ExportCodeID);
                                    updateCmd.Parameters.AddWithValue("@ProductPackingID", item.ProductPackingID);

                                    await updateCmd.ExecuteNonQueryAsync();
                                }
                            }
                            else
                            {
                                string insertQuery = @"INSERT INTO OrdersTotal (ExportCodeID, ProductPackingID, NetWeightFinal)
                                               VALUES (@ExportCodeID, @ProductPackingID, @NetWeightFinal)";

                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                                {
                                    insertCmd.Parameters.AddWithValue("@ExportCodeID", item.ExportCodeID);
                                    insertCmd.Parameters.AddWithValue("@ProductPackingID", item.ProductPackingID);
                                    insertCmd.Parameters.AddWithValue("@NetWeightFinal", item.NetWeightFinal ?? (object)DBNull.Value);

                                    await insertCmd.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DataTable> getOrdersDKKDAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT 
                                    p.ProductNameVN,
                                    p.ProductNameEN,
                                    o.ExportCodeID,
                                    p.BotanicalName,
                                    p.Priority,
                                    p.PlantingAreaCode,
                                    SUM(o.NWOther) * 1.1 AS NWOther
                                FROM Orders o
                                INNER JOIN ProductPacking pp ON o.ProductPackingID = pp.ProductPackingID
                                INNER JOIN ProductSKU p ON pp.SKU = p.SKU
                                INNER JOIN ExportCodes e ON o.ExportCodeID = e.ExportCodeID
                                WHERE e.Complete = 0
                                GROUP BY 
                                    p.ProductNameVN,
                                    p.ProductNameEN,
                                    o.ExportCodeID,
                                    p.BotanicalName,
                                    p.PlantingAreaCode,
                                    p.Priority
                                ORDER BY p.Priority DESC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> getOrdersPhytoAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT 
                                    p.ProductNameVN,
                                    p.ProductNameEN,
                                    o.ExportCodeID,
                                    p.BotanicalName,
                                    p.Priority,
                                    SUM(o.NetWeightFinal) AS NetWeightFinal
                                FROM OrdersTotal o
                                INNER JOIN ProductPacking pp ON o.ProductPackingID = pp.ProductPackingID
                                INNER JOIN ProductSKU p ON pp.SKU = p.SKU
                                INNER JOIN ExportCodes e ON o.ExportCodeID = e.ExportCodeID
                                WHERE e.Complete = 0
                                GROUP BY 
                                    p.ProductNameVN,
                                    p.ProductNameEN,
                                    o.ExportCodeID,
                                    p.BotanicalName,
                                    p.PlantingAreaCode,
                                    p.Priority
                                ORDER BY p.Priority DESC";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> getOrdersINVOICEAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT
                                    pp.PLU,
                                    sku.ProductNameEN,
                                    sku.ProductNameVN + ' ' + CAST(pp.Amount AS NVARCHAR(10)) + ' ' + pp.packing AS ProductNameVN,
                                    sku.Package,
                                    CAST(pp.Amount AS NVARCHAR(10)) + ' ' + pp.packing AS Packing,
                                    SUM(o.NWReal) AS NWReal,
                                    SUM(o.PCSReal) AS PCSReal,
                                    o.OrderPackingPriceCNF,
                                    o.ExportCodeID,
                                    sku.Priority
                                FROM Orders o
                                INNER JOIN ProductPacking pp
                                    ON o.ProductPackingID = pp.ProductPackingID
                                INNER JOIN ProductSKU sku
                                    ON pp.SKU = sku.SKU
                                INNER JOIN ExportCodes ec
                                    ON o.ExportCodeID = ec.ExportCodeID
                                WHERE ec.Complete = 0  
                                GROUP BY 
                                    pp.PLU,
                                    sku.ProductNameEN,
                                    sku.ProductNameVN,
                                    pp.Amount,
                                    pp.packing,
                                    sku.Package,
                                    o.OrderPackingPriceCNF,
                                    o.ExportCodeID,
                                    sku.Priority
                                ORDER BY sku.Priority ASC, o.ExportCodeID, pp.PLU;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetCustomersOrdersAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT
                                    o.ExportCodeID,
                                    c.FullName,
                                    SUM(o.NWReal) AS NWReal,
                                    SUM(o.PCSReal) AS PCSReal,
                                    sku.Package,
                                    o.OrderPackingPriceCNF,
                                    COUNT(*) AS CNTS
                                FROM Orders o
                                INNER JOIN Customers c
                                    ON o.CustomerID = c.CustomerID
                                INNER JOIN ProductPacking pp
                                    ON o.ProductPackingID = pp.ProductPackingID
                                INNER JOIN ProductSKU sku
                                    ON pp.SKU = sku.SKU
                                INNER JOIN ExportCodes ec
                                    ON o.ExportCodeID = ec.ExportCodeID
                                WHERE ec.Complete = 0 
                                GROUP BY
                                    o.ExportCodeID,
                                    c.FullName,
                                    sku.Package,
                                    o.OrderPackingPriceCNF
                                ORDER BY o.ExportCodeID, c.FullName;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetExportCartonCounts()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT
                                    o.ExportCodeID,
                                    o.CartonSize,
                                    COUNT(*) AS CountCarton
                                FROM Orders o
                                INNER JOIN ExportCodes ec
                                    ON o.ExportCodeID = ec.ExportCodeID
                                WHERE ec.Complete = 0
                                GROUP BY
                                    o.ExportCodeID,
                                    o.CartonSize
                                ORDER BY
                                    o.ExportCodeID,
                                    o.CartonSize;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetLOTCodeByExportCode_inComplete()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
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
                                    WHERE e.Complete = 0;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> UpsertOrdersLotCodesBySKUAsync(List<(int ExportCodeID, int SKU, string LOTCode, string LOTCodeComplete)> list)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    foreach (var item in list)
                    {
                        string updateQuery = @"UPDATE o
                        SET 
                            o.LOTCode = @LOTCode,
                            o.LOTCodeComplete = @LOTCodeComplete
                        FROM Orders o
                        INNER JOIN ProductPacking p ON o.ProductPackingID = p.ProductPackingID
                        WHERE o.ExportCodeID = @ExportCodeID
                          AND p.SKU = @SKU;";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@LOTCode", item.LOTCode ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@LOTCodeComplete", item.LOTCodeComplete ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@ExportCodeID", item.ExportCodeID);
                            cmd.Parameters.AddWithValue("@SKU", item.SKU);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DataTable> GetDetailPackingTotalByExportCode_incomplete()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT
                                    o.ExportCodeID,
                                    CONCAT(s.ProductNameVN, ' ', s.PackingType, ' ', p.Amount, ' ', p.packing) AS ProductNameVN,
                                    CONCAT(s.ProductNameEN, ' ', s.PackingType, ' ', p.Amount, ' ', p.packing) AS ProductNameEN,
                                    STRING_AGG(CAST(o.CartonNo AS NVARCHAR(20)), ', ') AS CartonNo,
                                    MAX(o.LOTCodeComplete) AS LOTCodeComplete,
                                    p.PLU,
                                    s.Package,
                                    SUM(o.NWReal) AS NWReal,
                                    CONCAT(p.Amount, ' ', p.packing) AS packing,
                                    SUM(o.PCSReal) AS PCSReal,
                                    STRING_AGG(o.CustomerCarton, ', ') AS CustomerCarton,
                                    s.Priority
                                FROM Orders o
                                INNER JOIN ProductPacking p ON o.ProductPackingID = p.ProductPackingID
                                INNER JOIN ProductSKU s ON p.SKU = s.SKU
                                INNER JOIN ExportCodes e ON o.ExportCodeID = e.ExportCodeID
                                WHERE e.Complete = 0
                                GROUP BY
                                    o.ExportCodeID,
                                    p.ProductPackingID,
                                    s.ProductNameVN,
                                    s.ProductNameEN,
                                    s.PackingType,
                                    p.Amount,
                                    p.packing,
                                    p.PLU,
                                    s.Package,
                                    s.Priority;";

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
