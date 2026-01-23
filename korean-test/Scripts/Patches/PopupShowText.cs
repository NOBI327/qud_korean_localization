using Genkit;
using HarmonyLib;
using XRL.UI;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(Popup), nameof(Popup.Show), new[]
	{
		typeof(string),
		typeof(string),
		typeof(string),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(Location2D)
	}),
	HarmonyPatch(typeof(Popup), nameof(Popup.ShowBlock), new[]
	{
		typeof(string),
		typeof(string),
		typeof(string),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(bool),
		typeof(Location2D)
	})]
	public static class Popup_Show_Text
	{
		public static void Prefix(ref string Message)
		{
			UnityEngine.Debug.LogError(Message);
			Message = TextTranslator.Translate(Message);
		}
	}
}
