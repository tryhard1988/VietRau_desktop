using RauViet.ui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataTable = System.Data.DataTable;

namespace RauViet.classes
{
    public sealed class SQLStore
    {
        private static SQLStore ins = null;
        private static readonly object padlock = new object();

        DataTable mSalaryLock_dt = null;
        DataTable mOvertimeType_dt = null;
        DataTable mLeaveType_dt = null;
        DataTable mDeductionType_dt = null;
        DataTable mDepartment_dt = null;
        DataTable mPosition_dt = null;
        DataTable mAllowance_dt = null;
        DataTable mApplyScope_dt = null;
        DataTable mContractType_dt = null;
        DataTable mSalaryGrade_dt = null;
        DataTable mEmployee_dt = null;
        DataTable mHoliday_dt = null;
        DataTable mEmployeeSalaryInfo_dt = null;

        Dictionary<int, DataTable> mAnnualLeaveBalances;
        Dictionary<int, DataTable> mDeductions;
        Dictionary<string, DataTable> mOvertimeAttendaces;
        Dictionary<int, DataTable> mLeaveAttendances;
        Dictionary<string, DataTable> mAttendances;
        Dictionary<string, DataTable> mSalaryInfoHistories;
        Dictionary<int, DataTable> mSalarySummaryByYears;

        //suong
        DataTable mProductSKUHistory_dt = null;
        DataTable mProductSKU_dt = null;
        DataTable mProductpacking_dt = null;
        DataTable mExportCodes_dt = null;        
        DataTable mEmployeesInDongGoi_dt = null;
        DataTable mCusromer_dt = null;
        DataTable mCartonSize_dt = null;
        DataTable mlatestOrders_dt = null;
        DataTable mExportCodeLog_dt = null;
        DataTable mReportCustomerOrderDetail_dt = null;
        DataTable mMonthlyAllowanceLog_dt = null;
        Dictionary<int, DataTable> mOrderLists;
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
        Dictionary<string, DataTable> mEmployeeDeductionLogs;
        Dictionary<string, DataTable> mMonthlyAllowanceLogs;
        Dictionary<int, DataTable> mLeaveAttendanceLogs;
        Dictionary<string, DataTable> mOvertimeAttendanceLogs;
        Dictionary<string, DataTable> mAttendanceLogs;
        private SQLStore() { }

        public static SQLStore Instance
        {
            get
            {
                lock (padlock)
                {
                    if (ins == null)
                        ins = new SQLStore();
                    return ins;
                }
            }
        }

        public async Task preload_Suong()
        {
            try
            {
                mReportExportByYears = new Dictionary<int, DataTable>();
                mReportCustomerOrderDetailByYears = new Dictionary<int, DataTable>();
                mOrderLists = new Dictionary<int, DataTable>();
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
                mEmployeeDeductionLogs = new Dictionary<string, DataTable>();
                mMonthlyAllowanceLogs = new Dictionary<string, DataTable>();
                mLeaveAttendanceLogs = new Dictionary<int, DataTable>();
                mOvertimeAttendanceLogs = new Dictionary<string, DataTable>();
                mAttendanceLogs = new Dictionary<string, DataTable>();

                var productSKUTask = SQLManager.Instance.getProductSKUAsync();
                var productPackingTask = SQLManager.Instance.getProductpackingAsync();
                var exportCodesTask = SQLManager.Instance.getExportCodesAsync();
                var employeesInDongGoiTask = SQLManager.Instance.GetActiveEmployeesIn_DongGoiAsync();
                var customersTask = SQLManager.Instance.getCustomersAsync();
                
                var cartonSizeTask = SQLManager.Instance.getCartonSizeAsync();
                await Task.WhenAll(productSKUTask, productPackingTask, exportCodesTask, employeesInDongGoiTask, employeesInDongGoiTask, customersTask, cartonSizeTask);

                if (employeesInDongGoiTask.Status == TaskStatus.RanToCompletion && employeesInDongGoiTask.Result != null) mEmployeesInDongGoi_dt = employeesInDongGoiTask.Result;
                if (exportCodesTask.Status == TaskStatus.RanToCompletion && exportCodesTask.Result != null) mExportCodes_dt = exportCodesTask.Result;
                if (productSKUTask.Status == TaskStatus.RanToCompletion && productSKUTask.Result != null) mProductSKU_dt = productSKUTask.Result;
                if (productPackingTask.Status == TaskStatus.RanToCompletion && productPackingTask.Result != null) mProductpacking_dt = productPackingTask.Result;
                if (customersTask.Status == TaskStatus.RanToCompletion && customersTask.Result != null) mCusromer_dt = customersTask.Result;                
                if (cartonSizeTask.Status == TaskStatus.RanToCompletion && cartonSizeTask.Result != null) mCartonSize_dt = cartonSizeTask.Result;

                editProductSKUA();
                editExportCodes();
                editProductpacking();

                mExportCodes_dt.DefaultView.Sort = "ExportCodeID ASC";
                mExportCodes_dt = mExportCodes_dt.DefaultView.ToTable();
                var exportIds = mExportCodes_dt.AsEnumerable().Where(r => r.Field<bool>("Complete") == false).Select(r => r.Field<int>("ExportCodeID")).Take(6).ToList();
                var tasks = exportIds.Select(async id =>
                {
                    DataTable data = await SQLManager.Instance.getOrdersAsync(id);
                    return (id, data);
                }).ToList();

                var results = await Task.WhenAll(tasks);

                foreach (var (id, data) in results)
                {
                    mOrderLists[id] = data;
                    editOrders(data);
                }
            }
            catch
            {
                Console.WriteLine("preload in SQLStore errror");
            }
        }

