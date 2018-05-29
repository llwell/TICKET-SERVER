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

        insertTicketSuccess =5000,//上传小票信息到数据库成功
        updateImgError = 5001,//上传图片失败
        updateOssError = 5002,//上传图片到oss失败
        insertTicketError = 5003,//上传小票信息到数据库失败
        repeatTicketError = 5004,//小票号重复

        updateTicketSuccess = 5100,//修改小票信息成功
        updateTicketError = 5101,//修改小票信息到数据库失败
        TicketZeroError = 5102,//未查到票信息
        updateTicketStatusError = 5103,//修改的小票状态不对

        deleteTicketSuccess = 5200,//删除小票信息成功
        deleteTicketError = 5201,//删除小票信息失败

        QRCoderError=6001,//获取二维码失败
    }
}
