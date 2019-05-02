# Developing Custom Providers
Collection of ARM templates and resources to get started with developing custom resource providers in ARM 


# About Custom Providers

Custom Providers gives publishers in Azure a way to extend azure and azure resources. They allow users to define resources and actions and have them behave as ARM resources and actions allowing features such as ARM template deployment of non-Azure resources, Integration of 3rd party tools and features as part of the template deployment and RBAC on these resources.

The customProviders resource providers also enables users to add actions and resources to managed apps to enable functionality on the managed apps to run programmatic commands on them.

# Usage

### Deploy To Azure
Each template file has an option to deploy to azure in the Readme.md file present at the the root of the folder


# Getting Started

Deploy a custom provider with a simple user resource and a ping action : 
+ [** Creating a Custom Provider with resources **](CustomRPWithFunction/Readme.md)

The above Custom resource provider is backed by an azure function app. You can read about to create this function and update it in the folllowing sample

Deploy the function app and understand the requirements for the api that backs the custom provider : 
+ [** Creating an azure function **](/SampleFunction/CSharpSimpleProvider/Readme.md)

Please check out the various samples of how you can use the custom providers in the Sample functions folder:
+ [** Sample Functions**](/SampleFunction/CSharpSimpleProvider/Readme.md)

It is recommended to supply a swagger spec for custom providers when deploying them. 
To learm more about swagger and how to incorporate validation for custom providers:
+ [** Incorporating swagger into Custom Providers **](CustomRPWithSwagger/Readme.md)








