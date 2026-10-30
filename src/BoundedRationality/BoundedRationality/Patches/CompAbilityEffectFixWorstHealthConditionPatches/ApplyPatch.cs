#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.CompAbilityEffectFixWorstHealthConditionPatches {
	[HarmonyPatch(typeof(CompAbilityEffect_FixWorstHealthCondition), nameof(CompAbilityEffect_FixWorstHealthCondition.Apply))]
	public static class ApplyPatch {
		private static readonly MethodInfo PawnMethod = PatchUtility.PropertyGetter(typeof(LocalTargetInfo), nameof(LocalTargetInfo.Pawn));

		private static readonly MethodInfo SendLetterMethod = PatchUtility.PropertyGetter(typeof(CompAbilityEffect), "SendLetter");

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarga_S, 1), new CodeInstruction(OpCodes.Call, PawnMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, SendLetterMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions, ControlCategory.Letter)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
