﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.GameData.Objects;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using VanillaPlusProfessions.Utilities;

namespace VanillaPlusProfessions.Talents.Patchers
{
    public static class MiscPatcher
    {
        readonly static Type PatcherType = typeof(MiscPatcher);
        readonly static string PatcherName = nameof(MiscPatcher);

        public static void ApplyPatches()
        {
            CoreUtility.PatchMethod(
                PatcherName, "Event.DefaultCommands.HospitalDeath",
                original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.HospitalDeath)),
                transpiler: new HarmonyMethod(PatcherType, nameof(Transpiler_2))
            );
            CoreUtility.PatchMethod(
                PatcherName, "Event.DefaultCommands.MineDeath",
                original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.MineDeath)),
                transpiler: new HarmonyMethod(PatcherType, nameof(Transpiler_2))
            );
            CoreUtility.PatchMethod(
                PatcherName, "GameLocation.tryAddPrismaticButterfly",
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.tryAddPrismaticButterfly)),
                postfix: new HarmonyMethod(PatcherType, nameof(tryAddPrismaticButterfly))
            );
            CoreUtility.PatchMethod(
                PatcherName, "Object.TryCreateBuffsFromData",
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.TryCreateBuffsFromData)),
                postfix: new HarmonyMethod(PatcherType, nameof(TryCreateBuffsFromData_Postfix))
            );
            CoreUtility.PatchMethod(
                PatcherName, "GameLocation.readNote",
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.readNote)),
                postfix: new HarmonyMethod(PatcherType, nameof(readNote))
            );
            CoreUtility.PatchMethod(
                PatcherName, "Chest.SetSpecialChestType",
                original: AccessTools.Method(typeof(Chest), nameof(Chest.SetSpecialChestType)),
                postfix: new HarmonyMethod(PatcherType, nameof(SetSpecialChestType_Postfix))
            );
            CoreUtility.PatchMethod(
                PatcherName, "Chest.ShowMenu",
                original: AccessTools.Method(typeof(Chest), nameof(Chest.ShowMenu)),
                transpiler: new HarmonyMethod(PatcherType, nameof(ShowMenu_Transpiler))
            );
            CoreUtility.PatchMethod(
                PatcherName, "Object constructor",
                original: AccessTools.Constructor(typeof(StardewValley.Object), new Type[] { typeof(string), typeof(int), typeof(bool), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(PatcherType, nameof(Object_Constructor_Postfix))
            );
            CoreUtility.PatchMethod(
                PatcherName, "Farmer.resetFriendshipsForNewDay",
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.resetFriendshipsForNewDay)),
                transpiler: new HarmonyMethod(PatcherType, nameof(Transpiler))
            );
        }

        public static void Object_Constructor_Postfix(ref StardewValley.Object __instance)
        {
            try
            {
                if (CoreUtility.CurrentPlayerHasProfession("Ranger") && !__instance.HasContextTag(TalentCore.ContextTag_Banned_Ranger)) //Ranger ++
                {
                    if (__instance.Category == StardewValley.Object.GreensCategory && __instance.HasContextTag("forage_item"))
                        __instance.Price *= 2;
                }
                if (CoreUtility.CurrentPlayerHasProfession("Adventurer") && !__instance.HasContextTag(TalentCore.ContextTag_Banned_Adventurer)) //Adventurer ++
                {
                    if (__instance.Category == StardewValley.Object.sellAtFishShopCategory || __instance.HasContextTag("forage_item_beach") || __instance.HasContextTag("forage_item_secret") || __instance.HasContextTag("forage_item_mines"))
                        __instance.Price *= 2;
                }
                if (TalentUtility.CurrentPlayerHasTalent("HauteCuisine") && Game1.activeClickableMenu is CraftingPage)
                {
                    if (__instance.Category == StardewValley.Object.CookingCategory)
                    {
                        __instance.Price *= 2;
                        __instance.Quality++;
                        __instance.FixQuality();
                    }
                }
                if (TalentUtility.CurrentPlayerHasTalent("Roemance"))
                {
                    if (__instance.ItemId is "812" or "447" or "445")
                        __instance.Price *= 5 / 4;
                }
                if (CoreUtility.CurrentPlayerHasProfession("Ironmonger"))
                {
                    if (__instance.HasContextTag("ore_item"))
                        __instance.Price *= 2;
                }
                if (TalentUtility.CurrentPlayerHasTalent("InsiderInfo"))
                {
                    Dictionary<string, string> InsiderInfo = ModEntry.Helper.GameContent.Load<Dictionary<string, string>>(ContentEditor.ContentPaths["InsiderInfo"]);
                    foreach (var item in InsiderInfo)
                    {
                        if (Game1.player.friendshipData.TryGetValue(item.Key ?? "", out Friendship val) && val.Points >= 1500)
                        {
                            string[] items = ArgUtility.SplitBySpace(item.Value.Replace(",", " "));
                            if (items.Contains(__instance.ItemId))
                            {
                                __instance.Price += (int)(__instance.Price * 0.2f);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, PatcherName, "Object Constuctor", "postfixed", true);
            }
        }

        public static IEnumerable<CodeInstruction> ShowMenu_Transpiler(IEnumerable<CodeInstruction> insns)
        {
            var list = insns.ToList();
            try
            {
                list.Reverse();
                foreach (var item in list)
                {
                    if (item.opcode == OpCodes.Ldnull)
                    {
                        item.opcode = OpCodes.Ldarg_0;
                        break;
                    }
                }
                list.Reverse();
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, PatcherName, "Chest.ShowMenu", "transpiling");
            }
            return list;
        }
        public static void SetSpecialChestType_Postfix(Chest __instance)
        {
            try
            {
                if (__instance.QualifiedItemId == "(BC)216" && TalentUtility.AnyPlayerHasTalent("MiniFridgeBigSpace"))
                {
                    __instance.SpecialChestType = Chest.SpecialChestTypes.BigChest;
                }
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, PatcherName, "Chest.SetSpecialChestType", "postfixed", true);
            }
        }
        public static string TrinketToString(this Trinket trinket)
        {
            return (trinket.GetEffect() as CompanionTrinketEffect).Variant.ToString();
        }
        public static Trinket StringToTrinket(this string trinket)
        {
            Trinket toReturn = new("FrogEgg", 11648541);
            if (toReturn.GetEffect() is not null and CompanionTrinketEffect effect && trinket.Length > 0)
            {
                effect.Variant = int.Parse(trinket);
            }
            return toReturn;
        }

        public static IEnumerable<CodeInstruction> Transpiler_2(IEnumerable<CodeInstruction> insns)
        {
            List<CodeInstruction> toReturn = insns.ToList();
            try
            {
                for (int i = 0; i < toReturn.Count; i++)
                {
                    if (toReturn[i].operand is not null and 15000 or 1000)
                    {
                        toReturn.Insert(i + 1, new(OpCodes.Call, AccessTools.Method(PatcherType, nameof(ReplaceGoldCost))));
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, PatcherName, "Event.DefaultCommands.HospitalDeath or Event.DefaultCommands.MineDeath", "transpiling");
            }           
            return toReturn;
        }

        public static int ReplaceGoldCost(int og) => TalentUtility.CurrentPlayerHasTalent("NarrowEscape") ? 200 : og; 
        
        public static void readNote(int which)
        {
            try
            {
                if (TalentUtility.CurrentPlayerHasTalent("LostAndFound") && Game1.netWorldState.Value.LostBooksFound >= which)
                {
                    string name = "";
                    int index = 0;
                    BuffEffects buffEffects = new();
                    switch (which)
                    {
                        case 0:  // 0 - Tips on Farming - +1 Farming
                            name = "Farming";
                            buffEffects.FarmingLevel.Value = 1;
                            index = 0;
                            break;
                        case 1: // 1 - A book by Marnie - +2 Farming?
                            name = "Farming";
                            buffEffects.FarmingLevel.Value = 2;
                            index = 0;
                            break;
                        case 2:  // 2 - On Foraging - +2 Foraging?
                            name = "Foraging";
                            buffEffects.ForagingLevel.Value = 2;
                            index = 5;
                            break;
                        case 3: // 3 - Fisherman act 1 - +1 Fishing?
                            name = "Fishing";
                            buffEffects.FishingLevel.Value = 1;
                            index = 1;
                            break;
                        case 4: // 4 - How deep the mines go? - +1 Mining 
                            name = "Mining";
                            buffEffects.MiningLevel.Value = 1;
                            index = 2;
                            break;
                        case 5:  // 5 - A note that hints on people will give you cooking recipes if you befriend them - Magnetism
                            name = "Magnetism";
                            buffEffects.MagneticRadius.Value = 40;
                            index = 8;
                            break;
                        case 6: // 6 - Scarecrows - +3 Farming
                            name = "Farming";
                            buffEffects.FarmingLevel.Value = 3;
                            index = 0;
                            break;
                        case 7: // 7 - The secret of Stardrop - Max Stamina?
                            name = "Max Stamina";
                            buffEffects.MaxStamina.Value = 40;
                            index = 16;
                            break;
                        case 8: // 8 - Journey of Prairie King - +2 Attack
                            name = "Attack";
                            buffEffects.Attack.Value = 2;
                            index = 11;
                            break;
                        case 9: // 9 - A study on Diamond yields - +3 Mining?
                            name = "Mining";
                            buffEffects.MiningLevel.Value = 3;
                            index = 2;
                            break;
                        case 10: // 10 - Brewmaster's Guide - +4 Farming
                            name = "Farming";
                            buffEffects.FarmingLevel.Value = 4;
                            index = 0;
                            break;
                        case 11:  // 11 - Mysteries of Dwarves - +3 Mining?
                            name = "Mining";
                            buffEffects.MiningLevel.Value = 3;
                            index = 2;
                            break;
                        case 12: // 12 - Hightlights from the book of yoba - +4 Foraging
                            name = "Foraging";
                            buffEffects.ForagingLevel.Value = 4;
                            index = 5;
                            break;
                        case 13: // 13 - Marriage Guide - +3 Luck
                            name = "Luck";
                            buffEffects.LuckLevel.Value = 3;
                            index = 4;
                            break;
                        case 14: // 14 - The fisherman act 2 - +3 Fishing?
                            name = "Fishing";
                            buffEffects.FishingLevel.Value = 3;
                            index = 1;
                            break;
                        case 15: // 15 - A note explaining how crystalariums work - +4 Mining?
                            name = "Mining";
                            buffEffects.MiningLevel.Value = 4;
                            index = 2;
                            break;
                        case 16: // 16 - Secrets of the Legendary Fish - +4 Fishing
                            name = "Fishing";
                            buffEffects.FishingLevel.Value = 4;
                            index = 1; 
                            break;
                        case 17: // 17 - A note that hints at Qi's casino quest - +4 Luck
                            name = "Luck";
                            buffEffects.LuckLevel.Value = 4;
                            index = 4;
                            break;
                        case 18: // 18 - Note From Gunther - +4 Speed
                            name = "Speed";
                            buffEffects.Speed.Value = 4;
                            index = 9;
                            break;
                        case 19: // 19 - Goblins by Jasper - +4 Defense
                            name = "Defense";
                            buffEffects.Defense.Value = 4;
                            index = 10;
                            break;
                        case 20: // 20 - Easter Egg book - +4 Speed
                            name = "Speed";
                            buffEffects.Speed.Value = 4;
                            index = 9;
                            break;
                        default:
                            break;
                    }
                    

                    Game1.player.buffs.Apply(new("Kedi.VPP.LostAndFound", "Lost Books", "Lost Books", -2, Game1.buffsIcons, index, buffEffects, false, name, ""));

                }
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, PatcherName, "GameLocation.readNote", "postfixed", true);
            }
        }

        public static void tryAddPrismaticButterfly(GameLocation __instance)
        {
            try
            {
                bool anyPrismaticButterflies = false;

                if (TalentUtility.HostHasTalent("ButterflyEffect") && !anyPrismaticButterflies && Game1.player.team.sharedDailyLuck.Value < -0.02)
                {
                    foreach (Critter critter in __instance.critters)
                    {
                        if (critter is VoidButterfly)
                        {
                            return;
                        }
                    }
                    Random r = Utility.CreateDaySaveRandom(Game1.player.UniqueMultiplayerID % 10000);
                    string[] possibleLocations = new string[] { "WitchSwamp", "BugLair", "Sewers", "PirateCove", "Railroad", "BusTunnel", "Mines121", "Caldera" };
                    string locationChoice = possibleLocations[r.Next(possibleLocations.Length)];
                    if (!__instance.Name.Equals(locationChoice))
                    {
                        return;
                    }
                    Vector2 prism_v = __instance.getRandomTile(r);
                    for (int i = 0; i < 32; i++)
                    {
                        if (__instance.isTileLocationOpen(prism_v))
                        {
                            break;
                        }
                        prism_v = __instance.getRandomTile(r);
                    }
                    __instance.critters.Add(new VoidButterfly(__instance, prism_v)
                    {
                        stayInbounds = true
                    });
                }

            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, PatcherName, "GameLocation.tryAddPrismaticButterfly", "postfixed", true);
            }
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> insns = new();
            try
            {
                foreach (var item in codeInstructions)
                {
                    insns.Add(item);
                    //Why is it so difficult to check for such simple numbers? ugh <.<
                    if (item.opcode == OpCodes.Ldc_I4_S && ((sbyte)item.operand == -2 || (sbyte)item.operand == -8 || (sbyte)item.operand == -20))
                    {
                        insns.Add(new(OpCodes.Call, AccessTools.Method(PatcherType, nameof(TryOverrideFriendshipDecay))));
                    }
                }
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, PatcherName, "Farmer.resetFriendshipsForNewDay", "transpiling", true);
            }
            return insns;
        }

        public static int TryOverrideFriendshipDecay(int oldDecay)
        {
            try
            {
                if (TalentUtility.CurrentPlayerHasTalent("Admiration"))
                {
                    return oldDecay / 2;
                }
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, PatcherName, $"{PatcherName}.TryOverrideFriendshipDecay", "transpiled", true);
            }
            return oldDecay;
        }

        public static void TryCreateBuffsFromData_Postfix(ref IEnumerable<Buff> __result, ObjectData obj, string name, string displayName, float durationMultiplier, Action<BuffEffects> adjustEffects)
        {
            try
            {
                if (TalentUtility.CurrentPlayerHasTalent("Misc_GoodEats") && obj?.Buffs?.Any() is true)
                {
                    List<Buff> buffs = new();
                    foreach (var item in obj.Buffs)
                    {
                        string bufftype = "";
                        var effects = new BuffEffects(item.CustomAttributes);
                        if (item.CustomAttributes is null)
                        {
                            bufftype = "CustomBuff_" + item.Id; 
                            goto IFCUSTOMATTRIBUTESISNULL;
                        }
                        
                        adjustEffects.Invoke(effects);

                        if (item.CustomAttributes.Immunity > 0)
                            bufftype = "immunity";
                        else if (item.CustomAttributes.MaxStamina > 0)
                            bufftype = "maxstamina";
                        else if (item.CustomAttributes.Attack > 0)
                            bufftype = "attack";
                        else if (item.CustomAttributes.Defense > 0)
                            bufftype = "defense";
                        else if (item.CustomAttributes.Speed > 0)
                            bufftype = "speed";
                        else if (item.CustomAttributes.MagneticRadius > 0)
                            bufftype = "magnet";

                        else if (item.CustomAttributes.WeaponPrecisionMultiplier > 0)
                            bufftype = "weaponprecision";
                        else if (item.CustomAttributes.WeaponSpeedMultiplier > 0)
                            bufftype = "weaponspeed";
                        else if (item.CustomAttributes.KnockbackMultiplier > 0)
                            bufftype = "knockback";
                        else if (item.CustomAttributes.AttackMultiplier > 0)
                            bufftype = "attackmultiplier";
                        else if (item.CustomAttributes.CriticalChanceMultiplier > 0)
                            bufftype = "critchancemultiplier";
                        else if (item.CustomAttributes.CriticalPowerMultiplier > 0)
                            bufftype = "critpowermultiplier";

                        else if (item.CustomAttributes.FarmingLevel > 0)
                            bufftype = "farming";
                        else if (item.CustomAttributes.FishingLevel > 0)
                            bufftype = "fishing";
                        else if (item.CustomAttributes.ForagingLevel > 0)
                            bufftype = "foraging";
                        else if (item.CustomAttributes.MiningLevel > 0)
                            bufftype = "mining";
                        else if (item.CustomAttributes.CombatLevel > 0)
                            bufftype = "combat";
                        else if (item.CustomAttributes.LuckLevel > 0)
                            bufftype = "luck";

                        IFCUSTOMATTRIBUTESISNULL:

                        Texture2D texture = null;
                        int spriteIndex = -1;
                        if (item.IconTexture != null)
                        {
                            texture = Game1.content.Load<Texture2D>(item.IconTexture);
                            spriteIndex = item.IconSpriteIndex;
                        }
                        int millisecondsDuration = -1;
                        if (item.Duration == -2)
                        {
                            millisecondsDuration = -2;
                        }
                        else if (item.Duration != 0)
                        {
                            millisecondsDuration = (int)(item.Duration * durationMultiplier) * Game1.realMilliSecondsPerGameMinute;
                        }

                        buffs.Add(new Buff((obj.IsDrink ? "drink" : "food") + "_" + bufftype, name, displayName, millisecondsDuration, texture, spriteIndex, effects: effects, item.IsDebuff));
                    }
                    if (buffs.Count > 0)
                    {
                        __result = buffs;
                    }
                }
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, PatcherName, "StardewValley.Object.TryCreateBuffsFromData", "postfixed", true);
            }
        }
    }
}
