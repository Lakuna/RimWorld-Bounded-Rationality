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
		private static readonly FieldInfo _ageTrackerField = AccessTools.Field(typeof(Pawn), nameof(Pawn.ageTracker));

		private static readonly MethodInfo _factionMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly MethodInfo _isCreepJoinerMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsCreepJoiner));
#endif

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
		private static readonly MethodInfo _isMutantMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.IsMutant));
#endif

		[HarmonyPrefix]
#if V1_0
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Prefix(ref bool writeAge, Pawn __instance) {
#pragma warning restore IDE1006
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance);
			writeAge = writeAge && basic;
		}
#elif V1_1 || V1_2 || V1_3
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Prefix(ref bool writeFaction, Pawn __instance) {
#pragma warning restore IDE1006
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance);
			writeFaction = writeFaction && basic;
		}
#else
#pragma warning disable IDE1006 // Harmony injected values require specific names.
		public static void Prefix(ref bool writeGender, ref bool writeFaction, Pawn __instance) {
#pragma warning restore IDE1006
			bool basic = KnowledgeUtility.IsInformationKnownFor(InformationCategory.Basic, __instance);
			writeGender = writeGender && basic;
			writeFaction = writeFaction && basic;
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
			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };
#else
			CodeInstruction[] getPawnInstructions = [new(OpCodes.Ldarg_0)];
#endif

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, _factionMethod) || PatchUtility.LoadsField(instruction, _ageTrackerField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3 || V1_4)
				if (PatchUtility.Calls(instruction, _isMutantMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _isCreepJoinerMethod)) {
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
