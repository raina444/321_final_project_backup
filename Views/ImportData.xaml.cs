using System;
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
using System.Data.SQLite;

namespace WpfApp_random_testing.Views
{
    /// <summary>
    /// Interaction logic for ImportData.xaml
    /// </summary>
    public partial class ImportData : Page
    {
        public string FileName;
        public string FilePath;
        public DateTime UploadDate;
        public ImportData()
        {
            InitializeComponent();
            FileName = (string)Application.Current.Properties["FileName"];
            FilePath = (string)Application.Current.Properties["FilePath"];
            UploadDate = (DateTime)Application.Current.Properties["UploadDate"];
        }

        public void SaveFileToDatabase(string fileName, string filePath)
        {
            using (var connection = new SQLiteConnection("Data Source=yourDatabase.db"))
            {
                connection.Open();
                var command = new SQLiteCommand("INSERT INTO UploadedFiles (FileName, UploadDate, FilePath) VALUES (@FileName, @UploadDate, @FilePath)", connection);
                command.Parameters.AddWithValue("@FileName", fileName);
                command.Parameters.AddWithValue("@UploadDate", DateTime.Now.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@FilePath", filePath);
                command.ExecuteNonQuery();
            }
        }
    }
}
