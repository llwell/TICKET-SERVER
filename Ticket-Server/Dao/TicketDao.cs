using Aliyun.OSS;
using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Ticket_Server.Buss;
using Ticket_Server.Common;

namespace Ticket_Server.Dao
{
    public class TicketDao
    {
        private string path = System.Environment.CurrentDirectory;
        public TicketDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }

        public ListResult getListByOpenId(string openId)
        {
            string sql = "select * from t_daigou_ticket where openId = '" + openId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            ListResult listResult = new ListResult();
            List<ListItem> tabPaneOneData = new List<ListItem>();//待处理
            List<ListItem> tabPaneTwoData = new List<ListItem>();//审批中
            List<ListItem> tabPaneThreeData = new List<ListItem>();//已完成
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ListItem listItem = new ListItem();
                listItem.img = dt.Rows[i]["img"].ToString();
                listItem.state = dt.Rows[i]["img"].ToString();
                listItem.ticketNum = dt.Rows[i]["ticketCode"].ToString();
                listItem.ticketPrice = getTicketPrice(dt.Rows[i]["ticketCode"].ToString()).ToString();
                switch (dt.Rows[i]["status"].ToString())
                {
                    case "0":
                        listItem.state = "待处理";
                        tabPaneOneData.Add(listItem);
                        break;
                    case "1":
                        listItem.state = "处理中";
                        tabPaneTwoData.Add(listItem);
                        break;
                    case "2":
                        listItem.state = "处理完成";
                        tabPaneThreeData.Add(listItem);
                        break;
                    //case "3":
                    //    listItem.state = "付款完成";
                    //    tabPaneOneData.Add(listItem);
                    //    break;
                    case "8":
                        listItem.state = "录入错误";
                        tabPaneOneData.Add(listItem);
                        break;
                    case "9":
                        listItem.state = "小票被退回";
                        tabPaneOneData.Add(listItem);
                        break;
                    default:
                        break;
                }
            }
            listResult.tabPaneOneData = tabPaneOneData;
            listResult.tabPaneTwoData = tabPaneTwoData;
            listResult.tabPaneThreeData = tabPaneTwoData;
            return listResult;
        }

        public double getTicketPrice(string ticketCode)
        {
            string sql = "select sum(IFNULL(price,0)) from t_daigou_brand where ticketCode = '" + ticketCode + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                return Convert.ToDouble(dt.Rows[0][0]);
            }
            else
            {
                return 0;
            }
        }

        public TicketParam getTicketItem(string openId, string ticketCode)
        {
            string sql = "select b.brand,b.price,t.img,t.ticketCode,t.shopName from t_daigou_brand b,t_daigou_ticket t where b.ticketCode =t.ticketCode and b.ticketCode = '" + ticketCode + "' and t.openId = '" + openId + "'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                TicketParam tp = new TicketParam();
                tp.imgbasesrc = dt.Rows[0]["img"].ToString();
                tp.shopName = dt.Rows[0]["shopName"].ToString();
                tp.ticketNum = dt.Rows[0]["ticketCode"].ToString();
                tp.goodsAll = new List<BrandParam>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BrandParam bp = new BrandParam();
                    bp.goodsName = dt.Rows[i]["brand"].ToString();
                    bp.goodsPrice = dt.Rows[i]["price"].ToString();
                    tp.goodsAll.Add(bp);
                }
                return tp;
            }
            else
            {
                return null;
            }
        }

        public object insertTicket(string openId, TicketParam listParam)
        {
            string fileName = listParam.ticketNum + ".jpg";
            string base64String = listParam.imgbasesrc.Split(",")[1];
            if (saveImageByBase64String(base64String, fileName))
            {
                if (!updateImgToOSS(fileName))
                {
                    return CodeMessage.updateOssError;
                }
            }
            else
            {
                return CodeMessage.updateImgError;
            }
            try
            {
                string sql = "insert into t_daigou_ticket(openId,createTime,img,ticketCode,shopName,status) " +
                    "values('" + openId + "',now(),'" + OssManager._ossUrl+OssManager._ossDir + fileName + "','" + listParam.ticketNum + "','" + listParam.shopName + "','0')";
                if (DatabaseOperationWeb.ExecuteDML(sql))
                {
                    ArrayList al = new ArrayList();
                    foreach (var goods in listParam.goodsAll)
                    {
                        string sql1 = "insert into t_daigou_brand(ticketCode,brand,price) values('" + listParam.ticketNum + "','" + goods.goodsName + "','" + goods.goodsPrice + "')";
                        al.Add(sql1);
                    }
                    if (!DatabaseOperationWeb.ExecuteDML(al))
                    {
                        return CodeMessage.insertTicketError;
                    }
                }
                else
                {
                    return CodeMessage.insertTicketError;
                }
            }
            catch (Exception)
            {
                return CodeMessage.insertTicketError;
            }
            return CodeMessage.insertTicketSuccess;
        }

        /// <summary>
        /// 将Base64位码保存成图片
        /// </summary>
        /// <param name="base64Img">Base64位码</param>
        /// <param name="fileName">图片名</param>
        /// <returns></returns>
        public bool saveImageByBase64String(string base64Img,string fileName)
        {
            try
            {
                byte[] bt = Convert.FromBase64String(base64Img);//获取图片base64
                string ImageFilePath = path+"\\"+ fileName;
                File.WriteAllBytes(ImageFilePath, bt); //保存图片到服务器，然后获取路径 
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        //上传图片到
        private bool updateImgToOSS(string fileName)
        {
            try
            {
                string imgFileName = fileName;
                string contentType = Path.GetExtension(imgFileName);

                OssClient client = OssManager.GetInstance();
                ObjectMetadata metadata = new ObjectMetadata();
                // 可以设定自定义的metadata。
                metadata.ContentType = contentType;
                metadata.UserMetadata.Add("uname", "airong");
                metadata.UserMetadata.Add("fromfileName", imgFileName);
                using (var fs = File.OpenRead(path + "\\" + fileName))
                {
                    var ret = client.PutObject(OssManager._bucket, OssManager._ossDir + imgFileName, fs, metadata);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }
    }
}
