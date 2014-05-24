using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace SQLRestoreScripter
{
    public enum backupType
    {
        full,
        diff,
        log
    }
    class Program
    {

        static void Main(string[] args)
        {
            MaintenancePlan maintPlan = new MaintenancePlan();
            Restore restore = new Restore();


            maintPlan = GetPaths();
            restore = new Restore(GetDB(maintPlan));

            if (restore.DBs.Count != 1)
                Console.WriteLine("You have selected all databases for restore");
            else
                Console.WriteLine("\nYou have selected: \"{0}\"", restore.DBs.Last().Database);

            string answer = "";

            Console.Clear();
            Console.WriteLine("Restore all databases to latest state?");
            answer = Console.ReadLine();

            if (answer.ToLower() == "y")
            {
                restore.SetDate(maintPlan);
                Console.WriteLine("All DBs have been set to the latest recovery points.");
            }
            else
            {
                foreach (RestoreDB dB in restore.DBs)
                {
                    restore.SetDate(dB.Database, GetDate(dB.Database, maintPlan));
                }
            }
            do
            {
                Console.WriteLine("please enter the database directory.");
                restore.RestoreTarget = Console.ReadLine();
            }while(restore.RestoreTarget == null);

            Program.SaveFile(restore.CompiledScript(maintPlan));

            Console.WriteLine("The restore command is:\n");
            Console.WriteLine(restore.CompiledScript(maintPlan));

            

            Console.ReadKey();
        }

        public static void SaveFile(string script)
        {
            string path = "";
            string str = "";

            do
            {
                Console.Clear();
                Console.WriteLine("Where would you like the script saved?");
                Console.Write("[Desktop]");
                path = Console.ReadLine();

                if (path == "")
                {
                    path = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
                    break;
                }
                else if (Directory.Exists(path))
                    break;
                else
                    Console.WriteLine("Directory does not exist");
            } while (Directory.Exists(path));
            path = path + "\\script.sql";
            System.IO.StreamWriter file = new System.IO.StreamWriter(path);
            file.WriteLine(script);

            file.Close();
        }

        /// <summary>
        /// lists the dates of available backups and returns selected DateTime ojbect
        /// </summary>
        /// <param name="restore"></param>
        /// <param name="maintPlan"></param>
        /// <returns></returns>
        public static DateTime GetDate(string dB, MaintenancePlan maintPlan)
        {
            DateTime date = new DateTime();

            string strIndex = "";
            int intIndex = 0;
            do
            {
                do
                {
                    strIndex = "";
                    Console.WriteLine(dB);
                    Console.WriteLine(CreateIndexedString(maintPlan.Dates()));

                    strIndex = Console.ReadLine();

                    if (strIndex == "")
                    {
                        intIndex = maintPlan.Dates().Count - 1;
                        strIndex = intIndex.ToString();
                        intIndex = 0;
                    }
                } while (!Int32.TryParse(strIndex, out intIndex));
                intIndex = Convert.ToInt32(strIndex);
            } while (intIndex > maintPlan.Dates().Count());
            date = maintPlan.Dates()[intIndex];

            return date;
        }


        /// <summary>
        /// Accepts a MaintenancePlan and returns a list of strings containing The users selected databases
        /// </summary>
        /// <param name="maintPlan"></param>
        /// <returns></returns>
        public static List<string> GetDB(MaintenancePlan maintPlan)
        {
            List<string> databases = new List<string>();
            string strIndex = "";
            int intIndex = 0;
            do
            {
                do
                {
                    strIndex = "";
                    Console.Clear();
                    Console.WriteLine("Choose databases from below list\n");
                    Console.WriteLine(CreateIndexedString(maintPlan.DBs()));
                    Console.Write("[All]: ");

                    strIndex = Console.ReadLine();

                    if (strIndex == "")
                        strIndex = maintPlan.DBs().Count.ToString();

                } while (!Int32.TryParse(strIndex, out intIndex));
                intIndex = Convert.ToInt32(strIndex);

            } while (maintPlan.DBs().Count < intIndex);
            if (intIndex == maintPlan.DBs().Count)
                databases.AddRange(maintPlan.DBs());
            else
                databases.Add(maintPlan.DBs()[intIndex]);

            return databases;
        }


        public static string CreateIndexedString(List<DateTime> dates)
        {
            string strReturn;
            int i = 0;
            StringBuilder sbString = new StringBuilder();

            foreach(DateTime date in dates)
            {
                sbString.AppendFormat("{0}>", i++);
                sbString.AppendFormat("\"{0}\"\n", date.Date.ToString("d"));
            }
            strReturn = sbString.ToString();
            return strReturn;
        }
        public static string CreateIndexedString(List<string> strings)
        {
            string strReturn;
            int i = 0;
            StringBuilder sbString = new StringBuilder();

            foreach (string str in strings)
            {
                sbString.AppendFormat("{0}>", i++);
                sbString.AppendFormat("\"{0}\"\n", str);
            }
            strReturn = sbString.ToString();
            return strReturn;
        }


        /// <summary>
        /// Collects the locations of full, diff, and log backup files.
        /// </summary>
        /// <returns></returns>
        public static MaintenancePlan GetPaths()
        {
            MaintenancePlan maintPlan = new MaintenancePlan();
            string answer = "";
            do
            {
                Console.Clear();
                if(maintPlan.FullBKPath == null)
                {
                    Console.WriteLine("Enter path to Full backups (c:\\FullBackups)");
                    Console.Write("Required: ");
                    maintPlan.FullBKPath = Console.ReadLine();
                    if (maintPlan.FullBKPath == null)
                    {
                        Console.Clear();
                        continue;
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Enter path to Full backups (c:\\FullBackups)");
                    Console.WriteLine("Required: {0}\n", maintPlan.FullBKPath);
                }
                
                if (maintPlan.DiffBKPath == null)
                {
                    Console.WriteLine("Enter path to Differential backups (c:\\DiffBackups)");
                    Console.Write("Optional [N\\A]: ");
                    maintPlan.DiffBKPath = Console.ReadLine();
                    if (maintPlan.DiffBKPath == null)
                    {
                        Console.Clear();
                        continue;
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Enter path to Differential backups (c:\\DiffBackups)");
                    Console.WriteLine("Optional [N\\A]: {0}\n", maintPlan.DiffBKPath);
                }

                if (maintPlan.LogBKPath == null)
                {
                    Console.WriteLine("Enter path to Log backups (c:\\LogBackups)");
                    Console.Write("Optional [N\\A]: ");
                    maintPlan.LogBKPath = Console.ReadLine();
                    if (maintPlan.LogBKPath == null)
                    {
                        Console.Clear();
                        continue;
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Enter path to Log backups (c:\\LogBackups)");
                    Console.WriteLine("Optional [N\\A]: {0}\n", maintPlan.LogBKPath);
                }
                
                Console.WriteLine("{0}", maintPlan.ToString());
                Console.Write("\nIs this correct?[N] ");
                answer = Console.ReadLine();
                
                if(answer.ToLower() != "y")
                {
                    maintPlan = new MaintenancePlan();
                }

            }while (answer.ToLower() != "y");
            return maintPlan;
        }


        public static MaintenancePlan OldGetPath()
        {
            MaintenancePlan MaintPlan = new MaintenancePlan();
            do
            {
                Console.WriteLine("Enter path to full backups (c:\\FullBackups)");
                Console.Write("Required: ");
                MaintPlan.FullBKPath = Console.ReadLine();

                if (MaintPlan.FullBKPath != null)
                    break;
                Console.Clear();
                Console.WriteLine("Supplied path does not exist!\n");


            } while (MaintPlan.FullBKPath == null);

            Console.Clear();

            Console.WriteLine("Enter path to full backups (c:\\FullBackups)");
            Console.WriteLine("Required: {0}\n", MaintPlan.FullBKPath);


            do
            {
                Console.WriteLine("Enter path to Differential backups (c:\\DiffBackups)");
                Console.Write("Optional: ");
                MaintPlan.DiffBKPath = Console.ReadLine();

                if (MaintPlan.DiffBKPath != null)
                    break;

                Console.Clear();
                Console.WriteLine("Enter path to full backups (c:\\DiffBackups)");
                Console.WriteLine("Required: {0}", MaintPlan.FullBKPath);
                Console.WriteLine("\nSupplied path does not exist!\n");

            } while (MaintPlan.DiffBKPath == null);

            Console.Clear();

            Console.WriteLine("Enter path to full backups (c:\\FullBackups)");
            Console.WriteLine("Required: {0}\n", MaintPlan.FullBKPath);

            Console.WriteLine("Enter path to Differential backups (c:\\DiffBackups)");
            Console.WriteLine("Optional: {0}\n", MaintPlan.DiffBKPath);

            do
            {
                Console.WriteLine("Enter path to Log backups (c:\\LogBackups)");
                Console.Write("Optional: ");
                MaintPlan.LogBKPath = Console.ReadLine();

                if (MaintPlan.LogBKPath != null)
                    break;

                Console.Clear();

                Console.WriteLine("Enter path to full backups (c:\\FullBackups)");
                Console.WriteLine("Required: {0}\n", MaintPlan.FullBKPath);

                Console.WriteLine("Enter path to Differential backups (c:\\DiffBackups)");
                Console.WriteLine("Optional: {0}\n", MaintPlan.DiffBKPath);

                Console.WriteLine("\nSupplied path does not exist!\n");

            } while (MaintPlan.LogBKPath == null);
            return MaintPlan;
        }


    }
}
