using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Afkgame_maui;
using CommunityToolkit.Maui.Alerts;

namespace Afkgame
{
    internal class GameManager
    {
        public double Money { get; set; }

        // Liste pour gérer les workers
        public List<Worker> Workers { get; private set; }

        public int TotalDevJuniors { get; private set; }
        public int TotalDevSeniors { get; private set; }
        public int TotalDesigners { get; private set; }
        public int MaxWorkers { get; set; }
        public int Cycles { get; private set; }
        public double Rent { get; set; }

        private bool isGameActive = true;

        private Timer? moneyGenerationTimer;
        public event EventHandler MoneyGenerated;
        public event EventHandler SalaryPaid;
        public event EventHandler RentPaid;

        public event EventHandler WorkersPaidSuccessfully;
        public event EventHandler RentPaidSuccessfully;
        public event EventHandler GameIsOver;


        public GameManager()
        {
            Money = 25;
            Workers = new List<Worker>();
            TotalDevJuniors = 0;
            TotalDevSeniors = 0;
            TotalDesigners = 0;
            MaxWorkers = 10;
            Cycles = 0;
            Rent = 45;

            // Timer
            if (isGameActive)
            {
                moneyGenerationTimer = new Timer(OnMoneyGenerated, null, 0, 1000); // 10 secondes
            } else {
                moneyGenerationTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        // Méthode pour acheter un worker
        public bool BuyWorker(Worker worker)
        {
            if (Money >= worker.Price && Workers.Count < MaxWorkers)
            {
                Money -= worker.Price;
                Workers.Add(worker);
                if (worker.Type == "Junior Dev")
                {
                    TotalDevJuniors++;
                }
                else if (worker.Type == "Senior Dev")
                {
                    TotalDevSeniors++;
                }
                else if (worker.Type == "Designer")
                {
                    TotalDesigners++;
                }
                return true;
            }
            return false;
        }

        private void OnMoneyGenerated(object? state)
        {
            if (!isGameActive)
            {
                return;
            }

            GenerateMoney();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                MoneyGenerated?.Invoke(this, EventArgs.Empty);
            });
        }

        private void GenerateMoney()
        {
            double totalMoneyGenerated = Workers.Sum(worker => worker.MoneyGenerated);
            Money += totalMoneyGenerated;
            Cycles += 1;
            // Pay workers
            if (Cycles % 10 == 0 && Cycles != 0)
            {
                PayWorkers();
            }
            if (Cycles % 18 == 0 && Cycles != 0)
            {
                PayRent();
            }
        }

        private void PayWorkers()
        {
            double totalSalary = Workers.Sum(worker => worker.Salary);
            Money -= totalSalary;
            if (Money <= -1)
            {
              GameOver();
            }
            else
             {
                  WorkersPaidSuccessfully?.Invoke(this, EventArgs.Empty);
                  SalaryPaid?.Invoke(this, EventArgs.Empty);
    }
        }

        private void PayRent()
        {
            Money -= Rent;
            if (Money <= -1)
            {
                GameOver();
            }
            else
            {
                RentPaidSuccessfully.Invoke(this, EventArgs.Empty);
                RentPaid?.Invoke(this, EventArgs.Empty);
            }
        }
        private void GameOver()
        {
            isGameActive = false;        
            GameIsOver?.Invoke(this, EventArgs.Empty);

            RestartGame();
        }

        private void RestartGame()
        {
            isGameActive = true;
            Money = 25;
            Workers.Clear();
            TotalDevJuniors = 0;
            TotalDevSeniors = 0;
            TotalDesigners = 0;
            MaxWorkers = 10;
            Cycles = 0;
            Rent = 45;
        }
    }
}
