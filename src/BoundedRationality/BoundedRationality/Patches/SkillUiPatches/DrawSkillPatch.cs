using System;

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using UnityEngine;

namespace Lakuna.BoundedRationality.Patches.SkillUiPatches {
	[HarmonyPatch(typeof(SkillUI), nameof(SkillUI.DrawSkill), new Type[] { typeof(SkillRecord), typeof(Rect), typeof(SkillUI.SkillDrawMode), typeof(string) })]
	public static class DrawSkillPatch {
		[HarmonyPrefix]
		private static bool Prefix(SkillRecord skill) => KnowledgeUtility.IsSkillKnown(skill);
	}
}
