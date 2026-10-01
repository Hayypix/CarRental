using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarRental
{
    // класс для работы с базой данных SQLite
    static class Database
    {
        // файл базы данных лежит рядом с программой
        private static string GetDbPath()
        {
            return Path.Combine(Application.StartupPath, "carrent.db");
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("Data Source=" + GetDbPath() + ";Version=3;");
        }

        // при первом запуске создаёт базу и заполняет тестовыми данными
        public static void Init()
        {
            bool newDb = !File.Exists(GetDbPath());

            using (SQLiteConnection con = GetConnection())
            {
                con.Open();
                SQLiteCommand cmd = con.CreateCommand();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Users (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Login TEXT NOT NULL,
                                    Password TEXT NOT NULL)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Cars (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Brand TEXT NOT NULL,
                                    Model TEXT NOT NULL,
                                    Year INTEGER,
                                    Plate TEXT,
                                    PricePerDay REAL NOT NULL,
                                    Status TEXT NOT NULL DEFAULT 'Свободен')";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Clients (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    FullName TEXT NOT NULL,
                                    Phone TEXT,
                                    Passport TEXT,
                                    Address TEXT)";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Rentals (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    CarId INTEGER NOT NULL,
                                    ClientId INTEGER NOT NULL,
                                    StartDate TEXT NOT NULL,
                                    EndDate TEXT,
                                    ReturnDate TEXT,
                                    TotalCost REAL,
                                    Status TEXT NOT NULL DEFAULT 'Открыт')";
                cmd.ExecuteNonQuery();

                if (newDb)
                {
                    // тестовые данные при первом запуске
                    cmd.CommandText = "INSERT INTO Users (Login, Password) VALUES ('admin', 'admin')";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"INSERT INTO Cars (Brand, Model, Year, Plate, PricePerDay, Status) VALUES
                        ('Lada', 'Vesta', 2020, 'А321ТК77', 2500, 'Свободен'),
                        ('Kia', 'Rio', 2019, 'В712НО77', 2800, 'Свободен'),
                        ('Hyundai', 'Solaris', 2021, 'Е405РС77', 3000, 'Свободен'),
                        ('Toyota', 'Camry', 2018, 'К918ММ77', 5500, 'В аренде'),
                        ('Renault', 'Logan', 2017, 'О234КХ77', 2200, 'Свободен'),
                        ('Skoda', 'Octavia', 2019, 'Т600АА77', 3800, 'Свободен')";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"INSERT INTO Clients (FullName, Phone, Passport, Address) VALUES
                        ('Иванов Сергей Петрович', '+7 (912) 345-67-89', '4509 123456', 'г. Москва, ул. Ленина, 15'),
                        ('Петрова Анна Викторовна', '+7 (922) 654-32-10', '4508 654321', 'г. Москва, ул. Мира, 24'),
                        ('Сидоров Дмитрий Александрович', '+7 (902) 111-22-33', '4507 987654', 'г. Москва, пр. Победы, 5'),
                        ('Козлова Мария Андреевна', '+7 (919) 777-88-99', '4506 456789', 'г. Москва, ул. Садовая, 3')";
                    cmd.ExecuteNonQuery();

                    // два договора для примера: один закрытый, один действующий
                    cmd.CommandText = @"INSERT INTO Rentals (CarId, ClientId, StartDate, EndDate, ReturnDate, TotalCost, Status) VALUES
                        (2, 1, '2026-09-01', '2026-09-06', '2026-09-06', 14000, 'Закрыт'),
                        (4, 3, '2026-09-28', '2026-10-02', NULL, 22000, 'Открыт')";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
