using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WpfApp_random_testing.Models
{
    public static class DatabaseInitializer
    {
        public static void CreateDatabase()
        {
            // Create database if it doesn't exist
            string dbPath = "DefectDetectionDB.sqlite";
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);

                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    string sql = @"CREATE TABLE IF NOT EXISTS Users (
                                    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                                    FirstName TEXT NOT NULL,
                                    LastName TEXT NOT NULL,
                                    Username TEXT NOT NULL UNIQUE,
                                    PasswordHash TEXT NOT NULL);";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static bool RegisterUser(string username, string firstName, string lastName, string password)
        {
            try
            {
                string dbPath = "DefectDetectionDB.sqlite";
                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();

                    // Check if the username already exists
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (var checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            return false; // Username already exists
                        }
                    }

                    // Hash the password
                    string hashedPassword = HashPassword(password);

                    // Insert the new user
                    string insertQuery = @"INSERT INTO Users (Username, FirstName, LastName, PasswordHash) 
                                           VALUES (@Username, @FirstName, @LastName, @PasswordHash)";
                    using (var insertCmd = new SQLiteCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@Username", username);
                        insertCmd.Parameters.AddWithValue("@FirstName", firstName);
                        insertCmd.Parameters.AddWithValue("@LastName", lastName);
                        insertCmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                        insertCmd.ExecuteNonQuery();
                    }
                }
                return true; // Registration successful
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error registering user: {ex.Message}");
                return false; // Registration failed
            }
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool VerifyLogin(string username, string password)
        {
            try
            {
                string dbPath = "DefectDetectionDB.sqlite";
                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    string query = "SELECT PasswordHash FROM Users WHERE Username = @Username";
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);

                        var storedHash = cmd.ExecuteScalar() as string;
                        if (storedHash != null)
                        {
                            // Check if the provided password matches the stored hash
                            return storedHash == HashPassword(password);
                        }
                    }
                }
                return false; // Username does not exist or password is incorrect
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying login: {ex.Message}");
                return false;
            }
        }
        public static string GetFullName(string username)
        {
            string dbPath = "DefectDetectionDB.sqlite";
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string query = "SELECT FirstName, LastName FROM Users WHERE Username = @Username";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string firstName = reader["FirstName"].ToString();
                            string lastName = reader["LastName"].ToString();
                            return $"{firstName} {lastName}";
                        }
                    }
                }
            }
            return "User"; // Default if not found
        }

    }
}
