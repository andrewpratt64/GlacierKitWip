using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Misc
{
	/// <summary>
	/// Describes an object related to a view that handles both activation and deactivation
	/// </summary>
	public interface IActivationHandler
	{
		/// <summary>
		/// Called when the view is added to the visual tree
		/// </summary>
		/// <param name="disposables">Objects to be disposed of when the view is deactivated</param>
		public abstract void HandleActivation(CompositeDisposable disposables);

		/// <summary>
		/// Called when the view is removed from the visual tree
		/// </summary>
		public abstract void HandleDeactivation();
	}
}
