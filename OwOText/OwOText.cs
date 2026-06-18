using BepInEx;
using HarmonyLib;
using System;
using Random = System.Random;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using OwOText;
using UnityEngine;

namespace OwOText {
    [BepInPlugin("com.balaur.OwO", "OwOText", "0.1.3")]
    public class OwOText : BaseUnityPlugin {
        public static int seed = new Random().Next(1000);

        public static float lisp_chance = 1f;
        public static float nya_chance = 1f/4;
        public static double stutter_chance = 1.0/20;
        public static int stutter_repeat_limit = 5;
        public static double sub_fullstop_chance = 1.0/5;
        public static double punct_repeat_chance = 1.0/3;
        public static int punct_repeat_limit = 5;
        public static double chaos_chance = 1.0/10;
        public static double tilde_chance = 1.0/10;
        public static double face_chance = 1.0/8;
        public static string[] faces =
            { ":<", ":>", "c:", ":c", "umu", ">//>", "x3", ">v>", "UvU", ":3", ":3c", ":P", ":p", "-w-", "=w=", ";w;", ">w>", "<w<", "<//<", "<v<" };

        // Not sure what the standard for logging from static is, but this'll do for now. 
        // public static ManualLogSource mod_logger;
        
        private void Awake() {
            var l = Logger;
            Logger.LogInfo("Initializing...");
            var harmony = new Harmony("com.balaur.OwO");

            MethodInfo? target;
            MethodInfo? patch;
            
            target = typeof(Talker).GetMethod("Talk", new Type[] { typeof(List<string>), typeof(Limb), typeof(bool), typeof(bool) });
            patch = typeof(OwOText).GetMethod("PrefixTalker");
            DoPatch(harmony, target, prefix: patch);

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
            Locale.currentLang.other.Add("gamesetstutterchance", "Stutter chance");
            Locale.currentLang.other.Add("gamesetstutterchancedsc", "Chance for the start of a word to s-s-sutter. Default 5%. L-Limited to 5 stutters to prevent infinite loops.");
            
            Locale.currentLang.other.Add("gamesetsubfullstopchance", "Substitute full stop chance");
            Locale.currentLang.other.Add("gamesetsubfullstopchancedsc", "Chance for a full stop to become an exclamation mark! Default 10%!");
            
            Locale.currentLang.other.Add("gamesetpunctrepeatchance", "Punctuation repeat chance");
            Locale.currentLang.other.Add("gamesetpunctrepeatchancedsc", "Chance for punctuation to repeat!!! Default 33%.. Limited to 5 repeats......");
            
            Locale.currentLang.other.Add("gamesetchaoschance", "Chaos chance");
            Locale.currentLang.other.Add("gamesetchaoschancedsc", "Chance for o to be replaced with owo, and u with uwu. Default 3%.");
            
            Locale.currentLang.other.Add("gamesettildechance", "Tilde chance");
            Locale.currentLang.other.Add("gamesettildechancedsc", "Chance to replace . with ~, or to place ~ after ! or ?. Default 10%~");
            
            Locale.currentLang.other.Add("gamesetfacechance", "Emote chance");
            Locale.currentLang.other.Add("gamesetfacechancedsc", ":3 Chance to add emoticons to the start or end of sentences, or replace commas. Default 5%. -w-");
            
            Locale.currentLang.other.Add("gamesetlispchance", "Lisp chance");
            Locale.currentLang.other.Add("gamesetlispchancedsc", "How wikewy it is fow l and r to be weplaced with w. Default 100%.");
            
            Locale.currentLang.other.Add("gamesetnyachance", "Nya chance");
            Locale.currentLang.other.Add("gamesetnyachancedsc", "Chance for n+vowel or m+vowel to become nya, mya, Mye, NYE, etc. Default 15%");
        }
        
        public static void PostfixDefaultSettings(List<Setting> __result) {
            List<Setting> my_settings = new List<Setting> {
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
        
        public static bool PrefixTalker(List<string> lines) {
            for (int i = 0; i < lines.Count; i++) {
                lines[i] = MakeOwO(lines[i]);
            }
            return true;
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
                
                this_loop += Tilde(c, rng);
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

        private static string Tilde(char character, Random rng) {
            if (".!?".IndexOf(character) == -1 || !(tilde_chance > RandomFloat(rng))) {
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