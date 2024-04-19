using HarmonyLib;
using Verse;

namespace WitcherTrials;

[HarmonyPatch(typeof(DamageDef), nameof(DamageDef.ExternalViolenceFor), typeof(Thing))]
public class DamageDef_ExternalViolenceFor
{
    public static void Postfix(Thing thing, DamageDef __instance, ref bool __result)
    {
        var damageExtension = __instance.GetModExtension<DamageModExtension>();
        if (damageExtension is not { isMonsterDamage: true })
        {
            return;
        }

        if (thing is not Pawn pawn)
        {
            __result = false;
            return;
        }

        __result = pawn.def.GetModExtension<PawnModExtension>()?.isMonster == true;
    }
}