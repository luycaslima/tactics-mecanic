using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //Singleton
    public static MapManager Instance;
    public GameObject gameObjectsGroundTileMap;
    public bool _drawConnections;
    
    //PATHFINDING
    public BattleUnit entity;
    public List<TileNode> path = new List<TileNode>();
    private void OnDestroy() => TileNode.OnHoverTile -= OnTileHover;
   

    public Dictionary<Vector3,TileNode> Tiles{get; private set;}
    private void Awake() => Instance = this;
    // Start is called before the first frame update
    void Start()
    {
        //Pega todos os gameobjects de TileNodes
        TileNode[] tilesObjects = gameObjectsGroundTileMap.GetComponentsInChildren<TileNode>();
        Tiles = GetTiles(tilesObjects); //Inicializa os tiles e entidades sobre ele e Retorna o dicionario de tiles 
        foreach(var tile in Tiles.Values) tile.FindNeighbors(); // Mandar todos saberem quem são seus vizinhos

        TileNode.OnHoverTile += OnTileHover;
    }

    public Dictionary<Vector3,TileNode> GetTiles( TileNode[] tiles) {
        Dictionary<Vector3,TileNode> grid = new Dictionary<Vector3, TileNode>();
        foreach(var tile in tiles){
            tile.Init();
            grid.Add(tile.Coordinates.Position, tile);
        }
        return grid;
    }



    //TODO refatorar daqui para baixo
    private void OnTileHover(TileNode tile) { 
        if (entity == null || entity.IsMovingUnit) return;
        //Checar se pode mudar o path se chegar aqui com range
        path = PathFinding.FindPath(entity.CurrentTile, tile);
    }

    public TileNode GetNodeAtPosition(Vector3 pos) => Tiles.TryGetValue(pos, out var tile) ? tile: null;


    private void OnDrawGizmos() {
            if (!Application.isPlaying || !_drawConnections || entity.IsMovingUnit) return;
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
