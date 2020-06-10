﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    private Animator _animator;

    private Transform _footR;
    private Transform _footL;

    private float _weightFootR = 1;
    private float _weightFootL = 1;

    private Vector3 _rFpos;
    private Vector3 _lFpos;
    private Quaternion _rFrot;
    private Quaternion _lFrot;

    [SerializeField] private float _rayLength = 1;
    [SerializeField] private LayerMask _rayLayer;
    private float _smoothness = 0.5f;
    private float _offsetY = 0.1f;

    private void OnValidate()
    {
        _animator = GetComponent<Animator>();
        _footR = _animator.GetBoneTransform(HumanBodyBones.RightFoot);
        _footL = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
    }

    private void Update()
    {
        var rPos = _footR.TransformPoint(Vector3.zero);
        if (Physics.Raycast(rPos, Vector3.down,
            out var rightHit, _rayLength, _rayLayer))
        {
            _rFpos = Vector3.Lerp(_footR.position, rightHit.point, _smoothness);
            _rFrot = Quaternion.FromToRotation(transform.up, rightHit.normal) *
                     transform.rotation;
        }
        var lPos = _footL.TransformPoint(Vector3.zero);
        if (Physics.Raycast(lPos, Vector3.down, out var leftHit, _rayLength, _rayLayer))
        {
            _lFpos = Vector3.Lerp(_footL.position, leftHit.point, _smoothness);
            _lFrot = Quaternion.FromToRotation(transform.up, leftHit.normal) * transform.rotation;
        }
    }

    private void OnAnimatorIK()
    {
        _weightFootR = _animator.GetFloat("RightLeg");
        _weightFootL = _animator.GetFloat("LeftLeg");

        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _weightFootR);
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _weightFootL);

        _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _weightFootR);
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _weightFootL);

        _animator.SetIKPosition(AvatarIKGoal.RightFoot,
            _rFpos + new Vector3(0, _offsetY, 0));
        _animator.SetIKPosition(AvatarIKGoal.LeftFoot,
            _lFpos + new Vector3(0, _offsetY, 0));

        _animator.SetIKRotation(AvatarIKGoal.RightFoot, _rFrot);
        _animator.SetIKRotation(AvatarIKGoal.LeftFoot, _lFrot);
    }
}