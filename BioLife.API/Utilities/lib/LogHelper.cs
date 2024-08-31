using System.Net;

namespace HuloToys_Service.Utilities.Lib
{
    public static class LogHelper
    {
        public static void InsertLogTelegramByUrl(string bot_token, string id_group, string msg)
        {
            string JsonContent = string.Empty;
            string url_api = "https://api.telegram.org/bot" + bot_token + "/sendMessage?chat_id=" + id_group + "&text=" + msg;
            try
            {
                using (var webclient = new WebClient())
                {
                    JsonContent = webclient.DownloadString(url_api);
                }
            }
            catch (Exception ex)
            {
                WriteLogActivity("D://", ex.ToString());
            }
        }
        public static void WriteLogActivity(string AppPath, string log_content)
        {
            StreamWriter sLogFile = null;
            try
            {
                //Ghi lại hành động của người sử dụng vào log file
                string sDay = string.Format("{0:dd}", DateTime.Now);
                string sMonth = string.Format("{0:MM}", DateTime.Now);
                string strLogFileName = sDay + "-" + sMonth + "-" + DateTime.Now.Year + ".log";
                string strFolderName = AppPath + @"\Logs\" + DateTime.Now.Year + "-" + sMonth;
                //Application.StartupPath
                //Tạo thư mục nếu chưa có
                if (!Directory.Exists(strFolderName + @"\"))
                {
                    Directory.CreateDirectory(strFolderName + @"\");
                }
                strLogFileName = strFolderName + @"\" + strLogFileName;

                if (File.Exists(strLogFileName))
                {
                    //Nếu đã tồn tại file thì tiếp tục ghi thêm
                    sLogFile = File.AppendText(strLogFileName);
                    sLogFile.WriteLine(string.Format("Thời điểm ghi nhận: {0:hh:mm:ss tt}", DateTime.Now));
                    sLogFile.WriteLine(string.Format("Chi tiết log: {0}", log_content));
                    sLogFile.WriteLine("-------------------------------------------");
                    sLogFile.Flush();
                }
                else
                {
                    //Nếu file chưa tồn tại thì có thể tạo mới và ghi log
                    sLogFile = new StreamWriter(strLogFileName);
                    sLogFile.WriteLine(string.Format("Thời điểm ghi nhận: {0:hh:mm:ss tt}", DateTime.Now));
                    sLogFile.WriteLine(string.Format("Chi tiết log: {0}", log_content));
                    sLogFile.WriteLine("-------------------------------------------");
                }
                sLogFile.Close();
            }
            catch (Exception)
            {
                if (sLogFile != null)
                {
                    sLogFile.Close();
                }
            }
        }


    }
}
