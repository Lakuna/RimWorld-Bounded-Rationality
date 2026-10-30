#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.IncidentWorkerPsychicRitualSiegePatches {
	[HarmonyPatch(typeof(IncidentWorker_PsychicRitualSiege), "GetLetterText")]
	public static class GetLetterTextPatch {
		private static readonly FieldInfo FactionField = AccessTools.Field(typeof(IncidentParms), nameof(IncidentParms.faction));

		private static readonly FieldInfo ArrivalTextEnemyField = AccessTools.Field(typeof(RaidStrategyDef), nameof(RaidStrategyDef.arrivalTextEnemy));

		private static readonly MethodInfo FindMethod = AccessTools.Method(typeof(List<Pawn>), nameof(List<Pawn>.Find));

		private static readonly FieldInfo RaidAgeRestrictionField = AccessTools.Field(typeof(IncidentParms), nameof(IncidentParms.raidAgeRestriction));

		private static readonly FieldInfo LetterAiArrivedTextField = AccessTools.Field(typeof(PsychicRitualDef), nameof(PsychicRitualDef.letterAIArrivedText));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getFactionInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1), new CodeInstruction(OpCodes.Ldfld, FactionField) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Hide information about the enemy strategy.
				if (PatchUtility.LoadsField(instruction, ArrivalTextEnemyField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfFactionNotKnown(InformationCategory.Meta, getFactionInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}

				// Used only to check if the enemy leader is part of the raid.
				if (PatchUtility.Calls(instruction, FindMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfFactionNotKnown(InformationCategory.Basic, getFactionInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, RaidAgeRestrictionField) || PatchUtility.LoadsField(instruction, LetterAiArrivedTextField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfFactionNotKnown(InformationCategory.Meta, getFactionInstructions, generator)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
