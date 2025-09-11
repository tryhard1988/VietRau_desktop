using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RauViet.classes
{
    public sealed class SQLManager
    {
        private static SQLManager ins = null;
        private static readonly object padlock = new object();

        string conStr = "Data Source =  DESKTOP-75MLKFG\\SQLEXPRESS;Initial Catalog = RauVietDB;Integrated Security=true;";
        SqlConnection con;

        public SQLManager()
        {
            
            con = new SqlConnection(conStr);
            
        }

        public static SQLManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (ins == null)
                    {
                        ins = new SQLManager();
                    }
                    return ins;
                }
            }
        }

        public DataTable getCustomers()
        {
            con.Open();

            string query = "Select * from Customers";
            SqlDataAdapter da = new SqlDataAdapter(query, con);

            con.Close();

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getDanhSachNSXKhac()
        {
            con.Open();

            string query = "Select * from DanhSachNSXKhac";
            SqlDataAdapter da = new SqlDataAdapter(query, con);

            con.Close();

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getDanhSachXuatCang()
        {
            con.Open();

            string query = "Select * from DanhSachXuatCang";
            SqlDataAdapter da = new SqlDataAdapter(query, con);

            con.Close();

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getHoaDonXuatKhau()
        {
            con.Open();

            string query = "Select * from HoaDonXuatKhau";
            SqlDataAdapter da = new SqlDataAdapter(query, con);

            con.Close();

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getInvoiceXuatNhapKhau()
        {
            con.Open();

            string query = "Select * from InvoiceXuatNhapKhau";
            SqlDataAdapter da = new SqlDataAdapter(query, con);

            con.Close();

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getLichThuHoach()
        {
            con.Open();

            string query = "Select * from LichThuHoach";
            SqlDataAdapter da = new SqlDataAdapter(query, con);

            con.Close();

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable getDonDatHang()
        {
            con.Open();

            string query = "Select * from DonDatHang";
            SqlDataAdapter da = new SqlDataAdapter(query, con);

            con.Close();

            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public bool updateCustomer(string maKH, string name, string address, string phoneNumber, string email, int b2BID, string note, bool uuTien)
        {
            string query = "UPDATE Customers SET Name = @Name, Address = @Address, Phone = @Phone, Email = @Email, B2BID = @B2BID, Note = @Note, UuTien = @UuTien";
            query += " WHERE MaKhachHang = '" + maKH + "'";

            SqlCommand cmdUpdate = new SqlCommand(query, con);
            cmdUpdate.Parameters.AddWithValue("@Name", name);
            cmdUpdate.Parameters.AddWithValue("@Address", address);
            cmdUpdate.Parameters.AddWithValue("@Phone", phoneNumber);
            cmdUpdate.Parameters.AddWithValue("@Email", email);
            cmdUpdate.Parameters.AddWithValue("@B2BID", b2BID);
            cmdUpdate.Parameters.AddWithValue("@Note", note);
            cmdUpdate.Parameters.AddWithValue("@UuTien", uuTien);

            try
            {
                con.Open();
                cmdUpdate.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();
                return false;
            }
            return true;
        }

        public bool insertCustomer(string maKH, string name, string address, string phoneNumber, string email, int b2BID, string note, bool uuTien)
        {
            string query = "INSERT INTO Customers (MaKhachHang, Name, Address, Phone, Email, B2BID, Note, UuTien)";
            query += " VALUES (@MaKhachHang, @Name, @Address, @Phone, @Email, @B2BID, @Note, @UuTien)";

            SqlCommand cmdInsert = new SqlCommand(query, con);
            cmdInsert.Parameters.AddWithValue("@MaKhachHang", maKH);
            cmdInsert.Parameters.AddWithValue("@Name", name);
            cmdInsert.Parameters.AddWithValue("@Address", address);
            cmdInsert.Parameters.AddWithValue("@Phone", phoneNumber);
            cmdInsert.Parameters.AddWithValue("@Email", email);
            cmdInsert.Parameters.AddWithValue("@B2BID", b2BID);
            cmdInsert.Parameters.AddWithValue("@Note", note);
            cmdInsert.Parameters.AddWithValue("@UuTien", uuTien);
            try
            {
                con.Open();
                cmdInsert.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception ex)
            {
                con.Close();

                return false;
            }


            return true;
        }
    }
}
