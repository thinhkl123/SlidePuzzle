using CustomUtils;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleSortController : SingletonMono<PuzzleSortController>
{
    private List<List<int>> _result;
    private Vector2Int _puzzleSize;
    public int Index = -1;

    public void SetPuzzleSort(int puzzleSortId)
    {
        this.Index = puzzleSortId - 1;
        if (this.Index < 0) return;

        Vector2Int puzzleSideTmp =  DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].EndGroundPos -
                            DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartGroundPos +
                            Vector2Int.one;
        this._puzzleSize = new Vector2Int(puzzleSideTmp.y, puzzleSideTmp.x);
        this._result = new List<List<int>>();

        for (int i = 0; i < _puzzleSize.x; i++)
        {
            this._result.Add(new List<int>());
            for (int j = 0; j < _puzzleSize.y; j++)
            {
                this._result[i].Add(i * _puzzleSize.y + j);
            }
        }
        this.Shuffle();
    }

    // Không quan trọng, sau tự design map.
    private void Shuffle()
    {
        Debug.Log(_puzzleSize);
        for (int i = 0; i < _puzzleSize.x; i++)
        {
            for (int j = 0; j < _puzzleSize.y; j++)
            {
                int randomI = Random.Range(0, _puzzleSize.x);
                int randomJ = Random.Range(0, _puzzleSize.y);

                // Swap Result
                this.SwapResult(new Vector2Int(i, j), new Vector2Int(randomI, randomJ));

                // Swap Tilemap
                Vector2Int fromPos = DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartPuzzlePos;
                Vector2Int toPos = DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartPuzzlePos;
                fromPos += new Vector2Int(j, i);
                toPos += new Vector2Int(randomJ, randomI);
                this.SwapTiles(fromPos, toPos);
            }
        }

        for (int i = 0; i < _puzzleSize.x; i++)
        {
            for (int j = 0; j < _puzzleSize.y; j++)
            {
                if (this._result[i][j] == 1)
                {
                    this.SwapResult(new Vector2Int(i, j), new Vector2Int(0, 1));

                    // Swap Tilemap
                    Vector2Int fromPos = DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartPuzzlePos;
                    Vector2Int toPos = DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartPuzzlePos;
                    fromPos += new Vector2Int(j, i);
                    toPos += new Vector2Int(1, 0);
                    this.SwapTiles(fromPos, toPos);
                    break;
                }
            }
        }
    }

    public void MovePuzzleSortTile(Player player, Vector2Int offsetPlayer, Tilemap puzzleSortTilemap)
    {
        Vector2Int offset = player.GetCurrentPos() - DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartGroundPos;
        Vector2Int puzzlePos1 = DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartPuzzlePos;
        puzzlePos1 += offset;

        offset = (player.GetCurrentPos() - offsetPlayer) - DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartGroundPos;
        Vector2Int puzzlePos2 = DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartPuzzlePos;
        puzzlePos2 += offset;

        // Swap Tiles
        this.SwapTilesEffect(puzzlePos1, puzzlePos2);

        // Swap result
        puzzlePos1 = puzzlePos1 - DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartPuzzlePos;
        puzzlePos2 = puzzlePos2 - DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartPuzzlePos;
        this.SwapResult(new Vector2Int(puzzlePos1.y, puzzlePos1.x), new Vector2Int(puzzlePos2.y, puzzlePos2.x));
    }

    private void SwapTilesEffect(Vector2Int fromPos, Vector2Int toPos)
    {
        Vector3Int tilePosFrom = new Vector3Int(fromPos.x, fromPos.y, 0);
        Vector3Int tilePosTo = new Vector3Int(toPos.x, toPos.y, 0);

        Tilemap puzzleSortTilemap = SlideController.Instance.puzzleSortTilemap;
        TileBase tileFrom = puzzleSortTilemap.GetTile(tilePosFrom);
        TileBase tileTo = puzzleSortTilemap.GetTile(tilePosTo);

        Vector3 worldPosFrom = puzzleSortTilemap.GetCellCenterWorld(tilePosFrom);
        Vector3 worldPosTo = puzzleSortTilemap.GetCellCenterWorld(tilePosTo);

        GameObject goFrom = CreateTileOnject(tileFrom, worldPosFrom);
        GameObject goTo = CreateTileOnject(tileTo, worldPosTo);

        puzzleSortTilemap.SetTile(tilePosFrom, null);
        puzzleSortTilemap.SetTile(tilePosTo, null);

        Sequence seq = DOTween.Sequence();

        seq.Join(goFrom.transform.DOMove(worldPosTo, 0.1f).SetEase(Ease.InOutSine));
        seq.Join(goTo.transform.DOMove(worldPosFrom, 0.1f).SetEase(Ease.InOutSine));
        
        seq.OnComplete(() =>
        {
            goFrom.transform.DOScale(1.3f, 0.13f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                puzzleSortTilemap.SetTile(tilePosFrom, tileTo);
                puzzleSortTilemap.SetTile(tilePosTo, tileFrom);
                Destroy(goFrom);
                Destroy(goTo);
            });
            
        });
    }

    private GameObject CreateTileOnject(TileBase tile, Vector3 position)
    {
        GameObject go = new GameObject("TileEffectForPuzzleSort");
        SpriteRenderer sR = go.AddComponent<SpriteRenderer>();
        sR.sprite = (tile as Tile).sprite;
        sR.sortingOrder = 100;
        go.transform.position = position;
        return go;
    }

    private void SwapTiles(Vector2Int fromPos, Vector2Int toPos)
    {
        Vector3Int tilePosFrom = new Vector3Int(fromPos.x, fromPos.y, 0);
        Vector3Int tilePosTo = new Vector3Int(toPos.x, toPos.y, 0);

        Tilemap puzzleSortTilemap = SlideController.Instance.puzzleSortTilemap;
        TileBase tileFrom = puzzleSortTilemap.GetTile(tilePosFrom);
        TileBase tileTo = puzzleSortTilemap.GetTile(tilePosTo);

        puzzleSortTilemap.SetTile(tilePosFrom, tileTo);
        puzzleSortTilemap.SetTile(tilePosTo, tileFrom);
    }


    private void SwapResult(Vector2Int pos1, Vector2Int pos2)
    {
        int temp = this._result[pos1.x][pos1.y];
        this._result[pos1.x][pos1.y] = this._result[pos2.x][pos2.y];
        this._result[pos2.x][pos2.y] = temp;
    }

    public bool CheckResult(Player player)
    {
        for (int i = 0; i < _puzzleSize.x; i++)
        {
            for (int j = 0; j < _puzzleSize.y; j++)
            {
                if (this._result[i][j] != i * _puzzleSize.y + j)
                {
                    return false;
                }
            }
        }
        this.MovePlayer(player, DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].PortEndPos);
        return true;
    }

    public bool CheckPlayerInPuzzleSort(Player player)
    {
        if (Index < 0) return false;
        if (player.GetCurrentPos().x >= DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartGroundPos.x &&
            player.GetCurrentPos().x <= DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].EndGroundPos.x &&
            player.GetCurrentPos().y >= DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].StartGroundPos.y &&
            player.GetCurrentPos().y <= DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].EndGroundPos.y
            )
        {
            return true;
        }
        return false;
    }

    public bool CheckPortPos(Player player)
    {
        Vector2Int portPos = DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].PortStartPos;
        Vector2Int playerPos = player.GetCurrentPos();
        if (portPos != playerPos)
        {
            return false;
        }
        this.MovePlayer(player, DataManager.Instance.PuzzleSortLevelData.ListPuzzleSortData[Index].PlayerStartPos);
        return true;
    }

    public void MovePlayer(Player player, Vector2Int newPlayerPos)
    {
        Vector3Int playerGridPos = new Vector3Int(newPlayerPos.x, newPlayerPos.y, 0);
        Vector3 worldPos = SlideController.Instance.groundTilemap.GetCellCenterWorld(playerGridPos);
        Debug.Log("Move Player to: " + newPlayerPos + " World Pos: " + worldPos);
        player.Teleport(newPlayerPos, worldPos);
    }
}
