using TMPro;
using UnityEngine;
using Fusion;

public class PlayerNameUI : NetworkBehaviour
{
    public TextMeshProUGUI nameText;

    public override void Spawned()
    {
        nameText.text = Object.InputAuthority.PlayerId.ToString();
    }
}