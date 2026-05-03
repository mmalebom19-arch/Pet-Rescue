Use Master
GO

If exists (Select * from sys.databases where name = 'RescuePet')
Drop Database RescuePet
GO

Create Database RescuePet
GO

Use RescuePet
GO

-------------------------------------Tables--------------------------

--------Users---------
Create Table Users
(
    User_ID int Primary Key identity(1,1) NOT NULL,
    User_Name varchar(50),
    User_Surname varchar(50),
    User_PhoneNumber varchar(14),
    User_Email varchar(100)  
)

--------Pet_Types---------
Create Table Pet_Types
(
    Type_ID int Primary Key identity(1,1) NOT NULL,
    Type_Name varchar(50)
)

--------Pet_Breeds---------
Create Table Pet_Breeds
(
    Breed_ID int Primary Key identity(1,1) NOT NULL,
    Breed_Name varchar(50),
    Type_ID int references Pet_Types(Type_ID)
)

--------Locations---------
Create Table Locations
(
    Location_ID int Primary Key identity(1,1) NOT NULL,
    Location_Name varchar(100)
)

--------Pets---------
Create Table Pets
(
    Pet_ID int primary key identity(1,1) not null,
    Pet_Name varchar(50),
    Pet_Age int,
    Pet_Weight decimal(5,2),
    Pet_Gender varchar(15),
    Pet_PetStory varchar(MAX),     
    Pet_FullStory varchar(MAX),     
    Pet_Status varchar(14),
    Pet_ImagePath varchar(MAX),
    Pet_PostedBy int references Users(User_ID),
    Type_ID int references Pet_Types(Type_ID),
    Breed_ID int references Pet_Breeds(Breed_ID),
    Location_ID int references Locations(Location_ID)
)

--------Adoptions---------
Create Table Adoptions
(
    Adoption_ID int Primary Key identity(1,1) NOT NULL,
    Adoption_Date datetime,
    Pet_ID int references Pets(Pet_ID),
    User_ID int references Users(User_ID)
)

--------Donations---------
Create Table Donations
(
    Donation_ID int Primary Key identity(1,1) NOT NULL,
    Donation_Amount decimal(10,2),
    Donation_Date datetime,
    User_ID int references Users(User_ID)
)
GO



--------Users---------
INSERT INTO Users (User_Name, User_Surname, User_PhoneNumber, User_Email) VALUES
('Maria', 'Lawliet', '0674307946', 'maria@email.com'),
('Jason', 'Smith', '0821002003', 'jason@email.com'),
('Thabo', 'Zulu', '0835551122', 'thabo@email.com'),
('Lola', 'Smith', '0712345678', 'lerato@email.com'),
('James', 'Brown', '0849876543', 'james@email.com'),
('Sarah', 'Johnson', '0791122334', 'sarah@email.com'),
('David', 'Williams', '0815566778', 'david@email.com')

--------Pet_Types---------
INSERT INTO Pet_Types (Type_Name) VALUES
('Dog'),
('Cat'),
('Rabbit')  

--------Pet_Breeds---------
INSERT INTO Pet_Breeds (Breed_Name, Type_ID) VALUES
('Golden Retriever', 1),
('German Shepherd', 1), 
('Labrador', 1),
('Siamese', 2),
('Persian', 2),
('Bengal', 2),
('Holland Lop', 3),
('Flemish Giant', 3)

--------Locations---------
INSERT INTO Locations (Location_Name) VALUES
('Pretoria'),
('Johannesburg'), 
('Durban'),
('Cape Town'),
('Bloemfontein')

--------Pets---------
INSERT INTO Pets (Pet_Name, Pet_Age, Pet_Weight, Pet_Gender, Pet_PetStory, Pet_FullStory, Pet_Status, Pet_ImagePath, Pet_PostedBy, Type_ID, Breed_ID, Location_ID) VALUES

('Bella', 2, 10.5, 'Female', 'Friendly golden retriever.', 'Bella is a very friendly and energetic golden retriever who loves playing fetch and going for long walks. She is great with children and other pets.', 'Available', '/Images/Bella.png', 1, 1, 1, 1),
('Rocky', 4, 15.2, 'Male', 'Playful German Shepherd.', 'Rocky is a loyal and protective German Shepherd. He has been trained in basic obedience and would make a great guard dog for any family.', 'Adopted', '/Images/Rocky.png', 2, 1, 2, 2),
('Max', 3, 12.8, 'Male', 'Energetic Labrador.', 'Max is full of energy and loves swimming. He is very intelligent and learns commands quickly. Perfect for an active family.', 'Available', '/Images/Max.png', 3, 1, 3, 3),
('Luna', 1, 3.4, 'Female', 'Curious Siamese cat.', 'Luna is a curious and affectionate Siamese cat who loves attention. She enjoys climbing and exploring her surroundings. Perfect for a quiet home.', 'Adopted', '/Images/Luna.png', 4, 2, 4, 1),
('Simba', 2, 4.1, 'Male', 'Majestic Persian cat.', 'Simba has a beautiful long coat and a calm personality. He enjoys lounging around and being pampered. Low maintenance and very loving.', 'Available', '/Images/Simba.png', 5, 2, 5, 4),
('Misty', 1, 3.2, 'Female', 'Playful Bengal cat.', 'Misty is very active and loves playing with toys. She has stunning spotted fur and is very talkative. Will keep you entertained for hours.', 'Available', '/Images/Misty.png', 6, 2, 6, 2),
('Coco', 1, 1.8, 'Female', 'Sweet Holland Lop rabbit.', 'Coco is a gentle rabbit with floppy ears. She is litter trained and enjoys fresh vegetables. Great for first-time rabbit owners.', 'Available', '/Images/Coco.png', 7, 3, 7, 5),
('Thumper', 2, 2.3, 'Male', 'Friendly Flemish Giant.', 'Thumper is larger than average but very gentle. He enjoys hopping around in the garden and being petted. Good with older children.', 'Adopted', '/Images/Thumper.png', 1, 3, 8, 3),
('Charlie', 5, 8.7, 'Male', 'Senior dog looking for love.', 'Charlie is a sweet older dog who prefers calm environments. He is house trained and just wants a comfortable home for his golden years.', 'Available', '/Images/Charlie.png', 2, 1, 1, 1),
('Daisy', 2, 4.5, 'Female', 'Affectionate mixed breed.', 'Daisy is a mixed breed with a wonderful personality. She gets along with everyone and would fit into any family situation.', 'Available', '/Images/Daisy.png', 3, 1, 2, 2)

--------Adoptions---------
INSERT INTO Adoptions (Adoption_Date, Pet_ID, User_ID) VALUES
(GETDATE(), 2, 1),   
(GETDATE(), 4, 5),     
('2024-09-15', 8, 6) 
--------Donations---------
INSERT INTO Donations (Donation_Amount, Donation_Date, User_ID) VALUES
(500.00, GETDATE(), 1),
(250.00, '2024-09-20', 2),
(1000.00, '2024-10-01', 4),
(750.00, GETDATE(), 7)

SELECT * FROM Users
SELECT * FROM Pet_Types
SELECT * FROM Pet_Breeds
SELECT * FROM Locations
SELECT * FROM Pets
SELECT * FROM Adoptions
SELECT * FROM Donations