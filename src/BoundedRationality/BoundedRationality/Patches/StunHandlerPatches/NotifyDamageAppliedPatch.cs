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

namespace Lakuna.BoundedRationality.Patches.StunHandlerPatches {
	[HarmonyPatch(typeof(StunHandler), nameof(StunHandler.Notify_DamageApplied))]
	public static class NotifyDamageAppliedPatch {
		private static readonly FieldInfo _parentField = AccessTools.Field(typeof(StunHandler), nameof(StunHandler.parent));

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), new Type[] { typeof(Vector3), typeof(Map), typeof(string), typeof(Color), typeof(float) });
#else
		private static readonly MethodInfo _throwTextMethod = AccessTools.Method(typeof(MoteMaker), nameof(MoteMaker.ThrowText), [typeof(Vector3), typeof(Map), typeof(string), typeof(Color), typeof(float)]);
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
			CodeInstruction[] getThingInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, _parentField) };
#else
			CodeInstruction[] getThingInstructions = [new(OpCodes.Ldarg_0), new(OpCodes.Ldfld, _parentField)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				// This text mote is thrown only when the damaged thing has adapted to being stunned.
				if (PatchUtility.Calls(instruction, _throwTextMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfThingNotKnown(instruction, InformationCategory.Meta, getThingInstructions, generator, controlCategory: ControlCategory.TextMote)) {
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
