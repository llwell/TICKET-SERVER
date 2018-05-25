using Ticket_Server.Buss;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Senparc.Weixin.Cache.Redis;
using Senparc.Weixin.Cache;

namespace Ticket_Server.Common
{
    public class Global
    {
        public static DataTable provincesDT;
        public static DataTable cityDT;
        public static DataTable areasDT;

        /// <summary>
        /// 基础业务处理类对象
        /// </summary>
        public static BaseBuss BUSS = new BaseBuss();

        /// <summary>
        /// 初始化启动预加载
        /// </summary>
        public static void StartUp()
        {
            try
            {
                RedisManager.ConfigurationOption = REDIS;
                CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisContainerCacheStrategy.Instance);
            }
            catch
            {
                Console.WriteLine("Redis Error, Change Local");
            }
        }

        public static string REDIS
        {
            get
            {
#if DEBUG
                var redis = System.Environment.GetEnvironmentVariable("redis", EnvironmentVariableTarget.User);
#endif
#if !DEBUG
                var redis = "redis-api";
#endif
                return redis;
            }
        }

        /// <summary>
        /// 小程序APPID
        /// </summary>
        public static string APPID
        {
            get
            {
#if DEBUG
                var appId = System.Environment.GetEnvironmentVariable("WxAppId", EnvironmentVariableTarget.User);
#endif
#if !DEBUG
                var appId = System.Environment.GetEnvironmentVariable("WxAppId");
#endif
                return appId;
            }
        }

        /// <summary>
        /// 小程序APPSECRET
        /// </summary>
        public static string APPSECRET
        {
            get
            {
#if DEBUG
                var appSecret = System.Environment.GetEnvironmentVariable("WxAppSecret", EnvironmentVariableTarget.User);
#endif
#if !DEBUG
                var appSecret = System.Environment.GetEnvironmentVariable("WxAppSecret");
#endif
                return appSecret;
            }
        }


        /// <summary>
        /// 微信支付MchId
        /// </summary>
        public static string MCHID
        {
            get
            {
#if DEBUG
                var mchId = System.Environment.GetEnvironmentVariable("WxMchId", EnvironmentVariableTarget.User);
#endif
#if !DEBUG
                var mchId = System.Environment.GetEnvironmentVariable("WxMchId");
#endif
                return mchId;
            }
        }


        /// <summary>
        /// 微信支付Key
        /// </summary>
        public static string PaymentKey
        {
            get
            {
#if DEBUG
                var paymentKey = System.Environment.GetEnvironmentVariable("WxPaymentKey", EnvironmentVariableTarget.User);
#endif
#if !DEBUG
                var paymentKey = System.Environment.GetEnvironmentVariable("WxPaymentKey");
#endif
                return paymentKey;
            }
        }

        /// <summary>
        /// 微信支付回调地址
        /// </summary>
        public static string CallBackUrl
        {
            get
            {
#if DEBUG
                var callBackUrl = "http://dlxm.f3322.net:10010/api/O2O/PaymentCallBack";
#endif
#if !DEBUG
                var callBackUrl = "https://wxapp.llwell.net/api/O2O/PaymentCallBack";
#endif
                return callBackUrl;
            }
        }

        /// <summary>
        /// 支付成功消息模板
        /// </summary>
        public static string PaySuccessTemplate
        {
            get
            {
                return "bjTZpPW5j7qG2zhzr_y1NYs_P3ZKZNdvGZgI8gbvT68";
            }
        }


    }
}
