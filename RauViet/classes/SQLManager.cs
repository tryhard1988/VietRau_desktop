
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;

namespace RauViet.classes
{
    public sealed class SQLManager
    {
        private static SQLManager ins = null;
        private static readonly object padlock = new object();

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

        public string ql_User_conStr(){ return "Server=192.168.1.8,1433;Database=QL_User;User Id=ql_user;Password=A7t#kP2x;";}       
        public string salaryLock_conStr() { return "Server=192.168.1.8,1433;Database=SalaryLock;User Id=salary_lock;Password=A7t#kP2x;"; }        

        public async Task<string> GetPasswordHashFromDatabase(string username)
        {
            string hash = "";

            using (SqlConnection con = new SqlConnection(ql_User_conStr()))
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

        public async Task<DataTable> GetRolesAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_User_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT 
                                    r.RoleID, 
                                    r.RoleName, 
                                    r.RoleGroupID,
                                    g.RoleGroupName,
                                    g.Priority
                                FROM Roles r
                                LEFT JOIN RoleGroup g ON r.RoleGroupID = g.RoleGroupID
                                ORDER BY g.Priority, r.RoleGroupID, r.RoleID;";

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
            using (SqlConnection con = new SqlConnection(ql_User_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT 
                                    u.UserID,
                                    u.Username,
                                    u.PasswordHash,
                                    u.EmployeeCode,
                                    u.IsActive,
                                    STRING_AGG(r.RoleID, ',') AS RoleIDs
                                FROM Users u
                                LEFT JOIN UserRoles ur ON u.UserID = ur.UserID
                                LEFT JOIN Roles r ON ur.RoleID = r.RoleID
                                GROUP BY 
                                    u.UserID, u.Username, u.PasswordHash, u.EmployeeCode, 
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

        public async Task<bool> updateUserAsync(int userID, string userName, string password, string employeeCode, Boolean isActive, List<int> roleIDs)
        {
            string query = @"UPDATE Users SET 
                                Username=@Username, PasswordHash=@PasswordHash, EmployeeCode=@EmployeeCode, IsActive=@IsActive 
                            WHERE UserID=@UserID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Username", userName);
                        cmd.Parameters.AddWithValue("@PasswordHash", password);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
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

        public async Task<bool> updateUser_notPasswordAsync(int userID, string userName, string employeeCode, Boolean isActive, List<int> roleIDs)
        {
            string query = @"UPDATE Users SET 
                                Username=@Username, EmployeeCode=@EmployeeCode, IsActive=@IsActive 
                            WHERE UserID=@UserID";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Username", userName);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
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

        public async Task<int> insertUserDataAsync(string userName, string password, string employeeCode, Boolean isActive, List<int> roleIDs)
        {
            int newId = -1;
            string query = @"INSERT INTO Users (Username, PasswordHash, EmployeeCode, IsActive) 
                             OUTPUT INSERTED.UserID
                            VALUES (@Username, @PasswordHash, @EmployeeCode, @IsActive)";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", userName);
                        cmd.Parameters.AddWithValue("@PasswordHash", password);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
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
                using (SqlConnection con = new SqlConnection(ql_User_conStr()))
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

        public async Task<DataRow> GetUserRoleAsync(string userName)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ql_User_conStr()))
            {
                await con.OpenAsync();

                string query = @"SELECT 
                                    u.EmployeeCode,
                                    STRING_AGG(r.RoleCode, ',') AS RoleCodes
                                FROM Users u
                                LEFT JOIN UserRoles ur ON u.UserID = ur.UserID
                                LEFT JOIN Roles r ON ur.RoleID = r.RoleID
                                WHERE u.Username = @Username
                                    AND u.IsActive = 1
                                GROUP BY u.EmployeeCode;";

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

                using (SqlConnection con = new SqlConnection(ql_User_conStr()))
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

        public async Task<DataTable> GetSalaryLockAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(salaryLock_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM SalaryLock";

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

        public async Task<bool> UpsertLockSalaryAsync(int Month, int Year, bool IsLocked)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(salaryLock_conStr()))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_SetSalaryLock", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Month", Month);
                        cmd.Parameters.AddWithValue("@Year", Year);
                        cmd.Parameters.AddWithValue("@IsLocked", IsLocked);

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

        public async Task updateMachineInfoWhenLoginAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return;

            string computerName = Environment.MachineName;
            string ipAddress = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())
                                              .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                                              .ToString();

            string machineInfo = $"{computerName} - {ipAddress}";

            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr()))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateUserMachineInfo", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", userName);
                        cmd.Parameters.AddWithValue("@MachineInfo", machineInfo);
                        await cmd.ExecuteNonQueryAsync();

                        UserManager.Instance.machineInfo = machineInfo;
                    }
                }
            }
            catch { }
        }

        public async Task<bool> HaveOtherComputerLoginAsync()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr()))
                {
                    await con.OpenAsync();
                    string query = "SELECT MachineInfo FROM Users WHERE UserName = @UserName";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserName", UserManager.Instance.userName);
                        var result = await cmd.ExecuteScalarAsync();

                        if (result != null && result.ToString() != UserManager.Instance.machineInfo)
                        {
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public async Task AutoDeleteLoginHistoryAsync()
        {
            string query = "DELETE FROM UserLoginHistory WHERE  LoginTime < DATEADD(MONTH, -3, GETDATE());";
            try
            {
                using (SqlConnection con = new SqlConnection(ql_User_conStr()))
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
                Console.WriteLine($"Error deleting login history: {ex.Message}");
            }
        }

        public async Task<DataTable> GetHistoryLoginAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ql_User_conStr()))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM UserLoginHistory WHERE UserName = @UserName ORDER BY Id DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", UserManager.Instance.userName);
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

