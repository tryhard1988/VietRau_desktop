using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
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

        bool isLoadSalaryLock = false;
        DataTable mSalaryLock_dt = null;

        bool isLoadOvertimeType = false;
        DataTable mOvertimeType_dt = null;

        bool isLoadLeaveType_HavePaid = false;
        DataTable mLeaveType_HavePaid_dt = null;

        bool isLoadDeductionType = false;
        DataTable mDeductionType_dt = null;

        

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

        public async Task<bool> IsSalaryLockAsync(int month, int year)
        {
            if (!isLoadSalaryLock)
            {
                try
                {
                    mSalaryLock_dt = await SQLManager.Instance.GetSalaryLockAsync();
                    isLoadSalaryLock = true;
                }
                catch
                {
                    isLoadSalaryLock = false;
                }
            }

            if (!isLoadSalaryLock || mSalaryLock_dt == null || mSalaryLock_dt.Rows.Count == 0)
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

        public void restOvertimeTypeAsync()
        {
            isLoadOvertimeType = false;
        }

        public async Task<DataTable> GetOvertimeTypeAsync()
        {
            if (!isLoadOvertimeType)
            {
                try
                {
                    mOvertimeType_dt = await SQLManager.Instance.GetOvertimeTypeAsync();
                    isLoadOvertimeType = true;
                }
                catch
                {
                    isLoadOvertimeType = false;
                }
            }
            
            return mOvertimeType_dt;
        }

        public async Task<DataTable> GetLeaveType_HavePaidAsync()
        {
            if (!isLoadLeaveType_HavePaid)
            {
                try
                {
                    mLeaveType_HavePaid_dt = await SQLManager.Instance.GetLeaveType_HavePaid_Async();
                    isLoadLeaveType_HavePaid = true;
                }
                catch
                {
                    isLoadLeaveType_HavePaid = false;
                }
            }

            return mLeaveType_HavePaid_dt;
        }

        public async Task<DataTable> GetDeductionTypeAsync()
        {
            if (!isLoadDeductionType)
            {
                try
                {
                    mDeductionType_dt = await SQLManager.Instance.GetDeductionTypeAsync();
                    isLoadDeductionType = true;
                }
                catch
                {
                    isLoadDeductionType = false;
                }
            }

            return mDeductionType_dt;
        }

        public async void preload()
        {
            try
            {
                var salaryLockTask = SQLManager.Instance.GetSalaryLockAsync();
                var overtimeTypeAsync = SQLManager.Instance.GetOvertimeTypeAsync();
                var leaveTypeAsync = SQLManager.Instance.GetLeaveType_HavePaid_Async();
                var deductionTypeAsync = SQLManager.Instance.GetDeductionTypeAsync();

                await Task.WhenAll(salaryLockTask, overtimeTypeAsync, leaveTypeAsync, deductionTypeAsync);

                if (salaryLockTask.Status == TaskStatus.RanToCompletion && salaryLockTask.Result != null)
                {
                    mSalaryLock_dt = salaryLockTask.Result;
                    isLoadSalaryLock = true;
                }

                if (overtimeTypeAsync.Status == TaskStatus.RanToCompletion && overtimeTypeAsync.Result != null)
                {
                    mOvertimeType_dt = overtimeTypeAsync.Result;
                    isLoadOvertimeType = true;
                }

                if (leaveTypeAsync.Status == TaskStatus.RanToCompletion && leaveTypeAsync.Result != null)
                {
                    mLeaveType_HavePaid_dt = leaveTypeAsync.Result;
                    isLoadLeaveType_HavePaid = true;
                }

                if (deductionTypeAsync.Status == TaskStatus.RanToCompletion && deductionTypeAsync.Result != null)
                {
                    mDeductionType_dt = deductionTypeAsync.Result;
                    isLoadDeductionType = true;
                }
            }
            catch
            {
                isLoadSalaryLock = false;
            }
        }
    }
}
