using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using YalkoEmvClassLibrary;

namespace WebTerminalServer
{
    public class EMVLibraryDelegate : YalkoEmvClass, YalkoEmvClass.EMVLibraryDelegate
    {
        public string language = string.Empty;

        public string LogFileName = string.Empty;
        public ManualResetEvent mre = null;
        public ManualResetEvent mreWait = null;
        public FuncNumber funcNum;

        static string CardSimulator_Approve_two_app(string sCmd, string sData)
        {
            if (sData.Length > 0)//&& apdu_data_len < sizeof(sData)/2)
            {
                if (sCmd.Equals("00A404000E") && sData.Equals("315041592E5359532E4444463031"))
                {
                    return "6A82";
                }
                else
                    if (sCmd.Equals("00A4040007") && sData.Equals("A0000000031010"))
                    {
                        //return "7081B39081B0A5161387F9B37986F83F84B3C713FB988032439BDCD2CC2CC106D64DC53802D0C20CA3972A00EE7C954F7CFE66411127C83A322AB312E6B0DA64ACA1551C5479B88CCB9FD8A43F819E20A9F97BB46F5A3DB2A15D02A6F048209C41435490447863705C1E9CA9AF42248F17995479E0305A0985C4E0AB98E4E8D320C911841EB8BEDBED8D4BA3CEFC62A7D433AEA35A19F4BDAF9822FE16B994ABEB0B5BB8FE037FBBA2E3366B7ECB2FA12670BF7A183B9000" ;
                        return "6F318407A0000000031010A526500B56495341204352454449549F120F4352454449544F20444520564953418701019F1101019000";
                    }
                    else
                        if (sCmd.Equals("00A4040007") && sData.Equals("A0000000041010"))
                        {
                            return "6F318407A0000000041010A526500B4D415354455220434152449F120F4452454449544F20444520564953418701019F1101019000";
                        }
                        else

                            if (sCmd.Equals("80A8000002") && sData.Equals("8300"))
                            {
                                return "800E5C000801010010010300180102019000";
                            }
                            else
                                if (sCmd.Equals("80AE80001D"))// && sData.Equals("00000001000000000000000008404000048000064313083100E9C4B090"))
                                {
                                    return "80128000018B5911CDEBC190F506010A039000009000";
                                }
                                else
                                    if (sCmd.Equals("0020008008"))
                                    {
                                        return "9000";
                                    }
                                    else
                                        if (sCmd.Equals("008200000A"))// && sData.Equals("993680ED37FC09533030"))
                                        {
                                            return "9000";
                                        }
                                        else
                                            if (sCmd.Equals("80AE40001F"))// && sData.Equals("3030000000010000000000000000084040000480000643130831006D4B0149"))
                                            {
                                                return "8012400001492136F956F8B83006010A036000009000";
                                            }
                                            else
                                                if (sCmd.Equals("80AE00001F"))// && sData.Equals("3030000000010000000000000000084040000480000643130831006D4B0149"))
                                                {
                                                    return "8012000001492136F956F8B83006010A036000009000";
                                                }
                                                else
                                                    if (sCmd.Substring(0, 8).Equals("00A404000"))
                                                    {
                                                        return "6A82";
                                                    }
                                                    else
                                                    {
                                                        //Log.i("Emv", "CardSimulator ERROR! No Equals for command = " + sCmd +" data = " + sData);
                                                    }

            }
            else
            {
                if (sCmd.Equals("00B2010C00"))
                {
                    return "70435F201A564953412041435155495245522054455354204341524420303157114761739001010010D101220111438780899F1F10313134333830303738303030303030309000";
                }
                else
                    if (sCmd.Equals("00B2011400"))
                    {
                        return "7081939081908B3901F6253048A8B2CB08974A4245D90E1F0C4A2A69BCA469615A71DB21EE7B3AA94200CFAEDCD6F0A7D9AD0BF79213B6A418D7A49D234E5C9715C9140D87940F2E04D6971F4A204C927A455D4F8FC0D6402A79A1CE05AA3A526867329853F5AC2FEB3C6F59FF6C453A7245E39D73451461725795ED73097099963B82EBF7203C1F78A529140C182DBBE6B42AE00C029000";
                    }
                    else
                        if (sCmd.Equals("00B2021400"))
                        {
                            return "702D8F01959F320103922433F5E4447D4A32E5936E5A1339329BB4E8DD8BF0044CE4428E24D0866FAEFD2348809D719000";
                        }
                        else
                            if (sCmd.Equals("00B2031400"))
                            {
                                return "708193938190919D6C210B3981D1C99B3AD55EDF36A138FFAD54D838FA40622AB97046E05EA6E6230AB89D5BE871114EB5431B97403B8C3D2D4CA9BB625AC13FD8C6B825433656CB56557AAC396D945F6D4014FB6E71E8DBEA74B285E9CF3FCEABDFA61D5A4BE16DAAA433F7F2644B178A7DD93DA98BB9D10E84298BDB6B6AE02D04E6E5558C77E79F82C9E046DF821DD0277AB9D00C9000";
                            }
                            else
                                if (sCmd.Equals("00B2011C00"))
                                {
                                    return "700E5A0847617390010100105F3401019000";
                                }
                                else
                                    if (sCmd.Equals("00B2021C00"))
                                    {
                                        return "707D9F420208405F25039507015F24031012319F0802008C5F280208405F300202019F0702FF008E0E000000000000000041031E031F008C159F02069F03069F1A0295055F2A029A039C019F37048D178A029F02069F03069F1A0295055F2A029A039C019F37049F0D05F0400088009F0E0500100000009F0F05F0400098009000";
                                    }
                                    else
                                    {
                                        //Log.i("Emv", "CardSimulator ERROR! No Equals for command = " + sCmd);
                                    }
            }
            return "";
        }
        static string CardSimulator(string sCmd, string sData)
        {
            //Thread.Sleep(1000);
            return CardSimulator_Approve_two_app(sCmd, sData);
            //return CardSimulator_OffLine_and_OnLine_Pin(sCmd, sData);
        }

