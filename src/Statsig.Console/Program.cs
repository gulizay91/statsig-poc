﻿// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using System.Text.Json;
using Statsig;
using Statsig.Server;

await InitStatsig();

var key = "";

do
{
  Menu();
  Run(key);
} while (key != "q" && key != "Q");


void Menu()
{
  Console.WriteLine("/*******************************************************************/");
  Console.WriteLine("Statsig Menu: ");
  Console.WriteLine("1- Test-FeatureGate");
  Console.WriteLine("2- Test-FeatureGate Concurrent 50x100");
  Console.WriteLine("3- Test-Experiment");
  Console.WriteLine("4- Test-Experiment Concurrent 50x100");
  Console.WriteLine("5- Test-FeatureGate-PrivateAttributes: mastercard & CustomEvent");
  Console.WriteLine("6- Test-FeatureGate & Experiment: mastercard & show-campaign-mc");
  Console.WriteLine("7- Test-FeatureGate & Segment & Experiment: matrix-experiment");
  Console.WriteLine("/*******************************************************************/");
  Console.Write("Select Process: ");
  key = Console.ReadLine();
}

void Run(string key)
{
  var featureGateName = string.Empty;
  var experimentName = string.Empty;
  
  switch (key)
  {
    case "1":
      Console.Write("Feature Gate Name: ");
      featureGateName = Console.ReadLine();
      var userGate = GenerateUsers(1).First();
      var featureGate = StatsigServer.CheckGateSync(userGate, featureGateName);
      var featureGateInitializeResponse = StatsigServer.GetClientInitializeResponse(userGate);
      Console.WriteLine("User: {0}, feature-gate:{1}, result: {2}", userGate.UserID, featureGate.ToString(), JsonSerializer.Serialize(featureGateInitializeResponse));
      break;
    case "2":
      Console.Write("Feature Gate Name: ");
      featureGateName = Console.ReadLine();
      RunFeatureGate(50, 100, featureGateName);
      break;
    case "3":
      Console.Write("Experiment Name: ");
      experimentName = Console.ReadLine();
      var userExperiment = GenerateUsers(1).First();
      var experiment = StatsigServer.GetExperimentSync(userExperiment, experimentName);
      var result = StatsigServer.GetClientInitializeResponse(userExperiment);
      Console.WriteLine("User: {0}, experiment-group&value: {1}&{2}, result: {3}", userExperiment.UserID, experiment.GroupName, experiment.Value["isEnabled"], JsonSerializer.Serialize(result));
      break;
    case "4":
      Console.Write("Experiment Name: ");
      experimentName = Console.ReadLine();
      RunExperiment(50, 100, experimentName);
      break;
    case "5":
      Console.Write("input CreditCard: ");
      var creditCard = Console.ReadLine();
      var maskedCard = MaskedCard(creditCard);
      var statsigUser = GenerateUsers(1).First();
      statsigUser.AddPrivateAttribute("creditCard", creditCard);
      statsigUser.AddCustomProperty("maskedCreditCard", maskedCard);
      featureGateName = "mastercard";
      var isMasterCard = StatsigServer.CheckGateSync(statsigUser, featureGateName);
      if (isMasterCard)
      {
        StatsigServer.LogEvent(
          statsigUser,
          "MasterCardCustomEvent",
          $"sku-{Guid.NewGuid().ToString()}",
          new Dictionary<string, string>() {
            { "price", "9.99" },
            { "item_name", "diet_coke_48_pack" }
          }
        );
      }
      Console.WriteLine("User: {0}, feature-gate:{1}, creditCard: {2}, result: {3}", statsigUser.UserID, featureGateName, maskedCard, isMasterCard.ToString());
      break;
    case "6":
      Console.Write("input CreditCard: ");
      var cc = Console.ReadLine();
      var maskedCC = MaskedCard(cc);
      var userMcGate = GenerateUsers(1).First();
      userMcGate.AddPrivateAttribute("creditCard", cc);
      userMcGate.AddCustomProperty("maskedCreditCard", maskedCC);
      experimentName = "show-campaign-mc";
      var experimentConfig = StatsigServer.GetExperimentSync(userMcGate, experimentName);
      if (experimentConfig.IsUserInExperiment)
      {
        StatsigServer.LogEvent(
          userMcGate,
          "MasterCardCustomEvent",
          $"sku-{Guid.NewGuid().ToString()}",
          new Dictionary<string, string>() {
            { "price", "6.99" },
            { "item_name", "diet_coke_48_pack" }
          }
        );
      }
      var isExperimentEnabled = experimentConfig.Get<bool>("isEnabled");
      Console.WriteLine("User: {0}, experiment-group&value: {1}&{2}, result: {3}", userMcGate.UserID, experimentConfig.GroupName, isExperimentEnabled, JsonSerializer.Serialize(experimentConfig));
      break;
    case "7":
      Console.Write("input id: ");
      var id = Console.ReadLine();
      RunExperimentMatrix(Convert.ToInt32(id));
      break;
    case "q" or "Q":
      Console.WriteLine("Bye");
      StatsigServer.Shutdown();
      break;
    default:
      Console.WriteLine("Wrong Choice");
      break;
  }
  
}

