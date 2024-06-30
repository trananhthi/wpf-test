using DentalClinicManagement.Customer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace DentalClinicManagement.Account
{
    public class AccountClass
    {
        public int? AccountID { get; set; }
        public string? PhoneNo { get; set; }
        public string? Password { get; set; }
        public bool? Status { get; set; }

        // Constructor để tạo đối tượng Account từ SqlDataReader
        public AccountClass(SqlDataReader reader)
        {
            AccountID = (Int32)reader["AccountID"];
            PhoneNo = reader["PhoneNo"].ToString();
            Password = reader["Password"].ToString();
            Status = DBNull.Value.Equals(reader["Status"]) ? (bool?)null : (bool)reader["Status"];
        }

        // Constructor mặc định
        public AccountClass() { }

        // Copy constructor
        public AccountClass(AccountClass other)
        {          
            AccountID = other.AccountID;
            PhoneNo = other.PhoneNo;
            Password = other.Password;
            Status = other.Status;
        }     
    }
}
