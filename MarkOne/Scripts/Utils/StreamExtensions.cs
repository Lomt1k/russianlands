using System.IO;

namespace MarkOne.Scripts.Utils;
public static class StreamExtensions
{
    public static byte[] ReadAllBytes(this Stream stream)
    {
        byte[] bytes;
        using (var binaryReader = new BinaryReader(stream))
        {
            bytes = binaryReader.ReadBytes((int)stream.Length);
        }
        return bytes;
    }

}
