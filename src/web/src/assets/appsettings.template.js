// Environment variables
(function (window) {
  window["env"] = window["env"] || {};
  window["env"]["apiUrl"] = "${API_URL}";
  window["env"]["apiKey"] = "${API_KEY}";
  window["env"]["applicationInsightsConnectionString"] =
    "${APPLICATION_INSIGHTS_CONNECTION_STRING}";
})(this);
