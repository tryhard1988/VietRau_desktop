using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using RauViet.ui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO.Packaging;
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

        public async Task<string> GetPasswordHashFromDatabase(string username)
        {
            string hash = "";

            using (SqlConnection con = new SqlConnection(conStr))
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

        public async Task<int> insertCustomerAsync(string name, string code)
        {
            int newId = -1;
            string query = @"INSERT INTO Customers (FullName, CustomerCode) 
                             OUTPUT INSERTED.CustomerID
                            VALUES (@FullName, @CustomerCode)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FullName", name);
                        cmd.Parameters.AddWithValue("@CustomerCode", code);
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

        public async Task<int> insertProductSKUAsync(string ProductNameVN, string ProductNameEN, string PackingType, string Package, 
            string PackingList, string BotanicalName, decimal PriceCNF, int priority, string plantingareaCode, string LOTCodeHeader)
        {
            int newId = -1;

            string query = @"INSERT INTO ProductSKU (ProductNameVN, ProductNameEN, PackingType, Package, PackingList, BotanicalName, PriceCNF, Priority, PlantingAreaCode, LOTCodeHeader)
                            OUTPUT INSERTED.SKU                             
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
                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
                return newId;
            }
            catch { return -1; }
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

        public async Task<int> insertProductpackingAsync(int SKU, string BarCode, string PLU, int? Amount, string packing, string barCodeEAN13, string artNr, string GGN)
        {
            int newId = -1;
            string query = @"INSERT INTO ProductPacking (SKU, BarCode, PLU, Amount, packing, BarCodeEAN13, ArtNr, GGN)
                            OUTPUT INSERTED.ProductPackingID
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
                string query = @"SELECT 
                                    ec.ExportCodeID,
                                    ec.ExportCode,
                                    ec.ExportDate,
                                    ec.ExchangeRate,
                                    ec.ShippingCost,
                                    ec.ExportCodeIndex,
                                    ei.FullName AS InputByName,
                                    ep.FullName AS PackingByName
                                FROM 
                                    ExportCodes ec
                                LEFT JOIN Employee ei ON ec.InputBy = ei.EmployeeID
                                LEFT JOIN Employee ep ON ec.PackingBy = ep.EmployeeID
                                WHERE 
                                    ec.Complete = 0
                                ORDER BY 
                                    ec.ExportDate DESC;";
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
                using (SqlConnection con = new SqlConnection(conStr))
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
                using (SqlConnection con = new SqlConnection(conStr))
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

        public async Task<int> insertOrderAsync(int customerId, int exportCodeId, int packingId, int PCSOther, decimal NWOther, decimal priceCNF)
        {
            int newId = -1;
            string query = @"INSERT INTO Orders (CustomerID, ExportCodeID, ProductPackingID, OrderPackingPriceCNF, PCSOther, NWOther)
                            OUTPUT INSERTED.OrderId
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

                string query = @"SELECT 
                                    e.ExportCode,
                                    e.ExportDate,
                                    CASE 
                                        WHEN sku.Package = 'kg' AND pp.Amount > 0 AND ISNULL(pp.packing,'') <> ''
                                        THEN CONCAT(sku.ProductNameVN, ' ', sku.PackingType, ' ', 
                                                    CASE 
                                                        WHEN pp.Amount = FLOOR(pp.Amount) THEN CAST(FLOOR(pp.Amount) AS VARCHAR(20))
                                                        ELSE CAST(pp.Amount AS VARCHAR(20))
                                                    END
                                                    , ' ', pp.packing)
                                        ELSE sku.ProductNameVN
                                    END AS ProductPackingName,
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
                                    sku.Package,
                                    e.ExportCode,
                                    e.ExportDate,
                                    sku.Priority
                                ORDER BY 
                                    sku.Priority,
                                    sku.ProductNameVN;
                                    

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
                s.ProductNameVN AS ProductPackingName,
                s.PackingType,  
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
                                    sku.SKU,
                                    sku.ProductNameVN,
                                    pp.Amount,
                                    pp.packing,
                                    sku.Package,
                                    SUM(op.NWOther) AS TotalNWOther,
                                    SUM(op.NWReal) AS TotalNWReal,
                                    MAX(ot.NetWeightFinal) AS NetWeightFinal,
                                    sku.Priority,
                                    op.ProductPackingID,  
                                    op.ExportCodeID,
                                    ec.ExportCode
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
                                    sku.SKU,
                                    sku.ProductNameVN,
                                    pp.Amount,
                                    pp.packing,
                                    sku.Package,
                                    sku.Priority,
                                    op.ProductPackingID,
                                    op.ExportCodeID,
                                    ec.ExportCode
                                ORDER BY 
                                    sku.Priority ASC,
                                    op.ExportCodeID,
                                    op.ProductPackingID;
";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> deleteOrderTotalAsync(int exportCodeID)
        {
            string query = "DELETE FROM OrdersTotal WHERE ExportCodeID =@ExportCodeID ";
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
                                    FORMAT(SUM(o.NWOther) * 1.1, 'N2') AS NWOther
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
                                    CASE 
                                        WHEN sku.Package = 'kg' AND pp.Amount > 0 AND ISNULL(pp.packing, '') <> '' 
                                        THEN CONCAT(sku.ProductNameEN, ' ', FORMAT(pp.Amount, '0.##'), ' ', pp.packing)
                                        ELSE sku.ProductNameEN
                                    END AS ProductNameEN,
                                    CASE 
                                        WHEN sku.Package = 'kg' AND pp.Amount > 0 AND ISNULL(pp.packing, '') <> '' 
                                        THEN CONCAT(sku.ProductNameVN, ' ', FORMAT(pp.Amount, '0.##'), ' ', pp.packing)
                                        ELSE sku.ProductNameVN
                                    END AS ProductNameVN,
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
                                        SUM(
                                            CASE 
                                                WHEN sku.Package IN ('kg', 'weight') 
                                                    THEN o.OrderPackingPriceCNF * o.NWReal
                                                ELSE o.OrderPackingPriceCNF * o.PCSReal
                                            END
                                        ) AS AmountCHF,
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
                                        c.FullName
                                    ORDER BY 
                                        o.ExportCodeID, c.FullName;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetExportCartonCountsAsync()
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

        public async Task<DataTable> GetLOTCodeByExportCode_inCompleteAsync()
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

        public async Task<DataTable> GetDetailPackingTotalByExportCode_incompleteAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT
                                    o.ExportCodeID,
                                    CASE 
                                        WHEN s.Package = 'kg' AND p.Amount > 0 AND ISNULL(p.packing, '') <> '' 
                                        THEN CONCAT(s.ProductNameVN, ' ', s.PackingType, ' ', FORMAT(p.Amount, '0.##'), ' ', p.packing)
                                        ELSE s.ProductNameVN
                                    END AS ProductNameVN,
                                    CASE 
                                        WHEN s.Package = 'kg' AND p.Amount > 0 AND ISNULL(p.packing, '') <> '' 
                                        THEN CONCAT(s.ProductNameEN, ' ', s.PackingType, ' ', FORMAT(p.Amount, '0.##'), ' ', p.packing)
                                        ELSE s.ProductNameEN
                                    END AS ProductNameEN,
                                    STRING_AGG(CAST(o.CartonNo AS NVARCHAR(50)), ', ') AS CartonNo,
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
                                    s.Priority;
";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetCustomerDetailPacking_incompleteAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT
                                        c.FullName,
                                        sku.ProductNameVN,
                                        sku.ProductNameEN,
                                        sku.Package,
                                        pp.PLU,
                                        o.ExportCodeID,
                                        pp.Amount,
                                        pp.packing,
                                        SUM(o.NWReal) AS NWReal,
                                        SUM(o.PCSReal) AS PCSReal,
                                        STRING_AGG(o.CustomerCarton, ', ') AS CustomerCarton,
                                        STRING_AGG(CAST(o.CartonNo AS NVARCHAR(50)), ', ') AS CartonNo
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
                                        c.FullName,
                                        sku.ProductNameVN,
                                        sku.ProductNameEN,
                                        sku.Package,
                                        pp.PLU,
                                        o.ExportCodeID,
                                        pp.Amount,
                                        pp.packing
                                    ORDER BY c.FullName;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
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
            using (SqlConnection con = new SqlConnection(conStr))
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
                                    bool isMale, string homeTown, string address, string citizenID, DateTime? issueDate, string issuePlace, int department,
                                    int position, int contractType, bool isActive, bool canCreateUserName)
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
                                PositionID=@PositionID, 
                                DepartmentID=@DepartmentID,
                                ContractTypeID=@ContractTypeID, 
                                IsActive=@IsActive,
                                canCreateUserName=@canCreateUserName
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
                        cmd.Parameters.AddWithValue("@PositionID", position);
                        cmd.Parameters.AddWithValue("@DepartmentID", department);
                        cmd.Parameters.AddWithValue("@ContractTypeID", contractType);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@canCreateUserName", canCreateUserName);
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
                                                    string address, string citizenID, DateTime? issueDate, string issuePlace, int department, 
                                                    int position, int contractType, bool isActive, bool canCreateUserName)
        {
            int newId = -1;

            string insertQuery = @"
        INSERT INTO Employee (
            EmployeeCode, FullName, BirthDate, HireDate, Gender, Hometown, Address,
            CitizenID, IssueDate, IssuePlace,
            PositionID, DepartmentID, ContractTypeID, IsActive, canCreateUserName
        )
        OUTPUT INSERTED.EmployeeID
        VALUES (
            @EmployeeCode, @FullName, @BirthDate, @HireDate, @Gender, @Hometown, @Address,
            @CitizenID, @IssueDate, @IssuePlace,
            @PositionID, @DepartmentID, @ContractTypeID, @IsActive, @canCreateUserName
        )";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
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
                        cmd.Parameters.AddWithValue("@PositionID", position);
                        cmd.Parameters.AddWithValue("@DepartmentID", department);
                        cmd.Parameters.AddWithValue("@ContractTypeID", contractType);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@canCreateUserName", canCreateUserName);

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }

                    // 3️⃣ Cập nhật lại EmployeeCode thật
                    if (newId > 0)
                    {
                        string employeeCode = $"VR{newId:D4}";

                        using (SqlCommand updateCmd = new SqlCommand("UPDATE Employee SET EmployeeCode = @EmployeeCode WHERE EmployeeID = @ID", con))
                        {
                            updateCmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                            updateCmd.Parameters.AddWithValue("@ID", newId);
                            await updateCmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                return newId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<DataTable> GetActiveDepartmentAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Department WHERE IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetActivePositionAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Position WHERE IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
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

        public async Task<DataTable> GetUserDataAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT 
                                    u.UserID,
                                    u.Username,
                                    u.PasswordHash,
                                    u.EmployeeID,
                                    u.IsActive,
                                    STRING_AGG(r.RoleID, ',') AS RoleIDs
                                FROM Users u
                                LEFT JOIN UserRoles ur ON u.UserID = ur.UserID
                                LEFT JOIN Roles r ON ur.RoleID = r.RoleID
                                GROUP BY 
                                    u.UserID, u.Username, u.PasswordHash, u.EmployeeID, 
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

        public async Task<bool> updateUserAsync(int userID, string userName, string password, int employeeID, Boolean isActive, List<int> roleIDs)
        {
            string query = @"UPDATE Users SET 
                                Username=@Username, PasswordHash=@PasswordHash, EmployeeID=@EmployeeID, IsActive=@IsActive 
                            WHERE UserID=@UserID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Username", userName);
                        cmd.Parameters.AddWithValue("@PasswordHash", password);
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
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

        public async Task<bool> updateUser_notPasswordAsync(int userID, string userName, int employeeID, Boolean isActive, List<int> roleIDs)
        {
            string query = @"UPDATE Users SET 
                                Username=@Username, EmployeeID=@EmployeeID, IsActive=@IsActive 
                            WHERE UserID=@UserID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Username", userName);
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
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

        public async Task<int> insertUserDataAsync(string userName, string password, int employeeID, Boolean isActive, List<int> roleIDs)
        {
            int newId = -1;
            string query = @"INSERT INTO Users (Username, PasswordHash, EmployeeID, IsActive) 
                             OUTPUT INSERTED.UserID
                            VALUES (@Username, @PasswordHash, @EmployeeID, @IsActive)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", userName);
                        cmd.Parameters.AddWithValue("@PasswordHash", password);
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
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
                using (SqlConnection con = new SqlConnection(conStr))
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

        public async Task<DataRow> GetInfoUserAsync(string userName)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                string query = @"SELECT 
                                    e.EmployeeID,
                                    e.EmployeeCode,
                                    e.FullName,
                                    STRING_AGG(r.RoleCode, ',') AS RoleCodes
                                FROM Users u
                                INNER JOIN Employee e ON u.EmployeeID = e.EmployeeID
                                LEFT JOIN UserRoles ur ON u.UserID = ur.UserID
                                LEFT JOIN Roles r ON ur.RoleID = r.RoleID
                                WHERE u.Username = @Username
                                    AND u.IsActive = 1
                                    AND e.IsActive = 1
                                GROUP BY e.EmployeeID, e.EmployeeCode, e.FullName;
";

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

                using (SqlConnection con = new SqlConnection(conStr))
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

        public async Task<DataTable> GetEmployeeShiftAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT 
                                        e.EmployeeID,
                                        e.EmployeeCode,
                                        e.FullName,
                                        STRING_AGG(CAST(s.ShiftID AS NVARCHAR(10)), ',') AS ShiftIDs
                                    FROM Employee e
                                    LEFT JOIN EmployeeShift es 
                                        ON e.EmployeeID = es.EmployeeID
                                    LEFT JOIN Shift s 
                                        ON es.ShiftID = s.ShiftID
                                    WHERE e.IsActive = 1
                                    GROUP BY e.EmployeeID, e.EmployeeCode, e.FullName;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
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

        public async Task<DataTable> GetEmployeesForEttendamceAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT 
                                    e.EmployeeCode,
                                    e.FullName,
                                    p.PositionCode,
                                    p.PositionName,
                                    ct.ContractTypeCode,
                                    ct.ContractTypeName
                                FROM Employee e
                                LEFT JOIN Position p 
                                    ON e.PositionID = p.PositionID
                                LEFT JOIN ContractType ct 
                                    ON e.ContractTypeID = ct.ContractTypeID
                                WHERE e.IsActive = 1;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetEmployeesForEttendamceAsync(string contractTypeCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT 
                                    e.EmployeeCode,
                                    e.FullName,
                                    p.PositionCode,
                                    p.PositionName,
                                    ct.ContractTypeCode,
                                    ct.ContractTypeName
                                FROM Employee e
                                LEFT JOIN Position p 
                                    ON e.PositionID = p.PositionID
                                LEFT JOIN ContractType ct 
                                    ON e.ContractTypeID = ct.ContractTypeID
                                WHERE e.IsActive = 1 AND ct.ContractTypeCode = @ContractTypeCode;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ContractTypeCode", contractTypeCode);
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
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT EmployeeCode, WorkDate, WorkingHours, Note, AttendanceLog
                                FROM Attendance
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
            string query = @"INSERT INTO Holiday (HolidayDate, HolidayName) 
                     VALUES (@HolidayDate, @HolidayName)";

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@HolidayDate", holidayDate.Date);
                        cmd.Parameters.AddWithValue("@HolidayName", holidayName);

                        int rows = await cmd.ExecuteNonQueryAsync(); // <-- đúng ở đây
                        return rows > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> deleteHolidayAsync(DateTime Holidate)
        {
            string query = "DELETE FROM Holiday WHERE HolidayDate=@HolidayDate";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@HolidayDate", Holidate.Date);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
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

        public async Task<DataTable> GetOvertimeTypeAsync(bool isActive)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                string query = @"SELECT *
                                FROM OvertimeType
                                WHERE IsActive = @IsActive";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
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

        public async Task<DataTable> GetOvertimeAttendamceAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT OvertimeAttendanceID, EmployeeCode, WorkDate, StartTime, EndTime, OvertimeTypeID, Note, UpdatedHistory
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
                string query = @"SELECT 
                                    E.EmployeeID, 
                                    E.EmployeeCode,
                                    E.FullName,
                                    P.PositionName,                 
                                    ISNULL(ALB.Year, @Year) AS [Year],
                                    ISNULL(ALB.Month, '') AS [Month],
                                    ISNULL(LV.LeaveCount, 0) AS LeaveCount
                                FROM Employee AS E
                                LEFT JOIN Position AS P 
                                    ON E.PositionID = P.PositionID
                                LEFT JOIN ContractType AS CT
                                    ON E.ContractTypeID = CT.ContractTypeID
                                LEFT JOIN AnnualLeaveBalance AS ALB 
                                    ON E.EmployeeCode = ALB.EmployeeCode
                                    AND ALB.Year = @Year
                                LEFT JOIN (
                                    SELECT 
                                        EmployeeCode,
                                        COUNT(DateOff) AS LeaveCount
                                    FROM LeaveAttendance
                                    WHERE YEAR(DateOff) = @Year
                                    GROUP BY EmployeeCode
                                ) AS LV 
                                    ON E.EmployeeCode = LV.EmployeeCode
                                WHERE 
                                    E.IsActive = 1
                                    AND CT.ContractTypeCode = 'c_thuc'
                                ORDER BY 
                                    E.EmployeeCode;";

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

        public async Task<DataTable> GetLeaveAttendanceAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT *
                                FROM LeaveAttendance
                                WHERE MONTH(DateOff) = @Month
                                  AND YEAR(DateOff) = @Year
                                ORDER BY EmployeeCode, DateOff;";

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

        public async Task<DataTable> GetLeaveTypeAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                await con.OpenAsync();

                string query = @"SELECT *
                                FROM LeaveType";

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

        public async Task<int> insertLeaveAttendanceAsync(string employeeCode, string leaveTypeCode, DateTime dateOff, string Note, string UpdatedHistory)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO LeaveAttendance  (
                                EmployeeCode, LeaveTypeCode, DateOff, Note, UpdatedHistory
                            )
                            OUTPUT INSERTED.LeaveID
                            VALUES (
                                @EmployeeCode, @LeaveTypeCode, @DateOff, @Note, @UpdatedHistory
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

        public async Task<bool> updateLeaveAttendanceAsync(int leaveID, string employeeCode, string leaveTypeCode, DateTime dateOff, string Note, string UpdatedHistory)
        {
            string query = @"UPDATE LeaveAttendance SET 
                                EmployeeCode=@EmployeeCode, 
                                LeaveTypeCode=@LeaveTypeCode,
                                DateOff=@DateOff, 
                                Note=@Note,
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
                        cmd.Parameters.AddWithValue("@UpdatedHistory", UpdatedHistory);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }
    }
}
