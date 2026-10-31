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
		private static readonly FieldInfo _arrivalTextEnemyField = AccessTools.Field(typeof(RaidStrategyDef), nameof(RaidStrategyDef.arrivalTextEnemy));

		private static readonly MethodInfo _countMethod = SymbolExtensions.GetMethodInfo((List<Pawn> list) => list.Count((_) => true));

		private static readonly FieldInfo _factionField = AccessTools.Field(typeof(IncidentParms), nameof(IncidentParms.faction));

		private static readonly FieldInfo _pawnGroupsField = AccessTools.Field(typeof(IncidentParms), nameof(IncidentParms.pawnGroups));

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction[] getFactionInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1), new CodeInstruction(OpCodes.Ldfld, _factionField) };
#else
			CodeInstruction[] getFactionInstructions = [new(OpCodes.Ldarg_1), new(OpCodes.Ldfld, _factionField)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Hide information about the enemy strategy.
				if (PatchUtility.LoadsField(instruction, _arrivalTextEnemyField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfFactionNotKnown(InformationCategory.Meta, getFactionInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}

				// Used only to check the number of Gorehulks in the raid.
				if (PatchUtility.Calls(instruction, _countMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfFactionNotKnown(InformationCategory.Meta, getFactionInstructions, generator, 0)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _pawnGroupsField)) {
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
