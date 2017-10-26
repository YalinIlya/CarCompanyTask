using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace WebTerminalServer
{
    public class EmvKernelSession
    {   
        EMVLibraryDelegate emv_delegate;
        public ManualResetEvent mre;
        public ManualResetEvent mreWait;
        public YalkoEmvClassLibrary.YalkoEmvClass EmvKernel = new YalkoEmvClassLibrary.YalkoEmvClass();
        public Thread ThreadEmvStart = null;
        public byte emv_result;

        public FuncNumber FuncNum
        {
            get { return emv_delegate.funcNum; }
        }
        public string Language
        {
            set { emv_delegate.language = value; }
        }

        void EmvInitTerminalParam()
        {
            string currentDirectory = @"E:\ProjectsYalko\WebTerminal\WebTerminalServer";//Directory.GetCurrentDirectory();
            string dir_path = currentDirectory + "\\EmvParams\\";
            string fpath = dir_path
                     + "TrParam.txt";

            SetLog("read file : " + fpath);

            try
            {
                if (Directory.Exists(dir_path))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(fpath))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (EmvKernel.EmvInitTerminalParam(line) == 0)//ok
                                {
                                    SetLog("Ok: " + line);
                                }
                                else
                                {
                                    SetLog("Err: " + line);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        SetLog("The file could not be read:");
                        SetLog(e.Message);
                    }
                }
                else
                {
                    SetLog("The folder is not exist : " + dir_path);
                    SetLog("Can't create file : " + fpath);
                }
            }
            catch (System.Exception ex)
            {
                SetLog("Can not to create file : " + fpath + " Exception=" + ex.ToString());
                SetLog("Can not to create file : " + fpath + " Exception=" + ex.ToString());
            }
        }
        void EmvInitPublicKeys()
        {
            string currentDirectory = @"E:\ProjectsYalko\WebTerminal\WebTerminalServer";//Directory.GetCurrentDirectory();
            string dir_path = currentDirectory + "\\EmvParams\\";
            string fpath = dir_path
                     + "PublicKeys.txt";

            SetLog("read file : " + fpath);

            try
            {
                if (Directory.Exists(dir_path))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(fpath))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (EmvKernel.EmvInitTerminalParam(line) == 0)
                                {
                                    SetLog("Ok: " + line);
                                }
                                else
                                {
                                    SetLog("Err: " + line);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        SetLog("The file could not be read:");
                        SetLog(e.Message);
                    }
                }
                else
                {
                    SetLog("The folder is not exist : " + dir_path);
                    SetLog("Can't create file : " + fpath);
                }
            }
            catch (System.Exception ex)
            {
                SetLog("Can not to create file : " + fpath + " Exception=" + ex.ToString());
                SetLog("Can not to create file : " + fpath + " Exception=" + ex.ToString());
            }
        }
        void EmvInitRevocationList()
        {
            string currentDirectory = @"E:\ProjectsYalko\WebTerminal\WebTerminalServer";//Directory.GetCurrentDirectory();
            string dir_path = currentDirectory + "\\EmvParams\\";
            string fpath = dir_path
                     + "RevocationList.txt";

            SetLog("read file : " + fpath);

            try
            {
                if (Directory.Exists(dir_path))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(fpath))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (EmvKernel.EmvInitTerminalParam(line) == 0)//ok
                                {
                                    SetLog("Ok: " + line);
                                }
                                else
                                {
                                    SetLog("Err: " + line);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        SetLog("The file could not be read:");
                        SetLog(e.Message);
                    }
                }
                else
                {
                    SetLog("The folder is not exist : " + dir_path);
                    SetLog("Can't create file : " + fpath);
                }
            }
            catch (System.Exception ex)
            {
                SetLog("Can not to create file : " + fpath + " Exception=" + ex.ToString());
                SetLog("Can not to create file : " + fpath + " Exception=" + ex.ToString());
            }
        }

        public void Run()
        {
            ThreadEmvStart = new Thread(EmvStart);
            ThreadEmvStart.Start();
        }

        void EmvStart()
        {
            EmvInitTerminalParam();
            EmvInitPublicKeys();
            EmvInitRevocationList();

            emv_delegate = new EMVLibraryDelegate();
            emv_delegate.mreWait = mreWait;
            emv_delegate.mre = mre;

            DateTime dateTime = DateTime.Now;
            emv_delegate.LogFileName = string.Format(@"E:\ProjectsYalko\WebTerminal\WebTerminalServer\EmvLogs\EmvLogs_{0}_{1}_{2}.txt",
                dateTime.Hour.ToString(), dateTime.Minute.ToString(), dateTime.Second.ToString());

            emv_result = EmvKernel.EmvRun(emv_delegate);
        }

        public void SetLog(string str)
        { }
    }
}