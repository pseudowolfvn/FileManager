using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Entities
{
    public class Panel
    {
        public ComboBox PanelsComboBox { get; set; }
        public ListView PanelsListView { get; set; }
        static int panelID;
        int currentDriveID;
        DriveInfo currentDrive;
        DirectoryInfo currentDirectory;
        public Panel(ComboBox panelsComboBox, ListView panelsListView)
        {
            currentDriveID = (panelID++) % FileSystem.GetNumOfDrives();
            currentDrive = FileSystem.GetAllDrives()[currentDriveID];
            currentDirectory = currentDrive.RootDirectory;
            PanelsComboBox = panelsComboBox;
            PanelsListView = panelsListView;
        }
        public int GetCurrentDriveID()
        {
            return currentDriveID;
        }
        public DirectoryInfo[] GetSubdirectories()
        {
            return currentDirectory.GetDirectories();
        }

        public FileInfo[] GetFiles()
        {
            return currentDirectory.GetFiles();
        }

        public bool IsChanged(DirectoryInfo changedDirectory)
        {
            return (this.PanelsListView.SelectedItem != null 
                || changedDirectory.FullName == FileSystem.ExistingSubdirectory(currentDirectory).FullName);
        }
        
        public void Update()
        {
            PanelsListView.Items.Clear();
            if (Directory.Exists(currentDirectory.FullName) != true)
                currentDirectory = FileSystem.ExistingSubdirectory(currentDirectory);
            if (currentDirectory.FullName != currentDrive.RootDirectory.FullName)
            {
                PanelsListView.Items.Add(new Item(currentDirectory.Parent, @"↑..."));
            }
            foreach (var x in this.GetSubdirectories() )
            {
                PanelsListView.Items.Add(new Item(x));
            }
            foreach (var x in this.GetFiles())
            {
                PanelsListView.Items.Add(new Item(x));
            }
        }

        public void ChangeDrive(DriveInfo root)
        {
            currentDrive = root;
            currentDirectory = currentDrive.RootDirectory;
        }

        public void ChangeDirectory(DirectoryInfo root)
        {
            currentDirectory = root;
        }

        public DirectoryInfo GetCurrentDirectory()
        {
            return currentDirectory;
        }
        
        public List<Item> GetSelectedItems()
        {
            List<Item> SelectedItems = new List<Item>();
            foreach(Item x in this.PanelsListView.SelectedItems)
                SelectedItems.Add(x);        
            return SelectedItems;
        }
    }
}
