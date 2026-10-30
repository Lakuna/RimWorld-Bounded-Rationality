#if !(V1_0 || V1_1 || V1_2 || V1_3)
using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.NeedKillThirstPatches {
	[HarmonyPatch(typeof(Need_KillThirst), nameof(Need_KillThirst.ShowOnNeedList), MethodType.Getter)]
	public static class ShowOnNeedListPatch {
		[HarmonyPostfix]
#pragma warning disable CA1707
		public static void Postfix(ref bool __result, Pawn ___pawn) =>
#pragma warning restore CA1707
			__result = __result
			&& KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, ___pawn);
	}
}
#endif
