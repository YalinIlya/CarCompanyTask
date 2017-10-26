using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebTerminalClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LogWindow LogWnd = new LogWindow();
        static TextBox OutTextBox = null;
        EmvKernelWebRef.EmvWebService EmvKerWebService = new EmvKernelWebRef.EmvWebService();
        SmartCard CardReader = new SmartCard();

        enum MenuType
        {
            Main, Sale, Lang, SelApp, Amount, Wait, Pin,
            HostSwop, Result, Print, Auth, Refund, Batch,
            VoidPswd, OutService, SignOn, Settlement
        };
        MenuType MenuTyp = MenuType.Main;
        Mess Message;
        int page = 0;
        byte emvResult = 0xFF;

        Thread ThreadEmvStart = null;
        Thread ThreadSelLangStart = null;

        public const string fname_Void = "trace\\Void.txt";
        public const string fname_Batch = "trace\\Batch.txt";
        public const string fname_Advice = "trace\\Advice.txt";
        public const string fname_Log = "trace\\EmvWinLog.txt";

        byte[] refundPswdMatch = new byte[4];
        byte[] voidPswdMatch = new byte[4];
        byte[] keyboardBuff = new byte[40];
        int keyboardDataLen = 0;
        string sectedFromMenu = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            Message = new Mess();
            OutTextBox = LogWnd.tbxLog;

            SetText("Application start");

            refundPswdMatch[0] = 1;
            refundPswdMatch[1] = 2;
            refundPswdMatch[2] = 3;
            refundPswdMatch[3] = 4;

            voidPswdMatch[0] = 1;
            voidPswdMatch[1] = 2;
            voidPswdMatch[2] = 3;
            voidPswdMatch[3] = 4;

            EmvKerWebService.EmvRunCompleted += EmvKerWebService_EmvRunCompleted;
            EmvKerWebService.SelLangCompleted += EmvKerWebService_SelLangCompleted;
            CardReader.OnCardIn += EmvKernel_OnCardIn;

            LogWnd.Show();
            ShowMainMenu();
        }

        void EmvKerWebService_SelLangCompleted(object sender, EmvKernelWebRef.SelLangCompletedEventArgs e)
        {
        }

        void EmvKerWebService_EmvRunCompleted(object sender, EmvKernelWebRef.EmvRunCompletedEventArgs e)
        {
            byte funcNum = e.Result;
            switch (funcNum)
            {
                case 0: // SelectLang
                    {
                        ShowSelectLangMenu();
                    } break;
            }
        }

        void EmvKernel_OnCardIn(object sender, EventArgs e)
        {
            ShowWaitMenu();
        }

        void EmvStart()
        {
            if (CardReader.WaitCardIn() == true)
            {
                EmvKerWebService.EmvRunAsync();
            }
        }
        void SelLang(object state)
        {
            string language = (string)state;
            EmvKerWebService.SelLangAsync(language);
        }

        void ShowMainMenuFromRefund()
        {
            MenuTyp = MenuType.Main;
            MenuTitleMaster_Text(Message.MessagesEng[129]);
            MenuTitleSlave_Text(Message.MessagesEng[129]);
            GrKeyBoard_Visibility(System.Windows.Visibility.Hidden);
            GrContent_Visibility(System.Windows.Visibility.Visible);
        }
        void ShowMainMenu()
        {
            MenuTyp = MenuType.Main;
            emvResult = 0xFF;
            MenuTitleMaster_Text(Message.MessagesEng[129]);
            MenuTitleSlave_Text(Message.MessagesEng[129]);
            switch (page)
            {
                case 0:
                    {
                        MenuButton1_Content(Message.MessagesEng[130]);//Sale
                        MenuButton2_Content(Message.MessagesEng[137]);//Auth
                        MenuButton3_Content(Message.MessagesEng[138]);//Sign-on
                        MenuButton4_Content(Message.MessagesEng[20]);//Refund
                        MenuButton5_Content(Message.MessagesEng[92]);//Cancel
                        MenuButton8_Content(Message.MessagesEng[90] + " " + page.ToString());//Next Page

                        MenuButton1_Visibility(System.Windows.Visibility.Visible);
                        MenuButton2_Visibility(System.Windows.Visibility.Visible);
                        MenuButton3_Visibility(System.Windows.Visibility.Visible);
                        MenuButton4_Visibility(System.Windows.Visibility.Visible);
                        MenuButton5_Visibility(System.Windows.Visibility.Visible);
                        MenuButton6_Visibility(System.Windows.Visibility.Hidden);
                        MenuButton7_Visibility(System.Windows.Visibility.Hidden);
                        MenuButton8_Visibility(System.Windows.Visibility.Visible);
                    } break;
                case 1:
                    {
                        MenuButton4_Content(Message.MessagesEng[135]);//Settlement
                        MenuButton5_Content(Message.MessagesEng[92]);//Cancel
                        MenuButton7_Content(Message.MessagesEng[91] + " " + page.ToString());//Previous Page

                        MenuButton1_Visibility(System.Windows.Visibility.Hidden);
                        MenuButton2_Visibility(System.Windows.Visibility.Hidden);
                        MenuButton3_Visibility(System.Windows.Visibility.Hidden);
                        MenuButton4_Visibility(System.Windows.Visibility.Visible);
                        MenuButton5_Visibility(System.Windows.Visibility.Visible);
                        MenuButton6_Visibility(System.Windows.Visibility.Hidden);
                        MenuButton7_Visibility(System.Windows.Visibility.Visible);
                        MenuButton8_Visibility(System.Windows.Visibility.Hidden);
                    } break;
            }
        }
        void ShowBatchMenu()
        {
            MenuTyp = MenuType.Batch;
            byte emvrun_result = 0xFF;
            if (File.Exists(fname_Batch))
            {
                byte[] fileBuf = File.ReadAllBytes(fname_Batch);
                emvrun_result = fileBuf[0];
            }
            if (emvrun_result == 0x10 || emvrun_result == 0x00)
            {
                MenuTitleMaster_Text(Message.MessagesEng[129]);
                MenuTitleSlave_Text(Message.MessagesEng[129]);

                MenuButton1_Content(Message.MessagesEng[131]);//Batch Upload
                MenuButton2_Content(Message.MessagesEng[132]);//Void
                MenuButton5_Content(Message.MessagesEng[92]);//Cancel

                MenuButton1_Visibility(System.Windows.Visibility.Visible);
                MenuButton2_Visibility(System.Windows.Visibility.Visible);
                MenuButton3_Visibility(System.Windows.Visibility.Hidden);
                MenuButton4_Visibility(System.Windows.Visibility.Hidden);
                MenuButton5_Visibility(System.Windows.Visibility.Visible);
                MenuButton6_Visibility(System.Windows.Visibility.Hidden);
                MenuButton7_Visibility(System.Windows.Visibility.Hidden);
                MenuButton8_Visibility(System.Windows.Visibility.Hidden);
            }
            else
            {
                MenuTitleMaster_Text(Message.MessagesEng[129]);
                MenuTitleSlave_Text(Message.MessagesEng[129]);

                MenuButton1_Content(Message.MessagesEng[131]);//Batch Upload
                MenuButton5_Content(Message.MessagesEng[92]);//Cancel

                MenuButton1_Visibility(System.Windows.Visibility.Visible);
                MenuButton2_Visibility(System.Windows.Visibility.Hidden);
                MenuButton3_Visibility(System.Windows.Visibility.Hidden);
                MenuButton4_Visibility(System.Windows.Visibility.Hidden);
                MenuButton5_Visibility(System.Windows.Visibility.Visible);
                MenuButton6_Visibility(System.Windows.Visibility.Hidden);
                MenuButton7_Visibility(System.Windows.Visibility.Hidden);
                MenuButton8_Visibility(System.Windows.Visibility.Hidden);
            }
        }
        void ShowSelectLangMenu()
        {
            MenuTyp = MenuType.Lang;
            MenuTitleMaster_Text(Message.MessagesEng[106]);//SELECT LANGUAGE
            MenuTitleSlave_Text(Message.MessagesMs[106]);//SELECT LANGUAGE

            MenuButton1_Content("Melayu");
            MenuButton2_Content("ENGLISH");
            MenuButton5_Content(Message.MessagesEng[92]);//Cancel

            MenuButton1_Visibility(System.Windows.Visibility.Visible);
            MenuButton2_Visibility(System.Windows.Visibility.Visible);
            MenuButton3_Visibility(System.Windows.Visibility.Hidden);
            MenuButton4_Visibility(System.Windows.Visibility.Hidden);
            MenuButton5_Visibility(System.Windows.Visibility.Visible);
            MenuButton6_Visibility(System.Windows.Visibility.Hidden);
            MenuButton7_Visibility(System.Windows.Visibility.Hidden);
            MenuButton8_Visibility(System.Windows.Visibility.Hidden);
        }
        void ShowWaitCardInMenu()
        {
            txtTitleSlave.Text = txtTitleMaster.Text = Message.MessagesEng[0];
            MenuButton5.Content = Message.MessagesEng[92];//Cancel

            MenuButton1.Visibility = System.Windows.Visibility.Hidden;
            MenuButton2.Visibility = System.Windows.Visibility.Hidden;
            MenuButton3.Visibility = System.Windows.Visibility.Hidden;
            MenuButton4.Visibility = System.Windows.Visibility.Hidden;
            MenuButton5.Visibility = System.Windows.Visibility.Visible;
            MenuButton6.Visibility = System.Windows.Visibility.Hidden;
            MenuButton7.Visibility = System.Windows.Visibility.Hidden;
            MenuButton8.Visibility = System.Windows.Visibility.Hidden;
        }
        void ShowWaitCardOutMenu()
        {
            MenuTitleMaster_Text(Message.MessagesEng[48]);//Transaction Terminated
            MenuTitleSlave_Text("Please take your card");

            MenuButton1_Visibility(System.Windows.Visibility.Hidden);
            MenuButton2_Visibility(System.Windows.Visibility.Hidden);
            MenuButton3_Visibility(System.Windows.Visibility.Hidden);
            MenuButton4_Visibility(System.Windows.Visibility.Hidden);
            MenuButton5_Visibility(System.Windows.Visibility.Hidden);
            MenuButton6_Visibility(System.Windows.Visibility.Hidden);
            MenuButton7_Visibility(System.Windows.Visibility.Hidden);
            MenuButton8_Visibility(System.Windows.Visibility.Hidden);
        }
        void ShowSelectAppMenu(string[] lsAppNames)
        {
            MenuTyp = MenuType.SelApp;
            MenuTitleMaster_Text(Message.MessagesEng[9]);//Select Application:
            MenuTitleSlave_Text(Message.MessagesEng[9]);//Select Application:

            if (lsAppNames[0] != null && lsAppNames[0] != string.Empty)
                MenuButton1_Content(lsAppNames[0]);
            if (lsAppNames[1] != null && lsAppNames[1] != string.Empty)
                MenuButton2_Content(lsAppNames[1]);
            if (lsAppNames[2] != null && lsAppNames[2] != string.Empty)
                MenuButton3_Content(lsAppNames[2]);
            if (lsAppNames[3] != null && lsAppNames[3] != string.Empty)
                MenuButton4_Content(lsAppNames[3]);
            MenuButton5_Content(Message.MessagesEng[92]);//Cancel

            MenuButton1_Visibility(lsAppNames[0] != null && lsAppNames[0] != string.Empty ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
            MenuButton2_Visibility(lsAppNames[1] != null && lsAppNames[1] != string.Empty ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
            MenuButton3_Visibility(lsAppNames[2] != null && lsAppNames[2] != string.Empty ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
            MenuButton4_Visibility(lsAppNames[3] != null && lsAppNames[3] != string.Empty ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
            MenuButton5_Visibility(System.Windows.Visibility.Visible);
            MenuButton6_Visibility(System.Windows.Visibility.Hidden);
            MenuButton7_Visibility(System.Windows.Visibility.Hidden);
            MenuButton8_Visibility(System.Windows.Visibility.Hidden);
        }
        void ShowEnterAmountMenu()
        {
            MenuTyp = MenuType.Amount;
            MenuTitleMaster_Text(Message.MessagesEng[8]);//Select Application:
            MenuTitleSlave_Text("VISA CREDIT");

            MenuButton1_Content("100");
            MenuButton2_Content("200");
            MenuButton3_Content("300");
            MenuButton5_Content(Message.MessagesEng[92]);//Cancel

            MenuButton1_Visibility(System.Windows.Visibility.Visible);
            MenuButton2_Visibility(System.Windows.Visibility.Visible);
            MenuButton3_Visibility(System.Windows.Visibility.Visible);
            MenuButton4_Visibility(System.Windows.Visibility.Hidden);
            MenuButton5_Visibility(System.Windows.Visibility.Visible);
            MenuButton6_Visibility(System.Windows.Visibility.Hidden);
            MenuButton7_Visibility(System.Windows.Visibility.Hidden);
            MenuButton8_Visibility(System.Windows.Visibility.Hidden);
        }
        void ShowWaitMenu()
        {
            MenuTyp = MenuType.Wait;
            MenuTitleMaster_Text(Message.MessagesEng[3]);//Please Wait
            MenuTitleSlave_Text(Message.MessagesEng[3]);//Please Wait

            MenuButton1_Visibility(System.Windows.Visibility.Hidden);
            MenuButton2_Visibility(System.Windows.Visibility.Hidden);
            MenuButton3_Visibility(System.Windows.Visibility.Hidden);
            MenuButton4_Visibility(System.Windows.Visibility.Hidden);
            MenuButton5_Visibility(System.Windows.Visibility.Hidden);
            MenuButton6_Visibility(System.Windows.Visibility.Hidden);
            MenuButton7_Visibility(System.Windows.Visibility.Hidden);
            MenuButton8_Visibility(System.Windows.Visibility.Hidden);
        }
        void ShowEnterPinMenu()
        {
            MenuTyp = MenuType.Pin;
            MenuTitleMaster_Text(Message.MessagesEng[7]);//Enter PIN
            MenuTitleSlave_Text(Message.MessagesEng[7]);//Enter PIN

            //if (EmvKernelDll.TypeOfReader == EmvKernelDll.ReaderType.USB)
            {
                TxtKeyBoardInput_Text(string.Empty);
                GrKeyBoard_Visibility(System.Windows.Visibility.Visible);
                GrContent_Visibility(System.Windows.Visibility.Hidden);
                Array.Clear(keyboardBuff, 0, keyboardBuff.Length);
                keyboardDataLen = 0;
            }
            //else
            //{
            //    MenuButton1_Visibility(System.Windows.Visibility.Hidden);
            //    MenuButton2_Visibility(System.Windows.Visibility.Hidden);
            //    MenuButton3_Visibility(System.Windows.Visibility.Hidden);
            //    MenuButton4_Visibility(System.Windows.Visibility.Hidden);
            //    MenuButton5_Visibility(System.Windows.Visibility.Hidden);
            //    MenuButton6_Visibility(System.Windows.Visibility.Hidden);
            //    MenuButton7_Visibility(System.Windows.Visibility.Hidden);
            //    MenuButton8_Visibility(System.Windows.Visibility.Hidden);
            //}
        }
        void ShowHostSwopMenu()
        {
            MenuTyp = MenuType.HostSwop;
            MenuTitleMaster_Text(Message.MessagesEng[1]);//Goes Swap Data
            MenuTitleSlave_Text(Message.MessagesEng[1]);//Goes Swap Data

            MenuButton1_Visibility(System.Windows.Visibility.Hidden);
            MenuButton2_Visibility(System.Windows.Visibility.Hidden);
            MenuButton3_Visibility(System.Windows.Visibility.Hidden);
            MenuButton4_Visibility(System.Windows.Visibility.Hidden);
            MenuButton5_Visibility(System.Windows.Visibility.Hidden);
            MenuButton6_Visibility(System.Windows.Visibility.Hidden);
            MenuButton7_Visibility(System.Windows.Visibility.Hidden);
            MenuButton8_Visibility(System.Windows.Visibility.Hidden);
        }
        void ShowResultMenu(byte result)
        {
            MenuTyp = MenuType.Result;
            switch (result)
            {
                case 0x00:
                    {
                        MenuTitleMaster_Text(Message.MessagesEng[49]);//Transaction Approved
                    } break;
                case 0x10:
                    {
                        MenuTitleMaster_Text(Message.MessagesEng[49]);//Transaction Approved
                    } break;
                case 0x02:
                    {
                        MenuTitleMaster_Text(Message.MessagesEng[40]);//Transaction Declined
                    } break;
                case 0x12:
                    {
                        MenuTitleMaster_Text(Message.MessagesEng[40]);//Transaction Declined
                    } break;
                case 0x13://unable to go online
                    {
                        MenuTitleMaster_Text(Message.MessagesEng[40]);//Transaction Declined
                    } break;
                case 0x03: ShowMainMenu(); return;
                case 0x55: ShowMainMenu(); return;
                case 0x66: ShowMainMenu(); return;
                default:
                    {
                        //MenuTitleMaster_Text("EMV result = " + result.ToString());
                    } break;
            }
            MenuTitleSlave_Text("Please take your card");

            MenuButton1_Visibility(System.Windows.Visibility.Hidden);
            MenuButton2_Visibility(System.Windows.Visibility.Hidden);
            MenuButton3_Visibility(System.Windows.Visibility.Hidden);
            MenuButton4_Visibility(System.Windows.Visibility.Hidden);
            MenuButton5_Visibility(System.Windows.Visibility.Hidden);
            MenuButton6_Visibility(System.Windows.Visibility.Hidden);
            MenuButton7_Visibility(System.Windows.Visibility.Hidden);
            MenuButton8_Visibility(System.Windows.Visibility.Hidden);
        }
        void ShowPrintMenu()
        {
            MenuTyp = MenuType.Print;

            MenuTitleMaster_Text(Message.MessagesEng[98]);//Please take your receipt...
            MenuTitleSlave_Text("");

            MenuButton5_Content(Message.MessagesEng[125]);//Print
            MenuButton7_Content(Message.MessagesEng[92]);//Cancel

            MenuButton1_Visibility(System.Windows.Visibility.Hidden);
            MenuButton2_Visibility(System.Windows.Visibility.Hidden);
            MenuButton3_Visibility(System.Windows.Visibility.Hidden);
            MenuButton4_Visibility(System.Windows.Visibility.Hidden);
            MenuButton5_Visibility(System.Windows.Visibility.Visible);
            MenuButton6_Visibility(System.Windows.Visibility.Hidden);
            MenuButton7_Visibility(System.Windows.Visibility.Visible);
            MenuButton8_Visibility(System.Windows.Visibility.Hidden);
        }
        void ShowKeyBoardMenu()
        {
            MenuTitleMaster_Text(Message.MessagesEng[141]);
            MenuTitleSlave_Text(Message.MessagesEng[141]);

            txtKeyBoardInput.Text = string.Empty;
            GrKeyBoard_Visibility(System.Windows.Visibility.Visible);
            GrContent_Visibility(System.Windows.Visibility.Hidden);
            Array.Clear(keyboardBuff, 0, keyboardBuff.Length);
            keyboardDataLen = 0;
        }
        void ShowOutOfServiceMenu()
        {
            MenuTyp = MenuType.OutService;

            MenuTitleMaster_Text(Message.MessagesEng[114]);//Terminal Is Out Of Service
            MenuTitleSlave_Text(Message.MessagesEng[114]);//Terminal Is Out Of Service

            MenuButton5_Content(Message.MessagesEng[92]);//Cancel

            MenuButton1_Visibility(System.Windows.Visibility.Hidden);
            MenuButton2_Visibility(System.Windows.Visibility.Hidden);
            MenuButton3_Visibility(System.Windows.Visibility.Hidden);
            MenuButton4_Visibility(System.Windows.Visibility.Hidden);
            MenuButton5_Visibility(System.Windows.Visibility.Visible);
            MenuButton6_Visibility(System.Windows.Visibility.Hidden);
            MenuButton7_Visibility(System.Windows.Visibility.Hidden);
            MenuButton8_Visibility(System.Windows.Visibility.Hidden);
        }

        private void MenuButton1_Click(object sender, RoutedEventArgs e)//Enter
        {
            switch (MenuTyp)
            {
                case MenuType.Main:
                    {
                        MenuTyp = MenuType.Sale;
                        sectedFromMenu = "SALE";
                        ShowWaitCardInMenu();
                        ThreadEmvStart = new Thread(EmvStart);
                        ThreadEmvStart.Start();
                    } break;
                case MenuType.Lang:
                    {
                        ShowWaitMenu();
                        string language = "MS";
                        ThreadSelLangStart = new Thread(SelLang);
                        ThreadSelLangStart.Start(language);
                    } break;
            }
        }
        private void MenuButton2_Click(object sender, RoutedEventArgs e)
        {
            switch (MenuTyp)
            {
                case MenuType.Main:
                    {
                    } break;
                case MenuType.Lang:
                    {
                        ShowWaitMenu();
                        string language = "EN";
                        ThreadSelLangStart = new Thread(SelLang);
                        ThreadSelLangStart.Start(language);
                    } break;
            }
        }
        private void MenuButton3_Click(object sender, RoutedEventArgs e)
        {
        }
        private void MenuButton4_Click(object sender, RoutedEventArgs e)
        {            
        }
        private void MenuButton5_Click(object sender, RoutedEventArgs e)//Cancel
        {
            switch (MenuTyp)
            {
                case MenuType.Main:
                    {
                        this.Close();
                    } break;
                case MenuType.Sale:
                    {
                        ShowMainMenu();
                    } break;
            }
        }
        private void MenuButton7_Click(object sender, RoutedEventArgs e)
        {            
        }
        private void MenuButton8_Click(object sender, RoutedEventArgs e)
        {
        }

        delegate void SetTextCallback(string text);
        static void SetText(string text)
        {
            if (!OutTextBox.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(SetText);
                OutTextBox.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                OutTextBox.AppendText(text);
                OutTextBox.ScrollToEnd();
                File.AppendAllText(fname_Log, text);
            }
        }
        void MenuTitleMaster_Text(string text)
        {
            if (!txtTitleMaster.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(MenuTitleMaster_Text);
                txtTitleMaster.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                txtTitleMaster.Text = text;
            }
        }
        void MenuTitleSlave_Text(string text)
        {
            if (!txtTitleSlave.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(MenuTitleSlave_Text);
                txtTitleSlave.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                txtTitleSlave.Text = text;
            }
        }
        void TxtKeyBoardInput_Text(string text)
        {
            if (!txtKeyBoardInput.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(TxtKeyBoardInput_Text);
                txtKeyBoardInput.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                txtKeyBoardInput.Text = text;
            }
        }
        void TxtKeyBoardInput_AddText(string text)
        {
            if (!txtKeyBoardInput.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(TxtKeyBoardInput_AddText);
                txtKeyBoardInput.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                txtKeyBoardInput.Text += text;
            }
        }
        void MenuButton1_Content(string text)
        {
            if (!MenuButton1.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(MenuButton1_Content);
                MenuButton1.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                MenuButton1.Content = text;
            }
        }
        void MenuButton2_Content(string text)
        {
            if (!MenuButton2.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(MenuButton2_Content);
                MenuButton2.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                MenuButton2.Content = text;
            }
        }
        void MenuButton3_Content(string text)
        {
            if (!MenuButton3.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(MenuButton3_Content);
                MenuButton3.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                MenuButton3.Content = text;
            }
        }
        void MenuButton4_Content(string text)
        {
            if (!MenuButton4.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(MenuButton4_Content);
                MenuButton4.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                MenuButton4.Content = text;
            }
        }
        void MenuButton5_Content(string text)
        {
            if (!MenuButton5.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(MenuButton5_Content);
                MenuButton5.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                MenuButton5.Content = text;
            }
        }
        void MenuButton6_Content(string text)
        {
            if (!MenuButton6.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(MenuButton6_Content);
                MenuButton6.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                MenuButton6.Content = text;
            }
        }
        void MenuButton7_Content(string text)
        {
            if (!MenuButton7.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(MenuButton7_Content);
                MenuButton7.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                MenuButton7.Content = text;
            }
        }
        void MenuButton8_Content(string text)
        {
            if (!MenuButton8.Dispatcher.CheckAccess())
            {
                SetTextCallback d = new SetTextCallback(MenuButton8_Content);
                MenuButton8.Dispatcher.BeginInvoke(d, new object[] { text });
            }
            else
            {
                MenuButton8.Content = text;
            }
        }

        delegate void SetVisibilityCallBack(Visibility visible);
        void MenuButton1_Visibility(Visibility visible)
        {
            if (!MenuButton1.Dispatcher.CheckAccess())
            {
                SetVisibilityCallBack d = new SetVisibilityCallBack(MenuButton1_Visibility);
                MenuButton1.Dispatcher.BeginInvoke(d, new object[] { visible });
            }
            else
            {
                MenuButton1.Visibility = visible;
            }
        }
        void MenuButton2_Visibility(Visibility visible)
        {
            if (!MenuButton2.Dispatcher.CheckAccess())
            {
                SetVisibilityCallBack d = new SetVisibilityCallBack(MenuButton2_Visibility);
                MenuButton2.Dispatcher.BeginInvoke(d, new object[] { visible });
            }
            else
            {
                MenuButton2.Visibility = visible;
            }
        }
        void MenuButton3_Visibility(Visibility visible)
        {
            if (!MenuButton3.Dispatcher.CheckAccess())
            {
                SetVisibilityCallBack d = new SetVisibilityCallBack(MenuButton3_Visibility);
                MenuButton3.Dispatcher.BeginInvoke(d, new object[] { visible });
            }
            else
            {
                MenuButton3.Visibility = visible;
            }
        }
        void MenuButton4_Visibility(Visibility visible)
        {
            if (!MenuButton4.Dispatcher.CheckAccess())
            {
                SetVisibilityCallBack d = new SetVisibilityCallBack(MenuButton4_Visibility);
                MenuButton4.Dispatcher.BeginInvoke(d, new object[] { visible });
            }
            else
            {
                MenuButton4.Visibility = visible;
            }
        }
        void MenuButton5_Visibility(Visibility visible)
        {
            if (!MenuButton5.Dispatcher.CheckAccess())
            {
                SetVisibilityCallBack d = new SetVisibilityCallBack(MenuButton5_Visibility);
                MenuButton5.Dispatcher.BeginInvoke(d, new object[] { visible });
            }
            else
            {
                MenuButton5.Visibility = visible;
            }
        }
        void MenuButton6_Visibility(Visibility visible)
        {
            if (!MenuButton6.Dispatcher.CheckAccess())
            {
                SetVisibilityCallBack d = new SetVisibilityCallBack(MenuButton6_Visibility);
                MenuButton6.Dispatcher.BeginInvoke(d, new object[] { visible });
            }
            else
            {
                MenuButton6.Visibility = visible;
            }
        }
        void MenuButton7_Visibility(Visibility visible)
        {
            if (!MenuButton7.Dispatcher.CheckAccess())
            {
                SetVisibilityCallBack d = new SetVisibilityCallBack(MenuButton7_Visibility);
                MenuButton7.Dispatcher.BeginInvoke(d, new object[] { visible });
            }
            else
            {
                MenuButton7.Visibility = visible;
            }
        }
        void MenuButton8_Visibility(Visibility visible)
        {
            if (!MenuButton8.Dispatcher.CheckAccess())
            {
                SetVisibilityCallBack d = new SetVisibilityCallBack(MenuButton8_Visibility);
                MenuButton8.Dispatcher.BeginInvoke(d, new object[] { visible });
            }
            else
            {
                MenuButton8.Visibility = visible;
            }
        }
        void GrKeyBoard_Visibility(Visibility visible)
        {
            if (!grKeyBoard.Dispatcher.CheckAccess())
            {
                SetVisibilityCallBack d = new SetVisibilityCallBack(GrKeyBoard_Visibility);
                grKeyBoard.Dispatcher.BeginInvoke(d, new object[] { visible });
            }
            else
            {
                grKeyBoard.Visibility = visible;
            }
        }
        void GrContent_Visibility(Visibility visible)
        {
            if (!grContent.Dispatcher.CheckAccess())
            {
                SetVisibilityCallBack d = new SetVisibilityCallBack(GrContent_Visibility);
                grContent.Dispatcher.BeginInvoke(d, new object[] { visible });
            }
            else
            {
                grContent.Visibility = visible;
            }
        }

        private void KeyboardButton0_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardDataLen < 4)
            {
                TxtKeyBoardInput_AddText("*");
                keyboardDataLen++;
                keyboardBuff[keyboardDataLen - 1] = 0;
            }
            else return;
        }
        private void KeyboardButton1_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardDataLen < 4)
            {
                TxtKeyBoardInput_AddText("*");
                keyboardDataLen++;
                keyboardBuff[keyboardDataLen - 1] = 1;
            }
            else return;
        }
        private void KeyboardButton2_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardDataLen < 4)
            {
                TxtKeyBoardInput_AddText("*");
                keyboardDataLen++;
                keyboardBuff[keyboardDataLen - 1] = 2;
            }
            else return;
        }
        private void KeyboardButton3_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardDataLen < 4)
            {
                TxtKeyBoardInput_AddText("*");
                keyboardDataLen++;
                keyboardBuff[keyboardDataLen - 1] = 3;
            }
            else return;
        }
        private void KeyboardButton4_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardDataLen < 4)
            {
                TxtKeyBoardInput_AddText("*");
                keyboardDataLen++;
                keyboardBuff[keyboardDataLen - 1] = 4;
            }
            else return;
        }
        private void KeyboardButton5_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardDataLen < 4)
            {
                TxtKeyBoardInput_AddText("*");
                keyboardDataLen++;
                keyboardBuff[keyboardDataLen - 1] = 5;
            }
            else return;
        }
        private void KeyboardButton6_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardDataLen < 4)
            {
                TxtKeyBoardInput_AddText("*");
                keyboardDataLen++;
                keyboardBuff[keyboardDataLen - 1] = 6;
            }
            else return;
        }
        private void KeyboardButton7_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardDataLen < 4)
            {
                TxtKeyBoardInput_AddText("*");
                keyboardDataLen++;
                keyboardBuff[keyboardDataLen - 1] = 7;
            }
            else return;
        }
        private void KeyboardButton8_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardDataLen < 4)
            {
                TxtKeyBoardInput_AddText("*");
                keyboardDataLen++;
                keyboardBuff[keyboardDataLen - 1] = 8;
            }
            else return;
        }
        private void KeyboardButton9_Click(object sender, RoutedEventArgs e)
        {
            if (keyboardDataLen < 4)
            {
                TxtKeyBoardInput_AddText("*");
                keyboardDataLen++;
                keyboardBuff[keyboardDataLen - 1] = 9;
            }
            else return;
        }
        private void KeyboardButton10_Click(object sender, RoutedEventArgs e)//Enter
        {
            
        }
    }
}
