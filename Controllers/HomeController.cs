using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace u24608174_Assignment_02.Controllers
{
    public class HomeController : Controller
    {
        public static string ConnectionString = "Data Source=DESKTOP-9MBPVRI\\SQLEXPRESS;Initial Catalog=RescuePet;Integrated Security=True";

        public ActionResult Index()
        {
            ViewBag.AdoptedCount = 0;
            var adoptions = new List<AdoptionRecord>();
            ViewBag.RecentAdoptions = adoptions;

            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                try
                {
                    myConnection.Open();

                    string countQuery = "SELECT COUNT(*) FROM Pets WHERE Pet_Status = 'Adopted'";
                    SqlCommand countCommand = new SqlCommand(countQuery, myConnection);
                    ViewBag.AdoptedCount = (int)countCommand.ExecuteScalar();

                    string adoptionQuery = @"SELECT u.User_Name, u.User_Surname, p.Pet_Name 
                                           FROM Adoptions a 
                                           INNER JOIN Users u ON a.User_ID = u.User_ID 
                                           INNER JOIN Pets p ON a.Pet_ID = p.Pet_ID 
                                           ORDER BY a.Adoption_Date DESC";

                    SqlCommand adoptionCommand = new SqlCommand(adoptionQuery, myConnection);
                    SqlDataReader Reader = adoptionCommand.ExecuteReader();

                    while (Reader.Read())
                    {
                        adoptions.Add(new AdoptionRecord
                        {
                            AdopterName = Reader["User_Name"].ToString() + " " + Reader["User_Surname"].ToString(),
                            PetName = Reader["Pet_Name"].ToString()
                        });
                    }
                    Reader.Close();
                }
                catch (Exception err)
                {
                    ViewBag.Message = "Error: " + err.Message;
                }
            }

            return View();
        }
    }

    public class AdoptionRecord
    {
        public string AdopterName { get; set; }
        public string PetName { get; set; }
    }
}