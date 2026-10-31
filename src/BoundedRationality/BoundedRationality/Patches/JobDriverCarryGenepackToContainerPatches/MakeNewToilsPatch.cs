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
#if V1_4
		private static readonly HarmonyMethod _actionDelegateTranspilerMethod = new HarmonyMethod(AccessTools.Method(typeof(MakeNewToilsPatch), nameof(ActionDelegateTranspiler)));
#else
		private static readonly MethodInfo _actionDelegateTranspilerMethod = AccessTools.Method(typeof(MakeNewToilsPatch), nameof(ActionDelegateTranspiler));
#endif

#if V1_4
		private static readonly HarmonyMethod _innerActionDelegateTranspilerMethod = new HarmonyMethod(AccessTools.Method(typeof(MakeNewToilsPatch), nameof(InnerActionDelegateTranspiler)));
#else
		private static readonly MethodInfo _innerActionDelegateTranspilerMethod = AccessTools.Method(typeof(MakeNewToilsPatch), nameof(InnerActionDelegateTranspiler));
#endif

		private static readonly FieldInfo _pawnField = AccessTools.Field(typeof(JobDriver), nameof(JobDriver.pawn));

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
#else
#endif
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), [typeof(Vector3), typeof(Map), typeof(string), typeof(float)]);

		private static IEnumerable<CodeInstruction> ActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Apply a transpiler to functions referenced via pointer.
				if (instruction.opcode == OpCodes.Ldftn && instruction.operand is MethodInfo methodInfo && methodInfo.DeclaringType == typeof(JobDriver_CarryGenepackToContainer)) {
					_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: _innerActionDelegateTranspilerMethod);
					continue;
				}
			}
		}

		private static IEnumerable<CodeInstruction> InnerActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, _pawnField) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0), new(OpCodes.Ldfld, _pawnField)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				if (PatchUtility.Calls(instruction, _throwTextMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Basic, getPawnInstructions, generator, controlCategory: ControlCategory.TextMote)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}

				yield return instruction;
			}
		}

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				// Apply a transpiler to action delegates.
				if (instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructorInfo && constructorInfo.DeclaringType?.DeclaringType == typeof(JobDriver_CarryGenepackToContainer)) {
					foreach (MethodInfo methodInfo in AccessTools.GetDeclaredMethods(constructorInfo.DeclaringType)) {
						_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: _actionDelegateTranspilerMethod);
					}

					continue;
				}
			}
		}
	}
}
#endif
