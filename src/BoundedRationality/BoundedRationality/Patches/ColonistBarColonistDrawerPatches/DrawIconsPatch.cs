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
using RimWorld.Planet;

using Verse;
using Verse.AI;

namespace Lakuna.BoundedRationality.Patches.ColonistBarColonistDrawerPatches {
	[HarmonyPatch(typeof(ColonistBarColonistDrawer), "DrawIcons")]
	public static class DrawIconsPatch {
		private static readonly MethodInfo _curJobMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.CurJob));

#if !(V1_0 || V1_1 || V1_2)
		private static readonly MethodInfo _ideoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));
#endif

		private static readonly MethodInfo _inAggroMentalStateMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.InAggroMentalState));

		private static readonly MethodInfo _inBedMethod = AccessTools.Method(typeof(RestUtility), nameof(RestUtility.InBed));

		private static readonly MethodInfo _inMentalStateMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.InMentalState));

		private static readonly MethodInfo _inspiredMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Inspired));

#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
		private static readonly MethodInfo _isBurningMethod = AccessTools.Method(typeof(FireUtility), nameof(FireUtility.IsBurning), new Type[] { typeof(Pawn) });
#else
		private static readonly MethodInfo _isBurningMethod = AccessTools.Method(typeof(FireUtility), nameof(FireUtility.IsBurning), [typeof(Pawn)]);
#endif

		private static readonly MethodInfo _isFormingCaravanMethod = AccessTools.Method(typeof(CaravanFormingUtility), nameof(CaravanFormingUtility.IsFormingCaravan));

		private static readonly MethodInfo _isIdleMethod = PatchUtility.PropertyGetter(typeof(Pawn_MindState), nameof(Pawn_MindState.IsIdle));

		private static readonly FieldInfo _needsField = AccessTools.Field(typeof(Pawn), nameof(Pawn.needs));

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

				if (PatchUtility.Calls(instruction, _curJobMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _isFormingCaravanMethod) || PatchUtility.Calls(instruction, _isIdleMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Basic, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, _inAggroMentalStateMethod) || PatchUtility.Calls(instruction, _inMentalStateMethod) || PatchUtility.Calls(instruction, _inspiredMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Needs, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				// `InBed` is used only to check if the pawn is currently healing in a medical bed.
				if (PatchUtility.Calls(instruction, _inBedMethod) || PatchUtility.Calls(instruction, _isBurningMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, _needsField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Needs, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2)
				if (PatchUtility.Calls(instruction, _ideoMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Ideoligion, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif
			}
		}
	}
}
