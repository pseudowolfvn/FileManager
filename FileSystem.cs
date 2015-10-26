using System;
using System.IO;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Entities
{
    static class FileSystem
    {
        static DriveInfo[] allDrives = DriveInfo.GetDrives();
        public static int GetNumOfDrives()
        {
            return allDrives.Length;
        }

        public static DriveInfo[] GetAllDrives()
        {
            return allDrives;
        }

        public static DriveInfo GetDrive(string name)
        {
            foreach (var x in allDrives)
                if (name == x.Name)
                    return x;
            return null;
        }

        private static  string GetCorrectName(DirectoryInfo root, ItemType type, string name, string extension = "")
        {
            int index = 0;
            string correctName;
            if (type == ItemType.Directory)
            {
                correctName = name;
                DirectoryInfo dir = new DirectoryInfo(root.FullName + @"\" + correctName);
                while (dir.Exists)
                {
                    correctName = name + "(" + (++index).ToString() + ")";
                    dir = new DirectoryInfo(root.FullName + @"\" + correctName);
                }
                return correctName;
            }
            else
            {
                correctName = name + extension;
                FileInfo file = new FileInfo(root.FullName + @"\" + correctName);
                while (file.Exists)
                {
                    correctName = name + "(" + (++index).ToString() + ")" + extension;
                    file = new FileInfo(root.FullName + @"\" + correctName);
                }
                return correctName;
            }
        }

        private static void AddText(FileStream f, string content)
        {
            byte[] temp = new UTF8Encoding(true).GetBytes(content);
            f.Write(temp, 0, temp.Length);
        }

        private static void CreateNewBitOfFile(string content, int index, Item item)
        {
            string name = item.Name + ".part" + index.ToString();
            string fullName = item.File.Directory.FullName + @"\" + name;
            FileInfo temp = new FileInfo(fullName);
            FileStream text = null;
            if (!temp.Exists) text = temp.Create();
            AddText(text, content);
            text.Close();
        }

        public static void Split(string source, int length, Item item)
        {
            int i = 0, index = 1;
            string temp;
            for (; i <= source.Length - length; i += length)
            {
                temp = source.Substring(i, length);
                CreateNewBitOfFile(temp, index++, item);
            }
            temp = source.Substring(i);
            CreateNewBitOfFile(temp, index, item);
        }

        public static void Copy(DirectoryInfo from, DirectoryInfo to)
        {
            string newName = GetCorrectName(to, ItemType.Directory, from.Name);
            string newPath = to.FullName + @"\" + newName;
            to.CreateSubdirectory(newName);
            foreach (var x in from.GetFiles())
            {
                x.CopyTo(newPath + @"\" + x.Name);
            }
            foreach (var x in from.GetDirectories())
            {
                Copy(x, new DirectoryInfo(newPath));
            }
        }

        public static void Copy(List<Item> items, DirectoryInfo directory)
        {
            foreach (Item x in items)
            {
                if (x.File != null)
                    x.File.CopyTo(directory.FullName.ToString() + @"\" + x.Name + x.Extension);
                else Copy(x.Directory, directory);
            }
                
        }

        public static void Move(List<Item> items, DirectoryInfo directory)
        {
            foreach (Item x in items)
            {
                int index = 0;
                string dirFullName = directory.FullName.ToString() + @"\";
                if (x.File != null)
                {
                    FileInfo file = new FileInfo(dirFullName + x.Name + x.Extension);
                    while (file.Exists)
                    {
                        file = new FileInfo(dirFullName + x.Name + "(" + (++index).ToString() + ")" + x.Extension);
                    }
                    x.File.MoveTo(file.FullName);
                    index = 0;
                }
                else if (x.Directory.Root == directory.Root) x.Directory.MoveTo(dirFullName);
                else
                {
                    FileSystem.Copy(x.Directory, new DirectoryInfo(dirFullName));
                    x.Directory.Delete(true);
                }
            }
        }

        public static void Delete(List<Item> items)
        {
            foreach (Item x in items)
                if (x.File != null)
                    x.File.Delete();
                else
                    x.Directory.Delete(true);
        }

        public static void Rename(DirectoryInfo directory, string name)
        {
            //string fullName = directory.FullName + @"\" + name;
            //DirectoryInfo dir = new DirectoryInfo(fullName);

            directory.MoveTo(directory.Parent.FullName + @"\" + name);
        }

        public static void Rename(FileInfo file, string name)
        {
            file.MoveTo(file.Directory.FullName + @"\" + name);
        }

        public static void New(DirectoryInfo currentDirectory, ItemType type, string name, string extension)
        {
            int index = 1;
            string fullName = currentDirectory.FullName + @"\" + name + extension;
            if (type == ItemType.Directory)
            {
                var newDirectory = new DirectoryInfo(fullName);
                if (!newDirectory.Exists) newDirectory.Create();
                else
                {
                    while (newDirectory.Exists)
                    {
                        string temp = currentDirectory.FullName + @"\" + name + "(" + index.ToString() + ")" + extension;
                        ++index;
                        newDirectory = new DirectoryInfo(temp);
                    }
                    newDirectory.Create();
                }
            }
            else if (type == ItemType.File)
            {
                var newFile = new FileInfo(fullName);
                if (!newFile.Exists)  newFile.Create();
                else
                {
                    while (newFile.Exists)
                    {
                        string temp = currentDirectory.FullName + @"\" + name + "(" + index.ToString() + ")" + extension;
                        ++index;
                        newFile = new FileInfo(temp);
                    }
                    newFile.Create();
                }
            }
        }
        }

    public enum ItemType { Directory, File, Indefinite };

    public class Item
    {
        private string name;
        public string Name { get { return name; } set { name = value; } }
        private string extension;
        public string Extension { get { return extension; } set { extension = value; } }
        private string length;
        public string Length { get { return length; } set { length = value; } }
        private DateTime creationTime;
        public DateTime CreationTime { get { return creationTime; } set { creationTime = value; } }
        private DirectoryInfo directory;
        public DirectoryInfo Directory { get { return directory; } set { directory = value; } }
        private FileInfo file;
        public FileInfo File { get { return file; } set { file = value; } }

        public Item(DirectoryInfo directory, string name = "")
        {
            this.directory = directory;
            if (name == "") this.name = directory.Name;
            else this.name = name;
            this.extension = "";
            this.length = @"<DIR>";
            this.creationTime = directory.CreationTime;
        }

        public Item(FileInfo file)
        {
            this.file = file;
            string withoutExtension = file.Name;
            int i = file.Name.LastIndexOf('.');
            if (i != -1) withoutExtension = file.Name.Substring(0, i);
            this.name = withoutExtension;
            this.extension = file.Extension;
            this.length = file.Length.ToString();
            this.creationTime = file.CreationTime;
        }

    }

}
