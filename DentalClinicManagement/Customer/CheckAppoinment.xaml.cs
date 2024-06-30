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
    /// Interaction logic for CheckAppoinment.xaml
    /// </summary>
    public partial class CheckAppoinment : Page
    {
        // Biến lưu User từ cửa sổ Sign In
        private CustomerClass user;
        List<DentistAvailableTime>? dentistAvailableTimes;

        public CheckAppoinment(CustomerClass user)
        {
            InitializeComponent();
            this.user = new CustomerClass(user);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = user;
        }

        private void homePageButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.DashBoard(user));
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            DateTime selectedDate = datePicker.SelectedDate ?? DateTime.Now;
            dentistAvailableTimes = DB.GetDentistAvailableTimes(selectedDate);
            listView.ItemsSource = dentistAvailableTimes;
        }

        private void makeAppointment(object sender, MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null)
            {
                var index = listView.SelectedIndex;

                if (index != -1)
                {
                    if(getDAT(dentistAvailableTimes[index]))
                    {
                        MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


                        if (mainWindow != null && mainWindow.MainFrame != null)
                        {
                            mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.MakeAppointment(user, dentistAvailableTimes[index]));
                        }
                    }
                    else
                    {
                        MessageBox.Show("This is not available now");
                        return;
                    }
                        
                    
                    
                    
                }
            }
        }
        private bool getDAT(DentistAvailableTime _dat)
        {
            var dat = DB.GetDentistAvailableTimes(_dat.date, _dat.dentistID, _dat.shift);
            return dat;
            /*if (dat == true)
            {
                retur;
            }
            else
            {
                MessageBox.Show("This is not available now");
                MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


                if (mainWindow != null && mainWindow.MainFrame != null)
                {
                    mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.CheckAppoinment(user));
                }

            }*/
        }

    }
}
