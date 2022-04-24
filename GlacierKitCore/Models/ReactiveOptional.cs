using Avalonia.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitCore.Models
{
	/// <summary>
	/// A reative object that may or may not contain a value. Similar to <see cref="Optional{T}"/>
	/// </summary>
	public class ReactiveOptional<T> : ReactiveObject
	{
		/// <summary>
		/// The most recent non-empty value, or a default value if no value has yet been assigned
		/// </summary>
		/// <remarks>
		/// Using this property while HasValue is false does NOT result in an error
		/// </remarks>
		[Reactive]
		public T? LastValue { get; set; }

		/// <summary>
		/// True if this object currently has a value assigned, false otherwise
		/// </summary>
		[Reactive]
		public bool HasValue { get; set; }


		private ReactiveOptional(T? value, bool hasValue)
		{
			LastValue = value;
			HasValue = hasValue;

			// When LastValue is assigned a value
			this.WhenAnyValue(x => x.LastValue)
				// and HasValue is false
				.Where(_ => !HasValue)
				// excluding the first time the constructor sets LastValue
				.Skip(1)
				// Set HasValue to true
				.Subscribe(x => HasValue = true);
		}

		public ReactiveOptional() :
			this(default, false)
		{}

		public ReactiveOptional(T value) :
			this(value, true)
		{ }

		public static ReactiveOptional<T> MakeEmpty()
		{
			return new();
		}
	}
}
