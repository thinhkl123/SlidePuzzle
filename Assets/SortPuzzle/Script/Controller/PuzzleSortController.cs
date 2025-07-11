using CustomUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleSortController : SingletonMono<PuzzleSortController>
{
    [Header(" Data tmp ")]
    public PuzzleSortLevelSO PuzzleSortLevelData;

    private List<List<int>> _result;
    private Vector2Int _puzzleSize;
    private int _levelId = 0;

    private void Start()
    {
        //this.InitResult(0);
        //this.Shuffle();
    }

    public void InitResult(int levelId)
    {
        this._levelId = levelId;
        this._puzzleSize =  PuzzleSortLevelData.ListPuzzleSortData[levelId].EndGroundPos -
                            PuzzleSortLevelData.ListPuzzleSortData[levelId].StartGroundPos +
                            Vector2Int.one;
        this._result = new List<List<int>>();

        for (int i = 0; i < _puzzleSize.x; i++)
        {
            this._result.Add(new List<int>());
            for (int j = 0; j < _puzzleSize.y; j++)
            {
                this._result[i].Add(i * _puzzleSize.y + j);
            }
        }
    }

    // Not quan trọng, sau tự design map.
    private void Shuffle()
    {
        for (int i = 0; i < _puzzleSize.x; i++)
        {
            for (int j = 0; j < _puzzleSize.y; j++)
            {
                int randomI = Random.Range(0, _puzzleSize.x);
                int randomJ = Random.Range(0, _puzzleSize.y);

                // Swap Result
                this.SwapResult(new Vector2Int(i, j), new Vector2Int(randomI, randomJ));

                // Swap Tilemap
                Vector2Int fromPos = PuzzleSortLevelData.ListPuzzleSortData[_levelId].StartPuzzlePos;
                Vector2Int toPos = PuzzleSortLevelData.ListPuzzleSortData[_levelId].StartPuzzlePos;
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
                    Vector2Int fromPos = PuzzleSortLevelData.ListPuzzleSortData[_levelId].StartPuzzlePos;
                    Vector2Int toPos = PuzzleSortLevelData.ListPuzzleSortData[_levelId].StartPuzzlePos;
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
        Vector2Int offset = player.GetCurrentPos() - PuzzleSortLevelData.ListPuzzleSortData[_levelId].StartGroundPos;
        Vector2Int puzzlePos1 = PuzzleSortLevelData.ListPuzzleSortData[_levelId].StartPuzzlePos;
        puzzlePos1 += offset;

        offset = (player.GetCurrentPos() - offsetPlayer) - PuzzleSortLevelData.ListPuzzleSortData[_levelId].StartGroundPos;
        Vector2Int puzzlePos2 = PuzzleSortLevelData.ListPuzzleSortData[_levelId].StartPuzzlePos;
        puzzlePos2 += offset;

        // Swap Tiles
        this.SwapTiles(puzzlePos1, puzzlePos2);

        // Swap result
        puzzlePos1 = puzzlePos1 - PuzzleSortLevelData.ListPuzzleSortData[_levelId].StartPuzzlePos;
        puzzlePos2 = puzzlePos2 - PuzzleSortLevelData.ListPuzzleSortData[_levelId].StartPuzzlePos;
        this.SwapResult(new Vector2Int(puzzlePos1.y, puzzlePos1.x), new Vector2Int(puzzlePos2.y, puzzlePos2.x));
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
        this.MovePlayer(player, PuzzleSortLevelData.ListPuzzleSortData[0].PortEndPos);
        return true;
    }

    public bool CheckPlayerInPuzzleSort(Player player, int levelId)
    {
        if (player.GetCurrentPos().x >= PuzzleSortLevelData.ListPuzzleSortData[levelId].StartGroundPos.x &&
            player.GetCurrentPos().x <= PuzzleSortLevelData.ListPuzzleSortData[levelId].EndGroundPos.x &&
            player.GetCurrentPos().y >= PuzzleSortLevelData.ListPuzzleSortData[levelId].StartGroundPos.y &&
            player.GetCurrentPos().y <= PuzzleSortLevelData.ListPuzzleSortData[levelId].EndGroundPos.y
            )
        {
            return true;
        }
        return false;
    }

    public bool CheckPortPos(Player player)
    {
        Vector2Int portPos = PuzzleSortLevelData.ListPuzzleSortData[0].PortStartPos;
        Vector2Int playerPos = player.GetCurrentPos();
        if (portPos != playerPos)
        {
            return false;
        }
        this.MovePlayer(player, PuzzleSortLevelData.ListPuzzleSortData[0].PlayerStartPos);
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
