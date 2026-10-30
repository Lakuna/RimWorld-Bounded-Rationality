#if !(V1_0 || V1_1 || V1_2 || V1_3)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffCompMessageBasePatches {
	[HarmonyPatch(typeof(HediffComp_MessageBase), "Message")]
	public static class MessagePatch {
		[HarmonyPrefix]
		public static bool Prefix(HediffComp_MessageBase __instance) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.Pawn, ControlCategory.Message);
	}
}
#endif
