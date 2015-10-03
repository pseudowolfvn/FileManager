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
        public static DriveInfo[] GetDrives()
        {
            return allDrives;
        }
    }
}
