using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MachineCondition {Hopeless, Down, Decayed, OK, Good, Pristine}
public class Machine : MonoBehaviour {
    [HideInInspector]
    public MachineCondition machineCondition = MachineCondition.OK;
    public string machineName = "Machine";
    public int tileWidth = 1;
    public int tileHeight = 2;
    public ShipSystemType shipSystemsControlled = ShipSystemType.None;
    [HideInInspector]
    public Vector3Int worldPosition;
    public TileData baseTile;
    [HideInInspector]
    public ShipManager ship_Controller;

    List<TileData> neighborTiles;

    public virtual void InitMachine(TileData tile, ShipManager ship){
        if (tile.AddMachine(this) == false)
            return;
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
        } 
        
    }
    public virtual bool CanUse(){
        if ((int)machineCondition < 2){
            return false;
        }
        return true;
    }
    public virtual void UseMachine(){
    
    }
 /*    public virtual bool TryRepair(){

    } */
    public virtual void RepairCondition(){
    
    }
    public virtual void DecayCondition(){
        int curCondition = (int)machineCondition;
        if (curCondition <= 0){
            return;
        }
        machineCondition = (MachineCondition) curCondition - 1;
    }

    public virtual void DestroyMachine(){
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
}
