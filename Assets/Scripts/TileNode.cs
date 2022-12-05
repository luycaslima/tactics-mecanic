using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TileNode : MonoBehaviour
{
    //TODO Criar alguma representação visual no editor do objeto a ser spawnado  ou um tile especifico pra isso (herda essa classe)
    public bool isWalkable;
    public bool IsSelectable {get; private set;} 
    public GameObject entityToSpawn;
    [SerializeField] private GameObject selectableQuad; 

    public BattleUnit WhoIsAtThisPosition {get; private set;}

    private static readonly List<Vector3> PossibleMovabletDir = new List<Vector3>() {
        new Vector3(1,0,0),new Vector3(-1,0,0), new Vector3(0,0,1),new Vector3(0,0,-1)
    };
    public List<TileNode> Neighbors ;//{ get; protected set; }
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
    

    public static event Action<TileNode> OnClickTile;

    void OnDestroy() =>  MapManager.ResetSelectableTiles -= ResetSelectableTile;

    void Start(){ 
        MapManager.ResetSelectableTiles += ResetSelectableTile;
    }

    public void Init(){
        Coordinates.Position = transform.position;
        SpawnEntity();
    }
    

    //TODO For some weird reason, when beneath -4 in the axis Z, the tiles can't find all of the neighbours
    public void FindNeighbors(){
        Neighbors = new List<TileNode>();
        foreach (var tile in PossibleMovabletDir.Select(dir => BattleManager.Instance.mapManager.GetNodeAtPosition(Coordinates.Position + dir)).Where(tile => tile != null)) {
            Neighbors.Add(tile);
        }
    }
    
    public void SetIsSelectable(bool value){
        selectableQuad.SetActive(value);
        IsSelectable = value;
    } 
    

    private void ResetSelectableTile(){
        if (!IsSelectable) return;
        selectableQuad.SetActive(false);
        IsSelectable = false;
    }

    void onMouseOver(){
        if(!IsSelectable) return;
        //TODO make glow when hover
    }
    void OnMouseDown(){
        //TODO change color when clicked and only invoke if is selectable
        if(!isWalkable || !IsSelectable) return;
        OnClickTile?.Invoke(this);
    }

    public void SetBattleUnitInThisTile(BattleUnit unit){
        WhoIsAtThisPosition = unit;
        unit.SetCurrentTilePosition(this);
        isWalkable = false; //change this if troops can pass eachother
    }

    private void SpawnEntity(){
       if(entityToSpawn == null) return;
        
        var entity = Instantiate(entityToSpawn);
        entity.transform.position = Coordinates.Position + new Vector3(0,1.5f,0);
        entity.transform.rotation = Quaternion.identity;
        
        var battleUnit = entity.GetComponent<BattleUnit>();
        if (battleUnit != null) SetBattleUnitInThisTile(battleUnit);   
    }

}


public struct Coords {
    //Distancie between two tiles
    public float GetDistance(Coords target){
        var dist = new Vector3Int(Mathf.Abs((int)Position.x - (int)target.Position.x), 0,Mathf.Abs((int)Position.z - (int)target.Position.z));
        var min = Mathf.Min(dist.x, dist.z);
        return  min;
    }
    public Vector3 Position{get; set;}
}