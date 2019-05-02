var request = require('request');
var async = require('async');
var models = require("../common/models.js")
var auth = require("../common/auth.js")
var pki = require('node-forge').pki;
var asn1 = require('node-forge').asn1;
var md = require('node-forge').md;

module.exports = async function (context, req) {
    context.log('Item read received');
    var result;
    var authCheck = await auth(context,req)
    context.log("Auth result = " + authCheck)
    if(authCheck === true){
        if (req.params.resourceName != null){
            //get resource by Id
            context.log("Read item by ID")
            result = await readItem(context, req);
        }else{
            //list resources
            context.log("list items")
            result = await listItems(context, req);
        }
    }else{
        context.log.error("Authorization failed due to client cert or subscriptionId");
        result = models.handleError(403,"NotAuthorized","Caller is unauthorized.");
    }

    context.res = result;
};

async function readItem(context, req){
    return new Promise((resolve,reject) => {
        var subscriptionId = req.params.subscriptionId;
        var rgName = req.params.rgName;
        var resourceName = req.params.resourceName; 
        var body = models.resourceModel(subscriptionId,rgName, resourceName,'{"hello":"world"}');
        context.log("returning resource: " + JSON.stringify(body));
        var result =  models.handleResponse(200, body);
        resolve(result);
   }).catch(function(err) {
    context.log.error(err);
   });
};

async function listItems(context, req){
    return new Promise((resolve,reject) => {
        var subscriptionId = req.params.subscriptionId
        var rgName = req.params.rgame

        responseList = [];
        for (var i=1; i<=10; i++) {
            var resourceName = "item" + i;
            var item = models.resourceModel(subscriptionId, rgName, resourceName, '{"hello":"world"}')
            responseList.push(item);
          }
          context.log("returning resource list: " + responseList);
          var result =  models.handleResponse(200, responseList);

        resolve(result);
   }).catch(function(err) {
     context.log.error(err);
 });
};


  