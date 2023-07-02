using System;

namespace GameDataEditor.ViewModels.UserControls;

[AttributeUsage(AttributeTargets.Property)]
internal class IgnoreInEditorAttribute : Attribute
{
}
