﻿using System;
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
    public partial class RenameDialog : Window
    {
        ItemType dialogType;
        string name;
        string extension;
        public string Name { get { return name; } set { name = value; } }
        public string Extension { get { return extension; } set { extension = value; } }
        public ItemType Type { get { return dialogType; } set { dialogType = value; } }
        public RenameDialog(ItemType type = ItemType.Indefinite, string name = "", string extension = "")
        {
            dialogType = type;
            InitializeComponent();
            if (type == ItemType.Indefinite)
            {
                typeComboBox.IsEnabled = true;
                this.Title = "New";
                this.typeTextBlock.Text = "Create new :";
            }
            else
            {
                typeComboBox.IsEnabled = false;
                this.Title = "Rename";
                this.typeTextBlock.Text = "Rename this :";
            }
            if (type == ItemType.Directory) extensionTextBox.IsEnabled = false;
            nameTextBox.Text = name;
            extensionTextBox.Text = extension;
        }

        private void AddTypes(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Items.Add(ItemType.Directory);
            comboBox.Items.Add(ItemType.File);
            comboBox.SelectedIndex = (int)dialogType;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            this.Name = nameTextBox.Text;
            this.Extension = extensionTextBox.Text;
            this.Close();
        }

        private void TypeChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox child = sender as ComboBox;
            this.Type = (ItemType)child.SelectedItem;
            if (Type == ItemType.Directory) extensionTextBox.IsEnabled = false;
            else if (Type == ItemType.File) extensionTextBox.IsEnabled = true;
        }
    }
}
