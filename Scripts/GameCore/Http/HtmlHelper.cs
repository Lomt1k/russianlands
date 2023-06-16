using MarkOne.Scripts.GameCore.Http.AdminService;
using Obisoft.HSharp.Models;

namespace MarkOne.Scripts.GameCore.Http;
public record struct InputFieldInfo(string id, string placeholder, bool isHidden = false);

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

    public static HDoc CreateErrorPage(string title, string error, string backButtonUrl)
    {
        var document = CreateDocument(title);
        document["html"]["body"].AddProperties(StylesHelper.CenterScreenParent());
        var div = new HTag("div", StylesHelper.CenterScreenBlock(700, 250))
            {
                { "h1", error },
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

    public static HTag CreateForm(string url, string page, string buttonText, InputFieldInfo inputField)
    {
        var form = new HTag("form", new HProp("action", url), new HProp("method", "GET"));
        form.AddChild("div");

        var div = new HTag("div")
        {
            { "input", new HProp("id", "page"), new HProp("name", "page"), new HProp("value", page), new HProp("type", "hidden") },
            { 
                "input", new HProp("id", inputField.id), new HProp("name", inputField.id), new HProp("placeholder", inputField.placeholder),
                new HProp("style", "border: 2px solid gray; border-radius: 4px; height: 50px; margin-right: 20px; font-size:28px;")
            },
            { "button", buttonText, StylesHelper.Button("#808080", 24) },
        };

        form.AddChild(div);
        return form;
    }

    public static HTag CreateForm(string url, string page, string header, string buttonText, params InputFieldInfo[] inputFields)
    {
        var form = new HTag("form", new HProp("action", url), new HProp("method", "GET"));
        form.Add("div").Add("h1", header);

        var fieldsDiv = form.Add("div");
        fieldsDiv.Add("input", new HProp("id", "page"), new HProp("name", "page"), new HProp("value", page), new HProp("type", "hidden"));

        foreach (var input in inputFields)
        {
            var div = new HTag("div")
            {
                { 
                    "input",
                    new HProp("id", input.id), new HProp("name", input.id), new HProp("placeholder", input.placeholder),
                    new HProp("style", "border: 2px solid gray; border-radius: 4px; height: 50px; margin-bottom: 10px; font-size:28px;")
                }
            };
            if (input.isHidden)
            {
                div["input"].AddProperties(new HProp("type", "hidden"));
            }
            form.AddChild(div);
        }

        var buttonDiv = new HTag("div")
        {
            { "button", buttonText, StylesHelper.Button("#808080", 24) }
        };
        form.AddChild(buttonDiv);

        return form;
    }

}
