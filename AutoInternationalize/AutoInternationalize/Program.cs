using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoInternationalize
{
    //enum StrategyKeyWord
    //{
    //    _text = "Text=",
    //    _title = "Title=",
    //    _tooltip = "ToolTip=",
    //    _content = "Content="
    //};    
    class Program
    {
        static string output = "";    
        static string[] StgKeyWordArray = { "Text=", "Title=", "ToolTip=", "Content=" };
        static void Main(string[] args)
        {        
            int selection;
            char choice='y';
            do
            {
                Console.WriteLine("\t\t\tMENU");
                Console.WriteLine("1. Create Resource");
                Console.WriteLine("2. Generate Localised Files");
                Console.WriteLine("3. Exit");
                Int32.TryParse(Console.ReadLine(),out selection);
                if (selection == 1)
                {
                    createResouces();
                }
                else if (selection == 2)
                {
                    CreateLocalizeFiles();
                }
                else
                {
                    choice = 'n';
                }
            } while (choice == 'y');
        }
        static void CreateLocalizeFiles()
        {
            //String[] files = Directory.GetFiles(@"C:\Users\sambhav.patni\Desktop\WCap_Edits\temp\AddressBook", "*.xaml", SearchOption.AllDirectories);
            String[] files = Directory.GetFiles(@"C:\Users\sambhav.patni\Desktop\WCap_Edits\temp\AddressBookModule", "*.xaml", SearchOption.AllDirectories);            
            foreach (string file in files)
            {
                replaceStatic(file, @"C:\Users\sambhav.patni\Desktop\WCap_Edits\Key.txt", @"C:\Users\sambhav.patni\Desktop\WCap_Edits\Value.txt");
            }
        }
        static void createResouces()
        {
            HashSet<string> strSet = new HashSet<string>();
            List<string> keySet = new List<string>();
            //String[] files = Directory.GetFiles(@"C:\Users\sambhav.patni\Desktop\WCap_Edits\temp\AddressBook", "*.xaml", SearchOption.AllDirectories);
            String[] files = Directory.GetFiles(@"C:\Users\sambhav.patni\Desktop\WCap_Edits\temp\AddressBookModule", "*.xaml", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Console.WriteLine(file);
                output += "\n" + file;
                strSet = fetchStatic(file);
                //File.AppendAllText(@"C:\Users\sambhav.patni\Desktop\WCap_Edits\out.txt", file + "\n");
                File.AppendAllLines(@"C:\Users\sambhav.patni\Desktop\WCap_Edits\Value.txt", strSet);
                foreach (string str in strSet)
                {
                    keySet.Add(Regex.Replace(str, "[^a-zA-Z0-9]", ""));
                }
                File.AppendAllLines(@"C:\Users\sambhav.patni\Desktop\WCap_Edits\Key.txt", keySet);
            }
            File.WriteAllText(@"C:\Users\sambhav.patni\Desktop\WCap_Edits\log.txt", output);
            //File.WriteAllLines(@"C:\Users\sambhav.patni\Desktop\WCap_Edits\out.txt",strSet);
        }
        static HashSet<string> fetchStatic(string file)
        {              
            string[] fileText = File.ReadAllLines(file);
            HashSet<string> stringSet = new HashSet<string>();
            foreach (string fileline in fileText)
            {
                foreach (string keyword in StgKeyWordArray)
                {
                    if (fileline.Contains(keyword))
                    {
                        int x = fileline.IndexOf('\"', fileline.IndexOf(keyword + "\"") + keyword.Length + 1);
                        int y = fileline.IndexOf('\"', fileline.IndexOf(keyword));
                        if (fileline[y + 1] == '{') { }
                        else
                        {
                            {                                
                                int length = x - y;
                                string text = fileline.Substring(fileline.IndexOf(keyword + "\"") + keyword.Length, length + 1);
                                Console.WriteLine(text);
                                output += "\n" + fileline;
                                stringSet.Add(text);
                                break;
                            }
                        }
                    }
                }
                //if (fileline.Contains("Text=") || fileline.Contains("Title=") || fileline.Contains("ToolTip=") || fileline.Contains("Content="))
                //{
                //    int length = fileline.IndexOf('\"', fileline.IndexOf("Text=\"") + 6) - fileline.IndexOf('\"', fileline.IndexOf("Text="));
                //    string text = fileline.Substring(fileline.IndexOf("Text=\""), length);
                //}
            }
            return stringSet;
        }
        static void replaceStatic(string file,string keyfilePath,string valuesfilePath)
        {
            string[] keySet = File.ReadAllLines(keyfilePath);
            string[] valueSet = File.ReadAllLines(valuesfilePath);
            string fileText = File.ReadAllText(file);
            for (int i = 0; i < valueSet.Length;i++ )
            {
                fileText = fileText.Replace(valueSet[i], "\"{x:Static localization:Resources." + keySet[i] + "}\"");
            }
            int x = fileText.IndexOf('\"', fileText.IndexOf("x:Class=\"") + "x:Class=\"".Length + 1);
            int y = fileText.IndexOf('.', fileText.IndexOf("x:Class=\""));
            int length = x - y;
            string localizeNamespace = "xmlns:localization=\"clr-namespace:" + fileText.Substring(fileText.IndexOf("x:Class=\"") + "x:Class=\"".Length, length + 1) + ".Resource\"";
            fileText = fileText.Insert(fileText.IndexOf("xmlns="), localizeNamespace + "\n");
            File.WriteAllText(@"C:\Users\sambhav.patni\Desktop\WCap_Edits\temp\"+Path.GetFileName(file) + ".done", fileText);
            //x:Class=
        }
    }
}
