using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalClinicManagement.Dentist
{
    public class DentistAvailableTime
    {
        public int dentistID {  get; set; }

        public string dentistName { get; set; }

        public int shift { get; set; }

        public DateTime date { get; set; }
    }
}
