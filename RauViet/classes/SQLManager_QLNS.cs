
using DocumentFormat.OpenXml.Bibliography;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;



namespace RauViet.classes
{
    public class OvertimeAttendanceResult
    {
        public int OvertimeAttendanceID { get; set; }
        public string EmployeeCode { get; set; }
    }

    public sealed class SQLManager_QLNS
    {
        private static SQLManager_QLNS ins = null;
        private static readonly object padlock = new object();

        private SQLManager_QLNS() { }

        public static SQLManager_QLNS Instance
        {
            get
            {
                lock (padlock)
                {
                    if (ins == null)
                        ins = new SQLManager_QLNS();
                    return ins;
                }
            }
        }

        public string qlnvHis_conStr() { return "Server=192.168.1.8,1433;Database=QLNV_VR_History;User Id=qlnv_vr_history;Password=A7t#kP2x;"; }
        public string ql_NhanSu_conStr() { return "Server=192.168.1.8,1433;Database=QLNV;User Id=vietrau;Password=A7t#kP2x;"; }
        public string ql_NhanSu_Log_conStr() { return "Server=192.168.1.8,1433;Database=QLNV_Log;User Id=vietrau;Password=A7t#kP2x;"; }

        public async Task<DataTable> GetActiveEmployeesIn_DongGoiAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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

        public async Task<DataTable> GetEmployeesAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
                                    bool isMale, string homeTown, string address, string citizenID, DateTime? issueDate, string issuePlace, 
                                    bool isActive, bool canCreateUserName, string phone, string noteResign, bool isInsuranceRefund, int salaryGradeID)
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
                                IsActive=@IsActive,
                                canCreateUserName=@canCreateUserName,
                                PhoneNumber=@PhoneNumber,
                                IsInsuranceRefund=@IsInsuranceRefund,
                                SalaryGradeID=@SalaryGradeID,
                                NoteResign=@NoteResign
                            WHERE EmployeeID=@EmployeeID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@canCreateUserName", canCreateUserName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phone);
                        cmd.Parameters.AddWithValue("@NoteResign", noteResign);
                        cmd.Parameters.AddWithValue("@IsInsuranceRefund", isInsuranceRefund);
                        cmd.Parameters.AddWithValue("@SalaryGradeID", salaryGradeID);
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
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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


        public async Task<(int EmployeeID, string EmployeeCode)> insertEmployeeAsync(string maNV_temp, string tenNV, DateTime birthDate, DateTime hireDate, bool isMale, string homeTown,
                                                    string address, string citizenID, DateTime? issueDate, string issuePlace, bool isActive, bool canCreateUserName, 
                                                    string phone, string noteResign, bool isInsuranceRefund, int salaryGradeID)
        {
            int newId = -1;
            string newCode = null;
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertEmployee", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

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
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@CanCreateUserName", canCreateUserName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phone);
                        cmd.Parameters.AddWithValue("@NoteResign", noteResign);
                        cmd.Parameters.AddWithValue("@IsInsuranceRefund", isInsuranceRefund);
                        cmd.Parameters.AddWithValue("@SalaryGradeID", salaryGradeID);

                        // Output params
                        SqlParameter idParam = new SqlParameter("@NewEmployeeID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        SqlParameter codeParam = new SqlParameter("@NewEmployeeCode", SqlDbType.NVarChar, 50)
                        {
                            Direction = ParameterDirection.Output
                        };

                        cmd.Parameters.Add(idParam);
                        cmd.Parameters.Add(codeParam);

                        await cmd.ExecuteNonQueryAsync();

                        newId = Convert.ToInt32(idParam.Value);
                        newCode = codeParam.Value?.ToString();

                        // 👉 bạn có thể return code nếu muốn
                        Console.WriteLine($"Employee created: {newCode}");
                    }
                }

