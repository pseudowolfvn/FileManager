using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace FileManager
{
    static class CreateSaveConfigFile
    {
        static string DEFAULT_PATH = @"D:\dev\Projects\VS\FileManager\FileManager";
        static public void Create(int num_of_panels, List<DirectoryInfo> list)
        {
            var configFile = new XmlDocument();
            configFile.AppendChild(configFile.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement root = configFile.CreateElement("FileManeger");
            configFile.AppendChild(root);
            for (int numOfPanels = 0; numOfPanels < num_of_panels; numOfPanels++)
            {
                XmlElement panel = configFile.CreateElement("Panel");
                XmlAttribute directory = configFile.CreateAttribute("CurrentDirectory");
                directory.Value = list[numOfPanels].FullName;
                panel.Attributes.Append(directory);
                root.AppendChild(panel);
            }
            configFile.Save(GetCorrectPath("SaveConfigFile.xml"));
        }
        static private string GetCorrectPath(string name)
        {
            string result = CreateSaveConfigFile.DEFAULT_PATH + name;
            return result;
        }
    }
}