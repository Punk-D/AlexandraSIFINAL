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

namespace WindowsFormsApp6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public int LoggedInUserID;
        public static string connectionString = @"Server=(localdb)\Local;Database=ROTARUSIFINAL;Integrated Security=true;";
        SqlConnection connection = new SqlConnection(connectionString);


        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            this.Hide();
            form2.ShowDialog();
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT UserID FROM Accounts WHERE Email = @Email AND Passwords = @Password", conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                var userID = cmd.ExecuteScalar();
                if (userID != null)
                {
                    LoggedInUserID = Convert.ToInt32(userID); // Store user ID globally
                    this.Hide();
                    Form3 menu = new Form3(LoggedInUserID);
                    menu.Show();
                    //this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid login credentials.");
                }
            }
        }
    }
}
