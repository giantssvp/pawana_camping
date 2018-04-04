using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;

namespace pawana_camping.Models
{
    public class db_connect
    {
        private MySqlConnection connection;
        public List<string>[] list_feedback_show = new List<string>[3];
        public List<string>[] list_time_show = new List<string>[1];
        public List<string>[] list_gallery_show = new List<string>[2];
        public List<string>[] list_events_show = new List<string>[3];
        public List<string>[] list_bookings_show = new List<string>[14];

        private bool OpenConnection()
        {
            string connetionString = null;
            connetionString = "server=182.50.133.77;database=pawna_camping;uid=pawnaadmin;pwd=Pawnaadmin@123;Allow User Variables=True;";
            connection = new MySqlConnection(connetionString);
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {

                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        public int insert_feedback(string name, string email, string phone, string sub, string msg)
        {
            try
            {
                int id = -1;
                string query = "INSERT INTO feedback (name, email, phone, subject, message, status, date) VALUES(@name, @email, @phone, @sub, @msg, @sts, NOW())";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@sub", sub);
                    cmd.Parameters.AddWithValue("@msg", msg);
                    cmd.Parameters.AddWithValue("@sts", false);

                    cmd.ExecuteNonQuery();

                    MySqlCommand cmd1 = new MySqlCommand("SELECT LAST_INSERT_ID()", connection);
                    id = Convert.ToInt32(cmd1.ExecuteScalar());

                    this.CloseConnection();
                }
                return id;
            }
            catch (MySqlException ex)
            {
                return -1;
            }
        }

        public int Insert_Booking(string transaction_id, string transaction_status, string transaction_date, string product_info,
                                  string name, string email, string phone, string booking_date, int adult, int children,
                                  int part_payment, int paid_amount)
        {
            try
            {
                int total_amount = ((adult * get_rates("adult")) + (children * get_rates("child")));
                string query = "INSERT INTO booking_details (transaction_id, transaction_status, transaction_date, product_info," + 
                                  "name, email, phone, booking_date, adults, children, total_amount, part_payment, paid_amount)VALUES(\"" + 
                                  transaction_id + "\",\"" + transaction_status + "\",\"" + transaction_date + "\",\"" + product_info + "\",\"" + name + "\",\"" + email + "\",\"" + phone + "\",\"" +
                                  booking_date + "\"," + adult + "," + children + "," + total_amount + "," + part_payment + "," + paid_amount + ")";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();

                    this.CloseConnection();
                }
                return 0;
            }
            catch (MySqlException ex)
            {
                return ex.Number;
            }
        }

