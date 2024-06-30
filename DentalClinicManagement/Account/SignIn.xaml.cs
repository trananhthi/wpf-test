using DentalClinicManagement.Admin;
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
using DentalClinicManagement.Employee;
using DentalClinicManagement.Dentist;
using DentalClinicManagement.Customer;
using System.Data;

namespace DentalClinicManagement.Account
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class SignIn : Page
    {
        const string ADMINISTRATOR = "Administrator";
        const string STAFF = "Staff";
        const string DENTIST = "Dentist";
        const string CUSTOMER = "Customer";

        private Administrator? admin;
        private StaffClass? staff;
        private DentistClass? dentist;
        private CustomerClass? customer;
        private AccountClass? account;

        public SignIn()
        {
            InitializeComponent();
        }
        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Account.Home());
            }
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tạo đối tượng Account
                account = new AccountClass
                {
                    // Lấy thông tin từ giao diện người dùng
                    PhoneNo = UsernameTextBox.Text,
                    Password = PasswordTextBox.Text
                };

                // Kiểm tra đăng nhập
                if (AuthenticateUser(account) == -1)
                {
                    MessageBox.Show("Đăng nhập thất bại. Vui lòng kiểm tra tên người dùng và mật khẩu.");
                    return;
                }
                else if(AuthenticateUser(account) == 0)
                {
                    MessageBox.Show("Đăng nhập thất bại. Tài khoản đã bị khóa.");
                    return;
                }    
                else
                {
                    // Load user's account information
                    account = LoadAccountInformation(account.PhoneNo);
                }

                if (CheckUserRole(ADMINISTRATOR))
                {
                    // Allocate memory
                    admin = new Administrator();

                    // Load user to Admin object 
                    admin = admin.LoadUser(account);
                    if (admin != null)
                    {
                        MessageBox.Show($"Đăng nhập thành công! Chào mừng Administrator: {admin.Name}!");
                        MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

                        if (mainWindow != null && mainWindow.MainFrame != null)
                        {
                            mainWindow.MainFrame.Navigate(new DentalClinicManagement.Admin.DashBoard(admin));
                            return;
                        }
                    }
                }
                if (CheckUserRole(STAFF))
                {
                    // Allocate memory
                    staff = new StaffClass();

                    // Load user to Staff object 
                    staff = staff.LoadUser(account);
                    if (staff != null)
                    {
                        MessageBox.Show($"Đăng nhập thành công. Chào mừng Staff: {staff.Name}!");
                        MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

                        if (mainWindow != null && mainWindow.MainFrame != null)
                        {
                            mainWindow.MainFrame.Navigate(new DentalClinicManagement.Employee.Dashboard(staff));
                            return;
                        }
                    }
                }
                if (CheckUserRole(DENTIST))
                {
                    // Allocate memory
                    dentist = new DentistClass();

                    // Load user to Dentist object 
                    dentist = dentist.LoadUser(account);
                    if (dentist != null)
                    {
                        MessageBox.Show($"Đăng nhập thành công. Chào mừng Dentist: {dentist.Name}!");
                        MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

                        if (mainWindow != null && mainWindow.MainFrame != null)
                        {
                            mainWindow.MainFrame.Navigate(new DentalClinicManagement.Dentist.DashBoard(dentist));
                        }
                    }
                }
                if (CheckUserRole(CUSTOMER))
                {
                    // Allocate memory
                    customer = new CustomerClass();

                    // Load user to Customer object 
                    customer = customer.LoadUser(account);
                    if (customer != null)
                    {
                        MessageBox.Show($"Đăng nhập thành công. Chào mừng Customer: {customer.Name}!");
                        MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

                        if (mainWindow != null && mainWindow.MainFrame != null)
                        {
                            mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.DashBoard(customer));
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private int AuthenticateUser(AccountClass user)
        {
            try
            {
                // Câu truy vấn SQL để kiểm tra đăng nhập
                string query = "SELECT TOP 1 * FROM [Account] WHERE PhoneNo = @Username AND Password = @Password";

                using (DB.Instance.Connection)
                {
                    // Tạo đối tượng SqlCommand
                    using (SqlCommand command = new SqlCommand(query, DB.Instance.Connection))
                    {
                        // Thêm các tham số
                        command.Parameters.AddWithValue("@Username", user.PhoneNo);
                        command.Parameters.AddWithValue("@Password", user.Password);

                        // Thực hiện truy vấn
                        //int count = (int)command.ExecuteScalar();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Kiểm tra xem có dữ liệu hay không
                            if (reader.HasRows)
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                DataRow row = dataTable.Rows[0];
                                using (SqlCommand command2 = new SqlCommand($"Select Status from Account where AccountID = {Convert.ToInt32(row["AccountID"])}", DB.Instance.Connection))
                                {
                                    object statusObj = command2.ExecuteScalar();
                                    int status = Convert.ToInt32(statusObj);

                                    // Kiểm tra xem giá trị Status có null không
                                    if (status == 0)
                                    {
                                        return 0;
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }
                            }
                            else
                            {
                                // Nếu không có dữ liệu, có thể xử lý tương ứng ở đây
                                return -1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return -1;
            }
        }

        private AccountClass? LoadAccountInformation(string? phoneNo)
        {
            try
            {
                // Câu truy vấn SQL để lấy thông tin Account từ database dựa trên PhoneNo
                string query = "SELECT TOP 1* FROM [Account] WHERE PhoneNo = @PhoneNo";

                // Tạo và mở kết nối
                DB dB = new DB();
                using (SqlConnection connection = dB.Connection)
                {
                    // Tạo đối tượng SqlCommand
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Thêm tham số cho câu truy vấn
                        command.Parameters.AddWithValue("@PhoneNo", phoneNo);

                        // Sử dụng SqlDataReader để đọc dữ liệu từ database
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Kiểm tra xem có dữ liệu hay không
                            if (reader.Read())
                            {
                                // Tạo đối tượng AccountClass từ SqlDataReader
                                AccountClass account = new AccountClass(reader);
                                return account;
                            }
                            else
                            {
                                // Trường hợp không tìm thấy thông tin AccountClass
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }

        private bool CheckUserRole(string ROLE)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT COUNT(*) FROM [");
                builder.Append(ROLE);
                builder.Append("] WHERE AccountID = @AccountID");
                string query = builder.ToString();

                // Tạo và mở kết nối
                DB dB = new DB();
                using (SqlConnection connection = dB.Connection)
                {
                    // Tạo đối tượng SqlCommand
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Thêm tham số cho câu truy vấn
                        command.Parameters.AddWithValue("@AccountID", account.AccountID);
                        int count = (int)command.ExecuteScalar();

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }

        private void signInAdmin_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            //if (mainWindow != null && mainWindow.MainFrame != null)
            //{
            //    Admin.DashBoard dashBoard = new Admin.DashBoard(admin);
            //    dashBoard.Username = UsernameTextBox.Text;
            //    mainWindow.MainFrame.Navigate(dashBoard);
            //}

            MessageBox.Show($"Đăng nhập để vào trang Admin");
        }

        private void signInDentist_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            //if (mainWindow != null && mainWindow.MainFrame != null)
            //{
            //    Dentist.DashBoard dashBoard = new Dentist.DashBoard();
            //    dashBoard.Username = UsernameTextBox.Text;
            //    mainWindow.MainFrame.Navigate(dashBoard);
            //}

            MessageBox.Show($"Đăng nhập để vào trang Dentist");
        }

        private void signInEmployee_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            //if (mainWindow != null && mainWindow.MainFrame != null)
            //{
            //    Employee.Dashboard dashboard = new Employee.Dashboard();
            //    dashboard.Username = UsernameTextBox.Text;
            //    mainWindow.MainFrame.Navigate(dashboard);
            //}

            MessageBox.Show($"Đăng nhập để vào trang Staff");
        }

        private void signInCustomer_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            //if (mainWindow != null && mainWindow.MainFrame != null)
            //{
            //    string username = UsernameTextBox.Text;
            //    Customer.DashBoard dashboard = new Customer.DashBoard(c);
            //    dashboard.Username = username;
            //    mainWindow.MainFrame.Navigate(dashboard);
            //}

            MessageBox.Show($"Đăng nhập để vào trang Customer");
        }
    }
}
