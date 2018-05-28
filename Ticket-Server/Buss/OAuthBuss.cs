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
            UserParam userParam = JsonConvert.DeserializeObject<UserParam>(param.ToString());
            if(userParam == null)
            {
                Console.WriteLine("InvalidParam");
                Console.WriteLine(param.ToString());
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var appBag = AppContainer.GetAppBag(userParam.token);
            if (appBag != null)
            {
                return appBag;
            }
            else
            {
                Console.WriteLine("GetUserError");
                Console.WriteLine(userParam.token);
                throw new ApiException(CodeMessage.GetUserError, "GetUserError");
            }
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
