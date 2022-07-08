using GlacierKitCore.Attributes.DataProviders;
using GlacierKitCore.Commands;
using GlacierKitTestShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GlacierKitCoreTest.Tests.Attributes.DataProviders
{
	public class MainMenuItemSetupInfoTest
	{
		#region Theory_data

		public class MainMenuItemSetupInfoTestConstructorParams :
			TheoryData<string, IEnumerable<string>, GKCommand<Unit, Unit>?, int>
		{
			public MainMenuItemSetupInfoTestConstructorParams()
			{
				string[] pathValues = new string[] { "One", "Two", "Three" };
				GKCommand<Unit, Unit>?[] commandValues = new GKCommand<Unit, Unit>?[] { null, GeneralUseData.StubGKCommand };
				int[] orderValues = new int[] { -5, 0, 5 };

				for (int i = 0; i < pathValues.Length; i++)
				{
					foreach (GKCommand<Unit, Unit>? commandValue in commandValues)
						foreach (int orderValue in orderValues)
							Add("Foo", pathValues.SkipLast(i), commandValue, orderValue);
				}
			}
		}
		public static readonly MainMenuItemSetupInfoTestConstructorParams
			MainMenuItemSetupInfoTestConstructorParamsValue = new();

		#endregion


		#region Constructor

		[Theory]
		[MemberData(nameof(MainMenuItemSetupInfoTestConstructorParamsValue))]
		[Trait("TestingMember", "Constructor")]
		public static void MainMenuItemSetupInfo_constructor_doesnt_throw(
			string title,
			IEnumerable<string> path,
			GKCommand<Unit, Unit>? command,
			int order
		)
		{
			Util.AssertCodeDoesNotThrowException(
				() => _ = new MainMenuItemSetupInfo(title, path, command, order)
			);
		}

		#endregion


		#region Title

		[Theory]
		[MemberData(nameof(MainMenuItemSetupInfoTestConstructorParamsValue))]
		[Trait("TestingMember", "Property_Title")]
		public static void Title_is_set_by_constructor(
			string title,
			IEnumerable<string> path,
			GKCommand<Unit, Unit>? command,
			int order
		)
		{
			// Arrange
			MainMenuItemSetupInfo setupInfo;
			string actualValue;

			// Act
			setupInfo = new(title, path, command, order);
			actualValue = setupInfo.Title;

			// Assert
			Assert.Equal(title, actualValue);
		}

		#endregion


		#region Path

		[Theory]
		[MemberData(nameof(MainMenuItemSetupInfoTestConstructorParamsValue))]
		[Trait("TestingMember", "Property_Path")]
		public static void Path_is_set_by_constructor(
			string title,
			IEnumerable<string> path,
			GKCommand<Unit, Unit>? command,
			int order
		)
		{
			// Arrange
			MainMenuItemSetupInfo setupInfo;
			IEnumerable<string> actualValue;

			// Act
			setupInfo = new(title, path, command, order);
			actualValue = setupInfo.Path;

			// Assert
			Assert.Equal(path, actualValue);
		}

		[Theory]
		[MemberData(nameof(MainMenuItemSetupInfoTestConstructorParamsValue))]
		[Trait("TestingMember", "Property_Path")]
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
#pragma warning disable IDE0060 // Remove unused parameter
		public static void Path_with_empty_value_throws(
			string title,
			IEnumerable<string> path,
			GKCommand<Unit, Unit>? command,
			int order
		)
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
		[MemberData(nameof(MainMenuItemSetupInfoTestConstructorParamsValue))]
		[Trait("TestingMember", "Property_Command")]
		public static void Command_is_set_by_constructor(
			string title,
			IEnumerable<string> path,
			GKCommand<Unit, Unit>? command,
			int order
		)
		{
			// Arrange
			MainMenuItemSetupInfo setupInfo;
			GKCommand<Unit, Unit>? actualValue;

			// Act
			setupInfo = new(title, path, command, order);
			actualValue = setupInfo.Command;

			// Assert
			Assert.Equal(command, actualValue);
		}

		#endregion


		#region Order

		[Theory]
		[MemberData(nameof(MainMenuItemSetupInfoTestConstructorParamsValue))]
		[Trait("TestingMember", "Property_Order")]
		public static void Order_is_set_by_constructor(
			string title,
			IEnumerable<string> path,
			GKCommand<Unit, Unit>? command,
			int order
		)
		{
			// Arrange
			MainMenuItemSetupInfo setupInfo;
			int actualValue;

			// Act
			setupInfo = new(title, path, command, order);
			actualValue = setupInfo.Order;

			// Assert
			Assert.Equal(order, actualValue);
		}

		#endregion
	}
}
