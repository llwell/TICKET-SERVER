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

        public object Do_GetUser(object param)
        {
            UserParam userParam = JsonConvert.DeserializeObject<UserParam>(param.ToString());
            if(userParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var appBag = AppContainer.GetAppBag(userParam.token);
            if (appBag != null)
            {
                return appBag;
            }
            else
            {
                throw new ApiException(CodeMessage.GetUserError, "GetUserError");
            }
        }

    }

    public class UserParam
    {
        public string token;
    }

}
