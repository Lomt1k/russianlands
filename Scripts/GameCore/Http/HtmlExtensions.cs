using Obisoft.HSharp.Models;

public static class HtmlExtensions
{
    public static HTag Add(this HTag htag, string tag)
    {
        var newTag = new HTag(tag);
        htag.AddChild(newTag);
        return newTag;
    }

    public static HTag Add(this HTag htag, HTag newTag)
    {
        htag.AddChild(newTag);
        return newTag;
    }

    public static HTag Add(this HTag htag, string tag, params HProp[] properties)
    {
        var newTag = new HTag(tag);
        newTag.AddProperties(properties);
        htag.AddChild(newTag);
        return newTag;
    }

    public static HTag Add(this HTag htag, string tag, string innerContent)
    {
        var newTag = new HTag(tag, innerContent);
        htag.AddChild(newTag);
        return newTag;
    }

    public static HTag Add(this HTag htag, string tag, string innerContent, params HProp[] properties)
    {
        var newTag = new HTag(tag, innerContent);
        newTag.AddProperties(properties);
        htag.AddChild(newTag);
        return newTag;
    }

}
