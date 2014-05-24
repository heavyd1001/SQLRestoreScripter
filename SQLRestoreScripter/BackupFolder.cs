using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SQLRestoreScripter
{
    public class BackupFolder
    {
        private string path;
        private DirectoryInfo info;
        private List<BackupFile> files;

        public BackupFolder(string path, backupType type)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            Files = new List<BackupFile>();

            Path = path;
            Type = type;

            info = new DirectoryInfo(Path);

            fileInfos = info.GetFiles().Select(x => x).ToList();
            foreach (FileInfo fileInfo in fileInfos)
            {
                BackupFile bakFile = new BackupFile(Type, fileInfo);
                Files.Add(bakFile);
            }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public backupType Type
        {
            get;
            set;
        }
        public DirectoryInfo Info
        {
            get { return info; }
        }
        public List<BackupFile> Files
        {
            get { return files; }
            set { files = value; }
        }
    }
}
