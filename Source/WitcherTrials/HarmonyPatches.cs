using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WitcherTrials;

[StaticConstructorOnStartup]
public static class HarmonyPatches
{
    static HarmonyPatches()
    {
        var harmony = new Harmony("Rimworld.imoja.witchertrials");
        var original = typeof(HealthCardUtility).GetNestedTypes(AccessTools.all)
            .First(t => t.GetMethods(AccessTools.all).Any(mi => mi.ReturnType == typeof(List<FloatMenuOption>)))
            .GetMethods(AccessTools.all).First(mi => mi.ReturnType == typeof(List<FloatMenuOption>));
        var type = typeof(HarmonyPatches);
        const string name = nameof(Transpiler1);
        var transpiler = new HarmonyMethod(type, name);

        // erdlefs harmony.Patch (for Transpiler) <3 (for hiding operations)
        harmony.Patch(original, null, null, transpiler);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    // erdelfs Transpiler <3 (for hiding operations)
    public static IEnumerable<CodeInstruction> Transpiler1(IEnumerable<CodeInstruction> instructions)
    {
        var defInfo = AccessTools.Field(typeof(Thing), nameof(Thing.def));

        var codeInstructions = instructions as CodeInstruction[] ?? instructions.ToArray();
        for (var index = 0; index < codeInstructions.Length; index++)
        {
            var instruction = codeInstructions[index];
            if (instruction.opcode == OpCodes.Ldfld && defInfo == instruction.operand as FieldInfo)
            {
                index += 1;
                instruction = new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(HarmonyPatches), nameof(CreateRecipes)));
            }

            yield return instruction;
        }
    }

    // erdelf's CreateRecipes <3
    public static List<RecipeDef> CreateRecipes(Pawn pawn)
    {
        var recipes = pawn.def.AllRecipes;

        if (GlobalVars.FirstRunCheck)
        {
            // Purge recipelist of WitcherTrials.Recipes
            GlobalVars.FirstRunCheck = false;
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
            if (!recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Grasses))
            {
                recipes.Add(RecipeDefOf.WitcherTrials_Recipe_Grasses); // add grasses
            }

            if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Dreams))
            {
                recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Dreams); // remove dreams
            }

            if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Elders))
            {
                recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Elders); // remove elders
            }
        } // Add recipe.Dreams if pawn has witcher hediff Grasses

        if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Grasses) != null)
        {
            if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Grasses))
            {
                recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Grasses); // remove grasses
            }

            if (!recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Dreams))
            {
                recipes.Add(RecipeDefOf.WitcherTrials_Recipe_Dreams); // add dreams
            }

            if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Elders))
            {
                recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Elders); // remove elders
            }
        } // Add recipe.Elders if pawn has witcher hediff dreams

        if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Dreams) != null)
        {
            if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Grasses))
            {
                recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Grasses); // remove grasses
            }

            if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Dreams))
            {
                recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Dreams); // remove dreams
            }

            if (!recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Elders))
            {
                recipes.Add(RecipeDefOf.WitcherTrials_Recipe_Elders); // add elders
            }
        } // Remove all recipes if pawn has witcher hediff elders

        if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Elders) == null)
        {
            return recipes;
        }

        if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Grasses))
        {
            recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Grasses); // remove grasses
        }

        if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Dreams))
        {
            recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Dreams); // remove dreams
        }

        if (recipes.Contains(RecipeDefOf.WitcherTrials_Recipe_Elders))
        {
            recipes.Remove(RecipeDefOf.WitcherTrials_Recipe_Elders); // remove elders
        }

        return recipes;
    }
}