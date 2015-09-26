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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void treeView_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo dirDev = new DirectoryInfo(@"D:\");
            DirectoryInfo[] subdirDev = dirDev.GetDirectories();
            TreeViewItem root = new TreeViewItem();
            root.Header = dirDev.FullName.ToString();
            List<string> childs = new List<string>();
            foreach (var x in subdirDev)
            {
                childs.Add(x.Name.ToString());
            }
            root.ItemsSource = childs.ToArray();
            var tree = sender as TreeView;
            tree.Items.Add(root);
        }
    }
}
