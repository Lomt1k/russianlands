using MarkOne.Scripts.GameCore.Http.AdminService;
using Obisoft.HSharp;
using Obisoft.HSharp.Models;
using System.Web;

namespace MarkOne.Scripts.GameCore.Http;
public class HtmlHelper
{
    public static HDoc CreateDocument(string title = "Example")
    {
        var document = new HDoc();
        document.AddChild("html", new HProp("lang", "ru"), new HProp("dir", "ltr"));
        document["html"].AddChild("head");
        document["html"].AddChild("body");
        document["html"]["head"].AddChild("meta", new HProp("http-equiv", "content-type"), new HProp("charset", "utf-8"), new HProp("content", "text/html; charset=UTF-8;"));
        document["html"]["head"].AddChild("title", title);
        return document;
    }

    public static HTag CreateLinkButton(string text, string url, string color = "#4CAF50", int size = 16)
    {
        var result = new HTag("a", new HProp("href", url));
        var button = new HTag("button", text);
        button.AddProperties(StylesHelper.Button(color, size));
        result.AddChild(button);
        return result;
    }
}
