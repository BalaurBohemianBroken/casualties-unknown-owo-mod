using HarmonyLib;

namespace OwOText.Patches {
    [HarmonyPatch(typeof(Locale), nameof(Locale.GetString))]
    public class GetString {
        public static void Postfix(ref string __result) {
            if (!OwOText.apply_everywhere)
                return;
            __result = OwOText.MakeOwO(__result);
        }
    }
}