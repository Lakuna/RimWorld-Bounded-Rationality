#if !(V1_0 || V1_1 || V1_2 || V1_3)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffCompLetterOnDeathPatches {
	[HarmonyPatch(typeof(HediffComp_LetterOnDeath), "ShouldSendLetter", MethodType.Getter)]
	public static class ShouldSendLetterPatch {
		[HarmonyPostfix]
		private static void Postfix(HediffComp_LetterOnDeath __instance, ref bool __result) =>
			__result = __result && KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, __instance.parent.pawn, ControlCategory.Letter);
	}
}
#endif
