﻿using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreApp.Services.Controls.History
{
    /// <summary>
    /// 历史记录存储服务
    /// </summary>
    public static class HistoryStorageService
    {
        private const string QueryLinks = "QueryLinks";
        private const string SearchStore = "SearchStore";

        private const string CreateTimeStamp = "CreateTimeStamp";
        private const string HistoryKey = "HistoryKey";
        private const string HistoryAppName = "HistoryAppName";
        private const string HistoryContent = "HistoryContent";
        private const string HistoryType = "HistoryType";
        private const string HistoryChannel = "HistoryChannel";
        private const string HistoryLink = "HistoryLink";

        private static readonly Lock historyStorageLock = new();
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer queryLinksContainer;
        private static ApplicationDataContainer searchStoreContainer;

        public static event Action QueryLinksCleared;

        public static event Action SearchStoreCleared;

        /// <summary>
        /// 初始化历史记录存储服务
        /// </summary>
        public static void Initialize()
        {
            queryLinksContainer = localSettingsContainer.CreateContainer(QueryLinks, ApplicationDataCreateDisposition.Always);
            searchStoreContainer = localSettingsContainer.CreateContainer(SearchStore, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 获取查询链接历史记录数据
        /// </summary>
        public static List<HistoryModel> GetQueryLinksData()
        {
            List<HistoryModel> queryLinksHistoryList = [];

            historyStorageLock.Enter();

            try
            {
                if (queryLinksContainer is not null)
                {
                    for (int index = 1; index <= 3; index++)
                    {
                        if (queryLinksContainer.Values.TryGetValue(QueryLinks + index.ToString(), out object value))
                        {
                            if (value is ApplicationDataCompositeValue compositeValue)
                            {
                                TypeModel typeItem = ResourceService.TypeList.Find(item => item.InternalName.Equals(compositeValue[HistoryType] as string, StringComparison.OrdinalIgnoreCase));
                                ChannelModel channelItem = ResourceService.ChannelList.Find(item => item.InternalName.Equals(compositeValue[HistoryChannel] as string, StringComparison.OrdinalIgnoreCase));

                                queryLinksHistoryList.Add(new HistoryModel()
                                {
                                    CreateTimeStamp = Convert.ToInt64(compositeValue[CreateTimeStamp]),
                                    HistoryKey = Convert.ToString(compositeValue[HistoryKey]),
                                    HistoryAppName = Convert.ToString(compositeValue[HistoryAppName]),
                                    HistoryType = new KeyValuePair<string, string>(typeItem.InternalName, typeItem.DisplayName),
                                    HistoryChannel = new KeyValuePair<string, string>(channelItem.InternalName, channelItem.DisplayName),
                                    HistoryLink = Convert.ToString(compositeValue[HistoryLink])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Get query links history data failed", e);
            }
            finally
            {
                historyStorageLock.Exit();
            }

            return queryLinksHistoryList;
        }

        /// <summary>
        /// 获取搜索应用历史记录数据
        /// </summary>
        public static List<HistoryModel> GetSearchStoreData()
        {
            List<HistoryModel> searchStoreHistoryList = [];

            historyStorageLock.Enter();
            try
            {
                for (int index = 1; index <= 3; index++)
                {
                    if (searchStoreContainer.Values.TryGetValue(SearchStore + index.ToString(), out object value))
                    {
                        if (value is ApplicationDataCompositeValue compositeValue)
                        {
                            TypeModel typeItem = ResourceService.TypeList.Find(item => item.InternalName.Equals(compositeValue["HistoryType"] as string, StringComparison.OrdinalIgnoreCase));
                            ChannelModel channelItem = ResourceService.ChannelList.Find(item => item.InternalName.Equals(compositeValue["HistoryChannel"] as string, StringComparison.OrdinalIgnoreCase));

                            searchStoreHistoryList.Add(new HistoryModel()
                            {
                                CreateTimeStamp = Convert.ToInt64(compositeValue[CreateTimeStamp]),
                                HistoryKey = Convert.ToString(compositeValue[HistoryKey]),
                                HistoryContent = Convert.ToString(compositeValue[HistoryContent]),
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Get query links history data failed", e);
            }
            finally
            {
                historyStorageLock.Exit();
            }

            return searchStoreHistoryList;
        }

        /// <summary>
        /// 存储查询链接历史记录数据
        /// </summary>
        public static void SaveQueryLinksData(List<HistoryModel> queryLinksHistoryList)
        {
            int endIndex = queryLinksHistoryList.Count >= 3 ? 3 : queryLinksHistoryList.Count;

            historyStorageLock.Enter();

            try
            {
                for (int index = 1; index <= endIndex; index++)
                {
                    ApplicationDataCompositeValue compositeValue = new()
                    {
                        [CreateTimeStamp] = queryLinksHistoryList[index - 1].CreateTimeStamp,
                        [HistoryKey] = queryLinksHistoryList[index - 1].HistoryKey,
                        [HistoryAppName] = queryLinksHistoryList[index - 1].HistoryAppName,
                        [HistoryType] = queryLinksHistoryList[index - 1].HistoryType.Key,
                        [HistoryChannel] = queryLinksHistoryList[index - 1].HistoryChannel.Key,
                        [HistoryLink] = queryLinksHistoryList[index - 1].HistoryLink
                    };

                    string queryLinksKey = QueryLinks + index.ToString();
                    queryLinksContainer.Values[queryLinksKey] = compositeValue;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Save query links history data failed", e);
            }
            finally
            {
                historyStorageLock.Exit();
            }
        }

        /// <summary>
        /// 存储搜索商店历史记录数据
        /// </summary>
        public static void SaveSearchStoreData(List<HistoryModel> searchStoreHistoryList)
        {
            int endIndex = searchStoreHistoryList.Count > 3 ? 3 : searchStoreHistoryList.Count;

            historyStorageLock.Enter();

            try
            {
                for (int index = 1; index <= endIndex; index++)
                {
                    ApplicationDataCompositeValue compositeValue = new()
                    {
                        [CreateTimeStamp] = searchStoreHistoryList[index - 1].CreateTimeStamp,
                        [HistoryKey] = searchStoreHistoryList[index - 1].HistoryKey,
                        [HistoryContent] = searchStoreHistoryList[index - 1].HistoryContent
                    };

                    string searchStoreKey = SearchStore + index.ToString();
                    searchStoreContainer.Values[searchStoreKey] = compositeValue;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Save search store history data failed", e);
            }
            finally
            {
                historyStorageLock.Exit();
            }
        }

        /// <summary>
        /// 清除历史记录
        /// </summary>
        public static bool ClearData()
        {
            historyStorageLock.Enter();

            try
            {
                queryLinksContainer.Values.Clear();
                QueryLinksCleared?.Invoke();
                searchStoreContainer.Values.Clear();
                SearchStoreCleared?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Clear history record failed", e);
                return false;
            }
            finally
            {
                historyStorageLock.Exit();
            }
        }
    }
}
