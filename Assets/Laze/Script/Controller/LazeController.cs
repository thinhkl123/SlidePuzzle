using CustomUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LazeController : SingletonMono<LazeController>
{
    public Tilemap LazeTilemap;

    public int Index = -1;
    private TileBase _lazeTilePrefab;
    private LazeSO _lazeData;
    private List<Vector2Int> _lazePostList;
    private List<Direction> _lazeDirectionList;
    private List<List<Vector3Int>> _listLight;

    public void SetLaze(int index, Player player)
    {
        if (index < 1)
        {
            this.Index = -1;
            return;
        }
        this.Index = index - 1;
        this._lazeData = DataManager.Instance.LazeData;
        Vector3Int pos3Prefab = new Vector3Int(this._lazeData.PosPrefab.x, this._lazeData.PosPrefab.y, 0);
        this._lazeTilePrefab = this.LazeTilemap.GetTile(pos3Prefab);
        this._lazePostList = new List<Vector2Int>();
        this._lazeDirectionList = new List<Direction>();
        foreach (var laze in this._lazeData.ListLazeLevel[Index].LazeList)
        {
            this._lazePostList.Add(laze.Position);
            this._lazeDirectionList.Add(laze.Direction);
        }
        this._listLight = new List<List<Vector3Int>>();
        for (int i = 0; i < this._lazePostList.Count; ++i)
        {
            this._listLight.Add(new List<Vector3Int>());
        }   
        this.SetLazes(player);
    }

    public void SetLazes(Player player)
    {
        Debug.Log("SetLazes");
        if (this._listLight.Count != 0)
        {
            for (int i = 0; i < this._lazeDirectionList.Count; ++i)
            {
                if (this._listLight.Count != 0)
                {
                    this.SetNullLight(i);
                }
            }
        }

        for (int i = 0; i < this._lazeDirectionList.Count; ++i)
        {
            Vector2Int pos2 = this._lazePostList[i];
            Direction direction = this._lazeDirectionList[i];
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
            if (this._listLight.Count <= 0)
            {
                this._listLight.Add(new List<Vector3Int>());
            }
            else
            {
                this._listLight[i] = this.SetLaze(pos2, offset, player);
            }
                
        }
    }

    public void SetNullLight(int index)
    {
        if (this._listLight[index] == null) {
            return;
        }

        for (int i = 0; i < this._listLight[index].Count; ++i)
        {
            this.LazeTilemap.SetTile(this._listLight[index][i], null);
        }
        this._listLight[index] = new List<Vector3Int>();
    }

    private List<Vector3Int> SetLaze(Vector2Int pos2, Vector2Int offset, Player player)
    {
        List<Vector3Int> listLight = new List<Vector3Int>();
        Vector3Int pos3 = new Vector3Int(pos2.x, pos2.y, 0);
        for (int i = 0; i <= 100; ++i)
        {
            pos2 += offset;
            Debug.Log(pos2);
            pos3 = new Vector3Int(pos2.x, pos2.y, 0);
            if (!CheckLaze(pos3, player))
            {
                return listLight;
            }
            pos3 = new Vector3Int(pos2.x, pos2.y, 0);
            listLight.Add(pos3);
            this.LazeTilemap.SetTile(pos3, this._lazeTilePrefab);
        }
        return listLight;
    }

    private bool CheckLaze(Vector3Int pos3, Player player)
    {
        if (!SlideController.Instance.groundTilemap.HasTile(pos3))
        {
            return false;
        }

        if (SlideController.Instance.obstacleTilemap.HasTile(pos3))
        {
            return false;
        }

        if (SlideController.Instance.itemTilemap.HasTile(pos3))
        {
            return false;
        }

        Vector2Int pos2Player = player.GetCurrentPos();
        Vector2Int pos2Laze = new Vector2Int(pos3.x, pos3.y);
        if (pos2Player == pos2Laze)
        {
            return false;
        }

        return true;
    }

    

    public bool CheckPlayerCanMove(Vector3Int posPlayer)
    {
        if (this.Index < 0)
        {
            return true;
        }
        foreach (Vector2Int pos2 in this._lazePostList)
        {
            Vector3Int pos3 = new Vector3Int(pos2.x, pos2.y, 0);
            if (posPlayer == pos3)
            {
                return false;
            }
        }
        return true;
    }
}
