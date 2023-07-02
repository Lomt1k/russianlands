using System;

namespace MarkOne.ViewModels.UserControls;

[AttributeUsage(AttributeTargets.Property)]
internal class IgnoreInEditorAttribute : Attribute
{
}
