using System;
using System.Collections.Generic;
using UE4DataTableInterpreter.Enums;
using KH3Randomizer.Models;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.IO.Compression;
using KH3Randomizer.Enums;

namespace KH3Randomizer.Data
{
    public class RandomizerService
    {
        // TODO Find out if this is even used now that we've switched to this new system
        public readonly List<DataTableEnum> FirstPassCategories = new() { DataTableEnum.ChrInit, DataTableEnum.EquipItem, DataTableEnum.VBonus, DataTableEnum.WeaponEnhance, DataTableEnum.Event };

        // Any Ability, Bonus, Armor, Accessory, Map, Weapon, Key Item, Report, CK or Boost Item
        public readonly List<string> ImportantChecks = new() { "KEY", "REPORT", "LSI", "ITEM_APUP", "ITEM_MAGICUP", "ITEM_POWERUP", "ITEM_GUARDUP",
                                                               "LIBRA", "DODGE", "AIRSLIDE", "REFLECT_GUARD", "POLE_SPIN", "WALL_KICK", "AIR_RECOVERY", 
                                                               "DOUBLEFLIGHT", "LAST_LEAVE", "COMBO_LEAVE",
                                                               "MELEM", "HP_UP", "MP_UP", "SLOT_UP" };

        public readonly List<string> ReplaceableAbilities = new() { "SLASH_UPPER", "AIR_ROLL_BEAT", "AIR_DOWN", "TRIPPLE_SLASH", "CHARGE_THRUST", "MAGICFLUSH",
                                                                    "MP_SAFETY", "EXPZERO", "FRIEND_AID", "COMBO_PLUS", "AIRCOMBO_PLUS", "FIRE_UP",
                                                                    "BLIZZARD_UP", "THUNDER_UP", "WATER_UP", "AERO_UP", "WIZZARD_STAR", "LUCK_UP", "ITEM_UP",
                                                                    "PRIZE_DRAW", "FOCUS_ASPIR", "ATTRACTION_TIME", "LINK_BOOST", "FORM_TIME", "DEFENDER", 
                                                                    "CRITICAL_HALF", "DAMAGE_ASPIR", "MP_HASTE", "MP_HASTERA", "WALK_REGENE", "WALK_HEALING",
                                                                    "MAGIC_DRAW", "ATTRACTION_UP", "BURN_GUARD", "CLOUD_GUARD", "SNEEZE_GUARD", "FREEZE_GUARD",
                                                                    "DISCHARGE_GUARD", "STUN_GUARD", "COUNTER_UP", "AUTO_FINISH", "FORM_UP", "MAGIC_TIME", 
                                                                    "AUTO_LOCK_MAGIC", "MP_LEAVE", "FULLMP_BURST", "HARVEST", "HP_CONVERTER", "MP_CONVERTER",
                                                                    "MUNNY_CONVERTER", "ENDLESS_MAGIC", "FP_CONVERTER", "FIRE_ASPIR", "BLIZZARD_ASPIR", 
                                                                    "THUNDER_ASPIR", "WATER_ASPIR", "AERO_ASPIR", "DARK_ASPIR", "SONIC_SLASH", "SONIC_DOWN",
                                                                    "SUMMERSALT", "BATTLE_GRAPHER", "CHARISMA_CHEF", "POWER_CURE", "LUNCH_TIME", "POWER_LUNCH",
                                                                    "OVER_TIME", "BEST_CONDITION", "PRIZE_FEEVER", "MILLIONAIRE", "CURAGAN", "CHARGE_BERSERK", 
                                                                    "GRAND_MAGIC", "FIRAGAN", "BLIZZAGAN", "THUNDAGAN", "WATAGAN", "AEROGAN", "MAGIC_ROULETTE",
                                                                    "UNISON_FIRE", "UNISON_BLIZZARD", "UNISON_THUNDER", "FUSION_SPIN", "FUSION_ROCKET" };

        public readonly List<string> Heartbinders = new() { "KEY_ITEM06", "KEY_ITEM07", "KEY_ITEM08", "KEY_ITEM09", "KEY_ITEM10" };

        public readonly List<string> VBonusCriticalAbilities = new() { "Vbonus_017", "Vbonus_026", "Vbonus_028", "Vbonus_032", "Vbonus_036", "Vbonus_041",
                                                                       "Vbonus_045", "Vbonus_049", "Vbonus_050", "Vbonus_055", "Vbonus_058", "Vbonus_060", "Vbonus_069" };

        // OBSELETE
        public Dictionary<string, Dictionary<string, bool>> GetAvailableOptions(Dictionary<string, RandomizeOptionEnum> availablePools, ref Dictionary<string, Dictionary<string, bool>> availableOptions, bool backTo = false)
        {
            if (backTo)
                return availableOptions;

            availableOptions = new();

            foreach (var pool in availablePools)
            {
                switch (pool.Key)
                {
                    // Worlds
                    case "Olympus":
                    case "Twilight Town":
                    case "Toy Box":
                    case "Kingdom of Corona":
                    case "Monstropolis":
                    case "Arendelle":
                    case "San Fransokyo":
                    case "The Caribbean":
                    case "Keyblade Graveyard":
                        if (!availableOptions.ContainsKey("Worlds"))
                            availableOptions.Add("Worlds", new Dictionary<string, bool>());

                        availableOptions["Worlds"].Add(pool.Key, true);
                        availableOptions["Worlds"].Add($"{pool.Key}: Treasures", true);
                        availableOptions["Worlds"].Add($"{pool.Key}: Events", true);
                        availableOptions["Worlds"].Add($"{pool.Key}: Bonuses", true);

                        break;


                    case "100 Acre Wood":
                        if (!availableOptions.ContainsKey("Worlds"))
                            availableOptions.Add("Worlds", new Dictionary<string, bool>());

                        availableOptions["Worlds"].Add(pool.Key, true);
                        availableOptions["Worlds"].Add($"{pool.Key}: Events", true);

                        break;
                    case "Re:Mind":
                        if (!availableOptions.ContainsKey("Worlds"))
                            availableOptions.Add("Worlds", new Dictionary<string, bool>());

                        availableOptions["Worlds"].Add(pool.Key, true);
                        availableOptions["Worlds"].Add($"{pool.Key}: Treasures", true);
                        availableOptions["Worlds"].Add($"{pool.Key}: Bonuses", true);

                        break;
                    case "Dark World":
                        if (!availableOptions.ContainsKey("Worlds"))
                            availableOptions.Add("Worlds", new Dictionary<string, bool>());

                        availableOptions["Worlds"].Add(pool.Key, true);
                        availableOptions["Worlds"].Add($"{pool.Key}: Bonuses", true);

                        break;
                    case "Unreality":
                        if (!availableOptions.ContainsKey("Worlds"))
                            availableOptions.Add("Worlds", new Dictionary<string, bool>());
                        
                        availableOptions["Worlds"].Add(pool.Key, true);
                        availableOptions["Worlds"].Add($"{pool.Key}: Events", true);

                        break;

                    // Miscellaneous
                    case "Sora":
                        var soraOptions = new Dictionary<string, bool>
                        {
                            { "Level Ups", true }, { "Weapons", true }, { "Abilities", true }, { "Critical Abilities", true }
                        };

                        availableOptions.Add(pool.Key, soraOptions);

                        break;
                    case "Equipment Abilities":
                        var equipmentOptions = new Dictionary<string, bool>
                        {
                            { "Weapons", true }, { "Accessories", true }, { "Armor", true }
                        };

                        availableOptions.Add(pool.Key, equipmentOptions);

                        break;
                    case "Data Battle Rewards":
                        var dataBattleOptions = new Dictionary<string, bool>
                        {
                            { "Rewards", true }
                        };

                        availableOptions.Add(pool.Key, dataBattleOptions);

                        break;
                    case "Moogle Workshop":
                        var moogleOptions = new Dictionary<string, bool>
                        {
                            { "Synthesis Items", true }, { "Photo Missions", true }, { "Weapon Upgrades", true },
                            // Weapon Upgrades
                            { "Weapon Upgrades: Kingdom Key", true }, { "Weapon Upgrades: Hero's Origin", true }, { "Weapon Upgrades: Shooting Star", true }, { "Weapon Upgrades: Favorite Deputy", true },
                            { "Weapon Upgrades: Ever After", true }, { "Weapon Upgrades: Happy Gear", true }, { "Weapon Upgrades: Crystal Snow", true }, { "Weapon Upgrades: Hunny Spout", true },
                            { "Weapon Upgrades: Wheel of Fate", true }, { "Weapon Upgrades: Nano Gear", true }, { "Weapon Upgrades: Starlight", true }, { "Weapon Upgrades: Grand Chef", true },
                            { "Weapon Upgrades: Classic Tone", true }, { "Weapon Upgrades: Ultima Weapon", true }, { "Weapon Upgrades: Elemental Encoder", true }, { "Weapon Upgrades: Oblivion", true },
                            { "Weapon Upgrades: Oathkeeper", true }
                        };

                        availableOptions.Add(pool.Key, moogleOptions);

                        break;
                    case "Fullcourse Abilities":
                        var fullcourseOptions = new Dictionary<string, bool>
                        {
                            { "Abilities", true }
                        };

                        availableOptions.Add(pool.Key, fullcourseOptions);

                        break;
                    case "Lucky Emblems":
                        var luckyEmblemOptions = new Dictionary<string, bool>
                        {
                            { "Lucky Emblems", true }
                        };

                        availableOptions.Add(pool.Key, luckyEmblemOptions);

                        break;
                    case "Flantastic Flans":
                        var flanOptions = new Dictionary<string, bool>
                        {
                            { "Flantastic Seven", true }
                        };

                        availableOptions.Add(pool.Key, flanOptions);

                        break;
                    case "Minigames":
                        var minigameOptions = new Dictionary<string, bool>
                        {
                            { "Minigames", true }
                        };

                        availableOptions.Add(pool.Key, minigameOptions);

                        break;
                    case "Battle Portals":
                        var battlePortalOptions = new Dictionary<string, bool>
                        {
                            { "Reports", true }, { "Rewards", true }
                        };

                        availableOptions.Add(pool.Key, battlePortalOptions);

                        break;

                    // Always On
                    case "Always On":
                        var replacedOptions = new Dictionary<string, bool>
                        {
                            { "Replaced Items", true }
                        };

                        availableOptions.Add(pool.Key, replacedOptions);

                        break;

                    default:
                        break;
                }
            }

            return availableOptions;
        }

        
        public void UpdateRandomizedItem(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions,
                                         DataTableEnum dataTableEnum, string category, string subCategory, string itemToChange,
                                         DataTableEnum swapDataTableEnum, string swapCategory, string swapSubCategory, string swapItemToChange)
        {
            if (dataTableEnum == DataTableEnum.ChrInit && subCategory == "Weapon")
            {
                var alteredSwapItemToChange = this.ConvertKeybladeWeaponToDefenseWeaponEnum(swapItemToChange);
                var alteredItemToChange = this.ConvertDefenseWeaponEnumToKeybladeWeapon(itemToChange);

                randomizedOptions[dataTableEnum][category][subCategory] = alteredSwapItemToChange;
                randomizedOptions[swapDataTableEnum][swapCategory][swapSubCategory] = alteredItemToChange;
            }
            else if (swapDataTableEnum == DataTableEnum.ChrInit && swapSubCategory == "Weapon")
            {
                var alteredSwapItemToChange = this.ConvertDefenseWeaponEnumToKeybladeWeapon(swapItemToChange);
                var alteredItemToChange = this.ConvertKeybladeWeaponToDefenseWeaponEnum(itemToChange);

                randomizedOptions[dataTableEnum][category][subCategory] = alteredSwapItemToChange;
                randomizedOptions[swapDataTableEnum][swapCategory][swapSubCategory] = alteredItemToChange;
            }
            else
            {
                randomizedOptions[dataTableEnum][category][subCategory] = swapItemToChange;
                randomizedOptions[swapDataTableEnum][swapCategory][swapSubCategory] = itemToChange;
            }
        }

        public static Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> GetDefaultOptions()
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot\DefaultKH3.json"));
            return JsonSerializer.Deserialize<Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>>>(streamReader.ReadToEnd());
        }


        public Option UpdateRandomizedItemWithDefault(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions,
                                                      DataTableEnum dataTableEnum, string category, string subCategory, string itemToChange)
        {
            var defaultOptions = GetDefaultOptions();
            var swapItemToFind = (string)defaultOptions[dataTableEnum][category][subCategory].Clone();

            var option = new Option();

            var isFound = false;
            foreach (var randomOption in randomizedOptions)
            {
                foreach (var randomSubOption in randomOption.Value)
                {
                    foreach (var (name, value) in randomSubOption.Value)
                    {
                        if (value == swapItemToFind)
                        {
                            option = new Option { Category = randomOption.Key, SubCategory = randomSubOption.Key, Name = name, Value = value };

                            randomizedOptions[dataTableEnum][category][subCategory] = value;
                            randomizedOptions[randomOption.Key][randomSubOption.Key][name] = itemToChange;

                            isFound = true;
                        }
                    }

                    if (isFound)
                        break;
                }

                if (isFound)
                    break;
            }

            return option;
        }

        public Option UpdateRandomizedItemWithNone(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions,
                                                   ref Dictionary<string, Dictionary<string, bool>> availableOptions,
                                                   DataTableEnum dataTableEnum, string category, string subCategory, string itemToChange)
        {
            var rng = new Random();

            var option = new Option();

            while (true) // Is there a way we can use a var instead of true?
            {
                var swapDataTable = randomizedOptions.ElementAt(rng.Next(0, randomizedOptions.Count()));
                var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                if (!availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                    continue;

                if (swapCategory.Value.Where(x => x.Value.Contains("NONE")).Count() > 0)
                {
                    var swapData = swapCategory.Value.Where(x => x.Value.Contains("NONE")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => x.Value.Contains("NONE")).Count()));

                    randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = itemToChange;
                    randomizedOptions[dataTableEnum][category][subCategory] = swapData.Value;

                    option = new Option { Category = swapDataTable.Key, SubCategory = swapCategory.Key, Name = swapData.Key, Value = swapData.Value };

                    break;
                }
            }


