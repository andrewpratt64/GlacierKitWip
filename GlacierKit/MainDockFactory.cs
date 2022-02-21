using Avalonia.Data;
using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI;
using GlacierKit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKit
{
    internal class MainDockFactory : Factory
    {
        // TODO: Change data type?
        private readonly object _context;

        public MainDockFactory(object context)
        {
            _context = context;
        }


        public override IRootDock CreateLayout()
        {
            var mainViewModel = new MainViewModel(this);

            var root = CreateRootDock();

            root.Id = "Root";
            //root.Title = "Root";
            root.ActiveDockable = mainViewModel;
            root.DefaultDockable = mainViewModel;
            root.VisibleDockables = CreateList<IDockable>(mainViewModel);
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
                    var hostWindow = new HostWindow()
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
