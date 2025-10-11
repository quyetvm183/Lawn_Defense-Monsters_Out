using UnityEngine;
namespace RGame
{
	public class AnimationHelper
	{
		//returns the length (time) of an animation
		public static float getAnimationLength(Animator animator, string animName)
		{
			if (animator.isInitialized)
			{
				RuntimeAnimatorController ac = animator.runtimeAnimatorController;
				//Get all the clips and find the name of the animation need to get the length
				for (int i = 0; i < ac.animationClips.Length; i++)
				{
					if (ac.animationClips[i].name == animName)
					{
						return ac.animationClips[i].length;
					}
				}
			}
			return 0;
		}

		//returns the length (time) of an animation
		public static float getCurrentStateTime(Animator animator, int layer)
		{
			//Get the clip and find the name of the animation need to get the length
			AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(layer);
			AnimatorClipInfo[] myAnimatorClip = animator.GetCurrentAnimatorClipInfo(layer);
			float myTime = myAnimatorClip[layer].clip.length * animationState.normalizedTime;
			return myTime;
		}
	}
}