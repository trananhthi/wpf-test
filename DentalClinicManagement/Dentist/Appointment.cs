using System;

namespace DentalClinicManagement.Dentist
{
    public class Appointment
    {
        public int STT { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public int AppointmentID { get; set; }
        public int DentistID { get; set; }
        public int Shift { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public int CustomerID { get; set; }
    }
}