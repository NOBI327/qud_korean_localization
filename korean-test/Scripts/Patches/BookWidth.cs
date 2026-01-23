using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using XRL.UI;

namespace KorFontTest
{
    [HarmonyPatch(typeof(BookUI))]
    [HarmonyPatch("AutoformatPages", new[] { typeof(string), typeof(string), typeof(string), typeof(int), typeof(int), typeof(int), typeof(int) })]
    public static class BookUI_AutoformatPages_LogList2
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand is sbyte value && value == 80){
                    // Logger.buildLog.Error("Success");
                    yield return new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)65);
                }
                else {
                    // Logger.buildLog.Error("Failed to find IL code");
                    yield return instruction;
                }
            }
        }
    }
}


// namespace KorFontTest
// {
//     [HarmonyPatch(typeof(Look))]
//     [HarmonyPatch(nameof(Look.GenerateTooltipContent))]
//     public static class Look_GenerateTooltipContent
//     {
//         public static void Postfix(Look __instance, string __result)
//         {
//             UnityEngine.Debug.LogError(__result);
//         }
//     }

//     [HarmonyPatch(typeof(BookScreen))]
//     [HarmonyPatch(nameof(BookScreen.showScreen))]
//     public static class BookUI_ShowBookByID
//     {
//         public static void PrefixMarkovBook Book, string Sound = "Sounds/Interact/sfx_interact_book_read", Action<int> onShowPage = null, Action<int> afterShowPage = null)
//         {
//             UnityEngine.Debug.LogError("BookID : " + BookID + "" + Sound);
//             UnityEngine.Debug.LogError("ModernUI : " + Options.ModernUI);

//         }
//     }
// }


// namespace KorFontTest
// {
//     [HarmonyPatch(typeof(BookUI))]
//     [HarmonyPatch("AutoformatPages", new[] { typeof(string), typeof(string), typeof(string), typeof(int), typeof(int), typeof(int), typeof(int) })]
//     public static class BookUI_AutoformatPages_Debug
//     {
//         private static bool _logged;

//         public static void Prefix(string Title, string Text, string Format, int LeftMargin, int RightMargin, int TopMargin, int BottomMargin)
//         {
//             if (_logged) return;
//             _logged = true;

//             int maxClipped;
//             int maxWidth = 40;
//             List<string> list2 = StringFormat.ClipTextToArray(
//                 GameText.VariableReplace(Text),
//                 maxWidth,
//                 out maxClipped,
//                 KeepNewlines: true);

//             UnityEngine.Debug.LogError("AutoformatPages hit: " + Text);
//             for (int i = 0; i < list2.Count; i++)
//                 UnityEngine.Debug.LogError("list2[" + i + "]=" + list2[i]);
//         }
//     }
// }


// namespace KorFontTest
// {
//     [HarmonyPatch(typeof(BookUI))]
//     [HarmonyPatch("AutoformatPages", new[] { typeof(string), typeof(string), typeof(string), typeof(int), typeof(int), typeof(int), typeof(int) })]
//     public static class BookUI_AutoformatPages_Debug
//     {
//         public static void Postfix(List<BookPage> __result)
//         {
//             if (__result == null || __result.Count == 0)
//                 return;

//             const int targetWidth = 40;
//             for (int p = 0; p < __result.Count; p++)
//             {
//                 var page = __result[p];
//                 var lines = page.Lines;
//                 if (lines == null || lines.Count == 0)
//                     continue;

//                 bool needsWrap = false;
//                 for (int i = 0; i < lines.Count; i++)
//                 {
//                     string line = lines[i];
//                     if (line != null && line.Length > targetWidth)
//                     {
//                         needsWrap = true;
//                         break;
//                     }
//                 }
//                 if (!needsWrap)
//                     continue;

//                 var newLines = new List<string>(lines.Count);
//                 for (int i = 0; i < lines.Count; i++)
//                 {
//                     string line = lines[i] ?? string.Empty;
//                     int len = line.Length;
//                     if (len <= targetWidth)
//                     {
//                         newLines.Add(line);
//                         continue;
//                     }

//                     for (int start = 0; start < len; start += targetWidth)
//                     {
//                         int take = len - start;
//                         if (take > targetWidth)
//                             take = targetWidth;

//                         newLines.Add(line.Substring(start, take));
//                     }
//                 }

//                 page.Lines = newLines;
//             }
//         }
//     }
// }
