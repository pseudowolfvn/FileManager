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

        public static List<List<string>> LongestWords(string source, int n)
        {
            string[] words = source.Split(' ');
            List<List<string>> result = new List<List<string>>(n);
            for (int i = 0; i < n; i++)
            {
                result.Add(new List<string>());
                result[i].Add("");
            }
            foreach (var word in words)
            {
                int i = 0;
                while (i < n && word.Length < result[i][0].Length) ++i;
                if (i != n)
                {
                    if (word.Length == result[i][0].Length) result[i].Add(word);
                    else
                    {
                        for (int index = n - 1; index > i; --index)
                        {
                            List<string> copy = new List<string>(result[index - 1]);
                            result[index].Clear();
                            result[index] = copy;
                        }
                        result[i].Clear();
                        result[i].Add(word);
                    }

                }
            }
            return result;
        }
        private enum States { Default, WhiteSpaces, NewLine };
        public static string RemoveWhitespaces(string source)
        {
            string result = "";
            States state = States.Default;
            bool OtherThanWhite = false;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == ' ' || source[i] == '\t')
                {
                    if (state == States.Default || state == States.NewLine) state = States.WhiteSpaces;
                }
                else if (source[i] == '\r')
                {
                    if (state == States.Default || (OtherThanWhite))
                    {
                        if (state == States.WhiteSpaces) result += " ";
                        result += "\n";
                    }
                    ++i;
                    OtherThanWhite = false;
                    state = States.NewLine;
                }
                else
                {
                    if (state == States.WhiteSpaces)
                    {
                        result += " ";
                    }
                    result += source[i];
                    OtherThanWhite = true;
                    state = States.Default;
                }

            }
            return result;
        }
    }
}
