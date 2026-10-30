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
		private static readonly MethodInfo CurJobMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.CurJob));

		private static readonly MethodInfo IsFormingCaravanMethod = AccessTools.Method(typeof(CaravanFormingUtility), nameof(CaravanFormingUtility.IsFormingCaravan));

		private static readonly MethodInfo InAggroMentalStateMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.InAggroMentalState));

		private static readonly MethodInfo InMentalStateMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.InMentalState));

		private static readonly MethodInfo InBedMethod = AccessTools.Method(typeof(RestUtility), nameof(RestUtility.InBed));

		private static readonly FieldInfo NeedsField = AccessTools.Field(typeof(Pawn), nameof(Pawn.needs));

		private static readonly MethodInfo IsIdleMethod = PatchUtility.PropertyGetter(typeof(Pawn_MindState), nameof(Pawn_MindState.IsIdle));

		private static readonly MethodInfo IsBurningMethod = AccessTools.Method(typeof(FireUtility), nameof(FireUtility.IsBurning), new Type[] { typeof(Pawn) });

		private static readonly MethodInfo InspiredMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Inspired));

#if !(V1_0 || V1_1 || V1_2)
		private static readonly MethodInfo IdeoMethod = PatchUtility.PropertyGetter(typeof(Pawn), nameof(Pawn.Ideo));
#endif

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getPawnInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_2) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, CurJobMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Basic, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, IsFormingCaravanMethod) || PatchUtility.Calls(instruction, IsIdleMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Basic, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, InAggroMentalStateMethod) || PatchUtility.Calls(instruction, InMentalStateMethod) || PatchUtility.Calls(instruction, InspiredMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Needs, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				// `InBed` is used only to check if the pawn is currently healing in a medical bed.
				if (PatchUtility.Calls(instruction, InBedMethod) || PatchUtility.Calls(instruction, IsBurningMethod)) {
					foreach (CodeInstruction i in PatchUtility.AndPawnKnown(InformationCategory.Health, getPawnInstructions)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.LoadsField(instruction, NeedsField)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfPawnNotKnown(InformationCategory.Needs, getPawnInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2)
				if (PatchUtility.Calls(instruction, IdeoMethod)) {
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
