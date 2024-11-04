using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp_random_testing.Models;
using WpfApp_random_testing.ViewModels;

namespace WpfApp_random_testing.Views
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            DataContext = new LoginPageViewModel();
        }

        private void textUsername_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUsername.Focus(); // Set focus to txtUsername when clicking on the textUsername placeholder
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Hide the placeholder text when there is content in the text box
            textUsername.Visibility = string.IsNullOrEmpty(txtUsername.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword.Focus(); // Set focus to txtPassword when clicking on the textPassword placeholder
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Hide the placeholder text when there is content in the password box
            textPassword.Visibility = string.IsNullOrEmpty(txtPassword.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            
            
            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Login successful!");

                // Retrieve the full name from the database
                string fullName = DatabaseInitializer.GetFullName(username);

                // Store the full name in Application properties
                Application.Current.Properties["FullName"] = fullName;
                Application.Current.Properties["Username"] = username;

                // Navigate to the Dashboard
                if (MainWindow.MainContentFrame != null)
                {
                    MainWindow.MainContentFrame.Navigate(new Dashboard(fullName));
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.");
            }
        }


        internal bool AuthenticateUser(string username, string password)
        {
            try
            {
                string connectionString = "Data Source=DefectDetectionDB.sqlite;Version=3;";
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT PasswordHash FROM Users WHERE Username = @Username";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        var result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string storedHash = result.ToString();
                            string hashedPassword = HashPassword(password);
                            return storedHash == hashedPassword;
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}");
            }
            return false;
        }

        private string HashPassword(string password)
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

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Window.GetWindow(this)?.DragMove(); // Enable dragging the window by holding down the mouse button on the border
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.MainFrame.Content = new SignUpPage(); // Navigate to the SignUp page
            }
        }
    }
}
