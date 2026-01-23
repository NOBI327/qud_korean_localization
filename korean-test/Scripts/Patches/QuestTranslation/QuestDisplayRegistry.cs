using System.Collections.Generic;

namespace KorFontTest.Patches
{
	public static class QuestDisplayRegistry
	{
		private static readonly Dictionary<string, string> QuestNames = new Dictionary<string, string>();
		private static readonly Dictionary<string, Dictionary<string, string>> StepNames = new Dictionary<string, Dictionary<string, string>>();

		public static void SetQuestName(string questId, string displayName)
		{
			if (string.IsNullOrEmpty(questId) || string.IsNullOrEmpty(displayName))
			{
				return;
			}

			QuestNames[questId] = displayName;
			if (QuestNames.Count > 2048)
			{
				QuestNames.Clear();
			}
		}

		public static bool TryGetQuestName(string questId, out string displayName)
		{
			if (!string.IsNullOrEmpty(questId) && QuestNames.TryGetValue(questId, out displayName))
			{
				return true;
			}

			displayName = null;
			return false;
		}

		public static void SetStepName(string questId, string stepId, string displayName)
		{
			if (string.IsNullOrEmpty(questId) || string.IsNullOrEmpty(stepId) || string.IsNullOrEmpty(displayName))
			{
				return;
			}

			if (!StepNames.TryGetValue(questId, out var steps))
			{
				steps = new Dictionary<string, string>();
				StepNames[questId] = steps;
			}

			steps[stepId] = displayName;
			if (StepNames.Count > 2048)
			{
				StepNames.Clear();
			}
		}

		public static bool TryGetStepName(string questId, string stepId, out string displayName)
		{
			if (!string.IsNullOrEmpty(questId) &&
				!string.IsNullOrEmpty(stepId) &&
				StepNames.TryGetValue(questId, out var steps) &&
				steps.TryGetValue(stepId, out displayName))
			{
				return true;
			}

			displayName = null;
			return false;
		}
	}
}
