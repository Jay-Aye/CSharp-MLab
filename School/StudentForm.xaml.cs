using System;
using System.Windows;

namespace School
{
    /// <summary>
    /// Interaction logic for StudentForm.xaml
    /// </summary>
    public partial class StudentForm : Window
    {
        #region Predefined code

        public StudentForm()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.firstName.Text))
            {
                MessageBox.Show("The student must have a first name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (String.IsNullOrEmpty(this.lastName.Text))
            {
                MessageBox.Show("The student must have a last name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime result;
            if (!DateTime.TryParse(this.dateOfBirth.Text, out result))
            {
                MessageBox.Show("The date of birth must be a valid date", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            TimeSpan age = DateTime.Now.Subtract(result);
            if (age.Days / 362.25 < 5)
            {
                MessageBox.Show("The student must be at least 5 years old", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.DialogResult = true;
        }
    }
}
