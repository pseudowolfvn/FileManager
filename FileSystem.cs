using System;
using System.IO;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Entities
{
    public static class FileSystem
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
        public static void Copy(DirectoryInfo from, DirectoryInfo to)
        {
            string newPath = to.FullName + @"\" + from.Name;
            to.CreateSubdirectory(from.Name);
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
                string dirFullName = directory.FullName.ToString() + @"\";
                if (x.File != null)
                    x.File.MoveTo(dirFullName + x.Name + x.Extension);
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
            directory.MoveTo(directory.FullName + @"\" + name);
        }

        public static void Rename(FileInfo file, string name)
        {
            file.MoveTo(file.Directory.FullName + @"\" + name);
        }

        public static void New(DirectoryInfo currentDirectory, ItemType type, string name)
        {
            int index = 1;
            string fullName = currentDirectory.FullName + @"\" + name;
            if (type == ItemType.Directory)
            {
                var newDirectory = new DirectoryInfo(fullName);
                if (!newDirectory.Exists) newDirectory.Create();
                else
                {
                    while (newDirectory.Exists)
                    {
                        string temp = fullName + "(" + index.ToString() + ")";
                        ++index;
                        newDirectory = new DirectoryInfo(temp);
                    }
                    newDirectory.Create();
                }
            }
            else if (type == ItemType.File)
            {
                var newFile = new FileInfo(fullName);
                if (!newFile.Exists) newFile.Create();
                else
                {
                    while (newFile.Exists)
                    {
                        string temp = fullName + "(" + index.ToString() + ")";
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
