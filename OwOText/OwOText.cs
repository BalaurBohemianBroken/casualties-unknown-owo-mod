using BepInEx;
using HarmonyLib;
using System;
using Random = System.Random;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using UnityEngine;

namespace OwOText {
    [BepInPlugin("com.balaur.OwO", "OwOText", "1.0.6")]
    public class OwOText : BaseUnityPlugin {
        public static int seed = new Random().Next(1000);

        public static float lisp_chance;
        public static float nya_chance;
        public static float first_stutter_chance;
        public static float stutter_repeat_chance;
        public static int stutter_repeat_limit = 5;
        public static float sub_fullstop_chance;
        public static float punct_repeat_chance;
        public static int punct_repeat_limit = 5;
        public static float chaos_chance;
        public static float tilde_chance;
        public static float face_chance;
        public static float trail_off_chance;

        public static bool apply_everywhere = false;
        public static bool apply_dialogue = false;
        public static bool apply_written_text = false;
        public static bool apply_tooltips = false;
        public static bool apply_other = false;
        public static bool apply_pause = false;
        public static string[] faces = {
            ":<", 
            ":>", 
            "c:", 
            ":c", 
            "umu", 
            ">//>", 
            "x3", 
            ">v>", 
            "UvU", 
            ":3", 
            ":3c", 
            ":P", 
            ":p", 
            "-w-", 
            "=w=", 
            ";w;", 
            ">w>", 
            "<w<", 
            "<//<", 
            "<v<"
        };
        
        public static HashSet<string> modified_strings_this_frame = new HashSet<string>();
        public static ManualLogSource logger;
        
        private void Awake() {
            logger = Logger;
            var harmony = new Harmony("com.balaur.OwO");
            harmony.PatchAll();
        }

        private void LateUpdate() {
            // NOTE ON THIS: It may cause a very obscure bug!
            // If one object modifies a string, then this code runs, and then another object modifies the string,
            // It will cause a duplication. The very thing this is trying to avoid.
            // There is only one object that runs on LateUpdate at the moment, which is PlayerCamera.
            // This looks like it does do some UI stuff so that's a risk.
            modified_strings_this_frame = new HashSet<string>();
        }

        // I thought about making this an extension, but I think there's a tiny chance of mod conflict doing that.
        // No NextSingle function in net standard 2.1.
        private static float RandomFloat(Random rng) {
            return (float)rng.NextDouble();
        }

        public static string MakeOwO(string input) {
            // This should avoid us modifying the same string multiple times.
            if (modified_strings_this_frame.Contains(input))
                return input;
            
            // Based on my Stardew mod.
            string new_string = "";
            int len = input.Length;
            // This provides a consistent result. The same string and seed will always produce the same result.
            Random rng = new Random(input.GetHashCode() + seed);
            string this_loop = "";
            int open_tags = 0;
            for (int i = 0; i < len; i++) {
                char c = input[i];
                if (c == '<') {
                    open_tags++;
                    continue;
                }

                if (open_tags > 0) {
                    if (c == '>') {
                        open_tags--;
                    }
                }

                if (open_tags > 0) {
                    continue;
                }

                this_loop += Lisp(c, rng);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Stutter(input, i, c, rng, this_loop);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Nya(input, i, c, rng);
                c = this_loop != "" ? this_loop[^1] : c;

                this_loop += Chaos(c, rng);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Tilde(input, i, c, rng);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Excitement(c, rng, this_loop);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += Faces(input, i, c, rng, this_loop);
                c = this_loop != "" ? this_loop[^1] : c;
                
                this_loop += TrailOff(c, rng);
                c = this_loop != "" ? this_loop[^1] : c;

                if (this_loop == "")
                    new_string += c.ToString();
                else
                    new_string += this_loop;
                this_loop = "";
            }

            modified_strings_this_frame.Add(new_string);
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

        private static string Stutter(string source, int index, char character, Random rng, string current_text) {
            // Only on the start of words.
            if (index != 0 && source[index - 1] != ' ') {
                return "";
            }
            if (!Char.IsLetter(character))
                return "";

            string new_string = "";
            bool first = true;
            int stutters = 0;
            float _stutter_chance = first_stutter_chance;
            while (RandomFloat(rng) < _stutter_chance && stutters < stutter_repeat_limit) {
                if (first) {
                    first = false;
                    _stutter_chance = stutter_repeat_chance;
                    // Place initial character if doesn't exists.
                    if (current_text == "")
                        new_string = character.ToString();
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
        
        private static string Excitement(char character, Random rng, string current_text) {
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
                if (current_text == "") {
                    new_string = character.ToString();
                }
                new_string += character.ToString();
                repeats += 1;
            }

            return new_string;
        }

        private static string Faces(string source, int index, char character, Random rng, string current_text) {
            if (!(face_chance > RandomFloat(rng))) {
                return "";
            }
            string face = faces[rng.Next(faces.Length)];

            // Start of sentence
            if (index == 0 && current_text == "") {
                return $"{face} {character}";
            }
            
            // Replace ,
            if (character == ',') {
                return $" {face}";
            }
            
            // Place after punctuation
            if (index == 0)
                return "";
            char last_char = source[index - 1];
            if (character == ' ' && ".!?".IndexOf(last_char) != -1) {
                return $" {face} ";
            }
            
            return "";
        }

        private static string TrailOff(char character, Random rng) {
            if (character != ' ')
                return "";
            if (RandomFloat(rng) > trail_off_chance)
                return "";
            return "... ";
        }
        #endregion
    }
}