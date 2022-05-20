using GlacierKitCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Services
{
	/// <summary>
	/// Denotes a provider of data for GlacierKit
	/// </summary>
	public abstract class GKDataProviderService
	{
		// HACK: This is a duplicate of EditorWindowViewModel.IsTypeAnInstantiableEditorWindow; consider
		//	making some sort of common IsTypeAConcreteSubclassOf(Type) predicate method.
		/// <summary>
		/// Determines whether or not a given type is a concrete subclass of GKDataProviderService
		/// </summary>
		/// <param name="type">Type to test</param>
		/// <returns>True if <paramref name="type"/> is a concrete subclass, false otherwise</returns>
		public static bool IsTypeAnInstantiableGKDataProviderService(Type? type)
		{
			// A type may be instantiated as a data provider if it is:
			//  - Not null
			//  - A subclass of GKDataProviderService
			//  - Not abstract
			return (type?.IsSubclassOf(typeof(GKDataProviderService)) ?? false) && !type!.IsAbstract;
		}


		/// <summary>
		/// The editor context instance
		/// </summary>
		public EditorContext Ctx { get; }


		public GKDataProviderService(EditorContext ctx)
		{
			Ctx = ctx;
		}
	}
}
