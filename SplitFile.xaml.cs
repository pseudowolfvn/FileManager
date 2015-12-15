using System;
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
using System.Windows.Shapes;

namespace FileManager
{
    /// <summary>
    /// Interaction logic for SplitFile.xaml
    /// </summary>
    public partial class SplitFile : Window
    {
        private int size ;
        public int Size { get { return size; } set { size = value; } }
        public SplitFile()
        {
            InitializeComponent();
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(textBox.Text) <= 0)
            {
                string message = "Size must be positive value! Let's try again.";
                MessageBox.Show(message);
                return;
            }
            else
            {
                int temp = Convert.ToInt32(textBox.Text);
                Size = temp;
                this.Close();
            }
            this.Close();
        }
    }
}
