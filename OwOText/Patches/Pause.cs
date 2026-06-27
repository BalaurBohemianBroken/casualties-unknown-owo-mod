using HarmonyLib;

namespace OwOText.Patches {
    [HarmonyPatch(typeof(Locale), nameof(Locale.GetPauseQuote))]
    public class Pause {
        public static void Postfix(ref string __result) {
            // Pause doesn't use GetString.
            if (!OwOText.apply_everywhere && !OwOText.apply_pause)
                return;
            __result = OwOText.MakeOwO(__result);
        }
    }
}