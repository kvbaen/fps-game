using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BotIK : MonoBehaviour
{
    public Transform leftHandIKTarget;
    public Transform rightHandIKTarget;
    public Transform leftElbowIKTarget;
    public Transform rightElbowIKTarget;
    public Transform gunHolder;
    [SerializeField]
    private Transform leftHand;
    [SerializeField]
    private TwoBoneIKConstraint leftHandReloadIK, rightHandReloadIK;
    [SerializeField]
    private MultiParentConstraint magMultiplier;
    [SerializeField]
    private MultiAimConstraint headAim, spineAim, upperSpineAim;
    [SerializeField]
    private Transform magPositionRight, magPositionLeft, leftHandReload, rightHandReload, mag;

    [Range(0, 1f)]
    public float handIkAmount = 1f;
    [Range(0, 1f)]
    public float elbowIkAmount = 1f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (leftHandIKTarget != null)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handIkAmount);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handIkAmount);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKTarget.rotation);
        }
        if (rightHandIKTarget != null)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIkAmount);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIkAmount);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIKTarget.rotation);
        }
        if (leftElbowIKTarget != null)
        {
            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowIKTarget.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, elbowIkAmount);
        }
        if (rightElbowIKTarget != null)
        {
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowIKTarget.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, elbowIkAmount);
        }
    }

    public void AttachArms(Transform gun, bool isTwoHanded)
    {
        if (gunHolder != null && gun != null)
        {
            leftHandIKTarget = gun.Find("LeftHand");
            leftElbowIKTarget = gun.Find("LeftElbow");
            rightHandIKTarget = gun.Find("RightHand");
            rightElbowIKTarget = gun.Find("RightElbow");
            magPositionLeft = leftHand.Find(gun.name + "_MagPositionLeft");
            magPositionRight = gun.Find("MagPositionRight");
            mag = gun.Find("mag");
            leftHandReload = gun.Find("LeftHandReload");
            rightHandReload = gun.Find("RightHandReload");
            var data = magMultiplier.data.sourceObjects;
            data.Clear();

            if (magPositionRight != null)
            {
                data.Add(new WeightedTransform(magPositionRight, 0));
            }
            if (magPositionLeft != null)
            {
                data.Add(new WeightedTransform(magPositionLeft, 0));
            }
            if (leftHandReload != null)
            {
                leftHandReloadIK.data.target = leftHandReload;
            }
            if (rightHandReload != null)
            {
                rightHandReloadIK.data.target = rightHandReload;
            }
            if (mag != null)
            {
                magMultiplier.data.constrainedObject = mag;
            }
            magMultiplier.data.sourceObjects = data;
            RigBuilder rigs = GetComponent<RigBuilder>();
            rigs.Build();
        }
    }

    public void DisableIK()
    {
        handIkAmount = 0f;
        elbowIkAmount = 0f;
    }

    public void EnableIK()
    {
        handIkAmount = 1f;
        elbowIkAmount = 1f;
    }
    public void ChangeIKAmountHand(float value)
    {
        handIkAmount = value;
    }
    public void ChangeIKAmountElbow(float value)
    {
        handIkAmount = value;
    }

    public void ActivateAim(bool active)
    {
        if (active)
        {
            headAim.weight = 0.8f;
            spineAim.weight = 0.3f;
            upperSpineAim.weight = 0.2f;
        }
        else
        {
            headAim.weight = 0f;
            spineAim.weight = 0f;
            upperSpineAim.weight = 0f;
        }
    }
}
