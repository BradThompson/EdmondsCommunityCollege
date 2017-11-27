using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text.RegularExpressions;

namespace JPEGImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            string imagesDirectory = "Images";
            if (args.Length == 1)
                imagesDirectory = args[0];
            if (!Directory.Exists(imagesDirectory))
            {
                Console.WriteLine("Could not find directory {0}", imagesDirectory);
                return;
            }
            CreateDatabase();
            CreateStoredProcedure();
            PopulateDatabase(imagesDirectory);
        }

        static void CreateDatabase()
        {
            SqlConnection conn = new SqlConnection("Integrated Security=SSPI;");
            try
            {
                conn.Open();

                string script = File.ReadAllText("CreateDatabase.sql");

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

        static void CreateStoredProcedure()
        {
            SqlConnection conn = new SqlConnection("Integrated Security=SSPI;Initial Catalog=JPEGImporter");
            try
            {
                conn.Open();

                string script = File.ReadAllText("CreateStoredProcedure.sql");

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
                Console.WriteLine("Stored Procedure created successfully.");
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

        static void PopulateDatabase(string imagesDirectory)
        {
            var di = new DirectoryInfo(imagesDirectory);
            FileInfo[] files = di.GetFiles("*.jpg");
            if (files.Length == 0)
            {
                Console.WriteLine("No files");
                return;
            }
            try
            {
                foreach (FileInfo f in files)
                {
                    SqlConnection conn = new SqlConnection("Integrated Security=SSPI;Initial Catalog=JPEGImporter");
                    conn.Open();
                    using (SqlCommand cm = new SqlCommand("SaveImage", conn))
                    {
                        cm.CommandType = CommandType.StoredProcedure;
                        cm.Parameters.Add(new SqlParameter("@Filename", SqlDbType.NVarChar, 255, ParameterDirection.Input, false, 0, 0, "Title", DataRowVersion.Current, (SqlString)f.Name));
                        if (f.Length > 0)
                        {
                            byte[] image = System.IO.File.ReadAllBytes(f.FullName);
                            cm.Parameters.Add(new SqlParameter("@Image", SqlDbType.VarBinary, image.Length, ParameterDirection.Input, false, 0, 0, "Data", DataRowVersion.Current, (SqlBinary)image));
                        }
                        else
                        {
                            cm.Parameters.Add(new SqlParameter("@Image", SqlDbType.VarBinary, 0, ParameterDirection.Input, false, 0, 0, "Data", DataRowVersion.Current, DBNull.Value));
                        }

                        cm.ExecuteNonQuery();
                    }
                }
                //byte[] data = System.IO.File.ReadAllBytes(f.FullName);
                //Console.WriteLine("File: {0}, size: {1}", f.Name, f.Length);
                //query = string.Format("INSERT INTO Images ( Filename ) VALUES ( '{0}' )", f.Name);
                //insertCommand = new SqlCommand(query, conn);
                //insertCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}", e.Message);
            }
        }
    }
}
