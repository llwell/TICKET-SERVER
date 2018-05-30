using Aliyun.OSS;
using Com.ACBC.Framework.Database;
using QRCoder;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System.Data;
using System.Drawing;
using System.IO;
using Ticket_Server.Common;

namespace Ticket_Server.Dao
{
    public class UserDao
    {
        private string path = System.Environment.CurrentDirectory;

        public UserDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }
        public string getQRCoder(string openId)
        {
            string sql = "select qrcode from t_daigou_user where openId = '" + openId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_user").Tables[0];
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        public void insertUser(OAuthUserInfo userInfo)
        {
            string sql = "select * from t_daigou_user where openId ='"+userInfo.openid+"'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_daigou_ticket").Tables[0];
            if (dt.Rows.Count == 0)
            {
                string insql = "insert into t_daigou_user(openId,nickname,sex,province,city,country,headimgurl,drawCode,qrcode) " +
                    "values('" + userInfo.openid + "','" + userInfo.nickname + "','" + userInfo.sex + "'," +
                    "'" + userInfo.province + "','" + userInfo.city + "','" + userInfo.country + "'," +
                    "'" + userInfo.headimgurl + "','" + initQRCoder(userInfo.openid) + "','"+ Global.OssUrl + Global.OssDir + userInfo.openid + ".jpg" + "')";
                DatabaseOperationWeb.ExecuteDML(insql);
            }
        }


        public string initQRCoder(string openId)
        {
            try
            {
                string drawCode = System.Guid.NewGuid().ToString("N");
                string fileName = openId + ".jpg";
                string strCode = "http://www.walys.com";
                QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
                QRCode qrcode = new QRCode(qrCodeData);

                qrcode.GetGraphic(5, Color.Black, Color.White, null, 15, 6, false).Save(path + "\\" + fileName);

                OssClient client = OssManager.GetInstance();
                ObjectMetadata metadata = new ObjectMetadata();
                // 可以设定自定义的metadata。
                metadata.ContentType = ".jpg";
                metadata.UserMetadata.Add("uname", "airong");
                metadata.UserMetadata.Add("fromfileName", fileName);
                using (var fs = File.OpenRead(path + "\\" + fileName))
                {
                    var ret = client.PutObject(Global.OssBucket, Global.OssDir + fileName, fs, metadata);
                }
                return drawCode;
            }
            catch (System.Exception)
            {
                return "";
            }
            
        }

        public string updateQRCode(string openId)
        {
            string drawCode = initQRCoder(openId);
            if (drawCode!="")
            {
                string sql = "update t_daigou_user set drawCode = '"+ drawCode + "' where openId ='" + openId + "'";
                DatabaseOperationWeb.ExecuteDML(sql);
            }
            return Global.OssUrl + Global.OssDir + openId + ".jpg";
        }
    }
}
