using CustomUtils;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleSortController : SingletonMono<PuzzleSortController>
{
    private List<List<int>> _result;
    private List<int> _shuffleList;
    private Vector2Int _puzzleSize;
    private int _index = -1;
    private Vector2Int _playerStartPos;

    public void SetPuzzleSort(int puzzleSortId)
    {
        this._index = puzzleSortId - 1;
        if (this._index < 0) return;

        Vector2Int puzzleSideTmp =  DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.EndGroundPos -
                            DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartGroundPos +
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


    private void Shuffle()
    {
        this._shuffleList = new List<int>();
        this._shuffleList = DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleShuffle.ShuffleList;

        int count = -1;
        for (int i = 0; i < _puzzleSize.x; i++)
        {
            for (int j = 0; j < _puzzleSize.y; j++)
            {
                count++;
                Vector2Int posGridShuffle = this.FindPosInResult(this._shuffleList[count]);
                this.SwapResult(new Vector2Int(i, j), posGridShuffle);

                // Swap Tilemap
                Vector2Int fromPos = DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartPuzzlePos;
                Vector2Int toPos = DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartPuzzlePos;
                fromPos += new Vector2Int(j, i);
                toPos += new Vector2Int(posGridShuffle.y, posGridShuffle.x);
                this.SwapTiles(fromPos, toPos);
            }
        }

        int valuePlayer = DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.ValuePlayer;
        Vector2Int playerGridPos = this.FindPosInResult(valuePlayer);
        this._playerStartPos = new Vector2Int(playerGridPos.y, playerGridPos.x) 
            + DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartGroundPos;
    }

    private Vector2Int FindPosInResult(int value)
    {
        for (int i = 0; i < _puzzleSize.x; i++)
        {
            for (int j = 0; j < _puzzleSize.y; j++)
            {
                if (this._result[i][j] == value)
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return Vector2Int.zero;
    }

    public void MovePuzzleSortTile(Player player, Vector2Int offsetPlayer, Tilemap puzzleSortTilemap)
    {
        Vector2Int offset = player.GetCurrentPos() - DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartGroundPos;
        Vector2Int puzzlePos1 = DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartPuzzlePos;
        puzzlePos1 += offset;

        offset = (player.GetCurrentPos() - offsetPlayer) - DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartGroundPos;
        Vector2Int puzzlePos2 = DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartPuzzlePos;
        puzzlePos2 += offset;

        // Swap Tiles
        this.SwapTilesEffect(puzzlePos1, puzzlePos2);

        // Swap result
        puzzlePos1 = puzzlePos1 - DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartPuzzlePos;
        puzzlePos2 = puzzlePos2 - DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartPuzzlePos;
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

    public bool CheckResult()
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
        return true;
    }

    public bool CheckPlayerInPuzzleSort(Player player)
    {
        if (_index < 0) return false;
        if (player.GetCurrentPos().x >= DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartGroundPos.x &&
            player.GetCurrentPos().x <= DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.EndGroundPos.x &&
            player.GetCurrentPos().y >= DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.StartGroundPos.y &&
            player.GetCurrentPos().y <= DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.EndGroundPos.y
            )
        {
            return true;
        }
        return false;
    }

    public bool CheckPortPos(Player player)
    {
        Vector2Int portPos = DataManager.Instance.PuzzleSortLevelData.PuzzleSortDataList[_index].PuzzleSort.PortStartPos;
        Vector2Int playerPos = player.GetCurrentPos();
        if (portPos != playerPos)
        {
            return false;
        }
        this.MovePlayer(player, this._playerStartPos);
        return true;
    }

    public void MovePlayer(Player player, Vector2Int newPlayerPos)
    {
        Vector3Int playerGridPos = new Vector3Int(newPlayerPos.x, newPlayerPos.y, 0);
        Vector3 worldPos = SlideController.Instance.groundTilemap.GetCellCenterWorld(playerGridPos);
        player.Teleport(newPlayerPos, worldPos);
    }

    public void Win()
    {
        Debug.Log("Win Puzzle Sort");
    }
}
