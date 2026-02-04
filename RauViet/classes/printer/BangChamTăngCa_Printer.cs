using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public class BangChamTăngCa_Printer
{
    private DataTable employeeData, overtimeData;
    private int month, year;
    private int countSTT = 1;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 40;
    private int lineHeight1 = 25;
    private List<string> departmentNames;

    public BangChamTăngCa_Printer(List<string> departmentNames, DataTable employeeData, DataTable overtimeData, int month, int year)
    {
        this.departmentNames = departmentNames;
        this.employeeData = employeeData;
        this.overtimeData = overtimeData;
        this.month = month;
        this.year = year;

        DataView dv = overtimeData.DefaultView;
        dv.Sort = "WorkDate ASC";   // hoặc DESC

        this.overtimeData = dv.ToTable();
    }

    private void DrawCellText(Graphics g, string text, Font font, Rectangle rect, StringAlignment alignment = StringAlignment.Center)
    {
        StringFormat format = new StringFormat()
        {
            Alignment = alignment,
            LineAlignment = StringAlignment.Center
        };
        g.DrawString(text, font, Brushes.Black, rect, format);
    }

    public void PrintPreview(Form owner)
    {
        rowIndex = 0; // reset trước khi in
        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
        pd.DefaultPageSettings.Landscape = true;
        pd.PrintPage += Pd_PrintPage;
        PrintPreviewDialog preview = new PrintPreviewDialog();
        preview.Document = pd;
        preview.WindowState = FormWindowState.Maximized;  // phóng to toàn màn hình
        preview.StartPosition = FormStartPosition.CenterScreen;

        foreach (Control c in preview.Controls)
        {
            if (c is ToolStrip ts)
            {
                foreach (ToolStripItem item in ts.Items)
                {
                    if (
                        item.Name.Equals("print", StringComparison.OrdinalIgnoreCase) ||
                        item.Name.Equals("printButton", StringComparison.OrdinalIgnoreCase) ||
                        item.Name.Equals("toolStripButton1", StringComparison.OrdinalIgnoreCase) ||
                        (item.ToolTipText?.Contains("Print") ?? false) ||
                        (item.Text?.Contains("Print") ?? false)
                       )
                    {
                        item.Visible = false;
                    }
                }
            }
        }

        preview.ShowDialog(owner);
    }

    public void PrintDirect()
    {
        rowIndex = 0; // reset trước khi in

        PrintDocument pd = new PrintDocument();
        pd.DefaultPageSettings.Landscape = true;
        pd.PrintPage += Pd_PrintPage;

        // 🔹 Hộp thoại chọn máy in
        PrintDialog printDialog = new PrintDialog();
        printDialog.Document = pd;
        printDialog.AllowSomePages = false;
        printDialog.AllowSelection = false;
        printDialog.UseEXDialog = true;

        if (printDialog.ShowDialog() == DialogResult.OK)
        {
            // Máy in đã được gán tự động cho pd
            pd.Print();
        }
    }

    private void Pd_PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics;
        int pageWidth = e.PageBounds.Width;
        int pageHeight = e.PageBounds.Height - 50; // margin dưới
        int days = DateTime.DaysInMonth(year, month);
        // Cột
        int col1Width = 29, col2Width = 40, col3Width = 110, col4Width = 40, col5Width = 40, col6Width = 40, col7Width = 40, colDayWidth = 19;
        int col1 = startX;
        int col2 = col1 + col1Width;
        int col3 = col2 + col2Width;
        int col4 = col3 + col3Width;
        int col5 = col4 + col4Width;
        int col6 = col5 + col5Width;
        int col7 = col6 + col6Width;
        int colDay = col7 + col7Width;
        int colKT = colDay + colDayWidth * days;
        int colKTWidth = pageWidth - startX - colKT - 50;

        Font header1Font = new Font("Times New Roman", 7, FontStyle.Bold);
        Font headerFont = new Font("Times New Roman", 14, FontStyle.Bold);
        Font headerFont1 = new Font("Times New Roman", 11, FontStyle.Regular);
        Font normalFont = new Font("Times New Roman", 7);
        Font tableHeaderFont = new Font("Times New Roman", 7, FontStyle.Bold);

        int y = startX;

        int lineHeightHeader = lineHeight1 * 2;
        // Header chỉ in 1 lần ở đầu trang
        if (rowIndex == 0)
        {
            g.DrawString("Công Ty CP Việt Rau", header1Font, Brushes.Black, startX, y);
            y += lineHeight1 / 2;
            g.DrawString($"MST:   {Utils.getTaxCode()}", normalFont, Brushes.Black, startX, y);
            y += lineHeight1/2;

            g.DrawString($"Địa Chỉ:   {Utils.getCompanyAddress()}", normalFont, Brushes.Black, startX, y);
            y += lineHeight1;

            string result = string.Join(", ", departmentNames);
            g.DrawString($"Phòng Ban: {result}", headerFont1, Brushes.Black, startX, y);

            string titleStr = $"BẢNG DÒ TĂNG CA T{month}/{year}";
            SizeF codeSize = g.MeasureString(titleStr, headerFont);
            g.DrawString(titleStr, headerFont, Brushes.Black, (pageWidth - codeSize.Width) / 2, y);
            y += lineHeight1;
        }

        // Table Gray
        g.DrawRectangle(Pens.Black, col1, y, colKT + colKTWidth - col1, lineHeightHeader);
        g.DrawLine(Pens.Gray, col2, y, col2, y + lineHeightHeader);
        g.DrawLine(Pens.Gray, col3, y, col3, y + lineHeightHeader);
        g.DrawLine(Pens.Gray, col4, y, col4, y + lineHeightHeader);
        g.DrawLine(Pens.Gray, col5, y, col5, y + lineHeightHeader);
        g.DrawLine(Pens.Gray, col6, y, col6, y + lineHeightHeader);
        g.DrawLine(Pens.Gray, col7, y, col7, y + lineHeightHeader);
        g.DrawLine(Pens.Gray, colKT, y, colKT, y + lineHeightHeader);

        DrawCellText(g, "STT", tableHeaderFont, new Rectangle(col1, y, col1Width, lineHeightHeader));
        DrawCellText(g, "Mã NV", tableHeaderFont, new Rectangle(col2, y, col2Width, lineHeightHeader));
        DrawCellText(g, "Họ và tên", tableHeaderFont, new Rectangle(col3, y, col3Width, lineHeightHeader));
        DrawCellText(g, "T.Ca Ngày Thường", tableHeaderFont, new Rectangle(col4, y, col4Width, lineHeightHeader));
        DrawCellText(g, "T.Ca Ngày Lễ", tableHeaderFont, new Rectangle(col5, y, col5Width, lineHeightHeader));
        DrawCellText(g, "T.Ca CN", tableHeaderFont, new Rectangle(col6, y, col6Width, lineHeightHeader));
        DrawCellText(g, "T.Ca Đêm", tableHeaderFont, new Rectangle(col7, y, col7Width, lineHeightHeader));

        SolidBrush bgBrush_LightGreen = new SolidBrush(Color.FromArgb(198, 224, 180));
        SolidBrush bgBrush_TCA_CN = new SolidBrush(Color.Yellow);//T.Ca Ngày Nghỉ
        SolidBrush bgBrush_TCA_Thuong = new SolidBrush(Color.LightCyan);//T.Ca Thường
        SolidBrush bgBrush_TCA_NL = new SolidBrush(Color.LightCoral);//T.Ca Thường
        SolidBrush bgBrush_TCA_Dem = new SolidBrush(Color.LightBlue);//T.Ca Thường

        for (int i = 0; i < days; i++)
        {
            DateTime date = new DateTime(year, month, i + 1);
            
            g.FillRectangle(date.DayOfWeek == DayOfWeek.Sunday ? bgBrush_TCA_CN : bgBrush_LightGreen, new Rectangle(colDay + colDayWidth * i + 1, y + lineHeightHeader / 2 + 1, colDayWidth - 2, lineHeightHeader / 2 - 2));
            g.DrawLine(Pens.Gray, colDay + colDayWidth * i, y, colDay + colDayWidth * i, y + lineHeightHeader);
            
            DrawCellText(g, (i + 1).ToString(), normalFont, new Rectangle(colDay + colDayWidth*i, y, colDayWidth, lineHeightHeader/2));
            DrawCellText(g, Utils.GetThu_Viet(date).ToString(), normalFont, new Rectangle(colDay + colDayWidth * i, y + lineHeightHeader/2, colDayWidth, lineHeightHeader/2));
        }

        g.DrawLine(Pens.Gray, colDay, y + lineHeightHeader / 2, colDay + colDayWidth * days, y + lineHeightHeader / 2);

        DrawCellText(g, "Ký Nhận", tableHeaderFont, new Rectangle(colKT, y, colKTWidth, lineHeightHeader));

        y += lineHeightHeader;

        // Table Data với phân trang
        while (rowIndex < employeeData.Rows.Count)
        {
            if (y + lineHeight > pageHeight)
            {
                e.HasMorePages = true;
                return; // sang trang tiếp theo
            }

            DataRow row = employeeData.Rows[rowIndex];

            string departmentName = row["DepartmentName"].ToString();
            if (!this.departmentNames.Contains(departmentName)) 
            {
                rowIndex++;
                continue; 
            }

            string employeeCode = row["EmployeeCode"].ToString();
            DataRow[] attendanceRows = overtimeData.Select($"EmployeeCode = '{employeeCode}'");

            g.FillRectangle(bgBrush_TCA_Thuong, new Rectangle(col4, y, col4Width, lineHeight));
            g.FillRectangle(bgBrush_TCA_NL, new Rectangle(col5, y, col5Width, lineHeight));
            g.FillRectangle(bgBrush_TCA_CN, new Rectangle(col6, y, col6Width, lineHeight));
            g.FillRectangle(bgBrush_TCA_Dem, new Rectangle(col7, y, col7Width, lineHeight));

            g.DrawRectangle(Pens.Gray, col1, y, colKT + colKTWidth - col1, lineHeight);
            g.DrawLine(Pens.Gray, col2, y, col2, y + lineHeight);
            g.DrawLine(Pens.Gray, col3, y, col3, y + lineHeight);
            g.DrawLine(Pens.Gray, col4, y, col4, y + lineHeight);
            g.DrawLine(Pens.Gray, col5, y, col5, y + lineHeight);
            g.DrawLine(Pens.Gray, col6, y, col6, y + lineHeight);
            g.DrawLine(Pens.Gray, col7, y, col7, y + lineHeight);
            g.DrawLine(Pens.Gray, colKT, y, colKT, y + lineHeight);

            DrawCellText(g, (countSTT).ToString(), normalFont, new Rectangle(col1, y, col1Width, lineHeight));
            DrawCellText(g, employeeCode, tableHeaderFont, new Rectangle(col2, y, col2Width, lineHeight));
            DrawCellText(g, row["FullName"].ToString(), tableHeaderFont, new Rectangle(col3, y, col3Width, lineHeight), StringAlignment.Near);

            decimal total_CN = 0;
            decimal total_Thuong = 0;
            decimal total_Le = 0;
            decimal total_Dem = 0;
            foreach (DataRow attendanceRow in attendanceRows)
            {                
                DateTime workDate = Convert.ToDateTime(attendanceRow["WorkDate"]);
                int overtimeTypeID = Convert.ToInt32(attendanceRow["OvertimeTypeID"]);
                decimal hourWork = Convert.ToDecimal(attendanceRow["HourWork"]);

                var rect = new Rectangle(colDay + colDayWidth * (workDate.Day - 1) + 1, y + 1, colDayWidth - 2, lineHeight - 2);
                if (overtimeTypeID == 1)//T.Ca Thường
                {
                    total_Thuong += hourWork;
                    g.FillRectangle(bgBrush_TCA_Thuong, rect);
                }
                else if (overtimeTypeID == 2)//T.Ca Ngày Nghỉ
                {
                    total_CN += hourWork;
                    g.FillRectangle(bgBrush_TCA_CN, rect);
                }
                else if (overtimeTypeID == 3)//T.Ca Ngày Lễ
                {
                    total_Le += hourWork;
                    g.FillRectangle(bgBrush_TCA_NL, rect);
                }
                else if (overtimeTypeID == 4)//T.Ca Đêm
                {
                    total_Dem += hourWork;
                    g.FillRectangle(bgBrush_TCA_Dem, rect);
                }

                var rect1 = new Rectangle(colDay + colDayWidth * (workDate.Day - 1), y, colDayWidth, lineHeight);
                DrawCellText(g, hourWork.ToString("F1"), normalFont, rect1);
                                
                
            }

            for (int i = 0; i < days; i++)
            {
                g.DrawLine(Pens.Gray, colDay + colDayWidth * i, y, colDay + colDayWidth * i, y + lineHeight);
            }
                
            DrawCellText(g, total_Thuong.ToString("F1"), normalFont, new Rectangle(col4, y, col4Width, lineHeight));
            DrawCellText(g, total_Le.ToString("F1"), normalFont, new Rectangle(col5, y, col5Width, lineHeight));
            DrawCellText(g, total_CN.ToString("F1"), normalFont, new Rectangle(col6, y, col6Width, lineHeight));
            DrawCellText(g, total_Dem.ToString("F1"), normalFont, new Rectangle(col7, y, col7Width, lineHeight));

            y += lineHeight;
            rowIndex++;
            countSTT++;
        }

        e.HasMorePages = false;

    }
}
