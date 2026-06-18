using BepInEx;
using HarmonyLib;
using System;
using Random = System.Random;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OwOText {
    [BepInPlugin("com.balaur.OwO", "OwOText", "1.0.2")]
    public class OwOText : BaseUnityPlugin {
        public static int seed = new Random().Next(1000);

        public static float lisp_chance;
        public static float nya_chance;
        public static double stutter_chance;
        public static int stutter_repeat_limit;
        public static double sub_fullstop_chance;
        public static double punct_repeat_chance;
        public static int punct_repeat_limit;
        public static double chaos_chance;
        public static double tilde_chance;
        public static double face_chance;

        public static bool apply_everywhere = false;
        public static bool apply_dialogue = false;
        public static bool apply_written_text = false;
        public static bool apply_tooltips = false;
        public static bool apply_other = false;
        public static bool apply_pause = false;
        public static string[] faces =
            { ":<", ":>", "c:", ":c", "umu", ">//>", "x3", ">v>", "UvU", ":3", ":3c", ":P", ":p", "-w-", "=w=", ";w;", ">w>", "<w<", "<//<", "<v<" };
        
        private void Awake() {
            var l = Logger;
            Logger.LogInfo("Initializing...");
            var harmony = new Harmony("com.balaur.OwO");

            MethodInfo? target;
            MethodInfo? patch;

            target = typeof(Locale).GetMethod("GetString");
            patch = typeof(OwOText).GetMethod("PostfixString");
            DoPatch(harmony, target, postfix: patch);
            
            target = typeof(Talker).GetMethod("Talk", new Type[] { typeof(List<string>), typeof(Limb), typeof(bool), typeof(bool) });
            patch = typeof(OwOText).GetMethod("PrefixTalker");
            DoPatch(harmony, target, prefix: patch);

            target = typeof(Locale).GetMethod("GetPdaNote", new Type[] {typeof(int)});
            patch = typeof(OwOText).GetMethod("PostfixEpda");
            DoPatch(harmony, target, postfix: patch);

            target = typeof(Locale).GetMethod("GetNote");
            patch = typeof(OwOText).GetMethod("PostfixNote");
            DoPatch(harmony, target, postfix: patch);

            target = typeof(Locale).GetMethod("GetItem");
            patch = typeof(OwOText).GetMethod("PostfixTooltip");
            DoPatch(harmony, target, postfix: patch);

            target = typeof(Locale).GetMethod("GetBuilding");
            patch = typeof(OwOText).GetMethod("PostfixTooltip");
            DoPatch(harmony, target, postfix: patch);

            target = typeof(Locale).GetMethod("GetMoodle");
            patch = typeof(OwOText).GetMethod("PostfixTooltip");
            DoPatch(harmony, target, postfix: patch);

            target = typeof(Locale).GetMethod("GetOther");
            patch = typeof(OwOText).GetMethod("PostfixOther");
            DoPatch(harmony, target, postfix: patch);

            target = typeof(Locale).GetMethod("GetTutorial");
            patch = typeof(OwOText).GetMethod("PostfixOther");
            DoPatch(harmony, target, postfix: patch);

            target = typeof(Locale).GetMethod("GetPauseQuote");
            patch = typeof(OwOText).GetMethod("PostfixPause");
            DoPatch(harmony, target, postfix: patch);

            target = typeof(Settings).GetMethod("DefaultSettings");
            patch = typeof(OwOText).GetMethod("PostfixDefaultSettings");
            DoPatch(harmony, target, postfix: patch);

            target = typeof(Locale).GetMethod("LoadLanguage");
            patch = typeof(OwOText).GetMethod("PostfixLoadLanguage");
            DoPatch(harmony, target, postfix: patch);
        }

        private void DoPatch(Harmony harmony, MethodInfo? target, MethodInfo? prefix = null, MethodInfo? postfix = null) {
            // TODO: Logging for null target.
            var pre = prefix != null ? new HarmonyMethod(prefix) : null;
            var post = postfix != null ? new HarmonyMethod(postfix) : null;
            harmony.Patch(target, prefix: pre, postfix: post);
        }

        // I thought about making this an extension, but I think there's a tiny chance of mod conflict doing that.
        // No NextSingle function in net standard 2.1.
        private static float RandomFloat(Random rng) {
            return (float)rng.NextDouble();
        }

        // I could do reflection stuff to get the game's GetSetting function and all that stuff.
        // But this is just way faster. Taken from decomp.
        private static T GetSetting<T>(string name) where T : Setting {
            return Settings.settings.Find((Setting s) => s.name == name) as T;
        }
        
        #region Patches
        public static void PostfixLoadLanguage() {
            Locale.currentLang.other.Add("gamesetapplyowotoeverywhere", "Apply OwO to everywhere");
            Locale.currentLang.other.Add("gamesetapplyowotoeverywheredsc", "Performs modifiers on nearly everything. This will break some coloured text.");

            Locale.currentLang.other.Add("gamesetapplyowotodialogue", "Apply OwO to dialogue");
            Locale.currentLang.other.Add("gamesetapplyowotodialoguedsc", "Performs modifiers on any spoken dialogue.");

            Locale.currentLang.other.Add("gamesetapplyowototooltips", "Apply OwO to gameplay hover");
            Locale.currentLang.other.Add("gamesetapplyowototooltipsdsc", "Performs modifiers on moodles, buildings and items. Doesn't apply to many other tooltips, they're affected by 'other'");

            Locale.currentLang.other.Add("gamesetapplyowotoother", "Apply OwO to other");
            Locale.currentLang.other.Add("gamesetapplyowotootherdsc", "Performs modifiers on a lot of things, like settings menus, the health panel, the tutorial.");

            Locale.currentLang.other.Add("gamesetapplyowotopause", "Apply OwO to pause");
            Locale.currentLang.other.Add("gamesetapplyowotopausedsc", "Performs modifiers on pause quotes.");

            Locale.currentLang.other.Add("gamesetapplyowotowritten", "Apply OwO to written");
            Locale.currentLang.other.Add("gamesetapplyowotowrittendsc", "Performs modifiers on EPDAs and notes.");

            Locale.currentLang.other.Add("gamesetstutterchance", "Stutter chance");
            Locale.currentLang.other.Add("gamesetstutterchancedsc", "Chance for the start of a word to s-s-sutter. Default 5%. L-Limited to 5 stutters to prevent infinite loops.");

            Locale.currentLang.other.Add("gamesetsubfullstopchance", "Substitute full stop chance");
            Locale.currentLang.other.Add("gamesetsubfullstopchancedsc", "Chance for a full stop to become an exclamation mark! Default 10%!");
            
            Locale.currentLang.other.Add("gamesetpunctrepeatchance", "Punctuation repeat chance");
            Locale.currentLang.other.Add("gamesetpunctrepeatchancedsc", "Chance for punctuation to repeat!!! Default 33%.. Limited to 5 repeats......");
            
            Locale.currentLang.other.Add("gamesetchaoschance", "Chaos chance");
            Locale.currentLang.other.Add("gamesetchaoschancedsc", "Chance for o to be replaced with owo, and u with uwu. Default 3%.");
            
            Locale.currentLang.other.Add("gamesettildechance", "Tilde chance");
            Locale.currentLang.other.Add("gamesettildechancedsc", "Chance to replace . with ~, or to place ~ at the end of sentences. Default 10%~");
            
            Locale.currentLang.other.Add("gamesetfacechance", "Emote chance");
            Locale.currentLang.other.Add("gamesetfacechancedsc", ":3 Chance to add emoticons to the start or end of sentences, or replace commas. Default 5%. -w-");
            
            Locale.currentLang.other.Add("gamesetlispchance", "Lisp chance");
            Locale.currentLang.other.Add("gamesetlispchancedsc", "How wikewy it is fow l and r to be weplaced with w. Default 100%.");
            
            Locale.currentLang.other.Add("gamesetnyachance", "Nya chance");
            Locale.currentLang.other.Add("gamesetnyachancedsc", "Chance for n+vowel or m+vowel to become nya, mya, Mye, NYE, etc. Default 15%");
        }
        
        public static void PostfixDefaultSettings(List<Setting> __result) {
            List<Setting> my_settings = new List<Setting> {
                new SettingBool
                {
                    name = "applyowotoeverywhere",
                    value = false,
                    apply = delegate
                    {
                        OwOText.apply_everywhere = GetSetting<SettingBool>("applyowotoeverywhere").value;
                    },
                    category = Setting.SettingCategory.Game
                },
                new SettingBool
                {
                    name = "applyowotodialogue",
                    value = true,
                    apply = delegate
                    {
                        OwOText.apply_dialogue = GetSetting<SettingBool>("applyowotodialogue").value;
                    },
                    category = Setting.SettingCategory.Game
                },
                new SettingBool
                {
                    name = "applyowotowritten",
                    value = true,
                    apply = delegate
                    {
                        OwOText.apply_written_text = GetSetting<SettingBool>("applyowotowritten").value;
                    },
                    category = Setting.SettingCategory.Game
                },
                new SettingBool
                {
                    name = "applyowototooltips",
                    value = false,
                    apply = delegate
                    {
                        OwOText.apply_tooltips = GetSetting<SettingBool>("applyowototooltips").value;
                    },
                    category = Setting.SettingCategory.Game
                },
                new SettingBool
                {
                    name = "applyowotoother",
                    value = false,
                    apply = delegate
                    {
                        OwOText.apply_other = GetSetting<SettingBool>("applyowotoother").value;
                    },
                    category = Setting.SettingCategory.Game
                },
                new SettingBool
                {
                    name = "applyowotopause",
                    value = true,
                    apply = delegate
                    {
                        OwOText.apply_pause = GetSetting<SettingBool>("applyowotopause").value;
                    },
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "lispchance",
                    value = 1f,
                    max = 1f,
                    min = 0f,
                    apply = delegate {
                        OwOText.lisp_chance = GetSetting<SettingFloat>("lispchance").value;
                    },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "stutterchance",
                    value = 1f / 20,
                    max = 1f,
                    min = 0f,
                    apply = delegate {
                        OwOText.stutter_chance = GetSetting<SettingFloat>("stutterchance").value;
                    },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "tildechance",
                    value = 1f / 10,
                    max = 1f,
                    min = 0f,
                    apply = delegate {
                        OwOText.tilde_chance = GetSetting<SettingFloat>("tildechance").value;
                    },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "facechance",
                    value = 1f / 20,
                    max = 1f,
                    min = 0f,
                    apply = delegate {
                        OwOText.face_chance = GetSetting<SettingFloat>("facechance").value;
                    },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "nyachance",
                    value = 0.15f,
                    max = 1f,
                    min = 0f,
                    apply = delegate {
                        OwOText.nya_chance = GetSetting<SettingFloat>("nyachance").value;
                    },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
                new SettingFloat {
                    name = "chaoschance",
                    value = 0.03f,
                    max = 1f,
                    min = 0f,
                    apply = delegate {
                        OwOText.chaos_chance = GetSetting<SettingFloat>("chaoschance").value;
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
                        OwOText.punct_repeat_chance = GetSetting<SettingFloat>("punctrepeatchance").value;
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
                        OwOText.sub_fullstop_chance = GetSetting<SettingFloat>("subfullstopchance").value;
                    },
                    formatValue = (float v) => Mathf.RoundToInt(v * 100f) + "%",
                    category = Setting.SettingCategory.Game
                },
            };

            __result.AddRange(my_settings);
        }
        
        public static void PostfixString(ref string __result) {
            if (!apply_everywhere)
                return;
            __result = MakeOwO(__result);
        }
        
        public static bool PrefixTalker(List<string> lines) {
            if (!apply_dialogue || apply_everywhere)
                return true;
            for (int i = 0; i < lines.Count; i++) {
                lines[i] = MakeOwO(lines[i]);
            }
            return true;
        }
        
        public static void PostfixEpda(ref (string text, string sprite) __result) {
            if (!apply_written_text || apply_everywhere)
                return;
            __result.text = MakeOwO(__result.text);
        }
        
        public static void PostfixNote(ref (string text, string sprite, string font) __result) {
            if (!apply_written_text || apply_everywhere)
                return;
            __result.text = MakeOwO(__result.text);
        }
        
        public static void PostfixTooltip(ref string __result) {
            if (!apply_tooltips || apply_everywhere)
                return;
            __result = MakeOwO(__result);
        }
        
        public static void PostfixOther(ref string __result) {
            if (!apply_other || apply_everywhere)
                return;
            __result = MakeOwO(__result);
        }
        
        public static void PostfixPause(ref string __result) {
            // Pause doesn't use GetString.
            if (!apply_everywhere && !apply_pause)
                return;
            __result = MakeOwO(__result);
        }
        #endregion

        public static string MakeOwO(string input) {
            // Based on my Stardew mod.
            string new_string = "";
            int len = input.Length;
            // This provides a consistent result. The same string and seed will always produce the same result.
            Random rng = new Random(input.GetHashCode() + seed);
            string this_loop = "";
            for (int i = 0; i < len; i++) {
                char c = input[i];

                this_loop += Lisp(c, rng);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Stutter(input, i, c, rng);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Nya(input, i, c, rng);
                c = this_loop != "" ? this_loop[^1] : c;

                this_loop += Chaos(c, rng);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Tilde(input, i, c, rng);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Excitement(c, rng);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Faces(input, i, c, rng);
                c = this_loop != "" ? this_loop[^1] : c;

                if (this_loop == "")
                    new_string += c.ToString();
                else
                    new_string += this_loop;
                this_loop = "";
            }

            return new_string;
        }

        #region OwO effects
        private static string Lisp(char input, Random rng) {
            if ("rlRL".IndexOf(input) == -1 || RandomFloat(rng) > lisp_chance) {
                return "";
            }
            
            switch (input) {
                case 'r':
                case 'l':
                    return "w";
                case 'R':
                case 'L':
                    return "W";
            }

            return "";
        }

        private static string Stutter(string source, int index, char character, Random rng) {
            // Only on the start of words.
            if (index != 0 && source[index - 1] != ' ') {
                return "";
            }
            if (!Char.IsLetter(character))
                return "";

            string new_string = "";
            bool first = true;
            int stutters = 0;
            while (RandomFloat(rng) < stutter_chance && stutters < stutter_repeat_limit) {
                if (first) {
                    new_string = character.ToString();
                    first = false;
                }
                new_string += $"-{character}";
                stutters++;
            }

            return new_string;
        }

        private static string Nya(string source, int index, char character, Random rng) {
            string new_string = "";
            if ("nmNM".IndexOf(character) == -1)
                return "";
            // Next char must be vowel.
            if (index == source.Length - 1)
                return "";
            if ("aeiouAEIOU".IndexOf(source[index + 1]) == -1)
                return "";

            if (RandomFloat(rng) > nya_chance) {
                return "";
            }

            char next_char = source[index + 1];
            if (Char.IsLower(character))
                new_string = $"{character}y";
            else {
                if (Char.IsUpper(next_char)) {
                    new_string = $"{character}Y";
                }
                else {
                    new_string = $"{character}y";
                }
            }

            return new_string;
        }

        private static string Chaos(char character, Random rng) {
            if ("ouOU".IndexOf(character) == -1)
                return "";
            if (!(chaos_chance > RandomFloat(rng)))
                return "";

            switch (character) {
                case 'o':
                    return "owo";
                case 'u':
                    return "uwu";
                case 'O':
                    return "OwO";
                case 'U':
                    return "UwU";
            }

            return "";
        }

        private static string Tilde(string source, int index, char character, Random rng) {
            if ((index != source.Length - 1 && ".!?".IndexOf(character) == -1) || !(tilde_chance > RandomFloat(rng))) {
                return "";
            }

            // Replace
            if (character == '.') {
                return "~";
            }
            
            // Suffix
            // This can cause unintended repetition behaviour when combined with the Excitement modifier,
            // but that's fine. no one will notice.
            return $"{character}~";
        }
        
        private static string Excitement(char character, Random rng) {
            if (".!?".IndexOf(character) == -1)
                return "";

            string new_string = "";
            if (character == '.') {
                if (sub_fullstop_chance > RandomFloat(rng)) {
                    character = '!';
                    new_string = "!";
                }
            }

            int repeats = 0;
            while (punct_repeat_chance > RandomFloat(rng) && repeats < punct_repeat_limit) {
                if (new_string == "") {
                    new_string = character.ToString();
                }
                new_string += character.ToString();
                repeats += 1;
            }

            return new_string;
        }

        private static string Faces(string source, int index, char character, Random rng) {
            if (!(face_chance > RandomFloat(rng))) {
                return "";
            }
            string face = faces[rng.Next(faces.Length)];

            // Start of sentence
            if (index == 0) {
                return $"{face} {character}";
            }
            
            // Replace ,
            if (character == ',') {
                return $" {face}";
            }
            
            // Place after punctuation
            char last_char = source[index - 1];
            if (character == ' ' && ".!?".IndexOf(last_char) != -1) {
                return $" {face} ";
            }
            
            return "";
        }
        #endregion
    }
}