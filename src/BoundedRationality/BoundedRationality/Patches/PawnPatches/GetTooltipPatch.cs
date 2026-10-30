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
		private static readonly FieldInfo _equipmentField = AccessTools.Field(typeof(Pawn), nameof(Pawn.equipment));

		private static readonly FieldInfo _genderField = AccessTools.Field(typeof(Pawn), nameof(Pawn.gender));

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.LoadsField(instruction, _genderField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, Gender.None)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _equipmentField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Gear, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
