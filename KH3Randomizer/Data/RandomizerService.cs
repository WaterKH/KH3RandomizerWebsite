using System;
using System.Collections.Generic;
using UE4DataTableInterpreter.Enums;
using KH3Randomizer.Models;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.IO.Compression;
using KH3Randomizer.Enums;
using UE4DataTableInterpreter.Models;
using KH3Randomizer.Models.Configuration;
using KH3Randomizer.Models.Stats;
using System.Threading.Tasks;

namespace KH3Randomizer.Data
{
    public class RandomizerService
    {
        // Certain Abilities, Bonus, Armor, Accessory, Map, Weapon, Key Item, Report, CK or Boost Item
        private readonly List<string> ImportantChecks = new() { "KEY", "REPORT", "LSI", "ITEM_APUP", "ITEM_MAGICUP", "ITEM_POWERUP", "ITEM_GUARDUP", "WEP", "ACC", "PRT",
                                                               "LIBRA", "DODGE", "AIRSLIDE", "REFLECT_GUARD", "POLE_SPIN", "WALL_KICK", "AIR_RECOVERY", 
                                                               "DOUBLEFLIGHT", "LAST_LEAVE", "COMBO_LEAVE",
                                                               "MELEM", "HP_UP", "MP_UP", "SLOT_UP" };

        private readonly List<string> ReplaceableAbilities = new() { "SLASH_UPPER", "AIR_ROLL_BEAT", "AIR_DOWN", "TRIPPLE_SLASH", "CHARGE_THRUST", "MAGICFLUSH",
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

        private readonly List<string> Heartbinders = new() { "KEY_ITEM06", "KEY_ITEM07", "KEY_ITEM08", "KEY_ITEM09", "KEY_ITEM10" };

        private readonly List<string> VBonusCriticalAbilities = new() { "Vbonus_017", "Vbonus_026", "Vbonus_028", "Vbonus_032", "Vbonus_036", "Vbonus_041",
                                                                       "Vbonus_045", "Vbonus_049", "Vbonus_050", "Vbonus_055", "Vbonus_058", "Vbonus_060", "Vbonus_069" };

        private readonly Dictionary<string, Tuple<int, int>> StatBalancedValues = new() {
            { "Base Sora Stats", new Tuple<int, int>(80, 120) },
            { "Level Up Stats", new Tuple<int, int>(0, 3) },
            { "Keyblade Enhance Stats", new Tuple<int, int>(0, 2) },
            { "Equipment Stats", new Tuple<int, int>(0, 4) },
            { "Food Effect Stats", new Tuple<int, int>(0, 4) }
        };

        private readonly Dictionary<string, Tuple<int, int>> StatBoostedValues = new() {
            { "Base Sora Stats", new Tuple<int, int>(120, 150) },
            { "Level Up Stats", new Tuple<int, int>(0, 7) },
            { "Keyblade Enhance Stats", new Tuple<int, int>(0, 3) },
            { "Equipment Stats", new Tuple<int, int>(0, 6) },
            { "Food Effect Stats", new Tuple<int, int>(0, 6) }
        };

        public HintService HintService { get; set; }

        public RandomizerService(HintService hintService)
        {
            this.HintService = hintService;
        }

        // TODO Remove this call and replace it with an AssetIds.json call - This way we only get what we need and don't bother with a dictionary of results, instead just a list of unique results
        public Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> GetDefaultOptions()
        {
            using var streamReader = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/DefaultKH3.json"));
            return JsonSerializer.Deserialize<Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>>>(streamReader.ReadToEnd());
        }

        public List<string> GetDefaultEnemies()
        {
            var distinctEnemies = new List<string>();

            using var streamReaderEnemies = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/DefaultEnemies.json"));
            var allEntities = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Enemy>>>(streamReaderEnemies.ReadToEnd());
            
            foreach (var (world, enemies) in allEntities)
            {
                foreach (var (name, enemy) in enemies)
                {
                    if (distinctEnemies.Contains(enemy.EnemyPath))
                        continue;

                    distinctEnemies.Add(enemy.EnemyPath);
                }
            }

            return distinctEnemies;
        }

        public Dictionary<string, string> GetDefaultBosses()
        {
            using var streamReaderBosses = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/DefaultBosses.json"));
            return JsonSerializer.Deserialize<Dictionary<string, string>>(streamReaderBosses.ReadToEnd());
        }

        public Dictionary<string, string> GetDefaultPartyMembers()
        {
            using var streamReaderPartyMembers = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/DefaultPartyMembers.json"));
            return JsonSerializer.Deserialize<Dictionary<string, string>>(streamReaderPartyMembers.ReadToEnd());
        }

        public Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> Process(string seed, Dictionary<string, RandomizeOptionEnum> pools, Dictionary<string, bool> exceptions, Dictionary<string, int> limiters, bool canUseNone = true)
        {
            var randomizedOptions = new Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>>();

            // Read in Default KH3 Options
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
                            randomizedOptions[category][subCategory].Add(name, value);
                    }
                }
            }

            // Process exceptions like start with default abilities, etc.
            this.ProcessExceptions(ref randomizedOptions, randomizePools, exceptions, random, canUseNone);

            // Process limiters
            this.ProcessLimiters(ref randomizedOptions, randomizePools, limiters, random, canUseNone);

            // Add some clean-up after the randomization
            this.CleanUpOptions(ref randomizedOptions, defaultOptions, randomizePools, random, canUseNone);

            // Give Default Abilities To Sora (Pole Spin, Air Slide & Doubleflight) unless Exception is already ticked
            if (!exceptions["Default Abilities"])
            {
                this.GiveDefaultAbilities(ref randomizedOptions);
            }

            //foreach (var (category, categoryOptions) in randomizedOptions.Where(x => x.Value.Any(y => y.Value.Any(z => z.Value.Contains("NONE")))))
            //{
            //    if (category != DataTableEnum.LevelUp && category != DataTableEnum.TreasureTS && category != DataTableEnum.TreasureRA && category != DataTableEnum.TreasureMI && category != DataTableEnum.TreasureFZ && category != DataTableEnum.TreasureBX)
            //        continue;

            //    foreach (var (subCategory, subCategoryOptions) in categoryOptions.Where(y => y.Value.Any(z => z.Value.Contains("NONE"))))
            //    {
            //        foreach (var (name, value) in subCategoryOptions.Where(z => z.Value.Contains("NONE")))
            //        {
            //            Console.WriteLine($"NONE FOUND: {category} - {subCategory} - {name} - {value}");
            //        }
            //    }
            //}

