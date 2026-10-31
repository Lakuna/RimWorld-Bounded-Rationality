#if !(V1_0 || V1_1 || V1_2)
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
#endif

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.TraitPatches {
	[HarmonyPatch(typeof(Trait), nameof(Trait.TipString))]
	public static class TipStringPatch {
#if !(V1_0 || V1_1 || V1_2)
		private static readonly MethodInfo _getAffectedIssuesMethod = AccessTools.Method(typeof(TraitDegreeData), nameof(TraitDegreeData.GetAffectedIssues));

		private static readonly MethodInfo _getAffectedMemesMethod = AccessTools.Method(typeof(TraitDegreeData), nameof(TraitDegreeData.GetAffectedMemes));

		private static readonly ConstructorInfo _issueDefListConstructor = AccessTools.Constructor(typeof(List<IssueDef>));

		private static readonly ConstructorInfo _memeDefListConstructor = AccessTools.Constructor(typeof(List<MemeDef>));

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly FieldInfo _sourceGeneField = AccessTools.Field(typeof(Trait), nameof(Trait.sourceGene));
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly MethodInfo _suppressedMethod = PatchUtility.PropertyGetter(typeof(Trait), nameof(Trait.Suppressed));
#endif

		[HarmonyPostfix]
#if V1_0 || V1_1
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Pawn pawn, Trait __instance, ref string __result) {
#pragma warning restore IDE1006
			if (KnowledgeUtility.IsTraitKnown(pawn, __instance.def)) {
				return;
			}

			__result = "BR.Unknown".Translate().CapitalizeFirst().EndWithPeriod().StringSafe;
		}
#else
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Postfix(Trait __instance, ref string __result) {
#pragma warning restore IDE1006
			if (KnowledgeUtility.IsTraitKnown(__instance)) {
				return;
			}

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
			__result = "BR.Unknown".Translate().CapitalizeFirst().EndWithPeriod().StringSafe();
#else
			__result = "BR.Unknown".Translate().CapitalizeFirst().EndWithPeriod().StringSafe;
#endif
		}
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

				if (PatchUtility.Calls(instruction, _getAffectedIssuesMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator, _issueDefListConstructor)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _getAffectedMemesMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator, _memeDefListConstructor)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
				if (PatchUtility.LoadsField(instruction, _sourceGeneField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Personal, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _suppressedMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Personal, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
#endif
			}
		}
#endif
	}
}
