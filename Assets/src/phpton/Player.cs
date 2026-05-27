using Fusion;
using UnityEngine;
using TMPro;
using VRM;

public class VRPlayer : NetworkBehaviour
{
    public Transform headTarget;
    public Transform leftHandTarget;
    public Transform rightHandTarget;

    public Transform avatarRoot;

    public Transform xrHead;
    public Transform xrLeftHand;
    public Transform xrRightHand;

    public TextMeshPro nameText;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            var xr = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();

            if (xr != null)
            {
                xrHead = xr.Camera != null ? xr.Camera.transform : Camera.main?.transform;
                xrLeftHand = xr.transform.Find("Camera Offset/Left Controller");
                xrRightHand = xr.transform.Find("Camera Offset/Right Controller");
            }
            else
            {
                xrHead = Camera.main?.transform;
            }
        }

        if (nameText != null)
        {
            nameText.text = $"Player {Object.InputAuthority.PlayerId}";
            nameText.color = Object.HasInputAuthority ? Color.green : Color.white;
        }
    }

    bool m_lookAtAssigned = false;

   public override void FixedUpdateNetwork()
{
    // LocalPlayerだけ tracking 更新
    if (Object.HasInputAuthority)
    {
        if (xrHead != null && headTarget != null)
        {
            headTarget.position = xrHead.position;
            headTarget.rotation = xrHead.rotation;
        }

        if (xrLeftHand != null && leftHandTarget != null)
        {
            leftHandTarget.position = xrLeftHand.position;
            leftHandTarget.rotation = xrLeftHand.rotation;
        }

        if (xrRightHand != null && rightHandTarget != null)
        {
            rightHandTarget.position = xrRightHand.position;
            rightHandTarget.rotation = xrRightHand.rotation;
        }
    }

    // 全プレイヤーでAvatar更新
    if (avatarRoot != null && headTarget != null)
    {
        avatarRoot.position =
            headTarget.position + new Vector3(0, -1.5f, 0);

        avatarRoot.rotation =
            Quaternion.Euler(
                0,
                headTarget.rotation.eulerAngles.y,
                0
            );
    }

    // Root位置
    if (headTarget != null)
    {
        transform.position = headTarget.position;
    }

    // LookAt設定
    if (!m_lookAtAssigned &&
        avatarRoot != null &&
        headTarget != null)
    {
        var lookAt =
            avatarRoot.GetComponentInChildren<VRMLookAtHead>();

        if (lookAt != null)
        {
            lookAt.Target = headTarget;
            m_lookAtAssigned = true;
        }
    }
}
}