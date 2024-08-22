﻿using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 文件夹选取框
    /// </summary>
    public partial class OpenFolderDialog : IDisposable
    {
        private readonly Guid CLSID_FileOpenDialog = new("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7");
        private readonly Guid IID_IUnknown = new("00000000-0000-0000-C000-000000000046");
        private static readonly StrategyBasedComWrappers strategyBasedComWrappers = new();
        private IFileOpenDialog FileOpenDialog;
        private WindowId parentWindowId;

        public string Description { get; set; } = string.Empty;

        public string SelectedPath { get; set; } = string.Empty;

        public string RootFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public OpenFolderDialog(WindowId windowId)
        {
            if (windowId.Value is 0)
            {
                throw new Win32Exception("窗口句柄无效");
            }

            parentWindowId = windowId;
            int result = Ole32Library.CoCreateInstance(ref CLSID_FileOpenDialog, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, ref IID_IUnknown, out IntPtr ppv);

            if (result is 0)
            {
                FileOpenDialog = (IFileOpenDialog)strategyBasedComWrappers.GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.None);
            }

            FileOpenDialog.SetOptions(FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS);
            FileOpenDialog.SetTitle(Description);
            Guid iShellItemGuid = typeof(IShellItem).GUID;
            Shell32Library.SHCreateItemFromParsingName(RootFolder, IntPtr.Zero, ref iShellItemGuid, out IntPtr initialFolder);
            FileOpenDialog.SetFolder((IShellItem)strategyBasedComWrappers.GetOrCreateObjectForComInstance(initialFolder, CreateObjectFlags.None));
        }

        ~OpenFolderDialog()
        {
            Dispose(false);
        }

        /// <summary>
        /// 显示文件夹选取对话框
        /// </summary>
        public bool ShowDialog()
        {
            try
            {
                if (FileOpenDialog is not null)
                {
                    int result = FileOpenDialog.Show((IntPtr)parentWindowId.Value);

                    if (result is not 0)
                    {
                        return false;
                    }

                    FileOpenDialog.GetResult(out IShellItem pItem);
                    pItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out IntPtr pszString);
                    SelectedPath = Marshal.PtrToStringUni(pszString);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "OpenFolderDialog(IFileOpenDialog) initialize failed.", e);
                FileOpenDialog = null;
                return false;
            }
        }

        /// <summary>
        /// 释放打开文件夹选取对话框所需的资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            lock (this)
            {
                FileOpenDialog = null;
            }
        }
    }
}
