using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace OwOText.Patches {
    [HarmonyPatch(typeof(Locale), "LoadLanguage")]
    public class LoadLanguage {
        public static void Postfix() {
            Locale.currentLang.other.Add("gamesetapplyowotoeverywhere", "Apply OwO to everywhere");
            Locale.currentLang.other.Add("gamesetapplyowotoeverywheredsc",
                "Performs modifiers on nearly everything. This will break some coloured text.");

            Locale.currentLang.other.Add("gamesetapplyowotodialogue", "Apply OwO to dialogue");
            Locale.currentLang.other.Add("gamesetapplyowotodialoguedsc", "Performs modifiers on any spoken dialogue.");

            Locale.currentLang.other.Add("gamesetapplyowototooltips", "Apply OwO to gameplay hover");
            Locale.currentLang.other.Add("gamesetapplyowototooltipsdsc",
                "Performs modifiers on moodles, buildings and items. Doesn't apply to many other tooltips, they're affected by 'other'");

            Locale.currentLang.other.Add("gamesetapplyowotoother", "Apply OwO to other");
            Locale.currentLang.other.Add("gamesetapplyowotootherdsc",
                "Performs modifiers on a lot of things, like settings menus, the health panel, the tutorial.");

            Locale.currentLang.other.Add("gamesetapplyowotopause", "Apply OwO to pause");
            Locale.currentLang.other.Add("gamesetapplyowotopausedsc", "Performs modifiers on pause quotes.");

            Locale.currentLang.other.Add("gamesetapplyowotowritten", "Apply OwO to written");
            Locale.currentLang.other.Add("gamesetapplyowotowrittendsc", "Performs modifiers on EPDAs and notes.");

            Locale.currentLang.other.Add("gamesetstutterchance", "Stutter begin chance");
            Locale.currentLang.other.Add("gamesetstutterchancedsc",
                "Chance for a s-stutter to begin on a word. Default 15%.");

            Locale.currentLang.other.Add("gamesetstutter_repeat_chance", "Stutter repeat chance");
            Locale.currentLang.other.Add("gamesetstutter_repeat_chancedsc",
                "Chance for stutter to r-r-repeat on a w-w-word once it begins. L-Limited to 5 stutters to prevent infinite loops. Default 20%");

            Locale.currentLang.other.Add("gamesetsubfullstopchance", "Substitute full stop chance");
            Locale.currentLang.other.Add("gamesetsubfullstopchancedsc",
                "Chance for a full stop to become an exclamation mark! Default 10%!");

            Locale.currentLang.other.Add("gamesetpunctrepeatchance", "Punctuation repeat chance");
            Locale.currentLang.other.Add("gamesetpunctrepeatchancedsc",
                "Chance for punctuation to repeat!!! Default 33%.. Limited to 5 repeats......");

            Locale.currentLang.other.Add("gamesetchaoschance", "Chaos chance");
            Locale.currentLang.other.Add("gamesetchaoschancedsc",
                "Chance for o to be replaced with owo, and u with uwu. Default 3%.");

            Locale.currentLang.other.Add("gamesettildechance", "Tilde chance");
            Locale.currentLang.other.Add("gamesettildechancedsc",
                "Chance to replace . with ~, or to place ~ at the end of sentences. Default 10%~");

            Locale.currentLang.other.Add("gamesetfacechance", "Emote chance");
            Locale.currentLang.other.Add("gamesetfacechancedsc",
                ":3 Chance to add emoticons to the start or end of sentences, or replace commas. Default 5%. -w-");

            Locale.currentLang.other.Add("gamesetlispchance", "Lisp chance");
            Locale.currentLang.other.Add("gamesetlispchancedsc",
                "How wikewy it is fow l and r to be weplaced with w. Default 100%.");

            Locale.currentLang.other.Add("gamesettrail_off_chance", "Trail off chance");
            Locale.currentLang.other.Add("gamesettrail_off_chancedsc",
                "The chance for dialogue... to trail off arbitrarily... default 5%.");

            Locale.currentLang.other.Add("gamesetnyachance", "Nya chance");
            Locale.currentLang.other.Add("gamesetnyachancedsc",
                "Chance for n+vowel or m+vowel to become nya, mya, Mye, NYE, etc. Default 15%");
        }
    }

    [HarmonyPatch(typeof(Settings), "DefaultSettings")]
    public class DefaultSettings {
        public static void Postfix(List<Setting> __result) {
            List<Setting> my_settings = new List<Setting> {
                new SettingBool {
                    name = "applyowotoeverywhere",
                    value = false,
                    apply = delegate {
                        OwOText.apply_everywhere = Settings.Get<SettingBool>("applyowotoeverywhere").value;
                    },
                    category = Setting.SettingCategory.Game
                },
                new SettingBool {
                    name = "applyowotodialogue",
                    value = true,
                    apply = delegate {
                        OwOText.apply_dialogue = Settings.Get<SettingBool>("applyowotodialogue").value;
                    },
                    category = Setting.SettingCategory.Game
                },
                new SettingBool {
                    name = "applyowotowritten",
                    value = true,
                    apply = delegate {
                        OwOText.apply_written_text = Settings.Get<SettingBool>("applyowotowritten").value;
                    },
                    category = Setting.SettingCategory.Game
                },
                new SettingBool {
                    name = "applyowototooltips",
                    value = false,
                    apply = delegate {
                        OwOText.apply_tooltips = Settings.Get<SettingBool>("applyowototooltips").value;
                    },
                    category = Setting.SettingCategory.Game
                },
                new SettingBool {
                    name = "applyowotoother",
                    value = false,
                    apply = delegate { OwOText.apply_other = Settings.Get<SettingBool>("applyowotoother").value; },
                    category = Setting.SettingCategory.Game
                },
                new SettingBool {
                    name = "applyowotopause",
                    value = true,
                    apply = delegate { OwOText.apply_pause = Settings.Get<SettingBool>("applyowotopause").value; },
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "lispchance",
                    value = 1f,
                    max = 1f,
                    min = 0f,
                    apply = delegate { OwOText.lisp_chance = Settings.Get<SettingFloat>("lispchance").value; },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "stutterchance",
                    value = 0.15f,
                    max = 1f,
                    min = 0f,
                    apply = delegate {
                        OwOText.first_stutter_chance = Settings.Get<SettingFloat>("stutterchance").value;
                    },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "stutter_repeat_chance",
                    value = 0.20f,
                    max = 1f,
                    min = 0f,
                    apply = delegate {
                        OwOText.stutter_repeat_chance = Settings.Get<SettingFloat>("stutter_repeat_chance").value;
                    },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "tildechance",
                    value = 1f / 10,
                    max = 1f,
                    min = 0f,
                    apply = delegate { OwOText.tilde_chance = Settings.Get<SettingFloat>("tildechance").value; },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "facechance",
                    value = 1f / 20,
                    max = 1f,
                    min = 0f,
                    apply = delegate { OwOText.face_chance = Settings.Get<SettingFloat>("facechance").value; },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "nyachance",
                    value = 0.15f,
                    max = 1f,
                    min = 0f,
                    apply = delegate { OwOText.nya_chance = Settings.Get<SettingFloat>("nyachance").value; },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "chaoschance",
                    value = 0.03f,
                    max = 1f,
                    min = 0f,
                    apply = delegate { OwOText.chaos_chance = Settings.Get<SettingFloat>("chaoschance").value; },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "trail_off_chance",
                    value = 0.05f,
                    max = 1f,
                    min = 0f,
                    apply = delegate {
                        OwOText.trail_off_chance = Settings.Get<SettingFloat>("trail_off_chance").value;
                    },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "punctrepeatchance",
                    value = 1f / 3,
                    max = 1f,
                    min = 0f,
                    apply = delegate {
                        OwOText.punct_repeat_chance = Settings.Get<SettingFloat>("punctrepeatchance").value;
                    },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "subfullstopchance",
                    value = 0.1f,
                    max = 1f,
                    min = 0f,
                    apply = delegate {
                        OwOText.sub_fullstop_chance = Settings.Get<SettingFloat>("subfullstopchance").value;
                    },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
            };

            __result.AddRange(my_settings);
        }
    }
}