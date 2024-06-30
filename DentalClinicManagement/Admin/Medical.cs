using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalClinicManagement.Admin
{
    public class Medical
    {
        public int medicalID { get; set; }
        public string medicalName { get; set; }
        public int price { get; set; }
        public string allocation { get; set; }
        public int inventoryNumber { get; set; }
        public DateTime expirationDate { get; set; }
        public Medical(DataRow row)
        {
            medicalID = Convert.ToInt32(row["MedicalID"]);
            medicalName = row["Name"].ToString();
            price = Convert.ToInt32(row["Price"]);
            allocation = row["Allocation"].ToString();
            inventoryNumber = Convert.ToInt32(row["InventoryNumber"]);
            expirationDate = Convert.ToDateTime(row["ExpirationDate"]);
        }
    }
}