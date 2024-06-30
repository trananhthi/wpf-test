using DentalClinicManagement.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for AddRecord.xaml
    /// </summary>
    public partial class AddRecord : Page
    {
        public DentistClass user;
        public AddRecord(DentistClass user, Appointment _app)
        {
            InitializeComponent();
            this.user = new DentistClass(user);
            this._app = _app;

        }

        public AddRecord()
        {
            InitializeComponent();
        }

        private Appointment _app;
        private BindingList<Medicine> _mec;
        private BindingList<Service> _ser;
        private BindingList<Service> _service;
        private BindingList<Medicine> _medicine;
        private void backHome(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Dentist.ViewAppointment(user));
            }
        }

        private void SaveChange(object sender, RoutedEventArgs e)
        {
            Decimal fee = 0;
            List<Service> serviceSelected = new List<Service> ();
            foreach (Service service in _service) { 
                serviceSelected.Add(service);
                fee += service.price;
            }
            Decimal medicinePrice = 0;
            List<Medicine> medicineSelected = new List<Medicine> ();
            foreach (Medicine medicine in _medicine)
            {
                medicinePrice += medicine.price*medicine.quantity;
                medicineSelected.Add(medicine);
            }
            int recID = DB.addPatientRecord(_app.CustomerID);
            int appRecID = DB.addAppointmentRecord(recID, _app.AppointmentID);
            DB.addServiceDetail(serviceSelected, appRecID);
            DB.addMedicationRecord(medicineSelected, appRecID);
            DB.AddPayment(_app.Date, fee, fee + medicinePrice, appRecID);
            DB.updateAppointmentComplete(_app);
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Dentist.ViewAppointment(user));
            }
        }

        private void loaded(object sender, RoutedEventArgs e)
        {
            txtFullname.Text = _app.CustomerName;
            txtPhone.Text = _app.CustomerPhone;
            txtAddress.Text = _app.CustomerID.ToString();
            txtBirthday.Text = _app.AppointmentID.ToString();
            txtDate.Text = _app.Date.ToString();
            _mec = DB.getAllMedicines();
            medicalListComboBox.ItemsSource = _mec;
            _ser = DB.getAllServices();
            serviceListComboBox.ItemsSource = _ser;
            _service = new BindingList<Service>();
            serviceListView.ItemsSource = _service;
            _medicine = new BindingList<Medicine>();
            medicalListView.ItemsSource = _medicine;

            this.DataContext = user;
        }

        private void deleteService(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView)
            {
                // Lấy mục được chọn trong ListView
                var selectedIndex = listView.SelectedIndex;

                // Kiểm tra xem có mục nào được chọn không
                if (selectedIndex != -1)
                {
                    Service selectedService = _service[selectedIndex];

                    // Thêm service đã chọn vào _service
                    _ser.Add(selectedService);

                    // Xóa service đã chọn khỏi _ser
                    _service.RemoveAt(selectedIndex);
                }
            }
        }

        private void deleteMedical(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView)
            {
                // Lấy mục được chọn trong ListView
                var selectedIndex = listView.SelectedIndex;

                // Kiểm tra xem có mục nào được chọn không
                if (selectedIndex != -1)
                {
                    Medicine selectedService = _medicine[selectedIndex];
                    _mec.Add(selectedService);

                    // Xóa service đã chọn khỏi _ser
                    _medicine.RemoveAt(selectedIndex);
                }
            }
        }

        private void selectService(object sender, RoutedEventArgs e)
        {
            var index = serviceListComboBox.SelectedIndex;
            if (index == -1)
            {
                MessageBox.Show("please select service to add");
                return;
            }
            else
            {
                Service selectedService = _ser[index];

                // Thêm service đã chọn vào _service
                _service.Add(selectedService);

                // Xóa service đã chọn khỏi _ser
                _ser.RemoveAt(index);
            }
        }

        private void addMedicine(object sender, RoutedEventArgs e)
        {
            var index = medicalListComboBox.SelectedIndex;
            if (index == -1)
            {
                MessageBox.Show("please select medicine to add");
                return;
            }
            else
            {
                Medicine selectedService = _mec[index];

                // Thêm service đã chọn vào _service
                _medicine.Add(selectedService);

                // Xóa service đã chọn khỏi _ser
                _mec.RemoveAt(index);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
