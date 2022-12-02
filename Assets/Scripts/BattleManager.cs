using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    private BattleStates battleStates;

    [SerializeField]
    private Team teamTurn;

    public static BattleManager Instance;
    public MapManager mapManager;

    //Battle variables
    private Queue<BattleUnit> actQueue = new Queue<BattleUnit>(); //Controla a fila de atores no turno
    [SerializeField] private List<BattleUnit> blueTeamEntities = new List<BattleUnit>(); 
    [SerializeField] private List<BattleUnit> redTeamEntities = new List<BattleUnit>(); 
    [SerializeField] private BattleUnit actualUnit;
    void Awake() => Instance = this;
    void OnDestroy(){ 

        BattleUnit.PlaceBattleUnit -= AddBattleUnitToList;
        BattleUnit.UnitDied -= RemoveUnitOfList;
        BattleUnit.EndOfAction -= EndUnitAction;
        TileNode.OnClickTile -=  MoveBattleUnit;
    }

    // Start is called before the first frame update
    void Start()
    {
        mapManager = GetComponentInChildren<MapManager>();
        mapManager.Init();

        TileNode.OnClickTile += MoveBattleUnit;
        BattleUnit.PlaceBattleUnit += AddBattleUnitToList;
        BattleUnit.UnitDied += RemoveUnitOfList;
        BattleUnit.EndOfAction += EndUnitAction;
        
        battleStates = BattleStates.BEGIN_BATTLE;

    }


    //Add entities in the list of enemies(red) or player(blue)
    private void AddBattleUnitToList(BattleUnit unit){
        if (unit.teamType == Team.BLUE){
            blueTeamEntities.Add(unit);
        }else{
            redTeamEntities.Add(unit);
        }
    }
    private void RemoveUnitOfList(BattleUnit unit){
        if (unit.teamType == Team.BLUE){
            blueTeamEntities.Remove(unit);
        }else{
            redTeamEntities.Remove(unit);
        }
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

    private void InitiateBattle(){
        battleStates = BattleStates.BEGIN_TURN;
        teamTurn = Team.BLUE;
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
        mapManager.GetTilesInRange(actualUnit);
    }

    private void EndUnitAction(){
        actQueue.Dequeue();
        if(actQueue.Count != 0 ){
            BeginNextUnitAction();
        }
    }

    private void MoveBattleUnit(TileNode target){
        if (actualUnit.IsMovingUnit) return;
        mapManager.SetBestRouteForTheActor(actualUnit,target);
        actualUnit.MoveAction();
    }

    // Update is called once per frame
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
            break;

        }   
    }
}
