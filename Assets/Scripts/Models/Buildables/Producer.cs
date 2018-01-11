 public class Producer : Machine
    {
        public ProductionItem[] itemsProduced {get; protected set;}
        protected Producer(ProducerPrototype b) : base(b)
        {
            this.itemsProduced = b.itemsProduced;
        }
        public static Producer CreateInstance(ProducerPrototype prototype){
            return new Producer(prototype);
        }
    }