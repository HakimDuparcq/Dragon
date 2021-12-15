using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
    
public class IKFootBehavior : MonoBehaviour
{
    [SerializeField] private Transform footTransformRF;
    [SerializeField] private Transform footTransformRB;
    [SerializeField] private Transform footTransformLF;
    [SerializeField] private Transform footTransformLB;
    private Transform[] allFootTransforms = new Transform[4];

    [SerializeField] private Transform footTargetTransformRF;
    [SerializeField] private Transform footTargetTransformRB;
    [SerializeField] private Transform footTargetTransformLF;
    [SerializeField] private Transform footTargetTransformLB;
    private Transform[] allTargetTransforms = new Transform[4];

    [SerializeField] private GameObject footRigRF;
    [SerializeField] private GameObject footRigRB;
    [SerializeField] private GameObject footRigLF;
    [SerializeField] private GameObject footRigLB;
    private TwoBoneIKConstraint[] allFootIKConstraints= new TwoBoneIKConstraint[4];

    private LayerMask groundLayerMask;
    private float maxHitDistance = 5f;
    private float addesHeight = 3f;
    private bool[] allGroundSpherecastHits= new bool[5];
    private LayerMask hitLayer;
    private Vector3[] allHitNormals = new Vector3[4];
    private float angleAboutX;
    private float angleAboutZ;
    private float yOffset;
    [SerializeField] Animator animator;
    private float[] allFootWeights = new float[4];
    private Vector3 aveHitNormal;

    [SerializeField, Range(-0.5f, 2)] private float upperFootYlimit = 0.3f;
    [SerializeField, Range(-0.5f, 2)] private float lowerFootYlimit = -0.1f;
    private int[] checkLocalTargetY = new int[4];
    private CapsuleCollider capsuleCollider ;

