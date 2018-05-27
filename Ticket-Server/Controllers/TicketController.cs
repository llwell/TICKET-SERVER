using System;
using Microsoft.AspNetCore.Mvc;
using Ticket_Server.Common;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.TenPayLibV3;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP;
using Senparc.Weixin.HttpUtility;

namespace Ticket_Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class TicketController : Controller
    {
        /// <summary>
        /// 微信用户类API
        /// </summary>
        /// <param name="userApi"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Users([FromBody]Common.UserApi userApi)
        {
            if (userApi == null)
                return Json(new ResultsJson(new Message(CodeMessage.PostNull, "PostNull"), null));
            return Json(Global.BUSS.BussResults(ApiType.UserApi,
                                                userApi.token,
                                                userApi.method, 
                                                userApi.param));
        }

        /// <summary>
        /// 团购票据类API
        /// </summary>
        /// <param name="ticketApi"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Ticket([FromBody]TicketApi ticketApi)
        {
            if (ticketApi == null)
                return Json(new ResultsJson(new Message(CodeMessage.PostNull, "PostNull"), null));
            return Json(Global.BUSS.BussResults(ApiType.TicketApi,
                                                ticketApi.token,
                                                ticketApi.method,
                                                ticketApi.param));
        }

        

    }

}
