using BepInEx;
using HarmonyLib;
using System;
using Random = System.Random;
using TMPro;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;

namespace OwOText {
    [BepInPlugin("com.balaur.OwO", "OwOText", "0.1.2")]
    public class OwOText : BaseUnityPlugin {
        public static int seed = new Random().Next(1000);
        
        public static double stutter_chance = 1.0/20;
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

            var target = typeof(Talker).GetMethod("Talk", new Type[] { typeof(List<string>), typeof(Limb), typeof(bool), typeof(bool) });
            Logger.LogInfo($"Targeting: {target}");
            var patch = typeof(OwOText).GetMethod("PatchTalker");
            DoPatch(harmony, target, prefix: patch);
        }

        private void DoPatch(Harmony harmony, MethodInfo target, MethodInfo? prefix = null, MethodInfo? postfix = null) {
            var pre = prefix != null ? new HarmonyMethod(prefix) : null;
            var post = postfix != null ? new HarmonyMethod(postfix) : null;
            harmony.Patch(target, prefix: pre, postfix: post);
        }

        public static bool PatchTalker(List<string> lines) {
            for (int i = 0; i < lines.Count; i++) {
                lines[i] = MakeOwO(lines[i]);
            }
            return true;
        }

        public static string MakeOwO(string input) {
            // Based on my Stardew mod.
            string new_string = "";
            int len = input.Length;
            // This provides a consistent result. The same string and seed will always produce the same result.
            Random rng = new Random(input.GetHashCode() + seed);
            string this_loop = "";
            for (int i = 0; i < len; i++) {
                char c = input[i];

                this_loop += Lisp(c);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Stutter(input, i, c, rng);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Nya(input, i, c);
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

        private static string Lisp(char input) {
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
            while (rng.NextDouble() < stutter_chance) {
                if (first) {
                    new_string = character.ToString();
                    first = false;
                }
                new_string += $"-{character}";
            }

            return new_string;
        }

        private static string Nya(string source, int index, char character) {
            string new_string = "";
            if ("nmNM".IndexOf(character) != -1)
                return "";
            // Next char must be vowel.
            if (index == source.Length - 1)
                return "";
            if ("aeiouAEIOU".IndexOf(source[index + 1]) == -1)
                return "";

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
            if (!(chaos_chance > rng.NextDouble()))
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
            if (".!?".IndexOf(character) == -1 || !(tilde_chance > rng.NextDouble())) {
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
                if (sub_fullstop_chance > rng.NextDouble()) {
                    character = '!';
                    new_string = "!";
                }
            }

            int repeats = 0;
            while (punct_repeat_chance > rng.NextDouble() && repeats < punct_repeat_limit) {
                if (new_string == "") {
                    new_string = character.ToString();
                }
                new_string += character.ToString();
                repeats += 1;
            }

            return new_string;
        }

        private static string Faces(string source, int index, char character, Random rng) {
            if (!(face_chance > rng.NextDouble())) {
                return "";
            }
            string face = faces[rng.Next(faces.Length)];

            // Start of sentence
            if (index == 0) {
                return $"{face} {character}";
            }
            
            // Replace ,
            if (character == ',') {
                return $" {face} ";
            }
            
            // Place after punctuation
            char last_char = source[index - 1];
            if (character == ' ' && ".!?".IndexOf(last_char) != -1) {
                return $" {face} ";
            }
            
            return "";
        }
    }
}