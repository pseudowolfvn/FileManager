using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileManager.Entities;
using System.Windows;

namespace FileManager
{
    static public class ReadXMLConfigFileByDom
    {
        static string DEFAULT_PATH = @"D:\dev\Projects\VS\FileManager\FileManager";
        static public List<DirectoryInfo> ReadXMLSaveConfigFile()
            {
                XmlDocument configFile = new XmlDocument();
                configFile.Load(GetCorrectPath("SaveConfigFile.xml"));
                return ReturnListOfDirectories(configFile.DocumentElement);
            }
        static public string GetCorrectPath(string name)
        {
            string result = ReadXMLConfigFileByDom.DEFAULT_PATH + name;
            return result;
        }
        
        static private List<DirectoryInfo> ReturnListOfDirectories(XmlNode node)
        {
            List<DirectoryInfo> resultList = new List<DirectoryInfo>();
            foreach (XmlNode n in node.ChildNodes)
            {
                DirectoryInfo temp = new DirectoryInfo(n.Attributes[0].Value);
                resultList.Add(temp);
            }
            return resultList;
        }

    }
    static public class ReadXMLConfigFileBySax
    {
        static public List<DirectoryInfo> ReadXMLSaveConfigFile()
        {
            var configFile = new XmlTextReader(ReadXMLConfigFileByDom.GetCorrectPath("SaveConfigFile.xml"));
            return ReturnListOfDirectories(configFile);
        }
        static private List<DirectoryInfo> ReturnListOfDirectories(XmlTextReader reader)
        {
            List<DirectoryInfo> resultList = new List<DirectoryInfo>(); 
            XmlTextReader tempReader = reader;
            while(reader.Read())
            {
                if((reader.Name.Equals("Panel")) && (reader.NodeType == XmlNodeType.Element))
                {
                    if (reader.HasAttributes)
                    {
                        reader.MoveToAttribute("CurrentDirectory");
                        DirectoryInfo temp = new DirectoryInfo(reader.Value);
                        resultList.Add(temp);
                    }
                }
            }
            return resultList;
        }
    }
    static public class ReadXMLConfigFileByLinq
    {
        static public List<DirectoryInfo> ReadXMLSaveConfigFile()
        {
            XDocument configFile = XDocument.Load(ReadXMLConfigFileByDom.GetCorrectPath("SaveConfigFile.xml"));
            return ReturnListOfDirectories(configFile);
        }

        //----------LONG_METHOD----------
        //static private List<DirectoryInfo> ReturnListOfDirectories(XDocument doc)
        //{
        //    List<DirectoryInfo> resultList = new List<DirectoryInfo>();
        //    foreach (XElement elem in doc.Root.Elements())
        //    {
        //        if (elem.Name.ToString().Equals("Panel"))
        //        {
        //            DirectoryInfo temp = new DirectoryInfo(elem.Attribute("CurrentDirectory").Value.ToString());
        //            resultList.Add(temp);
        //        }
        //    }
        //    return resultList;
        //}

        //----------SHORT_METHOD----------
        static private List<DirectoryInfo> ReturnListOfDirectories(XDocument doc)
        {
            List<DirectoryInfo> resultList = new List<DirectoryInfo>();
            var ListOfDirs = from pan in doc.Root.Elements("Panel")
                             let str = new DirectoryInfo(pan.Attributes().First().Value)
                             select str;
            foreach (DirectoryInfo dir in ListOfDirs)
                resultList.Add(dir);
            return resultList;
        }
    }
}
