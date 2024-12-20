﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace WindowsFormsApp6
{
    public partial class Form6 : Form
    {
        public Form6(int u)
        {
            InitializeComponent();
            LoggedInUserID = u;
            MenuForm_Load();
        }

        private void MenuForm_Load()
        {
            cmbCards.Items.Clear();
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

        int LoggedInUserID;
        public static string connectionString = @"Server=(localdb)\Local;Database=ROTARUSIFINAL;Integrated Security=true;";
        SqlConnection conn = new SqlConnection(connectionString);
        private void button2_Click(object sender, EventArgs e)
        {
            conn.Open();

            // Step 1: Validate the T2C code and fetch its details
            SqlCommand cmdValidate = new SqlCommand(
                "SELECT Amount FROM T2C_Receivers WHERE Code = @Code AND UserID = @UserID", conn);
            cmdValidate.Parameters.AddWithValue("@Code", t2cCode.Text);
            cmdValidate.Parameters.AddWithValue("@UserID", userID.Text);

            decimal amount;
            object result = cmdValidate.ExecuteScalar();

            if (result == null)
            {
                MessageBox.Show("Invalid or expired T2C code.");
                return;
            }
            else
            {
                amount = (decimal)result;
            }

            // Step 2: Select the first card or use the combo box selection
            string selectedCard;
            if (cmbCards.SelectedItem != null)
            {
                selectedCard = cmbCards.SelectedItem.ToString().Split(':')[0].Trim(); // Use ComboBox selection
            }
            else
            {
                // Default to the first card available
                SqlCommand cmdGetCard = new SqlCommand(
                    "SELECT TOP 1 CardNo FROM Cards WHERE UserID = @UserID ORDER BY CardNo", conn);
                cmdGetCard.Parameters.AddWithValue("@UserID", userID.Text);

                object cardResult = cmdGetCard.ExecuteScalar();
                if (cardResult == null)
                {
                    MessageBox.Show("No available card found to receive the funds.");
                    return;
                }
                selectedCard = cardResult.ToString();
            }

            // Step 3: Update the card's balance
            SqlCommand cmdUpdateBalance = new SqlCommand(
                "UPDATE Cards SET Balance = Balance + @Amount WHERE CardNo = @CardNo", conn);
            cmdUpdateBalance.Parameters.AddWithValue("@Amount", amount);
            cmdUpdateBalance.Parameters.AddWithValue("@CardNo", selectedCard);
            cmdUpdateBalance.ExecuteNonQuery();

            // Step 4: Delete the T2C code to prevent reuse
            SqlCommand cmdDeleteT2C = new SqlCommand(
                "DELETE FROM T2C_Receivers WHERE Code = @Code AND UserID = @UserID", conn);
            cmdDeleteT2C.Parameters.AddWithValue("@Code", t2cCode.Text);
            cmdDeleteT2C.Parameters.AddWithValue("@UserID", userID.Text);
            cmdDeleteT2C.ExecuteNonQuery();

            MessageBox.Show($"T2C funds of {amount:C} received successfully to card ending in {selectedCard.Substring(selectedCard.Length - 4)}.");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void Form6_Load(object sender, EventArgs e)
        {

        }
    }
}
