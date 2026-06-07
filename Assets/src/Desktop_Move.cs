using UnityEngine;
using UnityEngine.InputSystem; // InputSystem を使用するために必要な名前空間を追加

public class Desktop_Move : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f; // 移動速度を調整するための変数を追加
    [SerializeField] private float rotationSpeed = 100f; // 回転速度を調整するための変数を追加
    [SerializeField] private Transform cameraTransform; // カメラの Transform をインスペクターから割り当てるための変数を追加

    private float yaw; // カメラの水平回転を保持する変数
    private float pitch; // カメラの垂直回転を保持する変数

    void Update(){
        Move(); // 毎フレーム Move メソッドを呼び出して、プレイヤーの移動を処理する
        RotateCamera(); // 毎フレーム RotateCamera メソッドを呼び出して、カメラの回転を処理する
    }

    void Move()
    {
        Vector2 input = Vector2.zero; // 入力を格納する Vector2 変数を初期化

        if (Keyboard.current.wKey.isPressed) 
        {
            input.y += 1;
        } // wキーが押されている場合、前方向に移動するために y 成分を増加させる

        if (Keyboard.current.sKey.isPressed)
        {
            input.y -= 1;
        } // sキーが押されている場合、後方向に移動するために y 成分を減少させる

        if (Keyboard.current.dKey.isPressed)
        {
            input.x += 1;
        } // dキーが押されている場合、右方向に移動するために x 成分を増加させる

        if (Keyboard.current.aKey.isPressed)
        {
            input.x -= 1;
        } // aキーが押されている場合、左方向に移動するために x 成分を減少させる

        Vector3 forward = cameraTransform.forward; // カメラの前方向を取得
        Vector3 right = cameraTransform.right; // カメラの右方向を取得

        // 水平方向の移動にするために、y成分を0にして正規化する
        forward.y = 0; 
        right.y = 0;
        
        // 移動ベクトルを正規化して、斜め移動の速度が速くならないようにする
        forward.Normalize();
        right.Normalize();


        // 入力に基づいて移動ベクトルを計算する
        Vector3 move = forward * input.y + right * input.x;

        // 移動ベクトルの大きさが1を超える場合は正規化して、移動速度が速くならないようにする
        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        // 移動する
        transform.position += move * moveSpeed * Time.deltaTime;
    }

    private void RotateCamera()
    {
        if(Keyboard.current.rightArrowKey.isPressed)
        {
            yaw += rotationSpeed * Time.deltaTime; // 右矢印キーが押されている場合、水平回転を増加させる
        }
        if(Keyboard.current.leftArrowKey.isPressed)
        {
            yaw -= rotationSpeed * Time.deltaTime; // 左矢印キーが押されている場合、水平回転を減少させる
        }
        if(Keyboard.current.upArrowKey.isPressed)
        {
            pitch -= rotationSpeed * Time.deltaTime; // 上矢印キーが押されている場合、垂直回転を減少させる
        }
        if(Keyboard.current.downArrowKey.isPressed)
        {
            pitch += rotationSpeed * Time.deltaTime; // 下矢印キーが押されている場合、垂直回転を増加させる
        }

        pitch = Mathf.Clamp(pitch, -90f, 90f); // 垂直回転を-90度から90度の範囲に制限する

        transform.rotation = Quaternion.Euler(0, yaw, 0); // プレイヤーの水平回転を設定する
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0); //
     
    }
}
