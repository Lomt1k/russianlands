using System.Text;

namespace StatViewer.Scripts;
public class FilterModel
{
    public string regVersion { get; set; } = string.Empty;
    public string regInfo { get; set; } = string.Empty;

    public bool isEmpty => string.IsNullOrWhiteSpace(regVersion) && string.IsNullOrWhiteSpace(regInfo);

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(regVersion))
        {
            sb.AppendLine(regVersion);
        }
        if (!string.IsNullOrWhiteSpace(regInfo))
        {
            sb.AppendLine(regInfo);
        }
        return sb.ToString();
    }
}
