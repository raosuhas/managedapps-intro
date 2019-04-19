var request = require("request");
var async = require('async');
var models = require("../common/models.js")
var auth = require("../common/auth.js")
var pki = require('node-forge').pki;
var asn1 = require('node-forge').asn1;
var md = require('node-forge').md;

module.exports = async function (context, req) {
    context.log('Item create or update received');

    var result;
    var authCheck = await auth(context,req)
    context.log("Auth result = " + authCheck)
    if(authCheck === true){
        //create or update resource
        //check if body is RPC compliant via check for "properties" property
        if(req.body.hasOwnProperty('properties')){
            //create or update resource
            result = await createUpdateItem(context, req);
        }else{
            result = models.handleError(400, "BadRequest","No properties object found in the request.")
        }
    }else{
        context.log.error("Authorization failed due to client cert or subscriptionId");
        result = models.handleError(403,"NotAuthorized","Caller is unauthorized.");
    } 

    context.res = result;
};

async function createUpdateItem(context, req){
    return new Promise((resolve,reject) => {
     //create or update resource
     var subscriptionId = req.params.subscriptionId;
     var rgName = req.params.rgame;
     var resourceName = req.params.resourceName
     var resProperties = req.body.properties
  
    context.log("Request body for PUT: \n" + JSON.stringify(req.body)); //log request body 

    //resource creation routine

    //return well-formed resource schema, mocked currently
    var body = models.resourceModel(subscriptionId, rgName, resourceName, resProperties);
    var result =  models.handleResponse(200, body);
    resolve(result);
    }).catch(function(err) {
        context.log.error(err);
    });
}

