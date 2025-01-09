﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using VanillaPlusProfessions.Utilities;

namespace VanillaPlusProfessions
{
    public static class CorePatcher
    {
        internal static void ApplyPatches()
        {
            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getProfessionForSkill)),
                    prefix: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(getProfessionForSkill_Prefix)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "Farmer.getProfessionForSkill", "postfixing");
            }

            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.checkForLevelGain)),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(checkForLevelGain_Postfix)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "Farmer.checkForLevelGain", "postfixing");
            }

            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(Stats), nameof(Stats.checkForSkillAchievements)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(Transpiler)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "LevelUpMenu constructor", "transpiling");
            }

            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), new Type[] { typeof(string[]), typeof(Farmer), typeof(xTile.Dimensions.Location) }),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(Transpiler)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "GameLocation.performAction", "transpiling");
            }

            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(LevelUpMenu), "getProfessionName"),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(getProfessionName_Postfix)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "LevelUpMenu.getProfessionName", "postfixing");
            }

            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(answerDialogueAction_Postfix)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "GameLocation.answerDialogueAction", "postfixing");
            }
            
            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.canRespec)),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(canRespec_Postfix)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "GameLocation.canRespec", "postfixing");
            }

            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(gainExperience_Transpiler)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "Farmer.gainExperience", "transpiling");
            }
            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(draw_LevelUpMenu_Transpiler)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "LevelUpMenu.draw", "transpiling");
            }

            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(draw_LevelUpMenu_Transpiler)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "LevelUpMenu.draw", "transpiling");
            }

            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(Axe), nameof(Axe.DoFunction)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(DoFunction_Transpiler)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "Axe.DoFunction", "transpiling");
            }

            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(Pickaxe), nameof(Pickaxe.DoFunction)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(DoFunction_Transpiler)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "Pickaxe.DoFunction", "transpiling");
            }

            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(WateringCan), nameof(WateringCan.DoFunction)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(DoFunction_Transpiler)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "WateringCan.DoFunction", "transpiling");
            }

            try
            {
                ModEntry.Harmony.Patch(
                    original: AccessTools.Method(typeof(Hoe), nameof(Hoe.DoFunction)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(CorePatcher), nameof(DoFunction_Transpiler)))
                );
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "Hoe.DoFunction", "transpiling");
            }
        }
        public static IEnumerable<CodeInstruction> DoFunction_Transpiler(IEnumerable<CodeInstruction> insns)
        {
            try
            {
                foreach (var ins in insns)
                {
                    if (ins.opcode == OpCodes.Ldc_R4 && (float)ins.operand == 0.1f)
                    {
                        ins.opcode = OpCodes.Call;
                        ins.operand = AccessTools.Method(typeof(CorePatcher), nameof(GetEnergyCostRate));
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                CoreUtility.PrintError(e, nameof(CorePatcher), "<VanillaTool>.DoFunction", "transpiled", true);
            }
            return insns;
        }
        public static float GetEnergyCostRate()
        {
            return ModEntry.ModConfig.Value.StaminaCostAdjustments ? 0.08f : 0.1f;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int number = 6;
            MethodInfo replacement = AccessTools.Method(typeof(CoreUtility), nameof(CoreUtility.GetMaxLevel));

            foreach (var item in instructions)
            {
                if (item.opcode.Equals(OpCodes.Ldc_I4_S) && item.OperandIs(10))
                {
                    if (number > 0)
                    {
                        item.opcode = OpCodes.Call;
                        item.operand = replacement;
                    }
                    number--;
                }
                yield return item;
            }
        }
        public static IEnumerable<CodeInstruction> draw_LevelUpMenu_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int index = 0;
            int expected_cursors = 0;
            FieldInfo mouseCursors = AccessTools.Field(typeof(Game1), nameof(Game1.mouseCursors));
            ConstructorInfo rectangleConstructor = AccessTools.Constructor(typeof(Rectangle?), parameters: new Type[] { typeof(Rectangle) });
            List<CodeInstruction> toReturn = instructions.ToList();
            object obj = null;
            object obj2 = null;

            var list = AccessTools.GetDeclaredMethods(typeof(List<int>));

            for (int i = 0; i < 2; i++)
            {
                index = 0;
                foreach (var instruct in toReturn)
                {
                    index++;
                    if (instruct.opcode.Equals(OpCodes.Ldsfld) && instruct.operand.Equals(mouseCursors))
                    {
                        if (expected_cursors is not 0)
                        {
                            toReturn.Insert(index - 1, new(OpCodes.Ldarg_0));
                            instruct.operand = AccessTools.Method(typeof(CoreUtility), nameof(CoreUtility.GetProfessionIconImage));
                            instruct.opcode = OpCodes.Call;
                            break;
                        }
                        else
                        {
                            expected_cursors++;
                        }
                    }
                }
                expected_cursors = 0;
            }
            index = 0;
            int whichCallvirt = 0;
            int whichCtor = 0;
            foreach (var instruct in toReturn)
            {
                if (instruct.opcode == OpCodes.Callvirt)
                {
                    if (whichCallvirt < 16)
                    {
                        whichCallvirt++;
                    }
                    else
                    {
                        obj = instruct.operand;
                        obj2 = instruct.operand;
                        break;
                    }
                }
            }
            for (int i = 0; i < 2; i++)
            {
                foreach (var instruct2 in toReturn)
                {
                    index++;
                    if (instruct2.opcode == OpCodes.Newobj && (ConstructorInfo)instruct2.operand == rectangleConstructor)
                    {
                        if (whichCtor == 0)
                        {
                            whichCtor++;
                            continue;
                        }
                        else
                        {
                            toReturn[index - 2].opcode = OpCodes.Callvirt;
                            toReturn[index - 2].operand = obj ?? obj2;

                            toReturn[index - 1].opcode = OpCodes.Call;
                            toReturn[index - 1].operand = AccessTools.Method(typeof(CoreUtility), nameof(CoreUtility.GetProfessionSourceRect));
                           
                            toReturn.Insert(index - 2, new(OpCodes.Ldarg_0));
                            toReturn.Insert(index - 1, new(OpCodes.Ldfld, AccessTools.Field(typeof(LevelUpMenu), "professionsToChoose")));
                            toReturn.Insert(index, new(i == 0 ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1));


                            break;
                        }
                    }
                    //GetProfessionSourceRect
                }
                index = 0;
                whichCtor = 0;
            }
            return toReturn;
        }

        //Patched
        public static void answerDialogueAction_Postfix(string questionAndAnswer)
        {
            string[] answers = new string[] { "professionForget_farming", "professionForget_fishing", "professionForget_foraging", "professionForget_mining", "professionForget_combat" };
            int whichSkill = -1;
            for (int i = 0; i < answers.Length; i++)
                if (questionAndAnswer.Contains(answers[i]))
                    whichSkill = i;
            if (whichSkill > -1)
            {
                foreach (var item in ModEntry.Professions.Values)
                    if (item.Skill == whichSkill)
                        Game1.player.professions.Remove(item.ID);

                int level = Farmer.checkForLevelGain(0, Game1.player.experiencePoints[whichSkill]);
                if (level >= 15)
                {
                    Game1.player.newLevels.Add(new Point(whichSkill, 15));
                    if (level >= 20)
                        Game1.player.newLevels.Add(new Point(whichSkill, 20));
                }
            }
        }
        //Patched
        public static void canRespec_Postfix(int skill_index, ref bool __result)
        {
            if (Game1.player.newLevels.Contains(new Point(skill_index, 15)) || Game1.player.newLevels.Contains(new Point(skill_index, 20)))
            {
                __result = false;
            }
        }
        //Patched
        public static void getProfessionName_Postfix(int whichProfession, ref string __result)
        {
            foreach (var item in ModEntry.Professions)
            {
                if (item.Value.ID == whichProfession)
                {
                    __result = item.Key;
                }
            }
        }
        //Patched
        public static void getProfessionForSkill_Prefix(Farmer __instance, int skillType, int skillLevel, ref int __result)
        {
            int result = skillLevel / 5;
            if (result == 3)
            {
                foreach (var item in ModEntry.Managers[skillType].RelatedProfessions)
                    if (__instance.professions.Contains(item.Value.Requires))
                        __result = item.Value.ID;
            }
            else if (result == 4)
                foreach (var item in ModEntry.Managers[6].RelatedProfessions)
                    if (__instance.professions.Contains(item.Value.ID))
                        __result = item.Value.ID;
        }
        //Patched
        public static void checkForLevelGain_Postfix(ref int __result, int oldXP, int newXP)
        {
            for (int level = 1; level <= 10; level++)
            {
                if (oldXP < ModEntry.levelExperiences[level - 1] && newXP >= ModEntry.levelExperiences[level - 1])
                {
                    //but I cant leave it like this, otherwise players will keep getting level up messages

                    __result = level + 10;
                }
            }
        }

        public static IEnumerable<CodeInstruction> gainExperience_Transpiler(IEnumerable<CodeInstruction> insns)
        {
            var list = insns.ToList();
            MethodInfo methodInfo = AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.Level));
            MethodInfo replacement = AccessTools.Method(typeof(CorePatcher), nameof(TryOverrideVanillaLevel));
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].opcode == OpCodes.Call && (MethodInfo)list[i].operand == methodInfo)
                {
                    list[i].operand = replacement;
                    break;
                }
            }
            return list;
        }
        
        public static int TryOverrideVanillaLevel(Farmer who)
        {
            int result = 0;
            for (int i = 0; i < 5; i++)
            {
                if (who.GetUnmodifiedSkillLevel(i) >= CoreUtility.GetMaxLevel())
                {
                    result += 5;
                }
            }
            return result;
        }

        //100 = 1Lv - 100     | 21000 = 11Lv - 6000
        //380 = 2Lv - 280     | 28000 = 12Lv - 7000
        //770 = 3Lv - 390     | 36500 = 13Lv - 8500
        //1300 = 4Lv - 530    | 50000 = 14v - 9500
        //2150 = 5Lv - 850    | 60000 = 15Lv - 10000
        //3300 = 6Lv - 1150   | 72000 = 16Lv - 12000
        //4800 = 7Lv - 1500   | 85500 = 17Lv - 13500
        //6900 = 8Lv - 2100   | 101000 = 18Lv - 15500
        //10000 = 9Lv - 3100  | 118000 = 19Lv - 17000
        //15000 = 10Lv - 5000 | 138000 = 20Lv - 20000
    }
}
