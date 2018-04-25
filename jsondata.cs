using System;
using Newtonsoft.Json;

namespace rpcs3Updater
{
		public class Windows
		{
			public string datetime { get; set; }
			public string download { get; set; }
		}
		public class Linux
		{
			public string datetime { get; set; }
			public string download { get; set; }
		}
		public class LatestBuild
		{
			public string pr { get; set; }
			public Windows windows { get; set; }
			public Linux linux { get; set; }
		}
		public class RootObject
		{
			public int return_code { get; set; }
			public LatestBuild latest_build { get; set; }
		}
}