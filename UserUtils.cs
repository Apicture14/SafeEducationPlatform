using System.Text.Json;
using System.Text;
using System.Net;
using ClosedXML.Excel;

namespace Utils;

public class UserUtils{
    
    public delegate void FuncHandler(Dictionary<string,string> para);
    public static void SignEach(string filepath){
        if (!File.Exists(filepath)){return;}
        List<Student> stus = JsonSerializer.Deserialize<List<Student>>(File.ReadAllText(filepath));
        int i = 0;
        foreach (var s in stus){
            i++;
            Console.WriteLine(DateTime.Now.ToShortTimeString()+$"====START{i}");
            Console.WriteLine("Name:"+s.name);
            OneStepSign(s.usr,s.pwd);
            Console.WriteLine(DateTime.Now.ToShortTimeString()+$"====END{i}");
        }
    }
    public static bool OneStepSign(string usr,string pwd){
        WebUtils u = new WebUtils();
        u.Init();
        u.cc = u.login(usr,pwd);

        string specialId =  u.GetSpecialId(u.DisplayWorklist(u.FormatResp(u.cc)));

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
    public static string GenJsonFromXlsx(string path){
        if (!File.Exists(path)){throw new FileNotFoundException($"{path} not found");}
        var wb = new XLWorkbook(path);
        var ws = wb.Worksheet(1);
        var cn = ws.Column(1);
        var cu = ws.Column(2);
        var cp = ws.Column(3);
        List<Student> ls = new List<Student>();
        foreach(var c in cn.CellsUsed()){
            Student s = new Student();
            int rn = c.WorksheetRow().RowNumber();

            s.name = (string)c.Value;
            s.usr = (string)cu.Cell(rn).Value;
            s.pwd = (string)cp.Cell(rn).Value;

            ls.Add(s);
        }
        JsonSerializerOptions js = new JsonSerializerOptions(){
            WriteIndented = true
        };
        string json = JsonSerializer.Serialize(ls,js);
        using (FileStream fs = new FileStream("./userconfig.json",FileMode.CreateNew)){
            fs.Write(Encoding.UTF8.GetBytes(json));
            return fs.Name;
        }
    }
}
