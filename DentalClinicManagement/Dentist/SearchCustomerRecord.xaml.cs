using DentalClinicManagement.Customer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for SearchCustomerRecord.xaml
    /// </summary>
   
    public partial class SearchCustomerRecord : Page
    {
        DentistClass user;
        CustomerRecord? _customer;

        public SearchCustomerRecord(DentistClass user)
        {
            InitializeComponent();
            this.user = new DentistClass(user);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPhoneSearch.Text))
            {
                _customer = new CustomerRecord();
                _customer.CustomerDetail = DB.LoadCustomerInfo(txtPhoneSearch.Text);
                if (_customer.CustomerDetail != null)
                {
                    this.DataContext = _customer.CustomerDetail;
                    _customer.RecordList = DB.GetCustomerRecordList(txtPhoneSearch.Text);
                    listView.ItemsSource = _customer.RecordList;
                }
                else
                {
                    MessageBox.Show("Phone number not exits");
                }
            }
            else
            {
                MessageBox.Show("Please enter phone number");
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Dentist.DashBoard(user));
            }
        }

        private void AddRecord(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                //mainWindow.MainFrame.Navigate(new DentalClinicManagement.Dentist.AddRecord(_customer.CustomerDetail));
            }
        }
    }
}
