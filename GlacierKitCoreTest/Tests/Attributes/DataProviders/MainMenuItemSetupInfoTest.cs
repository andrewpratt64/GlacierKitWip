using GlacierKitCore.Attributes.DataProviders;
using GlacierKitCore.Commands;
using GlacierKitTestShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Attributes.DataProviders
{
	public class MainMenuItemSetupInfoTest
	{
		#region Theory_data

#pragma warning disable IDE1006 // Naming Styles
		public class _TYPE_MainMenuItemSetupInfoTestConstructorParams : TheoryData<string, IEnumerable<string>, IGKCommand?>
#pragma warning restore IDE1006 // Naming Styles
		{
			public _TYPE_MainMenuItemSetupInfoTestConstructorParams()
			{
				string[] pathValues = new string[] { "One", "Two", "Three" };
				IGKCommand?[] commandValues = new IGKCommand?[] { null, GeneralUseData.StubGKCommand };
				for (int i = 0; i < pathValues.Length; i++)
				{
					foreach (IGKCommand? commandValue in commandValues)
						Add("Foo", pathValues.SkipLast(i), commandValue);
				}
			}
		}
		public static readonly _TYPE_MainMenuItemSetupInfoTestConstructorParams
			_DATA_MainMenuItemSetupInfoTestConstructorParams = new();

		#endregion


		#region Constructor

		[Theory]
		[MemberData(nameof(_DATA_MainMenuItemSetupInfoTestConstructorParams))]
		[Trait("TestingMember", "Constructor")]
		public static void MainMenuItemSetupInfo_constructor_doesnt_throw(string title, IEnumerable<string> path, IGKCommand? command)
		{
			Util.AssertCodeDoesNotThrowException(
				() => _ = new MainMenuItemSetupInfo(title, path, command)
			);
		}

		#endregion


		#region Title

		[Theory]
		[MemberData(nameof(_DATA_MainMenuItemSetupInfoTestConstructorParams))]
		[Trait("TestingMember", "Property_Title")]
		public static void Title_is_set_by_constructor(string title, IEnumerable<string> path, IGKCommand? command)
		{
			// Arrange
			MainMenuItemSetupInfo setupInfo;
			string actualValue;

			// Act
			setupInfo = new(title, path, command);
			actualValue = setupInfo.Title;

			// Assert
			Assert.Equal(title, actualValue);
		}

		#endregion


		#region Path

		[Theory]
		[MemberData(nameof(_DATA_MainMenuItemSetupInfoTestConstructorParams))]
		[Trait("TestingMember", "Property_Path")]
		public static void Path_is_set_by_constructor(string title, IEnumerable<string> path, IGKCommand? command)
		{
			// Arrange
			MainMenuItemSetupInfo setupInfo;
			IEnumerable<string> actualValue;

			// Act
			setupInfo = new(title, path, command);
			actualValue = setupInfo.Path;

			// Assert
			Assert.Equal(path, actualValue);
		}

		[Theory]
		[MemberData(nameof(_DATA_MainMenuItemSetupInfoTestConstructorParams))]
		[Trait("TestingMember", "Property_Path")]
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
#pragma warning disable IDE0060 // Remove unused parameter
		public static void Path_with_empty_value_throws(string title, IEnumerable<string> path, IGKCommand? command)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
#pragma warning restore IDE0079 // Remove unnecessary suppression
		{
			Util.AssertCodeThrowsException(
				() => _ = new MainMenuItemSetupInfo(title, Enumerable.Empty<string>(), command)
			);
		}
		
		#endregion


		#region Command

		[Theory]
		[MemberData(nameof(_DATA_MainMenuItemSetupInfoTestConstructorParams))]
		[Trait("TestingMember", "Property_Command")]
		public static void Command_is_set_by_constructor(string title, IEnumerable<string> path, IGKCommand? command)
		{
			// Arrange
			MainMenuItemSetupInfo setupInfo;
			IGKCommand? actualValue;

			// Act
			setupInfo = new(title, path, command);
			actualValue = setupInfo.Command;

			// Assert
			Assert.Equal(command, actualValue);
		}

		#endregion
	}
}
