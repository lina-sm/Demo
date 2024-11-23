namespace ProductionStationSimulator
{
    public class ControlSystem
    {
        public event EventHandler EmptyStatusChanged;
        public event EventHandler FullStatusChanged;

        public ControlSystem() { }

        public void RaiseAnEmptyEvent(bool isEmpty)
        {
            if (isEmpty)
            {
                // TODO
                Thread.Sleep(3000);
                EmptyStatusChanged?.Invoke(this, EventArgs.Empty);
            }

        }

        public void RaiseAnFullEvent(bool isFull)
        {
            if (isFull)
            {
                // TODO
                Thread.Sleep(3000);
                FullStatusChanged?.Invoke(this, EventArgs.Empty);
            }

        }

    }
}