    void Start()
    {
        allFootTransforms[0] = footTransformRF;
        allFootTransforms[1] = footTransformRB;
        allFootTransforms[2] = footTransformLF;
        allFootTransforms[3] = footTransformLB;

        allTargetTransforms[0] = footTargetTransformRF;
        allTargetTransforms[1] = footTargetTransformRB;
        allTargetTransforms[2] = footTargetTransformLF;
        allTargetTransforms[3] = footTargetTransformLB;

        allFootIKConstraints[0] = footRigRF.GetComponent<TwoBoneIKConstraint>();
        allFootIKConstraints[1] = footRigRB.GetComponent<TwoBoneIKConstraint>();
        allFootIKConstraints[2] = footRigLF.GetComponent<TwoBoneIKConstraint>();
        allFootIKConstraints[3] = footRigLB.GetComponent<TwoBoneIKConstraint>();

        groundLayerMask = LayerMask.NameToLayer("Ground");

        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    
    void FixedUpdate()
    {
        RotateCharacterFeet();
        RotateCharacterBody();
    }


    private void CheckGroundBelow(out Vector3 hitPoint, out bool gotGroundSpherecastHit, out Vector3 hitNormal, out LayerMask hitLayer, 
                                  out float currentHitDistance, Transform objectTransform, int checkForLayerMask, float maxHitDistance, float addedHeiht)
    {
        RaycastHit hit;
        Vector3 startSpherecast = objectTransform.position + new Vector3(0f, addedHeiht, 0f);
        if (checkForLayerMask==-1)
        {
            Debug.LogError("Layer does not exist");
            gotGroundSpherecastHit = false;
            currentHitDistance = 0f;
            hitLayer = LayerMask.NameToLayer("Player");
            hitNormal = Vector3.up;
            hitPoint = objectTransform.position;
        }
        else
        {
            int layerMask = (1 << checkForLayerMask);
            if (Physics.SphereCast(startSpherecast, 0.2f, Vector3.down, out hit, maxHitDistance, layerMask, QueryTriggerInteraction.UseGlobal))
            {
                hitLayer = hit.transform.gameObject.layer;
                currentHitDistance = hit.distance - addedHeiht;
                hitNormal = hit.normal;
                gotGroundSpherecastHit = true;
                hitPoint = hit.point;
            }
            else
            {
                Debug.LogError("Layer does not exist");
                gotGroundSpherecastHit = false;
                currentHitDistance = 0f;
                hitLayer = LayerMask.NameToLayer("Player");
                hitNormal = Vector3.up;
                hitPoint = objectTransform.position;
            }
        }
        
    }

    Vector3 ProjectOnContactPlane(Vector3 vector, Vector3 hitNormal)
    {
        return vector - hitNormal * Vector3.Dot(vector, hitNormal);
    }

    private void ProjectedAxisAngles(out float angleAboutX, out float angleAboutZ, Transform footTargetTransform, Vector3 hitNormal)
    {
        Vector3 xAxisProjected = ProjectOnContactPlane(footTargetTransform.forward, hitNormal).normalized;
        Vector3 zAxisProjected = ProjectOnContactPlane(footTargetTransform.right, hitNormal).normalized;

        angleAboutX = Vector3.SignedAngle(footTargetTransform.forward, xAxisProjected, footTargetTransform.right);
        angleAboutZ = Vector3.SignedAngle(footTargetTransform.right, zAxisProjected, footTargetTransform.forward);
    }

    private void RotateCharacterFeet()
    {
        allFootWeights[0] = animator.GetFloat("RF Foot Weight");
        allFootWeights[1] = animator.GetFloat("RB Foot Weight");
        allFootWeights[2] = animator.GetFloat("LF Foot Weight");
        allFootWeights[3] = animator.GetFloat("LB Foot Weight");


        for (int i = 0; i < 4; i++)
        {
            allFootIKConstraints[i].weight = allFootWeights[i];

            CheckGroundBelow(out Vector3 hitPoint, out allGroundSpherecastHits[i], out Vector3 hitNormal, out hitLayer, out _,
                              allFootTransforms[i], groundLayerMask, maxHitDistance, addesHeight);
            allHitNormals[i] = hitNormal;

            if (allGroundSpherecastHits[i]==true)
            {
                yOffset = 0.15f;
                if (allFootTransforms[i].position.y<allTargetTransforms[i].position.y-0.1f)
                {
                    yOffset += allTargetTransforms[i].position.y - allTargetTransforms[i].position.y;
                }


                ProjectedAxisAngles(out angleAboutX, out angleAboutZ, allFootTransforms[i], allHitNormals[i]);

                allTargetTransforms[i].position = new Vector3(allFootTransforms[i].position.x, hitPoint.y + yOffset, allFootTransforms[i].position.z);

                allTargetTransforms[i].rotation = allFootTransforms[i].rotation;

                allTargetTransforms[i].localEulerAngles = new Vector3(allTargetTransforms[i].localEulerAngles.x + angleAboutX,
                                                                      allTargetTransforms[i].localEulerAngles.y,
                                                                       allTargetTransforms[i].localEulerAngles.z + angleAboutZ);
                Debug.Log("ok");
            }
            else
            {
                allTargetTransforms[i].position = allFootTransforms[i].position;

                allTargetTransforms[i].rotation = allFootTransforms[i].rotation;
            }


        }
    }


    private void RotateCharacterBody()
    {
        float maxRotationStep=1f;
        float aveHitNormalX = 0f;
        float aveHitNormalY = 0f;
        float aveHitNormalZ = 0f;
        for (int i = 0; i < 4; i++)
        {
            aveHitNormalX += allHitNormals[i].x;
            aveHitNormalY += allHitNormals[i].y;
            aveHitNormalZ += allHitNormals[i].z;
        }
        aveHitNormal = new Vector3(aveHitNormalX / 4, aveHitNormalY / 4, aveHitNormalZ / 4).normalized;

        ProjectedAxisAngles(out angleAboutX, out angleAboutZ, transform, aveHitNormal);

        float maxRotationX = 50;
        float maxRotationZ = 20;
        float characterXRotation = transform.eulerAngles.x;
        float characterZRotation = transform.eulerAngles.z;

        if (characterXRotation>180)
        {
            characterXRotation -= 360;
        }
        if (characterZRotation>180)
        {
            characterZRotation -= 360;
        }

        if (characterXRotation+angleAboutX<-maxRotationX)
        {
            angleAboutX = maxRotationX + characterXRotation;
        }
        else if (characterXRotation + angleAboutX >maxRotationX)
        {
            angleAboutX = maxRotationX - characterXRotation;
        }
        if (characterZRotation + angleAboutZ< -maxRotationZ)
        {
            angleAboutZ = maxRotationZ + characterZRotation;
        }
        else if (characterZRotation + angleAboutZ > maxRotationZ)
        {
            angleAboutZ = maxRotationZ - characterZRotation;
        }

        float bodyEulerX = Mathf.MoveTowardsAngle(0, angleAboutX, maxRotationStep);
        float bodyEulerZ = Mathf.MoveTowardsAngle(0, angleAboutZ, maxRotationStep);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x + bodyEulerX, transform.eulerAngles.y,
                                                transform.eulerAngles.z + angleAboutZ);

    }

    private void CharacterHeightAdjustments()
    {
        for (int i = 0; i < 4; i++)
        {
            if (allFootTransforms[i].localPosition.y<upperFootYlimit && allTargetTransforms[i].localPosition.y>lowerFootYlimit)
            {
                checkLocalTargetY[i] = 0;
            }
            else if (allTargetTransforms[i].localPosition.y>upperFootYlimit)
            {
                checkLocalTargetY[i] = 1;
            }
            else
            {
                checkLocalTargetY[i] = -1;
            }
        }

        if (checkLocalTargetY[0] ==1 && checkLocalTargetY[2]==1 || checkLocalTargetY[1] == 1 && checkLocalTargetY[3] == 1)
        {
            if (capsuleCollider.center.y>-1.4)
            {
                capsuleCollider.center -= new Vector3(0f, 0.05f, 0f);
            }
            else
            {
                capsuleCollider.center = new Vector3(0f, 3.4f, 0f);
            }
            
        }
        else if (checkLocalTargetY[0] == -1 && checkLocalTargetY[2] == -1 || checkLocalTargetY[1] == -1 && checkLocalTargetY[3] == -1)
        {
            if (capsuleCollider.center.y < 1.5)
            {
                capsuleCollider.center += new Vector3(0f, 0.05f, 0f);
            }

            
        }

    }





}
