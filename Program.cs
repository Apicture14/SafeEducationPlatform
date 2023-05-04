using System.Net;
using HtmlAgilityPack;
using System.Text;
using System.Text.Json;
using Utils;

string specialId = "";

WebUtils u = new WebUtils();
u.Init();
u.cc = u.login("yinboge","11223344qQ");

specialId =  u.GetSpecialId(u.DisplayWorklist(u.FormatResp(u.cc)),u.cc);

Console.WriteLine("X:"+specialId);

u.sign_req.CookieContainer = u.cc;
u.sign_req.ContentType = "application/json; charset=utf-8";
u.sign_req.UserAgent = u.UserAgent;
u.sign_req.Method = "POST";

Stream s = u.sign_req.GetRequestStream();
s.Write(WebUtils.ToBytes($"\"specialId\":{specialId},\"step\":1"));
HttpWebResponse sresp = (HttpWebResponse)u.sign_req.GetResponse();
Console.WriteLine(WebUtils.GetData(sresp));

