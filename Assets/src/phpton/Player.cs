using Fusion;
using UnityEngine;
using TMPro;

public class VRPlayer : NetworkBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Transform xrHead;
    public Transform xrLeftHand;
    public Transform xrRightHand;

    public TextMeshPro nameText;

   public override void Spawned()
{
    if (Object.HasInputAuthority)
    {
        var xr = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();

        xrHead = xr.Camera.transform;

        xrLeftHand =
            xr.transform.Find("Camera Offset/Left Controller");

        xrRightHand =
            xr.transform.Find("Camera Offset/Right Controller");
    }

    nameText.text =
        $"Player {Object.InputAuthority.PlayerId}";

        if (Object.HasInputAuthority)
{
    nameText.color = Color.green;
}
else
{
    nameText.color = Color.white;
}
}

    public override void FixedUpdateNetwork()
{
    if (Object.HasInputAuthority)
    {
        head.position = xrHead.position;
        head.rotation = xrHead.rotation;

        leftHand.position = xrLeftHand.position;
        leftHand.rotation = xrLeftHand.rotation;

        rightHand.position = xrRightHand.position;
        rightHand.rotation = xrRightHand.rotation;

        transform.position = head.position;
    }
}}