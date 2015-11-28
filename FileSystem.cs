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
        static List<DriveInfo> allDrives;
        static FileSystem()
        {
            allDrives = new List<DriveInfo>();
            foreach (var x in DriveInfo.GetDrives())
                if (x.IsReady) allDrives.Add(x);
        }
        public static int GetNumOfDrives()
        {
            return allDrives.Count;
        }
        public static DriveInfo[] GetAllDrives()
        {
            return allDrives.ToArray();
        }

        public static DriveInfo GetDrive(string name)
        {
            foreach (var x in allDrives)
                if (name == x.Name)
                    return x;
             return null;
        }

        public static DirectoryInfo ExistingSubdirectory(DirectoryInfo dir)
        {
            DirectoryInfo existDirectory = dir;
            while (Directory.Exists(existDirectory.FullName) != true) existDirectory = existDirectory.Parent;
            return existDirectory;
        }

        private static string GetCorrectName(DirectoryInfo root, ItemType type, string name, string extension = "")
        {
            int index = 0;
            string correctName = name;
            if (type == ItemType.Directory)
            {
                while (Directory.Exists(root.FullName + @"\" + correctName))
                {
                    correctName = name + "(" + (++index).ToString() + ")";
                }
            }
            else
            {
                correctName += extension;
                while (File.Exists(root.FullName + @"\" + correctName))
                {
                    correctName = name + "(" + (++index).ToString() + ")" + extension;
                }
            }
            return correctName;
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
                    x.File.CopyTo(directory.FullName.ToString() + @"\" + GetCorrectName(directory, ItemType.File, x.Name, x.Extension));
                else Copy(x.Directory, directory);
            }
                
        }

        public static void Move(List<Item> items, DirectoryInfo directory)
        {
            foreach (Item x in items)
            {
                string dirFullName = directory.FullName.ToString() + @"\";
                if (x.File != null)
                {
                    string correctName = x.Name + x.Extension;
                    if (File.Exists(directory.FullName + @"\" + correctName))
                        correctName = GetCorrectName(directory, ItemType.File, x.Name, x.Extension);
                    x.File.MoveTo(directory.FullName + @"\" + correctName);
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
                if (x.File != null && File.Exists(x.File.FullName))
                    x.File.Delete();
                else if (Directory.Exists(x.Directory.FullName))
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
            if (type == ItemType.Directory)
            {
                string fullname = currentDirectory.FullName + @"\" + GetCorrectName(currentDirectory, ItemType.Directory, name, extension);
                var newDirectory = new DirectoryInfo(fullname);
                newDirectory.Create();
            }
            else if (type == ItemType.File)
            {
                string fullname = currentDirectory.FullName + @"\" + GetCorrectName(currentDirectory, ItemType.File, name, extension);
                var newFile = new FileInfo(fullname);
                newFile.Create();
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
