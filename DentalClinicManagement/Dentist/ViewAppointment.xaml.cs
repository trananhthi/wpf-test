using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Data.SqlClient;
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
using System.Data;

namespace DentalClinicManagement.Dentist
{
    /// <summary>
    /// Interaction logic for ViewAppointment.xaml
    /// </summary>
    public partial class ViewAppointment : Page
    {
        public DentistClass user;
        public ObservableCollection<Appointment> Appointment { get; set; }
        private readonly string[] StatusArray = {"Đã hủy", "Quá hẹn", "Waiting", "Not Available" };

        public ViewAppointment(DentistClass user)
        {
            InitializeComponent();
            this.user = new DentistClass(user);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadAllData();
        }

        private void loadAllData()
        {
            try
            {
                DataContext = this;
                sttTextBlock.IsEnabled = false;
                idTextBlock.IsEnabled = false;
                nameTextBox.IsEnabled = false;
                phoneTextBox.IsEnabled = false;
                shiftTextBox.IsEnabled = false;
                timeTextBox.IsEnabled = false;
                dentistIDTextBlock.IsEnabled = false;
                noteTextBox.IsEnabled = false;
                updateStatusComboBox.IsEnabled = false;

                var i = 1;
                Appointment = new ObservableCollection<Appointment>();
                // view dentist available time
                var sql = @"SELECT * FROM DentistsAvailTime
                        WHERE DentistID = @DentistID";
                var command = new SqlCommand(sql, DB.Instance.Connection);
                command.Parameters.Add("@DentistID", SqlDbType.Int).Value = user.DentistID;
                string cStatus;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int stt = i;
                    int dentistID = (int)reader["DentistID"];
                    if ((bool)reader["Status"] == false)
                    {
                        cStatus = "Available";
                    }
                    else
                    {
                        cStatus = "Not Available";
                    }
                    Appointment.Add(new Appointment
                    {
                        STT = i,
                        CustomerName = "",
                        CustomerPhone = "",
                        AppointmentID = 0,
                        DentistID = dentistID,
                        Shift = (int)reader["Shift"],
                        Date = (DateTime)reader["Date"],
                        Note = "",
                        Status = cStatus,
                        CustomerID = 0
                    });

                    i++;
                }
                reader.Close();
                // use for dirty read
                command = new SqlCommand("View_Appointment", DB.Instance.Connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                command.Parameters.AddWithValue("@MANS", user.DentistID);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //int stt = i;
                    int appointmentID = (int)reader["AppointmentID"];
                    int dentistID = (int)reader["DentistID"];
                    int shift = (int)reader["Shift"];
                    DateTime date = (DateTime)reader["Date"];
                    int customerID = (int)reader["CustomerID"];
                    string note = (string)reader["Note"];
                    string status = (string)reader["Status"];
                    string customerName = (string)reader["CustomerName"];
                    string customerPhone = (string)reader["CustomerPhone"];

                    foreach(var apm in Appointment)
                    {
                        if(apm.DentistID == dentistID && apm.Date == date && apm.Shift == shift)
                        {
                            //apm.STT = i;
                            apm.CustomerName = customerName;
                            apm.CustomerPhone = customerPhone;
                            apm.AppointmentID = appointmentID;
                            apm.DentistID = dentistID;
                            apm.Shift = shift;
                            apm.Date = date;
                            apm.Note = note;
                            apm.Status = status;
                            apm.CustomerID = customerID;
                            break;
                        }
                    }

                    /*Appointment.Add(new Appointment
                    {
                        STT = i,
                        CustomerName = customerName,
                        CustomerPhone = customerPhone,
                        AppointmentID = appointmentID,
                        DentistID = dentistID,
                        Shift = shift,
                        Date = date,
                        Note = note,
                        Status = status,
                        CustomerID = customerID
                    });*/

                   // i++;
                }

                reader.Close();

                
                appointmentDataGrid.ItemsSource = Appointment;
                
            } catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void appointmentDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Chỉnh màu sắc cho các hàng chẵn và lẻ
            if (e.Row.Item is Appointment appointment && appointment.STT % 2 == 0)
            {
                e.Row.Background = Brushes.LightBlue; // Màu cho hàng chẵn
            }
            else
            {
                e.Row.Background = Brushes.White; // Màu cho hàng lẻ hoặc khi giá trị STT là lẻ
            }
        }

        

