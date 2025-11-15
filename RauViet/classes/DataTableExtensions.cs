using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

public static class DataTableExtensions
{
    public static DataTable ToDataTable<T>(this IEnumerable<T> data)
    {
        var props = TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new DataTable();

        // Tạo cột
        foreach (PropertyDescriptor prop in props)
        {
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        // Thêm dữ liệu
        foreach (var item in data)
        {
            DataRow row = table.NewRow();
            foreach (PropertyDescriptor prop in props)
            {
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            }
            table.Rows.Add(row);
        }

        return table;
    }
}
