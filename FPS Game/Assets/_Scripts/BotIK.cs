using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotIK : MonoBehaviour
{
    public Transform LeftHandIKTarget;
    public Transform RightHandIKTarget;
    public Transform LeftElbowIKTarget;
    public Transform RightElbowIKTarget;
    public Transform GunHolder;

    [Range(0, 1f)]
    public float HandIkAmount = 1f;
    [Range(0, 1f)]
    public float ElbowIkAmount = 1f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (LeftHandIKTarget != null)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, HandIkAmount);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, HandIkAmount);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);
        }
        if (RightHandIKTarget != null)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, HandIkAmount);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, HandIkAmount);
            animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
        }
        if (LeftElbowIKTarget != null)
        {
            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowIKTarget.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ElbowIkAmount);
        }
        if (RightElbowIKTarget != null)
        {
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowIKTarget.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ElbowIkAmount);
        }
    }

    public void AttachArms(Transform gun, bool isTwoHanded)
    {
        if (GunHolder != null && gun != null)
        {
            LeftHandIKTarget = gun.Find("LeftHand");
            LeftElbowIKTarget = gun.Find("LeftElbow");
            RightHandIKTarget = gun.Find("RightHand");
            RightElbowIKTarget = gun.Find("RightElbow");
        }
    }
}
