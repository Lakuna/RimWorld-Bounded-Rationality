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
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo ToStringFullMethod = PatchUtility.PropertyGetter(typeof(Name), nameof(Name.ToStringFull));

		private static readonly MethodInfo SpawnedMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Spawned));

		private static readonly MethodInfo IsColonistMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsColonist));

		private static readonly MethodInfo GetBackstoryMethod = AccessTools.Method(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.GetBackstory));

#if V1_0
		private static readonly MethodInfo CombinedDisabledWorkTagsMethod = PatchUtility.PropertyGetter(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.CombinedDisabledWorkTags));
#else
		private static readonly MethodInfo CombinedDisabledWorkTagsMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.CombinedDisabledWorkTags));
#endif

		private static readonly FieldInfo TitleField = AccessTools.Field(typeof(Pawn_StoryTracker), nameof(Pawn_StoryTracker.title));

		private static readonly FieldInfo AllTraitsField = AccessTools.Field(typeof(TraitSet), nameof(TraitSet.allTraits));

		private static readonly MethodInfo FilterTraitsMethod = AccessTools.Method(typeof(DrawCharacterCardPatch), nameof(FilterTraits));

		private static List<Trait> FilterTraits(List<Trait> traits, Pawn pawn) {
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

#if V1_1 || V1_2 || V1_3 || V1_4
		private static readonly MethodInfo FactionMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly MethodInfo GetExtraFactionsFromQuestPartsMethod = AccessTools.Method(typeof(QuestUtility), nameof(QuestUtility.GetExtraFactionsFromQuestParts));

		private static readonly FieldInfo AbilitiesField = AccessTools.Field(typeof(Pawn_AbilityTracker), nameof(Pawn_AbilityTracker.abilities));

		private static readonly ConstructorInfo AbilityListConstructor = AccessTools.Constructor(typeof(List<Ability>));

		private static readonly MethodInfo ActionDelegateTranspilerMethod = AccessTools.Method(typeof(DrawCharacterCardPatch), nameof(ActionDelegateTranspiler));

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

				if (PatchUtility.Calls(instruction, GetBackstoryMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceBackstoryIfNotKnown(getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, TitleField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
			}
		}
#endif

#if !V1_0
		private static readonly FieldInfo RoyaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));
#endif

#if !(V1_0 || V1_1)
		private static readonly FieldInfo GuiltField = AccessTools.Field(typeof(Pawn), nameof(Pawn.guilt));
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
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
#if V1_1 || V1_2 || V1_3 || V1_4
				if (PatchUtility.Calls(instruction, GetExtraFactionsFromQuestPartsMethod)) {
					foreach (CodeInstruction i in PatchUtility.SkipIfPawnNotKnown(instruction, InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					// Skip the normal instruction (already returned above).
					continue;
				}
#endif

				yield return instruction;

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4
				if (PatchUtility.Calls(instruction, ToStringFullMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, SpawnedMethod) || PatchUtility.Calls(instruction, IsColonistMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Basic, getPawnInstructions, ControlCategory.Control)) { // `Spawned` is used only for the "banish" button. `IsColonist` is used only for the "rename colonist" button.
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, GetBackstoryMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceBackstoryIfNotKnown(getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, CombinedDisabledWorkTagsMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Skills, getPawnInstructions, generator, (int)WorkTags.None)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, TitleField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, AllTraitsField)) {
					foreach (CodeInstruction i in getPawnInstructions) {
						yield return new CodeInstruction(i);
					}

					yield return new CodeInstruction(OpCodes.Call, FilterTraitsMethod);
					continue;
				}
#endif

#if V1_1 || V1_2 || V1_3 || V1_4
				if (PatchUtility.Calls(instruction, FactionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, AbilitiesField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Abilities, getPawnInstructions, generator, AbilityListConstructor)) {
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

						_ = HarmonyPatcher.Instance.Patch(methodInfo, transpiler: new HarmonyMethod(ActionDelegateTranspilerMethod));
					}

					continue;
				}
#endif

#if !V1_0
				if (PatchUtility.LoadsField(instruction, RoyaltyField)) {
					// Royalty is used only for the "renounce title" button.
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator, controlCategory: ControlCategory.Control)) {
						yield return i;
					}

					continue;
				}
#endif

#if !(V1_0 || V1_1)

				if (PatchUtility.LoadsField(instruction, GuiltField)) {
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
