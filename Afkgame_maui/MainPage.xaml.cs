using Afkgame;

namespace Afkgame_maui;

public partial class MainPage : ContentPage
{
	
	private GameManager gameManager;
	int cost = 300;


	public MainPage()
	{
		InitializeComponent();
		gameManager = new GameManager();
		gameManager.MoneyGenerated += GameManager_MoneyGenerated;
		UpdateUI();
		AnimateProgressBar();
	}

private void GameManager_MoneyGenerated(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateUI();
            });
        }

        private void IncreaseWorkersLimit(object sender, EventArgs e)
        {
            increasWorkersLimitBtn.IsEnabled = false;
            if (gameManager.Money >= cost)
            {
                gameManager.MaxWorkers += 10;
                gameManager.Money -= 300;
                gameManager.Rent *= 3;
                cost = cost * 3;
                UpdateUI();
            }
            else
            {
                increasWorkersLimitBtn.IsEnabled = false;
                UpdateUI();
            }
        }

        private void BuyJuniorDev(object sender, EventArgs e)
        {
            Worker newJuniorDev = new Worker("Junior Dev", 5, 3, 18);
            if (gameManager.BuyWorker(newJuniorDev))
            {
                DisplayAlert("Info", "Junior bought!", "OK");
                UpdateUI();
            }
            else
            {
                buyJuniorDevBtn.IsEnabled = false;
            }
        }

        private void BuySeniorDev(object sender, EventArgs e)
        {
            Worker newSeniorDev = new Worker("Senior Dev", 25, 9, 60);
            if (gameManager.BuyWorker(newSeniorDev))
            {
                DisplayAlert("Info", "Senior bought!", "OK");
                UpdateUI();
            }
            else
            {
                buySeniorDevBtn.IsEnabled = false;
            }
        }

        private void BuyDesigner(object sender, EventArgs e)
        {
            Worker newDesigner = new Worker("Designer", 32, 13, 700);
            if (gameManager.BuyWorker(newDesigner))
            {
                DisplayAlert("Info", "Designer bought!", "OK");
                UpdateUI();
            }
            else
            {
                buyDesignerBtn.IsEnabled = false;
            }
        }

        private void UpdateUI()
        {
            cycleLabel.Text = gameManager.Cycles.ToString();
            moneyLabel.Text = $"Money: {gameManager.Money}$";
            totalDevJuniorsLabel.Text = $"Dev Juniors: {gameManager.TotalDevJuniors}";
            totalDevSeniorsLabel.Text = $"Dev Seniors: {gameManager.TotalDevSeniors}";
            totalDesignerLabel.Text = $"Designers: {gameManager.TotalDesigners}";
            workerLimitLabel.Text = $"{gameManager.Workers.Count} / {gameManager.MaxWorkers} Workers";
            rentLabel.Text = $"Rent : {gameManager.Rent} ";

            increasWorkersLimitBtn.Text = $"Add 10 slots ({cost}$)";

            bool canBuyMoreWorkers = (gameManager.TotalDevJuniors + gameManager.TotalDevSeniors + gameManager.TotalDesigners) < gameManager.MaxWorkers;


            buyJuniorDevBtn.IsEnabled = canBuyMoreWorkers && gameManager.Money >= 5; 
            buySeniorDevBtn.IsEnabled = canBuyMoreWorkers && gameManager.Money >= 25;
            buyDesignerBtn.IsEnabled = canBuyMoreWorkers && gameManager.Money >= 32;

            if (cost >= gameManager.Money)
            {
                increasWorkersLimitBtn.IsEnabled = false;
            }
            else
            {
                increasWorkersLimitBtn.IsEnabled = true;
            }
        }

          private void AnimateProgressBar()
        {
            var progressBarAnimation = new Animation(v => moneyProgressBar.Progress = v, 0, 1);
            progressBarAnimation.Commit(this, "ProgressBarAnimation", 16, 10000, Easing.Linear, (v, c) =>
            {
                moneyProgressBar.Progress = 0;
                AnimateProgressBar();
            });
        }
}