using Dock.Model.Core;
using GlacierKitCore.Commands;
using GlacierKitCore.Models;
using GlacierKitCore.Services;
using GlacierKitCore.ViewModels;
using GlacierKitCore.ViewModels.Common;
using GlacierKitCore.ViewModels.EditorWindows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;

namespace GlacierKit.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ReactiveCommand<Type, Unit> CreateEditorWindow
        { get; set; }


        public EditorContext Ctx
        { get; set; }

        public MainDockFactory DockFactory
        { get; }

        [Reactive]
        public IDock DockLayout
        { get; private set; }

		

		public MainWindowViewModel() : this(null) { }
        public MainWindowViewModel(EditorContext? ctx)
        {
            // Use the provided context, or create a new one
            Ctx = ctx ?? new();

            // Create the dock factory
            DockFactory = new MainDockFactory(Ctx);
            // Creare the dock layout
            IDock? dockLayout = DockFactory.CreateLayout();
            // Initialize the dock layout
            DockLayout = dockLayout;
            DockFactory.InitLayout(DockLayout);


            // Initialize command for creating new editor window instances
            CreateEditorWindow = ReactiveCommand.Create((Type windowType) =>
            {
                DockFactory!.MainDockable!.NewDocumentType = windowType;
                DockFactory!.MainDockable!.CreateDocument!.Execute(Unit.Default);
                return Unit.Default;
            });

			// Execute our private CreateEditorWindow command when the editor context's
			//  CreateEditorWindow command is executed with valid arguments
			this.WhenAnyObservable(x => x.Ctx.CreateEditorWindow)
                .WhereNotNull()
                .InvokeCommand(CreateEditorWindow);
            
        }
    }
}
