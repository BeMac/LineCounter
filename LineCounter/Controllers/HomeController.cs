using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Windows.Forms;
using LineSampleBrianMc.Models;

namespace LineSampleBrianMc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Result()
        {
            string selectedPath = string.Empty;

            var t = new Thread((ThreadStart) (() =>
            {
                var dialog = new FolderBrowserDialog();
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }

                selectedPath = dialog.SelectedPath;
            }));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            List<DirectoryInfo> directories = GetDirectories(selectedPath);
            IEnumerable<FileInfo> files = GetFiles(directories);

            FileInfo[] allFiles = files.OrderBy(f => f.Name).ToArray();

            List<string> allowedExtensions = GetAllowedFileExtensions();
            List<FileInfo> includedFiles = allFiles.Where(x => allowedExtensions.Contains(x.Extension)).ToList();
            List<FileInfo> excludedFiles = allFiles.Where(x => !allowedExtensions.Contains(x.Extension)).ToList();

            var totalLineCount = CountLinesInFiles(includedFiles);
            var orphanLineCount = CountLinesInFiles(excludedFiles);

            var model = new FileInfoModel
            {
                IncludedFiles = includedFiles,
                ExcludedFiles = excludedFiles,
                IncludedLineCount = totalLineCount,
                ExludedLineCount = orphanLineCount
            };

            return View(model);
        }

        private List<string> GetAllowedFileExtensions()
        {
            string config = System.Configuration.ConfigurationManager.AppSettings["SearchableFileExtensions"];
            List<string> list = config.Split(',').ToList();

            return list;
        }

        private List<DirectoryInfo> GetDirectories(string root)
        {
            var directories = new List<DirectoryInfo>();
            if (root != string.Empty)
            {
                var topDirectory = new DirectoryInfo(root);
                GetSubDirectories(topDirectory, directories);
            }

            return directories;
        }

        private void GetSubDirectories(DirectoryInfo rootDirectory, ICollection<DirectoryInfo> directories)
        {
            directories.Add(rootDirectory);

            foreach (var directory in rootDirectory.GetDirectories())
            {
                GetSubDirectories(directory, directories);
            }
        }

        private IEnumerable<FileInfo> GetFiles(List<DirectoryInfo> directories)
        {
            var files = new List<FileInfo>();

            foreach (var directory in directories)
            {
                files.AddRange(directory.GetFiles());
            }

            return files;
        }

        private int CountLinesInFile(FileInfo fileInfo)
        {
            int count = 0;
            string line;
            var sr = new StreamReader(fileInfo.FullName);
            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine(line);
                count++;
            }

            sr.Close();

            return count;
        }

        private int CountLinesInFiles(IEnumerable<FileInfo> files)
        {
            var count = 0;
            foreach (var file in files)
            {
                count += CountLinesInFile(file); ;
            }

            return count;
        }
    }
}