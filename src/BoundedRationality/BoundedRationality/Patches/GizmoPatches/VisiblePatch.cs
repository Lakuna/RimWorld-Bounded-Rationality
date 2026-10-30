using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.GizmoPatches {
	[HarmonyPatch(typeof(Gizmo), nameof(Gizmo.Visible), MethodType.Getter)]
	public static class VisiblePatch {
#if !V1_0
		private static readonly FieldInfo TrackerField = AccessTools.Field(typeof(PsychicEntropyGizmo), "tracker");
#endif

#if V1_0 || V1_1 || V1_2 || V1_3
		private static readonly MethodInfo PawnOwnerMethod = PatchUtility.PropertyGetter(typeof(Apparel), nameof(Apparel.Wearer));
#else
		private static readonly FieldInfo ResourceGeneField = AccessTools.Field(typeof(GeneGizmo_Resource), "gene");

		private static readonly MethodInfo PawnOwnerMethod = PatchUtility.PropertyGetter(typeof(CompShield), "PawnOwner");

		private static readonly FieldInfo DeathrestGeneField = AccessTools.Field(typeof(GeneGizmo_DeathrestCapacity), "gene");
#endif

		[HarmonyPostfix]
#pragma warning disable CA1707
		private static void Postfix(Gizmo __instance, ref bool __result) {
#pragma warning restore CA1707
			if (__instance is Command_VerbTarget commandVerbTarget) {
				__result = __result
					&& (!commandVerbTarget.verb.CasterIsPawn
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, commandVerbTarget.verb.CasterPawn, ControlCategory.Control));
				return;
			}

			if (__instance is Gizmo_EnergyShieldStatus gizmoEnergyShieldStatus) {
				__result = __result
					&& (!(PawnOwnerMethod.Invoke(gizmoEnergyShieldStatus.shield, MiscellaneousUtility.EmptyArray()) is Pawn pawn)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Gear, pawn, ControlCategory.Control));
				return;
			}

#if !V1_0
			if (__instance is PsychicEntropyGizmo psychicEntropyGizmo) {
				__result = __result
					&& (!(TrackerField.GetValue(psychicEntropyGizmo) is Pawn_PsychicEntropyTracker pawnPsychicEntropyTracker)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawnPsychicEntropyTracker.Pawn, ControlCategory.Control));
				return;
			}
#endif

#if V1_0 || V1_1
#elif V1_2 || V1_3 || V1_4
			if (__instance is Command_Psycast commandPsycast) {
				__result = __result
					&& (commandPsycast.Ability.pawn is null
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, commandPsycast.Ability.pawn, ControlCategory.Control));
				return;
			}
#else
			if (__instance is Command_Psycast commandPsycast) {
				__result = __result
					&& (commandPsycast.Pawn is null
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, commandPsycast.Pawn, ControlCategory.Control));
				return;
			}
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
			if (__instance is GeneGizmo_Resource geneGizmoResource) {
				__result = __result
					&& (!(ResourceGeneField.GetValue(geneGizmoResource) is Gene_Resource geneResource)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, geneResource.pawn, ControlCategory.Control));
				return;
			}

			if (__instance is GeneGizmo_DeathrestCapacity geneGizmoDeathrestCapacity) {
				__result = __result
					&& (!(DeathrestGeneField.GetValue(geneGizmoDeathrestCapacity) is Gene_Deathrest geneDeathrest)
					|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, geneDeathrest.pawn, ControlCategory.Control));
			}
#endif
		}
	}
}
