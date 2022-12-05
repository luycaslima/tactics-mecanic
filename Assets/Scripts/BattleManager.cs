using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum BattleStates{
    BEGIN_BATTLE,
    BEGIN_TURN,
    TEAM_TURN,
    END_BATTLE, 
    WIN,
    DEFEAT
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("Self-References")]
    [Space]
    public MapManager mapManager;
    private Camera cam;

    [Header("States of the Game")]
    [Space]
    
    [SerializeField]
    private BattleStates battleStates;
    [SerializeField]
    private Team teamTurn;

    [Header("Battle variables")]
    [Space]
    [SerializeField] private List<BattleUnit> blueTeamEntities = new List<BattleUnit>(); 
    [SerializeField] private List<BattleUnit> redTeamEntities = new List<BattleUnit>(); 
    public BattleUnit actualUnit {get; private set;}
    private Queue<BattleUnit> actQueue = new Queue<BattleUnit>(); //Controla a fila de atores no turno


    [Header("UI Events")]
    [Space]
    [SerializeField] 
    private UnityEvent showActionsMenu;
    [SerializeField] 
    private UnityEvent hideActionsMenu;


    void Awake() => Instance = this;
    void OnDestroy(){
        BattleUnit.PlaceBattleUnit -= AddBattleUnitToList;
        BattleUnit.UnitDied -= RemoveUnitOfList;
        BattleUnit.EndOfAction -= EndUnitAction;
        TileNode.OnClickTile -=  MoveBattleUnit;
    }

    void Start()
    {  
        cam = Camera.main;
        mapManager = GetComponentInChildren<MapManager>();
        mapManager.Init();

        TileNode.OnClickTile += MoveBattleUnit;
        BattleUnit.PlaceBattleUnit += AddBattleUnitToList;
        BattleUnit.UnitDied += RemoveUnitOfList;
        BattleUnit.EndOfAction += EndUnitAction;
        
        battleStates = BattleStates.BEGIN_BATTLE;

    }

    private void InitiateBattle(){
        battleStates = BattleStates.BEGIN_TURN;
        teamTurn = Team.BLUE;
    }

    private void EndBattle(){
        if ( redTeamEntities.Count == 0) battleStates = BattleStates.WIN;
        else battleStates = BattleStates.DEFEAT;
    }

    //Rearrange the queue of actors for the turn
    private void RearrangeActQueue(){
        actQueue.Clear();
        //RULE TODO- Check here if an entity was summoned this turn, and not add to the queue
        if(teamTurn == Team.RED){
            foreach(var entity in redTeamEntities) actQueue.Enqueue(entity);
        }else if(teamTurn == Team.BLUE){
            foreach(var entity in blueTeamEntities) actQueue.Enqueue(entity);
        }

        if(actQueue.Count == 0){
            battleStates= BattleStates.END_BATTLE;
        }else{
            battleStates = BattleStates.TEAM_TURN;
            BeginNextUnitAction();
        }
        //actQueue.OrderByDescending(unit => unit.speedInitiave); If arrange the queue based on a factor (Ex: Speed)
    }

    private void WaitQueueOfActors(){
        if (actQueue.Count == 0){
            teamTurn =  (teamTurn == Team.RED) ? Team.BLUE : Team.RED; 
            battleStates = BattleStates.BEGIN_TURN;
        }
    }

    private void BeginNextUnitAction(){
        actualUnit = actQueue.Peek();
        actualUnit.SetIsMyTurn(true);
        //Call here a event to change the target of the camera on the character
        showActionsMenu.Invoke();
        //mapManager.GetTilesInRange(actualUnit);
    }

#region EVENTS_FUNCTIONS
    //When unit is summoned, inkoke this function
    //Add entities in the list based on the respective team (red/enemies, blue/your party)
    private void AddBattleUnitToList(BattleUnit unit){
        if (unit.teamType == Team.BLUE){
            blueTeamEntities.Add(unit);
        }else{
            redTeamEntities.Add(unit);
        }
    }
    //When the unit dies, invoke this function
    private void RemoveUnitOfList(BattleUnit unit){
        if (unit.teamType == Team.BLUE){
            blueTeamEntities.Remove(unit);
        }else{
            redTeamEntities.Remove(unit);
        }
    }

    private void EndUnitAction(){
        actQueue.Dequeue();
        if(actQueue.Count != 0 ){
            BeginNextUnitAction();
        }
        mapManager.InvokeResetSelectableTiles();
    }

    private void MoveBattleUnit(TileNode target){
        if (actualUnit.IsMovingUnit) return;
        mapManager.SetBestRouteForTheActor(actualUnit,target);
        actualUnit.MoveAction();
    }

    #endregion



 
#region UNITY_UI_EVENTS
    public void MoveActionEvent(){
        mapManager.GetTilesInRange(actualUnit);
        hideActionsMenu.Invoke();
    }

    public void AtackActionEvent(){

    }

    public void WaitActionEvent(){
        actualUnit.WaitAction();
        hideActionsMenu.Invoke(); 
    }

#endregion
    void Update()
    {
        switch(battleStates){
            case BattleStates.BEGIN_BATTLE:
                InitiateBattle();
            break;
            case BattleStates.BEGIN_TURN:
                RearrangeActQueue();
            break;
            case BattleStates.TEAM_TURN:
                WaitQueueOfActors();
            break;
            case BattleStates.END_BATTLE:
                EndBattle();
            break;
            case BattleStates.WIN:
            break;

            case BattleStates.DEFEAT:
            break;

            default:
                throw new Exception("BATTLE STATE ERROR");

        }   
    }
}
