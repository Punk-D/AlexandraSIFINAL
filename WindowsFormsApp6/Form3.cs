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
    public partial class Form3 : Form
    {
        public Form3(int UserID)
        {
            InitializeComponent();
            LoggedInUserID = UserID;
            MenuForm_Load();
        }
        int LoggedInUserID;
        public static string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=ROTARUSIFINAL;Integrated Security=true;";
        SqlConnection connection = new SqlConnection(connectionString);
        private void MenuForm_Load()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT CardNo, Balance FROM Cards WHERE UserID = @UserID", conn);
                cmd.Parameters.AddWithValue("@UserID", LoggedInUserID);

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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateCard();
            MenuForm_Load();
        }

        private void CreateCard()
        {
            string cardNo = GenerateCardNumber();
            string cvc = GenerateCVC();
            DateTime expiryDate = DateTime.Now.AddYears(2); // Expiration date: 2 years from now
            decimal balance = 0.00M; // Default balance for the new card

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Prepare the insert command to include Balance
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Cards (CardNo, ExpiryDate, CVC, UserID, Balance) VALUES (@CardNo, @ExpiryDate, @CVC, @UserID, @Balance)", conn);
                cmd.Parameters.AddWithValue("@CardNo", cardNo);
                cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate);
                cmd.Parameters.AddWithValue("@CVC", cvc);
                cmd.Parameters.AddWithValue("@UserID", LoggedInUserID); // LoggedInUserID should be set appropriately
                cmd.Parameters.AddWithValue("@Balance", balance); // Insert the default balance

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Card Created! Card No: {cardNo}, CVC: {cvc}, Expiry Date: {expiryDate.ToShortDateString()}, Balance: {balance}");
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Error creating card: {ex.Message}");
                }
            }
        }


        private string GenerateCardNumber()
        {
            Random random = new Random();
            StringBuilder cardNo = new StringBuilder();

            // Generate a 16-digit card number
            for (int i = 0; i < 16; i++)
            {
                cardNo.Append(random.Next(0, 10));
            }

            return cardNo.ToString();
        }

        private string GenerateCVC()
        {
            Random random = new Random();
            return random.Next(100, 1000).ToString(); // Generate a 3-digit CVC
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4(LoggedInUserID);
            this.Hide();
            form4.ShowDialog();
            this.Show();
        }
    }
}
