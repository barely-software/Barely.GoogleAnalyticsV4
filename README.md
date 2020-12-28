# Barely.GoogleAnalyticsV4
A minimal wrapper for the Google Analytics Measurement Protocol supporting netstandard2.0.

[View the Measurement Protocol Documentation][1]

## code:
```c#
var ga = new GoogleAnalytics("UA-XXXXX-Y");
ga.SendPageView("/home", "example.com", "Home page");
ga.SendEvent("category", "action", "label", "300");
```


[1]: https://developers.google.com/analytics/devguides/collection/protocol/v1
