using HarmonyLib;
using XRL.Messages;

namespace KorFontTest.Patches
{
	[HarmonyPatch(typeof(MessageQueue), nameof(MessageQueue.AddPlayerMessage), new[] { typeof(string), typeof(string), typeof(bool) })]
	public static class MessageQueue_AddPlayerMessage_Translate
	{
		public static void Prefix(ref string Message)
		{
			UnityEngine.Debug.LogError("MessageQueue Message : " + Message);
			Message = TextTranslator.Translate(Message);
		}
	}
}
