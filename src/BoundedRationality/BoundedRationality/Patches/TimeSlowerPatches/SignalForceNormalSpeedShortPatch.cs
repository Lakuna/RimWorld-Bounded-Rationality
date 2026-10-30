#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.TimeSlowerPatches {
	[HarmonyPatch(typeof(TimeSlower), nameof(TimeSlower.SignalForceNormalSpeedShort))]
	public static class SignalForceNormalSpeedShortPatch {
		[HarmonyPrefix]
		public static bool Prefix() =>
			!(BoundedRationalityMod.Settings?.PreventForcedSpeed ?? false)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, PawnType.Colonist);
	}
}
