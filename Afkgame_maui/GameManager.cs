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
        private Settings settings;

        // Liste pour gérer les workers
        public List<Worker> Workers { get; private set; }
    
        public bool CanIncrease { get => Money >= settings.cost ; }
    
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
            if (Money >= worker.Price && Workers.Count < settings.maxWorkers)
            {
                Money -= worker.Price;
                Workers.Add(worker);
                if (worker.Type == "Junior Dev")
                {
                    settings.totalDevJuniors++;
                }
                else if (worker.Type == "Senior Dev")
                {
                    settings.totalDevSeniors++;
                }
                else if (worker.Type == "Designer")
                {
                    settings.totalDesigners++;
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
            settings.cycles += 1;
            // Pay workers
            if (settings.cycles % 10 == 0 && settings.cycles != 0)
            {
                PayWorkers();
            }
            if (settings.cycles % 18 == 0 && settings.cycles != 0)
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
            Money -= settings.rent;
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
            settings.totalDevJuniors = 0;
            settings.totalDevSeniors = 0;
            settings.totalDesigners = 0;
            settings.maxWorkers = 10;
            settings.cycles = 0;
            settings.rent = 45;
        }
    }
}
