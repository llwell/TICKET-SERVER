using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Ticket_Server.Common;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.Containers;

namespace Ticket_Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class WeixinController : Controller
    {
        [HttpGet]
        public ActionResult OAuth(string code, string state, string returnUrl)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            var checkBag = AppContainer.GetAppBag(state);

            if(checkBag.Values != returnUrl)
            {
                return Content("验证错误！");
            }
            AppContainer.UpdateAppBag(checkBag.Key, "", null);

            OAuthAccessTokenResult result = null;
            try
            {
                result = OAuthApi.GetAccessToken(Global.APPID, Global.APPSECRET, code);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + result.errmsg);
            }

            try
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return Content("目标页面无效");
                }

                OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                var appBag = AppContainer.UpdateAppBag(null, userInfo.openid, userInfo);

                return Redirect(returnUrl + "#/?token=" + appBag.Key);
            }
            catch (ErrorJsonResultException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult GetOAuthUrl(string returnUrl)
        {
            var appBag = AppContainer.UpdateAppBag(null, returnUrl, null);
            string url =
                OAuthApi.GetAuthorizeUrl(Global.APPID,
                "http://weixin.llwell.net/api/Weixin/OAuth?returnUrl=" + returnUrl.UrlEncode(),
                appBag.Key, OAuthScope.snsapi_userinfo);
            return Json(new { url = url });
        }
    }
}