using HarmonyLib;

namespace OwOText.Patches {
    [HarmonyPatch(typeof(Locale), nameof(Locale.GetItem))]
    public class GetItem {
        public static void Postfix(ref string __result) {
            if (!OwOText.apply_tooltips && !OwOText.apply_everywhere)
                return;
            __result = OwOText.MakeOwO(__result);
        }
    }
}