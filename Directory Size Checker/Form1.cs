using System;
using System.IO;
using System.Windows.Forms;

namespace Directory_Size_Checker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = ProgramFilesx86();
            DirectoryInfo rootDir = new DirectoryInfo(path);
            GetPrograms(rootDir);

            void GetPrograms(DirectoryInfo root)
            {
                try
                {
                    DirectoryInfo[] subDirs = root.GetDirectories();
                    string log = "";

                    foreach (DirectoryInfo di in subDirs)
                    {
                        decimal convertedBytes = WalkDirectoryTree(di) / 1024 / 1024;
                        log += di.Name + " " + Decimal.Round(convertedBytes, 2) + " MB\n";
                    }

                    MessageBox.Show(log);
                }

                catch (DirectoryNotFoundException error)
                {
                    Console.WriteLine(error.Message);
                }
            }
        }

        //Gives access to program files
        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        //Recursive walk through tree
        static decimal WalkDirectoryTree(DirectoryInfo root)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            decimal totalBytes = 0;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles();
            }

            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }

            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (FileInfo fi in files)
                {
                    //Add current dir files to running total
                    totalBytes += fi.Length;
                }

                //Find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo);
                }
            }

            return totalBytes;
        }
    }
}
