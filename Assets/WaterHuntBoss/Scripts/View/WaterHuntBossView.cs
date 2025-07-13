using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterHuntBossView : MonoBehaviour
{
    public GameObject HealthFull;
    public GameObject HealthHalf;
    public GameObject HealthEmpty;

    public void Move(Vector2Int newGridPos)
    {
        Vector3Int newPos = new Vector3Int(newGridPos.x, newGridPos.y, 0);
        Tilemap groundTilemap = SlideController.Instance.groundTilemap;
        Vector3 worldPos = groundTilemap.GetCellCenterWorld(newPos);
        this.transform.DOMove(worldPos, 0.25f).SetEase(Ease.InOutSine);
    }

    public void SetHide()
    {
        Sequence seq = DOTween.Sequence();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            seq.Join(sr.DOFade(0, 0.2f));
        }
        seq.Join(transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        seq.OnComplete(() => Destroy(gameObject));
    }

    public void SetHealth(int health)
    {
        if (health == 3)
        {
            this.SetHealthFull();
        }
        else if (health == 2)
        {
            this.SetHealthHalf();
        }
        else if (health == 1)
        {
            this.SetHealthEmpty();
        }
    }

    public void SetHealthFull()
    {
        HealthFull.SetActive(true);
        HealthHalf.SetActive(false);
        HealthEmpty.SetActive(false);
    }

    public void SetHealthHalf()
    {
        HealthFull.SetActive(false);
        HealthHalf.SetActive(true);
        HealthEmpty.SetActive(false);
    }

    public void SetHealthEmpty()
    {
        HealthFull.SetActive(false);
        HealthHalf.SetActive(false);
        HealthEmpty.SetActive(true);
    }
}
