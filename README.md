# Barely.GoogleAnalyticsV4
A minimal wrapper for the Google Analytics Measurement Protocol supporting netstandard2.0.

## Minimal code:
```c#
var ga = new GoogleAnalytics("UA-XXXXX-Y);
ga.SendPageView("/home", "example.com", "Home page");
ga.SendEvent("category", "action", "label", "300");
```