        public async Task preload_NhanSu()
        {
            try
            {
                int monthCur = DateTime.Now.Month;
                int yearCur = DateTime.Now.Year;
                mAnnualLeaveBalances = new Dictionary<int, DataTable>();
                mDeductions = new Dictionary<int, DataTable>();
                mOvertimeAttendaces = new Dictionary<string, DataTable>();
                mLeaveAttendances = new Dictionary<int, DataTable>();
                mAttendances = new Dictionary<string, DataTable>();
                mSalaryInfoHistories = new Dictionary<string, DataTable>();
                mSalarySummaryByYears = new Dictionary<int, DataTable>();

                var salaryLockTask = SQLManager.Instance.GetSalaryLockAsync();
                var overtimeTypeAsync = SQLManager.Instance.GetOvertimeTypeAsync();
                var leaveTypeAsync = SQLManager.Instance.GetLeaveTypeAsync();
                var deductionTypeAsync = SQLManager.Instance.GetDeductionTypeAsync();
                var departmentTask = SQLManager.Instance.GetDepartmentAsybc();
                var postionTask = SQLManager.Instance.GetPositionAsync();
                var allowanceTypeTask = SQLManager.Instance.GetAllowanceTypeAsync();
                var applyScopeAsync = SQLManager.Instance.GetApplyScopeAsync();
                var contractTypeTask = SQLManager.Instance.GetContractTypeAsync();
                var employeesTask = SQLManager.Instance.GetEmployeesAsync();
                var salaryGradeTask = SQLManager.Instance.GetSalaryGradeAsync();
                var holidayAsync = SQLManager.Instance.GetHolidayAsync();


                await Task.WhenAll(employeesTask, salaryLockTask, overtimeTypeAsync, leaveTypeAsync, deductionTypeAsync, departmentTask, postionTask,
                    allowanceTypeTask, applyScopeAsync, contractTypeTask, salaryGradeTask, holidayAsync);


                var employeeALBTask1 = SQLManager.Instance.GetAnnualLeaveBalanceAsync(yearCur - 1);
                var employeeALBTask2 = SQLManager.Instance.GetAnnualLeaveBalanceAsync(yearCur);
                var deductionAsync1 = SQLManager.Instance.GetEmployeeDeductions(yearCur - 1);
                var deductionAsync2 = SQLManager.Instance.GetEmployeeDeductions(yearCur);
                
                int monthPre = monthCur - 1;
                int yearPre = yearCur;
                if (monthPre < 1)
                {
                    monthPre = 12;
                    yearPre -= 1;

                }
                var overtimeAttendamceTask1 = SQLManager.Instance.GetOvertimeAttendamceAsync(monthCur, yearCur);
                var overtimeAttendamceTask2 = SQLManager.Instance.GetOvertimeAttendamceAsync(monthPre, yearPre);
                var employeeSalaryInfoAsync = SQLManager.Instance.GetEmployeeSalaryInfoAsybc();
                var leaveAttendanceTask1 = SQLManager.Instance.GetLeaveAttendanceAsync(yearCur - 1);
                var leaveAttendanceTask2 = SQLManager.Instance.GetLeaveAttendanceAsync(yearCur);
                var attendamceTask1 = SQLManager.Instance.GetAttendamceAsync(monthCur, yearCur);
                var attendamceTask2 = SQLManager.Instance.GetAttendamceAsync(monthPre, yearPre);
                var salaryHistoryTask1 = SQLManager.Instance.GetSalaryHistoryAsyc(monthCur, yearCur);
                var salaryHistoryTask2 = SQLManager.Instance.GetSalaryHistoryAsyc(monthPre, yearPre);

                await Task.WhenAll(leaveAttendanceTask1, leaveAttendanceTask2, employeeSalaryInfoAsync, employeeALBTask1, employeeALBTask2, 
                    deductionAsync1, deductionAsync2, overtimeAttendamceTask1, overtimeAttendamceTask2, attendamceTask1, attendamceTask2, salaryHistoryTask1, salaryHistoryTask2);

                string keyStrCur = monthCur.ToString() + "_" + yearCur.ToString();
                string keyStrPre = monthPre.ToString() + "_" + yearPre.ToString();
                if (attendamceTask1.Status == TaskStatus.RanToCompletion && attendamceTask1.Result != null) mAttendances[keyStrCur] = attendamceTask1.Result;
                if (attendamceTask2.Status == TaskStatus.RanToCompletion && attendamceTask2.Result != null) mAttendances[keyStrPre] = attendamceTask2.Result;
                if (leaveAttendanceTask1.Status == TaskStatus.RanToCompletion && leaveAttendanceTask1.Result != null) mLeaveAttendances[yearCur - 1] = leaveAttendanceTask1.Result;
                if (leaveAttendanceTask2.Status == TaskStatus.RanToCompletion && leaveAttendanceTask2.Result != null) mLeaveAttendances[yearCur] = leaveAttendanceTask2.Result;
                if (employeeSalaryInfoAsync.Status == TaskStatus.RanToCompletion && employeeSalaryInfoAsync.Result != null) mEmployeeSalaryInfo_dt = employeeSalaryInfoAsync.Result;
                if (overtimeAttendamceTask1.Status == TaskStatus.RanToCompletion && overtimeAttendamceTask1.Result != null) mOvertimeAttendaces[keyStrCur] = overtimeAttendamceTask1.Result;
                if (overtimeAttendamceTask2.Status == TaskStatus.RanToCompletion && overtimeAttendamceTask2.Result != null) mOvertimeAttendaces[keyStrPre] = overtimeAttendamceTask2.Result;
                if (salaryHistoryTask1.Status == TaskStatus.RanToCompletion && salaryHistoryTask1.Result != null) mSalaryInfoHistories[keyStrCur] = salaryHistoryTask1.Result;
                if (salaryHistoryTask2.Status == TaskStatus.RanToCompletion && salaryHistoryTask2.Result != null) mSalaryInfoHistories[keyStrPre] = salaryHistoryTask2.Result;

                if (employeeALBTask1.Status == TaskStatus.RanToCompletion && employeeALBTask1.Result != null) mAnnualLeaveBalances[yearCur - 1] = employeeALBTask1.Result;
                if (employeeALBTask2.Status == TaskStatus.RanToCompletion && employeeALBTask2.Result != null) mAnnualLeaveBalances[yearCur] = employeeALBTask2.Result;
                if (deductionAsync1.Status == TaskStatus.RanToCompletion && deductionAsync1.Result != null)  mDeductions[yearCur - 1] = deductionAsync1.Result;
                if (deductionAsync2.Status == TaskStatus.RanToCompletion && deductionAsync2.Result != null) mDeductions[yearCur] = deductionAsync2.Result;
                if (employeesTask.Status == TaskStatus.RanToCompletion && employeesTask.Result != null) mEmployee_dt = employeesTask.Result;
                if (salaryLockTask.Status == TaskStatus.RanToCompletion && salaryLockTask.Result != null) mSalaryLock_dt = salaryLockTask.Result;
                if (overtimeTypeAsync.Status == TaskStatus.RanToCompletion && overtimeTypeAsync.Result != null) mOvertimeType_dt = overtimeTypeAsync.Result;
                if (leaveTypeAsync.Status == TaskStatus.RanToCompletion && leaveTypeAsync.Result != null) mLeaveType_dt = leaveTypeAsync.Result;
                if (deductionTypeAsync.Status == TaskStatus.RanToCompletion && deductionTypeAsync.Result != null) mDeductionType_dt = deductionTypeAsync.Result;
                if (departmentTask.Status == TaskStatus.RanToCompletion && departmentTask.Result != null) mDepartment_dt = departmentTask.Result;
                if (postionTask.Status == TaskStatus.RanToCompletion && postionTask.Result != null) mPosition_dt = postionTask.Result;
                if (allowanceTypeTask.Status == TaskStatus.RanToCompletion && allowanceTypeTask.Result != null) mAllowance_dt = allowanceTypeTask.Result; 
                if (applyScopeAsync.Status == TaskStatus.RanToCompletion && applyScopeAsync.Result != null) mApplyScope_dt = applyScopeAsync.Result;
                if (contractTypeTask.Status == TaskStatus.RanToCompletion && contractTypeTask.Result != null) mContractType_dt = contractTypeTask.Result;
                if (salaryGradeTask.Status == TaskStatus.RanToCompletion && salaryGradeTask.Result != null) mSalaryGrade_dt = salaryGradeTask.Result;
                if (holidayAsync.Status == TaskStatus.RanToCompletion && holidayAsync.Result != null) mHoliday_dt = holidayAsync.Result;

                editEmployee();
                editAllowanceType();
                editEmployeeSalaryInfo();
                editAnnualLeaveBalance(mAnnualLeaveBalances[yearCur - 1], yearCur - 1);
                editAnnualLeaveBalance(mAnnualLeaveBalances[yearCur], yearCur);
                editOvertimeAttendamce(overtimeAttendamceTask1.Result);
                editOvertimeAttendamce(overtimeAttendamceTask2.Result);
                editLeaveAttendances(leaveAttendanceTask1.Result);
                editLeaveAttendances(leaveAttendanceTask2.Result);
                editAttendamce(attendamceTask1.Result);
                editAttendamce(attendamceTask2.Result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"------preload in SQLStore error: {ex.Message}");
                Console.WriteLine("-----" + ex.StackTrace);
            }
        }

