namespace ProductionStationSimulator
{
    public class Simulator
    {
        private bool _emptyPlaceSensor;
        private bool _fullPlaceSensor = false;
        private readonly List<string> _machines = new();
        private readonly int _processTime = 5;
        private readonly object _lock = new();
        private bool running = true;
        ControlSystem controlSystem = new ControlSystem();
       

        public Simulator()
        {

            _machines.Add("machine_1");
            _machines.Add("machine_2");
            _machines.Add("machine_3");

        }

        public void StartSimulation()
        {
            Console.WriteLine("Simulator started");
            
                SetEmptyPlaceSensor(false);


                foreach (var machine in _machines)
                {
                    Task.Run(() => RunMachine(machine));
                }
            
        }

        public void StopSimulation()
        {
            SetFullPlaceSensor(false);
            Console.WriteLine("Stopping simulation...");

        }

        public void SetEmptyPlaceSensor(bool value)
        {
            _emptyPlaceSensor = value;

            Console.WriteLine($"EmptyPlaceSensor set to: {(value ? "occupied" : "free")}");

            // notify the control system and wait for the delivery, we assume that this event is raised

            Console.WriteLine("The control system is notified that the empty place is free");
            Console.WriteLine("Wait for delivery from the control system");

            
            controlSystem.EmptyStatusChanged += (s, args) => OnEmptyPlaceSensorChanged();
            controlSystem.RaiseAnEmptyEvent(true);

        }

        public bool GetEmptyPlaceSensor() => _emptyPlaceSensor;

        public void SetFullPlaceSensor(bool value)
        {
            _fullPlaceSensor = value;
            Console.WriteLine($"FullPlaceSensor set to: {(value ? "occupied" : "free")}");

         

            if (value)
            {
                // raise an event to notify the control system that the full place is occupied

                controlSystem.FullStatusChanged += (s, args) => OnFullPlaceSensorChanged();
                controlSystem.RaiseAnFullEvent(true);
                Console.WriteLine("The control system is notified that the full place is occupied");
                Console.WriteLine("Wait for pickup from full place");

            }

        }

        public bool GetFullPlaceSensor() => _fullPlaceSensor;

        public static int StartTimer(int seconds, Action callback)
        {
            var timer = new Timer(_ => callback(), null, seconds * 1000, Timeout.Infinite);
            return timer.GetHashCode(); 
        }

        public static void KillTimer(int timerId)
        {
            Console.WriteLine($"Timer {timerId} killed.");
        }
        
        private void RunMachine(string machine)
        {

            while (running)
            {

                lock (_lock)
                {
                    if (GetEmptyPlaceSensor())
                    {
                        Console.WriteLine($"{machine} fetched the empty container.");

                        SetEmptyPlaceSensor(false);

                        StartTimer(_processTime, () => FinishProcessing(machine));
                        break;

                    }
                    else
                    {
                        Console.WriteLine($"{machine} waits until empty Place is occupied");
                        Thread.Sleep(5000);
                    }
                }
            }

        }

        private void FinishProcessing(string machine)
        {
            lock (_lock)
            {
                if (!GetFullPlaceSensor())
                {
                    Console.WriteLine($"{machine} completed delivery the goods to full place ");

                    SetFullPlaceSensor(true);
                    Task.Run(() => RunMachine(machine));

                }
                else
                {
                    Console.WriteLine($"{machine} waits until the full place is free");
                    Thread.Sleep(5000);
                }
            }
        }

        // Observer pattern Methods
        private void OnEmptyPlaceSensorChanged()
        {
            if (!GetEmptyPlaceSensor())
            {
                Console.WriteLine("Empty Place Sensor is occupied now.");
                _emptyPlaceSensor = true;

            }
        }

        private void OnFullPlaceSensorChanged()
        {
            if (GetFullPlaceSensor())
            {
                Console.WriteLine("Full goods is picked up and the fullplace sensor is free now");
                _fullPlaceSensor = false;

            }
        }
    }

}
