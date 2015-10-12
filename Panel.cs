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
        static int DriveID;
        DriveInfo currentDrive;
        DirectoryInfo currentDirectory;
        public Panel(ComboBox panelsComboBox, ListView panelsListView)
        {
            currentDrive = FileSystem.GetAllDrives()[(DriveID++) % FileSystem.GetNumOfDrives()];
            currentDirectory = currentDrive.RootDirectory;
            PanelsComboBox = panelsComboBox;
            PanelsListView = panelsListView;
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
            int i = 0;
            foreach (var x in this.GetSubdirectories())
            {
                PanelsListView.Items.Add(new ListViewItem());
                PanelsListView.Items[i++] = x.Name;
            }
            foreach (var x in this.GetFiles())
            {
                PanelsListView.Items.Add(new ListViewItem());
                PanelsListView.Items[i++] = x.Name;
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

        public void SetCurrentDirectory(DirectoryInfo value)
        {
            currentDirectory = value;
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
        

    }
}
