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

namespace Lakuna.BoundedRationality.Patches.ToilsInterpersonalPatches {
	[HarmonyPatch(typeof(Toils_Interpersonal), nameof(Toils_Interpersonal.TryTrain))]
	public static class TryTrainPatch {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly HarmonyMethod _actionDelegateTranspilerMethod = new HarmonyMethod(AccessTools.Method(typeof(TryTrainPatch), nameof(ActionDelegateTranspiler)));
#else
		private static readonly MethodInfo _actionDelegateTranspilerMethod = AccessTools.Method(typeof(TryTrainPatch), nameof(ActionDelegateTranspiler));
#endif

		private static readonly FieldInfo _actorField = AccessTools.Field(typeof(Toil), nameof(Toil.actor));

		private static readonly FieldInfo _curJobField = AccessTools.Field(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.curJob));

		private static readonly MethodInfo _getTargetMethod = AccessTools.Method(typeof(Job), nameof(Job.GetTarget));

		private static readonly FieldInfo _jobsField = AccessTools.Field(typeof(Pawn), nameof(Pawn.jobs));

		private static readonly MethodInfo _thingMethod = PatchUtility.PropertyGetter(typeof(LocalTargetInfo), nameof(LocalTargetInfo.Thing));

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(float) });
#else
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), [typeof(Vector3), typeof(Map), typeof(string), typeof(float)]);
#endif

		private static IEnumerable<CodeInstruction> ActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (original is null) {
				throw new ArgumentNullException(nameof(original));
			}

			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			if (generator is null) {
				throw new ArgumentNullException(nameof(instructions));
			}
#else
			ArgumentNullException.ThrowIfNull(original);
			ArgumentNullException.ThrowIfNull(instructions);
			ArgumentNullException.ThrowIfNull(generator);
#endif

			// If the toil or trainee index field isn't present, return unmodified instructions.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			FieldInfo toilField = original.DeclaringType?.GetField("toil");
			FieldInfo traineeIndField = original.DeclaringType?.GetField("traineeInd");
#else
			FieldInfo? toilField = original.DeclaringType?.GetField("toil");
			FieldInfo? traineeIndField = original.DeclaringType?.GetField("traineeInd");
#endif
			if (toilField is null || traineeIndField is null) {
				foreach (CodeInstruction instruction in instructions) {
					yield return instruction;
				}

				yield break;
			}

			LocalBuilder localTargetInfo = generator.DeclareLocal(typeof(LocalTargetInfo));
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] {
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldfld, toilField),
				new CodeInstruction(OpCodes.Ldfld, _actorField),
				new CodeInstruction(OpCodes.Ldfld, _jobsField),
				new CodeInstruction(OpCodes.Ldfld, _curJobField),
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldfld, traineeIndField),
				new CodeInstruction(OpCodes.Callvirt, _getTargetMethod),
				new CodeInstruction(OpCodes.Stloc, localTargetInfo),
				new CodeInstruction(OpCodes.Ldloca_S, localTargetInfo),
				new CodeInstruction(OpCodes.Call, _thingMethod),
				new CodeInstruction(OpCodes.Castclass, typeof(Pawn))
			};
#else
			CodeInstruction[] getPawnInstructions = [
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldfld, toilField),
				new(OpCodes.Ldfld, _actorField),
				new(OpCodes.Ldfld, _jobsField),
				new(OpCodes.Ldfld, _curJobField),
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldfld, traineeIndField),
				new(OpCodes.Callvirt, _getTargetMethod),
				new(OpCodes.Stloc, localTargetInfo),
				new(OpCodes.Ldloca_S, localTargetInfo),
				new(OpCodes.Call, _thingMethod),
				new(OpCodes.Castclass, typeof(Pawn))
			];
#endif

			foreach (CodeInstruction instruction in instructions) {
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
				if (instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructorInfo && constructorInfo.DeclaringType?.DeclaringType == typeof(Toils_Interpersonal)) {
					foreach (MethodInfo methodInfo in AccessTools.GetDeclaredMethods(constructorInfo.DeclaringType)) {
						_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: _actionDelegateTranspilerMethod);
					}

					continue;
				}
			}
		}
	}
}
