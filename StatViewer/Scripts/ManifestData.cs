using SQLite;

namespace StatViewer.Scripts;
[Table("Manifest")]
internal class ManifestData
{
    [PrimaryKey]
    public string fileName { get; set; } = string.Empty;
    public long fileSize { get; set; }
}
