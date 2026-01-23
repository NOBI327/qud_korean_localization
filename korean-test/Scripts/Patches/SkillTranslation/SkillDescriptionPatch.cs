using HarmonyLib;
using XRL;
using XRL.World.Parts.Skill;

namespace KorFontTest.Patches.SkillTranslation
{
	[HarmonyPatch(typeof(BaseSkill), nameof(BaseSkill.GetDescription))]
	public static class BaseSkill_GetDescription_DisplayOverride
	{
		static void Postfix(IBaseSkillEntry Entry, ref string __result)
		{
			if (Entry == null)
			{
				return;
			}

			if (SkillDisplayTextRegistry.TryGetDescription(Entry.Class, out string displayDescription))
			{
				__result = displayDescription;
			}
		}
	}
}
