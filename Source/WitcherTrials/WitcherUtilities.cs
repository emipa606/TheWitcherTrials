using Verse;

namespace WitcherTrials
{
    public class WitcherUtilities
    {
        public static void HediffCleanup(Pawn pawn)
        {
            //This gets the 'Hediff' of the 'HediffDef's listed.
            var hediffGrasses = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Grasses);
            var hediffDreams = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Dreams);
            var hediffElders = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.WitcherTrials_Hediff_Elders);

            //If the pawn has the 'Hediff' Remove it.
            if (hediffGrasses != null)
            {
                pawn.health.RemoveHediff(hediffGrasses);
            }

            if (hediffDreams != null)
            {
                pawn.health.RemoveHediff(hediffDreams);
            }

            if (hediffElders != null)
            {
                pawn.health.RemoveHediff(hediffElders);
            }
        }
    }
}