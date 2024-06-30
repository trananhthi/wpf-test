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

namespace DentalClinicManagement.Admin
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>

    public partial class DashBoard : Page
    {
        Administrator admin;
        //public string Username { get; set; }

        public DashBoard(Administrator admin)
        {
            InitializeComponent();
            this.admin = new Administrator(admin);
        }

        //private Administrator GetAdmin(string username)
        //{
        //    var sql = @"SELECT * FROM Administrator WHERE PhoneNo = @PhoneNo";
        //    var command = new SqlCommand(sql, DB.Instance.Connection);
        //    command.Parameters.Add("@PhoneNo", System.Data.SqlDbType.Char)
        //        .Value = username;

        //    var reader = command.ExecuteReader();
        //    if (reader.HasRows)
        //    {
        //        if (reader.Read())
        //        {
        //            int adID = (int)reader["AdministratorID"];
        //            string name = (string)reader["Name"];
        //            DateTime dob = ((DateTime)reader["DateOfBirth"]);
        //            string address = (string)reader["Address"];
        //            string gender = (string)reader["Gender"];
        //            string phoneNo = (string)reader["PhoneNo"];
        //            int accID = (int)reader["AccountID"];
        //            Administrator cus = new Administrator()
        //            {
        //                AdministratorID = adID,
        //                Name = name,
        //                DateOfBirth = dob,
        //                Address = address,
        //                Gender = gender,
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

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            loadAllData();
        }

        private void loadAllData()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.ConnectionString = DB.Instance.ConnectionString;
            this.DataContext = admin;
        }

        private void viewAccount(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Admin.AccountManagement(admin));
            }
        }

        private void viewMedical(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Admin.MedicalManagement(admin));
            }
        }

        private void logOut(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Account.Home());
            }
        }
    }
}
