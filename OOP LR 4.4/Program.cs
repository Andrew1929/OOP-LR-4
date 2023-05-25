using System;
using System.Collections.Generic;

namespace OOP_LR_4._4
{
    class Program
    {
        static void Main(string[] args)
        {

        }
    }
    public abstract class WorkflowStep
    {
        public Action Completed { get; internal set; }
        public string Name { get; internal set; }

        public abstract void Execute();

        internal void Start()
        {
            throw new NotImplementedException();
        }
    }
    public class CreateOrderStep : WorkflowStep
    {
        public override void Execute()
        {
            Console.WriteLine("Creating a new order...");
        }
    }

    public class ProcessPaymentStep : WorkflowStep
    {
        public override void Execute()
        {
            Console.WriteLine("Processing payment...");
        }
    }
    public class Workflow
    {
        public event EventHandler<OrderEventArgs> OrderCreated;
        public event EventHandler<OrderEventArgs> PaymentProcessed;

        public void Run()
        {
            var createOrderStep = new CreateOrderStep();
            createOrderStep.Execute();
            OnOrderCreated(new OrderEventArgs());

            var processPaymentStep = new ProcessPaymentStep();
            processPaymentStep.Execute();
            OnPaymentProcessed(new OrderEventArgs());
        }

        protected virtual void OnOrderCreated(OrderEventArgs e)
        {
            OrderCreated?.Invoke(this, e);
        }

        protected virtual void OnPaymentProcessed(OrderEventArgs e)
        {
            PaymentProcessed?.Invoke(this, e);
        }
    }

    public class OrderEventArgs : EventArgs
    {

    }
    public class WorkflowManager
    {
        public event Action Started;
        public event Action<string> StepCompleted;

        private readonly List<WorkflowStep> _steps;
        private int _currentStepIndex;

        public WorkflowManager(IEnumerable<WorkflowStep> steps)
        {
            _steps = new List<WorkflowStep>(steps);
            _currentStepIndex = 0;
        }

        public void Start()
        {
            Started?.Invoke();
            MoveToNextStep();
        }

        private void MoveToNextStep()
        {
            if (_currentStepIndex >= _steps.Count)
            {
                return;
            }

            var currentStep = _steps[_currentStepIndex];
            currentStep.Completed += OnStepCompleted;
            currentStep.Start();
        }

        private void OnStepCompleted()
        {
            var currentStep = _steps[_currentStepIndex];
            currentStep.Completed -= OnStepCompleted;

            StepCompleted?.Invoke(currentStep.Name);

            _currentStepIndex++;
            MoveToNextStep();
        }
    }


}
