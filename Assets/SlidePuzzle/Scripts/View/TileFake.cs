using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFake : MonoBehaviour
{
    public Vector2Int gridPos;
    public SpriteRenderer render;

    public void SetSprite(Sprite sprite)
    {
        this.render.sprite = sprite;
    }

    public void MoveTo(Vector2Int newGridPos, Vector3 worldPos)
    {
        if (Vector2Int.Distance(newGridPos, this.gridPos) != 1f)
        {
            // 1. Scale nhỏ dần để ẩn tile
            this.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                // 2. Di chuyển đến đầu hàng
                this.transform.position = worldPos;
                this.gridPos = newGridPos;

                // 3. Scale lớn lên để hiện tile lại
                this.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
            });
        }
        else
        {
            gridPos = newGridPos;
            transform.DOMove(worldPos, 0.25f).SetEase(Ease.InOutSine);
        }
    }
}
