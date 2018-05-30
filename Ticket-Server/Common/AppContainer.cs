using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Senparc.Weixin.CacheUtility;
using Senparc.Weixin.Containers;
using Senparc.Weixin.WxOpen.Helpers;

namespace Ticket_Server.Common
{
    [Serializable]
    public class AppBag : BaseContainerBag
    {
        public string Key
        {
            get { return _key; }
#if NET35 || NET40
            set { this.SetContainerProperty(ref _key, value, "Key"); }
#else
            set { this.SetContainerProperty(ref _key, value); }
#endif
        }

        /// <summary>
        /// Values
        /// </summary>
        public string Values
        {
            get { return _values; }
#if NET35 || NET40
            set { this.SetContainerProperty(ref _values, value, "Values"); }
#else
            set { this.SetContainerProperty(ref _values, value); }
#endif
        }

        /// <summary>
        /// AppObj
        /// </summary>
        public string AppObj
        {
            get { return _appObj; }
#if NET35 || NET40
            set { this.SetContainerProperty(ref _sessionKey, value, "SessionKey"); }
#else
            set { this.SetContainerProperty(ref _appObj, value); }
#endif
        }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime
        {
            get { return _expireTime; }
#if NET35 || NET40
            set { this.SetContainerProperty(ref _expireTime, value, "ExpireTime"); }
#else
            set { this.SetContainerProperty(ref _expireTime, value); }
#endif
        }

        private string _key;
        private string _values;
        private string _appObj;
        private DateTime _expireTime;

        /// <summary>
        /// ComponentBag
        /// </summary>
        public AppBag()
        {
        }
    }

    public class AppContainer : BaseContainer<AppBag>
    {
        /// <summary>
        /// 获取最新的过期时间
        /// </summary>
        /// <returns></returns>
        private static DateTime GetExpireTime()
        {
            return DateTime.Now.AddDays(2);//有效期2天
        }

        #region 同步方法

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static AppBag GetAppBag(string key)
        {
            var bag = TryGetItem(key);
            if (bag == null)
            {
                return null;
            }

            if (bag.ExpireTime < DateTime.Now)
            {
                //已经过期
                Cache.RemoveFromCache(key);
                return null;
            }

            using (FlushCache.CreateInstance())
            {
                bag.ExpireTime = GetExpireTime();//滚动过期时间
                Update(key, bag);
            }
            return bag;
        }

        /// <summary>
        /// 更新或插入
        /// </summary>
        /// <param name="key">如果留空，则新建一条记录</param>
        /// <param name="values">值</param>
        /// <param name="appObj">对象</param>
        /// <returns></returns>
        public static AppBag UpdateAppBag(string key, string values, string appObj)
        {
            key = key ?? SessionHelper.GetNewThirdSessionName();

            using (FlushCache.CreateInstance())
            {
                var appBag = new AppBag()
                {
                    Key = key,
                    Values = values,
                    AppObj = appObj,
                    ExpireTime = GetExpireTime()
                };
                Update(key, appBag);
                return appBag;
            }
        }

        #endregion
    }
}