        private void editEmployee()
        {
            mEmployee_dt.Columns.Add(new DataColumn("GenderName", typeof(string)));
            mEmployee_dt.Columns.Add(new DataColumn("PositionCode", typeof(string)));
            mEmployee_dt.Columns.Add(new DataColumn("PositionName", typeof(string)));
            mEmployee_dt.Columns.Add(new DataColumn("DepartmentName", typeof(string)));
            mEmployee_dt.Columns.Add(new DataColumn("ContractTypeName", typeof(string)));
            mEmployee_dt.Columns.Add(new DataColumn("ContractTypeCode", typeof(string)));
            mEmployee_dt.Columns.Add(new DataColumn("GradeName", typeof(string)));
            mEmployee_dt.Columns.Add(new DataColumn("SalaryGrade", typeof(int)));

            foreach (DataRow dr in mEmployee_dt.Rows)
            {
                if (dr["PositionName"] == DBNull.Value) dr["PositionName"] = "";
                if (dr["ContractTypeName"] == DBNull.Value) dr["ContractTypeName"] = "";
            }

            int count = 0;
            foreach (DataRow dr in mEmployee_dt.Rows)
            {
                if (dr["BirthDate"] != DBNull.Value)
                {
                    int age = DateTime.Now.Year - Convert.ToDateTime(dr["BirthDate"]).Year;
                    Boolean gender = Convert.ToBoolean(dr["Gender"]);
                    dr["GenderName"] = (gender == true ? "Nam" : "Nữ  ") + " - " + age;
                }

                

                count++;
                int EmployeeCode = mEmployee_dt.Rows.Count;
                int? positionID = Utils.GetIntValue(dr["PositionID"]);
                int? departmentID = Utils.GetIntValue(dr["DepartmentID"]);
                int? salaryGradeID = Utils.GetIntValue(dr["SalaryGradeID"]);
                int? contractTypeID = Utils.GetIntValue(dr["ContractTypeID"]);

                string positionName = "";
                string positionCode = "";
                string departmentName = "";
                string gradeName = "";
                int salaryGrade = 0;
                string contractTypeName = "";
                string contractTypeCode = "";
                if (positionID != null)
                {
                    DataRow[] postionRows = mPosition_dt.Select($"PositionID = {positionID}");
                    if (postionRows.Length > 0)
                    {
                        positionName = postionRows[0]["PositionName"].ToString();
                        positionCode = postionRows[0]["PositionCode"].ToString();
                    }
                }

                if (departmentID != null)
                {
                    DataRow[] departmentRows = mDepartment_dt.Select($"departmentID = {departmentID}");
                    if (departmentRows.Length > 0)
                        departmentName = departmentRows[0]["DepartmentName"].ToString();
                }
                if (salaryGradeID != null)
                {
                    DataRow[] salaryGradeRows = mSalaryGrade_dt.Select($"SalaryGradeID = {salaryGradeID}");
                    if (salaryGradeRows.Length > 0)
                    {
                        gradeName = salaryGradeRows[0]["GradeName"].ToString();
                        salaryGrade = Convert.ToInt32(salaryGradeRows[0]["Salary"]);
                    }
                }

                if (contractTypeID != null)
                {
                    DataRow[] contractTypeRows = mContractType_dt.Select($"ContractTypeID = {contractTypeID}");
                    if (contractTypeRows.Length > 0)
                    {
                        contractTypeName = contractTypeRows[0]["ContractTypeName"].ToString();
                        contractTypeCode = contractTypeRows[0]["ContractTypeCode"].ToString();
                    }
                }

                dr["PositionCode"] = positionCode;
                dr["PositionName"] = positionName;
                dr["DepartmentName"] = departmentName;
                dr["ContractTypeName"] = contractTypeName;
                dr["ContractTypeCode"] = contractTypeCode;
                dr["GradeName"] = gradeName;
                dr["SalaryGrade"] = salaryGrade;
            }
        }

        public async Task<DataTable> GetLeaveAttendancesAsyn(int year)
        {
            if (!mLeaveAttendances.ContainsKey(year))
            {
                try
                {
                    mLeaveAttendances[year] = await SQLManager.Instance.GetLeaveAttendanceAsync(year);
                    editLeaveAttendances(mLeaveAttendances[year]);
                }
                catch
                {
                    Console.WriteLine("error GetLeaveAttendancesAsync_WithoutCode SQLStore");
                    return null;
                }
            }

            return mLeaveAttendances[year];
        }

        public void removeLeaveAttendances(int year)
        {
            mLeaveAttendances.Remove(year);
        }

        public async Task<DataTable> GetLeaveAttendancesAsyn(string[] colNames, int month, int year, bool isPaid)
        {
            if (!mLeaveAttendances.ContainsKey(year))
            {
                try
                {
                    mLeaveAttendances[year] = await SQLManager.Instance.GetLeaveAttendanceAsync(year);
                    editLeaveAttendances(mLeaveAttendances[year]);
                }
                catch
                {
                    Console.WriteLine("error GetLeaveAttendancesAsync_WithoutCode SQLStore");
                    return null;
                }
            }

            DataTable leaveAttendanceYear = mLeaveAttendances[year];

            var query = leaveAttendanceYear.AsEnumerable().Where(r =>r.Field<DateTime>("DateOff").Month == month && r.Field<DateTime>("DateOff").Year == year);

            // 2️⃣ Clone cấu trúc bảng gốc
            DataTable result = leaveAttendanceYear.Clone();

            // 3️⃣ Giữ lại chỉ các cột trong colNames
            foreach (DataColumn col in result.Columns.Cast<DataColumn>().ToList())
            {
                if (!colNames.Contains(col.ColumnName))
                    result.Columns.Remove(col);
            }

            // 4️⃣ Copy dữ liệu được chọn
            foreach (var row in query)
            {
                string leaveTypeCode = row["LeaveTypeCode"].ToString();

                DataRow[] foundRows = mLeaveType_dt.Select($"LeaveTypeCode = '{leaveTypeCode}'");
                if(foundRows.Length > 0)
                {
                    bool isPaid1 = Convert.ToBoolean(foundRows[0]["IsPaid"]);
                    if (isPaid1 != isPaid)
                        continue;
                }
                DataRow newRow = result.NewRow();
                foreach (string col in colNames)
                {
                    newRow[col] = row[col];
                }
                result.Rows.Add(newRow);
            }

            return result;
        }

        public void editLeaveAttendances(DataTable data)
        {
            data.Columns.Add(new DataColumn("LeaveTypeName", typeof(string)));
            data.Columns.Add(new DataColumn("DayOfWeek", typeof(string)));

            string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
            foreach (DataRow row in data.Rows)
            {
                string leaveTypeCode = Convert.ToString(row["LeaveTypeCode"]);
                DataRow[] foundRows = mLeaveType_dt.Select($"LeaveTypeCode = '{leaveTypeCode}'");
                if (foundRows.Length > 0)
                {
                    DateTime dayOff = Convert.ToDateTime(row["DateOff"]);
                    row["LeaveTypeName"] = foundRows[0]["LeaveTypeName"].ToString();
                    row["DayOfWeek"] = vietDays[(int)dayOff.DayOfWeek];
                }
            }
        }

        public async Task<DataTable> GetHolidaysAsync()
        {
            if (mHoliday_dt == null)
            {
                try
                {
                    mHoliday_dt = await SQLManager.Instance.GetHolidayAsync();
                }
                catch
                {
                    Console.WriteLine("GetHolidaysAsync errror");
                    return null;
                }
            }

            return mHoliday_dt;
        }

        public async Task<bool> IsHolidaysAsync(DateTime date)
        {
            await GetHolidaysAsync();

            DataRow[] rows = mHoliday_dt.Select($"HolidayDate = #{date:MM/dd/yyyy}#");

            return rows.Length > 0;
        }

