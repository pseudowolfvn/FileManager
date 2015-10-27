﻿using System;
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
        private List<Panel> panels = new List<Panel>();
        static int numOfPanels = 0;
        private ListView ActiveListView;
        public MainWindow()
        {
            InitializeComponent();
            panels.Add(AddNewPanel());
            panels.Add(AddNewPanel());
            ActiveListView = panels[0].PanelsListView;
        }

        public Panel AddNewPanel()
        {
            ListView newLV = new ListView();
            newLV.Style = Resources["PanelListView"] as Style;
            newLV.ItemContainerStyle = Resources["PanelListViewItem"] as Style; ;
            GridView columns = new GridView();
            GridViewColumn name = new GridViewColumn();
            name.Header = "Name";
            name.DisplayMemberBinding = new Binding("Name");
            name.Width = 100;
            GridViewColumn type = new GridViewColumn();
            type.Header = "Type";
            type.DisplayMemberBinding = new Binding("Extension");
            type.Width = 100;
            GridViewColumn size = new GridViewColumn();
            size.Header = "Size";
            size.DisplayMemberBinding = new Binding("Length");
            size.Width = 100;
            GridViewColumn date = new GridViewColumn();
            date.Header = "Date of creation";
            date.DisplayMemberBinding = new Binding("CreationTime");
            date.Width = 100;
            columns.Columns.Add(name);
            columns.Columns.Add(type);
            columns.Columns.Add(size);
            columns.Columns.Add(date);
            newLV.View = columns;
            ComboBox newCB = new ComboBox();
            newCB.Style = Resources["DrivesComboBox"] as Style;
            ColumnDefinition newColumn = new ColumnDefinition();
            newColumn.Width = new GridLength(1, GridUnitType.Star);
            PanelsGrid.ColumnDefinitions.Add(newColumn);
            newLV.SetValue(Grid.RowProperty, 1);
            newLV.SetValue(Grid.ColumnProperty, numOfPanels);
            newCB.SetValue(Grid.RowProperty, 0);
            newCB.SetValue(Grid.ColumnProperty, numOfPanels);
            PanelsGrid.Children.Add(newLV);
            PanelsGrid.Children.Add(newCB);
            ++numOfPanels;
            return new Panel(newCB, newLV);
        }

        public void DeletePanel()
        {
            PanelsGrid.ColumnDefinitions.RemoveAt(PanelsGrid.ColumnDefinitions.Count - 1);
            PanelsGrid.Children.RemoveAt(PanelsGrid.Children.Count - 1);
            --numOfPanels;
        }

        public void NotifyObserves(string message)
        {
            if (message == "Delete" || message == "Move" || message == "Rename")
                foreach (var x in panels)
                    if (x.IsChanged()) x.Update();
            if (message == "Copy" || message == "Move" || message == "New" || message == "Init" || message == "Change" || message == "Split")
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
            DriveInfo[] allDrives = FileSystem.GetAllDrives();
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

        private void ItemHandled(object sender, MouseButtonEventArgs a)
        {
            ListViewItem child = sender as ListViewItem;
            Panel x = GetActivePanel();
            Item item = (Item)child.Content;
                if ((item.Directory != null))// && (!item.Directory.GetAccessControl().AccessRightType.))
                {
                    x.ChangeDirectory(item.Directory);
                    NotifyObserves("Change");
                }
                else if (item.File != null)
                    FileOpen(item.File);
                else
            {
                string text = "You haven't access to this directory";
                MessageBoxResult exception = MessageBox.Show(text);
            }
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

        private Item GetOneSelected()
        {
            List<Item> selected = GetSelected();
            //if (selected[1] == null)
                return selected[0];
            //else - exception
        }

        public void SplitFile(object sender, RoutedEventArgs e)
        {
            Item document = GetOneSelected();
            SplitFile dialog = new SplitFile();
            dialog.ShowDialog();
            FileSystem.Split(File.ReadAllText(document.File.FullName, Encoding.UTF8), dialog.Size, document);
            NotifyObserves("Split");
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
            if (dialog.Name != "")
            {
                FileSystem.New(panel.GetCurrentDirectory(), dialog.Type, dialog.Name, dialog.Extension);
                NotifyObserves("New");
            }
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
                if (dialog.Name != "")
                {
                    if (type == ItemType.Directory) FileSystem.Rename(x.Directory, dialog.Name);
                    else FileSystem.Rename(x.File, dialog.Name + dialog.Extension);
                }
            }
            NotifyObserves("Rename");
        }

        private void PanelAdded(object sender, RoutedEventArgs e)
        {
            panels.Add(AddNewPanel());
        }

        private void PanelDeleted(object sender, RoutedEventArgs e)
        {
            DeletePanel();
        }
    }
}