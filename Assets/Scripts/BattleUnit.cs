using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum Team{
    RED,
    BLUE
}

//This class only exist to load a entity and represent their actions on the screen

public class BattleUnit : MonoBehaviour
{
    public float speedInitiave = 10f;
    public Team teamType = Team.BLUE;

    public int maxDistance = 3; 
    
    [SerializeField] 
    private bool IsMyTurn;
    
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

    public void SetIsMyTurn(bool value){
        this.IsMyTurn = value;
    }

    private void EndAction(){ 
        this.SetIsMyTurn(false);
        EndOfAction?.Invoke();
    }

    #region  ENTITY_ACTIONS
    private IEnumerator IniatePathMovement(){

        this.CurrentTile.isWalkable = true; 
        for(var tile = BattleManager.Instance.mapManager.path.Count - 1 ; tile >= 0; tile--) {
            MoveIE =  StartCoroutine(MoveToTile(BattleManager.Instance.mapManager.path[tile]));
            yield return MoveIE;
        }
        BattleManager.Instance.mapManager.path[0].SetBattleUnitInThisTile(this);
        BattleManager.Instance.mapManager.path.Clear();

        this.IsMovingUnit = false;
        EndAction();
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

    //TODO not use this courotine, make one for the movement of the camera that when centers show the menu again
    //This is only a workaround for problem below 
    //Hidemenu in BattleManager needs some interval to show again because it will activate almost simultaneously with showMenu when call wait
    private IEnumerator Wait(){
        yield return new WaitForSeconds(.25f);
        EndAction();
    }
   
    public void MoveAction(){
        if(!IsMovingUnit && IsMyTurn){
            IsMovingUnit = true;
            StartCoroutine(IniatePathMovement());   
        }
    }

    //Definir essas funções numa classe de Entidade => Monster, Summonner
    public void WaitAction(){
        StartCoroutine(Wait());
       //EndAction();
       
    }

    public void AtackAction(){

    }

    public void SkillAction(){

    }
    public void SummonAction(){

    }

    #endregion

}
