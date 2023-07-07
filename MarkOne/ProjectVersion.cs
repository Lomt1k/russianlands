namespace MarkOne;

/*
 * Version format can be "x.y" or "x.y.z" where:
 * x - global version
 * y - update number
 * z - patch number (optional)
 */

internal readonly struct ProjectVersion
{
    public static readonly ProjectVersion Current = new ProjectVersion("0.1");

    public byte globalNumber { get; }
    public byte update { get; }
    public byte patch { get; }

    public ProjectVersion(byte _globalNumber, byte _update, byte _patch = 0)
    {
        globalNumber = _globalNumber;
        update = _update;
        patch = _patch;
    }

    public ProjectVersion(string version)
    {
        try
        {
            var array = version.Split('.');
            globalNumber = byte.Parse(array[0]);
            update = byte.Parse(array[1]);
            patch = array.Length > 2 ? byte.Parse(array[2]) : (byte)0;
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("Incorrect version string", ex);
        }

    }

    public string GetView()
    {
        return patch == 0
            ? $"{globalNumber}.{update}"
            : $"{globalNumber}.{update}.{patch}";
    }

    public override string ToString()
    {
        return $"{globalNumber}.{update}.{patch}";
    }

    public override bool Equals(object? obj)
    {
        if (obj != null && obj is ProjectVersion other)
        {
            return globalNumber == other.globalNumber
                && update == other.update
                && patch == other.patch;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}
