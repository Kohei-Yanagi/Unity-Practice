using Fusion;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using VRM;

public class VRPlayer : NetworkBehaviour
{
    // アバターの追従先です.
    // ローカルのトラッキング情報で動かし、その位置を使ってアバターを動かします.
    public Transform headTarget;
    public Transform leftHandTarget;
    public Transform rightHandTarget;

    // 実際に表示される VRM アバターの親 Transform です.
    public Transform avatarRoot;

    // ローカル側のトラッキング元です.
    // VR では XROrigin のカメラやコントローラーを指します.
    // Laptop では xrHead が DesktopHead を指します.
    public Transform xrHead;
    public Transform xrLeftHand;
    public Transform xrRightHand;

    // アバター付近に表示するプレイヤー名です.
    public TextMeshPro nameText;

    // Laptop プレイヤーの頭位置として使う Hierarchy 上の GameObject 名です.
    // GameObject の名前は必ず "DesktopHead" にします.
    [SerializeField] private string desktopHeadObjectName = "DesktopHead";

    private bool m_lookAtAssigned = false;

    public override void Spawned()
    {
        // 自分自身のプレイヤーだけが、この端末のカメラやコントローラー情報を読みます.
        if (Object.HasInputAuthority)
        {
            AssignLocalTrackingTargets();
        }

        SetupNameText();
    }

    private void AssignLocalTrackingTargets()
    {
        // VR 実機の場合は、XROrigin のカメラを頭、コントローラーを手として使います.
        if (XRSettings.isDeviceActive && TryAssignXrTrackingTargets())
        {
            return;
        }

        // Laptop の場合は、シーンに置いた DesktopHead を頭として使います.
        if (TryAssignDesktopTrackingTarget())
        {
            return;
        }

        // 最後の保険として Camera.main を使います.
        // Camera.main がプレイヤーの頭とは限らないため、できれば DesktopHead を用意してください.
        AssignCameraMainFallback();
    }

    private bool TryAssignXrTrackingTargets()
    {
        var xr = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();

        if (xr == null)
        {
            return false;
        }

        xrHead = xr.Camera != null ? xr.Camera.transform : Camera.main?.transform;
        xrLeftHand = xr.transform.Find("Camera Offset/Left Controller");
        xrRightHand = xr.transform.Find("Camera Offset/Right Controller");

        Debug.Log($"Using XR tracking target: {xrHead}");
        return true;
    }

    private bool TryAssignDesktopTrackingTarget()
    {
        var desktopHead = GameObject.Find(desktopHeadObjectName);

        if (desktopHead == null)
        {
            return false;
        }

        xrHead = desktopHead.transform;

        // Laptop には XR コントローラーがないので、手のトラッキングは使いません.
        xrLeftHand = null;
        xrRightHand = null;

        Debug.Log($"Using desktop tracking target: {xrHead}");
        return true;
    }

    private void AssignCameraMainFallback()
    {
        xrHead = Camera.main?.transform;
        xrLeftHand = null;
        xrRightHand = null;

        Debug.LogWarning($"DesktopHead was not found. Falling back to Camera.main: {xrHead}");
    }

    private void SetupNameText()
    {
        if (nameText == null)
        {
            return;
        }

        nameText.text = $"Player {Object.InputAuthority.PlayerId}";
        nameText.color = Object.HasInputAuthority ? Color.green : Color.white;
    }

    public override void FixedUpdateNetwork()
    {
        // 1. ローカルプレイヤーの処理です.
        // この端末のトラッキング情報を、ネットワーク同期用のターゲットへコピーします.
        if (Object.HasInputAuthority)
        {
            UpdateLocalTrackingTargets();
        }

        // 2. 全プレイヤーで、同期された headTarget を基準にアバターを動かします.
        UpdateAvatarRoot();

        // 3. 全プレイヤーで、NetworkObject のルート位置を headTarget 付近に保ちます.
        UpdateNetworkRootPosition();

        // 4. 全プレイヤーで、VRM の LookAt 対象を一度だけ設定します.
        AssignLookAtTargetOnce();
    }

    private void UpdateLocalTrackingTargets()
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

    private void UpdateAvatarRoot()
    {
        if (avatarRoot == null || headTarget == null)
        {
            return;
        }

        // アバターの体は頭より下に置き、少し前方向にずらします.
        avatarRoot.position = headTarget.position + new Vector3(0, -1.5f, 0.15f);

        // 頭の Y 回転だけを使い、体は水平方向にだけ回転させます.
        avatarRoot.rotation = Quaternion.Euler(
            0,
            headTarget.rotation.eulerAngles.y,
            0
        );
    }

    private void UpdateNetworkRootPosition()
    {
        if (headTarget == null)
        {
            return;
        }

        transform.position = headTarget.position;
    }

    private void AssignLookAtTargetOnce()
    {
        if (m_lookAtAssigned || avatarRoot == null || headTarget == null)
        {
            return;
        }

        var lookAt = avatarRoot.GetComponentInChildren<VRMLookAtHead>();

        if (lookAt == null)
        {
            return;
        }

        lookAt.Target = headTarget;
        m_lookAtAssigned = true;
    }
}
