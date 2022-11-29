using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    public GameObject gameObjectsGroundTileMap;
    public bool _drawConnections;
    
    //TODO PATHFINDING REESTRUTURAR COMO ESSE PATH É SALVO E COMO SALVAR OS TILES
    //CADA ENTIDADE SO PRECISA SABER UMA DISTANCIA ESPECIFICA
    public List<TileNode> path = new List<TileNode>();
    //private void OnDestroy() => TileNode.OnHoverTile -= OnTileHover;

    public Dictionary<Vector3,TileNode> Tiles{get; private set;}
    private void Awake() => Instance = this;


    void Start()
    {
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
            /*foreach (var tile in Tiles) {
                if (tile.Value.Connection == null) continue;
                Gizmos.DrawLine(tile.Key + new Vector3(0,1.2f,0) , tile.Value.Connection.Coordinates.Position + new Vector3(0,1.2f,0));
            }*/
            if (path == null) return;
            foreach(var tile in path){
                Gizmos.DrawLine(tile.Coordinates.Position + new Vector3(0,1.2f,0) , tile.Connection.Coordinates.Position + new Vector3(0,1.2f,0));
            }
    }


}
