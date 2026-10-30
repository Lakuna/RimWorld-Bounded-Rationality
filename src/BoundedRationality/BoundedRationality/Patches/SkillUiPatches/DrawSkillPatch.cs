#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
using System;
#endif

#if V1_0
using Harmony;
#else
using HarmonyLib;
#endif

using Lakuna.BoundedRationality.Utility;

using RimWorld;

using UnityEngine;

namespace Lakuna.BoundedRationality.Patches.SkillUiPatches {
#if V1_0 || V1_1 || V1_2 || V1_3 || V1_4 || V1_5 || V1_6
	[HarmonyPatch(typeof(SkillUI), nameof(SkillUI.DrawSkill), new Type[] { typeof(SkillRecord), typeof(Rect), typeof(SkillUI.SkillDrawMode), typeof(string) })]
#else
	[HarmonyPatch(typeof(SkillUI), nameof(SkillUI.DrawSkill), [typeof(SkillRecord), typeof(Rect), typeof(SkillUI.SkillDrawMode), typeof(string)])]
#endif
	public static class DrawSkillPatch {
		[HarmonyPrefix]
		public static bool Prefix(SkillRecord skill) =>
			KnowledgeUtility.IsSkillKnown(skill);
	}
}
