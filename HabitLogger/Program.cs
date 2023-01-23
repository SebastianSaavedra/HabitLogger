using System;
using System.Diagnostics;
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

            Console.Clear();
            if(habit != "")
            {
                Console.WriteLine($"CURRENT HABIT: {habit}");
            }

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
                }
                Console.WriteLine("-------------------------------------------------------");

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
                    case "1":
                        CreateHabit();
                        break;
                    case "2":
                        InsertRecord();
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
            int quantity = GetNumberInput();

            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @$"INSERT INTO {habit}(date,quantity) VALUES('{date}',{quantity})";
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }

        }

        private static string GetHabitInput()
        {
            Console.WriteLine("\n\nPlease write the habit that you want to track. Type 0 to return to main menu.\n");
            string input = Console.ReadLine();

            if (input == "0") GetUserInput();

            return input;
        }

        private static int GetNumberInput()
        {
            Console.WriteLine("\n\nPlease insert the measure type of your choice (Only numbers, no decimals allowed). Type 0 to return to main menu.\n");
            string numberInput = "";
            int finalInput = -1;
            do
            {
                try
                {
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

            string dateInput = Console.ReadLine();

            if (dateInput == "0") GetUserInput();

            return dateInput;
        }

        private static void GetUserInput()
        {
            do
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine("Welcome to your Habit Tracker App");
                Console.WriteLine("");
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Track Habit.");
                Console.WriteLine("2. View record.");
                Console.WriteLine("3. View all records.");
                Console.WriteLine("0. Quit");
                Console.WriteLine("-------------------------------------------------------");

                input = Console.ReadLine();
                Console.WriteLine("");
            } while (input == "");

            switch (input)
            {
                default:
                    break;

                case "1":
                    TrackHabit();
                    break;

                case "2":
                    break;

                case "3":

                    break;
            }
        }

        //private static void OnGet()
        //{
        //    using (var connection = new SqliteConnection(CONNECTION_STRING))
        //    {
        //        connection.Open();
        //        var tableCmd = connection.CreateCommand();

        //        tableCmd.CommandText = @$"SELECT * FROM {tableName}";

        //        tableCmd.ExecuteNonQuery();

        //        connection.Close();
        //    }
        //}
    }
}