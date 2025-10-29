using System;
using System.Data;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using RauViet.classes;

namespace RauViet.ui
{
    public partial class EditExcelFormEmployee : Form
    {
        public EditExcelFormEmployee()
        {
            InitializeComponent();
            dataGV.AllowUserToAddRows = false;

            this.WindowState = FormWindowState.Maximized;
            openFileExcel_mi.Click += openFileExcel_mi_Click;
            save_mi.Click += save_mi_Click;
            importToSQL_mi.Click += importToSQL_mi_Click;

            position_btn.Click += position_btn_click;
            department_btn.Click += department_btn_click;
            Contract_btn.Click += Contract_btn_click;
            gender_btn.Click += gender_btn_click;
        }

        private void EmployeeID_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            if (!dt.Columns.Contains("EmployeeID"))
            {
                dt.Columns.Add("EmployeeID", typeof(int)); // typeof(int), typeof(DateTime)... tùy loại
            }
            dataGV.DataSource = dt;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.Cells["EmployeeID1"].Value != null)
                {
                    string code = row.Cells["EmployeeID1"].Value.ToString().Trim();

                    if (code.Length > 2)
                    {
                        string numberPart = code.Substring(2); // cắt 2 ký tự đầu
                        if (int.TryParse(numberPart, out int employeeNumber))
                        {
                            // ✅ Gán lại hoặc xử lý theo ý bạn
                            row.Cells["EmployeeID"].Value =Convert.ToInt32(employeeNumber);
                        }
                    }
                }
            }

            if (dt.Columns.Contains("EmployeeID1"))
            {
                dt.Columns.Remove("EmployeeID1");
            }
        }

        private void gender_btn_click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.Cells["Gender"].Value != null)
                {
                    string positionName = row.Cells["Gender"].Value.ToString();

                    switch (positionName)
                    {
                        case "Nam":
                            row.Cells["Gender"].Value = true;
                            break;
                        default:
                            row.Cells["Gender"].Value = false;
                            break;
                    }
                }
            }
        }

        private void Contract_btn_click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.Cells["ContractTypeID"].Value != null)
                {
                    string positionName = row.Cells["ContractTypeID"].Value.ToString();

                    switch (positionName)
                    {
                        case "Chính thức":
                            row.Cells["ContractTypeID"].Value = 1;
                            break;
                        case "Thử việc":
                            row.Cells["ContractTypeID"].Value = 2;
                            break;
                        default:
                            row.Cells["ContractTypeID"].Value = 3;
                            break;

                    }
                }
            }
        }

        private void position_btn_click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.Cells["PositionID"].Value != null)
                {
                    string positionName = row.Cells["PositionID"].Value.ToString().Replace(" ", "").ToLower();

                    switch (positionName)
                    {
                        case "côngnhân":
                            row.Cells["PositionID"].Value = 22;
                            break;
                        case "tổtrưởng":
                            row.Cells["PositionID"].Value = 17;
                            break;
                        case "nhânviênvănphòng":
                            row.Cells["PositionID"].Value = 21;
                            break;
                        case "bảovệ":
                            row.Cells["PositionID"].Value = 24;
                            break;
                        case "nhânviênIT":
                            row.Cells["PositionID"].Value = 19;
                            break;
                        case "nhânviênmedia":
                            row.Cells["PositionID"].Value = 20;
                            break;
                        case "giámđốcđiềuhành":
                            row.Cells["PositionID"].Value = 13;
                            break;
                        case "nhânviênkỹthuật":
                            row.Cells["PositionID"].Value = 17;
                            break;
                        case "quảnđốc":
                            row.Cells["PositionID"].Value = 16;
                            break;
                        case "quảnlýxưởng":
                            row.Cells["PositionID"].Value = 15;
                            break;
                        case "trưởngphòng":
                            row.Cells["PositionID"].Value = 14;
                            break;
                        case "tàixế":
                            row.Cells["PositionID"].Value = 23;
                            break;
                        case "nhânviên":
                            row.Cells["PositionID"].Value = 23;
                            break;
                        default:
                            row.Cells["PositionID"].Value = 27; // mặc định
                            break;
                    }
                }
            }
        }

        private void department_btn_click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.Cells["DepartmentID"].Value != null)
                {
                    string positionName = row.Cells["DepartmentID"].Value.ToString();

                    switch (positionName)
                    {
                        case "Hành chính - Nhân sự":
                            row.Cells["DepartmentID"].Value = 23;
                            break;
                        case "Tổ A":
                            row.Cells["DepartmentID"].Value = 24;
                            break;
                        case "Thu hoạch, sơ chế":
                            row.Cells["DepartmentID"].Value = 26;
                            break;
                        case "Nhà ươm - Thủy canh":
                            row.Cells["DepartmentID"].Value = 24;
                            break;
                        case "Chế biến, đóng gói":
                            row.Cells["DepartmentID"].Value = 28;
                            break;
                        case "Farm VR":
                            row.Cells["DepartmentID"].Value = 30;
                            break;
                        case "Tổ B":
                            row.Cells["DepartmentID"].Value = 25;
                            break;
                        case "Đóng thùng":
                            row.Cells["DepartmentID"].Value = 29;
                            break;
                        case "Farm BT":
                            row.Cells["DepartmentID"].Value = 31;
                            break;
                        case "Xưởng":
                            row.Cells["DepartmentID"].Value = 33;
                            break;
                        case "Phòng kỹ thuật trồng trọt":
                            row.Cells["DepartmentID"].Value = 32;
                            break;
                        default:
                            row.Cells["DepartmentID"].Value = 36;
                            break;
                    }
                }
            }
        }

       

        private void save_mi_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Files|*.xlsx";
                sfd.Title = "Chọn nơi lưu file Excel";
                sfd.FileName = "Export.xlsx"; // tên mặc định

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveDataGridViewToExcel(dataGV, sfd.FileName);
                    MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void openFileExcel_mi_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file Excel";
                ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*";
                ofd.Multiselect = false; // chỉ cho chọn 1 file

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    // TODO: gọi hàm đọc excel -> DataGridView
                    LoadExcelClosedXML(filePath);
                }
            }
        }

        private void LoadExcelClosedXML(string filePath)
        {
            var dt = new DataTable();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheets.First(); // lấy sheet đầu tiên
                bool firstRow = true;

                foreach (var row in worksheet.RowsUsed())
                {
                    var cellValues = row.Cells().Select(c => c.GetValue<string>().Trim()).ToList();

                    if (firstRow)
                    {
                        // Tạo cột từ header
                        foreach (var colName in cellValues)
                        {
                            if (!string.IsNullOrWhiteSpace(colName))
                                dt.Columns.Add(colName);
                            else
                                dt.Columns.Add("Column" + dt.Columns.Count); // tránh null header
                        }
                        firstRow = false;
                    }
                    else
                    {
                        // Nếu số cell < số cột thì bổ sung cho đủ
                        while (cellValues.Count < dt.Columns.Count)
                            cellValues.Add("");

                        // Nếu số cell > số cột thì cắt bớt
                        if (cellValues.Count > dt.Columns.Count)
                            cellValues = cellValues.Take(dt.Columns.Count).ToList();

                        // Kiểm tra dòng có toàn ô trống không
                        bool allEmpty = cellValues.All(string.IsNullOrWhiteSpace);
                        if (!allEmpty)
                            dt.Rows.Add(cellValues.ToArray());
                    }
                }
            }

            dataGV.DataSource = dt;
        }

        private void SaveDataGridViewToExcel(DataGridView dgv, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                int colIndex = 1;

                // Ghi header
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    worksheet.Cell(1, colIndex).Value = col.HeaderText;
                    worksheet.Cell(1, colIndex).Style.Font.Bold = true;
                    worksheet.Cell(1, colIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    colIndex++;
                }

                // Ghi dữ liệu
                int rowIndex = 2;
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue; // bỏ dòng trống cuối cùng

                    colIndex = 1;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        worksheet.Cell(rowIndex, colIndex).Value = cell.Value?.ToString();
                        colIndex++;
                    }
                    rowIndex++;
                }

                // Auto fit cột
                worksheet.Columns().AdjustToContents();

                // Lưu file
                workbook.SaveAs(filePath);
            }
        }

        private DataTable GetDataTableFromDataGridView(DataGridView dgv)
        {
            var dt = new DataTable();

            // Tạo cột
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                dt.Columns.Add(col.HeaderText, typeof(string));
            }

            // Thêm dữ liệu
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow)
                {
                    var values = new object[dgv.Columns.Count];
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        values[i] = row.Cells[i].Value?.ToString();
                    }
                    dt.Rows.Add(values);
                }
            }

            return dt;
        }

        private void importToSQL_mi_Click(object sender, EventArgs e)
        {
            ImportDataGridViewToSql(dataGV, SQLManager.Instance.conStr, "Employee");
        }

        private void ImportDataGridViewToSql(DataGridView dgv, string connectionString, string tableName)
        {
            
            DataTable dt = GetDataTableFromDataGridView(dgv);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Bật IDENTITY_INSERT để có thể insert EmployeeID
                using (SqlCommand cmd = new SqlCommand("SET IDENTITY_INSERT Employee ON", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                foreach (DataGridViewRow row in dataGV.Rows)
                {
                    if (row.IsNewRow) continue;

                    int employeeID = Convert.ToInt32(row.Cells["EmployeeID"].Value);
                    string employeeCode = row.Cells["EmployeeCode"].Value?.ToString();
                    string fullName = row.Cells["FullName"].Value?.ToString();

                    DateTime? birthDate = DateTime.TryParse(row.Cells["BirthDate"].Value?.ToString(), out DateTime bd) ? bd.Date : (DateTime?)null;
                    DateTime? hireDate = DateTime.TryParse(row.Cells["HireDate"].Value?.ToString(), out DateTime hd) ? hd.Date : (DateTime?)null;
                    DateTime? issueDate = DateTime.TryParse(row.Cells["IssueDate"].Value?.ToString(), out DateTime idt) ? idt.Date : (DateTime?)null;
                    bool? gender = null;
                    if (row.Cells["Gender"].Value != null)
                        gender = Convert.ToBoolean(row.Cells["Gender"].Value);

                    string hometown = row.Cells["Hometown"].Value?.ToString();
                    string address = row.Cells["Address"].Value?.ToString();
                    string citizenID = row.Cells["CitizenID"].Value?.ToString();                    
                    string issuePlace = row.Cells["IssuePlace"].Value?.ToString();
                    int SalaryGradeID = Convert.ToInt32(row.Cells["SalaryGradeID"].Value?.ToString());

                    string SocialInsuranceNumber = row.Cells["SocialInsuranceNumber"].Value?.ToString();
                    string HealthInsuranceNumber = row.Cells["HealthInsuranceNumber"].Value?.ToString();

                    string BankName = row.Cells["BankName"].Value?.ToString();
                    string BankBranch = row.Cells["BankBranch"].Value?.ToString();
                    string BankAccountNumber = row.Cells["BankAccountNumber"].Value?.ToString();
                    string BankAccountHolder = row.Cells["BankAccountHolder"].Value?.ToString();

                    string PhoneNumber = row.Cells["PhoneNumber"].Value?.ToString();

                    int? positionID = row.Cells["PositionID"].Value != null ? Convert.ToInt32(row.Cells["PositionID"].Value) : (int?)null;
                    int? departmentID = row.Cells["DepartmentID"].Value != null ? Convert.ToInt32(row.Cells["DepartmentID"].Value) : (int?)null;
                    int? contractTypeID = row.Cells["ContractTypeID"].Value != null ? Convert.ToInt32(row.Cells["ContractTypeID"].Value) : (int?)null;

                    string sql = @"
            INSERT INTO Employee
            (EmployeeID, EmployeeCode, FullName, BirthDate, HireDate, Gender,
             Hometown, Address, CitizenID, IssueDate, IssuePlace,
             PositionID, DepartmentID, ContractTypeID, SalaryGradeID, SocialInsuranceNumber, HealthInsuranceNumber,
            BankName, BankBranch, BankAccountNumber, BankAccountHolder, PhoneNumber, NoteResign)
            VALUES
            (@EmployeeID, @EmployeeCode, @FullName, @BirthDate, @HireDate, @Gender,
             @Hometown, @Address, @CitizenID, @IssueDate, @IssuePlace,
             @PositionID, @DepartmentID, @ContractTypeID, @SalaryGradeID, @SocialInsuranceNumber, @HealthInsuranceNumber,
            @BankName, @BankBranch, @BankAccountNumber, @BankAccountHolder, @PhoneNumber, @NoteResign)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                        cmd.Parameters.AddWithValue("@EmployeeCode", employeeCode);
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@SalaryGradeID", SalaryGradeID);
                        cmd.Parameters.AddWithValue("@SocialInsuranceNumber", SocialInsuranceNumber);
                        cmd.Parameters.AddWithValue("@HealthInsuranceNumber", HealthInsuranceNumber);
                        cmd.Parameters.AddWithValue("@BankName", BankName);
                        cmd.Parameters.AddWithValue("@BankBranch", BankBranch);
                        cmd.Parameters.AddWithValue("@BankAccountNumber", BankAccountNumber);
                        cmd.Parameters.AddWithValue("@BankAccountHolder", BankAccountHolder);
                        cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        cmd.Parameters.AddWithValue("@NoteResign", "");

                        cmd.Parameters.AddWithValue("@BirthDate", (object)birthDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@HireDate", (object)hireDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Gender", (object)gender ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Hometown", (object)hometown ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Address", (object)address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CitizenID", (object)citizenID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IssueDate", (object)issueDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IssuePlace", (object)issuePlace ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PositionID", (object)positionID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DepartmentID", (object)departmentID ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContractTypeID", (object)contractTypeID ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Tắt IDENTITY_INSERT
                using (SqlCommand cmd = new SqlCommand("SET IDENTITY_INSERT Employee OFF", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void BirthDate_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            if (!dt.Columns.Contains("BirthDate"))
            {
                dt.Columns.Add("BirthDate", typeof(DateTime)); // typeof(int), typeof(DateTime)... tùy loại
            }
            dataGV.DataSource = dt;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.Cells["BirthDate1"].Value != null)
                {
                    string birthDate = row.Cells["BirthDate1"].Value.ToString().Trim();
                    if (DateTime.TryParse(birthDate?.ToString(), out DateTime parsedDate))
                    {
                        row.Cells["BirthDate"].Value = parsedDate.Date;
                    }
                    else
                    {
                        row.Cells["BirthDate"].Value = DBNull.Value; // hoặc để trống
                    }
                }
            }

            if (dt.Columns.Contains("BirthDate1"))
            {
                dt.Columns.Remove("BirthDate1");
            }
        }

        private void HireDate_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            if (!dt.Columns.Contains("HireDate"))
            {
                dt.Columns.Add("HireDate", typeof(DateTime)); // typeof(int), typeof(DateTime)... tùy loại
            }
            dataGV.DataSource = dt;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.Cells["HireDate1"].Value != null)
                {
                    string hireDate = row.Cells["HireDate1"].Value.ToString().Trim();
                    row.Cells["HireDate"].Value = Convert.ToDateTime(hireDate).Date;
                }
            }

            if (dt.Columns.Contains("HireDate1"))
            {
                dt.Columns.Remove("HireDate1");
            }
        }

        private void IssueDate_btn_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGV.DataSource;
            if (!dt.Columns.Contains("IssueDate"))
            {
                dt.Columns.Add("IssueDate", typeof(DateTime)); // typeof(int), typeof(DateTime)... tùy loại
            }
            dataGV.DataSource = dt;

            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.Cells["IssueDate1"].Value != null)
                {
                    string issueDateStr = row.Cells["IssueDate1"].Value.ToString().Trim();

                    if (!string.IsNullOrEmpty(issueDateStr))
                    {
                        if (DateTime.TryParse(issueDateStr, out DateTime issueDate))
                        {
                            row.Cells["IssueDate"].Value = issueDate.Date; // chỉ giữ ngày
                        }
                        else
                        {
                            row.Cells["IssueDate"].Value = DBNull.Value;
                        }
                    }
                    else
                        row.Cells["IssueDate"].Value = DBNull.Value;
                }
            }

            if (dt.Columns.Contains("IssueDate1"))
            {
                dt.Columns.Remove("IssueDate1");
            }
        }

        private void SalaryGrade_btn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGV.Rows)
            {
                if (row.Cells["SalaryGradeID"].Value != null)
                {
                    string name = row.Cells["SalaryGradeID"].Value.ToString();

                    switch (name)
                    {
                        case "Bậc 2.1":
                            row.Cells["SalaryGradeID"].Value = 5;
                            break;
                        case "Bậc 1.1.2":
                            row.Cells["SalaryGradeID"].Value = 1;
                            break;
                        case "Bậc 3.1":
                            row.Cells["SalaryGradeID"].Value = 7;
                            break;
                        case "Bậc 1.2":
                            row.Cells["SalaryGradeID"].Value = 4;
                            break;
                        case "Bậc 1.1":
                            row.Cells["SalaryGradeID"].Value = 3;
                            break;
                        case "Bậc 1.1.3":
                            row.Cells["SalaryGradeID"].Value = 2;
                            break;
                        default:
                            row.Cells["SalaryGradeID"].Value = 11;
                            break;
                    }
                }
            }
        }
    }
}
