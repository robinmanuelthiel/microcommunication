// Environment variables
(function (window) {
  window['env'] = window['env'] || {};
  window['env']['apiUrl'] = '${API_URL}';
  window['env']['apiKey'] = '${API_KEY}';
  window['env']['applicationInsightsInstrumentationKey'] = '${APPLICATION_INSIGHTS_INSTRUMENTATION_KEY}';
})(this);
