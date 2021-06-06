using System.Collections.Generic;
using RimWorld;
using Verse;

namespace WitcherTrials
{
    public class RecipeSurgery_Grasses : Recipe_Surgery
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
            pawn.health.AddHediff(HediffDefOf.WitcherTrials_Hediff_Grasses, part);
            //Adjust the visible surgery list (maintain the sequential nature)
            HarmonyPatches.CreateRecipes(pawn);
        }
    }
}