using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using UnityEngine.Tilemaps;
using System;
using DG.Tweening;

public class SlideController : SingletonMono<SlideController> 
{
    [Header(" Tile Fake ")]
    [SerializeField] private TileFake groudTileFakePrefab;
    [SerializeField] private TileFake itemTileFakePrefab;
    [SerializeField] private TileFake enemyNotMoveTileFakePrefab;
    [SerializeField] private TileFake bossLongTileFakePrefab;

    [Header(" Player ")]
    [SerializeField] private Player playerPrefab;

    [Header(" TileMap ")]
    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap itemTilemap;
    public Tilemap enemyNotMoveTilemap;
    public Tilemap bossLongTilemap;

    private Player _player;
    public bool canSlide;
    public bool isWaitMore;
    private int curLevelId;

    //Level Data Variable
    int itemId;
    int enemyNotMoveId;
    int blockId;
    bool hasBossLong;

    private void Start()
    {
        SpawnLevel();
        canSlide = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) 
        {
            Slide(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Slide(Direction.Right);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Slide(Direction.Up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Slide(Direction.Down);
        }
    }

    private void Slide(Direction direction)
    {
        if (!canSlide)
        {
            return;
        }

        //Setup, get cell list
        Vector2Int dir1 = new Vector2Int(0, 0), dir2 = new Vector2Int(0, 0);
        Vector2Int curPlayerPos = _player.GetCurrentPos();

        if (direction == Direction.Left || direction == Direction.Right)
        {
            dir1 = new Vector2Int(-1, 0);
            dir2 = new Vector2Int(1, 0);
        } 
        else if (direction == Direction.Up || direction == Direction.Down)
        {
            dir1 = new Vector2Int(0, -1);
            dir2 = new Vector2Int(0, 1);
        }

        List<Vector2Int> cellMovePosList = new List<Vector2Int>();

        for (int i = 1; i <= 100; i++)
        {
            Vector2Int newPos = curPlayerPos + i * dir1;
            if (groundTilemap.HasTile(new Vector3Int(newPos.x, newPos.y, 0)))
            {
                cellMovePosList.Add(newPos);
            }
            else
            {
                break;
            }
        }

        cellMovePosList.Reverse();

        cellMovePosList.Add(curPlayerPos);

        for (int i = 1; i <= 100; i++)
        {
            Vector2Int newPos = curPlayerPos + i * dir2;
            if (groundTilemap.HasTile(new Vector3Int(newPos.x, newPos.y, 0)))
            {
                cellMovePosList.Add(newPos);
            }
            else
            {
                break;
            }
        }

        if (direction == Direction.Right || direction == Direction.Up)
        {
            cellMovePosList.Reverse();
        }

        //Move Player
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

        Vector2Int newPlayerPos = new Vector2Int(0, 0);
        bool isTeleport = false;
        if (cellMovePosList[0] == _player.GetCurrentPos())
        {
            newPlayerPos = cellMovePosList[cellMovePosList.Count-1];
            isTeleport = true;
        }
        else
        {
            newPlayerPos = _player.GetCurrentPos() + offset;
        }
        Vector3Int newPlayerGridPos = new Vector3Int(newPlayerPos.x, newPlayerPos.y, 0);

        if (!CheckPlayerCanMove(newPlayerGridPos, cellMovePosList))
        {
            _player.Shake();

            return;
        }

        canSlide = false;
        isWaitMore = false;

        Vector3 pos = groundTilemap.GetCellCenterWorld(newPlayerGridPos);

        if (isTeleport)
        {
            _player.Teleport(newPlayerPos, pos);
        }
        else
        {
            _player.MoveTo(newPlayerPos, pos);
        }

        MoveGroundTile(cellMovePosList, direction);

        MoveItemTile(cellMovePosList, direction);

        MoveBossLongTile(cellMovePosList, direction);

        ResetCanSlide();
    }

    private void ResetCanSlide()
    {
        float time = 0.25f;
        if (isWaitMore)
        {
            time += 0.25f;
        }

        Invoke(nameof(SetCanSlide), time);
    }

    private void SetCanSlide()
    {
        canSlide = true;
    }

    private void MoveBossLongTile(List<Vector2Int> cellsToSlide, Direction direction)
    {
        if (!hasBossLong)
        {
            return;
        }

        if (!cellsToSlide.Contains(BossLongController.Instance.posList[0]))
        {
            return;
        }

        //Spawn các tile động theo thứ tự cells
        List<TileFake> clones = new List<TileFake>();
        List<TileBase> tileOrder = new List<TileBase>();

        for (int i = 0; i < BossLongController.Instance.posList.Count; i++)
        {
            Vector2Int cellPos = BossLongController.Instance.posList[i];
            Vector3Int cell = new Vector3Int(cellPos.x, cellPos.y, 0);
            TileBase tile = bossLongTilemap.GetTile(cell);

            // Lưu thứ tự tile để xử lý logic wrap-around
            tileOrder.Add(tile);

            // Tạo GameObject clone để tween
            TileFake obj = Instantiate(bossLongTileFakePrefab, bossLongTilemap.GetCellCenterWorld(cell), Quaternion.identity);
            Sprite sprite = GetSpriteFromTile(tile);
            if (sprite != null)
                obj.SetSprite(sprite);

            obj.gridPos = cellPos;
            clones.Add(obj);

            bossLongTilemap.SetTile(cell, null);
        }

        int count = BossLongController.Instance.posList.Count;

        Vector2Int newHeadPos = new Vector2Int(0, 0);
        if (cellsToSlide.IndexOf(BossLongController.Instance.posList[0]) == 0)
        {
            TileFake wrapTile = clones[0];
            Vector2Int wrapToGrid = cellsToSlide[cellsToSlide.Count - 1]; newHeadPos = wrapToGrid;
            Vector3 wrapToPos = bossLongTilemap.GetCellCenterWorld((Vector3Int)cellsToSlide[cellsToSlide.Count - 1]);

            // 1. Scale nhỏ dần để ẩn tile
            wrapTile.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                // 2. Di chuyển đến đầu hàng
                wrapTile.transform.position = wrapToPos;
                wrapTile.gridPos = wrapToGrid;

                // 3. Scale lớn lên để hiện tile lại
                wrapTile.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
            });
        }
        else
        {
            Vector2Int toGrid = cellsToSlide[cellsToSlide.IndexOf(BossLongController.Instance.posList[0]) - 1];
            newHeadPos = toGrid;
            Vector3 worldPos = bossLongTilemap.GetCellCenterWorld(new Vector3Int(toGrid.x, toGrid.y, 0));

            clones[0].MoveTo(toGrid, worldPos);
        }

