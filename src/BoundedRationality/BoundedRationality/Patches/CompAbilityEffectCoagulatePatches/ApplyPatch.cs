#if !(V1_0 || V1_1 || V1_2 || V1_3)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using UnityEngine;

using Verse;

namespace Lakuna.BoundedRationality.Patches.CompAbilityEffectCoagulatePatches {
	[HarmonyPatch(typeof(CompAbilityEffect_Coagulate), nameof(CompAbilityEffect_Coagulate.Apply))]
	public static class ApplyPatch {
		private static readonly MethodInfo _pawnMethod = PatchUtility.PropertyGetter(typeof(LocalTargetInfo), nameof(LocalTargetInfo.Pawn));

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(float) });
#else
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), [typeof(Vector3), typeof(Map), typeof(string), typeof(float)]);
#endif

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarga_S, 1), new CodeInstruction(OpCodes.Call, _pawnMethod) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarga_S, 1), new(OpCodes.Call, _pawnMethod)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				if (PatchUtility.Calls(instruction, _throwTextMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Health, getPawnInstructions, generator, controlCategory: ControlCategory.TextMote)) {
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
