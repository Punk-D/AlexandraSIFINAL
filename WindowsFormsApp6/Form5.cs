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
    public partial class Form5 : Form
    {
        public Form5(int u)
        {
            InitializeComponent();
            userID = u;
            MenuForm_Load();
        }
        int userID;
        public static string connectionString = @"Server=(localdb)\Local;Database=ROTARUSIFINAL;Integrated Security=true;";
        SqlConnection connection = new SqlConnection(connectionString);
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
        private void button2_Click(object sender, EventArgs e)
        {
            string receiverPhoneNumber = txtPhoneNumber.Text.Trim(); // Get the phone number from the user
            decimal amount = decimal.Parse(txtAmount.Text); // Amount to send

            if (cmbCards.SelectedItem == null)
            {
                MessageBox.Show("Please select a sender card.");
                return;
            }

            string selectedCardEnding = cmbCards.SelectedItem.ToString().Split(':')[0].Trim(); // Get the last 4 digits of the sender's card
            string senderCardNo = null;
            string receiverCardNo = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Get full sender card number based on the last 4 digits
                SqlCommand cmdFetchSenderCard = new SqlCommand(
                    "SELECT CardNo FROM Cards WHERE UserID = @UserID AND RIGHT(CardNo, 4) = @CardEnding", conn);
                cmdFetchSenderCard.Parameters.AddWithValue("@UserID", userID);
                cmdFetchSenderCard.Parameters.AddWithValue("@CardEnding", selectedCardEnding);

                senderCardNo = cmdFetchSenderCard.ExecuteScalar() as string;

                if (string.IsNullOrEmpty(senderCardNo))
                {
                    MessageBox.Show("Could not find the full card number for the selected sender card.");
                    return;
                }

                // Get the first receiver card number associated with the provided phone number
                SqlCommand cmdFetchReceiverCard = new SqlCommand(
                    "SELECT TOP 1 c.CardNo FROM Users u JOIN Cards c ON u.UserID = c.UserID WHERE u.Phone_number = @PhoneNumber", conn);
                cmdFetchReceiverCard.Parameters.AddWithValue("@PhoneNumber", receiverPhoneNumber);

                receiverCardNo = cmdFetchReceiverCard.ExecuteScalar() as string;

                if (string.IsNullOrEmpty(receiverCardNo))
                {
                    MessageBox.Show("No card found for the provided phone number.");
                    return;
                }

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
