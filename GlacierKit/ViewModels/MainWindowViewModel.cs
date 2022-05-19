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
		#region Private_fields

		private ReactiveCommand<Type, Unit> CreateEditorWindow
        { get; set; }

		#endregion


		#region Public_properties

		/// <summary>
		/// The editor context instance
		/// </summary>
		public EditorContext Ctx { get; set; }

		/// <summary>
		/// The main dock factory instance
		/// </summary>
        public MainDockFactory DockFactory { get; }

		/// <summary>
		/// The main layout for the editor's docking system
		/// </summary>
        [Reactive]
        public IDock DockLayout { get; private set; }

		#endregion


		#region Public_OAPHs

		/// <summary>
		/// The main menu bar
		/// </summary>
		[ObservableAsProperty]
		public MenuBarViewModel MainMenuBar { get; }

		#endregion


		#region Constructor

		public MainWindowViewModel() : this(null) { }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public MainWindowViewModel(EditorContext? ctx)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			// Use the provided context, or create a new one
			if (ctx == null)
			{
				Ctx = new();
				Ctx.ModuleLoader.LoadModules();
			}
			else
			{
				Ctx = ctx;
			}

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

			// Set MainMenuBar from the context after gk modules have loaded
			this.WhenAnyValue(x => x.Ctx.ModuleLoader.State)
				.Where(x => x == GKModuleLoaderService.ELoaderState.Loaded)
				.Select(_ => Ctx.MainMenuBar)
				.ToPropertyEx(this, x => x.MainMenuBar);
        }

		#endregion
	}
}