        public string fApdu_Delegate(int lpHandle, string sApdu)
        {
            if (sApdu.Length > 9)
            {
                if (sApdu.Length > 10)
                {
                    string sApdu_cmd = sApdu.Substring(0, 10);
                    string sApdu_data = sApdu.Substring(10, sApdu.Length - 10);
                    string str = CardSimulator(sApdu_cmd, sApdu_data);

                    return str;
                }
                else
                {
                    string sApdu_cmd = sApdu.Substring(0, 10);
                    string str = CardSimulator(sApdu_cmd, "");

                    return str;
                }
            }
            else
            {
                return null;
            }
        }
        public void fAppSelected() { }
        public unsafe byte fAtr_Delegate(int lpHandle, ref string atr, byte* protocol)
        {
            atr = "3B620000811F";
            *protocol = 0;//0 => T0, 1 => T1
            return 1;//ok
        }
        public void fEmvEnd_Delegate(int lpHandle) { }
        public void fEmvTrace(int lpHandle, string s) 
        {
            File.AppendAllText(LogFileName, s);
        }
        public byte fGetCryptPin_Delegate(int lpHandle, ref string pin_data) { return 0; }
        public byte fGetPin_Delegate(int lpHandle, byte FlagTryAgain, ref string pin_data, byte min, byte max) { return 0; }
        public byte fGetTrData_Delegate(int lpHandle, ref string amount_6_bytes, ref string amount_other_6_bytes, ref string currency_2_bytes, ref string operation_type_1_byte, ref string pin_entry_statys_1_byte) { return 0; }
        public byte fHostSwop_Delegate(int lpHandle, string data_after_GAC1, ref string host_answer, ref string host_answer71, ref string host_answer72) { return 0; }
        public byte fSelApp_Delegate(int lpHandle, byte flagRetry, byte[] code_table, string[] AppList, string[] AidList) { return 0; }
        public byte fSelectAccount_Delegate(int lpHandle) { return 0; }
        public byte fSelLang_Delegate(int lpHandle, ref string lang)
        {
            funcNum = FuncNumber.SelLang;
            mre.Set();
            mreWait.WaitOne();
            mreWait.Reset();
            lang = language;
            return 0;
        }
    }
}