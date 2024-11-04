using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp_random_testing.Models
{


    public class PasswordBoxBindingBehavior : Behavior<PasswordBox>
    {

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordBoxBindingBehavior), new PropertyMetadata(string.Empty));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += PasswordBox_PasswordChanged;
            AssociatedObject.Loaded += (s, e) => AssociatedObject.Password = Password; // Set initial password
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= PasswordBox_PasswordChanged;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = AssociatedObject.Password;
        }
    }
}
