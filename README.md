# Line of Business Accelerator

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fjwendl%2Flob-application-accelerator%2Fmaster%2Ftemplate%2FLobAccelerator.Template%2Fazuredeploy.json)

## Description

This is an application to generate O365 resources based on a simplified JSON payload.

## Getting Started

1. Click on the "Deploy to Azure" button above.
1. Create a POST request to the endpoint that was create <https://yourname.azurewebsites.net/api/StartDeployment> and pass in the Authorization Bearer token.
1. The body of the POST request will look like the below code

``` json
{
  "Name": "My MS Teams Group",
  "Description": "My MS Teams Group Description",
  "Members": [
    "juanr@microsoft.com",
    "juswen@microsoft.com",
    "tedistef@microsoft.com",
    "altagin@microsoft.com"
  ],
  "Channels": [
    {
      "Name": "my channel one",
      "Description": "my channel one description",
      "FilesAzstorageFolder": "/team-files/",
      "Members": [
        "juanr@microsoft.com",
        "juswen@microsoft.com"
      ]
    },
    {
      "Name": "my channel two",
      "Description": "my channel two description",
      "FilesAzstorageFolder": "/team-files-alt/",
      "Members": [
        "tedistef@microsoft.com",
        "juswen@microsoft.com",
        "andrg@microsoft.com",
        "altagin@microsoft.com"
      ]
    }
  ]
}
```

## Contributions

If there is any services that you'd like to add to this that do not already have functionality built for, please feel free to modify the project LobAccelerator.Library as needed to add the additional functionality.

## Prerequisites

- [Get an access token](docs/access_tokens.md)