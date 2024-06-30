using DentalClinicManagement.Account;
using DentalClinicManagement.Employee;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DentalClinicManagement.Customer
{
    public class CustomerClass
    {
        public int? CustomerID { get; set; }
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public int? AccountID { get; set; }

        // For Binding
        public bool? IsMale { get; set; }
        public bool? IsFemale { get; set; }

        // Convert string Gender to Boolean Gender
        public bool StringToBooleanGender(string? gender)
        {
            // Kiểm tra xem giới tính có giá trị không rỗng và có phải là "Male" hay không
            return !string.IsNullOrWhiteSpace(gender) && gender.Trim().Equals("Nam", StringComparison.OrdinalIgnoreCase);
        }

        public void ConvertToStringGender(bool isMale)
        {
            // Chuyển đổi từ giá trị boolean về chuỗi giới tính
            Gender = isMale ? "Nam" : "Nữ";
        }

        // Constructor để tạo đối tượng Customer từ SqlDataReader
        public CustomerClass(SqlDataReader reader)
        {
            CustomerID = (Int32)reader["CustomerID"];
            Name = reader["Name"].ToString();
            DateOfBirth = (DateTime)reader["DateOfBirth"];
            Address = reader["Address"].ToString();
            Gender = reader["Gender"].ToString();
            PhoneNo = reader["PhoneNo"].ToString();
            Email = reader["Email"].ToString();
            AccountID = (Int32)reader["AccountID"];
            IsMale = this.StringToBooleanGender(Gender);
            IsFemale = !IsMale;
        }

        // Constructor mặc định
        public CustomerClass() { }

        // Copy constructor
        public CustomerClass(CustomerClass other)
        {
            CustomerID = other.CustomerID;
            Name = other.Name;
            DateOfBirth = other.DateOfBirth;
            Address = other.Address;
            Gender = other.Gender;
            PhoneNo = other.PhoneNo;
            Email = other.Email;
            AccountID = other.AccountID;
            IsMale = other.IsMale;
            IsFemale = other.IsFemale;
        }

        // Load Customer data by Account
        public CustomerClass? LoadUser(AccountClass? account)
        {
            try
            {
                // Câu truy vấn SQL để lấy thông tin Customer từ database dựa trên AccountID
                string query = "SELECT TOP 1* FROM [Customer] WHERE AccountID = @AccountID";

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
                                CustomerClass user = new CustomerClass(reader);
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
