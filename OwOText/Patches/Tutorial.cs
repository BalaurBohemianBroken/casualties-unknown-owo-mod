using HarmonyLib;

namespace OwOText.Patches {
    [HarmonyPatch(typeof(Locale), "GetTutorial")]
    public class Tutorial {
        public static void Postfix(ref string __result) {
            if (!OwOText.apply_other && !OwOText.apply_everywhere)
                return;
            __result = OwOText.MakeOwO(__result);
        }
    }
}