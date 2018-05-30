using Newtonsoft.Json;
using Ticket_Server.Dao;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP.AdvancedAPIs;
using Ticket_Server.Common;
using Senparc.Weixin.MP;
using System;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;

namespace Ticket_Server.Buss
{
    public class OAuthBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.OAuthApi;
        }

        public bool NeedCheck()
        {
            return false;
        }

        public object Do_GetUser(object param)
        {
            //try
            //{
            UserParam userParam = JsonConvert.DeserializeObject<UserParam>(param.ToString());
            if (userParam == null)
            {
                Console.WriteLine("InvalidParam");
                Console.WriteLine(param.ToString());
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var appBag = AppContainer.GetAppBag(userParam.token);
            OAuthUserInfo userInfo = JsonConvert.DeserializeObject<OAuthUserInfo>(appBag.AppObj.ToString());
            if (appBag != null)
            {
                return userInfo;
            }
            else
            {
                Console.WriteLine("InvalidToken");
                Console.WriteLine(userParam.token);
                throw new ApiException(CodeMessage.InvalidToken, "InvalidToken");
            }
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    Console.WriteLine(ex.StackTrace);
            //    Console.WriteLine(ex.InnerException.Message);
            //    Console.WriteLine(ex.InnerException.StackTrace);
            //    throw new ApiException(CodeMessage.InnerError, "InnerError");
            //}

        }


        public object Do_GetOAuthUrl(object param)
        {
            OAuthUrlParam oAuthUrlParam = JsonConvert.DeserializeObject<OAuthUrlParam>(param.ToString());
            if (oAuthUrlParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var appBag = AppContainer.UpdateAppBag(null, oAuthUrlParam.returnUrl, null);
            string url =
                Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAuthorizeUrl(Global.APPID,
                "http://weixin.llwell.net/api/Weixin/OAuthCallback?returnUrl=" + oAuthUrlParam.returnUrl.UrlEncode(),
                appBag.Key, OAuthScope.snsapi_userinfo);
            return new { url = url };
        }
    }

    public class UserParam
    {
        public string token;
    }

    public class OAuthUrlParam
    {
        public string returnUrl;
    }

}
