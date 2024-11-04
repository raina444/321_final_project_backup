using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WpfApp_random_testing.Views.Controls;

namespace WpfApp_random_testing.Views
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public string FullName { get; set; }

        public Dashboard(string fullName)
        {
            InitializeComponent();

            // Set the FullName property using the parameter
            FullName = fullName;

            // Set the userNameTextBlock with the full name from the property or Application.Current.Properties
            userNameTextBlock.Text = $"User: {FullName}";
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = sidebar.SelectedItem as NavSidebar;

            if (selected != null)
            {
                navFrame.Navigate(selected.Navlink);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear any session or stored data related to the user
            Application.Current.Properties["LoggedInUsername"] = null;

            // Optional: Display a logout message
            MessageBox.Show("You have been logged out successfully.", "Logout", MessageBoxButton.OK, MessageBoxImage.Information);

            // Navigate to the login page (assuming your login page is called LoginPage.xaml)
            NavigationService navService = NavigationService.GetNavigationService(this);
            if (navService != null)
            {
                navService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.Relative));
            }
        }

    }
}
