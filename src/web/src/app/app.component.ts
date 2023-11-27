import { Component } from "@angular/core";
import { ApplicationInsights } from "@microsoft/applicationinsights-web";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.sass"],
})
export class AppComponent {
  title = "MicroCommunication-Web";
  appInsights: ApplicationInsights;

  constructor() {
    if (environment.applicationInsightsConnectionString) {
      // Initialize Application Insights
      this.appInsights = new ApplicationInsights({
        config: {
          instrumentationKey: environment.applicationInsightsConnectionString,
          extensions: [],
          enableAutoRouteTracking: true,
        },
      });
      this.appInsights.loadAppInsights();

      // Log a Demo Event
      this.appInsights.trackEvent({ name: "Web Frontend Loaded (Test Event)" });
    }
  }
}
