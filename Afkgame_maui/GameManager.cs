using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Afkgame_maui;
using Microsoft.Maui.Controls;

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
                moneyGenerationTimer = new Timer(OnMoneyGenerated, null, 0, 10000); // 10 secondes
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
                Application.Current.MainPage.DisplayAlert("Notification", $"You just paid yours workers {totalSalary}", "OK");
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
                App.Current.MainPage.DisplayAlert("Notification", $"You just paid your rent {Rent}", "OK");
                RentPaid?.Invoke(this, EventArgs.Empty);
            }
        }

        private void GameOver()
        {
            isGameActive = false;
            moneyGenerationTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            Application.Current.MainPage.DisplayAlert("Game Over", "Game over .....", "OK");
            Money = 25;
            Workers.Clear();
            TotalDevJuniors = 0;
            TotalDevSeniors = 0;
            TotalDesigners = 0;
            MaxWorkers = 10;
            Cycles = 0;
            Rent = 45;
            RestartGame();
        }

        private void RestartGame()
        {
            isGameActive = true;
            moneyGenerationTimer?.Change(0, 10000);
        }
    }
}
