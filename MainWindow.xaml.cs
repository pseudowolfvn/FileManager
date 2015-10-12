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
    public partial class MainWindow : Window
    {
        Entities.Panel[] panels = new Entities.Panel[2];

        public MainWindow()
        {
            InitializeComponent();
            panels[0] = new Entities.Panel(comboBox, listView);
            panels[1] = new Entities.Panel(comboBox1, listView1);
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
                    x.Update();
                break;
            }
        }

        private void Change_Directory( object sender, MouseButtonEventArgs e)
        {
            ListViewItem child = sender as ListViewItem;
            ListView parent = (ListView)child.Parent;
            foreach (var x in panels)
            {
                if (x.PanelsListView == parent)
                {
                    x.SetCurrentDirectory(new DirectoryInfo(x.GetParentDirectory().FullName + @"\" + child.ToString()));
                    //
                    x.Update();
                }
                break;
            }
            
        }


    }
}