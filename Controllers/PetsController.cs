using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace u24608174_Assignment_02.Controllers
{
    public class PetsController : Controller
    {
        public static string ConnectionString = "Data Source=DESKTOP-9MBPVRI\\SQLEXPRESS;Initial Catalog=RescuePet;Integrated Security=True";

        public ActionResult Index(string Type, string Breed, string Location)
        {
            ViewBag.Pets = new List<PetData>();
            ViewBag.Types = new List<string>();
            ViewBag.Breeds = new List<string>();
            ViewBag.Locations = new List<string>();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    myConnection.Open();
                    PopulateDropdowns(myConnection);

                    string query = @"SELECT 
                                    p.Pet_ID, 
                                    p.Pet_Name, 
                                    p.Pet_Age, 
                                    p.Pet_Weight, 
                                    p.Pet_Gender, 
                                    p.Pet_PetStory, 
                                    p.Pet_Status,
                                    p.Pet_ImagePath,
                                    pt.Type_Name, 
                                    pb.Breed_Name, 
                                    l.Location_Name, 
                                    u.User_Name + ' ' + u.User_Surname as Owner
                                FROM Pets p
                                INNER JOIN Pet_Types pt ON p.Type_ID = pt.Type_ID
                                INNER JOIN Pet_Breeds pb ON p.Breed_ID = pb.Breed_ID
                                INNER JOIN Locations l ON p.Location_ID = l.Location_ID
                                INNER JOIN Users u ON p.Pet_PostedBy = u.User_ID
                                WHERE 1=1";

                    var parameters = new List<SqlParameter>();

                    if (!string.IsNullOrEmpty(Type))
                    {
                        query += " AND pt.Type_Name = @Type";
                        parameters.Add(new SqlParameter("@Type", Type));
                    }
                    if (!string.IsNullOrEmpty(Breed))
                    {
                        query += " AND pb.Breed_Name = @Breed";
                        parameters.Add(new SqlParameter("@Breed", Breed));
                    }
                    if (!string.IsNullOrEmpty(Location))
                    {
                        query += " AND l.Location_Name = @Location";
                        parameters.Add(new SqlParameter("@Location", Location));
                    }

                    SqlCommand command = new SqlCommand(query, myConnection);
                    command.Parameters.AddRange(parameters.ToArray());

                    SqlDataReader reader = command.ExecuteReader();
                    var pets = new List<PetData>();

                    while (reader.Read())
                    {
                        pets.Add(new PetData
                        {
                            PetID = Convert.ToInt32(reader["Pet_ID"]),
                            PetName = reader["Pet_Name"].ToString(),
                            Age = Convert.ToInt32(reader["Pet_Age"]),
                            Weight = Convert.ToDecimal(reader["Pet_Weight"]),
                            Gender = reader["Pet_Gender"].ToString(),
                            PetStory = reader["Pet_PetStory"].ToString(),
                            Status = reader["Pet_Status"].ToString(),
                            ImagePath = reader["Pet_ImagePath"] != DBNull.Value ? reader["Pet_ImagePath"].ToString() : "",
                            BreedName = reader["Breed_Name"].ToString(),
                            LocationName = reader["Location_Name"].ToString(),
                            Owner = reader["Owner"].ToString()
                        });
                    }
                    reader.Close();
                    ViewBag.Pets = pets;
                }
            }
            catch (Exception err)
            {
                ViewBag.ErrorMessage = "Database Error: " + err.Message;
            }

            return View("Pets");
        }

        public ActionResult Adopt(int id)
        {
            ViewBag.Users = new List<UserData>();

            try
            {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    myConnection.Open();
                    string query = @"SELECT p.*, l.Location_Name FROM Pets p 
                                   INNER JOIN Locations l ON p.Location_ID = l.Location_ID 
                                   WHERE p.Pet_ID = @PetID";
                    SqlCommand command = new SqlCommand(query, myConnection);
                    command.Parameters.AddWithValue("@PetID", id);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        ViewBag.Pet = new PetData
                        {
                            PetID = Convert.ToInt32(reader["Pet_ID"]),
                            PetName = reader["Pet_Name"].ToString(),
                            Age = Convert.ToInt32(reader["Pet_Age"]),
                            Weight = Convert.ToDecimal(reader["Pet_Weight"]),
                            Gender = reader["Pet_Gender"].ToString(),
                            PetStory = reader["Pet_FullStory"].ToString(),
                            LocationName = reader["Location_Name"].ToString(),
                            ImagePath = reader["Pet_ImagePath"] != DBNull.Value ? reader["Pet_ImagePath"].ToString() : ""
                        };
                    }
                    reader.Close();
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
        public ActionResult ProcessAdoption(int PetID, int UserID)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    myConnection.Open();

                    SqlCommand adoptCommand = new SqlCommand("INSERT INTO Adoptions (Adoption_Date, Pet_ID, User_ID) VALUES (GETDATE(), @PetID, @UserID)", myConnection);
                    adoptCommand.Parameters.AddWithValue("@PetID", PetID);
                    adoptCommand.Parameters.AddWithValue("@UserID", UserID);
                    adoptCommand.ExecuteNonQuery();

                    SqlCommand updateCommand = new SqlCommand("UPDATE Pets SET Pet_Status = 'Adopted' WHERE Pet_ID = @PetID", myConnection);
                    updateCommand.Parameters.AddWithValue("@PetID", PetID);
                    updateCommand.ExecuteNonQuery();

                    ViewBag.Message = "Pet adopted successfully!";
                }
            }
            catch (Exception err)
            {
                ViewBag.Message = "Error: " + err.Message;
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Post()
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    myConnection.Open();
                    PopulateDropdowns(myConnection);
                    PopulateUserDropdown(myConnection);
                }
            }
            catch (Exception err)
            {
                ViewBag.Message = "Error: " + err.Message;
                ViewBag.Types = new List<string>();
                ViewBag.Breeds = new List<string>();
                ViewBag.Locations = new List<string>();
                ViewBag.Users = new List<UserData>();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Post(string PetName, string Type, string Breed, string Location, int Age, decimal Weight, string Gender, string PostedBy, string PetStory, HttpPostedFileBase PetImage)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    myConnection.Open();
                    string imagePath = "/Images/placeholder.png";
                    if (PetImage != null && PetImage.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(PetImage.FileName);
                        string serverPath = Server.MapPath("~/Images/");
                        if (!Directory.Exists(serverPath))
                            Directory.CreateDirectory(serverPath);

                        string fullPath = Path.Combine(serverPath, fileName);
                        PetImage.SaveAs(fullPath);
                        imagePath = "/Images/" + fileName;
                    }

                    string getUserQuery = @"SELECT User_ID FROM Users WHERE User_Name + ' ' + User_Surname = @PostedBy";
                    SqlCommand getUserCommand = new SqlCommand(getUserQuery, myConnection);
                    getUserCommand.Parameters.AddWithValue("@PostedBy", PostedBy);
                    var userResult = getUserCommand.ExecuteScalar();

                    if (userResult == null)
                    {
                        ViewBag.Message = "Error: User not found!";
                        return Post();
                    }
                    int userId = Convert.ToInt32(userResult);

                    string getTypeQuery = "SELECT Type_ID FROM Pet_Types WHERE Type_Name = @Type";
                    SqlCommand getTypeCommand = new SqlCommand(getTypeQuery, myConnection);
                    getTypeCommand.Parameters.AddWithValue("@Type", Type);
                    int typeId = Convert.ToInt32(getTypeCommand.ExecuteScalar());

                    string getBreedQuery = "SELECT Breed_ID FROM Pet_Breeds WHERE Breed_Name = @Breed";
                    SqlCommand getBreedCommand = new SqlCommand(getBreedQuery, myConnection);
                    getBreedCommand.Parameters.AddWithValue("@Breed", Breed);
                    int breedId = Convert.ToInt32(getBreedCommand.ExecuteScalar());

                    string getLocationQuery = "SELECT Location_ID FROM Locations WHERE Location_Name = @Location";
                    SqlCommand getLocationCommand = new SqlCommand(getLocationQuery, myConnection);
                    getLocationCommand.Parameters.AddWithValue("@Location", Location);
                    int locationId = Convert.ToInt32(getLocationCommand.ExecuteScalar());

                    string insertQuery = @"INSERT INTO Pets (Pet_Name, Pet_Age, Pet_Weight, Pet_Gender, Pet_PetStory, Pet_FullStory, Pet_Status, Pet_ImagePath, Pet_PostedBy, Type_ID, Breed_ID, Location_ID) 
                                 VALUES (@PetName, @Age, @Weight, @Gender, @PetStory, @PetStory, 'Available', @ImagePath, @UserId, @TypeId, @BreedId, @LocationId)";

                    SqlCommand command = new SqlCommand(insertQuery, myConnection);
                    command.Parameters.AddWithValue("@PetName", PetName);
                    command.Parameters.AddWithValue("@Age", Age);
                    command.Parameters.AddWithValue("@Weight", Weight);
                    command.Parameters.AddWithValue("@Gender", Gender);
                    command.Parameters.AddWithValue("@PetStory", PetStory);
                    command.Parameters.AddWithValue("@ImagePath", imagePath);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@TypeId", typeId);
                    command.Parameters.AddWithValue("@BreedId", breedId);
                    command.Parameters.AddWithValue("@LocationId", locationId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        ViewBag.Message = "Pet posted successfully!";
                }
            }
            catch (Exception err)
            {
                ViewBag.Message = "Error: " + err.Message;
            }
            return View();
        }

        private void PopulateDropdowns(SqlConnection connection)
        {
            SqlCommand typeCommand = new SqlCommand("SELECT Type_Name FROM Pet_Types", connection);
            SqlDataReader typeReader = typeCommand.ExecuteReader();
            var types = new List<string>();
            while (typeReader.Read())
            {
                types.Add(typeReader["Type_Name"].ToString());
            }
            typeReader.Close();
            ViewBag.Types = types;

            SqlCommand breedCommand = new SqlCommand("SELECT Breed_Name FROM Pet_Breeds", connection);
            SqlDataReader breedReader = breedCommand.ExecuteReader();
            var breeds = new List<string>();
            while (breedReader.Read())
            {
                breeds.Add(breedReader["Breed_Name"].ToString());
            }
            breedReader.Close();
            ViewBag.Breeds = breeds;

            SqlCommand locationCommand = new SqlCommand("SELECT Location_Name FROM Locations", connection);
            SqlDataReader locationReader = locationCommand.ExecuteReader();
            var locations = new List<string>();
            while (locationReader.Read())
            {
                locations.Add(locationReader["Location_Name"].ToString());
            }
            locationReader.Close();
            ViewBag.Locations = locations;
        }

        private void PopulateUserDropdown(SqlConnection connection)
        {
            SqlCommand userCommand = new SqlCommand("SELECT User_ID, User_Name, User_Surname, User_PhoneNumber FROM Users", connection);
            SqlDataReader userReader = userCommand.ExecuteReader();

            var users = new List<UserData>();

            while (userReader.Read())
            {
                users.Add(new UserData
                {
                    UserID = Convert.ToInt32(userReader["User_ID"]),
                    FullName = userReader["User_Name"].ToString() + " " + userReader["User_Surname"].ToString(),
                    PhoneNumber = userReader["User_PhoneNumber"].ToString()
                });
            }
            userReader.Close();
            ViewBag.Users = users;
        }

        public JsonResult GetBreedsByType(string typeName)
        {
            var breeds = new List<string>();
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                myConnection.Open();
                string query = @"SELECT pb.Breed_Name FROM Pet_Breeds pb
                       INNER JOIN Pet_Types pt ON pb.Type_ID = pt.Type_ID
                       WHERE pt.Type_Name = @TypeName";
                SqlCommand cmd = new SqlCommand(query, myConnection);
                cmd.Parameters.AddWithValue("@TypeName", typeName);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    breeds.Add(reader["Breed_Name"].ToString());
                }
                reader.Close();
            }
            return Json(breeds, JsonRequestBehavior.AllowGet);
        }
    }

    public class PetData
    {
        public int PetID { get; set; }
        public string PetName { get; set; }
        public int Age { get; set; }
        public decimal Weight { get; set; }
        public string Gender { get; set; }
        public string PetStory { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
        public string BreedName { get; set; }
        public string LocationName { get; set; }
        public string Owner { get; set; }
    }

    public class UserData
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }
}