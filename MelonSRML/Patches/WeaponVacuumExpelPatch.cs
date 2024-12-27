using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;

namespace MelonSRML.Patches;

[HarmonyPatch(typeof(VacuumItem), nameof(VacuumItem.Expel), typeof(GameObject), typeof(bool), typeof(float), typeof(SlimeAppearance.AppearanceSaveSet))]
internal class WeaponVacuumExpelPatch
{
    public static void Prefix(VacuumItem __instance, ref bool ignoreEmotions)
    {
        if (true) // Porting to 6.0: Reference Unity.Mathematics, it appears Ammo.GetSelectedEmotions() returns a 'float4' now.
                  // It seems it cannot be null so please have somebody look into this.
        {
            ignoreEmotions = true;
        }
    }
}