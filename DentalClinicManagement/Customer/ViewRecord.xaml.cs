using DentalClinicManagement.Account;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DentalClinicManagement.Customer
{
    /// <summary>
    /// Interaction logic for ViewRecord.xaml
    /// </summary>
    public partial class ViewRecord : Page
    {
        // Biến lưu User từ cửa sổ Sign In
        private CustomerClass user;
        public ObservableCollection<RecordList>? listRecords { get; set; }

        public ViewRecord(CustomerClass user)
        {
            InitializeComponent();
            this.user = new CustomerClass(user);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadAllData();
        }

        private void loadAllData()
        {
            this.DataContext = user;

            var sql = @"SELECT
	                    Appointment.Date AS dateOfAppointment,
	                    MedicationList.Name AS medicalList,
	                    Dentist.Name AS DentistName,
	                    Payment.TotalPrice AS totalPrice
                    FROM Appointment JOIN AppointmentRecord  ON Appointment.AppointmentID = AppointmentRecord.AppointmentID
				                     JOIN MedicationRecord  ON AppointmentRecord.AppointRecordID = MedicationRecord.AppointRecordID
				                     JOIN MedicationList ON MedicationRecord.MedicalID = MedicationList.MedicalID
				                     JOIN Dentist ON Appointment.DentistID = Dentist.DentistID
				                     JOIN Payment ON AppointmentRecord.AppointRecordID = Payment.AppointmentRecordID
				                     JOIN Customer ON Appointment.CustomerID = Customer.CustomerID
				                     JOIN ServiceDetail ON AppointmentRecord.AppointRecordID = ServiceDetail.AppointmentRecordID
                    WHERE Customer.PhoneNo = @PhoneNo";

            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@PhoneNo", System.Data.SqlDbType.Char)
                .Value = user.PhoneNo;

            var reader = command.ExecuteReader();
            listRecords = new ObservableCollection<RecordList>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    RecordList record = new RecordList()
                    {
                        dateOfAppointment = (DateTime)reader["dateOfAppointment"],
                        medicalList = (string)reader["medicalList"],
                        DentistName = (string)reader["DentistName"],
                        totalPrice = (decimal)reader["totalPrice"]
                    };

                    listRecords.Add(record);
                }

                reader.Close();
                dataGrid.ItemsSource = listRecords;
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.DashBoard(user));
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem != null && dataGrid.SelectedItems.Count == 1)
            {
                RecordList selectedRecord = (RecordList)dataGrid.SelectedItem;

                dateTextBlock.Text = selectedRecord.dateOfAppointment.ToString();
                medicineTextBlock.Text = selectedRecord.medicalList;
                dentistTextBlock.Text = selectedRecord.DentistName;
                paymentTextBlock.Text = selectedRecord.totalPrice.ToString();
            }
            else
            {
                dateTextBlock.Text = string.Empty;
                medicineTextBlock.Text = string.Empty;
                dentistTextBlock.Text = string.Empty;
                paymentTextBlock.Text = string.Empty;
            }
        }
    }
}
