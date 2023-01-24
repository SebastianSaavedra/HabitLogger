using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;

namespace HabitTracker
{
    class Program
    {
        const string CONNECTION_STRING = @"Data Source=HabitTracker.db";

        //private static bool closeApp = false;
        private static string input = "";
        private static string habit = "";

        static void Main(string[] args)
        {
            CheckIfDatabaseExists();
            GetUserInput();
        }

        private static void CheckIfDatabaseExists()
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();

                Console.WriteLine($"Database Path: {connection.DataSource}");

                connection.Close();
            }
        }
        private static void TrackHabit()
        {
            string input = "";

            Console.WriteLine("");
            Console.WriteLine("        ╔═════════════════════════════════════════╗");
            Console.WriteLine($"         CURRENT HABIT SELECTED: {habit} ");
            Console.WriteLine("        ╚═════════════════════════════════════════╝");
            Console.WriteLine("");

            do
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Please select an option.");
                Console.WriteLine("");
                Console.WriteLine("1. Select Habit");
                if (habit != "")
                {
                    Console.WriteLine("2. Insert record");
                    Console.WriteLine("3. Update record");
                    Console.WriteLine("4. Delete record");
                    Console.WriteLine("0. Main Menu");
                }
                Console.WriteLine("-------------------------------------------------------");

                Console.Write("Input: ");
                input = Console.ReadLine();
                Console.WriteLine("");

                switch (input)
                {
                    default:
                        Console.WriteLine("----------- INVALID INPUT -----------");
                        input = "";
                        Thread.Sleep(1000);
                        Console.Clear();
                        break;
                    case "0":
                        GetUserInput();
                        break;
                    case "1":
                        CreateHabit();
                        break;
                    case "2":
                        InsertRecord();
                        break;
                    case "3":
                        Update();
                        break;
                    case "4":
                        Delete();
                        break;
                }
            } while (input == "");
        }

        private static void CreateHabit()
        {
            habit = GetHabitInput();

            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {habit}(
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Date TEXT,
                                            Quantity INTEGER
                                            )";
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
            TrackHabit();
        }
        private static void InsertRecord()
        {
            string date = GetDateInput();
            int quantity = GetNumberInput("\n\nPlease write how many hours you spend on your habit. (Only numbers, no decimals allowed). Type 0 to return to main menu.\n");

            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @$"INSERT INTO {habit}(date,quantity) VALUES('{date}',{quantity})";
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
            OnGet(TrackHabit);
        }

        private static string GetHabitInput()
        {
            Console.WriteLine("\n\nPlease write the habit that you want to track. Type 0 to return to main menu.\n");
            Console.Write("Input: ");
            string input = Console.ReadLine();

            if (input == "0") GetUserInput();

            return input;
        }

        private static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
            string numberInput = "";
            int finalInput = -1;
            do
            {
                try
                {
                    Console.Write("Input: ");
                    numberInput = Console.ReadLine();
                    finalInput = Convert.ToInt32(numberInput);
                }
                catch
                {
                    Console.WriteLine("Please write a number.");
                    finalInput = -1;
                }
            } while (finalInput > 5 && finalInput < 0);

            if (numberInput == "0") GetUserInput();

            return finalInput;
        }

        private static string GetDateInput()
        {
            Console.WriteLine("Please insert the date: (Format: dd-mm-yyyy). Type 0 to return to main menu.");

            Console.Write("Input: ");
            string dateInput = Console.ReadLine();

            if (dateInput == "0") GetUserInput();

            while (!DateTime.TryParseExact(dateInput,"dd-MM-yyyy",new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yyyy). Type 0 to return to the main menu or try again:\n");
                Console.Write("Input: ");
                dateInput = Console.ReadLine();
            }

            return dateInput;
        }

        private static void GetUserInput()
        {
            do
            {
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine("        ╔═════════════════════════════════════════╗");
                Console.WriteLine("             Welcome to your Habit Tracker App");
                Console.WriteLine("        ╚═════════════════════════════════════════╝");
                Console.WriteLine("");
                Console.WriteLine(" Please select an option:");
                Console.WriteLine(" 1. Track Habit.");
                Console.WriteLine(" 2. View records.");
                Console.WriteLine(" 0. Quit");
                Console.WriteLine("--------------------------------------------------------");

                Console.Write("Input: ");
                input = Console.ReadLine();
                Console.WriteLine("");
            } while (input == "");

            switch (input)
            {
                default:
                    Console.WriteLine("                     ╔═════════════════╗");
                    Console.WriteLine("                       INVALID INPUT");
                    Console.WriteLine("                     ╚═════════════════╝");
                    input = "";
                    Thread.Sleep(1000);
                    Console.Clear();
                    GetUserInput();
                    break;

                case "0":
                    Console.WriteLine("                     ╔═════════════════╗");
                    Console.WriteLine("                           Goodbye!");
                    Console.WriteLine("                     ╚═════════════════╝");
                    Environment.Exit(0);
                    break;

                case "1":
                    TrackHabit();
                    Console.Clear();
                    break;

                case "2":
                    habit = GetHabitInput();
                    Console.Clear();
                    OnGet(GetUserInput);
                    break;
            }
        }

        private static void OnGet(Action action = null!)
        {
            CheckIfTableExists();
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @$"SELECT * FROM {habit}";

                List<Habit> tableData = new List<Habit>();
                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    { 
                        tableData.Add(new Habit
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yyyy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)
                        });
                    }

                    Console.WriteLine("");
                    Console.WriteLine($"LISTED RECORDS FOR '{habit}' HABIT:");
                    Console.WriteLine("══════════════════════════════════════════════════════");
                    foreach (var row in tableData)
                    {
                        Console.WriteLine($"{row.Id} - {row.Date.ToString("dd-MMM-yyyy")} - Quantity: {row.Quantity}");
                    }
                    Console.WriteLine("══════════════════════════════════════════════════════");
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("   ══════════════════════════════════════");
                    Console.WriteLine($"    No records found for '{habit}' habit.");
                    Console.WriteLine("   ══════════════════════════════════════");
                }

                connection.Close();
            }
            action?.Invoke();
        }

        private static void CheckIfTableExists()
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @$"SELECT count(*) FROM sqlite_schema WHERE type='table' AND name='{habit}'";
                int checkQuery = Convert.ToInt32(tableCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nHabit '{habit}' doesn't exist. \n");
                    connection.Close();
                    habit = "";
                    Thread.Sleep(2000);
                    Console.Clear();
                    GetUserInput();
                }
                connection.Close();
            }
        }

        public static void Delete()
        {
            OnGet();

            var recordId = GetNumberInput("\n\nPlease type the Id of the record you want to delete. Type 0 to return to main menu.\n");

            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var tablecmd = connection.CreateCommand();

                tablecmd.CommandText = $"DELETE FROM {habit} WHERE Id = '{recordId}'";

                int rowCount = tablecmd.ExecuteNonQuery();

                if (rowCount == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. \n");
                    connection.Close();
                    Thread.Sleep(2000);
                    Console.Clear();
                    Delete();
                }

                Console.WriteLine($"\n\nRecord with Id {recordId} was deleted. \n");
                connection.Close();
            }
            OnGet(TrackHabit);
        }

        public static void Update()
        {
            OnGet(null);

            var recordId = GetNumberInput("\n\nPlease type the Id of the record you want to update. Type 0 to return to main menu.\n");

            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var tablecmd = connection.CreateCommand();

                tablecmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM {habit} WHERE Id = {recordId})";
                int checkQuery = Convert.ToInt32(tablecmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. \n");
                    connection.Close();
                    Thread.Sleep(2000);
                    Console.Clear();
                    Update();
                }

                var newDate = GetDateInput();
                var newQuantity = GetNumberInput("\n\nPlease write how many hours you spend on your habit. (Only numbers, no decimals allowed). Type 0 to return to main menu.\n");

                tablecmd.CommandText = $"UPDATE {habit} SET date = '{newDate}' , quantity = {newQuantity} WHERE Id = '{recordId}'";
                tablecmd.ExecuteNonQuery();

                Console.WriteLine($"\n\n¡Record with Id {recordId} was succesfully updated!. \n");
                connection.Close();
            }
            OnGet(TrackHabit);
        }
    }
}

    public class Habit
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }