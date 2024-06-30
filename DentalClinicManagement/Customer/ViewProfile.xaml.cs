using DentalClinicManagement.Account;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ViewProfile.xaml
    /// </summary>
    public partial class ViewProfile : Page
    {
        // Biến lưu User từ cửa sổ Sign In
        private CustomerClass user;

        public ViewProfile(CustomerClass user)
        {
            InitializeComponent();
            if(user.AccountID != null)
            {
                loadInfor((int)user.AccountID);
            }    
            
            this.user = new CustomerClass(user);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //this.DataContext = user;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.DashBoard(user));
            }
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            user.Name = FullNameTextBox.Text;
            user.PhoneNo = PhoneNumberTextBox.Text;
            user.Email = EmailTextBox.Text;
            user.Address = AddressTextBox.Text;
            bool newGender = MaleCheckBox.IsChecked ?? false;
            user.ConvertToStringGender(newGender);

            // Gọi hàm lưu thông tin vào database
            if (UpdateCustomerInfo(user))
            {
                MessageBox.Show("Thông tin đã được cập nhật thành công.");
                MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

                if (mainWindow != null && mainWindow.MainFrame != null)
                {
                    mainWindow.MainFrame.Navigate(new DentalClinicManagement.Customer.ViewProfile(user));
                }
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật thông tin. Vui lòng thử lại.");
            }
        }

        private bool UpdateCustomerInfo(CustomerClass updatedCustomer)
        {
            //DB dB = new DB();
            using (SqlConnection connection = DB.Instance.Connection)
            {
                // Bắt đầu giao dịch
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Câu truy vấn SQL để cập nhật thông tin của Customer
                        string queryForCustomer = "UPDATE Customer SET Name = @Name, PhoneNo = @PhoneNo, Address = @Address, Email = @Email " +
                                                  "WHERE AccountID = @AccountID";

                        string queryForAccount = "UPDATE Account SET PhoneNo = @PhoneNo " +
                                                 "WHERE AccountID = @AccountID";

                        // Tạo đối tượng SqlCommand và liên kết với giao dịch
                        using (SqlCommand commandForCustomer = new SqlCommand(queryForCustomer, connection, transaction))
                        {
                            commandForCustomer.Parameters.AddWithValue("@Name", updatedCustomer.Name);
                            commandForCustomer.Parameters.AddWithValue("@PhoneNo", updatedCustomer.PhoneNo);
                            commandForCustomer.Parameters.AddWithValue("@Address", updatedCustomer.Address);
                            commandForCustomer.Parameters.AddWithValue("@Email", updatedCustomer.Email);
                            commandForCustomer.Parameters.AddWithValue("@AccountID", user.AccountID);

                            // Thực hiện truy vấn
                            commandForCustomer.ExecuteNonQuery();
                        }

                        using (SqlCommand commandForAccount = new SqlCommand(queryForAccount, connection, transaction))
                        {
                            commandForAccount.Parameters.AddWithValue("@PhoneNo", updatedCustomer.PhoneNo);
                            commandForAccount.Parameters.AddWithValue("@AccountID", user.PhoneNo);

                            // Thực hiện truy vấn
                            commandForAccount.ExecuteNonQuery();
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

        private async Task<int> getInfor(int customerID)
        {
            try
            {
                int result = await Task.Run(() =>
                {
                    try
                    {
                        using (DB.Instance.Connection)
                        {
                            using (SqlCommand cmd = new SqlCommand("SP_UNREPEAT_THI", DB.Instance.Connection))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // Thêm tham số vào stored procedure
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    cmd.Parameters.AddWithValue("@AccountID", customerID);
                                });

                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    // Kiểm tra xem có dữ liệu hay không
                                    if (reader.HasRows)
                                    {
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            DataTable dataTable = new DataTable();
                                            dataTable.Load(reader);
                                            DataRow row = dataTable.Rows[0];
                                            FullNameTextBox.Text = row["Name"].ToString();
                                            PhoneNumberTextBox.Text = row["PhoneNo"].ToString();
                                            EmailTextBox.Text = row["Email"].ToString();
                                            AddressTextBox.Text = row["Address"].ToString();
                                            birthday.SelectedDate = Convert.ToDateTime(row["DateOfBirth"]);
                                            if (row["Gender"].ToString() == "Nam")
                                            {
                                                MaleCheckBox.IsChecked = true;
                                                FemaleCheckBox.IsChecked = false;
                                            }
                                            else
                                            {
                                                FemaleCheckBox.IsChecked = true;
                                                MaleCheckBox.IsChecked = false;
                                            }
                                        });

                                        return 1;
                                    }
                                    else
                                    {
                                        // Nếu không có dữ liệu, có thể xử lý tương ứng ở đây
                                        return 0;
                                    }
                                }

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                        return 0;
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý bất đồng bộ: " + ex.Message);
                return 0;
            }
        }
        private async void loadInfor(int customerID)
        {
            try
            {
                // Hiển thị dialog loading (nếu có)
                showLoadingWindow();

                // Thực hiện đăng nhập bất đồng bộ


                int result = await getInfor(customerID);

                // Tắt dialog loading khi truy vấn hoàn tất (nếu có)
                closeLoadingWindow();

                if (result == 0)
                {
                    MessageBoxResult resultMessageBox = MessageBox.Show("Đã xảy ra lỗi. Vui lòng đăng nhập lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (resultMessageBox == MessageBoxResult.OK)
                    {
                        MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

                        if (mainWindow != null && mainWindow.MainFrame != null)
                        {
                            mainWindow.MainFrame.Navigate(new DentalClinicManagement.Account.SignIn());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý: " + ex.Message);
            }
        }
        private void showLoadingWindow()
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.IsHitTestVisible = false;
                popup.PlacementTarget = mainWindow;
                popup.IsOpen = true;
            }
        }

        private void closeLoadingWindow()
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.IsHitTestVisible = true;
                popup.PlacementTarget = mainWindow;
                popup.IsOpen = false;
            }
        }



        private void MaleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            FemaleCheckBox.IsChecked = false;
        }

        private void FemaleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            MaleCheckBox.IsChecked = false;
        }
    }
}
