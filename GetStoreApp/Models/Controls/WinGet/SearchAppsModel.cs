﻿using GetStoreApp.Models.Base;

namespace GetStoreApp.Models.Controls.WinGet
{
    /// <summary>
    /// 搜索应用数据模型
    /// </summary>
    public class SearchAppsModel : ModelBase
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 应用的发布者
        /// </summary>
        public string AppPublisher { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// 应用是否处于正在安装中状态
        /// </summary>
        public bool _isInstalling;

        public bool IsInstalling
        {
            get { return _isInstalling; }

            set
            {
                _isInstalling = value;
                OnPropertyChanged();
            }
        }
    }
}
