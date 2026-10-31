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
		private static readonly FieldInfo _guiltField = AccessTools.Field(typeof(Pawn), nameof(Pawn.guilt));

		private static readonly MethodInfo _selectorMethod = PatchUtility.PropertyGetter(typeof(Find), nameof(Find.Selector));

		private static readonly MethodInfo _singleSelectedThingMethod = PatchUtility.PropertyGetter(typeof(Selector), nameof(Selector.SingleSelectedThing));

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
			CodeInstruction[] getThingInstructions = new CodeInstruction[] { PatchUtility.LoadValue(_selectorMethod), new CodeInstruction(OpCodes.Call, _singleSelectedThingMethod) };
#else
			CodeInstruction[] getThingInstructions = [PatchUtility.LoadValue(_selectorMethod), new(OpCodes.Call, _singleSelectedThingMethod)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.LoadsField(instruction, _guiltField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfThingNotKnown(InformationCategory.Basic, getThingInstructions, generator)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
