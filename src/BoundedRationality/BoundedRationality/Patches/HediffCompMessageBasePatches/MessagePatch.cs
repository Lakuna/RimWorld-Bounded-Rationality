#if !(V1_0 || V1_1 || V1_2 || V1_3)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffCompMessageBasePatches {
	[HarmonyPatch(typeof(HediffComp_MessageBase), "Message")]
	public static class MessagePatch {
		[HarmonyPrefix]
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static bool Prefix(HediffComp_MessageBase __instance) =>
#pragma warning restore IDE1006
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance?.Pawn, ControlCategory.Message);
	}
}
#endif
