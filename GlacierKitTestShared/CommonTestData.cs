using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace GlacierKitTestShared.CommonTestData
{
    /// <summary>
    /// Provides theory data for all possible boolean values
    /// </summary>
    public class BoolTheoryData : TheoryData<bool>
    {
        public BoolTheoryData()
        {
            Add(false);
            Add(true);
        }
    }


    /// <summary>
    /// Provides theory data for all possible values of a given enumerated type
    /// </summary>
    /// <typeparam name="TEnum">The enum who's values are to be used</typeparam>
    public class EnumTheoryData<TEnum> : TheoryData<TEnum>
        where TEnum : struct, Enum
    {
        public EnumTheoryData()
        {
            foreach (TEnum enumVal in Enum.GetValues<TEnum>())
                Add(enumVal);
        }
    }


	// TODO: Remove ExpectedFlagValue<TFlag>?
	/// <summary>
	/// Represents the expected state of a bitflag
	/// </summary>
	/// <typeparam name="TFlag">Type containing the bitflag</typeparam>
	public class ExpectedFlagValue<TFlag>
		where TFlag : Enum
	{
		/// <summary>
		/// Expected value of the bitflag
		/// </summary>
		/// <remarks>Set to true if flag is expected to be 1, flase if expected to be 0, or null to ignore the value</remarks>
		public bool? ExpectedValue
		{ get; set; }

		/// <summary>
		/// The bitmask for the flags to test
		/// </summary>
		public TFlag Mask
		{ get; set; }


		public ExpectedFlagValue(TFlag mask, bool? expectedValue = null)
		{
			//Debug.Assert(typeof(TFlag).IsDefined(typeof(FlagsAttribute)))

			Mask = mask;
			ExpectedValue = expectedValue;
		}


		/// <summary>
		/// Test if the actual value of the flag has the expected value
		/// </summary>
		/// <param name="actual">The actual flag value</param>
		/// <returns>True if <paramref name="actual"/> has the expected value, false otherwise</returns>
		public bool HasExpectedValue(TFlag actual)
		{
			// If ExpectedValue is true, then ignore the value and always return true
			if (ExpectedValue == null)
				return true;
			// If the actual value has the flag,
			//	return true if it was expected to have the flag or
			//	return false if it wasn't expected to have the flag
			if (actual.HasFlag(Mask))
				return ExpectedValue.Value;
			// If the actual value does NOT have the flag,
			//	return false if it was expected to have the flag or
			//	return true if it wasn't expected to have the flag
			return !ExpectedValue.Value;
		}
	}



	public static class RecursiveTheoryData
	{
		public static IEnumerable ValuesOfType(Type type)
		{
			if (type == typeof(bool))
			{
				return new List<bool> { true, false };
			}
			if (type.IsEnum)
			{
				return

					from object enumValue in Enum.GetValues(type)
					select enumValue
				;
			}
			throw new NotImplementedException($"Can't get values of type \"{type.Name}\"");
		}

		public static IEnumerable<T> ValuesOfType<T>()
		{
			return (IEnumerable<T>)ValuesOfType(typeof(T));
		}


		public static IEnumerable ValuesOf(object source)
		{
			if (source is IEnumerable enumerableSource)
				return enumerableSource;
			if (source is Type sourceType)
				return ValuesOfType(sourceType);
			throw new NotImplementedException($"Can't get values of type \"{source.GetType().Name}\"");
		}


		private static void CreateTheoryDataFromInternal(
			IDictionary<string, object> sources,
			ICollection<IDictionary<string, object?>> handledSources,
			IDictionary<string, object?> withValues
		)
		{
			KeyValuePair<string, object> source = sources.First();
			bool isMoreRecursionNeeded = sources.Count > 1;

			Dictionary<string, object>? nextLevelSources =
				isMoreRecursionNeeded
				? sources
					.Where(param => param.Key != source.Key)
					.ToDictionary(param => param.Key, param => param.Value)
				: null
			;

			foreach (object? value in ValuesOf(source.Value))
			{
				withValues[source.Key] = value;
				if (isMoreRecursionNeeded)
					CreateTheoryDataFromInternal(nextLevelSources!, handledSources, withValues);
				else
					handledSources.Add(new Dictionary<string, object?>(withValues));
			}
			withValues.Remove(source.Key);
		}
		
		public static IEnumerable<IDictionary<string, object?>> CreateTheoryDataFrom(IDictionary<string, object> sources)
		{
			ICollection<IDictionary<string, object?>> data = new List<IDictionary<string, object?>>();
			CreateTheoryDataFromInternal(
				sources,
				data,
				new Dictionary<string, object?>()
			);

			return data;
		}
	}
}
