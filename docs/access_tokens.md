# Access Tokens

The accelerator assumes that you have an AAD access token with proper permissions to provision the required O365 resources. You can retrieve an access token by following steps 1-3 on the Microsoft Graph docs for [Getting access on behalf of a user](https://developer.microsoft.com/en-us/graph/docs/concepts/auth_v2_user).

In step 1, when registering your app, add the following delegated permissions in the Microsoft Graph Permissions section:
- `Group.ReadWrite.All` (for Teams)
- `Sites.ReadWrite.All` (for SharePoint)

In steps 2-3, include these scopes in the scope parameters. For example: `scope=offline_access group.readwrite.all sites.readwrite.all`