        //Debug.Log(newHeadPos);

        if (BossLongController.Instance.posList.IndexOf(newHeadPos) == count - 1 && BossLongController.Instance.IsFitLength())
        {
            Debug.Log("Next Phase");
            for (int i = 1; i < count; i++)
            {
                clones[i].transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack);
            }
            

            DOVirtual.DelayedCall(0.25f, () =>
            {
                BossLongController.Instance.NextPhase(newHeadPos);

                bossLongTilemap.SetTile(new Vector3Int(newHeadPos.x, newHeadPos.y, 0), DataManager.Instance.BossLongData.headTile);

                foreach (var obj in clones)
                {
                    if (obj != null)
                        Destroy(obj.gameObject);
                }
            });

            return;
        }

        bool isDecrease = false;

        if (count >= 2)
        {
            if (BossLongController.Instance.posList.Contains(newHeadPos))
            {
                clones[count - 1].transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack);
                isDecrease = true;
            }
            else
            {
                Vector2Int toGrid = BossLongController.Instance.posList[count - 2];
                Vector3 worldPos = bossLongTilemap.GetCellCenterWorld(new Vector3Int(toGrid.x, toGrid.y, 0));

                clones[count - 1].MoveTo(toGrid, worldPos);
            }
        }

        //Di chuyen cac tile
        if (!isDecrease)
        {
            for (int i = 1; i < count - 1; i++)
            {
                Vector2Int toGrid = BossLongController.Instance.posList[i - 1];
                Vector3 worldPos = bossLongTilemap.GetCellCenterWorld(new Vector3Int(toGrid.x, toGrid.y, 0));

                clones[i].MoveTo(toGrid, worldPos);
            }

            isWaitMore = true;
        }
        else
        {
            for (int i = 1; i < count - 1; i++)
            {
                Vector2Int toGrid = BossLongController.Instance.posList[i + 1];
                Vector3 worldPos = bossLongTilemap.GetCellCenterWorld(new Vector3Int(toGrid.x, toGrid.y, 0));

                clones[i].MoveTo(toGrid, worldPos);
            }
        }

        //Bước 3: Sau khi tween xong → cập nhật lại Tilemap và xóa clone
        DOVirtual.DelayedCall(0.25f, () =>
        {
            if (!isDecrease)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    int fromIndex = (i + 1) % count;

                    Vector2Int toCell = BossLongController.Instance.posList[i];
                    bossLongTilemap.SetTile(new Vector3Int(toCell.x, toCell.y, 0), tileOrder[fromIndex]);
                }
            }
            else
            {
                for (int i = 2; i < count; i++)
                {
                    int fromIndex = (i - 1) % count;

                    Vector2Int toCell = BossLongController.Instance.posList[i];
                    bossLongTilemap.SetTile(new Vector3Int(toCell.x, toCell.y, 0), tileOrder[fromIndex]);
                }
            }

            //if (count >=2 && !isDecrease)
            //{
            //    int fromIndex = (count - 2 + 1) % count;

            //    Vector2Int toCell = BossLongController.Instance.posList[count - 2];
            //    bossLongTilemap.SetTile(new Vector3Int(toCell.x, toCell.y, 0), tileOrder[fromIndex]);
            //}

            bossLongTilemap.SetTile(new Vector3Int(newHeadPos.x, newHeadPos.y, 0), DataManager.Instance.BossLongData.headTile);
            
            BossLongController.Instance.UpdatePosList(newHeadPos, isDecrease);

            foreach (var obj in clones)
            {
                if (obj != null)
                    Destroy(obj.gameObject);
            }
        });
    }

    public void SpawnBossLongTile(TileBase tile, Vector3Int gridPos)
    {
        TileFake tileFake = Instantiate(bossLongTileFakePrefab, bossLongTilemap.GetCellCenterWorld(gridPos), Quaternion.identity);
        tileFake.transform.localScale = Vector3.zero;
        tileFake.SetSprite(GetSpriteFromTile(tile));
        tileFake.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        DOVirtual.DelayedCall(0.25f, () =>
        {
            Destroy(tileFake.gameObject);
            bossLongTilemap.SetTile(gridPos, tile);
        });
    }

    private void MoveItemTile(List<Vector2Int> cellsToSlide, Direction direction)
    {
        if (itemId == 0)
        {
            return;
        }

        //Spawn các tile động theo thứ tự cells
        List<TileFake> clones = new List<TileFake>();
        List<TileBase> tileOrder = new List<TileBase>();

        foreach (Vector2Int cellPos in cellsToSlide)
        {
            Vector3Int cell = new Vector3Int(cellPos.x, cellPos.y, 0);
            TileBase tile = itemTilemap.GetTile(cell);
            if (tile == null)
            {
                clones.Add(null);
                tileOrder.Add(null);

                continue;
            }

            // Lưu thứ tự tile để xử lý logic wrap-around
            tileOrder.Add(tile);

            // Tạo GameObject clone để tween
            TileFake obj = Instantiate(itemTileFakePrefab, itemTilemap.GetCellCenterWorld(cell), Quaternion.identity);
            Sprite sprite = GetSpriteFromTile(tile);
            if (sprite != null)
                obj.SetSprite(sprite);

            obj.gridPos = cellPos;
            clones.Add(obj);

            itemTilemap.SetTile(cell, null);
        }

        //Di chuyển các tile
        int count = cellsToSlide.Count;
        for (int i = 1; i < count; i++)
        {
            if (clones[i] == null)
            {
                continue;
            }
            Vector2Int toGrid = cellsToSlide[i - 1];
            Vector3 worldPos = itemTilemap.GetCellCenterWorld(new Vector3Int(toGrid.x, toGrid.y, 0));

            clones[i].MoveTo(toGrid, worldPos);
        }

        // Tile cuối
        if (clones[0] != null)
        {
            TileFake wrapTile = clones[0];
            Vector2Int wrapToGrid = cellsToSlide[count - 1];
            Vector3 wrapToPos = itemTilemap.GetCellCenterWorld((Vector3Int)cellsToSlide[count - 1]);

            // 1. Scale nhỏ dần để ẩn tile
            wrapTile.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                // 2. Di chuyển đến đầu hàng
                wrapTile.transform.position = wrapToPos;
                wrapTile.gridPos = wrapToGrid;

                // 3. Scale lớn lên để hiện tile lại
                wrapTile.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
            });
        }

        //Bước 3: Sau khi tween xong → cập nhật lại Tilemap và xóa clone
        DOVirtual.DelayedCall(0.25f, () =>
        {
            for (int i = 0; i < count; i++)
            {
                int fromIndex = (i + 1) % count;

                if (tileOrder[fromIndex] == null)
                {
                    continue;
                }

                Vector2Int toCell = cellsToSlide[i];
                ItemTileController.Instance.UpdateItemPosList(cellsToSlide[fromIndex], toCell); //Update Item Pos List
                itemTilemap.SetTile(new Vector3Int(toCell.x, toCell.y, 0), tileOrder[fromIndex]);
            }

            foreach (var obj in clones)
            {
                if (obj != null)
                    Destroy(obj.gameObject);
            }
        });
    }

    private bool CheckPlayerCanMove(Vector3Int cellPlayer, List<Vector2Int> cellMoveList)
    {
        if (hasBossLong)
        {
            bool isDecrease = false;
            Vector2Int newHeadPos = new Vector2Int(0, 0);

            if (cellMoveList.Contains(BossLongController.Instance.posList[0]))
            {
                if (cellMoveList.IndexOf(BossLongController.Instance.posList[0]) == 0)
                {
                    newHeadPos = cellMoveList[cellMoveList.Count - 1];
                }
                else
                {
                    newHeadPos = cellMoveList[cellMoveList.IndexOf(BossLongController.Instance.posList[0]) - 1];
                }

                if (BossLongController.Instance.posList.Count >= 2)
                {  
                    if (BossLongController.Instance.posList.Contains(newHeadPos))
                    {
                        isDecrease = true;
                    }
                }
            }


            if (obstacleTilemap.HasTile(new Vector3Int(newHeadPos.x, newHeadPos.y, 0)))
            {
                return false;
            }

            if (BossLongController.Instance.posList.Contains(new Vector2Int(cellPlayer.x, cellPlayer.y)))
            {
                if (BossLongController.Instance.posList.IndexOf(new Vector2Int(cellPlayer.x, cellPlayer.y)) != 0)
                {
                    return false;
                }
                else
                {
                    if (!isDecrease)
                    {
                        return false;
                    }
                }
            }
        }

        if (CheckItemCollideWithObstacle(cellMoveList))
        {
            return false;
        }

        if (enemyNotMoveTilemap.HasTile(cellPlayer))
        {
            return false;
        }

        if (cellMoveList.Count <= 1 || obstacleTilemap.HasTile(cellPlayer))
        {
            return false;
        }

        return true;
    }

    private bool CheckItemCollideWithObstacle(List<Vector2Int> cellMoveList)
    {
        for (int i = 0; i < cellMoveList.Count; i++)
        {
            Vector3Int cell = new Vector3Int(cellMoveList[i].x, cellMoveList[i].y, 0);

            if (itemTilemap.HasTile(cell))
            {
                int fromIndex = cellMoveList.Count-1;
                if (i > 0)
                    fromIndex = i - 1;

                cell = new Vector3Int(cellMoveList[fromIndex].x, cellMoveList[fromIndex].y, 0);

                if (obstacleTilemap.HasTile(cell))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void MoveGroundTile(List<Vector2Int> cellsToSlide, Direction direction)
    {
        //Spawn các tile động theo thứ tự cells
        List<TileFake> clones = new List<TileFake>();
        List<TileBase> tileOrder = new List<TileBase>();

        foreach (Vector2Int cellPos in cellsToSlide)
        {
            Vector3Int cell = new Vector3Int(cellPos.x, cellPos.y, 0);
            TileBase tile = groundTilemap.GetTile(cell);
            if (tile == null) continue;

            // Lưu thứ tự tile để xử lý logic wrap-around
            tileOrder.Add(tile);

            // Tạo GameObject clone để tween
            TileFake obj = Instantiate(groudTileFakePrefab, groundTilemap.GetCellCenterWorld(cell), Quaternion.identity);
            Sprite sprite = GetSpriteFromTile(tile);
            if (sprite != null)
                obj.SetSprite(sprite);

            obj.gridPos = cellPos;
            clones.Add(obj);

            groundTilemap.SetTile(cell, null);
        }

        //Di chuyển các tile
        int count = cellsToSlide.Count;
        for (int i = 1; i < count; i++)
        {
            Vector2Int toGrid = cellsToSlide[i - 1];
            Vector3 worldPos = groundTilemap.GetCellCenterWorld(new Vector3Int(toGrid.x, toGrid.y, 0));

            clones[i].MoveTo(toGrid, worldPos);
        }

        // Tile cuối
        TileFake wrapTile = clones[0];
        Vector2Int wrapToGrid = cellsToSlide[count - 1];
        Vector3 wrapToPos = groundTilemap.GetCellCenterWorld((Vector3Int)cellsToSlide[count - 1]);

        // 1. Scale nhỏ dần để ẩn tile
        wrapTile.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            // 2. Di chuyển đến đầu hàng
            wrapTile.transform.position = wrapToPos;
            wrapTile.gridPos = wrapToGrid;

            // 3. Scale lớn lên để hiện tile lại
            wrapTile.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
        });

        //Bước 3: Sau khi tween xong → cập nhật lại Tilemap và xóa clone
        DOVirtual.DelayedCall(0.25f, () =>
        {
            for (int i = 0; i < count; i++)
            {
                int fromIndex = (i + 1) % count;
                Vector2Int toCell = cellsToSlide[i];
                groundTilemap.SetTile(new Vector3Int(toCell.x, toCell.y, 0), tileOrder[fromIndex]);
            }

            foreach (var obj in clones)
                Destroy(obj.gameObject);
        });
    }

    public void DefeatEnemyNotMove(Vector2Int enemyNotMovePos)
    {
        Vector3Int cell = new Vector3Int(enemyNotMovePos.x, enemyNotMovePos.y, 0);
        TileBase tile = enemyNotMoveTilemap.GetTile(cell);

        TileFake obj = Instantiate(enemyNotMoveTileFakePrefab, enemyNotMoveTilemap.GetCellCenterWorld(cell), Quaternion.identity);
        Sprite sprite = GetSpriteFromTile(tile);
        if (sprite != null)
            obj.SetSprite(sprite);

        enemyNotMoveTilemap.SetTile(cell, null);

        obj.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
    }

    public Sprite GetSpriteFromTile(TileBase tile)
    {
        if (tile is Tile t)
            return t.sprite;

        // Handle RuleTile or custom tile types here if needed
        return null;
    }

    private void SpawnLevel()
    {
        curLevelId = PlayerPrefs.GetInt(Constant.LEVELID, 1);
        SetItemTile();
        SetBlockTile();
        SetEnemyNotMoveTile();
        InitBossLong();
        SpawnPlayer();
    }

    private void InitBossLong()
    {
        hasBossLong = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].hasBossLong;
        if (hasBossLong)
        {
            BossLongController.Instance.Init(DataManager.Instance.BossLongData.InitBossPos);
        }
    }

    private void SetBlockTile()
    {
        blockId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].BlockId;
        if (blockId != 0)
            BlockTileController.Instance.SetBlockList(DataManager.Instance.BlockData.BlockDetails[blockId - 1].Blocks);
    }

    private void SetEnemyNotMoveTile()
    {
        enemyNotMoveId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].EnemyNotMoveId;
        if (enemyNotMoveId != 0)
            EnemyNotMoveTileController.Instance.SetEnemyNotMovePosList(DataManager.Instance.EnemyNotMoveData.EnemyNotMoveDetails[enemyNotMoveId - 1].enemyNotMovePosList);
    }

    private void SetItemTile()
    {
        itemId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].ItemId;
        if (itemId != 0)
            ItemTileController.Instance.SetWeaponPosList(DataManager.Instance.ItemData.ItemDetails[itemId - 1].WeaponPosList);
        if (itemId != 0)
            ItemTileController.Instance.SetKeyPosList(DataManager.Instance.ItemData.ItemDetails[itemId-1].KeyPosList);
    }

    private void SpawnPlayer()
    {
        Vector2Int pp = DataManager.Instance.LevelData.LevelDetails[curLevelId-1].PlayerPosition;
        Vector3Int initPlayerPos = new Vector3Int(pp.x, pp.y, 0);
        _player = Instantiate(playerPrefab, groundTilemap.CellToWorld(initPlayerPos) + groundTilemap.cellSize / 2, Quaternion.identity);
        _player.SetCurrentPos(pp);
    }

    //public Vector3 CellToWorld(Vector2Int gridPos)
    //{
    //    return groundTilemap.CellToWorld((Vector3Int)gridPos) + groundTilemap.cellSize / 2;
    //}

}

[Serializable]
public enum Direction
{
    None = 0,
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4,
}
