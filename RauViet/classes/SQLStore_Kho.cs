using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataTable = System.Data.DataTable;

namespace RauViet.classes
{
    public sealed class SQLStore_Kho
    {
        private static SQLStore_Kho ins = null;
        private static readonly object padlock = new object();
              
        //suong
        DataTable mProductSKUHistory_dt = null;
        DataTable mProductSKU_dt = null;
        DataTable mProductpacking_dt = null;
        DataTable mProductDomesticPrices_dt = null;
        DataTable mExportCodes_dt = null;
        DataTable mOrderDomesticCode_dt = null;
        DataTable mEmployeesInDongGoi_dt = null;
        DataTable mCusromer_dt = null;
        DataTable mCartonSize_dt = null;
        DataTable mlatestOrders_dt = null;
        DataTable mExportCodeLog_dt = null;
        DataTable mReportCustomerOrderDetail_dt = null;
                
        DataTable mProductType_dt = null;
        DataTable mOrderDomesticCodeLog_dt = null;
        DataTable mProductDomesticPricesLog_dt = null;

        Dictionary<int, DataTable> mOrderLists;
        Dictionary<int, DataTable> mOrderDomesticDetails;
        Dictionary<int, DataTable> mReportExportByYears;
        Dictionary<int, DataTable> mReportCustomerOrderDetailByYears;
        Dictionary<int, DataTable> mOrdersTotals;
        Dictionary<int, DataTable> mLOTCodes;
        Dictionary<int, DataTable> mOrdersDKKDs;
        Dictionary<int, DataTable> mCustomerDetailPackings;
        Dictionary<int, DataTable> mPhytos;
        Dictionary<int, DataTable> mPhytoChots;
        Dictionary<int, DataTable> mOrderInvoice;
        Dictionary<int, DataTable> mOrderCusInvoices;
        Dictionary<int, DataTable> mOrderCartonInvoices;
        Dictionary<int, DataTable> mDetailPackingTotals;
        Dictionary<int, DataTable> mOrderLogs;
        Dictionary<int, DataTable> mOrderPackingLogs;
        Dictionary<int, DataTable> mDo47Logs;
        Dictionary<int, DataTable> mLotCodeLogs;
        Dictionary<int, DataTable> mOrderDomesticDetailLogs;
        Dictionary<int, Dictionary<int, DataTable>> mtOrderDomestics;

        private SQLStore_Kho() { }

        public static SQLStore_Kho Instance
        {
            get
            {
                lock (padlock)
                {
                    if (ins == null)
                        ins = new SQLStore_Kho();
                    return ins;
                }
            }
        }

        public async Task preload()
        {
            try
            {
                mReportExportByYears = new Dictionary<int, DataTable>();
                mReportCustomerOrderDetailByYears = new Dictionary<int, DataTable>();
                mOrderLists = new Dictionary<int, DataTable>();
                mOrderDomesticDetails = new Dictionary<int, DataTable>();
                mOrdersTotals = new Dictionary<int, DataTable>();
                mLOTCodes = new Dictionary<int, DataTable>();
                mOrdersDKKDs = new Dictionary<int, DataTable>();
                mCustomerDetailPackings = new Dictionary<int, DataTable>();
                mPhytos = new Dictionary<int, DataTable>();
                mPhytoChots = new Dictionary<int, DataTable>();
                mOrderInvoice = new Dictionary<int, DataTable>();
                mOrderCusInvoices = new Dictionary<int, DataTable>();
                mOrderCartonInvoices = new Dictionary<int, DataTable>();
                mDetailPackingTotals = new Dictionary<int, DataTable>();
                mOrderLogs = new Dictionary<int, DataTable>();
                mOrderPackingLogs = new Dictionary<int, DataTable>();
                mDo47Logs = new Dictionary<int, DataTable>();
                mLotCodeLogs = new Dictionary<int, DataTable>();
                mOrderDomesticDetailLogs = new Dictionary<int, DataTable>();
                mtOrderDomestics = new Dictionary<int, Dictionary<int, DataTable>>();

                var productSKUTask = SQLManager_Kho.Instance.getProductSKUAsync();
                var productPackingTask = SQLManager_Kho.Instance.getProductpackingAsync();
                var exportCodesTask = SQLManager_Kho.Instance.getExportCodesAsync();
                var orderDomesticCodeTask = SQLManager_Kho.Instance.getOrderDomesticCodeAsync();
                var employeesInDongGoiTask = SQLManager_QLNS.Instance.GetActiveEmployeesIn_DongGoiAsync();
                var customersTask = SQLManager_Kho.Instance.getCustomersAsync();
                var pdpTask = SQLManager_Kho.Instance.getProductDomesticPricesAsync();
                var productTypeTask = SQLManager_Kho.Instance.getProductTypeAsync();
                var cartonSizeTask = SQLManager_Kho.Instance.getCartonSizeAsync();
                await Task.WhenAll(productSKUTask, productPackingTask, exportCodesTask, employeesInDongGoiTask, employeesInDongGoiTask, customersTask, cartonSizeTask, pdpTask, orderDomesticCodeTask, productTypeTask);

                if (employeesInDongGoiTask.Status == TaskStatus.RanToCompletion && employeesInDongGoiTask.Result != null) mEmployeesInDongGoi_dt = employeesInDongGoiTask.Result;
                if (exportCodesTask.Status == TaskStatus.RanToCompletion && exportCodesTask.Result != null) mExportCodes_dt = exportCodesTask.Result;
                if (productSKUTask.Status == TaskStatus.RanToCompletion && productSKUTask.Result != null) mProductSKU_dt = productSKUTask.Result;
                if (productPackingTask.Status == TaskStatus.RanToCompletion && productPackingTask.Result != null) mProductpacking_dt = productPackingTask.Result;
                if (customersTask.Status == TaskStatus.RanToCompletion && customersTask.Result != null) mCusromer_dt = customersTask.Result;                
                if (cartonSizeTask.Status == TaskStatus.RanToCompletion && cartonSizeTask.Result != null) mCartonSize_dt = cartonSizeTask.Result;
                if (pdpTask.Status == TaskStatus.RanToCompletion && pdpTask.Result != null) mProductDomesticPrices_dt = pdpTask.Result;
                if (orderDomesticCodeTask.Status == TaskStatus.RanToCompletion && orderDomesticCodeTask.Result != null) mOrderDomesticCode_dt = orderDomesticCodeTask.Result;
                if (productTypeTask.Status == TaskStatus.RanToCompletion && productTypeTask.Result != null) mProductType_dt = productTypeTask.Result;

                editProductSKUA();
                editExportCodes();
                editProductpacking();
                editProductDomesticPrices();
                editOrderDomesticCode();

                mExportCodes_dt.DefaultView.Sort = "ExportCodeID ASC";
                mExportCodes_dt = mExportCodes_dt.DefaultView.ToTable();
                var exportIds = mExportCodes_dt.AsEnumerable().Where(r => r.Field<bool>("Complete") == false).Select(r => r.Field<int>("ExportCodeID")).Take(6).ToList();
                var tasks = exportIds.Select(async id =>
                {
                    DataTable data = await SQLManager_Kho.Instance.getOrdersAsync(id);
                    return (id, data);
                }).ToList();

                var results = await Task.WhenAll(tasks);

                foreach (var (id, data) in results)
                {
                    await editOrders(data);
                    mOrderLists[id] = data;                    
                }

                mOrderDomesticCode_dt.DefaultView.Sort = "OrderDomesticCodeID ASC";
                mOrderDomesticCode_dt = mOrderDomesticCode_dt.DefaultView.ToTable();
                var OrderDomesticCodeIds = mOrderDomesticCode_dt.AsEnumerable().Where(r => r.Field<bool>("Complete") == false).Select(r => r.Field<int>("OrderDomesticCodeID")).Take(6).ToList();
                var tasks1 = OrderDomesticCodeIds.Select(async id =>
                {
                    DataTable data = await SQLManager_Kho.Instance.getOrderDomesticDetailAsync(id);
                    return (id, data);
                }).ToList();

                var results1 = await Task.WhenAll(tasks1);

                foreach (var (id, data) in results1)
                {
                    mOrderDomesticDetails[id] = data;
                    editOrderDomesticDetail(data);
                }
            }
            catch
            {
                Console.WriteLine("preload in SQLStore errror");
            }
        }

        private void editSalarySummaryByYear(DataTable data)
        {
            data.Columns.Add("MonthYear", typeof(string));
            foreach (DataRow row in data.Rows)
            {
                row["MonthYear"] = row["Month"] + "/" + row["Year"];
            }
        }

        public void removeProductSKU()
        {
            mProductSKU_dt = null;
        }

        public async Task<DataTable> getProductSKUAsync()
        {
            if (mProductSKU_dt == null)
            {
                try
                {
                    mProductSKU_dt = await SQLManager_Kho.Instance.getProductSKUAsync();
                    editProductSKUA();
                }
                catch
                {
                    Console.WriteLine("error getProductSKUAsync SQLStore");
                    return null;
                }
            }

            return mProductSKU_dt;
        }

