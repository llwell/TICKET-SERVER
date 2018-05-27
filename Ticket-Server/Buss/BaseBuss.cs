using Senparc.Weixin.WxOpen.Containers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ticket_Server.Common;

namespace Ticket_Server.Buss
{
    /// <summary>
    /// 具体业务实现类接口
    /// </summary>
    public interface IBuss
    {
        ApiType GetApiType();
    }

    /// <summary>
    /// 基础业务实现类
    /// </summary>
    public class BaseBuss
    {
        /// <summary>
        /// 具体业务实现类字典列表
        /// </summary>
        private Dictionary<ApiType, IBuss> bussList = new Dictionary<ApiType, IBuss>();

        /// <summary>
        /// 构造函数
        /// 构造时反射加载所有具体业务类
        /// </summary>
        public BaseBuss()
        {
            this.BuildBuss();
        }

        /// <summary>
        /// 遍历所有IBUSS接口的实现类
        /// 全部实例化并插入具体业务实现类字典列表
        /// </summary>
        private void BuildBuss()
        {
            ///获取所有IBUSS的实现类
            var types = AppDomain.CurrentDomain.GetAssemblies()
                  .SelectMany(a => a.GetTypes()
                  .Where(t => t.GetInterfaces().Contains(typeof(IBuss))))
                  .ToArray();
            ///遍历实现类，实例化非接口与抽象类，并插入具体业务实现类字典列表
            foreach (var v in types)
            {
                if (v.IsClass)
                {
                    var buss = (Activator.CreateInstance(v) as IBuss);
                    AddBuss(buss.GetApiType(), buss);
                }
            }
        }

        /// <summary>
        /// 插入具体业务实现类字典列表
        /// 相同组名的业务实现类唯一
        /// </summary>
        /// <param name="apiType">API方法组</param>
        /// <param name="buss">具体业务实现类</param>
        private void AddBuss(ApiType apiType, IBuss buss)
        {
            if (bussList.ContainsKey(apiType))
            {
                bussList[apiType] = buss;
            }
            else
            {
                bussList.Add(apiType, buss);
            }
        }

        /// <summary>
        /// 检查token判断执行权限
        /// </summary>
        /// <param name="apiType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool CheckToken(ApiType apiType, string token)
        {
            bool b = true;
#if !DEBUG
            var appBag = AppContainer.GetAppBag(token);
            if (appBag == null)
            {
                b = false;
            }
#endif
            return b;
        }

        /// <summary>
        /// 执行业务方法
        /// </summary>
        /// <param name="apiType">API方法组</param>
        /// <param name="token">token</param>
        /// <param name="method">方法名</param>
        /// <param name="param">参数JSON对象</param>
        /// <returns></returns>
        public ResultsJson BussResults(ApiType apiType, string token, string method, object param)
        {
            if (!CheckToken(apiType, token))
            {
                return new ResultsJson(new Message(CodeMessage.InvalidToken, "InvalidToken"), null);
            }
            var obj = bussList[apiType];
            MethodInfo methodInfo = obj.GetType().GetMethod("Do_" + method);
            if (methodInfo == null)
            {
                return new ResultsJson(new Message(CodeMessage.InvalidMethod, "InvalidMethod"), null);
            }
            else
            {
                Message message = null;
                object data = null;
                try
                {
                    data = methodInfo.Invoke(obj, new object[] { param });
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.GetType() == typeof(ApiException))
                    {
                        ApiException apiException = (ApiException)ex.InnerException;
                        message = new Message(apiException.code, apiException.msg);
                    }
                    else
                    {
                        message = new Message(CodeMessage.InnerError, "InnerError");
                    }
                }
                return new ResultsJson(message, data);
            }
        }
    }
}
