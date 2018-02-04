using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory_Manager : MonoBehaviour {
    public static Inventory_Manager instance {get; protected set;}
   
    private void Awake() {
        instance = this;
    }

    public Inventory LoadInventory(string owner_id){
        Item_Manager item_Manager = Item_Manager.instance;
        // Try to load and return
        SavedInventory savedInventory = JsonLoader.instance.LoadSavedInventory(owner_id);
        if (savedInventory.maxSpace > 0 && savedInventory.items != null && savedInventory.items.Length > 0){
            Inventory loadedInventory = new Inventory(savedInventory.maxSpace);
            foreach(ItemReference itemRef in savedInventory.items){
                // TODO : Load the updated stats by updating the item prototype
                //      To do this we might have to store Item stats in saved inventory with each item reference
                Item newItem = item_Manager.CreateInstance(item_Manager.GetPrototype(itemRef.itemName));
                loadedInventory.AddItem(newItem, itemRef.count);
            }
            return loadedInventory;
        }
        // if none found...
        return null;

    }

    public void SaveInventory(string owner_id, Inventory inventory){
          // Save the inventory to file
            SavedInventory sInventory = new SavedInventory();
            sInventory.maxSpace = inventory.maxSpaces;
            if (inventory.IsEmpty() == false){
                List<ItemReference> references = new List<ItemReference>();
                foreach (InventoryItem item in inventory.inventory_items)
                {
                    if (item.item == null)
                        continue;
                    ItemReference reference;
                    reference.itemName = item.item.name;
                    reference.itemType = item.item.itemType;
                    reference.count = item.count;
                    references.Add(reference);
                }
                sInventory.items = references.ToArray();
            }
            JsonWriter.WriteInventoryToJson(sInventory, owner_id);
    }
}