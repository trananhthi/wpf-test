using DentalClinicManagement.Account;
using DentalClinicManagement.Customer;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for CheckBill.xaml
    /// </summary>
    public partial class CheckBill : Page
    {
        StaffClass user;
        decimal paidAmount;

        public CheckBill(StaffClass user, CustomerClass _c, RecordList _r)
        {
            _cus = _c;
            _rec = _r;
            InitializeComponent();
            this.user = new StaffClass(user);
        }

        private CustomerClass _cus { get; set; }

        private RecordList _rec { get; set; }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Employee.SearchCustomerRecord(user));
            }
        }

        private void medicines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void payButton_Click(object sender, RoutedEventArgs e)
        {
            //DB.setPayment(_rec.AppointRecordID, _rec.dateOfAppointment);
            //MessageBox.Show("Success", "Message", MessageBoxButton.OK, MessageBoxImage.Information);

            // Get paid amount update and convert to money type
            paidAmount = decimal.TryParse(MakePaymentTextBox.Text, out decimal paid) ? paid : 0;

            int isSuccess = DB.setFullPayment(_rec.AppointRecordID, _rec.dateOfAppointment, paidAmount);
            if (isSuccess == 1)
            {
                MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

                if (mainWindow != null && mainWindow.MainFrame != null)
                {
                    mainWindow.MainFrame.Navigate(new DentalClinicManagement.Employee.SearchCustomerRecord(user));
                }
            }
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            txtFullname.Text = _cus.Name;
            txtBirthday.Text = _cus.DateOfBirth.ToString();
            txtPhone.Text = _cus.PhoneNo;
            txtAddress.Text = _cus.Address;
            txtDentis.Text = _rec.DentistName;
            txtService.Text = _rec.serviceList;
            txtThuoc.Text = _rec.medicalList;
            txtNgayKham.Text = _rec.dateOfAppointment.ToString();
            txtPrice.Text = _rec.totalPrice.ToString();

            // Add default payment equals to total payment
            MakePaymentTextBox.Text = txtPrice.Text;

        }
    }
}
