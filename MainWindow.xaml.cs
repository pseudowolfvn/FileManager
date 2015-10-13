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

        public Panel GetActivePanel()
        {
            foreach (var x in panels)
                if (x.PanelsListView.Equals(ActiveListView))
                    return x;
            return null;
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

        }
        private void PanelInitialized(object sender, RoutedEventArgs e)
        {
            ListView test = sender as ListView;
            foreach (var x in panels)
            {
                if (x.PanelsListView.Equals(test))
                {
                    x.Update();
                    break;
                }
            }
        }

        private void DirectoryChanged(object sender, MouseButtonEventArgs e)
        {
            ListViewItem child = sender as ListViewItem;
            Panel x = GetActivePanel();
            Item item = (Item)child.Content;
            if (x.PanelsListView.SelectedIndex == 0)
            {
                x.ChangeDirectory(x.GetParentDirectory());
            }
            else
            {
                x.ChangeDirectory(new DirectoryInfo(FileSystem.GetPathToDirectory(x, item.Name)));
            }
        }

        
        private void PanelChanged(object sender, RoutedEventArgs e)
        {
            ActiveListView = sender as ListView;
        }

        private void DiskChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox child = sender as ComboBox;
            string driveName = child.SelectedValue.ToString();
            GetConnectedPanel(child).ChangeDrive(FileSystem.GetDrive(driveName));
        }
    }
}