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


        // TODO: Some collection property for the Menu control (probably with ITree?)

        // TEMPORARY START
        public class TMP_WindowType : ViewModelBase
        {
            public string Name
            { get; set; } = "<unnamed>";
            public Type? TypeData
            { get; set; }
            public MainWindowViewModel? Owner
            { get; set; }

            public void OnHit()
            {
                if (Owner != null)
                {
                    Owner.TMP_SelectedWindowType = TypeData;
					IObservable<Unit> observable = Owner.TMP_WindowTypeHit.Execute();
					IDisposable disposable = observable.Subscribe();
                    observable.Wait();
                    disposable.Dispose();
                }
            }
        }
        internal ObservableCollection<TMP_WindowType> TMP_WindowTypes
        { get; }

        internal ObservableCollection<IGKCommand> TMP_Commands
        { get; }

        public object? TMP_SelectedWindowType
        { get; set; }
        public ReactiveCommand<Unit, Unit> TMP_WindowTypeHit
        { get; set; }

		public MenuBarViewModel TMP_Menu
		{ get; set; }

		// TEMPORARY END


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


            // TEMPORARY START
            Ctx.ModuleLoader.LoadModules();

            TMP_WindowTypes = new ObservableCollection<TMP_WindowType> {};
            TMP_Commands = new ObservableCollection<IGKCommand> {};
            
            TMP_WindowTypeHit = ReactiveCommand.Create(() =>
            {
                object? input = TMP_SelectedWindowType;
                Type? type = input as Type;
                Trace.WriteLine("Calling TMP_WindowTypeHit with param=" + (type?.ToString() ?? "null"));
				IObservable<Type?> observable = Ctx.CreateEditorWindow.Execute(type);
				IDisposable disposable = observable.Subscribe();
				Type? result = observable.Wait();
                Trace.WriteLine("TMP_WindowTypeHit returned " + (result?.ToString() ?? "null"));
                return Unit.Default;
            });
            
            foreach (Type windowType in Ctx.ModuleLoader.EditorWindowViewModels)
            {
               TMP_WindowTypes.Add(new TMP_WindowType{
                   Name = (windowType.GetProperty("DisplayName")?.GetValue(null) as string) ?? "<FAILED TO GET PROPERTY VALUE>",
                   TypeData = windowType,
                   Owner = this
               });
            }

            foreach (IGKCommand command in Ctx.ModuleLoader.GKCommands)
            {
                TMP_Commands.Add(command);
            }

			TMP_Menu = new();
			TreeNode<MenuBarItemViewModel> TMP_Menu_File = TMP_Menu.ItemTree.CreateRootNode.Execute(new("File")).Wait();
				TreeNode<MenuBarItemViewModel> TMP_Menu_File_New = TMP_Menu_File.AddChild.Execute(new("New")).Wait();
					TMP_Menu_File_New.AddChild.Execute(new("Thing")).Wait();
					TMP_Menu_File_New.AddChild.Execute(new("Stuff")).Wait();
				TMP_Menu_File.AddChild.Execute(new("Open")).Wait();
				TMP_Menu_File.AddChild.Execute(new("Close")).Wait();
			TreeNode<MenuBarItemViewModel> TMP_Menu_Edit = TMP_Menu.ItemTree.CreateRootNode.Execute(new("Edit")).Wait();
				TMP_Menu_Edit.AddChild.Execute(new("Copy")).Wait();
				TMP_Menu_Edit.AddChild.Execute(new("Paste")).Wait();
				TMP_Menu_Edit.AddChild.Execute(new("Undo")).Wait();
			TreeNode<MenuBarItemViewModel> TMP_Menu_View = TMP_Menu.ItemTree.CreateRootNode.Execute(new("View")).Wait();
				TMP_Menu_View.AddChild.Execute(new("Some things")).Wait();

			// TEMPORARY END

			// Execute our private CreateEditorWindow command when the editor context's
			//  CreateEditorWindow command is executed with valid arguments
			this.WhenAnyObservable(x => x.Ctx.CreateEditorWindow)
                .WhereNotNull()
                .InvokeCommand(CreateEditorWindow);
            
        }
    }
}
