using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using MySqlX.XDevAPI.Common;
using PdfSharp.Pdf.Content.Objects;
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
    public sealed class SQLStore_QLNS
    {
        private static SQLStore_QLNS ins = null;
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
        DataTable mEmployeeLog_dt = null;
        DataTable mEmployeeInsuranceLog_dt = null;
        DataTable mEmployeeBankLog_dt = null;
        DataTable mEmployee_POS_DEP_CON_Log_dt = null;
        DataTable mEmployeeSalary_Log_dt = null;
        DataTable mMonthlyAllowanceLog_dt = null;
        DataTable mAnnualLeaveBalance_dt = null;
        DataTable mEmployeeAllowances_dt= null;

        Dictionary<int, DataTable> mDeductions;
        Dictionary<string, DataTable> mOvertimeAttendaces;
        Dictionary<int, DataTable> mLeaveAttendances;
        Dictionary<string, DataTable> mAttendances;
        Dictionary<string, DataTable> mSalaryInfoHistories;
        Dictionary<int, DataTable> mSalarySummaryByYears;
        Dictionary<string, DataTable> mEmployeeDeductionLogs;
        Dictionary<string, DataTable> mMonthlyAllowanceLogs;
        Dictionary<int, DataTable> mLeaveAttendanceLogs;
        Dictionary<string, DataTable> mOvertimeAttendanceLogs;
        Dictionary<string, DataTable> mAttendanceLogs;
        private SQLStore_QLNS() { }

        public static SQLStore_QLNS Instance
        {
            get
            {
                lock (padlock)
                {
                    if (ins == null)
                        ins = new SQLStore_QLNS();
                    return ins;
                }
            }
        }

        public async Task preload()
        {
            try
            {
                int monthCur = DateTime.Now.Month;
                int yearCur = DateTime.Now.Year;
                mDeductions = new Dictionary<int, DataTable>();
                mOvertimeAttendaces = new Dictionary<string, DataTable>();
                mLeaveAttendances = new Dictionary<int, DataTable>();
                mAttendances = new Dictionary<string, DataTable>();
                mSalaryInfoHistories = new Dictionary<string, DataTable>();
                mSalarySummaryByYears = new Dictionary<int, DataTable>();
                mEmployeeDeductionLogs = new Dictionary<string, DataTable>();
                mMonthlyAllowanceLogs = new Dictionary<string, DataTable>();
                mLeaveAttendanceLogs = new Dictionary<int, DataTable>();
                mOvertimeAttendanceLogs = new Dictionary<string, DataTable>();
                mAttendanceLogs = new Dictionary<string, DataTable>();

                var salaryLockTask = SQLManager.Instance.GetSalaryLockAsync();
                var overtimeTypeAsync = SQLManager_QLNS.Instance.GetOvertimeTypeAsync();
                var leaveTypeAsync = SQLManager_QLNS.Instance.GetLeaveTypeAsync();
                var deductionTypeAsync = SQLManager_QLNS.Instance.GetDeductionTypeAsync();
                var departmentTask = SQLManager_QLNS.Instance.GetDepartmentAsybc();
                var postionTask = SQLManager_QLNS.Instance.GetPositionAsync();
                var allowanceTypeTask = SQLManager_QLNS.Instance.GetAllowanceTypeAsync();
                var applyScopeAsync = SQLManager_QLNS.Instance.GetApplyScopeAsync();
                var contractTypeTask = SQLManager_QLNS.Instance.GetContractTypeAsync();
                var employeesTask = SQLManager_QLNS.Instance.GetEmployeesAsync();
                var salaryGradeTask = SQLManager_QLNS.Instance.GetSalaryGradeAsync();
                var holidayAsync = SQLManager_QLNS.Instance.GetHolidayAsync();


                await Task.WhenAll(employeesTask, salaryLockTask, overtimeTypeAsync, leaveTypeAsync, deductionTypeAsync, departmentTask, postionTask,
                    allowanceTypeTask, applyScopeAsync, contractTypeTask, salaryGradeTask, holidayAsync);


                var employeeALBTask = SQLManager_QLNS.Instance.GetAnnualLeaveBalanceAsync();
                var deductionAsync1 = SQLManager_QLNS.Instance.GetEmployeeDeductions(yearCur - 1);
                var deductionAsync2 = SQLManager_QLNS.Instance.GetEmployeeDeductions(yearCur);

                int monthPre = monthCur - 1;
                int yearPre = yearCur;
                if (monthPre < 1)
                {
                    monthPre = 12;
                    yearPre -= 1;

                }
                var overtimeAttendamceTask1 = SQLManager_QLNS.Instance.GetOvertimeAttendamceAsync(monthCur, yearCur);
                var overtimeAttendamceTask2 = SQLManager_QLNS.Instance.GetOvertimeAttendamceAsync(monthPre, yearPre);
                var employeeSalaryInfoAsync = SQLManager_QLNS.Instance.GetEmployeeSalaryInfoAsybc();
                var leaveAttendanceTask1 = SQLManager_QLNS.Instance.GetLeaveAttendanceAsync(yearCur - 1);
                var leaveAttendanceTask2 = SQLManager_QLNS.Instance.GetLeaveAttendanceAsync(yearCur);
                var attendamceTask1 = SQLManager_QLNS.Instance.GetAttendamceAsync(monthCur, yearCur);
                var attendamceTask2 = SQLManager_QLNS.Instance.GetAttendamceAsync(monthPre, yearPre);
                var salaryHistoryTask1 = SQLManager_QLNS.Instance.GetSalaryHistoryAsyc(monthCur, yearCur);
                var salaryHistoryTask2 = SQLManager_QLNS.Instance.GetSalaryHistoryAsyc(monthPre, yearPre);

                await Task.WhenAll(leaveAttendanceTask1, leaveAttendanceTask2, employeeSalaryInfoAsync, employeeALBTask, deductionAsync1, deductionAsync2,
                    overtimeAttendamceTask1, overtimeAttendamceTask2, attendamceTask1, attendamceTask2, salaryHistoryTask1, salaryHistoryTask2);

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

                if (employeeALBTask.Status == TaskStatus.RanToCompletion && employeeALBTask.Result != null) mAnnualLeaveBalance_dt = employeeALBTask.Result;
                if (deductionAsync1.Status == TaskStatus.RanToCompletion && deductionAsync1.Result != null) mDeductions[yearCur - 1] = deductionAsync1.Result;
                if (deductionAsync2.Status == TaskStatus.RanToCompletion && deductionAsync2.Result != null) mDeductions[yearCur] = deductionAsync2.Result;
                if (employeesTask.Status == TaskStatus.RanToCompletion && employeesTask.Result != null) mEmployee_dt = employeesTask.Result;
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
                editAnnualLeaveBalance(mAnnualLeaveBalance_dt);
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

        private void editEmployee()
        {
            Utils.AddColumnIfNotExists(mEmployee_dt, "GenderName", typeof(string));
            Utils.AddColumnIfNotExists(mEmployee_dt, "PositionCode", typeof(string));
            Utils.AddColumnIfNotExists(mEmployee_dt, "PositionName", typeof(string));
            Utils.AddColumnIfNotExists(mEmployee_dt, "DepartmentName", typeof(string));
            Utils.AddColumnIfNotExists(mEmployee_dt, "WorkBlock", typeof(string));
            Utils.AddColumnIfNotExists(mEmployee_dt, "ContractTypeName", typeof(string));
            Utils.AddColumnIfNotExists(mEmployee_dt, "ContractTypeCode", typeof(string));
            Utils.AddColumnIfNotExists(mEmployee_dt, "GradeName", typeof(string));
            Utils.AddColumnIfNotExists(mEmployee_dt, "SalaryGrade", typeof(int));
            Utils.AddColumnIfNotExists(mEmployee_dt, "EmployessName_NoSign", typeof(string));

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
                string workBlock = "";
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
                    {
                        departmentName = departmentRows[0]["DepartmentName"].ToString();
                        workBlock = departmentRows[0]["WorkBlock"].ToString();
                    }
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
                dr["WorkBlock"] = workBlock;
                dr["DepartmentName"] = departmentName;
                dr["ContractTypeName"] = contractTypeName;
                dr["ContractTypeCode"] = contractTypeCode;
                dr["GradeName"] = gradeName;
                dr["SalaryGrade"] = salaryGrade; 
                dr["EmployessName_NoSign"] = Utils.RemoveVietnameseSigns(dr["EmployeeCode"].ToString() + " " + dr["FullName"].ToString()).ToLower();
            }
        }

        public async Task<DataTable> GetLeaveAttendancesAsyn(int year)
        {
            if (!mLeaveAttendances.ContainsKey(year))
            {
                try
                {
                    mLeaveAttendances[year] = await SQLManager_QLNS.Instance.GetLeaveAttendanceAsync(year);
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

        public async Task<DataTable> GetLeaveAttendancesAsyn(string[] colNames, int month, int year)
        {
            if (!mLeaveAttendances.ContainsKey(year))
            {
                try
                {
                    mLeaveAttendances[year] = await SQLManager_QLNS.Instance.GetLeaveAttendanceAsync(year);
                    editLeaveAttendances(mLeaveAttendances[year]);
                }
                catch
                {
                    Console.WriteLine("error GetLeaveAttendancesAsync_WithoutCode SQLStore");
                    return null;
                }
            }

            DataTable leaveAttendanceYear = mLeaveAttendances[year];

            var query = leaveAttendanceYear.AsEnumerable().Where(r => r.Field<DateTime>("DateOff").Month == month && r.Field<DateTime>("DateOff").Year == year);

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

                //DataRow[] foundRows = mLeaveType_dt.Select($"LeaveTypeCode = '{leaveTypeCode}'");
                //if (foundRows.Length > 0)
                //{
                //    bool isPaid1 = Convert.ToBoolean(foundRows[0]["IsPaid"]);
                //    if (isPaid1 != isPaid)
                //        continue;
                //}
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
            Utils.AddColumnIfNotExists(data, "LeaveTypeName", typeof(string));
            Utils.AddColumnIfNotExists(data, "DayOfWeek", typeof(string));

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

            Utils.SetGridOrdinal(data, new[] { "DayOfWeek", "DateOff", "LeaveTypeName", "Note" });
        }

        public void removeHoliday() { mHoliday_dt = null; }
        public async Task<DataTable> GetHolidaysAsync()
        {
            if (mHoliday_dt == null)
            {
                try
                {
                    mHoliday_dt = await SQLManager_QLNS.Instance.GetHolidayAsync();
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

            var query = mHoliday_dt.AsEnumerable().Where(r => r.Field<DateTime>("HolidayDate").Month == month && r.Field<DateTime>("HolidayDate").Year == year);

            // 2️⃣ Clone cấu trúc bảng gốc
            DataTable result = mHoliday_dt.Clone();

            // 3️⃣ Import các dòng phù hợp
            foreach (var row in query)
                result.ImportRow(row);

            return result;
        }

        public void removeEmployeeSalaryInfo() { mEmployeeSalaryInfo_dt = null; }
        public async Task<DataTable> GetEmployeeSalaryInfoAsync()
        {
            if (mEmployeeSalaryInfo_dt == null)
            {
                try
                {
                    mEmployeeSalaryInfo_dt = await SQLManager_QLNS.Instance.GetEmployeeSalaryInfoAsybc();
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
        public async Task UpdateEmployeeSalaryInfo(int id, string employeeCode, int month, int year, int baseSalary, int insuranceBaseSalary, string note)
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
                    mSalaryInfoHistories[key] = await SQLManager_QLNS.Instance.GetSalaryHistoryAsyc(month, year);
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
            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(monthInput, yearInput);
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
                                        decimal baseSalary = (Convert.ToDecimal(salaryRow[col]));
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
                    Utils.AddColumnIfNotExists(result, "InsuranceBaseSalary", typeof(int));
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
            Utils.AddColumnIfNotExists(mEmployeeSalaryInfo_dt, "Date", typeof(string));
            foreach (DataRow dr in mEmployeeSalaryInfo_dt.Rows)
            {
                dr["Date"] = dr["Month"] + "/" + dr["Year"];
            }

            Utils.SetGridOrdinal(mEmployeeSalaryInfo_dt, new[] { "Date", "BaseSalary", "InsuranceBaseSalary", "Note", "CreatedAt" });
        }

        public void removeEmployees() { mEmployee_dt = null; }
        public async Task<DataTable> GetEmployeesAsync()
        {
            if (mEmployee_dt == null)
            {
                try
                {
                    mEmployee_dt = await SQLManager_QLNS.Instance.GetEmployeesAsync();
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
                    Utils.AddColumnIfNotExists(activeEmployees, colName, mEmployee_dt.Columns[colName].DataType);
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

        public void removeOverTimeType() { mOvertimeType_dt = null; }
        public async Task<DataTable> GetOvertimeTypeAsync()
        {
            if (mOvertimeType_dt == null)
            {
                try
                {
                    mOvertimeType_dt = await SQLManager_QLNS.Instance.GetOvertimeTypeAsync();
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
                    mLeaveType_dt = await SQLManager_QLNS.Instance.GetLeaveTypeAsync();
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
            if (rows.Length > 0)
            {
                return Convert.ToBoolean(rows[0]["IsDeductAnnualLeave"]);
            }

            return false;
        }
        public async Task<DataTable> GetLeaveTypeWithPaidAsync(bool IsPaid)
        {
            await GetLeaveTypeAsync();

            DataTable activeLeaveType = new DataTable();
            Utils.AddColumnIfNotExists(activeLeaveType, "LeaveTypeCode", typeof(string));
            Utils.AddColumnIfNotExists(activeLeaveType, "LeaveTypeName", typeof(string));

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
            Utils.AddColumnIfNotExists(activeLeaveType, "LeaveTypeCode", typeof(string));
            Utils.AddColumnIfNotExists(activeLeaveType, "LeaveTypeName", typeof(string));

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
                    mDeductionType_dt = await SQLManager_QLNS.Instance.GetDeductionTypeAsync();
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
                    mDeductionType_dt = await SQLManager_QLNS.Instance.GetDeductionTypeAsync();
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

        public void removeDepartment() { mDepartment_dt = null; }
        public async Task<DataTable> GetDepartmentAsync()
        {
            if (mDepartment_dt == null)
            {
                try
                {
                    mDepartment_dt = await SQLManager_QLNS.Instance.GetDepartmentAsybc();
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

        public async Task<DataTable> GetActiveDepartmentAsync(int[] departmentIds)
        {
            await GetDepartmentAsync();
            DataTable filtered_dt = mDepartment_dt.AsEnumerable()
                                                .Where(r =>departmentIds.Contains(r.Field<int>("DepartmentID")) && r.Field<bool>("IsActive") == true)
                                                .CopyToDataTable();

            return filtered_dt;
        }

        public void removePosition() { mPosition_dt = null; }
        public async Task<DataTable> GetPositionAsync()
        {
            if (mPosition_dt == null)
            {
                try
                {
                    mPosition_dt = await SQLManager_QLNS.Instance.GetPositionAsync();
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
            Utils.AddColumnIfNotExists(mAllowance_dt, "ScopeName", typeof(string));
            foreach (DataRow dr in mAllowance_dt.Rows)
            {
                int applyScopeID = Convert.ToInt32(dr["ApplyScopeID"]);
                DataRow[] applyScopeRows = mApplyScope_dt.Select($"ApplyScopeID = {applyScopeID}");
                if (applyScopeRows.Length > 0)
                    dr["ScopeName"] = applyScopeRows[0]["ScopeName"].ToString();
            }
        }

        public void removeAllowanceType() { mAllowance_dt = null; }
        public async Task<DataTable> GetAllowanceTypeAsync()
        {
            if (mAllowance_dt == null)
            {
                try
                {
                    mAllowance_dt = await SQLManager_QLNS.Instance.GetAllowanceTypeAsync();
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
                    mApplyScope_dt = await SQLManager_QLNS.Instance.GetApplyScopeAsync();
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
                    mContractType_dt = await SQLManager_QLNS.Instance.GetContractTypeAsync();
                }
                catch
                {
                    Console.WriteLine("GetContractTypeAsync errror");
                    return null;
                }
            }

            return mContractType_dt;
        }

        public void removeSalaryGrade() { mSalaryGrade_dt = null; }
        public async Task<DataTable> GetSalaryGradeAsync()
        {
            if (mSalaryGrade_dt == null)
            {
                try
                {
                    mSalaryGrade_dt = await SQLManager_QLNS.Instance.GetSalaryGradeAsync();
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

        public void removeAnnualLeaveBalance()
        {
            mAnnualLeaveBalance_dt = null;
        }

        public async Task<DataTable> GetAnnualLeaveBalanceAsync()
        {
            if (mAnnualLeaveBalance_dt == null)
            {
                try
                {
                    mAnnualLeaveBalance_dt = await SQLManager_QLNS.Instance.GetAnnualLeaveBalanceAsync();
                    editAnnualLeaveBalance(mAnnualLeaveBalance_dt);
                }
                catch
                {
                    Console.WriteLine("error GetAnnualLeaveBalanceAsync SQLStore");
                    return null;
                }
            }

            return mAnnualLeaveBalance_dt;
        }

        private void editAnnualLeaveBalance(DataTable employeeALB_dt)
        {
            Utils.AddColumnIfNotExists(employeeALB_dt, "FullName", typeof(string));
            Utils.AddColumnIfNotExists(employeeALB_dt, "PositionName", typeof(string));
            Utils.AddColumnIfNotExists(employeeALB_dt, "RemainingLeaveDays_1", typeof(int));
            Utils.AddColumnIfNotExists(employeeALB_dt, "EmployessName_NoSign", typeof(string));

            var employeeLookup = mEmployee_dt.AsEnumerable().ToDictionary(r => r.Field<string>("EmployeeCode"));
            foreach (DataRow dr in employeeALB_dt.Rows)
            {
                string empCode = dr["EmployeeCode"]?.ToString();
                if (string.IsNullOrEmpty(empCode)) continue;

                if (employeeLookup.TryGetValue(empCode, out DataRow matchRow))
                {
                    dr["PositionName"] = matchRow["PositionName"]?.ToString();
                    dr["FullName"] = matchRow["FullName"]?.ToString();
                    dr["EmployessName_NoSign"] = Utils.RemoveVietnameseSigns(empCode + " " + matchRow["FullName"]?.ToString());
                    int remaining = 0;
                    int leaveCount = 0;

                    int.TryParse(dr["RemainingLeaveDays"]?.ToString(), out remaining);
                    int.TryParse(dr["LeaveCount"]?.ToString(), out leaveCount);

                    dr["RemainingLeaveDays_1"] = remaining - leaveCount;
                }
            }
        }

        public void removeDeduction(int year) { mDeductions.Remove(year); }
        public async Task<DataTable> GetDeductionAsync(int year)
        {
            if (!mDeductions.ContainsKey(year))
            {
                try
                {
                    mDeductions[year] = await SQLManager_QLNS.Instance.GetEmployeeDeductions(year);
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

        public void removeOvertimeAttendamce(int month, int year)
        {
            string key = month.ToString() + "_" + year.ToString();
            mOvertimeAttendaces.Remove(key);
        }
        public async Task<DataTable> GetOvertimeAttendamceAsync(int month, int year)
        {
            string key = month.ToString() + "_" + year.ToString();
            if (!mOvertimeAttendaces.ContainsKey(key))
            {
                try
                {
                    mOvertimeAttendaces[key] = await SQLManager_QLNS.Instance.GetOvertimeAttendamceAsync(month, year);
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

        public async Task<DataTable> GetMonthlyAllowance_AnDem_Async(int month, int year)
        {
            string key = month.ToString() + "_" + year.ToString();
            if (!mOvertimeAttendaces.ContainsKey(key))
            {
                try
                {
                    mOvertimeAttendaces[key] = await SQLManager_QLNS.Instance.GetOvertimeAttendamceAsync(month, year);
                    editOvertimeAttendamce(mOvertimeAttendaces[key]);
                }
                catch
                {
                    Console.WriteLine("error GetĐeuctionAsync SQLStore");
                    return null;
                }
            }

            DataTable data = mOvertimeAttendaces[key];
            var rows = data.AsEnumerable().Where(r =>
            {
                int deptId = Convert.ToInt32(r["DepartmentID"]);
                double hour = Convert.ToDouble(r["HourWork"]);
                TimeSpan? startTime = r.Field<TimeSpan?>("StartTime");

                return (deptId == 26 || deptId == 28 || deptId == 29)
                    && hour >= 2.5
                    && startTime.Value >= new TimeSpan(17, 0, 0);
            });
            string[] keepColumns ={"EmployeeCode", "EmployeeName", "WorkDate", "OvertimeTypeID", "HourWork", "Note", "DepartmentID", "StartTime", "EndTime"};

            DataTable dtOTDem;

            if (rows.Any())
            {
                dtOTDem = rows.CopyToDataTable().DefaultView.ToTable(false, keepColumns);
            }
            else
            {
                dtOTDem = data.Clone().DefaultView.ToTable(false, keepColumns);
            }
            return dtOTDem;
        }

        public void editOvertimeAttendamce(DataTable data)
        {
            Utils.AddColumnIfNotExists(data, "DayOfWeek", typeof(string));
            Utils.AddColumnIfNotExists(data, "OvertimeName", typeof(string));
            Utils.AddColumnIfNotExists(data, "OvertimeTypeCode", typeof(string));
            Utils.AddColumnIfNotExists(data, "SalaryFactor", typeof(decimal));
            Utils.AddColumnIfNotExists(data, "EmployeeName", typeof(string));
            Utils.AddColumnIfNotExists(data, "DepartmentID", typeof(string));

            data.Columns["HourWork"].ReadOnly = false;

            foreach (DataRow row in data.Rows)
            {
                DateTime dt = Convert.ToDateTime(row["WorkDate"]);
                string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                row["DayOfWeek"] = vietDays[(int)dt.DayOfWeek];


                int overtimeTypeID = Convert.ToInt32(row["OvertimeTypeID"]);
                string employeeCode = Convert.ToString(row["EmployeeCode"]);
                DataRow[] rows = mOvertimeType_dt.Select($"OvertimeTypeID = {overtimeTypeID}");
                DataRow[] eRows = mEmployee_dt.Select($"EmployeeCode = '{employeeCode}'");
                if (rows.Length > 0)
                {
                    row["OvertimeName"] = rows[0]["OvertimeName"].ToString();
                    row["OvertimeTypeCode"] = rows[0]["OvertimeTypeCode"].ToString();
                    row["SalaryFactor"] = rows[0]["SalaryFactor"].ToString();
                }
                if(eRows.Length > 0)
                {
                    row["EmployeeName"] = eRows[0]["FullName"].ToString();
                    row["DepartmentID"] = Convert.ToInt32(eRows[0]["DepartmentID"]);
                }

                TimeSpan startTime = (TimeSpan)row["StartTime"];
                TimeSpan endTime = (TimeSpan)row["EndTime"];

                TimeSpan duration = endTime - startTime;
            }

            Utils.SetGridOrdinal(data, new[] { "EmployeeCode", "EmployeeName", "WorkDate", "DayOfWeek", "WorkDate", "OvertimeName", "StartTime", "EndTime", "HourWork", "Note" });
        }

        public async Task<DataTable> GetAttendamceAsync(int month, int year)
        {
            string key = month.ToString() + "_" + year.ToString();
            if (!mAttendances.ContainsKey(key))
            {
                try
                {
                    mAttendances[key] = await SQLManager_QLNS.Instance.GetAttendamceAsync(month, year);
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

            DataTable data = mAttendances[key];
            var filteredRows = data.AsEnumerable().Where(r =>
            {
                DateTime workDate = r.Field<DateTime>("WorkDate");
                return workDate.Month == month && workDate.Year == year;
            });

            // --- Nếu không có dòng nào thì trả về DataTable rỗng với các cột cần giữ ---
            DataTable cloned;

            if (!filteredRows.Any())
            {
                cloned = new DataTable();
                foreach (string col in colnames)
                    Utils.AddColumnIfNotExists(cloned, col, data.Columns[col].DataType);
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
            Utils.AddColumnIfNotExists(data, "DayOfWeek", typeof(string));

            foreach (DataRow row in data.Rows)
            {
                DateTime dt = Convert.ToDateTime(row["WorkDate"]);
                string[] vietDays = { "CN", "T.2", "T.3", "T.4", "T.5", "T.6", "T.7" };
                row["DayOfWeek"] = vietDays[(int)dt.DayOfWeek];
            }

            Utils.SetGridOrdinal(data, new[] { "DayOfWeek", "WorkDate", "WorkingHours", "AttendanceLog", "LeaveTypeName", "OvertimeName", "Note" });
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
                    mSalarySummaryByYears[year] = await SQLManager_QLNS.Instance.GetSalarySummaryByYearAsync(year);
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
            Utils.AddColumnIfNotExists(data, "MonthYear", typeof(string));
            foreach (DataRow row in data.Rows)
            {
                row["MonthYear"] = row["Month"] + "/" + row["Year"];
            }
        }

        public async Task<DataTable> GetEmployeeDeductionLogAsync(int month, int year, string code)
        {
            string key = month + "_" + year + "_" + code;
            if (!mEmployeeDeductionLogs.ContainsKey(key))
            {
                DataTable dt = await SQLManager_QLNS.Instance.GetEmployeeDeductionLogAsync(month, year, code);
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
                    DataTable dt = await SQLManager_QLNS.Instance.GetEmployeeDeductionLogAsync(month, year, code);
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

        public async Task<DataTable> GetMonthlyAllowanceLogAsync(int month, int year)
        {
            string key = month + "_" + year;
            if (!mMonthlyAllowanceLogs.ContainsKey(key))
            {
                DataTable dt = await SQLManager_QLNS.Instance.GetMonthlyAllowanceLogAsync(month, year);
                mMonthlyAllowanceLogs[key] = dt;
            }
            return mMonthlyAllowanceLogs[key];
        }
        public async Task<DataTable> GetEmployeeAllowanceLogAsync()
        {
            if (mMonthlyAllowanceLog_dt == null)
            {
                mMonthlyAllowanceLog_dt = await SQLManager_QLNS.Instance.GetEmployeeAllowanceLogAsync();
            }
            return mMonthlyAllowanceLog_dt;
        }

        public async Task<DataTable> GetLeaveAttendanceLogAsync(int year)
        {
            if (!mLeaveAttendanceLogs.ContainsKey(year))
            {
                DataTable dt = await SQLManager_QLNS.Instance.GetLeaveAttandanceLogAsync(year);
                mLeaveAttendanceLogs[year] = dt;
            }
            return mLeaveAttendanceLogs[year];
        }

        public async Task<DataTable> GetOvertimeAttendanceLogAsync(int month, int year)
        {
            string key = month + "_" + year;
            if (!mOvertimeAttendanceLogs.ContainsKey(key))
            {
                DataTable dt = await SQLManager_QLNS.Instance.GetOvertimeAttandanceLogAsync(month, year);
                mOvertimeAttendanceLogs[key] = dt;
            }
            return mOvertimeAttendanceLogs[key];
        }

        public async Task<DataTable> GetAttendanceLogAsync(int month, int year)
        {
            string key = month + "_" + year;
            if (!mAttendanceLogs.ContainsKey(key))
            {
                DataTable dt = await SQLManager_QLNS.Instance.GetAttendanceLogAsync(month, year);
                mAttendanceLogs[key] = dt;
            }
            return mAttendanceLogs[key];
        }

        public async Task<DataTable> GetEmployeeLogAsync()
        {
            if (mEmployeeLog_dt == null)
            {
                mEmployeeLog_dt = await SQLManager_QLNS.Instance.GetEmployeeLogAsync();
            }
            return mEmployeeLog_dt;
        }
        public async Task<DataTable> GetEmployeeInsuranceLogAsync()
        {
            if (mEmployeeInsuranceLog_dt == null)
            {
                mEmployeeInsuranceLog_dt = await SQLManager_QLNS.Instance.GetEmployeeInsuranceLogAsync();
            }
            return mEmployeeInsuranceLog_dt;
        }
        public async Task<DataTable> GetEmployeeBankLogAsync()
        {
            if (mEmployeeBankLog_dt == null)
            {
                mEmployeeBankLog_dt = await SQLManager_QLNS.Instance.GetEmployeeBankLogAsync();
            }
            return mEmployeeBankLog_dt;
        }
        public async Task<DataTable> GetEmployee_POS_DEP_CON_LogAsync()
        {
            if (mEmployee_POS_DEP_CON_Log_dt == null)
            {
                mEmployee_POS_DEP_CON_Log_dt = await SQLManager_QLNS.Instance.GetEmployee_POS_DEP_CON_LogAsync();
            }
            return mEmployee_POS_DEP_CON_Log_dt;
        }

        public async Task<DataTable> GetEmployeeSalary_LogAsync()
        {
            if (mEmployeeSalary_Log_dt == null)
            {
                mEmployeeSalary_Log_dt = await SQLManager_QLNS.Instance.GetEmployeeSalary_LogAsync();
            }
            return mEmployeeSalary_Log_dt;
        }

        public void removeEmployeeAllowances_All() { mEmployeeAllowances_dt = null; }
        public async Task<DataTable> GetEmployeeAllowances_AllAsync()
        {
            if (mEmployeeAllowances_dt == null)
            {
                mEmployeeAllowances_dt = await SQLManager_QLNS.Instance.GetEmployeeAllowances_AllAsync();
            }
            return mEmployeeAllowances_dt;
        }
    }
}
