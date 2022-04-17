using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UE4DataTableInterpreter.Enums;

namespace KH3Randomizer.Data
{
    public static class Extensions
    {
        public static void ExecuteCommand(this string filePath)
        {
            int exitCode;

            var processStartInfo = new ProcessStartInfo()
            {
                Arguments = $"UnrealPak-With-Compression.bat {filePath}",
                FileName = @"call",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(processStartInfo);
            process.WaitForExit();
            
            var output = process.StandardOutput.ReadToEnd();

            exitCode = process.ExitCode;

            System.Diagnostics.Trace.TraceError("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            System.Diagnostics.Trace.TraceError("ExitCode: " + exitCode.ToString(), "ExecuteCommand");

            process.Close();
        }

        public static void Shuffle<T>(this IList<T> list, Random rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static long StringToSeed(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            var hash = 0L;
            foreach (var c in input.ToCharArray())
                hash = 31L * hash + c;

            return hash;
        }

        public static string QoLKeyToId(this string input)
        {
            var result = "";

            switch (input)
            {
                case "Easier Mini-UFO":
                    result = "BOSS_001";
                    break;
                case "Faster Raging Vulture":
                    result = "BOSS_002";
                    break;
                case "Dark Baymax Phase 1 Skip":
                    result = "BOSS_003";
                    break;
                case "Faster Lich Sequence":
                    result = "BOSS_004";
                    break;

                case "Frozen Chase Skip":
                    result = "EVENT_001";
                    break;
                case "Faster Crab Collection":
                    result = "EVENT_002";
                    break;
                case "Big Hero Rescue Skip":
                    result = "EVENT_003";
                    break;
                case "Faster Sora Collection":
                    result = "EVENT_004";
                    break;
                case "Union χ Skip":
                    result = "EVENT_005";
                    break;
                case "Guardians of Light Skip":
                    result = "EVENT_006";
                    break;
                case "Slow Mickey Skip":
                    result = "EVENT_007";
                    break;

                case "All Maps Unlocked":
                    result = "ITEM_003";
                    break;
            }

            return result;
        }

        public static string KeyIdToDisplay(this string input)
        {
            var inputSplit = input.Replace("\u0000", "").Split('_');

            if (inputSplit.Length == 3 && (inputSplit[1] == "SBOX" || inputSplit[1] == "LBOX"))
                return inputSplit[1] == "LBOX" ? $"Large Chest {int.Parse(inputSplit[2])}" : $"Small Chest {int.Parse(inputSplit[2])}";
            else if (inputSplit[0].ToLower() == "vbonus")
                if (inputSplit.Length == 3)
                    return $"Victory Bonus {inputSplit[1]} {int.Parse(inputSplit[2])}";
                else if (inputSplit[1].Contains("Minigame"))
                    return $"Victory Bonus {inputSplit[1]}";
                else
                    return $"Victory Bonus {int.Parse(inputSplit[1])}";
            else if (inputSplit[0] == "IW")
                return $"Level {(int.Parse(inputSplit[1]) % 10) + 1}";
            else if (inputSplit[0] == "IS")
                return $"Synthesis Item {int.Parse(inputSplit[1]) + 1}";

            switch (input.Replace("\u0000", ""))
            {
                case "m_PlayerSora":
                    return "Sora";

                #region Keyblades
                case "I03001":
                    return "Kingdom Key";
                case "I03002":
                    return "Hero's Origin";
                case "I03003":
                    return "Shooting Star";
                case "I03004":
                    return "Favorite Deputy";
                case "I03005":
                    return "Ever After";
                case "I03006":
                    return "Happy Gear";
                case "I03007":
                    return "Crystal Snow";
                case "I03008":
                    return "Hunny Spout";
                case "I03009":
                    return "Nano Gear";
                case "I03010":
                    return "Wheel of Fate";
                case "I03011":
                    return "Grand Chef";
                case "I03012":
                    return "Classic Tone";
                case "I03013":
                    return "Oathkeeper";
                case "I03014":
                    return "Oblivion";
                case "I03015":
                    return "Ultima Weapon";
                case "I03018":
                    return "Starlight";
                case "I03019":
                    return "Elemental Encoder";
                #endregion Keyblades

                #region Armor
                case "I04001":
                    return "Hero's Belt";
                case "I04002":
                    return "Hero's Glove";
                case "I04045":
                    return "Firefighter Rosette";
                case "I04046":
                    return "Umbrella Rosette";
                case "I04047":
                    return "Mask Rosette";
                case "I04048":
                    return "Snowman Rosette";
                case "I04049":
                    return "Insulator Rosette";
                #endregion Armor

                #region Accessories
                case "I05001":
                    return "Laughter Pin";
                case "I05002":
                    return "Forest Clasp";
                case "I05013":
                    return "Buster Ring";
                case "I05014":
                    return "Valor Ring";
                case "I05015":
                    return "Phantom Ring";
                case "I05016":
                    return "Orichalcum Ring";

                case "I05017":
                    return "Magic Ring";
                case "I05018":
                    return "Rune Ring";
                case "I05019":
                    return "Force Ring";
                case "I05020":
                    return "Sorcerer's Ring";
                case "I05021":
                    return "Wisdom Ring";

                case "I05022":
                    return "Bronze Necklace";
                case "I05023":
                    return "Silver Necklace";
                case "I05024":
                    return "Master's Necklace";

                case "I05025":
                    return "Bronze Amulet";
                case "I05026":
                    return "Silver Amulet";
                case "I05027":
                    return "Gold Amulet";

                case "I05028":
                    return "Junior Medal";
                case "I05029":
                    return "Star Medal";
                case "I05030":
                    return "Master Medal";

                case "I05031":
                    return "Mickey's Brooch";

                case "I05032":
                    return "Soldier's Earring";
                case "I05033":
                    return "Fencer's Earring";
                case "I05034":
                    return "Mage's Earring";
                case "I05035":
                    return "Slayer's Earring";

                case "I05036":
                    return "Moon Amulet";
                case "I05037":
                    return "Star Charm";
                case "I05038":
                    return "Cosmic Arts";

                case "I05039":
                    return "Crystal Regalia";

                case "I05040":
                    return "Water Cufflink";
                case "I05041":
                    return "Thunder Cufflink";
                case "I05042":
                    return "Fire Cufflink";
                case "I05043":
                    return "Aero Cufflink";
                case "I05044":
                    return "Blizzard Cufflink";
                case "I05045":
                    return "Celestriad";
                case "I05046":
                    return "Yin-Yang Cufflink";

                case "I05047":
                    return "Gourmand's Ring";
                case "I05048":
                    return "Draw Ring";
                case "I05049":
                    return "Lucky Ring";

                case "I05050":
                    return "Flanniversary Badge";
                
                case "I05051":
                case "I05052":
                case "I05053":
                case "I05054":
                case "I05055":
                case "I05056":
                case "I05057":
                case "I05058":
                case "I05059":
                case "I05060":
                case "I05061":
                case "I05062":
                    return "Bronze Necklace";

                case "I05063":
                case "I05064":
                case "I05065":
                case "I05066":
                case "I05067":
                case "I05068":
                case "I05069":
                case "I05070":
                case "I05071":
                case "I05072":
                case "I05073":
                    return "Silver Necklace";

                case "I05074":
                case "I05075":
                case "I05076":
                case "I05077":
                case "I05078":
                case "I05079":
                case "I05080":
                    return "Master's Necklace";

                case "I05081":
                case "I05082":
                    return "Star Medal";

                case "I05083":
                    return "Junior Medal";

                case "I05084":
                    return "Star Medal";

                case "I05085":
                case "I05086":
                case "I05087":
                case "I05088":
                case "I05089":
                case "I05090":
                case "I05091":
                case "I05092":
                    return "Junior Medal";

                case "I05093":
                    return "Master Medal";

                case "I05094":
                case "I05095":
                    return "Star Medal";

                case "I05096":
                    return "Master Medal";

                case "I05097":
                    return "Star Medal";

                case "I05098":
                    return "Master Medal";

                case "I05099":
                case "I05100":
                case "I05101":
                case "I05102":
                case "I05103":
                    return "Star Medal";
                
                case "I05104":
                case "I05105":
                case "I05106":
                case "I05107":
                case "I05108":
                case "I05109":
                case "I05110":
                    return "Master Medal";

                case "I05111":
                    return "Breakthrough";
                case "I05112":
                    return "Crystal Regalia+";
                #endregion Accessories

                #region Fullcourse
                case "item00":
                    return "Base Pool";
                case "item01":
                    return "1 Star Pool";
                case "item02":
                case "item03":
                case "item04":
                case "item05":
                    return "2 Star Pool";
                case "item06":
                case "item07":
                case "item08":
                case "item09":
                case "item10":
                    return "3 Star Pool";
                case "item11":
                case "item12":
                    return "4 Star Pool";
                case "item13":
                case "item14":
                    return "5 Star Pool";
                #endregion Fullcourse

                #region Weapon Upgrades
                case "IW_0":
                    return "Kingdom Key Upgrade 1";
                case "IW_1":
                    return "Kingdom Key Upgrade 2";
                case "IW_2":
                    return "Kingdom Key Upgrade 3";
                case "IW_3":
                    return "Kingdom Key Upgrade 4";
                case "IW_4":
                    return "Kingdom Key Upgrade 5";
                case "IW_5":
                    return "Kingdom Key Upgrade 6";
                case "IW_6":
                    return "Kingdom Key Upgrade 7";
                case "IW_7":
                    return "Kingdom Key Upgrade 8";
                case "IW_8":
                    return "Kingdom Key Upgrade 9";
                case "IW_9":
                    return "Kingdom Key Upgrade 10";

                case "IW_10":
                    return "Shooting Star Upgrade 1";
                case "IW_11":
                    return "Shooting Star Upgrade 2";
                case "IW_12":
                    return "Shooting Star Upgrade 3";
                case "IW_13":
                    return "Shooting Star Upgrade 4";
                case "IW_14":
                    return "Shooting Star Upgrade 5";
                case "IW_15":
                    return "Shooting Star Upgrade 6";
                case "IW_16":
                    return "Shooting Star Upgrade 7";
                case "IW_17":
                    return "Shooting Star Upgrade 8";
                case "IW_18":
                    return "Shooting Star Upgrade 9";
                case "IW_19":
                    return "Shooting Star Upgrade 10";

                case "IW_20":
                    return "Hero's Origin Upgrade 1";
                case "IW_21":
                    return "Hero's Origin Upgrade 2";
                case "IW_22":
                    return "Hero's Origin Upgrade 3";
                case "IW_23":
                    return "Hero's Origin Upgrade 4";
                case "IW_24":
                    return "Hero's Origin Upgrade 5";
                case "IW_25":
                    return "Hero's Origin Upgrade 6";
                case "IW_26":
                    return "Hero's Origin Upgrade 7";
                case "IW_27":
                    return "Hero's Origin Upgrade 8";
                case "IW_28":
                    return "Hero's Origin Upgrade 9";
                case "IW_29":
                    return "Hero's Origin Upgrade 10";

                case "IW_30":
                    return "Favorite Deputy Upgrade 1";
                case "IW_31":
                    return "Favorite Deputy Upgrade 2";
                case "IW_32":
                    return "Favorite Deputy Upgrade 3";
                case "IW_33":
                    return "Favorite Deputy Upgrade 4";
                case "IW_34":
                    return "Favorite Deputy Upgrade 5";
                case "IW_35":
                    return "Favorite Deputy Upgrade 6";
                case "IW_36":
                    return "Favorite Deputy Upgrade 7";
                case "IW_37":
                    return "Favorite Deputy Upgrade 8";
                case "IW_38":
                    return "Favorite Deputy Upgrade 9";
                case "IW_39":
                    return "Favorite Deputy Upgrade 10";
                
                case "IW_40":
                    return "Ever After Upgrade 1";
                case "IW_41":
                    return "Ever After Upgrade 2";
                case "IW_42":
                    return "Ever After Upgrade 3";
                case "IW_43":
                    return "Ever After Upgrade 4";
                case "IW_44":
                    return "Ever After Upgrade 5";
                case "IW_45":
                    return "Ever After Upgrade 6";
                case "IW_46":
                    return "Ever After Upgrade 7";
                case "IW_47":
                    return "Ever After Upgrade 8";
                case "IW_48":
                    return "Ever After Upgrade 9";
                case "IW_49":
                    return "Ever After Upgrade 10";

                case "IW_50":
                    return "Wheel of Fate Upgrade 1";
                case "IW_51":
                    return "Wheel of Fate Upgrade 2";
                case "IW_52":
                    return "Wheel of Fate Upgrade 3";
                case "IW_53":
                    return "Wheel of Fate Upgrade 4";
                case "IW_54":
                    return "Wheel of Fate Upgrade 5";
                case "IW_55":
                    return "Wheel of Fate Upgrade 6";
                case "IW_56":
                    return "Wheel of Fate Upgrade 7";
                case "IW_57":
                    return "Wheel of Fate Upgrade 8";
                case "IW_58":
                    return "Wheel of Fate Upgrade 9";
                case "IW_59":
                    return "Wheel of Fate Upgrade 10";

                case "IW_60":
                    return "Crystal Snow Upgrade 1";
                case "IW_61":
                    return "Crystal Snow Upgrade 2";
                case "IW_62":
                    return "Crystal Snow Upgrade 3";
                case "IW_63":
                    return "Crystal Snow Upgrade 4";
                case "IW_64":
                    return "Crystal Snow Upgrade 5";
                case "IW_65":
                    return "Crystal Snow Upgrade 6";
                case "IW_66":
                    return "Crystal Snow Upgrade 7";
                case "IW_67":
                    return "Crystal Snow Upgrade 8";
                case "IW_68":
                    return "Crystal Snow Upgrade 9";
                case "IW_69":
                    return "Crystal Snow Upgrade 10";

                case "IW_70":
                    return "Hunny Spout Upgrade 1";
                case "IW_71":
                    return "Hunny Spout Upgrade 2";
                case "IW_72":
                    return "Hunny Spout Upgrade 3";
                case "IW_73":
                    return "Hunny Spout Upgrade 4";
                case "IW_74":
                    return "Hunny Spout Upgrade 5";
                case "IW_75":
                    return "Hunny Spout Upgrade 6";
                case "IW_76":
                    return "Hunny Spout Upgrade 7";
                case "IW_77":
                    return "Hunny Spout Upgrade 8";
                case "IW_78":
                    return "Hunny Spout Upgrade 9";
                case "IW_79":
                    return "Hunny Spout Upgrade 10";

                case "IW_80":
                    return "Nano Gear Upgrade 1";
                case "IW_81":
                    return "Nano Gear Upgrade 2";
                case "IW_82":
                    return "Nano Gear Upgrade 3";
                case "IW_83":
                    return "Nano Gear Upgrade 4";
                case "IW_84":
                    return "Nano Gear Upgrade 5";
                case "IW_85":
                    return "Nano Gear Upgrade 6";
                case "IW_86":
                    return "Nano Gear Upgrade 7";
                case "IW_87":
                    return "Nano Gear Upgrade 8";
                case "IW_88":
                    return "Nano Gear Upgrade 9";
                case "IW_89":
                    return "Nano Gear Upgrade 10";

                case "IW_90":
                    return "Happy Gear Upgrade 1";
                case "IW_91":
                    return "Happy Gear Upgrade 2";
                case "IW_92":
                    return "Happy Gear Upgrade 3";
                case "IW_93":
                    return "Happy Gear Upgrade 4";
                case "IW_94":
                    return "Happy Gear Upgrade 5";
                case "IW_95":
                    return "Happy Gear Upgrade 6";
                case "IW_96":
                    return "Happy Gear Upgrade 7";
                case "IW_97":
                    return "Happy Gear Upgrade 8";
                case "IW_98":
                    return "Happy Gear Upgrade 9";
                case "IW_99":
                    return "Happy Gear Upgrade 10";

                case "IW_100":
                    return "Grand Chef Upgrade 1";
                case "IW_101":
                    return "Grand Chef Upgrade 2";
                case "IW_102":
                    return "Grand Chef Upgrade 3";
                case "IW_103":
                    return "Grand Chef Upgrade 4";
                case "IW_104":
                    return "Grand Chef Upgrade 5";
                case "IW_105":
                    return "Grand Chef Upgrade 6";
                case "IW_106":
                    return "Grand Chef Upgrade 7";
                case "IW_107":
                    return "Grand Chef Upgrade 8";
                case "IW_108":
                    return "Grand Chef Upgrade 9";
                case "IW_109":
                    return "Grand Chef Upgrade 10";

                case "IW_110":
                    return "Classic Tone Upgrade 1";
                case "IW_111":
                    return "Classic Tone Upgrade 2";
                case "IW_112":
                    return "Classic Tone Upgrade 3";
                case "IW_113":
                    return "Classic Tone Upgrade 4";
                case "IW_114":
                    return "Classic Tone Upgrade 5";
                case "IW_115":
                    return "Classic Tone Upgrade 6";
                case "IW_116":
                    return "Classic Tone Upgrade 7";
                case "IW_117":
                    return "Classic Tone Upgrade 8";
                case "IW_118":
                    return "Classic Tone Upgrade 9";
                case "IW_119":
                    return "Classic Tone Upgrade 10";

                case "IW_120":
                    return "Ultima Weapon Upgrade 1";
                case "IW_121":
                    return "Ultima Weapon Upgrade 2";
                case "IW_122":
                    return "Ultima Weapon Upgrade 3";
                case "IW_123":
                    return "Ultima Weapon Upgrade 4";
                case "IW_124":
                    return "Ultima Weapon Upgrade 5";
                case "IW_125":
                    return "Ultima Weapon Upgrade 6";
                case "IW_126":
                    return "Ultima Weapon Upgrade 7";
                case "IW_127":
                    return "Ultima Weapon Upgrade 8";
                case "IW_128":
                    return "Ultima Weapon Upgrade 9";
                case "IW_129":
                    return "Ultima Weapon Upgrade 10";

                case "IW_150":
                    return "Elemental Encoder Upgrade 1";
                case "IW_151":
                    return "Elemental Encoder Upgrade 2";
                case "IW_152":
                    return "Elemental Encoder Upgrade 3";
                case "IW_153":
                    return "Elemental Encoder Upgrade 4";
                case "IW_154":
                    return "Elemental Encoder Upgrade 5";
                case "IW_155":
                    return "Elemental Encoder Upgrade 6";
                case "IW_156":
                    return "Elemental Encoder Upgrade 7";
                case "IW_157":
                    return "Elemental Encoder Upgrade 8";
                case "IW_158":
                    return "Elemental Encoder Upgrade 9";
                case "IW_159":
                    return "Elemental Encoder Upgrade 10";

                case "IW_160":
                    return "Starlight Upgrade 1";
                case "IW_161":
                    return "Starlight Upgrade 2";
                case "IW_162":
                    return "Starlight Upgrade 3";
                case "IW_163":
                    return "Starlight Upgrade 4";
                case "IW_164":
                    return "Starlight Upgrade 5";
                case "IW_165":
                    return "Starlight Upgrade 6";
                case "IW_166":
                    return "Starlight Upgrade 7";
                case "IW_167":
                    return "Starlight Upgrade 8";
                case "IW_168":
                    return "Starlight Upgrade 9";
                case "IW_169":
                    return "Starlight Upgrade 10";

                case "IW_170":
                    return "Oathkeeper Upgrade 1";
                case "IW_171":
                    return "Oathkeeper Upgrade 2";
                case "IW_172":
                    return "Oathkeeper Upgrade 3";
                case "IW_173":
                    return "Oathkeeper Upgrade 4";
                case "IW_174":
                    return "Oathkeeper Upgrade 5";
                case "IW_175":
                    return "Oathkeeper Upgrade 6";
                case "IW_176":
                    return "Oathkeeper Upgrade 7";
                case "IW_177":
                    return "Oathkeeper Upgrade 8";
                case "IW_178":
                    return "Oathkeeper Upgrade 9";
                case "IW_179":
                    return "Oathkeeper Upgrade 10";

                case "IW_180":
                    return "Oblivion Upgrade 1";
                case "IW_181":
                    return "Oblivion Upgrade 2";
                case "IW_182":
                    return "Oblivion Upgrade 3";
                case "IW_183":
                    return "Oblivion Upgrade 4";
                case "IW_184":
                    return "Oblivion Upgrade 5";
                case "IW_185":
                    return "Oblivion Upgrade 6";
                case "IW_186":
                    return "Oblivion Upgrade 7";
                case "IW_187":
                    return "Oblivion Upgrade 8";
                case "IW_188":
                    return "Oblivion Upgrade 9";
                case "IW_189":
                    return "Oblivion Upgrade 10";
                #endregion Weapon Upgrades

                #region Events
                case "EVENT_001":
                    return "Forge Goofy's Shield";
                case "EVENT_002":
                    return "Find 1st Golden Herc Statue";
                case "EVENT_003":
                    return "Find 2nd Golden Herc Statue";
                case "EVENT_004":
                    return "Find 3rd Golden Herc Statue";
                case "EVENT_005":
                    return "Find 4th Golden Herc Statue";
                case "EVENT_006":
                    return "Find 5th Golden Herc Statue";
                case "EVENT_007":
                    return "Return All Golden Herc Statues";
                case "EVENT_008":
                    return "Defeat the Unversed before the Power Plant";
                case "EVENT_009":
                    return "Rescue the Trapped Big Hero 6 Members";

                case "TresUIMobilePortalDataAsset":
                    return "Complete All Classic Mickey Games";
                case "EVENT_KEYBLADE_001":
                    return "Complete Olympus";
                case "EVENT_KEYBLADE_002":
                    return "Complete Twilight Town";
                case "EVENT_KEYBLADE_003":
                    return "Complete Toy Box";
                case "EVENT_KEYBLADE_004":
                    return "Complete Kingdom of Corona";
                case "EVENT_KEYBLADE_005":
                    return "Complete Monstropolis";
                case "EVENT_KEYBLADE_006":
                    return "Complete 100 Acre Wood";
                case "EVENT_KEYBLADE_007":
                    return "Complete Arendelle";
                case "EVENT_KEYBLADE_008":
                    return "Complete The Caribbean";
                case "EVENT_KEYBLADE_009":
                    return "Complete San Fransokyo";
                case "EVENT_KEYBLADE_010":
                    return "Complete All of Tiny Chef's Recipes";
                case "EVENT_KEYBLADE_011":
                    return "Defeat Demon Tide in The Keyblade Graveyard";
                case "EVENT_KEYBLADE_012":
                    return "Return the Proof of Promises to the Moogle";
                case "EVENT_KEYBLADE_013":
                    return "Return the Proof of Times Past to the Moogle";
                
                case "EVENT_HEARTBINDER_001":
                    return "Received After Returning to Yen Sid's Tower";
                case "EVENT_HEARTBINDER_002":
                    return "Complete Verum Rex: Beat of Lead in Toy Box";
                case "EVENT_HEARTBINDER_003":
                    return "Received After Putting Out the Fires in Monstropolis";
                case "EVENT_HEARTBINDER_004":
                    return "Complete Flash Tracer in San Fransokyo";

                case "EVENT_REPORT_001a":
                case "EVENT_REPORT_001b":
                    return "Complete Battle Portal 1 in Olympus";
                case "EVENT_REPORT_002a":
                case "EVENT_REPORT_002b":
                    return "Complete Battle Portal 2 in Olympus";
                case "EVENT_REPORT_003a":
                case "EVENT_REPORT_003b":
                    return "Complete Battle Portal 3 in Twilight Town";
                case "EVENT_REPORT_004a":
                case "EVENT_REPORT_004b":
                    return "Complete Battle Portal 4 in Toy Box";
                case "EVENT_REPORT_005a":
                case "EVENT_REPORT_005b":
                    return "Complete Battle Portal 5 in Toy Box";
                case "EVENT_REPORT_006a":
                case "EVENT_REPORT_006b":
                    return "Complete Battle Portal 6 in Kingdom of Corona";
                case "EVENT_REPORT_007a":
                case "EVENT_REPORT_007b":
                    return "Complete Battle Portal 7 in Kingdom of Corona";
                case "EVENT_REPORT_008a":
                case "EVENT_REPORT_008b":
                    return "Complete Battle Portal 8 in Monstropolis";
                case "EVENT_REPORT_009a":
                case "EVENT_REPORT_009b":
                    return "Complete Battle Portal 9 in Arendelle";
                case "EVENT_REPORT_010a":
                case "EVENT_REPORT_010b":
                    return "Complete Battle Portal 10 in The Caribbean";
                case "EVENT_REPORT_011a":
                case "EVENT_REPORT_011b":
                    return "Complete Battle Portal 11 in San Fransokyo";
                case "EVENT_REPORT_012a":
                case "EVENT_REPORT_012b":
                    return "Complete Battle Portal 12 in San Fransokyo";
                case "EVENT_REPORT_013a":
                case "EVENT_REPORT_013b":
                    return "Complete Battle Portal 13 in The Keyblade Graveyard";
                case "EVENT_REPORT_014":
                    return "Defeat Dark Inferno";

                case "EVENT_CKGAME_001":
                    return "Complete Twilight Town";

                case "EVENT_KEYITEM_001":
                    return "Received After Returning to Yen Sid's Tower";
                case "EVENT_KEYITEM_002":
                    return "Received During First Visit to Hiro's Garage";
                case "EVENT_KEYITEM_003":
                    return "Find All Lucky Emblems";
                case "EVENT_KEYITEM_004":
                    return "Complete the Game on Critical";
                case "EVENT_KEYITEM_005":
                    return "Defeat Yozora";

                case "EVENT_DATAB_001":
                    return "Defeat Data Master Xehanort";
                case "EVENT_DATAB_002":
                    return "Defeat Data Ansem: Seeker of Darkness";
                case "EVENT_DATAB_003":
                    return "Defeat Data Xemnas";
                case "EVENT_DATAB_004":
                    return "Defeat Data Xigbar";
                case "EVENT_DATAB_005":
                    return "Defeat Data Luxord";
                case "EVENT_DATAB_006":
                    return "Defeat Data Larxene";
                case "EVENT_DATAB_007":
                    return "Defeat Data Marluxia";
                case "EVENT_DATAB_008":
                    return "Defeat Data Saix";
                case "EVENT_DATAB_009":
                    return "Defeat Data Terra-Xehanort";
                case "EVENT_DATAB_010":
                    return "Defeat Data Dark Riku";
                case "EVENT_DATAB_011":
                    return "Defeat Data Vanitas";
                case "EVENT_DATAB_012":
                    return "Defeat Data Young Xehanort";
                case "EVENT_DATAB_013":
                    return "Defeat Data Xion";

                case "EVENT_YOZORA_001":
                    return "Defeat Yozora";
                #endregion Events

                default:
                    return input;
            }
        }

        public static string KeyIdToDescription(this string input)
        {
            switch (input.Replace("\u0000", ""))
            {
                #region VBonus

                case "Vbonus_001":
                    return "After the first battle in Olympus - Cliff Ascent";
                case "Vbonus_002":
                    return "After the 1st Flame Core battle in Olympus - Thebes: Agora";
                case "Vbonus_005":
                    return "After the 2nd Flame Core battle in Olympus - Thebes: Overlook";
                case "Vbonus_006":
                    return "After the 3rd Flame Core battle in Olympus - Thebes: Gardens";
                case "Vbonus_007":
                    return "After the timed Heartless battle in Olympus - Thebes: Alleyway";
                case "Vbonus_008":
                    return "After the Rock Troll battle in Olympus - Thebes: Agora";
                case "Vbonus_009":
                    return "After the Water Core(?) battle on the way up mountain in Olympus";
                case "Vbonus_010":
                    return "After the Rock Titan boss battle in Olympus";
                case "Vbonus_011":
                    return "After the Satyr Mob battle in Olympus - Realm of the Gods: Courtyard";
                case "Vbonus_013":
                    return "After the Tornado Titan boss battle in Olympus";

                case "Vbonus_014":
                    return "After the Demon Tide battle in Twilight Town - Tram Common";
                case "Vbonus_015":
                    return "After the Powerwild Mob battle in Twilight Town - The Woods";
                case "Vbonus_016":
                    return "After the Heartless & Nobody Mob battle in Twilight Town - The Old Mansion";

                case "Vbonus_017":
                    return "After the Heartless Mob battle in Toy Box - Andy's House";
                case "Vbonus_018":
                    return "After the Gigas Mob battle in Toy Box - Galaxy Toys Main Floor: 1F";
                case "Vbonus_019":
                    return "After the Supreme Smashes battle in Toy Box - Action Figures";
                case "Vbonus_020":
                    return "After the Angelic Amber boss battle in Toy Box - Babies & Toddlers: Dolls";
                case "Vbonus_021":
                    return "After the UFO battle in Toy Box - Babies & Toddlers: Outdoors";
                case "Vbonus_022":
                    return "After the Verum Rex: Beat of Lead minigame in Toy Box";
                case "Vbonus_023":
                    return "After the King of Toys boss battle in Toy Box";


                case "Vbonus_024":
                    return "After the 1st Heartless Mob battle in Kingdom of Corona - Hills";
                case "Vbonus_025":
                    return "After the 2nd Heartless Mob battle in Kingdom of Corona - Hills";
                case "Vbonus_026":
                    return "After the Reapers Nobody battle in Kingdom of Corona - Hills";
                case "Vbonus_027":
                    return "After the Chaos Carriage Heartless battle in Kingdom of Corona";
                case "Vbonus_028":
                    return "After the Reapers Nobody battle in the Castle Town in Kingdom of Corona";
                case "Vbonus_029":
                    return "After the Heartless Mob battle in Kingdom of Corona - Tower";
                case "Vbonus_030":
                    return "After the Grim Guardianess boss battle in Kingdom of Corona - Tower";

                case "Vbonus_032":
                    return "After the Unversed battle in Monstropolis - Lobby & Offices";
                case "Vbonus_033":
                    return "After the Unversed battle in Monstropolis - Laugh Floor";
                case "Vbonus_034":
                    return "After the battle to make Boo laugh in in Monstropolis - The Door Vault: Upper Level";
                case "Vbonus_035":
                    return "After the Heartless & Unversed battle in Monstropolis - The Factory: Second Floor";
                case "Vbonus_036":
                    return "After the battle to make Boo laugh in Monstropolis - The Factory: Second Floor";
                case "Vbonus_037":
                    return "After the Heartless & Unversed battle in Monstropolis - The Power Plant: Accessway";
                case "Vbonus_038":
                    return "After the Heartless Mob battle in Monstropolis  - The Power Plant: Tank Yard";
                case "Vbonus_039":
                    return "After the Unversed battle in Monstropolis - The Power Plant: Tank Yard";
                case "Vbonus_040":
                    return "After the Lump of Horror boss battle in Monstropolis - The Door Vault: Service Area";

                case "Vbonus_041":
                    return "After the Rock Troll battle in Arendelle";
                case "Vbonus_042":
                    return "After the first Ninja Nobody battle in Arendelle - The Labyrinth of Ice";
                case "Vbonus_043":
                    return "After the second Ninja Nobody battle in Arendelle - The Labyrinth of Ice";
                case "Vbonus_044":
                    return "After the third Ninja Nobody battle in Arendelle - The Labyrinth of Ice";
                case "Vbonus_045":
                    return "After the fourth Ninja Nobody battle in Arendelle - The Labyrinth of Ice";
                case "Vbonus_047":
                    return "After the Marshmallow boss battle in Arendelle";
                case "Vbonus_048":
                    return "After the 3 Frost Serpent Heartless battle in Arendelle";
                case "Vbonus_049":
                    return "After the Heartless Mob battle in Valley of Ice in Arendelle";
                case "Vbonus_050":
                    return "After the Skoll boss battle in Arendelle";

                case "Vbonus_051":
                    return "After the Metal Troll battle on the bridge in San Fransokyo";
                case "Vbonus_052":
                    return "After the meeting Hiro in the garage in San Fransokyo";
                case "Vbonus_053":
                    return "After the Heartless Mob battle on the roof in San Fransokyo - The City (Day)";
                case "Vbonus_054":
                    return "After the Catastrochorus boss battle in San Fransokyo - The City (Day)";
                case "Vbonus_055":
                    return "After the rescue mission for the Big Hero 6 team in San Fransokyo - The City (Night)";
                case "Vbonus_056":
                    return "After the Darkube boss battle in San Fransokyo - The City (Night)";
                case "Vbonus_057":
                    return "After the Dark Baymax boss battle in San Fransokyo - The City (Day)";

                case "Vbonus_058":
                    return "After catching the Black Pearl in Davy Jones Locker in The Caribbean";
                case "Vbonus_059":
                    return "After the first ship battle in The Caribbean";
                case "Vbonus_060":
                    return "After the Raging Vulture boss battle in The Caribbean";
                case "Vbonus_061":
                    return "After the Lightning Angler boss battle in The Caribbean";
                case "Vbonus_062":
                    return "After the Luxord ship race to Port Royal in The Caribbean";
                case "Vbonus_063":
                    return "After the second ship battle in The Caribbean";
                case "Vbonus_064":
                    return "After the third ship battle in The Caribbean";
                case "Vbonus_065":
                    return "After the Kraken boss battle in The Caribbean";
                case "Vbonus_066":
                    return "After the Davy Jones boss battle in The Caribbean";

                case "Vbonus_067":
                    return "After the Anti-Aqua boss battle in The Dark World";
                case "Vbonus_068":
                    return "After the last Lich boss battle in San Fransokyo";

                case "Vbonus_069":
                    return "After the Heartless, Nobody & Unversed Mob battle in The Keyblade Graveyard";
                case "Vbonus_070":
                    return "After the Demon Tide boss battle in The Keyblade Graveyard";
                case "Vbonus_071":
                    return "After the Xigbar & Dark Riku boss battle in The Keyblade Graveyard";
                case "Vbonus_072":
                    return "After the Luxord, Larxene & Marluxia boss battle in The Keyblade Graveyard";
                case "Vbonus_073":
                    return "After the Terra-Xehanort & Vanitas boss battle in The Keyblade Graveyard";
                case "Vbonus_074":
                    return "After the Saix boss battle in The Keyblade Graveyard";
                case "Vbonus_075":
                    return "After the Young Xehanort, Ansem & Xemnas boss battle in The Keyblade Graveyard";
                case "Vbonus_076":
                    return "After the Heartless, Nobody & Unversed Mob battle in The Keyblade Graveyard";

                case "Vbonus_082":
                    return "After the Darkside boss battle in The Final World";
                case "Vbonus_083":
                    return "After collecting 222 Soras in The Final World";
                case "Vbonus_084":
                    return "After collecting 333 Soras in The Final World";

                case "VBonus_Minigame001":
                    return "After obtaining the first A-rank in Verum Rex: Beat of Lead minigame in Toy Box";
                case "VBonus_Minigame002":
                    return "After obtaining the first A-rank in the Festival Dance minigame in Kingdom of Corona";
                case "VBonus_Minigame003":
                    return "After obtaining the first A-rank in the Frozen Slider minigame in Arendelle";
                case "VBonus_Minigame004":
                    return "After obtaining all the treasures in the Frozen Slider minigame in Arendelle";
                case "VBonus_Minigame005":
                    return "After obtaining the first A-Rank in the Flash Tracer 1 (Fred) minigame in San Fransokyo";
                case "VBonus_Minigame006":
                    return "After obtaining the first A-Rank in the Flash Tracer 2 (Go Go) minigame in San Fransokyo";

                case "VBonus_Minigame007":
                    return "After obtaining the first A-Rank in the Cherry Flan minigame in Olympus";
                case "VBonus_Minigame008":
                    return "After obtaining the first A-Rank in the Strawberry Flan minigame in Toy Box";
                case "VBonus_Minigame009":
                    return "After obtaining the first A-Rank in the Orange Flan minigame in Kingdom of Corona";
                case "VBonus_Minigame010":
                    return "After obtaining the first A-Rank in the Banana Flan minigame in Monstropolis";
                case "VBonus_Minigame011":
                    return "After obtaining the first A-Rank in the Grape Flan minigame in Arendelle";
                case "VBonus_Minigame012":
                    return "After obtaining the first A-Rank in the Watermelon Flan minigame in The Caribbean";
                case "VBonus_Minigame013":
                    return "After obtaining the first A-Rank in the Melon Flan minigame in San Fransokyo";

                case "VBonus_DLC_001":
                    return "After the Dark Inferno χ boss battle in The Keyblade Graveyard (Re:Mind)";
                case "VBonus_DLC_002":
                    return "After the Anti-Aqua boss battle in The Keyblade Graveyard (Re:Mind)";
                case "VBonus_DLC_003":
                    return "After the Terra-Xehanort boss battle in The Keyblade Graveyard (Re:Mind)";
                case "VBonus_DLC_004":
                    return "After the Xigbar & Dark Riku boss battle in The Keyblade Graveyard (Re:Mind)";
                case "VBonus_DLC_005":
                    return "After the Luxord, Larxene & Marluxia boss battle in The Keyblade Graveyard (Re:Mind)";
                case "VBonus_DLC_006":
                    return "After the Terra-Xehanort & Vanitas boss battle in The Keyblade Graveyard (Re:Mind)";
                case "VBonus_DLC_007":
                    return "After the Saix boss battle in The Keyblade Graveyard (Re:Mind)";
                case "VBonus_DLC_008":
                    return "After the Young Xehanort, Ansem & Xemnas boss battle in The Keyblade Graveyard (Re:Mind)";

                case "VBonus_DLC_009":
                    return "After the Shadow Heartless battle in Scala Ad Caelum (Re:Mind)";
                case "VBonus_DLC_010":
                    return "After the Darkside boss battle in Scala Ad Caelum (Re:Mind)";
                
                case "VBonus_DLC_011":
                    return "After collecting all of Kairi's heart fragments in Re:Mind";
                case "VBonus_DLC_012":
                    return "After the Replica Xehanorts boss battle as the Guardians of Light in Re:Mind";
                case "VBonus_DLC_013":
                    return "After the Replica Xehanorts boss battle as King Mickey in Re:Mind";
                case "VBonus_DLC_014":
                    return "After connecting all the keyholes in Re:Mind";
                case "VBonus_DLC_015":
                    return "After the Armored Xehanort boss battle in Re:Mind";

                #endregion VBonus

                #region Synthesis Items

                case "IS_0":
                    return "Originally Ether";
                case "IS_1":
                    return "Originally Fire Bangle";
                case "IS_2":
                    return "Originally Fira Bangle";
                case "IS_3":
                    return "Originally Shadow Anklet";
                case "IS_4":
                    return "Originally Ability Ring+";
                case "IS_5":
                    return "Originally Elven Bandanna";
                case "IS_6":
                    return "Originally Thunder Trinket";
                case "IS_7":
                    return "Originally Thundara Trinket";
                case "IS_8":
                    return "Originally Refocuser";
                case "IS_9":
                    return "Originally Mythril Shard";
                case "IS_10":
                    return "Originally Wind Fan";
                case "IS_11":
                    return "Originally AP Boost";
                case "IS_12":
                    return "Originally Warhammer+";
                case "IS_13":
                    return "Originally Clockwork Shield+";
                case "IS_14":
                    return "Originally Technician's Ring+";
                case "IS_15":
                    return "Originally Ether";
                case "IS_16":
                    return "Originally Mega-Potion";
                case "IS_17":
                    return "Originally Dark Anklet";
                case "IS_18":
                    return "Originally Mythril Stone";
                case "IS_19":
                    return "Originally Aegis Shield+";
                case "IS_20":
                    return "Originally Blizzard Choker";
                case "IS_21":
                    return "Originally Blizzara Choker";
                case "IS_22":
                    return "Originally Skill Ring+";
                case "IS_23":
                    return "Originally Mega-Ether";
                case "IS_24":
                    return "Originally Strength Boost";
                case "IS_25":
                    return "Originally Magic Boost";
                case "IS_26":
                    return "Originally Defense Boost";
                case "IS_27":
                    return "Originally Firaga Bangle";
                case "IS_28":
                    return "Originally Blizzaga Choker";
                case "IS_29":
                    return "Originally Thundaga Trinket";
                case "IS_30":
                    return "Originally Divine Bandanna";
                case "IS_31":
                    return "Originally Storm Fan";
                case "IS_32":
                    return "Originally Midnight Anklet";
                case "IS_33":
                    return "Originally Astrolabe+";
                case "IS_34":
                    return "Originally Hi-Refocuser";
                case "IS_35":
                    return "Originally Mythril Gem";
                case "IS_36":
                    return "Originally Phantom Ring";
                case "IS_37":
                    return "Originally Sorcerer's Ring";
                case "IS_38":
                    return "Originally Firagun Bangle";
                case "IS_39":
                    return "Originally Blizzaza Choker";
                case "IS_40":
                    return "Originally Thundaza Trinket";
                case "IS_41":
                    return "Originally Chaos Anklet";
                case "IS_42":
                    return "Originally Acrisius";
                case "IS_43":
                    return "Originally Elixir";
                case "IS_44":
                    return "Originally Mythril Crystal";
                case "IS_45":
                    return "Originally Buster Band";
                case "IS_46":
                    return "Originally Orichalcum Ring";
                case "IS_47":
                    return "Originally Wisdom Ring";
                case "IS_48":
                    return "Originally Heartless Maul";
                case "IS_49":
                    return "Originally Nobody Guard";
                case "IS_50":
                    return "Originally Buster Band+";
                case "IS_51":
                    return "Originally Acrisius+";
                case "IS_52":
                    return "Originally Megalixir";
                case "IS_53":
                    return "Originally Heartless Maul+";
                case "IS_54":
                    return "Originally Nobody Guard+";
                case "IS_55":
                    return "Originally Save the Queen";
                case "IS_56":
                    return "Originally Save the King";
                case "IS_57":
                    return "Originally Cosmic Chain";
                case "IS_58":
                    return "Originally Ultima Weapon";
                case "IS_59":
                    return "Originally Save the Queen+";
                case "IS_60":
                    return "Originally Save the King+";
                
                case "IS_61":
                    return "Originally Recipe for Firefighter Rosette";
                case "IS_62":
                    return "Originally Recipe for Umbrella Rosette";
                case "IS_63":
                    return "Originally Recipe for Soldier's Earring";
                case "IS_64":
                    return "Originally Recipe for Mage's Earring";
                case "IS_65":
                    return "Originally Recipe for Mask Rosette";
                case "IS_66":
                    return "Originally Recipe for Insulator Rosette";
                case "IS_67":
                    return "Originally Recipe for Cosmic Ring";
                case "IS_68":
                    return "Originally Recipe for Moon Amulet";
                case "IS_69":
                    return "Originally Recipe for Fire Chain";
                case "IS_70":
                    return "Originally Recipe for Blizzard Chain";
                case "IS_71":
                    return "Originally Recipe for Thunder Chain";
                case "IS_72":
                    return "Originally Recipe for Snowman Rosette";
                case "IS_73":
                    return "Originally Recipe for Star Charm";
                case "IS_74":
                    return "Originally Recipe for Draw Ring";
                case "IS_75":
                    return "Originally Recipe for Aqua Chaplet";
                case "IS_76":
                    return "Originally Recipe for Aero Armlet";
                case "IS_77":
                    return "Originally Recipe for Fencer's Earring";
                case "IS_78":
                    return "Originally Recipe for Slayer's Earring";
                case "IS_79":
                    return "Originally Recipe for Dark Chain";
                case "IS_80":
                    return "Originally Recipe for Petite Ribbon";
                
                case "IS_81":
                    return "Originally Lucid Crystal";
                case "IS_82":
                    return "Originally Soothing Crystal";
                case "IS_83":
                    return "Originally Writhing Crystal";
                case "IS_84":
                    return "Originally Pulsing Crystal";
                case "IS_85":
                    return "Originally Blazing Crystal";
                case "IS_86":
                    return "Originally Frost Crystal";
                case "IS_87":
                    return "Originally Lightning Crystal";

                #endregion Synthesis Items

                default:
                    return input;
            }
        }

        public static string ValueIdToDisplay(this string input)
        {
            switch (input.Replace("\u0000", ""))
            {
                #region Battle Items
                case "ITEM_POTION":
                    return "Potion";
                case "ITEM_HIGHPOTION":
                    return "Hi-Potion";
                case "ITEM_MEGAPOTION":
                    return "Mega-Potion";
                case "ITEM_ETHER":
                    return "Ether";
                case "ITEM_HIGHETHER":
                    return "Hi-Ether";
                case "ITEM_MEGAETHER":
                    return "Mega-Ether";
                case "ITEM_ELIXIR":
                    return "Elixir";
                case "ITEM_LASTELIXIR":
                    return "Megalixir";
                case "ITEM_FOCUSSUPPLY":
                    return "Refocuser";
                case "ITEM_HIGHFOCUSSUPPLY":
                    return "Hi-Refocuser";
                case "ITEM_ALLCURE":
                    return "Panacea";
                #endregion Battle Items

                #region Camp Items
                case "ITEM_TENT":
                    return "Tent";
                case "ITEM_POWERUP":
                    return "Strength Boost";
                case "ITEM_MAGICUP":
                    return "Magic Boost";
                case "ITEM_GUARDUP":
                    return "Defense Boost";
                case "ITEM_APUP":
                    return "AP Boost";
                #endregion Camp Items

                #region Keyblades
                case "WEP_KEYBLADE_SO_00":
                case "ETresItemDefWeapon::WEP_KEYBLADE00":
                    return "Kingdom Key";
                case "WEP_KEYBLADE_SO_01":
                case "ETresItemDefWeapon::WEP_KEYBLADE02":
                    return "Hero's Origin";
                case "WEP_KEYBLADE_SO_02":
                case "ETresItemDefWeapon::WEP_KEYBLADE01":
                    return "Shooting Star";
                case "WEP_KEYBLADE_SO_03":
                case "ETresItemDefWeapon::WEP_KEYBLADE03":
                    return "Favorite Deputy";
                case "WEP_KEYBLADE_SO_04":
                case "ETresItemDefWeapon::WEP_KEYBLADE04":
                    return "Ever After";
                case "WEP_KEYBLADE_SO_05":
                case "ETresItemDefWeapon::WEP_KEYBLADE09":
                    return "Happy Gear";
                case "WEP_KEYBLADE_SO_06":
                case "ETresItemDefWeapon::WEP_KEYBLADE06":
                    return "Crystal Snow";
                case "WEP_KEYBLADE_SO_07":
                case "ETresItemDefWeapon::WEP_KEYBLADE07":
                    return "Hunny Spout";
                case "WEP_KEYBLADE_SO_08":
                case "ETresItemDefWeapon::WEP_KEYBLADE08":
                    return "Nano Gear";
                case "WEP_KEYBLADE_SO_09":
                case "ETresItemDefWeapon::WEP_KEYBLADE05":
                    return "Wheel of Fate";
                case "WEP_KEYBLADE_SO_011":
                case "ETresItemDefWeapon::WEP_KEYBLADE11":
                    return "Grand Chef";
                case "WEP_KEYBLADE_SO_012":
                case "ETresItemDefWeapon::WEP_KEYBLADE10":
                    return "Classic Tone";
                case "WEP_KEYBLADE_SO_013":
                case "ETresItemDefWeapon::WEP_KEYBLADE12":
                    return "Oathkeeper";
                case "WEP_KEYBLADE_SO_014":
                case "ETresItemDefWeapon::WEP_KEYBLADE13":
                    return "Oblivion";
                case "WEP_KEYBLADE_SO_015":
                case "ETresItemDefWeapon::WEP_KEYBLADE14":
                    return "Ultima Weapon";
                case "WEP_KEYBLADE_SO_018":
                case "ETresItemDefWeapon::WEP_KEYBLADE17":
                    return "Starlight";
                case "WEP_KEYBLADE_SO_019":
                case "ETresItemDefWeapon::WEP_KEYBLADE18":
                    return "Elemental Encoder";
                #endregion Keyblades

                #region Weapons
                case "WEP_DONALD_01":
                    return "Mage's Staff+";
                case "WEP_DONALD_03":
                    return "Warhammer+";
                case "WEP_DONALD_05":
                    return "Magician's Wand+";
                case "WEP_DONALD_07":
                    return "Nirvana+";
                case "WEP_DONALD_09":
                    return "Astrolabe+";
                case "WEP_DONALD_011":
                    return "Heartless Maul";
                case "WEP_DONALD_012":
                    return "Heartless Maul+";
                case "WEP_DONALD_013":
                    return "Save the Queen";
                case "WEP_DONALD_014":
                    return "Save the Queen+";


                case "WEP_GOOFY_01":
                    return "Knight's Shield+";
                case "WEP_GOOFY_03":
                    return "Clockwork Shield+";
                case "WEP_GOOFY_05":
                    return "Star Shield+";
                case "WEP_GOOFY_07":
                    return "Aegis Shield+";
                case "WEP_GOOFY_09":
                    return "Storm Anchor+";
                case "WEP_GOOFY_011":
                    return "Nobody Guard";
                case "WEP_GOOFY_012":
                    return "Nobody Guard+";
                case "WEP_GOOFY_013":
                    return "Save the King";
                case "WEP_GOOFY_014":
                    return "Save the King+";
                #endregion Weapons

                #region Armor
                case "PRT_ITEM01":
                    return "Hero's Belt";
                case "PRT_ITEM02":
                    return "Hero's Glove";
                
                case "PRT_ITEM03":
                    return "Shield Belt";
                case "PRT_ITEM04":
                    return "Defense Belt";
                case "PRT_ITEM05":
                    return "Guardian Belt";

                case "PRT_ITEM07":
                    return "Buster Band";
                case "PRT_ITEM08":
                    return "Buster Band+";
                case "PRT_ITEM09":
                    return "Cosmic Belt";
                case "PRT_ITEM10":
                    return "Cosmic Belt+";

                case "PRT_ITEM11":
                    return "Fire Bangle";
                case "PRT_ITEM12":
                    return "Firaga Bangle";
                case "PRT_ITEM13":
                    return "Firaza Bangle";

                case "PRT_ITEM14":
                    return "Fire Chain";

                case "PRT_ITEM15":
                    return "Blizzard Choker";
                case "PRT_ITEM16":
                    return "Blizzara Choker";
                case "PRT_ITEM17":
                    return "Blizzaga Choker";
                
                case "PRT_ITEM18":
                    return "Blizzard Chain";

                case "PRT_ITEM19":
                    return "Thunder Trinket";
                case "PRT_ITEM20":
                    return "Thundaga Trinket";
                case "PRT_ITEM21":
                    return "Thundaza Trinket";

                case "PRT_ITEM22":
                    return "Thunder Chain";

                case "PRT_ITEM23":
                    return "Dark Anklet";
                case "PRT_ITEM24":
                    return "Midnight Anklet";
                case "PRT_ITEM25":
                    return "Chaos Anklet";

                case "PRT_ITEM26":
                    return "Dark Chain";

                case "PRT_ITEM27":
                    return "Divine Bandanna";
                case "PRT_ITEM28":
                    return "Elven Bandanna";

                case "PRT_ITEM29":
                    return "Aqua Chaplet";

                case "PRT_ITEM30":
                    return "Wind Fan";
                case "PRT_ITEM31":
                    return "Storm Fan";

                case "PRT_ITEM32":
                    return "Aero Armlet";

                case "PRT_ITEM33":
                    return "Aegis Chain";
                case "PRT_ITEM34":
                    return "Acrisius";
                case "PRT_ITEM35":
                    return "Cosmic Chain";
                case "PRT_ITEM36":
                    return "Petite Ribbon";
                case "PRT_ITEM37":
                    return "Ribbon";

                case "PRT_ITEM38":
                    return "Fira Bangle";
                case "PRT_ITEM39":
                    return "Blizzaza Choker";
                case "PRT_ITEM40":
                    return "Thundara Trinket";
                case "PRT_ITEM41":
                    return "Shadow Anklet";

                case "PRT_ITEM42":
                    return "Abas Chain";
                case "PRT_ITEM43":
                    return "Acrisius+";
                case "PRT_ITEM44":
                    return "Royal Ribbon";
                case "PRT_ITEM45":
                    return "Firefighter Rosette";
                case "PRT_ITEM46":
                    return "Umbrella Rosette";
                case "PRT_ITEM47":
                    return "Mask Rosette";
                case "PRT_ITEM48":
                    return "Snowman Rosette";
                case "PRT_ITEM49":
                    return "Insulator Rosette";
                case "PRT_ITEM50":
                    return "Power Weight";
                case "PRT_ITEM51":
                    return "Magic Weight";
                case "PRT_ITEM52":
                    return "Master Belt";
                #endregion Armor

                #region Accessories
                case "ACC_ITEM01":
                    return "Laughter Pin";
                case "ACC_ITEM02":
                    return "Forest Clasp";

                case "ACC_ITEM03":
                    return "Ability Ring";
                case "ACC_ITEM04":
                    return "Ability Ring+";

                case "ACC_ITEM06":
                    return "Technician's Ring+";

                case "ACC_ITEM08":
                    return "Skill Ring+";
                case "ACC_ITEM09":
                    return "Expert's Ring";
                case "ACC_ITEM10":
                    return "Master's Ring";
                case "ACC_ITEM11":
                    return "Cosmic Ring";
                case "ACC_ITEM12":
                    return "Power Ring";
                case "ACC_ITEM13":
                    return "Buster Ring";
                case "ACC_ITEM14":
                    return "Valor Ring";
                case "ACC_ITEM15":
                    return "Phantom Ring";
                case "ACC_ITEM16":
                    return "Orichalcum Ring";

                case "ACC_ITEM17":
                    return "Magic Ring";
                case "ACC_ITEM18":
                    return "Rune Ring";
                case "ACC_ITEM19":
                    return "Force Ring";
                case "ACC_ITEM20":
                    return "Sorcerer's Ring";
                case "ACC_ITEM21":
                    return "Wisdom Ring";

                case "ACC_ITEM22":
                    return "Bronze Necklace";
                case "ACC_ITEM23":
                    return "Silver Necklace";
                case "ACC_ITEM24":
                    return "Master's Necklace";

                case "ACC_ITEM25":
                    return "Bronze Amulet";
                case "ACC_ITEM26":
                    return "Silver Amulet";
                case "ACC_ITEM27":
                    return "Gold Amulet";

                case "ACC_ITEM28":
                    return "Junior Medal";
                case "ACC_ITEM29":
                    return "Star Medal";
                case "ACC_ITEM30":
                    return "Master Medal";

                case "ACC_ITEM31":
                    return "Mickey's Brooch";

                case "ACC_ITEM32":
                    return "Soldier's Earring";
                case "ACC_ITEM33":
                    return "Fencer's Earring";
                case "ACC_ITEM34":
                    return "Mage's Earring";
                case "ACC_ITEM35":
                    return "Slayer's Earring";

                case "ACC_ITEM36":
                    return "Moon Amulet";
                case "ACC_ITEM37":
                    return "Star Charm";
                case "ACC_ITEM38":
                    return "Cosmic Arts";

                case "ACC_ITEM39":
                    return "Crystal Regalia";

                case "ACC_ITEM40":
                    return "Water Cufflink";
                case "ACC_ITEM41":
                    return "Thunder Cufflink";
                case "ACC_ITEM42":
                    return "Fire Cufflink";
                case "ACC_ITEM43":
                    return "Aero Cufflink";
                case "ACC_ITEM44":
                    return "Blizzard Cufflink";
                case "ACC_ITEM45":
                    return "Celestriad";
                case "ACC_ITEM46":
                    return "Yin-Yang Cufflink";

                case "ACC_ITEM47":
                    return "Gourmand's Ring";
                case "ACC_ITEM48":
                    return "Draw Ring";
                case "ACC_ITEM49":
                    return "Lucky Ring";

                case "ACC_ITEM50":
                    return "Flanniversary Badge";

                case "ACC_ITEM51":
                case "ACC_ITEM52":
                case "ACC_ITEM53":
                case "ACC_ITEM54":
                case "ACC_ITEM55":
                case "ACC_ITEM56":
                case "ACC_ITEM57":
                case "ACC_ITEM58":
                case "ACC_ITEM59":
                case "ACC_ITEM60":
                case "ACC_ITEM61":
                case "ACC_ITEM62":
                    return "Bronze Necklace";

                case "ACC_ITEM63":
                case "ACC_ITEM64":
                case "ACC_ITEM65":
                case "ACC_ITEM66":
                case "ACC_ITEM67":
                case "ACC_ITEM68":
                case "ACC_ITEM69":
                case "ACC_ITEM70":
                case "ACC_ITEM71":
                case "ACC_ITEM72":
                case "ACC_ITEM73":
                    return "Silver Necklace";

                case "ACC_ITEM74":
                case "ACC_ITEM75":
                case "ACC_ITEM76":
                case "ACC_ITEM77":
                case "ACC_ITEM78":
                case "ACC_ITEM79":
                case "ACC_ITEM80":
                    return "Master's Necklace";

                case "ACC_ITEM81":
                case "ACC_ITEM82":
                    return "Star Medal";

                case "ACC_ITEM83":
                    return "Junior Medal";

                case "ACC_ITEM84":
                    return "Star Medal";

                case "ACC_ITEM85":
                case "ACC_ITEM86":
                case "ACC_ITEM87":
                case "ACC_ITEM88":
                case "ACC_ITEM89":
                case "ACC_ITEM90":
                case "ACC_ITEM91":
                case "ACC_ITEM92":
                    return "Junior Medal";

                case "ACC_ITEM93":
                    return "Master Medal";

                case "ACC_ITEM94":
                case "ACC_ITEM95":
                    return "Star Medal";

                case "ACC_ITEM96":
                    return "Master Medal";

                case "ACC_ITEM97":
                    return "Star Medal";

                case "ACC_ITEM98":
                    return "Master Medal";

                case "ACC_ITEM99":
                case "ACC_ITEM100":
                case "ACC_ITEM101":
                case "ACC_ITEM102":
                case "ACC_ITEM103":
                    return "Star Medal";

                case "ACC_ITEM104":
                case "ACC_ITEM105":
                case "ACC_ITEM106":
                case "ACC_ITEM107":
                case "ACC_ITEM108":
                case "ACC_ITEM109":
                case "ACC_ITEM110":
                    return "Master Medal";

                case "ACC_ITEM111":
                    return "Breakthrough";
                case "ACC_ITEM112":
                    return "Crystal Regalia+";
                #endregion Accessories

                #region Food Items
                case "FOOD_ITEM41":
                    return "Sea Bass en Papillote+";
                case "FOOD_ITEM56":
                    return "Tarte Aux Fruits+";
                #endregion Food Items

                #region Materials
                case "MAT_ITEM04":
                    return "Blazing Crystal";

                case "MAT_ITEM08":
                    return "Frost Crystal";

                case "MAT_ITEM12":
                    return "Lightning Crystal";

                case "MAT_ITEM16":
                    return "Lucid Crystal";

                case "MAT_ITEM20":
                    return "Pulsing Crystal";

                case "MAT_ITEM24":
                    return "Writhing Crystal";

                case "MAT_ITEM33":
                    return "Mythril Shard";
                case "MAT_ITEM34":
                    return "Mythril Stone";
                case "MAT_ITEM35":
                    return "Mythril Gem";
                case "MAT_ITEM36":
                    return "Mythril Crystal";

                case "MAT_ITEM44":
                    return "Soothing Crystal";

                case "MAT_ITEM47":
                    return "Wellspring Gem";
                case "MAT_ITEM48":
                    return "Wellspring Crystal";

                case "MAT_ITEM52":
                    return "Hungry Crystal";

                case "MAT_ITEM53":
                    return "Fluorite";
                case "MAT_ITEM54":
                    return "Damascus";
                case "MAT_ITEM55":
                    return "Adamantite";
                case "MAT_ITEM56":
                    return "Orichalcum";
                case "MAT_ITEM57":
                    return "Orichalcum+";
                case "MAT_ITEM58":
                    return "Electrum";
                case "MAT_ITEM59":
                    return "Evanescent Crystal";
                case "MAT_ITEM60":
                    return "Illusory Crystal";
                #endregion Materials

                #region Key Items
                case "KEY_ITEM02":
                    return "Gummiphone";
                case "KEY_ITEM03":
                    return "AR Device";
                case "KEY_ITEM04":
                    return "Prize Postcard";
                case "KEY_ITEM05":
                    return "M.O.G. Card";
                case "KEY_ITEM06":
                    return "Dream Heartbinder";
                case "KEY_ITEM07":
                    return "Pixel Heartbinder";
                case "KEY_ITEM08":
                    return "\'Ohana Heartbinder";
                case "KEY_ITEM09":
                    return "Pride Heartbinder";
                case "KEY_ITEM10":
                    return "Ocean Heartbinder";
                case "KEY_ITEM11":
                    return "Golden Herc Figure";
                case "KEY_ITEM15":
                    return "Proof of Promises";
                case "KEY_ITEM16":
                    return "Proof of Times Past";
                case "KEY_ITEM14":
                    return "Proof of Fantasy";
                #endregion Key Items

                #region Classic Kingdom
                case "LSIGAME01":
                    return "CK: Giantland";
                case "LSIGAME02":
                    return "CK: Mickey, The Mail Pilot";
                case "LSIGAME03":
                    return "CK: The Musical Farmer";
                case "LSIGAME04":
                    return "CK: Building a Building";
                case "LSIGAME05":
                    return "CK: The Mad Doctor";
                case "LSIGAME06":
                    return "CK: Mickey's Kitten Catch";
                case "LSIGAME07":
                    return "CK: The Klondike Kid";
                case "LSIGAME08":
                    return "CK: Fishin\' Frenzy";
                case "LSIGAME09":
                    return "CK: The Karnival Kid";
                case "LSIGAME10":
                    return "CK: Mickey Cuts Up";
                case "LSIGAME11":
                    return "CK: Mickey's Prison Escape";
                case "LSIGAME12":
                    return "CK: How to Play Baseball";
                case "LSIGAME13":
                    return "CK: How to Play Golf";
                case "LSIGAME14":
                    return "CK: Mickey's Circus";
                case "LSIGAME15":
                    return "CK: Camping Out";
                case "LSIGAME16":
                    return "CK: Taxi Troubles";
                case "LSIGAME17":
                    return "CK: Beach Party";
                case "LSIGAME18":
                    return "CK: The Wayward Canary";
                case "LSIGAME19":
                    return "CK: Mickey's Mechanical Man";
                case "LSIGAME20":
                    return "CK: The Barnyard Battle";
                case "LSIGAME21":
                    return "CK: Cast Out to Sea";
                case "LSIGAME22":
                    return "CK: Backyard Sports";
                case "LSIGAME23":
                    return "CK: Mickey Steps Out";
                #endregion Classic Kingdom

                #region Maps
                case "NAVI_MAP_HE01":
                    return "Map: Realm of the Gods";
                case "NAVI_MAP_HE02":
                    return "Map: Mount Olympus";
                case "NAVI_MAP_HE03":
                    return "Map: Thebes";

                case "NAVI_MAP_TT01":
                    return "Map: The Neighborhood";

                case "NAVI_MAP_RA01":
                    return "Map: The Forest";
                case "NAVI_MAP_RA02":
                    return "Map: The Marsh";

                case "NAVI_MAP_TS01":
                    return "Map: Andy's House";
                case "NAVI_MAP_TS02":
                    return "Map: Galaxy Toys";

                case "NAVI_MAP_MI01":
                    return "Map: Monsters, Inc.";
                case "NAVI_MAP_MI02":
                    return "Map: The Factory";

                case "NAVI_MAP_FZ01":
                    return "Map: The North Mountain";
                case "NAVI_MAP_FZ02":
                    return "Map: The Labyrinth of Ice";

                case "NAVI_MAP_CA01":
                    return "Map: Port Royal Waters";
                case "NAVI_MAP_CA02":
                    return "Map: Isla de los Mastiles";
                case "NAVI_MAP_CA03":
                    return "Map: Ship's End";
                case "NAVI_MAP_CA04":
                    return "Map: Huddled Isles";
                case "NAVI_MAP_CA05":
                    return "Map: Sandbar Isle";

                case "NAVI_MAP_BX01":
                    return "Map: The City";

                case "NAVI_MAP_KG01":
                    return "Map: The Badlands";
                case "NAVI_MAP_KG02":
                    return "Map: The Skein of Severance";

                case "NAVI_MAP_BT01":
                    return "Map: The Stairway to the Sky";
                case "NAVI_MAP_BT02":
                    return "Map: Breezy Quarter";
                #endregion Maps

                #region Secret Reports
                case "REPORT_ITEM01":
                    return "Secret Report NA";
                case "REPORT_ITEM02":
                    return "Secret Report 1";
                case "REPORT_ITEM03":
                    return "Secret Report 2";
                case "REPORT_ITEM04":
                    return "Secret Report 3";
                case "REPORT_ITEM05":
                    return "Secret Report 4";
                case "REPORT_ITEM06":
                    return "Secret Report 5";
                case "REPORT_ITEM07":
                    return "Secret Report 6";
                case "REPORT_ITEM08":
                    return "Secret Report 7";
                case "REPORT_ITEM09":
                    return "Secret Report 8";
                case "REPORT_ITEM10":
                    return "Secret Report 9";
                case "REPORT_ITEM11":
                    return "Secret Report 10";
                case "REPORT_ITEM12":
                    return "Secret Report 11";
                case "REPORT_ITEM13":
                    return "Secret Report 12";
                case "REPORT_ITEM14":
                    return "Secret Report 13";
                #endregion Secret Reports

                #region Abilities
                case "ETresAbilityKind::AIR_RECOVERY":
                    return "Ability: Aerial Recovery";
                case "ETresAbilityKind::BLOW_COUNTER":
                    return "Ability: Payback Strike";
                case "ETresAbilityKind::REFLECT_GUARD":
                    return "Ability: Block";
                case "ETresAbilityKind::GUARD_COUNTER":
                    return "Ability: Counter Slash";
                case "ETresAbilityKind::REVENGEIMPACT":
                    return "Ability: Counter Impact";
                case "ETresAbilityKind::REVENGEDIVE":
                    return "Ability: Counter Kick";
                case "ETresAbilityKind::REVENGE_EX":
                    return "Ability: Final Blow";
                case "ETresAbilityKind::RISKDODGE":
                    return "Ability: Risk Dodge";
                case "ETresAbilityKind::SLASH_UPPER":
                    return "Ability: Rising Spiral";
                case "ETresAbilityKind::AIR_ROLL_BEAT":
                    return "Ability: Groundbreaker";
                case "ETresAbilityKind::AIR_DOWN":
                    return "Ability: Falling Slash";
                case "ETresAbilityKind::TRIPPLE_SLASH":
                    return "Ability: Speed Slash";
                case "ETresAbilityKind::CHARGE_THRUST":
                    return "Ability: Triple Rush";
                case "ETresAbilityKind::MAGICFLUSH":
                    return "Ability: Magic Flash";
                case "ETresAbilityKind::HIGHJUMP":
                    return "Ability: High Jump";
                case "ETresAbilityKind::DOUBLEFLIGHT":
                    return "Ability: Doubleflight";
                case "ETresAbilityKind::SUPERJUMP":
                    return "Ability: Superjump";
                case "ETresAbilityKind::SUPERSLIDE":
                    return "Ability: Superslide";
                case "ETresAbilityKind::GLIDE":
                    return "Ability: Glide";
                case "ETresAbilityKind::LIBRA":
                    return "Ability: Scan";
                case "ETresAbilityKind::DODGE":
                    return "Ability: Dodge Roll";
                case "ETresAbilityKind::AIRSLIDE":
                    return "Ability: Air Slide";
                case "ETresAbilityKind::AIRDODGE":
                    return "Ability: Aerial Dodge";
                case "ETresAbilityKind::MP_SAFETY":
                    return "Ability: MP Safety";
                case "ETresAbilityKind::EXPZERO":
                    return "Ability: Zero EXP";
                case "ETresAbilityKind::FRIEND_AID":
                    return "Ability: Assist Friends";
                case "ETresAbilityKind::COMBO_PLUS":
                    return "Ability: Combo Plus";
                case "ETresAbilityKind::AIRCOMBO_PLUS":
                    return "Ability: Air Combo Plus";
                case "ETresAbilityKind::COMBO_MASTER":
                    return "Ability: Combo Master";
                case "ETresAbilityKind::COMBO_UP":
                    return "Ability: Combo Boost";
                case "ETresAbilityKind::AIRCOMBO_UP":
                    return "Ability: Air Combo Boost";
                case "ETresAbilityKind::FIRE_UP":
                    return "Ability: Fire Boost";
                case "ETresAbilityKind::BLIZZARD_UP":
                    return "Ability: Blizzard Boost";
                case "ETresAbilityKind::THUNDER_UP":
                    return "Ability: Thunder Boost";
                case "ETresAbilityKind::WATER_UP":
                    return "Ability: Water Boost";
                case "ETresAbilityKind::AERO_UP":
                    return "Ability: Aero Boost";
                case "ETresAbilityKind::WIZZARD_STAR":
                    return "Ability: Wizard's Ruse";
                case "ETresAbilityKind::LUCK_UP":
                    return "Ability: Lucky Strike";
                case "ETresAbilityKind::ITEM_UP":
                    return "Ability: Item Boost";
                case "ETresAbilityKind::PRIZE_DRAW":
                    return "Ability: Treasure Magnet";
                case "ETresAbilityKind::LEAF_VEIL":
                    return "Ability: Leaf Bracer";
                case "ETresAbilityKind::LAST_LEAVE":
                    return "Ability: Second Chance";
                case "ETresAbilityKind::COMBO_LEAVE":
                    return "Ability: Withstand Combo";
                case "ETresAbilityKind::FOCUS_ASPIR":
                    return "Ability: Focus Syphon";
                case "ETresAbilityKind::ATTRACTION_TIME":
                    return "Ability: Attraction Extender";
                case "ETresAbilityKind::LINK_BOOST":
                    return "Ability: Link Extender";
                case "ETresAbilityKind::FORM_TIME":
                    return "Ability: Formchange Extender";
                case "ETresAbilityKind::DEFENDER":
                    return "Ability: Defender";
                case "ETresAbilityKind::CRITICAL_HALF":
                    return "Ability: Damage Control";
                case "ETresAbilityKind::DAMAGE_ASPIR":
                    return "Ability: Damage Syphon";
                case "ETresAbilityKind::MP_HASTE":
                    return "Ability: MP Haste";
                case "ETresAbilityKind::MP_HASTERA":
                    return "Ability: MP Hastera";
                case "ETresAbilityKind::MP_HASTEGA":
                    return "Ability: MP Hastega";
                case "ETresAbilityKind::MAGIC_COMBO_SAVE":
                    return "Ability: Magic Combo Thrift";
                case "ETresAbilityKind::MAGIC_COMBO_UP":
                    return "Ability: Magic Galvanizer";
                case "ETresAbilityKind::WALK_REGENE":
                    return "Ability: MP Walker";
                case "ETresAbilityKind::WALK_HEALING":
                    return "Ability: HP Walker";
                case "ETresAbilityKind::MAGIC_DRAW":
                    return "Ability: Magic Treasure Magnet";
                case "ETresAbilityKind::MASTER_DRAW":
                    return "Ability: Master Treasure Magnet";
                case "ETresAbilityKind::ATTRACTION_UP":
                    return "Ability: Attraction Enhancer";
                case "ETresAbilityKind::BURN_GUARD":
                    return "Ability: Burn Protection";
                case "ETresAbilityKind::CLOUD_GUARD":
                    return "Ability: Cloud Protection";
                case "ETresAbilityKind::SNEEZE_GUARD":
                    return "Ability: Sneeze Protection";
                case "ETresAbilityKind::FREEZE_GUARD":
                    return "Ability: Freeze Protection";
                case "ETresAbilityKind::DISCHARGE_GUARD":
                    return "Ability: Electric Protection";
                case "ETresAbilityKind::STUN_GUARD":
                    return "Ability: Stun Protection";
                case "ETresAbilityKind::COUNTER_UP":
                    return "Ability: Reprisal Boost";
                case "ETresAbilityKind::AUTO_FINISH":
                    return "Ability: Auto-Finish";
                case "ETresAbilityKind::FORM_UP":
                    return "Ability: Situation Boost";
                case "ETresAbilityKind::MAGIC_TIME":
                    return "Ability: Grand Magic Extender";
                case "ETresAbilityKind::AUTO_LOCK_MAGIC":
                    return "Ability: Magic Lock-On";
                case "ETresAbilityKind::GUARD_REGENE":
                    return "Ability: Block Replenisher";
                case "ETresAbilityKind::MP_SAVE":
                    return "Ability: MP Thrift";
                case "ETresAbilityKind::MP_LEAVE":
                    return "Ability: Extra Cast";
                case "ETresAbilityKind::FULLMP_BURST":
                    return "Ability: Full MP Blast";
                case "ETresAbilityKind::HARVEST":
                    return "Ability: Harvester";
                case "ETresAbilityKind::HP_CONVERTER":
                    return "Ability: HP Converter";
                case "ETresAbilityKind::MP_CONVERTER":
                    return "Ability: MP Converter";
                case "ETresAbilityKind::MUNNY_CONVERTER":
                    return "Ability: Munny Converter";
                case "ETresAbilityKind::ENDLESS_MAGIC":
                    return "Ability: Endless Magic";
                case "ETresAbilityKind::FP_CONVERTER":
                    return "Ability: Focus Converter";
                case "ETresAbilityKind::FIRE_ASPIR":
                    return "Ability: Fire Syphon";
                case "ETresAbilityKind::BLIZZARD_ASPIR":
                    return "Ability: Blizzard Syphon";
                case "ETresAbilityKind::THUNDER_ASPIR":
                    return "Ability: Thunder Syphon";
                case "ETresAbilityKind::WATER_ASPIR":
                    return "Ability: Water Syphon";
                case "ETresAbilityKind::AERO_ASPIR":
                    return "Ability: Aero Syphon";
                case "ETresAbilityKind::DARK_ASPIR":
                    return "Ability: Dark Syphon";
                case "ETresAbilityKind::SONIC_SLASH":
                    return "Ability: Sonic Slash";
                case "ETresAbilityKind::SONIC_DOWN":
                    return "Ability: Sonic Cleave";
                case "ETresAbilityKind::TURN_CUTTER":
                    return "Ability: Buzz Saw";
                case "ETresAbilityKind::SUMMERSALT":
                    return "Ability: Somersault";
                case "ETresAbilityKind::POLE_SPIN":
                    return "Ability: Pole Spin";
                case "ETresAbilityKind::POLE_SWING":
                    return "Ability: Pole Swing";
                case "ETresAbilityKind::WALL_KICK":
                    return "Ability: Wall Kick";
                case "ETresAbilityKind::BATTLE_GRAPHER":
                    return "Ability: Frontline Photographer";
                case "ETresAbilityKind::CHARISMA_CHEF":
                    return "Ability: Chef Extraordinaire";
                case "ETresAbilityKind::POWER_CURE":
                    return "Ability: Cure Converter";
                case "ETresAbilityKind::CRITICAL_COUNTER":
                    return "Ability: Critical Counter";
                case "ETresAbilityKind::CRITICAL_CHARGE":
                    return "Ability: Critical Recharge";
                case "ETresAbilityKind::CRITICAL_CONVERTER":
                    return "Ability: Critical Converter";
                case "ETresAbilityKind::QUICK_SHAFT":
                    return "Ability: Quick Slash";
                case "ETresAbilityKind::FLASH_STEP":
                    return "Ability: Flash Step";
                case "ETresAbilityKind::RADIAL_CUT":
                    return "Ability: Radial Blaster";
                case "ETresAbilityKind::FINAL_HEAVEN":
                    return "Ability: Last Charge";
                case "ETresAbilityKind::AERIAL_SWEEP":
                    return "Ability: Aerial Sweep";
                case "ETresAbilityKind::AERIAL_DIVE":
                    return "Ability: Aerial Dive";

                case "ETresAbilityKind::LUNCH_TIME":
                    return "Ability: Cuisine Extender";
                case "ETresAbilityKind::POWER_LUNCH":
                    return "Ability: Hearty Meal";
                case "ETresAbilityKind::OVER_TIME":
                    return "Ability: Overtime";
                case "ETresAbilityKind::BEST_CONDITION":
                    return "Ability: Top Condition";
                case "ETresAbilityKind::EXP_BARGAIN":
                    return "Ability: EXP Incentive";
                case "ETresAbilityKind::PRIZE_FEEVER":
                    return "Ability: Prize Proliferator";
                case "ETresAbilityKind::MILLIONAIRE":
                    return "Ability: Rags to Riches";
                case "ETresAbilityKind::CURAGAN":
                    return "Ability: Curaza";
                case "ETresAbilityKind::CHARGE_BERSERK":
                    return "Ability: Berserk Charge";
                case "ETresAbilityKind::OVERCOME":
                    return "Ability: Hidden Potential";
                case "ETresAbilityKind::GRAND_MAGIC":
                    return "Ability: More Grand Magic";
                case "ETresAbilityKind::FIRAGAN":
                    return "Ability: Firaza";
                case "ETresAbilityKind::BLIZZAGAN":
                    return "Ability: Blizzaza";
                case "ETresAbilityKind::THUNDAGAN":
                    return "Ability: Thundaza";
                case "ETresAbilityKind::WATAGAN":
                    return "Ability: Waterza";
                case "ETresAbilityKind::AEROGAN":
                    return "Ability: Aeroza";
                case "ETresAbilityKind::MAGIC_ROULETTE":
                    return "Ability: Magic Roulette";

                case "ETresAbilityKind::UNISON_FIRE":
                    return "Ability: Unison Fire";
                case "ETresAbilityKind::UNISON_BLIZZARD":
                    return "Ability: Unison Blizzard";
                case "ETresAbilityKind::UNISON_THUNDER":
                    return "Ability: Unison Thunder";
                case "ETresAbilityKind::FUSION_SPIN":
                    return "Ability: Fusion Spin";
                case "ETresAbilityKind::FUSION_ROCKET":
                    return "Ability: Fusion Rocket";
                #endregion Abilities

                #region Bonuses
                case "ETresVictoryBonusKind::HP_UP5":
                    return "Bonus: HP +5";
                case "ETresVictoryBonusKind::HP_UP10":
                    return "Bonus: HP +10";
                case "ETresVictoryBonusKind::MP_UP3":
                    return "Bonus: MP +3";
                case "ETresVictoryBonusKind::MP_UP5":
                    return "Bonus: MP +5";
                case "ETresVictoryBonusKind::ITEM_SLOT_UP1":
                    return "Bonus: Item Slot +1";
                case "ETresVictoryBonusKind::ACC_SLOT_UP1":
                    return "Bonus: Accessory Slot +1";
                case "ETresVictoryBonusKind::DEF_SLOT_UP1":
                    return "Bonus: Armor Slot +1";

                case "ETresVictoryBonusKind::MELEM_FIRE":
                    return "Magic: Fire";
                case "ETresVictoryBonusKind::MELEM_WATER":
                    return "Magic: Water";
                case "ETresVictoryBonusKind::MELEM_CURE":
                    return "Magic: Cure";
                case "ETresVictoryBonusKind::MELEM_BLIZZARD":
                    return "Magic: Blizzard";
                case "ETresVictoryBonusKind::MELEM_THUNDER":
                    return "Magic: Thunder";
                case "ETresVictoryBonusKind::MELEM_AERO":
                    return "Magic: Aero";

                case "ETresAbilityKind::NONE":
                case "ETresVictoryBonusKind::NONE":
                    return "";
                #endregion Bonuses

                #region Synthesis Items

                #endregion Synthesis Items

                #region Magic
                case "Fire":
                case "Fira":
                case "Firaga":
                    return "Fire Spell";
                case "Water":
                case "Watera":
                case "Waterga":
                    return "Water Spell";
                case "Cure":
                case "Cura":
                case "Curaga":
                    return "Cure Spell";
                case "Blizzard":
                case "Blizzara":
                case "Blizzaga":
                    return "Blizzard Spell";
                case "Thunder":
                case "Thundara":
                case "Thundaga":
                    return "Thunder Spell";
                case "Aero":
                case "Aerora":
                case "Aeroga":
                    return "Aero Spell";
                #endregion Magic

                default:
                    return input;
            }
        }

        public static string DataTableEnumToKey(this DataTableEnum dataTableEnum)
        {
            switch (dataTableEnum)
            {
                case DataTableEnum.ChrInit:
                    return "Starting Stats";
                case DataTableEnum.EquipItem:
                    return "Equippables";
                case DataTableEnum.Event:
                    return "Events";
                case DataTableEnum.FullcourseAbility:
                    return "Fullcourse Abilities";
                case DataTableEnum.LevelUp:
                    return "Level Ups";
                case DataTableEnum.LuckyMark:
                    return "Lucky Emblems";
                case DataTableEnum.VBonus:
                    return "Bonuses";
                case DataTableEnum.WeaponEnhance:
                    return "Weapon Upgrades";
                case DataTableEnum.SynthesisItem:
                    return "Synthesis Items";

                case DataTableEnum.TreasureHE:
                case DataTableEnum.TreasureTT:
                case DataTableEnum.TreasureRA:
                case DataTableEnum.TreasureTS:
                case DataTableEnum.TreasureFZ:
                case DataTableEnum.TreasureMI:
                case DataTableEnum.TreasureCA:
                case DataTableEnum.TreasureBX:
                case DataTableEnum.TreasureKG:
                case DataTableEnum.TreasureEW:
                case DataTableEnum.TreasureBT:
                    return "Treasures";

                default:
                    return "";
            }
        }

        public static DataTableEnum KeyToDataTableEnum(this string key)
        {
            switch (key)
            {
                case "Starting Stats":
                    return DataTableEnum.ChrInit;
                case "Equippables":
                    return DataTableEnum.EquipItem;
                case "Events":
                    return DataTableEnum.Event;
                case "Fullcourse Abilities":
                    return DataTableEnum.FullcourseAbility;
                case "Level Ups":
                    return DataTableEnum.LevelUp;
                case "Lucky Emblems":
                    return DataTableEnum.LuckyMark;
                case "Bonuses":
                    return DataTableEnum.VBonus;
                case "Weapon Upgrades":
                    return DataTableEnum.WeaponEnhance;
                case "Synthesis Items":
                    return DataTableEnum.SynthesisItem;

                case "Olympus":
                    return DataTableEnum.TreasureHE;
                case "Twilight Town":
                    return DataTableEnum.TreasureTT;
                case "Kingdom of Corona":
                    return DataTableEnum.TreasureRA;
                case "Toy Box":
                    return DataTableEnum.TreasureTS;
                case "Arendelle":
                    return DataTableEnum.TreasureFZ;
                case "Monstropolis":
                    return DataTableEnum.TreasureMI;
                case "The Caribbean":
                    return DataTableEnum.TreasureCA;
                case "San Fransokyo":
                    return DataTableEnum.TreasureBX;
                case "Keyblade Graveyard":
                    return DataTableEnum.TreasureKG;
                case "The Final World":
                    return DataTableEnum.TreasureEW;
                case "Scala Ad Caelum":
                    return DataTableEnum.TreasureBT;

                default:
                    return DataTableEnum.None;
            }
        }

        public static string CategoryToKey(this string category, DataTableEnum dataTableEnum)
        {
            switch (dataTableEnum)
            {
                case DataTableEnum.ChrInit:
                    if (category == "Weapon")
                        return "Weapons";
                    else if (category != "Weapon" && !category.Contains("Crit"))
                        return "Abilities";
                    else if (category.Contains("Crit"))
                        return "Critical Abilities";
                    
                    break;
                case DataTableEnum.EquipItem:
                    if (category.Contains("I03"))
                        return "Weapons";
                    else if (category.Contains("I04"))
                        return "Armor";
                    else if (category.Contains("I05"))
                        return "Accessories";
                    
                    break;
                case DataTableEnum.Event:
                    if (!category.Contains("TresUIMobilePortalDataAsset") && !category.Contains("KEYBLADE") && !category.Contains("HEARTBINDER") && !category.Contains("REPORT") && !category.Contains("CKGAME") && !category.Contains("KEYITEM") && !category.Contains("DATAB") && !category.Contains("YOZORA"))
                        return "Events";
                    else if (category.Contains("KEYBLADE") || category.Equals("TresUIMobilePortalDataAsset"))
                        return "Keyblades";
                    else if (category.Contains("HEARTBINDER"))
                        return "Heartbinders";
                    else if (category.Contains("REPORT"))
                        return "Reports";
                    else if (category.Contains("CKGAME"))
                        return "Classic Kingdom";
                    else if (category.Contains("KEYITEM"))
                        return "Key Items";
                    else if (category.Contains("DATAB"))
                        return "Data Battles";
                    else if (category.Contains("YOZORA"))
                        return "Yozora";
                    
                    break;
                case DataTableEnum.FullcourseAbility:
                    return "Abilities";
                case DataTableEnum.LevelUp:
                    return "Levels";
                case DataTableEnum.LuckyMark:
                    return "Lucky Emblems";
                case DataTableEnum.VBonus:
                    if (!category.Contains("Minigame"))
                        return "VBonus";
                    else if (category.Contains("Minigame") && int.Parse(category[^3..]) >= 7)
                        return "Flantastic Seven";
                    else if (category.Contains("Minigame") && int.Parse(category[^3..]) < 7)
                        return "Minigames";

                    break;
                case DataTableEnum.WeaponEnhance:
                    var splitCategory = int.Parse(category.Split('_')[1]);

                    if (splitCategory < 10)
                        return "Kingdom Key";
                    else if (splitCategory >= 10 && splitCategory < 20)
                        return "Shooting Star";
                    else if (splitCategory >= 20 && splitCategory < 30)
                        return "Hero's Origin";
                    else if (splitCategory >= 30 && splitCategory < 40)
                        return "Favorite Deputy";
                    else if (splitCategory >= 40 && splitCategory < 50)
                        return "Ever After";
                    else if (splitCategory >= 50 && splitCategory < 60)
                        return "Wheel of Fate";
                    else if (splitCategory >= 60 && splitCategory < 70)
                        return "Crystal Snow";
                    else if (splitCategory >= 70 && splitCategory < 80)
                        return "Hunny Spout";
                    else if (splitCategory >= 80 && splitCategory < 90)
                        return "Nano Gear";
                    else if (splitCategory >= 90 && splitCategory < 100)
                        return "Happy Gear";
                    else if (splitCategory >= 100 && splitCategory < 110)
                        return "Classic Tone";
                    else if (splitCategory >= 110 && splitCategory < 120)
                        return "Grand Chef";
                    else if (splitCategory >= 120 && splitCategory < 130)
                        return "Ultima Weapon";
                    else if (splitCategory >= 150 && splitCategory < 160)
                        return "Elemental Encoder";
                    else if (splitCategory >= 160 && splitCategory < 170)
                        return "Starlight";
                    else if (splitCategory >= 170 && splitCategory < 180)
                        return "Oathkeeper";
                    else if (splitCategory >= 180 && splitCategory < 190)
                        return "Oblivion";

                    break;
                case DataTableEnum.SynthesisItem:
                    return "Synthesis Items";

                case DataTableEnum.TreasureHE:
                    return "Olympus";
                case DataTableEnum.TreasureTT:
                    return "Twilight Town";
                case DataTableEnum.TreasureRA:
                    return "Kingdom of Corona";
                case DataTableEnum.TreasureTS:
                    return "Toy Box";
                case DataTableEnum.TreasureFZ:
                    return "Arendelle";
                case DataTableEnum.TreasureMI:
                    return "Monstropolis";
                case DataTableEnum.TreasureCA:
                    return "The Caribbean";
                case DataTableEnum.TreasureBX:
                    return "San Fransokyo";
                case DataTableEnum.TreasureKG:
                    return "Keyblade Graveyard";
                case DataTableEnum.TreasureEW:
                    return "The Final World";
                case DataTableEnum.TreasureBT:
                    return "Scala Ad Caelum";

                default:
                    return "";
            }

            return "";
        }

        public static string ToLevelUpRoute(this string type)
        {
            switch (type)
            {
                case "TypeA":
                    return "Warrior";
                case "TypeB":
                    return "Mystic";
                case "TypeC":
                    return "Guardian";
                default:
                    return "";
            }
        }

        public static string GetChestLocation(this string chest, DataTableEnum dataTableEnum)
        {
            switch (dataTableEnum)
            {
                case DataTableEnum.TreasureHE:
                    switch (chest)
                    {
                        case "Large Chest 1":
                            //return "Chest 24 (Large Chest, Thebes: Overlook)"; Large Chest 1 and 4 are swapped right now
                            return "Chest 29 (Large Chest, Realm of the Gods: Corridors)";
                        case "Large Chest 2":
                            return "Chest 9 (Large Chest, Mount Olympus: Cliff Ascent)";
                        case "Large Chest 3":
                            return "Chest 31 (Large Chest, Realm of the Gods: Apex)";
                        case "Large Chest 4":
                            //return "Chest 29 (Large Chest, Realm of the Gods: Corridors)"; Large Chest 1 and 4 are swapped right now
                            return "Chest 24 (Large Chest, Thebes: Overlook)";
                        case "Small Chest 1":
                            return "Chest 25 (Small Chest, Realm of the Gods: Courtyard)";
                        case "Small Chest 2":
                            return "Chest 26 (Small Chest, Realm of the Gods: Courtyard)";
                        case "Small Chest 3":
                            return "Chest 27 (Small Chest, Realm of the Gods: Courtyard)";
                        case "Small Chest 4":
                            return "Chest 28 (Small Chest, Realm of the Gods: Corridors)";
                        case "Small Chest 6":
                            return "Chest 30 (Small Chest, Realm of the Gods: Cloud Ridge)";
                        case "Small Chest 8":
                            return "Chest 32 (Small Chest, Realm of the Gods: Apex)";
                        case "Small Chest 9":
                            return "Chest 1 (Small Chest, Mount Olympus: Ravine)";
                        case "Small Chest 10":
                            return "Chest 2 (Small Chest, Mount Olympus: Ravine)";
                        case "Small Chest 11":
                            return "Chest 3 (Small Chest, Mount Olympus: Cliff Ascent)";
                        case "Small Chest 12":
                            return "Chest 4 (Small Chest, Mount Olympus: Cliff Ascent)";
                        case "Small Chest 13":
                            return "Chest 5 (Small Chest, Mount Olympus: Cliff Ascent)";
                        case "Small Chest 14":
                            return "Chest 6 (Small Chest, Mount Olympus: Cliff Ascent)";
                        case "Small Chest 15":
                            return "Chest 7 (Small Chest, Mount Olympus: Cliff Ascent)";
                        case "Small Chest 16":
                            return "Chest 8 (Small Chest, Mount Olympus: Cliff Ascent)";
                        case "Small Chest 17":
                            return "Chest 10 (Small Chest, Mount Olympus: Mountainside)";
                        case "Small Chest 20":
                            return "Chest 11 (Small Chest, Mount Olympus: Summit)";
                        case "Small Chest 21":
                            return "Chest 12 (Small Chest, Mount Olympus: Mountainside)";
                        case "Small Chest 22":
                            return "Chest 13 (Small Chest, Thebes: Alleyway)";
                        case "Small Chest 23":
                            return "Chest 14 (Small Chest, Thebes: Alleyway)";
                        case "Small Chest 24":
                            return "Chest 15 (Small Chest, Thebes: Agora)";
                        case "Small Chest 25":
                            return "Chest 16 (Small Chest, Thebes: The Big Olive)";
                        case "Small Chest 26":
                            return "Chest 17 (Small Chest, Thebes: The Big Olive)";
                        case "Small Chest 27":
                            return "Chest 18 (Small Chest, Thebes: Gardens)";
                        case "Small Chest 28":
                            return "Chest 19 (Small Chest, Thebes: Overlook)";
                        case "Small Chest 29":
                            return "Chest 20 (Small Chest, Thebes: Overlook)";
                        case "Small Chest 30":
                            return "Chest 21 (Small Chest, Thebes: Gardens)";
                        case "Small Chest 31":
                            return "Chest 22 (Small Chest, Thebes: Overlook)";
                        case "Small Chest 33":
                            return "Chest 23 (Small Chest, Thebes: Overlook)";
                        default: 
                            return "";
                    }
                case DataTableEnum.TreasureTT:
                    switch (chest)
                    {
                        case "Large Chest 1":
                            return "Chest 1 (Large Chest, The Neighborhood: Tram Common)";
                        case "Small Chest 1":
                            return "Chest 2 (Small Chest, The Neighborhood: Tram Common)";
                        case "Small Chest 2":
                            return "Chest 3 (Small Chest, The Neighborhood: Tram Common)";
                        case "Small Chest 3":
                            return "Chest 4 (Small Chest, The Neighborhood: Tram Common)";
                        case "Small Chest 4":
                            return "Chest 5 (Small Chest, The Neighborhood: Tram Common)";
                        case "Small Chest 5":
                            return "Chest 6 (Small Chest, Underground Conduit)";
                        case "Small Chest 6":
                            return "Chest 7 (Small Chest, The Woods)";
                        case "Small Chest 7":
                            return "Chest 8 (Small Chest, The Woods)";
                        case "Small Chest 8":
                            return "Chest 9 (Small Chest, The Woods)";
                        case "Small Chest 9":
                            return "Chest 10 (Small Chest, The Old Mansion)";
                        default:
                            return "";
                    }
                case DataTableEnum.TreasureTS:
                    switch (chest)
                    {
                        case "Large Chest 1":
                            return "Chest 1 (Large Chest, Andy's House)";
                        case "Large Chest 2":
                            return "Chest 2 (Large Chest, Andy's House)";
                        case "Large Chest 3":
                            return "Chest 5 (Large Chest, Galaxy Toys: Main Floor 1F)";
                        case "Large Chest 4":
                            return "Chest 13 (Large Chest, Galaxy Toys: Lower Vents)";
                        case "Large Chest 5":
                            return "Chest 24 (Large Chest, Galaxy Toys: Kid Korral)";
                        case "Large Chest 6":
                            return "Chest 29 (Large Chest, Galaxy Toys: Rail 3)";
                        case "Small Chest 1":
                            return "Chest 3 (Small Chest, Andy's House)";
                        case "Small Chest 2":
                            return "Chest 4 (Small Chest, Andy's House)";
                        case "Small Chest 3":
                            return "Chest 6 (Small Chest, Galaxy Toys: Exit)";
                        case "Small Chest 4":
                            return "Chest 7 (Small Chest, Galaxy Toys: Rail 3)";
                        case "Small Chest 5":
                            return "Chest 8 (Small Chest, Galaxy Toys: Action Figures)";
                        case "Small Chest 6":
                            return "Chest 9 (Small Chest, Galaxy Toys: Action Figures)";
                        case "Small Chest 7":
                            return "Chest 10 (Small Chest, Galaxy Toys: Action Figures)";
                        case "Small Chest 8":
                            return "Chest 11 (Small Chest, Galaxy Toys: Lower Vents)";
                        case "Small Chest 9":
                            return "Chest 12 (Small Chest, Galaxy Toys: Lower Vents)";
                        case "Small Chest 11":
                            return "Chest 14 (Small Chest, Babies and Toddlers: Dolls)";
                        case "Small Chest 12":
                            return "Chest 15 (Small Chest, Babies and Toddlers: Dolls)";
                        case "Small Chest 13":
                            return "Chest 16 (Small Chest, Babies and Toddlers: Dolls)";
                        case "Small Chest 14":
                            return "Chest 17 (Small Chest, Babies and Toddlers: Dolls)";
                        case "Small Chest 15":
                            return "Chest 18 (Small Chest, Babies and Toddlers: Outdoors)";
                        case "Small Chest 16":
                            return "Chest 19 (Small Chest, Babies and Toddlers: Outdoors)";
                        case "Small Chest 17":
                            return "Chest 20 (Small Chest, Galaxy Toys: Video Games)";
                        case "Small Chest 18":
                            return "Chest 21 (Small Chest, Galaxy Toys: Kid Korral)";
                        case "Small Chest 19":
                            return "Chest 22 (Small Chest, Galaxy Toys: Kid Korral)";
                        case "Small Chest 20":
                            return "Chest 23 (Small Chest, Galaxy Toys: Kid Korral)";
                        case "Small Chest 22":
                            return "Chest 25 (Small Chest, Galaxy Toys: Kid Korral)";
                        case "Small Chest 23":
                            return "Chest 26 (Small Chest, Galaxy Toys: Main Floor 2F)";
                        case "Small Chest 24":
                            return "Chest 27 (Small Chest, Galaxy Toys: Main Floor 1F)";
                        case "Small Chest 25":
                            return "Chest 28 (Small Chest, Babies and Toddlers: Outdoors)";
                        default:
                            return "";
                    }
                case DataTableEnum.TreasureRA:
                    switch (chest)
                    {
                        case "Large Chest 1":
                            return "Chest 7 (Large Chest, The Forest: Hills)";
                        case "Large Chest 2":
                            return "Chest 10 (Large Chest, The Forest: Marsh)";
                        case "Large Chest 3":
                            return "Chest 9 (Large Chest, The Forest: Hills)";
                        case "Large Chest 4":
                            return "Chest 22 (Large Chest, The Forest: Wildflower Clearing)";
                        case "Large Chest 5":
                            return "Chest 24 (Large Chest, The Kingdom: Thoroughfare)";
                        case "Large Chest 6":
                            return "Chest 14 (Large Chest, The Forest: Wetlands)";
                        case "Small Chest 1":
                            return "Chest 1 (Small Chest, The Forest: Tower)";
                        case "Small Chest 2":
                            return "Chest 2 (Small Chest, The Forest: Hills)";
                        case "Small Chest 3":
                            return "Chest 3 (Small Chest, The Forest: Hills)";
                        case "Small Chest 4":
                            return "Chest 4 (Small Chest, The Forest: Hills)";
                        case "Small Chest 5":
                            return "Chest 5 (Small Chest, The Forest: Hills)";
                        case "Small Chest 6":
                            return "Chest 6 (Small Chest, The Forest: Hills)";
                        case "Small Chest 7":
                            return "Chest 8 (Small Chest, The Forest: Hills)";
                        case "Small Chest 8":
                            return "Chest 11 (Small Chest, The Forest: Marsh)";
                        case "Small Chest 9":
                            return "Chest 12 (Small Chest, The Forest: Marsh)";
                        case "Small Chest 10":
                            return "Chest 13 (Small Chest, The Forest: Wetlands)";
                        case "Small Chest 11":
                            return "Chest 15 (Small Chest, The Forest: Wetlands)";
                        case "Small Chest 12":
                            return "Chest 16 (Small Chest, The Forest: Wetlands)";
                        case "Small Chest 13":
                            return "Chest 17 (Small Chest, The Forest: Wetlands)";
                        case "Small Chest 14":
                            return "Chest 18 (Small Chest, The Forest: Wetlands/Campsite)";
                        case "Small Chest 15":
                            return "Chest 19 (Small Chest, The Forest: Campsite)";
                        case "Small Chest 16":
                            return "Chest 20 (Small Chest, The Forest: Shore)";
                        case "Small Chest 17":
                            return "Chest 21 (Small Chest, The Forest: Wildflower Clearing)";
                        case "Small Chest 18":
                            return "Chest 23 (Small Chest, The Kingdom: Thoroughfare)";
                        case "Small Chest 19":
                            return "Chest 25 (Small Chest, The Kingdom: Thoroughfare)";
                        case "Small Chest 20":
                            return "Chest 26 (Small Chest, The Kingdom: Wharf)";
                        case "Small Chest 21":
                            return "Chest 27 (Small Chest, The Kingdom: Wharf)";
                        case "Small Chest 22":
                            return "Chest 28 (Small Chest, The Kingdom: Wharf)";
                        default:
                            return "";
                    }
                case DataTableEnum.TreasureMI:
                    switch (chest)
                    {
                        case "Large Chest 1":
                            return "Chest 1 (Large Chest, Monsters, Inc.)";
                        case "Large Chest 2":
                            return "Chest 9 (Large Chest, The Factory)";
                        case "Large Chest 3":
                            return "Chest 19 (Large Chest, The Power Plant: Tank Yard)";
                        case "Large Chest 4":
                            return "Chest 10 (Large Chest, The Factory)";
                        case "Small Chest 1":
                            return "Chest 2 (Small Chest, Monsters, Inc.)";
                        case "Small Chest 2":
                            return "Chest 3 (Small Chest, Monsters, Inc.)";
                        case "Small Chest 3":
                            return "Chest 4 (Small Chest, Monsters, Inc.)";
                        case "Small Chest 5":
                            return "Chest 11 (Small Chest, The Factory)";
                        case "Small Chest 6":
                            return "Chest 12 (Small Chest, The Factory)";
                        case "Small Chest 7":
                            return "Chest 13 (Small Chest, Vault Door: Service Area)";
                        case "Small Chest 8":
                            return "Chest 14 (Small Chest, The Factory: Second Floor)";
                        case "Small Chest 9":
                            return "Chest 15 (Small Chest, The Power Plant: Accessway)";
                        case "Small Chest 10":
                            return "Chest 16 (Small Chest, The Power Plant: Accessway)";
                        case "Small Chest 11":
                            return "Chest 17 (Small Chest, The Power Plant: Accessway)";
                        case "Small Chest 12":
                            return "Chest 18 (Small Chest, The Power Plant: Tank Yard)";
                        case "Small Chest 13":
                            return "Chest 20 (Small Chest, The Power Plant: Vault Passage)";
                        case "Small Chest 14":
                            return "Chest 21 (Small Chest, The Power Plant: Vault Passage)";
                        case "Small Chest 15":
                            return "Chest 22 (Small Chest, The Power Plant: Accessway)";
                        case "Small Chest 16":
                            return "Chest 5 (Small Chest, Monsters, Inc.: Upper Level)";
                        case "Small Chest 17":
                            return "Chest 6 (Small Chest, Monsters, Inc.: Lower Levels)";
                        case "Small Chest 18":
                            return "Chest 7 (Small Chest, The Door Vault: Service Area)";
                        case "Small Chest 19":
                            return "Chest 8 (Small Chest, The Door Vault: Upper Level)";
                        default:
                            return "";
                    }
                case DataTableEnum.TreasureFZ:
                    switch (chest)
                    {
                        case "Large Chest 1":
                            return "Chest 1 (Large Chest, The North Mountain: Treescape)";
                        case "Large Chest 2":
                            return "Chest 10 (Large Chest, The Labyrinth of Ice: Middle Tier)";
                        case "Large Chest 3":
                            return "Chest 8 (Large Chest, The North Mountain: Treescape)";
                        case "Large Chest 4":
                            return "Chest 7 (Large Chest, The North Mountain: Snowfield)";
                        case "Large Chest 5":
                            return "Chest 14 (Large Chest, The Labyrinth of Ice: Lower Tier)";
                        case "Large Chest 6":
                            return "Chest 9 (Large Chest, The North Mountain: Snowfield)";
                        case "Small Chest 1":
                            return "Chest 2 (Small Chest, The North Mountain: Treescape)";
                        case "Small Chest 2":
                            return "Chest 3 (Small Chest, The North Mountain: Treescape)";
                        case "Small Chest 4":
                            return "Chest 4 (Small Chest, The North Mountain: Gorge)";
                        case "Small Chest 5":
                            return "Chest 5 (Small Chest, The North Mountain: Gorge)";
                        case "Small Chest 7":
                            return "Chest 6 (Small Chest, The North Mountain: Snowfield)";
                        case "Small Chest 11":
                            return "Chest 11 (Small Chest, The Labyrinth of Ice: Lower Tier)";
                        case "Small Chest 12":
                            return "Chest 12 (Small Chest, The Labyrinth of Ice: Middle Tier)";
                        case "Small Chest 13":
                            return "Chest 13 (Small Chest, The Labyrinth of Ice: Lower Tier)";
                        case "Small Chest 15":
                            return "Chest 15 (Small Chest, The Labyrinth of Ice: Middle Tier)";
                        case "Small Chest 16":
                            return "Chest 16 (Small Chest, The North Mountain: Valley of Ice)";
                        case "Small Chest 17":
                            return "Chest 17 (Small Chest, The North Mountain: Valley of Ice)";
                        case "Small Chest 19":
                            return "Chest 18 (Small Chest, The North Mountain: Valley of Ice)";
                        case "Small Chest 20":
                            return "Chest 19 (Small Chest, The North Mountain: Valley of Ice)";
                        case "Small Chest 22":
                            return "Chest 20 (Small Chest, The North Mountain: Valley of Ice)";
                        case "Small Chest 23":
                            return "Chest 21 (Small Chest, The North Mountain: The Frozen Wall)";
                        case "Small Chest 24":
                            return "Chest 22 (Small Chest, The North Mountain: The Frozen Wall)";
                        case "Small Chest 25":
                            return "Chest 23 (Small Chest, The North Mountain: The Frozen Wall)";
                        case "Small Chest 27":
                            return "Chest 24 (Small Chest, The North Mountain: Foothills)";
                        case "Small Chest 29":
                            return "Chest 25 (Small Chest, The North Mountain: Foothills)";
                        default:
                            return "";
                    }
                case DataTableEnum.TreasureCA:
                    switch (chest)
                    {
                        // Large Chest 1-5 are being shifted up by one right now
                        case "Large Chest 1":
                            //return "Chest 20 (Large Chest, The Huddled Isles)";
                            return "Chest 19 (Large Chest, Sandbar Isle)";
                        case "Large Chest 2":
                            //return "Chest 19 (Large Chest, Sandbar Isle)";
                            return "Chest 17 (Large Chest, Isla de los Mástiles)";
                        case "Large Chest 3":
                            //return "Chest 17 (Large Chest, Isla de los Mástiles)";
                            return "Chest 18 (Large Chest, Ship's End)";
                        case "Large Chest 4":
                            //return "Chest 18 (Large Chest, Ship's End)";
                            return "Chest 51 (Large Chest, Port Royal: Docks)";
                        case "Large Chest 5":
                            //return "Chest 51 (Large Chest, Port Royal: Docks)";
                            return "Chest 20 (Large Chest, The Huddled Isles)";
                        case "Large Chest 6":
                            return "Chest 47 (Large Chest, Port Royal: Fort)";
                        case "Large Chest 7":
                            return "Chest 4 (Large Chest, The Huddled Isles)";
                        case "Large Chest 8":
                            return "Chest 10 (Large Chest, Isla Verdemontaña)";
                        case "Large Chest 9":
                            return "Chest 13 (Large Chest, Confinement Island)";
                        case "Large Chest 10":
                            return "Chest 15 (Large Chest, The Huddled Isles)";
                        case "Large Chest 11":
                            return "Chest 6 (Large Chest, Isla de los Mástiles)";
                        case "Small Chest 1":
                            return "Chest 46 (Small Chest, Port Royal: Fort)";
                        case "Small Chest 2":
                            return "Chest 48 (Small Chest, Port Royal: Seaport)";
                        case "Small Chest 3":
                            return "Chest 49 (Small Chest, Port Royal: Seaport)";
                        case "Small Chest 4":
                            return "Chest 50 (Small Chest, Port Royal: Seaport)";
                        case "Small Chest 5":
                            return "Chest 52 (Small Chest, Port Royal: Settlement)";
                        case "Small Chest 6":
                            return "Chest 53 (Small Chest, Port Royal: Docks)";
                        case "Small Chest 7":
                            return "Chest 54 (Small Chest, Port Royal: Underwater)";
                        case "Small Chest 8":
                            return "Chest 55 (Small Chest, Port Royal: Settlement)";
                        case "Small Chest 9":
                            return "Chest 56 (Small Chest, Port Royal: Seaport)";
                        case "Small Chest 10":
                            return "Chest 1 (Small Chest, The Huddled Isles)";
                        case "Small Chest 11":
                            return "Chest 2 (Small Chest, The Huddled Isles)";
                        case "Small Chest 12":
                            return "Chest 3 (Small Chest, The Huddled Isles)";
                        case "Small Chest 13":
                            return "Chest 5 (Small Chest, Isla de los Mástiles)";
                        case "Small Chest 14":
                            return "Chest 7 (Small Chest, Ship's End)";
                        case "Small Chest 15":
                            return "Chest 8 (Small Chest, Ship's End)";
                        case "Small Chest 16":
                            return "Chest 9 (Small Chest, Isla Verdemontaña)";
                        case "Small Chest 17":
                            return "Chest 11 (Small Chest, Sandbar Isle)";
                        case "Small Chest 18":
                            return "Chest 12 (Small Chest, Exile Island)";
                        case "Small Chest 19":
                            return "Chest 14 (Small Chest, The Gateway of Regret)";
                        case "Small Chest 20":
                            return "Chest 16 (Small Chest, Horseshoe Island)";
                        case "Small Chest 21":
                            return "Chest 21 (Small Chest, The Huddled Isles)";
                        case "Small Chest 22":
                            return "Chest 22 (Small Chest, The Huddled Isles)";
                        case "Small Chest 23":
                            return "Chest 23 (Small Chest, Isla de los Mástiles)";
                        case "Small Chest 24":
                            return "Chest 24 (Small Chest, Isla de los Mástiles)";
                        case "Small Chest 25":
                            return "Chest 45 (Small Chest, Leviathan)";
                        case "Small Chest 26":
                            return "Chest 25 (Small Chest, Isla Verdemontaña)";
                        case "Small Chest 27":
                            return "Chest 26 (Small Chest, Sandbar Isle)";
                        case "Small Chest 28":
                            return "Chest 27 (Small Chest, Sandbar Isle)";
                        case "Small Chest 29":
                            return "Chest 28 (Small Chest, Sandbar Isle)";
                        case "Small Chest 30":
                            return "Chest 29 (Small Chest, Sandbar Isle)";
                        case "Small Chest 31":
                            return "Chest 30 (Small Chest, Sandbar Isle)";
                        case "Small Chest 32":
                            return "Chest 31 (Small Chest, Sandbar Isle)";
                        case "Small Chest 33":
                            return "Chest 32 (Small Chest, Sandbar Isle)";
                        case "Small Chest 34":
                            return "Chest 33 (Small Chest, Sandbar Isle)";
                        case "Small Chest 35":
                            return "Chest 34 (Small Chest, Sandbar Isle)";
                        case "Small Chest 36":
                            return "Chest 35 (Small Chest, Sandbar Isle)";
                        case "Small Chest 37":
                            return "Chest 36 (Small Chest, Sandbar Isle)";
                        case "Small Chest 38":
                            return "Chest 37 (Small Chest, Sandbar Isle)";
                        case "Small Chest 39":
                            return "Chest 38 (Small Chest, Sandbar Isle)";
                        case "Small Chest 40":
                            return "Chest 39 (Small Chest, Sandbar Isle)";
                        case "Small Chest 41":
                            return "Chest 40 (Small Chest, Sandbar Isle)";
                        case "Small Chest 42":
                            return "Chest 41 (Small Chest, Horseshoe Island)";
                        case "Small Chest 43":
                            return "Chest 42 (Small Chest, Horseshoe Island)";
                        case "Small Chest 44":
                            return "Chest 43 (Small Chest, Horseshoe Island)";
                        case "Small Chest 45":
                            return "Chest 44 (Small Chest, Horseshoe Island)";
                        default:
                            return "";
                    }
                case DataTableEnum.TreasureBX:
                    switch (chest)
                    {
                        case "Large Chest 1":
                            return "Chest 3 (Large Chest, The City: South District)";
                        case "Large Chest 5":
                            return "Chest 34 (Large Chest, The City: Central District)";
                        case "Large Chest 6":
                            return "Chest 35 (Large Chest, The City: South District)";
                        case "Large Chest 7":
                            return "Chest 36 (Large Chest, The City: Central District)";

                        case "Small Chest 1":
                            return "Chest 1 (Small Chest, The City: Central District)";
                        case "Small Chest 2":
                            return "Chest 2 (Small Chest, The City: South District)";
                        case "Small Chest 3":
                            return "Chest 4 (Small Chest, The City: South District)";
                        case "Small Chest 4":
                            return "Chest 5 (Small Chest, The City: North District)";
                        case "Small Chest 5":
                            return "Chest 6 (Small Chest, The City: North District)";
                        case "Small Chest 6":
                            return "Chest 7 (Small Chest, The City: South District)";
                        case "Small Chest 7":
                            return "Chest 8 (Small Chest, The City: Central District)";
                        case "Small Chest 8":
                            return "Chest 9 (Small Chest, The City: Central District)";
                        case "Small Chest 9":
                            return "Chest 10 (Small Chest, The City: North District)";
                        case "Small Chest 10":
                            return "Chest 11 (Small Chest, The City: South District)";
                        case "Small Chest 11":
                            return "Chest 12 (Small Chest, The City: South District)";
                        case "Small Chest 12":
                            return "Chest 13 (Small Chest, The City: North District)";
                        case "Small Chest 13":
                            return "Chest 14 (Small Chest, The City: North District)";
                        case "Small Chest 14":
                            return "Chest 15 (Small Chest, The City: North District)";
                        case "Small Chest 15":
                            return "Chest 16 (Small Chest, The City: Central District)";
                        case "Small Chest 16":
                            return "Chest 17 (Small Chest, The City: Central District)";
                        case "Small Chest 17":
                            return "Chest 18 (Small Chest, The City: South District)";
                        case "Small Chest 18":
                            return "Chest 19 (Small Chest, The City: North District)";
                        case "Small Chest 19":
                            return "Chest 20 (Small Chest, The City: South District)";
                        case "Small Chest 20":
                            return "Chest 21 (Small Chest, The City: South District)";
                        case "Small Chest 21":
                            return "Chest 22 (Small Chest, The City: North District)";
                        case "Small Chest 22":
                            return "Chest 23 (Small Chest, The City: Central District)";
                        case "Small Chest 23":
                            return "Chest 24 (Small Chest, The City: Central District)";
                        case "Small Chest 24":
                            return "Chest 25 (Small Chest, The City: Central District)";
                        case "Small Chest 25":
                            return "Chest 26 (Small Chest, The City: Central District)";
                        case "Small Chest 26":
                            return "Chest 27 (Small Chest, The City: South District)";
                        case "Small Chest 27":
                            return "Chest 28 (Small Chest, The City: North District)";
                        case "Small Chest 28":
                            return "Chest 29 (Small Chest, The City: North District)";
                        case "Small Chest 29":
                            return "Chest 30 (Small Chest, The City: Central District)";
                        case "Small Chest 30":
                            return "Chest 31 (Small Chest, The City: South District)";
                        case "Small Chest 31":
                            return "Chest 32 (Small Chest, The City: North District)";
                        case "Small Chest 32":
                            return "Chest 33 (Small Chest, The City: North District)";
                        default:
                            return "";
                    }

                case DataTableEnum.TreasureKG:
                    switch (chest)
                    {
                        case "Large Chest 1":
                            return "Chest 1 (Large Chest, The Badlands)";
                        case "Large Chest 2":
                            return "Chest 3 (Large Chest, The Skein of Severance: Trail of Valediction)";
                        case "Small Chest 1":
                            return "Chest 4 (Small Chest, The Skein of Severance: Trail of Valediction)";
                        case "Small Chest 2":
                            return "Chest 5 (Small Chest, The Skein of Severance: Trail of Valediction/Twist of Isolation)";
                        case "Small Chest 3":
                            return "Chest 6 (Small Chest, The Skein of Severance: Twist of Isolation)";
                        case "Small Chest 4":
                            return "Chest 2 (Small Chest, The Badlands)";
                        default:
                            return "";
                    }

                case DataTableEnum.TreasureEW:
                    switch (chest)
                    {
                        case "Large Chest 1":
                            return "Chest 1 (Large Chest, The Final World)";
                        default:
                            return "";
                    }

                case DataTableEnum.TreasureBT:
                    switch (chest)
                    {
                        case "Large Chest 1":
                            return "Chest 1 (Large Chest, The Stairway to the Sky)";
                        case "Large Chest 2":
                            return "Chest 2 (Large Chest, Breezy Quarter)";
                        case "Small Chest 1":
                            return "Chest 3 (Small Chest, Breezy Quarter)";
                        case "Small Chest 2":
                            return "Chest 4 (Small Chest, Breezy Quarter)";
                        case "Small Chest 3":
                            return "Chest 5 (Small Chest, Breezy Quarter)";
                        case "Small Chest 4":
                            return "Chest 6 (Small Chest, Breezy Quarter)";
                        case "Small Chest 5":
                            return "Chest 7 (Small Chest, Breezy Quarter)";
                        case "Small Chest 6":
                            return "Chest 8 (Small Chest, Breezy Quarter)";
                        case "Small Chest 7":
                            return "Chest 9 (Small Chest, Breezy Quarter)";
                        default:
                            return "";
                    }
                default:
                    return "";
            }
        }
    }
}