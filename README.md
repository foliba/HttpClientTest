# HttpClientTest
This tiny test should showcase the need of try/catch around HttpClient.SendAsync and HttpResponseMessage.Content.Read


what I would expect is:
a call using HttpClient which responds with anything else than 200-299 would cause an exception when calling GetStringAsync().

strangely what I observe is:
there is no exception thrown...
