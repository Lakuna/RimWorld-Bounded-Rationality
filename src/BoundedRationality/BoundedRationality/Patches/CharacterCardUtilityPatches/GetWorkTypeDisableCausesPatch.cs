#if !V1_0
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "GetWorkTypeDisableCauses")]
	public static class GetWorkTypeDisableCausesPatch {
#if V1_1 || V1_2 || V1_3
		private static readonly FieldInfo _adulthoodField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.adulthood));
#endif

#if !(V1_1 || V1_2 || V1_3)
		private static readonly MethodInfo _adulthoodMethod = PatchUtility.PropertyGetter(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.Adulthood));
#endif

#if V1_1 || V1_2 || V1_3
		private static readonly FieldInfo _childhoodField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.childhood));
#endif

#if !(V1_1 || V1_2 || V1_3)
		private static readonly MethodInfo _childhoodMethod = PatchUtility.PropertyGetter(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.Childhood));
#endif

#if !(V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo _genesField = AccessTools.Field(typeof(Pawn), nameof(Pawn.genes));
#endif

		private static readonly FieldInfo _healthField = AccessTools.Field(typeof(Pawn), nameof(Pawn.health));

#if !(V1_1 || V1_2 || V1_3)
		private static readonly MethodInfo _ideoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));
#endif

#if !(V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly MethodInfo _isMutantMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));
#endif

		private static readonly FieldInfo _royaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

		private static readonly FieldInfo _traitsField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.traits));

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

#if V1_1 || V1_2 || V1_3
				if (PatchUtility.LoadsField(instruction, _childhoodField) || PatchUtility.LoadsField(instruction, _adulthoodField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceBackstoryIfNotKnown(getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#else
				if (PatchUtility.Calls(instruction, _childhoodMethod) || PatchUtility.Calls(instruction, _adulthoodMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceBackstoryIfNotKnown(getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _ideoMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

#if !(V1_1 || V1_2 || V1_3 || V1_4)
				if (PatchUtility.Calls(instruction, _isMutantMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
#endif

				if (PatchUtility.LoadsField(instruction, _healthField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Health, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _traitsField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Traits, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _royaltyField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_1 || V1_2 || V1_3)
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
#endif
