using TMPro;
using UnityEngine;
using Fusion;

public class PlayerNameUI : NetworkBehaviour
{
    public TextMeshProUGUI nameText;

    public override void Spawned()
    {
        // 既に割当があればそれを使う
        if (nameText == null)
        {
            // 優先: 自分の子要素にある UI を探す
            nameText = GetComponentInChildren<TextMeshProUGUI>(true);

            // 見つからなければシーン内から最初の TextMeshProUGUI を使う（安全策）
            if (nameText == null)
            {
                var all = FindObjectsOfType<TextMeshProUGUI>();
                if (all != null && all.Length > 0) nameText = all[0];
            }
        }

        if (nameText == null)
        {
            // UI が見つからなければログ出力して何もしない（エラー回避）
            Debug.LogWarning("[PlayerNameUI] nameText is not assigned and none found in scene.");
            return;
        }

        // InputAuthority が未設定の可能性に備えデフォルトを用意
        try
        {
            nameText.text = Object != null ? Object.InputAuthority.PlayerId.ToString() : "Player";
        }
        catch
        {
            nameText.text = "Player";
        }
    }
}