using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalClinicManagement.Dentist
{
    public class Medicine
    {
        public int ID {  get; set; }
        public string Name { get; set; }
        public Decimal price { get; set; }
        public int inventoryNumber { get; set; }
        public int status { get; set; }

        public int quantity {get; set; } = 0;
    }
}
