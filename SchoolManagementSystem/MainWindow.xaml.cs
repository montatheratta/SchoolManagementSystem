using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using Dapper;

namespace SchoolManagementSystem
{
    public class DatabaseConnection
    {
        private string connectionString = "Data Source=D:\\c#\\SchoolManagementSystem\\schoolmangment.db; Version=3;";
        //create a connection to the SQLite database
        public SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        // Method to test the database connection
        public void TestConnection()
        {
            using (var conn = CreateConnection())
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    MessageBox.Show("Connection successful!");
                }
                else
                {
                    MessageBox.Show("Connection failed!");
                }
            }
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Create  DatabaseConnection class
            DatabaseConnection db = new DatabaseConnection();

            // Test the connection to the  database
            db.TestConnection();
          

        }
        ///////////////////////////////////////////////////////////////////////////////////////
        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            DateTime? dob = DateOfBirthPicker.SelectedDate;
            string gender = ((ComboBoxItem)GenderComboBox.SelectedItem).Content.ToString();

            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName) && dob.HasValue)
            {
                DatabaseConnection db = new DatabaseConnection();

                using (var conn = db.CreateConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Students (FirstName, LastName, DateOfBirth, Gender) VALUES (@FirstName, @LastName, @DateOfBirth, @Gender)";
                    conn.Execute(query, new { FirstName = firstName, LastName = lastName, DateOfBirth = dob, Gender = gender });

                    MessageBox.Show("Student added successfully!");
                }
            }
            else
            {
                MessageBox.Show("Please fill out all fields.");
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        private void AddCurriculumButton_Click(object sender, RoutedEventArgs e)
        {
            string curriculumName = CurriculumNameTextBox.Text;

            if (!string.IsNullOrEmpty(curriculumName))
            {
                DatabaseConnection db = new DatabaseConnection();

                using (var conn = db.CreateConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Curriculum (CurriculumName) VALUES (@CurriculumName)";
                    conn.Execute(query, new { CurriculumName = curriculumName });

                    MessageBox.Show("Curriculum added successfully!");

                    // Optional: Reload curriculums after adding a new one
                    LoadCurriculums();
                }
            }
            else
            {
                MessageBox.Show("Please enter a curriculum name.");
            }
            
        }
        private void LoadCurriculums()
        {
            try
            {
                DatabaseConnection db = new DatabaseConnection();

                using var conn = db.CreateConnection();
                
                conn.Open();
                string query = "SELECT CurriculumID, CurriculumName FROM Curriculum";
                var curriculum = conn.Query<Curriculum>(query).ToList();
                var result = curriculum.Select(x=>x.CurriculumName).ToList();
                CurriculumComboBox.ItemsSource = result;
                CurriculumComboBoxForClasses.ItemsSource = result;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading curriculum: " + ex.Message);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        private void AddClassButton_Click(object sender, RoutedEventArgs e)
        {
       
            string className = ClassNameTextBox.Text;
            var selectedCurriculum = CurriculumComboBox.SelectedValue;

            if (!string.IsNullOrEmpty(className) && selectedCurriculum != null)
            {
                DatabaseConnection db = new DatabaseConnection();

                using (var conn = db.CreateConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Classes (ClassName, CurriculumID) VALUES (@ClassName, @CurriculumID)";
                    conn.Execute(query, new { ClassName = className, CurriculumID = selectedCurriculum });

                    MessageBox.Show("Class added successfully!");
                }
            }
            else
            {
                MessageBox.Show("Please enter a class name and select a curriculum.");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCurriculums();

        }
        ///////////////////////////////////////////////////////////////////////////////////////

    }

}
