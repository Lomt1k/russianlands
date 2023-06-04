using MarkOne.Scripts.GameCore.Http.AdminService;
using Obisoft.HSharp;
using Obisoft.HSharp.Models;

namespace MarkOne.Scripts.GameCore.Http;
public class HtmlHelper
{
    public static HDoc CreateDocument(string title = "Example")
    {
        return HtmlConvert.DeserializeHtml($@"
        <html>
            <head>
                <meta charset={"\"utf-8\""}>
                <meta name={"\"viewport\""}>
                <title>{title}</title>
            </head>
        <body>
        </body>
        </html>");
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
