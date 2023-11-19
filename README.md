# Statsig .NET SDK
Statsig server SDKs are sets of tools/methods for developing applications to run tests and are available to help developers integrate their services easily.

![Screenshot](https://github.com/gulizay91/statsig-poc/blob/main/etc/statsig-overview.png?raw=true)

The server SDK polls Statsig servers for updates to the rule sets every 10 seconds and sends events to the server every 60 seconds by default (and these values can be overridden)

There are some useful methods for us to develop our tests in .NET server SDK.

### Install package
```sh
nuget install Statsig
```

## Initialize
We have to call the initialize method when your application is started. Otherwise, you can not use other server SDK methods. initialize will perform a network request and is asynchronous. After initialization completes, virtually all SDK operations will be synchronous. The serverSecret parameter is mandatory for the initialize method. You can access this parameter from Project Settings > Keys & Environments tab. (Use the “Server Secret Keys” value for the Server SDK.) The [StatsigOptions](https://docs.statsig.com/server/dotnetSDK#statsig-options) parameter can be set according to your intended use.

![Screenshot](https://github.com/gulizay91/statsig-poc/blob/main/etc/statsig-secret-key.png?raw=true)


```sh
using Statsig;
using Statsig.Server;

await StatsigServer.Initialize(
    "server-secret-key",
    // optionally customize the SDKs configuration via StatsigOptions
    new StatsigOptions(
        new StatsigEnvironment(EnvironmentTier.Development)
    )
);
```

## Feature Gates
Feature Gates control which user see a given feature and which doesn’t and this is accomplished via a set of conditionals.
```sh
var user = new StatsigUser { UserID = userId };
var checkGate = StatsigServer.CheckGateSync(user, "feature-gate-name");
if (checkGate)
{
  // Gate is on, enable new feature
}
else
{
  // Gate is off
}
```

## Experiment
Experiments allows you run a/b or a/b/n tests.
Create an experiment which name is "button-color". A group is added with the Control group being "as_is_blue" and the Test group being "to_be_green". Extra parameters can be added if desired, for example the "isEnabled" parameter is added as number.
![Screenshot](https://github.com/gulizay91/statsig-poc/blob/main/etc/experiment-button-color.png?raw=true)
```sh
var user = new StatsigUser { UserID = userId };
var experimentConfig = StatsigServer.GetExperimentSync(user, "button-color");
if (experimentConfig.Value["isEnabled"] == true || experimentConfig.GroupName == "to_be_green")
{
  // set button color green
}
else
{
  // set button color blue, as is
}
```

## LogEvent
You can use LogEvent if you want to track your Custom Events. You can see your custom event in the Metrics screen.
```sh
StatsigClient.LogEvent(
  "level_completed", // Event Name
  11, // Level number
  new Dictionary<string, string>()
  { 
    { "score", "452" } 
  }
);
```

## ShutDown
Statsig SDK batch and periodically flush events to Statsig Servers. So, you should call StatsigServer.Shutdown method when your app/server is closing, if you do not want to lose your some events.
```sh
StatsigServer.Shutdown();
```

## PrivateAttributes in FeatureGate & CustomEvent in Metrics
You can use private attributes to hide private information when using private info as condition. PrivateAttributes will be sent with this call but will not be stored or logged on Statsig servers.
First, create FeatureGate which name is "mastercard" and add a rule match regex for credit card number.
![Screenshot](https://github.com/gulizay91/statsig-poc/blob/main/etc/statsig-feature-gate.png?raw=true)
#### Sample Code
```sh
var creditCard = "5114496353984312";
var maskedCard = string.Concat(creditCard[..4], new string('*', creditCard.Length - 4));
var user = new StatsigUser { UserID = userId, Country = "TR" };
user.AddPrivateAttribute("creditCard", creditCard);
user.AddCustomProperty("maskedCreditCard", maskedCard);
var isMasterCard = StatsigServer.CheckGateSync(user, "mastercard");
if (isMasterCard)
{
  // do something for user who has a Mastercard
  StatsigServer.LogEvent(
          user,
          "MasterCardEvent",
          "sku-1115552222",
          new Dictionary<string, string>() {
            { "price", "9.99" },
            { "item_name", "diet_coke_48_pack" }
          }
        );
  Console.WriteLine("User: {0} has mastercard {1} maskedCreditCard: {2}", user.UserID, isMasterCard.ToString(), masketCard);
}
else
{
  // do something for user who has not a Mastercard
  StatsigServer.LogEvent(
          user,
          "OtherCardEvent",
          "sku-1115552222",
          new Dictionary<string, string>() {
            { "price", "9.99" },
            { "item_name", "diet_coke_48_pack" }
          }
        );
  Console.WriteLine("User: {0} has othercard {1} maskedCreditCard: {2}", user.UserID, isMasterCard.ToString(), masketCard);
}

```
#### FeatureGate mastercard Log
![Screenshot](https://github.com/gulizay91/statsig-poc/blob/main/etc/mastercard-feature-gate-log.png?raw=true)

#### Metrics Custom Event MasterCardCustomEvent
![Screenshot](https://github.com/gulizay91/statsig-poc/blob/main/etc/metric-mastercard-event.png?raw=true)