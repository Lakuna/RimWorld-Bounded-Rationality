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

using UnityEngine;

using Verse;

namespace Lakuna.BoundedRationality.Patches.GenMapUiPatches {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
	[HarmonyPatch(typeof(GenMapUI), nameof(GenMapUI.DrawPawnLabel), new Type[] { typeof(Pawn), typeof(Rect), typeof(float), typeof(float), typeof(Dictionary<string, string>), typeof(GameFont), typeof(bool), typeof(bool) })]
#else
	[HarmonyPatch(typeof(GenMapUI), nameof(GenMapUI.DrawPawnLabel), [typeof(Pawn), typeof(Rect), typeof(float), typeof(float), typeof(Dictionary<string, string>), typeof(GameFont), typeof(bool), typeof(bool)])]
#endif
	public static class DrawPawnLabelPatch {
		private static readonly MethodInfo _summaryHealthPercentMethod = PatchUtility.PropertyGetter(typeof(SummaryHealthHandler), nameof(SummaryHealthHandler.SummaryHealthPercent));

		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn) =>
			(BoundedRationalityMod.Settings?.NeverHidePawnLabels ?? false)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn);

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, _summaryHealthPercentMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator, 1f)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
