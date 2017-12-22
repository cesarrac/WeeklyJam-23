using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MachineCondition {Hopeless, Down, Decayed, OK, Good, Pristine}
public class Machine_Controller : MonoBehaviour {
    public MachineCondition machineCondition {get; protected set;}
    public string machineName = "Machine";
    int tileWidth = 1;
    int tileHeight = 2;
    public ShipSystemType shipSystemsControlled {get; protected set;}
    Vector3Int worldPosition;
    public Tile_Data baseTile;
    ShipManager shipManager;
    List<Tile_Data> neighborTiles;
    float efficiencyRate = 1;
    public MiniGameDifficulty repairDifficulty {get; protected set;}
    public BoxCollider2D collidable;
    public SpriteRenderer shadowRenderer;
    public void InitData(string _name, Sprite machineSprite, int _tileWidth, int _tileHeight,ShipSystemType _systemsControlled, float _efficiency, MiniGameDifficulty _repairDifficulty){
        machineName = _name;
        tileWidth = _tileWidth;
        tileHeight = _tileHeight;
        shipSystemsControlled = _systemsControlled;
        efficiencyRate = _efficiency;   
        repairDifficulty = _repairDifficulty;
        GetComponent<SpriteRenderer>().sprite = machineSprite;
    }

    public void InitMachine(Tile_Data tile, ShipManager ship){
        if (tile.AddMachine(this) == false)
            return;
        machineCondition = MachineCondition.OK;
        gameObject.name = machineName;
        baseTile = tile;
        worldPosition = tile.worldPos;
        shipManager = ship;
        neighborTiles = new List<Tile_Data>();
        if (tileWidth > 1 || tileHeight > 1){
            if (baseTile == null){
                return;
            }
            for(int x = 0; x < tileWidth; x++){
                    for(int y = 0; y < tileHeight; y++){
                        Tile_Data neighbor = TileManager.instance.GetTile(baseTile.X + x, baseTile.Y + y);
                        if (neighbor != null && neighbor != baseTile){
                            if (neighbor.AddMachine(this) == true){
                                neighborTiles.Add(neighbor);
                                //Debug.Log("machine added to neighbor at " + neighbor.worldPos);
                            }
                        }
                    }
            }

           collidable.offset = new Vector2(tileWidth > 1 ? 1 : 0.5f, 0.5f);
           collidable.size  = new Vector2(tileWidth, 1);
           /* interactable.offset = new Vector2(tileWidth > 1 ? 1 : 0.5f, tileHeight > 1 ? 1 : 0.5f);
           interactable.size  = new Vector2(tileWidth, tileHeight); */
           shadowRenderer.size = new Vector2(tileWidth, 1);
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
    public bool Interact(GameObject user){
        if (shipManager.SystemInteract(shipSystemsControlled, user) == true){
            // animate machine to show it being interfaced
            return true;
        }
        return false;
    }
    public void TryRepair(){
        // Start mini game ui
        MiniGameManager.instance.StartMiniGame(this);
    }
    public void RepairCondition(){
        // called if mini game was succesful
        // RETURN if already at MAX condition
        if ((int)machineCondition >= System.Enum.GetValues(typeof(MachineCondition)).Length)
            return;
        int curCondition = (int)machineCondition;
        
        machineCondition = (MachineCondition) curCondition + 1;
        Debug.Log("MACHINE REPAIRED! " + machineCondition);
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
            foreach(Tile_Data tile in neighborTiles){
                tile.RemoveMachine();
            }
        }
		// Pool this machine's gameobject
    }
    void OnDisable(){
		DestroyMachine();
	}
}
