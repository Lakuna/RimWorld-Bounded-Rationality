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

namespace Lakuna.BoundedRationality.Patches.PawnStoryTrackerPatches {
	[HarmonyPatch(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.TitleShort), MethodType.Getter)]
	public static class TitleShortPatch {
		private static readonly FieldInfo PawnField = AccessTools.Field(typeof(Pawn_StoryTracker), "pawn");

		private static readonly FieldInfo TitleField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.title));

#if V1_0 || V1_1 || V1_2 || V1_3
		private static readonly FieldInfo AdulthoodField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.adulthood));

		private static readonly FieldInfo ChildhoodField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.childhood));
#else
		private static readonly FieldInfo AdulthoodField = AccessTools.Field(typeof(Pawn_StoryTracker), "adulthood");

		private static readonly FieldInfo ChildhoodField = AccessTools.Field(typeof(Pawn_StoryTracker), "childhood");
#endif

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, PawnField) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.LoadsField(instruction, TitleField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, AdulthoodField) || PatchUtility.LoadsField(instruction, ChildhoodField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceBackstoryIfNotKnown(getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