        public async Task<DataTable> getProductSKUAsync(Dictionary<string, object> parameters)
        {
            var result = mProductSKU_dt.Clone();
            IEnumerable<DataRow> filteredRows = mProductSKU_dt.AsEnumerable();

            foreach (var kv in parameters)
            {
                string columnName = kv.Key;
                object value = kv.Value;

                // Kiểm tra tồn tại cột
                if (mProductSKU_dt.Columns.Contains(columnName))
                {
                    filteredRows = filteredRows.Where(row =>
                    {
                        var cellValue = row[columnName];
                        if (cellValue == DBNull.Value) return false;
                        return cellValue.Equals(value);
                    });
                }
            }

            // Copy dữ liệu đúng
            foreach (var row in filteredRows)
            {
                result.ImportRow(row);   // <--- Quan trọng!
            }

            return result;
        }

        private void editProductSKUA()
        {
            mProductSKU_dt.Columns.Add("ProductNameVN_NoSign", typeof(string));

            foreach (DataRow row in mProductSKU_dt.Rows)
            {
                string name = row["ProductNameVN"]?.ToString() ?? "";
                int SKU = Convert.ToInt32(row["SKU"]);
                row["ProductNameVN_NoSign"] = Utils.RemoveVietnameseSigns(name + " " + SKU).ToLower();
            }
        }

        public void removeProductpacking()
        {
            mProductpacking_dt = null;
        }

        public async Task<DataTable> getProductpackingAsync(bool loadNew = false)
        {
            if (mProductpacking_dt == null || loadNew)
            {
                try
                {
                    mProductpacking_dt = await SQLManager_Kho.Instance.getProductpackingAsync();
                    editProductpacking();
                }
                catch
                {
                    Console.WriteLine("error getProductpackingAsync SQLStore");
                    return null;
                }
            }

            return mProductpacking_dt;
        }

        public void removeProductDomesticPrices() { mProductDomesticPrices_dt = null; }
        public async Task<DataTable> getProductDomesticPricesAsync()
        {
            if (mProductDomesticPrices_dt == null)
            {
                try
                {
                    mProductDomesticPrices_dt = await SQLManager_Kho.Instance.getProductDomesticPricesAsync();
                    editProductDomesticPrices();
                }
                catch
                {
                    Console.WriteLine("error getProductDomesticPricesAsync SQLStore");
                    return null;
                }
            }

            return mProductDomesticPrices_dt;
        }

        private void editProductpacking()
        {
            mProductpacking_dt.Columns.Add(new DataColumn("IsActive_SKU", typeof(bool)));
            mProductpacking_dt.Columns.Add(new DataColumn("PackingName", typeof(string)));
            mProductpacking_dt.Columns.Add("ProductNameVN_NoSign", typeof(string));

            mProductpacking_dt.Columns.Add(new DataColumn("PriceCNF", typeof(string)));
            mProductpacking_dt.Columns.Add(new DataColumn("Name_VN", typeof(string)));
            mProductpacking_dt.Columns.Add(new DataColumn("Name_EN", typeof(string)));
            mProductpacking_dt.Columns.Add(new DataColumn("Priority", typeof(int)));
            mProductpacking_dt.Columns.Add(new DataColumn("Package", typeof(string)));
            mProductpacking_dt.Columns.Add(new DataColumn("GroupProduct", typeof(int)));

            foreach (DataRow dr in mProductpacking_dt.Rows)
            {
                int sku = Convert.ToInt32(dr["SKU"]);
                DataRow proRow = mProductSKU_dt.Select($"SKU = '{sku}'")[0];

                bool isActive_SKU =  Convert.ToBoolean(proRow["IsActive"]);
                string package = proRow["Package"].ToString();
                string nameVN = proRow["ProductNameVN"].ToString();
                string nameEN = proRow["ProductNameEN"].ToString();
                string packingType = proRow["PackingType"].ToString();
                int priority = Convert.ToInt32(proRow["Priority"]);
                int groupProduct = Convert.ToInt32(proRow["GroupProduct"]);

                decimal amount = dr["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Amount"]);
                string packing = dr["Packing"].ToString();
                string resultAmount = amount.ToString("0.##");

                dr["PackingName"] = $"{resultAmount} {packing}";
                dr["ProductNameVN_NoSign"] = Utils.RemoveVietnameseSigns(nameVN + " " + sku).ToLower();
                dr["Priority"] = priority;                
                dr["Amount"] = resultAmount;
                dr["Package"] = package;
                dr["IsActive_SKU"] = isActive_SKU;
                dr["GroupProduct"] = groupProduct;
                dr["PriceCNF"] = proRow["PriceCNF"].ToString();

                if (package.CompareTo("kg") == 0 && packing.CompareTo("") != 0 && amount > 0)
                {

                    dr["Name_VN"] = nameVN + " " + packingType + " " + resultAmount + " " + packing;
                    dr["Name_EN"] = nameEN + " " + packingType + " " + resultAmount + " " + packing;
                }
                else
                {
                    dr["Name_VN"] = nameVN;
                    dr["Name_EN"] = nameEN;
                }
                
            }
        }

        private void editProductDomesticPrices()
        {
            mProductDomesticPrices_dt.Columns.Add(new DataColumn("ProductName_VN", typeof(string)));
            mProductDomesticPrices_dt.Columns.Add(new DataColumn("ProductNameVN_NoSign", typeof(string))); 
            mProductDomesticPrices_dt.Columns.Add(new DataColumn("Package", typeof(string)));
            mProductDomesticPrices_dt.Columns.Add(new DataColumn("Priority", typeof(int)));
            foreach (DataRow dr in mProductDomesticPrices_dt.Rows)
            {
                int sku = Convert.ToInt32(dr["SKU"]);
                DataRow proRow = mProductSKU_dt.Select($"SKU = '{sku}'")[0];

                bool isActive_SKU = Convert.ToBoolean(proRow["IsActive"]);
                string nameVN = proRow["ProductNameVN"].ToString();
                string package = proRow["Package"].ToString();

                dr["ProductName_VN"] = nameVN;
                dr["Priority"] = Convert.ToInt32(proRow["Priority"]);
                dr["ProductNameVN_NoSign"] = Utils.RemoveVietnameseSigns(nameVN + " " + sku).ToLower();
                dr["Package"] = package;
                if (!isActive_SKU)
                    dr["IsActive"] = isActive_SKU;
            }

            int count = 0;
            mProductDomesticPrices_dt.Columns["SKU"].SetOrdinal(count++);
            mProductDomesticPrices_dt.Columns["ProductName_VN"].SetOrdinal(count++);
            mProductDomesticPrices_dt.Columns["RawPrice"].SetOrdinal(count++);
            mProductDomesticPrices_dt.Columns["RefinedPrice"].SetOrdinal(count++);
            mProductDomesticPrices_dt.Columns["PackedPrice"].SetOrdinal(count++);
            mProductDomesticPrices_dt.Columns["IsActive"].SetOrdinal(count++);
        }

        public async Task<DataTable> GetActiveEmployeesIn_DongGoiAsync()
        {
            if (mEmployeesInDongGoi_dt == null)
            {
                try
                {
                    mEmployeesInDongGoi_dt = await SQLManager_QLNS.Instance.GetActiveEmployeesIn_DongGoiAsync();
                }
                catch
                {
                    Console.WriteLine("error getEmployeesInDongGoiAsync SQLStore");
                    return null;
                }
            }

            return mEmployeesInDongGoi_dt;
        }

        public void removeExportCodes()
        {
            mExportCodes_dt = null;
        }

        public async Task<DataTable> getExportCodesAsync()
        {
            if (mExportCodes_dt == null)
            {
                try
                {
                    mExportCodes_dt = await SQLManager_Kho.Instance.getExportCodesAsync();
                    editExportCodes();
                }
                catch
                {
                    Console.WriteLine("error getExportCodesAsync SQLStore");
                    return null;
                }
            }

            return mExportCodes_dt;
        }
        private void editExportCodes()
        {            
            mExportCodes_dt.Columns.Add(new DataColumn("InputByName", typeof(string)));
            mExportCodes_dt.Columns.Add(new DataColumn("InputByName_NoSign", typeof(string)));
            mExportCodes_dt.Columns.Add(new DataColumn("PackingByName", typeof(string)));
            
            foreach (DataRow dr in mExportCodes_dt.Rows)
            {
                int inputBy = Convert.ToInt32(dr["InputBy"]);
                int packingBy = Convert.ToInt32(dr["PackingBy"]);

                DataRow[] inputByRow = mEmployeesInDongGoi_dt.Select($"EmployeeID = {inputBy}");
                DataRow[] packingByRow = mEmployeesInDongGoi_dt.Select($"EmployeeID = {packingBy}");

                if (inputByRow.Length > 0)
                {
                    string employeeName = inputByRow[0]["FullName"].ToString();
                    dr["InputByName"] = employeeName;
                    dr["InputByName_NoSign"] = Utils.RemoveVietnameseSigns(employeeName).Replace(" ", "");

                }

                if (packingByRow.Length > 0)
                    dr["PackingByName"] = packingByRow[0]["FullName"].ToString();
            }
        }


