using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SHMA.Enterprise.Configuration;
using System.Text;
using System.IO;


namespace ServcPullACTPay
{
    static class TextLogger
    {

        public delegate T RetryOpenDelegate<T>();

        /// <summary>
        /// Perform an action until succeeds without throwing IOException
        /// </summary>
        /// <typeparam name="T">object returned</typeparam>
        /// <param name="action">action performed</param>
        /// <returns></returns>

        public static T RetryOpen<T>(RetryOpenDelegate<T> action)
        {
            while (true)
            {
                try
                {
                    return action();
                }
                catch (IOException)
                {
                    System.Threading.Thread.Sleep(50);
                }
            }
        }

        public static void WriteToLogFile(string msg)
        {
        	string Setting = AppSettings.GetSetting("ExceptionLogEnabled");
            if ((Setting != null) && (Setting.ToLower().Equals("true")))
            {
                TextWriter tw = null;
                try
                {

                    // create our daily directory
                    
                    //string dir = Application.StartupPath + "\\logs\\" + DateTime.Now.ToString("ddMMyyyy");
                    string dir = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + DateTime.Now.ToString("ddMMyyyy");
                    //string dir = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "logs";

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    string LogFile = dir + "\\" + DateTime.Now.ToString("ddMMyyyy") + "_log.txt";

                    // create a writer and open the file
                    tw = RetryOpen<StreamWriter>(delegate()
                    {
                        return new StreamWriter(LogFile, true);
                    });
                    // write a line of text to the file
                    tw.WriteLine(DateTime.Now + " : " + msg);
                }
                catch(System.Exception e) {
                    //MessageBox.Show(e.Message);
                }
                finally
                {
                    // close the stream
                    if (tw != null)
                    {
                        tw.Close();
                        tw.Dispose();
                    }
                }
            }
        }


    }
}
