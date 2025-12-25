using RauViet.classes;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

public class DSTangCa_Printer
{
    private DataTable dt;
    private string mPurposeOvertime;
    private string mDepartmentName;
    private string mOvertimeTypeName;
    private TimeSpan mStartTime;
    private TimeSpan mEndTime;
    private System.DateTime mOvertimeDate;

    private int rowIndex = 0; // để phân trang
    private int startX = 50;
    private int lineHeight = 27;
    private int lineHeight1 = 25;

    public DSTangCa_Printer(DataTable data, string purposeOvertime, string departmentName, string overtimeTypeName, TimeSpan startTime, TimeSpan endTime, System.DateTime overtimeDate)
    {
        dt = data;
        this.mPurposeOvertime = purposeOvertime;
        this.mDepartmentName = departmentName;
        this.mOvertimeTypeName = overtimeTypeName;
        this.mStartTime = startTime;
        this.mEndTime = endTime;
        this.mOvertimeDate = overtimeDate;
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
            pd.PrintPage += Pd_PrintPage;
            pd.Print();
        }
        
    }

    private void Pd_PrintPage(object sender, PrintPageEventArgs e)
    {
        Graphics g = e.Graphics;
        int pageWidth = e.PageBounds.Width;
        int pageHeight = e.PageBounds.Height - 50; // margin dưới

        // Cột
        int col1Width = 40, col2Width = 60, col3Width = 200, col4Width = 50, col5Width = 50, col6Width = 85, col7Width = 80;
        int col1 = startX;
        int col2 = col1 + col1Width;
        int col3 = col2 + col2Width;
        int col4 = col3 + col3Width;
        int col5 = col4 + col4Width;
        int col6 = col5 + col5Width;
        int col7 = col6 + col6Width;
        int col8 = col7 + col7Width;
        int col8Width = pageWidth - startX - col8;

        Font headerFont = new Font("Times New Roman", 20, FontStyle.Bold);
        Font header1Font = new Font("Times New Roman", 12);
        Font header2Font = new Font("Times New Roman", 12, FontStyle.Bold);
        Font normalFont = new Font("Times New Roman", 10);
        Font tableHeaderFont = new Font("Times New Roman", 11, FontStyle.Bold);

        int y = startX;

        // Header chỉ in 1 lần ở đầu trang
        if (rowIndex == 0)
        {
            g.DrawString($"CÔNG TY CỔ PHẦN VIỆT RAU", header2Font, Brushes.Black, startX, y - lineHeight);
            y += lineHeight;

            string t1 = "DANH SÁCH CÔNG NHÂN TĂNG CA";
            SizeF codeSize = g.MeasureString(t1, headerFont);
            g.DrawString(t1, headerFont, Brushes.Black, (pageWidth - codeSize.Width) / 2, y);
            y += (lineHeight + 15);

            g.DrawString($"Ngày:", header2Font, Brushes.Black, startX, y);
            g.DrawString($"{mOvertimeDate.Date.ToString("dd/MM/yyyy")}", header1Font, Brushes.Black, startX + 200, y);
            y += lineHeight;

            g.DrawString($"Phòng Ban:", header2Font, Brushes.Black, startX, y);
            g.DrawString($"{mDepartmentName}", header1Font, Brushes.Black, startX + 200, y);
            y += lineHeight;

            g.DrawString($"Mục Đích Tăng Ca:", header2Font, Brushes.Black, startX, y);
            g.DrawString($"{mPurposeOvertime}", header1Font, Brushes.Black, startX + 200, y);
            y += lineHeight;

            g.DrawString($"Thời Gian:", header2Font, Brushes.Black, startX, y);
            g.DrawString($"từ {mStartTime.Hours.ToString("00")}h{mStartTime.Minutes.ToString("00")} đến {mEndTime.Hours.ToString("00")}h{mEndTime.Minutes.ToString("00")} ({mOvertimeTypeName})", header1Font, Brushes.Black, startX + 200, y);
            y += lineHeight;
        }

        // Table Header
        int lineHeight1 = lineHeight + 12;
        g.DrawRectangle(Pens.Black, col1, y, col8 + col8Width - col1, lineHeight1);
        g.DrawLine(Pens.Black, col2, y, col2, y + lineHeight1);
        g.DrawLine(Pens.Black, col3, y, col3, y + lineHeight1);
        g.DrawLine(Pens.Black, col4, y, col4, y + lineHeight1);
        g.DrawLine(Pens.Black, col5, y, col5, y + lineHeight1);
        g.DrawLine(Pens.Black, col6, y, col6, y + lineHeight1);
        g.DrawLine(Pens.Black, col7, y, col7, y + lineHeight1);
        g.DrawLine(Pens.Black, col8, y, col8, y + lineHeight1);

        DrawCellText(g, "STT", tableHeaderFont, new Rectangle(col1, y, col2 - col1, lineHeight1));
        DrawCellText(g, "Mã NV", tableHeaderFont, new Rectangle(col2, y, col3 - col2, lineHeight1));
        DrawCellText(g, "Họ và Tên", tableHeaderFont, new Rectangle(col3, y, col4 - col3, lineHeight1));
        DrawCellText(g, "Giờ Vào", tableHeaderFont, new Rectangle(col4, y, col5 - col4, lineHeight1));
        DrawCellText(g, "Giờ Ra", tableHeaderFont, new Rectangle(col5, y, col5Width, lineHeight1));
        DrawCellText(g, "Tổng Số Giờ", tableHeaderFont, new Rectangle(col6, y, col6Width, lineHeight1));
        DrawCellText(g, "Ký Tên", tableHeaderFont, new Rectangle(col7, y, col7Width, lineHeight1));
        DrawCellText(g, "Ghi Chú", tableHeaderFont, new Rectangle(col8, y, col8Width, lineHeight1));

        y += lineHeight1;

        lineHeight1 = lineHeight + 5;
        // Table Data với phân trang
        while (rowIndex < dt.Rows.Count)
        {
            if (y + lineHeight1 > pageHeight)
            {
                e.HasMorePages = true;
                return; // sang trang tiếp theo
            }

            DataRow row = dt.Rows[rowIndex];

            g.DrawRectangle(Pens.Black, col1, y, col8 + col8Width - col1, lineHeight1);
            g.DrawLine(Pens.Black, col2, y, col2, y + lineHeight1);
            g.DrawLine(Pens.Black, col3, y, col3, y + lineHeight1);
            g.DrawLine(Pens.Black, col4, y, col4, y + lineHeight1);
            g.DrawLine(Pens.Black, col5, y, col5, y + lineHeight1);
            g.DrawLine(Pens.Black, col6, y, col6, y + lineHeight1);
            g.DrawLine(Pens.Black, col7, y, col7, y + lineHeight1);
            g.DrawLine(Pens.Black, col8, y, col8, y + lineHeight1);

            TimeSpan startTime = (TimeSpan)row["StartTime"];
            TimeSpan endTime = (TimeSpan)row["EndTime"];

            decimal hourWork = 0;
            if (row["HourWork"] != DBNull.Value)
            {
                decimal.TryParse(row["HourWork"].ToString(), out hourWork);
            }


            DrawCellText(g, (rowIndex + 1).ToString(), normalFont, new Rectangle(col1, y, col2 - col1, lineHeight1));
            DrawCellText(g, row["EmployeeCode"].ToString(), normalFont, new Rectangle(col2, y, col3 - col2, lineHeight1));
            DrawCellText(g, row["EmployeeName"].ToString(), normalFont, new Rectangle(col3, y, col4 - col3, lineHeight1), StringAlignment.Near);
            DrawCellText(g, startTime.ToString(@"hh\:mm"), normalFont, new Rectangle(col4, y, col5 - col4, lineHeight1));
            DrawCellText(g, endTime.ToString(@"hh\:mm"), normalFont, new Rectangle(col5, y, col5Width, lineHeight1));
            DrawCellText(g, hourWork == 0 ? "" : hourWork.ToString("F2"), normalFont, new Rectangle(col6, y, col6Width, lineHeight1));
            DrawCellText(g, "", normalFont, new Rectangle(col7, y, col7Width, lineHeight1));
            DrawCellText(g, row["Note"].ToString(), normalFont, new Rectangle(col8, y, col8Width, lineHeight1));

            y += lineHeight1;
            rowIndex++;
        }

        e.HasMorePages = false;

        if (y + lineHeight + 70 > pageHeight)
        {
            e.HasMorePages = true;
            return; // sang trang tiếp theo
        }
        y += lineHeight;

        DrawCellText(g, "Người Lập Đơn", normalFont, new Rectangle(col6, y, col6Width* 3, lineHeight));
        DrawCellText(g, UserManager.Instance.fullName, normalFont, new Rectangle(col6, y + 70, col6Width * 3, lineHeight));
    }
}
