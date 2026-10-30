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

using Verse.AI;

namespace Lakuna.BoundedRationality.Patches.MentalStatePatches {
	[HarmonyPatch(typeof(MentalState), nameof(MentalState.PostEnd))]
	public static class PostEndPatch {
		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(MentalState), nameof(MentalState.pawn));

		private static readonly MethodInfo ShouldSendNotificationAboutMethod = AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.ShouldSendNotificationAbout));

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, PawnField) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, ShouldSendNotificationAboutMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Needs, getPawnInstructions, ControlCategory.Letter)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
