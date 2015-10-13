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

        public static string GetPathToDirectory(Entities.Panel x, string child)
        {
            string newPath = x.GetCurrentDirectory().FullName;
            if (!newPath.EndsWith(@"\")) newPath += @"\";
            return newPath += child;
        }
    }
                
}
