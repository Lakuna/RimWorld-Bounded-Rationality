#if !V1_0
using System;

using HarmonyLib;

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using Verse;

namespace Lakuna.BoundedRationality.Patches.WidgetsPatches {
	[HarmonyPatch(typeof(Widgets), nameof(Widgets.InfoCardButton), new Type[] { typeof(float), typeof(float), typeof(Faction) })]
	public static class InfoCardButtonFactionPatch {
		[HarmonyPrefix]
		private static bool Prefix(Faction faction) => KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, faction, ControlCategory.Control);
	}
}
#endif
