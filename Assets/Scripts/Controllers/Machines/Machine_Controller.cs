using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MachineCondition {Hopeless, Down, Decayed, OK, Good, Pristine}
public class Machine_Controller : MonoBehaviour {
    [HideInInspector]
    public MachineCondition machineCondition {get; protected set;}
    public string machineName = "Machine";
    int tileWidth = 1;
    int tileHeight = 2;
    public ShipSystemType shipSystemsControlled {get; protected set;}
    Vector3Int worldPosition;
    public TileData baseTile;
    ShipManager ship_Controller;
    List<TileData> neighborTiles;
    float efficiencyRate = 1;
    MiniGame machineMiniGame;
    public void InitData(string _name, int _tileWidth, int _tileHeight,ShipSystemType _systemsControlled, float _efficiency){
        machineName = _name;
        tileWidth = _tileWidth;
        tileHeight = _tileHeight;
        shipSystemsControlled = _systemsControlled;
        efficiencyRate = _efficiency;   
    }

    public void InitMachine(TileData tile, ShipManager ship){
        if (tile.AddMachine(this) == false)
            return;
        machineCondition = MachineCondition.OK;
        baseTile = tile;
        worldPosition = tile.worldPos;
        ship_Controller = ship;
        neighborTiles = new List<TileData>();
        if (tileWidth > 1 || tileHeight > 1){
            if (baseTile == null){
                return;
            }
            for(int x = 0; x < tileWidth; x++){
                    for(int y = 0; y < tileHeight; y++){
                        TileData neighbor = TileManager.instance.GetTile(baseTile.X + x, baseTile.Y + y);
                        if (neighbor != null && neighbor != baseTile){
                            if (neighbor.AddMachine(this) == true){
                                neighborTiles.Add(neighbor);
                                Debug.Log("machine added to neighbor at " + neighbor.worldPos);
                            }
                        }
                    }
            }

           BoxCollider2D collider2D = GetComponentInChildren<BoxCollider2D>();
           collider2D.offset = new Vector2(tileWidth > 1 ? 1 : 0.5f, 0.5f);
           collider2D.size  = new Vector2(tileWidth, 1);
           SpriteRenderer shadowSR = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
           shadowSR.size = new Vector2(tileWidth, 1);

           machineMiniGame = GetComponentInChildren<MiniGame>();
        } 
        
    }
    public bool CanUse(){
        if ((int)machineCondition < 2){
            return false;
        }
        return true;
    }
    public void UseMachine(){
        
        // Play animation of machine being used

        float roll = Random.Range (1, 100);
        if (roll <= efficiencyRate){
            // Machine does NOT decay
            return;
        }

        DecayCondition();
    
    }
    public void TryRepair(GameObject gameObject){
        // Start mini game ui
        machineMiniGame.gameObject.SetActive(true);
        machineMiniGame.StartMiniGame(machineCondition);
    }
    void RepairCondition(){
        // called if mini game was succesful
    }
    public void DecayCondition(){
        int curCondition = (int)machineCondition;
        if (curCondition <= 0){
            return;
        }
        machineCondition = (MachineCondition) curCondition - 1;
    }

    public void DestroyMachine(){
        if (baseTile != null){
			if (baseTile.RemoveMachine() == true){
				Debug.Log(machineName + " removed from tile");
			}
		}
        if (neighborTiles != null && neighborTiles.Count > 0){
            foreach(TileData tile in neighborTiles){
                tile.RemoveMachine();
            }
        }
		// Pool this machine's gameobject
    }
    void OnDisable(){
		DestroyMachine();
	}
}
