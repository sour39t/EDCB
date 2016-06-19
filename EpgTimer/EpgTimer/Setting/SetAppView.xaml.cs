﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Interop;

namespace EpgTimer.Setting
{
    using BoxExchangeEdit;

    /// <summary>
    /// SetAppView.xaml の相互作用ロジック
    /// </summary>
    public partial class SetAppView : UserControl
    {
        private MenuUtil mutil = CommonManager.Instance.MUtil;

        private List<String> ngProcessList = new List<String>();
        private String ngMin = "10";
        public bool ngUsePC = false;
        public String ngUsePCMin = "3";
        public bool ngFileStreaming = false;
        public bool ngShareFile = false;

        private EpgSearchKeyInfo defSearchKey;

        private List<String> extList = new List<string>();
        private List<String> delChkFolderList = new List<string>();

        private List<string> buttonItem;
        private List<string> taskItem;
        
        private Dictionary<UInt64, ServiceViewItem> serviceList = new Dictionary<UInt64, ServiceViewItem>();
        private List<IEPGStationInfo> stationList = new List<IEPGStationInfo>();

        public SetAppView()
        {
            InitializeComponent();

            if (CommonManager.Instance.NWMode == true)
            {
                tabItem1.Foreground = Brushes.Gray;
                groupBox1.Foreground = Brushes.Gray;
                radioButton_none.IsEnabled = false;
                radioButton_standby.IsEnabled = false;
                radioButton_suspend.IsEnabled = false;
                radioButton_shutdown.IsEnabled = false;
                checkBox_reboot.IsEnabled = false;
                label1.IsEnabled = false;
                label4.IsEnabled = false;
                textBox_pcWakeTime.IsEnabled = false;
                label2.IsEnabled = false;
                label5.IsEnabled = false;
                ViewUtil.DisableControlChildren(groupBox2);

                checkBox_back_priority.IsEnabled = false;
                checkBox_autoDel.IsEnabled = false;
                checkBox_recname.IsEnabled = false;
                comboBox_recname.IsEnabled = false;
                button_recname.IsEnabled = false;

                tabControl1.SelectedItem = tabItem2;
                checkBox_tcpServer.IsEnabled = false;
                label41.IsEnabled = false;
                textBox_tcpPort.IsEnabled = false;
                label_tcpAcl.IsEnabled = false;
                textBox_tcpAcl.IsEnabled = false;
                label_tcpResTo.IsEnabled = false;
                textBox_tcpResTo.IsEnabled = false;
                checkBox_autoDelRecInfo.IsEnabled = false;
                checkBox_autoDelRecFile.IsEnabled = false;
                label42.IsEnabled = false;
                textBox_autoDelRecInfo.IsEnabled = false;
                stackPanel_timeSync.IsEnabled = false;
                checkBox_wakeReconnect.IsEnabled = true;
                stackPanel_WoLWait.IsEnabled = true;
                checkBox_suspendClose.IsEnabled = true;
                checkBox_ngAutoEpgLoad.IsEnabled = true;
                checkBox_keepTCPConnect.IsEnabled = true;
                checkBox_srvResident.IsEnabled = false;
                checkBox_noChkYen.IsEnabled = false;
                checkBox_srvSaveNotifyLog.IsEnabled = false;
                checkBox_srvSaveDebugLog.IsEnabled = false;
                button_recDef.Content = "録画プリセットを確認";
            }

            try
            {
                int recEndMode = IniFileHandler.GetPrivateProfileInt("SET", "RecEndMode", 2, SettingPath.TimerSrvIniPath);
                switch (recEndMode)
                {
                    case 0:
                        radioButton_none.IsChecked = true;
                        break;
                    case 1:
                        radioButton_standby.IsChecked = true;
                        break;
                    case 2:
                        radioButton_suspend.IsChecked = true;
                        break;
                    case 3:
                        radioButton_shutdown.IsChecked = true;
                        break;
                    default:
                        break;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "Reboot", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_reboot.IsChecked = true;
                }
                else
                {
                    checkBox_reboot.IsChecked = false;
                }
                textBox_pcWakeTime.Text = IniFileHandler.GetPrivateProfileInt("SET", "WakeTime", 5, SettingPath.TimerSrvIniPath).ToString();

                textBox_megine_start.Text = IniFileHandler.GetPrivateProfileInt("SET", "StartMargin", 5, SettingPath.TimerSrvIniPath).ToString();
                textBox_margine_end.Text = IniFileHandler.GetPrivateProfileInt("SET", "EndMargin", 2, SettingPath.TimerSrvIniPath).ToString();
                textBox_appWakeTime.Text = IniFileHandler.GetPrivateProfileInt("SET", "RecAppWakeTime", 2, SettingPath.TimerSrvIniPath).ToString();

                if (IniFileHandler.GetPrivateProfileInt("SET", "RecMinWake", 1, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_appMin.IsChecked = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "RecView", 1, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_appView.IsChecked = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "DropLog", 1, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_appDrop.IsChecked = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "PgInfoLog", 1, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_addPgInfo.IsChecked = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "RecNW", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_appNW.IsChecked = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "KeepDisk", 1, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_appKeepDisk.IsChecked = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "RecOverWrite", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_appOverWrite.IsChecked = true;
                }

                int ngCount = IniFileHandler.GetPrivateProfileInt("NO_SUSPEND", "Count", 0, SettingPath.TimerSrvIniPath);
                if (ngCount == 0)
                {
                    ngProcessList.Add("EpgDataCap_Bon.exe");
                }
                else
                {
                    for (int i = 0; i < ngCount; i++)
                    {
                        ngProcessList.Add(IniFileHandler.GetPrivateProfileString("NO_SUSPEND", i.ToString(), "", SettingPath.TimerSrvIniPath));
                    }
                }
                ngMin = IniFileHandler.GetPrivateProfileString("NO_SUSPEND", "NoStandbyTime", "10", SettingPath.TimerSrvIniPath);
                if (IniFileHandler.GetPrivateProfileInt("NO_SUSPEND", "NoUsePC", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    ngUsePC = true;
                }
                ngUsePCMin = IniFileHandler.GetPrivateProfileString("NO_SUSPEND", "NoUsePCTime", "3", SettingPath.TimerSrvIniPath);
                if (IniFileHandler.GetPrivateProfileInt("NO_SUSPEND", "NoFileStreaming", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    ngFileStreaming = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("NO_SUSPEND", "NoShareFile", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    ngShareFile = true;
                }

                comboBox_process.Items.Add("リアルタイム");
                comboBox_process.Items.Add("高");
                comboBox_process.Items.Add("通常以上");
                comboBox_process.Items.Add("通常");
                comboBox_process.Items.Add("通常以下");
                comboBox_process.Items.Add("低");
                comboBox_process.SelectedIndex = IniFileHandler.GetPrivateProfileInt("SET", "ProcessPriority", 3, SettingPath.TimerSrvIniPath);

                //2
                if (IniFileHandler.GetPrivateProfileInt("SET", "BackPriority", 1, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_back_priority.IsChecked = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "AutoDel", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_autoDel.IsChecked = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "RecNamePlugIn", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_recname.IsChecked = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "NoChkYen", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_noChkYen.IsChecked = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "AutoDelRecInfo", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_autoDelRecInfo.IsChecked = true;
                }
                if (IniFileHandler.GetPrivateProfileInt("SET", "RecInfoDelFile", 0, SettingPath.CommonIniPath) == 1)
                {
                    checkBox_autoDelRecFile.IsChecked = true;
                }
                textBox_autoDelRecInfo.Text = IniFileHandler.GetPrivateProfileInt("SET", "AutoDelRecInfoNum", 100, SettingPath.TimerSrvIniPath).ToString();

                if (IniFileHandler.GetPrivateProfileInt("SET", "TimeSync", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_timeSync.IsChecked = true;
                }

                String plugInFile = IniFileHandler.GetPrivateProfileString("SET", "RecNamePlugInFile", "RecName_Macro.dll", SettingPath.TimerSrvIniPath);

                checkBox_cautionOnRecChange.IsChecked = Settings.Instance.CautionOnRecChange;
                textBox_cautionOnRecMarginMin.Text = Settings.Instance.CautionOnRecMarginMin.ToString();

                //4
                checkBox_closeMin.IsChecked = Settings.Instance.CloseMin;
                checkBox_minWake.IsChecked = Settings.Instance.WakeMin;
                checkBox_noToolTips.IsChecked = Settings.Instance.NoToolTip;
                checkBox_noBallonTips.IsChecked = Settings.Instance.NoBallonTips;
                textBox_ForceHideBalloonTipSec.Text = Settings.Instance.ForceHideBalloonTipSec.ToString();
                checkBox_showTray.IsChecked = Settings.Instance.ShowTray;
                checkBox_minHide.IsChecked = Settings.Instance.MinHide;
                checkBox_cautionManyChange.IsChecked = Settings.Instance.CautionManyChange;
                textBox_cautionManyChange.Text = Settings.Instance.CautionManyNum.ToString();
                checkBox_saveSearchKeyword.IsChecked = Settings.Instance.SaveSearchKeyword;
                checkBox_SyncResAutoAddChange.IsChecked = Settings.Instance.SyncResAutoAddChange;
                checkBox_SyncResAutoAddChgNewRes.IsChecked = Settings.Instance.SyncResAutoAddChgNewRes;
                checkBox_SyncResAutoAddDelete.IsChecked = Settings.Instance.SyncResAutoAddDelete;
                checkBox_keepTCPConnect.IsChecked = Settings.Instance.ChkSrvRegistTCP;
                textBox_chkTimerInterval.Text = Settings.Instance.ChkSrvRegistInterval.ToString();
                textBox_upDateTaskText.IsChecked = Settings.Instance.UpdateTaskText;
                checkBox_forceNWMode.IsChecked = Settings.Instance.ForceNWMode;

                checkBox_wakeReconnect.IsChecked = Settings.Instance.WakeReconnectNW;
                checkBox_WoLWait.IsChecked = Settings.Instance.WoLWait;
                checkBox_WoLWaitRecconect.IsChecked = Settings.Instance.WoLWaitRecconect;
                textBox_WoLWaitSecond.Text = Settings.Instance.WoLWaitSecond.ToString();
                checkBox_suspendClose.IsChecked = Settings.Instance.SuspendCloseNW;
                checkBox_ngAutoEpgLoad.IsChecked = Settings.Instance.NgAutoEpgLoadNW;

                if (checkBox_srvResident.IsEnabled)
                {
                    int residentMode = IniFileHandler.GetPrivateProfileInt("SET", "ResidentMode", 0, SettingPath.TimerSrvIniPath);
                    checkBox_srvResident.IsChecked = residentMode >= 1;
                    checkBox_srvShowTray.IsChecked = residentMode >= 2;
                    checkBox_srvNoBalloonTip.IsChecked = IniFileHandler.GetPrivateProfileInt("SET", "NoBalloonTip", 0, SettingPath.TimerSrvIniPath) != 0;
                }
                checkBox_srvSaveNotifyLog.IsChecked = IniFileHandler.GetPrivateProfileInt("SET", "SaveNotifyLog", 0, SettingPath.TimerSrvIniPath) != 0;
                checkBox_AutoSaveNotifyLog.IsChecked = Settings.Instance.AutoSaveNotifyLog == 1;
                checkBox_srvSaveDebugLog.IsChecked = IniFileHandler.GetPrivateProfileInt("SET", "SaveDebugLog", 0, SettingPath.TimerSrvIniPath) != 0;

                int count;
                count = IniFileHandler.GetPrivateProfileInt("DEL_EXT", "Count", 0, SettingPath.TimerSrvIniPath);
                if (count == 0)
                {
                    extList.Add(".ts.err");
                    extList.Add(".ts.program.txt");
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        extList.Add(IniFileHandler.GetPrivateProfileString("DEL_EXT", i.ToString(), "", SettingPath.TimerSrvIniPath));
                    }
                }

                count = IniFileHandler.GetPrivateProfileInt("DEL_CHK", "Count", 0, SettingPath.TimerSrvIniPath);
                for (int i = 0; i < count; i++)
                {
                    delChkFolderList.Add(IniFileHandler.GetPrivateProfileString("DEL_CHK", i.ToString(), "", SettingPath.TimerSrvIniPath));
                }

                if (CommonManager.Instance.IsConnected == true)
                {
                    CommonManager.Instance.DB.ReloadPlugInFile();
                }
                comboBox_recname.ItemsSource = CommonManager.Instance.DB.RecNamePlugInList.Values;
                comboBox_recname.SelectedItem = plugInFile;

                if (IniFileHandler.GetPrivateProfileInt("SET", "EnableTCPSrv", 0, SettingPath.TimerSrvIniPath) == 1)
                {
                    checkBox_tcpServer.IsChecked = true;
                }
                textBox_tcpPort.Text = IniFileHandler.GetPrivateProfileInt("SET", "TCPPort", 4510, SettingPath.TimerSrvIniPath).ToString();
                textBox_tcpAcl.Text = IniFileHandler.GetPrivateProfileString("SET", "TCPAccessControlList", "+127.0.0.1,+192.168.0.0/16", SettingPath.TimerSrvIniPath);
                textBox_tcpResTo.Text = IniFileHandler.GetPrivateProfileInt("SET", "TCPResponseTimeoutSec", 120, SettingPath.TimerSrvIniPath).ToString();

                defSearchKey = Settings.Instance.DefSearchKey.Clone();

                checkBox_showAsTab.IsChecked = Settings.Instance.ViewButtonShowAsTab;
                checkBox_suspendChk.IsChecked = (Settings.Instance.SuspendChk == 1);
                textBox_suspendChkTime.Text = Settings.Instance.SuspendChkTime.ToString();
                if (CommonManager.Instance.NWMode == true)
                {
                    textblockTimer.Text = "EpgTimerNW側の設定です。";
                }
                else
                {
                    textblockTimer.Text = "録画終了時にスタンバイ、休止する場合は必ず表示されます(ただし、サービス未使用時はこの設定は使用されず15秒固定)。";
                }

                listBox_Button_Set();

                textBox_name1.Text = Settings.Instance.Cust1BtnName;
                textBox_exe1.Text = Settings.Instance.Cust1BtnCmd;
                textBox_opt1.Text = Settings.Instance.Cust1BtnCmdOpt;

                textBox_name2.Text = Settings.Instance.Cust2BtnName;
                textBox_exe2.Text = Settings.Instance.Cust2BtnCmd;
                textBox_opt2.Text = Settings.Instance.Cust2BtnCmdOpt;

                textBox_name3.Text = Settings.Instance.Cust3BtnName;
                textBox_exe3.Text = Settings.Instance.Cust3BtnCmd;
                textBox_opt3.Text = Settings.Instance.Cust3BtnCmdOpt;

                foreach (ChSet5Item info in ChSet5.ChList.Values)
                {
                    ServiceViewItem item = new ServiceViewItem(info);
                    serviceList.Add(item.Key, item);
                }
                listBox_service.ItemsSource = serviceList.Values;

                stationList = Settings.Instance.IEpgStationList;
                ReLoadStation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public void SaveSetting()
        {
            try
            {
                if (radioButton_none.IsChecked == true)
                {
                    IniFileHandler.WritePrivateProfileString("SET", "RecEndMode", "0", SettingPath.TimerSrvIniPath);
                }
                if (radioButton_standby.IsChecked == true)
                {
                    IniFileHandler.WritePrivateProfileString("SET", "RecEndMode", "1", SettingPath.TimerSrvIniPath);
                }
                if (radioButton_suspend.IsChecked == true)
                {
                    IniFileHandler.WritePrivateProfileString("SET", "RecEndMode", "2", SettingPath.TimerSrvIniPath);
                }
                if (radioButton_shutdown.IsChecked == true)
                {
                    IniFileHandler.WritePrivateProfileString("SET", "RecEndMode", "3", SettingPath.TimerSrvIniPath);
                }

                string setValue = (checkBox_reboot.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "Reboot", setValue, SettingPath.TimerSrvIniPath);

                IniFileHandler.WritePrivateProfileString("SET", "WakeTime", textBox_pcWakeTime.Text, SettingPath.TimerSrvIniPath);
                IniFileHandler.WritePrivateProfileString("SET", "StartMargin", textBox_megine_start.Text, SettingPath.TimerSrvIniPath);
                IniFileHandler.WritePrivateProfileString("SET", "EndMargin", textBox_margine_end.Text, SettingPath.TimerSrvIniPath);
                IniFileHandler.WritePrivateProfileString("SET", "RecAppWakeTime", textBox_appWakeTime.Text, SettingPath.TimerSrvIniPath);

                setValue = (checkBox_appMin.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "RecMinWake", setValue, SettingPath.TimerSrvIniPath);

                setValue = (checkBox_appView.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "RecView", setValue, SettingPath.TimerSrvIniPath);

                setValue = (checkBox_appDrop.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "DropLog", setValue, SettingPath.TimerSrvIniPath);

                setValue = (checkBox_addPgInfo.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "PgInfoLog", setValue, SettingPath.TimerSrvIniPath);

                setValue = (checkBox_appNW.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "RecNW", setValue, SettingPath.TimerSrvIniPath);

                setValue = (checkBox_appOverWrite.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "RecOverWrite", setValue, SettingPath.TimerSrvIniPath);

                IniFileHandler.WritePrivateProfileString("SET", "KeepDisk", checkBox_appKeepDisk.IsChecked == true ? "1" : "0", SettingPath.TimerSrvIniPath);

                IniFileHandler.WritePrivateProfileString("NO_SUSPEND", "Count", ngProcessList.Count.ToString(), SettingPath.TimerSrvIniPath);
                for (int i = 0; i < ngProcessList.Count; i++)
                {
                    IniFileHandler.WritePrivateProfileString("NO_SUSPEND", i.ToString(), ngProcessList[i], SettingPath.TimerSrvIniPath);
                }

                IniFileHandler.WritePrivateProfileString("NO_SUSPEND", "NoStandbyTime", ngMin, SettingPath.TimerSrvIniPath);

                setValue = (ngUsePC == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("NO_SUSPEND", "NoUsePC", setValue, SettingPath.TimerSrvIniPath);

                IniFileHandler.WritePrivateProfileString("NO_SUSPEND", "NoUsePCTime", ngUsePCMin, SettingPath.TimerSrvIniPath);

                setValue = (ngFileStreaming == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("NO_SUSPEND", "NoFileStreaming", setValue, SettingPath.TimerSrvIniPath);

                setValue = (ngShareFile == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("NO_SUSPEND", "NoShareFile", setValue, SettingPath.TimerSrvIniPath);

                IniFileHandler.WritePrivateProfileString("SET", "ProcessPriority", comboBox_process.SelectedIndex.ToString(), SettingPath.TimerSrvIniPath);

                setValue = (checkBox_back_priority.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "BackPriority", setValue, SettingPath.TimerSrvIniPath);

                setValue = (checkBox_autoDel.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "AutoDel", setValue, SettingPath.TimerSrvIniPath);

                setValue = (checkBox_recname.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "RecNamePlugIn", setValue, SettingPath.TimerSrvIniPath);

                setValue = (comboBox_recname.SelectedItem != null ? (string)comboBox_recname.SelectedItem : "");
                IniFileHandler.WritePrivateProfileString("SET", "RecNamePlugInFile", setValue, SettingPath.TimerSrvIniPath);

                IniFileHandler.WritePrivateProfileString("SET", "NoChkYen", checkBox_noChkYen.IsChecked == true ? "1" : "0", SettingPath.TimerSrvIniPath);

                setValue = (checkBox_autoDelRecInfo.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "AutoDelRecInfo", setValue, SettingPath.TimerSrvIniPath);

                setValue = (checkBox_autoDelRecFile.IsChecked == true ? "1" : null);
                IniFileHandler.WritePrivateProfileString("SET", "RecInfoDelFile", setValue, SettingPath.CommonIniPath);

                IniFileHandler.WritePrivateProfileString("SET", "AutoDelRecInfoNum", textBox_autoDelRecInfo.Text.ToString(), SettingPath.TimerSrvIniPath);

                setValue = (checkBox_timeSync.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "TimeSync", setValue, SettingPath.TimerSrvIniPath);

                Settings.Instance.CautionOnRecChange = (checkBox_cautionOnRecChange.IsChecked != false);
                Settings.Instance.CautionOnRecMarginMin = mutil.MyToNumerical(textBox_cautionOnRecMarginMin, Convert.ToInt32, Settings.Instance.CautionOnRecMarginMin); 

                Settings.Instance.CloseMin = (bool)checkBox_closeMin.IsChecked;
                Settings.Instance.WakeMin = (bool)checkBox_minWake.IsChecked;
                Settings.Instance.ShowTray = (bool)checkBox_showTray.IsChecked;
                Settings.Instance.MinHide = (bool)checkBox_minHide.IsChecked;

                IniFileHandler.WritePrivateProfileString("SET", "ResidentMode",
                    checkBox_srvResident.IsChecked == false ? "0" : checkBox_srvShowTray.IsChecked == false ? "1" : "2", SettingPath.TimerSrvIniPath);
                IniFileHandler.WritePrivateProfileString("SET", "NoBalloonTip", checkBox_srvNoBalloonTip.IsChecked == false ? "0" : "1", SettingPath.TimerSrvIniPath);
                IniFileHandler.WritePrivateProfileString("SET", "SaveNotifyLog", checkBox_srvSaveNotifyLog.IsChecked == false ? "0" : "1", SettingPath.TimerSrvIniPath);
                Settings.Instance.AutoSaveNotifyLog = (short)(checkBox_AutoSaveNotifyLog.IsChecked == true ? 1 : 0);
                IniFileHandler.WritePrivateProfileString("SET", "SaveDebugLog", checkBox_srvSaveDebugLog.IsChecked == false ? "0" : "1", SettingPath.TimerSrvIniPath);

                IniFileHandler.WritePrivateProfileString("DEL_EXT", "Count", extList.Count.ToString(), SettingPath.TimerSrvIniPath);
                for (int i = 0; i < extList.Count; i++)
                {
                    IniFileHandler.WritePrivateProfileString("DEL_EXT", i.ToString(), extList[i], SettingPath.TimerSrvIniPath);
                }

                IniFileHandler.WritePrivateProfileString("DEL_CHK", "Count", delChkFolderList.Count.ToString(), SettingPath.TimerSrvIniPath);
                for (int i = 0; i < delChkFolderList.Count; i++)
                {
                    IniFileHandler.WritePrivateProfileString("DEL_CHK", i.ToString(), delChkFolderList[i], SettingPath.TimerSrvIniPath);
                }

                setValue = (checkBox_tcpServer.IsChecked == true ? "1" : "0");
                IniFileHandler.WritePrivateProfileString("SET", "EnableTCPSrv", setValue, SettingPath.TimerSrvIniPath);

                IniFileHandler.WritePrivateProfileString("SET", "TCPPort", textBox_tcpPort.Text, SettingPath.TimerSrvIniPath);
                IniFileHandler.WritePrivateProfileString("SET", "TCPAccessControlList", textBox_tcpAcl.Text, SettingPath.TimerSrvIniPath);
                IniFileHandler.WritePrivateProfileString("SET", "TCPResponseTimeoutSec", textBox_tcpResTo.Text, SettingPath.TimerSrvIniPath);

                Settings.Instance.NoToolTip = (checkBox_noToolTips.IsChecked == true);
                Settings.Instance.NoBallonTips = (checkBox_noBallonTips.IsChecked == true);
                Settings.Instance.ForceHideBalloonTipSec = mutil.MyToNumerical(textBox_ForceHideBalloonTipSec, Convert.ToInt32, 255, 0, Settings.Instance.ForceHideBalloonTipSec);
                Settings.Instance.CautionManyChange = (checkBox_cautionManyChange.IsChecked != false);
                Settings.Instance.CautionManyNum = mutil.MyToNumerical(textBox_cautionManyChange, Convert.ToInt32, Settings.Instance.CautionManyNum);
                Settings.Instance.SaveSearchKeyword = (checkBox_saveSearchKeyword.IsChecked != false);
                Settings.Instance.SyncResAutoAddChange = (checkBox_SyncResAutoAddChange.IsChecked != false);
                Settings.Instance.SyncResAutoAddDelete = (checkBox_SyncResAutoAddDelete.IsChecked != false);
                Settings.Instance.SyncResAutoAddChgNewRes = (checkBox_SyncResAutoAddChgNewRes.IsChecked != false);
                Settings.Instance.WakeReconnectNW = (checkBox_wakeReconnect.IsChecked == true);
                Settings.Instance.WoLWait = (checkBox_WoLWait.IsChecked == true);
                Settings.Instance.WoLWaitRecconect = (checkBox_WoLWaitRecconect.IsChecked == true);
                Settings.Instance.WoLWaitSecond = mutil.MyToNumerical(textBox_WoLWaitSecond, Convert.ToDouble, 3600, 1, Settings.Instance.WoLWaitSecond);
                Settings.Instance.SuspendCloseNW = (checkBox_suspendClose.IsChecked == true);
                Settings.Instance.NgAutoEpgLoadNW = (checkBox_ngAutoEpgLoad.IsChecked == true);
                Settings.Instance.ChkSrvRegistTCP = (checkBox_keepTCPConnect.IsChecked != false);
                Settings.Instance.ChkSrvRegistInterval = mutil.MyToNumerical(textBox_chkTimerInterval, Convert.ToDouble, 1440 * 7, 1, Settings.Instance.ChkSrvRegistInterval);
                Settings.Instance.UpdateTaskText = (textBox_upDateTaskText.IsChecked == true);
                Settings.Instance.ForceNWMode = (checkBox_forceNWMode.IsChecked != false);

                Settings.Instance.DefSearchKey = defSearchKey.Clone();

                Settings.Instance.ViewButtonShowAsTab = checkBox_showAsTab.IsChecked == true;
                Settings.Instance.SuspendChk = (uint)(checkBox_suspendChk.IsChecked == true ? 1 : 0);
                Settings.Instance.SuspendChkTime = mutil.MyToNumerical(textBox_suspendChkTime, Convert.ToUInt32, Settings.Instance.SuspendChkTime);

                Settings.Instance.ViewButtonList = listBox_viewBtn.Items.OfType<StringItem>().ValueList();
                Settings.Instance.TaskMenuList = listBox_viewTask.Items.OfType<StringItem>().ValueList();

                Settings.Instance.Cust1BtnName = textBox_name1.Text;
                Settings.Instance.Cust1BtnCmd = textBox_exe1.Text;
                Settings.Instance.Cust1BtnCmdOpt = textBox_opt1.Text;

                Settings.Instance.Cust2BtnName = textBox_name2.Text;
                Settings.Instance.Cust2BtnCmd = textBox_exe2.Text;
                Settings.Instance.Cust2BtnCmdOpt = textBox_opt2.Text;

                Settings.Instance.Cust3BtnName = textBox_name3.Text;
                Settings.Instance.Cust3BtnCmd = textBox_exe3.Text;
                Settings.Instance.Cust3BtnCmdOpt = textBox_opt3.Text;

                Settings.Instance.IEpgStationList = stationList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void button_standbyCtrl_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SetAppCancelWindow();
            dlg.Owner = CommonUtil.GetTopWindow(this);
            dlg.processList = this.ngProcessList;
            dlg.ngMin = this.ngMin;
            dlg.ngUsePC = this.ngUsePC;
            dlg.ngUsePCMin = this.ngUsePCMin;
            dlg.ngFileStreaming = this.ngFileStreaming;
            dlg.ngShareFile = this.ngShareFile;

            if (dlg.ShowDialog() == true)
            {
                this.ngProcessList = dlg.processList;
                this.ngMin = dlg.ngMin;
                this.ngUsePC = dlg.ngUsePC;
                this.ngUsePCMin = dlg.ngUsePCMin;
                this.ngFileStreaming = dlg.ngFileStreaming;
                this.ngShareFile = dlg.ngShareFile;
            }
        }

        private void button_autoDel_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SetApp2DelWindow();
            dlg.Owner = CommonUtil.GetTopWindow(this);
            dlg.extList = this.extList;
            dlg.delChkFolderList = this.delChkFolderList;

            if (dlg.ShowDialog() == true)
            {
                this.extList = dlg.extList;
                this.delChkFolderList = dlg.delChkFolderList;
            }
        }

        private void button_recname_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_recname.SelectedItem != null)
            {
                string name = comboBox_recname.SelectedItem as string;
                string filePath = SettingPath.ModulePath + "\\RecName\\" + name;

                RecNamePluginClass plugin = new RecNamePluginClass();
                HwndSource hwnd = (HwndSource)HwndSource.FromVisual(this);

                plugin.Setting(filePath, hwnd.Handle);
            }
        }

        private void listBox_Button_Set()
        {
            //ボタン表示画面の上下ボタンのみ他と同じものを使用する。
            var bxb = new BoxExchangeEditor(this.listBox_itemBtn, this.listBox_viewBtn, true);
            var bxt = new BoxExchangeEditor(this.listBox_itemTask, this.listBox_viewTask, true);

            //上部表示ボタン関係
            bxb.AllowDuplication(StringItem.Items("（空白）"), StringItem.Cloner, StringItem.Comparator);
            button_btnUp.Click += new RoutedEventHandler(bxb.button_Up_Click);
            button_btnDown.Click += new RoutedEventHandler(bxb.button_Down_Click);
            button_btnAdd.Click += new RoutedEventHandler((sender, e) => button_Add(bxb, buttonItem));
            button_btnIns.Click += new RoutedEventHandler((sender, e) => button_Add(bxb, buttonItem, true));
            button_btnDel.Click += new RoutedEventHandler((sender, e) => button_Dell(bxb, bxt, buttonItem));
            bxb.sourceBoxAllowKeyAction(listBox_itemBtn, (sender, e) => button_btnAdd.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)));
            bxb.targetBoxAllowKeyAction(listBox_viewBtn, (sender, e) => button_btnDel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)));
            bxb.sourceBoxAllowDoubleClick(listBox_itemBtn, (sender, e) => button_btnAdd.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)));
            bxb.targetBoxAllowDoubleClick(listBox_viewBtn, (sender, e) => button_btnDel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)));
            bxb.sourceBoxAllowDragDrop(listBox_itemBtn, (sender, e) => button_btnDel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)));
            bxb.targetBoxAllowDragDrop(listBox_viewBtn, (sender, e) => drag_drop(sender, e, button_btnAdd, button_btnIns));
 
