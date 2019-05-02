var async = require('async');
pki = require('node-forge').pki;
asn1 = require('node-forge').asn1;
md = require('node-forge').md;

const reqAuthorization = true; //set to false for http testing
const authorizedSubscriptions  = [
    "f5319171-ae72-4aff-a1ef-77bb2a2658d7",
    "f58167cd-7be6-4657-b3cb-794be1816cdb"
]  //resourceId must be in one of these subscriptions for authorization

async function getThumbprints(context){
    //Get CustomProviders client certificates
    return new Promise((resolve, reject) => {
        var reqUrl = require("request");
        var metadataUri = "https://customproviders.management.azure.com:24652/metadata/authentication"
        var certs = [];
        reqUrl({
            url: metadataUri,
            json: true
        }, function (error, response, body) {    
            if (!error) {
                var certs = body.clientCertificates;
                resolve(certs)
            }else{
                context.log.error(error);
                reject(error);
            }
        })
    }).catch(function(err) {
        context.log.error(err);
    });
};

async function checkClientCert(certs, context, req){
    //validate that request client cert is authorized
    return new Promise((resolve, reject) => {
        var authenticated=false;
        var clientCert = req.headers['x-arr-clientcert'];
  
        //get client certificate thumbprint
        const pem = `-----BEGIN CERTIFICATE-----${clientCert}-----END CERTIFICATE-----`;
        const incomingCert = pki.certificateFromPem(pem);
        const reqThumbprint = md.sha1.create().update(asn1.toDer((pki).certificateToAsn1(incomingCert)).getBytes()).digest().toHex();
        for (i = 0; i < certs.length; i++) {
            context.log("Comparing client cert " + reqThumbprint + " to server cert: " + certs[i].thumbprint)
            if (certs[i].thumbprint.toUpperCase() == reqThumbprint.toUpperCase()) {
                authenticated = true;
                context.log("authentication match")
            }
        }
        if(authenticated == true){
            resolve(true)
        }else{
            context.log.error("client certificate is unauthorized");
            reject("unauthorized")
        };
    }).catch(function(err) {
        context.log.error(err);
    });
}


module.exports = async function(context,req){
    var isAuthorized = false;
    //check that auth is enabled
    if(reqAuthorization === true){
        var certs = [];
        try {
            certs = await getThumbprints(context);
          } catch (error) {
            context.log.error(error);
          }
        try {
            const authenticated = await checkClientCert(certs,context,req);
            var subscriptionId = req.params.subscriptionId;
            if (authorizedSubscriptions.indexOf(subscriptionId) > -1) {
                context.log("subscriptionId is authorized");
                isAuthorized = true;
            } else {
                context.log.error("subscriptionId is unauthorized")
                isAuthorized = false;
            };
            
          } catch (error) {
            isAuthorized = false;
            context.log.error("authorization error: "+ error);
          }
        
          context.log("Auth check result is " + isAuthorized);
          return(isAuthorized);
    }else{
        context.log("Authorization is disabled by config.");
        return(true);
    }
   
 
};
