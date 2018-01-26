 using UnityEngine;
 public class Producer : Machine
    {
        public ProductionBlueprint[] productionBlueprints {get; protected set;}
        public bool showsGrowth {get; protected set;}
        int pProductionStage;
	    public int productionStage {get{return pProductionStage;}set{pProductionStage = Mathf.Clamp(value, -1, 4);}}
        
	    public ProductionBlueprint current_Blueprint {get; protected set;}
        protected Producer(ProducerPrototype b) : base(b)
        {
            this.productionBlueprints = b.productionBlueprints;
            this.showsGrowth = b.showsGrowth;
            this.productionStage = b.productionStage;
            if (b.curProductionName.Length > 0){
               foreach (ProductionBlueprint blueprint in this.productionBlueprints)
               {
                   if (blueprint.itemProduced.itemName == b.curProductionName){
                       this.current_Blueprint = blueprint;
                       break;
                   }
               }
            }
        }
        public static Producer CreateInstance(ProducerPrototype prototype){
            return new Producer(prototype);
        }
        public bool SetCurrentBlueprint(string keyIngredientName){
            foreach (ProductionBlueprint blueprint in productionBlueprints)
            {
                if (blueprint.keyIngredient.itemName == keyIngredientName){
                    current_Blueprint = blueprint;
                    return true;
                }
            }
            return false;
        }
        public void ResetCurrentBlueprint(){
            current_Blueprint = new ProductionBlueprint();
        }
        public void DebugSetBlueprint(){
            current_Blueprint = productionBlueprints[0];
        }
    }