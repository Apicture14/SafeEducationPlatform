using System.Net;
using HtmlAgilityPack;
using System.Text;
using System.Text.Json;
using ConsoleApp1;

string specialId = "";

Utils u = new Utils();
u.Init();
CookieContainer cookie = u.login("yinboge","11223344qQ");

specialId =  u.GetSpecialId(u.DisplayWorklist(u.FormatResp(cookie)),cookie);

Console.WriteLine(specialId);

u.sign_req.CookieContainer = cookie;
u.sign_req.ContentType = "application/json; charset=utf-8";
u.sign_req.UserAgent = u.UserAgent;
u.sign_req.Method = "POST";

Stream s = u.sign_req.GetRequestStream();
s.Write(Utils.ToBytes($"\"specialId\":{specialId},\"step\":1"));
HttpWebResponse sresp = (HttpWebResponse)u.sign_req.GetResponse();
Console.WriteLine(Utils.GetData(sresp));
