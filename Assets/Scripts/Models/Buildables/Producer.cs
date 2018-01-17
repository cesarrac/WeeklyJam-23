 public class Producer : Machine
    {
        public ProductionBlueprint[] productionBlueprints {get; protected set;}
        protected Producer(ProducerPrototype b) : base(b)
        {
            this.productionBlueprints = b.productionBlueprints;
        }
        public static Producer CreateInstance(ProducerPrototype prototype){
            return new Producer(prototype);
        }
    }