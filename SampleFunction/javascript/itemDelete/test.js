function main(){
    var authenticated = false;
    var body = JSON.parse('{"clientCertificates":    [        {"thumbprint":"SOOBDDC0A0DB705811FFC6ACEEE70D1122FF7E253",        "notBefore":"2018-10-04T22:03:19Z",        "notAfter":"2020-10-04T22:03:19Z",        "certificate":"foobar"} ,    {"thumbprint":"BABDDC0A0DB705811FFC6ACEEE70D1122FF7E253",        "notBefore":"2018-10-04T22:03:19Z",        "notAfter":"2020-10-04T22:03:19Z",        "certificate":"bar"}]}')
    certs = body.clientCertificates;
    
    for (i = 0; i < certs.length; i++) {
        console.log(certs[i].thumbprint);
    }
}

var foo =main()


