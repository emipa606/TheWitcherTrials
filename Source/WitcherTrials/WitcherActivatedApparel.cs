using RimWorld;
using Verse;

namespace WitcherTrials;

public class WitcherActivatedApparel : Apparel
{
    public override void Notify_Equipped(Pawn pawn)
    {
        base.Notify_Equipped(pawn);
        if (!pawn.health.hediffSet.hediffs.Any(hediff => hediff.def.defName.StartsWith("WitcherTrials_Hediff")))
        {
            if (def.defName.EndsWith("_Active"))
            {
                def = ThingDef.Named(def.defName.Replace("_Active", string.Empty));
            }

            return;
        }

        def = ThingDef.Named($"{def.defName}_Active");
    }

    public override void Notify_Unequipped(Pawn pawn)
    {
        base.Notify_Unequipped(pawn);
        if (!def.defName.EndsWith("_Active"))
        {
            return;
        }

        def = ThingDef.Named(def.defName.Replace("_Active", string.Empty));
    }
}