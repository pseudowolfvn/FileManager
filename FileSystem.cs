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
        public static void CopyTo(List<FileInfo> files, DirectoryInfo directory)
        {
            foreach (var x in files)
                x.CopyTo(directory.FullName.ToString() + @"\" + x.Name);
        }
        public static void MoveTo(List<FileInfo> files, DirectoryInfo directory)
        {
            foreach (var x in files)
                x.MoveTo(directory.FullName.ToString() + @"\" + x.Name);
        }
        public static void Delete(List<FileInfo> files)
        {
            foreach (var x in files)
                x.Delete();
        }
    }

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
        }
        public Item(FileInfo file)
        {
            this.file = file;
            this.name = file.Name;
            this.extension = file.Extension;
            this.length = file.Length.ToString();
        }

    }

}
