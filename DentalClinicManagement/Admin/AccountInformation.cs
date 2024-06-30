using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalClinicManagement.Admin
{
    public class AccountInformation
    {
        public string name { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string address { get; set; }

        public string phone { get; set; }

        public string gender { get; set; }

        public string status { get; set; }
        public AccountInformation(DataRow row)
        {
            name = row["Name"].ToString();
            dateOfBirth = Convert.ToDateTime(row["DateOfBirth"]);
            address = row["Address"].ToString();
            phone = row["PhoneNo"].ToString();
            gender = row["Gender"].ToString();
            status = row["Status"].ToString();
        }
    }
}