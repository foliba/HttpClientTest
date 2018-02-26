# HttpClientTest
This tiny test should showcase the need of try/catch around HttpClient.GetStringAsync

what I would expect is:
a call using HttpClient which responds with anything else than 200-299 would cause an exception when calling GetStringAsync().

strangely what I observe is:
there is no exception thrown...


link to the documentation of .Net Core:
https://github.com/dotnet/corefx/blob/c8d23b4941b3b620633db397a0094f55b01be22b/src/System.Net.Http/src/System/Net/Http/HttpResponseMessage.cs

--> public HttpResponseMessage EnsureSuccessStatusCode()

Which is called by:
--> private async Task<string> GetStringAsyncCore()

Which is called by:
--> public Task<string> GetStringAsync(string requestUri)
