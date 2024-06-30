using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalClinicManagement.Customer
{
    public class CustomerRecord
    {
        public CustomerClass CustomerDetail { get; set; }

        public List<RecordList> RecordList { get; set; }
    }

    public class RecordList
    {
        public string DentistName { get; set; }
        public Decimal totalPrice { get; set; }
        public DateTime dateOfAppointment { get; set; }
        public Boolean isPaid { get; set; }

        public string medicalList { get; set; } 

        public string serviceList { get; set; }
        public int AppointRecordID { get; set; }
    }

   
    
}