        public async Task<DataTable> GetHolidaysAsync(int month, int year)
        {
            await GetHolidaysAsync();

            var query = mHoliday_dt.AsEnumerable().Where(r =>r.Field<DateTime>("HolidayDate").Month == month && r.Field<DateTime>("HolidayDate").Year == year);

            // 2️⃣ Clone cấu trúc bảng gốc
            DataTable result = mHoliday_dt.Clone();

            // 3️⃣ Import các dòng phù hợp
            foreach (var row in query)
                result.ImportRow(row);

            return result;
        }

        public async Task<DataTable> GetEmployeeSalaryInfoAsync()
        {
            if (mEmployeeSalaryInfo_dt == null)
            {
                try
                {
                    mEmployeeSalaryInfo_dt = await SQLManager.Instance.GetEmployeeSalaryInfoAsybc();
                    editEmployeeSalaryInfo();
                }
                catch
                {
                    Console.WriteLine("GetEmployeeSalaryInfoAsync errror");
                    return null;
                }
            }

            return mEmployeeSalaryInfo_dt;
        }
        public async Task UpdateEmployeeSalaryInfo(int id,string employeeCode, int month, int year, int baseSalary, int insuranceBaseSalary, string note)
        {
            await GetEmployeeSalaryInfoAsync();
            DataRow drToAdd = mEmployeeSalaryInfo_dt.NewRow();
            drToAdd["SalaryInfoID"] = id;
            drToAdd["EmployeeCode"] = employeeCode;
            drToAdd["Month"] = month;
            drToAdd["Year"] = year;
            drToAdd["BaseSalary"] = baseSalary;
            drToAdd["InsuranceBaseSalary"] = insuranceBaseSalary;
            drToAdd["Note"] = note;
            drToAdd["CreatedAt"] = DateTime.Now;

            mEmployeeSalaryInfo_dt.Rows.Add(drToAdd);
            mEmployeeSalaryInfo_dt.AcceptChanges();
        }

        public async Task<DataTable> GetSalaryHistoryAsyc(int month, int year)
        {
            string key = month.ToString() + "_" + year.ToString();
            if (!mSalaryInfoHistories.ContainsKey(key))
            {
                try
                {
                    mSalaryInfoHistories[key] = await SQLManager.Instance.GetSalaryHistoryAsyc(month, year);
                }
                catch
                {
                    Console.WriteLine("error GetSalaryHistoryAsyc SQLStore");
                    return null;
                }
            }

            return mSalaryInfoHistories[key];
        }

        public async Task<DataTable> GetEmployeeSalaryInfoAsync(string[] colNames, int monthInput, int yearInput)
        {
            bool isLock = await IsSalaryLockAsync(monthInput, yearInput);
            if (!isLock)
            {
                await GetEmployeeSalaryInfoAsync();
                // 1️⃣ Tạo dictionary lưu bản ghi gần nhất của từng EmployeeCode
                var latestSalary = mEmployeeSalaryInfo_dt.AsEnumerable()
                    .Where(r =>
                        r.Field<int>("Year") < yearInput ||
                        (r.Field<int>("Year") == yearInput && r.Field<int>("Month") <= monthInput))
                    .GroupBy(r => r.Field<string>("EmployeeCode"))
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(r => r.Field<int>("SalaryInfoID")).First()
                    );

                // 2️⃣ Clone cấu trúc từ mEmployeeSalaryInfo_dt (để có cùng kiểu dữ liệu)
                DataTable result = mEmployeeSalaryInfo_dt.Clone();

                // 3️⃣ Giữ lại cột cần thiết + EmployeeCode
                foreach (DataColumn col in result.Columns.Cast<DataColumn>().ToList())
                {
                    if (!colNames.Contains(col.ColumnName) && col.ColumnName != "EmployeeCode")
                        result.Columns.Remove(col);
                }

                // 4️⃣ Duyệt toàn bộ nhân viên trong mEmployee_dt
                foreach (DataRow empRow in mEmployee_dt.AsEnumerable().Where(r => r.Field<bool>("IsActive")))
                {
                    string empCode = empRow.Field<string>("EmployeeCode");
                    string contractTypeCode = empRow.Field<string>("ContractTypeCode");
                    decimal probationSalaryPercent = empRow.Field<decimal>("ProbationSalaryPercent");
                    DataRow newRow = result.NewRow();

                    newRow["EmployeeCode"] = empCode;

                    // Nếu có dữ liệu lương gần nhất
                    if (latestSalary.TryGetValue(empCode, out var salaryRow))
                    {
                        foreach (string col in colNames)
                        {
                            if (result.Columns.Contains(col))
                            {
                                if (contractTypeCode.CompareTo("t_viec") == 0)
                                {
                                    if (col.CompareTo("BaseSalary") == 0)
                                    {
                                        decimal baseSalary = (Convert.ToDecimal(salaryRow[col]) * probationSalaryPercent);
                                        newRow[col] = baseSalary;
                                    }
                                    else if (col.CompareTo("InsuranceBaseSalary") == 0)
                                        newRow[col] = 0;
                                    else
                                        newRow[col] = salaryRow[col];
                                }
                                else
                                {
                                    newRow[col] = salaryRow[col];
                                }
                            }
                        }
                    }
                    else // Không có dữ liệu → điền 0
                    {
                        foreach (string col in colNames)
                        {
                            if (result.Columns.Contains(col))
                            {
                                // Nếu kiểu là số → điền 0, nếu kiểu khác → DBNull
                                if (result.Columns[col].DataType == typeof(int) ||
                                    result.Columns[col].DataType == typeof(decimal) ||
                                    result.Columns[col].DataType == typeof(double))
                                {
                                    newRow[col] = 0;
                                }
                                else
                                {
                                    newRow[col] = DBNull.Value;
                                }
                            }
                        }
                    }

                    result.Rows.Add(newRow);
                }

                return result;
            }
            else
            {
                DataTable data = await GetSalaryHistoryAsyc(monthInput, yearInput);

                DataTable result = data.Clone();

                // 3️⃣ Giữ lại cột cần thiết + EmployeeCode
                foreach (DataColumn col in result.Columns.Cast<DataColumn>().ToList())
                {
                    if (!colNames.Contains(col.ColumnName) && col.ColumnName != "EmployeeCode")
                        result.Columns.Remove(col);
                }

                if (colNames.Contains("InsuranceBaseSalary") && !result.Columns.Contains("InsuranceBaseSalary"))
                {
                    result.Columns.Add("InsuranceBaseSalary", typeof(int));
                }

                foreach (DataRow row in data.Rows)
                {
                    DataRow newRow = result.NewRow();
                    newRow["EmployeeCode"] = row["EmployeeCode"];
                    foreach (string col in colNames)
                    {
                        if (data.Columns.Contains(col))
                            newRow[col] = row[col];
                    }

                    if (result.Columns.Contains("InsuranceBaseSalary"))
                    {
                        newRow["InsuranceBaseSalary"] = Convert.ToInt32(row["NetInsuranceSalary"]) - Convert.ToInt32(row["InsuranceAllowance"]);
                    }

                    result.Rows.Add(newRow);
                }

                return result;
            }
        }

