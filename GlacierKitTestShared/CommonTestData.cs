using GlacierKitCore.Utility.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


}
