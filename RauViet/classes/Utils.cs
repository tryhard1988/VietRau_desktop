using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

public static class Utils
{
    private const int SaltSize = 16; // bytes
    private const int KeySize = 32;  // bytes (256-bit)
    private const int Iterations = 150_000; // tăng lên theo khả năng server

    public static string HashPassword(string password)
    {
        var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[SaltSize];
        rng.GetBytes(salt);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] key = pbkdf2.GetBytes(KeySize);

        // lưu dạng: iterations.salt(keyBase64).hashBase64
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public static bool VerifyPassword(string password, string stored)
    {
        try
        {
            var parts = stored.Split('.');
            if (parts.Length != 3) return false;

            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] storedKey = Convert.FromBase64String(parts[2]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] key = pbkdf2.GetBytes(storedKey.Length);

            // So sánh an toàn
            return FixedTimeEquals(key, storedKey);
        }
        catch
        {
            return false;
        }
    }

    public static bool FixedTimeEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) return false;

        int diff = 0;
        for (int i = 0; i < a.Length; i++)
        {
            diff |= a[i] ^ b[i];
        }
        return diff == 0;
    }

    public static string RemoveVietnameseSigns(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        string[] VietnameseSigns = new string[]
        {
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
        };

        for (int i = 1; i < VietnameseSigns.Length; i++)
        {
            foreach (char c in VietnameseSigns[i])
                text = text.Replace(c, VietnameseSigns[0][i - 1]);
        }
        return text;
    }

    public static void SetTabStopRecursive(System.Windows.Forms.Control parent, bool tabStop)
    {
        foreach (System.Windows.Forms.Control ctrl in parent.Controls)
        {
            // dùng reflection để an toàn với mọi loại control
            var prop = ctrl.GetType().GetProperty("TabStop");
            if (prop != null && prop.PropertyType == typeof(bool) && prop.CanWrite)
            {
                prop.SetValue(ctrl, tabStop, null);
            }

            if (ctrl.HasChildren)
                SetTabStopRecursive(ctrl, tabStop);
        }
    }

    public static string FormatDate(DateTime dt)
    {
        return dt.ToString("dd/MM/yyyy");
    }

    public static decimal calNetWeight(int PCS, int amount, string packing)
    {
        float realAmount = amount;
        string packingLower = packing.Replace(" ", "").ToLower();
        switch (packingLower)
        {
            case "gr":
                realAmount = amount / 1000.0f;
                break;
        }

        decimal netWeight = Convert.ToDecimal(PCS * realAmount);
        return netWeight;
    }

    public static decimal calQuanity(int PCS, decimal NW, string package)
    {
        string packageLower = package.Replace(" ", "").ToLower();
        if (packageLower.CompareTo("kg") == 0 || packageLower.CompareTo("weight") == 0)
        {
            return NW;
        }
        return PCS;
    }

    public static string InputDialog(string prompt, string title, string defaultValue = "")
    {
        Form inputForm = new Form()
        {
            Width = 400,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = title,
            StartPosition = FormStartPosition.CenterScreen,
            MinimizeBox = false,
            MaximizeBox = false
        };

        Label textLabel = new Label() { Left = 10, Top = 20, Text = prompt, AutoSize = true };
        TextBox textBox = new TextBox() { Left = 10, Top = 50, Width = 360, Text = defaultValue };
        Button okButton = new Button() { Text = "OK", Left = 220, Width = 70, Top = 80, DialogResult = DialogResult.OK };
        Button cancelButton = new Button() { Text = "Cancel", Left = 300, Width = 70, Top = 80, DialogResult = DialogResult.Cancel };

        inputForm.Controls.Add(textLabel);
        inputForm.Controls.Add(textBox);
        inputForm.Controls.Add(okButton);
        inputForm.Controls.Add(cancelButton);
        inputForm.AcceptButton = okButton;
        inputForm.CancelButton = cancelButton;

        return inputForm.ShowDialog() == DialogResult.OK ? textBox.Text : null;
    }

    public static DataTable LoadExcel_NoHeader(string filePath, int skipRows = 0)
    {
        DataTable dt = new DataTable();
        try
        {
            using (var workbook = new XLWorkbook(filePath))
            {
                var ws = workbook.Worksheet(1); // Sheet đầu tiên
                var range = ws.RangeUsed();     // Lấy vùng có dữ liệu
                if (range == null) return dt;   // Trả về DataTable rỗng nếu sheet trống

                int rowCount = range.RowCount();
                int colCount = range.ColumnCount();
                int startRow = 1 + skipRows;
                // Tạo cột tự động: Column1, Column2, ...
                for (int c = 1; c <= colCount; c++)
                    dt.Columns.Add($"Column{c}");

                // Đọc dữ liệu
                for (int r = startRow; r <= rowCount; r++)
                {
                    DataRow row = dt.NewRow();
                    bool isEmptyRow = true;
                    for (int c = 1; c <= colCount; c++)
                    {
                        string value = ws.Cell(r, c).GetValue<string>();
                        row[c - 1] = value;

                        if (!string.IsNullOrWhiteSpace(value))
                            isEmptyRow = false;
                    }

                    if (!isEmptyRow)
                        dt.Rows.Add(row);
                }
            }

            
        }
        catch
        {

        }

        return dt;
    }

    public static int? GetIntValue(DataGridViewCell cell)
    {
        if (cell == null || cell.Value == null || cell.Value == DBNull.Value)
            return null;

        try
        {
            // Nếu đã là int thì ép kiểu trực tiếp
            if (cell.Value is int valueInt)
                return valueInt;

            // Nếu là kiểu khác (string, long, decimal, v.v.)
            string strValue = cell.Value.ToString().Trim();
            if (string.IsNullOrWhiteSpace(strValue))
                return null;

            return Convert.ToInt32(strValue);
        }
        catch
        {
            return null; // hoặc throw nếu bạn muốn bắt lỗi ở ngoài
        }
    }

    public static decimal? GetDecimalValue(DataGridViewCell cell)
    {
        if (cell == null || cell.Value == null || cell.Value == DBNull.Value)
            return null;

        try
        {
            // Nếu đã là decimal thì ép kiểu trực tiếp
            if (cell.Value is decimal decVal)
                return decVal;

            // Nếu là kiểu int, double, float, v.v.
            if (decimal.TryParse(cell.Value.ToString().Trim(), out decimal result))
                return result;

            return null;
        }
        catch
        {
            return null; // hoặc throw nếu muốn xử lý bên ngoài
        }
    }

    public static int? GetIntValue(object value)
    {
        if (value == null || value == DBNull.Value)
            return null;

        return int.TryParse(value.ToString(), out int result)
            ? (int?)result  // ép sang int? để tương thích với null
            : null;
    }


    public static void SafeSelectValue(ComboBox combo, object value)
    {
        if (value == null)
        {
            combo.SelectedIndex = -1;
            return;
        }

        // Ép về kiểu int nếu cần
        int intValue = Convert.ToInt32(value);

        // Kiểm tra danh sách có chứa không
        if (combo.DataSource is DataTable dt && dt.AsEnumerable().Any(r => r.Field<int>(combo.ValueMember) == intValue))
            combo.SelectedValue = intValue;
        else
        {
            combo.SelectedIndex = -1; // Không chọn gì nếu không tồn tại
        }
    }

    public static string getCompanyAddress()
    {
        return "Tổ 1, Ấp 4, X. Phước Thái, T. Đồng Nai, Việt Nam";
    }

    public static string getTaxCode()
    {
        return "0313983703";
    }

    public static string GetThu_Viet(DateTime date)
    {
        switch (date.DayOfWeek)
        {
            case DayOfWeek.Monday:
                return "T2";
            case DayOfWeek.Tuesday:
                return "T3";
            case DayOfWeek.Wednesday:
                return "T4";
            case DayOfWeek.Thursday:
                return "T5";
            case DayOfWeek.Friday:
                return "T6";
            case DayOfWeek.Saturday:
                return "T7";
            case DayOfWeek.Sunday:
                return "CN";
            default:
                return "";
        }
    }
}