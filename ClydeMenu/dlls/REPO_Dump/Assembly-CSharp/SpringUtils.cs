using System;
using UnityEngine;

// Token: 0x02000236 RID: 566
public static class SpringUtils
{
	// Token: 0x06001295 RID: 4757 RVA: 0x000A712C File Offset: 0x000A532C
	public static void CalcDampedSpringMotionParams(ref SpringUtils.tDampedSpringMotionParams pOutParams, float deltaTime, float angularFrequency, float dampingRatio)
	{
		if (dampingRatio < 0f)
		{
			dampingRatio = 0f;
		}
		if (angularFrequency < 0f)
		{
			angularFrequency = 0f;
		}
		if (angularFrequency < 0.0001f)
		{
			pOutParams.m_posPosCoef = 1f;
			pOutParams.m_posVelCoef = 0f;
			pOutParams.m_velPosCoef = 0f;
			pOutParams.m_velVelCoef = 1f;
			return;
		}
		if (dampingRatio > 1.0001f)
		{
			float num = -angularFrequency * dampingRatio;
			float num2 = angularFrequency * Mathf.Sqrt(dampingRatio * dampingRatio - 1f);
			float num3 = num - num2;
			float num4 = num + num2;
			float num5 = Mathf.Exp(num3 * deltaTime);
			float num6 = Mathf.Exp(num4 * deltaTime);
			float num7 = 1f / (2f * num2);
			float num8 = num5 * num7;
			float num9 = num6 * num7;
			float num10 = num3 * num8;
			float num11 = num4 * num9;
			pOutParams.m_posPosCoef = num8 * num4 - num11 + num6;
			pOutParams.m_posVelCoef = -num8 + num9;
			pOutParams.m_velPosCoef = (num10 - num11 + num6) * num4;
			pOutParams.m_velVelCoef = -num10 + num11;
			return;
		}
		if (dampingRatio < 0.9999f)
		{
			float num12 = angularFrequency * dampingRatio;
			float num13 = angularFrequency * Mathf.Sqrt(1f - dampingRatio * dampingRatio);
			float num14 = Mathf.Exp(-num12 * deltaTime);
			float num15 = Mathf.Cos(num13 * deltaTime);
			float num16 = Mathf.Sin(num13 * deltaTime);
			float num17 = 1f / num13;
			float num18 = num14 * num16;
			float num19 = num14 * num15;
			float num20 = num14 * num12 * num16 * num17;
			pOutParams.m_posPosCoef = num19 + num20;
			pOutParams.m_posVelCoef = num18 * num17;
			pOutParams.m_velPosCoef = -num18 * num13 - num12 * num20;
			pOutParams.m_velVelCoef = num19 - num20;
			return;
		}
		float num21 = Mathf.Exp(-angularFrequency * deltaTime);
		float num22 = deltaTime * num21;
		float num23 = num22 * angularFrequency;
		pOutParams.m_posPosCoef = num23 + num21;
		pOutParams.m_posVelCoef = num22;
		pOutParams.m_velPosCoef = -angularFrequency * num23;
		pOutParams.m_velVelCoef = -num23 + num21;
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x000A730C File Offset: 0x000A550C
	public static void UpdateDampedSpringMotion(ref float pPos, ref float pVel, float equilibriumPos, in SpringUtils.tDampedSpringMotionParams springParams)
	{
		float num = pPos - equilibriumPos;
		float num2 = pVel;
		pPos = num * springParams.m_posPosCoef + num2 * springParams.m_posVelCoef + equilibriumPos;
		pVel = num * springParams.m_velPosCoef + num2 * springParams.m_velVelCoef;
	}

	// Token: 0x020003F2 RID: 1010
	public class tDampedSpringMotionParams
	{
		// Token: 0x04002CFA RID: 11514
		public float m_posPosCoef;

		// Token: 0x04002CFB RID: 11515
		public float m_posVelCoef;

		// Token: 0x04002CFC RID: 11516
		public float m_velPosCoef;

		// Token: 0x04002CFD RID: 11517
		public float m_velVelCoef;
	}
}