        public void editEmployeeSalaryInfo()
        {
            mEmployeeSalaryInfo_dt.Columns.Add(new DataColumn("Date", typeof(string)));
            foreach (DataRow dr in mEmployeeSalaryInfo_dt.Rows)
            {
                dr["Date"] = dr["Month"] + "/" + dr["Year"];
            }

            int count = 0;
            mEmployeeSalaryInfo_dt.Columns["Date"].SetOrdinal(count++);
            mEmployeeSalaryInfo_dt.Columns["BaseSalary"].SetOrdinal(count++);
            mEmployeeSalaryInfo_dt.Columns["InsuranceBaseSalary"].SetOrdinal(count++);
            mEmployeeSalaryInfo_dt.Columns["Note"].SetOrdinal(count++);
            mEmployeeSalaryInfo_dt.Columns["CreatedAt"].SetOrdinal(count++);
        }

        public async Task<DataTable> GetEmployeesAsync()
        {
            if (mEmployee_dt == null)
            {
                try
                {
                    mEmployee_dt = await SQLManager.Instance.GetEmployeesAsync();
                    editEmployee();
                }
                catch
                {
                    Console.WriteLine("GetEmployeesAsync errror");
                    return null;
                }
            }

            return mEmployee_dt;
        }

        public async Task<DataTable> GetEmployeesAsync(string[] keepColumns)
        {
            await GetEmployeesAsync();

            // Các cột cần giữ lại            

            // Tạo DataTable mới
            DataTable activeEmployees = new DataTable();

            // Thêm các cột cần giữ (theo cấu trúc của mEmployee_dt)
            foreach (string colName in keepColumns)
            {
                if (mEmployee_dt.Columns.Contains(colName))
                    activeEmployees.Columns.Add(colName, mEmployee_dt.Columns[colName].DataType);
            }

            // Lọc và copy dữ liệu IsActive = true
            foreach (DataRow row in mEmployee_dt.Select("IsActive = true"))
            {
                DataRow newRow = activeEmployees.NewRow();
                foreach (string colName in keepColumns)
                {
                    newRow[colName] = row[colName];
                }
                activeEmployees.Rows.Add(newRow);
            }

            return activeEmployees;
        }

        public void updateEmploy(string employeeCode, Dictionary<string, object> parameters)
        {
            DataRow[] rows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");

            if (rows.Length == 0)
            {
                Console.WriteLine($"Không tìm thấy nhân viên có mã {employeeCode}");
                return;
            }

            DataRow row = rows[0];

            // Cập nhật từng cột theo key-value trong parameters
            foreach (var kvp in parameters)
            {
                string colName = kvp.Key;
                object value = kvp.Value ?? DBNull.Value;

                // Chỉ cập nhật nếu cột tồn tại trong DataTable
                if (mEmployee_dt.Columns.Contains(colName))
                {
                    row[colName] = value;
                }
            }
        }

        public void ResetlockSalary()
        {
            mSalaryLock_dt = null;
        }
        public async Task<bool> IsSalaryLockAsync(int month, int year)
        {
            if (mSalaryLock_dt == null)
            {
                try
                {
                    mSalaryLock_dt = await SQLManager.Instance.GetSalaryLockAsync();
                }
                catch
                {
                    Console.WriteLine("IsSalaryLockAsync errror");
                    return false;
                }
            }

            if (mSalaryLock_dt == null || mSalaryLock_dt.Rows.Count == 0)
                return false;

            // Tìm tháng bị khóa gần nhất
            var latestLock = mSalaryLock_dt.AsEnumerable()
                .Where(r => r.Field<bool>("IsLocked"))
                .OrderByDescending(r => r.Field<int>("Year"))
                .ThenByDescending(r => r.Field<int>("Month"))
                .FirstOrDefault();

            if (latestLock == null)
                return false;

            int lockedMonth = latestLock.Field<int>("Month");
            int lockedYear = latestLock.Field<int>("Year");

            // Nếu tháng/năm đang kiểm tra <= tháng/năm bị khóa gần nhất => true (đã bị khóa)
            if (year < lockedYear || (year == lockedYear && month <= lockedMonth))
                return true;

            return false;
        }

        public async Task<DataTable> GetOvertimeTypeAsync()
        {
            if (mOvertimeType_dt == null)
            {
                try
                {
                    mOvertimeType_dt = await SQLManager.Instance.GetOvertimeTypeAsync();
                }
                catch
                {
                    Console.WriteLine("GetOvertimeTypeAsync errror");
                    return null;
                }
            }
            
            return mOvertimeType_dt;
        }

        public decimal GetOvertime_SalaryFactor(int overtimeTypeID)
        {

            DataRow[] rows = mOvertimeType_dt.Select($"OvertimeTypeID = {overtimeTypeID}");

            decimal salaryFactor = 1; // mặc định 1
            if (rows.Length > 0)
            {
                salaryFactor = Convert.ToDecimal(rows[0]["SalaryFactor"]);
            }

            return salaryFactor;
        }

        public async Task<DataTable> GetOvertimeTypeAsync(bool isActive)
        {
            await GetOvertimeTypeAsync();

            DataTable activeOverTime = mOvertimeType_dt.Clone(); // Clone cấu trúc
            foreach (DataRow row in mOvertimeType_dt.Select($"IsActive = {isActive}"))
            {
                activeOverTime.ImportRow(row);
            }

            return activeOverTime;
        }

        public async Task<DataTable> GetLeaveTypeAsync()
        {
            if (mLeaveType_dt == null)
            {
                try
                {
                    mLeaveType_dt = await SQLManager.Instance.GetLeaveTypeAsync();
                }
                catch
                {
                    Console.WriteLine("GetLeaveTypeAsync errror");
                    return null;
                }
            }

            return mLeaveType_dt;
        }

        public bool IsDeductAnnualLeave(string code)
        {
            DataRow[] rows = mLeaveType_dt.Select($"LeaveTypeCode = '{code}'");
            if(rows.Length > 0)
            {
                return Convert.ToBoolean(rows[0]["IsDeductAnnualLeave"]);
            }

            return false;
        }
        public async Task<DataTable> GetLeaveTypeWithPaidAsync(bool IsPaid)
        {
            await GetLeaveTypeAsync();

            DataTable activeLeaveType = new DataTable();
            activeLeaveType.Columns.Add("LeaveTypeCode", typeof(string));
            activeLeaveType.Columns.Add("LeaveTypeName", typeof(string));

            // Lọc các dòng IsPaid = true
            foreach (DataRow row in mLeaveType_dt.Select("IsPaid = true"))
            {
                DataRow newRow = activeLeaveType.NewRow();
                newRow["LeaveTypeCode"] = row["LeaveTypeCode"];
                newRow["LeaveTypeName"] = row["LeaveTypeName"];
                activeLeaveType.Rows.Add(newRow);
            }

            return activeLeaveType;
        }

        public async Task<DataTable> GetLeaveTypeWithoutAsync(List<string> leaveCodes)
        {
            await GetLeaveTypeAsync();

            DataTable activeLeaveType = new DataTable();
            activeLeaveType.Columns.Add("LeaveTypeCode", typeof(string));
            activeLeaveType.Columns.Add("LeaveTypeName", typeof(string));

            // Lọc các dòng IsPaid = true
            foreach (DataRow row in mLeaveType_dt.Rows)
            {
                string code = row["LeaveTypeCode"].ToString();

                // Bỏ qua nếu code có trong danh sách parameters
                if (leaveCodes.Contains(code))
                    continue;

                DataRow newRow = activeLeaveType.NewRow();
                newRow["LeaveTypeCode"] = code;
                newRow["LeaveTypeName"] = row["LeaveTypeName"];
                activeLeaveType.Rows.Add(newRow);
            }

            return activeLeaveType;
        }

        public async Task<DataTable> GetDeductionTypeAsync()
        {
            if (mDeductionType_dt == null)
            {
                try
                {
                    mDeductionType_dt = await SQLManager.Instance.GetDeductionTypeAsync();
                }
                catch
                {
                    Console.WriteLine("GetDeductionTypeAsync errror");
                    return null;
                }
            }

            return mDeductionType_dt;
        }

