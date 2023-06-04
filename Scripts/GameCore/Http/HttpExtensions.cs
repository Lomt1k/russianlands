using System;
using System.Net;
using System.Text;

public static class HttpExtensions
{
    //
    // Сводка:
    //     Writes the specified data to the response.
    //     Response is closed and can not be longer modified.
    //
    // Параметры:
    //   response:
    //     HTTP response.
    //
    //   txt:
    //     Text data to write.
    //
    //   mime:
    //     Mime type.
    public static void AsTextUTF8(this HttpListenerResponse response, string txt, string mime = "text/html")
    {
        if (response == null)
        {
            throw new ArgumentNullException("response");
        }

        if (txt == null)
        {
            throw new ArgumentNullException("txt");
        }

        if (mime == null)
        {
            throw new ArgumentNullException("mime");
        }

        byte[] bytes = Encoding.UTF8.GetBytes(txt);
        response.ContentLength64 = bytes.Length;
        response.ContentType = mime;
        response.OutputStream.Write(bytes, 0, bytes.Length);
    }
}
