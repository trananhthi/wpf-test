using DentalClinicManagement.Account;
using DentalClinicManagement.Dentist;
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

namespace DentalClinicManagement.Customer
{
    /// <summary>
    /// Interaction logic for MakeAppointment.xaml
    /// </summary>
    public partial class MakeAppointment : Page
    {
        // Biến lưu User từ cửa sổ Sign In
        private CustomerClass user;
        private DentistAvailableTime _dat;

        public MakeAppointment(CustomerClass user, DentistAvailableTime dat)
        {
            InitializeComponent();
            this.user = new CustomerClass(user);
            _dat = dat;
            
        }
        private void getDAT()
        {
            var dat = DB.GetDentistAvailableTimes(_dat.date, _dat.dentistID, _dat.shift);
            if (dat == true)
            {
                
            }
            else
            {
                MessageBox.Show("This is not available now");
                MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


                if (mainWindow != null && mainWindow.MainFrame != null)
                {
                    mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.CheckAppoinment(user));
                }

            }
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            string msg;
            if (txtMessage.Text.Length == 0)
            {
                msg = "Ghi Chu 1";
            }
            else
            {
                msg = txtMessage.Text;
            }
            int rs = DB.MakeAppointment(_dat, user.CustomerID, msg);
            
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.CheckAppoinment(user));
            }
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.DashBoard(user));
            }
        }

        private void loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = _dat;
            txtPhone.Text = user.PhoneNo;
            //getDAT();
        }
    }
}
