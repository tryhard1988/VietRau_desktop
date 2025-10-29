﻿using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using RauViet.ui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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

        public async void preload_Suong()
        {
            try
            {
                var productSKUTask = SQLManager.Instance.getProductSKUAsync();
                var productPackingTask = SQLManager.Instance.getProductpackingAsync();
                var exportCodesTask = SQLManager.Instance.getExportCodesAsync();
                var employeesInDongGoiTask = SQLManager.Instance.GetActiveEmployeesIn_DongGoiAsync();
                await Task.WhenAll(productSKUTask, productPackingTask, exportCodesTask, employeesInDongGoiTask, employeesInDongGoiTask);

                if (employeesInDongGoiTask.Status == TaskStatus.RanToCompletion && employeesInDongGoiTask.Result != null) mEmployeesInDongGoi_dt = employeesInDongGoiTask.Result;
                if (exportCodesTask.Status == TaskStatus.RanToCompletion && exportCodesTask.Result != null) mExportCodes_dt = exportCodesTask.Result;
                if (productSKUTask.Status == TaskStatus.RanToCompletion && productSKUTask.Result != null) mProductSKU_dt = productSKUTask.Result;
                if (productPackingTask.Status == TaskStatus.RanToCompletion && productPackingTask.Result != null) mProductpacking_dt = productPackingTask.Result;

                editExportCodes();
            }
            catch
            {
                Console.WriteLine("preload in SQLStore errror");
            }
        }

        public async void preload_NhanSu()
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
                        gradeName = salaryGradeRows[0]["GradeName"].ToString();
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
                }
            }

            return mHoliday_dt;
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
                }
            }

            return mEmployeeSalaryInfo_dt;
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
                        g => g.OrderByDescending(r => r.Field<int>("Year"))
                              .ThenByDescending(r => r.Field<int>("Month"))
                              .First()
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

        public async Task<DataTable> GetAttendamceAsync(string[] colnames, int month, int year)
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
                }
            }

            
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

        public async Task<DataTable> getProductSKUAsync()
        {
            if (mProductSKU_dt == null)
            {
                try
                {
                    mProductSKU_dt = await SQLManager.Instance.getProductSKUAsync();
                }
                catch
                {
                    Console.WriteLine("error getProductSKUAsync SQLStore");
                }
            }

            return mProductSKU_dt;
        }

        public void resetProductpacking()
        {
            mProductpacking_dt = null;
        }

        public async Task<DataTable> getProductpackingAsync()
        {
            if (mProductpacking_dt == null)
            {
                try
                {
                    mProductpacking_dt = await SQLManager.Instance.getProductpackingAsync(); ;
                }
                catch
                {
                    Console.WriteLine("error getProductpackingAsync SQLStore");
                }
            }

            return mProductpacking_dt;
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
                }
            }

            return mEmployeesInDongGoi_dt;
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
                }
            }

            return mExportCodes_dt;
        }
        private void editExportCodes()
        {            
            mExportCodes_dt.Columns.Add(new DataColumn("InputByName", typeof(string)));
            mExportCodes_dt.Columns.Add(new DataColumn("PackingByName", typeof(string)));
            
            foreach (DataRow dr in mExportCodes_dt.Rows)
            {
                int inputBy = Convert.ToInt32(dr["InputBy"]);
                int packingBy = Convert.ToInt32(dr["PackingBy"]);

                DataRow[] inputByRow = mEmployeesInDongGoi_dt.Select($"EmployeeID = {inputBy}");
                DataRow[] packingByRow = mEmployeesInDongGoi_dt.Select($"EmployeeID = {packingBy}");

                if (inputByRow.Length > 0)
                    dr["InputByName"] = inputByRow[0]["FullName"].ToString();

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
                }
            }

            return mProductSKUHistory_dt;
        }
    }
}
