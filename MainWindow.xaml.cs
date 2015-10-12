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
        Panel[] panels = new Panel[2];
        private ListView ActiveListView; 

        public Panel GetActivePanel()
        {
            foreach (var x in panels)
                if (x.PanelsListView.Equals(ActiveListView))
                    return x;
            return null;
        }

        public MainWindow()
        {
            InitializeComponent();
            panels[0] = new Panel(comboBox, listView);
            panels[1] = new Panel(comboBox1, listView1);
        }


        public void AddDrivesInComboBox(object sender, RoutedEventArgs e)
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
        public void Test (object sender, RoutedEventArgs e)
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
            if (x.PanelsListView.SelectedIndex == 0)
            {
                x.ChangeDirectory(x.GetParentDirectory());
            }
            else
            {
                string newPath = x.GetCurrentDirectory().FullName;
                if (!newPath.EndsWith(@"\")) newPath += @"\";
                newPath += child.Content.ToString();
                x.ChangeDirectory(new DirectoryInfo(newPath));
            }
        }

        private void PanelChanged(object sender, RoutedEventArgs e)
        {
            ActiveListView = sender as ListView;
        }

        private void DiskChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ComboBoxItem child = sender as ComboBoxItem;
            string driveName = child.Content.ToString();
            GetActivePanel().ChangeDrive(FileSystem.GetDrive(driveName));
        }
    }
}