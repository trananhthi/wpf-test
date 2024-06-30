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
using DentalClinicManagement.Account;

namespace DentalClinicManagement.Customer
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : Page
    {
        // Biến lưu User từ cửa sổ Sign In
        private CustomerClass user;
        public string Username { get; set; }

        public DashBoard(CustomerClass user)
        {
            InitializeComponent();
            this.user = new CustomerClass(user);
        }

        //public DashBoard()
        //{
        //    InitializeComponent();
        //}

        //private CustomerClass GetCustomer(string username)
        //{
        //    var sql = @"SELECT * FROM Customer WHERE PhoneNo = @PhoneNo";
        //    var command = new SqlCommand(sql, DB.Instance.Connection);
        //    command.Parameters.Add("@PhoneNo", System.Data.SqlDbType.Char)
        //        .Value = username;

        //    var reader = command.ExecuteReader();
        //    if (reader.HasRows)
        //    {
        //        if (reader.Read())
        //        {
        //            CustomerClass cus = new CustomerClass()
        //            {
        //                CustomerID = (int)reader["CustomerID"],
        //                Name = (string)reader["Name"],
        //                DateOfBirth = (DateTime)reader["DateOfBirth"],
        //                Address = (string)reader["Address"],
        //                Gender = (string)reader["Gender"],
        //                PhoneNo = (string)reader["PhoneNo"],
        //                Email = "None",
        //                AccountID = (int)reader["AccountID"]
        //            };

        //            reader.Close();
        //            return cus;
        //        }
        //    }
        //    reader.Close();
        //    return null;
        //}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadAllData();
        }

        private void loadAllData()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.ConnectionString = DB.Instance.ConnectionString;

            //CustomerClass cus = GetCustomer(this.Username);
            this.DataContext = user;
        }

        private void LogOut(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Account.SignIn());
            }
        }

        private void ViewProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                //CustomerClass _cus = GetCustomer(Username);
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.ViewProfile(user));
            }
        }

        private void CheckAppointment(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.CheckAppoinment(user));
            }
        }

        private void ViewRecord(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.ViewRecord(user));
            }
        }
    }
}
