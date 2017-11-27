using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace EdmondsInterview
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "Winter2011CCSSE.txt";
            if (args.Length == 1)
                fileName = args[0];
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Could not find file {0}", fileName);
                return;
            }
            CreateDatabase();
            PopulateDatabase(fileName);
        }

        static void CreateDatabase()
        {
            // SQL Server (SQLEXPRESS)
            // SqlConnection conn = new SqlConnection("Data Source=localhost\MSSQL13.SQLEXPRESS; Initial Catalog=AdventureWorks");
            // SqlConnection conn = new SqlConnection("Data Source=(local);Initial Catalog=master;Integrated Security=SSPI;");
            // Server=localhost;Database=master;Trusted_Connection=True;
            SqlConnection conn = new SqlConnection("Integrated Security=SSPI;");
            try
            {
                conn.Open();

                string script = File.ReadAllText("EdmondsInterview.sql");

                // split script on GO command
                IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (string commandString in commandStrings)
                {
                    if (commandString.Trim() != "")
                    {
                        Console.WriteLine("{0}", commandString);
                        new SqlCommand(commandString, conn).ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Database created successfully.");
            }
            catch (SqlException er)
            {
                Console.WriteLine("{0}", er.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        static void PopulateDatabase(string fileName)
        {
            List<string> sidList = new List<string>();
            List<string> courseList = new List<string>();
            List<string> yearList = new List<string>();
            string corruptLine = "";

            SqlConnection conn = new SqlConnection("Integrated Security=SSPI;Initial Catalog=EdmondsInterview");
            conn.Open();
            string query = "";
            SqlCommand insertCommand = null;
            try
            {
                using (var sr = File.OpenText(fileName))
                {
                    var l = sr.ReadLine();      // Fetch headers
                    while (!sr.EndOfStream)
                    {
                        corruptLine = "";
                        l = sr.ReadLine();
                        string[] data = l.Split('|');
                        string SID = data[0];
                        if (!sidList.Contains(SID))
                        {
                            sidList.Add(SID);
                            query = string.Format("INSERT INTO Student ( SID ) VALUES ( '{0}' )", SID);
                            insertCommand = new SqlCommand(query, conn);
                            insertCommand.ExecuteNonQuery();
                        }
                        string DEPT_DIV = data[1];
                        string COURSE_NUM = data[2];
                        string deptCourse = DEPT_DIV + COURSE_NUM;
                        if (!courseList.Contains(deptCourse))
                        {
                            courseList.Add(deptCourse);
                            query = string.Format("INSERT INTO Course ( DEPT_DIV, COURSE_NUM ) VALUES ( '{0}', '{1}' )", DEPT_DIV, COURSE_NUM);
                            insertCommand = new SqlCommand(query, conn);
                            insertCommand.ExecuteNonQuery();
                        }
                        string YEAR_ID = data[4];
                        if (!yearList.Contains(YEAR_ID))
                        {
                            yearList.Add(YEAR_ID);
                            query = string.Format("INSERT INTO Year ( YEAR_ID ) VALUES ( '{0}' )", YEAR_ID);
                            insertCommand = new SqlCommand(query, conn);
                            insertCommand.ExecuteNonQuery();
                        }

                        string grade = data[3];
                        if (grade.IndexOf("missing") >= 0)
                        {
                            grade = "-1";
                            corruptLine = " - Bad grade";
                        }
                        string quarter = data[5];
                        int quarterId = 0;
                        switch (quarter.ToLower())
                        {
                            case "summer":
                                quarterId = 1;
                                break;
                            case "fall":
                                quarterId = 2;
                                break;
                            case "winter":
                                quarterId = 3;
                                break;
                            case "spring":
                                quarterId = 4;
                                break;
                            default:
                                quarterId = 1;
                                corruptLine += " - Bad Quarter";
                                break;
                        }
                        string credits = data[6];
                        if (credits.IndexOf("missing") >= 0)
                        {
                            credits = "-1";
                            corruptLine += " - Bad credits";
                        }
                        string date = data[7];
                        query = string.Format("INSERT INTO Class ( SID, DEPT_DIV, COURSE_NUM, Grade, YEAR_ID, QUARTER_ID, Credits, Date ) VALUES ( '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}' )",
                                SID, DEPT_DIV, COURSE_NUM, grade, YEAR_ID, quarterId, credits, date);
                        insertCommand = new SqlCommand(query, conn);
                        insertCommand.ExecuteNonQuery();
                        if (!string.IsNullOrEmpty(corruptLine))
                        {
                            Console.WriteLine("{0}{1}", l, corruptLine);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}", e.Message);
            }
        }
    }
}
