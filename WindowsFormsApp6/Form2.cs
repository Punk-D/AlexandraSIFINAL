using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsFormsApp6
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public static string connectionString = @"Server=(localdb)\Local;Database=ROTARUSIFINAL;Integrated Security=true;";
        SqlConnection connection = new SqlConnection(connectionString);
        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            this.Hide();
            form1.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userID = txtUserID.Text; // IDNP, provided by the user
            string name = txtName.Text;
            string surname = txtSurname.Text;
            string phone = txtPhone.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            // Basic validation for empty fields
            if (string.IsNullOrWhiteSpace(userID) || string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if the UserID (IDNP) already exists
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE UserID = @UserID", conn);
                checkCmd.Parameters.AddWithValue("@UserID", userID);
                int exists = (int)checkCmd.ExecuteScalar();

                if (exists > 0)
                {
                    MessageBox.Show("The provided User ID (IDNP) is already registered.");
                    return;
                }

                // Insert into Users table
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Users (UserID, Names, Surname, Phone_number) VALUES (@UserID, @Name, @Surname, @Phone)", conn);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Surname", surname);
                cmd.Parameters.AddWithValue("@Phone", phone);

                cmd.ExecuteNonQuery();

                // Insert into Accounts table
                SqlCommand cmd2 = new SqlCommand(
                    "INSERT INTO Accounts (Logins, Email, Passwords, UserID) VALUES (@Login, @Email, @Password, @UserID)", conn);
                cmd2.Parameters.AddWithValue("@Login", email);
                cmd2.Parameters.AddWithValue("@Email", email);
                cmd2.Parameters.AddWithValue("@Password", password);
                cmd2.Parameters.AddWithValue("@UserID", userID);

                cmd2.ExecuteNonQuery();

                MessageBox.Show("Registration successful.");
                this.Close();
            }
        }
    }
}

