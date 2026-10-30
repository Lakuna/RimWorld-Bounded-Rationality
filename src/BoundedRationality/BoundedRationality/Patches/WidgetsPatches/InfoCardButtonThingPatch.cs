using System;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using Verse;

namespace Lakuna.BoundedRationality.Patches.WidgetsPatches {
	[HarmonyPatch(typeof(Widgets), nameof(Widgets.InfoCardButton), new Type[] { typeof(float), typeof(float), typeof(Thing) })]
	public static class InfoCardButtonThingPatch {
		[HarmonyPrefix]
		private static bool Prefix(Thing thing) => !(thing is Pawn pawn) || KnowledgeUtility.IsInformationKnownFor(InformationCategory.Meta, pawn, ControlCategory.Control);
	}
}
