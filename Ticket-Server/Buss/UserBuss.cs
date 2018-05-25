using Newtonsoft.Json;
using Ticket_Server.Common;
using Ticket_Server.Dao;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;

namespace Ticket_Server.Buss
{
    public class UserBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.UserApi;
        }

        public object Do_Login(object param)
        {
            LoginParam loginParam = JsonConvert.DeserializeObject<LoginParam>(param.ToString());
            if(loginParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var jsonResult = SnsApi.JsCode2Json(Global.APPID, Global.APPSECRET, loginParam.code);
            if (jsonResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                var manager = Senparc.Weixin.Cache.Redis.RedisManager.Manager;
                
                AccessTokenContainer.Register(Global.APPID, Global.APPSECRET);
                var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);
                return new { sessionId = sessionBag.Key };
            }
            else
            {
                throw new ApiException(CodeMessage.SenparcCode, jsonResult.errmsg);
            }
        }

    }

    public class LoginParam
    {
        public string code;
    }

}
