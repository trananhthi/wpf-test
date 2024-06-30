using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
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
    /// Interaction logic for AddAppointment.xaml
    /// </summary>
    public partial class AddAvailableTime : Page
    {
        public DentistClass user;
        public AddAvailableTime(DentistClass user)
        {
            InitializeComponent();
            this.user = new DentistClass(user);

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dentistIDTextBlock.Text = user.DentistID.ToString();
        }

        private void home_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                ViewAppointment viewAppointment = new ViewAppointment(user);
                viewAppointment.user = user;
                mainWindow.MainFrame.Navigate(viewAppointment);
            }
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = DB.Instance.Connection)
                {
                    using (SqlCommand command = new SqlCommand(("SP_DIRTYREAD_3"), connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@DentistID", SqlDbType.Int).Value = int.Parse(dentistIDTextBlock.Text);
                        command.Parameters.Add("@Shift", SqlDbType.Int).Value = int.Parse(shiftTextBox.Text);
                        DateTime selectedDate = timeTextBox.SelectedDate ?? DateTime.Now;
                        command.Parameters.Add("@Date", SqlDbType.Date).Value = selectedDate;

                        int rs = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Thêm lịch rảnh thất bại!\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
