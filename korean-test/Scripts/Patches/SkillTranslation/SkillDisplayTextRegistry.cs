using System.Collections.Generic;

namespace KorFontTest.Patches.SkillTranslation
{
	public static class SkillDisplayTextRegistry
	{
		private static readonly Dictionary<string, string> DescriptionsByClass = new Dictionary<string, string>();

		public static void SetDescription(string className, string description)
		{
			if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(description))
			{
				return;
			}

			DescriptionsByClass[className] = description;
			if (DescriptionsByClass.Count > 4096)
			{
				DescriptionsByClass.Clear();
			}
		}

		public static bool TryGetDescription(string className, out string description)
		{
			if (!string.IsNullOrEmpty(className) && DescriptionsByClass.TryGetValue(className, out description))
			{
				return true;
			}

			description = null;
			return false;
		}
	}
}
