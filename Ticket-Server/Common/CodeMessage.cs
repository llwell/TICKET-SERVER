﻿using System;
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

        insertTicketSuccess =5000,//上传小票信息到数据库成功
        updateImgError = 5001,//上传图片失败
        updateOssError = 5002,//上传图片到oss失败
        insertTicketError = 5003,//上传小票信息到数据库失败
        repeatTicketError = 5004,//小票号重复
    }
}
