using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Entities
{
    class Panel
    {
        static int currentDrive;
        DriveInfo activeDrive;
        DirectoryInfo currentDirectory;
        public Panel()
        {
            activeDrive = FileSystem.GetDrives()[(currentDrive++) % FileSystem.GetNumOfDrives()];
            currentDirectory = activeDrive.RootDirectory;
        }

        public void GetSubdirectories(DirectoryInfo root)
        {
            DirectoryInfo[] SubDir = root.GetDirectories();
            foreach (var x in SubDir)
            {
                Console.WriteLine(x);
            }
        }
        
    }
}
