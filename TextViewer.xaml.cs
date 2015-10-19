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
using System.Windows.Shapes;

namespace FileManager
{
    /// <summary>
    /// Interaction logic for TextViewer.xaml
    /// </summary>
    public partial class TextViewer : Window
    {
        FileInfo file;
        public TextViewer(FileInfo file)
        {
            this.file = file;
            InitializeComponent();
            View();
        }
        private void View()
        {
            Paragraph text = new Paragraph();
            text.Inlines.Add(File.ReadAllText(this.file.FullName, Encoding.Default));
            FlowDocument content = new FlowDocument(text);
            flowDoc.Document = content;
        }
    }
}
