using System;
using HarmonyLib;

namespace OwOText.Patches {
    [HarmonyPatch(typeof(Locale), nameof(Locale.GetPdaNote), new Type[] { typeof(int) })]
    public class GetPdaNote {
        public static void Postfix(ref (string text, string sprite) __result) {
            if (!OwOText.apply_written_text && !OwOText.apply_everywhere)
                return;
            __result.text = OwOText.MakeOwO(__result.text);
        }
    }
}