        public async Task<DataTable> getExportCodesAsync(string[] keepColumns, Dictionary<string, object> parameters)
        {
            await getExportCodesAsync();

            // 1️⃣ Clone cấu trúc bảng và chỉ giữ lại các cột cần thiết
            DataTable result = new DataTable();
            foreach (string col in keepColumns)
            {
                if (mExportCodes_dt.Columns.Contains(col))
                    result.Columns.Add(col, mExportCodes_dt.Columns[col].DataType);
            }

            // 2️⃣ Lọc dữ liệu theo điều kiện trong parameters
            IEnumerable<DataRow> filteredRows = mExportCodes_dt.AsEnumerable();

            int top = 0;
            if (parameters.ContainsKey("Top"))
            {
                top = Convert.ToInt32(parameters["Top"]);
                parameters.Remove("Top");   // loại khỏi điều kiện filter
            }

            foreach (var kv in parameters)
            {
                string columnName = kv.Key;
                object value = kv.Value;

                // Kiểm tra tồn tại cột
                if (mExportCodes_dt.Columns.Contains(columnName))
                {
                    filteredRows = filteredRows.Where(row =>
                    {
                        var cellValue = row[columnName];
                        if (cellValue == DBNull.Value) return false;
                        return cellValue.Equals(value);
                    });
                }
            }

            // 3️⃣ Lấy 10 dòng mới nhất theo CreatedAt
            if (top > 0)
            {
                filteredRows = filteredRows.OrderBy(r => r.Field<bool>("Complete"))   // false (0) lên trước true (1)
                                            .ThenByDescending(r => r.Field<int>("ExportCodeID"))
                                            .Take(top);
            }

            // 4️⃣ Thêm các dòng đã lọc vào bảng kết quả (chỉ giữ cột cần)
            foreach (var row in filteredRows)
            {
                DataRow newRow = result.NewRow();
                foreach (string col in keepColumns)
                {
                    if (mExportCodes_dt.Columns.Contains(col))
                        newRow[col] = row[col];
                }
                result.Rows.Add(newRow);
            }

            return result;
        }

        public void removeProductSKUHistory()
        {
            mProductSKUHistory_dt = null;
        }
        public async Task<DataTable> GetProductSKUHistoryAsync()
        {
            if (mProductSKUHistory_dt == null)
            {
                try
                {
                    mProductSKUHistory_dt = await SQLManager_Kho.Instance.GetProductSKUHistoryAsync();
                }
                catch
                {
                    Console.WriteLine("error GetProductSKUHistoryAsync SQLStore");
                    return null;
                }
            }

            return mProductSKUHistory_dt;
        }

        public void removeCustomers()
        {
            mCusromer_dt = null;
        }

        public async Task<DataTable> getCustomersAsync()
        {
            if (mCusromer_dt == null)
            {
                try
                {
                    mCusromer_dt = await SQLManager_Kho.Instance.getCustomersAsync();
                }
                catch
                {
                    Console.WriteLine("error getCustomersAsync SQLStore");
                    return null;
                }
            }

            return mCusromer_dt;
        }

        public void removeOrders(int exportCodeID)
        {
            mOrderLists.Remove(exportCodeID);
        }
        public async Task<DataTable> getOrdersAsync(int exportCodeID, bool isNew = false)
        {
            if (!mOrderLists.ContainsKey(exportCodeID) || isNew)
            {
                try
                {
                    DataTable dt = await SQLManager_Kho.Instance.getOrdersAsync(exportCodeID);
                    await editOrders(dt);
                    mOrderLists[exportCodeID] = dt;                    
                }
                catch
                {
                    Console.WriteLine("error getCustomersAsync SQLStore");
                    return null;
                }
            }

            return mOrderLists[exportCodeID];
        }

        public async Task<DataTable> getOrdersAsync(int exportCodeID, string[] colNames)
        {
            await getOrdersAsync(exportCodeID);

            // Clone chỉ các cột mong muốn
            DataTable cloneTable = mOrderLists[exportCodeID].DefaultView.ToTable(false, colNames.ToArray());

            return cloneTable;
        }


