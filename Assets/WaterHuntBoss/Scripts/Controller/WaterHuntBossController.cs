using CustomUtils;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterHuntBossController : SingletonMono<WaterHuntBossController>
{
    [Header("Water Hunt Boss")]
    public WaterHuntBossView WaterHuntBossPrefab;
    public Vector2Int WaterHuntBossPos;
    public int Index = -1;

    [Header(" Trap ")] 
    public Tilemap TrapForWaterHuntTilemap;

    [Header(" Water ")]
    public GameObject WaterPrefab;
    public List<Vector2Int> WaterPosList;
    public int CurWaterIndex;

    private GameObject _curWater;
    private WaterHuntBossModel _waterHuntBoss;
    private WaterHuntBossSO _waterHuntBossData;

    public Vector2Int NewPosForBoss = new Vector2Int(-1, -1);

    public void SetWaterHuntBoss(int index)
    {
        if (index - 1 < 0)
        {
            this.Index = -1;
            return;
        }
        this.Index = index - 1;
        this._waterHuntBossData = DataManager.Instance.WaterHuntBossData;
        this.WaterHuntBossPos = _waterHuntBossData.WaterHuntBossList[Index].WaterHuntBossPos;
        this.SpawnWaterHuntBoss();

        this.WaterPosList = new List<Vector2Int>();
        foreach (var water in _waterHuntBossData.WaterHuntBossList[Index].WaterList)
        {
            this.WaterPosList.Add(water.WaterPos);
        }
        this.CurWaterIndex = 0;

        this.SpawnWater();
        this.NewPosForBoss = this.WaterPosList[this.CurWaterIndex];
    }

    private void SpawnWaterHuntBoss()
    {
        Vector3Int initWaterHuntBossPos = new Vector3Int(WaterHuntBossPos.x, WaterHuntBossPos.y, 0);
        Tilemap groundTilemap = SlideController.Instance.groundTilemap;
        WaterHuntBossView waterHuntBossView = Instantiate(WaterHuntBossPrefab, groundTilemap.CellToWorld(initWaterHuntBossPos) + groundTilemap.cellSize / 2, Quaternion.identity);
        _waterHuntBoss = new WaterHuntBossModel();
        _waterHuntBoss.SetView(waterHuntBossView);
    }

    private void SpawnWater()
    {
        Vector3Int initWaterPos = new Vector3Int(   this.WaterPosList[this.CurWaterIndex].x, 
                                                    this.WaterPosList[this.CurWaterIndex].y, 
                                                    0);
        Tilemap groundTilemap = SlideController.Instance.groundTilemap;
        _curWater = Instantiate(WaterPrefab, groundTilemap.CellToWorld(initWaterPos) + groundTilemap.cellSize / 2, Quaternion.identity);
    } 

    public void MoveWater(Vector2Int newGridPos, Vector3 worldPos)
    {
        this.WaterPosList[CurWaterIndex] = newGridPos;
        this._curWater.transform.DOMove(worldPos, 0.25f).SetEase(Ease.InOutSine);
    }

    public void TeleWater(Vector2Int newGridPos, Vector3 worldPos)
    {
        this._curWater.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            this._curWater.transform.position = worldPos;
            this.WaterPosList[CurWaterIndex] = newGridPos;
            this._curWater.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
        });
    }

    public void HideWater()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(this._curWater.gameObject.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        seq.Join(this._curWater.gameObject.transform.DORotate(new Vector3(0, 0, 360f), 0.2f, RotateMode.FastBeyond360));
        seq.OnComplete(() => 
        {
            Destroy(this._curWater.gameObject);
            this.ResetWater();
            
        });
    }

    public bool CheckAndMoveWater(Player player, List<Vector2Int> cellPosList, Direction direction)
    {
        Vector2Int offset = new Vector2Int(0, 0);
        switch (direction)
        {
            case Direction.Left:
                offset = new Vector2Int(-1, 0);
                break;
            case Direction.Right:
                offset = new Vector2Int(1, 0);
                break;
            case Direction.Up:
                offset = new Vector2Int(0, 1);
                break;
            case Direction.Down:
                offset = new Vector2Int(0, -1);
                break;
        }

        Vector2Int oldPlayerPos = player.GetCurrentPos();
        Vector2Int waterPos = WaterPosList[this.CurWaterIndex];
        bool check = false;

        if (((direction == Direction.Left || direction == Direction.Right) &&
            oldPlayerPos.y == waterPos.y) ||
            ((direction == Direction.Up || direction == Direction.Down) &&
            oldPlayerPos.x == waterPos.x))
        {
            check = true;
        }
        if (!check) 
        { 
            this.NewPosForBoss = this.WaterPosList[this.CurWaterIndex];
            return true;
        }
        bool isTeleport = false;
        if (cellPosList[0] == waterPos)
        {
            waterPos = cellPosList[cellPosList.Count - 1];
            isTeleport = true;
        }
        else
        {
            waterPos += offset;
        }

        Vector3 worldPos = SlideController.Instance.groundTilemap.GetCellCenterWorld(
                            new Vector3Int(waterPos.x, waterPos.y, 0));
        if (!this.CheckCanMoveWater(new Vector3Int(waterPos.x, waterPos.y, 0)))
        {
            return false;
        }

        this.NewPosForBoss = this.WaterPosList[this.CurWaterIndex];

        if (!isTeleport)
        {
            this.MoveWater(waterPos, worldPos);
        }
        else
        {
            this.TeleWater(waterPos, worldPos);
        }
        return true;
    }

    public bool CheckCanMoveWater(Vector3Int newPos)
    {
        Vector2Int newWaterPos = new Vector2Int(newPos.x, newPos.y);
        if (SlideController.Instance.obstacleTilemap.HasTile(newPos) || newWaterPos == WaterHuntBossPos)
        {
            return false;
        }
        return true;
    }

    public bool CheckMoveForBoss(Vector2Int waterPos, Player player)
    {
        Vector2Int offset = new Vector2Int(0, 0);
        offset = new Vector2Int(-1, 0);
        if (waterPos == player.GetCurrentPos()) return false;

        if (waterPos == WaterHuntBossPos + offset)
        {
            return true;
        }
        offset = new Vector2Int(1, 0);
        if (waterPos == WaterHuntBossPos + offset)
        {
            return true;
        }
        offset = new Vector2Int(0, -1);
        if (waterPos == WaterHuntBossPos + offset)
        {
            return true;
        }
        offset = new Vector2Int(0, 1);
        if (waterPos == WaterHuntBossPos + offset)
        {
            return true;
        }
        return false;
    }

    public void MoveWaterHuntBoss()
    {
        this.WaterHuntBossPos = this.NewPosForBoss;
        Vector3Int posCheckTrap = new Vector3Int(WaterHuntBossPos.x, WaterHuntBossPos.y, 0);
        this._waterHuntBoss.WaterHuntBossView.Move(this.WaterHuntBossPos);
        if (this.TrapForWaterHuntTilemap.HasTile(posCheckTrap))
        {
            this.TrapForWaterHuntTilemap.SetTile(posCheckTrap, null);
            this._waterHuntBoss.TakeDamage(1);
        }
        
        if (_waterHuntBoss.Health > 0 && 
            WaterHuntBossPos == this.WaterPosList[this.CurWaterIndex])
        {
            this.HideWater();
        }
    }

    public void ResetWater()
    {
        this.CurWaterIndex++;
        if (this.CurWaterIndex < this.WaterPosList.Count)
        {
            this.SpawnWater();
        }
        else
        {
            Debug.Log("Lose");
        }
    }
}
