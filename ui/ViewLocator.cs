using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using algo_vis.ui.ViewModels;

namespace algo_vis.ui;

public class ViewLocator : IDataTemplate
{
  public Control? Build(object? param)
  {
    if (param is null)
      return null;

    // 1) get the VM’s runtime type
    var vmType = param.GetType();

    // 2) if it’s a generic, reduce it to its open‐generic definition
    var typeToInspect = vmType.IsGenericType
      ? vmType.GetGenericTypeDefinition()
      : vmType;

    // 3) strip off any "`1", "`2", etc
    var rawName = typeToInspect.Name;
    var tick     = rawName.IndexOf('`');
    if (tick > 0)
       rawName = rawName.Substring(0, tick);

    // 4) swap "ViewModel" → "View"
    var viewName = rawName.Replace("ViewModel", "View", StringComparison.Ordinal);

    var viewNs = typeToInspect.Namespace?
                   .Replace("ViewModels", "Views", StringComparison.Ordinal) 
                 ?? throw new InvalidOperationException("No namespace on VM");

    // 6) build the full name and try to find it
    var fullTypeName = $"{viewNs}.{viewName}";
    var viewType     = Type.GetType(fullTypeName);

    if (viewType != null && typeof(Control).IsAssignableFrom(viewType))
      return (Control)Activator.CreateInstance(viewType)!;

    // fallback so you see what you missed:
    return new TextBlock { Text = $"View not found: {fullTypeName}" };
  }

  public bool Match(object? data) => data is ViewModelBase;
}