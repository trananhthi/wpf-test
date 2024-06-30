using DentalClinicManagement.Account;
using DentalClinicManagement.Customer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DentalClinicManagement.Employee
{
    public class StaffClass
    {
        public int? StaffID { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNo { get; set; }
        public int? AccountID { get; set; }

        // Constructor để tạo đối tượng Customer từ SqlDataReader
        public StaffClass(SqlDataReader reader)
        {
            StaffID = (Int32)reader["StaffID"];
            Name = reader["Name"].ToString();
            Gender = reader["Gender"].ToString();
            Address = reader["Address"].ToString();
            DateOfBirth = (DateTime)reader["DateOfBirth"];
            PhoneNo = reader["PhoneNo"].ToString();
            AccountID = (Int32)reader["AccountID"];
        }

        // Constructor mặc định
        public StaffClass() { }

        // Copy constructor
        public StaffClass(StaffClass other)
        {
            StaffID = other.StaffID;
            Name = other.Name;
            Gender = other.Gender;
            Address = other.Address;
            DateOfBirth = other.DateOfBirth;
            PhoneNo = other.PhoneNo;
            AccountID = other.AccountID;
        }

        // Load Staff data by Account
        public StaffClass? LoadUser(AccountClass? account)
        {
            try
            {
                // Câu truy vấn SQL để lấy thông tin Staff từ database dựa trên AccountID
                string query = "SELECT TOP 1* FROM [Staff] WHERE AccountID = @AccountID";

                // Tạo và mở kết nối
                DB dB = new DB();
                using (SqlConnection connection = dB.Connection)
                {
                    // Tạo đối tượng SqlCommand
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Thêm tham số cho câu truy vấn
                        command.Parameters.AddWithValue("@AccountID", account.AccountID);

                        // Sử dụng SqlDataReader để đọc dữ liệu từ database
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Kiểm tra xem có dữ liệu hay không
                            if (reader.Read())
                            {
                                // Tạo đối tượng Dentist từ SqlDataReader
                                StaffClass user = new StaffClass(reader);
                                return user;
                            }
                            else
                            {
                                // Trường hợp không tìm thấy thông tin Dentist
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
