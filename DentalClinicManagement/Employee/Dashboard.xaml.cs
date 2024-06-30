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

namespace DentalClinicManagement.Employee
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
 
    public partial class Dashboard : Page
    {
        StaffClass user;
        //public string Username { get; set; }

        public Dashboard(StaffClass user)
        {
            InitializeComponent();
            this.user = new StaffClass(user);
        }

        //private StaffClass GetStaff(string username)
        //{
        //    var sql = @"SELECT * FROM Staff WHERE PhoneNo = @PhoneNo";
        //    var command = new SqlCommand(sql, DB.Instance.Connection);
        //    command.Parameters.Add("@PhoneNo", System.Data.SqlDbType.Char)
        //        .Value = username;

        //    var reader = command.ExecuteReader();
        //    if (reader.HasRows)
        //    {
        //        if (reader.Read())
        //        {
        //            int staffID = (int)reader["StaffID"];
        //            string name = (string)reader["Name"];
        //            DateTime dob = ((DateTime)reader["DateOfBirth"]);
        //            string address = (string)reader["Address"];
        //            string phoneNo = (string)reader["PhoneNo"];
        //            int accID = (int)reader["AccountID"];
        //            string gender = (string)reader["Gender"];
        //            StaffClass cus = new StaffClass()
        //            {
        //                StaffID = staffID,
        //                Name = name,
        //                Address = address,
        //                PhoneNo = phoneNo,
        //                AccountID = accID,
        //                DateOfBirth = dob,
        //            };

        //            reader.Close();
        //            return cus;
        //        }
        //    }
        //    reader.Close();
        //    return null;
        //}

        private void logOutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Account.Home());
            }
        }

        private void takePatientButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Employee.CheckAppoinment(user));
            }
        }

        private void findPatientButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Employee.SearchCustomerRecord(user));
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadAllData();
        }

        private void loadAllData()
        {
            this.DataContext = user;
        }
    }
}
