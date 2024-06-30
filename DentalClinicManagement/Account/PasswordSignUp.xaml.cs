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
using DentalClinicManagement.Account;

namespace DentalClinicManagement.Account
{
    /// <summary>
    /// Interaction logic for PasswordSignUp.xaml
    /// </summary>
    public partial class PasswordSignUp : Page
    {
        // Biến lưu PhoneNo từ bảng Customer
        private string customerPhoneNo;

        public PasswordSignUp(string phoneNoFromCustomer)
        {
            InitializeComponent();
            customerPhoneNo = new string(phoneNoFromCustomer);
        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Account.SignUp());
            }
        }

        private void FinishSignUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy thông tin từ giao diện người dùng
                string password = PasswordTextBox.Text;
                string confirmPassword = ConfirmPasswordTextBox.Text;

                // Kiểm tra mật khẩu và xác nhận mật khẩu
                if (password == confirmPassword)
                {
                    // Tạo đối tượng AccountClass
                    AccountClass newAccount = new AccountClass
                    {
                        Password = password,
                        PhoneNo = customerPhoneNo
                        //Status = status
                    };
                    // Mật khẩu khớp, tiến hành ghi vào bảng Account
                    if (RegisterAccount(newAccount))
                    {
                        MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

                        if (mainWindow != null && mainWindow.MainFrame != null)
                        {
                            mainWindow.MainFrame.Navigate(new DentalClinicManagement.Account.SignUpSuccess());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Đăng ký tài khoản thất bại. Vui lòng thử lại.");
                    }
                }
                else
                {
                    MessageBox.Show("Mật khẩu và xác nhận mật khẩu không khớp.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private bool RegisterAccount(AccountClass account)
        {
            // Tạo và mở kết nối
            //DB dB = new DB();
            using (SqlConnection connection = DB.Instance.Connection)
            {
                // Bắt đầu giao dịch
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Câu truy vấn SQL để thêm dữ liệu vào bảng Account
                        string query = "INSERT INTO Account (PhoneNo, Password) " +
                                       "VALUES (@PhoneNo, @Password)";

                        // Thực hiện truy vấn 1
                        using (SqlCommand command = new SqlCommand(query, connection, transaction))
                        {
                            // Thêm các tham số
                            command.Parameters.AddWithValue("@PhoneNo", account.PhoneNo);
                            command.Parameters.AddWithValue("@Password", account.Password);
                            //command.Parameters.AddWithValue("@Status", 1);

                            // Thực hiện truy vấn
                            command.ExecuteNonQuery();
                        }

                        // Sau khi đăng ký được tài khoản, load thông tin tài khoản lên để lấy AccountID và update Customer
                        // Câu truy vấn SQL để lấy thông tin Admin từ database dựa trên PhoneNo
                        string query2 = "SELECT TOP 1* FROM [Account] WHERE PhoneNo = @PhoneNo";

                        // Tạo đối tượng SqlCommand
                        using (SqlCommand command2 = new SqlCommand(query2, connection, transaction))
                        {
                            // Thêm tham số cho câu truy vấn
                            command2.Parameters.AddWithValue("@PhoneNo", customerPhoneNo);

                            // Sử dụng SqlDataReader để đọc dữ liệu từ database
                            using (SqlDataReader reader = command2.ExecuteReader())
                            {
                                // Kiểm tra xem có dữ liệu hay không
                                if (reader.Read())
                                {
                                    // Tạo đối tượng AccountClass từ SqlDataReader
                                    AccountClass user = new AccountClass(reader);
                                    account = user;
                                }
                                else
                                {
                                    MessageBox.Show($"No data");
                                }
                            }
                        }

                        // Câu truy vấn SQL để cập nhật thông tin của Customer
                        string query3 = "UPDATE Customer SET AccountID = @AccountID " +
                                        "WHERE PhoneNo = @PhoneNo";

                        // Thực hiện truy vấn 2
                        using (SqlCommand command3 = new SqlCommand(query3, connection, transaction))
                        {
                            // Thêm các tham số
                            command3.Parameters.AddWithValue("@AccountID", account.AccountID);
                            command3.Parameters.AddWithValue("@PhoneNo", customerPhoneNo);

                            // Thực hiện truy vấn
                            command3.ExecuteNonQuery();

                        }
                        // Nếu mọi thứ đều OK, commit giao dịch
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi và rollback giao dịch
                        MessageBox.Show($"Error: {ex.Message}");
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}
