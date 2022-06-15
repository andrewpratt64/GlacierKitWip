using Avalonia.Data;
using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI;
using GlacierKitCore.Models;
using GlacierKit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GlacierKit
{
    public class MainDockFactory : Factory
    {
        private readonly EditorContext _context;


        internal MainViewModel? MainDockable
        { get; private set; }



        public MainDockFactory(EditorContext context)
        {
            _context = context;
            MainDockable = null;
        }


        public override IRootDock CreateLayout()
        {
            MainDockable = new MainViewModel(_context, this);

            IRootDock root = CreateRootDock();

            root.Id = "Root";
            //root.Title = "Root";
            root.ActiveDockable = MainDockable;
            root.DefaultDockable = MainDockable;
            root.VisibleDockables = CreateList<IDockable>(MainDockable);
            root.CanClose = false;
            root.CanFloat = false;
            root.IsCollapsable = false;
            root.CanPin = false;

            return root;
        }


        public override void InitLayout(IDockable layout)
        {
            // TODO: Do ContextLocator and DockableLocator need to be set up as well?

            HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
            {
                [nameof(IDockWindow)] = () =>
                {
					HostWindow? hostWindow = new()
                    {
                        [!Avalonia.Controls.Window.TitleProperty] = new Binding("ActiveDockable.Title")
                    };
                    return hostWindow;
                }
            };

            base.InitLayout(layout);
            SetActiveDockable(layout);
        }
    }
}