        public async Task<string> GetDeductionNameAsync(string deductionTypeCode)
        {
            if (mDeductionType_dt == null)
            {
                try
                {
                    mDeductionType_dt = await SQLManager.Instance.GetDeductionTypeAsync();
                }
                catch
                {
                    Console.WriteLine("GetDeductionNameAsync errror");
                    return null;
                }
            }

            DataRow[] foundRows = mDeductionType_dt.Select($"DeductionTypeCode = '{deductionTypeCode}'");

            string deductionTypeName = foundRows.Length > 0
                ? foundRows[0]["DeductionTypeName"].ToString()
                : string.Empty;

            return deductionTypeName;
        }

        public async Task<DataTable> GetDepartmentAsync()
        {
            if (mDepartment_dt == null)
            {
                try
                {
                    mDepartment_dt = await SQLManager.Instance.GetDepartmentAsybc();
                }
                catch
                {
                    Console.WriteLine("GetDepartmentAsync errror");
                    return null;
                }
            }

            return mDepartment_dt;
        }

        public async Task<DataTable> GetActiveDepartmentAsync()
        {
            await GetDepartmentAsync();

            DataTable activeDept = mDepartment_dt.Clone(); // Clone cấu trúc
            foreach (DataRow row in mDepartment_dt.Select("IsActive = true"))
            {
                activeDept.ImportRow(row);
            }

            return activeDept;
        }

        public async Task<DataTable> GetPositionAsync()
        {
            if (mPosition_dt == null)
            {
                try
                {
                    mPosition_dt = await SQLManager.Instance.GetPositionAsync();
                }
                catch
                {
                    Console.WriteLine("GetPositionAsync errror");
                    return null;
                }
            }

            return mPosition_dt;
        }

        public async Task<DataTable> GetActivePositionAsync()
        {
            await GetPositionAsync();

            DataTable activePos = mPosition_dt.Clone(); // Clone cấu trúc
            foreach (DataRow row in mPosition_dt.Select("IsActive = true"))
            {
                activePos.ImportRow(row);
            }

            return activePos;
        }
        private void editAllowanceType()
        {
            mAllowance_dt.Columns.Add(new DataColumn("ScopeName", typeof(string)));
            foreach (DataRow dr in mAllowance_dt.Rows)
            {
                int applyScopeID = Convert.ToInt32(dr["ApplyScopeID"]);
                DataRow[] applyScopeRows = mApplyScope_dt.Select($"ApplyScopeID = {applyScopeID}");
                if (applyScopeRows.Length > 0)
                    dr["ScopeName"] = applyScopeRows[0]["ScopeName"].ToString();
            }
        }

        public async Task<DataTable> GetAllowanceTypeAsync()
        {
            if (mAllowance_dt == null)
            {
                try
                {
                    mAllowance_dt = await SQLManager.Instance.GetAllowanceTypeAsync();
                    editAllowanceType();
                }
                catch
                {
                    Console.WriteLine("GetAllowanceTypeAsync errror");
                    return null;
                }
            }

            return mAllowance_dt;
        }

        public async Task<DataTable> GetApplyScopeAsync()
        {
            if (mApplyScope_dt == null)
            {
                try
                {
                    mApplyScope_dt = await SQLManager.Instance.GetApplyScopeAsync();
                }
                catch
                {
                    Console.WriteLine("GetApplyScopeAsync errror");
                    return null;
                }
            }

            return mApplyScope_dt;
        }

        public async Task<DataTable> GetContractTypeAsync()
        {
            if (mContractType_dt == null)
            {
                try
                {
                    mContractType_dt = await SQLManager.Instance.GetContractTypeAsync();
                }
                catch
                {
                    Console.WriteLine("GetContractTypeAsync errror");
                    return null;
                }
            }

            return mContractType_dt;
        }

        public async Task<DataTable> GetSalaryGradeAsync()
        {
            if (mSalaryGrade_dt == null)
            {
                try
                {
                    mSalaryGrade_dt = await SQLManager.Instance.GetSalaryGradeAsync();
                }
                catch
                {
                    Console.WriteLine("GetSalaryGradeAsync errror");
                    return null;
                }
            }

            return mSalaryGrade_dt;
        }

        public async Task<DataTable> GetActiveSalaryGradeAsync()
        {
            await GetSalaryGradeAsync();

            DataTable activeSalaryGrade = mSalaryGrade_dt.Clone(); // Clone cấu trúc
            foreach (DataRow row in mSalaryGrade_dt.Select("IsActive = true"))
            {
                activeSalaryGrade.ImportRow(row);
            }

            return activeSalaryGrade;
        }

        public async Task<DataTable> GetAnnualLeaveBalanceAsync(int year)
        {
            if (!mAnnualLeaveBalances.ContainsKey(year))
            {
                try
                {
                    mAnnualLeaveBalances[year] = await SQLManager.Instance.GetAnnualLeaveBalanceAsync(year);
                    editAnnualLeaveBalance(mAnnualLeaveBalances[year], year);
                }
                catch
                {
                    Console.WriteLine("error GetAnnualLeaveBalanceAsync SQLStore");
                    return null;
                }
            }

            return mAnnualLeaveBalances[year];
        }

        private void editAnnualLeaveBalance(DataTable employeeALB_dt, int year)
        {
            employeeALB_dt.Columns["LeaveCount"].ReadOnly = false;
            employeeALB_dt.Columns.Add(new DataColumn("RemainingLeave", typeof(int)));
            employeeALB_dt.Columns.Add(new DataColumn("Year", typeof(int)));
            employeeALB_dt.Columns.Add(new DataColumn("FullName", typeof(string)));
            employeeALB_dt.Columns.Add(new DataColumn("PositionName", typeof(string)));

            var employeeLookup = mEmployee_dt.AsEnumerable().ToDictionary(r => r.Field<string>("EmployeeCode"));
            foreach (DataRow dr in employeeALB_dt.Rows)
            {
                List<int> monthList = new List<int>();
                string month_str = Convert.ToString(dr["Month"]);
                if (!string.IsNullOrEmpty(month_str))
                {
                    monthList = month_str.Split(',', (char)StringSplitOptions.RemoveEmptyEntries).Select(m => int.Parse(m.Trim())).ToList();
                }

                dr["Year"] = year;
                dr["RemainingLeave"] = monthList.Count - Convert.ToInt32(dr["LeaveCount"]);

                string empCode = dr["EmployeeCode"]?.ToString();
                if (string.IsNullOrEmpty(empCode)) continue;

                if (employeeLookup.TryGetValue(empCode, out DataRow matchRow))
                {
                    dr["PositionName"] = matchRow["PositionName"]?.ToString();
                    dr["FullName"] = matchRow["FullName"]?.ToString();
                }
            }
        }

        public async Task<DataTable> GetDeductionAsync(int year)
        {
            if (!mDeductions.ContainsKey(year))
            {
                try
                {
                    mDeductions[year] = await SQLManager.Instance.GetEmployeeDeductions(year);
                }
                catch
                {
                    Console.WriteLine("error GetĐeuctionAsync SQLStore");
                    return null;
                }
            }

            return mDeductions[year];
        }
        public async Task<DataTable> GetDeductionAsync(int month, int year, string deductionCode)
        {
            await GetDeductionAsync(year);

            DataTable deduction_dt = mDeductions[year];
            var filteredRows = deduction_dt.AsEnumerable().Where(r =>
                                                            r.Field<DateTime>("DeductionDate").Month == month &&
                                                            r.Field<DateTime>("DeductionDate").Year == year &&
                                                            r.Field<string>("DeductionTypeCode") == deductionCode
                                                        );

            DataTable filteredTable;

            foreach (DataColumn col in deduction_dt.Columns)
            {
                Console.WriteLine(col.ColumnName);
            }

            // Nếu có dòng thỏa mãn thì copy vào DataTable mới
            if (filteredRows.Any())
                filteredTable = filteredRows.CopyToDataTable();
            else
                filteredTable = deduction_dt.Clone(); // clone cấu trúc rỗng

            return filteredTable;
        }

