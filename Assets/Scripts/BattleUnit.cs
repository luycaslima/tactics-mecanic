using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum Team{
    RED,
    BLUE
}
//Carrega todos os dados de um determinado personagem aqui para mostrar na tela
public class BattleUnit : MonoBehaviour
{
    //Existe unicamente pra representar as ações e guardar sua direção e posição
    //Avisar ao tile atual que ele não é walkable
    public float speedInitiave = 10f;
    public Team teamType = Team.BLUE;

    private int maxDistance = 3; [SerializeField]
    public bool IsMyTurn;
    
    public TileNode CurrentTile {get; private set;}

    public static event Action<BattleUnit> PlaceBattleUnit; // Sinal pro battle manager que foi invocado
    public static event Action EndOfAction; //Sinal de que a unidade terminou seu turno
    public static event Action<BattleUnit> UnitDied; //Sinal de que a unidade morreu

    // Movement of the Unit
    public float moveSpeed = 7f;
    public bool IsMovingUnit {get; private set;}
    private Coroutine MoveIE;

    void Start(){
        PlaceBattleUnit?.Invoke(this);
        this.IsMovingUnit = false;
        this.IsMyTurn = false;
    }

    public void SetCurrentTilePosition(TileNode tileNode){
        this.CurrentTile = tileNode;
    }


    //Tornar sobrecarregavel pra variar a distancia de movimento de acordo com o personagem?
    public void GetTilesInRange(){
        
    }

    public void SetIsMyTurn(bool value){
        this.IsMyTurn = value;
    }

    IEnumerator IniateMovement(){

        this.CurrentTile.isWalkable = true; 
        for(var tile = MapManager.Instance.path.Count - 1 ; tile >= 0; tile--) {
            MoveIE =  StartCoroutine(MoveToTile(MapManager.Instance.path[tile]));
            yield return MoveIE;
        }
        MapManager.Instance.path[0].SetBattleUnitInThisTile(this);
        MapManager.Instance.path.Clear();

        this.IsMovingUnit = false;
        this.SetIsMyTurn(false);
        EndOfAction?.Invoke();
    }

    private IEnumerator MoveToTile(TileNode tile){
        float step = moveSpeed * Time.deltaTime;
        
        var target = tile.Coordinates.Position + new Vector3(0,1.5f,0);
        var distance = transform.position - target;

        var direction = distance;
        var rotation = transform.rotation;

        while (distance.magnitude > 0.0001f){
            distance = transform.position - target;
            rotation = Quaternion.LookRotation(-direction);

            transform.position = Vector3.MoveTowards(transform.position,target,step);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation,Time.deltaTime * 25f );
            yield return null;

        }
    }

    //Entity Actions abaixo
    public void SetMovement(){
        if(!IsMovingUnit && IsMyTurn){
            IsMovingUnit = true;
            StartCoroutine(IniateMovement());
            
        }
    }

}