async Task InitStatsig()
{
  var apiKey = "<secret>";
  await StatsigServer.Initialize(apiKey, new StatsigServerOptions(environment: new StatsigEnvironment(EnvironmentTier.Production)));
}

string MaskedCard(string card)
{
  var firstFourDigits = card[..4];
  var maskedDigits = new string('*', card.Length - 4);
  return string.Concat(firstFourDigits, maskedDigits);
}

IEnumerable<StatsigUser> GenerateUsers(int count)
{
  var users = new List<StatsigUser>();
  for (var i = 0; i < count; i++)
  {
    var countryCode = i % 2 == 0 ?CountryCode.TR.ToString() : CountryCode.US.ToString();
    users.Add(new StatsigUser { UserID = Guid.NewGuid().ToString(), Country = countryCode });
  }

  return users;
}

async void RunFeatureGate(int processCount, int parallelUserCount, string gateName)
{
  var tasks = new List<Task>();
  var stopWatch = Stopwatch.StartNew();
  Console.WriteLine("Start Feature Gate {0}x{1}", processCount, parallelUserCount);
  for (var i = 0; i < processCount; i++)
  {
    Console.WriteLine("process: {0}", i);
    var users = GenerateUsers(parallelUserCount);
    Parallel.ForEach(users, user =>
    {
      Console.WriteLine("User: {0}", user.UserID);
      tasks.Add(Task.Run(() => StatsigServer.CheckGateSync(user, gateName)));
    });
    Console.WriteLine("process: {0} end, Parallel.ForEach() execution duration = {1} ", i, stopWatch.Elapsed.TotalMilliseconds);
  }
  await Task.WhenAll(tasks);
  Console.WriteLine("End Concurrent Feature Gate {0}x{1} duration: {2}", processCount, parallelUserCount, stopWatch.Elapsed.TotalMilliseconds);
  stopWatch.Stop();
}

async void RunExperiment(int processCount, int parallelUserCount, string experimentName)
{
  var tasks = new List<Task>();
  var stopWatch = Stopwatch.StartNew();
  Console.WriteLine("Start Concurrent Experiment {0}x{1}", processCount, parallelUserCount);
  for (var i = 0; i < processCount; i++)
  {
    Console.WriteLine("process: {0}", i);
    var users = GenerateUsers(parallelUserCount);
    Parallel.ForEach(users, user =>
    {
      Console.WriteLine("User: {0}", user.UserID);
      tasks.Add(Task.Run(() => StatsigServer.GetExperimentSync(user, experimentName)));
    });
    Console.WriteLine("process: {0} end, Parallel.ForEach() execution duration = {1} ", i, stopWatch.Elapsed.TotalMilliseconds);
  }
  await Task.WhenAll(tasks);
  Console.WriteLine("End Concurrent Experiment {0}x{1} duration: {2}", processCount, parallelUserCount, stopWatch.Elapsed.TotalMilliseconds);
  stopWatch.Stop();
}

void RunExperimentMatrix(int id)
{
  var experimentName = "matrix-experiment";
  var pill = id is > 0 and < 100 ?Pill.Red.ToString() : Pill.Blue.ToString();
  var userStatsig = new StatsigUser { UserID = id.ToString() };
  userStatsig.AddCustomProperty("pill", pill);
  var experimentConfig = StatsigServer.GetExperimentSync(userStatsig, experimentName);

  switch (experimentConfig.GroupName)
  {
    case "zion":
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("The human {0} woken, experiment-group: {1}, result: {2}", userStatsig.UserID, experimentConfig.GroupName, JsonSerializer.Serialize(experimentConfig));
      break;
    case "power-plant":
    {
      var segmentExposures = experimentConfig.SecondaryExposures.Find(item => item["gate"] == "segment:neo-segment");
      var isUserInSegment = segmentExposures?["gateValue"];
      //var who = experimentConfig.Value["who"].ToString();
      if (isUserInSegment is "true") // neo
      {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Welcome Neo {0} , experiment-group: {1}, result: {2}", userStatsig.UserID, experimentConfig.GroupName, JsonSerializer.Serialize(experimentConfig));
      }
      else // isUserInSegment is "false" agent-smith
      {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Agent-Smith {0} , experiment-group: {1}, result: {2}", userStatsig.UserID, experimentConfig.GroupName, JsonSerializer.Serialize(experimentConfig));
      }
    }
    break;
    case "Targeting Gate (matrix-landline-phone-gate)":
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.WriteLine("User {0} took blue pill zZz... -> result: {1}", userStatsig.UserID, JsonSerializer.Serialize(experimentConfig));
      break;
    case "Unstarted":
      Console.BackgroundColor = ConsoleColor.White;
      Console.ForegroundColor = ConsoleColor.Black;
      Console.WriteLine("Sorry {0}, The Architect is busy. -> result: {1}", userStatsig.UserID, JsonSerializer.Serialize(experimentConfig));
      break;
    default: // dont expect
      Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine("User {0} is limbo. -> result: {1}", userStatsig.UserID, JsonSerializer.Serialize(experimentConfig));
      break;
  }

  Console.ResetColor();
}

enum CountryCode
{
  TR,
  US
}

enum Pill
{
  Blue,
  Red
}

