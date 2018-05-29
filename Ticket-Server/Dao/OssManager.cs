
using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ticket_Server.Common;

namespace Ticket_Server.Dao
{
    public static class OssManager
    {

        private static OssClient ossClient;

        public static OssClient GetInstance()
        {
            if (ossClient == null)
            {
                ossClient = new OssClient(Global.OssHttp, Global.AccessId, Global.AccessKey);
            }
            return ossClient;
        }
    }
}
