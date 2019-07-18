using System;
using System.IO;
using UnityEngine;

namespace Thesis
{
    public interface ILogManager
    {
        void Write(string message);
    }

    public class LogManager : ILogManager
    {
        string filename = "Log.csv";
        string fullFilePath = String.Empty;

        public LogManager(string folderPath, string fileName)
        {
            //檔案名稱
            //filename = "Log" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".csv";
            filename = fileName + ".csv";

            //資料夾路徑
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            //完整路徑
            fullFilePath = Path.Combine(folderPath, filename);
            Debug.Log("fullFilePath: " + fullFilePath);
        }
        
        public void Write(string message)
        {
            var line = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "," + Time.time + "," + message;

            using (StreamWriter w = File.AppendText(fullFilePath))
            {
                w.WriteLine(line);
            }
        }
    }
}