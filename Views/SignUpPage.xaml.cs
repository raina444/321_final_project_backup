using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp_random_testing.Models;

namespace WpfApp_random_testing.Views
{
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        // Event handler to set focus on Username TextBox when clicking on placeholder text
        private void textUsername_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUsername.Focus(); // Set focus to txtUsername when clicking on the textUsername placeholder
        }

        // Event handler for Username TextBox placeholder visibility
        private void textUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Hide the placeholder text when there is content in the text box
            if (!string.IsNullOrEmpty(txtUsername.Text) && txtUsername.Text.Length > 0)
            {
                textUsername.Visibility = Visibility.Collapsed;
            }
            else
            {
                textUsername.Visibility = Visibility.Visible;
            }
        }

        // Event handler to set focus on PasswordBox when clicking on placeholder text
        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PasswordBox.Focus(); // Set focus to PasswordBox when clicking on the textPassword placeholder
        }

        // Event handler for PasswordBox placeholder visibility
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Hide the placeholder text when there is content in the password box
            if (!string.IsNullOrEmpty(PasswordBox.Password) && PasswordBox.Password.Length > 0)
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }
        }

        // Event handler to set focus on Confirm PasswordBox when clicking on placeholder text
        private void textConfirmPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ConfirmPasswordBox.Focus(); // Set focus to ConfirmPasswordBox when clicking on the textConfirmPassword placeholder
        }

        // Event handler for Confirm PasswordBox placeholder visibility
        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Hide the placeholder text when there is content in the confirm password box
            if (!string.IsNullOrEmpty(ConfirmPasswordBox.Password) && ConfirmPasswordBox.Password.Length > 0)
            {
                textConfirmPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textConfirmPassword.Visibility = Visibility.Visible;
            }
        }

        // Event handler for First Name placeholder visibility
        private void textFirstName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtFirstName.Focus(); // Set focus to txtFirstName when clicking on the textFirstName placeholder
        }

        private void textFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFirstName.Text) && textFirstName.Text.Length > 0)
            {
                textFirstName.Visibility = Visibility.Collapsed;
            }
            else
            {
                textFirstName.Visibility = Visibility.Visible;
            }
        }

        // Event handler for Last Name placeholder visibility
        private void textLastName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtLastName.Focus(); // Set focus to txtLastName when clicking on the textLastName placeholder
        }

        private void textLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtLastName.Text) && textLastName.Text.Length > 0)
            {
                textLastName.Visibility = Visibility.Collapsed;
            }
            else
            {
                textLastName.Visibility = Visibility.Visible;
            }
        }

        // Show Password functionality
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Show password functionality can be implemented with custom controls, as WPF's PasswordBox does not support direct visibility change.");
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Password will remain hidden.");
        }

        // Event handler for Create Account button click
        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string username = txtUsername.Text;
            string password = PasswordBox.Password;

            if (!string.IsNullOrEmpty(firstName) &&
                !string.IsNullOrEmpty(lastName) &&
                !string.IsNullOrEmpty(username) &&
                !string.IsNullOrEmpty(password))
            {
                // Attempt to register the user
                bool isRegistered = DatabaseInitializer.RegisterUser(username, firstName, lastName, password);

                if (isRegistered)
                {
                    MessageBox.Show("Account created successfully!");
                    NavigationService?.Navigate(new LoginPage());
                }
                else
                {
                    MessageBox.Show("Username already exists. Please choose a different username.");
                }
            }
            else
            {
                MessageBox.Show("Please fill out all fields.");
            }
        }
    }
}
