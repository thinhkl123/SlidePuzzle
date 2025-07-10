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
        gridPos = newGridPos;
        transform.DOMove(worldPos, 0.25f).SetEase(Ease.InOutSine);
    }
}
