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
        static int driveID;
        int currentDriveID;
        DriveInfo currentDrive;
        DirectoryInfo currentDirectory;
        public Panel(ComboBox panelsComboBox, ListView panelsListView)
        {
            currentDriveID = (driveID++) % FileSystem.GetNumOfDrives();
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
        
        public void Update ()
        {
            PanelsListView.Items.Clear();
            if (currentDirectory.FullName != currentDrive.RootDirectory.FullName)
            {
                PanelsListView.Items.Add(new Item(currentDirectory.Parent, @"↑..."));
            }
            foreach (var x in this.GetSubdirectories())
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
            Update();
        }

        public void ChangeDirectory(DirectoryInfo root)
        {
            currentDirectory = root;
            Update();
        }

        public DirectoryInfo GetCurrentDirectory()
        {
            return currentDirectory;
        }

        public DirectoryInfo GetParentDirectory ()
        {
            ListViewItem dev = new ListViewItem();
            ListView test = new ListView();
            return currentDirectory.Parent;
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
