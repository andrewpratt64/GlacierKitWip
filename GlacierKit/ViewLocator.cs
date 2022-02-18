using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GlacierKit.ViewModels;
using GlacierKitCore.Attributes;
using System;
using System.Reflection;

namespace GlacierKit
{
    public class ViewLocator : IDataTemplate
    {
        public IControl Build(object data)
        {
            Type viewModelType = data.GetType();
            var viewName = viewModelType.FullName!.Replace("ViewModel", "View");
            Type? type = Assembly.GetAssembly(viewModelType)?.GetType(viewName);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + viewName };
            }
        }

        public bool Match(object data)
        {
            return data.GetType().IsDefined(typeof(GKViewModel), true);
        }
    }
}
