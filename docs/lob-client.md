# Line of Business Accelerator Client

The Line of Business Client is a command line tool to simplify Office 365 resources provisioning, providing support for Json and Yaml files.

*Note: The CLI is a work in progress, so there is no stable version available yet.*

For Yaml files:

    $ lobctl -i yaml -d <your_file>.yaml

For Json files:

    $ lobctl -i json -d <your_file>.json


## Sample in Json format
```json
{
  "Name": "My MS Teams Group",
  "Description": "My MS Teams Group Description",
  "Members": [
    "jwendl@jwazuread.onmicrosoft.com",
    "testuser001@jwazuread.onmicrosoft.com",
    "testuser002@jwazuread.onmicrosoft.com",
    "testuser003@jwazuread.onmicrosoft.com"
  ],
  "Channels": [
    {
      "Name": "my channel one",
      "Description": "my channel one description",
      "Files": [
        "/team-files/"
      ],
      "Members": [
        "jwendl@jwazuread.onmicrosoft.com",
        "testuser001@jwazuread.onmicrosoft.com",
      ]
    },
    {
      "Name": "my channel two",
      "Description": "my channel two description",
      "FilesAzstorageFolder": "/team-files-alt/",
      "Members": [
        "testuser002@jwazuread.onmicrosoft.com",
        "testuser003@jwazuread.onmicrosoft.com"
      ]
    }
  ]
}
```

## Sample in Yaml format
```yaml
---
Name: My MS Teams Group
Description: My MS Teams Group Description
Members:
- jwendl@jwazuread.onmicrosoft.com
- testuser001@jwazuread.onmicrosoft.com
- testuser002@jwazuread.onmicrosoft.com
- testuser003@jwazuread.onmicrosoft.com
Channels:
- Name: my channel one
  Description: my channel one description
  Files:
  - "/team-files/"
  Members:
  - jwendl@jwazuread.onmicrosoft.com
  - testuser001@jwazuread.onmicrosoft.com
- Name: my channel two
  Description: my channel two description
  FilesAzstorageFolder: "/team-files-alt/"
  Members:
  - testuser002@jwazuread.onmicrosoft.com
  - testuser003@jwazuread.onmicrosoft.com
```
