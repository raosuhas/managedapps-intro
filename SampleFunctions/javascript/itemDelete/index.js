var request = require("request");
var async = require('async');
var models = require("../common/models.js")
var auth = require("../common/auth.js")
var pki = require('node-forge').pki;
var asn1 = require('node-forge').asn1;
var md = require('node-forge').md;


module.exports = async function (context, req) {
    context.log('Item delete received');
    var result;
    var authCheck = await auth(context,req)
    context.log("Auth result = " + authCheck)
    if(authCheck === true){
          //check for resource ID
          if (req.params.resourceName != null){
            //delete by ID
            context.log("Delete item by ID")
            result = await deleteItem(context, req);
        }else{
          //bad request
          result = models.handleError(400,"BadRequest","Missing resource Id")
          context.log.warn("Delete received with no resource name")
        }
    }else{
        context.log.error("Authorization failed due to client cert or subscriptionId");
        result = models.handleError(403,"NotAuthorized","Caller is unauthorized.");
    }
     context.log("Returning delete response: " + JSON.stringify(result));
    context.res = result;
};

async function deleteItem(context, req){
    return new Promise((resolve,reject) => {
        var subscriptionId = req.params.subscriptionId
        var rgName = req.params.rgame        
        var resourceName = req.params.resourceName
        //delete item, return 200 on success 
        var response = models.handleResponse(200,"");
        //if item does not exist, return 204
        //var response = models.handleResponse(204,"");
        resolve(response);
    }).catch(function(err) {
        reject(err);
        context.log.error(err);
    });
};