        public List<string>[] feedback_show(int offset, int limit)
        {
            try
            {
                string query = "SELECT * FROM feedback ORDER BY ID DESC LIMIT @limit OFFSET @offset";

                list_feedback_show[0] = new List<string>();
                list_feedback_show[1] = new List<string>();
                list_feedback_show[2] = new List<string>();

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@offset", offset);
                    cmd.Parameters.AddWithValue("@limit", limit);
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        list_feedback_show[0].Add(dataReader["subject"] + "");
                        list_feedback_show[1].Add(dataReader["message"] + "");
                        list_feedback_show[2].Add(dataReader["date"] + "");
                    }

                    dataReader.Close();
                    this.CloseConnection();
                    return list_feedback_show;
                }
                else
                {
                    return list_feedback_show;
                }
            }
            catch (MySqlException ex)
            {
                return list_feedback_show;
            }
        }

        public List<string>[] time_slot_show(string date)
        {
            try
            {
                string query = "SELECT start_time FROM booking where date = @date";

                list_time_show[0] = new List<string>();
                //list_feedback_show[1] = new List<string>();
                //list_feedback_show[2] = new List<string>();

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@date", date);
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        list_time_show[0].Add(dataReader["start_time"] + "");
                    }

                    dataReader.Close();
                    this.CloseConnection();
                    return list_time_show;
                }
                else
                {
                    return list_time_show;
                }
            }
            catch (MySqlException ex)
            {
                return list_time_show;
            }
        }

        /*News Section */
        public List<string>[] events_show(int offset, int limit)
        {
            try
            {
                string query = "SELECT * FROM events ORDER BY ID DESC LIMIT @limit OFFSET @offset";

                list_events_show[0] = new List<string>();
                list_events_show[1] = new List<string>();
                list_events_show[2] = new List<string>();


                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@offset", offset);
                    cmd.Parameters.AddWithValue("@limit", limit);
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        list_events_show[0].Add(dataReader["Heading"] + "");
                        list_events_show[1].Add(dataReader["Description"] + "");
                        list_events_show[2].Add(dataReader["Date"] + "");
                    }

                    dataReader.Close();
                    this.CloseConnection();
                    return list_events_show;
                }
                else
                {
                    return list_events_show;
                }
            }
            catch (MySqlException ex)
            {
                return list_events_show;
            }
        }

        public List<string>[] bookings_show(int offset, int limit)
        {
            try
            {
                string query = "SELECT * FROM booking_details ORDER BY ID DESC LIMIT @limit OFFSET @offset";

                list_bookings_show[0] = new List<string>();
                list_bookings_show[1] = new List<string>();
                list_bookings_show[2] = new List<string>();
                list_bookings_show[3] = new List<string>();
                list_bookings_show[4] = new List<string>();
                list_bookings_show[5] = new List<string>();
                list_bookings_show[6] = new List<string>();
                list_bookings_show[7] = new List<string>();
                list_bookings_show[8] = new List<string>();
                list_bookings_show[9] = new List<string>();
                list_bookings_show[10] = new List<string>();
                list_bookings_show[11] = new List<string>();
                list_bookings_show[12] = new List<string>();
                list_bookings_show[13] = new List<string>();


                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@offset", offset);
                    cmd.Parameters.AddWithValue("@limit", limit);
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        list_bookings_show[0].Add(dataReader["ID"] + "");
                        list_bookings_show[1].Add(dataReader["transaction_id"] + "");
                        list_bookings_show[2].Add(dataReader["transaction_status"] + "");
                        list_bookings_show[3].Add(dataReader["transaction_date"] + "");
                        list_bookings_show[4].Add(dataReader["product_info"] + "");
                        list_bookings_show[5].Add(dataReader["name"] + "");
                        list_bookings_show[6].Add(dataReader["email"] + "");
                        list_bookings_show[7].Add(dataReader["phone"] + "");
                        list_bookings_show[8].Add(dataReader["booking_date"] + "");
                        list_bookings_show[9].Add(dataReader["adults"] + "");
                        list_bookings_show[10].Add(dataReader["children"] + "");
                        list_bookings_show[11].Add(dataReader["total_amount"] + "");
                        list_bookings_show[12].Add(dataReader["part_payment"] + "");
                        list_bookings_show[13].Add(dataReader["paid_amount"] + "");
                    }

                    dataReader.Close();
                    this.CloseConnection();
                    return list_bookings_show;
                }
                else
                {
                    return list_bookings_show;
                }
            }
            catch (MySqlException ex)
            {
                return list_bookings_show;
            }
        }

        public int booking_count()
        {
            try
            {
                int count = 0;
                string query = "select count(id) from booking_details";
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                    this.CloseConnection();
                }
                return count;
            }
            catch (MySqlException ex)
            {
                return 0;
            }
        }

        public int feedback_count()
        {
            try
            {
                int count = 0;
                string query = "select count(id) from feedback";
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                    this.CloseConnection();
                }
                return count;
            }
            catch (MySqlException ex)
            {
                return 0;
            }
        }

        public int event_count()
        {
            try
            {
                int count = 0;
                string query = "select count(id) from pawna_camping.events";
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                    this.CloseConnection();
                }
                return count;
            }
            catch (MySqlException ex)
            {
                return 0;
            }
        }

        public int get_rates(string age_grp)
        {
            try
            {
                int count = 0;
                string query = "select amount from pawna_camping.rates where age_group = \"" + age_grp + "\"";
                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                    this.CloseConnection();
                }
                return count;
            }
            catch (MySqlException ex)
            {
                return 0;
            }
        }
        
        public int update_rates(string base_adult, string base_child)
        {
            try
            {
                int id = -1;
                string query1 = "UPDATE rates SET amount=@base_adult WHERE age_group=\"adult\"";
                string query2 = "UPDATE rates SET amount=@base_child WHERE age_group=\"child\"";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query1, connection);
                    cmd.Parameters.AddWithValue("@base_adult", Int32.Parse(base_adult));
                    MySqlCommand cmd1 = new MySqlCommand(query2, connection);
                    cmd1.Parameters.AddWithValue("@base_child", Int32.Parse(base_child));

                    cmd.ExecuteNonQuery();
                    cmd1.ExecuteNonQuery();

                    this.CloseConnection();
                }
                return id;
            }
            catch (MySqlException ex)
            {
                return -1;
            }
        }

        public int Insert_Event(string heading, string description)
        {
            try
            {
                int id = -1;
                string query = "INSERT INTO events (Heading, Description, Date) VALUES(@heading, @description, NOW())";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@heading", heading);
                    cmd.Parameters.AddWithValue("@description", description);

                    cmd.ExecuteNonQuery();

                    this.CloseConnection();
                }
                return id;
            }
            catch (MySqlException ex)
            {
                return -1;
            }
        }
        /*News Section*/

        /*Password Hashing*/
        private static string getHash(string text)
        {
            // SHA512 is disposable by inheritance. 
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash. 
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                // Get the hashed string. 
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        /*Login Section*/
        public Boolean Login(string name, string password)
        {
            try
            {
                var pass = password + "4804";
                var hash = getHash(pass);
                MySqlDataReader rdr;
                string query = "select password from login where username = @name and password = @password";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@password", hash);
                    rdr = cmd.ExecuteReader();
                    
                    if (rdr.Read())
                    {
                        this.CloseConnection();
                        return true;
                    }
                }
                this.CloseConnection();
                return false;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }
        /*Login Section*/

    } //db_connect class
    
} // namespace