            return randomizedOptions;
        }

        public Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> ProcessStats(string seed, Dictionary<string, RandomizeOptionEnum> pools, Dictionary<string, bool> exceptions)
        {
            var randomizedStats = new Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>>();

            // Read in Default KH3 Options
            var defaultStats = GetDefaultOptions().Where(x => x.Key == DataTableEnum.BaseCharStat || x.Key == DataTableEnum.EquipItemStat || x.Key == DataTableEnum.FoodItemEffectStat || 
                                                            x.Key == DataTableEnum.LevelUpStat || x.Key == DataTableEnum.WeaponEnhanceStat).ToDictionary(x => x.Key, y => y.Value);

            if (string.IsNullOrEmpty(seed))
                return defaultStats;

            var hash = seed.StringToSeed();
            var random = new Random((int)hash);

            var balancedPools = pools.Where(x => x.Value == RandomizeOptionEnum.Balanced).ToDictionary(x => x.Key, y => y.Value);
            var boostedPools = pools.Where(x => x.Value == RandomizeOptionEnum.Boosted).ToDictionary(x => x.Key, y => y.Value);

            if (balancedPools.Count == 0 && boostedPools.Count == 0)
                return defaultStats;

            // Randomize Stats

            // For Balanced, only allow an increase of 3 stat changes to the original value, check for if all stats can be changed and apply to all that are 0 as well if so
            foreach (var (poolName, balancedEnum) in balancedPools)
            {
                var prevLevelUpAttack = 11;
                var prevLevelUpMagic = 12;
                var prevLevelUpDefense = 8;
                var prevLevelUpAbility = 28;
                foreach (var (statCategory, statValues) in defaultStats[poolName.KeyToDataTableEnum()])
                {
                    foreach (var (name, value) in statValues)
                    {
                        var convertedValue = int.Parse(value);

                        if (convertedValue == 0 && !exceptions["Allow All Stats"])
                            continue;

                        var upperBound = convertedValue + 3;
                        var lowerBound = Math.Abs(convertedValue - 3);

                        var stats = this.StatBalancedValues[poolName];
                        if (poolName == "Base Sora Stats")
                        {
                            // Since the base values aren't used for str, def, mag, ap, this doesn't matter
                            upperBound = stats.Item2;
                            lowerBound = stats.Item1;
                        }
                        else if (poolName == "Keyblade Enhance Stats" || poolName == "Food Effect Stats")
                        {
                            upperBound = convertedValue + stats.Item2;
                            lowerBound = convertedValue;
                        }
                        else if (poolName == "Equipment Stats")
                        {
                            if (name.Contains("Resist"))
                            {
                                upperBound = convertedValue + 25;
                                lowerBound = Math.Abs(convertedValue - 25);
                            }
                            else
                            {
                                upperBound = convertedValue + stats.Item2;
                                lowerBound = Math.Abs(convertedValue - stats.Item1);
                            }
                        }
                        else if (poolName == "Level Up Stats")
                        {
                            if (name == "AttackPower")
                            {
                                upperBound = prevLevelUpAttack + stats.Item2;
                                lowerBound = prevLevelUpAttack;
                            }
                            else if (name == "MagicPower")
                            {
                                upperBound = prevLevelUpMagic + stats.Item2;
                                lowerBound = prevLevelUpMagic;
                            }
                            else if (name == "DefensePower")
                            {
                                upperBound = prevLevelUpDefense + stats.Item2;
                                lowerBound = prevLevelUpDefense;
                            }
                            else if (name == "AbilityPoint")
                            {
                                upperBound = prevLevelUpAbility + stats.Item2;
                                lowerBound = prevLevelUpAbility;
                            }
                        }

                        if (lowerBound < 0 && !exceptions["Allow Negative Stats"])
                            lowerBound = 0;

                        var randomValue = random.Next(lowerBound, upperBound);

                        if (poolName != "Base Sora Stats" && exceptions["Allow Negative Stats"])
                        {
                            var randomChance = random.Next(0, 100);

                            if (randomChance % 2 == 0)
                                randomValue = -randomValue;
                        }

                        if (poolName == "Level Up Stats")
                        {
                            if (name == "AttackPower")
                            {
                                prevLevelUpAttack = randomValue;
                            }
                            else if (name == "MagicPower")
                            {
                                prevLevelUpMagic = randomValue;
                            }
                            else if (name == "DefensePower")
                            {
                                prevLevelUpDefense = randomValue;
                            }
                            else if (name == "AbilityPoint")
                            {
                                prevLevelUpAbility = randomValue;
                            }
                        }

                        var dataTableEnum = poolName.KeyToDataTableEnum();
                        if (!randomizedStats.ContainsKey(dataTableEnum))
                            randomizedStats.Add(dataTableEnum, new Dictionary<string, Dictionary<string, string>>());

                        if (!randomizedStats[dataTableEnum].ContainsKey(statCategory))
                            randomizedStats[dataTableEnum].Add(statCategory, new Dictionary<string, string>());

                        randomizedStats[dataTableEnum][statCategory].Add(name, randomValue.ToString());
                    }
                }
            }

            // For Boosted, only do within 4-10 stat changes to the original value, check for if all stats can be changed and apply to all that are 0 as well if so
            foreach (var (poolName, boostedEnum) in boostedPools)
            {
                var prevLevelUpAttack = 11;
                var prevLevelUpMagic = 12;
                var prevLevelUpDefense = 8;
                var prevLevelUpAbility = 28;
                foreach (var (statCategory, statValues) in defaultStats[poolName.KeyToDataTableEnum()])
                {
                    foreach (var (name, value) in statValues)
                    {
                        var convertedValue = int.Parse(value);

                        if (convertedValue == 0 && !exceptions["Allow All Stats"])
                            continue;

                        var upperBound = convertedValue + 6;
                        var lowerBound = convertedValue + 3;

                        var stats = this.StatBoostedValues[poolName];
                        if (poolName == "Base Sora Stats")
                        {
                            // Since the base values aren't used for str, def, mag, ap, this doesn't matter
                            upperBound = stats.Item2;
                            lowerBound = stats.Item1;
                        }
                        else if (poolName == "Keyblade Enhance Stats" || poolName == "Food Effect Stats")
                        {
                            upperBound = convertedValue + stats.Item2;
                            lowerBound = convertedValue;
                        }
                        else if (poolName == "Equipment Stats")
                        {
                            if (name.Contains("Resist"))
                            {
                                upperBound = convertedValue + 50;
                                lowerBound = Math.Abs(convertedValue - 50);
                            }
                            else
                            {
                                upperBound = convertedValue + stats.Item2;
                                lowerBound = Math.Abs(convertedValue - stats.Item1);
                            }
                        }
                        else if (poolName == "Level Up Stats")
                        {
                            if (name == "AttackPower")
                            {
                                upperBound = prevLevelUpAttack + stats.Item2;
                                lowerBound = prevLevelUpAttack;
                            }
                            else if (name == "MagicPower")
                            {
                                upperBound = prevLevelUpMagic + stats.Item2;
                                lowerBound = prevLevelUpMagic;
                            }
                            else if (name == "DefensePower")
                            {
                                upperBound = prevLevelUpDefense + stats.Item2;
                                lowerBound = prevLevelUpDefense;
                            }
                            else if (name == "AbilityPoint")
                            {
                                upperBound = prevLevelUpAbility + stats.Item2;
                                lowerBound = prevLevelUpAbility;
                            }
                        }

                        var randomValue = random.Next(lowerBound, upperBound);

                        if (poolName != "Base Sora Stats" && exceptions["Allow Negative Stats"])
                        {
                            var randomChance = random.Next(0, 100);

                            if (randomChance % 2 == 0)
                                randomValue = -randomValue;
                        }

                        if (poolName == "Level Up Stats")
                        {
                            if (name == "AttackPower")
                            {
                                prevLevelUpAttack = randomValue;
                            }
                            else if (name == "MagicPower")
                            {
                                prevLevelUpMagic = randomValue;
                            }
                            else if (name == "DefensePower")
                            {
                                prevLevelUpDefense = randomValue;
                            }
                            else if (name == "AbilityPoint")
                            {
                                prevLevelUpAbility = randomValue;
                            }
                        }

                        var dataTableEnum = poolName.KeyToDataTableEnum();
                        if (!randomizedStats.ContainsKey(dataTableEnum))
                            randomizedStats.Add(dataTableEnum, new Dictionary<string, Dictionary<string, string>>());

                        if (!randomizedStats[dataTableEnum].ContainsKey(statCategory))
                            randomizedStats[dataTableEnum].Add(statCategory, new Dictionary<string, string>());

                        randomizedStats[dataTableEnum][statCategory].Add(name, randomValue.ToString());
                    }
                }
            }

            return randomizedStats;
        }

        public Dictionary<string, Enemy> ProcessEnemies(string seed, Dictionary<string, RandomizeOptionEnum> pools, Dictionary<string, bool> exceptions)
        {
            if (string.IsNullOrEmpty(seed))
                return new Dictionary<string, Enemy>();

            var randomizedPools = pools.Where(x => x.Key == "Enemies" && x.Value == RandomizeOptionEnum.Randomize).ToDictionary(x => x.Key, y => y.Value);

            if (randomizedPools.Count == 0)
                return new Dictionary<string, Enemy>();

            var randomizedEnemies = new Dictionary<string, Enemy>();

            // Read in Default KH3 Enemies
            using var streamReaderEnemies = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/DefaultEnemies.json"));
            var allEntities = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Enemy>>>(streamReaderEnemies.ReadToEnd());

            var defaultMiniBosses = new Dictionary<string, Enemy>();
            var defaultEnemies = new Dictionary<string, Enemy>();
            var defaultBosses = new Dictionary<string, Enemy>();

            foreach (var (world, entities) in allEntities)
            {
                foreach (var (name, entity) in entities)
                {
                    if (name.Contains("(Mini-Boss)"))
                        defaultMiniBosses.Add(name, entity);
                    else if (name.Contains("(Boss)"))
                        defaultBosses.Add(name, entity);
                    else
                        defaultEnemies.Add(name, entity);
                }
            }


            var hash = seed.StringToSeed();
            var random = new Random((int)hash);


            // Check for Mini-Boss Exception
            // Randomize Mini-Bosses
            var randomizedMiniBossList = defaultMiniBosses.Keys.ToList();

            //if (exceptions["Allow Mini-Boss Swap"])
            // If so, merge bosses and mini-bosses and remove all inputs for mini-bosses
            //    randomizedMiniBossList = randomizedBossList.Where(x => !x.Contains("Data ")).ToList();

            randomizedMiniBossList.Shuffle(random);

            var randomizedMiniBossQueue = new Queue<string>(randomizedMiniBossList);


            // Randomize Bosses
            var randomizedBossList = defaultBosses.Keys.ToList();

            if (exceptions["Default Data Battles"])
                randomizedBossList = randomizedBossList.Where(x => !x.Contains("Data ")).ToList();

            if (exceptions["Default End Game Battles"])
                randomizedBossList = randomizedBossList.Where(x => !x.Contains("Replica Xehanort") || !x.Contains("Armored Xehanort") || !x.Contains("Master Xehanort (Phase")).ToList();

            if (exceptions["Default Yozora Battle"])
                randomizedBossList = randomizedBossList.Where(x => !x.Contains("Yozora")).ToList();

            randomizedBossList.Shuffle(random);

            var randomizedBossQueue = new Queue<string>(randomizedBossList);


            // Randomize Enemies
            var enemyLookup = new Dictionary<string, Enemy>();
            var randomizedEnemyLookup = new Dictionary<string, string>();

            foreach (var (name, enemy) in defaultEnemies)
            {
                if (!randomizedEnemyLookup.ContainsKey(enemy.EnemyPath))
                    randomizedEnemyLookup.Add(enemy.EnemyPath, "");
            }

            var randomizedEnemyList = randomizedEnemyLookup.Keys.ToList();

            randomizedEnemyList.Shuffle(random);

            var randomizedEnemyQueue = new Queue<string>(randomizedEnemyList);

            foreach (var temp in randomizedEnemyLookup)
            {
                randomizedEnemyLookup[temp.Key] = randomizedEnemyQueue.Dequeue();
            }


            // Stitch the construction back together in order
            foreach (var (world, entities) in allEntities)
            {
                foreach (var (name, entity) in entities)
                {
                    if (name.Contains("(Mini-Boss)"))
                    {
                        var lookupMiniBoss = defaultMiniBosses[randomizedMiniBossQueue.Dequeue()];

                        randomizedEnemies.Add(name, new Enemy { Addresses = entity.Addresses, FilePath = entity.FilePath, EnemyPath = lookupMiniBoss.EnemyPath, SpawnOriginal = entity.SpawnOriginal });
                    }
                    else if (name.Contains("(Boss)"))
                    {
                        if (exceptions["Default Data Battles"] && name.Contains("Data "))
                            continue;
                        else if (exceptions["Default End Game Battles"] && (name.Contains("Armored Xehanort") || name.Contains("Master Xehanort (Phase")))
                            continue;
                        else if (exceptions["Default Yozora Battle"] && name.Contains("Yozora"))
                            continue;

                        var lookupBoss = defaultBosses[randomizedBossQueue.Dequeue()];

                        randomizedEnemies.Add(name, new Enemy { Addresses = entity.Addresses, FilePath = entity.FilePath, EnemyPath = lookupBoss.EnemyPath, SpawnOriginal = entity.SpawnOriginal });
                    }
                    else 
                    {
                        var lookupEnemyPath = randomizedEnemyLookup[entity.EnemyPath];

                        randomizedEnemies.Add(name, new Enemy { Addresses = entity.Addresses, FilePath = entity.FilePath, EnemyPath = lookupEnemyPath, SpawnOriginal = entity.SpawnOriginal });
                    }
                }
            }

            return randomizedEnemies;
        }

        public Dictionary<string, string> ProcessBosses(string seed, Dictionary<string, RandomizeOptionEnum> pools, Dictionary<string, bool> exceptions)
        {
            if (string.IsNullOrEmpty(seed))
                return new Dictionary<string, string>();

            var randomizedPools = pools.Where(x => x.Key == "Bosses" && x.Value == RandomizeOptionEnum.Randomize).ToDictionary(x => x.Key, y => y.Value);

            if (randomizedPools.Count == 0)
                return new Dictionary<string, string>();

            var randomizedBosses = new Dictionary<string, string>();

            // Read in Default KH3 Bosses
            using var streamReaderBosses = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/DefaultBosses.json"));
            var bosses = JsonSerializer.Deserialize<Dictionary<string, string>>(streamReaderBosses.ReadToEnd());

            var hash = seed.StringToSeed();
            var random = new Random((int)hash);

            var originalBossList = bosses.Keys.ToList();
            var randomizedBossList = new List<string>();

            foreach (var boss in originalBossList)
            {
                if (exceptions["Default Data Battles"] && boss.BossIdToBossName().Contains("Data "))
                    continue;
                else if (exceptions["Default End Game Battles"] && (boss.BossIdToBossName().Contains("Armored Xehanort") || boss.BossIdToBossName().Contains("Master Xehanort ")))
                    continue;
                else if (exceptions["Default Yozora Battle"] && boss.BossIdToBossName().Contains("Yozora"))
                    continue;

                randomizedBossList.Add(bosses[boss]);
            }

            randomizedBossList.Shuffle(random);

            var randomizedEnemyQueue = new Queue<string>(randomizedBossList);

            foreach (var (name, path) in bosses)
            {
                if (exceptions["Default Data Battles"] && name.BossIdToBossName().Contains("Data "))
                    continue;
                else if (exceptions["Default End Game Battles"] && (name.BossIdToBossName().Contains("Armored Xehanort") || name.BossIdToBossName().Contains("Master Xehanort ")))
                    continue;
                else if (exceptions["Default Yozora Battle"] && name.BossIdToBossName().Contains("Yozora"))
                    continue;

                bosses[name] = randomizedEnemyQueue.Dequeue();
            }

            // Take care of Skoll being on Gigas
            if (bosses["BOSS_007"] == "e_ex731_Pawn")
            {
                var randomKey = bosses.ElementAt(random.Next(0, bosses.Count)).Key;
                while (randomKey == "BOSS_007")
                {
                    randomKey = bosses.ElementAt(random.Next(0, bosses.Count)).Key;
                }

                var swap = bosses["BOSS_007"];
                bosses["BOSS_007"] = bosses[randomKey];
                bosses[randomKey] = swap;
            }

            return bosses;
        }

        public Dictionary<string, string> ProcessPartyMembers(string seed, Dictionary<string, RandomizeOptionEnum> pools, Dictionary<string, bool> exceptions)
        {
            if (string.IsNullOrEmpty(seed))
                return new Dictionary<string, string>();

            var randomizedPools = pools.Where(x => x.Value == RandomizeOptionEnum.Randomize).ToDictionary(x => x.Key, y => y.Value);

            if (randomizedPools.Count == 0)
                return new Dictionary<string, string>();

            var randomizedPartyMembers = new Dictionary<string, string>();

            // Read in Default KH3 Party Members
            using var streamReaderPartyMembers = new StreamReader(Path.Combine(Environment.CurrentDirectory, @"wwwroot/DefaultPartyMembers.json"));
            var partyMembers = JsonSerializer.Deserialize<Dictionary<string, string>>(streamReaderPartyMembers.ReadToEnd());

            var hash = seed.StringToSeed();
            var random = new Random((int)hash);

            var originalPartyMemberList = partyMembers.Keys.ToList();
            var randomizedPartyList = new List<string>();

            foreach (var partyMember in originalPartyMemberList)
            {
                if (exceptions["Default Donald & Goofy"] && (partyMember.PartyIdToPartyName().Contains("Donald") || partyMember.PartyIdToPartyName().Contains("Goofy")))
                    continue;

                randomizedPartyList.Add(partyMembers[partyMember]);
            }

            randomizedPartyList.Shuffle(random);

            var randomizedEnemyQueue = new Queue<string>(randomizedPartyList);

            foreach (var (name, path) in partyMembers)
            {
                if (exceptions["Default Donald & Goofy"] && (name.PartyIdToPartyName().Contains("Donald") || name.PartyIdToPartyName().Contains("Goofy")))
                    continue;

                partyMembers[name] = randomizedEnemyQueue.Dequeue();
            }

            return partyMembers;
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

            if (exceptions["Replace Classic Kingdom Reward"] && randomizedOptions.ContainsKey(DataTableEnum.Event))
            {
                var tempReward = randomizedOptions[DataTableEnum.Event]["TresUIMobilePortalDataAsset"]["Reward"];
                var swapOption = new Option { Category = DataTableEnum.Event, SubCategory = "TresUIMobilePortalDataAsset", Name = "Reward", Value = tempReward };

                this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "Item", swapOption, canUseNone, false);
            }

            if (exceptions["Replace Little Chef's Reward"] && randomizedOptions.ContainsKey(DataTableEnum.Event))
            {
                var tempReward = randomizedOptions[DataTableEnum.Event]["EVENT_KEYBLADE_010"]["RandomizedItem"];
                var swapOption = new Option { Category = DataTableEnum.Event, SubCategory = "EVENT_KEYBLADE_010", Name = "RandomizedItem", Value = tempReward };

                this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "Item", swapOption, canUseNone, false);
            }

            if (exceptions["Replace Golden Herc Rewards"] && randomizedOptions.ContainsKey(DataTableEnum.Event))
            {
                var keys = new List<string> { "EVENT_002", "EVENT_003", "EVENT_004", "EVENT_005", "EVENT_006", "EVENT_007" };

                foreach (var key in keys)
                {
                    var tempReward = randomizedOptions[DataTableEnum.Event][key]["RandomizedItem"];
                    var swapOption = new Option { Category = DataTableEnum.Event, SubCategory = key, Name = "RandomizedItem", Value = tempReward };

                    this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "Item", swapOption, canUseNone, false);
                }
            }

            if (exceptions["Replace Sora Collection Rewards"] && randomizedOptions.ContainsKey(DataTableEnum.VBonus))
            {
                var keys = new List<string> { "Vbonus_083", "Vbonus_084" };

                foreach (var key in keys)
                {
                    foreach (var (name, value) in randomizedOptions[DataTableEnum.VBonus][key])
                    {
                        if (value.Contains("NONE") && !canUseNone)
                            continue;

                        var tempReward = randomizedOptions[DataTableEnum.VBonus][key][name];
                        var swapOption = new Option { Category = DataTableEnum.VBonus, SubCategory = key, Name = name, Value = tempReward };

                        this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "", swapOption, canUseNone, false);
                    }
                }
            }

            if (exceptions["Replace Photo Missions"] && randomizedOptions.ContainsKey(DataTableEnum.SynthesisItem))
            {
                var photoMissions = new List<string> { "IS_61", "IS_62", "IS_63", "IS_64", "IS_65", "IS_66", "IS_67", "IS_68", "IS_69", "IS_70", 
                                                "IS_71", "IS_72", "IS_73", "IS_74", "IS_75", "IS_76", "IS_77", "IS_78", "IS_79", "IS_80" };

                foreach (var photoMission in photoMissions)
                {
                    foreach (var (name, value) in randomizedOptions[DataTableEnum.SynthesisItem][photoMission])
                    {
                        var tempReward = randomizedOptions[DataTableEnum.SynthesisItem][photoMission][name];
                        var swapOption = new Option { Category = DataTableEnum.SynthesisItem, SubCategory = photoMission, Name = name, Value = tempReward };

                        this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "", swapOption, canUseNone, false);
                    }
                }
            }

            if (exceptions["Extra Movement"])
            {
                var extras = new List<string>
                {
                    "ETresAbilityKind::DODGE\u0000", "ETresAbilityKind::DODGE\u0000",
                    "ETresAbilityKind::AIRSLIDE\u0000", "ETresAbilityKind::AIRSLIDE\u0000",
                    "ETresAbilityKind::AIRDODGE\u0000", "ETresAbilityKind::AIRDODGE\u0000",
                    "ETresAbilityKind::SUPERSLIDE\u0000", "ETresAbilityKind::SUPERSLIDE\u0000",
                    "ETresAbilityKind::POLE_SPIN\u0000", "ETresAbilityKind::POLE_SPIN\u0000",
                    "ETresAbilityKind::WALL_KICK\u0000", "ETresAbilityKind::WALL_KICK\u0000",
                    "ETresAbilityKind::GLIDE\u0000", "ETresAbilityKind::GLIDE\u0000",
                    "ETresAbilityKind::SUPERJUMP\u0000", "ETresAbilityKind::SUPERJUMP\u0000",
                };

                foreach (var extra in extras)
                {
                    var option = new Option();

                    while (!option.Found)
                    {
                        option.Category = randomizedOptions.ElementAt(random.Next(0, randomizedOptions.Count)).Key;
                        if (randomizedOptions[option.Category].Count == 0)
                            continue;

                        option.SubCategory = randomizedOptions[option.Category].ElementAt(random.Next(0, randomizedOptions[option.Category].Count)).Key;
                        if (option.SubCategory.Contains("GIVESORA") || randomizedOptions[option.Category][option.SubCategory].Count == 0)
                            continue;

                        if (!randomizePools.ContainsKey(this.GetPoolFromOption(option.Category, option.SubCategory)))
                            continue;

                        option.Name = randomizedOptions[option.Category][option.SubCategory].ElementAt(random.Next(0, randomizedOptions[option.Category][option.SubCategory].Count)).Key;
                        option.Value = randomizedOptions[option.Category][option.SubCategory][option.Name];


                        if (option.Value.Contains("NONE") && !canUseNone)
                            continue;


                        if (option.Name == "TypeB" || option.Name == "TypeC")
                            continue;
                        else if (option.Category == DataTableEnum.ChrInit && option.Name == "Weapon")
                            continue;

                        // Validate that we can swap these by checking that the random option found fits on our swap option
                        var altCategoryNeeded = this.RetrieveCategoryNeeded(option.Category, option.Name);

                        if (altCategoryNeeded == "Item" || !option.Value.Contains("MAT"))
                            continue;


                        option.Found = true;
                        break;
                    }

                    randomizedOptions[option.Category][option.SubCategory][option.Name] = extra;
                }
            }

            if (exceptions["Safe Magic Locations"])
            {
                // Remove from Levels
                foreach (var (subCategory, values) in randomizedOptions[DataTableEnum.LevelUp])
                {
                    foreach (var (name, value) in values)
                    {
                        if (name == "TypeB" || name == "TypeC")
                            continue;

                        if (value.Contains("MELEM"))
                        {
                            var swapOption = new Option { Category = DataTableEnum.LevelUp, SubCategory = subCategory, Name = name, Value = value };

                            this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "", swapOption, canUseNone);
                        }
                    }
                }


                var forbiddenLocations = new Dictionary<DataTableEnum, List<string>> {
                    { DataTableEnum.TreasureTS, new List<string> { "TS_LBOX_001", "TS_SBOX_001", "TS_SBOX_002" } },
                    { DataTableEnum.TreasureTT, new List<string> { "TT_LBOX_001", "TT_SBOX_001", "TT_SBOX_002", "TT_SBOX_003", "TT_SBOX_004" } }
                };

                // Remove from Forbidden Locations
                foreach (var (category, locations) in forbiddenLocations)
                {
                    foreach (var location in locations)
                    {
                        foreach (var (name, value) in randomizedOptions[category][location])
                        {
                            if (value.Contains("MELEM"))
                            {
                                var swapOption = new Option { Category = category, SubCategory = location, Name = name, Value = value };

                                this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "", swapOption, canUseNone);
                            }
                        }
                    }
                }
            }
        }

        public void ProcessLimiters(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<string, RandomizeOptionEnum> randomizePools, Dictionary<string, int> limiters, Random random, bool canUseNone = true)
        {
            // Handle Level Up Limit
            if (randomizedOptions.ContainsKey(DataTableEnum.LevelUp) && limiters["Level Up Limit"] != 50)
            {
                var levelStart = limiters["Level Up Limit"];
                for (int i = levelStart; i <= 50; ++i)
                {
                    var tempReward = randomizedOptions[DataTableEnum.LevelUp][i.ToString()]["TypeA"];

                    if (tempReward.Contains("NONE") && !canUseNone)
                        continue;

                    var swapOption = new Option { Category = DataTableEnum.LevelUp, SubCategory = i.ToString(), Name = "TypeA", Value = tempReward };

                    this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "", swapOption, canUseNone, false);
                }
            }

            // Handle Lucky Emblem Limit
            if (randomizedOptions.ContainsKey(DataTableEnum.LuckyMark) && limiters["Lucky Emblem Limit"] != 90)
            {
                var luckyMarkMilestones = new List<int> { 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 80, 90 };
                var luckyMarkStart = luckyMarkMilestones.IndexOf(limiters["Lucky Emblem Limit"]);
                for (int i = luckyMarkStart; i < luckyMarkMilestones.Count; ++i)
                {
                    var tempSubCategory = luckyMarkMilestones[i].ToString();

                    var tempReward = randomizedOptions[DataTableEnum.LuckyMark][tempSubCategory]["Reward"];
                    var swapOption = new Option { Category = DataTableEnum.LuckyMark, SubCategory = tempSubCategory, Name = "Reward", Value = tempReward };

                    this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "", swapOption, canUseNone, false);
                }
            }

            // Handle Moogle Synthesis Limit
            if (randomizedOptions.ContainsKey(DataTableEnum.SynthesisItem))
            {
                var synthesisLimit = limiters["Moogle Synthesis Limit"];

                // Find Moogle Synth Important Items
                var importantSynthItems = new List<Option>();
                foreach (var (subCategory, values) in randomizedOptions[DataTableEnum.SynthesisItem])
                {
                    foreach (var (name, value) in values)
                    {
                        if (this.VerifyImportantCheck(value, randomizedOptions))
                        {
                            var option = new Option { Category = DataTableEnum.SynthesisItem, SubCategory = subCategory, Name = name, Value = value };
                            
                            importantSynthItems.Add(option);
                        }
                    }
                }


                // Check that there are only 1 of each Proof and no heart piece
                var replaceProofs = importantSynthItems.Where(x => x.Name.Contains("KEY_ITEM14") || x.Name.Contains("KEY_ITEM15") || x.Name.Contains("KEY_ITEM16")).ToList();
                if (replaceProofs.Count > 1)
                {
                    for (int i = 0; i < replaceProofs.Count - 1; ++i)
                    {
                        var removeIndex = random.Next(0, replaceProofs.Count);
                        var tempOption = replaceProofs.ElementAt(removeIndex);
                        var swapOption = new Option { Category = tempOption.Category, SubCategory = tempOption.SubCategory, Name = tempOption.Name, Value = tempOption.Value };

                        this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "Item", swapOption, canUseNone, false);

                        replaceProofs.RemoveAt(removeIndex);
                    }
                }

                var replaceHeartPieces = importantSynthItems.Where(x => x.Name.Contains("KEY_ITEM17")).ToList();
                if (replaceHeartPieces.Count > 1)
                {
                    for (int i = 0; i < replaceHeartPieces.Count; ++i)
                    {
                        var removeIndex = random.Next(0, replaceHeartPieces.Count);
                        var tempOption = replaceHeartPieces.ElementAt(removeIndex);
                        var swapOption = new Option { Category = tempOption.Category, SubCategory = tempOption.SubCategory, Name = tempOption.Name, Value = tempOption.Value };

                        this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "Item", swapOption, canUseNone, false);

                        replaceHeartPieces.RemoveAt(removeIndex);
                    }
                }

                // Verify we aren't over our limit
                if (importantSynthItems.Count > synthesisLimit)
                {
                    var length = importantSynthItems.Count - synthesisLimit;
                    for (int i = 0; i < length; ++i)
                    {
                        var removeIndex = random.Next(0, importantSynthItems.Count);
                        var tempOption = importantSynthItems.ElementAt(removeIndex);
                        var swapOption = new Option { Category = tempOption.Category, SubCategory = tempOption.SubCategory, Name = tempOption.Name, Value = tempOption.Value };

                        this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "Item", swapOption, canUseNone, false);

                        importantSynthItems.RemoveAt(removeIndex);
                    }
                }


                // Replace Heartbinders & Hercule's Golden Statue in Moogle Shop
                var keyItems = new List<string> { "KEY_ITEM06", "KEY_ITEM07", "KEY_ITEM08", "KEY_ITEM09", "KEY_ITEM10", "KEY_ITEM11" };

                foreach (var (subCategoryName, subCategories) in randomizedOptions[DataTableEnum.SynthesisItem])
                {
                    foreach (var (name, value) in subCategories)
                    {
                        foreach (var keyItem in keyItems)
                        {
                            if (value.Contains(keyItem))
                            {
                                var tempOption = new Option { Category = DataTableEnum.SynthesisItem, SubCategory = subCategoryName, Name = name, Value = value };

                                this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "Item", tempOption, canUseNone, false);
                                break;
                            }
                        }
                    }
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
                    events = new List<string> { "EVENT_001", "EVENT_002", "EVENT_003", "EVENT_004", "EVENT_005", "EVENT_006", "EVENT_007", "EVENT_KEYBLADE_001" };
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
                    vbonuses = new List<string> { "Vbonus_058", "Vbonus_059", "Vbonus_060", "Vbonus_061", "Vbonus_062", "Vbonus_063", "Vbonus_064", "Vbonus_065", "Vbonus_066" };
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
                case "Re+Mind":
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
                    poolName = "Re+Mind";
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
                        case "EVENT_001":
                        case "EVENT_002":
                        case "EVENT_003":
                        case "EVENT_004":
                        case "EVENT_005":
                        case "EVENT_006":
                        case "EVENT_007":
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
                            poolName = "Re+Mind";
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
                        subPoolName = "Pandora's Power";
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
                    subPoolName = subCategory switch
                    {
                        "EVENT_DATAB_001" or "EVENT_DATAB_002" or "EVENT_DATAB_003" or "EVENT_DATAB_004" or "EVENT_DATAB_005" or "EVENT_DATAB_006" or "EVENT_DATAB_007" or "EVENT_DATAB_008" or "EVENT_DATAB_009" or "EVENT_DATAB_010" or "EVENT_DATAB_011" or "EVENT_DATAB_012" or "EVENT_DATAB_013" => "Rewards",
                        "EVENT_REPORT_001a" or "EVENT_REPORT_002a" or "EVENT_REPORT_003a" or "EVENT_REPORT_004a" or "EVENT_REPORT_005a" or "EVENT_REPORT_006a" or "EVENT_REPORT_007a" or "EVENT_REPORT_008a" or "EVENT_REPORT_009a" or "EVENT_REPORT_010a" or "EVENT_REPORT_011a" or "EVENT_REPORT_012a" or "EVENT_REPORT_013a" => "Reports",
                        "EVENT_REPORT_001b" or "EVENT_REPORT_002b" or "EVENT_REPORT_003b" or "EVENT_REPORT_004b" or "EVENT_REPORT_005b" or "EVENT_REPORT_006b" or "EVENT_REPORT_007b" or "EVENT_REPORT_008b" or "EVENT_REPORT_009b" or "EVENT_REPORT_010b" or "EVENT_REPORT_011b" or "EVENT_REPORT_012b" or "EVENT_REPORT_013b" or "EVENT_REPORT_014" => "Rewards",
                        "EVENT_KEYBLADE_012" or "EVENT_KEYBLADE_013" or "EVENT_HEARTBINDER_001" or "EVENT_KEYITEM_001" or "EVENT_KEYITEM_003" or "EVENT_KEYITEM_004" => "Replaced Items",
                        _ => "Events",
                    };
                    break;
                case DataTableEnum.VBonus:
                    subPoolName = subCategory switch
                    {
                        "VBonus_Minigame007" or "VBonus_Minigame008" or "VBonus_Minigame009" or "VBonus_Minigame010" or "VBonus_Minigame011" or "VBonus_Minigame012" or "VBonus_Minigame013" => "Flantastic Seven",
                        "VBonus_Minigame001" or "VBonus_Minigame002" or "VBonus_Minigame003" or "VBonus_Minigame004" or "VBonus_Minigame005" or "VBonus_Minigame006" => "Minigames",
                        _ => "Bonuses",
                    };
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
                            if (value.Contains("NONE") && !canUseNone)
                                continue;
                            else if (name == "TypeB" || name == "TypeC")
                                continue;
                            else if (name == "Weapon")
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
                            if (value.Contains("NONE") && !canUseNone)
                                continue;
                            else if (name == "TypeB" || name == "TypeC")
                                continue;
                            else if (category == DataTableEnum.ChrInit && name == "Weapon")
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
                if (options[option.Category].Count == 0 || (option.Category == swapOption.Category && option.Category == DataTableEnum.SynthesisItem))
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
                else if (categoryNeeded == "Bonus" && !option.Value.Contains("ETresVictoryBonusKind::"))
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

        public void CleanUpOptions(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions, Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> defaultOptions, 
                                   Dictionary<string, RandomizeOptionEnum> randomizePools, Random random, bool canUseNone)
        {
            // Account for early abilities for Critical Mode
            foreach (var vbonus in this.VBonusCriticalAbilities)
            {
                var results = this.GetDefaultAbilitiesBonusesForVBonus(vbonus);

                foreach (var result in results)
                {
                    var currentValue = randomizedOptions[DataTableEnum.VBonus][vbonus][result.Key];

                    if (result.Key.Contains("Ability") && currentValue.Contains("ETresAbilityKind::"))
                        continue;
                    else if (result.Key.Contains("Bonus") && currentValue.Contains("ETresVictoryBonusKind::"))
                        continue;

                    var vbonusOption = new Option { Category = DataTableEnum.VBonus, SubCategory = vbonus, Name = result.Key, Value = currentValue };
                    var categoryNeeded = result.Key.Contains("Ability") ? "Ability" : "Bonus";

                    this.SwapRandomOption(ref randomizedOptions, randomizePools, random, categoryNeeded, vbonusOption, canUseNone);
                }
            }

            // Account for Pole Spin
            //var poleSpinCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => y.Key != "GIVESORA_POLE_SPIN" && y.Value.Any(z => z.Value.Contains("POLE_SPIN")))).Key;
            //var poleSpinSubCategory = randomizedOptions[poleSpinCategory].FirstOrDefault(y => y.Key != "GIVESORA_POLE_SPIN" && y.Value.Any(z => z.Value.Contains("POLE_SPIN"))).Key;

            //while (this.IsPoleSpinDisallowed(poleSpinCategory, poleSpinSubCategory))
            //{
            //    var poleSpin = randomizedOptions[poleSpinCategory][poleSpinSubCategory].FirstOrDefault(z => z.Value.Contains("POLE_SPIN"));
            //    var poleSpinOption = new Option { Category = poleSpinCategory, SubCategory = poleSpinSubCategory, Name = poleSpin.Key, Value = poleSpin.Value };

            //    var poleSpinCategoryNeeded = this.RetrieveCategoryNeeded(poleSpinCategory, poleSpin.Key);

            //    this.SwapRandomOption(ref randomizedOptions, randomizePools, random, poleSpinCategoryNeeded, poleSpinOption, canUseNone);

            //    // Check that this is a good swap
            //    poleSpinCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => y.Key != "GIVESORA_POLE_SPIN" && y.Value.Any(z => z.Value.Contains("POLE_SPIN")))).Key;
            //    poleSpinSubCategory = randomizedOptions[poleSpinCategory].FirstOrDefault(y => y.Key != "GIVESORA_POLE_SPIN" && y.Value.Any(z => z.Value.Contains("POLE_SPIN"))).Key;
            //}

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

            // Replace BattleGates 2 (Olympus 2nd) and 7 (Kingdom of Corona 1st)
            if (randomizedOptions.ContainsKey(DataTableEnum.Event))
            {
                var portalEvents = new List<string> { "EVENT_REPORT_002a", "EVENT_REPORT_002b", "EVENT_REPORT_007a", "EVENT_REPORT_007b" };

                foreach (var portalEvent in portalEvents)
                {
                    if (randomizedOptions[DataTableEnum.Event].ContainsKey(portalEvent))
                    {
                        var value = randomizedOptions[DataTableEnum.Event][portalEvent]["RandomizedItem"];
                        var reportOption = new Option { Category = DataTableEnum.Event, SubCategory = portalEvent, Name = "RandomizedItem", Value = value };

                        this.SwapRandomOption(ref randomizedOptions, randomizePools, random, "", reportOption, canUseNone, false);
                    }
                }
            }
        }

        public void GiveDefaultAbilities(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var containsPoleSpin = false;
            var containsDoubleFlight = false;
            var containsAirSlide = false;
            foreach (var (name, value) in randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"])
            {
                if (value.Contains("POLE_SPIN"))
                    containsPoleSpin = true;
                else if (value.Contains("DOUBLEFLIGHT"))
                    containsDoubleFlight = true;
                else if (value.Contains("AIRSLIDE"))
                    containsAirSlide = true;
            }


            // Default Pole Spin
            if (!containsPoleSpin)
            {
                var poleSpinCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => y.Key != "GIVESORA_POLE_SPIN" && y.Value.Any(z => z.Value.Contains("POLE_SPIN")))).Key;
                var poleSpinSubCategory = randomizedOptions[poleSpinCategory].FirstOrDefault(y => y.Key != "GIVESORA_POLE_SPIN" && y.Value.Any(z => z.Value.Contains("POLE_SPIN"))).Key;
                var poleSpin = randomizedOptions[poleSpinCategory][poleSpinSubCategory].FirstOrDefault(z => z.Value.Contains("POLE_SPIN"));
                var poleSpinOption = new Option { Category = poleSpinCategory, SubCategory = poleSpinSubCategory, Name = poleSpin.Key, Value = poleSpin.Value };

                // Swap these options
                var poleSpinOptionValue = randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["EquipAbility0"];
                randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["EquipAbility0"] = poleSpinOption.Value;
                randomizedOptions[poleSpinOption.Category][poleSpinOption.SubCategory][poleSpinOption.Name] = poleSpinOptionValue;
            }


            // Default Double Flight
            if (!containsDoubleFlight)
            {
                var doubleFlightCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => y.Key != "GIVESORA_DOUBLEFLIGHT" && y.Value.Any(z => z.Value.Contains("DOUBLEFLIGHT")))).Key;
                var doubleFlightSubCategory = randomizedOptions[doubleFlightCategory].FirstOrDefault(y => y.Key != "GIVESORA_DOUBLEFLIGHT" && y.Value.Any(z => z.Value.Contains("DOUBLEFLIGHT"))).Key;
                var doubleFlight = randomizedOptions[doubleFlightCategory][doubleFlightSubCategory].FirstOrDefault(z => z.Value.Contains("DOUBLEFLIGHT"));
                var doubleFlightOption = new Option { Category = doubleFlightCategory, SubCategory = doubleFlightSubCategory, Name = doubleFlight.Key, Value = doubleFlight.Value };

                // Swap these options
                var doubleFlightOptionValue = randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["EquipAbility1"];
                randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["EquipAbility1"] = doubleFlightOption.Value;
                randomizedOptions[doubleFlightOption.Category][doubleFlightOption.SubCategory][doubleFlightOption.Name] = doubleFlightOptionValue;
            }


            // Default Air Slide
            if (!containsAirSlide)
            {
                var airSlideCategory = randomizedOptions.FirstOrDefault(x => x.Value.Any(y => y.Key != "GIVESORA_AIRSLIDE" && y.Value.Any(z => z.Value.Contains("AIRSLIDE")))).Key;
                var airSlideSubCategory = randomizedOptions[airSlideCategory].FirstOrDefault(y => y.Key != "GIVESORA_AIRSLIDE" && y.Value.Any(z => z.Value.Contains("AIRSLIDE"))).Key;
                var airSlide = randomizedOptions[airSlideCategory][airSlideSubCategory].FirstOrDefault(z => z.Value.Contains("AIRSLIDE"));
                var airSlideOption = new Option { Category = airSlideCategory, SubCategory = airSlideSubCategory, Name = airSlide.Key, Value = airSlide.Value };

                // Swap these options
                var airSlideOptionValue = randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["EquipAbility2"];
                randomizedOptions[DataTableEnum.ChrInit]["m_PlayerSora"]["EquipAbility2"] = airSlideOption.Value;
                randomizedOptions[airSlideOption.Category][airSlideOption.SubCategory][airSlideOption.Name] = airSlideOptionValue;
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
                        if (!canUseNone && value.Contains("NONE"))
                            continue;
                        else if (name == "TypeB" || name == "TypeC")
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

            //if (options.Count == 0)
            //    return null;

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
                else if (option.Name == "TypeB" || option.Name == "TypeC")
                    continue;
                else if (option.Category == DataTableEnum.ChrInit && option.Name == "Weapon" && options[option.Category][option.SubCategory].Count > 1)
                    continue;

                option.Found = true;
            }

            return option;
        }

        public Option RetrieveRandomOption(Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> options, Random random, Dictionary<string, RandomizeOptionEnum> randomizePools, string categoryNeeded, DataTableEnum category, bool canUseNone, bool useVerifyImportantCheck = true)
        {
            var option = new Option();

            // Loop through all items
            for (int i = 0; i < 1000; ++i)
            {
                try
                {
                    option.Category = options.ElementAt(random.Next(0, options.Count)).Key;
                    if (options[option.Category].Count == 0 || option.Category == category)
                        continue;

                    option.SubCategory = options[option.Category].ElementAt(random.Next(0, options[option.Category].Count)).Key;
                    if (option.SubCategory.Contains("GIVESORA") || !randomizePools.ContainsKey(this.GetPoolFromOption(option.Category, option.SubCategory)))
                        continue;

                    if (options[option.Category][option.SubCategory].Count == 0)
                        continue;

                    option.Name = options[option.Category][option.SubCategory].ElementAt(random.Next(0, options[option.Category][option.SubCategory].Count)).Key;
                    option.Value = options[option.Category][option.SubCategory][option.Name];


                    if (option.Value.Contains("NONE") && !canUseNone)
                        continue;
                    else if (option.Name == "TypeB" || option.Name == "TypeC")
                        continue;
                    else if (option.Category == DataTableEnum.ChrInit && option.Name == "Weapon")
                        continue;
                    else if (categoryNeeded == "Ability" && !option.Value.Contains("ETresAbilityKind::"))
                        continue;
                    else if (categoryNeeded == "Item" && (option.Value.Contains("Ability") || option.Value.Contains("Bonus") || option.Value.Contains("NONE")))
                        continue;
                    else if (categoryNeeded == "Keyblade" && (!option.Value.Contains("KEYBLADE") || option.Value.Contains("KEYBLADE_SO_018") || option.Value.Contains("KEYBLADE_SO_019")))
                        continue;

                    if (useVerifyImportantCheck && this.VerifyImportantCheck(option.Value, options))
                        continue;

                    option.Found = true;
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    break;
                }
            }

            if (!option.Found)
                Console.WriteLine($"OPTION NOT FOUND: {option.Category} - {option.SubCategory} - {option.Name} - {option.Value}");

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

        public string RetrieveCategoryNeeded(DataTableEnum category, string name, bool replace = false)
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
                    if (replace)
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

        public async Task<byte[]> GenerateRandomizerSeed(string currentSeed, Pool poolConfiguration, QoL qolConfiguration, Hint hintConfiguration, ClearCondition clearConditionConfiguration, List<Tuple<Option, Option>> modifications)
        {
            var worldPools = this.GetPools(poolConfiguration.Selections, "World");
            var miscellaneousPools = this.GetPools(poolConfiguration.Selections, "Miscellaneous");
            var statPools = this.GetPools(poolConfiguration.Selections, "Stats");
            var enemyPools = this.GetPools(poolConfiguration.Selections, "Enemy");
            var partyPools = this.GetPools(poolConfiguration.Selections, "Party Members");

            // Combine Separated Dictionaries
            var combinedDictionary = worldPools.ToDictionary(x => x.Key, y => y.Value);
            miscellaneousPools.ToList().ForEach(x => combinedDictionary.Add(x.Key, x.Value));

            var randomizedItems = this.Process(currentSeed, combinedDictionary, poolConfiguration.Exceptions, poolConfiguration.Limiters, poolConfiguration.Exceptions["Can Be None"]);

            // Update with Stats
            var randomizedStats = this.ProcessStats(currentSeed, statPools, poolConfiguration.Exceptions);

            foreach (var (randomizedCategory, randomizedStatSubCategories) in randomizedStats)
            {
                foreach (var (randomizedSubCategory, randomizedValues) in randomizedStatSubCategories)
                {
                    foreach (var (name, value) in randomizedValues)
                    {
                        if (!randomizedItems.ContainsKey(randomizedCategory))
                            randomizedItems.Add(randomizedCategory, new Dictionary<string, Dictionary<string, string>>());

                        if (!randomizedItems[randomizedCategory].ContainsKey(randomizedSubCategory))
                            randomizedItems[randomizedCategory].Add(randomizedSubCategory, new Dictionary<string, string>());

                        if (!randomizedItems[randomizedCategory][randomizedSubCategory].ContainsKey(name))
                            randomizedItems[randomizedCategory][randomizedSubCategory].Add(name, "");

                        randomizedItems[randomizedCategory][randomizedSubCategory][name] = value;
                    }
                }
            }

            // Add Exp Multiplier
            if (randomizedItems.ContainsKey(DataTableEnum.EXP))
            {
                randomizedItems[DataTableEnum.EXP]["EXP"]["Multiplier"] = poolConfiguration.EXPMultiplier.ToString();
            }
            else
            {
                randomizedItems.Add(DataTableEnum.EXP, new Dictionary<string, Dictionary<string, string>>());
                randomizedItems[DataTableEnum.EXP].Add("EXP", new Dictionary<string, string>());
                randomizedItems[DataTableEnum.EXP]["EXP"].Add("Multiplier", poolConfiguration.EXPMultiplier.ToString());
            }


            // Randomize Enemies
            var randomizedEnemies = this.ProcessEnemies(currentSeed, enemyPools, poolConfiguration.Exceptions);


            // Randomize Bosses
            var randomizedBosses = this.ProcessBosses(currentSeed, enemyPools, poolConfiguration.Exceptions);


            // Randomize Party Members
            var randomizedPartyMembers = this.ProcessPartyMembers(currentSeed, partyPools, poolConfiguration.Exceptions);


            // Generate Hints
            Dictionary<string, List<string>> hintValues = new();

            byte[] hintResults = this.HintService.GenerateHints(currentSeed, randomizedItems, poolConfiguration.Selections, hintConfiguration.Type, hintConfiguration.Checks, ref hintValues);


            // Generate QoL Settings
            Dictionary<string, bool> qolValues = new();

            foreach (var (category, values) in qolConfiguration.Selections)
            {
                foreach (var (name, active) in values)
                {
                    var id = name.QoLKeyToId();

                    qolValues.Add(id, active);
                }
            }

            var dataTableManager = new UE4DataTableInterpreter.DataTableManager();
            var tasks = new List<Task>();
            
            var dataTables = new Dictionary<string, List<byte>>(); // Roughly a minute calculation
            var enemies = new Dictionary<string, List<byte>>(); // Roughly 2 minutes calculation
            var partyMembers = new Dictionary<string, List<byte>>(); // < a couple second calculation
            var bosses = new Dictionary<string, List<byte>>(); // < a couple second calculation
            var hintDataTable = new Dictionary<string, List<byte>>(); // < a couple second calculation
            var qolDataTable = new Dictionary<string, List<byte>>(); // < a couple second calculation
            var clearConditionsDataTable = new Dictionary<string, List<byte>>(); // < a couple second calculation
            var pandorasPowerKeyblade = new Dictionary<string, List<byte>>(); // < a couple second calculation

            tasks.Add(Task.Run(async () => { try { dataTables = await dataTableManager.RandomizeDataTables(randomizedItems, qolValues["ITEM_004"]); } catch (Exception ex) { Console.WriteLine(ex); } }));
            tasks.Add(Task.Run(async () => { try { enemies = await dataTableManager.RandomizeEnemies(randomizedEnemies, currentSeed, poolConfiguration.Exceptions["Enemy Chaos"]); } catch (Exception ex) { Console.WriteLine(ex); } } ));
            tasks.Add(Task.Run(async () => { try { bosses = await dataTableManager.RandomizeBosses(randomizedBosses); } catch (Exception ex) { Console.WriteLine(ex); } }));
            tasks.Add(Task.Run(async () => { try { partyMembers = await dataTableManager.RandomizePartyMembers(randomizedPartyMembers, poolConfiguration.Exceptions["Default Donald & Goofy"], poolConfiguration.Exceptions["Party Chaos"]); } catch (Exception ex) { Console.WriteLine(ex); } } ));
            tasks.Add(Task.Run(async () => { try { hintDataTable = await dataTableManager.GenerateHintDataTable(hintValues); } catch (Exception ex) { Console.WriteLine(ex); } } ));
            tasks.Add(Task.Run(async () => { try { qolDataTable = await dataTableManager.GenerateQualityOfLifeDataTable(qolValues); } catch (Exception ex) { Console.WriteLine(ex); } }));
            tasks.Add(Task.Run(async () => { try { clearConditionsDataTable = await dataTableManager.GenerateCompletionConditionsDataTable(clearConditionConfiguration.Conditions); } catch (Exception ex) { Console.WriteLine(ex); } }));
            tasks.Add(Task.Run(async () => { try { pandorasPowerKeyblade = await dataTableManager.GeneratePandorasPowerKeyblade(currentSeed); } catch (Exception ex) { Console.WriteLine(ex); } }));

            await Task.WhenAll(tasks.ToArray());

            var zipArchive = this.CreateZipArchive(currentSeed, dataTables, enemies, bosses, partyMembers, poolConfiguration.Selections, modifications, poolConfiguration.Exceptions, poolConfiguration.Limiters, hintResults, hintDataTable, qolDataTable, clearConditionsDataTable, pandorasPowerKeyblade);

            return zipArchive;

            //var zipPath = Path.Combine(Environment.CurrentDirectory, @$"Seeds/pakchunk99-randomizer-{currentSeed}/pakchunk99-randomizer-{currentSeed}.zip");
            //ZipFile.ExtractToDirectory(zipPath, Path.Combine(Environment.CurrentDirectory, @$"Seeds/pakchunk99-randomizer-{currentSeed}"));

            //if (File.Exists(zipPath))
            //    File.Delete(zipPath);

            //var pakFile = Path.Combine(Environment.CurrentDirectory, @$"Seeds/pakchunk99-randomizer-{currentSeed}.pak");
            //if (File.Exists(pakFile))
            //    File.Delete(pakFile);

            //var pakPath = Path.Combine(Environment.CurrentDirectory, @$"wwwroot/pak/Seeds/pakchunk99-randomizer-{currentSeed}");
            //if (Directory.Exists(pakPath))
            //    Directory.Delete(pakPath, true);

            //Directory.Move(Path.Combine(Environment.CurrentDirectory, @$"Seeds/pakchunk99-randomizer-{currentSeed}"), pakPath);
            
            //pakPath.ExecuteCommand();

            //Directory.Move($"{pakPath}.pak", pakFile);

            //return new List<byte[]> { this.GetFile(pakFile), this.GetFile(@$"{pakPath}/SpoilerLog.json") };
        }

        public byte[] CreateZipArchive(string randomSeed, Dictionary<string, List<byte>> dataTables, 
                                       Dictionary<string, List<byte>> enemies, Dictionary<string, List<byte>> bosses, Dictionary<string, List<byte>> partyMembers,
                                       Dictionary<string, RandomizeOptionEnum> availablePools, List<Tuple<Option, Option>> modifications, 
                                       Dictionary<string, bool> exceptions, Dictionary<string, int> limiters,
                                       byte[] hints, Dictionary<string, List<byte>> hintDataTable, 
                                       Dictionary<string, List<byte>> qolDataTable,
                                       Dictionary<string, List<byte>> clearConditionsDataTable,
                                       Dictionary<string, List<byte>> pandorasPowerKeyblades)
        {
            var zipPath = Path.Combine(Environment.CurrentDirectory, @$"Seeds/pakchunk99-randomizer-{randomSeed}/pakchunk99-randomizer-{randomSeed}.zip");

            var directory = Path.Combine(Environment.CurrentDirectory, @$"Seeds/pakchunk99-randomizer-{randomSeed}");
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);

            // Create the ZIP Archive
            Directory.CreateDirectory(directory);

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
                    Limiters = limiters,
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


                // Create Clear Conditions
                foreach (var (filePathAndName, data) in clearConditionsDataTable)
                {
                    var dataTableEntry = archive.CreateEntry(filePathAndName);
                    using var memoryStream = new MemoryStream(data.ToArray());
                    using var stream = dataTableEntry.Open();

                    memoryStream.CopyTo(stream);
                }


                // Do not include these enemy files
                var excludeEnemyList = new List<string> {
                    "he_01_enemy_01", "he_01_enemy_02",
                    "ra_01_enemy",
                    "ts_02_enemy_01", "ts_02_enemy_04", "ts_02_enemy_04_Amber",
                    "mi_02_enemy_05", "mi_04_enemy_01",
                    "fz_01_enemy_15", "fz_01_enemy_16", "fz_01_enemy_65",
                    "ca_02_island01_mission_bigfishbattle",
                    "bx_02_enemy_04",
                    "kg_01_enemy_01"
                };

                // Create Enemies
                foreach (var (filePathAndName, data) in enemies)
                {
                    // Include the period (.) so that we get all strings that contain that file name
                    if (excludeEnemyList.Any(x => filePathAndName.Contains($"{x}.")))
                        continue;

                    var dataTableEntry = archive.CreateEntry(filePathAndName);
                    using var memoryStream = new MemoryStream(data.ToArray());
                    using var stream = dataTableEntry.Open();

                    memoryStream.CopyTo(stream);
                }


                // Create Bosses
                foreach (var (filePathAndName, data) in bosses)
                {
                    var dataTableEntry = archive.CreateEntry(filePathAndName);
                    using var memoryStream = new MemoryStream(data.ToArray());
                    using var stream = dataTableEntry.Open();

                    memoryStream.CopyTo(stream);
                }


                // Create PartyMembers
                foreach (var (filePathAndName, data) in partyMembers)
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

                // Create Pandora's Power Keyblade
                foreach (var (filePathAndName, data) in pandorasPowerKeyblades)
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
            //Directory.Delete($@"./wwwroot/pak/Seeds/pakchunk99-randomizer-{currentSeed}", true);
            //File.Delete($@"./Seeds/pakchunk99-randomizer-{currentSeed}.pak");
        }



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
                    case "Re+Mind":
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
                            { "Weapon Upgrades: Classic Tone", true }, { "Weapon Upgrades: Ultima Weapon", true }, { "Weapon Upgrades: Pandora's Power", true }, { "Weapon Upgrades: Oblivion", true },
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

                    // Stats
                    case "Base Sora Stats":
                        var baseStatOptions = new Dictionary<string, bool> {
                            { "Sora", true }
                        };

                        availableOptions.Add(pool.Key, baseStatOptions);

                        break;

                    case "Level Up Stats":
                        var levelUpStatOptions = new Dictionary<string, bool> {
                            { "Level Ups", true }
                        };

                        availableOptions.Add(pool.Key, levelUpStatOptions);

                        break;

                    case "Keyblade Enhance Stats":
                        var weaponUpgradeStatOptions = new Dictionary<string, bool> {
                            { "Keyblade Enhance Stats", true },
                            // Weapon Upgrades
                            { "Keyblade Enhance Stats: Kingdom Key", true }, { "Keyblade Enhance Stats: Hero's Origin", true }, { "Keyblade Enhance Stats: Shooting Star", true }, { "Keyblade Enhance Stats: Favorite Deputy", true },
                            { "Keyblade Enhance Stats: Ever After", true }, { "Keyblade Enhance Stats: Happy Gear", true }, { "Keyblade Enhance Stats: Crystal Snow", true }, { "Keyblade Enhance Stats: Hunny Spout", true },
                            { "Keyblade Enhance Stats: Wheel of Fate", true }, { "Keyblade Enhance Stats: Nano Gear", true }, { "Keyblade Enhance Stats: Starlight", true }, { "Keyblade Enhance Stats: Grand Chef", true },
                            { "Keyblade Enhance Stats: Classic Tone", true }, { "Keyblade Enhance Stats: Ultima Weapon", true }, { "Keyblade Enhance Stats: Pandora's Power", true }, { "Keyblade Enhance Stats: Oblivion", true },
                            { "Keyblade Enhance Stats: Oathkeeper", true }
                        };

                        availableOptions.Add(pool.Key, weaponUpgradeStatOptions);

                        break;

                    case "Equipment Stats":
                        var equipmentStatOptions = new Dictionary<string, bool>
                        {
                            { "Weapons", true }, { "Accessories", true }, { "Armor", true }
                        };

                        availableOptions.Add(pool.Key, equipmentStatOptions);

                        break;

                    case "Food Effect Stats":
                        var foodStatOptions = new Dictionary<string, bool>
                        {
                            { "Starters", true }, { "Soups", true }, { "Fish", true }, { "Meat", true }, { "Desserts", true }
                        };

                        availableOptions.Add(pool.Key, foodStatOptions);

                        break;

                    case "EXP":
                        var expStatOptions = new Dictionary<string, bool>
                        {
                            { "EXP Multiplier", true }
                        };

                        availableOptions.Add(pool.Key, expStatOptions);

                        break;

                    default:
                        break;
                }
            }

            return availableOptions;
        }


        public Dictionary<string, Dictionary<string, bool>> GetAvailableEnemies(Dictionary<string, Enemy> randomizedEnemies, ref Dictionary<string, Dictionary<string, bool>> availableEnemies, bool backTo = false)
        {
            if (backTo)
                return availableEnemies;

            availableEnemies = new();

            foreach (var (name, enemy) in randomizedEnemies)
            {
                var worldPrefix = name.Split(" - ")[1].Substring(0, 2);

                if (!availableEnemies.ContainsKey(worldPrefix))
                    availableEnemies.Add(worldPrefix, new Dictionary<string, bool>() { { worldPrefix, true } });

                if (name.Contains("(Boss)"))
                {
                    var bossesKey = $"{worldPrefix}: Bosses";
                    if (!availableEnemies[worldPrefix].ContainsKey(bossesKey))
                        availableEnemies[worldPrefix].Add(bossesKey, true);
                }
                else if (name.Contains("(Mini-Boss)"))
                {
                    var miniBossesKey = $"{worldPrefix}: Mini-Bosses";
                    if (!availableEnemies[worldPrefix].ContainsKey(miniBossesKey))
                        availableEnemies[worldPrefix].Add(miniBossesKey, true);
                }
                else
                {
                    var mobKey = $"{worldPrefix}: Mobs";
                    if (!availableEnemies[worldPrefix].ContainsKey(mobKey))
                        availableEnemies[worldPrefix].Add(mobKey, true);
                }
            }

            return availableEnemies;
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
                var swapDataTable = randomizedOptions.ElementAt(rng.Next(0, randomizedOptions.Count));
                var swapCategory = swapDataTable.Value.ElementAt(rng.Next(0, randomizedOptions[swapDataTable.Key].Count));

                if (!availableOptions[swapDataTable.Key.DataTableEnumToKey()][swapCategory.Key.CategoryToKey(swapDataTable.Key)])
                    continue;

                if (swapCategory.Value.Where(x => x.Value.Contains("NONE")).Any())
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


        #region Helper Methods

        public Dictionary<string, RandomizeOptionEnum> GetPools(Dictionary<string, RandomizeOptionEnum> pools, string category)
        {
            Dictionary<string, RandomizeOptionEnum> tempPools = new();

            if (category == "World")
            {
                tempPools = pools.Where(x => x.Key == "Olympus" || x.Key == "Twilight Town" || x.Key == "Toy Box" || x.Key == "Kingdom of Corona" ||
                                        x.Key == "Monstropolis" || x.Key == "Arendelle" || x.Key == "The Caribbean" || x.Key == "San Fransokyo" ||
                                        x.Key == "100 Acre Wood" || x.Key == "Keyblade Graveyard" || x.Key == "Re+Mind" || x.Key == "Dark World" ||
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
            else if (category == "Stats")
            {
                tempPools = pools.Where(x => x.Key == "Base Sora Stats" || x.Key == "Level Up Stats" || x.Key == "Equipment Stats" ||
                                        x.Key == "Keyblade Enhance Stats" || x.Key == "Food Effect Stats" || x.Key == "EXP Multiplier")
                                .ToDictionary(x => x.Key, y => y.Value);
            }
            else if (category == "Enemy")
            {
                tempPools = pools.Where(x => x.Key == "Enemies" || x.Key == "Bosses")
                                .ToDictionary(x => x.Key, y => y.Value);
            }
            else if (category == "Party Members")
            {
                tempPools = pools.Where(x => x.Key == "Party Members")
                                .ToDictionary(x => x.Key, y => y.Value);
            }

            return tempPools;
        }

        public Tuple<string, string> GetLevels(string subCategory)
        {
            return subCategory switch
            {
                "2"  => new Tuple<string, string>("6",   "6"),
                "4"  => new Tuple<string, string>("14", "16"),
                "6"  => new Tuple<string, string>("4",  "12"),
                "9"  => new Tuple<string, string>("30", "24"),
                "12" => new Tuple<string, string>("44", "18"),
                "14" => new Tuple<string, string>("9",  "16"),
                "16" => new Tuple<string, string>("40", "32"),
                "18" => new Tuple<string, string>("12", "28"),
                "20" => new Tuple<string, string>("46", "36"),
                "24" => new Tuple<string, string>("50", "42"),
                "26" => new Tuple<string, string>("28",  "4"),
                "28" => new Tuple<string, string>("14", "26"),
                "30" => new Tuple<string, string>("18", "38"),
                "32" => new Tuple<string, string>("48", "34"),
                "34" => new Tuple<string, string>("34", "40"),
                "36" => new Tuple<string, string>("36",  "9"),
                "38" => new Tuple<string, string>("38", "48"),
                "40" => new Tuple<string, string>("24", "20"),
                "42" => new Tuple<string, string>("20", "44"),
                "44" => new Tuple<string, string>("2",  "46"),
                "46" => new Tuple<string, string>("26", "50"),
                "48" => new Tuple<string, string>("42", "30"),
                "50" => new Tuple<string, string>("32",  "2"),
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
                        eventList = new List<string> { "EVENT_001", "EVENT_002", "EVENT_003", "EVENT_004", "EVENT_005", "EVENT_006", "EVENT_007", "EVENT_KEYBLADE_001" };
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
                        bonusList = new List<string> { "Vbonus_058", "Vbonus_059", "Vbonus_060", "Vbonus_061", "Vbonus_062", "Vbonus_063", "Vbonus_064", "Vbonus_065", "Vbonus_066" };
                        break;
                    case "Keyblade Graveyard":
                        bonusList = new List<string> { "Vbonus_068", "Vbonus_069", "Vbonus_070", "Vbonus_071", "Vbonus_072", "Vbonus_073", "Vbonus_074", "Vbonus_075", "Vbonus_076", "Vbonus_083", "Vbonus_084" };
                        break;
                    case "Re+Mind":
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
                else if (currentSelection.Equals("Pandora's Power"))
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
                        fullcourseSubsets = fullcourse.Value.Where(x => x.Key == "Ability").ToDictionary(x => x.Key, y => y.Value);

                    foreach (var subset in fullcourseSubsets)
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
                        luckyEmblemSubsets = luckyEmblem.Value.Where(x => x.Key == "Reward").ToDictionary(x => x.Key, y => y.Value);

                    foreach (var subset in luckyEmblemSubsets)
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

        public List<Event> GetAvailableAlwaysOnEvents(ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
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

        #region Stats

        public List<BaseCharStat> GetAvailableBaseCharStats(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var baseStats = new List<BaseCharStat>();

            if (randomizedOptions.ContainsKey(DataTableEnum.BaseCharStat))
            {
                foreach (var baseStat in randomizedOptions[DataTableEnum.BaseCharStat])
                {
                    baseStats.Add(new BaseCharStat
                    {
                        Player = baseStat.Key,
                        HP = baseStat.Value.GetValueOrDefault("MaxHitPoint", ""),
                        MP = baseStat.Value.GetValueOrDefault("MaxMagicPoint", ""),
                        FP = baseStat.Value.GetValueOrDefault("MaxFocusPoint", ""),
                        AP = baseStat.Value.GetValueOrDefault("AbilityPoint", ""),
                        Attack = baseStat.Value.GetValueOrDefault("AttackPower", ""),
                        Magic = baseStat.Value.GetValueOrDefault("MagicPower", ""),
                        Defense = baseStat.Value.GetValueOrDefault("DefensePower", "")
                    });
                }
            }

            return baseStats;
        }

        public List<LevelUpStat> GetAvailableLevelUpStats(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var levelUpStats = new List<LevelUpStat>();

            if (randomizedOptions.ContainsKey(DataTableEnum.LevelUpStat))
            {
                foreach (var levelUpStat in randomizedOptions[DataTableEnum.LevelUpStat])
                {
                    levelUpStats.Add(new LevelUpStat
                    {
                        Level = levelUpStat.Key,
                        AP = levelUpStat.Value.GetValueOrDefault("AbilityPoint", ""),
                        Attack = levelUpStat.Value.GetValueOrDefault("AttackPower", ""),
                        Magic = levelUpStat.Value.GetValueOrDefault("MagicPower", ""),
                        Defense = levelUpStat.Value.GetValueOrDefault("DefensePower", "")
                    });
                }
            }

            return levelUpStats;
        }

        public List<WeaponUpgradeStat> GetAvailableWeaponUpgradeStats(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var weaponUpgradeStats = new List<WeaponUpgradeStat>();

            if (randomizedOptions.ContainsKey(DataTableEnum.WeaponEnhanceStat))
            {
                var weaponUpgradeStatSubsets = new Dictionary<string, Dictionary<string, string>>();

                if (currentSelection.Equals("Kingdom Key"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) < 10).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Shooting Star"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 10 && int.Parse(x.Key.Split('_')[1]) < 20).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Hero's Origin"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 20 && int.Parse(x.Key.Split('_')[1]) < 30).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Favorite Deputy"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 30 && int.Parse(x.Key.Split('_')[1]) < 40).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Ever After"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 40 && int.Parse(x.Key.Split('_')[1]) < 50).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Wheel of Fate"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 50 && int.Parse(x.Key.Split('_')[1]) < 60).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Crystal Snow"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 60 && int.Parse(x.Key.Split('_')[1]) < 70).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Hunny Spout"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 70 && int.Parse(x.Key.Split('_')[1]) < 80).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Nano Gear"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 80 && int.Parse(x.Key.Split('_')[1]) < 90).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Happy Gear"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 90 && int.Parse(x.Key.Split('_')[1]) < 100).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Classic Tone"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 100 && int.Parse(x.Key.Split('_')[1]) < 110).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Grand Chef"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 110 && int.Parse(x.Key.Split('_')[1]) < 120).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Ultima Weapon"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 120 && int.Parse(x.Key.Split('_')[1]) < 130).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Pandora's Power"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 150 && int.Parse(x.Key.Split('_')[1]) < 160).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Starlight"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 160 && int.Parse(x.Key.Split('_')[1]) < 170).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Oathkeeper"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 170 && int.Parse(x.Key.Split('_')[1]) < 180).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Oblivion"))
                    weaponUpgradeStatSubsets = randomizedOptions[DataTableEnum.WeaponEnhanceStat].Where(x => int.Parse(x.Key.Split('_')[1]) >= 180 && int.Parse(x.Key.Split('_')[1]) < 190).ToDictionary(x => x.Key, y => y.Value);

                foreach (var weaponUpgradeStat in weaponUpgradeStatSubsets)
                {
                    weaponUpgradeStats.Add(new WeaponUpgradeStat 
                    { 
                        Id = weaponUpgradeStat.Key,
                        AttackPlus = weaponUpgradeStat.Value.GetValueOrDefault("AttackPlus", ""),
                        MagicPlus = weaponUpgradeStat.Value.GetValueOrDefault("MagicPlus", ""),
                    });
                }
            }

            return weaponUpgradeStats;
        }

        public List<EquipmentStat> GetAvailableEquipmentStats(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var equipmentStats = new List<EquipmentStat>();

            if (randomizedOptions.ContainsKey(DataTableEnum.EquipItemStat))
            {
                var equipmentSubsets = new Dictionary<string, Dictionary<string, string>>();

                if (currentSelection.Equals("Weapons"))
                    equipmentSubsets = randomizedOptions[DataTableEnum.EquipItemStat].Where(x => x.Key.Contains("I03")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Armor"))
                    equipmentSubsets = randomizedOptions[DataTableEnum.EquipItemStat].Where(x => x.Key.Contains("I04")).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Accessories"))
                    equipmentSubsets = randomizedOptions[DataTableEnum.EquipItemStat].Where(x => x.Key.Contains("I05")).ToDictionary(x => x.Key, y => y.Value);

                foreach (var subset in equipmentSubsets)
                {
                    equipmentStats.Add(new EquipmentStat
                    {
                        EquipItem = subset.Key,
                        AP = subset.Value.GetValueOrDefault("AP", ""),
                        AttackPlus = subset.Value.GetValueOrDefault("AttackPlus", ""),
                        MagicPlus = subset.Value.GetValueOrDefault("MagicPlus", ""),
                        DefensePlus = subset.Value.GetValueOrDefault("DefensePlus", ""),
                        PhysicalResistance = subset.Value.GetValueOrDefault("AttrResistPhysical", ""),
                        FireResistance = subset.Value.GetValueOrDefault("AttrResistFire", ""),
                        BlizzardResistance = subset.Value.GetValueOrDefault("AttrResistBlizzard", ""),
                        ThunderResistance = subset.Value.GetValueOrDefault("AttrResistThunder", ""),
                        WaterResistance = subset.Value.GetValueOrDefault("AttrResistWater", ""),
                        AeroResistance = subset.Value.GetValueOrDefault("AttrResistAero", ""),
                        DarkResistance = subset.Value.GetValueOrDefault("AttrResistDark", ""),
                        NullResistance = subset.Value.GetValueOrDefault("AttrResistNoType", "")
                    });
                }
            }

            return equipmentStats;
        }

        public List<FoodStat> GetAvailableFoodStats(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var foodStats = new List<FoodStat>();

            if (randomizedOptions.ContainsKey(DataTableEnum.FoodItemEffectStat))
            {
                var foodSubsets = new Dictionary<string, Dictionary<string, string>>();

                if (currentSelection.Equals("Soups"))
                    foodSubsets = randomizedOptions[DataTableEnum.FoodItemEffectStat]
                        .Where(x => (int.Parse(x.Key[^2..]) < 6) ||
                                (int.Parse(x.Key[^2..]) >= 29 && int.Parse(x.Key[^2..]) < 34)).ToDictionary(x => x.Key, y => y.Value);
                else if(currentSelection.Equals("Starters"))
                    foodSubsets = randomizedOptions[DataTableEnum.FoodItemEffectStat]
                        .Where(x => (int.Parse(x.Key[^2..]) >= 6 && int.Parse(x.Key[^2..]) < 11) ||
                                (int.Parse(x.Key[^2..]) >= 34 && int.Parse(x.Key[^2..]) < 39)).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Fish"))
                    foodSubsets = randomizedOptions[DataTableEnum.FoodItemEffectStat]
                        .Where(x => (int.Parse(x.Key[^2..]) >= 11 && int.Parse(x.Key[^2..]) < 17) ||
                                (int.Parse(x.Key[^2..]) >= 39 && int.Parse(x.Key[^2..]) < 45)).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Meat"))
                    foodSubsets = randomizedOptions[DataTableEnum.FoodItemEffectStat]
                        .Where(x => (int.Parse(x.Key[^2..]) >= 17 && int.Parse(x.Key[^2..]) < 22) ||
                                (int.Parse(x.Key[^2..]) >= 45 && int.Parse(x.Key[^2..]) < 50)).ToDictionary(x => x.Key, y => y.Value);
                else if (currentSelection.Equals("Desserts"))
                    foodSubsets = randomizedOptions[DataTableEnum.FoodItemEffectStat]
                        .Where(x => (int.Parse(x.Key[^2..]) >= 22 && int.Parse(x.Key[^2..]) < 29) ||
                                (int.Parse(x.Key[^2..]) >= 50)).ToDictionary(x => x.Key, y => y.Value);


                foreach (var subset in foodSubsets)
                {
                    foodStats.Add(new FoodStat
                    {
                        Name = subset.Key,
                        HPPlus = subset.Value.GetValueOrDefault("MaxHPPlus", ""),
                        MPPlus = subset.Value.GetValueOrDefault("MaxMPPlus", ""),
                        AttackPlus = subset.Value.GetValueOrDefault("AttackPlus", ""),
                        MagicPlus = subset.Value.GetValueOrDefault("MagicPlus", ""),
                        DefensePlus = subset.Value.GetValueOrDefault("DefensePlus", "")
                    });
                }
            }

            return foodStats;
        }

        public List<EXPStat> GetAvailableEXPStats(string currentSelection, ref Dictionary<DataTableEnum, Dictionary<string, Dictionary<string, string>>> randomizedOptions)
        {
            var expStats = new List<EXPStat>();

            if (randomizedOptions.ContainsKey(DataTableEnum.EXP))
            {
                // Maybe in the future allow different EXP Rates per enemy? player?
                foreach (var subset in randomizedOptions[DataTableEnum.EXP])
                {
                    expStats.Add(new EXPStat
                    {
                        EXPMultiplier = subset.Value.GetValueOrDefault("Multiplier", ""),
                    });
                }
            }

            return expStats;
        }

        #endregion Stats

        public List<Enemy> GetAvailableWorldEnemies(string currentSelection, string currentSubSelection, Dictionary<string, Enemy> randomizedEnemies)
        {
            var enemies = new List<Enemy>();

            // Maybe in the future allow different EXP Rates per enemy? player?
            foreach (var subset in randomizedEnemies.Where(x => x.Key.Split(" - ")[1].StartsWith(currentSelection.WorldNameToWorldPrefix())))
            {
                if (currentSubSelection == "Bosses" && subset.Key.Contains("(Boss)"))
                {
                    subset.Value.Key = subset.Key;
                    enemies.Add(subset.Value);
                }
                else if (currentSubSelection == "Mini-Bosses" && subset.Key.Contains("(Mini-Boss)"))
                {
                    subset.Value.Key = subset.Key;
                    enemies.Add(subset.Value);
                }
                else if (currentSubSelection == "Mobs" && !subset.Key.Contains("(Mini-Boss)") && !subset.Key.Contains("(Boss)"))
                {
                    subset.Value.Key = subset.Key;
                    enemies.Add(subset.Value);
                }
            }

            return enemies;
        }


        // Should probably be in extensions
        private DataTableEnum GetDataTableEnumFromSelection(string selection)
        {
            return selection switch
            {
                "Olympus" => DataTableEnum.TreasureHE,
                "Twilight Town" => DataTableEnum.TreasureTT,
                "Kingdom of Corona" => DataTableEnum.TreasureRA,
                "Toy Box" => DataTableEnum.TreasureTS,
                "Arendelle" => DataTableEnum.TreasureFZ,
                "Monstropolis" => DataTableEnum.TreasureMI,
                "The Caribbean" => DataTableEnum.TreasureCA,
                "San Fransokyo" => DataTableEnum.TreasureBX,
                "Keyblade Graveyard" => DataTableEnum.TreasureKG,
                "The Final World" => DataTableEnum.TreasureEW,
                "Re+Mind" => DataTableEnum.TreasureBT,
                _ => DataTableEnum.None,
            };
        }

        // Should probably be in extensions
        private DataTableEnum ConvertDisplayStringToEnum(string displayString)
        {
            return displayString switch
            {
                "Starting Stats" => DataTableEnum.ChrInit,
                "Equippables" => DataTableEnum.EquipItem,
                "Events" => DataTableEnum.Event,
                "Fullcourse Abilities" => DataTableEnum.FullcourseAbility,
                "Level Ups" => DataTableEnum.LevelUp,
                "Lucky Emblems" => DataTableEnum.LuckyMark,
                "Bonuses" => DataTableEnum.VBonus,
                "Weapon Upgrades" => DataTableEnum.WeaponEnhance,
                "Synthesis Items" => DataTableEnum.SynthesisItem,
                _ => DataTableEnum.None,
            };
        }

        // Should probably be in extensions
        private string ConvertKeybladeWeaponToDefenseWeaponEnum(string keyblade)
        {
            var value = keyblade.ValueIdToDisplay();

            return value switch
            {
                "Kingdom Key" => "ETresItemDefWeapon::WEP_KEYBLADE00\u0000",
                "Hero's Origin" => "ETresItemDefWeapon::WEP_KEYBLADE02\u0000",
                "Shooting Star" => "ETresItemDefWeapon::WEP_KEYBLADE01\u0000",
                "Favorite Deputy" => "ETresItemDefWeapon::WEP_KEYBLADE03\u0000",
                "Ever After" => "ETresItemDefWeapon::WEP_KEYBLADE04\u0000",
                "Happy Gear" => "ETresItemDefWeapon::WEP_KEYBLADE09\u0000",
                "Crystal Snow" => "ETresItemDefWeapon::WEP_KEYBLADE06\u0000",
                "Hunny Spout" => "ETresItemDefWeapon::WEP_KEYBLADE07\u0000",
                "Nano Gear" => "ETresItemDefWeapon::WEP_KEYBLADE08\u0000",
                "Wheel of Fate" => "ETresItemDefWeapon::WEP_KEYBLADE05\u0000",
                "Grand Chef" => "ETresItemDefWeapon::WEP_KEYBLADE11\u0000",
                "Classic Tone" => "ETresItemDefWeapon::WEP_KEYBLADE10\u0000",
                "Oathkeeper" => "ETresItemDefWeapon::WEP_KEYBLADE12\u0000",
                "Oblivion" => "ETresItemDefWeapon::WEP_KEYBLADE13\u0000",
                "Ultima Weapon" => "ETresItemDefWeapon::WEP_KEYBLADE14\u0000",
                // Shouldn't be hit
                "Starlight" => "ETresItemDefWeapon::WEP_KEYBLADE17\u0000",
                // Shouldn't be hit
                "Pandora's Power" => "ETresItemDefWeapon::WEP_KEYBLADE18\u0000",
                _ => "ETresItemDefWeapon::WEP_KEYBLADE00\u0000",
            };
        }

        // Should probably be in extensions
        private string ConvertDefenseWeaponEnumToKeybladeWeapon(string keyblade)
        {
            var value = keyblade.ValueIdToDisplay();

            return value switch
            {
                "Kingdom Key" => "WEP_KEYBLADE_SO_00\u0000",
                "Hero's Origin" => "WEP_KEYBLADE_SO_01\u0000",
                "Shooting Star" => "WEP_KEYBLADE_SO_02\u0000",
                "Favorite Deputy" => "WEP_KEYBLADE_SO_03\u0000",
                "Ever After" => "WEP_KEYBLADE_SO_04\u0000",
                "Happy Gear" => "WEP_KEYBLADE_SO_05\u0000",
                "Crystal Snow" => "WEP_KEYBLADE_SO_06\u0000",
                "Hunny Spout" => "WEP_KEYBLADE_SO_07\u0000",
                "Nano Gear" => "WEP_KEYBLADE_SO_08\u0000",
                "Wheel of Fate" => "WEP_KEYBLADE_SO_09\u0000",
                "Grand Chef" => "WEP_KEYBLADE_SO_011\u0000",
                "Classic Tone" => "WEP_KEYBLADE_SO_012\u0000",
                "Oathkeeper" => "WEP_KEYBLADE_SO_013\u0000",
                "Oblivion" => "WEP_KEYBLADE_SO_014\u0000",
                "Ultima Weapon" => "WEP_KEYBLADE_SO_015\u0000",
                // Shouldn't be hit
                "Starlight" => "WEP_KEYBLADE_SO_018\u0000",
                // SHouldn't be hit
                "Pandora's Power" => "WEP_KEYBLADE_SO_019\u0000",
                _ => "WEP_KEYBLADE_SO_00\u0000",
            };
        }

        #endregion
    }
}