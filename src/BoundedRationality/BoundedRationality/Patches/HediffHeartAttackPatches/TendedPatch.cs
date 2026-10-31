#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.BoundedRationality.Patches.HediffHeartAttackPatches {
	[HarmonyPatch(typeof(Hediff_HeartAttack), nameof(Hediff_HeartAttack.Tended))]
	public static class TendedPatch {
		private static readonly FieldInfo _pawnField = AccessTools.Field(typeof(Hediff), nameof(Hediff.pawn));

#if V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(float) });
#else
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), [typeof(Vector3), typeof(Map), typeof(string), typeof(float)]);
#endif

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
#if V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
#endif

#if V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, _pawnField) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0), new(OpCodes.Ldfld, _pawnField)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				// This text mote contains gamified information about the success chance of the treatment, so it is categorized as "meta" rather than "health."
				if (PatchUtility.Calls(instruction, _throwTextMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Meta, getPawnInstructions, generator, controlCategory: ControlCategory.TextMote)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}

				yield return instruction;
			}
		}
	}
}
#endif
