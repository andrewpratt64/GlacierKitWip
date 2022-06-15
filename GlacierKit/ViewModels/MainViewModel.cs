using Dock.Model.Core;
using Dock.Model.ReactiveUI.Controls;
using GlacierKitCore.Attributes;
using GlacierKitCore.Models;
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
        // We force CanCreateDocument to always be false since a plus icon to add a
        //  document is shown when it's true. There isn't a good way to determine
        //  what document should be added when hitting a generic plus button, so it
        //  should never be visible
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Should always be false, but also intentionally hides DocumentDock.CanCreateDocument")]
        public new bool CanCreateDocument
        {
            get => false;

            // The setter is a stub instead of read-only
            //  since it's possible code elsewhere won't
            //  expect this property to be read-only
            set { }
        }


		/// <summary>
		/// The editor context instance
		/// </summary>
        public EditorContext Ctx { get; }
		
		[Reactive]
        public Type? NewDocumentType
        { get; set; }


        [Reactive]
        public bool IsNewDocumentTypeValid
        { get; private set; }


        public MainViewModel() : this(new(), new MainDockFactory(new()), null) { }
        
        public MainViewModel(EditorContext ctx, IFactory? initialFactory, IDockable? initialDockable = null)
           : base()
        {
            // Do basic initialization
			Ctx = ctx;
            NewDocumentType = null;
            IsNewDocumentTypeValid = false;

            Factory = initialFactory ?? new MainDockFactory(new());

            Id = "Main";
            Title = "Main";

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


            CreateDocument = ReactiveCommand.Create(
                // Command implementation
                () =>
                {
                    // Don't execute if we aren't able to
                    if (!IsNewDocumentTypeValid)
                    {
                        Trace.TraceWarning(
                            "Can't create a document with type "
                            + NewDocumentType?.AssemblyQualifiedName ?? "null"
                            + "; must be a non-null concrete class." );
                        return;
                    }

                    Debug.Assert(Factory != null);

					int? index = VisibleDockables?.Count + 1;
                    if (Activator.CreateInstance(NewDocumentType!, Ctx) is not IDockable document)
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


            // Update IsNewDocumentTypeValid when NewDocumentType changes
            this.WhenAnyValue(x => x.NewDocumentType)
                .Subscribe(x => IsNewDocumentTypeValid = EditorWindowViewModel.IsTypeAnInstantiableEditorWindow(x));
        }
    }
}
