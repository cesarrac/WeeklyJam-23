using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class TriggerTile : MonoBehaviour {
   // public UnityEvent onTriggerEvent;
    public AreaID areaTO;
    void GoToArea(){
        Game_LevelManager.instance.ChangeArea(areaTO);
    }

    void OnTriggerEnter2D(Collider2D collider){
        GoToArea();
    }
}