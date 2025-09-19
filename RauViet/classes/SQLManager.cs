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
        private readonly string conStr = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=RauVietDB;Integrated Security=true;";

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

        public async Task<bool> updateCustomerAsync(int customerID, string name, string address, string phoneNumber, string email)
        {
            string query = "UPDATE Customers SET FullName=@Name, Address=@Address, PhoneNumber=@Phone, Email=@Email WHERE CustomerID=@CustomerID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                        cmd.Parameters.AddWithValue("@Email", email);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> insertCustomerAsync(string name, string address, string phoneNumber, string email)
        {
            string query = "INSERT INTO Customers (FullName, Address, PhoneNumber, Email) VALUES (@Name, @Address, @Phone, @Email)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                        cmd.Parameters.AddWithValue("@Email", email);
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

        public async Task<bool> updateProductSKUAsync(int SKU, string ProductNameVN, string ProductNameEN, string PackingType, string Package, string PackingList, string BotanicalName, decimal PriceCNF)
        {
            string query = @"UPDATE ProductSKU SET 
                                ProductNameVN=@ProductNameVN, 
                                ProductNameEN=@ProductNameEN, 
                                PackingType=@PackingType, 
                                Package=@Package, 
                                PackingList=@PackingList, 
                                BotanicalName=@BotanicalName, 
                                PriceCNF=@PriceCNF
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
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> insertProductSKUAsync(string ProductNameVN, string ProductNameEN, string PackingType, string Package, string PackingList, string BotanicalName, decimal PriceCNF)
        {
            string query = @"INSERT INTO ProductSKU (ProductNameVN, ProductNameEN, PackingType, Package, PackingList, BotanicalName, PriceCNF)
                             VALUES (@ProductNameVN, @ProductNameEN, @PackingType, @Package, @PackingList, @BotanicalName, @PriceCNF)";
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

        public async Task<bool> updateProductpackingAsync(int ID, int SKU, string BarCode, string PLU, string Amount, string packing, string barCodeEAN13, string artNr, string GGN)
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
                        cmd.Parameters.AddWithValue("@Amount", Amount);
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

        public async Task<bool> insertProductpackingAsync(int SKU, string BarCode, string PLU, string Amount, string packing, string barCodeEAN13, string artNr, string GGN)
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
                        cmd.Parameters.AddWithValue("@Amount", Amount);
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
                string query = "SELECT ExportCodeID, ExportCode, ExportDate FROM ExportCodes WHERE Complete = 0";
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateExportCodeAsync(int exportCodeID, string exportCode, DateTime exportDate, bool complete)
        {
            string query = "UPDATE ExportCodes SET ExportCode=@ExportCode, ExportDate=@ExportDate, ModifiedAt=@ModifiedAt, Complete=@Complete WHERE ExportCodeID=@ExportCodeID";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExportCodeID", exportCodeID);
                        cmd.Parameters.AddWithValue("@ExportCode", exportCode);
                        cmd.Parameters.AddWithValue("@ExportDate", exportDate);
                        cmd.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Complete", complete);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> insertExportCodeAsync(string exportCode)
        {
            string query = "INSERT INTO ExportCodes (ExportCode) VALUES (@ExportCode)";
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ExportCode", exportCode);
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


    }
}
