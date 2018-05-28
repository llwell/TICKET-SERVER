using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticket_Server.Dao
{
    public static class OssManager
    {
        private static string _accessId = "LTAIfASZSnhH7MjZ";
        private static string _accessKey = "Rdqi0iEjhIMSDBKvEIji5zlxStpJUK";
        private static string _http = "http://oss-cn-beijing.aliyuncs.com";
        public static string _bucket = "ecc-product";
        public static string _ossUrl = "http://ecc-product.oss-cn-beijing.aliyuncs.com/";
        public static string _ossDir = "daigou/";

        private static OssClient ossClient;

        public static OssClient GetInstance()
        {
            if (ossClient == null)
            {
                ossClient = new OssClient(_http, _accessId, _accessKey);
            }
            return ossClient;
        }
    }
}
