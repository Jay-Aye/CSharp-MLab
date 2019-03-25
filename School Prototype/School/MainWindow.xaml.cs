using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using School.Data;
using System.Globalization;
using System.Data;
using System.Data.Objects;

namespace School
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Connection to the School database
        private SchoolDBEntities schoolContext = null;

        // Field for tracking the currently selected teacher
        private Teacher teacher = null;

        // List for tracking the students assigned to the teacher's class
        private IList studentsInfo = null;

        #region Predefined code

        public MainWindow()
        {
            InitializeComponent();
        }

        // Connect to the database and display the list of teachers when the window appears
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.schoolContext = new SchoolDBEntities();
            teachersList.DataContext = this.schoolContext.Teachers;
        }

        // When the user selects a different teacher, fetch and display the students for that teacher
        private void teachersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Find the teacher that has been selected
            this.teacher = teachersList.SelectedItem as Teacher;
            this.schoolContext.LoadProperty<Teacher>(this.teacher, s => s.Students);

            // Find the students for this teacher
            this.studentsInfo = ((IListSource)teacher.Students).GetList();

            // Use databinding to display these students
            studentsList.DataContext = this.studentsInfo;
        }

        #endregion

        // When the user presses a key, determine whether to add a new student to a class, remove a student from a class, or modify the details of a student
        private void studentsList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter: Student student = this.studentsList.SelectedItem as Student;
                    EditStudent(student);
                    break;

                case Key.Insert:
                    AddNewStudent();
                    break;

                case Key.Delete: student = this.studentsList.SelectedItem as Student;
                    RemoveStudent(student);
                    break;
            }
        }

        private void studentsList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Student student = this.studentsList.SelectedItem as Student;
            EditStudent(student);
        }

        private void AddNewStudent()
        {
            StudentForm sf = new StudentForm();
            
            sf.Title = "New Student for Class " + teacher.Class;

            if (sf.ShowDialog().Value)
            {
                Student newStudent = new Student();
                newStudent.FirstName = sf.firstName.Text;
                newStudent.LastName = sf.lastName.Text;
                newStudent.DateOfBirth = DateTime.ParseExact(sf.dateOfBirth.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                this.teacher.Students.Add(newStudent);
                this.studentsInfo.Add(newStudent);
                saveChanges.IsEnabled = true;
            }
        }

        private void RemoveStudent(Student student)
        {
            MessageBoxResult response = MessageBox.Show(
                String.Format("Remove {0}?", student.FirstName + " " + student.LastName),
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question,
                MessageBoxResult.No);

            if (response == MessageBoxResult.Yes)
            {
                this.schoolContext.Students.DeleteObject(student);
                saveChanges.IsEnabled = true;
            }
        }

        private void EditStudent(Student student)
        {
            StudentForm sf = new StudentForm();

            sf.Title = "Edit Student Details";
            sf.firstName.Text = student.FirstName;
            sf.lastName.Text = student.LastName;
            sf.dateOfBirth.Text = student.DateOfBirth.ToString("d");

            if (sf.ShowDialog().Value)
            {
                student.FirstName = sf.firstName.Text;
                student.LastName = sf.lastName.Text;
                student.DateOfBirth = DateTime.ParseExact(sf.dateOfBirth.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                saveChanges.IsEnabled = true;
            }
        }

        private void saveChanges_onClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.schoolContext.SaveChanges();
                saveChanges.IsEnabled = false;
            }
            catch (OptimisticConcurrencyException)
            {
                // If another user has changed the same students earlier, then overwrite their changes with the new data
                this.schoolContext.Refresh(RefreshMode.ClientWins, schoolContext.Students);
                this.schoolContext.SaveChanges();
            }
            catch (UpdateException uEx)
            {
                // If some sort of database exception has occurred, then display the reason for the exception and rollback
                MessageBox.Show(uEx.InnerException.Message, "Error saving changes");
                this.schoolContext.Refresh(RefreshMode.StoreWins, schoolContext.Students);
            }
            catch (Exception ex)
            {
                // If some other exception occurs, report it to the user
                MessageBox.Show(ex.Message, "Error saving changes");
                this.schoolContext.Refresh(RefreshMode.ClientWins, schoolContext.Students);
            }
        }

        #region Predefined code

        private void studentsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
 
        }

        // Save changes back to the database and make them permanent
        private void saveChanges_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }

    [ValueConversion(typeof(string), typeof(Decimal))]
    class AgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                              System.Globalization.CultureInfo culture)
        {

            if (value != null)
            {
                DateTime studentDateOfBirth = (DateTime)value;

                TimeSpan difference = DateTime.Now.Subtract(studentDateOfBirth);

                int ageInYears = (int)(difference.Days / 365.25);

                return ageInYears.ToString();
            }
            else
            {
                return "";
            }
        }

        #region Predefined code

        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
