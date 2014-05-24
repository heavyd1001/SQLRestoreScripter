using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace SQLRestoreScripter
{
    public class BackupFile
    {
        private string _name;
        private string _path;
        private DateTime _lastModified;
        private long _size;
        private string _databaseName;
        private string _fileExtension;

        public BackupFile(backupType type, FileInfo file)
        {
            Name = file.Name;
            Path = file.DirectoryName;
            LastModified = file.LastWriteTime;
            BackupType = type;
            Size = file.Length;
            DatabaseName = Regex.Split(Name, "_backup_")[0];
            FileExtension = file.Extension;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        public DateTime LastModified
        {
            get { return _lastModified; }
            set { _lastModified = value; }
        }
        public backupType BackupType
        {
            get;
            set;
        }
        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }
        public string FileExtension
        {
            get { return _fileExtension; }
            set { _fileExtension = value; }
        }
    }
}