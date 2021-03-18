using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using QQBotXClashOfClans_v2;

namespace CocNET
{
    public class TokenApi
    {
        public static void GetNewToken()
        {
            Logger.Instance.AddLog(LogType.Debug, "更新Token中...");
            if (string.IsNullOrEmpty(BaseData.Instance.config["部落冲突"]["Api邮箱"]) || string.IsNullOrEmpty(BaseData.Instance.config["部落冲突"]["Api密码"]))
            {
                throw new Exception("缺少 [部落冲突][Api邮箱] 或者 [部落冲突][Api密码]");
            }
            try
            {
                UTF8Encoding encode = new UTF8Encoding();
                byte[] data = encode.GetBytes("{\"email\":\"" + BaseData.Instance.config["部落冲突"]["Api邮箱"] + "\",\"password\":\"" + BaseData.Instance.config["部落冲突"]["Api密码"] + "\"}");
                HttpWebRequest request = WebRequest.CreateHttp("https://developer.clashofclans.com/api/login");
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                Stream net = request.GetRequestStream();
                net.Write(data, 0, data.Length);
                net.Close();
                var res = (HttpWebResponse)request.GetResponse();
                string session = "";
                for (int i = 0; i < res.Headers.Count; i++)
                {
                    string name = res.Headers.GetKey(i);
                    if (name != "Set-Cookie")
                        continue;
                    string value = res.Headers.Get(i);
                    foreach (var singleCookie in value.Split(','))
                    {
                        var match = Regex.Match(singleCookie, "(.+?)=(.+?);");
                        if (match.Captures.Count == 0)
                            continue;
                        session = match.Groups[2].ToString();
                        break;
                    }
                }
                net = res.GetResponseStream();
                StreamReader reader = new StreamReader(net, Encoding.UTF8);
                var response = reader.ReadToEnd();
                string temptoken = response[response.LastIndexOf("temporaryAPIToken")..].Replace("\"", "");
                temptoken = temptoken.Remove(temptoken.IndexOf(","));
                temptoken = temptoken.Remove(0, 18);
                request = WebRequest.CreateHttp("https://developer.clashofclans.com/api/apikey/list");
                request.Method = "POST";
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Uri("https://developer.clashofclans.com"), new Cookie("session", session));
                request.CookieContainer.Add(new Uri("https://developer.clashofclans.com"), new Cookie("game-api-url", "https://api.clashofclans.com/v1/"));
                request.CookieContainer.Add(new Uri("https://developer.clashofclans.com"), new Cookie("game-api-token", temptoken));
                request.ContentType = "application/json";
                data = encode.GetBytes("{}");
                net = request.GetRequestStream();
                net.Write(data, 0, data.Length);
                net.Close();
                res = (HttpWebResponse)request.GetResponse();
                net = res.GetResponseStream();
                reader = new StreamReader(net, Encoding.UTF8);
                var reply = reader.ReadToEnd();
                net.Close();
                reply = reply.Remove(0, reply.IndexOf("["));
                var result = reply.Split(',');
                foreach (var r in result)
                {
                    if (r.Contains("\"id\""))
                    {
                        string id = r.Replace("\"id\":\"", "").Replace("\"", "").Replace("{", "").Replace("[", "");
                        string payload = "{\"id\":\"" + id + "\"}";
                        data = encode.GetBytes(payload);
                        request = WebRequest.CreateHttp("https://developer.clashofclans.com/api/apikey/revoke");
                        request.Method = "POST";
                        request.CookieContainer = new CookieContainer();
                        request.CookieContainer.Add(new Uri("https://developer.clashofclans.com"), new Cookie("session", session));
                        request.CookieContainer.Add(new Uri("https://developer.clashofclans.com"), new Cookie("game-api-url", "https://api.clashofclans.com/v1/"));
                        request.CookieContainer.Add(new Uri("https://developer.clashofclans.com"), new Cookie("game-api-token", temptoken));
                        request.ContentType = "application/json";
                        net = request.GetRequestStream();
                        net.Write(data, 0, data.Length);
                        net.Close();
                        res = (HttpWebResponse)request.GetResponse();
                        net = res.GetResponseStream();
                        reader = new StreamReader(net, Encoding.UTF8);
                        reply = reader.ReadToEnd();
                        Console.WriteLine(reply);
                        net.Close();
                    }
                }
                BaseData.GetIP(out string newIP);
                data = encode.GetBytes("{\"name\":\"Admin Bot\",\"description\":\"Admin\",\"cidrRanges\":[\"" + newIP + "\"],\"scopes\":null}");
                request = WebRequest.CreateHttp("https://developer.clashofclans.com/api/apikey/create");
                request.Method = "POST";
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Uri("https://developer.clashofclans.com"), new Cookie("session", session));
                request.CookieContainer.Add(new Uri("https://developer.clashofclans.com"), new Cookie("game-api-url", "https://api.clashofclans.com/v1/"));
                request.CookieContainer.Add(new Uri("https://developer.clashofclans.com"), new Cookie("game-api-token", temptoken));
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                net = request.GetRequestStream();
                net.Write(data, 0, data.Length);
                net.Close();
                res = (HttpWebResponse)request.GetResponse();
                net = res.GetResponseStream();
                reader = new StreamReader(net, Encoding.UTF8);
                reply = reader.ReadToEnd();
                net.Close();
                var token = reply[reply.LastIndexOf("\"key\":\"")..];
                token = token.Remove(0, 7);
                token = token.Remove(token.Length - 3);
                BaseData.SetToken(token);
                data = encode.GetBytes("{}");
                request = WebRequest.CreateHttp("https://developer.clashofclans.com/api/logout");
                request.Method = "POST";
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Uri("https://developer.clashofclans.com"), new Cookie("session", session));
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                net = request.GetRequestStream();
                net.Write(data, 0, data.Length);
                net.Close();
                res = (HttpWebResponse)request.GetResponse();
                net = res.GetResponseStream();
                reader = new StreamReader(net, Encoding.UTF8);
                reply = reader.ReadToEnd();
                net.Close();
                BaseData.UpdateIP();
                Logger.Instance.AddLog(LogType.Debug, "Token更新完毕！新Token为" + BaseData.Instance.config["部落冲突"]["Token"]);
            }
            catch
            {

            }
            
        }
    }
}
