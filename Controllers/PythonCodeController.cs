using AIUnlocked___backend.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AIUnlocked___backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PythonCodeController : ControllerBase
    {
        private void DeleteAllFilesInFolder(string folder)
        {
            var dir = new DirectoryInfo(Path.Combine(folder, "train_data"));

            foreach(var file in dir.GetFiles())
            {
                System.IO.File.Delete(file.FullName);
            }

            dir.Delete();
        }

        [HttpPost]
        [Route("postPythonCode")]
        public void RunPythonCode(RunCodeDto codeDto)
        {
            // copy relevant images to target folder
            var oldDirClass1 = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Storage\\OwnDatabase\\" + codeDto.class1 + "\\train_data"));
            var oldDirClass2 = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Storage\\OwnDatabase\\" + codeDto.class2 + "\\train_data"));

            var destDir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "PythonSandbox\\ImageDatabase\\"));

            // delete everything from destination folder
            foreach(var file in destDir.GetDirectories())
            {
                DeleteAllFilesInFolder(file.FullName);
                System.IO.Directory.Delete(file.FullName);
            }

            // copy both classes to destination folder
            
            // first class
            System.IO.Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "PythonSandbox\\ImageDatabase\\" + codeDto.class1));
            var dirClass1 = System.IO.Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "PythonSandbox\\ImageDatabase\\" + codeDto.class1 + "\\train_data"));

            foreach (var file in oldDirClass1.GetFiles())
            {
                string filenameOnly = file.FullName.Split("\\").Last();
                System.IO.File.Copy(file.FullName, Path.Combine(dirClass1.FullName, filenameOnly));
            }

            // second class
            System.IO.Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "PythonSandbox\\ImageDatabase\\" + codeDto.class2));
            var dirClass2 = System.IO.Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "PythonSandbox\\ImageDatabase\\" + codeDto.class2 + "\\train_data"));

            foreach (var file in oldDirClass2.GetFiles())
            {
                string filenameOnly = file.FullName.Split("\\").Last();
                System.IO.File.Copy(file.FullName, Path.Combine(dirClass2.FullName, filenameOnly));
            }

            string pySandboxFile = Path.Combine(Environment.CurrentDirectory, "PythonSandbox/pythonCode.py");

            System.IO.File.Create(pySandboxFile).Close();

            using (StreamWriter outputFile = new StreamWriter(pySandboxFile))
            {
                outputFile.Write(codeDto.pythonCode);
            }

            // TODO: move python exe closer (inside solution maybe?)

            // actually running the code
            run_cmd();
        }

        private void run_cmd()
        {
            string pyPath = "C:\\Users\\Triferment\\AppData\\Local\\Programs\\Python\\Python38-32\\python.exe";
            string scriptPath = Path.Combine(Environment.CurrentDirectory, "PythonSandbox\\pythonCode.py");

            ProcessStartInfo start = new ProcessStartInfo(pyPath);

            start.Arguments = scriptPath;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;

            Process myProcess = new Process();
            myProcess.StartInfo = start;

            myProcess.Start();

            StreamReader myStreamReader = myProcess.StandardOutput;
            string myString = myStreamReader.ReadLine();

            // wait exit signal from the app we called and then close it. 
            myProcess.WaitForExit();
            myProcess.Close();

            // write the output we got from python app 
            var resultString = "Value received from script: " + myString;
        }
    }
}
