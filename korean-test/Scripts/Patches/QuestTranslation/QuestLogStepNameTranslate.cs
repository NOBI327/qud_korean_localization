using System.Collections.Generic;
using HarmonyLib;
using XRL.UI;
using XRL.World;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(QuestLog), nameof(QuestLog.GetLinesForQuest))]
	public static class QuestLog_GetLinesForQuest_StepNameTranslate
	{
		private struct StepState
		{
			public QuestStep Step;
			public string Name;
		}

		static void Prefix(Quest Q, out List<StepState> __state)
		{
			__state = null;
			if (Q == null || Q.StepsByID == null)
			{
				return;
			}

			foreach (QuestStep step in Q.StepsByID.Values)
			{
				if (step == null)
				{
					continue;
				}

				if (QuestDisplayRegistry.TryGetStepName(Q.ID, step.ID, out string displayName) &&
					!string.IsNullOrEmpty(displayName) &&
					displayName != step.Name)
				{
					if (__state == null)
					{
						__state = new List<StepState>();
					}

					__state.Add(new StepState { Step = step, Name = step.Name });
					step.Name = displayName;
				}
			}
		}

		static void Postfix(List<StepState> __state)
		{
			if (__state == null)
			{
				return;
			}

			for (int i = 0; i < __state.Count; i++)
			{
				StepState state = __state[i];
				if (state.Step != null)
				{
					state.Step.Name = state.Name;
				}
			}
		}
	}
}
