using Newtonsoft.Json;
using Ticket_Server.Common;
using Ticket_Server.Dao;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using Senparc.Weixin.MP.AdvancedAPIs;
using System.Collections.Generic;

namespace Ticket_Server.Buss
{
    public class TicketBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.TicketApi;
        }
        public bool NeedCheck()
        {
            return true;
        }

        public object Do_GetTicketList(object param)
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
            TicketDao ticketDao = new TicketDao();
            ListResult listResult = ticketDao.getListByOpenId(openId);

            return listResult;
        }

        public object Do_InsertTicket(object param)
        {
            InsertListParam listParam = JsonConvert.DeserializeObject<InsertListParam>(param.ToString());
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

            TicketDao ticketDao = new TicketDao();

            return ticketDao.insertTicket(openId,listParam);
        }

    }
    public class ListParam
    {
        public string token;
    }

    public class ListResult
    {
        public List<ListItem> tabPaneOneData;//待处理
        public List<ListItem> tabPaneTwoData;//审批中
        public List<ListItem> tabPaneThreeData;//已完成
    }

    public class ListItem
    {
        public string ticketNum;//小票编码
        public string img;//小票图片
        public string state;//状态
        public string ticketPrice;//小票总价
    }

    public class InsertListParam
    {
        public string token;
        public string ticketNum;//小票编码
        public string imgbasesrc;//小票图片
        public string shopName;//店名
        public List<InsertBrandParam> goodsAll;//已完成
    }
    public class InsertBrandParam
    {
        public string goodsName;//对应品牌
        public string goodsPrice;//对应品牌商品价格和
    }
}
