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
    public partial class Form4 : Form
    {
        public static string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=ROTARUSIFINAL;Integrated Security=true;";
        SqlConnection connection = new SqlConnection(connectionString);
        public Form4(int u)
        {
            InitializeComponent();
            userID = u;
            MenuForm_Load();
        }
        int userID;
        private void MenuForm_Load()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT CardNo, Balance FROM Cards WHERE UserID = @UserID", conn);
                cmd.Parameters.AddWithValue("@UserID", userID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string cardNo = reader["CardNo"].ToString();
                        decimal balance = (decimal)reader["Balance"];
                        cmbCards.Items.Add($"{cardNo.Substring(cardNo.Length - 4)} : {balance}");
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string receiverCardNo = txtReceiverCard.Text;
            decimal amount = decimal.Parse(txtAmount.Text);

            // Get the selected sender card from the combo box (just like the previous steps)
            string senderCardNo = cmbCards.SelectedItem.ToString(); // Example: "<Last 4 digits of card no>"

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Call the stored procedure to perform the transaction
                SqlCommand cmd = new SqlCommand("DoTransaction", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SenderCardNo", senderCardNo);
                cmd.Parameters.AddWithValue("@GetterCardNo", receiverCardNo);
                cmd.Parameters.AddWithValue("@Amount", amount);

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Transaction completed successfully.");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Error processing transaction: {ex.Message}");
                }
            }

        }
    }
}
