using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SQLRestoreScripter
{
    public class MaintenancePlan
    {
        private string fullBKPath;
        private string diffBKPath;
        private string logBKPath;

        private BackupFolder fullBKFolder;
        private BackupFolder diffBKFolder;
        private BackupFolder logBKFolder;

        public MaintenancePlan()
        {
            fullBKPath = null;
            diffBKPath = null;
            logBKPath = null;

            fullBKFolder = null;
            diffBKFolder = null;
            logBKFolder = null;
        }

        public string FullBKPath
        {
            get { return fullBKPath; }
            set
            {
                if (Directory.Exists(value))
                { 
                    fullBKPath = value;
                    fullBKFolder = new BackupFolder(fullBKPath, backupType.full);
                }
                else { return; }
            }
        }
        public string DiffBKPath
        {
            get { return diffBKPath; }
            set
            {
                if (Directory.Exists(value))
                {
                    diffBKPath = value;
                    diffBKFolder = new BackupFolder(diffBKPath, backupType.diff);
                }
                else if (value == "") { diffBKPath = "N\\A"; }
                else { return; }
            }
        }
        public string LogBKPath
        {
            get { return logBKPath; }
            set
            {
                if (Directory.Exists(value))
                {
                    logBKPath = value;
                    logBKFolder = new BackupFolder(logBKPath, backupType.log);
                }
                else if (value == "") { logBKPath = "N\\A"; }
                else { return; }
            }
        }

        public BackupFolder FullBKFolder
        {
            get { return fullBKFolder; }
        }
        public BackupFolder DiffBKFolder
        {
            get { return diffBKFolder; }
        }
        public BackupFolder LogBKFolder
        {
            get { return logBKFolder; }
        }

        public override string ToString()
        {
            string toString = string.Format("Full Backup Path: {0}\nDiff Backup Path: {1}\n Log Backup Path: {2}"
                , this.FullBKPath
                , this.DiffBKPath
                , this.LogBKPath);
            return toString;
        }

        public List<string> DBs()
        {
            List<string> DBs;
            DBs =  fullBKFolder.Files.Select(x => x.DatabaseName).Distinct().ToList();
            return DBs;
        }

        public List<DateTime> Dates()
        {
            List<DateTime> dates;
            dates = FullBKFolder.Files.Select(x => x.LastModified.Date).Distinct().ToList();
            return dates;
        }

        public List<string> FileNames()
        {
            List<string> fileNames;
            fileNames = FullBKFolder.Files.Select(x => x.Name).ToList();
            return fileNames;
        }
    }
}
