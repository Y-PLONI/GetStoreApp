﻿using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.SystemTray;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Properties;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using WinRT;

namespace GetStoreApp
{
    /// <summary>
    /// 获取商店应用 桌面程序
    /// </summary>
    public class Program
    {
        private static bool IsDesktopProgram { get; set; } = true;

        public static bool IsAppLaunched { get; set; } = false;

        // 应用程序实例
        public static WinUIApp ApplicationRoot { get; private set; }

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            CheckAppBootState();

            IsDesktopProgram = GetAppExecuteMode(args);

            InitializeProgramResourcesAsync().Wait();

            // 以桌面应用程序方式正常启动
            if (IsDesktopProgram)
            {
                DesktopLaunchService.InitializeLaunchAsync(args).Wait();
                ComWrappersSupport.InitializeComWrappers();

                Application.Start((param) =>
                {
                    DispatcherQueueSynchronizationContext context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    ApplicationRoot = new WinUIApp();
                });
            }

            // 以控制台程序方式启动
            else
            {
                bool AttachResult = Kernel32Library.AttachConsole();

                if (!AttachResult)
                {
                    Kernel32Library.AllocConsole();
                }
                ConsoleLaunchService.InitializeLaunchAsync(args).Wait();

                Kernel32Library.FreeConsole();

                // 退出应用程序
                Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
            }
        }

        /// <summary>
        /// 检查应用的启动状态
        /// </summary>
        public static void CheckAppBootState()
        {
            Resources.Culture = CultureInfo.CurrentCulture.Parent;
            if (!RuntimeHelper.IsMSIX)
            {
                Environment.Exit(Convert.ToInt32(AppExitCode.Failed));
            }
        }

        /// <summary>
        /// 检查命令参数是否以桌面方式启动
        /// </summary>
        private static bool GetAppExecuteMode(string[] args)
        {
            return args.Length is 0 || !args[0].Equals("Console", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static async Task InitializeProgramResourcesAsync()
        {
            // 初始化应用资源，应用使用的语言信息和启动参数
            await LogService.InitializeAsync();
            await LanguageService.InitializeLanguageAsync();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);

            // 初始化通用设置选项（桌面应用程序和控制台应用程序）
            ResourceService.LocalizeReosurce();
            await LinkFilterService.InitializeLinkFilterValueAsnyc();
            await DownloadOptionsService.InitializeAsync();

            // 初始化应用任务跳转列表信息
            AppJumpList.GroupName = ResourceService.GetLocalized("Window/JumpListGroupName");
            AppJumpList.GroupKind = JumpListSystemGroupKind.Recent;

            if (IsDesktopProgram)
            {
                // 初始化存储数据信息
                await XmlStorageService.InitializeXmlFileAsync();
                await DownloadXmlService.InitializeDownloadXmlAsync();

                // 初始化应用配置信息
                await AppExitService.InitializeAppExitAsync();
                await InstallModeService.InitializeInstallModeAsync();

                await AlwaysShowBackdropService.InitializeAlwaysShowBackdropAsync();
                await BackdropService.InitializeBackdropAsync();
                await ThemeService.InitializeAsync();
                await TopMostService.InitializeTopMostValueAsync();

                await HistoryRecordService.InitializeAsync();
                await NotificationService.InitializeNotificationAsync();
                await UseInstructionService.InitializeUseInsVisValueAsync();
                await WinGetConfigService.InitializeWinGetConfigAsync();

                // 实验功能设置配置
                await NetWorkMonitorService.InitializeNetWorkMonitorValueAsync();
            }
        }
    }
}
