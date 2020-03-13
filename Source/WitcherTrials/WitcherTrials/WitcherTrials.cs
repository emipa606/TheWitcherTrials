using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using RimWorld;
using Verse;
using HarmonyLib;

namespace WitcherTrials
{
    public static class GlobalVars
    {
        public static bool firstRunCheck = true;
    }

    [DefOf]
    public static class HediffDefOf
    {
        public static HediffDef WitcherTrials_Hediff_Grasses;
        public static HediffDef WitcherTrials_Hediff_Dreams;
        public static HediffDef WitcherTrials_Hediff_Elders;
    }

    [DefOf]
    public static class RecipeDefOf
    {
        public static RecipeDef WitcherTrials_Recipe_Grasses;
        public static RecipeDef WitcherTrials_Recipe_Dreams;
        public static RecipeDef WitcherTrials_Recipe_Elders;
    }


    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        // erdelfs Transpiler <3 (for hiding operations)
        public static IEnumerable<CodeInstruction> Transpiler1(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo defInfo = AccessTools.Field(type: typeof(Thing), name: nameof(Thing.def));

            CodeInstruction[] codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
            for (int index = 0; index < codeInstructions.Length; index++)
            {
                CodeInstruction instruction = codeInstructions[index];
                if (instruction.opcode == OpCodes.Ldfld && defInfo == instruction.operand as FieldInfo)
                {
                    index += 1;
                    instruction = new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(type: typeof(WitcherTrials.HarmonyPatches), name: nameof(CreateRecipes)));
                }
                yield return instruction;
            }
        }

        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("Rimworld.imoja.witchertrials");
            var original = typeof(HealthCardUtility).GetNestedTypes(bindingAttr: AccessTools.all).First(predicate: t => t.GetMethods(bindingAttr: AccessTools.all).Any(predicate: mi => mi.ReturnType == typeof(List<FloatMenuOption>))).GetMethods(bindingAttr: AccessTools.all).First(predicate: mi => mi.ReturnType == typeof(List<FloatMenuOption>));
            var type = typeof(HarmonyPatches);
            var name = nameof(Transpiler1);
            var transpiler = new HarmonyMethod(type, name);

            // erdlefs harmony.Patch (for Transpiler) <3 (for hiding operations)
            harmony.Patch(original, null, null, transpiler);
        }

        // erdelf's CreateRecipes <3
        public static List<RecipeDef> CreateRecipes(Pawn pawn)
        {
            List<RecipeDef> recipes = pawn.def.AllRecipes;

            if (GlobalVars.firstRunCheck == true)
            {
                // Purge recipelist of WitcherTrials.Recipes
                GlobalVars.firstRunCheck = false;
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Grasses))
                {
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Grasses);
                }
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Dreams))
                {
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Dreams);
                }
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Elders))
                {
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Elders);
                }
            }

            // Add recipe.Grasses if pawn has no witcher hediffs
            if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Grasses) == null &&
                pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Dreams) == null &&
                pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Elders) == null)
            {
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Grasses) == false)
                    recipes.Add(RecipeDefOf.WitcherTrials_Recipe_Grasses); // add grasses
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Dreams))
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Dreams); // remove dreams
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Elders))
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Elders); // remove elders
            } // Add recipe.Dreams if pawn has witcher hediff Grasses
            if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Grasses) != null)
            {
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Grasses))
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Grasses); // remove grasses
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Dreams) == false)
                    recipes.Add(RecipeDefOf.WitcherTrials_Recipe_Dreams); // add dreams
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Elders))
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Elders); // remove elders
            } // Add recipe.Elders if pawn has witcher hediff dreams
            if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Dreams) != null)
            {
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Grasses))
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Grasses); // remove grasses
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Dreams))
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Dreams); // remove dreams
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Elders) == false)
                    recipes.Add(RecipeDefOf.WitcherTrials_Recipe_Elders); // add elders
            } // Remove all recipes if pawn has witcher hediff elders
            if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Elders) != null)
            {
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Grasses))
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Grasses); // remove grasses
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Dreams))
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Dreams); // remove dreams
                if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Elders))
                    recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Elders); // remove elders
            }
            return recipes;
        }
    }


    public class WitcherUtilities
    {
        public static void HediffCleanup(Pawn pawn)
        {
            //This gets the 'Hediff' of the 'HediffDef's listed.
            Hediff hediffGrasses = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Grasses);
            Hediff hediffDreams = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Dreams);
            Hediff hediffElders = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Elders);

            //If the pawn has the 'Hediff' Remove it.
            if (hediffGrasses != null)
                pawn.health.RemoveHediff(hediffGrasses);
            if (hediffDreams != null)
                pawn.health.RemoveHediff(hediffDreams);
            if (hediffElders != null)
                pawn.health.RemoveHediff(hediffElders);
        }
    }


    public class RecipeSurgery_Grasses : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
                if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
            //Cleanup any witcher 'Hediff's
            WitcherUtilities.HediffCleanup(pawn);
            //Apply required 'Hediff'
            pawn.health.AddHediff(HediffDefOf.WitcherTrials_Hediff_Grasses, part, null, null);
            //Adjust the visible surgery list (maintain the sequential nature)
            HarmonyPatches.CreateRecipes(pawn);
        }
    }

    public class RecipeSurgery_Dreams : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
                if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
            //Cleanup any witcher 'Hediff's
            WitcherUtilities.HediffCleanup(pawn);
            //Apply required 'Hediff'
            pawn.health.AddHediff(HediffDefOf.WitcherTrials_Hediff_Dreams, part, null, null);
            //Adjust the visible surgery list (maintain the sequential nature)
            HarmonyPatches.CreateRecipes(pawn);
        }
    }

    public class RecipeSurgery_Elders : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
                if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
            //Cleanup any witcher 'Hediff's
            WitcherUtilities.HediffCleanup(pawn);
            //Apply required 'Hediff'
            pawn.health.AddHediff(HediffDefOf.WitcherTrials_Hediff_Elders, part, null, null);
            //Adjust the visible surgery list (maintain the sequential nature)
            HarmonyPatches.CreateRecipes(pawn);
            //Change pawn haircolor to white
            pawn.story.hairColor = Color.white;
            //force redraw pawn hair
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
        }
    }
}