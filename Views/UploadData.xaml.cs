using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WpfApp_random_testing.Views
{
    public partial class UploadData : Page
    {
        public ObservableCollection<FileItem> SelectedFiles { get; set; }
        //private readonly string uploadDirectory;
        // You can remove this if not needed
        public ObservableCollection<FileItem> UploadedFiles { get; set; }

        public UploadData()
        {
            InitializeComponent();
            SelectedFiles = new ObservableCollection<FileItem>();
            DataContext = this;
        }

        // Browse Files Method
        private void BrowseFilesClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true, // Allow multiple file selection
                Filter = "All Files|*.*" // Set filter to all files; modify as needed
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    AddFileToList(fileName);
                }
                UploadFiles(); // Call to upload files directly to the database
            }
        }

        // Handle the drop event
        private void UploadData_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string fileName in files)
                {
                    AddFileToList(fileName);
                }
                UploadFiles(); // Call to upload files directly to the database
            }
        }

        // Helper method to add file to the SelectedFiles collection
        private void AddFileToList(string fileName)
        {
            var fileItem = new FileItem
            {
                FileName = Path.GetFileName(fileName),
                FilePath = fileName,
                UploadDate = DateTime.Now // Set the current date and time as the upload date
            };

            if (!SelectedFiles.Contains(fileItem))
            {
                SelectedFiles.Add(fileItem);
            }
        }

        // Upload Files Method - Save files directly to the database
        private void UploadFiles()
        {
            if (SelectedFiles.Count == 0)
            {
                MessageBox.Show("Please select files to upload.");
                return;
            }

            foreach (var file in SelectedFiles)
            {
                try
                {
                    // Read file data into a byte array
                    byte[] fileData = File.ReadAllBytes(file.FilePath);

                    // Save the file data to the database
                    SaveFileToDatabase(file.FileName, file.UploadDate, fileData);

                    MessageBox.Show($"File '{file.FileName}' uploaded successfully on {file.UploadDate:MM/dd/yyyy} to the database.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to upload file '{file.FileName}': {ex.Message}");
                }
            }

            // Clear the list after upload
            SelectedFiles.Clear();
        }

        public void SaveFileToDatabase(string fileName, DateTime uploadDate, byte[] fileData)
        {
            using (var connection = new SQLiteConnection("Data Source=DefectDetectionDB.sqlite;Version=3;"))
            {
                connection.Open();
                var command = new SQLiteCommand("INSERT INTO UploadedFiles (FileName, UploadDate, FileData) VALUES (@FileName, @UploadDate, @FileData)", connection);
                command.Parameters.AddWithValue("@FileName", fileName);
                command.Parameters.AddWithValue("@UploadDate", uploadDate);
                command.Parameters.AddWithValue("@FileData", fileData); // Store the file data as a BLOB
                command.ExecuteNonQuery();
            }
        }
    }

    // FileItem class to represent each file with its name, path, and upload date
    public class FileItem
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; } // Date of upload

        // Override Equals and GetHashCode to ensure uniqueness in the collection
        public override bool Equals(object obj)
        {
            return obj is FileItem item && FilePath == item.FilePath;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FilePath); // Use FilePath for the hash code
        }
    }
}
