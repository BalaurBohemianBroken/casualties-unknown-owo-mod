using System;
using System.Collections.Generic;
using HarmonyLib;

namespace OwOText.Patches {
    [HarmonyPatch(typeof(Talker), nameof(Talker.Talk), new Type[] { typeof(List<string>), typeof(Limb), typeof(bool), typeof(bool) })]
    public class Talk {
        public static bool Prefix(List<string> lines) {
            if (!OwOText.apply_dialogue && !OwOText.apply_everywhere)
                return true;
            for (int i = 0; i < lines.Count; i++) {
                lines[i] = OwOText.MakeOwO(lines[i]);
            }

            return true;
        }
    }
}