using System;
using System.IO;
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

namespace FileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    using FileManager.Entities;
    public partial class MainWindow : Window
    {
        private Panel[] panels;
        private ListView ActiveListView;
        public MainWindow()
        {
            InitializeComponent();
            panels = new Panel[2];
            panels[0] = new Panel(leftDiskChanger, leftPanel);
            panels[1] = new Panel(rightDiskChanger, rightPanel);
            ActiveListView = panels[0].PanelsListView;
        }

        private void NotifyObserves(string message)
        {
            if (message == "Delete" || message == "Move" || message == "Rename")
                foreach (var x in panels)
                    if (x.IsChanged()) x.Update();
            if (message == "Copy" || message == "Move" || message == "New" || message == "Init" || message == "Change")
                GetActivePanel().Update();
        }

        public Panel GetActivePanel()
        {
            foreach (var x in panels)
                if (x.PanelsListView.Equals(ActiveListView))
                    return x;
            return null;
        }

        public void SetActivePanel(Panel panel)
        {
            ActiveListView = panel.PanelsListView;
        }

        public Panel GetConnectedPanel(ListView item)
        {
            foreach (var x in panels)
                if (x.PanelsListView.Equals(item))
                    return x;
            return null;
        }

        public Panel GetConnectedPanel(ComboBox item)
        {
            foreach (var x in panels)
                if (x.PanelsComboBox.Equals(item))
                    return x;
            return null;
        }

        private void AddDrivesInComboBox(object sender, RoutedEventArgs e)
        {
            int i = 0;
            ComboBox comboBox = sender as ComboBox;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (var x in allDrives)
            {
                comboBox.Items.Add(new ComboBoxItem());
                comboBox.Items[i++] = x.ToString();
            }
            comboBox.SelectedIndex = GetConnectedPanel(comboBox).GetCurrentDriveID();

        }
        private void PanelInitialized(object sender, RoutedEventArgs e)
        {
            SetActivePanel(GetConnectedPanel(sender as ListView));
            NotifyObserves("Init");
        }

        private void ItemHandled(object sender, MouseButtonEventArgs e)
        {
            ListViewItem child = sender as ListViewItem;
            Panel x = GetActivePanel();
            Item item = (Item)child.Content;
            if (item.Directory != null)
            {
                x.ChangeDirectory(item.Directory);
                NotifyObserves("Change");
            }
            else if (item.File != null)
                FileOpen(item.File);
        }

        private static void FileOpen(FileInfo file)
        {
            var editor = new TextViewer(file);
            editor.ShowDialog();
        }

        private void PanelChanged(object sender, RoutedEventArgs e)
        {
            ActiveListView = sender as ListView;
        }

        private void DiskChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox child = sender as ComboBox;
            string driveName = child.SelectedValue.ToString();
            Panel panel = GetConnectedPanel(child);
            SetActivePanel(panel);
            panel.ChangeDrive(FileSystem.GetDrive(driveName));
            NotifyObserves("Change");
        }

        private List<Item> GetSelectedExceptActive()
        {
            List<Item> result = new List<Item>();
            foreach (var x in panels)
                if (!x.Equals(GetActivePanel()))
                {
                    result.AddRange(x.GetSelectedItems());
                }
            return result;
        }

        private List<Item> GetSelected()
        {
            List<Item> result = new List<Item>();
            foreach (var x in panels)
                result.AddRange(x.GetSelectedItems());
            return result;
        }

        
        private bool Agreement(string operation, List<Item> list, string to)
        {
            string message;
            message = "Do you really want to " + operation;
            foreach (Item x in list)
                message = message + " " + x.Name + 
                    " (creation time: " + x.CreationTime.ToString() + ", size: " + x.Length.ToString() + "b) ";
            if (operation != "delete")
                message = message + " to " + GetActivePanel().GetCurrentDirectory().FullName + "?";
            else message = message + "?";
            MessageBoxButton button = MessageBoxButton.OKCancel;
            MessageBoxImage icon = MessageBoxImage.Question;
            MessageBoxResult result = MessageBox.Show(message, operation, button, icon);
            if (result == MessageBoxResult.OK) return true;
            else return false;
        }

        private void OnClickCopy(object sender, RoutedEventArgs e)
        {
            if (Agreement("copy", GetSelectedExceptActive(), GetActivePanel().GetCurrentDirectory().FullName))
            {
                FileSystem.Copy(GetSelectedExceptActive(), GetActivePanel().GetCurrentDirectory());
                NotifyObserves("Copy");
            }
        }
        private void OnClickMove(object sender, RoutedEventArgs e)
        {
            if (Agreement("move", GetSelectedExceptActive(), GetActivePanel().GetCurrentDirectory().FullName))
            {
                FileSystem.Move(GetSelectedExceptActive(), GetActivePanel().GetCurrentDirectory());
                NotifyObserves("Move");
            }
        }
        private void OnClickDelete(object sender, RoutedEventArgs e)
        {
            if (Agreement("delete", GetSelected(), ""))
            {
                FileSystem.Delete(GetSelected());
                NotifyObserves("Delete");
            }
        }

        private void OnClickNew(object sender, RoutedEventArgs e)
        {
            RenameDialog dialog = new RenameDialog();
            var panel = GetActivePanel();
            dialog.ShowDialog();
            FileSystem.New(panel.GetCurrentDirectory(), dialog.Type, dialog.Name + dialog.Extension);
            NotifyObserves("New");
        }

        private void OnClickRename(object sender, RoutedEventArgs e)
        {
            ItemType type;
            List<Item> items = GetSelected();
            foreach (var x in items)
            {
                if (x.Directory != null) type = ItemType.Directory;
                else type = ItemType.File;
                RenameDialog dialog = new RenameDialog(type, x.Name, x.Extension);
                dialog.ShowDialog();
                if (type == ItemType.Directory) FileSystem.Rename(x.Directory, dialog.Name);
                else FileSystem.Rename(x.File, dialog.Name + dialog.Extension);
            }
            NotifyObserves("Rename");
        }
    }
}