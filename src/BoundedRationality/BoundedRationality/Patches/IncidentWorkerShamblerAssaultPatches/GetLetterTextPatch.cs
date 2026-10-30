#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.IncidentWorkerShamblerAssaultPatches {
	[HarmonyPatch(typeof(IncidentWorker_ShamblerAssault), "GetLetterText")]
	public static class GetLetterTextPatch {
		private static readonly FieldInfo FactionField = AccessTools.Field(typeof(IncidentParms), nameof(IncidentParms.faction));

		private static readonly FieldInfo ArrivalTextEnemyField = AccessTools.Field(typeof(RaidStrategyDef), nameof(RaidStrategyDef.arrivalTextEnemy));

		private static readonly MethodInfo CountMethod = SymbolExtensions.GetMethodInfo((List<Pawn> list) => list.Count((_) => true));

		private static readonly FieldInfo PawnGroupsField = AccessTools.Field(typeof(IncidentParms), nameof(IncidentParms.pawnGroups));

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

				// Used only to check the number of Gorehulks in the raid.
				if (PatchUtility.Calls(instruction, CountMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfFactionNotKnown(InformationCategory.Meta, getFactionInstructions, generator, 0)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, PawnGroupsField)) {
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
