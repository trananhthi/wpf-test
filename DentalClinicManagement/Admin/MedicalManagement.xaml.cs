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
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace DentalClinicManagement.Admin
{
    /// <summary>
    /// Interaction logic for MedicalManagement.xaml
    /// </summary>
    public partial class MedicalManagement : Page
    {
        Administrator admin;
        private DB db;
        public MedicalManagement(Administrator admin)
        {
            InitializeComponent();
            this.admin = new Administrator(admin);
            search();
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow? mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow != null && mainWindow.MainFrame != null)
            {
                mainWindow.MainFrame.Navigate(new DashBoard(admin));
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            search();
        }

        private void addMedical(object sender, RoutedEventArgs e)
        {
            ViewMedicalDetail.Visibility = Visibility.Collapsed;
            EditMedicalDetail.Visibility = Visibility.Visible;
        }

        private void deletemedical(object sender, RoutedEventArgs e)
        {
            if (ListMedical.SelectedItem != null && ListMedical.SelectedItem is DataRowView selectedRow)
            {
                DataRow medicalRow = selectedRow.Row;
                int MedicalID = Convert.ToInt32(medicalRow["MedicalID"]);

                MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn xóa loại thuốc này", "Xóa thuốc", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    // Thực hiện hành động khi người dùng chọn Yes
                    try
                    {
                        //db = new DB();
                        using (SqlConnection connection = DB.Instance.Connection)
                        {
                            string query = $"UPDATE MedicationList SET Status = 0  WHERE MedicalID = {MedicalID}";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {

                                int rowsAffected = command.ExecuteNonQuery();


                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Đã xóa loại thuốc này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                                    search();
                                }
                                else
                                {
                                    MessageBox.Show("Không thể thực hiện. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Thực hiện hành động khi người dùng chọn No

                }

            }
            ViewMedicalDetail.Visibility = Visibility.Collapsed;

        }

        private void editMedical(object sender, RoutedEventArgs e)
        {

            if (ListMedical.SelectedItem != null && ListMedical.SelectedItem is DataRowView selectedRow)
            {
                DataRow medicalRow = selectedRow.Row;
                int MedicalID = Convert.ToInt32(medicalRow["MedicalID"]);
                if (expirationDate.SelectedDate == null || medicalName.Text == "" || medicalPrice.Text == "" || allocation.Text == "" || quantity.Text == "")
                {
                    MessageBox.Show($"Hãy nhập đầy đủ thông tin", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Bạn có chắc muốn thay đổi thông tin loại thuốc này", "Cập nhật thông tin thuốc", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        // Thực hiện hành động khi người dùng chọn Yes
                        try
                        {
                            //db = new DB();
                            using (SqlConnection connection = DB.Instance.Connection)
                            {
                                //connection.Open();
                                using (SqlCommand command = new SqlCommand("Edit_Medical", connection))
                                {
                                    command.CommandType = CommandType.StoredProcedure;

                                    // Add parameters
                                    command.Parameters.AddWithValue("@MedicalID", MedicalID);
                                    command.Parameters.AddWithValue("@Name", medicalName.Text);
                                    command.Parameters.AddWithValue("@Price", Convert.ToDecimal(medicalPrice.Text.Replace(" ", "")));
                                    command.Parameters.AddWithValue("@Allocation", allocation.Text);
                                    command.Parameters.AddWithValue("@InventoryNumber", Convert.ToInt32(quantity.Text));
                                    command.Parameters.AddWithValue("@ExpirationDate", expirationDate.SelectedDate.Value);

                                    // Open the connection and execute the command

                                    int rowsAffected = command.ExecuteNonQuery();

                                    // Check the result
                                    if (rowsAffected > 0)
                                    {
                                        MessageBox.Show("Đã cập nhật thông tin cho thuốc này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                                        search();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Không thể thực hiện. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }
                    else
                    {
                        // Thực hiện hành động khi người dùng chọn No

                    }


                }

                EditMedicalDetail.Visibility = Visibility.Collapsed;
            }
        }

        private void completeMedical(object sender, RoutedEventArgs e)
        {
            if (addMedicalExDate.SelectedDate == null || addMedicalName.Text == "" || addMedicalPrice.Text == "" || addMedicalAllocation.Text == "" || addMedicalQuantity.Text == "")
            {
                MessageBox.Show($"Hãy nhập đầy đủ thông tin", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    //db = new DB();
                    using (SqlConnection connection = DB.Instance.Connection)
                    {
                        string query = $"INSERT INTO MedicationList (Name, Price, Unit, Allocation, InventoryNumber, ExpirationDate, AdministratorID, Status) VALUES (N'{addMedicalName.Text}', {Convert.ToInt32(addMedicalPrice.Text.Replace(" ", ""))}, 'vnd' ,  N'{addMedicalAllocation.Text}', {Convert.ToInt32(addMedicalQuantity.Text.Replace(" ", ""))}, '{addMedicalExDate.SelectedDate.Value.ToString("yyyy-MM-dd")}', 1, 1)";

                        // proc add medical fixed ver
                        using (SqlCommand command = new SqlCommand("SP_PHANTOMREAD_2", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Add parameters
                            command.Parameters.AddWithValue("@Name", addMedicalName.Text);
                            command.Parameters.AddWithValue("@Price", Convert.ToDecimal(addMedicalPrice.Text.Replace(" ", "")));
                            command.Parameters.AddWithValue("@Unit", "vnd");
                            command.Parameters.AddWithValue("@Allocation", addMedicalAllocation.Text);
                            command.Parameters.AddWithValue("@InventoryNumber", Convert.ToInt32(addMedicalQuantity.Text));
                            command.Parameters.AddWithValue("@ExpirationDate", addMedicalExDate.SelectedDate.Value);
                            command.Parameters.AddWithValue("@AdministratorID", 1);
                            command.Parameters.AddWithValue("@Status", 1);

                            // Open the connection and execute the command

                            int rowsAffected = command.ExecuteNonQuery();

                            // Check the result
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Thêm thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                                addMedicalName.Text = "";
                                addMedicalPrice.Text = "";
                                addMedicalAllocation.Text = "";
                                addMedicalExDate.SelectedDate = null;
                                addMedicalQuantity.Text = "";
                                search();

                            }
                            else
                            {
                                MessageBox.Show("Không thể thực hiện. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {

        }

        private void comboPage_Selected(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditMedicalDetail.Visibility = Visibility.Collapsed;
            ViewMedicalDetail.Visibility = Visibility.Visible;

            if (ListMedical.SelectedItem != null && ListMedical.SelectedItem is DataRowView selectedRow)
            {
                DataRow medicalRow = selectedRow.Row;
                int MedicalID = Convert.ToInt32(medicalRow["MedicalID"]);
                try
                {
                    //db = new DB();
                    using (SqlConnection connection = DB.Instance.Connection)
                    {
                        string searchText = SearchTextBox.Text;
                        string query = $"SELECT * FROM MedicationList WHERE MedicalID LIKE '%{MedicalID}%'";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {

                            DataRow row = dataTable.Rows[0];

                            Medical medical = new Medical(row);
                            medicalName.Text = medical.medicalName;
                            medicalID.Text = medical.medicalID.ToString();
                            medicalPrice.Text = string.Format("{0:N0}", medical.price).Replace(",", " ");
                            expirationDate.SelectedDate = medical.expirationDate;
                            quantity.Text = medical.inventoryNumber.ToString();
                            allocation.Text = medical.allocation;
                            unit.Visibility = Visibility.Visible;
                        }
                        else
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void search()
        {
            try
            {
                //db = new DB();
                using (SqlConnection connection = DB.Instance.Connection)
                {
                    using (SqlCommand command = new SqlCommand("SP_PHANTOMREAD_1", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@Name", $"%{SearchTextBox.Text}%");

                        var reader = command.ExecuteReader();

                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        ListMedical.ItemsSource = dataTable.DefaultView;
                    }
                    /*string searchText = SearchTextBox.Text;
                    string query = $"SELECT MedicalID, Name,InventoryNumber FROM MedicationList WHERE Name LIKE '%{searchText}%' AND Status = 1";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    ListMedical.ItemsSource = dataTable.DefaultView;*/
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
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