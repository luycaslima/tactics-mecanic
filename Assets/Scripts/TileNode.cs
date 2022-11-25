using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TileNode : MonoBehaviour
{
    //TODO Criar alguma representação visual no editor do objeto a ser spawnado 

    public bool isWalkable;
    public GameObject entityToSpawn;
    

    public static readonly List<Vector3> PossibleMovabletDir = new List<Vector3>() {
        new Vector3(1,0,0),new Vector3(0,0,1), new Vector3(-1,0,0),new Vector3(0,0,-1)
    };
    public List<TileNode> Neighbors { get; private set; }
    public Coords Coordinates;


    #region PathFinding
    public TileNode Connection {get; private set;}
    public float H {get; private set; }
    public float G {get; private set;}
    public float F => G + H;

    public void SetConnection(TileNode node) => Connection = node;
    public void setH ( float h ) => H = h;
    public void setG( float g ) => G = g;
    
    #endregion

    public static event Action<TileNode> OnHoverTile;

    public void Init(){
        Coordinates.Position = transform.position; //TODO descobrir como retirar o offset 0.5 quando não corrige isso no tileset
        SpawnEntity();
    }

    public void FindNeighbors(){
        Neighbors = new List<TileNode>();
        //Adiciona seus vizinhos para uma lista
        foreach (var tile in PossibleMovabletDir.Select(dir => MapManager.Instance.GetNodeAtPosition(Coordinates.Position + dir)).Where(tile => tile != null)) {
            Neighbors.Add(tile);
        }
    }

    
    void OnMouseOver(){
        if (isWalkable == false ) return;
        OnHoverTile?.Invoke(this);
    }


    private void SpawnEntity(){
        if(entityToSpawn != null){
            var entity = Instantiate(entityToSpawn);
            entity.transform.position = Coordinates.Position + new Vector3(0,1.5f,0);
            entity.transform.rotation = Quaternion.identity;
            var battleUnit = entity.GetComponent<BattleUnit>();
            
            if (battleUnit != null){
                battleUnit.SetCurrentTilePosition(this);
            }
        }
    }
}

public struct Coords {
    //Distancia entre um tile a outro 
    public float GetDistance(Coords target){
        var dist = new Vector3Int(Mathf.Abs((int)Position.x - (int)target.Position.x), 0,Mathf.Abs((int)Position.z - (int)target.Position.z));
        return Mathf.Min(dist.x, dist.y);
    }
    public Vector3 Position{get; set;}
}