using System;
using System.IO;
using System.Runtime.InteropServices;

class GodexPrinter
{
    // In trực tiếp qua tên máy in Windows
    [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true)]
    public static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

    [DllImport("winspool.Drv", EntryPoint = "ClosePrinter")]
    public static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA")]
    public static extern bool StartDocPrinter(IntPtr hPrinter, int Level, [In] ref DOCINFOA pDocInfo);

    [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter")]
    public static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter")]
    public static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter")]
    public static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "WritePrinter")]
    public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

    [StructLayout(LayoutKind.Sequential)]
    public struct DOCINFOA
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string pDocName;
        [MarshalAs(UnmanagedType.LPStr)]
        public string pOutputFile;
        [MarshalAs(UnmanagedType.LPStr)]
        public string pDataType;
    }

    public static void PrintTSPL(string printerName, string tsplCommand)
    {
        IntPtr hPrinter = IntPtr.Zero;
        DOCINFOA docInfo = new DOCINFOA
        {
            pDocName = "TSPL Label",
            pOutputFile = null,
            pDataType = "RAW"
        };

        if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
            throw new Exception("Cannot open printer.");

        try
        {
            if (!StartDocPrinter(hPrinter, 1, ref docInfo))
                throw new Exception("Cannot start doc.");

            if (!StartPagePrinter(hPrinter))
                throw new Exception("Cannot start page.");

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(tsplCommand);
            IntPtr pUnmanagedBytes = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, pUnmanagedBytes, bytes.Length);

            if (!WritePrinter(hPrinter, pUnmanagedBytes, bytes.Length, out int written))
                throw new Exception("Cannot write to printer.");

            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            EndPagePrinter(hPrinter);
            EndDocPrinter(hPrinter);
        }
        finally
        {
            ClosePrinter(hPrinter);
        }
    }
}
