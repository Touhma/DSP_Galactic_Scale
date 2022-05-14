using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Mono.Cecil.Cil;
using UnityEngine;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    public class PatchOnSprayCoaterComponent
    {
        // Replaces all 200f with planet.realRadius. There is a -200f in a Vector3 that will be missed, haven't noticed any issues yet with it.
        
        // [HarmonyDebug]
        // [HarmonyTranspiler]
        // [HarmonyPatch(typeof(SpraycoaterComponent), "Reshape")]
        public static IEnumerable<CodeInstruction> SpraycoaterComponentReshapeTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codeMatcher = new CodeMatcher(instructions, il).MatchForward(false,
                new CodeMatch(op => op.opcode == OpCodes.Ldc_R4 && op.OperandIs(200))); // Search for 200f

            if (codeMatcher.IsInvalid)
            {
                GS2.Error("Reshape Transpiler Failed");
                return instructions;
            }

            instructions =  codeMatcher.Repeat(z => z // Repeat for all occurences 
                .SetInstruction(Transpilers.EmitDelegate<Func<float>>(() => GameMain.localPlanet?.realRadius ?? 200f))).InstructionEnumeration();
            
            // codeMatcher = new CodeMatcher(instructions, il).MatchForward(false,
            //     new CodeMatch(op => op.opcode == OpCodes.Ldc_R4 && op.OperandIs(400))); // Search for 200f
            //
            // if (codeMatcher.IsInvalid)
            // {
            //     GS2.Error("Reshape Transpiler Failed");
            //     return instructions;
            // }
            //
            //
            // instructions = codeMatcher.Repeat(z => z // Repeat for all occurences 
            //     .SetInstruction(Transpilers.EmitDelegate<Func<float>>(() => GameMain.localPlanet?.realRadius * 2?? 400f))).InstructionEnumeration();
            
            
            return instructions;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SpraycoaterComponent), "Reshape")]
        public static bool Reshape(SpraycoaterComponent __instance, PlanetFactory factory, AnimData[] _animPool)

