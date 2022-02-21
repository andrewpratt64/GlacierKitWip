using Dock.Model.Core;
using Dock.Model.ReactiveUI.Controls;
using GlacierKitCore.Attributes;
using GlacierKitCore.ViewModels;
using System.Diagnostics;

namespace GlacierKit.ViewModels
{
    [GKViewModel]
    public class MainViewModel : DocumentDock
    {
        public MainViewModel(IFactory? initialFactory = null, IDockable? initialDockable = null)
           : base()
        {
            Factory = initialFactory;

            Id = "Main";
            Title = "Main";

            CanCreateDocument = true;
            CanClose = false;
            CanPin = false;
            CanFloat = false;
            IsCollapsable = false;

            if (initialDockable != null)
            {
                Debug.Assert(Factory != null);
                ActiveDockable = initialDockable;
                VisibleDockables = Factory.CreateList(initialDockable);
            }

            /*CreateDocument = ReactiveCommand.Create(() =>
            {
                Debug.Assert(Factory != null);
                var index = VisibleDockables?.Count + 1;
                var document = new FooViewModel { Id = $"Foo{index}", Title = $"Foo #{index}" };
                Factory.AddDockable(this, document);
                Factory.SetActiveDockable(document);
                Factory.SetFocusedDockable(this, document);
            });*/
        }
    }
}
