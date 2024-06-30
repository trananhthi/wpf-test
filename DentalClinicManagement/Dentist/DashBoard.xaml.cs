using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DentalClinicManagement.Dentist
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : Page
    {
        DentistClass user;
        //public string Username { get; set; }

        public DashBoard(DentistClass user)
        {
            InitializeComponent();
            this.user = new DentistClass(user);
        }

        //private DentistClass GetDentist(string username)
        //{
        //    var sql = @"SELECT * FROM Dentist WHERE PhoneNo = @PhoneNo";
        //    var command = new SqlCommand(sql, DB.Instance.Connection);
        //    command.Parameters.Add("@PhoneNo", System.Data.SqlDbType.Char)
        //        .Value = username;

        //    var reader = command.ExecuteReader();
        //    if (reader.HasRows)
        //    {
        //        if (reader.Read())
        //        {
        //            int dentistID = (int)reader["DentistID"];
        //            string name = (string)reader["Name"];
        //            DateTime dob = ((DateTime)reader["DateOfBirth"]);
        //            string address = (string)reader["Address"];
        //            string phoneNo = (string)reader["PhoneNo"];
        //            int accID = (int)reader["AccountID"];
        //            DentistClass cus = new DentistClass()
        //            {
        //                DentistID = dentistID,
        //                DateOfBirth = dob,
        //                Name = name,
        //                Address = address,
        //                PhoneNo = phoneNo,
        //                AccountID = accID
        //            };

        //            reader.Close();
        //            return cus;
        //        }
        //    }
        //    reader.Close();
        //    return null;
        //}

        private void view_appointment_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                //DentistClass dentistSend = GetDentist(Username);
                ViewAppointment viewAppointment = new ViewAppointment(user);
                //viewAppointment.user = dentistSend;
                mainWindow.MainFrame.Navigate(viewAppointment);
            }
        }

        private void logOut(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Account.SignIn());
            }
            else
            {
                MessageBox.Show("Null");
            }
        }

        private void ReceivePatient(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Dentist.ReceivePatient(user));
            }
        }

        private void AddRecord(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                //DentistClass dentistSend = GetDentist(Username);
                AddRecord addRecord = new AddRecord();
                addRecord.user = user;
                mainWindow.MainFrame.Navigate(addRecord);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadAllData();
        }

        private void loadAllData()
        {
            // Connect to database
            var builder = new SqlConnectionStringBuilder();
            builder.ConnectionString = DB.Instance.ConnectionString;

            //DentistClass den = GetDentist(Username);
            this.DataContext = user;
        }
    }
}
