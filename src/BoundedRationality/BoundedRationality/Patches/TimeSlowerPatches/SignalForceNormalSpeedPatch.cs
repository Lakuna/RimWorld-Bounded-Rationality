#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.TimeSlowerPatches {
	[HarmonyPatch(typeof(TimeSlower), nameof(TimeSlower.SignalForceNormalSpeed))]
	public static class SignalForceNormalSpeedPatch {
		[HarmonyPrefix]
		private static bool Prefix() => !BoundedRationalityMod.Settings.PreventForcedSpeed || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, PawnType.Colonist);
	}
}
