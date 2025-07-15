using CustomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LazeController : SingletonMono<LazeController>
{
    [Header(" Information ")]
    public Tilemap LazeTilemap; 
    public int Index = -1;
    public Vector2Int pos2Player;

    [Header(" Tile Prefab ")]
    private Tile _lightTile;
    private Tile _lockTile;

    [Header(" Get Data ")]
    public List<Vector2Int> LazePostList;
    private LazeSO _lazeData;
    private List<Direction> _lazeDirectionList;
    private List<Vector2Int> _lazeLockPosition;

    [Header(" Temp Data ")]
    private List<Vector3Int> _lazeLockPosTmp;
    private List<List<Vector3Int>> _listLight;

    public void SetInitLaze(int index)
    {
        if (index < 1)
        {
            this.Index = -1;
            return;
        }
        this.Index = index - 1;

        this._lazeData = DataManager.Instance.LazeData;
        this.LazePostList = new List<Vector2Int>();
        this._lazeDirectionList = new List<Direction>();
        this._lazeLockPosition = new List<Vector2Int>();

        _lightTile = ScriptableObject.CreateInstance<Tile>();
        _lightTile.sprite = this._lazeData.LightSprite;
        _lockTile = ScriptableObject.CreateInstance<Tile>();
        _lockTile.sprite = this._lazeData.LockSprite;
        
        foreach (var laze in this._lazeData.ListLazeLevel[Index].LazeList)
        {
            this.LazePostList.Add(laze.Position);
            this._lazeDirectionList.Add(laze.Direction);
            this._lazeLockPosition.Add(laze.LockPosition);
        }

        this._listLight = new List<List<Vector3Int>>();
        for (int i = 0; i < this.LazePostList.Count; ++i)
        {
            this._listLight.Add(new List<Vector3Int>());
        }
        this.SetLazes();
    }

    public void SetLazes()
    {
        // Null Lights
        this.SetNullLights();

        // Set Lock
        this._lazeLockPosTmp = new List<Vector3Int>();
        for (int i = 0; i < this._lazeDirectionList.Count; ++i)
        {
            Vector2Int pos2 = this.LazePostList[i];
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
            this.SetLaze(pos2, offset, i, false);
        }
        this.SetTileLock();

        // Set Laze
        for (int i = 0; i < this._lazeDirectionList.Count; ++i)
        {
            Vector2Int pos2 = this.LazePostList[i];
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
                this._listLight[i] = this.SetLaze(pos2, offset, i);
            }
        }
    }

    private List<Vector3Int> SetLaze(Vector2Int pos2, Vector2Int offset, int index, bool isSetTile = true)
    {
        List<Vector3Int> listLight = new List<Vector3Int>();
        Vector3Int pos3 = new Vector3Int(pos2.x, pos2.y, 0);
        for (int i = 0; i <= 100; ++i)
        {
            pos2 += offset;
            pos3 = new Vector3Int(pos2.x, pos2.y, 0);
            if (!CheckLaze(pos3, index) || this.LazePostList.Contains(pos2))
            {
                return listLight;
            }
            pos3 = new Vector3Int(pos2.x, pos2.y, 0);
            if (isSetTile)
            {
                listLight.Add(pos3);
                this.LazeTilemap.SetTile(pos3, this._lightTile);
            }

        }
        return listLight;
    }

    public void SetNullLights()
    {
        // Set Null Light
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
    }

    private void SetNullLight(int index)
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

    private bool CheckLaze(Vector3Int pos3, int i)
    {
        Vector2Int pos2Laze = new Vector2Int(pos3.x, pos3.y);
        Vector3Int pos3Lock = new Vector3Int(0, 0, 0);
        if (pos2Player == pos2Laze)
        {
            pos3Lock = new Vector3Int(this._lazeLockPosition[i].x, this._lazeLockPosition[i].y, 0);
            this._lazeLockPosTmp.Add(pos3Lock);
            return false;
        }

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

        return true;
    }

    public bool CheckPlayerCanMove(Vector3Int posPlayer)
    {
        if (this.Index < 0)
        {
            return true;
        }
        foreach (Vector2Int pos2 in this.LazePostList)
        {
            Vector3Int pos3 = new Vector3Int(pos2.x, pos2.y, 0);
            if (posPlayer == pos3)
            {
                return false;
            }
        }
        return true;
    }

    private void SetTileLock()
    {
        foreach (Vector2Int pos2 in this._lazeLockPosition)
        {
            Vector3Int pos3 = new Vector3Int(pos2.x, pos2.y, 0);
            SlideController.Instance.obstacleTilemap.SetTile(pos3, null);
        }

        foreach (Vector3Int pos3 in this._lazeLockPosTmp)
        {
            SlideController.Instance.obstacleTilemap.SetTile(pos3, this._lockTile);
        }
    }
}