        public async Task<DataTable> GetDeductionAsync(int year, string deductionCode)
        {
            await GetDeductionAsync(year);

            DataTable deduction_dt = mDeductions[year];
            var filteredRows = deduction_dt.AsEnumerable().Where(r =>
                                                            r.Field<DateTime>("DeductionDate").Year == year &&
                                                            r.Field<string>("DeductionTypeCode") == deductionCode
                                                        );

            DataTable filteredTable;

            foreach (DataColumn col in deduction_dt.Columns)
            {
                Console.WriteLine(col.ColumnName);
            }

            // Nếu có dòng thỏa mãn thì copy vào DataTable mới
            if (filteredRows.Any())
                filteredTable = filteredRows.CopyToDataTable();
            else
                filteredTable = deduction_dt.Clone(); // clone cấu trúc rỗng

            return filteredTable;
        }

        public async Task<DataTable> GetDeductionAsync(int month, int year)
        {
            await GetDeductionAsync(year);

            DataTable deduction_dt = mDeductions[year];
            var filteredRows = deduction_dt.AsEnumerable().Where(r =>
                                                            r.Field<DateTime>("DeductionDate").Month == month &&
                                                            r.Field<DateTime>("DeductionDate").Year == year
                                                        );

            DataTable filteredTable;

            foreach (DataColumn col in deduction_dt.Columns)
            {
                Console.WriteLine(col.ColumnName);
            }

            // Nếu có dòng thỏa mãn thì copy vào DataTable mới
            if (filteredRows.Any())
                filteredTable = filteredRows.CopyToDataTable();
            else
                filteredTable = deduction_dt.Clone(); // clone cấu trúc rỗng

            return filteredTable;
        }

