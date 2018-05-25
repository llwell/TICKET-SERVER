using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticket_Server.Common
{
    /// <summary>
    /// API类型分组
    /// </summary>
    public enum ApiType
    {
        UserApi,
        TicketApi,
    }

    /// <summary>
    /// 微信用户类API
    /// </summary>
    public class UserApi
    {
        public string method;
        public string token;
        public object param;
    }

    /// <summary>
    /// 团购票据类API
    /// </summary>
    public class TicketApi
    {
        public string method;
        public string token;
        public object param;
    }
}