        private void home_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DashBoard(user));
            }
        }


        private void FilterDataGrid()
        {
            DateTime? fromDate = fromDatePicker.SelectedDate;
            DateTime? toDate = toDatePicker.SelectedDate;

            string selectedStatus = ((ComboBoxItem)statusComboBox.SelectedItem)?.Content.ToString() ?? "Tất cả";

            // Kiểm tra nếu cả hai DatePicker đều đã được chọn và một trạng thái đã được chọn
            if (fromDate.HasValue && toDate.HasValue && selectedStatus != null)
            {
                if (fromDate.Value > toDate.Value)
                {
                    MessageBox.Show("Vui lòng chọn ngày bắt đầu và kết thúc hợp lệ", "Ngày không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Lọc dữ liệu trong khoảng từ fromDate đến toDate và theo trạng thái
                    appointmentDataGrid.ItemsSource = Appointment.Where(c => c.Date >= fromDate && c.Date <= toDate && (selectedStatus == "Tất cả" || c.Status == selectedStatus));
                }
            }
            if (!fromDate.HasValue && !toDate.HasValue && selectedStatus != null)
            {
                if (selectedStatus == "Tất cả")
                {
                    appointmentDataGrid.ItemsSource = Appointment; // Hiển thị tất cả dữ liệu nếu chọn "Tất cả"
                }
                else
                {
                    appointmentDataGrid.ItemsSource = Appointment.Where(c => c.Status == selectedStatus);
                }
            }
            if (fromDate.HasValue && !toDate.HasValue && selectedStatus != null)
            {
                appointmentDataGrid.ItemsSource = Appointment.Where(c => c.Date >= fromDate && (selectedStatus == "Tất cả" || c.Status == selectedStatus));
            }
        }

        private void Filter_Button_Click(object sender, RoutedEventArgs e)
        {
            FilterDataGrid();
        }


        string phoneKey = "";
        int shiftKey = 0;
        int dentistIDKey = 0;

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (appointmentDataGrid.SelectedItem != null)
            {
                Appointment selectedAppointment = (Appointment)appointmentDataGrid.SelectedItem;

                if(selectedAppointment.CustomerPhone != "")
                {
                    nameTextBox.IsEnabled = false;
                    phoneTextBox.IsEnabled = false;
                    shiftTextBox.IsEnabled = false;
                    timeTextBox.IsEnabled = false;
                    noteTextBox.IsEnabled = false;
                    updateStatusComboBox.IsEnabled = true;
                    // Cập nhật giá trị cho các TextBlock
                    sttTextBlock.Text = selectedAppointment.STT.ToString();
                    idTextBlock.Text = selectedAppointment.AppointmentID.ToString();
                    nameTextBox.Text = selectedAppointment.CustomerName;
                    phoneTextBox.Text = selectedAppointment.CustomerPhone;
                    phoneKey = selectedAppointment.CustomerPhone;
                    shiftKey = selectedAppointment.Shift;
                    shiftTextBox.Text = shiftKey.ToString();
                    dentistIDKey = selectedAppointment.DentistID;
                    dentistIDTextBlock.Text = dentistIDKey.ToString();
                    timeTextBox.Text = Convert.ToDateTime(selectedAppointment.Date).ToString("dd/MM/yyyy");
                    noteTextBox.Text = selectedAppointment.Note;
                    updateStatusComboBox.Text = selectedAppointment.Status;
                }
                else
                {
                    nameTextBox.IsEnabled = true;
                    phoneTextBox.IsEnabled = true;
                    shiftTextBox.IsEnabled = false;
                    timeTextBox.IsEnabled = false;
                    noteTextBox.IsEnabled = true;
                    updateStatusComboBox.IsEnabled = true;
                    // Cập nhật giá trị cho các TextBlock
                    sttTextBlock.Text = "";
                    idTextBlock.Text = "";
                    shiftKey = selectedAppointment.Shift;
                    shiftTextBox.Text = shiftKey.ToString();
                    dentistIDKey = selectedAppointment.DentistID;
                    dentistIDTextBlock.Text = dentistIDKey.ToString();
                    timeTextBox.Text = Convert.ToDateTime(selectedAppointment.Date).ToString("dd/MM/yyyy");
                }
            }
            else
            {
                // Không có dòng nào được chọn

                // Cập nhật giá trị và tắt trạng thái Enabled cho các TextBlock và TextBox
                sttTextBlock.Text = string.Empty;
                idTextBlock.Text = string.Empty;
                nameTextBox.Text = string.Empty;
                phoneTextBox.Text = string.Empty;
                shiftTextBox.Text = string.Empty;
                timeTextBox.Text = string.Empty;
                dentistIDTextBlock.Text = string.Empty;
                noteTextBox.Text = string.Empty;
                updateStatusComboBox.Text = string.Empty;

                sttTextBlock.IsEnabled = false;
                idTextBlock.IsEnabled = false;
                nameTextBox.IsEnabled = false;
                phoneTextBox.IsEnabled = false;
                shiftTextBox.IsEnabled = false;
                timeTextBox.IsEnabled = false;
                dentistIDTextBlock.IsEnabled = false;
                noteTextBox.IsEnabled = false;
                updateStatusComboBox.IsEnabled = false;
            }
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = nameTextBox.Text;
                string phone = phoneTextBox.Text;
                string shift = shiftTextBox.Text;
                string note = noteTextBox.Text;
                string newDate = timeTextBox.Text;
                string status = updateStatusComboBox.Text;
                Appointment selectedAppointment = (Appointment)appointmentDataGrid.SelectedItem;
                if(selectedAppointment.CustomerPhone != "")
                {
                    var sql = @"update Appointment
                            set Status = @Status
                            where CustomerID = (select CustomerID
                                                from Customer
                                                where @CustomerName = Name 
                                                and @Phone = PhoneNo)
                            and Shift = @Shift
                            and Date = @Date
                            and DentistID = @DentistID";
                    DateTime proDateTime = DateTime.ParseExact(newDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var command = new SqlCommand(sql, DB.Instance.Connection);
                    command.Parameters.Add("@CustomerName", SqlDbType.NVarChar).Value = name;
                    command.Parameters.Add("@Shift", SqlDbType.Int).Value = int.Parse(shiftTextBox.Text);
                    command.Parameters.Add("@Date", SqlDbType.Date).Value = proDateTime;
                    command.Parameters.Add("@Status", SqlDbType.NVarChar).Value = status;
                    command.Parameters.Add("@DentistID", SqlDbType.Int).Value = user.DentistID;
                    command.Parameters.Add("@Phone", SqlDbType.Char).Value = phoneKey;

                    int rs = (int)command.ExecuteNonQuery();

                    if (rs >= 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thành công!", MessageBoxButton.OK, MessageBoxImage.Information);
                        loadAllData();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại!", "Thất bại!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                } 
                else
                {
                    var sql = "";
                    if (status == "Not Available")
                    {
                        using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
                        {
                            connection.Open();
                            // Use for dirtyread
                            var command = new SqlCommand("Update_AvailableTime", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            DateTime proDateTime = DateTime.ParseExact(newDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            // Add parameters
                            command.Parameters.AddWithValue("@MANS", user.DentistID);
                            command.Parameters.AddWithValue("@Date", proDateTime);
                            command.Parameters.AddWithValue("@SHIFT", int.Parse(shiftTextBox.Text));

                            // Execute the command
                            int rs = command.ExecuteNonQuery();

                            if (rs >= 0)
                            {
                                MessageBox.Show("Thêm thành công!", "Thành công!", MessageBoxButton.OK, MessageBoxImage.Information);
                                loadAllData();
                            }
                            else
                            {
                                MessageBox.Show("Thêm thất bại!", "Thất bại!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        // insert appointment
                        sql = @"insert into Appointment(CustomerID, DentistID, Shift, Date, Note, Status)
                                values((select CustomerID
                                        from Customer
                                        where @CustomerName = Name 
                                        and @Phone = PhoneNo), @DentistID, @Shift, @Date, @Note, @Status)" +
                                    @"update DentistsAvailTime 
                                set Status = 1
                                where @DentistID = DentistID
	                            and @Date = Date
	                            and @Shift= Shift";
                        DateTime proDateTime = DateTime.ParseExact(newDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        var command = new SqlCommand(sql, DB.Instance.Connection);
                        command.Parameters.Add("@CustomerName", SqlDbType.NVarChar)
                            .Value = name;
                        command.Parameters.Add("@Phone", SqlDbType.Char)
                            .Value = phone;
                        command.Parameters.Add("@DentistID", SqlDbType.Int)
                            .Value = user.DentistID;
                        command.Parameters.Add("@Shift", SqlDbType.Int)
                            .Value = int.Parse(shiftTextBox.Text);
                        command.Parameters.Add("@Date", SqlDbType.Date)
                            .Value = proDateTime;
                        command.Parameters.Add("@Note", SqlDbType.NVarChar)
                            .Value = note;
                        command.Parameters.Add("@Status", SqlDbType.NVarChar)
                            .Value = "Waiting";

                        int rs = (int)command.ExecuteNonQuery();
                        if (rs >= 0)
                        {
                            MessageBox.Show("Thêm thành công!", "Thành công!", MessageBoxButton.OK, MessageBoxImage.Information);
                            loadAllData();
                        }
                        else
                        {
                            MessageBox.Show("Thêm thất bại!", "Thất bại!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Cập nhật thất bại!\n{ex.Message}", "Thất bại!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void addRecord(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                // Lấy chỉ mục của hàng được chọn
                int selectedIndex = grid.SelectedIndex;

                
                if (selectedIndex != -1)
                {
                    if (Appointment[selectedIndex].Status!= "Waiting")
                    {
                        
                        MessageBox.Show("Is not waiting appointment");
                        return;
                    }
                    MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


                    if (mainWindow != null && mainWindow.MainFrame != null)
                    {
                        
                        mainWindow.MainFrame.Navigate(new DentalClinicManagement.Dentist.AddRecord(user, Appointment[selectedIndex]));
                    }
                }
                else
                {
                    MessageBox.Show("Please select Appointment");
                }
            }
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                AddAvailableTime addAvailTime = new AddAvailableTime(user);
                addAvailTime.user = user;
                mainWindow.MainFrame.Navigate(addAvailTime);
            }
        }
    }

    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                switch (status)
                {
                    case "Quá hẹn":
                        return Brushes.Yellow; // Màu vàng cho trạng thái "Quá hẹn"
                    case "Đã hủy":
                        return Brushes.Red; // Màu đỏ cho trạng thái "Đã hủy"
                    case "Đã xong":
                    case "Not Available":
                        return Brushes.Transparent; // Màu xanh lá cho trạng thái "Đã xong"
                    default:
                        return Brushes.Green; // Mặc định màu trong suốt
                }
            }

            return Brushes.Transparent; // Mặc định màu trong suốt
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
