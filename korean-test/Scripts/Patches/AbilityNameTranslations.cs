using System.Collections.Generic;
using HarmonyLib;
using XRL.World.Parts;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(ActivatedAbilities), nameof(ActivatedAbilities.AddAbility))]
	public static class ActivatedAbilities_AddAbility_NameTranslation
	{
		private static readonly Dictionary<string, string> NameByCommand = new Dictionary<string, string>
		{
			// Add translations here by Command.
			{ "CommandToggleCyberNightVision", "나이트 비전" },
			{ "CommandToggleRunning", "달리기"},
			{ "CommandSurvivalCamp", "야영"}
		};

		private static readonly Dictionary<string, string> DescriptionByCommand = new Dictionary<string, string>
		{
			// Add description translations here by Command.
			{ "CommandSurvivalCamp", "야영지를 만들어 식사를 조리하고 음식을 보존합니다. 전투 중에는 야영햘 수 없습니다." },
		};

		public static void Prefix(ref string Name, ref string Description, string Command)
		{
			// UnityEngine.Debug.LogError("ACTIVATED ABILITY : " + Command);
			if (NameByCommand.ContainsKey(Command))
			{
				// UnityEngine.Debug.LogError("ACTIVATED TARGETING ABILTIY : " + Command);
			}
			if (string.IsNullOrEmpty(Command))
			{
				return;
			}

			if (NameByCommand.TryGetValue(Command, out string translated) && !string.IsNullOrEmpty(translated))
			{
				Name = translated;
			}
			if (DescriptionByCommand.TryGetValue(Command, out string translatedDescription) && !string.IsNullOrEmpty(translatedDescription))
			{
				Description = translatedDescription;
			}
		}
	}
}