                return (newId, newCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting employee: {ex.Message}");
                return (newId, newCode);
            }
        }

        public async Task<DataTable> GetActiveEmployee_DeductionATT_Async(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeAllowance_ChuyenCan", con))
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

        public async Task<DataTable> GetEmployeeLeave_PT_KP_Async(int month, int year)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeLeave_PT_KP", con))
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


        public async Task<DataTable> GetContractTypeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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

        public async Task<DataTable> GetApplyScopeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM ApplyScope";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetDepartmentAsybc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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

        public async Task<DataTable> GetUserInfoAsync(string employeeCode)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();

                string query = @"SELECT EmployeeID, EmployeeCode, FullName FROM Employee WHERE EmployeeCode = @EmployeeCode AND IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            // Trả về dòng đầu tiên nếu có
            return dt;
        }


        public async Task<bool> UpsertAttendanceBatchAsync(List<(string EmployeeCode, DateTime WorkDate, decimal WorkingHours, string Note, string AttendanceLog)> attendanceData)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("EmployeeCode", typeof(string));
                dt.Columns.Add("WorkDate", typeof(DateTime));
                dt.Columns.Add("WorkingHours", typeof(decimal));
                dt.Columns.Add("Note", typeof(string));
                dt.Columns.Add("AttendanceLog", typeof(string));

                foreach (var item in attendanceData)
                {
                    dt.Rows.Add(item.EmployeeCode, item.WorkDate.Date, item.WorkingHours, item.Note, item.AttendanceLog);
                }

                using (SqlConnection conn = new SqlConnection(ql_NhanSu_conStr()))
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

        public async Task<DataTable> GetGetRemainingLeaveAsync(int year)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetRemainingLeave", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Year", year);

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
            var dt = new DataTable();

            try
            {
                using (var con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    using (var cmd = new SqlCommand("sp_GetAttendanceSummary", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Thêm tham số
                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;

                        // Đọc dữ liệu
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy dữ liệu chấm công: " + ex.Message, ex);
            }

            return dt;
        }

        public async Task<DataTable> GetHolidayAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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

            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
            string query = "sp_InsertHolidayAndAttendance";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HolidayDate", holidayDate.Date);
                        cmd.Parameters.AddWithValue("@HolidayName", holidayName);

                        // Dùng ReturnValue để nhận giá trị trả về
                        SqlParameter returnParam = new SqlParameter("@ReturnVal", SqlDbType.Int);
                        returnParam.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(returnParam);

                        await cmd.ExecuteNonQueryAsync();

                        int result = (int)returnParam.Value;
                        return result == 1;
                    }
                }
            }
            catch
            {
                return false;
            }
        }



        public async Task<bool> DeleteHolidayAsync(DateTime holidayDate)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                using (SqlCommand cmd = new SqlCommand("sp_DeleteHolidayAndAttendance", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@HolidayDate", holidayDate.Date);

                    await con.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();

                    // Vì SP có COMMIT TRANSACTION nên nếu không lỗi => xem như thành công
                    return true;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                MessageBox.Show($"General Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public async Task<DataTable> GetOvertimeTypeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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

        public async Task<bool> updateOvertimeTypeAsync(int overtimeTypeID, string overtimeName, double salaryFactor, bool isActive)
        {
            string query = @"UPDATE OvertimeType SET 
                                OvertimeName=@OvertimeName, 
                                SalaryFactor=@SalaryFactor, 
                                IsActive=@IsActive 
                            WHERE OvertimeTypeID=@OvertimeTypeID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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

        public async Task<bool> deleteOvertimeTypeAsync(int ID)
        {
            string query = "DELETE FROM OvertimeType WHERE OvertimeTypeID=@OvertimeTypeID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OvertimeTypeID", ID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetOvertimeAttendamceForSalaryAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT 
                                    oa.OvertimeTypeID,
                                    oa.EmployeeCode, oa.WorkDate,
                                    ot.OvertimeName, ot.SalaryFactor,
                                    CAST(DATEDIFF(MINUTE, oa.StartTime, oa.EndTime) / 60.0 AS DECIMAL(10,2)) AS HourWork                                    
                                FROM OvertimeAttendance oa
                                JOIN OvertimeType ot ON oa.OvertimeTypeID = ot.OvertimeTypeID
                                WHERE MONTH(oa.WorkDate) = @Month
                                  AND YEAR(oa.WorkDate) = @Year
                                ORDER BY oa.EmployeeCode, oa.WorkDate;";

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

        public async Task<DataTable> GetAttendamceForSalaryAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT EmployeeCode, WorkDate, WorkingHours
                                FROM Attendance
                                WHERE MONTH(WorkDate) = @Month AND YEAR(WorkDate) = @Year
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

        public async Task<DataTable> GetOvertimeAttendamceAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT OvertimeAttendanceID, EmployeeCode, WorkDate, StartTime, EndTime, OvertimeTypeID, Note,
                                CAST(DATEDIFF(MINUTE, StartTime, EndTime) / 60.0 AS DECIMAL(10,2)) AS HourWork  
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

        public async Task<bool> updateOvertimeAttendanceAsync(int overtimeAttendanceID, string employeeCode,
                                        TimeSpan startTime, TimeSpan endTime, int overtimeTypeID, string note)
        {
            string query = @"UPDATE OvertimeAttendance SET 
                                EmployeeCode=@EmployeeCode, 
                                StartTime=@StartTime, 
                                EndTime=@EndTime,
                                OvertimeTypeID=@OvertimeTypeID, 
                                Note=@Note                               
                            WHERE OvertimeAttendanceID=@OvertimeAttendanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OvertimeAttendanceID", overtimeAttendanceID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@StartTime", startTime);
                        cmd.Parameters.AddWithValue("@EndTime", endTime);
                        cmd.Parameters.AddWithValue("@OvertimeTypeID", overtimeTypeID);
                        cmd.Parameters.AddWithValue("@Note", note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<List<OvertimeAttendanceResult>> InsertOvertimeAttendanceAsync(List<string> employeeCodes, DateTime workDate, TimeSpan startTime, TimeSpan endTime, int overtimeTypeID, string note)
        {
            var results = new List<OvertimeAttendanceResult>();

            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(
                        "sp_InsertOvertimeAttendance_MultiEmployee", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // 1️⃣ Tạo DataTable cho TVP
                        DataTable dtEmployeeCodes = new DataTable();
                        dtEmployeeCodes.Columns.Add("EmployeeCode", typeof(string));

                        foreach (var code in employeeCodes)
                            dtEmployeeCodes.Rows.Add(code);

                        // 2️⃣ TVP parameter
                        SqlParameter tvpParam = cmd.Parameters.AddWithValue(
                            "@EmployeeCodes", dtEmployeeCodes);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.EmployeeCode_TVP";

                        // 3️⃣ Các param dùng chung
                        cmd.Parameters.Add("@WorkDate", SqlDbType.Date).Value = workDate.Date;
                        cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = startTime;
                        cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = endTime;
                        cmd.Parameters.Add("@OvertimeTypeID", SqlDbType.Int).Value = overtimeTypeID;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar, 255).Value = note ?? (object)DBNull.Value;

                        // 4️⃣ Đọc kết quả trả về
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new OvertimeAttendanceResult
                                {
                                    OvertimeAttendanceID = reader.GetInt32(0),
                                    EmployeeCode = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new List<OvertimeAttendanceResult>();
            }

            return results;
        }

        public async Task<bool> deleteOvertimeAttendanceAsync(int id)
        {
            string query = "DELETE FROM OvertimeAttendance WHERE OvertimeAttendanceID=@OvertimeAttendanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OvertimeAttendanceID", id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertOvertimeAttendanceAsync(string employeeCode, DateTime workDate, TimeSpan startTime, TimeSpan endTime, 
                                                                int overtimeTypeID, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO OvertimeAttendance (
                                        EmployeeCode, WorkDate, StartTime, EndTime, OvertimeTypeID, Note
                                    )
                                    OUTPUT INSERTED.OvertimeAttendanceID
                                    VALUES (
                                        @EmployeeCode, @WorkDate, @StartTime, @EndTime, @OvertimeTypeID, @Note
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
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
        public async Task<DataTable> GetAnnualLeaveBalanceAsync()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetAnnualLeaveReport", con))
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


        public async Task<bool> UpsertAnnualLeaveBalanceBatchAsync(List<(string EmployeeCode, int RemainingLeaveDays)> albData, bool isResetPhep, int month, int year)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("EmployeeCode", typeof(string));
                dt.Columns.Add("RemainingLeaveDays", typeof(int));

                foreach (var item in albData)
                {
                    dt.Rows.Add(item.EmployeeCode, item.RemainingLeaveDays);
                }

                using (SqlConnection conn = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("Upsert_AnnualLeaveBalance_Batch", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@AnnualLeaveList", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "AnnualLeaveBalanceType";

                        cmd.Parameters.Add("@IsResetPhep", SqlDbType.Bit).Value = isResetPhep;
                        cmd.Parameters.Add("@Month", SqlDbType.Int).Value = month;
                        cmd.Parameters.Add("@Year", SqlDbType.Int).Value = year;

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
                using (SqlConnection conn = new SqlConnection(ql_NhanSu_conStr()))
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

        public async Task<DataTable> GetLeaveAttendanceAsync(int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM LeaveAttendance WHERE YEAR(DateOff) = @Year ORDER BY EmployeeCode, DateOff;";

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

        public async Task<DataTable> GetLeaveAttendanceAsync_Withour_NghiLe(int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT *
                                FROM LeaveAttendance
                                WHERE YEAR(DateOff) = @Year
                                  AND LeaveTypeCode != 'NL_1'
                                ORDER BY EmployeeCode, DateOff;";

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

        public async Task<DataTable> GetLeaveAttendanceAsync_IsPaid(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetPaidLeaveByMonth", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@Month", month);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetEmployeeDeductions(int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeDeductions", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

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

            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();

                string query = @"SELECT * FROM LeaveType ";

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

        public async Task<int> insertLeaveAttendanceAsync(string employeeCode, string leaveTypeCode, DateTime dateOff, string Note, decimal hourLeave)
        {
            int newId = -1;

            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_InsertLeaveAttendance", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Thêm các tham số cho SP
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@LeaveTypeCode", leaveTypeCode);
                        cmd.Parameters.AddWithValue("@DateOff", dateOff.Date);
                        cmd.Parameters.AddWithValue("@Note", Note ?? "");
                        cmd.Parameters.AddWithValue("@LeaveHours", hourLeave);

                        // ExecuteScalarAsync sẽ trả về LeaveID (SP đã SELECT LeaveID)
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

        public async Task<bool> updateLeaveAttendanceAsync(int leaveID, string employeeCode, string leaveTypeCode, DateTime dateOff, string Note, decimal hourLeave)
        {
            int updatedId = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateLeaveAttendance", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@LeaveID", leaveID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@LeaveTypeCode", leaveTypeCode);
                        cmd.Parameters.AddWithValue("@DateOff", dateOff.Date);
                        cmd.Parameters.AddWithValue("@Note", Note);
                        cmd.Parameters.AddWithValue("@LeaveHours", hourLeave);

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            updatedId = Convert.ToInt32(result);
                    }
                }
                return updatedId == leaveID;
            }
            catch
            { 
                return false;
            }
        }

        public async Task<bool> deleteLeaveAttendanceAsync(int leaveID)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteLeaveAttendance", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@LeaveID", leaveID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetAllowanceTypeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM AllowanceType";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetAllowanceTypeAsync(string scopeCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT at.AllowanceTypeID,
                                at.AllowanceName,
                                at.IsInsuranceIncluded,
                                at.ApplyScopeID,
                                at.IsActive
                         FROM AllowanceType at
                         INNER JOIN ApplyScope aps
                             ON at.ApplyScopeID = aps.ApplyScopeID
                         WHERE aps.ScopeCode = @ScopeCode;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ScopeCode", scopeCode);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }


        public async Task<int> insertAllowanceTypeAsync(string allowanceName, string allowanceCode, int applyScopeID, bool isActive, bool isInsuranceIncluded)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO AllowanceType (
                                        AllowanceName, AllowanceCode, ApplyScopeID, IsActive, IsInsuranceIncluded
                                    )
                                    OUTPUT INSERTED.AllowanceTypeID
                                    VALUES (
                                        @AllowanceName, @AllowanceCode, @ApplyScopeID, @IsActive, @IsInsuranceIncluded
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@AllowanceName", allowanceName);
                        cmd.Parameters.AddWithValue("@AllowanceCode", allowanceCode);
                        cmd.Parameters.AddWithValue("@ApplyScopeID", applyScopeID);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        cmd.Parameters.AddWithValue("@IsInsuranceIncluded", isInsuranceIncluded);

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

        public async Task<bool> updateAllowanceTypeAsync(int allowanceTypeID, string allowanceName, string allowanceCode, int applyScopeID, bool isActive, bool isInsuranceIncluded)
        {
            string query = @"UPDATE AllowanceType SET 
                                AllowanceName=@AllowanceName, 
                                AllowanceCode=@AllowanceCode, 
                                IsInsuranceIncluded=@IsInsuranceIncluded,
                                ApplyScopeID=@ApplyScopeID, 
                                IsActive=@IsActive
                            WHERE AllowanceTypeID=@AllowanceTypeID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@AllowanceName", allowanceName);
                        cmd.Parameters.AddWithValue("@AllowanceCode", allowanceCode);
                        cmd.Parameters.AddWithValue("@ApplyScopeID", applyScopeID);
                        cmd.Parameters.AddWithValue("@IsInsuranceIncluded", isInsuranceIncluded);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> deleteAllowanceTypeAsync(int allowanceTypeID)
        {
            string query = "DELETE FROM AllowanceType WHERE AllowanceTypeID=@AllowanceTypeID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetDepartmentAllowanceAsybc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM DepartmentAllowance";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateDepartmentAllowanceAsync(int departmentAllowanceID, int departmentID, int amount, int allowanceTypeID, string note)
        {
            string query = @"UPDATE DepartmentAllowance SET 
                                DepartmentID=@DepartmentID, 
                                AllowanceTypeID=@AllowanceTypeID,
                                Amount=@Amount, 
                                Note=@Note
                            WHERE DepartmentAllowanceID=@DepartmentAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentAllowanceID", departmentAllowanceID);
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertDepartmentAllowanceAsync(int departmentID, int amount, int allowanceTypeID, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO DepartmentAllowance (
                                        DepartmentID, AllowanceTypeID, Amount, Note
                                    )
                                    OUTPUT INSERTED.DepartmentAllowanceID
                                    VALUES (
                                        @DepartmentID, @AllowanceTypeID, @Amount, @Note
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentID", departmentID);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);

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

        public async Task<bool> deleteDepartmentAllowanceAsync(int departmentAllowanceID)
        {
            string query = "DELETE FROM DepartmentAllowance WHERE DepartmentAllowanceID=@DepartmentAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentAllowanceID", departmentAllowanceID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetPositionAllowanceAsybc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM PositionAllowance";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updatePositionAllowanceAsync(int positionAllowanceID, int positionID, int amount, int allowanceTypeID, string note)
        {
            string query = @"UPDATE PositionAllowance SET 
                                PositionID=@PositionID, 
                                AllowanceTypeID=@AllowanceTypeID,
                                Amount=@Amount, 
                                Note=@Note
                            WHERE PositionAllowanceID=@PositionAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PositionAllowanceID", positionAllowanceID);
                        cmd.Parameters.AddWithValue("@PositionID", positionID);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertPositionAllowanceAsync(int positionID, int amount, int allowanceTypeID, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO PositionAllowance (
                                        PositionID, AllowanceTypeID, Amount, Note
                                    )
                                    OUTPUT INSERTED.PositionAllowanceID
                                    VALUES (
                                        @PositionID, @AllowanceTypeID, @Amount, @Note
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@PositionID", positionID);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);

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

        public async Task<bool> deletePositionAllowanceAsync(int positionAllowanceID)
        {
            string query = "DELETE FROM PositionAllowance WHERE PositionAllowanceID=@PositionAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PositionAllowanceID", positionAllowanceID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetEmployeeAllowanceAsybc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM EmployeeAllowance";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateEmployeeAllowanceAsync(int employeeAllowanceID, string employeeCode, int amount, int allowanceTypeID, string note)
        {
            string query = @"UPDATE EmployeeAllowance SET 
                                EmployeeCode=@EmployeeCode, 
                                AllowanceTypeID=@AllowanceTypeID,
                                Amount=@Amount, 
                                Note=@Note
                            WHERE EmployeeAllowanceID=@EmployeeAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeAllowanceID", employeeAllowanceID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertEmployeeAllowanceAsync(string employeeCode, int amount, int allowanceTypeID, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO EmployeeAllowance (
                                        EmployeeCode, AllowanceTypeID, Amount, Note
                                    )
                                    OUTPUT INSERTED.EmployeeAllowanceID
                                    VALUES (
                                        @EmployeeCode, @AllowanceTypeID, @Amount, @Note
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);

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

        public async Task<bool> deleteEmployeeAllowanceAsync(int employeeAllowanceID)
        {
            string query = "DELETE FROM EmployeeAllowance WHERE EmployeeAllowanceID=@EmployeeAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeAllowanceID", employeeAllowanceID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetMonthlyAllowanceAsybc(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM MonthlyAllowance WHERE Month = @Month AND Year = @Year";

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

        public async Task<bool> updateMonthlyAllowanceAsync(int monthlyAllowanceID, string employeeCode, int month, int year, int amount, int allowanceTypeID, string note)
        {
            string query = @"UPDATE MonthlyAllowance SET 
                                EmployeeCode=@EmployeeCode, 
                                AllowanceTypeID=@AllowanceTypeID,
                                Month=@Month, 
                                Year=@Year, 
                                Amount=@Amount, 
                                Note=@Note
                            WHERE MonthlyAllowanceID=@MonthlyAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@MonthlyAllowanceID", monthlyAllowanceID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Month", month);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertMonthlyAllowanceAsync(string employeeCode, int month, int year, int amount, int allowanceTypeID, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO MonthlyAllowance (EmployeeCode, AllowanceTypeID, Month, Year, Amount, Note)
                                    OUTPUT INSERTED.MonthlyAllowanceID
                                    VALUES (@EmployeeCode, @AllowanceTypeID, @Month, @Year, @Amount, @Note)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@AllowanceTypeID", allowanceTypeID);
                        cmd.Parameters.AddWithValue("@Month", month);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);

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

        public async Task<bool> deleteMonthlyAllowanceAsync(int monthlyAllowanceID)
        {
            string query = "DELETE FROM MonthlyAllowance WHERE MonthlyAllowanceID=@MonthlyAllowanceID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@MonthlyAllowanceID", monthlyAllowanceID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetSalaryGradeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM SalaryGrade";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<bool> updateSalaryGradeAsync(int salaryGradeID, string gradeName, int salary, string note, bool isActive)
        {
            string query = @"UPDATE SalaryGrade SET 
                                GradeName=@GradeName,
                                Salary=@Salary, 
                                Note=@Note, 
                                IsActive=@IsActive                             
                            WHERE SalaryGradeID=@SalaryGradeID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SalaryGradeID", salaryGradeID);
                        cmd.Parameters.AddWithValue("@GradeName", gradeName);
                        cmd.Parameters.AddWithValue("@Salary", salary);
                        cmd.Parameters.AddWithValue("@Note", note);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertSalaryGradeAsync(string gradeName, int salary, string note, bool isActive)
        {
            int newId = -1;

            string query = @"INSERT INTO SalaryGrade (GradeName, Salary, Note, IsActive) 
                                OUTPUT INSERTED.SalaryGradeID
                                VALUES (@GradeName, @Salary, @Note, @IsActive)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@GradeName", gradeName);
                        cmd.Parameters.AddWithValue("@Salary", salary);
                        cmd.Parameters.AddWithValue("@Note", note);
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

        public async Task<bool> deleteSalaryGradeAsync(int ID)
        {
            string query = "DELETE FROM SalaryGrade WHERE SalaryGradeID=@SalaryGradeID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SalaryGradeID", ID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetEmployeeSalaryInfoAsybc()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM EmployeeSalaryInfo";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        public async Task<DataTable> GetSalaryHistoryAsyc(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(qlnvHis_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM EmployeeSalaryHistory WHERE Month=@Month AND Year=@Year";
                using (SqlCommand cmd = new SqlCommand(query, con)) {
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

        public async Task<int> insertEmployeeSalaryInfoAsync(string employeeCode, int month, int year, int baseSalary, int insuranceBaseSalary, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO EmployeeSalaryInfo (EmployeeCode, Month, Year, BaseSalary, InsuranceBaseSalary, Note, ActionBy)
                                    OUTPUT INSERTED.SalaryInfoID
                                    VALUES (@EmployeeCode, @Month, @Year, @BaseSalary, @InsuranceBaseSalary, @Note, @ActionBy)";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@Month", month);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@BaseSalary", baseSalary);
                        cmd.Parameters.AddWithValue("@InsuranceBaseSalary", insuranceBaseSalary);
                        cmd.Parameters.AddWithValue("@Note", note);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);

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

        public async Task<bool> deleteEmployeeSalaryInfoAsync(int salaryInfoID)
        {
            string query = "DELETE FROM EmployeeSalaryInfo WHERE SalaryInfoID=@SalaryInfoID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SalaryInfoID", salaryInfoID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> updateEmployeeBHAsync(string EmployeeCode, string SocialInsuranceNumber, string HealthInsuranceNumber)
        {
            string query = @"UPDATE Employee SET 
                                SocialInsuranceNumber=@SocialInsuranceNumber, HealthInsuranceNumber=@HealthInsuranceNumber
                            WHERE EmployeeCode=@EmployeeCode";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SocialInsuranceNumber", SocialInsuranceNumber);
                        cmd.Parameters.AddWithValue("@HealthInsuranceNumber", HealthInsuranceNumber);
                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> updateEmployeeBankAsync(string EmployeeCode, string BankName, string BankBranch, string BankAccountNumber, string BankAccountHolder)
        {
            string query = @"UPDATE Employee SET 
                                BankName=@BankName, BankBranch=@BankBranch, BankAccountNumber=@BankAccountNumber, BankAccountHolder=@BankAccountHolder
                            WHERE EmployeeCode=@EmployeeCode";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BankName", BankName);
                        cmd.Parameters.AddWithValue("@BankBranch", BankBranch);
                        cmd.Parameters.AddWithValue("@BankAccountNumber", BankAccountNumber);
                        cmd.Parameters.AddWithValue("@BankAccountHolder", BankAccountHolder);
                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> updateEmployeeWorkAsync(string EmployeeCode, int PositionID, int DepartmentID, int ContractTypeID)
        {
            string query = @"UPDATE Employee SET 
                                PositionID=@PositionID, DepartmentID=@DepartmentID, ContractTypeID=@ContractTypeID
                            WHERE EmployeeCode=@EmployeeCode";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PositionID", PositionID);
                        cmd.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                        cmd.Parameters.AddWithValue("@ContractTypeID", ContractTypeID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetDeductionTypeAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT DeductionTypeCode, DeductionTypeName FROM DeductionType";

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

        public async Task<DataTable> GetEmployeeDeductionAsync(int month, int year, string deductionCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT EmployeeDeductionID, EmployeeCode, DeductionDate, Amount, Note, updateHistory
                         FROM EmployeeDeduction
                         WHERE DeductionMonth = @Month AND DeductionYear = @Year AND DeductionTypeCode = @DeductionTypeCode";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@DeductionTypeCode", deductionCode);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }
        public async Task<DataTable> GetEmployeeDeductionAsync(int year, string deductionCode)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT EmployeeDeductionID, EmployeeCode, DeductionDate, Amount, Note, updateHistory
                         FROM EmployeeDeduction
                         WHERE DeductionYear = @Year AND DeductionTypeCode = @DeductionTypeCode";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@DeductionTypeCode", deductionCode);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }


        public async Task<bool> updateEmployeeDeductionAsync(int EmployeeDeductionID, string EmployeeCode, DateTime DeductionDate, int amount, string note)
        {
            string query = @"UPDATE EmployeeDeduction SET 
                                EmployeeCode=@EmployeeCode,
                                DeductionMonth=@DeductionMonth,
                                DeductionYear=@DeductionYear,
                                DeductionDate=@DeductionDate,
                                Amount=@Amount,
                                Note=@Note
                            WHERE EmployeeDeductionID=@EmployeeDeductionID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeDeductionID", EmployeeDeductionID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        cmd.Parameters.AddWithValue("@DeductionMonth", DeductionDate.Month);
                        cmd.Parameters.AddWithValue("@DeductionYear", DeductionDate.Year);
                        cmd.Parameters.AddWithValue("@DeductionDate", DeductionDate.Date);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<int> insertEmployeeDeductionAsync(string EmployeeCode, string DeductionTypeCode, DateTime DeductionDate, int amount, string note)
        {
            int newId = -1;

            string insertQuery = @" INSERT INTO EmployeeDeduction (
                                        EmployeeCode, DeductionTypeCode, DeductionMonth, DeductionYear, DeductionDate, Amount, Note
                                    )
                                    OUTPUT INSERTED.EmployeeDeductionID
                                    VALUES (
                                        @EmployeeCode, @DeductionTypeCode, @DeductionMonth, @DeductionYear, @DeductionDate, @Amount, @Note
                                    )";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    // 2️⃣ Insert và lấy ID mới
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        cmd.Parameters.AddWithValue("@DeductionTypeCode", DeductionTypeCode);
                        cmd.Parameters.AddWithValue("@DeductionMonth", DeductionDate.Month);
                        cmd.Parameters.AddWithValue("@DeductionYear", DeductionDate.Year);
                        cmd.Parameters.AddWithValue("@DeductionDate", DeductionDate.Date);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note);

                        object result = await cmd.ExecuteScalarAsync();
                        if (result != null)
                            newId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                // ✅ Tùy chọn: log lỗi ra console hoặc file
                Console.WriteLine($"[InsertEmployeeDeductionAsync] Error: {ex.Message}");
            }
            return newId;
        }

        public async Task<bool> UpsertEmployeeDeductionAsync(string EmployeeCode, string DeductionTypeCode, DateTime DeductionDate, int amount, string note)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("sp_UpsertEmployeeDeduction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@EmployeeCode", EmployeeCode);
                        cmd.Parameters.AddWithValue("@DeductionTypeCode", DeductionTypeCode);
                        cmd.Parameters.AddWithValue("@DeductionMonth", DeductionDate.Month);
                        cmd.Parameters.AddWithValue("@DeductionYear", DeductionDate.Year);
                        cmd.Parameters.AddWithValue("@DeductionDate", DeductionDate.Date);
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@Note", note ?? (object)DBNull.Value);

                        // ExecuteScalarAsync chỉ để trigger SP, không cần lấy giá trị
                        await cmd.ExecuteScalarAsync();
                    }
                }

                return true; // Thành công
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpsertEmployeeDeductionAsync] Error: {ex.Message}");
                return false; // Lỗi
            }
        }


        public async Task<bool> deleteEmployeeDeductionAsync(int EmployeeDeductionID)
        {
            string query = "DELETE FROM EmployeeDeduction WHERE EmployeeDeductionID=@EmployeeDeductionID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeDeductionID", EmployeeDeductionID);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public async Task<DataTable> GetEmployeeAllowanceAsync(int month, int year)
        {
            DataTable dt = new DataTable();

            bool isLock = await SQLStore_QLNS.Instance.IsSalaryLockAsync(month, year);
            using (SqlConnection con = new SqlConnection(isLock ? qlnvHis_conStr() : ql_NhanSu_conStr()))
            {
                await con.OpenAsync();
                string spStr = isLock ? "sp_GetEmployeeAllowanceHistory_ByMonth" : "sp_GetEmployeeAllowance";
                using (SqlCommand cmd = new SqlCommand(spStr, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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

        public async Task<bool> UpsertSaveEmployeeAllowanceHistoryBatchAsync(List<(string EmployeeCode, string scopeCode, int allowanceTypeID, string AllowanceName, bool IsInsuranceIncluded, int Amount, int Month, int Year)> data)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ScopeCode", typeof(string));
                dt.Columns.Add("AllowanceTypeID", typeof(int));
                dt.Columns.Add("EmployeeCode", typeof(string));                
                dt.Columns.Add("AllowanceName", typeof(string));
                dt.Columns.Add("IsInsuranceIncluded", typeof(bool));
                dt.Columns.Add("Amount", typeof(int));
                dt.Columns.Add("Month", typeof(int));
                dt.Columns.Add("Year", typeof(int));

                foreach (var item in data)
                {
                    dt.Rows.Add(item.scopeCode, item.allowanceTypeID, item.EmployeeCode, item.AllowanceName, item.IsInsuranceIncluded, item.Amount, item.Month, item.Year);
                }

                using (SqlConnection conn = new SqlConnection(qlnvHis_conStr()))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_SaveEmployeeAllowanceHistory_Batch", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@AllowanceData", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "EmployeeAllowanceHistoryTableType";

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("====Lỗi khi cập nhật: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> UpsertEmployeeSalaryHistoryAsync(List<(string EmployeeCode, string ContractTypeName, int Month, int Year, decimal BaseSalary, decimal NetSalary, decimal NetInsuranceSalary,
            decimal InsuranceAllowance, decimal NonInsuranceAllowance, decimal OvertimeSalary, decimal LeaveSalary, decimal DeductionAmount, bool IsInsuranceRefund)> data)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("EmployeeCode", typeof(string));
                dt.Columns.Add("ContractTypeName", typeof(string));
                dt.Columns.Add("Month", typeof(int));
                dt.Columns.Add("Year", typeof(int));
                dt.Columns.Add("BaseSalary", typeof(decimal));
                dt.Columns.Add("NetSalary", typeof(decimal));
                dt.Columns.Add("NetInsuranceSalary", typeof(decimal));
                dt.Columns.Add("InsuranceAllowance", typeof(decimal));
                dt.Columns.Add("NonInsuranceAllowance", typeof(decimal));
                dt.Columns.Add("OvertimeSalary", typeof(decimal));
                dt.Columns.Add("LeaveSalary", typeof(decimal));
                dt.Columns.Add("DeductionAmount", typeof(decimal));
                dt.Columns.Add("IsInsuranceRefund", typeof(bool));


                foreach (var item in data)
                {
                    dt.Rows.Add(item.EmployeeCode, item.ContractTypeName, item.Month, item.Year, item.BaseSalary, item.NetSalary, item.NetInsuranceSalary, item.InsuranceAllowance,
                         item.NonInsuranceAllowance, item.OvertimeSalary, item.LeaveSalary, item.DeductionAmount, item.IsInsuranceRefund);
                }

                using (SqlConnection conn = new SqlConnection(qlnvHis_conStr()))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_UpsertEmployeeSalaryHistory_Batch", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@SalaryData", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "EmployeeSalaryHistoryTableType";

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

        public async Task<DataTable> GetSalarySummaryByYearAsync(int year)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(qlnvHis_conStr()))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("sp_GetSalarySummaryByYear", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public async Task<bool> DeleteAttendanceByMonthYearAsync(int month, int year)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteAttendanceByMonthYear", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Month", month);
                        cmd.Parameters.AddWithValue("@Year", year);
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error DeleteAttendanceByMonthYearAsync: {ex.Message}");
            }

            return false;
        }

        public async Task InsertEmployeeDeductionLogAsync(string employeeCode, string deductionTypeCode, string description, DateTime deductionDate, int amount, string note)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertEmployeeDeductionLog", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@DeductionTypeCode", deductionTypeCode);
                        cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DeductionDate", deductionDate.Date);
                        cmd.Parameters.AddWithValue("@Amount", (object)amount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Note", (object)note ?? DBNull.Value);
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

        public async Task<DataTable> GetEmployeeDeductionLogAsync(int month, int year, string code)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM EmployeeDeduction_log WHERE MONTH(DeductionDate) = @Month AND YEAR(DeductionDate) = @Year AND DeductionTypeCode = @Code;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@Code", code);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }

        public async Task InsertMonthlyAllowanceLogAsync(string employeeCode, string allowanceName, string description, int month, int year, int amount, string note)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertMonthlyAllowanceLog", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@AllowanceName", allowanceName);
                        cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Month", month);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@Amount", (object)amount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Note", (object)note ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertMonthlyAllowanceLogAsync: {ex.Message}");
            }
        }

        public async Task<DataTable> GetMonthlyAllowanceLogAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM MonthlyAllowance_Log WHERE Month = @Month AND Year = @Year";

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

        public async Task InsertEmployeeAllowanceLogAsync(string employeeCode, string allowanceName, string description, int amount, string note)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertEmployeeAllowanceLog", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@AllowanceName", allowanceName);
                        cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Amount", (object)amount ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Note", (object)note ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertMonthlyAllowanceLogAsync: {ex.Message}");
            }
        }

        public async Task<DataTable> GetEmployeeAllowanceLogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM EmployeeAllowance_Log";

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

        public async Task InsertLeaveAttandanceLogAsync(string employeeCode, string leaveName, string description, DateTime dateOff, decimal leaveHour, string note)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertLeaveAttendanceLog", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@LeaveName", leaveName);
                        cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DateOff", (object)dateOff ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@LeaveHour", (object)leaveHour ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Note", (object)note ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertLeaveAttandanceLogAsync: {ex.Message}");
            }
        }

        public async Task<DataTable> GetLeaveAttandanceLogAsync(int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM LeaveAttendance_Log WHERE YEAR(DateOff) = @Year";

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

        public async Task InsertOvertimeAttandanceLogAsync(string employeeCode, string OverName, string description, DateTime workDate, TimeSpan startTime, TimeSpan endTime, string note)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("InsertOvertimeAttendance_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@OvertimeTypeName", OverName);
                        cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@WorkDate", (object)workDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@StartTime", (object)startTime ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EndTime", (object)endTime ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Note", (object)note ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertOvertimeAttandanceLogAsync: {ex.Message}");
            }
        }

        public async Task InsertOvertimeAttandance_MultiEmployee_LogAsync(List<string> employeeCodes, string overName, string description, DateTime workDate, TimeSpan startTime, TimeSpan endTime, string note)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(
                        "InsertOvertimeAttendance_MultiEmployee_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // 1️⃣ Tạo DataTable cho TVP
                        DataTable dtEmployeeCodes = new DataTable();
                        dtEmployeeCodes.Columns.Add("EmployeeCode", typeof(string));

                        foreach (string code in employeeCodes)
                        {
                            dtEmployeeCodes.Rows.Add(code);
                        }

                        // 2️⃣ TVP parameter
                        SqlParameter tvpParam = cmd.Parameters.AddWithValue("@EmployeeCodes", dtEmployeeCodes);
                        tvpParam.SqlDbType = SqlDbType.Structured;
                        tvpParam.TypeName = "dbo.EmployeeCode_TVP";

                        // 3️⃣ Các parameter dùng chung
                        cmd.Parameters.Add("@OvertimeTypeName", SqlDbType.NVarChar, 100).Value = overName;
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value = (object)description ?? DBNull.Value;
                        cmd.Parameters.Add("@WorkDate", SqlDbType.Date).Value = workDate.Date;
                        cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = startTime;
                        cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = endTime;
                        cmd.Parameters.Add("@Note", SqlDbType.NVarChar, 255).Value = (object)note ?? DBNull.Value;
                        cmd.Parameters.Add("@ActionBy", SqlDbType.NVarChar, 50).Value = UserManager.Instance.fullName;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error InsertOvertimeAttendance_MultiEmployee_LogAsync: {ex.Message}");
                throw; // hoặc bỏ throw nếu không muốn bubble error
            }
        }

        public async Task<DataTable> GetOvertimeAttandanceLogAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM OvertimeAttendance_Log WHERE YEAR(WorkDate) = @Year AND MONTH(WorkDate) = @Month";

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

        public async Task<bool> InsertAttendanceLogListAsync(List<(string EmployeeCode, string Description, DateTime WorkDate, decimal WorkingHours, string Note , string ActionBy)> albData)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("EmployeeCode", typeof(string));
                dt.Columns.Add("Description", typeof(string));
                dt.Columns.Add("WorkDate", typeof(DateTime));
                dt.Columns.Add("WorkingHours", typeof(decimal));
                dt.Columns.Add("Note", typeof(string));
                dt.Columns.Add("ActionBy", typeof(string));


                foreach (var item in albData)
                {
                    dt.Rows.Add(item.EmployeeCode, item.Description, item.WorkDate.Date, item.WorkingHours, item.Note, item.ActionBy);
                }

                using (SqlConnection conn = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertAttendanceLogList", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var param = cmd.Parameters.AddWithValue("@Logs", dt);
                        param.SqlDbType = SqlDbType.Structured;
                        param.TypeName = "AttendanceLogType";

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

        public async Task<DataTable> GetAttendanceLogAsync(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Attendance_Log WHERE YEAR(WorkDate) = @Year AND MONTH(WorkDate) = @Month";

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

        public async Task InsertEmployeesLogAsync(string employeeCode, string oldvalue, string newValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("Insert_Employee_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@OldValue", (object)oldvalue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)newValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertEmployeesLogAsync: {ex.Message}");
            }
        }

        public async Task<DataTable> GetEmployeeLogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Employee_Log";

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

        public async Task InsertEmployeesInsuranceLogAsync(string employeeCode, string oldvalue, string newValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertEmployeeInsurance_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@OldValue", (object)oldvalue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)newValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertEmployeesInsuranceLogAsync: {ex.Message}");
            }
        }

        public async Task<DataTable> GetEmployeeInsuranceLogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM EmployeeInsurance_Log";

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

        public async Task InsertEmployeesBankLogAsync(string employeeCode, string oldvalue, string newValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertEmployeeBank_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@OldValue", (object)oldvalue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)newValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertEmployeesBankLogAsync: {ex.Message}");
            }
        }

        public async Task<DataTable> GetEmployeeBankLogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM EmployeeBank_Log";

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

        public async Task InsertEmployees_POS_DEP_CON_LogAsync(string employeeCode, string oldvalue, string newValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertEmployee_POS_DEP_CON_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@OldValue", (object)oldvalue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)newValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertEmployees_POS_DEP_CON_LogAsync: {ex.Message}");
            }
        }

        public async Task<DataTable> GetEmployee_POS_DEP_CON_LogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Employee_POS_DEP_CON_Log";

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

        public async Task InsertEmployeesSalary_LogAsync(string employeeCode, string descrition)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertEmployeeSalary_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@Descrition", (object)descrition ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertEmployeesSalary_LogAsync: {ex.Message}");
            }
        }

        public async Task<DataTable> GetEmployeeSalary_LogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM EmployeeSalary_Log";

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

        public async Task InsertSalaryGrade_LogAsync(int gradeID, string oldValue, string newValue)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_InsertSalaryGrade_Log", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@GradeID", gradeID);
                        cmd.Parameters.AddWithValue("@OldValue", (object)oldValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@NewValue", (object)newValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ActionBy", UserManager.Instance.fullName);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error InsertEmployeesSalary_LogAsync: {ex.Message}");
            }
        }

        public async Task<DataTable> GetSalaryGrade_LogAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_NhanSu_Log_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM SalaryGrade_Log";

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
    }
}