        public void addOrUpdateDeduction(int year, DataRow row)
        {
            DataTable table = mDeductions[year];
            string deductionID = row["EmployeeDeductionID"].ToString();

            DataRow existingRow = table.AsEnumerable()
                .FirstOrDefault(r => r["EmployeeDeductionID"].ToString() == deductionID);

            if (existingRow != null)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (col.ColumnName != "EmployeeDeductionID") 
                        existingRow[col.ColumnName] = row[col.ColumnName];
                }
            }
            else
            {
                mDeductions[year].ImportRow(row);
            }
        }

        public async Task<DataTable> GetOvertimeAttendamceAsync(int month, int year)
        {
            string key = month.ToString() + "_" + year.ToString();
            if (!mOvertimeAttendaces.ContainsKey(key))
            {
                try
                {
                    mOvertimeAttendaces[key] = await SQLManager.Instance.GetOvertimeAttendamceAsync(month, year);
                    editOvertimeAttendamce(mOvertimeAttendaces[key]);
                }
                catch
                {
                    Console.WriteLine("error GetĐeuctionAsync SQLStore");
                    return null;
                }
            }
            
            return mOvertimeAttendaces[key];
        }

        public void editOvertimeAttendamce(DataTable data)
        {
            data.Columns.Add("DayOfWeek", typeof(string));
            data.Columns.Add("OvertimeName", typeof(string));
            data.Columns.Add("SalaryFactor", typeof(decimal));

            data.Columns["HourWork"].ReadOnly = false;

            foreach (DataRow row in data.Rows)
            {
                DateTime dt = Convert.ToDateTime(row["WorkDate"]);
                string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                row["DayOfWeek"] = vietDays[(int)dt.DayOfWeek];


                int overtimeTypeID = Convert.ToInt32(row["OvertimeTypeID"]);
                DataRow[] rows = mOvertimeType_dt.Select($"OvertimeTypeID = {overtimeTypeID}");
                if (rows.Length > 0)
                {
                    row["OvertimeName"] = rows[0]["OvertimeName"].ToString();
                    row["SalaryFactor"] = rows[0]["SalaryFactor"].ToString();
                }

                TimeSpan startTime = (TimeSpan)row["StartTime"];
                TimeSpan endTime = (TimeSpan)row["EndTime"];

                TimeSpan duration = endTime - startTime;
            }
        }

        public async Task<DataTable> GetAttendamceAsync(int month, int year)
        {
            string key = month.ToString() + "_" + year.ToString();
            if (!mAttendances.ContainsKey(key))
            {
                try
                {
                    mAttendances[key] = await SQLManager.Instance.GetAttendamceAsync(month, year);
                    editAttendamce(mAttendances[key]);
                }
                catch
                {
                    Console.WriteLine("error GetĐeuctionAsync SQLStore");
                    return null;
                }
            }

            return mAttendances[key];
        }
        public async Task<DataTable> GetAttendamceAsync(string[] colnames, int month, int year)
        {
            await GetAttendamceAsync(month, year);
            string key = month.ToString() + "_" + year.ToString();

            
            if (colnames == null)
            {       
                return mAttendances[key];
            }

            DataTable data =  mAttendances[key];
            var filteredRows = data.AsEnumerable() .Where(r =>  {
                                                            DateTime workDate = r.Field<DateTime>("WorkDate");
                                                            return workDate.Month == month && workDate.Year == year;
                                                        });

            // --- Nếu không có dòng nào thì trả về DataTable rỗng với các cột cần giữ ---
            DataTable cloned;

            if (!filteredRows.Any())
            {
                cloned = new DataTable();
                foreach (string col in colnames)
                    cloned.Columns.Add(col, data.Columns[col].DataType);
            }
            else
            {
                // --- Clone với các cột được chỉ định ---
                cloned = filteredRows.CopyToDataTable().DefaultView.ToTable(false, colnames);
            }

            return cloned;
        }

        private void editAttendamce(DataTable data)
        {
            data.Columns.Add("DayOfWeek", typeof(string));

            foreach (DataRow row in data.Rows)
            {
                DateTime dt = Convert.ToDateTime(row["WorkDate"]);
                string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                row["DayOfWeek"] = vietDays[(int)dt.DayOfWeek];
            }

            int count = 0;
            data.Columns["DayOfWeek"].SetOrdinal(count++);
            data.Columns["WorkDate"].SetOrdinal(count++);
            data.Columns["WorkingHours"].SetOrdinal(count++);
            data.Columns["AttendanceLog"].SetOrdinal(count++);
            data.Columns["LeaveTypeName"].SetOrdinal(count++);
            data.Columns["OvertimeName"].SetOrdinal(count++);
            data.Columns["Note"].SetOrdinal(count);
        }

        public void removeAttendamce(int month, int year)
        {
            string key = month.ToString() + "_" + year.ToString();
            mAttendances.Remove(key);
        }

        public async Task<DataTable> GetSalarySummaryByYearAsync(int year)
        {
            if (!mSalarySummaryByYears.ContainsKey(year))
            {
                try
                {
                    mSalarySummaryByYears[year] = await SQLManager.Instance.GetSalarySummaryByYearAsync(year);
                    editSalarySummaryByYear(mSalarySummaryByYears[year]);
                }
                catch
                {
                    Console.WriteLine("error GetSalarySummaryByYearAsync SQLStore");
                    return null;
                }
            }

            return mSalarySummaryByYears[year];
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
                    mProductSKU_dt = await SQLManager.Instance.getProductSKUAsync();
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
                    mProductpacking_dt = await SQLManager.Instance.getProductpackingAsync();
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

        public async Task<DataTable> GetActiveEmployeesIn_DongGoiAsync()
        {
            if (mEmployeesInDongGoi_dt == null)
            {
                try
                {
                    mEmployeesInDongGoi_dt = await SQLManager.Instance.GetActiveEmployeesIn_DongGoiAsync();
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
                    mExportCodes_dt = await SQLManager.Instance.getExportCodesAsync();
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

            // 3️⃣ Thêm các dòng đã lọc vào bảng kết quả (chỉ giữ cột cần)
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
                    mProductSKUHistory_dt = await SQLManager.Instance.GetProductSKUHistoryAsync();
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
                    mCusromer_dt = await SQLManager.Instance.getCustomersAsync();
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
                    DataTable dt = await SQLManager.Instance.getOrdersAsync(exportCodeID);
                    mOrderLists[exportCodeID] = dt;
                    editOrders(dt);
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


        private void editOrders(DataTable data)
        {
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
                    mReportExportByYears[year] = await SQLManager.Instance.GetExportHistoryAsync(year);
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
                    mReportCustomerOrderDetailByYears[year] = await SQLManager.Instance.GetCustomerOrderDetailHistory_InYearAsync(year);
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
                    mCartonSize_dt = await SQLManager.Instance.getCartonSizeAsync();
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
                    mlatestOrders_dt = await SQLManager.Instance.get3LatestOrdersAsync();
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
                DataTable dt = await SQLManager.Instance.getOrdersTotalAsync(exportCodeID);
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
                DataTable dt = await SQLManager.Instance.GetLOTCodeByExportCodeAsync(exportCodeID);
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
                DataTable dt = await SQLManager.Instance.getOrdersDKKDAsync(exportCodeID);
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
                DataTable dt = await SQLManager.Instance.GetCustomerDetailPackingAsync(exportCodeID);
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
                DataTable dt = await SQLManager.Instance.getOrdersPhytoAsync(exportCodeID);
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
                DataTable dt = await SQLManager.Instance.getOrdersChotPhytosync(exportCodeID);
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
                DataTable dt = await SQLManager.Instance.getOrdersINVOICEAsync(exportCodeID);
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
                DataTable dt = await SQLManager.Instance.GetCustomersOrdersAsync(exportCodeID);
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
                DataTable dt = await SQLManager.Instance.GetExportCartonCountsAsync(exportCodeID);
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
                DataTable dt = await SQLManager.Instance.GetDetailPackingTotalAsync(exportCodeID);
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
                mExportCodeLog_dt = await SQLManager.Instance.GetExportCodeLogAsync();
            }
            return mExportCodeLog_dt;
        }

        public async Task<DataTable> GetOrderLogAsync(int exportCodeID)
        {
            if (!mOrderLogs.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager.Instance.GetOrderLogAsync(exportCodeID);
                mOrderLogs[exportCodeID] = dt;
            }
            return mOrderLogs[exportCodeID];
        }

        public async Task<DataTable> GetOrderPackingLogAsync(int exportCodeID)
        {
            if (!mOrderPackingLogs.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager.Instance.GetOrderPackingLogAsync(exportCodeID);
                mOrderPackingLogs[exportCodeID] = dt;
            }
            return mOrderPackingLogs[exportCodeID];
        }

        public async Task<DataTable> GetDo47LogAsync(int exportCodeID)
        {
            if (!mDo47Logs.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager.Instance.GetDo47LogAsync(exportCodeID);
                mDo47Logs[exportCodeID] = dt;
            }
            return mDo47Logs[exportCodeID];
        }

        public async Task<DataTable> GetLotCodeLogAsync(int exportCodeID)
        {
            if (!mLotCodeLogs.ContainsKey(exportCodeID))
            {
                DataTable dt = await SQLManager.Instance.GetLotCodeLogAsync(exportCodeID);
                mLotCodeLogs[exportCodeID] = dt;
            }
            return mLotCodeLogs[exportCodeID];
        }
        public async Task<DataTable> GetEmployeeDeductionLogAsync(int month, int year, string code)
        {
            string key = month + "_" + year + "_" + code;
            if (!mEmployeeDeductionLogs.ContainsKey(key))
            {
                DataTable dt = await SQLManager.Instance.GetEmployeeDeductionLogAsync(month, year, code);
                mEmployeeDeductionLogs[key] = dt;
            }
            return mEmployeeDeductionLogs[key];
        }
        public async Task<DataTable> GetEmployeeDeductionLogAsync(int year, string code)
        {
            DataTable data = null;
            for (int month = 1; month < 13; month++)
            {
                string key = month + "_" + year + "_" + code;
                if (!mEmployeeDeductionLogs.ContainsKey(key))
                {
                    DataTable dt = await SQLManager.Instance.GetEmployeeDeductionLogAsync(month, year, code);
                    mEmployeeDeductionLogs[key] = dt;
                }
                DataTable source = mEmployeeDeductionLogs[key];

                // Clone schema nếu data chưa có cột
                if (data == null)
                {
                    data = source.Clone();
                }

                // Import rows
                foreach (DataRow row in source.Rows)
                {
                    data.ImportRow(row);
                }
            }
            return data;
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
                    mReportCustomerOrderDetail_dt = await SQLManager.Instance.GetCustomerOrderDetailHistoryAsync();
                }
                catch
                {
                    Console.WriteLine("error GetCustomerOrderDetailHistory SQLStore");
                    return null;
                }
            }

            return mReportCustomerOrderDetail_dt;
        }

        public async Task<DataTable> GetMonthlyAllowanceLogAsync(int month, int year)
        {
            string key = month + "_" + year;
            if (!mMonthlyAllowanceLogs.ContainsKey(key))
            {
                DataTable dt = await SQLManager.Instance.GetMonthlyAllowanceLogAsync(month, year);
                mMonthlyAllowanceLogs[key] = dt;
            }
            return mMonthlyAllowanceLogs[key];
        }
        public async Task<DataTable> GetEmployeeAllowanceLogAsync()
        {
            if (mMonthlyAllowanceLog_dt == null)
            {
                mMonthlyAllowanceLog_dt = await SQLManager.Instance.GetEmployeeAllowanceLogAsync();
            }
            return mMonthlyAllowanceLog_dt;
        }

        public async Task<DataTable> GetLeaveAttendanceLogAsync(int year)
        {
            if (!mLeaveAttendanceLogs.ContainsKey(year))
            {
                DataTable dt = await SQLManager.Instance.GetLeaveAttandanceLogAsync(year);
                mLeaveAttendanceLogs[year] = dt;
            }
            return mLeaveAttendanceLogs[year];
        }

        public async Task<DataTable> GetOvertimeAttendanceLogAsync(int month, int year)
        {
            string key = month + "_" + year;
            if (!mOvertimeAttendanceLogs.ContainsKey(key))
            {
                DataTable dt = await SQLManager.Instance.GetOvertimeAttandanceLogAsync(month, year);
                mOvertimeAttendanceLogs[key] = dt;
            }
            return mOvertimeAttendanceLogs[key];
        }

        public async Task<DataTable> GetAttendanceLogAsync(int month, int year)
        {
            string key = month + "_" + year;
            if (!mAttendanceLogs.ContainsKey(key))
            {
                DataTable dt = await SQLManager.Instance.GetAttendanceLogAsync(month, year);
                mAttendanceLogs[key] = dt;
            }
            return mAttendanceLogs[key];
        }
    }
}
