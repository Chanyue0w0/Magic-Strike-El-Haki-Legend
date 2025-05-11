//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class GifPlayerController : MonoBehaviour
{
	private Animator animator;
	[SerializeField] private AnimatorOverrideController overrideController;

	[SerializeField] private AnimationClip[] gifClips;


	private void Start()
	{
		animator = GetComponent<Animator>();
		animator.runtimeAnimatorController = overrideController;
		animator.updateMode = AnimatorUpdateMode.UnscaledTime;
		PlayGIF(gifClips[0]); // 預設撥第一個
	}

	public void PlayGIF(AnimationClip clip)
	{
		if (clip == null) return;
		overrideController["DefaultAnimation"] = clip; // 替換 Animator 中名為 "DefaultAnimation" 的 State
		animator.Play("DefaultAnimation", 0, 0f); // 撥放它
	}

	public void PlayGIF(string animationClipName)
	{
		AnimationClip clip = System.Array.Find(gifClips, c => c.name == animationClipName);
		if (clip != null)
		{
			PlayGIF(clip);
		}
		else
		{
			Debug.LogWarning($"AnimationClip '{animationClipName}' not found.");
		}
	}

	public void PlayGIF(int listIndex)
	{
		if (listIndex < gifClips.Length && gifClips[listIndex] != null)
		{
			PlayGIF(gifClips[listIndex]);
		}
		else
		{
			Debug.LogWarning($"AnimationClip '{listIndex}' not found.");
		}
	}

}
