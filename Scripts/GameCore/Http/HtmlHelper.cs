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
}
