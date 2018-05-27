using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticket_Server.Common
{
    /// <summary>
    /// 返回信息对照
    /// </summary>
    public enum CodeMessage
    {
        OK = 0,
        PostNull = -1,


        NotFound = 404,
        InnerError = 500,

        SenparcCode = 1000,

        GetUserError = 3000,

        InvalidToken = 4000,
        InvalidMethod = 4001,
        InvalidParam = 4002,
    }
}
