using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

public class SalarySlipPrinter
{
    private DataGridView dgv;
    private DataTable allowance_dt;
    private DataTable leaveType_dt;
    private DataTable overTimeType_dt;
    private DataTable deduction_dt;
    private List<string> employeeCodesToPrint;
    private int month;
    private int year;
    private Image companyLogo;
    private int currentIndex = 0;

    public SalarySlipPrinter(DataGridView dgv, DataTable allowance_dt, DataTable leaveType_dt, DataTable overTimeType_dt, DataTable deduction_dt,
        List<string> employeeCodesToPrint, int month, int year, Image logo)
    {
        this.dgv = dgv;
        this.allowance_dt = allowance_dt;
        this.leaveType_dt = leaveType_dt;
        this.overTimeType_dt = overTimeType_dt;
        this.deduction_dt = deduction_dt;
        this.employeeCodesToPrint = employeeCodesToPrint;
        this.month = month;
        this.year = year;
        this.companyLogo = logo;
    }

    public void PrintDirect()
    {
        if (employeeCodesToPrint == null || employeeCodesToPrint.Count == 0)
            return;

        DialogResult dialogResult = MessageBox.Show($"Số Lượng Nhân Viên: {employeeCodesToPrint.Count}\nChắc chắn Chưa?", "Xác nhận in ấn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (dialogResult == DialogResult.No)
            return;

        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.PaperSize = new PaperSize("A5", 583, 827);
        pd.PrintPage += PrintPage;

        currentIndex = 0;
        pd.Print(); // In trực tiếp ra máy in mặc định
    }


    public void PrintPreview()
    {
        if (employeeCodesToPrint == null || employeeCodesToPrint.Count == 0) return;

        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.PaperSize = new PaperSize("A5", 583, 827);
        pd.PrintPage += PrintPage;

        PrintPreviewDialog preview = new PrintPreviewDialog();
        preview.Document = pd;
        preview.WindowState = FormWindowState.Maximized;
        preview.ShowDialog();
    }

    public void ExportAllToPdf()
    {
        if (employeeCodesToPrint == null || employeeCodesToPrint.Count == 0)
            return;

        using (SaveFileDialog saveDlg = new SaveFileDialog())
        {
            saveDlg.Title = "Chọn nơi lưu file phiếu lương tổng hợp (PDF)";
            saveDlg.Filter = "PDF File|*.pdf";
            saveDlg.FileName = $"TongHopPhieuLuong_{month:00}{year}.pdf";

            if (saveDlg.ShowDialog() != DialogResult.OK)
                return;

            string outputPath = saveDlg.FileName;

            try
            {
                PrintDocument pd = new PrintDocument();
                pd.DefaultPageSettings.PaperSize = new PaperSize("A5", 583, 827);
                pd.PrintPage += PrintPage;

                // Bắt đầu từ nhân viên đầu tiên
                currentIndex = 0;

                // Cấu hình máy in ảo PDF
                pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                pd.PrinterSettings.PrintToFile = true;
                pd.PrinterSettings.PrintFileName = outputPath;

                // ⚠️ Tắt hộp thoại "Save As" mặc định
                pd.PrinterSettings.PrintFileName = outputPath;
                pd.PrinterSettings.PrintToFile = true;

                pd.Print();

                MessageBox.Show($"✅ Đã xuất toàn bộ {employeeCodesToPrint.Count} phiếu lương vào:\n{outputPath}",
                    "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi xuất PDF:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


    private void PrintPage(object sender, PrintPageEventArgs e)
    {
        if (currentIndex >= employeeCodesToPrint.Count)
        {
            e.HasMorePages = false;
            return;
        }

        string employeeCode = employeeCodesToPrint[currentIndex];
        DataGridViewRow row = null;

        foreach (DataGridViewRow r in dgv.Rows)
        {
            if (r.IsNewRow) continue;
            if ((r.Cells["EmployeeCode"].Value?.ToString() ?? "") == employeeCode)
            {
                row = r;
                break;
            }
        }

        if (row == null)
        {
            currentIndex++;
            e.HasMorePages = currentIndex < employeeCodesToPrint.Count;
            return;
        }

        // Lấy dữ liệu chi tiết nhân viên từ allowance_dt
        DataTable empAllowance = allowance_dt.Clone();
        foreach (DataRow dr in allowance_dt.Select($"EmployeeCode = '{employeeCode}'"))
            empAllowance.ImportRow(dr);

        Graphics g = e.Graphics;
        int pageWidth = e.PageBounds.Width;
        int margin = 20;

        Font titleFont = new Font("Arial", 14, FontStyle.Bold);
        Font subTitleFont = new Font("Arial", 11, FontStyle.Bold);
        Font tableHeaderFont = new Font("Arial", 10, FontStyle.Bold);
        Font tableFont = new Font("Arial", 9, FontStyle.Regular);
        Font thanksFont = new Font("Arial", 8, FontStyle.Italic);
        int y = margin;
        int logoWidth = pageWidth / 5;
        // --- HEADER ---
        if (companyLogo != null)
        {
            // Giữ đúng tỉ lệ gốc của logo
            float aspectRatio = (float)companyLogo.Height / companyLogo.Width;
            int logoHeight = (int)(logoWidth * aspectRatio);

            // Vẽ hình đúng kích thước
            g.DrawImage(companyLogo, margin, y, logoWidth, logoHeight);
        }


        int textX = logoWidth + margin;
        StringFormat centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Near
        };
        RectangleF headerRect = new RectangleF(textX, y, pageWidth - logoWidth - 2 * margin, 80);

        g.DrawString("CÔNG TY CỔ PHẦN VIỆT RAU", titleFont, Brushes.Black, headerRect, centerFormat);
        g.DrawString($"Phiếu Lương Tháng {month:00}-{year}", subTitleFont, Brushes.Black, new RectangleF(textX, y + 30, headerRect.Width, 50), centerFormat);
        g.DrawString($"Tên NV: {row.Cells["FullName"].Value}", subTitleFont, Brushes.Black, new RectangleF(textX, y + 55, headerRect.Width, 50), centerFormat);

        y += 90;
        
        // --- INFO NHÂN VIÊN ---
        g.DrawString($"Mã nhân viên: {employeeCode}", tableFont, Brushes.Black, new PointF(margin + 50, y));
        g.DrawString($"Ngày vào làm: {row.Cells["HireDate"].Value:dd/MM/yyyy}", tableFont, Brushes.Black, new PointF(margin + 50, y + 20));
        //g.DrawString($"Ngày vào làm: {row.Cells["StartDate"].Value:dd/MM/yyyy}", tableFont, Brushes.Black, new PointF(margin, y + 20));
        g.DrawString($"Lương Căn Bản: {row.Cells["BaseSalary"].Value:N0} ({row.Cells["ContractTypeName"].Value})", tableFont, Brushes.Black, new PointF(margin + 50, y + 40));
        g.DrawString($"Ngày phép năm còn lại: {row.Cells["RemainingLeave"].Value}", tableFont, Brushes.Black,new PointF(pageWidth * 2 / 3 - 50, y));
        g.DrawString($"Lương giờ: {row.Cells["HourSalary"].Value:N0} đồng/giờ", tableFont, Brushes.Black,new PointF(pageWidth * 2 / 3 - 50, y + 20));       

        y += 50;        
        // --- BẢNG LƯƠNG THEO GIỜ LÀM ---
        int tableLeft = margin - 2;
        int tableWidth = (pageWidth - 3 * margin) * 2 / 3;
        int tableRight = tableLeft + tableWidth + 5;
        int tableRightWidth = (pageWidth - 3 * margin) / 3;
        int rowHeight = 20;

        y += rowHeight;
        int rightY = y;
        Rectangle cttlCell = new Rectangle(tableLeft, y, tableWidth, rowHeight);
        g.DrawRectangle(Pens.Black, cttlCell);
        g.DrawString("Chi Tiết Tiền Lương", tableHeaderFont, Brushes.Black, cttlCell, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        y += rowHeight;
        string[] leftHeaders1 = { "Tiêu đề", "Số giờ", "Thành tiền" };
        int[] leftColWidths1 = { tableWidth / 2, tableWidth / 6, tableWidth / 3 };
        int[] leftColWidths1_Allowance = { tableWidth / 2, tableWidth / 6 + tableWidth / 3 };

        int x = tableLeft;
        for (int i = 0; i < leftHeaders1.Length; i++)
        {
            Rectangle cell = new Rectangle(x, y, leftColWidths1[i], rowHeight);
            g.DrawRectangle(Pens.Black, cell);
            g.DrawString(leftHeaders1[i], tableHeaderFont, Brushes.Black, cell,
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += leftColWidths1[i];
        }

        y += rowHeight;
        x = tableLeft;
        int countCa = 1;
        decimal tongthunhap = 0;
        tongthunhap += Convert.ToInt32(row.Cells["TotalSalaryHourWork"].Value);

        string[] totalHourWorkcells = { $"Lương theo giờ ({countCa++})", row.Cells["TotalHourWork"].Value.ToString(), $"{Convert.ToDecimal(row.Cells["TotalSalaryHourWork"].Value):N0}"};
        for (int i = 0; i < totalHourWorkcells.Length; i++)
        {
            Rectangle cell = new Rectangle(x, y, leftColWidths1[i], rowHeight);
            g.DrawRectangle(Pens.Black, cell);
            g.DrawString(totalHourWorkcells[i], tableFont, Brushes.Black, cell, new StringFormat { Alignment = i == 0 ? StringAlignment.Near : StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += leftColWidths1[i];
        }

        foreach (DataRow adr in leaveType_dt.Rows)
        {
            y += rowHeight;
            x = tableLeft;            
            string leaveTypeCol = "LeaveType" + adr["LeaveTypeCode"].ToString();
            string[] leaveTypeCells = { $"{adr["LeaveTypeName"]} ({countCa++})", row.Cells["c_" + leaveTypeCol].Value.ToString(), $"{Convert.ToDecimal(row.Cells[leaveTypeCol].Value):N0}"};
            for (int i = 0; i < leaveTypeCells.Length; i++)
            {
                Rectangle cell = new Rectangle(x, y, leftColWidths1[i], rowHeight);
                g.DrawRectangle(Pens.Black, cell);
                g.DrawString(leaveTypeCells[i], tableFont, Brushes.Black, cell, new StringFormat { Alignment = i == 0 ? StringAlignment.Near : StringAlignment.Center, LineAlignment = StringAlignment.Center });
                x += leftColWidths1[i];
            }

            tongthunhap += Convert.ToInt32(row.Cells[leaveTypeCol].Value);
        }

        foreach (DataRow adr in overTimeType_dt.Rows)
        {
            y += rowHeight;
            x = tableLeft;

            string overtimeTypeCol = "OvertimeType" + adr["OvertimeTypeID"].ToString();
            string[] overtimeTypeCells = { $"{adr["OvertimeName"]} ({countCa++})", row.Cells["c_" + overtimeTypeCol].Value.ToString(), $"{Convert.ToDecimal(row.Cells[overtimeTypeCol].Value):N0}" };
            for (int i = 0; i < overtimeTypeCells.Length; i++)
            {
                Rectangle cell = new Rectangle(x, y, leftColWidths1[i], rowHeight);
                g.DrawRectangle(Pens.Black, cell);
                g.DrawString(overtimeTypeCells[i], tableFont, Brushes.Black, cell, new StringFormat { Alignment = i == 0 ? StringAlignment.Near : StringAlignment.Center, LineAlignment = StringAlignment.Center });
                x += leftColWidths1[i];
            }
            tongthunhap += Convert.ToInt32(row.Cells[overtimeTypeCol].Value);
        }

        y += rowHeight;
        foreach (DataRow adr in empAllowance.Rows)
        {
            string scopeCode = adr["ScopeCode"].ToString();
            if (scopeCode.CompareTo("ONCE") == 0)
            {
                int allowanceTypeID = Convert.ToInt32(adr["AllowanceTypeID"]);

                x = tableLeft;

                string allowanceCol = "Allowance" + adr["AllowanceTypeID"].ToString();
                string[] allowanceCells ={$"{adr["AllowanceName"]} ({countCa++})", $"{Convert.ToDecimal(row.Cells[allowanceCol].Value):N0}"};

                // --- 1️⃣ Đo chiều cao thực tế của từng ô ---
                float maxCellHeight = 0f;
                for (int i = 0; i < allowanceCells.Length; i++)
                {
                    // Giới hạn đo theo chiều rộng cột tương ứng
                    SizeF textSize = g.MeasureString(allowanceCells[i], tableFont, leftColWidths1_Allowance[i]);
                    if (textSize.Height > maxCellHeight)
                        maxCellHeight = textSize.Height;
                }

                // Thêm padding nhỏ cho chữ không chạm viền
                int adjustedRowHeight = (int)Math.Ceiling(maxCellHeight) + 6;

                // --- 2️⃣ Vẽ các ô với chiều cao động ---
                for (int i = 0; i < allowanceCells.Length; i++)
                {
                    Rectangle cell = new Rectangle(x, y, leftColWidths1_Allowance[i], adjustedRowHeight);
                    g.DrawRectangle(Pens.Black, cell);

                    // Cấu hình căn lề và wrap text
                    StringFormat sf = new StringFormat
                    {
                        LineAlignment = StringAlignment.Center,
                        FormatFlags = StringFormatFlags.LineLimit
                    };

                    if (i == 0)
                        sf.Alignment = StringAlignment.Near; // Cột tên: canh trái
                    else
                        sf.Alignment = StringAlignment.Center; // Cột số: canh giữa

                    // Padding text để không dính sát viền
                    RectangleF textRect = new RectangleF(cell.X + 3, cell.Y + 3, cell.Width - 6, cell.Height - 6);
                    g.DrawString(allowanceCells[i], tableFont, Brushes.Black, textRect, sf);

                    x += leftColWidths1_Allowance[i];
                }

                tongthunhap += Convert.ToInt32(row.Cells[allowanceCol].Value);
                // --- 3️⃣ Tăng Y cho dòng kế tiếp ---
                y += adjustedRowHeight;
            }
        }

        x = tableLeft;
        string[] htBHXHCells = { $"Hoàn trả BHXH ({countCa})", $"{Convert.ToDecimal(row.Cells["InsuranceRefund"].Value):N0}"};
        for (int i = 0; i < htBHXHCells.Length; i++)
        {
            Rectangle cell = new Rectangle(x, y, leftColWidths1_Allowance[i], rowHeight);
            g.DrawRectangle(Pens.Black, cell);
            g.DrawString(htBHXHCells[i], tableFont, Brushes.Black, cell, new StringFormat { Alignment = i == 0 ? StringAlignment.Near : StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += leftColWidths1[i];
        }

        tongthunhap += Convert.ToInt32(row.Cells["InsuranceRefund"].Value);

        y += rowHeight;
        x = tableLeft;

        tongthunhap = Math.Truncate(tongthunhap / 1000) * 1000;
        string[] ttnCells = { $"I. Tổng thu nhập (1)->({countCa})", tongthunhap .ToString()};
        for (int i = 0; i < ttnCells.Length; i++)
        {
            Rectangle cell = new Rectangle(x, y, leftColWidths1_Allowance[i], rowHeight*2);
            g.DrawRectangle(Pens.Black, cell);
            g.DrawString(ttnCells[i], tableHeaderFont, Brushes.Black, cell, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += leftColWidths1[i];
        }

        y += rowHeight*2;
        Rectangle cktlCell = new Rectangle(tableLeft, y, tableWidth, rowHeight);
        g.DrawRectangle(Pens.Black, cktlCell);
        g.DrawString("Khác Khoản Trừ", tableHeaderFont, Brushes.Black, cktlCell, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        countCa = 1;
        decimal tongtru = 0;

        foreach (DataRow ddr in deduction_dt.Rows)
        {
            y += rowHeight;
            x = tableLeft;
            string deductionCol = "DeductionType" + ddr["DeductionTypeCode"].ToString();
            string[] deductionCells = { $"{ddr["DeductionTypeName"]} ({countCa++})", row.Cells[deductionCol].Value.ToString() };
            for (int i = 0; i < deductionCells.Length; i++)
            {
                Rectangle cell = new Rectangle(x, y, leftColWidths1_Allowance[i], rowHeight);
                g.DrawRectangle(Pens.Black, cell);
                g.DrawString(deductionCells[i], tableFont, Brushes.Black, cell, new StringFormat { Alignment = i == 0 ? StringAlignment.Near : StringAlignment.Center, LineAlignment = StringAlignment.Center });
                x += leftColWidths1[i];
            }

            tongtru += Convert.ToDecimal(row.Cells[deductionCol].Value);
        }

        y += rowHeight;
        x = tableLeft;

        tongtru = Math.Truncate(tongtru / 1000) * 1000;

        string[] cktCells = { $"II. Cộng Khoản Trừ (1)->({countCa})", tongtru.ToString() };
        for (int i = 0; i < cktCells.Length; i++)
        {
            Rectangle cell = new Rectangle(x, y, leftColWidths1_Allowance[i], rowHeight*2);
            g.DrawRectangle(Pens.Black, cell);            
            g.DrawString(cktCells[i], tableHeaderFont, Brushes.Black, cell, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += leftColWidths1[i];
        }

        y += rowHeight*2;
        x = tableLeft;

        decimal netSalary = tongthunhap - tongtru;
        netSalary = Math.Truncate(netSalary / 1000) * 1000;
        string[] TLCells = { "THỰC LÃNH (I - II)", $"{netSalary:N0}" };
        for (int i = 0; i < TLCells.Length; i++)
        {
            Rectangle cell = new Rectangle(x, y, leftColWidths1_Allowance[i], rowHeight);
            g.DrawRectangle(Pens.Black, cell);
            g.DrawString(TLCells[i], tableHeaderFont, Brushes.Black, cell, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            x += leftColWidths1[i];
        }

        // --- BẢNG PHỤ CẤP bên phải ---
        int[] rightColWidths = { tableRightWidth * 2 / 3, tableRightWidth / 3 };

        
        x = tableRight;
        Rectangle cell1 = new Rectangle(x, rightY, tableRightWidth, rowHeight);
        g.DrawRectangle(Pens.Black, cell1);
        g.DrawString("Chi Tiết phụ Cấp", tableHeaderFont, Brushes.Black, cell1, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        rightY += rowHeight;
        foreach (DataRow dr in empAllowance.Rows)
        {
            string scopeCode = dr["ScopeCode"].ToString();
            if (scopeCode.CompareTo("ONCE") != 0)
            {
                x = tableRight;
                string[] cells = { dr["AllowanceName"].ToString(), dr["Amount"].ToString() };

                // 1) Tính chiều cao cần thiết cho từng ô (với giới hạn chiều rộng tương ứng)
                float maxCellHeight = 0f;
                for (int i = 0; i < cells.Length; i++)
                {
                    // Dùng RectangleF rộng = rightColWidths[i] để đo wrap chính xác
                    var layout = new SizeF(rightColWidths[i], 10000f); // chiều cao lớn để đo
                    var measured = g.MeasureString(cells[i], tableFont, (int)rightColWidths[i]);
                    // MeasureString trả về kích thước cần để wrap
                    if (measured.Height > maxCellHeight) maxCellHeight = measured.Height;
                }

                // Thêm padding lên trên/dưới để chữ không sát viền
                int adjustedRowHeight = (int)Math.Ceiling(maxCellHeight) + 6;

                // 2) Vẽ từng ô ở Y hiện tại (chưa tăng rightY)
                for (int i = 0; i < cells.Length; i++)
                {
                    Rectangle cell = new Rectangle(x, rightY, rightColWidths[i], adjustedRowHeight);
                    g.DrawRectangle(Pens.Black, cell);

                    // Tạo StringFormat: nếu là cột tên thì canh trái/giữa theo dọc; nếu là tiền thì canh center
                    StringFormat sf = new StringFormat();
                    if (i == 0)
                    {
                        sf.Alignment = StringAlignment.Near; // canh trái cho tên (đẹp hơn)
                        sf.LineAlignment = StringAlignment.Center;
                    }
                    else
                    {
                        sf.Alignment = StringAlignment.Center; // canh giữa cho amount
                        sf.LineAlignment = StringAlignment.Center;
                    }
                    sf.FormatFlags = StringFormatFlags.LineLimit; // giới hạn trong box và cho phép wrap

                    // Vẽ text trong rectangle (dùng RectangleF để DrawString wrap đúng)
                    RectangleF rf = new RectangleF(cell.X + 3, cell.Y + 3, cell.Width - 6, cell.Height - 6); // padding nhỏ
                    g.DrawString(cells[i], tableFont, Brushes.Black, rf, sf);

                    x += rightColWidths[i];
                }

                // 3) Sau khi vẽ xong cả dòng, tăng rightY cho dòng tiếp theo
                rightY += adjustedRowHeight;
            }
        }

        string thanksCells = "‘*Chúng tôi rất cảm ơn về sự đóng góp của bạn. Vì lý do tế nhị vui lòng không để người khác biết thu nhập của bạn !";
        Rectangle cell2 = new Rectangle(tableRight, rightY, tableRightWidth, 100);
        g.DrawString(thanksCells, thanksFont, Brushes.Black, cell2, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });


        currentIndex++;
        e.HasMorePages = currentIndex < employeeCodesToPrint.Count;
    }
}
