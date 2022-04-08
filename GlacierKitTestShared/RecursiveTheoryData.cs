


// TODO: Unit test T4 code generation?


using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace GlacierKitTestShared.CommonTestData
{
			public partial class _TYPE_CommonTree_DeleteNodeTheoryData :
			TheoryData<GlacierKitCore.Utility.Tree.ITree.ENodeType,GlacierKitCore.Utility.Tree.ITree.ETreeAndNodeRelationshipType,GlacierKitCore.Utility.Tree.ITree.ENodeSetTypeFlags,bool,bool>
		{
			public _TYPE_CommonTree_DeleteNodeTheoryData()
			{
				foreach
				(
					IDictionary<string, object?> item
					in RecursiveTheoryData.CreateTheoryDataFrom(new Dictionary<string, object>(){["typeToDelete"] = typeof(GlacierKitCore.Utility.Tree.ITree.ENodeType),["relationshipOfNodeToDelete"] = typeof(GlacierKitCore.Utility.Tree.ITree.ETreeAndNodeRelationshipType),["deletableNodes"] = typeof(GlacierKitCore.Utility.Tree.ITree.ENodeSetTypeFlags),["shouldDeleteRecursively"] = typeof(bool),["expectedReturnValue"] = typeof(bool)})
				)
					Add((GlacierKitCore.Utility.Tree.ITree.ENodeType)item["theoryDataParam.Name"]!,(GlacierKitCore.Utility.Tree.ITree.ETreeAndNodeRelationshipType)item["theoryDataParam.Name"]!,(GlacierKitCore.Utility.Tree.ITree.ENodeSetTypeFlags)item["theoryDataParam.Name"]!,(bool)item["theoryDataParam.Name"]!,(bool)item["theoryDataParam.Name"]!);
			}
		}
	}


