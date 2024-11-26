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
        public static string connectionString = @"Server=(localdb)\Local;Database=ROTARUSIFINAL;Integrated Security=true;";
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

            // Get the selected sender card's last 4 digits from the combo box
            if (cmbCards.SelectedItem == null)
            {
                MessageBox.Show("Please select a sender card.");
                return;
            }

            string selectedCardEnding = cmbCards.SelectedItem.ToString().Split(':')[0].Trim(); // Get the last 4 digits

            string senderCardNo = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Fetch the full card number based on the last 4 digits for the logged-in user
                SqlCommand cmdFetchCard = new SqlCommand(
                    "SELECT CardNo FROM Cards WHERE UserID = @UserID AND RIGHT(CardNo, 4) = @CardEnding", conn);
                cmdFetchCard.Parameters.AddWithValue("@UserID", userID);
                cmdFetchCard.Parameters.AddWithValue("@CardEnding", selectedCardEnding);

                senderCardNo = cmdFetchCard.ExecuteScalar() as string;

                if (string.IsNullOrEmpty(senderCardNo))
                {
                    MessageBox.Show("Could not find the full card number for the selected card.");
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
