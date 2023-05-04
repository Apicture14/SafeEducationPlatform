using System.Text.Json;
using System.Text;
using System.Net;

namespace Utils;

public class UserUtils{
    
    public delegate void FuncHandler(Dictionary<string,string> para);
    public static void ExecEach(string filepath,FuncHandler func){
        if (!File.Exists(filepath)){return;}
        List<Dictionary<string,string>> stus = JsonSerializer.Deserialize<List<Dictionary<string,string>>>(File.ReadAllText(filepath));
        foreach (var s in stus){
            func(s);
        }
    }
    public static bool OneStepSign(string usr,string pwd){
        WebUtils u = new WebUtils();
        u.Init();
        u.cc = u.login(usr,pwd);

        string specialId =  u.GetSpecialId(u.DisplayWorklist(u.FormatResp(u.cc)),u.cc);

        u.sign_req.CookieContainer = u.cc;
        u.sign_req.ContentType = "application/json; charset=utf-8";
        u.sign_req.UserAgent = u.UserAgent;
        u.sign_req.Method = "POST";

        Stream s = u.sign_req.GetRequestStream();
        s.Write(WebUtils.ToBytes($"\"specialId\":{specialId},\"step\":1"));
        HttpWebResponse sresp = (HttpWebResponse)u.sign_req.GetResponse();
        Console.WriteLine(WebUtils.GetData(sresp));
        return JsonSerializer.Deserialize<Dictionary<string,dynamic>>(WebUtils.GetData(sresp))["result"];
            
        

    }
}
