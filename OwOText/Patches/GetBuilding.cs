using HarmonyLib;

namespace OwOText.Patches {
    [HarmonyPatch(typeof(Locale), nameof(Locale.GetBuilding))]
    public class GetBuilding {
        public static void Postfix(ref string __result) {
            if (!OwOText.apply_tooltips && !OwOText.apply_everywhere)
                return;
            __result = OwOText.MakeOwO(__result);
        }
    }
}