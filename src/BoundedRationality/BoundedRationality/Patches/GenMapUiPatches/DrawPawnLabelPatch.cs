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
	[HarmonyPatch(typeof(GenMapUI), nameof(GenMapUI.DrawPawnLabel), new Type[] { typeof(Pawn), typeof(Rect), typeof(float), typeof(float), typeof(Dictionary<string, string>), typeof(GameFont), typeof(bool), typeof(bool) })]
	public static class DrawPawnLabelPatch {
		private static readonly MethodInfo SummaryHealthPercentMethod = PatchUtility.PropertyGetter(typeof(SummaryHealthHandler), nameof(SummaryHealthHandler.SummaryHealthPercent));

		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn) => BoundedRationalityMod.Settings.NeverHidePawnLabels
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Health, pawn);

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, SummaryHealthPercentMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator, 1f)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
