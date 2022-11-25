using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    //Carrega todos os dados de um determinado personagem aqui para mostrar na tela
    //Existe unicamente pra representar as ações e guardar sua direção e posição
    //Avisar ao tile atual que ele não é walkable
    //TODO quando a entidade andar pelo cenário, limpar o 'Connection' do tile anterior
    public float moveSpeed = 7f;
    public TileNode CurrentTile {get; private set;}
    public bool IsMovingUnit {get; private set;}
    //public List<TileNode> tilesInRange = new List<TileNode>();
    private Coroutine MoveIE;
    

    void Start(){
        SetIsMovingUnit(false);
    }

    public void SetCurrentTilePosition(TileNode tileNode){
        CurrentTile = tileNode;
        MapManager.Instance.entity = this; //Somente para debug , num futuro mandar adicionar numa lista do manager
    }

    //TODO refatorar daqui para baixo
    //Criar classe de entidade possui essa funçao a ser sobrecarregada
    public virtual void GetTilesInRange(TileNode origin, int range){
        
    }

    public void SetIsMovingUnit(bool value){
        IsMovingUnit = value;
    }

    IEnumerator Move(){
        for(var tile = MapManager.Instance.path.Count - 1 ; tile >= 0; tile--) {
            MoveIE =  StartCoroutine(MoveToTile(MapManager.Instance.path[tile]));
            yield return MoveIE;
        }
        CurrentTile = MapManager.Instance.path[0]; //ultimo tile é o primeiro na lista
        MapManager.Instance.path.Clear();
        SetIsMovingUnit(false);
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
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation,Time.deltaTime *25f );
            yield return null;

        }
    }


    void Update(){
        if(Input.GetButtonDown("Fire1") && !IsMovingUnit ){
            SetIsMovingUnit(true);
            StartCoroutine(Move());
        }
    }

}
