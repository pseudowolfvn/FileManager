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
        private List<Panel> panels = new List<Panel>();
        static int numOfPanels = 0;
        private ListView ActiveListView;
        public MainWindow()
        {
            InitializeComponent();
            AddNewPanel();
            AddNewPanel();
            ActiveListView = panels[0].PanelsListView;
        }
        private GridViewColumn AddGridViewColumn(string header, string binding)
        {
            GridViewColumn column = new GridViewColumn();
            column.Header = header;
            column.DisplayMemberBinding = new Binding(binding);
            column.Width = 100;
            return column;
        }

        public Panel AddNewPanel()
        {
            ListView newLV = new ListView();
            ComboBox newCB = new ComboBox();
            panels.Add(new Panel(newCB, newLV));
            newLV.Style = Resources["PanelListView"] as Style;
            newLV.ItemContainerStyle = Resources["PanelListViewItem"] as Style; ;
            GridView columns = new GridView(); 
            columns.Columns.Add(AddGridViewColumn( "Name", "Name"));
            columns.Columns.Add(AddGridViewColumn( "Type", "Extension"));
            columns.Columns.Add(AddGridViewColumn( "Size", "Length"));
            columns.Columns.Add(AddGridViewColumn( "Date of creation", "CreationTime"));
            newLV.View = columns;
            newLV.Loaded += PanelInitialized;
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
            AddDrivesInComboBox(newCB);
            newCB.SelectionChanged += DiskChanged;
            return panels[numOfPanels++];
        }

        public void DeletePanel()
        {
            DeletePanelAt(numOfPanels);
        }

        public void DeletePanelAt(int index)
        {
            if (numOfPanels > 2)
            {
                PanelsGrid.ColumnDefinitions.RemoveAt(index - 1);
                PanelsGrid.Children.Remove(panels[index - 1].PanelsComboBox);
                PanelsGrid.Children.Remove(panels[index - 1].PanelsListView);
                panels.RemoveAt(index - 1);
                --numOfPanels;
            }
            else MessageBox.Show("Must be at least 2 panels!");
        }
        public void NotifyObserves(string message)
        {
            if (message == "Copy" ||  message == "Move" || message == "New" || message == "Rename" || message == "Delete")
            {
                DirectoryInfo changedDirectory = GetActivePanel().GetCurrentDirectory();
                foreach (var x in panels)
                    if (x.IsChanged(changedDirectory)) x.Update();
            }
            if (message == "Copy" || message == "Move" || message == "New" || message == "Init" || message == "Change" || message == "Split" || message == "Restore")
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

        private void AddDrivesInComboBox(ComboBox sender)
        {
            int i = 0;
            ComboBox comboBox = sender as ComboBox;
            DriveInfo[] allDrives = FileSystem.GetAllDrives();
            foreach (var x in allDrives)
            {
                comboBox.Items.Add(new ComboBoxItem());
                comboBox.Items[i++] = x.ToString();
            }
            comboBox.SelectionChanged -= DiskChanged;
            comboBox.SelectedIndex = GetConnectedPanel(comboBox).GetCurrentDriveID();
            comboBox.SelectionChanged += DiskChanged;

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
            if (item.IsReadable)
            {
                if ((item.Directory != null))
                {
                    x.ChangeDirectory(item.Directory);
                    NotifyObserves("Change");
                }
                else if (item.File != null)
                    FileOpen(item.File);
            }
            else MessageBox.Show("You haven't enough access rights to do so!");
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

        public void DiskChanged(object sender, SelectionChangedEventArgs e)
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
                return selected[0];
        }

        public void SplitFile(object sender, RoutedEventArgs e)
        {
            Item file = GetOneSelected();
            if (file.File == null) return;
            SplitFile dialog = new SplitFile();
            dialog.ShowDialog();
            if (dialog.Size > 0)
            {
                FileSystem.Split(File.ReadAllText(file.File.FullName, Encoding.UTF8), dialog.Size, file);
                NotifyObserves("Split");
            }
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
            if (dialog.Name.Length + dialog.Extension.Length + panel.GetCurrentDirectory().FullName.Length >= FileSystem.MaxPathLength)
            {
                MessageBox.Show("A full path cannot be longer than " + FileSystem.MaxPathLength + " symbols");
                return;
            }
            else if (dialog.Name != "")
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
                if ((x.Directory != null && dialog.Name.Length + x.Directory.Parent.FullName.Length >= FileSystem.MaxPathLength)
                    || (x.File != null && dialog.Name.Length + dialog.Extension.Length + x.File.Directory.FullName.Length >= FileSystem.MaxPathLength))
                {
                    MessageBox.Show("A full path cannot be longer than " + FileSystem.MaxPathLength + " symbols");
                    return;
                }
                if ((dialog.Name != x.Name) 
                    || (type == ItemType.File && dialog.Extension != x.Extension))
                {
                    if (type == ItemType.Directory)
                    {
                        string correctName = dialog.Name;
                        int dotOrSpace = correctName.Length - 1;
                        while (correctName[dotOrSpace] == '.' || correctName[dotOrSpace] == ' ')
                            --dotOrSpace;
                        if (dotOrSpace != correctName.Length - 1) correctName = correctName.Remove(dotOrSpace + 1);
                        if (correctName != x.Name) FileSystem.Rename(x.Directory, correctName);
                    }
                    else FileSystem.Rename(x.File, dialog.Name, dialog.Extension);
                    NotifyObserves("Rename");
                }
            }
        }

        private List<DirectoryInfo> CreateListOfCurrentDirecrtories()
        {
            List<DirectoryInfo> result = new List<DirectoryInfo>();
            foreach (Panel x in panels)
                result.Add(x.GetCurrentDirectory());
            return result;
        }
        private void OnClickSave(object sender, RoutedEventArgs e)
        {
            CreateSaveConfigFile.Create(numOfPanels, CreateListOfCurrentDirecrtories());
        }

        private void OnClickLoadByDom(object sender, RoutedEventArgs e)
        {
            List<DirectoryInfo> tempList = ReadXMLConfigFileByDom.ReadXMLSaveConfigFile();
            LoadByList(tempList);
        }
        private void OnClickLoadBySax(object sender, RoutedEventArgs e)
        {
            List<DirectoryInfo> tempList = ReadXMLConfigFileBySax.ReadXMLSaveConfigFile();
            LoadByList(tempList);
        }
        private void OnClickLoadByLinq(object sender, RoutedEventArgs e)
        {
            List<DirectoryInfo> tempList = ReadXMLConfigFileByLinq.ReadXMLSaveConfigFile();
            LoadByList(tempList);
        }
        private void LoadByList(List<DirectoryInfo> list)
        {
            List<DirectoryInfo> tempList = list;
            while (numOfPanels < tempList.Count)
                AddNewPanel();
            int panelsNum = 0;
            foreach (DirectoryInfo dir in tempList)
            {
                DriveInfo tempDrive = new DriveInfo(dir.Root.FullName);
                if (panels[panelsNum].ChangeSource(dir))
                {
                    SetActivePanel(panels[panelsNum]);
                    ++panelsNum;
                    NotifyObserves("Restore");
                }
                else DeletePanelAt(panelsNum + 1);
            }
        }

        private void OnClickSearch(object sender, KeyEventArgs e)
        {
            string required = "";
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                required = textBox.Text;
                List<Item> forSearch = GetSelected();
                List<Item> result = new List<Item>();
                foreach(Item x in forSearch)
                {
                    if (x.Directory != null)
                        result.AddRange(FileSystem.Search(x.Directory, required));
                }
                Panel resultPanel = AddNewPanel();
                ListView resultList = resultPanel.PanelsListView;
                resultList.Loaded -= PanelInitialized;
                resultList.Items.Clear();
                foreach (Item x in result)
                {
                    x.Name = x.FullName;
                    resultList.Items.Add(x);
                }

            }
        }

        private void PanelAdded(object sender, RoutedEventArgs e)
        {
            AddNewPanel();
        }
        
        private void PanelDeleted(object sender, RoutedEventArgs e)
        {
            DeletePanel();
        }
    }
}