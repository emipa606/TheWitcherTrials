using HarmonyLib;
using Verse;

namespace WitcherTrials;

[HarmonyPatch(typeof(DamageDef), "ExternalViolenceFor", typeof(Thing))]
public class DamageDef_ExternalViolenceFor
{
    public static void Postfix(Thing thing, DamageDef __instance, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        if (__instance.GetModExtension<DamageModExtension>()?.isMonsterDamage == false)
        {
            return;
        }

        if (thing is not Pawn pawn)
        {
            return;
        }

        if (pawn.def.GetModExtension<PawnModExtension>()?.isMonster == false)
        {
            return;
        }

        __result = true;
    }
}