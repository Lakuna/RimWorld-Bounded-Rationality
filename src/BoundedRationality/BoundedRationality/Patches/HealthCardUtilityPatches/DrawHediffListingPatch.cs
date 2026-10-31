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

namespace Lakuna.BoundedRationality.Patches.HealthCardUtilityPatches {
	[HarmonyPatch(typeof(HealthCardUtility), nameof(HealthCardUtility.DrawHediffListing))]
	public static class DrawHediffListingPatch {
		private static readonly MethodInfo _bleedRateTotalMethod = PatchUtility.PropertyGetter(typeof(HediffSet), nameof(HediffSet.BleedRateTotal));

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo _genesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.genes));
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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_1)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, _bleedRateTotalMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator, 0f)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
				if (PatchUtility.LoadsField(instruction, _genesField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif
			}
		}
	}
}
