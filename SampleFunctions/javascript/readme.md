#  Overview
This is a simple skeleton demonstrating an Azure Functions app in javascript to implement CRUD operations for a Microsoft.CustomProviders extensible resource provider. Operations are currently mocked, but demonstrate a framework to handle PUT, GET, and DELETE methods. An ARM template is provided to exhibit the use of the custom provider in deployment operations. 

## Getting started

### To prepare the Azure environment:
* Create a resource group 
* Create an [AppService plan](https://docs.microsoft.com/en-us/cli/azure/functionapp/plan?view=azure-cli-latest#az-functionapp-plan-create)
* Create a storage account: `az storage account create --name <storage_name> --location <location> --resource-group <RGName> --sku Standard_LRS`
* Create the [functionapp](https://docs.microsoft.com/en-us/cli/azure/functionapp?view=azure-cli-latest#az-functionapp-create)
* Register the feature flag: `az feature register --namespace Microsoft.CustomProviders --name customrp`
* Register the provider: `az provider register --namespace Micrsoft.CustomProviders`
* Configure [functionapp deployment](https://docs.microsoft.com/en-us/azure/azure-functions/functions-continuous-deployment) with connected repository or git local repo

### To prepare your handler code:
* Install an LTS version of node
* Install (via npm):  `request`, `async`, and `node-forge`
* Set `const reqAuthentication = false;` in /common/auth.js file for local develoment with [Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local)
* Add subscriptionIDs for allowed subscriptions in /common/auth.js: `authorizedSubscriptions`
* Find and replace (in all function.json and /common/models.js) the providerName (nodeJsExample) and resourceType name (myResource) with appropriate values

### To enable authentication:
* Enable TLS mutual authentication in App Service. In the portal: Platform features -> SSL -> `Incoming client certificates` = On, and `HTTPS Only` = On.
* Turn on client certificate thumbprint verification in the function modules (/common/auth.js), by setting `const reqAuthentication = true;`
* Ensure subscriptionIds are correct in /common/auth.js
* Operations can be monitored in [kudu](https://blogs.msdn.microsoft.com/benjaminperkins/2017/11/08/how-to-access-kudu-scm-for-an-azure-app-service-environment-ase/)

Optimizations and contributions, via pull request, are most welcomed.
