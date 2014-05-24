using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SQLRestoreScripter
{
    public class Restore
    {
        List<RestoreDB> _dBs;
        List<string> _cmds;
        string _restoreTarget;



        public Restore() : this(new List<string>()) { }
        public Restore(List<string> dBs)
        {
            Cmds = new List<string>();
            DBs = new List<RestoreDB>();
            foreach (string dB in dBs)
            {
                DBs.Add(new RestoreDB(dB));
            }
            _restoreTarget = null;
        }

        public void SetDate(string database, DateTime date)
        {
            foreach (RestoreDB dB in DBs)
            {
                if (dB.Database == database)
                {
                    dB.Date = date;
                    return;
                }
            }
        }
        public void SetDate(MaintenancePlan maintPlan)
        {
            foreach (RestoreDB dB in DBs)
            {
                foreach (BackupFile file in maintPlan.FullBKFolder.Files)
                {
                    if(file.DatabaseName == dB.Database)
                    {
                        int result = dB.Date.CompareTo(file.LastModified.Date);
                        if (result < 0)
                            dB.Date = file.LastModified.Date;
                    }
                }
            }
        }

        public string GenerateBackupCommand(MaintenancePlan maintPlan,RestoreDB genDatabase)
        {
            string command;
            StringBuilder sbString = new StringBuilder();

            command = string.Format("RESTORE DATABASE '{0}' FROM DISK = '{1}\\{2}' WITH FILE = 1 NORECOVERY;"
                , genDatabase.Database
                , maintPlan.FullBKFolder.Path
                , maintPlan.FullBKFolder.Files
                    .Where(x => x.LastModified.Date.Equals(genDatabase.Date)
                             && x.DatabaseName.Equals(genDatabase.Database))
                    .Select(x => x.Name)
                    .First()
                    .ToString());

            return command;
        }
        public string CompiledScript(MaintenancePlan maintPlan)
        {
            StringBuilder sbString = new StringBuilder();

            string script;
            foreach (RestoreDB database in DBs)
            {
                sbString.AppendLine(GenerateBackupCommand(maintPlan,database));
                sbString.AppendLine("");
            }
            script = sbString.ToString();
            return script;
        }

        public List<string> Cmds
        {
            get { return _cmds; }
            set { _cmds = value; }
        }
        public List<RestoreDB> DBs
        {
            get { return _dBs; }
            set { _dBs = value; }
        }
        public string RestoreTarget
        {
            get { return _restoreTarget; }
            set 
            {
                if (Directory.Exists(value))
                {
                    _restoreTarget = value;
                }
                else { return; }
            }
        }
    }
    public class RestoreDB
    {
        string _database;
        DateTime _date;
        DateTime _time;

        public RestoreDB() : this("", default(DateTime), default(DateTime)) { }
        public RestoreDB(string database) : this(database, default(DateTime), default(DateTime)) { }
        public RestoreDB(string database,DateTime date, DateTime time)
        {
            Database = database;
            Date = date;
            Time = time;
        }

        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }
        public backupType FileType { get; set; }

    }
}
