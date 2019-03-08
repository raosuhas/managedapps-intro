# Creating a Custom Provider with Resources

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fraosuhas%2Fmanagedapps-intro%2Fmaster%2FCustomRPWithFunction%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/> 
</a>

This sample template deploys a custom resource provider to azure and creates a user using an ARM template. 
To deploy this template please use the following command from the root of the github repo : 

### Deploy
```Deploy
.\Deploy-AzureResourceGroup.ps1 -ArtifactStagingDirectory CustomRPWithFunction -ResourceGroupLocation eastus -ResourceGroupName [ResourceGroupToDeploy]
```

# Details on the custom resource provider created. 

This sample deployment creates the following two apis on the resource. 

1) An ARM extended resource called "users"
2) An APi called "ping"

### Users 

The users resource is defined in the following part of the ARM template : 

```
"resourceTypes": [
                                {
                                    "name":"users",
                                    "routingType":"ProxyOnly",
                                    "endpoints": [
                                        {
                                            "enabled": true,
                                            "apiVersion": "[variables('customrpApiversion')]",
                                            "endpointUri": "[concat('https://', parameters('funcname'), '.azurewebsites.net/api')]",
                                            "timeout": "PT15S"
                                        }
                                    ]
                                }
                            ]
```
In the custom provider template we can see the function called





