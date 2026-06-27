using HarmonyLib;

namespace OwOText.Patches {
    [HarmonyPatch(typeof(Locale), nameof(Locale.GetNote))]
    public class GetNote {
        public static void Postfix(ref (string text, string sprite, string font) __result) {
            if (!OwOText.apply_written_text && !OwOText.apply_everywhere)
                return;
            __result.text = OwOText.MakeOwO(__result.text);
        }
    }
}