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

namespace Lakuna.BoundedRationality.Patches.CharacterCardUtilityPatches {
	[HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
	public static class DrawCharacterCardPatch {
#if V1_1 || V1_2 || V1_3 || V1_4
		private static readonly FieldInfo _abilitiesField = AccessTools.Field(typeof(Pawn_AbilityTracker), nameof(Pawn_AbilityTracker.abilities));
#endif

#if V1_1 || V1_2 || V1_3 || V1_4
		private static readonly ConstructorInfo _abilityListConstructor = AccessTools.Constructor(typeof(List<Ability>));
#endif

#if V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo _actionDelegateTranspilerMethod = AccessTools.Method(typeof(DrawCharacterCardPatch), nameof(ActionDelegateTranspiler));
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly FieldInfo _allTraitsField = AccessTools.Field(typeof(TraitSet), nameof(TraitSet.allTraits));
#endif

#if V1_0
		private static readonly MethodInfo _combinedDisabledWorkTagsMethod = PatchUtility.PropertyGetter(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.CombinedDisabledWorkTags));
#elif V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo _combinedDisabledWorkTagsMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.CombinedDisabledWorkTags));
#endif

#if V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo _factionMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction));
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo _filterTraitsMethod = AccessTools.Method(typeof(DrawCharacterCardPatch), nameof(FilterTraits));
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo _getBackstoryMethod = AccessTools.Method(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.GetBackstory));
#endif

#if V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo _getExtraFactionsFromQuestPartsMethod = AccessTools.Method(typeof(QuestUtility), nameof(QuestUtility.GetExtraFactionsFromQuestParts));
#endif

#if !(V1_0 || V1_1)
		private static readonly FieldInfo _guiltField = AccessTools.Field(typeof(Pawn), nameof(Pawn.guilt));
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo _isColonistMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsColonist));
#endif

#if !V1_0
		private static readonly FieldInfo _royaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo _spawnedMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Spawned));
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly FieldInfo _titleField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.title));
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo _toStringFullMethod = PatchUtility.PropertyGetter(typeof(Name), nameof(Name.ToStringFull));
#endif

#if V1_1 || V1_2 || V1_3 || V1_4
		private static IEnumerable<CodeInstruction> ActionDelegateTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original) {
			// If the pawn field isn't present, return unmodified instructions.
			FieldInfo pawnField = original.DeclaringType.GetField("pawn");
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
			}
		}
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
#pragma warning disable CA1002 // Use the same type as the value being patched.
		private static List<Trait> FilterTraits(List<Trait> traits, Pawn pawn) {
#pragma warning restore CA1002
			if (traits is null) {
				return null;
			}

			List<Trait> outValue = new List<Trait>();
			foreach (Trait trait in traits) {
				if (!KnowledgeUtility.IsTraitKnown(pawn, trait.def)) {
					continue;
				}

				outValue.Add(trait);
			}

			return outValue;
		}
#endif

		[HarmonyPrefix]
		public static bool Prefix(Pawn pawn,
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
			ref Action randomizeCallback
#else
			ref bool showName
#endif
		) {
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, pawn, ControlCategory.Control); // Name must be shown for renaming, banishing, and starting colonist randomization to work. Guilt must be shown for the "execute colonist" button.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
			if (!basic) {
				randomizeCallback = null;
			}
#else
			showName = showName && basic;
#endif
			return basic
#if !V1_0
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Personal, pawn, ControlCategory.Control) // "Renounce title" button.
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Abilities, pawn)
#endif
#if !(V1_0 || V1_1 || V1_2)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Ideoligion, pawn)
#endif
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Traits, pawn)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Backstory, pawn)
				|| KnowledgeUtility.IsInformationKnownFor(InformationCategory.Skills, pawn);
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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_1)];
#endif

			foreach (CodeInstruction instruction in instructions) {
#if V1_1 || V1_2 || V1_3 || V1_4
				if (PatchUtility.Calls(instruction, _getExtraFactionsFromQuestPartsMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}
#endif

				yield return instruction;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
				if (PatchUtility.Calls(instruction, _toStringFullMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _spawnedMethod) || PatchUtility.Calls(instruction, _isColonistMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Basic, getPawnInstructions, ControlCategory.Control)) { // `Spawned` is used only for the "banish" button. `IsColonist` is used only for the "rename colonist" button.
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _getBackstoryMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceBackstoryIfNotKnown(getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _combinedDisabledWorkTagsMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Skills, getPawnInstructions, generator, (int)WorkTags.None)) {
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

				if (PatchUtility.LoadsField(instruction, _allTraitsField)) {
					foreach (CodeInstruction i in getPawnInstructions) {
						yield return new CodeInstruction(i);
					}

					yield return new CodeInstruction(OpCodes.Call, _filterTraitsMethod);
					continue;
				}
#endif

#if V1_1 || V1_2 || V1_3 || V1_4
				if (PatchUtility.Calls(instruction, _factionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _abilitiesField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Abilities, getPawnInstructions, generator, _abilityListConstructor)) {
						yield return i;
					}

					continue;
				}

				// Apply a transpiler to action delegates.
				if (instruction.opcode == OpCodes.Newobj && instruction.operand is ConstructorInfo constructorInfo && constructorInfo.DeclaringType.DeclaringType == typeof(CharacterCardUtility)) {
					foreach (MethodInfo methodInfo in constructorInfo.DeclaringType.GetMethods()) {
						if (!methodInfo.IsDeclaredMember()) {
							continue;
						}

						_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: new HarmonyMethod(_actionDelegateTranspilerMethod));
					}

					continue;
				}
#endif

#if !V1_0
				if (PatchUtility.LoadsField(instruction, _royaltyField)) {
					// Royalty is used only for the "renounce title" button.
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator, controlCategory: ControlCategory.Control)) {
						yield return i;
					}

					continue;
				}
#endif

#if !(V1_0 || V1_1)

				if (PatchUtility.LoadsField(instruction, _guiltField)) {
					// Guilt is used only for the "execute colonist" button.
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, controlCategory: ControlCategory.Control)) {
						yield return i;
					}

					continue;
				}
#endif
			}
		}
	}
}
