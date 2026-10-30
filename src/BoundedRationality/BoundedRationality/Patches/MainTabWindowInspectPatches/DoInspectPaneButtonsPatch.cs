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

using Verse;

namespace Lakuna.BoundedRationality.Patches.MainTabWindowInspectPatches {
	[HarmonyPatch(typeof(MainTabWindow_Inspect), nameof(MainTabWindow_Inspect.DoInspectPaneButtons))]
	public static class DoInspectPaneButtonsPatch {
		private static readonly MethodInfo SelectorMethod = PatchUtility.PropertyGetter(typeof(Find), nameof(Find.Selector));

		private static readonly MethodInfo SingleSelectedThingMethod = PatchUtility.PropertyGetter(typeof(Selector), nameof(Selector.SingleSelectedThing));

		private static readonly FieldInfo GuiltField = AccessTools.Field(typeof(Pawn), nameof(Pawn.guilt));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getThingInstructions = new CodeInstruction[] { PatchUtility.LoadValue(SelectorMethod), new CodeInstruction(OpCodes.Call, SingleSelectedThingMethod) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.LoadsField(instruction, GuiltField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfThingNotKnown(InformationCategory.Basic, getThingInstructions, generator)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
