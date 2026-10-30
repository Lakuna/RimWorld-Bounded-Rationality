using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetTooltip))]
	public static class GetTooltipPatch {
		private static readonly FieldInfo GenderField = AccessTools.Field(typeof(Pawn), nameof(Pawn.gender));

		private static readonly FieldInfo EquipmentField = AccessTools.Field(typeof(Pawn), nameof(Pawn.equipment));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.LoadsField(instruction, GenderField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, Gender.None)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, EquipmentField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Gear, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
