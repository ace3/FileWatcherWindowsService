using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileMonitor
{
    public partial class Service1 : ServiceBase
    {
        string WatchPath1 = ConfigurationManager.AppSettings["WatchPath1"];

        string lastFiles = "";
        public Service1()
        {
            InitializeComponent();
            fileSystemWatcher1.Created += fileSystemWatcher1_Created;
            fileSystemWatcher1.Changed += fileSystemWatcher1_Changed;
            fileSystemWatcher1.Renamed += fileSystemWatcher1_Renamed;
            fileSystemWatcher1.Deleted += fileSystemWatcher1_Deleted;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var client = new RestClient("https://slack.com");
                
                var request = new RestRequest("api/chat.postMessage", Method.POST);
                request.AddParameter("token", "xoxb-455495421282-486439996225-8X35BQD68l6ipgz1buOMvSrp");
                request.AddParameter("channel", "general");
                request.AddParameter("text", "Service Started");
                request.AddParameter("as_user", false);
                // execute the request
                var response = client.Execute(request);
                var content = response.Content; // raw content as string
                fileSystemWatcher1.Path = WatchPath1;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnStop()
        {
            try
            {
                //Create_ServiceStoptextfile();
                var client = new RestClient("https://slack.com");
               
                var request = new RestRequest("api/chat.postMessage", Method.POST);
                request.AddParameter("token", "xoxb-455495421282-486439996225-8X35BQD68l6ipgz1buOMvSrp");
                request.AddParameter("channel", "general");
                request.AddParameter("text", "Service Stopped");
                request.AddParameter("as_user", false);

                // execute the request
                var response = client.Execute(request);
                var content = response.Content; // raw content as string
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void NotifyNewFile(string FullPath, string FileName)
        {
          
            if (lastFiles != FileName)
            {
                var client = new RestClient("https://slack.com");

                var request = new RestRequest("api/chat.postMessage", Method.POST);
                request.AddParameter("token", "xoxb-455495421282-486439996225-8X35BQD68l6ipgz1buOMvSrp");
                request.AddParameter("channel", "general");
                request.AddParameter("text", FileName + " created with fullPath of " + FullPath);
                request.AddParameter("as_user", false);
                // execute the request
                var response = client.Execute(request);
                var content = response.Content; // raw content as string

                lastFiles = FileName;
            }
        }

        private bool CheckFileExistance(string FullPath, string FileName)
        {
            // Get the subdirectories for the specified directory.'  
            bool IsFileExist = false;
            DirectoryInfo dir = new DirectoryInfo(FullPath);
            if (!dir.Exists)
                IsFileExist = false;
            else
            {
                string FileFullPath = Path.Combine(FullPath, FileName);
                if (File.Exists(FileFullPath))
                    IsFileExist = true;
            }
            return IsFileExist;


        }

        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {

        }

        private void fileSystemWatcher1_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            try
            {
               
                //Then we need to check file is exist or not which is created.  
                if (CheckFileExistance(WatchPath1, e.Name))
                {
                    //Then write code for log detail of file in text file.  
                    NotifyNewFile(WatchPath1, e.Name);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void fileSystemWatcher1_Renamed(object sender, System.IO.RenamedEventArgs e)
        {

        }

        private void fileSystemWatcher1_Deleted(object sender, FileSystemEventArgs e)
        {

        }
    }
}
