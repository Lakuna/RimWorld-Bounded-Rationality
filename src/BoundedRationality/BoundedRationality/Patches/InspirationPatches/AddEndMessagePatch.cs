#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.InspirationPatches {
	[HarmonyPatch(typeof(Inspiration), "AddEndMessage")]
	public static class AddEndMessagePatch {
		[HarmonyPrefix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static bool Prefix(Inspiration __instance) =>
#pragma warning restore IDE1006
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, __instance?.pawn, ControlCategory.Message);
	}
}
