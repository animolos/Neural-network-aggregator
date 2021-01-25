namespace NeuralNetworksAggregator.Application.CmdApplication
{
    public abstract class ConsoleCommand
    {
        protected ConsoleCommand(string name, string help)
        {
            Name = name;
            Help = help;;
        }

        public string Name { get; }
        public string Help { get; }
        public abstract void ExecuteAsync(string[] args);
    }
}