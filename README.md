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
  "teams": [
    {
      "displayName": "My MS Teams Group",
      "description": "My MS Teams Group Description",
      "mailNickName": "my1teaamsgroup",
      "members": [
        "jwendl@jwazuread.onmicrosoft.com",
        "testuser001@jwazuread.onmicrosoft.com",
        //"tedistef@jwazuread.onmicrosoft.com",
        "altagin@jwazuread.onmicrosoft.com"
      ],
      "channels": [
        {
          "displayName": "my channel one",
          "description": "my channel one description",
          "azFilesFolderPath": "/team-files/",
          "members": [
            "jwendl@jwazuread.onmicrosoft.com",
            "testuser001@jwazuread.onmicrosoft.com"
          ]
        },
        {
          "displayName": "my channel two",
          "description": "my channel two description",
          "azFilesFolderPath": "/team-files-alt/",
          "members": [
            "jwendl@jwazuread.onmicrosoft.com",
            "testuser001@jwazuread.onmicrosoft.com"
          ]
        }
      ]
    },
    {
      "displayName": "My Second Teams Group",
      "description": "My Second Teams Group Description",
      "mailNickName": "my1teaamsgroup",
      "members": [
        "testuser001@jwazuread.onmicrosoft.com",
        "jwendl@jwazuread.onmicrosoft.com"

      ],
      "channels": [
        {
          "displayName": "Second team my channel one",
          "description": "Second team my channel one description",
          "azFilesFolderPath": "/team-files-2/",
          "members": [
            "testuser001@jwazuread.onmicrosoft.com"
          ]
        },
        {
          "displayName": "my channel two",
          "description": "my channel two description",
          "azFilesFolderPath": "/team-files-alt/",
          "members": [
            "jwendl@jwazuread.onmicrosoft.com"
          ]
        }
      ]
    }
  ]
}

```

## Contributions

If there is any services that you'd like to add to this that do not already have functionality built for, please feel free to modify the project LobAccelerator.Library as needed to add the additional functionality.

## Prerequisites

- [Get an access token](docs/access_tokens.md)
