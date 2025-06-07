using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020001CB RID: 459
public class PlayerExpression : MonoBehaviour
{
	// Token: 0x06000FC7 RID: 4039 RVA: 0x0008F37C File Offset: 0x0008D57C
	private void Start()
	{
		this.playerVisuals = base.GetComponent<PlayerAvatarVisuals>();
		if (!this.playerAvatar)
		{
			this.playerAvatar = PlayerAvatar.instance;
		}
		if (this.playerAvatar && this.playerAvatar.isLocal)
		{
			this.isLocal = true;
		}
		this.leftUpperLidClosedAngle = new SpringFloat();
		this.leftUpperLidClosedAngle.damping = 0.5f;
		this.leftUpperLidClosedAngle.speed = 20f;
		this.leftLowerLidClosedAngle = new SpringFloat();
		this.leftLowerLidClosedAngle.damping = 0.5f;
		this.leftLowerLidClosedAngle.speed = 20f;
		this.rightUpperLidClosedAngle = new SpringFloat();
		this.rightUpperLidClosedAngle.damping = 0.5f;
		this.rightUpperLidClosedAngle.speed = 20f;
		this.rightLowerLidClosedAngle = new SpringFloat();
		this.rightLowerLidClosedAngle.damping = 0.5f;
		this.rightLowerLidClosedAngle.speed = 20f;
		this.leftUpperLidRotationZSpring = new SpringFloat();
		this.leftUpperLidRotationZSpring.damping = 0.5f;
		this.leftUpperLidRotationZSpring.speed = 20f;
		this.rightUpperLidRotationZSpring = new SpringFloat();
		this.rightUpperLidRotationZSpring.damping = 0.5f;
		this.rightUpperLidRotationZSpring.speed = 20f;
		this.leftLowerLidRotationZSpring = new SpringFloat();
		this.leftLowerLidRotationZSpring.damping = 0.5f;
		this.leftLowerLidRotationZSpring.speed = 20f;
		this.rightLowerLidRotationZSpring = new SpringFloat();
		this.rightLowerLidRotationZSpring.damping = 0.5f;
		this.rightLowerLidRotationZSpring.speed = 20f;
		this.leftLidsScale = new SpringFloat();
		this.leftLidsScale.damping = 0.5f;
		this.leftLidsScale.speed = 20f;
		this.rightLidsScale = new SpringFloat();
		this.rightLidsScale.damping = 0.5f;
		this.rightLidsScale.speed = 20f;
		this.leftPupilSizeSpring = new SpringFloat();
		this.leftPupilSizeSpring.damping = 0.5f;
		this.leftPupilSizeSpring.speed = 20f;
		this.rightPupilSizeSpring = new SpringFloat();
		this.rightPupilSizeSpring.damping = 0.5f;
		this.rightPupilSizeSpring.speed = 20f;
		this.pupilRightScale.localScale = Vector3.one;
		this.pupilLeftScale.localScale = Vector3.one;
		this.inputToggleList.Add(new PlayerExpression.ToggleExpressionInput
		{
			index = 1,
			active = false,
			inputKey = InputKey.Expression1
		});
		this.inputToggleList.Add(new PlayerExpression.ToggleExpressionInput
		{
			index = 2,
			active = false,
			inputKey = InputKey.Expression2
		});
		this.inputToggleList.Add(new PlayerExpression.ToggleExpressionInput
		{
			index = 3,
			active = false,
			inputKey = InputKey.Expression3
		});
		this.inputToggleList.Add(new PlayerExpression.ToggleExpressionInput
		{
			index = 4,
			active = false,
			inputKey = InputKey.Expression4
		});
		this.inputToggleList.Add(new PlayerExpression.ToggleExpressionInput
		{
			index = 5,
			active = false,
			inputKey = InputKey.Expression5
		});
		this.inputToggleList.Add(new PlayerExpression.ToggleExpressionInput
		{
			index = 6,
			active = false,
			inputKey = InputKey.Expression6
		});
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x0008F6D8 File Offset: 0x0008D8D8
	private void DoExpression(int _index, float _percent, bool _playerInput = false)
	{
		if (_playerInput)
		{
			PlayerExpressionsUI.instance.ShrinkReset();
			TutorialDirector.instance.playerUsedExpression = true;
		}
		this.activeExpressions.Add(_index);
		this.expressions[_index].weight = Mathf.Lerp(this.expressions[_index].weight, _percent, this.playerVisuals.deltaTime * 5f);
		this.expressions[_index].timer = 0.2f;
		if (!this.expressions[_index].isExpressing && !this.onlyVisualRepresentation)
		{
			this.playerAvatar.PlayerExpressionSet(_index, _percent);
		}
		this.expressions[_index].isExpressing = true;
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x0008F794 File Offset: 0x0008D994
	public void OverrideExpressionSet(int _index, float _percent)
	{
		foreach (PlayerExpression.OverrideExpression overrideExpression in this.overrideExpressions)
		{
			if (_index == overrideExpression.index)
			{
				overrideExpression.percent = _percent;
				overrideExpression.timer = 0.2f;
				return;
			}
		}
		PlayerExpression.OverrideExpression overrideExpression2 = new PlayerExpression.OverrideExpression();
		overrideExpression2.index = _index;
		overrideExpression2.percent = _percent;
		overrideExpression2.timer = 0.2f;
		this.overrideExpressions.Add(overrideExpression2);
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x0008F828 File Offset: 0x0008DA28
	private void ResetExpressions(int _index)
	{
		if (this.expressions[_index].isExpressing && !this.onlyVisualRepresentation)
		{
			this.playerAvatar.PlayerExpressionSet(_index, 0f);
		}
		this.expressions[_index].isExpressing = false;
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x0008F868 File Offset: 0x0008DA68
	private void ExpressionTimerTick()
	{
		this.isExpressing = false;
		foreach (ExpressionSettings expressionSettings in this.expressions)
		{
			if (expressionSettings.weight > 0f)
			{
				expressionSettings.weight = Mathf.Lerp(expressionSettings.weight, 0f, this.playerVisuals.deltaTime * 5f);
			}
			if (expressionSettings.timer > 0f)
			{
				expressionSettings.timer -= this.playerVisuals.deltaTime;
				if (expressionSettings.timer <= 0f)
				{
					this.StopExpression(this.expressions.IndexOf(expressionSettings));
				}
				this.isExpressing = true;
			}
		}
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x0008F940 File Offset: 0x0008DB40
	private void StopExpression(int _index)
	{
		this.playerAvatar.PlayerExpressionStop(_index);
		this.expressions[_index].isExpressing = false;
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x0008F960 File Offset: 0x0008DB60
	private void ToggleExpression(int _index)
	{
		TutorialDirector.instance.playerUsedExpression = true;
		foreach (PlayerExpression.ToggleExpressionInput toggleExpressionInput in this.inputToggleList)
		{
			if (toggleExpressionInput.index == _index)
			{
				this.inputToggleListNew.Add(toggleExpressionInput);
				this.inputToggleListNewTimer = 0.1f;
			}
		}
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x0008F9D8 File Offset: 0x0008DBD8
	private void Update()
	{
		if (!LevelGenerator.Instance.Generated && !SemiFunc.MenuLevel())
		{
			this.DoExpression(1, 100f, false);
		}
		if (!this.playerAvatar)
		{
			if (this.playerVisuals.expressionAvatar)
			{
				this.playerVisuals.animator.Play("Crouch", 0, 0f);
			}
			this.playerAvatar = PlayerAvatar.instance;
			this.isLocal = true;
		}
		List<int> list = new List<int>();
		foreach (int num in this.activeExpressions)
		{
			list.Add(num);
		}
		this.activeExpressions.Clear();
		if (this.isLocal)
		{
			if (MenuManager.instance.currentMenuPage)
			{
				goto IL_6C3;
			}
			foreach (PlayerExpression.OverrideExpression overrideExpression in this.overrideExpressions)
			{
				this.DoExpression(overrideExpression.index, overrideExpression.percent, false);
			}
			if (!InputManager.instance.InputToggleGet(InputKey.Expression1))
			{
				if (SemiFunc.InputHold(InputKey.Expression1))
				{
					this.DoExpression(1, 100f, true);
				}
				if (SemiFunc.InputHold(InputKey.Expression2))
				{
					this.DoExpression(2, 100f, true);
				}
				if (SemiFunc.InputHold(InputKey.Expression3))
				{
					this.DoExpression(3, 100f, true);
				}
				if (SemiFunc.InputHold(InputKey.Expression4))
				{
					this.DoExpression(4, 100f, true);
				}
				if (SemiFunc.InputHold(InputKey.Expression5))
				{
					this.DoExpression(5, 100f, true);
				}
				if (SemiFunc.InputHold(InputKey.Expression6))
				{
					this.DoExpression(6, 100f, true);
					goto IL_6C3;
				}
				goto IL_6C3;
			}
			else
			{
				if (this == this.playerAvatar.playerExpression)
				{
					this.inputToggleListNewTimer -= Time.deltaTime;
					if (SemiFunc.InputDown(InputKey.Expression1))
					{
						this.ToggleExpression(1);
					}
					if (SemiFunc.InputDown(InputKey.Expression2))
					{
						this.ToggleExpression(2);
					}
					if (SemiFunc.InputDown(InputKey.Expression3))
					{
						this.ToggleExpression(3);
					}
					if (SemiFunc.InputDown(InputKey.Expression4))
					{
						this.ToggleExpression(4);
					}
					if (SemiFunc.InputDown(InputKey.Expression5))
					{
						this.ToggleExpression(5);
					}
					if (SemiFunc.InputDown(InputKey.Expression6))
					{
						this.ToggleExpression(6);
					}
					if (this.inputToggleListNewTimer <= 0f && this.inputToggleListNew.Count > 0)
					{
						PlayerExpressionsUI.instance.ShrinkReset();
						int num2 = 0;
						using (List<PlayerExpression.ToggleExpressionInput>.Enumerator enumerator3 = this.inputToggleList.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								if (enumerator3.Current.active)
								{
									num2++;
								}
							}
						}
						bool flag = true;
						foreach (PlayerExpression.ToggleExpressionInput toggleExpressionInput in this.inputToggleList)
						{
							using (List<PlayerExpression.ToggleExpressionInput>.Enumerator enumerator4 = this.inputToggleListNew.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									if (enumerator4.Current.index == toggleExpressionInput.index && !toggleExpressionInput.active)
									{
										flag = false;
										break;
									}
								}
							}
							if (!flag)
							{
								break;
							}
						}
						if (flag)
						{
							foreach (PlayerExpression.ToggleExpressionInput toggleExpressionInput2 in this.inputToggleList)
							{
								if (toggleExpressionInput2.active)
								{
									bool flag2 = true;
									using (List<PlayerExpression.ToggleExpressionInput>.Enumerator enumerator4 = this.inputToggleListNew.GetEnumerator())
									{
										while (enumerator4.MoveNext())
										{
											if (enumerator4.Current.index == toggleExpressionInput2.index)
											{
												flag2 = false;
												break;
											}
										}
									}
									if (flag2)
									{
										flag = false;
										break;
									}
								}
							}
						}
						bool flag3 = false;
						foreach (PlayerExpression.ToggleExpressionInput toggleExpressionInput3 in this.inputToggleList)
						{
							bool flag4 = true;
							using (List<PlayerExpression.ToggleExpressionInput>.Enumerator enumerator4 = this.inputToggleListNew.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									if (enumerator4.Current.index == toggleExpressionInput3.index)
									{
										flag4 = false;
										break;
									}
								}
							}
							if (flag4)
							{
								if (!SemiFunc.InputHold(toggleExpressionInput3.inputKey))
								{
									toggleExpressionInput3.active = false;
								}
								else
								{
									flag3 = true;
								}
							}
						}
						if (this.inputToggleListNew.Count == 1)
						{
							using (List<PlayerExpression.ToggleExpressionInput>.Enumerator enumerator3 = this.inputToggleList.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									PlayerExpression.ToggleExpressionInput toggleExpressionInput4 = enumerator3.Current;
									using (List<PlayerExpression.ToggleExpressionInput>.Enumerator enumerator4 = this.inputToggleListNew.GetEnumerator())
									{
										while (enumerator4.MoveNext())
										{
											if (enumerator4.Current.index == toggleExpressionInput4.index)
											{
												if (num2 > 1 && !flag3)
												{
													toggleExpressionInput4.active = true;
												}
												else
												{
													toggleExpressionInput4.active = !toggleExpressionInput4.active;
												}
											}
										}
									}
								}
								goto IL_578;
							}
						}
						foreach (PlayerExpression.ToggleExpressionInput toggleExpressionInput5 in this.inputToggleList)
						{
							using (List<PlayerExpression.ToggleExpressionInput>.Enumerator enumerator4 = this.inputToggleListNew.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									if (enumerator4.Current.index == toggleExpressionInput5.index)
									{
										if (flag)
										{
											toggleExpressionInput5.active = false;
										}
										else
										{
											toggleExpressionInput5.active = true;
										}
									}
								}
							}
						}
						IL_578:
						this.inputToggleListNew.Clear();
					}
				}
				using (List<PlayerExpression.ToggleExpressionInput>.Enumerator enumerator3 = this.playerAvatar.playerExpression.inputToggleList.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						PlayerExpression.ToggleExpressionInput toggleExpressionInput6 = enumerator3.Current;
						if (toggleExpressionInput6.active)
						{
							this.DoExpression(toggleExpressionInput6.index, 100f, false);
						}
					}
					goto IL_6C3;
				}
			}
		}
		foreach (KeyValuePair<int, float> keyValuePair in this.playerAvatar.playerExpressions)
		{
			this.expressions[keyValuePair.Key].weight = Mathf.Lerp(this.expressions[keyValuePair.Key].weight, keyValuePair.Value, this.playerVisuals.deltaTime * 5f);
			if (!this.expressions[keyValuePair.Key].stopExpressing)
			{
				this.expressions[keyValuePair.Key].isExpressing = true;
				this.expressions[keyValuePair.Key].timer = 0.2f;
				this.activeExpressions.Add(keyValuePair.Key);
			}
		}
		IL_6C3:
		foreach (PlayerExpression.OverrideExpression overrideExpression2 in Enumerable.ToList<PlayerExpression.OverrideExpression>(this.overrideExpressions))
		{
			if (overrideExpression2.timer <= 0f)
			{
				this.overrideExpressions.Remove(overrideExpression2);
			}
			else
			{
				overrideExpression2.timer -= this.playerVisuals.deltaTime;
			}
		}
		bool flag5 = false;
		foreach (int num3 in this.activeExpressions)
		{
			if (!list.Contains(num3))
			{
				flag5 = true;
				break;
			}
		}
		if (!flag5)
		{
			foreach (int num4 in list)
			{
				if (!this.activeExpressions.Contains(num4))
				{
					flag5 = true;
					break;
				}
			}
		}
		if (flag5)
		{
			this.playerVisuals.HeadTiltImpulse(50f);
		}
		if (this.isExpressing)
		{
			if (this.playerVisuals.expressionAvatar)
			{
				this.playerVisuals.animator.SetBool("Crouching", false);
			}
			this.expressions[0].weight = Mathf.Lerp(this.expressions[0].weight, 0f, this.playerVisuals.deltaTime * 5f);
			this.blendedLeftEye = this.BlendEyeSettings(true);
			this.blendedRightEye = this.BlendEyeSettings(false);
			this.eyelidLeft.SetActive(true);
			this.eyelidRight.SetActive(true);
			this.blendedHeadTilt = 0f;
			float num5 = 0f;
			foreach (ExpressionSettings expressionSettings in this.expressions)
			{
				if (expressionSettings.isExpressing)
				{
					this.blendedHeadTilt += expressionSettings.headTiltAmount;
					num5 += 1f;
				}
			}
			if (num5 > 0f)
			{
				this.blendedHeadTilt /= num5;
			}
			this.playerVisuals.HeadTiltOverride(this.blendedHeadTilt);
			float num6 = SemiFunc.SpringFloatGet(this.leftLidsScale, 1f, this.playerVisuals.deltaTime);
			this.eyelidLeftScale.localScale = new Vector3(num6, num6, num6);
			num6 = SemiFunc.SpringFloatGet(this.rightLidsScale, 1f, this.playerVisuals.deltaTime);
			this.eyelidRightScale.localScale = new Vector3(num6, num6, num6);
			float x = SemiFunc.SpringFloatGet(this.leftUpperLidClosedAngle, this.blendedLeftEye.upperLidClosedPercent, this.playerVisuals.deltaTime);
			this.leftUpperEyelidRotationX.localRotation = Quaternion.Euler(x, 0f, 0f);
			float x2 = SemiFunc.SpringFloatGet(this.leftLowerLidClosedAngle, this.blendedLeftEye.lowerLidClosedPercent, this.playerVisuals.deltaTime);
			this.leftLowerEyelidRotationX.localRotation = Quaternion.Euler(x2, 0f, 0f);
			x = SemiFunc.SpringFloatGet(this.rightUpperLidClosedAngle, this.blendedRightEye.upperLidClosedPercent, this.playerVisuals.deltaTime);
			this.rightUpperEyelidRotationX.localRotation = Quaternion.Euler(x, 0f, 0f);
			x2 = SemiFunc.SpringFloatGet(this.rightLowerLidClosedAngle, this.blendedRightEye.lowerLidClosedPercent, this.playerVisuals.deltaTime);
			this.rightLowerEyelidRotationX.localRotation = Quaternion.Euler(x2, 0f, 0f);
			float z = SemiFunc.SpringFloatGet(this.leftUpperLidRotationZSpring, this.blendedLeftEye.upperLidAngle, this.playerVisuals.deltaTime);
			this.leftUpperEyeLidRotationZ.localRotation = Quaternion.Euler(0f, 0f, z);
			z = SemiFunc.SpringFloatGet(this.rightUpperLidRotationZSpring, this.blendedRightEye.upperLidAngle, this.playerVisuals.deltaTime);
			this.rightUpperEyelidRotationZ.localRotation = Quaternion.Euler(0f, 0f, z);
			float z2 = SemiFunc.SpringFloatGet(this.leftLowerLidRotationZSpring, this.blendedLeftEye.lowerLidAngle, this.playerVisuals.deltaTime);
			this.leftLowerEyelidRotationZ.localRotation = Quaternion.Euler(0f, 0f, z2);
			z2 = SemiFunc.SpringFloatGet(this.rightLowerLidRotationZSpring, this.blendedRightEye.lowerLidAngle, this.playerVisuals.deltaTime);
			this.rightLowerEyelidRotationZ.localRotation = Quaternion.Euler(0f, 0f, z2);
			this.pupilLeftScaleAmount = SemiFunc.SpringFloatGet(this.leftPupilSizeSpring, this.blendedLeftEye.pupilSize, this.playerVisuals.deltaTime);
			this.pupilRightScaleAmount = SemiFunc.SpringFloatGet(this.rightPupilSizeSpring, this.blendedRightEye.pupilSize, this.playerVisuals.deltaTime);
			if (this.isLocal && !this.playerAvatar.isDisabled)
			{
				PlayerExpressionsUI.instance.Show();
			}
		}
		else if (this.eyelidLeft.activeSelf)
		{
			this.blendedLeftEye = this.BlendEyeSettings(true);
			this.blendedRightEye = this.BlendEyeSettings(false);
			this.expressions[0].weight = Mathf.Lerp(this.expressions[0].weight, 100f, this.playerVisuals.deltaTime * 20f);
			this.expressions[1].weight = Mathf.Lerp(this.expressions[1].weight, 0f, this.playerVisuals.deltaTime * 20f);
			this.expressions[2].weight = Mathf.Lerp(this.expressions[2].weight, 0f, this.playerVisuals.deltaTime * 20f);
			if (this.expressions[0].weight > 50f)
			{
				if (this.playerVisuals.expressionAvatar)
				{
					this.playerVisuals.animator.SetBool("Crouching", true);
				}
				float num7 = SemiFunc.SpringFloatGet(this.leftLidsScale, 0.8f, this.playerVisuals.deltaTime);
				this.eyelidLeftScale.localScale = new Vector3(num7, num7, num7);
				num7 = SemiFunc.SpringFloatGet(this.rightLidsScale, 0.8f, this.playerVisuals.deltaTime);
				this.eyelidRightScale.localScale = new Vector3(num7, num7, num7);
			}
			float x3 = SemiFunc.SpringFloatGet(this.leftUpperLidClosedAngle, this.blendedLeftEye.upperLidClosedPercent, this.playerVisuals.deltaTime);
			this.leftUpperEyelidRotationX.localRotation = Quaternion.Euler(x3, 0f, 0f);
			float x4 = SemiFunc.SpringFloatGet(this.leftLowerLidClosedAngle, this.blendedLeftEye.lowerLidClosedPercent, this.playerVisuals.deltaTime);
			this.leftLowerEyelidRotationX.localRotation = Quaternion.Euler(x4, 0f, 0f);
			x3 = SemiFunc.SpringFloatGet(this.rightUpperLidClosedAngle, this.blendedRightEye.upperLidClosedPercent, this.playerVisuals.deltaTime);
			this.rightUpperEyelidRotationX.localRotation = Quaternion.Euler(x3, 0f, 0f);
			x4 = SemiFunc.SpringFloatGet(this.rightLowerLidClosedAngle, this.blendedRightEye.lowerLidClosedPercent, this.playerVisuals.deltaTime);
			this.rightLowerEyelidRotationX.localRotation = Quaternion.Euler(x4, 0f, 0f);
			float z3 = SemiFunc.SpringFloatGet(this.leftUpperLidRotationZSpring, this.blendedLeftEye.upperLidAngle, this.playerVisuals.deltaTime);
			this.leftUpperEyeLidRotationZ.localRotation = Quaternion.Euler(0f, 0f, z3);
			z3 = SemiFunc.SpringFloatGet(this.rightUpperLidRotationZSpring, this.blendedRightEye.upperLidAngle, this.playerVisuals.deltaTime);
			this.rightUpperEyelidRotationZ.localRotation = Quaternion.Euler(0f, 0f, z3);
			float z4 = SemiFunc.SpringFloatGet(this.leftLowerLidRotationZSpring, this.blendedLeftEye.lowerLidAngle, this.playerVisuals.deltaTime);
			this.leftLowerEyelidRotationZ.localRotation = Quaternion.Euler(0f, 0f, z4);
			z4 = SemiFunc.SpringFloatGet(this.rightLowerLidRotationZSpring, this.blendedRightEye.lowerLidAngle, this.playerVisuals.deltaTime);
			this.rightLowerEyelidRotationZ.localRotation = Quaternion.Euler(0f, 0f, z4);
			this.pupilLeftScaleAmount = SemiFunc.SpringFloatGet(this.leftPupilSizeSpring, this.blendedLeftEye.pupilSize, this.playerVisuals.deltaTime);
			this.pupilRightScaleAmount = SemiFunc.SpringFloatGet(this.rightPupilSizeSpring, this.blendedRightEye.pupilSize, this.playerVisuals.deltaTime);
			if (this.expressions[0].weight > 82f)
			{
				this.pupilRightScaleAmount = 1f;
				this.pupilRightScale.localScale = Vector3.one;
				this.pupilLeftScaleAmount = 1f;
				this.pupilLeftScale.localScale = Vector3.one;
				this.eyelidLeft.SetActive(false);
				this.eyelidRight.SetActive(false);
			}
		}
		this.ExpressionTimerTick();
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x00090AD0 File Offset: 0x0008ECD0
	private EyeSettings BlendEyeSettings(bool isLeft)
	{
		EyeSettings eyeSettings = new EyeSettings();
		float num = 0f;
		foreach (ExpressionSettings expressionSettings in this.expressions)
		{
			float weight = expressionSettings.weight;
			num += weight;
			EyeSettings eyeSettings2 = isLeft ? expressionSettings.leftEye : expressionSettings.rightEye;
			eyeSettings.upperLidAngle += eyeSettings2.upperLidAngle * weight;
			eyeSettings.upperLidClosedPercent += eyeSettings2.upperLidClosedPercent * weight;
			eyeSettings.upperLidClosedPercentJitterAmount += eyeSettings2.upperLidClosedPercentJitterAmount * weight;
			eyeSettings.upperLidClosedPercentJitterSpeed += eyeSettings2.upperLidClosedPercentJitterSpeed * weight;
			eyeSettings.lowerLidAngle += eyeSettings2.lowerLidAngle * weight;
			eyeSettings.lowerLidClosedPercent += eyeSettings2.lowerLidClosedPercent * weight;
			eyeSettings.lowerLidClosedPercentJitterAmount += eyeSettings2.lowerLidClosedPercentJitterAmount * weight;
			eyeSettings.lowerLidClosedPercentJitterSpeed += eyeSettings2.lowerLidClosedPercentJitterSpeed * weight;
			eyeSettings.pupilSize += eyeSettings2.pupilSize * weight;
			eyeSettings.pupilSizeJitterAmount += eyeSettings2.pupilSizeJitterAmount * weight;
			eyeSettings.pupilSizeJitterSpeed += eyeSettings2.pupilSizeJitterSpeed * weight;
			eyeSettings.pupilPositionJitter += eyeSettings2.pupilPositionJitter * weight;
			eyeSettings.pupilPositionJitterAmount += eyeSettings2.pupilPositionJitterAmount * weight;
			eyeSettings.pupilOffsetRotationX += eyeSettings2.pupilOffsetRotationX * weight;
			eyeSettings.pupilOffsetRotationY += eyeSettings2.pupilOffsetRotationY * weight;
		}
		if (num > 0f)
		{
			eyeSettings.upperLidAngle /= num;
			eyeSettings.upperLidClosedPercent /= num;
			eyeSettings.upperLidClosedPercentJitterAmount /= num;
			eyeSettings.upperLidClosedPercentJitterSpeed /= num;
			eyeSettings.lowerLidAngle /= num;
			eyeSettings.lowerLidClosedPercent /= num;
			eyeSettings.lowerLidClosedPercentJitterAmount /= num;
			eyeSettings.lowerLidClosedPercentJitterSpeed /= num;
			eyeSettings.pupilSize /= num;
			eyeSettings.pupilSizeJitterAmount /= num;
			eyeSettings.pupilSizeJitterSpeed /= num;
			eyeSettings.pupilPositionJitter /= num;
			eyeSettings.pupilPositionJitterAmount /= num;
			eyeSettings.pupilOffsetRotationX /= num;
			eyeSettings.pupilOffsetRotationY /= num;
		}
		return eyeSettings;
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x00090D94 File Offset: 0x0008EF94
	public void FetchTransformValues()
	{
		if (this.expressions == null)
		{
			this.expressions = new List<ExpressionSettings>();
		}
		ExpressionSettings expressionSettings;
		if (this.expressions.Count == 0)
		{
			expressionSettings = new ExpressionSettings();
			expressionSettings.expressionName = "Default Expression";
			expressionSettings.weight = 100f;
			expressionSettings.leftEye = new EyeSettings();
			expressionSettings.rightEye = new EyeSettings();
			this.expressions.Add(expressionSettings);
		}
		else
		{
			int num = this.expressions.Count - 1;
			expressionSettings = this.expressions[num];
		}
		if (this.leftUpperEyeLidRotationZ != null)
		{
			expressionSettings.leftEye.upperLidAngle = this.leftUpperEyeLidRotationZ.localRotation.eulerAngles.z;
		}
		if (this.leftUpperEyelidRotationX != null)
		{
			float x = this.leftUpperEyelidRotationX.localRotation.eulerAngles.x;
			float x2 = this.leftLowerEyelidRotationX.localRotation.eulerAngles.x;
			expressionSettings.leftEye.upperLidClosedPercent = x;
			expressionSettings.leftEye.lowerLidClosedPercent = x2;
		}
		if (this.leftLowerEyelidRotationZ != null)
		{
			expressionSettings.leftEye.lowerLidAngle = this.leftLowerEyelidRotationZ.localRotation.eulerAngles.z;
		}
		if (this.pupilLeftScale != null)
		{
			expressionSettings.leftEye.pupilSize = this.pupilLeftScale.localScale.x;
		}
		if (this.pupilLeftRotationOffset != null)
		{
			expressionSettings.leftEye.pupilOffsetRotationX = this.pupilLeftRotationOffset.localRotation.eulerAngles.x;
			expressionSettings.leftEye.pupilOffsetRotationY = this.pupilLeftRotationOffset.localRotation.eulerAngles.y;
		}
		if (this.rightUpperEyelidRotationZ != null)
		{
			expressionSettings.rightEye.upperLidAngle = this.rightUpperEyelidRotationZ.localRotation.eulerAngles.z;
		}
		if (this.rightUpperEyelidRotationX != null)
		{
			float x3 = this.rightUpperEyelidRotationX.localRotation.eulerAngles.x;
			float x4 = this.rightLowerEyelidRotationX.localRotation.eulerAngles.x;
			expressionSettings.rightEye.upperLidClosedPercent = x3;
			expressionSettings.rightEye.lowerLidClosedPercent = x4;
		}
		if (this.rightLowerEyelidRotationZ != null)
		{
			expressionSettings.rightEye.lowerLidAngle = this.rightLowerEyelidRotationZ.localRotation.eulerAngles.z;
		}
		if (this.leftUpperEyelidRotationX != null)
		{
			expressionSettings.rightEye.pupilSize = this.pupilRightScale.localScale.x;
		}
		if (this.pupilRightRotationOffset != null)
		{
			expressionSettings.rightEye.pupilOffsetRotationX = this.pupilRightRotationOffset.localRotation.eulerAngles.x;
			expressionSettings.rightEye.pupilOffsetRotationY = this.pupilRightRotationOffset.localRotation.eulerAngles.y;
		}
		Debug.Log("Transform values have been fetched into the EyeSettings of expression: " + expressionSettings.expressionName);
	}

	// Token: 0x04001AAA RID: 6826
	public PlayerAvatar playerAvatar;

	// Token: 0x04001AAB RID: 6827
	public bool onlyVisualRepresentation;

	// Token: 0x04001AAC RID: 6828
	public GameObject eyelidLeft;

	// Token: 0x04001AAD RID: 6829
	public GameObject eyelidRight;

	// Token: 0x04001AAE RID: 6830
	public Transform eyelidLeftScale;

	// Token: 0x04001AAF RID: 6831
	public Transform eyelidRightScale;

	// Token: 0x04001AB0 RID: 6832
	public Transform leftUpperEyelidRotationX;

	// Token: 0x04001AB1 RID: 6833
	public Transform leftUpperEyeLidRotationZ;

	// Token: 0x04001AB2 RID: 6834
	public Transform leftLowerEyelidRotationX;

	// Token: 0x04001AB3 RID: 6835
	public Transform leftLowerEyelidRotationZ;

	// Token: 0x04001AB4 RID: 6836
	public Transform rightUpperEyelidRotationX;

	// Token: 0x04001AB5 RID: 6837
	public Transform rightUpperEyelidRotationZ;

	// Token: 0x04001AB6 RID: 6838
	public Transform rightLowerEyelidRotationX;

	// Token: 0x04001AB7 RID: 6839
	public Transform rightLowerEyelidRotationZ;

	// Token: 0x04001AB8 RID: 6840
	public Transform pupilLeftScale;

	// Token: 0x04001AB9 RID: 6841
	internal float pupilLeftScaleAmount = 1f;

	// Token: 0x04001ABA RID: 6842
	public Transform pupilLeftRotationOffset;

	// Token: 0x04001ABB RID: 6843
	public Transform pupilRightScale;

	// Token: 0x04001ABC RID: 6844
	internal float pupilRightScaleAmount = 1f;

	// Token: 0x04001ABD RID: 6845
	public Transform pupilRightRotationOffset;

	// Token: 0x04001ABE RID: 6846
	private bool isExpressing;

	// Token: 0x04001ABF RID: 6847
	[Header("Expression Settings (set percentages here)")]
	public List<ExpressionSettings> expressions;

	// Token: 0x04001AC0 RID: 6848
	private EyeSettings blendedLeftEye;

	// Token: 0x04001AC1 RID: 6849
	private EyeSettings blendedRightEye;

	// Token: 0x04001AC2 RID: 6850
	private float blendedHeadTilt;

	// Token: 0x04001AC3 RID: 6851
	private SpringFloat leftLidsScale;

	// Token: 0x04001AC4 RID: 6852
	private SpringFloat rightLidsScale;

	// Token: 0x04001AC5 RID: 6853
	private SpringFloat leftUpperLidClosedAngle;

	// Token: 0x04001AC6 RID: 6854
	private SpringFloat leftLowerLidClosedAngle;

	// Token: 0x04001AC7 RID: 6855
	private SpringFloat rightUpperLidClosedAngle;

	// Token: 0x04001AC8 RID: 6856
	private SpringFloat rightLowerLidClosedAngle;

	// Token: 0x04001AC9 RID: 6857
	private SpringFloat leftUpperLidRotationZSpring;

	// Token: 0x04001ACA RID: 6858
	private SpringFloat rightUpperLidRotationZSpring;

	// Token: 0x04001ACB RID: 6859
	private SpringFloat leftLowerLidRotationZSpring;

	// Token: 0x04001ACC RID: 6860
	private SpringFloat rightLowerLidRotationZSpring;

	// Token: 0x04001ACD RID: 6861
	private SpringFloat leftPupilSizeSpring;

	// Token: 0x04001ACE RID: 6862
	private SpringFloat rightPupilSizeSpring;

	// Token: 0x04001ACF RID: 6863
	private PlayerAvatarVisuals playerVisuals;

	// Token: 0x04001AD0 RID: 6864
	private List<int> activeExpressions = new List<int>();

	// Token: 0x04001AD1 RID: 6865
	private bool isLocal;

	// Token: 0x04001AD2 RID: 6866
	private List<PlayerExpression.OverrideExpression> overrideExpressions = new List<PlayerExpression.OverrideExpression>();

	// Token: 0x04001AD3 RID: 6867
	private List<PlayerExpression.ToggleExpressionInput> inputToggleList = new List<PlayerExpression.ToggleExpressionInput>();

	// Token: 0x04001AD4 RID: 6868
	private List<PlayerExpression.ToggleExpressionInput> inputToggleListNew = new List<PlayerExpression.ToggleExpressionInput>();

	// Token: 0x04001AD5 RID: 6869
	private float inputToggleListNewTimer;

	// Token: 0x020003BC RID: 956
	public class OverrideExpression
	{
		// Token: 0x04002C31 RID: 11313
		public int index;

		// Token: 0x04002C32 RID: 11314
		public float percent;

		// Token: 0x04002C33 RID: 11315
		public float timer;
	}

	// Token: 0x020003BD RID: 957
	public class ToggleExpressionInput
	{
		// Token: 0x04002C34 RID: 11316
		public int index;

		// Token: 0x04002C35 RID: 11317
		public bool active;

		// Token: 0x04002C36 RID: 11318
		public InputKey inputKey;
	}
}
