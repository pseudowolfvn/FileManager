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
    /// Interaction logic for RenameDialog.xaml
    /// </summary>
    using FileManager.Entities;
    public enum DialogType { New, Rename };
    public partial class RenameDialog : Window
    {
        DialogType dialogType;
        ItemType itemType;
        string name;
        string extension;
        public string Name { get { return name; } set { name = value; } }
        public string Extension { get { return extension; } set { extension = value; } }
        public ItemType Type { get { return itemType; } set { itemType = value; } }

        static readonly string defaultFolderName = "New Folder";
        static readonly string defaultFileName = "New File";
        char[] forbiddenSymbols = { ':', '\\', '/', '*', '?', '"', '|', '<', '>' };
        public RenameDialog(ItemType type = ItemType.Indefinite, string name = "", string extension = "")
        {
            this.name = name;
            this.extension = extension;
            itemType = type;
            InitializeComponent();
            if (type == ItemType.Indefinite)
            {
                dialogType = DialogType.New;
                typeComboBox.IsEnabled = true;
                this.Title = "New";
                this.typeTextBlock.Text = "Create new :";
            }
            else
            {
                dialogType = DialogType.Rename;
                typeComboBox.IsEnabled = false;
                this.Title = "Rename";
                this.typeTextBlock.Text = "Rename this :";
            }
            if (type == ItemType.Directory) extensionTextBox.IsEnabled = false;
            nameTextBox.Text = name;
            extensionTextBox.Text = extension.Substring(extension.IndexOf('.') + 1);
        }

        private void AddTypes(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Items.Add(ItemType.Directory);
            comboBox.Items.Add(ItemType.File);
            if (dialogType == DialogType.New) comboBox.SelectedIndex = (int)ItemType.Directory;
            else comboBox.SelectedIndex = (int)itemType;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            if ((nameTextBox.Text.IndexOfAny(forbiddenSymbols) != -1) 
                || (extensionTextBox.Text.IndexOfAny(forbiddenSymbols) != -1))
            {
                string forbiddenList = "";
                foreach (char x in  forbiddenSymbols)
                    forbiddenList += x + " ";
                string text = "A name of file cannot contain any of this symbols: " + forbiddenList;
                MessageBox.Show(text);
                return;
            }
            if (nameTextBox.Text == "")
            {
                if (dialogType == DialogType.New)
                    if (itemType == ItemType.Directory)
                        this.Name = defaultFolderName;
                    else this.Name = defaultFileName;
            }
            else 
            {
                this.Name = nameTextBox.Text;
                if (itemType == ItemType.File) this.Extension = "." + extensionTextBox.Text;
            }
            this.Close();
        }

        private void TypeChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox child = sender as ComboBox;
            this.Type = (ItemType)child.SelectedItem;
            if (Type == ItemType.Directory)
            {
                extensionTextBox.Text = "";
                extensionTextBox.IsEnabled = false;
            } 
            else if (Type == ItemType.File) extensionTextBox.IsEnabled = true;
        }
    }
}
