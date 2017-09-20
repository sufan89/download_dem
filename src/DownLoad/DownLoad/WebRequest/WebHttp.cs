using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using System.Threading;
using System.Windows.Forms;

namespace DownLoad
{
    public class WebHttp
    {
        public WebHttp()
        {
            m_DownLoadList = new List<string>();
        }
        public List<string> m_DownLoadList = null;
        /// <summary>
        /// Http发送Get请求方法
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public  string HttpGet(string Url, string postDataStr, CookieCollection LoginCooikes=null)
        {

            string retString = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                if (LoginCooikes != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(LoginCooikes);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
            }
            catch (Exception ex)
            {

            }
            return retString;
        }
        public  string HttpGetDownLoad(string Url, string fileName, DataGridViewRow pDownRow, CookieCollection LoginCooikes = null)
        {
            string retString = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                if (LoginCooikes != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(LoginCooikes);
                }
                IAsyncResult ar= request.BeginGetResponse(DownloadFinished, new DownLoadState() { FileName = fileName, WebRequestObject = request,DownloadRow= pDownRow });
            }
            catch (Exception ex)
            {
                retString = ex.Message;
            }
            return retString;
        }
        /// <summary>
        /// Http发送Post请求方法
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public  string HttpPost(string Url, string postDataStr, CookieCollection cookie)
        {
            HttpWebRequest request;
            string retString = string.Empty ;
            try
            {
                request = WebRequest.Create(Url) as HttpWebRequest;
                request.Accept = "application/json, text/javascript, */*; q=0.01";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.79 Safari/537.36";
                request.Method = "POST";
                request.Host = "www.gscloud.cn";
                request.KeepAlive = true;
                request.Referer = "http://www.gscloud.cn/sources/list_dataset/421?cdataid=302&pdataid=10&datatype=gdem_utm2";
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.ContentLength = postDataStr.Length;

                if (request.CookieContainer == null)
                {
                    request.CookieContainer = new CookieContainer();
                }
                //Cookie ck = new Cookie("csrftoken", "23mw5HtHge4Itkum4odNsgPTvhNUUYjD", "/", ".gscloud.cn");
                //cookie.Add(ck);
                //ck = new Cookie("_next_", "/", "/", ".gscloud.cn");
                //cookie.Add(ck);
                //ck = new Cookie("_gscu_1565221615", "05463371ulkf7k15", "/", ".gscloud.cn");
                //cookie.Add(ck);
                //ck = new Cookie("_gscs_1565221615", "t05466512i62xft14|pv:5", "/", ".gscloud.cn");
                //cookie.Add(ck);
                //ck = new Cookie("_gscbrs_1565221615", "1", "/", ".gscloud.cn");
                //cookie.Add(ck);
                request.CookieContainer.Add(cookie);
                                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                writer.Write(postDataStr);
                writer.Flush();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码 
                }

                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                retString = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            return retString;
        }
        /// <summary>
        /// 登录获取用户登录的Cookie
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="firstCC"></param>
        /// <returns></returns>
        public  CookieCollection LoginWeb(string url,string userName, string passWord, CookieCollection firstCC)
        {
            CookieCollection cca = new CookieCollection();  
            HttpWebRequest request = null;
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.79 Safari/537.36";
            request.Method = "POST";
            request.Host = "www.gscloud.cn";
            request.KeepAlive = true;
            request.Referer = "http://www.gscloud.cn/accounts/login";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            //如果没有 下面这行代码 将获取不到相应的Cookie （response.Cookies）  
            //其实 不添加这行代码对于获取 数据没有影响，但是过后的POST操作会用到相应的Cookie  
            request.CookieContainer = new CookieContainer();
            //以下两行对于获取Cookie 对于本次测试 没有影响  
            Cookie ck = new Cookie("csrftoken", "23mw5HtHge4Itkum4odNsgPTvhNUUYjD","/", ".gscloud.cn");
            firstCC.Add(ck);
            request.CookieContainer.Add(firstCC);
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            writer.Write(string.Format("csrfmiddlewaretoken=23mw5HtHge4Itkum4odNsgPTvhNUUYjD&next=&userid={0}&password={1}", userName,passWord));
            writer.Flush();
            try
            {
                //响应  
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                int statusInt = response.StatusCode.GetHashCode();
                //相应成功  
                if (response.StatusCode.ToString().ToLower() == "ok")
                {
                    string strtUrl = response.ResponseUri.AbsoluteUri;
                    cca = response.Cookies;
                    string responseText = null;
                    responseText = new StreamReader(response.GetResponseStream(), Encoding.Default).ReadToEnd();
                }
            }
            catch (Exception ex)
            {

            }
            return cca;
        }

         void DownloadFinished(IAsyncResult ar)
        {
            DownLoadState state = ar.AsyncState as DownLoadState;
            DataGridViewRow pDownRow = state.DownloadRow;
            try
            {
                WebResponse response = state.WebRequestObject.EndGetResponse(ar);
                Stream inStream = response.GetResponseStream();
                byte[] buffer = new byte[1024 * 1024];
                Stream outStream = System.IO.File.Create(state.FileName);
                long dataLength = response.ContentLength;
                long temp = 0;
                try
                {
                    pDownRow.Cells["uri"].Value = response.ResponseUri;
                    int l;
                    do
                    {
                        l = inStream.Read(buffer, 0, buffer.Length);
                        temp += l;
                        if (l > 0)
                        {
                            outStream.Write(buffer, 0, l);
                            pDownRow.Cells["progress"].Value = (((double)(temp) / dataLength) * 100).ToString("0.00") + "%";
                        }
                    } while (l > 0);
                    pDownRow.Cells["downloadstate"].Value = EnDownLoadState.DownLoaded;
                }
                catch (Exception ex)
                {
                    pDownRow.Cells["progress"].Value = "下载有误，未完成！:" + ex.Message;
                    pDownRow.Cells["downloadstate"].Value = EnDownLoadState.Error;
                }
                finally
                {
                    if (outStream != null) outStream.Close();
                    if (inStream != null) inStream.Close();
                    if (ar.IsCompleted)
                    {
                        response.Close();
                    }
                    if (m_DownLoadList.Contains(pDownRow.Cells["dataid"].Value.ToString()))
                    {
                        m_DownLoadList.Remove(pDownRow.Cells["dataid"].Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                pDownRow.Cells["progress"].Value = ex.Message;
                pDownRow.Cells["downloadstate"].Value = EnDownLoadState.Error;
            }
            finally
            {
                if (m_DownLoadList.Contains(pDownRow.Cells["dataid"].Value.ToString()))
                {
                    m_DownLoadList.Remove(pDownRow.Cells["dataid"].Value.ToString());
                }
            }
        }
    }
    public class DownLoadState
    {
        public string FileName { get; set; }
        public WebRequest WebRequestObject { get; set; }
        public DataGridViewRow DownloadRow { get; set; }
    }
}
