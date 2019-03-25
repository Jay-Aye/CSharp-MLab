using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using GradesPrototype.Data;
using GradesPrototype.Services;

namespace GradesPrototype.Views
{
    /// <summary>
    /// Interaction logic for StudentsPage.xaml
    /// </summary>
    public partial class StudentsPage : UserControl
    {
        public StudentsPage()
        {
            InitializeComponent();
        }

        #region Display Logic
        public void Refresh()
        {
            ArrayList students = new ArrayList();
            foreach (Student student in DataSource.Students)
            {
                if (student.TeacherID == SessionContext.CurrentTeacher.TeacherID)
                {
                    students.Add(student);
                }
            }

            list.ItemsSource = students;

            txtClass.Text = String.Format("Class {0}", SessionContext.CurrentTeacher.Class);
        }
        #endregion

        #region Event Members
        public delegate void StudentSelectionHandler(object sender, StudentEventArgs e);
        public event StudentSelectionHandler StudentSelected;
        #endregion

        #region Event Handlers
        private void Student_Click(object sender, RoutedEventArgs e)
        {
            Button itemClicked = sender as Button;
            if (itemClicked != null)
            {
                int studentID = (int)itemClicked.Tag;
                if (StudentSelected != null)
                {
                    Student student = (Student)itemClicked.DataContext;
                    
                    StudentSelected(sender, new StudentEventArgs(student));
                }
            }
        }
        #endregion
    }

    public class StudentEventArgs : EventArgs
    {
        public Student Child { get; set; }

        public StudentEventArgs(Student s)
        {
            Child = s;
        }
    }
}