{
	Vector3 vector = factory.entityPool[__instance.entityId].pos;
	Quaternion rot = factory.entityPool[__instance.entityId].rot;
	Vector3 vector2 = rot.Forward();
	Vector3 normalized = vector.normalized;
	if (normalized.x * normalized.x + normalized.z * normalized.z <= 1E-06f)
	{
		return false;
	}

	var r = factory.planet.realRadius;
	vector = normalized * r;
	Vector3 rhs = (vector.y > 0f) ? (new Vector3(0f, r, 0f) - vector).normalized : (new Vector3(0f, -r, 0f) - vector).normalized;
	float num = Mathf.Abs(Vector3.Dot(Vector3.Cross(normalized, rhs), vector2));
	Vector3 reshapeData = SpraycoaterComponent.GetReshapeData(vector, rot);
	float num2 = reshapeData.x;
	float num3 = reshapeData.y;
	float num4 = reshapeData.z;
	ModelProto modelProto = LDB.models.Select((int)factory.entityPool[__instance.entityId].modelIndex);
	Pose[] addonAreaColPoses = modelProto.prefabDesc.addonAreaColPoses;
	Vector3[] addonAreaSize = modelProto.prefabDesc.addonAreaSize;
	if (__instance.incBeltId != 0)
	{
		Vector3 b = vector + rot * addonAreaColPoses[1].position;
		Vector3 lineStart = vector + rot * (addonAreaColPoses[1].position + addonAreaColPoses[1].forward * addonAreaSize[1].z * 2.5f);
		Vector3 lineEnd = vector + rot * (addonAreaColPoses[1].position - addonAreaColPoses[1].forward * addonAreaSize[1].z * 2.5f);
		Vector3 vector3 = factory.entityPool[factory.cargoTraffic.beltPool[__instance.incBeltId].entityId].pos;
		b = b.normalized * r;
		lineStart = lineStart.normalized * r;
		lineEnd = lineEnd.normalized * r;
		vector3 = vector3.normalized * r;
		float num5 = Maths.DistancePointLine(vector3, lineStart, lineEnd);
		num4 = -Mathf.Sign(Vector3.Dot((vector3 - b).normalized, vector2)) * num5 * num;
	}
	if (__instance.cargoBeltId != 0)
	{
		int objId = factory.cargoTraffic.beltPool[__instance.cargoBeltId].entityId;
		Vector3 vector4 = vector + rot * addonAreaColPoses[0].position;
		Vector3 vector5 = vector + rot * (addonAreaColPoses[0].position + addonAreaColPoses[0].forward * 2.4f);
		Vector3 vector6 = vector + rot * (addonAreaColPoses[0].position - addonAreaColPoses[0].forward * (2.4f + num4));
		vector4 = vector4.normalized * r;
		vector5 = vector5.normalized * r;
		vector6 = vector6.normalized * r;
		bool flag;
		int num6;
		int num7;
		factory.ReadObjectConn(objId, 0, out flag, out num6, out num7);
		int num8 = num6;
		factory.ReadObjectConn(objId, 1, out flag, out num6, out num7);
		int num9 = num6;
		Vector3 lineStart2 = vector5;
		Vector3 lineEnd2 = vector5;
		Vector3 lineStart3 = vector6;
		Vector3 lineEnd3 = vector6;
		if (num8 != 0)
		{
			Vector3 a = ((num8 > 0) ? factory.entityPool[num8].pos : factory.prebuildPool[-num8].pos).normalized * r;
			bool flag2 = Mathf.Sign(Vector3.Dot((a - vector4).normalized, vector2)) > 0f;
			Vector3 normalized2 = (a - vector4).normalized;
			Vector3.Angle(vector2, normalized2);
			if (flag2)
			{
				lineStart2 = vector4 + normalized2 * 5f;
				lineEnd2 = vector4 - normalized2 * 5f;
			}
			else
			{
				lineStart3 = vector4 + normalized2 * 5f;
				lineEnd3 = vector4 - normalized2 * 5f;
			}
			bool flag3;
			int num10;
			int num11;
			factory.ReadObjectConn(num8, 0, out flag3, out num10, out num11);
			if (num10 != 0)
			{
				Vector3 vector7 = ((num10 > 0) ? factory.entityPool[num10].pos : factory.prebuildPool[-num10].pos).normalized * r;
				normalized2 = (vector7 - vector4).normalized;
				Vector3.Angle(vector2, normalized2);
				if (flag2)
				{
					lineStart2 = vector4 + normalized2 * 5f;
					lineEnd2 = vector4 - normalized2 * 5f;
				}
				else
				{
					lineStart3 = vector4 + normalized2 * 5f;
					lineEnd3 = vector4 - normalized2 * 5f;
				}
				a = vector7;
			}
			Vector3 rhs2 = Vector3.Cross(vector2, normalized);
			if (flag2)
			{
				float num12 = -Mathf.Sign(Vector3.Dot((a - vector4).normalized, rhs2));
				num2 = Maths.DistancePointLine(vector5, lineStart2, lineEnd2) * num12;
			}
			else
			{
				float num13 = -Mathf.Sign(Vector3.Dot((a - vector4).normalized, rhs2));
				num3 = Maths.DistancePointLine(vector6, lineStart3, lineEnd3) * num13;
			}
		}
		if (num9 != 0)
		{
			Vector3 a2 = ((num9 > 0) ? factory.entityPool[num9].pos : factory.prebuildPool[-num9].pos).normalized * r;
			bool flag4 = Mathf.Sign(Vector3.Dot((a2 - vector4).normalized, vector2)) > 0f;
			Vector3 normalized3 = (a2 - vector4).normalized;
			Vector3.Angle(vector2, normalized3);
			if (flag4)
			{
				lineStart2 = vector4 + normalized3 * 5f;
				lineEnd2 = vector4 - normalized3 * 5f;
			}
			else
			{
				lineStart3 = vector4 + normalized3 * 5f;
				lineEnd3 = vector4 - normalized3 * 5f;
			}
			bool flag5;
			int num14;
			int num15;
			factory.ReadObjectConn(num9, 1, out flag5, out num14, out num15);
			if (num14 != 0)
			{
				Vector3 vector8 = ((num14 > 0) ? factory.entityPool[num14].pos : factory.prebuildPool[-num14].pos).normalized * r;
				normalized3 = (vector8 - vector4).normalized;
				Vector3.Angle(vector2, normalized3);
				if (flag4)
				{
					lineStart2 = vector4 + normalized3 * 5f;
					lineEnd2 = vector4 - normalized3 * 5f;
				}
				else
				{
					lineStart3 = vector4 + normalized3 * 5f;
					lineEnd3 = vector4 - normalized3 * 5f;
				}
				a2 = vector8;
			}
			Vector3 rhs3 = Vector3.Cross(vector2, normalized);
			if (flag4)
			{
				float num12 = -Mathf.Sign(Vector3.Dot((a2 - vector4).normalized, rhs3));
				num2 = Maths.DistancePointLine(vector5, lineStart2, lineEnd2) * num12;
			}
			else
			{
				float num13 = -Mathf.Sign(Vector3.Dot((a2 - vector4).normalized, rhs3));
				num3 = Maths.DistancePointLine(vector6, lineStart3, lineEnd3) * num13;
			}
		}
	}
	_animPool[__instance.entityId].working_length = -1.265f - num4;
	_animPool[__instance.entityId].prepare_length = Mathf.Round(num3 * r) + Mathf.Round(num2 * r) * 400f;
	return false;
}
    }
}