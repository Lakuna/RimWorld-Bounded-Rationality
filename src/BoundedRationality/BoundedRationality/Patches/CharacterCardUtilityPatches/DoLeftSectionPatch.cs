#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), "DoLeftSection")]
	public static class DoLeftSectionPatch {
		private static readonly ConstructorInfo _abilityListConstructor = AccessTools.Constructor(typeof(List<Ability>));

		private static readonly MethodInfo _actionDelegateTranspilerMethod = AccessTools.Method(typeof(DoLeftSectionPatch), nameof(ActionDelegateTranspiler));

		private static readonly MethodInfo _allAbilitiesForReadingMethod = PatchUtility.PropertyGetter(typeof(Pawn_AbilityTracker), nameof(Pawn_AbilityTracker.AllAbilitiesForReading));

		private static readonly FieldInfo _allTraitsField = AccessTools.Field(typeof(TraitSet), nameof(TraitSet.allTraits));

		private static readonly MethodInfo _combinedDisabledWorkTagsMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.CombinedDisabledWorkTags));

		private static readonly MethodInfo _getBackstoryMethod = AccessTools.Method(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.GetBackstory));

		private static readonly FieldInfo _sourceGeneField = AccessTools.Field(typeof(Trait), nameof(Trait.sourceGene));

		private static readonly FieldInfo _titleField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.title));

		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn) =>
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn)
			|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn);

		private static IEnumerable<CodeInstruction> ActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			if (original is null) {
				throw new ArgumentNullException(nameof(original));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
			ArgumentNullException.ThrowIfNull(original);
#endif

			// If the pawn field isn't present, return unmodified instructions.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			FieldInfo pawnField = original.DeclaringType.GetField("pawn");
#else
			FieldInfo? pawnField = original.DeclaringType?.GetField("pawn");
#endif
			if (pawnField is null) {
				foreach (CodeInstruction instruction in instructions) {
					yield return instruction;
				}

				yield break;
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldfld, pawnField) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, _getBackstoryMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceBackstoryIfNotKnown(getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _titleField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _sourceGeneField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
			}
		}

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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_2) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_2)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, _allAbilitiesForReadingMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Abilities, getPawnInstructions, generator, _abilityListConstructor)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _titleField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				// Apply a transpiler to action delegates.
				if (instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructorInfo && constructorInfo.DeclaringType?.DeclaringType == typeof(CharacterCardUtility)) {
					foreach (MethodInfo methodInfo in constructorInfo.DeclaringType.GetDeclaredMethods()) {
						_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: _actionDelegateTranspilerMethod);
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _allTraitsField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Traits, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _combinedDisabledWorkTagsMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Skills, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
			}
		}
	}
}
#endif
