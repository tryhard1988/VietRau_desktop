using Org.BouncyCastle.Asn1.X509;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

public class PCTienAnDem_Printer
{
    private DataTable allowanceData, overtimeData;
    private int month, year;
    private int countSTT = 1;
    private int rowIndex = 0; // để phân trang
    private int startX = 0;
    private int lineHeight = 40;
    private int lineHeight1 = 25;
    private string departmentName;
    private int departmentID;
    public PCTienAnDem_Printer(int departmentID, string departmentName, DataTable allowanceData, DataTable overtimeData, int month, int year)
    {
        this.departmentID = departmentID;
        this.departmentName = departmentName;
        this.allowanceData = allowanceData;
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
        var workDates = overtimeData.AsEnumerable()
                                    .Where(r => r["WorkDate"] != DBNull.Value
                                             && r["DepartmentID"] != DBNull.Value
                                             && r["HourWork"] != DBNull.Value
                                             && Convert.ToInt32(r["DepartmentID"]) == departmentID
                                             && Convert.ToDecimal(r["HourWork"]) >= 2.5m)
                                    .Select(r => Convert.ToDateTime(r["WorkDate"]).Date)
                                    .Distinct()
                                    .OrderBy(d => d)
                                    .ToList();
        // Cột
        int col1Width = 40, col2Width = 80, col3Width = 200, colDayWidth = 60, col4Width = 90, col5Width = 90;
        int col1 = startX;
        int col2 = col1 + col1Width;
        int col3 = col2 + col2Width;       
        int colDay = col3 + col3Width;
        int col4 = colDay + colDayWidth * workDates.Count;
        int col5 = col4 + col4Width;
        int colKT = col5 + col5Width; ;
        int colKTWidth = pageWidth - startX - colKT - 50;

        Font header1Font = new Font("Times New Roman", 7, FontStyle.Bold);
        Font normal1Font = new Font("Times New Roman", 7);
        Font headerFont = new Font("Times New Roman", 14, FontStyle.Bold);
        Font normalFont = new Font("Times New Roman", 9);
        Font tableHeaderFont = new Font("Times New Roman", 9, FontStyle.Bold);

        int y = startX;

        int lineHeightHeader = lineHeight1 * 2;
        // Header chỉ in 1 lần ở đầu trang
        if (rowIndex == 0)
        {
            g.DrawString("Công Ty CP Việt Rau", header1Font, Brushes.Black, startX, y);
            y += lineHeight1 / 2;
            g.DrawString($"MST:   {Utils.getTaxCode()}", normal1Font, Brushes.Black, startX, y);
            y += lineHeight1/2;

            g.DrawString($"Địa Chỉ:   {Utils.getCompanyAddress()}", normal1Font, Brushes.Black, startX, y);
            y += lineHeight1;

            g.DrawString($"Phòng: {departmentName}", headerFont, Brushes.Black, startX, y);

            string titleStr = $"PHỤ CẤP TIỀN ĂN ĐÊM T{month}/{year}";
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
        g.DrawLine(Pens.Gray, colKT, y, colKT, y + lineHeightHeader);

        DrawCellText(g, "STT", tableHeaderFont, new Rectangle(col1, y, col1Width, lineHeightHeader));
        DrawCellText(g, "Mã NV", tableHeaderFont, new Rectangle(col2, y, col2Width, lineHeightHeader));
        DrawCellText(g, "Họ và tên", tableHeaderFont, new Rectangle(col3, y, col3Width, lineHeightHeader));
        DrawCellText(g, "Tổng Suất Ăn", tableHeaderFont, new Rectangle(col4, y, col4Width, lineHeightHeader));
        DrawCellText(g, "Tổng Phần Mì", tableHeaderFont, new Rectangle(col5, y, col5Width, lineHeightHeader));
        DrawCellText(g, "Ký Nhận", tableHeaderFont, new Rectangle(colKT, y, colKTWidth, lineHeightHeader));

        SolidBrush bgBrush_LightGreen = new SolidBrush(Color.FromArgb(198, 224, 180));
        SolidBrush bgBrush_TCA_CN = new SolidBrush(Color.Yellow);//T.Ca Ngày Nghỉ

        for (int i = 0; i < workDates.Count; i++)
        {
            DateTime date = workDates[i];
            
            g.FillRectangle(date.DayOfWeek == DayOfWeek.Sunday ? bgBrush_TCA_CN : bgBrush_LightGreen, new Rectangle(colDay + colDayWidth * i + 1, y + lineHeightHeader / 2 + 1, colDayWidth - 2, lineHeightHeader / 2 - 2));
            g.DrawLine(Pens.Gray, colDay + colDayWidth * i, y, colDay + colDayWidth * i, y + lineHeightHeader);
            
            DrawCellText(g, $"{date.Day}/{date.Month}", normalFont, new Rectangle(colDay + colDayWidth*i, y, colDayWidth, lineHeightHeader/2));
            DrawCellText(g, Utils.GetThu_Viet(date).ToString(), normalFont, new Rectangle(colDay + colDayWidth * i, y + lineHeightHeader/2, colDayWidth, lineHeightHeader/2));
        }

        g.DrawLine(Pens.Gray, colDay, y + lineHeightHeader / 2, colDay + colDayWidth * workDates.Count, y + lineHeightHeader / 2);


        y += lineHeightHeader;

        // Table Data với phân trang
        while (rowIndex < allowanceData.Rows.Count)
        {
            if (y + lineHeight > pageHeight)
            {
                e.HasMorePages = true;
                return; // sang trang tiếp theo
            }

            DataRow row = allowanceData.Rows[rowIndex];

            int departmentID = Convert.ToInt32(row["DepartmentID"]);
            if (departmentID != this.departmentID)
            {
                rowIndex++;
                continue;
            }

            string employeeCode = row["EmployeeCode"].ToString();
            DataRow[] attendanceRows = overtimeData.Select($"EmployeeCode = '{employeeCode}' AND HourWork >= 2.5");//

            if (attendanceRows.Length <= 0) {
                rowIndex++;
                continue; 
            }

            int rice = 0;
            int nodle = 0;
            foreach (DataRow attendanceRow in attendanceRows)
            {
                DateTime targetDate = Convert.ToDateTime(attendanceRow["WorkDate"]);
                decimal hourWork = Convert.ToDecimal(attendanceRow["HourWork"]);
                if (hourWork < 2.5m) continue;

                int index = workDates.FindIndex(d => d.Date == targetDate);
                var rect = new Rectangle(colDay + colDayWidth * index + 1, y + 1, colDayWidth - 2, lineHeight - 2);                
                var rect1 = new Rectangle(colDay + colDayWidth * index, y, colDayWidth, lineHeight);

                string text = hourWork.ToString("F1");
                if (hourWork >= 3.0m)
                {
                    rice ++;
                    text += "\n(Cơm)";
                }
                else if (hourWork >= 2.5m)
                {
                    nodle ++;
                    text += "\n(Mì)";
                }
                else
                    text += "\n(ko pc)";
                DrawCellText(g, text, normalFont, rect1);


            }

            for (int i = 0; i < workDates.Count; i++)
            {
                g.DrawLine(Pens.Gray, colDay + colDayWidth * i, y, colDay + colDayWidth * i, y + lineHeight);
            }

            
            g.DrawRectangle(Pens.Gray, col1, y, colKT + colKTWidth - col1, lineHeight);
            g.DrawLine(Pens.Gray, col2, y, col2, y + lineHeight);
            g.DrawLine(Pens.Gray, col3, y, col3, y + lineHeight);
            g.DrawLine(Pens.Gray, col4, y, col4, y + lineHeight);
            g.DrawLine(Pens.Gray, col5, y, col5, y + lineHeight);
            g.DrawLine(Pens.Gray, colKT, y, colKT, y + lineHeight);

            DrawCellText(g, (countSTT).ToString(), normalFont, new Rectangle(col1, y, col1Width, lineHeight));
            DrawCellText(g, employeeCode, normalFont, new Rectangle(col2, y, col2Width, lineHeight));
            DrawCellText(g, row["EmployeeName"].ToString(), normalFont, new Rectangle(col3, y, col3Width, lineHeight), StringAlignment.Near);

            DrawCellText(g, rice.ToString(), normalFont, new Rectangle(col4, y, col4Width, lineHeight));
            DrawCellText(g, nodle.ToString(), normalFont, new Rectangle(col5, y, col5Width, lineHeight));

            y += lineHeight;
            rowIndex++;
            countSTT++;
        }

        e.HasMorePages = false;
    }
}
