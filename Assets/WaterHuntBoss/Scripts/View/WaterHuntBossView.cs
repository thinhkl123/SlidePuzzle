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
        this.transform.DOPunchScale(
            punch: new Vector3(0.2f, 0.2f, 0f),
            duration: 0.23f,
            vibrato: 5,
            elasticity: 0.5f
        );
        this.transform.DOMove(worldPos, 0.25f).SetEase(Ease.InOutSine);
    }

    public void SetHide()
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        seq.OnComplete(() => 
        {
            WaterHuntBossController.Instance.Win();
        });
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
        this.Shake();
    }

    private void Shake()
    {
        this.transform.DOShakePosition(
            duration: 0.2f,
            strength: new Vector3(0.1f, 0.1f, 0.1f),
            vibrato: 10,
            randomness: 90f,
            snapping: false,
            fadeOut: true
        ).OnComplete(() => 
        {
            CheckDie();
        });
    }

    private void CheckDie()
    {
        if (WaterHuntBossController.Instance.WaterHuntBoss.Health <= 0)
        {
            WaterHuntBossController.Instance.WaterHuntBoss.WaterHuntBossView.SetHide();
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
