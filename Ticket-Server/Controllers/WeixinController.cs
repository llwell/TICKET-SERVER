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
using Senparc.Weixin.WxOpen.Containers;
using Newtonsoft.Json;
using Ticket_Server.Dao;

namespace Ticket_Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class WeixinController : Controller
    {
        [HttpGet]
        public ActionResult OAuthCallBack(string code, string state, string returnUrl)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            var checkBag = AppContainer.GetAppBag(state);

            if (checkBag.Values != returnUrl)
            {
                return Content("验证错误！");
            }
            AppContainer.UpdateAppBag(checkBag.Key, "", null);

            OAuthAccessTokenResult result = null;
            try
            {
                result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(Global.APPID, Global.APPSECRET, code);
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

                OAuthUserInfo userInfo = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetUserInfo(result.access_token, result.openid);
                string jsonUser = JsonConvert.SerializeObject(userInfo);

                var appBag = AppContainer.UpdateAppBag(null, userInfo.openid, jsonUser);

                UserDao userDao = new UserDao();
                userDao.insertUser(userInfo);

                return Redirect(returnUrl + "#/?token=" + appBag.Key);
            }
            catch (ErrorJsonResultException ex)
            {
                return Content(ex.Message);
            }
        }

        /// <summary>
        /// 微信授权登录类API
        /// </summary>
        /// <param name="oAuthApi"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OAuth([FromBody]Common.OAuthApi oAuthApi)
        {
            if (oAuthApi == null)
                return Json(new ResultsJson(new Message(CodeMessage.PostNull, "PostNull"), null));
            return Json(Global.BUSS.BussResults(ApiType.OAuthApi,
                                                oAuthApi.token,
                                                oAuthApi.method,
                                                oAuthApi.param));
        }
    }
}
