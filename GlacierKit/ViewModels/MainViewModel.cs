using Dock.Model.Core;
using Dock.Model.ReactiveUI.Controls;
using GlacierKitCore.Attributes;
using GlacierKitCore.ViewModels;
using GlacierKitCore.ViewModels.EditorWindows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace GlacierKit.ViewModels
{
    [GKViewModel]
    public class MainViewModel : DocumentDock
    {
        [Reactive]
        public Type? NewDocumentType
        { get; set; }


        public MainViewModel() : this(new MainDockFactory(new()), null) { }
        
        public MainViewModel(IFactory? initialFactory, IDockable? initialDockable = null)
           : base()
        {
            // Do basic initialization
            Factory = initialFactory ?? new MainDockFactory(new());

            Id = "Main";
            Title = "Main";

            CanCreateDocument = false;
            CanClose = false;
            CanPin = false;
            CanFloat = false;
            IsCollapsable = false;

            // If there's an initial dockable, activate and show it
            if (initialDockable != null)
            {
                Debug.Assert(Factory != null);
                ActiveDockable = initialDockable;
                VisibleDockables = Factory.CreateList(initialDockable);
            }


            // A document may be created when the new document type is not null
            this.WhenAnyValue(x => x.NewDocumentType)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => CanCreateDocument = !(x?.IsAbstract ?? true));


            CreateDocument = ReactiveCommand.Create(
                // Command implementation
                () =>
                {
                    // Don't execute if we aren't able to
                    if (!CanCreateDocument)
                    {
                        Trace.TraceWarning(
                            "Can't create a document with type "
                            + NewDocumentType?.AssemblyQualifiedName ?? "null"
                            + "; must be a non-null concrete class." );
                        return;
                    }

                    Debug.Assert(Factory != null);

                    var index = VisibleDockables?.Count + 1;
                    if (Activator.CreateInstance(NewDocumentType!) is not IDockable document)
                    {
                        Trace.TraceWarning($"Failed to create an editor window of type, \"{NewDocumentType!.AssemblyQualifiedName}\"");
                    }
                    else
                    {
                        Factory.AddDockable(this, document);
                        Factory.SetActiveDockable(document);
                        Factory.SetFocusedDockable(this, document);
                    }
                }
            );
        }
    }
}
