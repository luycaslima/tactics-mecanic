using System;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
   
    public GameObject gameObjectsGroundTileMap;
    public bool _drawConnections;
    
    public List<TileNode> path = new List<TileNode>();
    public Dictionary<Vector3,TileNode> Tiles{get; private set;}

    public static event Action ResetSelectableTiles;

    public void Init(){
         //Pega todos os gameobjects de TileNodes
        var tilesObjects = gameObjectsGroundTileMap.GetComponentsInChildren<TileNode>();
        Tiles = GetTiles(tilesObjects); //Inicializa os tiles e entidades sobre ele e Retorna o dicionario de tiles 
        foreach(var tile in tilesObjects) tile.FindNeighbors(); // Mandar todos saberem quem são seus vizinhos
    }

    public Dictionary<Vector3,TileNode> GetTiles(TileNode[] tiles) {
        Dictionary<Vector3,TileNode> grid = new Dictionary<Vector3, TileNode>();
        foreach(var tile in tiles){
            tile.Init();
            grid.Add(tile.Coordinates.Position, tile);
        }
        return grid;
    }
    
    public void InvokeResetSelectableTiles(){
        ResetSelectableTiles?.Invoke();
    }

    public void GetTilesInRange(BattleUnit unit){
        InvokeResetSelectableTiles();

        List<TileNode> inRange = new List<TileNode>();
        List<TileNode> toVisit = new List<TileNode>() {unit.CurrentTile};

        int count = 0;
       
        while(count < unit.maxDistance){
            var neighborTiles = new List<TileNode>();
            
            foreach(var tile in toVisit){
                neighborTiles.AddRange(tile.Neighbors);
                tile.SetIsSelectable(true);
            }

            inRange.AddRange(neighborTiles);
            toVisit = neighborTiles;
            count++;
        }
    }

    //Pega o tile na posição dita no dicionário (usado por todos os Tilenodes saberem seu vizinho)
    public TileNode GetNodeAtPosition(Vector3 pos) => Tiles.TryGetValue(pos, out var tile) ? tile: null;

    public void SetBestRouteForTheActor(BattleUnit unit, TileNode target){
        path = new List<TileNode>();
        path = PathFinding.FindPath(unit.CurrentTile, target); 
        if (path == null) throw new Exception();
    }
    

    private void OnDrawGizmos() {
            if (!Application.isPlaying || !_drawConnections) return;
            Gizmos.color = Color.red;
            if (path == null) return;
            foreach(var tile in path){
                Gizmos.DrawLine(tile.Coordinates.Position + new Vector3(0,1.2f,0) , tile.Connection.Coordinates.Position + new Vector3(0,1.2f,0));
            }
    }


}
