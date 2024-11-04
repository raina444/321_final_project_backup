using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp_random_testing.Models;
using WpfApp_random_testing.ViewModels;

namespace WpfApp_random_testing.Views
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Page
    {
        public string FullName { get; set; }
        public string Username { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public Profile() // Default constructor
        {
            InitializeComponent();

            //LoggedInUsername = UserName; // Set the logged-in username


            Username = (string)Application.Current.Properties["Username"];

            if (string.IsNullOrEmpty(Username))
                MessageBox.Show("Username empty");

            LoadUserProfile();
            
            ProfileFullName.Text = FullName;
            ProfileUserName.Text = Username;

            //if (string.IsNullOrEmpty(fullName))
            //throw new ArgumentNullException(nameof(fullName));

            //sDataContext = new UserProfileViewModel(UserName);
            //DataContext = new PasswordBoxBindingBehavior();


            // Set the userNameTextBlock with the full name from the property or Application.Current.Properties
            // Ensure userName is assigned before using it
        }


        private void LoadUserProfile()
        {
            // Load user details from the database
            using (var connection = new SQLiteConnection("Data Source=DefectDetectionDB.sqlite;Version=3;"))
            {
                connection.Open();
                string query = "SELECT FirstName, LastName FROM Users WHERE Username = @Username"; // Modify as needed
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", Username); // Replace with the actual username
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Firstname = reader["FirstName"].ToString();
                            Lastname = reader["LastName"].ToString();
                        }
                    }
                }
            }
            // Set FullName after Firstname and Lastname have been populated
            FullName = Firstname + " " + Lastname;
           // OnPropertyChanged(nameof(FullName)); // Notify UI of FullName change
        }

        private void ChangePasswordClick(object sender, RoutedEventArgs e)
        {
            string NewPassword = NewPasswordBox.Password;
            string ConfirmPassword = ConfirmNewPasswordBox.Password;

            if (IsOldPasswordCorrect())
            {
                if (NewPassword == ConfirmPassword)
                {
                    UpdatePasswordInDatabase(NewPassword);
                    MessageBox.Show("Password changed successfully.");
                }
                else
                {
                    MessageBox.Show("New passwords do not match.");
                }
            }
            else
            {
                MessageBox.Show("Old password is incorrect.");
            }
        }

        private bool IsOldPasswordCorrect()
        {
            string OldPassword = OldPasswordBox.Password;

            // Ensure LoggedInUsername is not null or empty
            if (string.IsNullOrEmpty(Username))
            {
                throw new InvalidOperationException("Username must be set before checking old password.");
            }

            using (var connection = new SQLiteConnection("Data Source=DefectDetectionDB.sqlite;Version=3;"))
            {
                connection.Open();

                string query = "SELECT PasswordHash FROM Users WHERE Username = @Username"; // Ensure this matches

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", Username); // Using the correct parameter name

                    // ExecuteScalar may return null if the user doesn't exist
                    var result = command.ExecuteScalar() as string;

                    // Hash the entered old password and compare it with the stored hash
                    return result != null && result == HashPassword(OldPassword);
                }
            }
        }


        private void UpdatePasswordInDatabase(string newPassword)
        {
            // Hash the new password before storing
            string hashedNewPassword = HashPassword(newPassword);

            using (var connection = new SQLiteConnection("Data Source=DefectDetectionDB.sqlite;Version=3;"))
            {
                connection.Open();
                string query = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Username = @Username";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PasswordHash", hashedNewPassword);
                    command.Parameters.AddWithValue("@Username", Username); // Replace with the actual username
                    command.ExecuteNonQuery();
                }
            }
        }

        // Reuse the HashPassword function from your DatabaseInitializer class
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

        //** Change password **//

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }//public partial class
}//namespace