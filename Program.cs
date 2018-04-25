using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Diagnostics;
using Newtonsoft.Json;

namespace rpcs3Updater
{
    public class Program
    {
		static int percentage = 0;
        public static void Main(string[] args)
        {			
			Process[] pname = Process.GetProcessesByName("rpcs3");
			if (File.Exists("RPCS3.log") & pname.Length == 0)
			{						                           		
				var getjson = _get_json_data<RootObject>("https://update.rpcs3.net/?c=somecommit"); //get json data from rpcs3 commit api using newtonsoft.json library
				string windowsurl = getjson.latest_build.windows.download;                          //get the latest build url from json
				Match sversionmatch = Regex.Match(windowsurl, @"/artifacts/rpcs3-([^:]+)_win64.7z"); //use regex to get the server version.
				Match lversionmatch = Regex.Match(File.ReadLines("RPCS3.log").First() , @"RPCS3 ([^:]+) Alpha"); //use regex to get local version id of rpcs3
				string lversion = lversionmatch.Groups[1].Value.Remove(lversionmatch.Groups[1].Value.Length - 1); //delete the last char of local version
				string sversion = sversionmatch.Groups[1].Value; 
				if (lversion == sversion)			//if local version equals to server version
				{
					Console.WriteLine("RPCS3 is up to date"); 
				}           
				else
				{
					Console.WriteLine("RPCS3 is outdated, latest version available is: " + sversion );  //print the up to date version
					Console.WriteLine("Do you want to download?(Y/n)");
					string answer = Console.ReadLine();
					if (answer == "y")
					{
						File.Delete("rpcs3.exe");                                                          
						if (File.Exists("rpcs3Update.7z"))
						{
							File.Delete("rpcs3Update.7z");
						}
						using (WebClient wc = new WebClient())
						{
							wc.DownloadProgressChanged += wc_stringpercentage;
							wc.DownloadFileAsync(new System.Uri(windowsurl), "rpcs3Update.7z");   //download latest build
							
							while (percentage < 100)
							{
								string message = "Downloading rpcs3 archive for updating: ";						
								Console.Write("\r{0}{1}% complete.",message, percentage);   
							}							
						}						
						Console.WriteLine(" ");
						Console.WriteLine("File downloaded succesfully, extracting rpcs3.exe");						
						try
						{
							ProcessStartInfo process = new ProcessStartInfo();
							process.WindowStyle = ProcessWindowStyle.Hidden;
							process.FileName = "7zr.exe";                        //use 7zip to extract rpcs3.exe                 
							process.Arguments = "e rpcs3Update.7z rpcs3.exe";
							Process p = Process.Start(process);
							p.WaitForExit();
						}
						catch (System.Exception Ex) 
						{								
						}
					}
					Console.WriteLine("Done!");	
				}							
				Console.WriteLine("Press a key to close...");
				Console.ReadKey();
			}
			else
			{
				Console.WriteLine("RPCS3.log couldn't found! Copy this executable to rpcs3 directory or close RPCS3.");
				Console.WriteLine("Press a key to close...");
				Console.ReadKey();
			}			           
        }
		static void wc_stringpercentage(object sender, DownloadProgressChangedEventArgs e)
		{
			percentage = e.ProgressPercentage;
		}

        public static T _get_json_data<T>(string url) where T : new()
        {
             using (var w = new WebClient())
             {
                 var json_data = string.Empty;

                 try
                 {
                    json_data = w.DownloadString(url);
                 }
                 catch (Exception) { }
                 return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
             }
        }
	}
}
