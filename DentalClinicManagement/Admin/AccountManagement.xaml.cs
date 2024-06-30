using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using DentalClinicManagement.Dentist;
using System.Collections;
using System.Text.RegularExpressions;

namespace DentalClinicManagement.Admin
{
    /// <summary>
    /// Interaction logic for AccountManagement.xaml
    /// </summary>
    public partial class AccountManagement : Page
    {
        Administrator admin;
        private DB db;
        private string selectedRole = "Nhân viên";
        private string selectedGender = "Nam";
        public AccountManagement(Administrator admin)
        {
            InitializeComponent();
            this.admin = new Administrator(admin);
            Search();
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;


            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DentalClinicManagement.Admin.DashBoard(admin));
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {

        }

        private void comboPage_Selected(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            loadDataForDataGridChanged();

        }

        private async void loadDataForDataGridChanged()
        {
            showLoadingWindow();

            if (ListAccount.SelectedItem != null && ListAccount.SelectedItem is DataRowView selectedRow)
            {
                ViewAccountDetail.Visibility = Visibility.Visible;
                EditAccountDetail.Visibility = Visibility.Collapsed;
                DataRow accountRow = selectedRow.Row;
                int accountID = Convert.ToInt32(accountRow["AccountID"]);
                string? accountPhone = await getAccountByID(accountID);
                string query1 = $"select * from Customer c, Account a where c.AccountID = a.AccountID AND a.AccountID = {accountID}";
                string query2 = $"select * from Dentist c, Account a where c.AccountID = a.AccountID AND a.AccountID = {accountID}";
                string query3 = $"select * from Staff c, Account a where c.AccountID = a.AccountID AND a.AccountID = {accountID}";
                string query4 = $"select * from Administrator c, Account a where c.AccountID = a.AccountID AND a.AccountID = {accountID}";
                try
                {

                    using (DB.Instance.Connection)
                    {

                        SqlDataAdapter adapter1 = new SqlDataAdapter(query1, DB.Instance.Connection);
                        DataTable dataTable1 = new DataTable();
                        adapter1.Fill(dataTable1);


                        if (dataTable1.Rows.Count > 0)
                        {

                            DataRow row = dataTable1.Rows[0];


                            AccountInformation account = new AccountInformation(row);
                            name.Text = account.name;
                            phone.Text = accountPhone;
                            address.Text = account.address;
                            birthday.SelectedDate = account.dateOfBirth;
                            gender.Text = account.gender;
                            status.Text = account.status == "True" ? "Đang hoạt động" : "Đã khóa";
                            role.Text = "Khách hàng";

                        }
                        else
                        {
                            SqlDataAdapter adapter2 = new SqlDataAdapter(query2, DB.Instance.Connection);
                            DataTable dataTable2 = new DataTable();
                            adapter2.Fill(dataTable2);

                            if (dataTable2.Rows.Count > 0)
                            {

                                DataRow row = dataTable2.Rows[0];


                                AccountInformation account = new AccountInformation(row);
                                name.Text = account.name;
                                phone.Text = accountPhone;
                                address.Text = account.address;
                                birthday.SelectedDate = account.dateOfBirth;
                                gender.Text = account.gender;
                                status.Text = account.status == "True" ? "Đang hoạt động" : "Đã khóa";
                                role.Text = "Nha sĩ";

                            }
                            else
                            {
                                SqlDataAdapter adapter3 = new SqlDataAdapter(query3, DB.Instance.Connection);
                                DataTable dataTable3 = new DataTable();
                                adapter3.Fill(dataTable3);

                                if (dataTable3.Rows.Count > 0)
                                {

                                    DataRow row = dataTable3.Rows[0];


                                    AccountInformation account = new AccountInformation(row);
                                    name.Text = account.name;
                                    phone.Text = accountPhone;
                                    address.Text = account.address;
                                    birthday.SelectedDate = account.dateOfBirth;
                                    gender.Text = account.gender;
                                    status.Text = account.status == "True" ? "Đang hoạt động" : "Đã khóa";
                                    role.Text = "Nhân viên";

                                }
                                else
                                {
                                    SqlDataAdapter adapter4 = new SqlDataAdapter(query4, DB.Instance.Connection);
                                    DataTable dataTable4 = new DataTable();
                                    adapter4.Fill(dataTable4);

                                    if (dataTable4.Rows.Count > 0)
                                    {

                                        DataRow row = dataTable4.Rows[0];


                                        AccountInformation account = new AccountInformation(row);
                                        name.Text = account.name;
                                        phone.Text = accountPhone;
                                        address.Text = account.address;
                                        birthday.SelectedDate = account.dateOfBirth;
                                        gender.Text = account.gender;
                                        status.Text = account.status == "True" ? "Đang hoạt động" : "Đã khóa";
                                        role.Text = "Quản trị viên";
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            else
            {
                ViewAccountDetail.Visibility = Visibility.Collapsed;
            }
            closeLoadingWindow();
        }

        public async Task<string?> getAccountByID(int accountID)
        {

            try
            {
                string? result = await Task.Run(() =>
                {

                    try
                    {
                        using (DB.Instance.Connection)
                        {
                            using (SqlCommand cmd = new SqlCommand("SP_DIRTYREAD_2_THI", DB.Instance.Connection))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // Thêm tham số vào stored procedure
                                cmd.Parameters.AddWithValue("@AccountID", accountID);

                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    // Kiểm tra xem có dữ liệu hay không
                                    if (reader.HasRows)
                                    {
                                        DataTable dataTable = new DataTable();
                                        dataTable.Load(reader);
                                        DataRow row = dataTable.Rows[0];
                                        return row["PhoneNo"].ToString();
                                    }
                                    else
                                    {
                                        // Nếu không có dữ liệu, có thể xử lý tương ứng ở đây
                                        MessageBox.Show("Không tìm thấy dữ liệu.");
                                        return null;
                                    }
                                }

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                        return null;
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý bất đồng bộ: " + ex.Message);
                return null;
            }

        }

        private void addAccount(object sender, RoutedEventArgs e)
        {
            EditAccountDetail.Visibility = Visibility.Visible;
            ViewAccountDetail.Visibility = Visibility.Collapsed;
        }

        private void completeEdit(object sender, RoutedEventArgs e)
        {

            if (editBirthday.SelectedDate == null || editName.Text == "" || editPhone.Text == "" || editAddress.Text == "")
            {
                MessageBox.Show($"Hãy nhập đầy đủ thông tin", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (editBirthday.SelectedDate != null && CalculateAge(editBirthday.SelectedDate.Value) < 18)
                {
                    MessageBox.Show("Bạn chưa đủ 18 tuổi.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (IsPhoneNumberExists(editPhone.Text))
                {
                    MessageBox.Show("Số điện thoại đã tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    InsertNewAccount(selectedRole, editName.Text, editBirthday.SelectedDate.Value, editAddress.Text, editPhone.Text.Replace(" ", ""), selectedGender);
                    EditAccountDetail.Visibility = Visibility.Visible;
                    ViewAccountDetail.Visibility = Visibility.Collapsed;
                }
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

        private async void blockAccount(object sender, RoutedEventArgs e)
        {
            if (ListAccount.SelectedItem != null && ListAccount.SelectedItem is DataRowView selectedRow)
            {
                DataRow accountRow = selectedRow.Row;
                int accountID = Convert.ToInt32(accountRow["AccountID"]);
                try
                {

                    using (DB.Instance.Connection)
                    {
                        string searchText = SearchTextBox.Text;
                        string query = $"SELECT * FROM Account WHERE AccountID = {accountID}";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, DB.Instance.Connection);

                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {

                            DataRow row = dataTable.Rows[0];
                            string accountStatus = row["Status"].ToString();
                            if (accountStatus == "True")
                            {
                                MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn khóa tài khoản này", "Khóa tài khoản", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                if (result == MessageBoxResult.Yes)
                                {
                                    showLoadingWindow();
                                    // Thực hiện hành động khi người dùng chọn Yes
                                    bool bloclResult = await BlockAccountAction(accountID);
                                    closeLoadingWindow();
                                    if (bloclResult)
                                    {
                                        MessageBox.Show("Tài khoản đã được chặn thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                                        status.Text = "Đã khóa";
                                        accountRow["Status"] = "False";
                                    }
                                    else
                                    {
                                        MessageBox.Show("Không thể chặn tài khoản. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }


                                }
                                else
                                {
                                    // Thực hiện hành động khi người dùng chọn No
                                }
                            }
                            else
                            {
                                MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn mở khóa tài khoản này", "Mở khóa tài khoản", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                                if (result == MessageBoxResult.Yes)
                                {
                                    // Thực hiện hành động khi người dùng chọn Yes
                                    UnBlockAccountAction(accountID);
                                    status.Text = "Đang hoạt động";
                                    accountRow["Status"] = "True";
                                }
                                else
                                {
                                    // Thực hiện hành động khi người dùng chọn No

                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private async void editAccount(object sender, RoutedEventArgs e)
        {
            if (birthday.SelectedDate == null || name.Text == "" || phone.Text == "" || address.Text == "")
            {
                MessageBox.Show($"Hãy nhập đầy đủ thông tin", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (ListAccount.SelectedItem != null && ListAccount.SelectedItem is DataRowView selectedRow)
                {
                    DataRow accountRow = selectedRow.Row;
                    int accountID = Convert.ToInt32(accountRow["AccountID"]);
                    string accountRole = role.Text;
                    if (birthday.SelectedDate != null && CalculateAge(birthday.SelectedDate.Value) < 18)
                    {
                        MessageBox.Show("Bạn chưa đủ 18 tuổi.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        try
                        {
                            // Hiển thị dialog loading (nếu có)
                            showLoadingWindow();

                            // Thực hiện đăng nhập bất đồng bộ


                            int loginResult = await UpdateAccountInformation(accountID, accountRole, name.Text, birthday.SelectedDate.Value, address.Text, phone.Text.Replace(" ", ""));

                            // Tắt dialog loading khi truy vấn hoàn tất (nếu có)
                            closeLoadingWindow();


                            if (loginResult == 1)
                            {
                                MessageBox.Show("Thông tin tài khỏan đã được cập nhật thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else if (loginResult == 0)
                            {
                                MessageBox.Show("Số điện thoại đã tồn tại. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                loadDataForDataGridChanged();
                            }
                            else
                            {
                                MessageBox.Show("Không thể cập nhật tài khoản. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                loadDataForDataGridChanged();
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi xử lý: " + ex.Message);
                        }

                    }

                }
            }

        }
        private void Search()
        {
            try
            {

                using (DB.Instance.Connection)
                {
                    string searchText = SearchTextBox.Text;
                    string query = $"SELECT * FROM Account WHERE PhoneNo LIKE '%{searchText}%'";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, DB.Instance.Connection);

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    ListAccount.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async Task<bool> BlockAccountAction(int accountID)
        {
            try
            {
                bool result = await Task.Run(() =>
                {
                    try
                    {

                        using (DB.Instance.Connection)
                        {
                            string query = $"exec SP_UNREPEAT_2_THI @AccountID = {accountID}";

                            using (SqlCommand command = new SqlCommand(query, DB.Instance.Connection))
                            {

                                int rowsAffected = command.ExecuteNonQuery();


                                if (rowsAffected > 0)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý bất đồng bộ: " + ex.Message);
                return false;
            }

        }

        private void UnBlockAccountAction(int accountID)
        {
            try
            {

                using (DB.Instance.Connection)
                {
                    string query = $"UPDATE Account SET Status = 1 WHERE AccountID = {accountID}";

                    using (SqlCommand command = new SqlCommand(query, DB.Instance.Connection))
                    {

                        int rowsAffected = command.ExecuteNonQuery();


                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Tài khoản đã được mở khóa thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Không thể chặn tài khoản. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int CalculateAge(DateTime birthDate)
        {
            DateTime currentDate = DateTime.Now;
            int age = currentDate.Year - birthDate.Year;

            // Kiểm tra xem ngày sinh đã qua ngày hiện tại chưa
            if (currentDate.Month < birthDate.Month || (currentDate.Month == birthDate.Month && currentDate.Day < birthDate.Day))
            {
                age--;
            }

            return age;
        }

        private async Task<int> UpdateAccountInformation(int accountID, string role, string name, DateTime birthDate, string newAddress, string phone)
        {
            try
            {
                int result = await Task.Run(() =>
                {

                    try
                    {

                        using (DB.Instance.Connection)
                        {
                            // Câu lệnh SQL UPDATE để cập nhật thông tin nhân viên
                            string query1 = $"UPDATE Administrator SET DateOfBirth = '{birthDate.ToString("yyyy-MM-dd")}', Address = '{newAddress}', PhoneNo = {phone} , Name = N'{name}' where accountID = {accountID}";
                            string query2 = $"UPDATE Customer SET DateOfBirth = '{birthDate.ToString("yyyy-MM-dd")}', Address = '{newAddress}', PhoneNo = {phone} , Name = N'{name}' where accountID = {accountID}";
                            string query3 = $"UPDATE Dentist SET DateOfBirth = '{birthDate.ToString("yyyy-MM-dd")}', Address = '{newAddress}', PhoneNo = {phone} , Name = N'{name}' where accountID = {accountID}";
                            string query4 = $"UPDATE Staff SET DateOfBirth = '{birthDate.ToString("yyyy-MM-dd")}', Address = '{newAddress}', PhoneNo = {phone} , Name = N'{name}' where accountID = {accountID}";

                            if (role == "Quản trị viên")
                            {
                                using (SqlCommand command = new SqlCommand(query1, DB.Instance.Connection))
                                {
                                    int rowsAffected = command.ExecuteNonQuery();

                                    if (rowsAffected > 0 && updatePhoneNoAccount(accountID, phone))
                                    {
                                        return 1;
                                    }
                                    else if (rowsAffected > 0 && !updatePhoneNoAccount(accountID, phone))
                                    {
                                        return 0;
                                    }
                                    else
                                    {
                                        return -1;
                                    }
                                }
                            }
                            else if (role == "Khách hàng")
                            {
                                using (SqlCommand command = new SqlCommand(query2, DB.Instance.Connection))
                                {
                                    int rowsAffected = command.ExecuteNonQuery();

                                    if (rowsAffected > 0 && updatePhoneNoAccount(accountID, phone))
                                    {
                                        return 1;
                                    }
                                    else if (rowsAffected > 0 && !updatePhoneNoAccount(accountID, phone))
                                    {
                                        return 0;
                                    }
                                    else
                                    {
                                        return -1;
                                    }
                                }
                            }
                            else if (role == "Nha sĩ")
                            {
                                using (SqlCommand command = new SqlCommand(query3, DB.Instance.Connection))
                                {
                                    int rowsAffected = command.ExecuteNonQuery();

                                    if (rowsAffected > 0 && updatePhoneNoAccount(accountID, phone))
                                    {
                                        return 1;
                                    }
                                    else if (rowsAffected > 0 && !updatePhoneNoAccount(accountID, phone))
                                    {
                                        return 0;
                                    }
                                    else
                                    {
                                        return -1;
                                    }
                                }
                            }
                            else
                            {
                                using (SqlCommand command = new SqlCommand(query4, DB.Instance.Connection))
                                {
                                    int rowsAffected = command.ExecuteNonQuery();

                                    if (rowsAffected > 0 && updatePhoneNoAccount(accountID, phone))
                                    {
                                        return 1;
                                    }
                                    else if (rowsAffected > 0 && !updatePhoneNoAccount(accountID, phone))
                                    {
                                        return 0;
                                    }
                                    else
                                    {
                                        return -1;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi: {ex.Message}");
                        return -1;
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xử lý bất đồng bộ: " + ex.Message);
                return -1;
            }

        }

        private bool updatePhoneNoAccount(int accountID, string newPhone)
        {
            try
            {
                using (DB.Instance.Connection)
                {
                    using (SqlCommand cmd = new SqlCommand("SP_DIRTYREAD_THI", DB.Instance.Connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Thêm tham số vào stored procedure
                        cmd.Parameters.AddWithValue("@AccountID", accountID);
                        cmd.Parameters.AddWithValue("@NewPhone", newPhone);

                        SqlParameter returnCode = new SqlParameter("@ReturnCode", SqlDbType.Int);
                        returnCode.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(returnCode);

                        cmd.ExecuteNonQuery();

                        int resultCode = (int)returnCode.Value;
                        if (resultCode == 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }

        private void RoleRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.IsChecked == true)
            {
                selectedRole = radioButton.Content.ToString();
            }
        }
        private void GenderRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.IsChecked == true)
            {
                selectedGender = radioButton.Content.ToString();
            }
        }

        private bool IsPhoneNumberExists(string phoneNumber)
        {
            try
            {

                using (DB.Instance.Connection)
                {
                    string query = $"SELECT COUNT(*) FROM Account WHERE PhoneNo = '{phoneNumber}'";
                    SqlCommand command = new SqlCommand(query, DB.Instance.Connection);

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }

        private void InsertNewAccount(string role, string name, DateTime birthDate, string newAddress, string phone, string gender)
        {
            int newAccountID = 0;
            try
            {

                using (DB.Instance.Connection)
                {

                    string query = $"INSERT INTO Account (PhoneNo, Password, Status) OUTPUT INSERTED.AccountID VALUES (@PhoneNo, @Password, @Status)";

                    using (SqlCommand command = new SqlCommand(query, DB.Instance.Connection))
                    {
                        command.Parameters.AddWithValue("@PhoneNo", phone);
                        command.Parameters.AddWithValue("@Password", "123456");
                        command.Parameters.AddWithValue("@Status", "1");

                        newAccountID = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");

            }
            if (newAccountID > 0)
            {
                try
                {

                    using (DB.Instance.Connection)
                    {
                        string query1 = $"INSERT INTO Staff (Name,Gender,Address, DateOfBirth, PhoneNo, AccountID) VALUES (N'{name}','{gender}','{newAddress}', '{birthDate.ToString("yyyy-MM-dd")}', '{phone}', {newAccountID})";
                        string query2 = $"INSERT INTO Dentist (Name,Gender,Address, DateOfBirth, PhoneNo, AccountID) VALUES (N'{name}','{gender}','{newAddress}', '{birthDate.ToString("yyyy-MM-dd")}', '{phone}', {newAccountID})";

                        if (role == "Nhân viên")
                        {
                            using (SqlCommand command = new SqlCommand(query1, DB.Instance.Connection))
                            {
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Tài khoản đã được thêm thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                                    editName.Text = "";
                                    editPhone.Text = "";
                                    editBirthday.SelectedDate = null;
                                    editAddress.Text = "";
                                    Search();
                                }
                                else
                                {
                                    MessageBox.Show("Không thể thêm tài khoản. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            using (SqlCommand command = new SqlCommand(query2, DB.Instance.Connection))
                            {
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Tài khoản đã được thêm thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                                    editName.Text = "";
                                    editPhone.Text = "";
                                    editBirthday.SelectedDate = null;
                                    editAddress.Text = "";
                                    Search();
                                }
                                else
                                {
                                    MessageBox.Show("Không thể thêm tài khoản. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Không thể thêm tài khoản. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumericTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Chỉ cho phép nhập số
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}