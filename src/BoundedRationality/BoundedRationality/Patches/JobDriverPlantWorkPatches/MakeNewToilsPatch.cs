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

using RimWorld;

using UnityEngine;

using Verse;
using Verse.AI;

namespace Lakuna.BoundedRationality.Patches.JobDriverPlantWorkPatches {
	[HarmonyPatch(typeof(JobDriver_PlantWork), "MakeNewToils")]
	public static class MakeNewToilsPatch {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly HarmonyMethod _actionDelegateTranspilerMethod = new HarmonyMethod(AccessTools.Method(typeof(MakeNewToilsPatch), nameof(ActionDelegateTranspiler)));
#else
		private static readonly MethodInfo _actionDelegateTranspilerMethod = AccessTools.Method(typeof(MakeNewToilsPatch), nameof(ActionDelegateTranspiler));
#endif

		private static readonly FieldInfo _actorField = AccessTools.Field(typeof(Toil), nameof(Toil.actor));

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly HarmonyMethod _innerActionDelegateTranspilerMethod = new HarmonyMethod(AccessTools.Method(typeof(MakeNewToilsPatch), nameof(InnerActionDelegateTranspiler)));
#else
		private static readonly MethodInfo _innerActionDelegateTranspilerMethod = AccessTools.Method(typeof(MakeNewToilsPatch), nameof(InnerActionDelegateTranspiler));
#endif


#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(float) });
#else
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), [typeof(Vector3), typeof(Map), typeof(string), typeof(float)]);
#endif

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
				if (instruction.opcode == OpCodes.Ldftn && instruction.operand is MethodInfo methodInfo && methodInfo.DeclaringType?.DeclaringType == typeof(JobDriver_PlantWork)) {
					_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: _innerActionDelegateTranspilerMethod);
					continue;
				}
			}
		}

		private static IEnumerable<CodeInstruction> InnerActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
#endif

			// If the cut field isn't present, return unmodified instructions.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			FieldInfo cutField = original?.DeclaringType?.GetField("cut");
#else
			FieldInfo? cutField = original?.DeclaringType?.GetField("cut");
#endif
			if (cutField is null) {
				foreach (CodeInstruction instruction in instructions) {
					yield return instruction;
				}

				yield break;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, cutField), new CodeInstruction(OpCodes.Ldfld, _actorField) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0), new(OpCodes.Ldfld, cutField), new(OpCodes.Ldfld, _actorField)];
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
				if (instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructorInfo && constructorInfo.DeclaringType?.DeclaringType == typeof(JobDriver_PlantWork)) {
					foreach (MethodInfo methodInfo in AccessTools.GetDeclaredMethods(constructorInfo.DeclaringType)) {
						_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: _actionDelegateTranspilerMethod);
					}

					continue;
				}
			}
		}
	}
}
