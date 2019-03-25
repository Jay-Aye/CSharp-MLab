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
using GradesPrototype.Views;

namespace GradesPrototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataSource.CreateData();
            GotoLogon();
        }

        #region Navigation

        public void GotoLogon()
        {
            logonPage.Visibility = Visibility.Visible;
            studentsPage.Visibility = Visibility.Collapsed;
            studentProfile.Visibility = Visibility.Collapsed;
        }

        private void GotoStudentsPage()
        {   
            studentProfile.Visibility = Visibility.Collapsed;

            studentsPage.Visibility = Visibility.Visible;
            studentsPage.Refresh();
        }
        
        public void GotoStudentProfile()
        {
            studentsPage.Visibility = Visibility.Collapsed;
            
            studentProfile.Visibility = Visibility.Visible;
            studentProfile.Refresh();
        }
        #endregion

        #region Event Handlers

        private void Logon_Success(object sender, EventArgs e)
        {
            logonPage.Visibility = Visibility.Collapsed;
            gridLoggedIn.Visibility = Visibility.Visible;
            Refresh();
        }

        private void Logon_Failed(object sender, EventArgs e)
        {
            MessageBox.Show("Invalid Username or Password", "Logon Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Logoff_Click(object sender, RoutedEventArgs e)
        {
            gridLoggedIn.Visibility = Visibility.Collapsed;
            studentsPage.Visibility = Visibility.Collapsed;
            studentProfile.Visibility = Visibility.Collapsed;
            logonPage.Visibility = Visibility.Visible;
        }
        
        private void studentPage_Back(object sender, EventArgs e)
        {
            GotoStudentsPage();
        }
        
        private void studentsPage_StudentSelected(object sender, StudentEventArgs e)
        {
            SessionContext.CurrentStudent = e.Child;
            GotoStudentProfile();
        }
        #endregion

        #region Display Logic
        
        private void Refresh()
        {
            switch (SessionContext.UserRole)
            {
                case Role.Student:
                    txtName.Text = string.Format("Welcome {0} {1}", SessionContext.CurrentStudent.FirstName, SessionContext.CurrentStudent.LastName);
                    
                    GotoStudentProfile();
                    break;

                case Role.Teacher:
                    txtName.Text = string.Format("Welcome {0} {1}", SessionContext.CurrentTeacher.FirstName, SessionContext.CurrentTeacher.LastName);
                    
                    GotoStudentsPage();                    
                    break;
            }
        }
        #endregion
    }
}
