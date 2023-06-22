using MarkOne.Scripts.GameCore.Http.AdminService;
using Obisoft.HSharp.Models;

namespace MarkOne.Scripts.GameCore.Http;

public static class HtmlHelper
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

    public static HDoc CreateMessagePage(string title, string error, string backButtonUrl)
    {
        var document = CreateDocument(title);
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());
        var div = new HTag("div", StylesHelper.CenterScreenBlock(700, 250))
        {
            { "h3", error },
            CreateLinkButton("<< Back", backButtonUrl),
        };
        document["html"]["body"].Add(div);
        return document;
    }

    public static HTag CreateLinkButton(string text, string url, string color = "#4CAF50", int size = 24)
    {
        var result = new HTag("a", new HProp("href", url));
        var button = new HTag("button", text);
        button.AddProperties(StylesHelper.Button(color, size));
        result.AddChild(button);
        return result;
    }

    public static HTag CreateSimpleForm(string url, string page, string buttonText, string id, string placeholder)
    {
        var form = new HTag("form", new HProp("action", url), new HProp("method", "GET"));
        form.AddChild("div");

        var div = new HTag("div")
        {
            { "input", new HProp("id", "page"), new HProp("name", "page"), new HProp("value", page), new HProp("type", "hidden") },
            { 
                "input", new HProp("id", id), new HProp("name", id), new HProp("placeholder", placeholder),
                new HProp("style", "border: 2px solid gray; border-radius: 4px; height: 50px; margin-right: 20px; font-size:28px;")
            },
            { "button", buttonText, StylesHelper.Button("#808080", 24) },
        };

        form.AddChild(div);
        return form;
    }

}
