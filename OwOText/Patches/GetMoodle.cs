using HarmonyLib;

namespace OwOText.Patches {
    [HarmonyPatch(typeof(Locale), nameof(Locale.GetMoodle))]
    public class GetMoodle {
        public static void Postfix(ref string __result) {
            if (!OwOText.apply_tooltips && !OwOText.apply_everywhere)
                return;
            __result = OwOText.MakeOwO(__result);
        }
    }
}