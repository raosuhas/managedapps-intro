# Developing Managed Applications
Collection of ARM templates and scripts to get started with developing managed apps


# Prerequisites

- **Azure Powershell**
Please install the latest version of Azure Powershell by running the following command in your powershell session : 
Install-Module Az


# Custom Providers

Custom Providers gives publishers in Azure a way to extend azure and azure resources. The customProviders resource providers also enables users to add actions and resources to managed apps to enable functionality on the managed apps to run programmatic commands on them.

# Usage

The samples provided in this github repo can be deployed to your Azure renvironment by running the powershell command provided at the root of this repo. 
To run a template in a folder please use the following command in a powershell session:

```PowerShell
.\Deploy-AzureResourceGroup.ps1 -ArtifactStagingDirectory [NameofthefolderToDEploy] -ResourceGroupLocation eastus -ResourceGroupName [ResourceGroupToDeploy]]
```
The Deploy-AzureResourceGroup.ps1 is same as used in the azure-quickstart-templates along with a few additions to enable smooth deployment of the templates provided here. 

### Deploy To Azure
In addition each template file also has an option to deploy to azure in the Readme.md file present at the the root of the folder

# Getting Started

Deploy a custom provider with a simple user resource and a ping action : 
+ [** Creating a Custom Provider with resources **](CustomRPWithFunction/Readme.md)

The above Custom resource provider is backed by an azure function app.
Deploy the function app and understand the requirements for the api that backs the custom provider : 
+ [** Creating an azure function **](SampleFunction/Readme.md)

It is recommended to supply a swagger spec for custom providers when deploying them. 
To learm more about swagger and how to incorporate validation for custom providers:
+ [** Incorporating swagger into Custom Providers **](CustomRPWithSwagger/Readme.md)






