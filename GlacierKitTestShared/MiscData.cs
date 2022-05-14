using GlacierKitCore.Commands;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace GlacierKitTestShared
{
    /// <summary>
    /// Miscellaneous data for unit test
    /// </summary>
    public sealed class MiscData
    {
        /// <summary>
        /// The amount of modules in the gkmodules directory (one .dll = one module)
        /// </summary>
        public static readonly int ExpectedModules = 1;
    }


	public sealed class GeneralUseData
	{
		public static string OneCharString => "@";
		public static string TinyString => "Abc";
		public static string SmallString => "Example";

		// via https://www.lipsum.com
		public static string LongString =>
			"Lorem ipsum dolor sit amet, consectetur adipiscing elit."
			+ "Etiam ullamcorper massa nisl, nec fringilla lacus porta eget."
			+ "Fusce feugiat ac lacus id tempor. In posuere imperdiet dictum."
			+ "Curabitur malesuada id magna sit amet iaculis."
			+ "Nullam venenatis euismod lacus, vitae ultricies arcu faucibus quis."
			+ "Mauris nec ipsum quis sapien commodo consectetur vel eu tortor."
			+ "Mauris pulvinar eget metus ut tincidunt."
			+ "Nam nec ornare sem, aliquet condimentum est."
			+ "Sed maximus nisi vitae neque maximus, nec accumsan velit blandit.";


		public static int SmallInt => 5;
		public static int LargeInt => 123456789;
		public static int SmallPositiveInt => SmallInt;
		public static int LargePositiveInt => LargeInt;
		public static int SmallNegativeInt => -SmallInt;
		public static int LargeNegativeInt => -LargeInt;


		public static GKCommand<Unit, Unit> StubGKCommand => new
		(
			commandId: "TestData_Stub",
			displayName: "Stub Command for Testing",
			command: ReactiveCommand.Create<Unit, Unit>(_ => Unit.Default)
		);
	}
}
