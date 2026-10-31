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
		private static readonly FieldInfo _allTraitsField = AccessTools.Field(typeof(TraitSet), nameof(TraitSet.allTraits));

#if V1_3
		private static readonly Backstory _defaultBackstory = BackstoryDatabase.allBackstories.Values.ToList().Find((def) => !def.DisabledWorkTypes.Any());
#elif V1_4 || V1_5 || V1_6
		private static readonly BackstoryDef _defaultBackstory = DefDatabase<BackstoryDef>.AllDefsListForReading?.Find((def) => !def.DisabledWorkTypes.Any());
#else
		private static readonly BackstoryDef? _defaultBackstory = DefDatabase<BackstoryDef>.AllDefsListForReading?.Find((def) => !def.DisabledWorkTypes.Any());
#endif

		private static readonly FieldInfo _defaultBackstoryField = AccessTools.Field(typeof(GetReasonsForDisabledWorkTypePatch), nameof(_defaultBackstory));

#if V1_3
		private static readonly MethodInfo _disabledWorkTypesMethod = PatchUtility.PropertyGetter(typeof(Backstory), nameof(Backstory.DisabledWorkTypes));
#else
		private static readonly MethodInfo _disabledWorkTypesMethod = PatchUtility.PropertyGetter(typeof(BackstoryDef), nameof(BackstoryDef.DisabledWorkTypes));
#endif

		private static readonly MethodInfo _filterTraitsMethod = AccessTools.Method(typeof(GetReasonsForDisabledWorkTypePatch), nameof(FilterTraits));

		private static readonly FieldInfo _healthField = AccessTools.Field(typeof(Pawn), nameof(Pawn.health));

		private static readonly MethodInfo _ideoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));

#if !(V1_3 || V1_4)
		private static readonly MethodInfo _isMutantMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));
#endif

#if !V1_3
		private static readonly MethodInfo _isWorkTypeDisabledByAgeMethod = AccessTools.Method(typeof(PawnUtility), nameof(PawnUtility.IsWorkTypeDisabledByAge));
#endif

		private static readonly FieldInfo _royaltyField = AccessTools.Field(typeof(Pawn), nameof(Pawn.royalty));

#pragma warning disable CA1002 // Use the same type as the value being patched.
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static List<Trait> FilterTraits(List<Trait> traits, Pawn pawn) {
#else
		private static List<Trait>? FilterTraits(List<Trait>? traits, Pawn? pawn) {
#endif
#pragma warning restore CA1002
			if (traits is null) {
				return null;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			List<Trait> outValue = new List<Trait>();
#else
			List<Trait> outValue = [];
#endif
			foreach (Trait trait in traits) {
				if (!KnowledgeUtility.IsTraitKnown(pawn, trait.def)) {
					continue;
				}

				outValue.Add(trait);
			}

			return outValue;
		}

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			if (generator is null) {
				throw new ArgumentNullException(nameof(generator));
			}
#else
			ArgumentNullException.ThrowIfNull(instructions);
			ArgumentNullException.ThrowIfNull(generator);
#endif

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };
			CodeInstruction[] getWorkTypeInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_1) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0)];
			CodeInstruction[] getWorkTypeInstructions = [new(OpCodes.Ldarg_1)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				// Replace unknown backstories with a backstory that doesn't have any disabled work types before getting the disabled work types. This is done this way so that it can utilize the existing `PatchUtility.ReplaceBackstoryIfNotKnown` method.
				if (PatchUtility.Calls(instruction, _disabledWorkTypesMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceBackstoryIfNotKnown(getPawnInstructions, generator, _defaultBackstoryField)) {
						yield return i;
					}
				}

				yield return instruction;

				if (PatchUtility.LoadsField(instruction, _allTraitsField)) {
					foreach (CodeInstruction i in getPawnInstructions) {
						yield return new CodeInstruction(i);
					}

					yield return new CodeInstruction(OpCodes.Call, _filterTraitsMethod);
					continue;
				}

				if (PatchUtility.LoadsField(instruction, _royaltyField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
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

#if !(V1_3 || V1_4)
				if (PatchUtility.Calls(instruction, _isMutantMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif

				if (PatchUtility.LoadsField(instruction, _healthField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !V1_3
				if (PatchUtility.Calls(instruction, _isWorkTypeDisabledByAgeMethod)) {
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