            return option;
        }

        // OBSELETE
        public void RandomizeItems(string seed, ref Dictionary<string, Dictionary<string, bool>> availableOptions,
                                   ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var hash = seed.StringToSeed();
            var rng = new Random((int)hash);

            // Use randomizedItems
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/DefaultKH3.json"));
            // Category > Id > Item > Value
            var defaultOptions = GetDefaultOptions();

            randomizedOptions = new();
            var swapList = new List<string>();

            if (string.IsNullOrEmpty(seed))
            {
                randomizedOptions = defaultOptions;
                return;
            }

            // Get all related items
            foreach (var option in availableOptions)
            {
                var firstPass = false;

                foreach (var subOption in option.Value)
                {
                    if (!subOption.Value || firstPass)
                        continue;

                    var dataTableEnum = this.GetDataTableEnumFromSelection(subOption.Key);

                    if (dataTableEnum == DataTableEnum.None)
                        dataTableEnum = this.ConvertDisplayStringToEnum(option.Key);

                    if (randomizedOptions.ContainsKey(dataTableEnum))
                        continue;

                    firstPass = (dataTableEnum == DataTableEnum.ChrInit || dataTableEnum == DataTableEnum.EquipItem ||
                                 dataTableEnum == DataTableEnum.VBonus || dataTableEnum == DataTableEnum.WeaponEnhance ||
                                 dataTableEnum == DataTableEnum.Event);

                    foreach (var (id, values) in defaultOptions[dataTableEnum])
                    {
                        if (id == "m_PlayerSora") { }

                        else if (id.Contains("GIVESORA") || !availableOptions[dataTableEnum.DataTableEnumToKey()][id.CategoryToKey(dataTableEnum)])
                            continue;

                        foreach (var value in values)
                        {
                            if (value.Key == "TypeB" || value.Key == "TypeC")// ||  value.Value.Contains("NONE"))
                                continue;

                            if (id == "m_PlayerSora" && !availableOptions[dataTableEnum.DataTableEnumToKey()][value.Key.CategoryToKey(dataTableEnum)])
                                continue;

                            swapList.Add(value.Value);
                        }
                    }
    
                    var copy = defaultOptions[dataTableEnum].Where(x => !x.Key.Contains("GIVESORA")).ToDictionary(x => x.Key, y => new Dictionary<string, string>(y.Value));

                    randomizedOptions.Add(dataTableEnum, copy);
                }
            }

            // Shuffle these around with our rng created from the seed
            swapList.Shuffle(rng);
            var queue = new Queue<string>(swapList);
            var tempQueue = new Queue<string>();

            // Put them back
            foreach (var option in availableOptions)
            {
                var firstPass = false;
                //if (!this.randomizedOptions.ContainsKey(option.Key))
                //    continue;

                foreach (var subOption in option.Value)
                {
                    if (!subOption.Value || firstPass)
                        continue;

                    var dataTableEnum = this.GetDataTableEnumFromSelection(subOption.Key);

                    if (dataTableEnum == DataTableEnum.None)
                        dataTableEnum = this.ConvertDisplayStringToEnum(option.Key);

                    firstPass = (dataTableEnum == DataTableEnum.ChrInit || dataTableEnum == DataTableEnum.EquipItem ||
                                 dataTableEnum == DataTableEnum.VBonus || dataTableEnum == DataTableEnum.WeaponEnhance ||
                                 dataTableEnum == DataTableEnum.Event);

                    foreach (var (id, values) in defaultOptions[dataTableEnum])
                    {
                        if (id == "m_PlayerSora") { }

                        else if (id.Contains("GIVESORA") || !availableOptions[dataTableEnum.DataTableEnumToKey()][id.CategoryToKey(dataTableEnum)])
                            continue;

                        foreach (var value in values)
                        {
                            if (value.Key == "TypeB" || value.Key == "TypeC")// || value.Value.Contains("NONE"))
                                continue;


                            if (id == "m_PlayerSora" && !availableOptions[dataTableEnum.DataTableEnumToKey()][value.Key.CategoryToKey(dataTableEnum)])
                                continue;

                            if (value.Key.Contains("Ability"))
                            {
                                var foundAvailableValue = false;

                                while (!foundAvailableValue && queue.Count > 0)
                                {
                                    var peekValue = queue.Peek();

                                    if (peekValue.Contains("ETresAbilityKind::"))
                                        foundAvailableValue = true;
                                    else
                                        tempQueue.Enqueue(queue.Dequeue());
                                }

                                if (queue.Count > 0)
                                {
                                    randomizedOptions[dataTableEnum][id][value.Key] = queue.Dequeue();
                                }
                                else
                                {
                                    // Empty TempQueue by enqueueing back into the queue with random lookups for abilities to swap the TempQueue with
                                    while(tempQueue.Count > 0)
                                    {
                                        // Find abilities by randomly selecting a randomizedOption to swap with
                                        var randCategory = randomizedOptions.ElementAt(rng.Next(0, randomizedOptions.Count));

                                        if (randCategory.Key != DataTableEnum.ChrInit && randCategory.Key != DataTableEnum.EquipItem && randCategory.Key != DataTableEnum.FullcourseAbility && randCategory.Key != DataTableEnum.WeaponEnhance)
                                        {
                                            var randData = randCategory.Value.ElementAt(rng.Next(0, randCategory.Value.Count));

                                            if (randData.Value.Count > 0)
                                            {
                                                var randValue = randData.Value.ElementAt(rng.Next(0, randData.Value.Count));

                                                if (!availableOptions[randCategory.Key.DataTableEnumToKey()][randData.Key.CategoryToKey(randCategory.Key)])
                                                    continue;

                                                if (id == "m_PlayerSora" && !availableOptions[randCategory.Key.DataTableEnumToKey()][randValue.Key.CategoryToKey(dataTableEnum)])
                                                    continue;

                                                if (randValue.Value.Contains("ETresAbilityKind::") && !(randValue.Key == "TypeB" || randValue.Key == "TypeC"))
                                                {
                                                    queue.Enqueue(randomizedOptions[randCategory.Key][randData.Key][randValue.Key]);

                                                    randomizedOptions[randCategory.Key][randData.Key][randValue.Key] = tempQueue.Dequeue();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (tempQueue.Count > 0)
                            {
                                randomizedOptions[dataTableEnum][id][value.Key] = tempQueue.Dequeue();
                            }
                            else
                            {
                                if (queue.Count > 0)
                                {
                                    randomizedOptions[dataTableEnum][id][value.Key] = queue.Dequeue();
                                }
                                else
                                {
                                    // Empty TempQueue by enqueueing back into the queue with random lookups for abilities to swap the TempQueue with
                                    while (tempQueue.Count > 0)
                                    {
                                        // Find abilities by randomly selecting a randomizedOption to swap with
                                        var randCategory = randomizedOptions.ElementAt(rng.Next(0, randomizedOptions.Count));

                                        if (randCategory.Key != DataTableEnum.ChrInit && randCategory.Key != DataTableEnum.EquipItem && randCategory.Key != DataTableEnum.FullcourseAbility && randCategory.Key != DataTableEnum.WeaponEnhance)
                                        {
                                            var randData = randCategory.Value.ElementAt(rng.Next(0, randCategory.Value.Count));

                                            if (randData.Value.Count > 0)
                                            {
                                                var randValue = randData.Value.ElementAt(rng.Next(0, randData.Value.Count));

                                                if (!availableOptions[randCategory.Key.DataTableEnumToKey()][randData.Key.CategoryToKey(randCategory.Key)])
                                                    continue;

                                                if (id == "m_PlayerSora" && !availableOptions[randCategory.Key.DataTableEnumToKey()][randValue.Key.CategoryToKey(dataTableEnum)])
                                                    continue;

                                                if (randValue.Value.Contains("ETresAbilityKind::") && !(randValue.Key == "TypeB" || randValue.Key == "TypeC"))
                                                {
                                                    queue.Enqueue(randomizedOptions[randCategory.Key][randData.Key][randValue.Key]);

                                                    randomizedOptions[randCategory.Key][randData.Key][randValue.Key] = tempQueue.Dequeue();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Account for VBonuses that need to be swapped for Proofs
            //if (availableOptions.ContainsKey("Bonuses") && availableOptions["Bonuses"]["VBonus"])
            //{
            //    var vbonusesToReplace = new string[2] { "Vbonus_077", "Vbonus_078" };

            //    foreach (var vbonus in vbonusesToReplace)
            //    {
            //        for (int i = 0; i < randomizedOptions[DataTableEnum.VBonus][vbonus].Count; ++i)
            //        {
            //            var bonus = randomizedOptions[DataTableEnum.VBonus][vbonus].ElementAt(i);
            //            if (bonus.Value == "KEY_ITEM12" || bonus.Value == "KEY_ITEM13" || bonus.Value == "KEY_ITEM14")
            //            {
            //                var swapDataTable = randomizedOptions.ElementAt(rng.Next(0, randomizedOptions.Count));
            //                var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));
            //                var swapData = swapCategory.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key][swapCategory.Key].Count));

            //                // Unsuccessful because we found another proof (somehow)
            //                if (swapData.Value == "KEY_ITEM12" || swapData.Value == "KEY_ITEM13" || swapData.Value == "KEY_ITEM14")
            //                {
            //                    --i;
            //                }
            //                else
            //                {
            //                    randomizedOptions[DataTableEnum.VBonus][vbonus][bonus.Key] = swapData.Value;
            //                    randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = bonus.Value;
            //                }
            //            }
            //        }
            //    }
            //}

            // Account for Events that have None in them
            if (availableOptions.ContainsKey("Events"))
            {
                this.RemoveNoneFromEvents(DataTableEnum.Event, rng, ref randomizedOptions, availableOptions);
            }

            // Account for Keyblade on ChrInit
            if (availableOptions.ContainsKey("Starting Stats") && availableOptions["Starting Stats"]["Weapons"])
            {
                var swapWeapon = "";
                var swapOther = randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["Weapon"];

                foreach (var (dataTableEnum, categories) in randomizedOptions)
                {
                    foreach (var (categoryKey, options) in categories)
                    {
                        if (!availableOptions[dataTableEnum.DataTableEnumToKey()][categoryKey.CategoryToKey(dataTableEnum)])
                            continue;

                        var temp = options.FirstOrDefault(x => x.Value.Contains("WEP_KEYBLADE") && x.Value != ("WEP_KEYBLADE_SO_018"));

                        if (!string.IsNullOrEmpty(temp.Value))
                        {
                            swapWeapon = temp.Value;
                            randomizedOptions[dataTableEnum][categoryKey][temp.Key] = swapOther;
                            break;
                        }

                    }

                    if (!string.IsNullOrEmpty(swapWeapon))
                        break;
                }

                var altWeapon = this.ConvertKeybladeWeaponToDefenseWeaponEnum(swapWeapon);
                randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["Weapon"] = altWeapon;
            }

            // Account for Luckymark Data
            if (availableOptions.ContainsKey("Lucky Emblems") && availableOptions["Lucky Emblems"]["Lucky Emblems"])
            {
                foreach (var luckyMark in randomizedOptions[DataTableEnum.LuckyMark])
                {
                    foreach (var subLuckyMark in luckyMark.Value)
                    {
                        if (!subLuckyMark.Value.Contains("::"))
                            continue;

                        while (true) // Is there a way we can use a var instead of true?
                        {
                            var tempOptions = randomizedOptions.Where(x => x.Key == DataTableEnum.EquipItem || x.Key == DataTableEnum.LevelUp || x.Key == DataTableEnum.VBonus);

                            var swapDataTable = tempOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                            var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                            if (swapCategory.Key == "EVENT_KEYBLADE_012" || swapCategory.Key == "EVENT_KEYBLADE_013" || !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                                continue;

                            if (swapCategory.Value.Where(x => !x.Value.Contains("::")).Count() > 0)
                            {
                                var swapData = swapCategory.Value.Where(x => !x.Value.Contains("::")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => !x.Value.Contains("::")).Count()));


                                if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                    continue;

                                randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = subLuckyMark.Value;
                                randomizedOptions[DataTableEnum.LuckyMark][luckyMark.Key][subLuckyMark.Key] = swapData.Value;

                                break;
                            }
                        }
                    }
                }
            }

            // Account for Synth Data (It will always needs items)
            if (availableOptions.ContainsKey("Synthesis Items") && availableOptions["Synthesis Items"]["Synthesis Items"])
            {
                foreach (var synthesisItem in randomizedOptions[DataTableEnum.SynthesisItem])
                {
                    foreach (var subSynthesisItem in synthesisItem.Value)
                    {
                        if (!subSynthesisItem.Value.Contains("::"))
                            continue;

                        while (true) // Is there a way we can use a var instead of true?
                        {
                            var tempOptions = randomizedOptions.Where(x => x.Key != DataTableEnum.SynthesisItem && x.Key != DataTableEnum.FullcourseAbility && x.Key != DataTableEnum.ChrInit && x.Key != DataTableEnum.LuckyMark);

                            var swapDataTable = tempOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                            var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                            if (swapCategory.Key == "EVENT_KEYBLADE_012" || swapCategory.Key == "EVENT_KEYBLADE_013" || !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                                continue;

                            if (swapCategory.Value.Where(x => !x.Value.Contains("::")).Count() > 0)
                            {
                                var swapData = swapCategory.Value.Where(x => !x.Value.Contains("::")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => !x.Value.Contains("::")).Count()));

                                if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                    continue;
                                else if ((swapDataTable.Key == DataTableEnum.TreasureBT || swapDataTable.Key == DataTableEnum.TreasureBX || swapDataTable.Key == DataTableEnum.TreasureCA || swapDataTable.Key == DataTableEnum.TreasureEW ||
                                         swapDataTable.Key == DataTableEnum.TreasureFZ || swapDataTable.Key == DataTableEnum.TreasureHE || swapDataTable.Key == DataTableEnum.TreasureKG || swapDataTable.Key == DataTableEnum.TreasureMI ||
                                         swapDataTable.Key == DataTableEnum.TreasureRA || swapDataTable.Key == DataTableEnum.TreasureTS || swapDataTable.Key == DataTableEnum.TreasureTT) && subSynthesisItem.Value.ToLower().Contains("none"))
                                    continue;

                                randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = subSynthesisItem.Value;
                                randomizedOptions[DataTableEnum.SynthesisItem][synthesisItem.Key][subSynthesisItem.Key] = swapData.Value;

                                break;
                            }
                        }
                    }
                }
            }

            // Account for Pole Spin not being locked behind Frozen

            // List of places it can't be
            var disallowedPlaces = new List<string> { //"FZ_", // Disallow it in Frozen's treasures
                                                      //"21", // Disallow after level 20
                                                      //"20", // Disallow after lucky emblem milestone 15
                                                      //"IW_", // Disallow on weapon enhances
                                                      //"I0", // Disallow on equip items
                                                      "EVENT_007", // Disallow on Herc's Gold Statue Return
                                                      "TresUIMobilePortalDataAsset", // Disallow on all CK game reward
                                                      "EVENT_KEYBLADE_007", // Disallow on Keyblade event for Frozen
                                                      "EVENT_REPORT_009a", "EVENT_REPORT_009b", // Disallow on Reports in Arendelle
                                                      "Vbonus_041", "Vbonus_042", "Vbonus_043", "Vbonus_044", "Vbonus_045",
                                                      "Vbonus_046", "Vbonus_047", "Vbonus_048", "Vbonus_049", "Vbonus_050",
                                                      "VBonus_Minigame003", "VBonus_Minigame004", "VBonus_Minigame011" // Disallow on VBonuses related to Frozen
                                                    };
            foreach (var category in randomizedOptions)
            {
                foreach (var subCategory in category.Value)
                {
                    foreach (var option in subCategory.Value)
                    {
                        if (option.Value.Contains("POLE_SPIN"))
                        {
                            var swapLogic = this.IsPoleSpinDisallowed(category.Key, subCategory.Key);

                            while (swapLogic) // Is there a way we can use a var instead of true?
                            {
                                var tempOptions = randomizedOptions.Where(x => x.Key == DataTableEnum.EquipItem || x.Key == DataTableEnum.WeaponEnhance || x.Key == DataTableEnum.TreasureFZ);

                                var swapDataTable = randomizedOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                                var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                                if (swapCategory.Key == "EVENT_KEYBLADE_012" || swapCategory.Key == "EVENT_KEYBLADE_013" || !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                                    continue;

                                if (!this.IsPoleSpinDisallowed(swapDataTable.Key, swapCategory.Key) && swapCategory.Value.Where(x => x.Value.Contains("ETresAbilityKind::")).Count() > 0)
                                {
                                    var swapData = swapCategory.Value.Where(x => x.Value.Contains("ETresAbilityKind::")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => x.Value.Contains("ETresAbilityKind::")).Count()));

                                    if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                        continue;

                                    randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = option.Value;
                                    randomizedOptions[category.Key][subCategory.Key][option.Key] = swapData.Value;

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // Account for Treasures that have None in them
            if (availableOptions.ContainsKey("Treasures"))
            {
                this.RemoveNoneFromTreasure(DataTableEnum.TreasureBT, rng, ref randomizedOptions, availableOptions);
                this.RemoveNoneFromTreasure(DataTableEnum.TreasureBX, rng, ref randomizedOptions, availableOptions);
                this.RemoveNoneFromTreasure(DataTableEnum.TreasureCA, rng, ref randomizedOptions, availableOptions);
                this.RemoveNoneFromTreasure(DataTableEnum.TreasureEW, rng, ref randomizedOptions, availableOptions);
                this.RemoveNoneFromTreasure(DataTableEnum.TreasureFZ, rng, ref randomizedOptions, availableOptions);
                this.RemoveNoneFromTreasure(DataTableEnum.TreasureHE, rng, ref randomizedOptions, availableOptions);
                this.RemoveNoneFromTreasure(DataTableEnum.TreasureKG, rng, ref randomizedOptions, availableOptions);
                this.RemoveNoneFromTreasure(DataTableEnum.TreasureMI, rng, ref randomizedOptions, availableOptions);
                this.RemoveNoneFromTreasure(DataTableEnum.TreasureRA, rng, ref randomizedOptions, availableOptions);
                this.RemoveNoneFromTreasure(DataTableEnum.TreasureTS, rng, ref randomizedOptions, availableOptions);
                this.RemoveNoneFromTreasure(DataTableEnum.TreasureTT, rng, ref randomizedOptions, availableOptions);
            }

            // Account for Levelup Data
            if (availableOptions.ContainsKey("Level Ups") && availableOptions["Level Ups"]["Levels"])
            {
                foreach (var level in defaultOptions[DataTableEnum.LevelUp])
                {
                    var typeA = level.Value["TypeA"];
                    var randValue = randomizedOptions[DataTableEnum.LevelUp][level.Key]["TypeA"];

                    if (typeA.Contains("NONE"))
                    {
                        randomizedOptions[DataTableEnum.LevelUp][level.Key]["TypeB"] = randValue;
                        randomizedOptions[DataTableEnum.LevelUp][level.Key]["TypeC"] = randValue;

                        continue;
                    }

                    var levelTypeB = "";
                    var levelTypeC = "";

                    foreach (var subLevel in defaultOptions[DataTableEnum.LevelUp])
                    {
                        if (!string.IsNullOrEmpty(levelTypeB) && !string.IsNullOrEmpty(levelTypeC))
                            break;

                        if (typeA == subLevel.Value["TypeB"])
                            levelTypeB = subLevel.Key;

                        if (typeA == subLevel.Value["TypeC"])
                            levelTypeC = subLevel.Key;
                    }

                    if (!string.IsNullOrEmpty(levelTypeB))
                        randomizedOptions[DataTableEnum.LevelUp][levelTypeB]["TypeB"] = randValue;

                    if (!string.IsNullOrEmpty(levelTypeC))
                        randomizedOptions[DataTableEnum.LevelUp][levelTypeC]["TypeC"] = randValue;
                }
            }

            // Add back the default values that were not included
            foreach (var option in availableOptions)
            {
                foreach (var subOption in option.Value.Where(x => !x.Value))
                {
                    var dataTableEnum = this.GetDataTableEnumFromSelection(subOption.Key);

                    if (dataTableEnum == DataTableEnum.None)
                        dataTableEnum = this.ConvertDisplayStringToEnum(option.Key);

                    if (!randomizedOptions.ContainsKey(dataTableEnum))
                        randomizedOptions.Add(dataTableEnum, defaultOptions[dataTableEnum]);
                }
            }
        }

        public Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> Process(string seed, Dictionary<string, RandomizeOptionEnum> pools, Dictionary<string, bool> exceptions, bool canUseNone = true)
        {
            var randomizedOptions = new Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>>();

            // Read in Default KH3 Options
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/DefaultKH3.json"));
            var defaultOptions = GetDefaultOptions();

            if (string.IsNullOrEmpty(seed))
                return defaultOptions;

            var hash = seed.StringToSeed();
            var random = new Random((int)hash);

            var randomizePools = pools.Where(x => x.Value == RandomizeOptionEnum.Randomize).ToDictionary(x => x.Key, y => y.Value);
            var replacePools = pools.Where(x => x.Value == RandomizeOptionEnum.Replace).ToDictionary(x => x.Key, y => y.Value);

            if (randomizePools.Count == 0)
                return defaultOptions;

            var copiedOptions = this.CopyOptions(defaultOptions, randomizePools, canUseNone);

            // Randomize all checks within the randomize pools
            this.RandomizeOptions(randomizePools, defaultOptions, ref randomizedOptions, ref copiedOptions, random, canUseNone);

            // Validate randomized options
            this.ValidateOptions(ref randomizedOptions, randomizePools, random, canUseNone);

            // Move important items from the replace pools to the randomize pools
            this.ReplaceOptions(replacePools, randomizePools, defaultOptions, ref randomizedOptions, random, canUseNone);

            // Process exceptions like start with default abilities, etc.
            this.ProcessExceptions(ref randomizedOptions, randomizePools, exceptions, random, canUseNone);

            // Add some clean-up after the randomization
            this.CleanUpOptions(ref randomizedOptions, ref copiedOptions, defaultOptions, randomizePools, random, canUseNone);

            //foreach (var (category, categoryOptions) in randomizedOptions.Where(x => x.Value.Any(y => y.Value.Any(z => z.Value.ValueIdToDisplay().Contains("Proof")))))
            //{
            //    foreach (var (subCategory, subCategoryOptions) in categoryOptions.Where(y => y.Value.Any(z => z.Value.ValueIdToDisplay().Contains("Proof"))))
            //    {
            //        foreach (var (name, value) in subCategoryOptions.Where(z => z.Value.ValueIdToDisplay().Contains("Proof")))
            //        {
            //            Console.WriteLine();
            //        }
            //    }
            //}

            return randomizedOptions;
        }

        public void ProcessExceptions(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, RandomizeOptionEnum> randomizePools, Dictionary<string, bool> exceptions, Random random, bool canUseNone = true)
        {
            if (exceptions["Default Abilities"] && randomizedOptions.ContainsKey(DataTableEnum.ChrInit))
            {
                foreach (var (name, value) in randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"])
                {
                    if (name == "Weapon" || name.Contains("Crit"))
                        continue;

                    var defaultAbility = this.GetDefaultAbility(name);

                    var abilityCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => !y.Key.Contains("GIVESORA") && y.Value.Any(z => z.Value == defaultAbility))).Key;
                    var abilitySubCategory = randomizedOptions[abilityCategory].FirstOrDefault(y => !y.Key.Contains("GIVESORA") && y.Value.Any(z => z.Value == defaultAbility)).Key;
                    var ability = randomizedOptions[abilityCategory][abilitySubCategory].FirstOrDefault(z => z.Value == defaultAbility);

                    // Swap these options
                    randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"][name] = ability.Value;

                    // Extra precaution to verify we swap this into a correct spot
                    var swapOption = new Option { Category = abilityCategory, SubCategory = abilitySubCategory, Name = ability.Key, Value = value };

                    var swapCategoryNeeded = this.RetrieveCategoryNeeded(abilityCategory, ability.Key);

                    this.SwapRandomOption(ref randomizedOptions, randomizePools, random, swapCategoryNeeded, swapOption, canUseNone, false);
                }
            }

            if (exceptions["Default Critical Abilities"] && randomizedOptions.ContainsKey(DataTableEnum.ChrInit))
            {
                foreach (var (name, value) in randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"])
                {
                    if (name == "Weapon" || !name.Contains("Crit"))
                        continue;

                    var defaultAbility = this.GetDefaultAbility(name);

                    var abilityCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => !y.Key.Contains("GIVESORA") && y.Value.Any(z => z.Value == defaultAbility))).Key;
                    var abilitySubCategory = randomizedOptions[abilityCategory].FirstOrDefault(y => !y.Key.Contains("GIVESORA") && y.Value.Any(z => z.Value == defaultAbility)).Key;
                    var ability = randomizedOptions[abilityCategory][abilitySubCategory].FirstOrDefault(z => z.Value == defaultAbility);

                    // Swap these options
                    randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"][name] = ability.Value;

                    // Extra precaution to verify we swap this into a correct spot
                    var swapOption = new Option { Category = abilityCategory, SubCategory = abilitySubCategory, Name = ability.Key, Value = value };

                    var swapCategoryNeeded = this.RetrieveCategoryNeeded(abilityCategory, ability.Key);

                    this.SwapRandomOption(ref randomizedOptions, randomizePools, random, swapCategoryNeeded, swapOption, canUseNone, false);
                }
            }

            if (exceptions["Early Critical Abilities"] && randomizedOptions.ContainsKey(DataTableEnum.VBonus))
            {
                var airSlidesSeen = new List<string>();

                foreach (var (subCategory, subCategoryOptions) in randomizedOptions[DataTableEnum.VBonus])
                {
                    if (!this.VBonusCriticalAbilities.Contains(subCategory))
                        continue;

                    var results = this.GetDefaultAbilitiesBonusesForVBonus(subCategory);

                    foreach (var result in results)
                    {
                        var abilityBonusCategory = DataTableEnum.None;
                        var abilityBonusSubCategory = string.Empty;
                        var abilityBonus = new KeyValuePair<string, string>();

                        if (result.Value.Contains("AIRSLIDE"))
                        {
                            abilityBonusCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => !y.Key.Contains("GIVESORA") && y.Key != subCategory && !airSlidesSeen.Contains(y.Key) && y.Value.Any(z => z.Key != "EquipAbility2" && z.Value == result.Value))).Key;
                            
                            if (abilityBonusCategory != DataTableEnum.None)
                            {
                                abilityBonusSubCategory = randomizedOptions[abilityBonusCategory].FirstOrDefault(y => !y.Key.Contains("GIVESORA") && y.Key != subCategory && y.Value.Any(z => z.Key != "EquipAbility2" && z.Value == result.Value)).Key;
                                abilityBonus = randomizedOptions[abilityBonusCategory][abilityBonusSubCategory].FirstOrDefault(z => z.Key != "EquipAbility2" && z.Value == result.Value);

                                airSlidesSeen.Add(subCategory);
                            }
                        }
                        else
                        {
                            abilityBonusCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => !y.Key.Contains("GIVESORA") && y.Key != subCategory && y.Value.Any(z => z.Value == result.Value))).Key;

                            if (abilityBonusCategory != DataTableEnum.None)
                            {
                                abilityBonusSubCategory = randomizedOptions[abilityBonusCategory].FirstOrDefault(y => !y.Key.Contains("GIVESORA") && y.Key != subCategory && y.Value.Any(z => z.Value == result.Value)).Key;
                                abilityBonus = randomizedOptions[abilityBonusCategory][abilityBonusSubCategory].FirstOrDefault(z => z.Value == result.Value);
                            }
                        }

                        if (abilityBonusCategory != DataTableEnum.None)
                        {

                            // Swap these options
                            var temp = randomizedOptions[DataTableEnum.VBonus][subCategory][result.Key];
                            randomizedOptions[DataTableEnum.VBonus][subCategory][result.Key] = abilityBonus.Value;

                            // Extra precaution to verify we swap this into a correct spot
                            var swapOption = new Option { Category = abilityBonusCategory, SubCategory = abilityBonusSubCategory, Name = abilityBonus.Key, Value = temp };

                            var swapCategoryNeeded = this.RetrieveCategoryNeeded(abilityBonusCategory, abilityBonus.Key);

                            this.SwapRandomOption(ref randomizedOptions, randomizePools, random, swapCategoryNeeded, swapOption, canUseNone, false);
                        }
                    }

                    // Once we hit this, we've reached the end of our list
                    if (subCategory == "Vbonus_069")
                        break;
                }
            }
        }

        public Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> GetOptionsForPool(string pool, Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> defaultOptions)
        {
            var options = new Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>>();

            List<string> events;
            List<string> vbonuses;

            switch (pool)
            {
                case "Olympus":
                    // Treasures
                    options.Add(DataTableEnum.TreasureHE, defaultOptions[DataTableEnum.TreasureHE]);

                    // Events
                    events = new List<string> { "EVENT_01", "EVENT_02", "EVENT_03", "EVENT_04", "EVENT_05", "EVENT_06", "EVENT_07", "EVENT_KEYBLADE_001" };
                    var olympusEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, olympusEvents);

                    // VBonuses
                    vbonuses = new List<string> { "Vbonus_001", "Vbonus_002", "Vbonus_005", "Vbonus_006", "Vbonus_007", "Vbonus_008", "Vbonus_009", "Vbonus_010", "Vbonus_011", "Vbonus_013", "Vbonus_082" };
                    var olympusVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, olympusVbonuses);

                    break;
                case "Twilight Town":
                    // Treasures
                    options.Add(DataTableEnum.TreasureTT, defaultOptions[DataTableEnum.TreasureTT]);

                    // Events
                    events = new List<string> { "EVENT_KEYBLADE_002", "EVENT_KEYBLADE_010", "EVENT_CKGAME_001" };
                    var twilightTownEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, twilightTownEvents);

                    // VBonuses
                    vbonuses = new List<string> { "Vbonus_014", "Vbonus_015", "Vbonus_016" };
                    var twilightTownVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, twilightTownVbonuses);

                    break;
                case "Toy Box":
                    // Treasures
                    options.Add(DataTableEnum.TreasureTS, defaultOptions[DataTableEnum.TreasureTS]);

                    // Events
                    events = new List<string> { "EVENT_KEYBLADE_003", "EVENT_HEARTBINDER_002" };
                    var toyBoxEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, toyBoxEvents);

                    // VBonuses
                    vbonuses = new List<string> { "Vbonus_017", "Vbonus_018", "Vbonus_019", "Vbonus_020", "Vbonus_021", "Vbonus_022", "Vbonus_023" };
                    var toyBoxVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, toyBoxVbonuses);

                    break;
                case "Kingdom of Corona":
                    // Treasures
                    options.Add(DataTableEnum.TreasureRA, defaultOptions[DataTableEnum.TreasureRA]);

                    // Events
                    events = new List<string> { "EVENT_KEYBLADE_004" };
                    var kingdomOfCoronaEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, kingdomOfCoronaEvents);

                    // VBonuses
                    vbonuses = new List<string> { "Vbonus_024", "Vbonus_025", "Vbonus_026", "Vbonus_027", "Vbonus_028", "Vbonus_029", "Vbonus_030" };
                    var kingdomOfCoronaVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, kingdomOfCoronaVbonuses);

                    break;
                case "Monstropolis":
                    // Treasures
                    options.Add(DataTableEnum.TreasureMI, defaultOptions[DataTableEnum.TreasureMI]);

                    // Events
                    events = new List<string> { "EVENT_008", "EVENT_KEYBLADE_005", "EVENT_HEARTBINDER_003" };
                    var monstropolisEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, monstropolisEvents);

                    // VBonuses
                    vbonuses = new List<string> { "Vbonus_032", "Vbonus_033", "Vbonus_034", "Vbonus_035", "Vbonus_036", "Vbonus_037", "Vbonus_038", "Vbonus_039", "Vbonus_040" };
                    var monstropolisVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, monstropolisVbonuses);

                    break;
                case "100 Acre Wood":
                    // Events
                    events = new List<string> { "EVENT_KEYBLADE_006" };
                    var poohEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, poohEvents);

                    break;
                case "Arendelle":
                    // Treasures
                    options.Add(DataTableEnum.TreasureFZ, defaultOptions[DataTableEnum.TreasureFZ]);

                    // Events
                    events = new List<string> { "EVENT_KEYBLADE_007" };
                    var arendelleEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, arendelleEvents);

                    // VBonuses
                    vbonuses = new List<string> { "Vbonus_041", "Vbonus_042", "Vbonus_043", "Vbonus_044", "Vbonus_045", "Vbonus_047", "Vbonus_048", "Vbonus_049", "Vbonus_050" };
                    var arendelleVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, arendelleVbonuses);

                    break;
                case "San Fransokyo":
                    // Treasures
                    options.Add(DataTableEnum.TreasureBX, defaultOptions[DataTableEnum.TreasureBX]);

                    // Events
                    events = new List<string> { "EVENT_009", "EVENT_KEYBLADE_009", "EVENT_HEARTBINDER_004", "EVENT_KEYITEM_002" };
                    var sanFransokyoEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, sanFransokyoEvents);

                    // VBonuses
                    vbonuses = new List<string> { "Vbonus_051", "Vbonus_052", "Vbonus_053", "Vbonus_054", "Vbonus_055", "Vbonus_056", "Vbonus_057" };
                    var sanFransokyoVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, sanFransokyoVbonuses);

                    break;
                case "The Caribbean":
                    // Treasures
                    options.Add(DataTableEnum.TreasureCA, defaultOptions[DataTableEnum.TreasureCA]);

                    // Events
                    events = new List<string> { "EVENT_KEYBLADE_008" };
                    var caribbeanEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, caribbeanEvents);

                    // VBonuses
                    vbonuses = new List<string> { "Vbonus_058", "Vbonus_059", "Vbonus_060", "Vbonus_061", "Vbonus_062", "Vbonus_063", "Vbonus_065", "Vbonus_066" };
                    var caribbeanVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, caribbeanVbonuses);

                    break;
                case "Keyblade Graveyard":
                    // Treasures
                    options.Add(DataTableEnum.TreasureKG, defaultOptions[DataTableEnum.TreasureKG]);
                    options.Add(DataTableEnum.TreasureEW, defaultOptions[DataTableEnum.TreasureEW]);

                    // Events
                    events = new List<string> { "EVENT_KEYBLADE_011" };
                    var kgEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, kgEvents);

                    // VBonuses
                    vbonuses = new List<string> { "Vbonus_068", "Vbonus_069", "Vbonus_070", "Vbonus_071", "Vbonus_072", "Vbonus_073", "Vbonus_074", "Vbonus_075", "Vbonus_076", "Vbonus_083", "Vbonus_084" };
                    var kgVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, kgVbonuses);

                    break;
                case "Re:Mind":
                    // Treasures
                    options.Add(DataTableEnum.TreasureBT, defaultOptions[DataTableEnum.TreasureBT]);

                    // VBonuses
                    vbonuses = new List<string> { "VBonus_DLC_001", "VBonus_DLC_002", "VBonus_DLC_003", "VBonus_DLC_004", "VBonus_DLC_005", "VBonus_DLC_006", "VBonus_DLC_007", "VBonus_DLC_008",
                                                  "VBonus_DLC_009", "VBonus_DLC_010", "VBonus_DLC_011", "VBonus_DLC_012", "VBonus_DLC_013", "VBonus_DLC_014", "VBonus_DLC_015" };
                    var remindVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, remindVbonuses);

                    break;
                case "Dark World":
                    // VBonuses
                    vbonuses = new List<string> { "Vbonus_067" };
                    var darkWorldVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, darkWorldVbonuses);

                    break;
                case "Unreality":
                    // Events
                    events = new List<string> { "EVENT_KEYITEM_005", "EVENT_YOZORA_001" };
                    var unrealityEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, unrealityEvents);

                    break;

                case "Sora":
                    // Level Ups
                    options.Add(DataTableEnum.LevelUp, defaultOptions[DataTableEnum.LevelUp]);

                    // Starting Weapon + Abilities
                    options.Add(DataTableEnum.ChrInit, defaultOptions[DataTableEnum.ChrInit]);

                    break;
                case "Equipment Abilities":
                    options.Add(DataTableEnum.EquipItem, defaultOptions[DataTableEnum.EquipItem]);

                    break;
                case "Data Battle Rewards":
                    // Events
                    events = new List<string> { "EVENT_DATAB_001", "EVENT_DATAB_002", "EVENT_DATAB_003", "EVENT_DATAB_004", "EVENT_DATAB_005", "EVENT_DATAB_006", "EVENT_DATAB_007",
                                                "EVENT_DATAB_008", "EVENT_DATAB_009", "EVENT_DATAB_010", "EVENT_DATAB_011", "EVENT_DATAB_012", "EVENT_DATAB_013" };
                    var dataBattleEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, dataBattleEvents);

                    break;
                case "Moogle Workshop":
                    // Synthesis Items + Photos
                    options.Add(DataTableEnum.SynthesisItem, defaultOptions[DataTableEnum.SynthesisItem]);

                    // Weapon Upgrades
                    options.Add(DataTableEnum.WeaponEnhance, defaultOptions[DataTableEnum.WeaponEnhance]);

                    break;
                case "Fullcourse Abilities":
                    options.Add(DataTableEnum.FullcourseAbility, defaultOptions[DataTableEnum.FullcourseAbility]);

                    break;
                case "Lucky Emblems":
                    options.Add(DataTableEnum.LuckyMark, defaultOptions[DataTableEnum.LuckyMark]);

                    break;
                case "Flantastic Flans":
                    // Events
                    vbonuses = new List<string> { "VBonus_Minigame007", "VBonus_Minigame008", "VBonus_Minigame009", "VBonus_Minigame010", "VBonus_Minigame011", "VBonus_Minigame012", "VBonus_Minigame013" };
                    var flanVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, flanVbonuses);

                    break;
                case "Minigames":
                    // Events
                    vbonuses = new List<string> { "VBonus_Minigame001", "VBonus_Minigame002", "VBonus_Minigame003", "VBonus_Minigame004", "VBonus_Minigame005", "VBonus_Minigame006" };
                    var minigameVbonuses = defaultOptions[DataTableEnum.VBonus].Where(x => vbonuses.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.VBonus, minigameVbonuses);

                    break;
                case "Battle Portals":
                    // Events
                    events = new List<string> { "EVENT_REPORT_001a", "EVENT_REPORT_001b", "EVENT_REPORT_002a", "EVENT_REPORT_002b", "EVENT_REPORT_003a", "EVENT_REPORT_003b", "EVENT_REPORT_004a", "EVENT_REPORT_004b",
                                                "EVENT_REPORT_005a", "EVENT_REPORT_005b", "EVENT_REPORT_006a", "EVENT_REPORT_006b", "EVENT_REPORT_007a", "EVENT_REPORT_007b", "EVENT_REPORT_008a", "EVENT_REPORT_008b",
                                                "EVENT_REPORT_009a", "EVENT_REPORT_009b", "EVENT_REPORT_010a", "EVENT_REPORT_010b", "EVENT_REPORT_011a", "EVENT_REPORT_011b", "EVENT_REPORT_012a", "EVENT_REPORT_012b",
                                                "EVENT_REPORT_013a", "EVENT_REPORT_013b", "EVENT_REPORT_014" };
                    var portalsEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, portalsEvents);

                    break;

                case "Always On":
                    // Events
                    events = new List<string> { "EVENT_KEYBLADE_012", "EVENT_KEYBLADE_013", "EVENT_HEARTBINDER_001", "EVENT_KEYITEM_001", "EVENT_KEYITEM_003", "EVENT_KEYITEM_004" };
                    var alwaysOnEvents = defaultOptions[DataTableEnum.Event].Where(x => events.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                    options.Add(DataTableEnum.Event, alwaysOnEvents);

                    break;

                default:
                    break;
            }

            return options;
        }

        public string GetPoolFromOption(DataTableEnum dataTableEnum, string subCategory)
        {
            var poolName = "";

            switch (dataTableEnum)
            {
                case DataTableEnum.TreasureHE:
                    poolName = "Olympus";
                    break;
                case DataTableEnum.TreasureTT:
                    poolName = "Twilight Town";
                    break;
                case DataTableEnum.TreasureTS:
                    poolName = "Toy Box";
                    break;
                case DataTableEnum.TreasureRA:
                    poolName = "Kingdom of Corona";
                    break;
                case DataTableEnum.TreasureMI:
                    poolName = "Monstropolis";
                    break;
                case DataTableEnum.TreasureFZ:
                    poolName = "Arendelle";
                    break;
                case DataTableEnum.TreasureBX:
                    poolName = "San Fransokyo";
                    break;
                case DataTableEnum.TreasureCA:
                    poolName = "The Caribbean";
                    break;
                case DataTableEnum.TreasureKG:
                case DataTableEnum.TreasureEW:
                    poolName = "Keyblade Graveyard";
                    break;
                case DataTableEnum.TreasureBT:
                    poolName = "Re:Mind";
                    break;
                case DataTableEnum.LevelUp:
                case DataTableEnum.ChrInit:
                    poolName = "Sora";
                    break;
                case DataTableEnum.EquipItem:
                    poolName = "Equipment Abilities";
                    break;
                case DataTableEnum.SynthesisItem:
                case DataTableEnum.WeaponEnhance:
                    poolName = "Moogle Workshop";
                    break;
                case DataTableEnum.FullcourseAbility:
                    poolName = "Fullcourse Abilities";
                    break;
                case DataTableEnum.LuckyMark:
                    poolName = "Lucky Emblems";
                    break;
                case DataTableEnum.Event:
                    switch (subCategory)
                    {
                        case "EVENT_01":
                        case "EVENT_02":
                        case "EVENT_03":
                        case "EVENT_04":
                        case "EVENT_05":
                        case "EVENT_06":
                        case "EVENT_07":
                        case "EVENT_KEYBLADE_001":
                            poolName = "Olympus";
                            break;
                        case "EVENT_KEYBLADE_002":
                        case "EVENT_KEYBLADE_010":
                        case "EVENT_CKGAME_001":
                            poolName = "Twilight Town";
                            break;
                        case "EVENT_KEYBLADE_003":
                        case "EVENT_HEARTBINDER_002":
                            poolName = "Toy Box";
                            break;
                        case "EVENT_KEYBLADE_004":
                            poolName = "Kingdom of Corona";
                            break;
                        case "EVENT_008":
                        case "EVENT_KEYBLADE_005":
                        case "EVENT_HEARTBINDER_003":
                            poolName = "Monstropolis";
                            break;
                        case "EVENT_KEYBLADE_006":
                            poolName = "100 Acre Wood";
                            break;
                        case "EVENT_KEYBLADE_007":
                            poolName = "Arendelle";
                            break;
                        case "EVENT_009":
                        case "EVENT_KEYBLADE_009":
                        case "EVENT_HEARTBINDER_004":
                        case "EVENT_KEYITEM_002":
                            poolName = "San Fransokyo";
                            break;
                        case "EVENT_KEYBLADE_008":
                            poolName = "The Caribbean";
                            break;
                        case "EVENT_KEYBLADE_011":
                            poolName = "Keyblade Graveyard";
                            break;
                        case "EVENT_KEYITEM_005":
                        case "EVENT_YOZORA_001":
                            poolName = "Unreality";
                            break;
                        case "EVENT_DATAB_001":
                        case "EVENT_DATAB_002":
                        case "EVENT_DATAB_003":
                        case "EVENT_DATAB_004":
                        case "EVENT_DATAB_005":
                        case "EVENT_DATAB_006":
                        case "EVENT_DATAB_007":
                        case "EVENT_DATAB_008":
                        case "EVENT_DATAB_009":
                        case "EVENT_DATAB_010":
                        case "EVENT_DATAB_011":
                        case "EVENT_DATAB_012":
                        case "EVENT_DATAB_013":
                            poolName = "Data Battle Rewards";
                            break;
                        case "EVENT_REPORT_001a":
                        case "EVENT_REPORT_001b":
                        case "EVENT_REPORT_002a":
                        case "EVENT_REPORT_002b":
                        case "EVENT_REPORT_003a":
                        case "EVENT_REPORT_003b":
                        case "EVENT_REPORT_004a":
                        case "EVENT_REPORT_004b":
                        case "EVENT_REPORT_005a":
                        case "EVENT_REPORT_005b":
                        case "EVENT_REPORT_006a":
                        case "EVENT_REPORT_006b":
                        case "EVENT_REPORT_007a":
                        case "EVENT_REPORT_007b":
                        case "EVENT_REPORT_008a":
                        case "EVENT_REPORT_008b":
                        case "EVENT_REPORT_009a":
                        case "EVENT_REPORT_009b":
                        case "EVENT_REPORT_010a":
                        case "EVENT_REPORT_010b":
                        case "EVENT_REPORT_011a":
                        case "EVENT_REPORT_011b":
                        case "EVENT_REPORT_012a":
                        case "EVENT_REPORT_012b":
                        case "EVENT_REPORT_013a":
                        case "EVENT_REPORT_013b":
                        case "EVENT_REPORT_014":
                            poolName = "Battle Portals";
                            break;
                        case "EVENT_KEYBLADE_012":
                        case "EVENT_KEYBLADE_013":
                        case "EVENT_HEARTBINDER_001":
                        case "EVENT_KEYITEM_001":
                        case "EVENT_KEYITEM_003":
                        case "EVENT_KEYITEM_004":
                            poolName = "Always On";
                            break;
                        default:
                            break;
                    }

                    break;
                case DataTableEnum.VBonus:
                    switch (subCategory)
                    {
                        case "Vbonus_001":
                        case "Vbonus_002":
                        case "Vbonus_005":
                        case "Vbonus_006":
                        case "Vbonus_007":
                        case "Vbonus_008":
                        case "Vbonus_009":
                        case "Vbonus_010":
                        case "Vbonus_011":
                        case "Vbonus_013":
                        case "Vbonus_082":
                            poolName = "Olympus";
                            break;
                        case "Vbonus_014":
                        case "Vbonus_015":
                        case "Vbonus_016":
                            poolName = "Twilight Town";
                            break;
                        case "Vbonus_017":
                        case "Vbonus_018":
                        case "Vbonus_019":
                        case "Vbonus_020":
                        case "Vbonus_021":
                        case "Vbonus_022":
                        case "Vbonus_023":
                            poolName = "Toy Box";
                            break;
                        case "Vbonus_024":
                        case "Vbonus_025":
                        case "Vbonus_026":
                        case "Vbonus_027":
                        case "Vbonus_028":
                        case "Vbonus_029":
                        case "Vbonus_030":
                            poolName = "Kingdom of Corona";
                            break;
                        case "Vbonus_032":
                        case "Vbonus_033":
                        case "Vbonus_034":
                        case "Vbonus_035":
                        case "Vbonus_036":
                        case "Vbonus_037":
                        case "Vbonus_038":
                        case "Vbonus_039":
                        case "Vbonus_040":
                            poolName = "Monstropolis";
                            break;
                        case "Vbonus_041":
                        case "Vbonus_042":
                        case "Vbonus_043":
                        case "Vbonus_044":
                        case "Vbonus_045":
                        case "Vbonus_047":
                        case "Vbonus_048":
                        case "Vbonus_049":
                        case "Vbonus_050":
                            poolName = "Arendelle";
                            break;
                        case "Vbonus_051":
                        case "Vbonus_052":
                        case "Vbonus_053":
                        case "Vbonus_054":
                        case "Vbonus_055":
                        case "Vbonus_056":
                        case "Vbonus_057":
                            poolName = "San Fransokyo";
                            break;
                        case "Vbonus_058":
                        case "Vbonus_059":
                        case "Vbonus_060":
                        case "Vbonus_061":
                        case "Vbonus_062":
                        case "Vbonus_063":
                        case "Vbonus_064":
                        case "Vbonus_065":
                        case "Vbonus_066":
                            poolName = "The Caribbean";
                            break;
                        case "Vbonus_068":
                        case "Vbonus_069":
                        case "Vbonus_070":
                        case "Vbonus_071":
                        case "Vbonus_072":
                        case "Vbonus_073":
                        case "Vbonus_074":
                        case "Vbonus_075":
                        case "Vbonus_076":
                        case "Vbonus_083":
                        case "Vbonus_084":
                            poolName = "Keyblade Graveyard";
                            break;
                        case "VBonus_DLC_001":
                        case "VBonus_DLC_002":
                        case "VBonus_DLC_003":
                        case "VBonus_DLC_004":
                        case "VBonus_DLC_005":
                        case "VBonus_DLC_006":
                        case "VBonus_DLC_007":
                        case "VBonus_DLC_008":
                        case "VBonus_DLC_009":
                        case "VBonus_DLC_010":
                        case "VBonus_DLC_011":
                        case "VBonus_DLC_012":
                        case "VBonus_DLC_013":
                        case "VBonus_DLC_014":
                        case "VBonus_DLC_015":
                            poolName = "Re:Mind";
                            break;
                        case "Vbonus_067":
                            poolName = "Dark World";
                            break;
                        case "VBonus_Minigame007":
                        case "VBonus_Minigame008":
                        case "VBonus_Minigame009":
                        case "VBonus_Minigame010":
                        case "VBonus_Minigame011":
                        case "VBonus_Minigame012":
                        case "VBonus_Minigame013":
                            poolName = "Flantastic Flans";
                            break;
                        case "VBonus_Minigame001":
                        case "VBonus_Minigame002":
                        case "VBonus_Minigame003":
                        case "VBonus_Minigame004":
                        case "VBonus_Minigame005":
                        case "VBonus_Minigame006":
                            poolName = "Minigames";
                            break;
                        default:
                            break;
                    }

                    break;

                default:
                    break;
            }

            return poolName;
        }

        public string GetSubPoolFromOption(DataTableEnum dataTableEnum, string subCategory)
        {
            var subPoolName = "";

            switch (dataTableEnum)
            {
                case DataTableEnum.TreasureHE:
                case DataTableEnum.TreasureTT:
                case DataTableEnum.TreasureTS:
                case DataTableEnum.TreasureRA:
                case DataTableEnum.TreasureMI:
                case DataTableEnum.TreasureFZ:
                case DataTableEnum.TreasureBX:
                case DataTableEnum.TreasureCA:
                case DataTableEnum.TreasureKG:
                case DataTableEnum.TreasureEW:
                case DataTableEnum.TreasureBT:
                    subPoolName = "Treasures";
                    break;
                case DataTableEnum.LevelUp:
                    subPoolName = "Level Ups";
                    break;
                case DataTableEnum.ChrInit:
                    if (subCategory.Contains("Weapon"))
                        subPoolName = "Weapons";
                    else if (subCategory.Contains("Crit"))
                        subPoolName = "Critical Abilities";
                    else if (subCategory.Contains("Ability"))
                        subPoolName = "Abilities";
                    break;
                case DataTableEnum.EquipItem:
                    if (subCategory.Contains("I03"))
                        subPoolName = "Weapons";
                    else if (subCategory.Contains("I04"))
                        subPoolName = "Accessories";
                    else if (subCategory.Contains("I05"))
                        subPoolName = "Armor";
                    break;
                case DataTableEnum.SynthesisItem:
                    var splitSynth = int.Parse(subCategory.Split('_')[1]);
                    
                    if (splitSynth < 61 || splitSynth > 80)
                        subPoolName = "Synthesis Items";
                    else if (splitSynth >= 61 && splitSynth <= 80)
                        subPoolName = "Photo Missions";

                    break;
                case DataTableEnum.WeaponEnhance:
                    var splitWeapon = int.Parse(subCategory.Split('_')[1]);

                    if (splitWeapon < 10)
                        subPoolName = "Kingdom Key";
                    else if (splitWeapon >= 10 && splitWeapon < 20)
                        subPoolName = "Shooting Star";
                    else if (splitWeapon >= 20 && splitWeapon < 30)
                        subPoolName = "Hero's Origin";
                    else if (splitWeapon >= 30 && splitWeapon < 40)
                        subPoolName = "Favorite Deputy";
                    else if (splitWeapon >= 40 && splitWeapon < 50)
                        subPoolName = "Ever After";
                    else if (splitWeapon >= 50 && splitWeapon < 60)
                        subPoolName = "Wheel of Fate";
                    else if (splitWeapon >= 60 && splitWeapon < 70)
                        subPoolName = "Crystal Snow";
                    else if (splitWeapon >= 70 && splitWeapon < 80)
                        subPoolName = "Hunny Spout";
                    else if (splitWeapon >= 80 && splitWeapon < 90)
                        subPoolName = "Nano Gear";
                    else if (splitWeapon >= 90 && splitWeapon < 100)
                        subPoolName = "Happy Gear";
                    else if (splitWeapon >= 100 && splitWeapon < 110)
                        subPoolName = "Classic Tone";
                    else if (splitWeapon >= 110 && splitWeapon < 120)
                        subPoolName = "Grand Chef";
                    else if (splitWeapon >= 120 && splitWeapon < 130)
                        subPoolName = "Ultima Weapon";
                    else if (splitWeapon >= 150 && splitWeapon < 160)
                        subPoolName = "Elemental Encoder";
                    else if (splitWeapon >= 160 && splitWeapon < 170)
                        subPoolName = "Starlight";
                    else if (splitWeapon >= 170 && splitWeapon < 180)
                        subPoolName = "Oathkeeper";
                    else if (splitWeapon >= 180 && splitWeapon < 190)
                        subPoolName = "Oblivion";

                    break;
                case DataTableEnum.FullcourseAbility:
                    subPoolName = "Abilities";
                    break;
                case DataTableEnum.LuckyMark:
                    subPoolName = "Lucky Emblems";
                    break;
                case DataTableEnum.Event:
                    switch (subCategory)
                    {
                        case "EVENT_DATAB_001":
                        case "EVENT_DATAB_002":
                        case "EVENT_DATAB_003":
                        case "EVENT_DATAB_004":
                        case "EVENT_DATAB_005":
                        case "EVENT_DATAB_006":
                        case "EVENT_DATAB_007":
                        case "EVENT_DATAB_008":
                        case "EVENT_DATAB_009":
                        case "EVENT_DATAB_010":
                        case "EVENT_DATAB_011":
                        case "EVENT_DATAB_012":
                        case "EVENT_DATAB_013":
                            subPoolName = "Rewards";
                            break;
                        case "EVENT_REPORT_001a":
                        case "EVENT_REPORT_002a":
                        case "EVENT_REPORT_003a":
                        case "EVENT_REPORT_004a":
                        case "EVENT_REPORT_005a":
                        case "EVENT_REPORT_006a":
                        case "EVENT_REPORT_007a":
                        case "EVENT_REPORT_008a":
                        case "EVENT_REPORT_009a":
                        case "EVENT_REPORT_010a":
                        case "EVENT_REPORT_011a":
                        case "EVENT_REPORT_012a":
                        case "EVENT_REPORT_013a":
                            subPoolName = "Reports";
                            break;
                        case "EVENT_REPORT_001b":
                        case "EVENT_REPORT_002b":
                        case "EVENT_REPORT_003b":
                        case "EVENT_REPORT_004b":
                        case "EVENT_REPORT_005b":
                        case "EVENT_REPORT_006b":
                        case "EVENT_REPORT_007b":
                        case "EVENT_REPORT_008b":
                        case "EVENT_REPORT_009b":
                        case "EVENT_REPORT_010b":
                        case "EVENT_REPORT_011b":
                        case "EVENT_REPORT_012b":
                        case "EVENT_REPORT_013b":
                        case "EVENT_REPORT_014":
                            subPoolName = "Rewards";
                            break;
                        case "EVENT_KEYBLADE_012":
                        case "EVENT_KEYBLADE_013":
                        case "EVENT_HEARTBINDER_001":
                        case "EVENT_KEYITEM_001":
                        case "EVENT_KEYITEM_003":
                        case "EVENT_KEYITEM_004":
                            subPoolName = "Replaced Items";
                            break;
                        default:
                            subPoolName = "Events";
                            break;
                    }


                    break;
                case DataTableEnum.VBonus:
                    switch (subCategory)
                    {
                        case "VBonus_Minigame007":
                        case "VBonus_Minigame008":
                        case "VBonus_Minigame009":
                        case "VBonus_Minigame010":
                        case "VBonus_Minigame011":
                        case "VBonus_Minigame012":
                        case "VBonus_Minigame013":
                            subPoolName = "Flantastic Seven";
                            break;
                        case "VBonus_Minigame001":
                        case "VBonus_Minigame002":
                        case "VBonus_Minigame003":
                        case "VBonus_Minigame004":
                        case "VBonus_Minigame005":
                        case "VBonus_Minigame006":
                            subPoolName = "Minigames";
                            break;
                        default:
                            subPoolName = "Bonuses";
                            break;
                    }

                    break;
                default:
                    break;
            }

            return subPoolName;
        }

        public void ReplaceOptions(Dictionary<string, RandomizeOptionEnum> replacePools, Dictionary<string, RandomizeOptionEnum> randomizePools, 
                                   Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> defaultOptions, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, 
                                   Random random, bool canUseNone)
        {
            foreach (var (poolName, randomizeOptionEnum) in replacePools)
            {
                var options = this.GetOptionsForPool(poolName, defaultOptions);

                foreach (var (category, subOptions) in options)
                {
                    foreach (var (subCategory, subCategoryOptions) in subOptions)
                    {
                        foreach (var (name, value) in subCategoryOptions)
                        {
                            if (name == "TypeB" || name == "TypeC")
                                continue;
                            else if (name == "Weapon")
                                continue;
                            else if (value.Contains("NONE") && !canUseNone)
                                continue;

                            var isImportantCheck = this.VerifyImportantCheck(value, randomizedOptions);

                            if (!isImportantCheck)
                                continue;

                            var randomOption = new Option();

                            // I think this 'true' we want to refactor in the future
                            var categoryNeeded = this.RetrieveCategoryNeeded(category, name, true);

                            // While we have a random important check, we want to keep looking until we found an unimportant one
                            while (!randomOption.Found)
                            {
                                // Use our current randomized options as our base to look from
                                randomOption = this.RetrieveRandomOption(randomizedOptions, random, randomizePools, categoryNeeded, category, canUseNone);
                            }


                            if (!randomizedOptions.ContainsKey(category))
                                randomizedOptions.Add(category, new Dictionary<string, Dictionary<string, string>>());

                            if (!randomizedOptions[category].ContainsKey(subCategory))
                                randomizedOptions[category].Add(subCategory, new Dictionary<string, string>());

                            if (!randomizedOptions[category][subCategory].ContainsKey(name))
                                randomizedOptions[category][subCategory].Add(name, "");


                            // Swap these options
                            randomizedOptions[category][subCategory][name] = randomOption.Value;
                            randomizedOptions[randomOption.Category][randomOption.SubCategory][randomOption.Name] = value;
                        }
                    }
                }
            }
        }

        public void RandomizeOptions(Dictionary<string, RandomizeOptionEnum> randomizePools, Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> defaultOptions, 
                                     ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> copiedOptions,
                                     Random random, bool canUseNone)
        {
            foreach (var (poolName, randomizeOptionEnum) in randomizePools)
            {
                var options = this.GetOptionsForPool(poolName, defaultOptions);

                foreach (var (category, subOptions) in options)
                {
                    foreach (var (subCategory, subCategoryOptions) in subOptions)
                    {
                        foreach (var (name, value) in subCategoryOptions)
                        {
                            if (name == "TypeB" || name == "TypeC")
                                continue;
                            else if (category == DataTableEnum.ChrInit && name == "Weapon")
                                continue;

                            if (value.Contains("NONE") && !canUseNone)
                                continue;

                            // Get random option
                            var randomOption = this.RetrieveRandomOption(copiedOptions, random, canUseNone);

                            if (!randomizedOptions.ContainsKey(category))
                                randomizedOptions.Add(category, new Dictionary<string, Dictionary<string, string>>());

                            if (!randomizedOptions[category].ContainsKey(subCategory))
                                randomizedOptions[category].Add(subCategory, new Dictionary<string, string>());

                            // Add to our random options dictionary and remove from our options dictionary
                            randomizedOptions[category][subCategory].Add(name, randomOption.Value);

                            this.RemoveRandomValue(ref copiedOptions, randomOption);
                        }
                    }
                }
            }
        }

        public void ValidateOptions(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, RandomizeOptionEnum> randomizePools, Random random, bool canUseNone)
        {
            foreach (var (category, subOptions) in randomizedOptions)
            {
                foreach (var (subCategory, subCategoryOptions) in subOptions)
                {
                    foreach (var (name, value) in subCategoryOptions)
                    {
                        if (value.Contains("NONE") && !canUseNone)
                            continue;
                        else if (name == "TypeB" || name == "TypeC")
                            continue;
                        else if (category == DataTableEnum.ChrInit && name == "Weapon")
                            continue;

                        // Get category to validate correct option in slot
                        var categoryNeeded = this.RetrieveCategoryNeeded(category, name);

                        var needSwapping = false;
                        if (categoryNeeded == "Ability" && !value.Contains("ETresAbilityKind::"))
                            needSwapping = true;
                        else if (categoryNeeded == "Item" && (value.Contains("Ability") || value.Contains("Bonus")))
                            needSwapping = true;

                        if (!needSwapping)
                            continue;

                        var swapOption = new Option { Category = category, SubCategory = subCategory, Name = name, Value = value, Found = false };

                        // Use our current randomized options as our base to look from
                        this.SwapRandomOption(ref randomizedOptions, randomizePools, random, categoryNeeded, swapOption, canUseNone);
                    }
                }
            }
        }

        public void SwapRandomOption(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> options, Dictionary<string, RandomizeOptionEnum> randomizePools, Random random, string categoryNeeded, Option swapOption, bool canUseNone, bool canSwapImportant = true)
        {
            var option = new Option();

            while (!option.Found)
            {
                option.Category = options.ElementAt(random.Next(0, options.Count)).Key;
                if (options[option.Category].Count == 0)
                    continue;

                option.SubCategory = options[option.Category].ElementAt(random.Next(0, options[option.Category].Count)).Key;
                if (option.SubCategory.Contains("GIVESORA") || options[option.Category][option.SubCategory].Count == 0)
                    continue;

                if (!randomizePools.ContainsKey(this.GetPoolFromOption(option.Category, option.SubCategory)))
                    continue;

                option.Name = options[option.Category][option.SubCategory].ElementAt(random.Next(0, options[option.Category][option.SubCategory].Count)).Key;
                option.Value = options[option.Category][option.SubCategory][option.Name];


                if (option.Value.Contains("NONE") && !canUseNone)
                    continue;


                if (option.Name == "TypeB" || option.Name == "TypeC")
                    continue;
                else if (option.Category == DataTableEnum.ChrInit && option.Name == "Weapon")
                    continue;
                else if (categoryNeeded == "Ability" && !option.Value.Contains("ETresAbilityKind::"))
                    continue;
                else if (categoryNeeded == "Item" && (option.Value.Contains("Ability") || option.Value.Contains("Bonus")))
                    continue;

                // Validate that we can swap these by checking that the random option found fits on our swap option
                var altCategoryNeeded = this.RetrieveCategoryNeeded(option.Category, option.Name);

                if (altCategoryNeeded == "Ability" && !swapOption.Value.Contains("ETresAbilityKind::"))
                    continue;
                else if (altCategoryNeeded == "Item" && (swapOption.Value.Contains("Ability") || swapOption.Value.Contains("Bonus")))
                    continue;

                if (!canSwapImportant && this.VerifyImportantCheck(option.Value, options))
                    continue;


                option.Found = true;
                break;
            }

            // Swap these options
            options[option.Category][option.SubCategory][option.Name] = swapOption.Value;
            options[swapOption.Category][swapOption.SubCategory][swapOption.Name] = option.Value;
        }

        public void CleanUpOptions(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> copiedOptions,
                                   Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> defaultOptions, Dictionary<string, RandomizeOptionEnum> randomizePools, Random random, bool canUseNone)
        {
            // Add rest of default options (From Vanilla Pools)
            foreach (var (category, subOptions) in defaultOptions)
            {
                foreach (var (subCategory, subCategoryOptions) in subOptions)
                {
                    foreach (var (name, value) in subCategoryOptions)
                    {
                        if (!randomizedOptions.ContainsKey(category))
                            randomizedOptions.Add(category, new Dictionary<string, Dictionary<string, string>>());

                        if (!randomizedOptions[category].ContainsKey(subCategory))
                            randomizedOptions[category].Add(subCategory, new Dictionary<string, string>());

                        if (!randomizedOptions[category][subCategory].ContainsKey(name))
                        {
                            randomizedOptions[category][subCategory].Add(name, value);


                            //if (name == "Weapon" && !value.Contains("ETresItemDefWeapon"))
                            //    randomizedOptions[category][subCategory][name] = this.ConvertKeybladeWeaponToDefenseWeaponEnum(value);
                        }
                    }
                }
            }

            // Account for early abilities for Critical Mode
            foreach (var vbonus in this.VBonusCriticalAbilities)
            {
                var results = this.GetDefaultAbilitiesBonusesForVBonus(vbonus);

                foreach (var result in results)
                {
                    try
                    {
                        var vbonusCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => !y.Key.Contains("GIVESORA") && y.Key != vbonus && y.Value.Any(z => z.Value == result.Value))).Key;
                        var vbonusSubCategory = randomizedOptions[vbonusCategory].FirstOrDefault(y => !y.Key.Contains("GIVESORA") && y.Key != vbonus && y.Value.Any(z => z.Value == result.Value)).Key;
                        var vbonusFound = randomizedOptions[vbonusCategory][vbonusSubCategory].FirstOrDefault(z => z.Value == result.Value);
                        var vbonusOption = new Option { Category = vbonusCategory, SubCategory = vbonusSubCategory, Name = vbonusFound.Key, Value = vbonusFound.Value };

                        var vbonusCategoryNeeded = this.RetrieveCategoryNeeded(vbonusCategory, vbonusFound.Key);

                        this.SwapRandomOption(ref randomizedOptions, randomizePools, random, vbonusCategoryNeeded, vbonusOption, canUseNone);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            // Account for Pole Spin
            var poleSpinCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => y.Key != "GIVESORA_POLE_SPIN" && y.Value.Any(z => z.Value.Contains("POLE_SPIN")))).Key;
            var poleSpinSubCategory = randomizedOptions[poleSpinCategory].FirstOrDefault(y => y.Key != "GIVESORA_POLE_SPIN" && y.Value.Any(z => z.Value.Contains("POLE_SPIN"))).Key;

            while (this.IsPoleSpinDisallowed(poleSpinCategory, poleSpinSubCategory))
            {
                var poleSpin = randomizedOptions[poleSpinCategory][poleSpinSubCategory].FirstOrDefault(z => z.Value.Contains("POLE_SPIN"));
                var poleSpinOption = new Option { Category = poleSpinCategory, SubCategory = poleSpinSubCategory, Name = poleSpin.Key, Value = poleSpin.Value };

                var poleSpinCategoryNeeded = this.RetrieveCategoryNeeded(poleSpinCategory, poleSpin.Key);

                this.SwapRandomOption(ref randomizedOptions, randomizePools, random, poleSpinCategoryNeeded, poleSpinOption, canUseNone);

                // Check that this is a good swap
                poleSpinCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => y.Key != "GIVESORA_POLE_SPIN" && y.Value.Any(z => z.Value.Contains("POLE_SPIN")))).Key;
                poleSpinSubCategory = randomizedOptions[poleSpinCategory].FirstOrDefault(y => y.Key != "GIVESORA_POLE_SPIN" && y.Value.Any(z => z.Value.Contains("POLE_SPIN"))).Key;
            }

            // Distribute TypeB and Type C Level Up Rewards
            if (randomizedOptions.ContainsKey(DataTableEnum.LevelUp))
            {
                for (int i = 2; i <= 50; ++i)
                {
                    var levelString = i.ToString();
                    if (randomizedOptions[DataTableEnum.LevelUp].ContainsKey(levelString))
                    {
                        var levelData = randomizedOptions[DataTableEnum.LevelUp][levelString]["TypeA"];

                        var levels = this.GetLevels(levelString);

                        randomizedOptions[DataTableEnum.LevelUp][levels.Item1]["TypeB"] = levelData;
                        randomizedOptions[DataTableEnum.LevelUp][levels.Item2]["TypeC"] = levelData;
                    }
                }
            }

            // Remove Heartbinders from Synth
            if (randomizedOptions.ContainsKey(DataTableEnum.SynthesisItem))
            {
                foreach (var (subCategory, subCategoryOptions) in randomizedOptions[DataTableEnum.SynthesisItem])
                {
                    foreach (var (name, value) in subCategoryOptions)
                    {
                        foreach (var heartbinder in this.Heartbinders)
                        {
                            if (value.Contains(heartbinder))
                            {
                                var heartbinderCategoryNeeded = this.RetrieveCategoryNeeded(poleSpinCategory, name);
                                var heartbinderOption = new Option { Category = DataTableEnum.SynthesisItem, SubCategory = subCategory, Name = name, Value = value };

                                this.SwapRandomOption(ref randomizedOptions, randomizePools, random, heartbinderCategoryNeeded, heartbinderOption, canUseNone);

                                break;
                            }
                        }
                    }
                }
            }

            // Add Keyblade to Sora's Weapon slot
            if (randomizedOptions.ContainsKey(DataTableEnum.ChrInit))
            {
                var weapon = randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["Weapon"];
                var randomWeapon = this.RetrieveRandomOption(randomizedOptions, random, randomizePools, "Keyblade", DataTableEnum.ChrInit, canUseNone, false);

                if (randomizePools.ContainsKey("Sora"))
                {
                    randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["Weapon"] = this.ConvertKeybladeWeaponToDefenseWeaponEnum(randomWeapon.Value);
                    randomizedOptions[randomWeapon.Category][randomWeapon.SubCategory][randomWeapon.Name] = this.ConvertDefenseWeaponEnumToKeybladeWeapon(weapon);
                }
                else
                {
                    randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["Weapon"] = this.ConvertKeybladeWeaponToDefenseWeaponEnum(weapon);
                }
            }
        }

        public Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> CopyOptions(Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> defaultOptions, Dictionary<string, RandomizeOptionEnum> pools, bool canUseNone)
        {
            var copiedOptions = new Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>>();

            foreach (var (category, subOptions) in defaultOptions)
            {
                foreach (var (subCategory, subCategoryOptions) in subOptions)
                {
                    if (subCategory.Contains("GIVESORA"))
                        continue;

                    var poolName = this.GetPoolFromOption(category, subCategory);

                    if (!pools.ContainsKey(poolName))
                        continue;

                    foreach (var (name, value) in subCategoryOptions)
                    {
                        if (name == "TypeB" || name == "TypeC")
                            continue;

                        if (!canUseNone && value.Contains("NONE"))
                            continue;

                        if (!copiedOptions.ContainsKey(category))
                            copiedOptions.Add(category, new Dictionary<string, Dictionary<string, string>>());
                        
                        if (!copiedOptions[category].ContainsKey(subCategory))
                            copiedOptions[category].Add(subCategory, new Dictionary<string, string>());
                        
                        copiedOptions[category][subCategory].Add(name, value);
                    }
                }
            }

            return copiedOptions;
        }

        /// <summary>
        /// Get a random option from Dictionary of options with a passed in random generator. Does not validate.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="random"></param>
        /// <param name="canUseNone"></param>
        /// <returns></returns>
        public Option RetrieveRandomOption(Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> options, Random random, bool canUseNone)
        {
            var option = new Option();

            while (!option.Found)
            {
                option.Category = options.ElementAt(random.Next(0, options.Count)).Key;
                if (options[option.Category].Count == 0)
                    continue;

                option.SubCategory = options[option.Category].ElementAt(random.Next(0, options[option.Category].Count)).Key;
                if (options[option.Category][option.SubCategory].Count == 0 || option.SubCategory.Contains("GIVESORA"))
                    continue;

                option.Name = options[option.Category][option.SubCategory].ElementAt(random.Next(0, options[option.Category][option.SubCategory].Count)).Key;
                option.Value = options[option.Category][option.SubCategory][option.Name];


                if (option.Value.Contains("NONE") && !canUseNone)
                    continue;


                if (option.Name == "TypeB" || option.Name == "TypeC")
                    continue;
                else if (option.Category == DataTableEnum.ChrInit && option.Name == "Weapon")
                    continue;

                option.Found = true;
            }

            return option;
        }

        // TRANSITIONING INTO OBSELETE
        public Option RetrieveRandomOption(Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> options, Random random, Dictionary<string, RandomizeOptionEnum> randomizePools, string categoryNeeded, DataTableEnum category, bool canUseNone, bool useVerifyImportantCheck = true)
        {
            var option = new Option();

            var optionsTemp = options;
            //var optionsTemp = new Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>>();

            //if (categoryNeeded == "Ability")
            //    optionsTemp = options.Where(x => x.Value.Any(y => y.Value.Any(z => z.Value.Contains("ETresAbilityKind::")))).ToDictionary(x => x.Key, y => y.Value);
            //else if (categoryNeeded == "Keyblade")
            //    optionsTemp = options.Where(x => x.Key != DataTableEnum.ChrInit && x.Value.Any(y => y.Value.Any(z => z.Value.Contains("KEYBLADE")))).ToDictionary(x => x.Key, y => y.Value);
            //else if (categoryNeeded == "Item")
            //    optionsTemp = options.Where(x => x.Value.Any(y => y.Value.Any(z => !z.Value.Contains("Ability") && !z.Value.Contains("Bonus") && !z.Value.Contains("NONE")))).ToDictionary(x => x.Key, y => y.Value);
            //else
            //    optionsTemp = options;
            //else if (categoryNeeded == "None")
            //    optionsTemp = options.Where(x => x.Value.Any(y => y.Value.Any(z => !z.Value.Contains("Ability") && !z.Value.Contains("Bonus") && !z.Value.Contains("NONE")))).ToDictionary(x => x.Key, y => y.Value);


            // Loop through all items
            for (int i = 0; i < 1000; ++i)
            {
                try
                {
                    option.Category = optionsTemp.ElementAt(random.Next(0, optionsTemp.Count)).Key;
                    if (optionsTemp[option.Category].Count == 0 || option.Category == category)
                        continue;

                    option.SubCategory = optionsTemp[option.Category].ElementAt(random.Next(0, optionsTemp[option.Category].Count)).Key;
                    if (option.SubCategory.Contains("GIVESORA") || !randomizePools.ContainsKey(this.GetPoolFromOption(option.Category, option.SubCategory)))
                        continue;

                    if (optionsTemp[option.Category][option.SubCategory].Count == 0)
                        continue;

                    option.Name = optionsTemp[option.Category][option.SubCategory].ElementAt(random.Next(0, optionsTemp[option.Category][option.SubCategory].Count)).Key;
                    option.Value = optionsTemp[option.Category][option.SubCategory][option.Name];


                    if (option.Value.Contains("NONE") && !canUseNone)
                        continue;


                    if (option.Name == "TypeB" || option.Name == "TypeC")
                        continue;
                    else if (option.Category == DataTableEnum.ChrInit && option.Name == "Weapon")
                        continue;
                    else if (categoryNeeded == "Ability" && !option.Value.Contains("ETresAbilityKind::"))
                        continue;
                    else if (categoryNeeded == "Item" && (option.Value.Contains("Ability") || option.Value.Contains("Bonus") || option.Value.Contains("NONE")))
                        continue;
                    else if (categoryNeeded == "Keyblade" && !option.Value.Contains("KEYBLADE"))
                        continue;

                    if (useVerifyImportantCheck && this.VerifyImportantCheck(option.Value, options))
                        continue;

                    option.Found = true;
                    break;
                }
                catch (Exception ex)
                {
                    break;
                }
            }

            return option;
        }

        public void RemoveRandomValue(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> options, Option option)
        {
            options[option.Category][option.SubCategory].Remove(option.Name);

            if (options[option.Category][option.SubCategory].Count == 0)
                options[option.Category].Remove(option.SubCategory);

            if (options[option.Category].Count == 0)
                options.Remove(option.Category);
        }

        public string RetrieveCategoryNeeded(DataTableEnum category, string name, bool replacing = false)
        {
            var categoryNeeded = "";

            switch (category)
            {
                case DataTableEnum.ChrInit:
                    categoryNeeded = (name == "Weapon") ? "Item" : "Ability";
                    break;
                case DataTableEnum.EquipItem:
                case DataTableEnum.FullcourseAbility:
                case DataTableEnum.WeaponEnhance:
                    categoryNeeded = "Ability";
                    break;
                case DataTableEnum.SynthesisItem:
                case DataTableEnum.LuckyMark:
                    categoryNeeded = "Item";
                    break;
                case DataTableEnum.Event:
                    if (replacing)
                        categoryNeeded = "Item";
                    break;
                default:
                    break;
            }

            return categoryNeeded;
        }

        public bool VerifyImportantCheck(string value, Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> options)
        {
            var isAllowed = false;

            if (value.Contains("Ability"))
            {
                foreach (var ability in this.ReplaceableAbilities)
                {
                    if (value.Contains(ability))
                    {
                        return false;
                    }
                }
            }

            foreach (var importantCheck in this.ImportantChecks)
            {
                // Check if ability is on piece of equipment
                if (options.ContainsKey(DataTableEnum.EquipItem) && (value.Contains("PRT") || value.Contains("ACC")))
                {
                    var result = "";
                    if (value.Contains("PRT"))
                        result = "I04" + value.Replace("\u0000", "").Replace("PRT_ITEM", "").PadLeft(3, '0');
                    else if (value.Contains("ACC"))
                        result = "I05" + value.Replace("\u0000", "").Replace("ACC_ITEM", "").PadLeft(3, '0');

                    if (options[DataTableEnum.EquipItem].ContainsKey(result))
                    {
                        foreach (var equipOption in options[DataTableEnum.EquipItem][result])
                        {
                            if (equipOption.Value.Contains(importantCheck))
                            {
                                isAllowed = true;
                                break;
                            }
                        }
                    }
                }

                if (value.Contains(importantCheck))
                    isAllowed = true;
                

                if (isAllowed)
                    break;
            }

            return isAllowed;
        }

        // OBSELETE
        public byte[] CreateZipArchive(Dictionary<string, List<byte>> dataTables, string randomSeed, Dictionary<string, RandomizeOptionEnum> availablePools, Dictionary<string, bool> exceptions, List<Tuple<Option, Option>> modifications, byte[] hints)
        {
            var zipPath = @$"./Seeds/pakchunk99-randomizer-{randomSeed}/pakchunk99-randomizer-{randomSeed}.zip";

            if (Directory.Exists(@$"./Seeds/pakchunk99-randomizer-{randomSeed}"))
                Directory.Delete(@$"./Seeds/pakchunk99-randomizer-{randomSeed}", true);

            // Create the ZIP Archive
            Directory.CreateDirectory(@$"./Seeds/pakchunk99-randomizer-{randomSeed}");

            if (File.Exists(zipPath))
                File.Delete(zipPath);

            using (var zipFile = new FileStream(zipPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using var archive = new ZipArchive(zipFile, ZipArchiveMode.Update);

                // Create the SpoilerLog file
                var spoilerEntry = archive.CreateEntry("SpoilerLog.json");
                using var spoilerWriter = new StreamWriter(spoilerEntry.Open());

                var jsonTupleList = new List<Tuple<Tuple<int, string, string, string>, Tuple<int, string, string, string>>>();
                foreach (var (initial, swap) in modifications)
                {
                    jsonTupleList.Add(new(new Tuple<int, string, string, string>((int)initial.Category, initial.SubCategory, initial.Name, initial.Value),
                                           new Tuple<int, string, string, string>((int)swap.Category, swap.SubCategory, swap.Name, swap.Value)));
                }

                var spoilerLogFile = new SpoilerLogFile
                {
                    SeedName = randomSeed,
                    AvailablePools = availablePools,
                    Exceptions = exceptions,
                    Modifications = jsonTupleList
                };

                var jsonSpoiler = JsonSerializer.Serialize(spoilerLogFile);

                spoilerWriter.WriteLine(jsonSpoiler);


                // Create Hints
                var hintEntry = archive.CreateEntry(@"KINGDOM HEARTS III/Content/Localization/Game/en/kh3_mobile.locres");
                using var hintStream = new MemoryStream(hints);
                using var hintEntryStream = hintEntry.Open();

                hintStream.CopyTo(hintEntryStream);


                // Create the entry from the file path/ name, open the data in a memory stream and copy it to the entry
                foreach (var (filePathAndName, data) in dataTables)
                {
                    var dataTableEntry = archive.CreateEntry(filePathAndName);
                    using var memoryStream = new MemoryStream(data.ToArray());
                    using var stream = dataTableEntry.Open();

                    memoryStream.CopyTo(stream);
                }
            }

            return this.GetFile(zipPath);
        }

        public byte[] GenerateRandomizerSeed(string currentSeed, Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions,
                                             Dictionary<string, RandomizeOptionEnum> availablePools, List<Tuple<Option, Option>> modifications, Dictionary<string, bool> exceptions,
                                             byte[] hintBytes, Dictionary<string, List<string>> hintValues, Dictionary<string, bool> qolValues)
        {
            //foreach(var (topKey, topValue) in randomizedOptions)
            //{
            //    foreach(var (midKey, midValue) in topValue)
            //    {
            //        foreach(var (lowKey, lowValue) in midValue)
            //        {
            //            if (lowValue == "")
            //                Console.WriteLine();
            //        }
            //    }
            //}

            var dataTableManager = new UE4DataTableInterpreter.DataTableManager();
            var dataTables = dataTableManager.RandomizeDataTables(randomizedOptions);
            var hintDataTable = dataTableManager.GenerateHintDataTable(hintValues);
            var qolDataTable = dataTableManager.GenerateQualityOfLifeDataTable(qolValues);

            var zipArchive = this.CreateZipArchive(currentSeed, dataTables, availablePools, modifications, exceptions, hintBytes, hintDataTable, qolDataTable);

            return zipArchive;

            //var zipPath = @$".\Seeds\pakchunk99-randomizer-{currentSeed}\pakchunk99-randomizer-{currentSeed}.zip";
            //ZipFile.ExtractToDirectory(zipPath, @$".\Seeds\pakchunk99-randomizer-{currentSeed}\");

            //if (File.Exists(zipPath))
            //    File.Delete(zipPath);

            //if (File.Exists(@$".\Seeds\pakchunk99-randomizer-{currentSeed}.pak"))
            //    File.Delete(@$".\Seeds\pakchunk99-randomizer-{currentSeed}.pak");

            //var pakPath = Path.Combine(Environment.CurrentDirectory, @$"wwwroot\pak\Seeds\pakchunk99-randomizer-{currentSeed}");
            //Directory.Move(@$".\Seeds\pakchunk99-randomizer-{currentSeed}", pakPath);

            //pakPath.ExecuteCommand();

            //Directory.Move($"{pakPath}.pak", @$".\Seeds\pakchunk99-randomizer-{currentSeed}.pak");

            //return new List<byte[]> { this.GetFile(@$".\Seeds\pakchunk99-randomizer-{currentSeed}.pak"), this.GetFile(@$"{pakPath}\SpoilerLog.json") };
        }

        public byte[] CreateZipArchive(string randomSeed, Dictionary<string, List<byte>> dataTables,
                                       Dictionary<string, RandomizeOptionEnum> availablePools, List<Tuple<Option, Option>> modifications, Dictionary<string, bool> exceptions,
                                       byte[] hints, Dictionary<string, List<byte>> hintDataTable, Dictionary<string, List<byte>> qolDataTable)
        {
            var zipPath = @$"./Seeds/pakchunk99-randomizer-{randomSeed}/pakchunk99-randomizer-{randomSeed}.zip";

            if (Directory.Exists(@$"./Seeds/pakchunk99-randomizer-{randomSeed}"))
                Directory.Delete(@$"./Seeds/pakchunk99-randomizer-{randomSeed}", true);

            // Create the ZIP Archive
            Directory.CreateDirectory(@$"./Seeds/pakchunk99-randomizer-{randomSeed}");

            if (File.Exists(zipPath))
                File.Delete(zipPath);

            using (var zipFile = new FileStream(zipPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using var archive = new ZipArchive(zipFile, ZipArchiveMode.Update);

                // Create the SpoilerLog file
                var spoilerEntry = archive.CreateEntry("SpoilerLog.json");
                using var spoilerWriter = new StreamWriter(spoilerEntry.Open());

                var jsonTupleList = new List<Tuple<Tuple<int, string, string, string>, Tuple<int, string, string, string>>>();
                foreach(var (initial, swap) in modifications)
                {
                    jsonTupleList.Add(new (new Tuple<int, string, string, string>((int)initial.Category, initial.SubCategory, initial.Name, initial.Value),
                                           new Tuple<int, string, string, string>((int)swap.Category, swap.SubCategory, swap.Name, swap.Value)));
                }

                var spoilerLogFile = new SpoilerLogFile
                {
                    SeedName = randomSeed,
                    AvailablePools = availablePools,
                    Exceptions = exceptions,
                    Modifications = jsonTupleList,
                };

                var jsonSpoiler = JsonSerializer.Serialize(spoilerLogFile);

                spoilerWriter.WriteLine(jsonSpoiler);


                // Create Hints
                var hintEntry = archive.CreateEntry(@"KINGDOM HEARTS III/Content/Localization/Game/en/kh3_mobile.locres");
                using var hintStream = new MemoryStream(hints);
                using var hintEntryStream = hintEntry.Open();
                
                hintStream.CopyTo(hintEntryStream);

                foreach (var (filePathAndName, data) in hintDataTable)
                {
                    var dataTableEntry = archive.CreateEntry(filePathAndName);
                    using var memoryStream = new MemoryStream(data.ToArray());
                    using var stream = dataTableEntry.Open();

                    memoryStream.CopyTo(stream);
                }


                // Create QoL
                foreach (var (filePathAndName, data) in qolDataTable)
                {
                    var dataTableEntry = archive.CreateEntry(filePathAndName);
                    using var memoryStream = new MemoryStream(data.ToArray());
                    using var stream = dataTableEntry.Open();

                    memoryStream.CopyTo(stream);
                }


                // Create the entry from the file path/ name, open the data in a memory stream and copy it to the entry
                foreach (var (filePathAndName, data) in dataTables)
                {
                    var dataTableEntry = archive.CreateEntry(filePathAndName);
                    using var memoryStream = new MemoryStream(data.ToArray());
                    using var stream = dataTableEntry.Open();

                    memoryStream.CopyTo(stream);
                }

            }

            return this.GetFile(zipPath);
        }

        public byte[] GetFile(string path)
        {
            using var reader = new FileStream(path, FileMode.Open);
            using var fileDataStream = new MemoryStream();
            reader.CopyTo(fileDataStream);

            return fileDataStream.ToArray();
        }

        public void DeleteRandomizerSeed(string currentSeed)
        {
            Directory.Delete($@"./Seeds/pakchunk99-randomizer-{currentSeed}", true);
            //Directory.Delete($@".\wwwroot\pak\Seeds\pakchunk99-randomizer-{currentSeed}", true);
            //File.Delete($@".\Seeds\pakchunk99-randomizer-{currentSeed}.pak");
        }


        #region Helper Methods

        public Dictionary<string, RandomizeOptionEnum> GetPools(Dictionary<string, RandomizeOptionEnum> pools, string category)
        {
            Dictionary<string, RandomizeOptionEnum> tempPools = new Dictionary<string, RandomizeOptionEnum>();

            if (category == "World")
            {
                tempPools = pools.Where(x => x.Key == "Olympus" || x.Key == "Twilight Town" || x.Key == "Toy Box" || x.Key == "Kingdom of Corona" ||
                                        x.Key == "Monstropolis" || x.Key == "Arendelle" || x.Key == "The Caribbean" || x.Key == "San Fransokyo" ||
                                        x.Key == "100 Acre Wood" || x.Key == "Keyblade Graveyard" || x.Key == "Re:Mind" || x.Key == "Dark World" ||
                                        x.Key == "Unreality")
                                .ToDictionary(x => x.Key, y => y.Value);
            }
            else if (category == "Miscellaneous")
            {
                tempPools = pools.Where(x => x.Key == "Sora" || x.Key == "Equipment Abilities" || x.Key == "Data Battle Rewards" || x.Key == "Moogle Workshop" || 
                                        x.Key == "Fullcourse Abilities" || x.Key == "Lucky Emblems" || x.Key == "Flantastic Flans" || 
                                        x.Key == "Minigames" || x.Key == "Battle Portals" || x.Key == "Always On")
                                .ToDictionary(x => x.Key, y => y.Value);
            }

            return tempPools;
        }

        public Tuple<string, string> GetLevels(string subCategory)
        {
            return subCategory switch
            {
                "2"  => new Tuple<string, string>("44", "50"),
                "4"  => new Tuple<string, string>("6",  "26"),
                "6"  => new Tuple<string, string>("2",  "2"),
                "9"  => new Tuple<string, string>("14", "36"),
                "12" => new Tuple<string, string>("18", "6"),
                "14" => new Tuple<string, string>("28", "4"),
                "16" => new Tuple<string, string>("4",  "14"),
                "18" => new Tuple<string, string>("30", "12"),
                "20" => new Tuple<string, string>("42", "40"),
                "24" => new Tuple<string, string>("40", "9"),
                "26" => new Tuple<string, string>("46", "28"),
                "28" => new Tuple<string, string>("26", "18"),
                "30" => new Tuple<string, string>("9",  "48"),
                "32" => new Tuple<string, string>("50", "16"),
                "34" => new Tuple<string, string>("34", "32"),
                "36" => new Tuple<string, string>("36", "20"),
                "38" => new Tuple<string, string>("38", "30"),
                "40" => new Tuple<string, string>("16", "34"),
                "42" => new Tuple<string, string>("48", "24"),
                "44" => new Tuple<string, string>("12", "42"),
                "46" => new Tuple<string, string>("20", "44"),
                "48" => new Tuple<string, string>("32", "38"),
                "50" => new Tuple<string, string>("24", "46"),
                "3" or "5" or "7" or "8" or "10" or "11" or "13" or "15" or "17" or "19" or "21" or "22" or "23" or "25" or "27" or "29" or "31" or "33" or "35" or "37" or "39" or "41" or "43" or "45" or "47" or "49" => new Tuple<string, string>(subCategory, subCategory),
                _    => new Tuple<string, string>("", ""),
            };
        }

        public bool IsPoleSpinDisallowed(DataTableEnum category, string subCategory)
        {
            bool swapLogic;

            if (category != DataTableEnum.TreasureFZ && category != DataTableEnum.EquipItem && category != DataTableEnum.WeaponEnhance)
            {
                if (category == DataTableEnum.LuckyMark && int.Parse(subCategory) <= 20)
                {
                    swapLogic = true;
                }
                else if (category == DataTableEnum.VBonus && (subCategory != "Vbonus_041" || subCategory != "Vbonus_042" || subCategory != "Vbonus_043" ||
                    subCategory != "Vbonus_044" || subCategory != "Vbonus_045" || subCategory != "Vbonus_046" || subCategory != "Vbonus_047" ||
                    subCategory != "Vbonus_048" || subCategory != "Vbonus_049" || subCategory != "Vbonus_050" ||
                    subCategory != "VBonus_Minigame003" || subCategory != "VBonus_Minigame004" || subCategory != "VBonus_Minigame005"))
                {
                    swapLogic = true;
                }
                else if (category == DataTableEnum.Event && (subCategory != "EVENT_007" || subCategory != "TresUIMobilePortalDataAsset" ||
                    subCategory != "EVENT_KEYBLADE_007" || subCategory != "EVENT_REPORT_009a" || subCategory != "EVENT_REPORT_009b"))
                {
                    swapLogic = true;
                }
                else
                {
                    swapLogic = false;
                }
            }
            else
            {
                swapLogic = true;
            }

            return swapLogic;
        }

        public string GetDefaultAbility(string name)
        {
            var ability = "";

            switch (name)
            {
                case "EquipAbility0":
                    ability = "ETresAbilityKind::LIBRA\u0000";
                    break;
                case "EquipAbility1":
                    ability = "ETresAbilityKind::DODGE\u0000";
                    break;
                case "EquipAbility2":
                    ability = "ETresAbilityKind::AIRSLIDE\u0000";
                    break;
                case "EquipAbility3":
                    ability = "ETresAbilityKind::AIRDODGE\u0000";
                    break;
                case "EquipAbility4":
                    ability = "ETresAbilityKind::REFLECT_GUARD\u0000";
                    break;
                case "EquipAbility5":
                    ability = "ETresAbilityKind::SUPERSLIDE\u0000";
                    break;
                case "EquipAbility6":
                    ability = "ETresAbilityKind::FRIEND_AID\u0000";
                    break;
                case "EquipAbility7":
                    ability = "ETresAbilityKind::SONIC_SLASH\u0000";
                    break;
                case "EquipAbility8":
                    ability = "ETresAbilityKind::SONIC_DOWN\u0000";
                    break;
                case "EquipAbility9":
                    ability = "ETresAbilityKind::TURN_CUTTER\u0000";
                    break;
                case "EquipAbility10":
                    ability = "ETresAbilityKind::SUMMERSALT\u0000";
                    break;
                case "EquipAbility11":
                    ability = "ETresAbilityKind::POLE_SPIN\u0000";
                    break;
                case "EquipAbility12":
                    ability = "ETresAbilityKind::POLE_SWING\u0000";
                    break;
                case "EquipAbility13":
                    ability = "ETresAbilityKind::WALL_KICK\u0000";
                    break;
                case "EquipAbility14":
                    ability = "ETresAbilityKind::CRITICAL_HALF\u0000";
                    break;
                case "EquipAbility15":
                    ability = "ETresAbilityKind::AUTO_LOCK_MAGIC\u0000";
                    break;
                case "CritEquipAbility0":
                    ability = "ETresAbilityKind::CRITICAL_COUNTER\u0000";
                    break;
                case "CritEquipAbility1":
                    ability = "ETresAbilityKind::CRITICAL_CHARGE\u0000";
                    break;
                case "HaveAbility":
                    ability = "ETresAbilityKind::EXPZERO\u0000";
                    break;
                case "CritHaveAbility":
                    ability = "ETresAbilityKind::CRITICAL_CONVERTER\u0000";
                    break;
                default:
                    break;
            }

            return ability;
        }

        public Dictionary<string, string> GetDefaultAbilitiesBonusesForVBonus(string vbonus)
        {
            var results = new Dictionary<string, string>();

            switch (vbonus)
            {
                case "Vbonus_017":
                    results.Add("Sora_Ability1", "ETresAbilityKind::AIR_RECOVERY\u0000");
                    break;
                case "Vbonus_026":
                    results.Add("Sora_Ability1", "ETresAbilityKind::GUARD_COUNTER\u0000");
                    break;
                case "Vbonus_028":
                    results.Add("Sora_Ability1", "ETresAbilityKind::SLASH_UPPER\u0000");
                    break;
                case "Vbonus_032":
                    results.Add("Sora_Ability1", "ETresAbilityKind::REVENGEIMPACT\u0000");
                    break;
                case "Vbonus_036":
                    results.Add("Sora_Ability1", "ETresAbilityKind::COMBO_MASTER\u0000");
                    break;
                case "Vbonus_041":
                    results.Add("Sora_Ability1", "ETresAbilityKind::AIR_ROLL_BEAT\u0000");
                    break;
                case "Vbonus_045":
                    results.Add("Sora_Ability1", "ETresAbilityKind::RISKDODGE\u0000");
                    break;
                case "Vbonus_049":
                    results.Add("Sora_Ability1", "ETresAbilityKind::AIRSLIDE\u0000");
                    results.Add("Sora_Bonus2", "ETresVictoryBonusKind::ACC_SLOT_UP1\u0000");
                    break;
                case "Vbonus_050":
                    results.Add("Sora_Ability1", "ETresAbilityKind::SUPERSLIDE\u0000");
                    results.Add("Sora_Bonus2", "ETresVictoryBonusKind::MP_UP5\u0000");
                    break;
                case "Vbonus_055":
                    results.Add("Sora_Ability1", "ETresAbilityKind::SUPERSLIDE\u0000");
                    break;
                case "Vbonus_058":
                    results.Add("Sora_Ability1", "ETresAbilityKind::REVENGEDIVE\u0000");
                    break;
                case "Vbonus_060":
                    results.Add("Sora_Ability1", "ETresAbilityKind::AIRSLIDE\u0000");
                    break;
                case "Vbonus_069":
                    results.Add("Sora_Ability1", "ETresAbilityKind::REVENGE_EX\u0000");
                    results.Add("Sora_Bonus2", "ETresVictoryBonusKind::ITEM_SLOT_UP1\u0000");
                    break;
                default:
                    break;
            }

            return results;
        }

        #region World Options

        public List<Treasure> GetAvailableTreasuresForWorld(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var treasures = new List<Treasure>();
            var dataTableEnum = this.GetDataTableEnumFromSelection(currentSelection);

            if (randomizedOptions.ContainsKey(dataTableEnum))
            {
                foreach (var treasure in randomizedOptions[dataTableEnum])
                {
                    var treasureId = treasure.Key;
                    var treasureName = treasure.Value["Treasure"];

                    treasures.Add(new Treasure { Id = treasureId, Reward = treasureName });
                }
            }

            return treasures;
        }

        public List<Event> GetAvailableEventsForWorld(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var events = new List<Event>();

            if (randomizedOptions.ContainsKey(DataTableEnum.Event))
            {
                var eventList = new List<string>();
                var eventSubsets = new Dictionary<string, Dictionary<string, string>>();

                switch (currentSelection)
                {
                    case "Olympus":
                        eventList = new List<string> { "EVENT_01", "EVENT_02", "EVENT_03", "EVENT_04", "EVENT_05", "EVENT_06", "EVENT_07", "EVENT_KEYBLADE_001" };
                        break;
                    case "Twilight Town":
                        eventList = new List<string> { "EVENT_KEYBLADE_002", "EVENT_KEYBLADE_010", "EVENT_CKGAME_001" };
                        break;
                    case "Toy Box":
                        eventList = new List<string> { "EVENT_KEYBLADE_003", "EVENT_HEARTBINDER_002" };
                        break;
                    case "Kingdom of Corona":
                        eventList = new List<string> { "EVENT_KEYBLADE_004" };
                        break;
                    case "Monstropolis":
                        eventList = new List<string> { "EVENT_008", "EVENT_KEYBLADE_005", "EVENT_HEARTBINDER_003" };
                        break;
                    case "100 Acre Wood":
                        eventList = new List<string> { "EVENT_KEYBLADE_006" };
                        break;
                    case "Arendelle":
                        eventList = new List<string> { "EVENT_KEYBLADE_007" };
                        break;
                    case "San Fransokyo":
                        eventList = new List<string> { "EVENT_009", "EVENT_KEYBLADE_009", "EVENT_HEARTBINDER_004", "EVENT_KEYITEM_002" };
                        break;
                    case "The Caribbean":
                        eventList = new List<string> { "EVENT_KEYBLADE_008" };
                        break;
                    case "Keyblade Graveyard":
                        eventList = new List<string> { "EVENT_KEYBLADE_011" };
                        break;
                    case "Unreality":
                        eventList = new List<string> { "EVENT_KEYITEM_005", "EVENT_YOZORA_001" };
                        break;
                    default:
                        break;
                }

                eventSubsets = randomizedOptions[DataTableEnum.Event].Where(x => eventList.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                foreach (var tempEvent in eventSubsets)
                {
                    foreach (var subSubset in tempEvent.Value)
                        events.Add(new Event { Id = tempEvent.Key, Category = subSubset.Key, Reward = subSubset.Value });
                }
            }

            return events;
        }

        public List<Bonus> GetAvailableVictoryBonusesForWorld(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var bonuses = new List<Bonus>();

            if (randomizedOptions.ContainsKey(DataTableEnum.VBonus))
            {
                var bonusList = new List<string>();

                switch (currentSelection)
                {
                    case "Olympus":
                        bonusList = new List<string> { "Vbonus_001", "Vbonus_002", "Vbonus_005", "Vbonus_006", "Vbonus_007", "Vbonus_008", "Vbonus_009", "Vbonus_010", "Vbonus_011", "Vbonus_013", "Vbonus_082" };
                        break;
                    case "Twilight Town":
                        bonusList = new List<string> { "Vbonus_014", "Vbonus_015", "Vbonus_016" };
                        break;
                    case "Toy Box":
                        bonusList = new List<string> { "Vbonus_017", "Vbonus_018", "Vbonus_019", "Vbonus_020", "Vbonus_021", "Vbonus_022", "Vbonus_023" };
                        break;
                    case "Kingdom of Corona":
                        bonusList = new List<string> { "Vbonus_024", "Vbonus_025", "Vbonus_026", "Vbonus_027", "Vbonus_028", "Vbonus_029", "Vbonus_030" };
                        break;
                    case "Monstropolis":
                        bonusList = new List<string> { "Vbonus_032", "Vbonus_033", "Vbonus_034", "Vbonus_035", "Vbonus_036", "Vbonus_037", "Vbonus_038", "Vbonus_039", "Vbonus_040" };
                        break;
                    case "Arendelle":
                        bonusList = new List<string> { "Vbonus_041", "Vbonus_042", "Vbonus_043", "Vbonus_044", "Vbonus_045", "Vbonus_047", "Vbonus_048", "Vbonus_049", "Vbonus_050" };
                        break;
                    case "San Fransokyo":
                        bonusList = new List<string> { "Vbonus_051", "Vbonus_052", "Vbonus_053", "Vbonus_054", "Vbonus_055", "Vbonus_056", "Vbonus_057" };
                        break;
                    case "The Caribbean":
                        bonusList = new List<string> { "Vbonus_058", "Vbonus_059", "Vbonus_060", "Vbonus_061", "Vbonus_062", "Vbonus_063", "Vbonus_065", "Vbonus_066" };
                        break;
                    case "Keyblade Graveyard":
                        bonusList = new List<string> { "Vbonus_068", "Vbonus_069", "Vbonus_070", "Vbonus_071", "Vbonus_072", "Vbonus_073", "Vbonus_074", "Vbonus_075", "Vbonus_076", "Vbonus_083", "Vbonus_084" };
                        break;
                    case "Re:Mind":
                        bonusList = new List<string> { "VBonus_DLC_001", "VBonus_DLC_002", "VBonus_DLC_003", "VBonus_DLC_004", "VBonus_DLC_005", "VBonus_DLC_006", "VBonus_DLC_007", "VBonus_DLC_008",
                                                       "VBonus_DLC_009", "VBonus_DLC_010", "VBonus_DLC_011", "VBonus_DLC_012", "VBonus_DLC_013", "VBonus_DLC_014", "VBonus_DLC_015" };
                        break;
                    case "Dark World":
                        bonusList = new List<string> { "Vbonus_067" };
                        break;
                    default:
                        break;
                }

                var bonusSubsets = randomizedOptions[DataTableEnum.VBonus].Where(x => bonusList.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                foreach (var tempBonus in bonusSubsets)
                {
                    bonuses.Add(new Bonus
                    {
                        Name = tempBonus.Key,
                        Bonus1 = tempBonus.Value.GetValueOrDefault("Sora_Bonus1", ""),
                        Ability1 = tempBonus.Value.GetValueOrDefault("Sora_Ability1", ""),
                        Bonus2 = tempBonus.Value.GetValueOrDefault("Sora_Bonus2", ""),
                        Ability2 = tempBonus.Value.GetValueOrDefault("Sora_Ability2", "")
                    });
                }
            }

            return bonuses;
        }

        #endregion World Options

        #region Sora

        public List<LevelUp> GetAvailableLevelUps(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var levelUps = new List<LevelUp>();

            if (randomizedOptions.ContainsKey(DataTableEnum.LevelUp))
            {
                foreach (var levelUp in randomizedOptions[DataTableEnum.LevelUp])
                {
                    var levelUpSubsets = new Dictionary<string, string>();

                    if (currentSelection.Equals("Level Ups"))
                        levelUpSubsets = levelUp.Value.ToDictionary(x => x.Key, y => y.Value);

                    if (levelUpSubsets.Count > 0)
                        levelUps.Add(new LevelUp
                        {
                            Milestone = levelUp.Key,
                            TypeAReward = levelUpSubsets.GetValueOrDefault("TypeA", ""),
                            TypeBReward = levelUpSubsets.GetValueOrDefault("TypeB", ""),
                            TypeCReward = levelUpSubsets.GetValueOrDefault("TypeC", "")
                        });
                }
            }

            return levelUps;
        }

        public List<ChrInit> GetAvailableChrInits(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var chrInits = new List<ChrInit>();

            if (randomizedOptions.ContainsKey(DataTableEnum.ChrInit))
            {
                foreach (var chrInit in randomizedOptions[DataTableEnum.ChrInit])
                {
                    var chrInitSubsets = new Dictionary<string, string>();

                    if (currentSelection.Equals("Weapons"))
                        chrInitSubsets = chrInit.Value.Where(x => x.Key == "Weapon").ToDictionary(x => x.Key, y => y.Value);
                    else if (currentSelection.Equals("Abilities"))
                        chrInitSubsets = chrInit.Value.Where(x => !x.Key.Contains("Crit") && x.Key != "Weapon").ToDictionary(x => x.Key, y => y.Value);
                    else if (currentSelection.Equals("Critical Abilities"))
                        chrInitSubsets = chrInit.Value.Where(x => x.Key.Contains("Crit")).ToDictionary(x => x.Key, y => y.Value);


                    foreach (var subset in chrInitSubsets)
                        chrInits.Add(new ChrInit { Player = chrInit.Key, Name = subset.Key, Value = subset.Value });
                }
            }

            return chrInits;
        }

        #endregion Sora

        #region Equipment Abilities

        public List<Equippable> GetAvailableEquippables(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var equippables = new List<Equippable>();

            if (randomizedOptions.ContainsKey(DataTableEnum.EquipItem))
            {
                var equippableSubsets = new Dictionary<string, Dictionary<string, string>>();

                if (currentSelection.Equals("Weapons"))
                    equippableSubsets = randomizedOptions[DataTableEnum.EquipItem].Where(x => x.Key.Contains("I03")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Armor"))
                    equippableSubsets = randomizedOptions[DataTableEnum.EquipItem].Where(x => x.Key.Contains("I04")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Accessories"))
                    equippableSubsets = randomizedOptions[DataTableEnum.EquipItem].Where(x => x.Key.Contains("I05")).ToDictionary(x => x.Key, y => y.Value);

                foreach (var subset in equippableSubsets)
                {
                    if (subset.Value.Count > 0)
                        equippables.Add(new Equippable
                        {
                            EquipItem = subset.Key,
                            Ability1 = subset.Value.GetValueOrDefault("Ability0", ""),
                            Ability2 = subset.Value.GetValueOrDefault("Ability1", ""),
                            Ability3 = subset.Value.GetValueOrDefault("Ability2", "")
                        });
                }
            }

            return equippables;
        }

        #endregion Equipment Abilities

        #region Data Battle Rewards

        public List<Event> GetAvailableDataBattleRewards(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var events = new List<Event>();

            if (randomizedOptions.ContainsKey(DataTableEnum.Event))
            {
                var eventList = new List<string> { "EVENT_DATAB_001", "EVENT_DATAB_002", "EVENT_DATAB_003", "EVENT_DATAB_004", "EVENT_DATAB_005", "EVENT_DATAB_006", "EVENT_DATAB_007",
                                                   "EVENT_DATAB_008", "EVENT_DATAB_009", "EVENT_DATAB_010", "EVENT_DATAB_011", "EVENT_DATAB_012", "EVENT_DATAB_013" };
                
                var dataBattleEvents = randomizedOptions[DataTableEnum.Event].Where(x => eventList.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                foreach (var tempEvent in dataBattleEvents)
                {
                    foreach (var subSubset in tempEvent.Value)
                        events.Add(new Event { Id = tempEvent.Key, Category = subSubset.Key, Reward = subSubset.Value });
                }
            }

            return events;
        }

        #endregion Data Battle Rewards

        #region Moogle Workshop

        public List<SynthesisItem> GetAvailableSynthesisItems(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var synthesisItems = new List<SynthesisItem>();

            if (randomizedOptions.ContainsKey(DataTableEnum.SynthesisItem))
            {
                var synthesisItemSubsets = new Dictionary<string, Dictionary<string, string>>();

                if (currentSelection.Equals("Synthesis Items"))
                    synthesisItemSubsets = randomizedOptions[DataTableEnum.SynthesisItem].Where(x => int.Parse(x.Key.Split('_')[1]) < 61 || int.Parse(x.Key.Split('_')[1]) > 80).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Photo Missions"))
                    synthesisItemSubsets = randomizedOptions[DataTableEnum.SynthesisItem].Where(x => int.Parse(x.Key.Split('_')[1]) >= 61 && int.Parse(x.Key.Split('_')[1]) <= 80).ToDictionary(x => x.Key, y => y.Value);

                foreach (var tempSynthesisItem in synthesisItemSubsets)
                {
                    foreach (var subSubset in tempSynthesisItem.Value)
                        synthesisItems.Add(new SynthesisItem { Id = tempSynthesisItem.Key, Category = subSubset.Key, Reward = subSubset.Value });
                }
            }

            return synthesisItems;
        }

        public List<WeaponUpgrade> GetAvailableWeaponUpgrades(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var weaponUpgrades = new List<WeaponUpgrade>();

            if (randomizedOptions.ContainsKey(DataTableEnum.WeaponEnhance))
            {
                var weaponUpgradeSubsets = new Dictionary<string, Dictionary<string, string>>();

                if (currentSelection.Equals("Kingdom Key"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) < 10).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Shooting Star"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 10 && int.Parse(x.Key.Split('_')[1]) < 20).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Hero's Origin"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 20 && int.Parse(x.Key.Split('_')[1]) < 30).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Favorite Deputy"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 30 && int.Parse(x.Key.Split('_')[1]) < 40).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Ever After"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 40 && int.Parse(x.Key.Split('_')[1]) < 50).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Wheel of Fate"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 50 && int.Parse(x.Key.Split('_')[1]) < 60).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Crystal Snow"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 60 && int.Parse(x.Key.Split('_')[1]) < 70).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Hunny Spout"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 70 && int.Parse(x.Key.Split('_')[1]) < 80).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Nano Gear"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 80 && int.Parse(x.Key.Split('_')[1]) < 90).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Happy Gear"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 90 && int.Parse(x.Key.Split('_')[1]) < 100).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Classic Tone"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 100 && int.Parse(x.Key.Split('_')[1]) < 110).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Grand Chef"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 110 && int.Parse(x.Key.Split('_')[1]) < 120).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Ultima Weapon"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 120 && int.Parse(x.Key.Split('_')[1]) < 130).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Elemental Encoder"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 150 && int.Parse(x.Key.Split('_')[1]) < 160).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Starlight"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 160 && int.Parse(x.Key.Split('_')[1]) < 170).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Oathkeeper"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 170 && int.Parse(x.Key.Split('_')[1]) < 180).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Oblivion"))
                    weaponUpgradeSubsets = randomizedOptions[DataTableEnum.WeaponEnhance].Where(x => int.Parse(x.Key.Split('_')[1]) >= 180 && int.Parse(x.Key.Split('_')[1]) < 190).ToDictionary(x => x.Key, y => y.Value);

                foreach (var weaponUpgrade in weaponUpgradeSubsets)
                {
                    foreach (var subSubset in weaponUpgrade.Value)
                        weaponUpgrades.Add(new WeaponUpgrade { Id = weaponUpgrade.Key, Name = subSubset.Value });
                }
            }

            return weaponUpgrades;
        }

        #endregion Moogle Workshop

        #region Fullcourse Abilities

        public List<Fullcourse> GetAvailableFullcourses(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var fullcourses = new List<Fullcourse>();

            if (randomizedOptions.ContainsKey(DataTableEnum.FullcourseAbility))
            {
                foreach (var fullcourse in randomizedOptions[DataTableEnum.FullcourseAbility])
                {
                    var fullcourseSubsets = new Dictionary<string, string>();

                    if (currentSelection.Equals("Abilities"))
                        fullcourseSubsets = fullcourse.Value.Where(x => x.Key == "Abilities").ToDictionary(x => x.Key, y => y.Value);

                    foreach (var subset in fullcourse.Value)
                        fullcourses.Add(new Fullcourse { Id = fullcourse.Key, Ability = subset.Value });
                }
            }

            return fullcourses;
        }

        #endregion Fullcourse Abilities

        #region Lucky Emblems

        public List<LuckyEmblem> GetAvailableLuckyEmblems(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var luckyEmblems = new List<LuckyEmblem>();

            if (randomizedOptions.ContainsKey(DataTableEnum.LuckyMark))
            {
                foreach (var luckyEmblem in randomizedOptions[DataTableEnum.LuckyMark])
                {
                    var luckyEmblemSubsets = new Dictionary<string, string>();

                    if (currentSelection.Equals("Lucky Emblems"))
                        luckyEmblemSubsets = luckyEmblem.Value.Where(x => x.Key == "Lucky Emblems").ToDictionary(x => x.Key, y => y.Value);

                    foreach (var subset in luckyEmblem.Value)
                        luckyEmblems.Add(new LuckyEmblem { Milestone = luckyEmblem.Key, Reward = subset.Value });
                }
            }

            return luckyEmblems;
        }

        #endregion Lucky Emblems

        #region Flantastic Flans

        public List<Bonus> GetAvailableFlantasticFlanVictoryBonuses(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var bonuses = new List<Bonus>();

            if (randomizedOptions.ContainsKey(DataTableEnum.VBonus))
            {
                var bonusSubsets = new Dictionary<string, Dictionary<string, string>>();

                if (currentSelection.Equals("Flantastic Seven"))
                    bonusSubsets = randomizedOptions[DataTableEnum.VBonus].Where(x => x.Key.Contains("Minigame") && int.Parse(x.Key[^3..]) >= 7).ToDictionary(x => x.Key, y => y.Value);

                foreach (var tempBonus in bonusSubsets)
                {
                    bonuses.Add(new Bonus
                    {
                        Name = tempBonus.Key,
                        Bonus1 = tempBonus.Value.GetValueOrDefault("Sora_Bonus1", ""),
                        Ability1 = tempBonus.Value.GetValueOrDefault("Sora_Ability1", ""),
                        Bonus2 = tempBonus.Value.GetValueOrDefault("Sora_Bonus2", ""),
                        Ability2 = tempBonus.Value.GetValueOrDefault("Sora_Ability2", "")
                    });
                }
            }

            return bonuses;
        }

        #endregion Flantastic Flans

        #region Minigames

        public List<Bonus> GetAvailableMinigameVictoryBonuses(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var bonuses = new List<Bonus>();

            if (randomizedOptions.ContainsKey(DataTableEnum.VBonus))
            {
                var bonusSubsets = new Dictionary<string, Dictionary<string, string>>();

                if (currentSelection.Equals("Minigames"))
                    bonusSubsets = randomizedOptions[DataTableEnum.VBonus].Where(x => x.Key.Contains("Minigame") && int.Parse(x.Key[^3..]) < 7).ToDictionary(x => x.Key, y => y.Value);

                foreach (var tempBonus in bonusSubsets)
                {
                    bonuses.Add(new Bonus
                    {
                        Name = tempBonus.Key,
                        Bonus1 = tempBonus.Value.GetValueOrDefault("Sora_Bonus1", ""),
                        Ability1 = tempBonus.Value.GetValueOrDefault("Sora_Ability1", ""),
                        Bonus2 = tempBonus.Value.GetValueOrDefault("Sora_Bonus2", ""),
                        Ability2 = tempBonus.Value.GetValueOrDefault("Sora_Ability2", "")
                    });
                }
            }

            return bonuses;
        }

        #endregion Minigames

        #region Battle Portals

        public List<Event> GetAvailableBattlePortalEvents(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var events = new List<Event>();

            if (randomizedOptions.ContainsKey(DataTableEnum.Event))
            {
                var eventList = new List<string>();
                var eventSubsets = new Dictionary<string, Dictionary<string, string>>();

                switch (currentSelection)
                {
                    case "Reports":
                        eventList = new List<string> { "EVENT_REPORT_001a", "EVENT_REPORT_002a", "EVENT_REPORT_003a", "EVENT_REPORT_004a", "EVENT_REPORT_005a", "EVENT_REPORT_006a", "EVENT_REPORT_007a", 
                                                       "EVENT_REPORT_008a", "EVENT_REPORT_009a", "EVENT_REPORT_010a", "EVENT_REPORT_011a", "EVENT_REPORT_012a", "EVENT_REPORT_013a" };
                        break;
                    case "Rewards":
                        eventList = new List<string> { "EVENT_REPORT_001b", "EVENT_REPORT_002b", "EVENT_REPORT_003b", "EVENT_REPORT_004b", "EVENT_REPORT_005b", "EVENT_REPORT_006b", "EVENT_REPORT_007b", 
                                                       "EVENT_REPORT_008b", "EVENT_REPORT_009b", "EVENT_REPORT_010b", "EVENT_REPORT_011b", "EVENT_REPORT_012b", "EVENT_REPORT_013b", "EVENT_REPORT_014" };
                        break;
                    default:
                        break;
                }

                eventSubsets = randomizedOptions[DataTableEnum.Event].Where(x => eventList.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                foreach (var tempEvent in eventSubsets)
                {
                    foreach (var subSubset in tempEvent.Value)
                        events.Add(new Event { Id = tempEvent.Key, Category = subSubset.Key, Reward = subSubset.Value });
                }
            }

            return events;
        }

        #endregion Battle Portals

        #region Always On

        public List<Event> GetAvailableAlwaysOnEvents(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var events = new List<Event>();

            if (randomizedOptions.ContainsKey(DataTableEnum.Event))
            {
                var eventList = new List<string> { "EVENT_KEYBLADE_012", "EVENT_KEYBLADE_013", "EVENT_HEARTBINDER_001", "EVENT_KEYITEM_001", "EVENT_KEYITEM_003", "EVENT_KEYITEM_004" };

                var eventSubsets = randomizedOptions[DataTableEnum.Event].Where(x => eventList.Contains(x.Key)).ToDictionary(x => x.Key, y => y.Value);

                foreach (var tempEvent in eventSubsets)
                {
                    foreach (var subSubset in tempEvent.Value)
                        events.Add(new Event { Id = tempEvent.Key, Category = subSubset.Key, Reward = subSubset.Value });
                }
            }

            return events;
        }

        #endregion Always On


        // Should probably be in extensions
        private DataTableEnum GetDataTableEnumFromSelection(string selection)
        {
            switch(selection)
            {
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

        // Should probably be in extensions
        private DataTableEnum ConvertDisplayStringToEnum(string displayString)
        {
            switch (displayString)
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
                default:
                    return DataTableEnum.None;
            }
        }

        // Should probably be in extensions
        private string ConvertKeybladeWeaponToDefenseWeaponEnum(string keyblade)
        {
            var value = keyblade.ValueIdToDisplay();

            switch (value)
            {
                case "Kingdom Key":
                    return "ETresItemDefWeapon::WEP_KEYBLADE00\u0000";
                case "Hero's Origin":
                    return "ETresItemDefWeapon::WEP_KEYBLADE02\u0000";
                case "Shooting Star":
                    return "ETresItemDefWeapon::WEP_KEYBLADE01\u0000";
                case "Favorite Deputy":
                    return "ETresItemDefWeapon::WEP_KEYBLADE03\u0000";
                case "Ever After":
                    return "ETresItemDefWeapon::WEP_KEYBLADE04\u0000";
                case "Happy Gear":
                    return "ETresItemDefWeapon::WEP_KEYBLADE09\u0000";
                case "Crystal Snow":
                    return "ETresItemDefWeapon::WEP_KEYBLADE06\u0000";
                case "Hunny Spout":
                    return "ETresItemDefWeapon::WEP_KEYBLADE07\u0000";
                case "Nano Gear":
                    return "ETresItemDefWeapon::WEP_KEYBLADE08\u0000";
                case "Wheel of Fate":
                    return "ETresItemDefWeapon::WEP_KEYBLADE05\u0000";
                case "Grand Chef":
                    return "ETresItemDefWeapon::WEP_KEYBLADE11\u0000";
                case "Classic Tone":
                    return "ETresItemDefWeapon::WEP_KEYBLADE10\u0000";
                case "Oathkeeper":
                    return "ETresItemDefWeapon::WEP_KEYBLADE12\u0000";
                case "Oblivion":
                    return "ETresItemDefWeapon::WEP_KEYBLADE13\u0000";
                case "Ultima Weapon":
                    return "ETresItemDefWeapon::WEP_KEYBLADE14\u0000";
                case "Starlight":                                       // Shouldn't be hit
                    return "ETresItemDefWeapon::WEP_KEYBLADE17\u0000";
                case "Elemental Encoder":                               // SHouldn't be hit
                    return "ETresItemDefWeapon::WEP_KEYBLADE18\u0000";
                default:
                    return "ETresItemDefWeapon::WEP_KEYBLADE00\u0000";
            }
        }

        // Should probably be in extensions
        private string ConvertDefenseWeaponEnumToKeybladeWeapon(string keyblade)
        {
            var value = keyblade.ValueIdToDisplay();

            switch (value)
            {
                case "Kingdom Key":
                    return "WEP_KEYBLADE_SO_00\u0000";
                case "Hero's Origin":
                    return "WEP_KEYBLADE_SO_01\u0000";
                case "Shooting Star":
                    return "WEP_KEYBLADE_SO_02\u0000";
                case "Favorite Deputy":
                    return "WEP_KEYBLADE_SO_03\u0000";
                case "Ever After":
                    return "WEP_KEYBLADE_SO_04\u0000";
                case "Happy Gear":
                    return "WEP_KEYBLADE_SO_05\u0000";
                case "Crystal Snow":
                    return "WEP_KEYBLADE_SO_06\u0000";
                case "Hunny Spout":
                    return "WEP_KEYBLADE_SO_07\u0000";
                case "Nano Gear":
                    return "WEP_KEYBLADE_SO_08\u0000";
                case "Wheel of Fate":
                    return "WEP_KEYBLADE_SO_09\u0000";
                case "Grand Chef":
                    return "WEP_KEYBLADE_SO_011\u0000";
                case "Classic Tone":
                    return "WEP_KEYBLADE_SO_012\u0000";
                case "Oathkeeper":
                    return "WEP_KEYBLADE_SO_013\u0000";
                case "Oblivion":
                    return "WEP_KEYBLADE_SO_014\u0000";
                case "Ultima Weapon":
                    return "WEP_KEYBLADE_SO_015\u0000";
                case "Starlight":                                       // Shouldn't be hit
                    return "WEP_KEYBLADE_SO_018\u0000";
                case "Elemental Encoder":                               // SHouldn't be hit
                    return "WEP_KEYBLADE_SO_019\u0000";
                default:
                    return "WEP_KEYBLADE_SO_00\u0000";
            }
        }

        #endregion

        public void RemoveNoneFromTreasure(DataTableEnum dataTableEnum, Random rng, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, Dictionary<string, bool>> availableOptions)
        {
            if (!randomizedOptions.ContainsKey(dataTableEnum))
                return;

            foreach (var treasure in randomizedOptions[dataTableEnum])
            {
                var treasureId = treasure.Key;
                var treasureName = treasure.Value["Treasure"];

                if (treasureName.Contains("NONE"))
                {
                    while (true) // Is there a way we can use a var instead of true?
                    {
                        var tempOptions = randomizedOptions.Where(x => x.Key == DataTableEnum.EquipItem || x.Key == DataTableEnum.VBonus);

                        var swapDataTable = tempOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                        var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));
                        
                        if (swapCategory.Key == "EVENT_KEYBLADE_012" || swapCategory.Key == "EVENT_KEYBLADE_013" || !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                            continue;

                        if (swapCategory.Value.Where(x => !x.Value.Contains("NONE")).Count() > 0)
                        {
                            var swapData = swapCategory.Value.Where(x => !x.Value.Contains("NONE")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => !x.Value.Contains("NONE")).Count()));

                            if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                continue;

                            randomizedOptions[dataTableEnum][treasureId]["Treasure"] = swapData.Value;
                            randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = treasureName;

                            break;
                        }
                    }
                }
            }
        }

        public void RemoveNoneFromEvents(DataTableEnum dataTableEnum, Random rng, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, Dictionary<string, bool>> availableOptions)
        {
            if (!randomizedOptions.ContainsKey(dataTableEnum))
                return;

            foreach (var tempEvent in randomizedOptions[dataTableEnum])
            {
                var tempEventId = tempEvent.Key;

                var key = "RandomizedItem";
                if (tempEvent.Value.ContainsKey("Reward"))
                    key = "Reward";

                var tempEventName = tempEvent.Value[key];

                if (tempEventId == "EVENT_KEYBLADE_012" || tempEventId == "EVENT_KEYBLADE_013")
                {
                    if (!tempEventName.Contains("NONE"))
                    {
                        while (true) // Is there a way we can use a var instead of true?
                        {
                            var tempOptions = randomizedOptions.Where(x => x.Key == DataTableEnum.EquipItem || x.Key == DataTableEnum.VBonus);

                            var swapDataTable = tempOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                            var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                            if (!availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                                continue;

                            if (swapCategory.Value.Where(x => x.Value.Contains("NONE")).Count() > 0)
                            {
                                var swapData = swapCategory.Value.Where(x => x.Value.Contains("NONE")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => x.Value.Contains("NONE")).Count()));

                                if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                    continue;

                                randomizedOptions[dataTableEnum][tempEventId][key] = swapData.Value;
                                randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = tempEventName;

                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (tempEventName.Contains("NONE"))
                    {
                        while (true) // Is there a way we can use a var instead of true?
                        {
                            var tempOptions = randomizedOptions.Where(x => x.Key == DataTableEnum.EquipItem || x.Key == DataTableEnum.VBonus);

                            var swapDataTable = tempOptions.ElementAt(rng.Next(0, tempOptions.Count()));
                            var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                            if (swapCategory.Key == "EVENT_KEYBLADE_012" || swapCategory.Key == "EVENT_KEYBLADE_013" || !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                                continue;

                            if (swapCategory.Value.Where(x => !x.Value.Contains("NONE")).Count() > 0)
                            {
                                var swapData = swapCategory.Value.Where(x => !x.Value.Contains("NONE")).ElementAt(rng.Next(0, swapCategory.Value.Where(x => !x.Value.Contains("NONE")).Count()));

                                if (swapCategory.Key == "m_PlayerSora" && !availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapData.Key.CategoryToKey(swapDataTable.Key)])
                                    continue;

                                randomizedOptions[dataTableEnum][tempEventId][key] = swapData.Value;
                                randomizedOptions[swapDataTable.Key][swapCategory.Key][swapData.Key] = tempEventName;

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}