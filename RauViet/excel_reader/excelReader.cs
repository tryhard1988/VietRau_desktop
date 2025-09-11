using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RauViet.excel_reader
{
    public sealed class excelReader
    {
        private static excelReader ins = null;
        private static readonly object padlock = new object();

        public excelReader(){}

        public static excelReader Instance
        {
            get
            {
                lock (padlock)
                {
                    if (ins == null)
                    {
                        ins = new excelReader();
                    }
                    return ins;
                }
            }
        }
    }
}
