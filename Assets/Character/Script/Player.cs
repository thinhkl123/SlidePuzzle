using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2Int currentPos;

    public Vector2Int GetCurrentPos()
    {
        return currentPos;
    }

    public void SetCurrentPos(Vector2Int newPos)
    {
        currentPos = newPos;
    }

    public void MoveTo(Vector2Int newGridPos, Vector3 worldPos)
    {
        currentPos = newGridPos;
        transform.DOMove(worldPos, 0.25f).SetEase(Ease.InOutSine);
    }

    public void Teleport(Vector2Int newGridPos, Vector3 worldPos)
    {
        // 1. Scale nhỏ dần để ẩn 
        this.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            // 2. Di chuyển đến đầu hàng
            this.transform.position = worldPos;
            this.currentPos = newGridPos;

            // 3. Scale lớn lên để hiện  lại
            this.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
        });
    }

    public void Shake()
    {
        this.transform.localRotation = Quaternion.identity;

        this.transform.DOShakeRotation(
            duration: 0.4f,       // Thời gian lắc
            strength: new Vector3(0, 0, 15f), // Xoay quanh trục Z
            vibrato: 10,          // Số lần lắc
            randomness: 90f       // Ngẫu nhiên góc
        ).SetEase(Ease.OutQuad);
    }
}
