using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace WitcherTrials;

public class RecipeSurgery_Elders : Recipe_Surgery
{
    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients,
        Bill bill)
    {
        if (billDoer != null)
        {
            if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
            {
                return;
            }
        }

        //Cleanup any witcher 'Hediff's
        WitcherUtilities.HediffCleanup(pawn);
        //Apply required 'Hediff'
        pawn.health.AddHediff(HediffDefOf.WitcherTrials_Hediff_Elders, part);
        //Adjust the visible surgery list (maintain the sequential nature)
        HarmonyPatches.CreateRecipes(pawn);
        //Change pawn haircolor to white
        pawn.story.HairColor = Color.white;
        //force redraw pawn hair
        pawn.Drawer.renderer.graphics.ResolveAllGraphics();
    }
}