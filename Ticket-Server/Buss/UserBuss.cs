using Newtonsoft.Json;
using Ticket_Server.Common;
using Ticket_Server.Dao;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Ticket_Server.Buss
{
    public class UserBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.UserApi;
        }

        public bool NeedCheck()
        {
            return true;
        }
        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetQRCoder(object param)
        {
            ListParam listParam = JsonConvert.DeserializeObject<ListParam>(param.ToString());
            if (listParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
#if DEBUG
            var openId = listParam.token;
#endif
#if !DEBUG
            AppBag appBag = AppContainer.GetAppBag(listParam.token);
            if (appBag==null)
            {
                throw new ApiException(CodeMessage.GetUserError, "GetUserError");
            }
            var openId = appBag.Values;
#endif

            UserDao userDao = new UserDao();
            string s = userDao.getQRCoder(openId);
            if (s != "")
            {
                UrlResult ur = new UrlResult();
                ur.url = s;

                return ur;
            }
            else
            {
                throw new ApiException(CodeMessage.QRCoderError, "QRCoderError");
            }

        }

        /// <summary>
        /// 根据小票编号获取小票信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_UpdateQRCoder(object param)
        {
            ItemParam itemParam = JsonConvert.DeserializeObject<ItemParam>(param.ToString());
            if (itemParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
#if DEBUG
            var openId = itemParam.token;
#endif
#if !DEBUG
                AppBag appBag = AppContainer.GetAppBag(itemParam.token);
                if (appBag==null)
                {
                    throw new ApiException(CodeMessage.GetUserError, "GetUserError");
                }
                var openId = appBag.Values;
#endif

            UserDao userDao = new UserDao();

            UrlResult ur = new UrlResult();
            ur.url = userDao.updateQRCode(openId);

            return ur;
        }
    }
    public class UrlResult
    {
        public string url;
    }
}
