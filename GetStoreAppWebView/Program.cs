using GetStoreAppWebView.Services.Controls.Settings;
using GetStoreAppWebView.Services.Root;
using System;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;

namespace GetStoreAppWebView
{
    /// <summary>
    /// 网页浏览器
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        public static void Main()
        {
            InitializeResources();

            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[1])
            {
                Environment.SetEnvironmentVariable("WEBVIEW2_USE_VISUAL_HOSTING_FOR_OWNED_WINDOWS", "1");

                string systemWebView2Path = Path.Combine(SystemDataPaths.GetDefault().System, "msedgewebview2.exe");

                if (Directory.Exists(systemWebView2Path) && File.Exists(Path.Combine(systemWebView2Path, "WebView2.exe")))
                {
                    Environment.SetEnvironmentVariable("WEBVIEW2_BROWSER_EXECUTABLE_FOLDER", Path.Combine(SystemDataPaths.GetDefault().System, "Microsoft-Edge-WebView"));
                }
            }

            Application.Start(static (param) => _ = new App());
        }

        /// <summary>
        /// 加载应用程序所需的资源
        /// </summary>
        private static void InitializeResources()
        {
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            ResultService.Initialize();
            WebKernelService.InitializeWebKernel();
        }
    }
}
