#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

namespace Lakuna.BoundedRationality.Patches.InspirationPatches {
	[HarmonyPatch(typeof(Inspiration), "SendBeginLetter")]
	public static class SendBeginLetterPatch {
		[HarmonyPrefix]
		private static bool Prefix(Inspiration __instance) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Needs, __instance.pawn, ControlCategory.Letter);
	}
}
