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

namespace Lakuna.BoundedRationality.Patches.CorpsePatches {
	[HarmonyPatch(typeof(Corpse), nameof(Corpse.GetInspectString))]
	public static class GetInspectStringPatch {
		private static readonly MethodInfo FactionMethod = PatchUtility.PropertyGetter(typeof(Thing), nameof(Thing.Faction));

#if !(V1_0 || V1_1 || V1_2 || V1_3)
		private static readonly MethodInfo GetFirstHediffMethod = AccessTools.Method(typeof(HediffSet), nameof(HediffSet.GetFirstHediff));
#endif

		private static readonly MethodInfo ToStringTicksToPeriodVagueMethod = AccessTools.Method(typeof(GenDate), nameof(GenDate.ToStringTicksToPeriodVague));

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
			if (instructions is null) {
				throw new ArgumentNullException(nameof(instructions));
			}

			CodeInstruction[] getThingInstructions = new CodeInstruction[] { new CodeInstruction(OpCodes.Ldarg_0) };

			foreach (CodeInstruction instruction in instructions) {
				yield return instruction;

				if (PatchUtility.Calls(instruction, ToStringTicksToPeriodVagueMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfThingNotKnown(InformationCategory.Basic, getThingInstructions, generator, string.Empty)) {
						yield return i;
					}

					continue;
				}

				if (PatchUtility.Calls(instruction, FactionMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfThingNotKnown(InformationCategory.Basic, getThingInstructions, generator)) {
						yield return i;
					}

					continue;
				}

#if !(V1_0 || V1_1 || V1_2 || V1_3)
				if (PatchUtility.Calls(instruction, GetFirstHediffMethod)) {
					foreach (CodeInstruction i in PatchUtility.ReplaceIfThingNotKnown(InformationCategory.Health, getThingInstructions, generator)) {
						yield return i;
					}

					continue;
				}
#endif
			}
		}
	}
}
