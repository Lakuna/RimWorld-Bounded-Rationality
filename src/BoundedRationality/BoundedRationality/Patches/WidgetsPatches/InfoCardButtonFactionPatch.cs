#if !V1_0
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
using System;
#endif

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.WidgetsPatches {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
	[HarmonyPatch(typeof(Widgets), nameof(Widgets.InfoCardButton), new Type[] { typeof(float), typeof(float), typeof(Faction) })]
#else
	[HarmonyPatch(typeof(Widgets), nameof(Widgets.InfoCardButton), [typeof(float), typeof(float), typeof(Faction)])]
#endif
	public static class InfoCardButtonFactionPatch {
		[HarmonyPrefix]
		public static bool Prefix(Faction faction) =>
			KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, faction, ControlCategory.Control);
	}
}
#endif
