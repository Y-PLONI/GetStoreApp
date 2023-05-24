using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GetStoreApp.UI.Notifications
{
    /// <summary>
    /// WinGet 程序包应用安装、卸载、升级指令复制应用内通知视图
    /// </summary>
    public sealed partial class WinGetCopyNotification : UserControl
    {
        private Popup Popup { get; set; } = new Popup();

        private int Duration = 2000;

        public WinGetCopyNotification(bool copyState, [Optional, DefaultParameterValue(WinGetOptionArgs.SearchInstall)] WinGetOptionArgs optionArgs, [Optional] double? duration)
        {
            InitializeComponent();
            ViewModel.Initialize(copyState, optionArgs);

            SetPopUpPlacement();

            Popup.Child = this;
            Popup.XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();

            if (duration.HasValue)
            {
                Duration = Convert.ToInt32(duration * 1000);
            }
        }

        /// <summary>
        /// 控件加载完成后显示动画，动态设置控件位置
        /// </summary>
        private void NotificationLoaded(object sender, RoutedEventArgs args)
        {
            PopupIn.Begin();
            Program.ApplicationRoot.MainWindow.SizeChanged += NotificationPlaceChanged;
        }

        public bool ControlLoaded(bool copyState, WinGetOptionArgs optionArgs, string controlName)
        {
            if (controlName is "SearchInstallCopySuccess" && copyState && optionArgs == WinGetOptionArgs.SearchInstall)
            {
                return true;
            }
            else if (controlName is "SearchInstallCopyFailed" && !copyState && optionArgs == WinGetOptionArgs.SearchInstall)
            {
                return true;
            }
            else if (controlName is "UnInstallCopySuccess" && copyState && optionArgs == WinGetOptionArgs.UnInstall)
            {
                return true;
            }
            else if (controlName is "UnInstallCopyFailed" && !copyState && optionArgs == WinGetOptionArgs.UnInstall)
            {
                return true;
            }
            else if (controlName is "UpgradeInstallCopySuccess" && copyState && optionArgs == WinGetOptionArgs.UpgradeInstall)
            {
                return true;
            }
            else if (controlName is "UpgradeInstallCopyFailed" && !copyState && optionArgs == WinGetOptionArgs.UpgradeInstall)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 控件卸载时移除相应的事件
        /// </summary>
        private void NotificationUnLoaded(object sender, RoutedEventArgs args)
        {
            Program.ApplicationRoot.MainWindow.SizeChanged -= NotificationPlaceChanged;
        }

        /// <summary>
        /// 窗口大小调整时修改应用内通知的相对位置
        /// </summary>
        private void NotificationPlaceChanged(object sender, WindowSizeChangedEventArgs args)
        {
            SetPopUpPlacement();
        }

        /// <summary>
        /// 应用内通知加载动画演示完成时发生
        /// </summary>
        private async void PopupInCompleted(object sender, object e)
        {
            await Task.Delay(Duration);
            PopupOut.Begin();
        }

        /// <summary>
        /// 应用内通知加载动画卸载完成时发生，关闭控件
        /// </summary>
        public void PopupOutCompleted(object sender, object e)
        {
            Popup.IsOpen = false;
        }

        /// <summary>
        /// 设置PopUp的显示位置
        /// </summary>
        private void SetPopUpPlacement()
        {
            Width = Program.ApplicationRoot.MainWindow.Bounds.Width;
            Height = Program.ApplicationRoot.MainWindow.Bounds.Height;

            Popup.VerticalOffset = 75;
        }

        /// <summary>
        /// 显示弹窗
        /// </summary>
        public void Show()
        {
            Popup.IsOpen = true;
        }
    }
}
