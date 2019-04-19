const providerName = "nodeJsExample";
const resourceType = "myResource"  ;  

module.exports = {

    handleError: function(code, errorName, message){
        response = {
            status: code,
                body: {
                "error": {
                    "code": errorName,
                        "message": message,
                        }
            },
            headers: {
                'Content-Type': 'application/json'
            }
        };
        return response;
    },
    resourceModel: function(subscriptionId, rgName, resourceName,properties){
        var responseBody = {
            id: "/subscriptions/" + subscriptionId+ "resourceGroups/" + rgName + "/providers/Microsoft.CustomProviders/resourceProviders/" + providerName + "/" + resourceType + "/" + resourceName,
            type: "Microsoft.CustomProviders/resourceProviders/" + providerName + "/" + resourceType,
            name: resourceName,
            properties: properties
        };
        return responseBody;
    },

    handleResponse: function(code, body){
        var response= {
            status: code, 
            body: body,
            headers: {
             'Content-Type': 'application/json'
             }
         };
        return response;
    }
}