using RauViet.ui;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public class BangChamCong_Printer
{
    private DataTable employeeData, attendanceData;
    private int month, year;
    private int countSTT = 1;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 40;
    private int lineHeight1 = 25;
    private string departmentName;

    public BangChamCong_Printer(string departmentName, DataTable employeeData, DataTable attendanceData, int month, int year)
    {
        this.departmentName = departmentName;
        this.employeeData = employeeData;
        this.attendanceData = attendanceData;
        this.month = month;
        this.year = year;

        DataView dv = attendanceData.DefaultView;
        dv.Sort = "WorkDate ASC";   // hoặc DESC

        this.attendanceData = dv.ToTable();
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
        DialogResult dialogResult = MessageBox.Show($"Chắc chắn Chưa?", "Xác nhận in ấn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (dialogResult == DialogResult.Yes)
        {
            rowIndex = 0; // reset trước khi in
            PrintDocument pd = new PrintDocument();
            pd.DefaultPageSettings.Landscape = true;
            
            pd.PrintPage += Pd_PrintPage;
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
        int col1Width = 29, col2Width = 40, col3Width = 110, col4Width = 36, col5Width = 32, col6Width = 32, col7Width = 32, col8Width = 32, colDayWidth = 19;
        int col1 = startX;
        int col2 = col1 + col1Width;
        int col3 = col2 + col2Width;
        int col4 = col3 + col3Width;
        int col5 = col4 + col4Width;
        int col6 = col5 + col5Width;
        int col7 = col6 + col6Width;
        int col8 = col7 + col7Width;
        int colDay = col8 + col8Width;
        int colKT = colDay + colDayWidth * days;
        int colKTWidth = pageWidth - startX - colKT - 50;

        Font header1Font = new Font("Times New Roman", 7, FontStyle.Bold);
        Font headerFont = new Font("Times New Roman", 14, FontStyle.Bold);
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

            g.DrawString($"Phòng Ban: {departmentName}", headerFont, Brushes.Black, startX, y);

            string titleStr = $"BẢNG CHẤM CÔNG THÁNG {month}/{year}";
            SizeF codeSize = g.MeasureString(titleStr, headerFont);
            g.DrawString(titleStr, headerFont, Brushes.Black, (pageWidth - codeSize.Width) / 2, y);
            y += lineHeight1;
        }

        // Table Header
        g.DrawRectangle(Pens.Black, col1, y, colKT + colKTWidth - col1, lineHeightHeader);
        g.DrawLine(Pens.Black, col2, y, col2, y + lineHeightHeader);
        g.DrawLine(Pens.Black, col3, y, col3, y + lineHeightHeader);
        g.DrawLine(Pens.Black, col4, y, col4, y + lineHeightHeader);
        g.DrawLine(Pens.Black, col5, y, col5, y + lineHeightHeader);
        g.DrawLine(Pens.Black, col6, y, col6, y + lineHeightHeader);
        g.DrawLine(Pens.Black, col7, y, col7, y + lineHeightHeader);
        g.DrawLine(Pens.Black, col8, y, col8, y + lineHeightHeader);
        g.DrawLine(Pens.Black, colKT, y, colKT, y + lineHeightHeader);

        DrawCellText(g, "STT", tableHeaderFont, new Rectangle(col1, y, col1Width, lineHeightHeader));
        DrawCellText(g, "Mã NV", tableHeaderFont, new Rectangle(col2, y, col2Width, lineHeightHeader));
        DrawCellText(g, "Họ và tên", tableHeaderFont, new Rectangle(col3, y, col3Width, lineHeightHeader));
        DrawCellText(g, "Nghỉ phép thường", tableHeaderFont, new Rectangle(col4, y, col4Width, lineHeightHeader));
        DrawCellText(g, "Ngày lễ", tableHeaderFont, new Rectangle(col5, y, col5Width, lineHeightHeader));
        DrawCellText(g, "Nghỉ PN (h)", tableHeaderFont, new Rectangle(col6, y, col6Width, lineHeightHeader));
        DrawCellText(g, "Nghỉ hưởng lương", tableHeaderFont, new Rectangle(col7, y, col7Width, lineHeightHeader));
        DrawCellText(g, "Tổng giờ công", tableHeaderFont, new Rectangle(col8, y, col8Width, lineHeightHeader));

        SolidBrush bgBrush_LightGreen = new SolidBrush(Color.FromArgb(198, 224, 180));
        SolidBrush bgBrush_Yellow = new SolidBrush(Color.Yellow);

        for (int i = 0; i < days; i++)
        {
            DateTime date = new DateTime(year, month, i + 1);
            
            g.FillRectangle(date.DayOfWeek == DayOfWeek.Sunday ? bgBrush_Yellow : bgBrush_LightGreen, new Rectangle(colDay + colDayWidth * i, y + lineHeightHeader / 2, colDayWidth, lineHeightHeader / 2));
            g.DrawLine(Pens.Black, colDay + colDayWidth * i, y, colDay + colDayWidth * i, y + lineHeightHeader);
            
            DrawCellText(g, (i + 1).ToString(), tableHeaderFont, new Rectangle(colDay + colDayWidth*i, y, colDayWidth, lineHeightHeader/2));
            DrawCellText(g, Utils.GetThu_Viet(date).ToString(), tableHeaderFont, new Rectangle(colDay + colDayWidth * i, y + lineHeightHeader/2, colDayWidth, lineHeightHeader/2));
        }

        g.DrawLine(Pens.Black, colDay, y + lineHeightHeader / 2, colDay + colDayWidth * days, y + lineHeightHeader / 2);

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
            if (departmentName.CompareTo(this.departmentName) != 0) 
            {
                rowIndex++;
                continue; 
            }

            string employeeCode = row["EmployeeCode"].ToString();
            DataRow[] attendanceRows = attendanceData.Select($"EmployeeCode = '{employeeCode}'");

            g.FillRectangle(bgBrush_LightGreen, new Rectangle(col4, y, col4Width, lineHeight));
            g.FillRectangle(bgBrush_LightGreen, new Rectangle(col5, y, col5Width, lineHeight));
            g.FillRectangle(bgBrush_LightGreen, new Rectangle(col6, y, col6Width, lineHeight));
            g.FillRectangle(bgBrush_LightGreen, new Rectangle(col7, y, col7Width, lineHeight));
            g.FillRectangle(bgBrush_LightGreen, new Rectangle(col8, y, col8Width, lineHeight));

            g.DrawRectangle(Pens.Black, col1, y, colKT + colKTWidth - col1, lineHeight);
            g.DrawLine(Pens.Black, col2, y, col2, y + lineHeight);
            g.DrawLine(Pens.Black, col3, y, col3, y + lineHeight);
            g.DrawLine(Pens.Black, col4, y, col4, y + lineHeight);
            g.DrawLine(Pens.Black, col5, y, col5, y + lineHeight);
            g.DrawLine(Pens.Black, col6, y, col6, y + lineHeight);
            g.DrawLine(Pens.Black, col7, y, col7, y + lineHeight);
            g.DrawLine(Pens.Black, col8, y, col8, y + lineHeight);
            g.DrawLine(Pens.Black, colKT, y, colKT, y + lineHeight);

            DrawCellText(g, (countSTT).ToString(), normalFont, new Rectangle(col1, y, col1Width, lineHeight));
            DrawCellText(g, employeeCode, tableHeaderFont, new Rectangle(col2, y, col2Width, lineHeight));
            DrawCellText(g, row["FullName"].ToString(), tableHeaderFont, new Rectangle(col3, y, col3Width, lineHeight));
            DrawCellText(g, Convert.ToDecimal(row["c_LeaveTypeNL_1"]).ToString("F1"), normalFont, new Rectangle(col4, y, col4Width, lineHeight));
            DrawCellText(g, Convert.ToDecimal(row["c_LeaveTypeNL_1"]).ToString("F1"), normalFont, new Rectangle(col5, y, col5Width, lineHeight));
            DrawCellText(g, Convert.ToDecimal(row["c_LeaveTypePN_1"]).ToString("F1"), normalFont, new Rectangle(col6, y, col6Width, lineHeight));
            DrawCellText(g, Convert.ToDecimal(row["c_LeaveTypeCN_1"]).ToString("F1"), normalFont, new Rectangle(col7, y, col7Width, lineHeight));
            DrawCellText(g, Convert.ToDecimal(row["TotalHourWork"]).ToString("F1"), normalFont, new Rectangle(col8, y, col8Width, lineHeight));

            foreach (DataRow attendanceRow in attendanceRows)
            {                
                DateTime workDate = Convert.ToDateTime(attendanceRow["WorkDate"]);
                if(workDate.DayOfWeek == DayOfWeek.Sunday)
                    g.FillRectangle(bgBrush_Yellow, new Rectangle(colDay + colDayWidth * (workDate.Day - 1), y, colDayWidth, lineHeight));
                else
                    DrawCellText(g, Convert.ToDecimal(attendanceRow["WorkingHours"]).ToString("F1"), normalFont, new Rectangle(colDay + colDayWidth * (workDate.Day - 1), y, colDayWidth, lineHeight));
                g.DrawLine(Pens.Black, colDay + colDayWidth * (workDate.Day - 1), y, colDay + colDayWidth * (workDate.Day - 1), y + lineHeight);                
                
            }

            y += lineHeight;
            rowIndex++;
            countSTT++;
        }

        e.HasMorePages = false;

        // Tổng cộng và chữ ký (chỉ in ở trang cuối)
        //y += 5;
        //string totalLabel = "Tổng cộng (Count):";
        //string totalValue = $"{totalCount} thùng";
        //g.DrawString(totalLabel, headerFont, Brushes.Black, col1, y);
        //Rectangle countCell = new Rectangle(col3, y, col4 - col3, lineHeight);
        //SizeF textSize = g.MeasureString(totalValue, headerFont);
        //float x = countCell.X + (countCell.Width - textSize.Width) / 2;
        //float yy = countCell.Y + (countCell.Height - textSize.Height) / 2;
        //g.DrawString(totalValue, headerFont, Brushes.Black, x, yy);

        //y += lineHeight1 * 2;

        //g.DrawString("Người giao hàng", normalFont, Brushes.Black, col2, y);
        //g.DrawString("Người nhận hàng", normalFont, Brushes.Black, col5, y);
        //g.DrawString("(Ký, ghi rõ họ tên)", normalFont, Brushes.Black, col2, y + lineHeight1);
        //g.DrawString("(Ký, ghi rõ họ tên)", normalFont, Brushes.Black, col5, y + lineHeight1);
    }
}
