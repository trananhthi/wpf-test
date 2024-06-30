using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using DentalClinicManagement.Customer;
using DentalClinicManagement.Dentist;
using DentalClinicManagement.Account;
using System.ComponentModel;

namespace DentalClinicManagement
{
    public class DB
    {
        private static DB? _instance = null;
        private SqlConnection? _connection = null;

        public string ConnectionString { get; set; } = "Server=LAPTOP-9UBO3DBS;Database=DentalClinicManagement;Integrated Security=True;MultipleActiveResultSets=True;";
        /*public string ConnectionString { get; set; } = "Server=DESKTOP-8IL3H18\\SQLEXPRESS01;Database=DentalClinicManagement;Integrated Security=True;MultipleActiveResultSets=True;";*/

        // 21120082_PhanQuocHuy's ConnectionString
        //public string ConnectionString { get; set; } = "Server=LAPTOP-PRB0VQ46;Database=DentalClinicManagement;Integrated Security=True;";


        public string tableName = "DentalClinicManagement";
        public void ImportDataToSQL()
        {
        }

        public SqlConnection? Connection
        {
            get
            {
                if (_connection != null)
                {
                    if (_connection.State == ConnectionState.Open)
                    {
                        return _connection;
                    }
                }
                _connection = new SqlConnection(ConnectionString);
                _connection.Open();


                return _connection;
            }
        }

        // Chuỗi kết nối với tùy chọn Windows Authentication

        public static DB Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DB();
                }

