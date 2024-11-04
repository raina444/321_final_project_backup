using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
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

namespace WpfApp_random_testing.Views
{
    /// <summary>
    /// Interaction logic for MainDashboard.xaml
    /// </summary>
    public partial class MainDashboard : Page
    {
        public ObservableCollection<FileItem> UploadedFiles { get; set; }

        public MainDashboard()
        {
            InitializeComponent();
            UploadedFiles = new ObservableCollection<FileItem>();
            DataContext = this;
            LoadUploadedFiles();
            
        }

        private void LoadUploadedFiles()
        {
            using (var connection = new SQLiteConnection("Data Source=DefectDetectionDB.sqlite;Version=3;"))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT FileName, UploadDate FROM UploadedFiles", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var fileItem = new FileItem
                        {
                            FileName = reader.GetString(0),
                            UploadDate = reader.GetDateTime(1),
                        };
                        UploadedFiles.Add(fileItem);

                        // Debug: Check each loaded item
                        Console.WriteLine($"Loaded file: {fileItem.FileName}, Date: {fileItem.UploadDate}");
                    }
                }
            }
        }


        // New event handler for double-clicking on a ListBox item
        private void UploadedFilesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UploadedFilesListBox.SelectedItem is FileItem selectedFile)
            {
                OpenFileFromDatabase(selectedFile.FileName);
            }
        }

        public void OpenFileFromDatabase(string fileName)
        {
            using (var connection = new SQLiteConnection("Data Source=DefectDetectionDB.sqlite;Version=3;"))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT FileData FROM UploadedFiles WHERE FileName = @FileName", connection);
                command.Parameters.AddWithValue("@FileName", fileName);

                var fileData = command.ExecuteScalar() as byte[];

                if (fileData != null)
                {
                    string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName);
                    File.WriteAllBytes(tempFilePath, fileData);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempFilePath) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("File not found in the database.");
                }
            }
        }
    }

}

