using MarkOne.Scripts.GameCore.Http.AdminService;
using Obisoft.HSharp.Models;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Http;
public class HtmlFormBuilder
{
    private HTag _form;

    public HtmlFormBuilder(string buttonUrl)
    {
        _form = new HTag("form", new HProp("action", buttonUrl), new HProp("method", "GET"));
    }

    public HtmlFormBuilder AddHeader(string header)
    {
        _form.Add("div").Add("h1", header);
        return this;
    }

    public HtmlFormBuilder AddHiddenInputIfNotNull(string id, string? value)
    {
        if (value is not null)
        {
            return AddHiddenInput(id, value);
        }
        return this;
    }

    public HtmlFormBuilder AddHiddenInput(string id, string value)
    {
        _form.Add("div").Add("input", new HProp("id", id), new HProp("name", id), new HProp("value", value), new HProp("type", "hidden"));
        return this;
    }    

    public HtmlFormBuilder AddInput(string id, string placeholder, string defaultValue = "", bool fullWidth = false)
    {
        var input = new HTag("input",
            new HProp("id", id), new HProp("name", id), new HProp("placeholder", placeholder),
            new HProp("style", "border: 2px solid gray; border-radius: 4px; height: 50px; margin-bottom: 10px; font-size:28px;"
            + (fullWidth ? " width:100%" : string.Empty)));
        if (!string.IsNullOrEmpty(defaultValue))
        {
            input.AddProperties(new HProp("value", defaultValue));
        }
        _form.Add("div").Add(input);        
        return this;
    }

    public HtmlFormBuilder AddComboBox(string id, IEnumerable<string> options)
    {
        var select = new HTag("select", new HProp("id", id), new HProp("name", id),
            new HProp("style", "border: 2px solid gray; border-radius: 4px; height: 50px; margin-bottom: 10px; font-size:28px;"));
        foreach (var option in options)
        {
            select.Add("option", option, new HProp("value", option));
        }
        _form.Add("div").Add(select);
        return this;
    }

    public HtmlFormBuilder AddTextArea(string id, string placeholder, string defaultValue = "", int rows = 2, bool fullWidth = false)
    {
        var textArea = string.IsNullOrEmpty(defaultValue) ? new HTag("textarea") : new HTag("textarea", defaultValue);
        textArea.AddProperties(
            new HProp("name", id), new HProp("placeholder", placeholder), new HProp("rows", rows.ToString()),
            new HProp("style", "border: 2px solid gray; border-radius: 4px; margin-bottom: 10px; font-size:20px;"
            + (fullWidth ? " width:100%" : string.Empty) ));
        _form.Add("div").Add(textArea);
        return this;
    }

    public HtmlFormBuilder AddButton(string buttonText)
    {
        _form.Add("div").Add("button", buttonText, StylesHelper.Button("#808080", 24));
        return this;
    }

    public HTag GetResult()
    {
        return _form;
    }

}
