/*
 *  UtilMethod.cs
 *  Part of SqlServer2022-RdlcPdfRenderer
 *
 *  Copyright (C) 2025 Uzma Ashraf
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Threading;

namespace PDFCustom
{
    class UtilMethod
    {

        public static string folderLocation;
        public static string fileName;
        public static string extension;
        private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        public static void LogMessageToFile(Exception ex)
        {
            if (!string.IsNullOrEmpty(folderLocation) && !string.IsNullOrEmpty(fileName))
            {
                string drive = Path.GetPathRoot(folderLocation);
                if (Directory.Exists(drive))
                {
                    locker.EnterWriteLock();
                    if (!Directory.Exists(folderLocation))
                    {
                        // Try to create the directory.
                        DirectoryInfo di = Directory.CreateDirectory(folderLocation);
                    }
                    //string path = folderLocation + System.DateTime.Now.Date.ToString("yyyy-MM-dd") + fileName;
                    string path = String.Format("{0}/{1}_{2}{3}", folderLocation, fileName, System.DateTime.Now.Date.ToString("yyyy-MM-dd"), extension);
                    if (!File.Exists(path))
                        File.Create(path).Dispose();

                    try
                    {
                        using (StreamWriter sw = new StreamWriter(path, true))
                        {
                            string logLine = System.String.Format(
                                "{0:G}: {1}.", System.DateTime.Now, ex.Message + Environment.NewLine + "StackTrace :" + ex.StackTrace);
                            sw.WriteLine(logLine);
                            sw.Close();
                        }
                    }
                    catch (Exception)
                    {

                        //throw exception;
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
            }
        }


        public static void LogMessageToFile(string msg)
        {

            if (!string.IsNullOrEmpty(folderLocation) && !string.IsNullOrEmpty(fileName))
            {
                string drive = Path.GetPathRoot(folderLocation);
                if (Directory.Exists(drive))
                {
                    locker.EnterWriteLock();
                    if (!Directory.Exists(folderLocation))
                    {
                        // Try to create the directory.
                        DirectoryInfo di = Directory.CreateDirectory(folderLocation);
                    }
                    //string path = folderLocation + System.DateTime.Now.Date.ToString("yyyy-MM-dd") + fileName;
                    string path = String.Format("{0}/{1}_{2}{3}", folderLocation, fileName, System.DateTime.Now.Date.ToString("yyyy-MM-dd"), extension);
                    if (!File.Exists(path))
                        File.Create(path).Dispose();

                    try
                    {

                        using (StreamWriter sw = new StreamWriter(path, true))
                        {
                            string logLine = System.String.Format(
                                "{0:G}: {1}.", System.DateTime.Now, msg);
                            sw.WriteLine(logLine);
                            sw.Close();

                        }
                    }
                    catch (Exception)
                    {

                        //throw ex;
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
            }
        }
    }
}