                return _instance;
            }
        }

        public static List<RecordList> GetCustomerRecordList(string phoneNo)
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                connection.Open();
                var sql = @"
                    select d.Name as DentistName, s.Name as ServiceName, ml.Name as MedicalName, a.Date, p.Status, ar.AppointRecordID, p.TotalPrice as TotalPrice
                    from Customer c join PatientRecord pr on pr.CustomerID = c.CustomerID
		                            join AppointmentRecord ar on ar.RecordID = pr.RecordID
		                            join ServiceDetail sd on sd.AppointmentRecordID = ar.AppointRecordID
		                            join MedicationRecord mr on mr.AppointRecordID = ar.AppointRecordID
		                            join Payment p on p.AppointmentRecordID = ar.AppointRecordID
		                            join Appointment a on a.AppointmentID = ar.AppointmentID
		                            join Dentist d on d.DentistID = a.DentistID
		                            join Service s on s.ServiceID = sd.ServiceID
		                            join MedicationList ml on ml.MedicalID = mr.MedicalID
                    where c.PhoneNo = @phoneNo
                ";
                var command = new SqlCommand(sql, DB.Instance.Connection);
                command.Parameters.AddWithValue("@phoneNo", phoneNo);
                var reader = command.ExecuteReader();
                var _recList = new List<RecordList>();
                while (reader.Read())
                {
                    var appointRecordID = reader.GetInt32(reader.GetOrdinal("AppointRecordID"));
                    var existingRecord = _recList.FirstOrDefault(r => r.AppointRecordID == appointRecordID);
                    if (existingRecord != null)
                    {
                        // Update existing record
                        existingRecord.medicalList += ", " + reader.GetString(reader.GetOrdinal("MedicalName"));
                        existingRecord.serviceList += ", " + reader.GetString(reader.GetOrdinal("ServiceName"));
                    }
                    else
                    {
                        // Create new record
                        var newRecord = new RecordList
                        {
                            AppointRecordID = appointRecordID,
                            DentistName = reader.GetString(reader.GetOrdinal("DentistName")),
                            totalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                            dateOfAppointment = (reader.GetDateTime(reader.GetOrdinal("Date"))),
                            isPaid = reader.GetBoolean(reader.GetOrdinal("Status")),
                            medicalList = reader.GetString(reader.GetOrdinal("MedicalName")),
                            serviceList = reader.GetString(reader.GetOrdinal("ServiceName"))
                        };
                        _recList.Add(newRecord);
                    }
                }
                reader.Close();
                return _recList;
            }
        }


        /*public static List<MedicalDetail> GetMedicalDetails(int a_id, int r_id)
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                connection.Open();
                var sql = @"
                    Select m.Quantity, m.TotalPrice, ml.Name, m.MedicalID
                    from MedicationRecord m, MedicationList ml
                    where m.AppointmentID = @aID
                     and m.RecordID = @rID
                     and ml.MedicalID = m.MedicalID
                ";
                var command = new SqlCommand(sql, DB.Instance.Connection);
                command.Parameters.AddWithValue("@aID", a_id);
                command.Parameters.AddWithValue("@rID", r_id);

                var reader = command.ExecuteReader();
                List<MedicalDetail> _med = new List<MedicalDetail>();
                while (reader.Read())
                {
                    int quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                    decimal totalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice"));
                    string m_name = reader.GetString(reader.GetOrdinal("Name"));
                    int m_id = reader.GetInt32(reader.GetOrdinal("MedicalID"));

                    MedicalDetail newMed = new MedicalDetail()
                    {
                        MedicalQuantity = quantity,
                        MedicalID = m_id,
                        MedicalName = m_name,
                        MedicalPrice = Convert.ToInt32(totalPrice)
                    };
                    _med.Add(newMed);
                }
                reader.Close();
                return _med;

            }
            return null;
        }
*/
        /*public static List<ServiceDetail> GetServiceDetails(int a_id, int r_id)
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                connection.Open();
                var sql = @"
                    Select s.ServiceID, s.Name, s.Price
                    From Service s, ServiceDetail sd
                    where s.ServiceID = sd.ServiceID
	                    and sd.RecordID = @rID
	                    and sd.AppointmentID = @aID
                ";
                var command = new SqlCommand(sql, DB.Instance.Connection);
                command.Parameters.AddWithValue("@aID", a_id);
                command.Parameters.AddWithValue("@rID", r_id);

                var reader = command.ExecuteReader();
                List<ServiceDetail> _ser = new List<ServiceDetail>();
                while (reader.Read())
                {
                    decimal totalPrice = reader.GetDecimal(reader.GetOrdinal("Price"));
                    string s_name = reader.GetString(reader.GetOrdinal("Name"));
                    int s_id = reader.GetInt32(reader.GetOrdinal("ServiceID"));

                    ServiceDetail newSer = new ServiceDetail()
                    {
                        serviceID = s_id,
                        serviceName = s_name,
                        servicePrice = Convert.ToInt32(totalPrice)
                    };
                    _ser.Add(newSer);
                }
                reader.Close();
                return _ser;

            }
            return null;
        }*/

        public static void setPayment(int a_id, DateTime a_date)
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                connection.Open();
                var sql = @"
                    UPDATE Payment
                    SET Status = 1
                    WHERE AppointmentRecordID = @aID AND Date = @a_date;

                ";
                var command = new SqlCommand(sql, DB.Instance.Connection);
                command.Parameters.AddWithValue("@aID", a_id);
                command.Parameters.AddWithValue("@a_date", a_date);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Cập nhật thành công!");
                }
                else
                {
                    Console.WriteLine("Không có hàng nào được cập nhật.");
                }


            }
        }

        // Huy's Lost Update - non fix version
        public static int setFullPayment(int a_id, DateTime a_date, decimal paidAmount)
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("sp_LostUpdate_1", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@appointmentRecordID", a_id);
                    command.Parameters.AddWithValue("@date", a_date);
                    command.Parameters.AddWithValue("@paidAmount", paidAmount);

                    // Execute the command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Paid successfully", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error making payment.", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    return rowsAffected;
                }
            }
        }

        public static List<DentistAvailableTime> GetDentistAvailableTimes(DateTime dateTime)
        {
            using (SqlConnection connection = new SqlConnection(Instance.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SP_DIRTYREAD_4", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Date", dateTime);

                    using (var reader = command.ExecuteReader())
                    {
                        List<DentistAvailableTime> _dat = new List<DentistAvailableTime>();
                        while (reader.Read())
                        {
                            int d_shift = reader.GetInt32(reader.GetOrdinal("Shift"));
                            string d_name = reader.GetString(reader.GetOrdinal("Name"));
                            int d_id = reader.GetInt32(reader.GetOrdinal("DentistID"));

                            DentistAvailableTime newSer = new DentistAvailableTime()
                            {
                                shift = d_shift,
                                dentistID = d_id,
                                dentistName = d_name,
                                date = dateTime
                            };
                            _dat.Add(newSer);
                        }
                        reader.Close();
                        return _dat;
                    }
                }
                return null;
            }
        }
        public static bool GetDentistAvailableTimes(DateTime dateTime, int id, int shift)
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                connection.Open();
                // Use for repeatable 
                var command = new SqlCommand("GetDAT", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                command.Parameters.AddWithValue("@MANS", id);
                command.Parameters.AddWithValue("@Date", dateTime);
                command.Parameters.AddWithValue("@SHIFT", shift);

                // Execute the command
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false; // Return null if no row is found
                    }
                }
            }
        }

        public static int MakeAppointment(DentistAvailableTime dat, int? cusID, string note)
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                connection.Open();

                var command = new SqlCommand("Make_Appointment", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                command.Parameters.AddWithValue("@MANS", dat.dentistID);
                command.Parameters.AddWithValue("@Shift", dat.shift);
                command.Parameters.AddWithValue("@Ngay", dat.date);
                command.Parameters.AddWithValue("@KH", cusID);
                command.Parameters.AddWithValue("@Note", note);

                // Execute the command
                int rs = command.ExecuteNonQuery();

                return rs;
            }
        }

        public static BindingList<Service> getAllServices()
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                connection.Open();
                var sql = @"
                    select * from Service
                ";
                var command = new SqlCommand(sql, DB.Instance.Connection);
                var reader = command.ExecuteReader();
                BindingList<Service> _mec = new BindingList<Service>();
                while (reader.Read())
                {
                    string d_name = reader.GetString(reader.GetOrdinal("Name"));
                    int d_id = reader.GetInt32(reader.GetOrdinal("ServiceID"));
                    Decimal d_price = reader.GetDecimal(reader.GetOrdinal("Price"));
                    Service newSer = new Service()
                    {
                        serviceID = d_id,
                        serviceName = d_name,
                        price = d_price,
                    };
                    _mec.Add(newSer);
                }
                reader.Close();
                return _mec;
            }
            return null;
        }
        public static BindingList<Medicine> getAllMedicines()
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                connection.Open();
                // Use for repeatable
                var sql = @"
                    EXEC Select_AllMedical
                ";
                var command = new SqlCommand(sql, DB.Instance.Connection);
                var reader = command.ExecuteReader();
                BindingList<Medicine> _mec = new BindingList<Medicine>();
                while (reader.Read())
                {
                    int d_status = reader.GetInt32(reader.GetOrdinal("Status"));
                    string d_name = reader.GetString(reader.GetOrdinal("Name"));
                    int d_id = reader.GetInt32(reader.GetOrdinal("MedicalID"));
                    int d_invent = reader.GetInt32(reader.GetOrdinal("InventoryNumber"));
                    Decimal d_price = reader.GetDecimal(reader.GetOrdinal("Price"));
                    Medicine newSer = new Medicine()
                    {
                        status = d_status,
                        ID = d_id,
                        Name = d_name,
                        inventoryNumber = d_invent,
                        price = d_price,
                    };
                    _mec.Add(newSer);
                }
                reader.Close();
                return _mec;
            }
            return null;
        }
        public static CustomerClass LoadCustomerInfo(string username)
        {
            try
            {
                // Câu truy vấn SQL để lấy thông tin Customer từ database dựa trên username
                string query = "SELECT * FROM Customer WHERE PhoneNo = @Username";

                // Tạo và mở kết nối
                //DB dB = new DB();
                using (SqlConnection connection = DB.Instance.Connection)
                {
                    //connection.Open();
                    // Tạo đối tượng SqlCommand
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Thêm tham số cho câu truy vấn
                        command.Parameters.AddWithValue("@Username", username);

                        // Sử dụng SqlDataReader để đọc dữ liệu từ database
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Kiểm tra xem có dữ liệu hay không
                            if (reader.Read())
                            {
                                // Tạo đối tượng Customer từ SqlDataReader
                                CustomerClass customer = new CustomerClass(reader);
                                return customer;
                            }
                            else
                            {
                                // Trường hợp không tìm thấy thông tin Customer
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
        public static int addPatientRecord(int id)
        {
            int recordID = 0;

            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                //connection.Open();
                string query = "INSERT INTO PatientRecord (CustomerID) VALUES (@CustomerID); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CustomerID", id);

                    connection.Open();
                    recordID = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return recordID;
        }
        public static int addAppointmentRecord(int recordID, int appID)
        {
            int appointRecordID = 0;

            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                string query = "INSERT INTO AppointmentRecord (RecordID, AppointmentID) VALUES (@RecordID, @AppointmentID); SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordID", recordID);
                    command.Parameters.AddWithValue("@AppointmentID", appID);

                    connection.Open();
                    appointRecordID = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return appointRecordID;
        }
        public static void addServiceDetail (List<Service> _services, int appointmentRecordID)
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                connection.Open();

                foreach (Service service in _services)
                {
                    string query = "INSERT INTO ServiceDetail (ServiceID, AppointmentRecordID, Description) VALUES (@ServiceID, @AppointmentRecordID, NULL)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ServiceID", service.serviceID);
                        command.Parameters.AddWithValue("@AppointmentRecordID", appointmentRecordID);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public static void addMedicationRecord(List<Medicine>_medicines, int appointmentRecordID)
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                connection.Open();

                foreach (Medicine medicine in _medicines)
                {
                    string query = "INSERT INTO MedicationRecord (AppointRecordID, MedicalID, Quantity, TotalPrice) VALUES (@AppointRecordID, @MedicalID, @Quantity, @TotalPrice)";
                    string updateQuery = "UPDATE MedicationList SET InventoryNumber = InventoryNumber - @Quantity WHERE MedicalID = @MedicalID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AppointRecordID", appointmentRecordID);
                        command.Parameters.AddWithValue("@MedicalID", medicine.ID);
                        command.Parameters.AddWithValue("@Quantity", medicine.quantity);
                        command.Parameters.AddWithValue("@TotalPrice", medicine.quantity * medicine.price); // Assume you have a method to get the price for a medicine

                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@MedicalID", medicine.ID);
                        command.Parameters.AddWithValue("@Quantity", medicine.quantity);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public static void AddPayment(DateTime date, decimal fee, decimal totalPrice, int appointmentRecordID)
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                string query = @"INSERT INTO Payment (Date, Fee, TotalPrice, Status, StaffID, AppointmentRecordID) 
                             VALUES (@Date, @Fee, @TotalPrice, 0, 1, @AppointmentRecordID)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", date);
                    command.Parameters.AddWithValue("@Fee", fee);
                    command.Parameters.AddWithValue("@TotalPrice", totalPrice);
                    command.Parameters.AddWithValue("@AppointmentRecordID", appointmentRecordID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void updateAppointmentComplete(Appointment _app)
        {
            using (SqlConnection connection = new SqlConnection(DB.Instance.ConnectionString))
            {
                string query = @"UPDATE Appointment SET Status = N'Đã xong'
                                  where AppointmentID = @id      ";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", _app.AppointmentID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