            //タスクアイコン関係
            bxt.AllowDuplication(StringItem.Items("（セパレータ）"), StringItem.Cloner, StringItem.Comparator);
            button_taskUp.Click += new RoutedEventHandler(bxt.button_Up_Click);
            button_taskDown.Click += new RoutedEventHandler(bxt.button_Down_Click);
            button_taskAdd.Click += new RoutedEventHandler((sender, e) => button_Add(bxt, taskItem));
            button_taskIns.Click += new RoutedEventHandler((sender, e) => button_Add(bxt, taskItem, true));
            button_taskDel.Click += new RoutedEventHandler((sender, e) => button_Dell(bxt, bxb, taskItem));
            bxt.sourceBoxAllowKeyAction(listBox_itemTask, (sender, e) => button_taskAdd.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)));
            bxt.targetBoxAllowKeyAction(listBox_viewTask, (sender, e) => button_taskDel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)));
            bxt.sourceBoxAllowDoubleClick(listBox_itemTask, (sender, e) => button_taskAdd.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)));
            bxt.targetBoxAllowDoubleClick(listBox_viewTask, (sender, e) => button_taskDel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)));
            bxt.sourceBoxAllowDragDrop(listBox_itemTask, (sender, e) => button_taskDel.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)));
            bxt.targetBoxAllowDragDrop(listBox_viewTask, (sender, e) => drag_drop(sender, e, button_taskAdd, button_taskIns));

            listBox_viewBtn.Items.AddItems(StringItem.Items(Settings.Instance.ViewButtonList));
            buttonItem = Settings.Instance.GetViewButtonAllItems();
            reLoadButtonItem(bxb, buttonItem);

            listBox_viewTask.Items.AddItems(StringItem.Items(Settings.Instance.TaskMenuList));
            taskItem = Settings.Instance.GetTaskMenuAllItems();
            reLoadButtonItem(bxt, taskItem);

            //iEpg関係、キャンセルアクションだけは付けておく
            new BoxExchangeEditor(null, this.listBox_service, true);
            var bxi = new BoxExchangeEditor(null, this.listBox_iEPG, true);
            bxi.targetBoxAllowKeyAction(this.listBox_iEPG, new KeyEventHandler((sender, e) => button_del.RaiseEvent(new RoutedEventArgs(Button.ClickEvent))));
        }
        private void drag_drop(object sender, DragEventArgs e, Button add, Button ins)
        {
            var handler = (BoxExchangeEditor.GetDragHitItem(sender, e) == null ? add : ins);
            handler.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void button_Add(BoxExchangeEditor bx, List<string> src, bool isInsert = false)
        {
            int pos = bx.SourceBox.SelectedIndex - bx.SourceBox.SelectedItems.Count;
            bx.bxAddItems(bx.SourceBox, bx.TargetBox, isInsert);
            reLoadButtonItem(bx, src);
            if (bx.SourceBox.Items.Count != 0)
            {
                pos = Math.Max(0, Math.Min(pos, bx.SourceBox.Items.Count - 1));
                bx.SourceBox.SelectedIndex = pos;//順序がヘンだが、ENTERの場合はこの後に+1処理が入る模様
            }
        }
        private void button_Dell(BoxExchangeEditor bx, BoxExchangeEditor bx_other, List<string> src)
        {
            if (bx.TargetBox.SelectedItem == null) return;
            //
            var item1 = bx.TargetBox.SelectedItems.OfType<StringItem>().FirstOrDefault(item => item.Value == "設定");
            var item2 = bx_other.TargetBox.Items.OfType<StringItem>().FirstOrDefault(item => item.Value == "設定");
            if (item1 != null && item2 == null)
            {
                MessageBox.Show("設定は上部表示ボタンか右クリック表示項目のどちらかに必要です");
                return;
            }

            bx.bxDeleteItems(bx.TargetBox);
            reLoadButtonItem(bx, src);
        }
        private void reLoadButtonItem(BoxExchangeEditor bx, List<string> src)
        {
            var viewlist = bx.TargetBox.Items.OfType<StringItem>().Values();
            var diflist = src.Except(viewlist).ToList();
            diflist.Insert(0, (bx.DuplicationSpecific.First() as StringItem).Value);

            bx.SourceBox.ItemsSource = StringItem.Items(diflist.Distinct());
        }

        private void button_recDef_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SetDefRecSettingWindow();
            dlg.Owner = CommonUtil.GetTopWindow(this);
            dlg.ShowDialog();
        }

        private void button_searchDef_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SetDefSearchSettingWindow();
            dlg.Owner = CommonUtil.GetTopWindow(this);
            dlg.SetDefSetting(defSearchKey);

            if (dlg.ShowDialog() == true)
            {
                defSearchKey = dlg.GetSetting();
            }
        }

        private void button_exe_Click(object sender, RoutedEventArgs e)
        {
            CommonManager.GetFileNameByDialog((sender as Button).DataContext as TextBox, false, "", ".exe");
        }

        private void ReLoadStation()
        {
            listBox_iEPG.Items.Clear();
            if (listBox_service.SelectedItem != null)
            {
                ServiceViewItem item = listBox_service.SelectedItem as ServiceViewItem;
                foreach (IEPGStationInfo info in stationList)
                {
                    if (info.Key == item.Key)
                    {
                        listBox_iEPG.Items.Add(info);
                    }
                }
            }
        }

        private void button_add_iepg_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_service.SelectedItem != null)
            {
                ServiceViewItem item = listBox_service.SelectedItem as ServiceViewItem;
                foreach (IEPGStationInfo info in stationList)
                {
                    if (String.Compare(info.StationName, textBox_station.Text) == 0)
                    {
                        MessageBox.Show("すでに登録済みです");
                        return;
                    }
                }
                IEPGStationInfo addItem = new IEPGStationInfo();
                addItem.StationName = textBox_station.Text;
                addItem.Key = item.Key;

                stationList.Add(addItem);

                ReLoadStation();
            }
        }

        private void button_del_iepg_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_iEPG.SelectedItem != null)
            {
                IEPGStationInfo item = listBox_iEPG.SelectedItem as IEPGStationInfo;
                stationList.Remove(item);
                ReLoadStation();
            }
        }

        private void listBox_service_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReLoadStation();
        }

        private void checkBox_WoLWaitRecconect_Checked(object sender, RoutedEventArgs e)
        {
            checkBox_WoLWait.IsChecked = false;
        }
        private void checkBox_WoLWait_Checked(object sender, RoutedEventArgs e)
        {
            checkBox_WoLWaitRecconect.IsChecked = false;
        }

        private void button_clearSerchKeywords(object sender, RoutedEventArgs e)
        {
            Settings.Instance.AndKeyList = new List<string>();
            Settings.Instance.NotKeyList = new List<string>();
        }
    }
}
