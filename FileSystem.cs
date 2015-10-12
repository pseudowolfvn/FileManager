using System;
using System.IO;
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
    }
}
