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

using Verse;

namespace Lakuna.BoundedRationality.Patches.PawnPatches {
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.MainDesc))]
	public static class MainDescPatch {
		private static readonly MethodInfo FactionMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

		private static readonly FieldInfo AgeTrackerField = AccessTools.Field(typeof(Pawn), nameof(Pawn.ageTracker));

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly MethodInfo IsMutantMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));

		private static readonly MethodInfo IsCreepJoinerMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsCreepJoiner));
#endif

		[HarmonyPrefix]
		private static void Prefix(
#if V1_0
			ref bool writeAge,
#else
#if !(V1_1 || V1_2 || V1_3)
			ref bool writeGender,
#endif
			ref bool writeFaction,
#endif
#pragma warning disable CA1707
			Pawn __instance
#pragma warning restore CA1707
			) {
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance);
#if V1_0
			writeAge = writeAge && basic;
#else
#if !(V1_1 || V1_2 || V1_3)
			writeGender = writeGender && basic;
#endif
			writeFaction = writeFaction && basic;
#endif
		}

		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, FactionMethod) || PatchUtility.LoadsField(instruction, AgeTrackerField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
				if (PatchUtility.Calls(instruction, IsMutantMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, IsCreepJoinerMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Basic, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}
#endif
			}
		}
	}
}
