﻿using Newtonsoft.Json;
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

        /// <summary>
        /// 获取小票列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 保存小票信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_InsertTicket(object param)
        {
            TicketParam listParam = JsonConvert.DeserializeObject<TicketParam>(param.ToString());
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

            return ticketDao.insertTicket(openId, listParam);
        }


        /// <summary>
        /// 根据小票编号获取小票信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Do_GetTicketItem(object param)
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

            TicketDao ticketDao = new TicketDao();

            return ticketDao.getTicketItem(openId, itemParam.ticketNum);
        }


        public object updateTicket(object param)
        {
            TicketParam listParam = JsonConvert.DeserializeObject<TicketParam>(param.ToString());
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

            return ticketDao.insertTicket(openId, listParam);
        }
    }
    public class ListParam
    {
        public string token;
    }
    public class ItemParam
    {
        public string token;
        public string ticketNum;//小票编码
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

    public class TicketParam
    {
        public string token;
        public string ticketNum;//小票编码
        public string imgbasesrc;//小票图片
        public string shopName;//店名
        public List<BrandParam> goodsAll;//已完成
    }
    public class BrandParam
    {
        public string goodsName;//对应品牌
        public string goodsPrice;//对应品牌商品价格和
    }
}