        private async Task editOrders(DataTable data)
        {
            await getProductSKUAsync();
            await getProductpackingAsync();
            data.Columns.Add(new DataColumn("Search_NoSign", typeof(string)));
            data.Columns.Add(new DataColumn("SKU", typeof(int)));
            data.Columns.Add(new DataColumn("GroupProduct", typeof(int)));
            data.Columns.Add(new DataColumn("CustomerName", typeof(string)));
            data.Columns.Add(new DataColumn("CustomerCode", typeof(string)));
            data.Columns.Add(new DataColumn("ProductNameVN", typeof(string)));
            data.Columns.Add(new DataColumn("ProductNameEN", typeof(string)));
            data.Columns.Add(new DataColumn("ExportCode", typeof(string)));
            data.Columns.Add(new DataColumn("Priority", typeof(int)));
            data.Columns.Add(new DataColumn("Package", typeof(string)));
            data.Columns.Add(new DataColumn("packing", typeof(string)));
            //    mOrderList_dt.Columns.Add(new DataColumn("PackingType", typeof(string)));
            data.Columns.Add(new DataColumn("Amount", typeof(int)));
            data.Columns.Add(new DataColumn("ExportDate", typeof(DateTime)));

            foreach (DataRow dr in data.Rows)
            {
                int customerID = Convert.ToInt32(dr["CustomerID"]);
                int productPackingID = Convert.ToInt32(dr["ProductPackingID"]);
                int exportCodeID = Convert.ToInt32(dr["ExportCodeID"]);
                DataRow[] customerRows = mCusromer_dt.Select($"CustomerID = {customerID}");
                DataRow[] packingRows = mProductpacking_dt.Select($"ProductPackingID = {productPackingID}");
                DataRow[] exportCodeRows = mExportCodes_dt.Select($"ExportCodeID = {exportCodeID}");

                string cusName = customerRows.Length > 0 ? customerRows[0]["FullName"].ToString() : "Unknown";
                string proVN = packingRows.Length > 0 ? packingRows[0]["Name_VN"].ToString() : "Unknown"; ;

                dr["Search_NoSign"] = Utils.RemoveVietnameseSigns(cusName + " " + proVN).ToLower();

                dr["CustomerName"] = cusName;
                dr["CustomerCode"] = customerRows.Length > 0 ? customerRows[0]["CustomerCode"].ToString() : "Unknown";
                dr["ProductNameVN"] = proVN;
                dr["ProductNameEN"] = packingRows.Length > 0 ? packingRows[0]["Name_EN"].ToString() : "Unknown";
                dr["Amount"] = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["Amount"]) : 0;
                dr["Priority"] = packingRows.Length > 0 ? packingRows[0]["Priority"] : 100000;  
                dr["ExportCode"] = exportCodeRows.Length > 0 ? exportCodeRows[0]["ExportCode"].ToString() : "Unknown";
                dr["ExportDate"] = exportCodeRows.Length > 0 ? Convert.ToDateTime(exportCodeRows[0]["ExportDate"]) : DateTime.Now;
            //    dr["PackingType"] = packingRows.Length > 0 ? packingRows[0]["PackingType"].ToString() : "";
                dr["packing"] = packingRows.Length > 0 ? packingRows[0]["packing"].ToString() : "";
                dr["SKU"] = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["SKU"]) : 0;
                dr["GroupProduct"] = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["GroupProduct"]) : 0;
                string package = packingRows.Length > 0 ? packingRows[0]["Package"].ToString() : "";
                dr["Package"] = package;
                if (package.CompareTo("weight") == 0)
                {
                    dr["PCSReal"] = 0;
                }
            }

            int count = 0;
            data.Columns["OrderId"].SetOrdinal(count++);
            data.Columns["Customername"].SetOrdinal(count++);
            data.Columns["ProductNameVN"].SetOrdinal(count++);
            data.Columns["CustomerCarton"].SetOrdinal(count++);
            data.Columns["CartonNo"].SetOrdinal(count++);
            data.Columns["CartonSize"].SetOrdinal(count++);
            data.Columns["PCSReal"].SetOrdinal(count++);
            data.Columns["NWReal"].SetOrdinal(count++);
            data.Columns["PCSOther"].SetOrdinal(count++);
            data.Columns["NWOther"].SetOrdinal(count++);
        }

        public void RemoveExportHistoryByYear(int year)
        {
            mReportExportByYears.Remove(year);
        }
        public async Task<DataTable> GetExportHistoryByYear(int year)
        {
            if (!mReportExportByYears.ContainsKey(year))
            {
                try
                {
                    mReportExportByYears[year] = await SQLManager_Kho.Instance.GetExportHistoryAsync(year);
                    EditExportHistory(mReportExportByYears[year]);
                }
                catch
                {
                    Console.WriteLine("error GetReportExportByYear SQLStore");
                    return null;
                }
            }

            return mReportExportByYears[year];
        }

        private void EditExportHistory(DataTable data)
        {
            int count = 0;
            data.Columns["ExportCode"].SetOrdinal(count++);
            data.Columns["ExportDate"].SetOrdinal(count++);
            data.Columns["TotalMoney"].SetOrdinal(count++);
            data.Columns["TotalNW"].SetOrdinal(count++);
            data.Columns["NumberCarton"].SetOrdinal(count++);
            data.Columns["FreightCharge"].SetOrdinal(count++);
        }


        public void RemoveCustomerOrderDetailHistoryByYear(int year)
        {
            mReportCustomerOrderDetailByYears.Remove(year);
        }

        public async Task<DataTable> GetCustomerOrderDetailHistoryByYear(int year)
        {
            if (!mReportCustomerOrderDetailByYears.ContainsKey(year))
            {
                try
                {
                    mReportCustomerOrderDetailByYears[year] = await SQLManager_Kho.Instance.GetCustomerOrderDetailHistory_InYearAsync(year);
                }
                catch
                {
                    Console.WriteLine("error GetCustomerOrderDetailHistoryByYear SQLStore");
                    return null;
                }
            }

            return mReportCustomerOrderDetailByYears[year];
        }

        public async Task<bool> ExportHistoryIsAddedExportCode(string  exportCode, int year)
        {
            DataTable dt = await GetExportHistoryByYear(year);
            if(dt != null)
            {
                DataRow[] foundRows = dt.Select($"ExportCode = '{exportCode}'");

                return foundRows.Length > 0;
            }

            return false;
        }

        public async Task<DataTable> GetCartonSize()
        {
            if (mCartonSize_dt == null)
            {
                try
                {
                    mCartonSize_dt = await SQLManager_Kho.Instance.getCartonSizeAsync();
                }
                catch
                {
                    Console.WriteLine("error GetCartonSize SQLStore");
                    return null;
                }
            }

            return mCartonSize_dt;
        }

        public void removeLatestOrdersAsync()
        {
            mlatestOrders_dt = null;
        }
        public async Task<DataTable> get3LatestOrdersAsync()
        {
            if (mlatestOrders_dt == null)
            {
                try
                {
                    mlatestOrders_dt = await SQLManager_Kho.Instance.get3LatestOrdersAsync();
                    edit3LatestOrders();
                }
                catch
                {
                    Console.WriteLine("error get3GetLatestOrdersAsync SQLStore");
                    return null;
                }                
            }

            return mlatestOrders_dt;
        }

        private void edit3LatestOrders()
        {
            mlatestOrders_dt.Columns.Add(new DataColumn("ProductNameVN", typeof(string)));
            mlatestOrders_dt.Columns.Add(new DataColumn("OrderPackingPriceCNF", typeof(string)));
            mlatestOrders_dt.Columns.Add(new DataColumn("PCSOther", typeof(int)));
            mlatestOrders_dt.Columns.Add(new DataColumn("NWOther", typeof(decimal)));
            mlatestOrders_dt.Columns.Add(new DataColumn("Package", typeof(string)));
            mlatestOrders_dt.Columns.Add(new DataColumn("packing", typeof(string)));
            mlatestOrders_dt.Columns.Add(new DataColumn("Amount", typeof(decimal)));

            foreach (DataRow dr in mlatestOrders_dt.Rows)
            {
                int productPackingID = Convert.ToInt32(dr["ProductPackingID"]);
                DataRow[] packingRows = mProductpacking_dt.Select($"ProductPackingID = {productPackingID}");

                string proVN = packingRows.Length > 0 ? packingRows[0]["Name_VN"].ToString() : "Unknown"; ;

                dr["ProductNameVN"] = proVN;
                string package = packingRows.Length > 0 ? packingRows[0]["Package"].ToString() : "";
                dr["Package"] = package;
                dr["OrderPackingPriceCNF"] = packingRows.Length > 0 ? Convert.ToDecimal(packingRows[0]["PriceCNF"]) : 0;
                dr["packing"] = packingRows.Length > 0 ? packingRows[0]["packing"].ToString() : "";
                dr["Amount"] = packingRows.Length > 0 ? Convert.ToDecimal(packingRows[0]["Amount"]) : 0;
                if (package.CompareTo("weight") == 0)
                {
                    dr["PCSOther"] = 0;
                }
            }
        }

        public void removeOrdersTotal(int exportCodeID)
        {
            mOrdersTotals.Remove(exportCodeID);
        }
        public async Task<DataTable> getOrdersTotalAsync(int exportCodeID)
        {
            if (!mOrdersTotals.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.getOrdersTotalAsync(exportCodeID);
                editOrdersTotal(dt);
                mOrdersTotals[exportCodeID] = dt;
            }
            return mOrdersTotals[exportCodeID];
        }
        private void editOrdersTotal(DataTable dt)
        {
            dt.Columns.Add(new DataColumn("STT", typeof(string)));
            dt.Columns.Add(new DataColumn("NWRegistration", typeof(string)));
            dt.Columns.Add(new DataColumn("NWDifference", typeof(decimal)));

            int count = 1;
            dt.Columns["NetWeightFinal"].ReadOnly = false;
            foreach (DataRow dr in dt.Rows)
            {
                int SKU = Convert.ToInt32(dr["SKU"]);
                string productName = dr["ProductNameVN"].ToString();
                string packing = dr["packing"].ToString();
                string package = dr["Package"].ToString();

                decimal amount = dr["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Amount"]);

                if (package.CompareTo("kg") == 0 && packing.CompareTo("") != 0 && amount > 0)
                {
                    string amountStr = amount.ToString("0.##");
                    dr["ProductNameVN"] = $"{productName} {amountStr} {packing}";
                }
                else
                {
                    dr["ProductNameVN"] = $"{productName}";
                }

                decimal nwReal, nwFinal, nwOrder;

                // thử ép kiểu, nếu không thành công thì bỏ qua
                bool isNWRealValid = decimal.TryParse(dr["TotalNWReal"]?.ToString(), out nwReal);
                bool isNWFinalValid = decimal.TryParse(dr["NetWeightFinal"]?.ToString(), out nwFinal);
                bool isNWOrderValid = decimal.TryParse(dr["TotalNWOther"]?.ToString(), out nwOrder);


                if (isNWRealValid && isNWFinalValid) dr["NWDifference"] = nwReal - nwFinal;
                else dr["NWDifference"] = DBNull.Value;

                if (isNWOrderValid) dr["NWRegistration"] = nwOrder * Convert.ToDecimal(1.1);
                else dr["NWDifference"] = DBNull.Value;

                dr["STT"] = count++;

            }

            dt.Columns["STT"].SetOrdinal(0);
            dt.Columns["ProductNameVN"].SetOrdinal(1);
            dt.Columns["NWRegistration"].SetOrdinal(2);
            dt.Columns["TotalNWOther"].SetOrdinal(3);
            dt.Columns["NetWeightFinal"].SetOrdinal(4);
            dt.Columns["TotalNWReal"].SetOrdinal(5);
            dt.Columns["NWDifference"].SetOrdinal(6);
        }

        public void removeLOTCode(int exportCodeID)
        {
            mLOTCodes.Remove(exportCodeID);
        }
        public async Task<DataTable> GetLOTCodeAsync(int exportCodeID)
        {
            if (!mLOTCodes.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.GetLOTCodeByExportCodeAsync(exportCodeID);
                editLOTCode(dt);
                mLOTCodes[exportCodeID] = dt;
            }
            return mLOTCodes[exportCodeID];
        }

        private void editLOTCode(DataTable dt)
        {            
            dt.Columns["ProductNameVN"].SetOrdinal(0);
            dt.Columns["LOTCodeHeader"].SetOrdinal(1);
            dt.Columns["LotCode"].SetOrdinal(2);
            dt.Columns["LOTCodeComplete"].SetOrdinal(3);
        }

        public void removeOrdersDKKD(int exportCodeID)
        {
            mOrdersDKKDs.Remove(exportCodeID);
        }
        public async Task<DataTable> getOrdersDKKDAsync(int exportCodeID)
        {
            if (!mOrdersDKKDs.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.getOrdersDKKDAsync(exportCodeID);
                editOrdersDKKDA(dt);
                mOrdersDKKDs[exportCodeID] = dt;
            }
            return mOrdersDKKDs[exportCodeID];
        }

        private void editOrdersDKKDA(DataTable dt)
        {
            dt.Columns.Add(new DataColumn("No", typeof(int)));

            int count = 1;
            foreach (DataRow dr in dt.Rows)
            {
                dr["No"] = count++;

                string productNameVN = dr["ProductNameVN"].ToString();
                // Xóa nội dung trong ngoặc đơn và khoảng trắng dư
                productNameVN = Regex.Replace(productNameVN, @"\s*\([^)]*\)", "").Trim();
                dr["ProductNameVN"] = productNameVN;

                string productNameEN = dr["ProductNameEN"].ToString();
                // Xóa nội dung trong ngoặc đơn và khoảng trắng dư
                productNameEN = Regex.Replace(productNameEN, @"\s*\([^)]*\)", "").Trim();
                dr["ProductNameEN"] = productNameEN;
            }

            dt.Columns.Add(new DataColumn("Packing", typeof(string)));
            dt.Columns.Add(new DataColumn("PCS", typeof(string)));
            dt.Columns.Add(new DataColumn("PriceCHF", typeof(string)));
            dt.Columns.Add(new DataColumn("AmountCHF", typeof(string)));

            count = 0;
            dt.Columns["No"].SetOrdinal(count++);
            dt.Columns["ProductNameEN"].SetOrdinal(count++);
            dt.Columns["ProductNameVN"].SetOrdinal(count++);
            dt.Columns["BotanicalName"].SetOrdinal(count++);
            dt.Columns["NWOther"].SetOrdinal(count++);
            dt.Columns["Packing"].SetOrdinal(count++);
            dt.Columns["PCS"].SetOrdinal(count++);
            dt.Columns["PriceCHF"].SetOrdinal(count++);
            dt.Columns["AmountCHF"].SetOrdinal(count++);
        }

        public void removeCustomerDetailPacking(int exportCodeID)
        {
            mCustomerDetailPackings.Remove(exportCodeID);
        }
        public async Task<DataTable> GetCustomerDetailPackingAsync(int exportCodeID)
        {
            if (!mCustomerDetailPackings.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.GetCustomerDetailPackingAsync(exportCodeID);
                editCustomerDetailPacking(dt);
                mCustomerDetailPackings[exportCodeID] = dt;
            }
            return mCustomerDetailPackings[exportCodeID];
        }

        private void editCustomerDetailPacking(DataTable data)
        {
            data.Columns.Add(new DataColumn("No", typeof(string)));
            data.Columns.Add(new DataColumn("AmountPacking", typeof(string)));
            data.Columns["PCSReal"].ReadOnly = false;
            data.Columns["CartonNo"].ReadOnly = false;
            int count = 1;
            foreach (DataRow dr in data.Rows)
            {
                dr["No"] = count++;

                decimal amount = dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0;
                string package = dr["Package"].ToString();
                string packing = dr["packing"].ToString();
                string productNameVN = dr["ProductNameVN"].ToString();
                string productNameEN = dr["ProductNameEN"].ToString();
                string cartonNo = dr["CartonNo"].ToString();

                // 1️⃣ Tách chuỗi → List<int> (an toàn, bỏ ký tự lỗi)
                List<int> cartonNoList = (cartonNo ?? "")
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s =>
                    {
                        int n;
                        return int.TryParse(s.Trim(), out n) ? n : (int?)null;
                    })
                    .Where(n => n.HasValue)
                    .Select(n => n.Value)
                    .ToList();

                cartonNoList.Sort();
                dr["CartonNo"] = string.Join(", ", cartonNoList);

                if (package.CompareTo("kg") == 0 && packing.CompareTo("") != 0 && amount > 0)
                {
                    string amountStr = amount.ToString("0.##");
                    dr["ProductNameVN"] = $"{productNameVN} {amountStr} {packing}";
                    dr["ProductNameEN"] = $"{productNameEN} {amountStr} {packing}";
                }

                if (package.CompareTo("weight") == 0)
                {
                    dr["AmountPacking"] = packing;
                    dr["PCSReal"] = Convert.ToInt32(0);

                }
                else if (packing.CompareTo("") != 0 && amount > 0)
                {
                    string amountStr = amount.ToString("0.##");
                    dr["AmountPacking"] = $"{amountStr} {packing}";
                }
            }

            count = 0;
            data.Columns["No"].SetOrdinal(count++);
            data.Columns["FullName"].SetOrdinal(count++);
            data.Columns["CartonNo"].SetOrdinal(count++);
            data.Columns["ProductNameEN"].SetOrdinal(count++);
            data.Columns["ProductNameVN"].SetOrdinal(count++);
            data.Columns["PLU"].SetOrdinal(count++);
            data.Columns["Package"].SetOrdinal(count++);
            data.Columns["NWReal"].SetOrdinal(count++);
            data.Columns["AmountPacking"].SetOrdinal(count++);
            data.Columns["PCSReal"].SetOrdinal(count++);
            data.Columns["CustomerCarton"].SetOrdinal(count++);
        }

        public void removeOrdersPhyto(int exportCodeID)
        {
            mPhytos.Remove(exportCodeID);
        }
        public async Task<DataTable> getOrdersPhytoAsync(int exportCodeID)
        {
            if (!mPhytos.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.getOrdersPhytoAsync(exportCodeID);
                editOrdersPhyto(dt);
                mPhytos[exportCodeID] = dt;
            }
            return mPhytos[exportCodeID];
        }

        private void editOrdersPhyto(DataTable data)
        {
            data.Columns.Add(new DataColumn("No", typeof(string)));

            data.Columns["ProductNameVN"].ReadOnly = false; ;
            data.Columns["ProductNameEN"].ReadOnly = false; ;
            int count = 1;
            foreach (DataRow dr in data.Rows)
            {
                dr["No"] = count++;

                string productNameVN = dr["ProductNameVN"].ToString();
                // Xóa nội dung trong ngoặc đơn và khoảng trắng dư
                productNameVN = Regex.Replace(productNameVN, @"\s*\([^)]*\)", "").Trim();
                dr["ProductNameVN"] = productNameVN;

                string productNameEN = dr["ProductNameEN"].ToString();
                // Xóa nội dung trong ngoặc đơn và khoảng trắng dư
                productNameEN = Regex.Replace(productNameEN, @"\s*\([^)]*\)", "").Trim();
                dr["ProductNameEN"] = productNameEN;
            }

            count = 0;
            data.Columns["No"].SetOrdinal(count++);
            data.Columns["BotanicalName"].SetOrdinal(count++);
            data.Columns["ProductNameEN"].SetOrdinal(count++);
            data.Columns["ProductNameVN"].SetOrdinal(count++);
            data.Columns["NetWeightFinal"].SetOrdinal(count++);
        }

        public void removeOrdersChotPhyto(int exportCodeID)
        {
            mPhytoChots.Remove(exportCodeID);
        }
        public async Task<DataTable> getOrdersChotPhytosync(int exportCodeID)
        {
            if (!mPhytoChots.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.getOrdersChotPhytosync(exportCodeID);
                editOrdersChotPhyto(dt);
                mPhytoChots[exportCodeID] = dt;
            }
            return mPhytoChots[exportCodeID];
        }

        private void editOrdersChotPhyto(DataTable data)
        {
            data.Columns.Add(new DataColumn("No", typeof(string)));

            data.Columns["ProductNameVN"].ReadOnly = false;
            data.Columns["ProductNameEN"].ReadOnly = false;
            int count = 1;
            foreach (DataRow dr in data.Rows)
            {
                dr["No"] = count++;

                string productNameVN = dr["ProductNameVN"].ToString();
                // Xóa nội dung trong ngoặc đơn và khoảng trắng dư
                productNameVN = Regex.Replace(productNameVN, @"\s*\([^)]*\)", "").Trim();
                dr["ProductNameVN"] = productNameVN;

                string productNameEN = dr["ProductNameEN"].ToString();
                // Xóa nội dung trong ngoặc đơn và khoảng trắng dư
                productNameEN = Regex.Replace(productNameEN, @"\s*\([^)]*\)", "").Trim();
                dr["ProductNameEN"] = productNameEN;
            }

            data.Columns.Add(new DataColumn("Packing", typeof(string)));
            data.Columns.Add(new DataColumn("PCS", typeof(string)));
            data.Columns.Add(new DataColumn("PriceCHF", typeof(string)));
            data.Columns.Add(new DataColumn("AmountCHF", typeof(string)));

            count = 0;
            data.Columns["No"].SetOrdinal(count++);
            data.Columns["ProductNameEN"].SetOrdinal(count++);
            data.Columns["ProductNameVN"].SetOrdinal(count++);
            data.Columns["BotanicalName"].SetOrdinal(count++);
            data.Columns["TotalNetWeight"].SetOrdinal(count++);
            data.Columns["Packing"].SetOrdinal(count++);
            data.Columns["PCS"].SetOrdinal(count++);
            data.Columns["PriceCHF"].SetOrdinal(count++);
            data.Columns["AmountCHF"].SetOrdinal(count++);
        }

        public void removeOrdersInvoice(int exportCodeID)
        {
            mOrderInvoice.Remove(exportCodeID);
        }
        public async Task<DataTable> getOrdersInvoiceAsync(int exportCodeID)
        {
            if (!mOrderInvoice.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.getOrdersINVOICEAsync(exportCodeID);
                editOrdersInvoice(dt);
                mOrderInvoice[exportCodeID] = dt;
            }
            return mOrderInvoice[exportCodeID];
        }

        private void editOrdersInvoice(DataTable data)
        {
            data.Columns.Add(new DataColumn("No", typeof(int)));
            data.Columns.Add(new DataColumn("Quantity", typeof(decimal)));
            data.Columns.Add(new DataColumn("AmountCHF", typeof(decimal)));

            Dictionary<int, int> countDic = new Dictionary<int, int>();

            foreach (DataRow dr in data.Rows)
            {
                int exportCodeID = Convert.ToInt32(dr["ExportCodeID"]);
                if (!countDic.ContainsKey(exportCodeID))
                {
                    countDic.Add(exportCodeID, 1);
                }

                decimal NWReal = dr["NWReal"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["NWReal"]);
                int PCS = dr["PCSReal"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PCSReal"]);
                string Package = dr["Package"].ToString();
                decimal quantity = Utils.calQuanity(PCS, NWReal, Package);
                decimal price = dr["OrderPackingPriceCNF"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["OrderPackingPriceCNF"]);

                dr["No"] = countDic[exportCodeID]++;
                dr["Quantity"] = quantity;
                dr["AmountCHF"] = quantity * price;

                dr.Table.Columns["Packing"].ReadOnly = false; // mở khóa tạm
                dr.Table.Columns["PCSReal"].ReadOnly = false; // mở khóa tạm

                var cellValue = dr["Packing"]?.ToString();

                dr["PCSReal"] = Convert.ToInt32(PCS);

                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    // Tách số và phần đơn vị
                    string numberPart = "";
                    string unitPart = "";

                    int firstLetterIndex = -1;
                    for (int i = 0; i < cellValue.Length; i++)
                    {
                        if (char.IsLetter(cellValue[i]))
                        {
                            firstLetterIndex = i;
                            break;
                        }
                    }

                    if (firstLetterIndex >= 0)
                    {
                        numberPart = cellValue.Substring(0, firstLetterIndex).Trim();
                        unitPart = cellValue.Substring(firstLetterIndex).Trim();
                    }
                    else
                    {
                        numberPart = cellValue.Trim();
                        unitPart = "";
                    }

                    // Chuyển số sang decimal
                    if (decimal.TryParse(numberPart, out decimal value))
                    {
                        if (value == 0)
                        {
                            dr["Packing"] = unitPart.CompareTo("weight") == 0 ? "weight" : "";
                        }
                        else
                        {
                            // Loại bỏ .00 nếu là số nguyên
                            string newNumber = value % 1 == 0 ? ((int)value).ToString() : value.ToString();

                            dr["Packing"] = string.IsNullOrEmpty(unitPart) ? newNumber : $"{newNumber} {unitPart}";                    // gán giá trị mới
                            // trả về trạng thái cũ

                        }
                    }
                    else
                    {
                        dr["Packing"] = ""; // Nếu không parse được, để trống
                    }
                }
                else
                {
                    dr["Packing"] = ""; // Nếu cell null hoặc rỗng
                }
                dr.Table.Columns["Packing"].ReadOnly = true;
                dr.Table.Columns["PCSReal"].ReadOnly = true;
            }


            int count = 0;
            data.Columns["No"].SetOrdinal(count++);
            data.Columns["PLU"].SetOrdinal(count++);
            data.Columns["ProductNameEN"].SetOrdinal(count++);
            data.Columns["ProductNameVN"].SetOrdinal(count++);
            data.Columns["Package"].SetOrdinal(count++);
            data.Columns["Quantity"].SetOrdinal(count++);
            data.Columns["NWReal"].SetOrdinal(count++);
            data.Columns["Packing"].SetOrdinal(count++);
            data.Columns["PCSReal"].SetOrdinal(count++);
            data.Columns["OrderPackingPriceCNF"].SetOrdinal(count++);
            data.Columns["AmountCHF"].SetOrdinal(count++);
        }

        public void removeOrdersCusInvoice(int exportCodeID)
        {
            mOrderCusInvoices.Remove(exportCodeID);
        }
        public async Task<DataTable> getOrdersCusInvoiceAsync(int exportCodeID)
        {
            if (!mOrderCusInvoices.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.GetCustomersOrdersAsync(exportCodeID);
                editOrdersCusInvoice(dt);
                mOrderCusInvoices[exportCodeID] = dt;
            }
            return mOrderCusInvoices[exportCodeID];
        }

        private void editOrdersCusInvoice(DataTable data)
        {
            data.Columns.Add(new DataColumn("No", typeof(int)));

            int count = 0;
            data.Columns["No"].SetOrdinal(count++);
            data.Columns["FullName"].SetOrdinal(count++);
            data.Columns["AmountCHF"].SetOrdinal(count++);
            data.Columns["NWReal"].SetOrdinal(count++);
            data.Columns["CNTS"].SetOrdinal(count++);

            Dictionary<int, int> countDic = new Dictionary<int, int>();

            foreach (DataRow dr in data.Rows)
            {
                int exportCodeID = Convert.ToInt32(dr["ExportCodeID"]);
                if (!countDic.ContainsKey(exportCodeID))
                {
                    countDic.Add(exportCodeID, 1);
                }
                dr["No"] = countDic[exportCodeID]++;
            }
        }

        public void removeOrdersCartonInvoice(int exportCodeID)
        {
            mOrderCartonInvoices.Remove(exportCodeID);
        }
        public async Task<DataTable> getOrdersCartonInvoiceAsync(int exportCodeID)
        {
            if (!mOrderCartonInvoices.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.GetExportCartonCountsAsync(exportCodeID);
                editOrdersCartonInvoice(dt);
                mOrderCartonInvoices[exportCodeID] = dt;
            }
            return mOrderCartonInvoices[exportCodeID];
        }

        private void editOrdersCartonInvoice(DataTable data)
        {
            data.Columns.Add(new DataColumn("No", typeof(int)));
            data.Columns.Add(new DataColumn("Weight", typeof(decimal)));

            int count = 0;
            data.Columns["No"].SetOrdinal(count++);
            data.Columns["CartonSize"].SetOrdinal(count++);
            data.Columns["CountCarton"].SetOrdinal(count++);

            Dictionary<int, int> countDic = new Dictionary<int, int>();
            foreach (DataRow dr in data.Rows)
            {
                int exportCodeID = Convert.ToInt32(dr["ExportCodeID"]);
                if (!countDic.ContainsKey(exportCodeID))
                {
                    countDic.Add(exportCodeID, 1);
                }
                dr["No"] = countDic[exportCodeID]++;
                decimal countCarton = Convert.ToDecimal(dr["CountCarton"]);

                string cartonSizeStr = dr["CartonSize"].ToString().Replace(" ", "");
                if (cartonSizeStr.CompareTo("") != 0)
                {
                    string[] parts = cartonSizeStr.Split('x');
                    decimal result = parts.Select(p => int.Parse(p)).Aggregate(1, (a, b) => a * b);
                    result *= countCarton;
                    result /= Convert.ToDecimal(6000);

                    dr["Weight"] = result;
                }
            }
        }

        public void removeDetailPackingTotal(int exportCodeID)
        {
            mDetailPackingTotals.Remove(exportCodeID);
        }
        public async Task<DataTable> GetDetailPackingTotalAsync(int exportCodeID)
        {
            if (!mDetailPackingTotals.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.GetDetailPackingTotalAsync(exportCodeID);
                editDetailPackingTotal(dt);
                mDetailPackingTotals[exportCodeID] = dt;
            }
            return mDetailPackingTotals[exportCodeID];
        }

        private void editDetailPackingTotal(DataTable data)
        {
            data.Columns.Add(new DataColumn("No", typeof(string)));

            int count = 1;
            foreach (DataRow dr in data.Rows)
            {
                dr["No"] = count++;

                var cellValue = dr["Packing"]?.ToString();
                var PCS = dr["PCSReal"]?.ToString();

                dr.Table.Columns["Packing"].ReadOnly = false; // mở khóa tạm
                dr.Table.Columns["PCSReal"].ReadOnly = false;
                dr.Table.Columns["CartonNo"].ReadOnly = false;

                string cartonNo = dr["CartonNo"].ToString();

                // 1️⃣ Tách chuỗi → List<int> (an toàn, bỏ ký tự lỗi)
                List<int> cartonNoList = (cartonNo ?? "")
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s =>
                    {
                        int n;
                        return int.TryParse(s.Trim(), out n) ? n : (int?)null;
                    })
                    .Where(n => n.HasValue)
                    .Select(n => n.Value)
                    .ToList();

                cartonNoList.Sort();
                dr["CartonNo"] = string.Join(", ", cartonNoList);

                if (string.IsNullOrWhiteSpace(PCS))
                {
                    dr["PCSReal"] = Convert.ToInt32(0);
                }

                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    // Tách số và phần đơn vị
                    string numberPart = "";
                    string unitPart = "";

                    int firstLetterIndex = -1;
                    for (int i = 0; i < cellValue.Length; i++)
                    {
                        if (char.IsLetter(cellValue[i]))
                        {
                            firstLetterIndex = i;
                            break;
                        }
                    }

                    if (firstLetterIndex >= 0)
                    {
                        numberPart = cellValue.Substring(0, firstLetterIndex).Trim();
                        unitPart = cellValue.Substring(firstLetterIndex).Trim();
                    }
                    else
                    {
                        numberPart = cellValue.Trim();
                        unitPart = "";
                    }

                    // Chuyển số sang decimal
                    if (decimal.TryParse(numberPart, out decimal value))
                    {
                        if (value == 0)
                        {
                            dr["Packing"] = unitPart.CompareTo("weight") == 0 ? "weight" : "";
                        }
                        else
                        {
                            string newNumber = value.ToString("0.##");
                            dr["Packing"] = string.IsNullOrEmpty(unitPart) ? newNumber : $"{newNumber} {unitPart}";                    // gán giá trị mới
                                                                                                                                       // trả về trạng thái cũ

                        }
                    }
                    else
                    {
                        dr["Packing"] = ""; // Nếu không parse được, để trống
                    }
                }
                else
                {
                    dr["Packing"] = ""; // Nếu cell null hoặc rỗng
                }
                dr.Table.Columns["Packing"].ReadOnly = true;
                dr.Table.Columns["PCSReal"].ReadOnly = true;
            }

            count = 0;
            data.Columns["No"].SetOrdinal(count++);
            data.Columns["CartonNo"].SetOrdinal(count++);
            data.Columns["ProductNameEN"].SetOrdinal(count++);
            data.Columns["ProductNameVN"].SetOrdinal(count++);
            data.Columns["LOTCodeComplete"].SetOrdinal(count++);
            data.Columns["PLU"].SetOrdinal(count++);
            data.Columns["Package"].SetOrdinal(count++);
            data.Columns["NWReal"].SetOrdinal(count++);
            data.Columns["packing"].SetOrdinal(count++);
            data.Columns["PCSReal"].SetOrdinal(count++);
            data.Columns["CustomerCarton"].SetOrdinal(count++);
        }

        public async Task<DataTable> GetExportCodeLogAsync()
        {
            if (mExportCodeLog_dt == null)
            {
                mExportCodeLog_dt = await SQLManager_Kho.Instance.GetExportCodeLogAsync();
            }
            return mExportCodeLog_dt;
        }

        public async Task<DataTable> GetOrderLogAsync(int exportCodeID)
        {
            if (!mOrderLogs.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.GetOrderLogAsync(exportCodeID);
                mOrderLogs[exportCodeID] = dt;
            }
            return mOrderLogs[exportCodeID];
        }

        public async Task<DataTable> GetOrderPackingLogAsync(int exportCodeID)
        {
            if (!mOrderPackingLogs.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.GetOrderPackingLogAsync(exportCodeID);
                mOrderPackingLogs[exportCodeID] = dt;
            }
            return mOrderPackingLogs[exportCodeID];
        }

        public async Task<DataTable> GetDo47LogAsync(int exportCodeID)
        {
            if (!mDo47Logs.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.GetDo47LogAsync(exportCodeID);
                mDo47Logs[exportCodeID] = dt;
            }
            return mDo47Logs[exportCodeID];
        }

        public async Task<DataTable> GetLotCodeLogAsync(int exportCodeID)
        {
            if (!mLotCodeLogs.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager_Kho.Instance.GetLotCodeLogAsync(exportCodeID);
                mLotCodeLogs[exportCodeID] = dt;
            }
            return mLotCodeLogs[exportCodeID];
        }

        public void RemoveCustomerOrderDetailHistory()
        {
            mReportCustomerOrderDetail_dt = null;
        }

        public async Task<DataTable> GetCustomerOrderDetailHistory()
        {
            if (mReportCustomerOrderDetail_dt == null)
            {
                try
                {
                    mReportCustomerOrderDetail_dt = await SQLManager_Kho.Instance.GetCustomerOrderDetailHistoryAsync();
                }
                catch
                {
                    Console.WriteLine("error GetCustomerOrderDetailHistory SQLStore");
                    return null;
                }
            }

            return mReportCustomerOrderDetail_dt;
        }

        public async Task<DataTable> getOrderDomesticCodeAsync()
        {
            if (mOrderDomesticCode_dt == null)
            {
                try
                {
                    mOrderDomesticCode_dt = await SQLManager_Kho.Instance.getExportCodesAsync();
                    editOrderDomesticCode();
                }
                catch
                {
                    Console.WriteLine("error getExportCodesAsync SQLStore");
                    return null;
                }
            }

            return mOrderDomesticCode_dt;
        }

        public async Task<DataTable> getOrderDomesticCodeAsync(Dictionary<string, object> parameters)
        {
            await getOrderDomesticCodeAsync();

            // 1️⃣ Clone cấu trúc bảng và chỉ giữ lại các cột cần thiết
            DataTable result = mOrderDomesticCode_dt.Clone();

            // 2️⃣ Lọc dữ liệu theo điều kiện trong parameters
            IEnumerable<DataRow> filteredRows = mOrderDomesticCode_dt.AsEnumerable();

            int top = 0;
            if (parameters.ContainsKey("Top"))
            {
                top = Convert.ToInt32(parameters["Top"]);
                parameters.Remove("Top");   // loại khỏi điều kiện filter
            }

            foreach (var kv in parameters)
            {
                string columnName = kv.Key;
                object value = kv.Value;

                // Kiểm tra tồn tại cột
                if (mOrderDomesticCode_dt.Columns.Contains(columnName))
                {
                    filteredRows = filteredRows.Where(row =>
                    {
                        var cellValue = row[columnName];
                        if (cellValue == DBNull.Value) return false;
                        return cellValue.Equals(value);
                    });
                }
            }

            // 3️⃣ Lấy 10 dòng mới nhất theo CreatedAt
            if (top > 0)
            {
                filteredRows = filteredRows
                    .OrderByDescending(r => r.Field<int>("OrderDomesticCodeID"))
                    .Take(top);
            }

            // 4️⃣ Thêm các dòng đã lọc vào bảng kết quả (chỉ giữ cột cần)
            foreach (var row in filteredRows)
            {
                result.ImportRow(row);  // copy toàn bộ các cột
            }

            return result;
        }

        private void editOrderDomesticCode()
        {
            mOrderDomesticCode_dt.Columns.Add(new DataColumn("InputByName", typeof(string)));
            mOrderDomesticCode_dt.Columns.Add(new DataColumn("InputByName_NoSign", typeof(string)));
            mOrderDomesticCode_dt.Columns.Add(new DataColumn("PackingByName", typeof(string)));
            mOrderDomesticCode_dt.Columns.Add(new DataColumn("CustomerName", typeof(string)));
            mOrderDomesticCode_dt.Columns.Add(new DataColumn("CustomerCode", typeof(string)));
            mOrderDomesticCode_dt.Columns.Add(new DataColumn("Company", typeof(string)));
            mOrderDomesticCode_dt.Columns.Add(new DataColumn("Address", typeof(string)));
            foreach (DataRow dr in mOrderDomesticCode_dt.Rows)
            {
                int inputBy = Convert.ToInt32(dr["InputBy"]);
                int packingBy = Convert.ToInt32(dr["PackingBy"]);
                int customerID = Convert.ToInt32(dr["CustomerID"]);

                DataRow[] inputByRow = mEmployeesInDongGoi_dt.Select($"EmployeeID = {inputBy}");
                DataRow[] packingByRow = mEmployeesInDongGoi_dt.Select($"EmployeeID = {packingBy}");
                DataRow[] customerRow = mCusromer_dt.Select($"CustomerID = {customerID}");

                if (inputByRow.Length > 0)
                {
                    string employeeName = inputByRow[0]["FullName"].ToString();
                    dr["InputByName"] = employeeName;
                    dr["InputByName_NoSign"] = Utils.RemoveVietnameseSigns(employeeName).Replace(" ", "");

                }

                if (packingByRow.Length > 0)
                    dr["PackingByName"] = packingByRow[0]["FullName"].ToString();
                if (customerRow.Length > 0)
                {
                    dr["CustomerName"] = customerRow[0]["FullName"].ToString();
                    dr["CustomerCode"] = customerRow[0]["CustomerCode"].ToString();
                    dr["Company"] = customerRow[0]["Company"].ToString();
                    dr["Address"] = customerRow[0]["Address"].ToString();
                }
            }
        }

        public async void removeOrderDomesticDetail(int OrderDomesticCodeID)
        {
            mOrderDomesticDetails.Remove(OrderDomesticCodeID);
        }

        public async Task<DataTable> getOrderDomesticDetailAsync(int OrderDomesticCodeID)
        {
            if (!mOrderDomesticDetails.ContainsKey(OrderDomesticCodeID))
            {
                try
                {
                    DataTable dt = await SQLManager_Kho.Instance.getOrderDomesticDetailAsync(OrderDomesticCodeID);
                    mOrderDomesticDetails[OrderDomesticCodeID] = dt;
                    editOrderDomesticDetail(dt);
                }
                catch
                {
                    Console.WriteLine("error getCustomersAsync SQLStore");
                    return null;
                }
            }

            return mOrderDomesticDetails[OrderDomesticCodeID];
        }

        private void editOrderDomesticDetail(DataTable data)
        {
            data.Columns.Add(new DataColumn("SKU", typeof(int)));
            data.Columns.Add(new DataColumn("BarCodeEAN13", typeof(string)));
            data.Columns.Add(new DataColumn("ProductNameVN", typeof(string)));
            data.Columns.Add(new DataColumn("ProductTypeName", typeof(string)));
            data.Columns.Add(new DataColumn("Package", typeof(string)));
            data.Columns.Add(new DataColumn("packing", typeof(string)));
            data.Columns.Add(new DataColumn("Amount", typeof(int)));
            data.Columns.Add(new DataColumn("TotalAmountOrder", typeof(int)));
            data.Columns.Add(new DataColumn("TotalAmountReal", typeof(int)));

            foreach (DataRow dr in data.Rows)
            {
                int productPackingID = Convert.ToInt32(dr["ProductPackingID"]);
                string customerProductTypesCode = Convert.ToString(dr["CustomerProductTypesCode"]);

                DataRow[] packingRows = mProductpacking_dt.Select($"ProductPackingID = {productPackingID}");
                DataRow[] productTypeRows = mProductType_dt.Select($"CustomerProductTypesCode = '{customerProductTypesCode}'");

                string proVN = packingRows.Length > 0 ? packingRows[0]["Name_VN"].ToString() : "Unknown"; ;

                dr["ProductNameVN"] = proVN;
                dr["ProductTypeName"] = productTypeRows.Length > 0 ? Convert.ToString(productTypeRows[0]["TypeName"]) : "";
                dr["Amount"] = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["Amount"]) : 0;
                dr["BarCodeEAN13"] = packingRows.Length > 0 ? Convert.ToString(packingRows[0]["BarCodeEAN13"]) : "";
                dr["packing"] = packingRows.Length > 0 ? packingRows[0]["packing"].ToString() : "";
                dr["SKU"] = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["SKU"]) : 0;
                string package = packingRows.Length > 0 ? packingRows[0]["Package"].ToString() : "";
                dr["Package"] = package;
                if (package.CompareTo("weight") == 0)
                {
                    dr["PCSReal"] = 0;
                }

                decimal price = dr["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Price"]);
                decimal nwOrder = dr["NWOrder"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["NWOrder"]);
                decimal pcsOrder = dr["PCSOrder"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PCSOrder"]);
                decimal nwReal = dr["NWReal"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["NWReal"]);
                decimal pcsReal = dr["PCSReal"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PCSReal"]);
                if (package.CompareTo("weight") == 0 || package.CompareTo("kg") == 0)
                {
                    dr["TotalAmountOrder"] = nwOrder * price;
                    dr["TotalAmountReal"] = nwReal * price;
                }
                else
                {
                    dr["TotalAmountOrder"] = pcsOrder * price;
                    dr["TotalAmountReal"] = pcsReal * price;
                }
            }

            int count = 0;
            data.Columns["OrderDomesticDetailID"].SetOrdinal(count++);
            data.Columns["ProductNameVN"].SetOrdinal(count++);
            data.Columns["ProductTypeName"].SetOrdinal(count++);
            data.Columns["Price"].SetOrdinal(count++);
            data.Columns["PCSOrder"].SetOrdinal(count++);
            data.Columns["NWOrder"].SetOrdinal(count++);
            data.Columns["PCSReal"].SetOrdinal(count++);
            data.Columns["NWReal"].SetOrdinal(count++);
            
        }

        public async Task<DataTable> getProductTypeAsync()
        {
            if (mProductType_dt == null)
            {
                try
                {
                    mProductType_dt = await SQLManager_Kho.Instance.getProductTypeAsync();
                }
                catch
                {
                    Console.WriteLine("error getCustomersAsync SQLStore");
                    return null;
                }
            }

            return mProductType_dt;
        }

        public async Task<DataTable> GetOrderDomesticCodeLogAsync()
        {
            if (mOrderDomesticCodeLog_dt == null)
            {
                mOrderDomesticCodeLog_dt = await SQLManager_Kho.Instance.GetOrderDomesticCodeLogAsync();
            }
            return mOrderDomesticCodeLog_dt;
        }

        public async Task<DataTable> GetOrderDomesticDetailLogAsync(int OrderDomesticDetailID)
        {
            if (!mOrderDomesticDetailLogs.ContainsKey(OrderDomesticDetailID))
            {
                DataTable dt = await SQLManager_Kho.Instance.GetOrderDomesticDetailLogAsync(OrderDomesticDetailID);
                mOrderDomesticDetailLogs[OrderDomesticDetailID] = dt;
            }
            return mOrderDomesticDetailLogs[OrderDomesticDetailID];
        }

        public async Task<DataTable> GetProductDomesticPricesHistoryAsync()
        {
            if (mProductDomesticPricesLog_dt == null)
            {
                DataTable dt = await SQLManager_Kho.Instance.GetProductDomesticPricesHistoryAsync();
                mProductDomesticPricesLog_dt = dt;
            }
            return mProductDomesticPricesLog_dt;
        }

        public void removeOrderDomesticByYear(int year)
        {
            if (mtOrderDomestics.ContainsKey(year))
                mtOrderDomestics.Remove(year);
        }
        public void removeOrderDomesticByMonthYear(int month, int year)
        {
            if (!mtOrderDomestics.ContainsKey(year))
                return;

            if (mtOrderDomestics[year].ContainsKey(month))
                mtOrderDomestics[year].Remove(month);
        }
        public async Task<DataTable> GetOrderDomesticByMonthYearAsync(int month, int year)
        {
            await getOrderDomesticCodeAsync();

            if (!mtOrderDomestics.ContainsKey(year))
                mtOrderDomestics[year] = new Dictionary<int, DataTable>();
            if(!mtOrderDomestics[year].ContainsKey(month))
            {
                try
                {
                    DataTable dt = await SQLManager_Kho.Instance.GetOrderDomesticByMonthYearAsync(month, year);
                    mtOrderDomestics[year][month] = dt;
                    editOrderDomesticByMonthYear(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error getCustomersAsync SQLStore: " + ex.Message);
                    return null;
                }
            }

            return mtOrderDomestics[year][month];
        }

        public async Task<DataTable> GetOrderDomesticByYearAsync(int year)
        {
            await getOrderDomesticCodeAsync();

            if (!mtOrderDomestics.ContainsKey(year))
                mtOrderDomestics[year] = new Dictionary<int, DataTable>();

            if (mtOrderDomestics[year].Count < 12)
            {
                try
                {
                    DataTable dt = await SQLManager_Kho.Instance.GetOrderDomesticByYearAsync(year);
                    editOrderDomesticByMonthYear(dt);

                    var monthDict = dt.AsEnumerable()
                                    .Where(r => r.Field<DateTime?>("DeliveryDate") != null)
                                    .GroupBy(r => r.Field<DateTime>("DeliveryDate").Month)
                                    .ToDictionary(
                                        g => g.Key,               // month: 1..12
                                        g => g.CopyToDataTable()
                                    );


                    foreach (var kv in monthDict)
                    {
                        mtOrderDomestics[year][kv.Key] = kv.Value;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("error getCustomersAsync SQLStore: " + ex.Message);
                    return null;
                }
            }

            Dictionary<int, DataTable> yearDict = mtOrderDomestics[year];

            // Clone schema từ 1 DataTable bất kỳ
            DataTable dtYear = yearDict.Values.First().Clone();

            foreach (var dtMonth in yearDict.Values)
            {
                foreach (DataRow row in dtMonth.Rows)
                {
                    dtYear.ImportRow(row);
                }
            }
            return dtYear;
        }

        private void editOrderDomesticByMonthYear(DataTable data)
        {
            data.Columns.Add(new DataColumn("SKU", typeof(int)));
            data.Columns.Add(new DataColumn("ProductNameVN", typeof(string)));
            data.Columns.Add(new DataColumn("Package", typeof(string)));
            data.Columns.Add(new DataColumn("packing", typeof(string)));
            data.Columns.Add(new DataColumn("AmountPacking", typeof(string)));
            data.Columns.Add(new DataColumn("Amount", typeof(int)));
            data.Columns.Add(new DataColumn("CustomerID", typeof(int)));
            data.Columns.Add(new DataColumn("Company", typeof(string)));
            data.Columns.Add(new DataColumn("ProductTypeName", typeof(string)));
            data.Columns.Add(new DataColumn("DeliveryDate", typeof(DateTime)));
            data.Columns.Add(new DataColumn("TotalAmount", typeof(int)));

            foreach (DataRow dr in data.Rows)
            {
                int productPackingID = Convert.ToInt32(dr["ProductPackingID"]);
                int orderDomesticIndex = Convert.ToInt32(dr["OrderDomesticIndex"]);
                string customerProductTypesCode = Convert.ToString(dr["CustomerProductTypesCode"]);

                DataRow[] productTypeRows = mProductType_dt.Select($"CustomerProductTypesCode = '{customerProductTypesCode}'");
                DataRow[] packingRows = mProductpacking_dt.Select($"ProductPackingID = {productPackingID}");
                DataRow[] orderDomesticCodeRows = mOrderDomesticCode_dt.Select($"OrderDomesticIndex = {orderDomesticIndex}");

                string proVN = packingRows.Length > 0 ? packingRows[0]["Name_VN"].ToString() : "Unknown";
                string company = orderDomesticCodeRows.Length > 0 ? orderDomesticCodeRows[0]["Company"].ToString() : "Unknown";
                int customerID = orderDomesticCodeRows.Length > 0 ? Convert.ToInt32(orderDomesticCodeRows[0]["CustomerID"]) : 0;
                DateTime deliveryDate = orderDomesticCodeRows.Length > 0 ? Convert.ToDateTime(orderDomesticCodeRows[0]["DeliveryDate"]).Date : DateTime.Now.Date;
                int Amount = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["Amount"]) : 0;
                string Packing = packingRows.Length > 0 ? packingRows[0]["packing"].ToString() : "";
                string package = packingRows.Length > 0 ? packingRows[0]["Package"].ToString() : "";
                string AmountPacking = "";
                if (package.CompareTo("weight") == 0 || Packing.CompareTo("weight") == 0)
                    AmountPacking = Packing;
                else
                    AmountPacking = $"{Amount} {Packing}";

                dr["ProductNameVN"] = proVN;
                dr["Company"] = company;
                dr["CustomerID"] = customerID;
                dr["DeliveryDate"] = deliveryDate;
                dr["Amount"] = Amount;
                dr["packing"] = Packing;
                dr["AmountPacking"] = AmountPacking;
                dr["SKU"] = packingRows.Length > 0 ? Convert.ToInt32(packingRows[0]["SKU"]) : 0;
                
                dr["ProductTypeName"] = productTypeRows.Length > 0 ? Convert.ToString(productTypeRows[0]["TypeName"]) : "";
                dr["Package"] = package;
                if (package.CompareTo("weight") == 0)
                {
                    dr["PCSReal"] = 0;
                }

                decimal nw = dr.IsNull("NWReal") ? 0m : Convert.ToDecimal(dr["NWReal"]);
                decimal pcs = dr.IsNull("PCSReal") ? 0m : Convert.ToDecimal(dr["PCSReal"]);
                decimal price = dr.IsNull("Price") ? 0m : Convert.ToDecimal(dr["Price"]);

                if (package == "weight" || package == "kg")
                    dr["TotalAmount"] = nw * price;
                else
                    dr["TotalAmount"] = pcs * price;
            }

            int count = 0;
            data.Columns["DeliveryDate"].SetOrdinal(count++);
            data.Columns["ProductNameVN"].SetOrdinal(count++);
            data.Columns["ProductNameVN"].SetOrdinal(count++);
            data.Columns["ProductTypeName"].SetOrdinal(count++);
            data.Columns["AmountPacking"].SetOrdinal(count++);
            data.Columns["Package"].SetOrdinal(count++);            
            data.Columns["PCSReal"].SetOrdinal(count++);
            data.Columns["NWReal"].SetOrdinal(count++);
            data.Columns["Price"].SetOrdinal(count++);
            data.Columns["TotalAmount"].SetOrdinal(count++);
        }
    }
}
