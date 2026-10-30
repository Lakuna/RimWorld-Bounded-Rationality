using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.AlertThoughtPatches {
	[HarmonyPatch(typeof(Alert_Thought), nameof(Alert_Thought.GetReport))]
	public static class GetReportPatch {
		private static readonly MethodInfo ThoughtMethod = PatchUtility.PropertyGetter(typeof(Alert_Thought), "Thought");

		[HarmonyPostfix]
		public static void Postfix(Alert_Thought __instance, ref AlertReport __result) {
			if (BoundedRationalityMod.Settings.NeverHideAlerts || !(ThoughtMethod.Invoke(__instance, MiscellaneousUtility.EmptyArray()) is ThoughtDef thoughtDef)) {
				return;
			}

#if V1_0
			__result.culprits = __result.culprits?.Where((target) => {
				if (!(target.Thing is Pawn pawn)) {
					return false;
				}

				List<Thought> thoughts = new List<Thought>();
				pawn.needs.mood.thoughts.GetAllMoodThoughts(thoughts);
				return thoughts.Where((thought) => thought.def == thoughtDef).Any((thought) => KnowledgeUtility.IsThoughtKnown(thought));
			}).ToList();
#elif V1_1 || V1_2 || V1_3 || V1_4
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => {
				List<Thought> thoughts = new List<Thought>();
				pawn.needs.mood.thoughts.GetAllMoodThoughts(thoughts);
				return thoughts.Where((thought) => thought.def == thoughtDef).Any((thought) => KnowledgeUtility.IsThoughtKnown(thought));
			}).ToList();
#else
			__result.culpritsPawns = __result.culpritsPawns?.Where((pawn) => {
				List<Thought> thoughts = new List<Thought>();
				pawn.needs.mood.thoughts.GetMoodThoughtsFor(thoughtDef, thoughts);
				return thoughts.Any((thought) => KnowledgeUtility.IsThoughtKnown(thought));
			}).ToList();
#endif
			__result.active = __result.AnyCulpritValid;
		}
	}
}
