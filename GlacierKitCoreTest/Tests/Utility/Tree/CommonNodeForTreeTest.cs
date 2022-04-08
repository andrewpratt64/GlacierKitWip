using DynamicData;
using GlacierKitCore.Utility.Tree;
using GlacierKitTestShared;
using GlacierKitTestShared.CommonTestData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using static GlacierKitCore.Utility.CollectionUtils;
using static GlacierKitCoreTest.Tests.Utility.Tree.TreeTestShared;

namespace GlacierKitCoreTest.Tests.Utility.Tree
{
	public class CommonNodeForTreeTest
	{
		#region Ctor_and_Properties

		[Fact]
		public static void Ctor_doesnt_throw()
		{
			// Arrange
			CommonTree tree;

			// Act
			tree = new();

			// Assert
			Util.AssertCodeDoesNotThrowException(() =>
			{
				_ = new CommonNodeForTree(tree);
			});
		}

		[Fact]
		public static void Tree_is_set_by_ctor()
		{
			// Arrange
			CommonTree tree;
			CommonNodeForTree node;

			// Act
			tree = new();
			node = new CommonNodeForTree(tree);

			// Assert
			Assert.Equal(tree, node.Tree);
		}

		[Fact]
		public static void ParentNode_is_initially_null()
		{
			// Arrange
			CommonTree tree;
			CommonNodeForTree node;
			INodeForTree<CommonTree>? actualValue;

			// Act
			tree = new();
			node = new CommonNodeForTree(tree);
			actualValue = node.ParentNode;

			// Assert
			Assert.Null(actualValue);
		}

		[Fact]
		public static void ChildNodes_is_initially_empty_or_null()
		{
			// Arrange
			CommonTree tree;
			CommonNodeForTree node;
			ECollectionSizeFlags expectedFlags = ECollectionSizeFlags.NullOrEmpty;
			ECollectionSizeFlags actualFlags;
			IEnumerable<INodeForTree<CommonTree>>? actualValue;

			// Act
			tree = new();
			node = new CommonNodeForTree(tree);

			actualValue = node.ChildNodes;
			actualFlags = GetCollectionSizeFlagsOf(actualValue?.ToList());

			// Assert
			Assert.Equal(expectedFlags, actualFlags);
		}

		[Fact]
		public static void Siblings_is_initially_empty_or_null()
		{
			// Arrange
			CommonTree tree;
			CommonNodeForTree node;
			ECollectionSizeFlags expectedFlags = ECollectionSizeFlags.NullOrEmpty;
			ECollectionSizeFlags actualFlags;
			IEnumerable<INodeForTree<CommonTree>>? actualValue;

			// Act
			tree = new();
			node = new CommonNodeForTree(tree);

			actualValue = node.Siblings;
			actualFlags = GetCollectionSizeFlagsOf(actualValue?.ToList());

			// Assert
			Assert.Equal(expectedFlags, actualFlags);
		}

		#endregion


		#region Methods

		public static void CanReparentTo_returns_expected_value()

		#endregion
	}
}
