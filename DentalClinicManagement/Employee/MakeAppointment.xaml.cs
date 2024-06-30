using DentalClinicManagement.Account;
using DentalClinicManagement.Dentist;
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
    /// Interaction logic for MakeAppointment.xaml
    /// </summary>
    public partial class MakeAppointment : Page
    {
        StaffClass user;
        public MakeAppointment(StaffClass user, DentistAvailableTime dat)
        {
            InitializeComponent();
            this.user = new StaffClass(user);
            _dat = dat;
            
        }
        private void getDAT()
        {
            var dat = DB.GetDentistAvailableTimes(_dat.date, _dat.dentistID, _dat.shift);
            if (dat == true)
            {
                
                return;
            }
            else
            {
                MessageBox.Show("This AvailableTimes has been changed");
                MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


                if (mainWindow != null && mainWindow.MainFrame != null)
                {
                    mainWindow.MainFrame.Navigate(new DentalClinicManagement.Employee.Dashboard(user));
                }
            }
        }
        private DentistAvailableTime _dat;
        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerClass _cus = DB.LoadCustomerInfo(txtPhone.Text);

            if(_cus == null)
            {
                MessageBox.Show("Cus not exits");
                return;
            }
            string msg;
            if(txtMessage.Text.Length == 0)
            {
                msg = "Ghi Chu 1";
            }
            else
            {
                msg = txtMessage.Text;
            }
            int rs = DB.MakeAppointment(_dat, _cus.CustomerID, msg);
            
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Employee.CheckAppoinment(user));
            }
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Employee.Dashboard(user));
            }
        }

        private void loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = _dat;
            //getDAT();
        }
    }
}
