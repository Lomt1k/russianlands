using MarkOne.Scripts.GameCore.Http.AdminService;
using Obisoft.HSharp.Models;
using System.Security.Policy;

namespace MarkOne.Scripts.GameCore.Http;
public record struct InputFieldInfo(string id, string? description, string defaultValue = "", bool isHidden = false);

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
        form["div"].AddChild("input", new HProp("id", "page"), new HProp("name", "page"), new HProp("value", page), new HProp("type", "hidden"));

        if (inputField.description is not null)
        {
            form["div"].AddChild("label", inputField.description);
        }
        form["div"].AddChild("input", new HProp("id", inputField.id), new HProp("name", inputField.id), new HProp("value", inputField.defaultValue),
            new HProp("style", "border: 2px solid gray; border-radius: 4px; height: 50px; margin-right: 20px; font-size:28px;"));
        form["div"].AddChild("button", buttonText);
        form["div"]["button"].AddProperties(StylesHelper.Button("#808080", 24));

        return form;
    }

    public static HTag CreateForm(string url, string page, string buttonText, InputFieldInfo[] inputFields)
    {
        var form = new HTag("form", new HProp("action", url), new HProp("method", "GET"));
        form.AddChild("div");
        form["div"].AddChild("input", new HProp("id", "page"), new HProp("name", "page"), new HProp("value", page), new HProp("type", "hidden"));

        foreach (var input in inputFields)
        {
            var div = new HTag("div");
            if (input.description is not null)
            {
                div.AddChild("label", input.description);
            }
            div.AddChild("input", new HProp("id", input.id), new HProp("name", input.id), new HProp("value", input.defaultValue));
            if (input.isHidden)
            {
                div["input"].AddChild("type", "hidden");
            }
            form.AddChild(div);
        }

        var buttonDiv = new HTag("div");
        buttonDiv.AddChild("button", buttonText);
        buttonDiv["button"].AddProperties(StylesHelper.Button("#808080", 24));
        form.AddChild(buttonDiv);

        return form;
    }

}
