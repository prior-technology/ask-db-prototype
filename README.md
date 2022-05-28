# Ask DB Question Answering

This demonstrates using Large Language Models for answering questions on technical documentation. It uses the 
[OpenAI API Question Answering API](https://beta.openai.com/docs/guides/answers).

## Prototype Code

This code prioritises speed of development over reusability. It uses Blazor and several automatically configured Azure resources which may not be 
easy to replicate. Another codebase is under development to support more use cases, including different UI layers and support for other LLM providers.

## AskDbConsole

Command line application which uses the Open-AI-DotNet library to manage files and submit questions through the OpenAI API. It can parse individual HTML files or 
nested folders to plain text, generates a jsonl file in the format required for the Answers API and submits it using the Files API. OpenAI credentials are supplied 
as described in https://github.com/RageAgainstThePixel/OpenAI-DotNet#authentication 

## AskDbWebDemo

Build and run the web application using Visual Studio 2022.

The application depends on the following [secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#secret-manager):

|name|description|
|---|---|
|"OpenAI:key"|credentials to access the OpenAI API|
|"OpenAI:GIMP:FileId"|The id of a [file](https://beta.openai.com/docs/api-reference/answers/create#answers/create-file) uploaded to OpenAI which contains documents relating to the GIMP topic|
|"OpenAI:GIT:FileId"|The id of a [file](https://beta.openai.com/docs/api-reference/answers/create#answers/create-file) relating to the git topic|
|"Azure:SignalR:ConnectionString"|Connection string for an [Azure SignalR](https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-overview) endpoint. The connection string can be generated from the Connection Strings settings page under the SignalR resource in the Azure Console.|
|"TableStorageUri"|URI for an Azure Table Storage account used for logging questions and responses, and to store details of new topics added by users. https://example.table.core.windows.net |
|"TableStorageAccount"|The Azure Table Storage account name|
|"TableStorageKey"|Access key for storage account,|
|"TableStorageConnectionString"|The Resource ID connection string under Table Service in the Endpoints page of the Storage Account in Azure.|

