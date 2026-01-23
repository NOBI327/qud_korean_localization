// using System.Collections.Generic;

// namespace KorFontTest.Patches
// {
// 	public static class SkillDisplayNameRegistry
// 	{
// 		private static readonly Dictionary<string, string> ByClass = new Dictionary<string, string>();

// 		public static void SetDisplayName(string className, string displayName)
// 		{
// 			if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(displayName))
// 			{
// 				return;
// 			}

// 			ByClass[className] = displayName;
// 			if (ByClass.Count > 4096)
// 			{
// 				ByClass.Clear();
// 			}
// 		}

// 		public static bool TryGetDisplayName(string className, out string displayName)
// 		{
// 			if (!string.IsNullOrEmpty(className) && ByClass.TryGetValue(className, out displayName))
// 			{
// 				return true;
// 			}

// 			displayName = null;
// 			return false;
// 		}
// 	}
// }
