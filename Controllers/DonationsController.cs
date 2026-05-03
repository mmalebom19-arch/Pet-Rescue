using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace u24608174_Assignment_02.Controllers
{
    public class DonationsController : Controller
    {
        public static string ConnectionString = "Data Source=DESKTOP-9MBPVRI\\SQLEXPRESS;Initial Catalog=RescuePet;Integrated Security=True";

        public ActionResult Donations()
        {
            ViewBag.TotalDonations = 0;
            ViewBag.DonationGoal = 50000;
            ViewBag.ProgressPercentage = 0;
            ViewBag.GoalReached = false;
            ViewBag.RemainingAmount = 50000;
            ViewBag.Donations = new List<DonationData>();
            ViewBag.Users = new List<string>();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    myConnection.Open();

                    SqlCommand totalCommand = new SqlCommand("SELECT SUM(Donation_Amount) FROM Donations", myConnection);
                    var result = totalCommand.ExecuteScalar();
                    decimal totalDonations = result != DBNull.Value ? Convert.ToDecimal(result) : 0;

                    decimal donationGoal = 50000;
                    decimal progressPercentage = donationGoal > 0 ? (totalDonations / donationGoal) * 100 : 0;
                    bool goalReached = totalDonations >= donationGoal;

                    decimal remainingAmount = goalReached ? 0 : (donationGoal - totalDonations);

                    ViewBag.TotalDonations = totalDonations;
                    ViewBag.DonationGoal = donationGoal;
                    ViewBag.ProgressPercentage = progressPercentage;
                    ViewBag.GoalReached = goalReached;
                    ViewBag.RemainingAmount = remainingAmount;

                    string donationQuery = @"SELECT u.User_Name + ' ' + u.User_Surname as Donor, 
                                           d.Donation_Amount as Amount, d.Donation_Date as Date
                                           FROM Donations d
                                           INNER JOIN Users u ON d.User_ID = u.User_ID
                                           ORDER BY d.Donation_Date DESC";

                    SqlCommand donationCommand = new SqlCommand(donationQuery, myConnection);
                    SqlDataReader reader = donationCommand.ExecuteReader();

                    var donations = new List<DonationData>();
                    while (reader.Read())
                    {
                        donations.Add(new DonationData
                        {
                            Donor = reader["Donor"].ToString(),
                            Amount = Convert.ToDecimal(reader["Amount"]),
                            Date = Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd")
                        });
                    }
                    reader.Close();
                    ViewBag.Donations = donations;

                    PopulateUserDropdown(myConnection);
                }
            }
            catch (Exception err)
            {
                ViewBag.Message = "Error: " + err.Message;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Donations(string User, decimal Amount)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    myConnection.Open();

                    string getUserQuery = @"SELECT User_ID FROM Users 
                                  WHERE User_Name + ' ' + User_Surname = @User";

                    SqlCommand getUserCommand = new SqlCommand(getUserQuery, myConnection);
                    getUserCommand.Parameters.AddWithValue("@User", User);

                    var userResult = getUserCommand.ExecuteScalar();

                    if (userResult == null)
                    {
                        ViewBag.Message = "Error: User not found!";
                        return Donations();
                    }

                    int userId = Convert.ToInt32(userResult);

                    string insertQuery = @"INSERT INTO Donations (Donation_Amount, Donation_Date, User_ID)
                                 VALUES (@Amount, GETDATE(), @UserId)";

                    SqlCommand command = new SqlCommand(insertQuery, myConnection);
                    command.Parameters.AddWithValue("@Amount", Amount);
                    command.Parameters.AddWithValue("@UserId", userId);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        ViewBag.Message = "Thank you for your donation of R" + Amount + "!";
                }
            }
            catch (Exception err)
            {
                ViewBag.Message = "Error: " + err.Message;
            }

            return Donations();
        }

        private void PopulateUserDropdown(SqlConnection connection)
        {
            SqlCommand userCommand = new SqlCommand("SELECT User_Name, User_Surname FROM Users", connection);
            SqlDataReader userReader = userCommand.ExecuteReader();
            var users = new List<string>();
            while (userReader.Read())
            {
                users.Add(userReader["User_Name"].ToString() + " " + userReader["User_Surname"].ToString());
            }
            userReader.Close();
            ViewBag.Users = users;
        }

        public class DonationData
        {
            public string Donor { get; set; }
            public decimal Amount { get; set; }
            public string Date { get; set; }
        }
    }
}