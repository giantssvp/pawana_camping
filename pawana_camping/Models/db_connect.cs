using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace pawana_camping.Models
{
    public class db_connect
    {
        private MySqlConnection connection;
        public List<string>[] list_feedback_show = new List<string>[3];
        public List<string>[] list_time_show = new List<string>[1];
        public List<string>[] list_gallery_show = new List<string>[2];

        public List<string>[] list_events_show = new List<string>[3];

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

        public int Insert(string name, string email, string comment_type, string sub, string msg)
        {
            try
            {
                int id = -1;
                string query = "INSERT INTO testimony (Name, Email_id, Comment_type, Subject, Message, Status, Date) VALUES(@name, @email, @com_type, @sub, @msg, @sts, NOW())";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@com_type", comment_type);
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

        public int Insert_Booking(string name_booking, string email_booking, string phone, string start_time, string session_type, string date)
        {
            try
            {
                string query = "INSERT INTO booking (Name, Email, Phone, Start_time, Session_type, Date) VALUES(@name, @email, @phone, @time, @session, @dt)";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@name", name_booking);
                    cmd.Parameters.AddWithValue("@email", email_booking);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@time", start_time);
                    cmd.Parameters.AddWithValue("@session", session_type);
                    cmd.Parameters.AddWithValue("@dt", date);
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

        public List<string>[] testimony_show(int offset)
        {
            try
            {
                //string query = "SELECT * FROM testimony ORDER BY Date DESC, ID DESC LIMIT 2 OFFSET @offset";
                string query = "SELECT * FROM testimony where Comment_type='Testimony'";

                list_feedback_show[0] = new List<string>();
                list_feedback_show[1] = new List<string>();
                list_feedback_show[2] = new List<string>();

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@offset", offset);
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        list_feedback_show[0].Add(dataReader["Subject"] + "");
                        list_feedback_show[1].Add(dataReader["Message"] + "");
                        list_feedback_show[2].Add(dataReader["Date"] + "");
                    }

                    dataReader.Close();
                    this.CloseConnection();
                    MessageBox.Show(list_feedback_show[0].ToString());
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
        public List<string>[] events_show(int offset)
        {
            try
            {
                string query = "SELECT * FROM events ORDER BY ID DESC LIMIT 2 OFFSET @offset";

                list_events_show[0] = new List<string>();
                list_events_show[1] = new List<string>();
                list_events_show[2] = new List<string>();


                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@offset", offset);
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

        /*Login Section*/
        public Boolean Login(string name, string password)
        {
            try
            {
                MySqlDataReader rdr;
                string query = "select * from login where username = @name and password = @password";

                if (this.OpenConnection() == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@password", password);
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