using HarmonyLib;

namespace OwOText.Patches {
    [HarmonyPatch(typeof(Locale), "GetOther")]
    public class Other {
        public static void Postfix(ref string __result) {
            if (!OwOText.apply_other && !OwOText.apply_everywhere)
                return;
            __result = OwOText.MakeOwO(__result);
        }
    }
}