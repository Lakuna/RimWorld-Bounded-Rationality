#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetReasonsForDisabledWorkType))]
	public static class GetReasonsForDisabledWorkTypePatch {
#if V1_3
		private static readonly MethodInfo DisabledWorkTypesMethod = PatchUtility.PropertyGetter(typeof(Backstory), nameof(Backstory.DisabledWorkTypes));
#else
		private static readonly MethodInfo DisabledWorkTypesMethod = PatchUtility.PropertyGetter(typeof(BackstoryDef), nameof(BackstoryDef.DisabledWorkTypes));
#endif

		private static readonly FieldInfo AllTraitsField = AccessTools.Field(typeof(TraitSet), nameof(TraitSet.allTraits));

		private static readonly MethodInfo FilterTraitsMethod = AccessTools.Method(typeof(GetReasonsForDisabledWorkTypePatch), nameof(FilterTraits));

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

		private static readonly FieldInfo RoyaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

		private static readonly MethodInfo IdeoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));

#if !(V1_3 || V1_4)
		private static readonly MethodInfo IsMutantMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));
#endif

		private static readonly FieldInfo HealthField = AccessTools.Field(typeof(Pawn), nameof(Pawn.health));

#if !V1_3
		private static readonly MethodInfo IsWorkTypeDisabledByAgeMethod = AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.IsWorkTypeDisabledByAge));
#endif

#if V1_3
		private static readonly Backstory DefaultBackstory = BackstoryDatabase.allBackstories.Values.ToList().Find((def) => !def.DisabledWorkTypes.Any());
#else
		private static readonly BackstoryDef DefaultBackstory = DefDatabase<BackstoryDef>.AllDefsListForReading.Find((def) => !def.DisabledWorkTypes.Any());
#endif

		private static readonly FieldInfo DefaultBackstoryField = AccessTools.Field(typeof(GetReasonsForDisabledWorkTypePatch), nameof(DefaultBackstory));

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			if (generator is null) {
				throw new ArgumentNullException(nameof(generator));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };
			CodeInstruction[] getWorkTypeInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };

			foreach (CodeInstruction instruction in instructions) {
				// Replace unknown backstories with a backstory that doesn't have any disabled work types before getting the disabled work types. This is done this way so that it can utilize the existing `PatchUtility.ReplaceBackstoryIfNotKnown` method.
				if (PatchUtility.Calls(instruction, DisabledWorkTypesMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceBackstoryIfNotKnown(getPawnInstructions, generator, DefaultBackstoryField)) {
						yield return i;
					}
				}

				yield return instruction;

				if (PatchUtility.LoadsField(instruction, AllTraitsField)) {
					foreach (CodeInstruction i in getPawnInstructions) {
						yield return new CodeInstruction(i);
					}

					yield return new CodeInstruction(OpCodes.Call, FilterTraitsMethod);
					continue;
				}

				if (PatchUtility.LoadsField(instruction, RoyaltyField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, IdeoMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_3 || V1_4)
				if (PatchUtility.Calls(instruction, IsMutantMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

				if (PatchUtility.LoadsField(instruction, HealthField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !V1_3
				if (PatchUtility.Calls(instruction, IsWorkTypeDisabledByAgeMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator, false)) {
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
