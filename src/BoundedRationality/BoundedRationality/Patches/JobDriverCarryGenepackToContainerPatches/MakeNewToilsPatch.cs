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
using Verse.AI;

namespace Lakuna.BoundedRationality.Patches.JobDriverCarryGenepackToContainerPatches {
	[HarmonyPatch(typeof(JobDriver_CarryGenepackToContainer), "MakeNewToils")]
	public static class MakeNewToilsPatch {
		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(JobDriver), nameof(JobDriver.pawn));

		private static readonly MethodInfo ThrowTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(float) });

#if V1_4
		private static readonly HarmonyMethod InnerActionDelegateTranspilerMethod = new HarmonyMethod(AccessTools.Method(typeof(MakeNewToilsPatch), nameof(InnerActionDelegateTranspiler)));
#else
		private static readonly MethodInfo InnerActionDelegateTranspilerMethod = AccessTools.Method(typeof(MakeNewToilsPatch), nameof(InnerActionDelegateTranspiler));
#endif

		private static IEnumerable<CodeInstruction> InnerActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, PawnField) };

			foreach (CodeInstruction instruction in instructions) {
				if (PatchUtility.Calls(instruction, ThrowTextMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Basic, getPawnInstructions, generator, controlCategory: ControlCategory.TextMote)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}

				yield return instruction;
			}
		}

#if V1_4
		private static readonly HarmonyMethod ActionDelegateTranspilerMethod = new HarmonyMethod(AccessTools.Method(typeof(MakeNewToilsPatch), nameof(ActionDelegateTranspiler)));
#else
		private static readonly MethodInfo ActionDelegateTranspilerMethod = AccessTools.Method(typeof(MakeNewToilsPatch), nameof(ActionDelegateTranspiler));
#endif

		private static IEnumerable<CodeInstruction> ActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Apply a transpiler to functions referenced via pointer.
				if (instruction.opcode == OpCodes.Ldftn && instruction.operand is MethodInfo methodInfo && methodInfo.DeclaringType == typeof(JobDriver_CarryGenepackToContainer)) {
					_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: InnerActionDelegateTranspilerMethod);
					continue;
				}
			}
		}

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Apply a transpiler to action delegates.
				if (instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructorInfo && constructorInfo.DeclaringType.DeclaringType == typeof(JobDriver_CarryGenepackToContainer)) {
					foreach (MethodInfo methodInfo in AccessTools.GetDeclaredMethods(constructorInfo.DeclaringType)) {
						_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: ActionDelegateTranspilerMethod);
					}

					continue;
				}
			}
		}
	}
}
#endif
