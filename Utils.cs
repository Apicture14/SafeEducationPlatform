using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System;
using System.Text.Json;
using HtmlAgilityPack;

namespace ConsoleApp1;

public class Utils
{
    public string UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 Edg/112.0.1722.58";

    public const string login_url =
        "https://appapi.xueanquan.com/usercenter/api/v3/wx/login?checkShowQrCode=true&tmp=false";

    public const string homeworkcount_url =
        "https://yyapi.xueanquan.com/hunan/api/v1/StudentHomeWork/NoFinishHomeWorkCount";

    public const string homeworklist_url = "https://yyapi.xueanquan.com/hunan/safeapph5/api/v1/homework/homeworklist";

    public const string sign_url = "https://huodongapi.xueanquan.com/p/hunan/Topic/topic/platformapi/api/v1/records/sign";

    public HttpWebRequest req_log = (HttpWebRequest)HttpWebRequest.Create(login_url);
    
    public HttpWebRequest workc_log = (HttpWebRequest)HttpWebRequest.Create(homeworkcount_url);
    public HttpWebRequest workl_log = (HttpWebRequest)HttpWebRequest.Create(homeworklist_url);
    public HttpWebRequest sign_req = (HttpWebRequest)HttpWebRequest.Create(sign_url);

    public static byte[] ToBytes(string t){
        return Encoding.UTF8.GetBytes(t);
    }
    public static string GetData(HttpWebResponse resp){
        StreamReader s = new StreamReader(resp.GetResponseStream());
        return s.ReadToEnd();
    }
    public void Init()
    {
        req_log.CookieContainer = new CookieContainer();
        req_log.ContentType = "application/json;charset=UTF-8";
        req_log.UserAgent = UserAgent;
        req_log.Method = "POST";

        workl_log.Method = "GET";
        workl_log.UserAgent = UserAgent;
    }
    
    public CookieContainer login(string usr, string pwd)
    {
        string inf = "{username:\""+usr+"\",password:\""+pwd+"\",loginOrigin:1}";
        Stream s = req_log.GetRequestStream();
        StringBuilder sb = new StringBuilder();
        s.Write(Encoding.UTF8.GetBytes(inf));
        HttpWebResponse resp = (HttpWebResponse)req_log.GetResponse();
        // Console.WriteLine("*---"+resp);
        return req_log.CookieContainer;
    }
    public List<Dictionary<string,dynamic>> FormatResp(CookieContainer resp){
        workl_log.CookieContainer = resp;
        HttpWebResponse respo = (HttpWebResponse)workl_log.GetResponse();
        Thread.Sleep(1000);
        StreamReader sr = new StreamReader(respo.GetResponseStream());
        string works = sr.ReadToEnd();
        Console.WriteLine(works);
        //Console.WriteLine(works);
        List<Dictionary<string,dynamic>> lworks = JsonSerializer.Deserialize<List<Dictionary<string,dynamic>>>(works);
        return lworks;
    }

    public string DisplayWorklist(List<Dictionary<string,dynamic>> worklist){
        int j = 0;
        foreach (var i in worklist){
            
            string msg = $"[{j}] {i["title"]} -> From: {i["publishDateTime"]} To: {i["finishTime"]} | Finished: {i["finishStatus"]}";
            Console.WriteLine(msg);
            j++;
        }
        //Console.Write("TargetWork:");
        
        return (worklist[1]["url"].GetString().Replace("index","jiating"));
    }

    public string GetSpecialId(string WorkUrl,CookieContainer cookie){
        HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(WorkUrl);
        
        string ftarget = "//iframe";
        string ttarget = ".//title";
        string target = "/html[1]";

        req.AllowAutoRedirect = false;
        req.Method = "GET";
        req.UserAgent = UserAgent;
        req.CookieContainer = new CookieContainer();
        req.CookieContainer = cookie;
        
        HtmlDocument html = new HtmlDocument();
        HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
        Thread.Sleep(1000);
        Stream s = resp.GetResponseStream();
        string h = new StreamReader(s).ReadToEnd();
        Console.WriteLine(h);

        HtmlNode d = html.DocumentNode.SelectSingleNode(target);

        //Console.WriteLine(d.ChildNodes[4].Attributes["class"].Value);

        
        return "";
        //return (string)html.DocumentNode.SelectSingleNode(target).InnerHtml;

    }
    
}