namespace Afkgame
{
  internal class Worker
  {
    public string Type { get; set; }
    public double Price { get; set; }
    public double MoneyGenerated { get; set; }
    public double Salary { get; set; }

    public Worker(string type, double price, double moneyGenerated, double salary)
    {
      Type = type;
      Price = price;
      MoneyGenerated = moneyGenerated;
      Salary = salary;
    }
  }
}