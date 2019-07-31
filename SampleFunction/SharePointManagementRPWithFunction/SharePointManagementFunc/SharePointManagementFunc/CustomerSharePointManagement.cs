using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using SharePointManager.Models;

namespace SharePointManager
{
    public static class CustomerSharePointManagement
    {
        static string siteUrl = Environment.GetEnvironmentVariable("spSiteUrl");
        static string webTemplateUrl = Environment.GetEnvironmentVariable("spWebTemplate");
        static int webLanguage = Convert.ToInt32(Environment.GetEnvironmentVariable("spWebLanguage"));

        [FunctionName("sites")]
        public static async Task<HttpResponseMessage> sites([HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", "delete",
            Route = "sites/{url?}")]HttpRequestMessage req, string url, TraceWriter log)
        {
            var realm = TokenHelper.GetRealmFromTargetUrl(new Uri(siteUrl));

            //Get the access token for the URL.  
            var accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, new Uri(siteUrl).Authority, realm).AccessToken;

            //Get methods to return all customer sites and a customer site information
            if (req.Method == HttpMethod.Get)
            {
                var content = req.Content;
                string JsonContent = content.ReadAsStringAsync().Result;
                var webInfo = JsonConvert.DeserializeObject<CreateWebModel>(JsonContent);

                var allSitesRequest = true;
                var urlInfo = string.Empty;

                if (!string.IsNullOrEmpty(url) || (webInfo != null && !string.IsNullOrEmpty(webInfo.Url)))
                {
                    //Request is for 1 site
                    allSitesRequest = false;
                    urlInfo = !string.IsNullOrEmpty(url) ? url : webInfo.Url;
                }

                //returns a customer's site information
                if (!allSitesRequest)
                {
                    using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteUrl, accessToken))
                    {                       

                        var web = clientContext.Site.OpenWeb(urlInfo);

                        clientContext.Load(web);
                        clientContext.ExecuteQuery();

                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new JsonContent(new
                            {
                                properties = new
                                {
                                    title = web.Title,
                                    description = web.Description,
                                    url = web.Url
                                }
                            })
                        };
                    }
                }
                //Get all method, returns all sites
                else if (allSitesRequest)
                {
                    using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteUrl, accessToken))
                    {
                        var response = new List<string>();
                        var subWebs = clientContext.Web.Webs;
                        clientContext.Load(subWebs);
                        clientContext.ExecuteQuery();

                        foreach (var subweb in subWebs)
                        {
                            response.Add(subweb.Title);
                        }

                        var jsonResponse = JsonConvert.SerializeObject(response);

                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new JsonContent(new
                            {
                                properties = new
                                {
                                    sites = response
                                }
                            })
                        };
                    }
                }
                else
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Bad request. Check the path.");
                }


            }
            //Put method to create a new customer site
            else if (req.Method == HttpMethod.Put)
            {
                var content = req.Content;
                string JsonContent = content.ReadAsStringAsync().Result;
                var webInfo = JsonConvert.DeserializeObject<CreateWebModel>(JsonContent);

                var properRequest = false;
                var urlInfo = string.Empty;

                if (!string.IsNullOrEmpty(url) || !string.IsNullOrEmpty(webInfo.Url))
                {
                    properRequest = true;
                    urlInfo = !string.IsNullOrEmpty(url) ? url : webInfo.Url;
                }
                if (properRequest)
                {
                    using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteUrl, accessToken))
                    {
                        var wci = new WebCreationInformation();

                        wci.Url = urlInfo;
                        wci.Title = webInfo.Title;
                        wci.Description = webInfo.Description;
                        wci.UseSamePermissionsAsParentSite = true;
                        wci.WebTemplate = webTemplateUrl;
                        wci.Language = 1033;

                        Web w = clientContext.Site.RootWeb.Webs.Add(wci);

                        clientContext.ExecuteQuery();

                        var response = "web created";
                        var jsonResponse = JsonConvert.SerializeObject(response);

                        return new HttpResponseMessage(HttpStatusCode.Created)
                        {
                            Content = new JsonContent(new
                            {
                                properties = new
                                {
                                    message = response,
                                    title = webInfo.Title,
                                    description = webInfo.Description,
                                    url = urlInfo
                                }
                            })
                        };
                    }
                }
                else
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Bad request. Need url information.");
                }
            }
            //Delete method to delete a site
            else if (req.Method == HttpMethod.Delete)
            {
                var content = req.Content;
                string JsonContent = content.ReadAsStringAsync().Result;
                var webInfo = JsonConvert.DeserializeObject<CreateWebModel>(JsonContent);

                var properRequest = false;
                var urlInfo = string.Empty;

                if (!string.IsNullOrEmpty(url) || !string.IsNullOrEmpty(webInfo.Url))
                {
                    properRequest = true;
                    urlInfo = !string.IsNullOrEmpty(url) ? url : webInfo.Url;
                }
                if (properRequest)
                {
                    using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteUrl, accessToken))
                    {
                        var web = clientContext.Site.OpenWeb(urlInfo);

                        clientContext.Load(web);
                        clientContext.ExecuteQuery();

                        web.DeleteObject();
                        clientContext.ExecuteQuery();

                        return new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new JsonContent(new
                            {
                                properties = new
                                {
                                    message = urlInfo + " web deleted"
                                }
                            })
                        };
                    }
                }
                else
                {
                    return req.CreateResponse(HttpStatusCode.BadRequest, "Bad request. Specify url to delete either in path or in body.");
                }
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Bad request. Customer site supports only get, put and delete verbs.");
            }

        }

        [FunctionName("events")]
        public static async Task<HttpResponseMessage> events([HttpTrigger(AuthorizationLevel.Anonymous, "get", "put",
            Route = "events/{title?}")]HttpRequestMessage req, string title, TraceWriter log)
        {
            var realm = TokenHelper.GetRealmFromTargetUrl(new Uri(siteUrl));

            //Get the access token for the URL.  
            var accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, new Uri(siteUrl).Authority, realm).AccessToken;

            //Get methods to get all Events created in all webs
            if (req.Method == HttpMethod.Get)
            {
                //All events created in all customer sites
                if (string.IsNullOrEmpty(title))
                {
                    using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteUrl, accessToken))
                    {                        

                        var subWebs = clientContext.Web.Webs;

                        clientContext.Load(subWebs);
                        clientContext.ExecuteQuery();

                        var response = new List<string>();

                        foreach (var subWeb in subWebs)
                        {
                            var eventList = subWeb.GetListByTitle("Events");
                            clientContext.Load(eventList);
                            clientContext.ExecuteQuery();

                            CamlQuery camlQuery = new CamlQuery();
                            camlQuery.ViewXml = "<View><RowLimit>100</RowLimit></View>";

                            var itemCollection = eventList.GetItems(camlQuery);

                            clientContext.Load(itemCollection, items => items.Include(
                        item => item.Id,
                        item => item.DisplayName));

                            clientContext.ExecuteQuery();

                            foreach (var item in itemCollection)
                            {
                                response.Add(subWeb.Title + ": " + item.DisplayName);
                            }
                        }

                        return new HttpResponseMessage(HttpStatusCode.Created)
                        {
                            Content = new JsonContent(new
                            {
                                properties = new
                                {
                                    events = response
                                }
                            })
                        };
                    }
                }
                //All events in a customer site
                else
                {
                    using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteUrl, accessToken))
                    {                        
                        var subWeb = clientContext.Site.OpenWeb(title);

                        clientContext.Load(subWeb);
                        clientContext.ExecuteQuery();

                        var response = new List<string>();

                        var eventList = subWeb.GetListByTitle("Events");
                        clientContext.Load(eventList);
                        clientContext.ExecuteQuery();

                        CamlQuery camlQuery = new CamlQuery();
                        camlQuery.ViewXml = "<View><RowLimit>100</RowLimit></View>";

                        var itemCollection = eventList.GetItems(camlQuery);

                        clientContext.Load(itemCollection, items => items.Include(
                    item => item.Id,
                    item => item.DisplayName));

                        clientContext.ExecuteQuery();

                        foreach (var item in itemCollection)
                        {
                            response.Add(item.DisplayName);
                        }


                        return new HttpResponseMessage(HttpStatusCode.Created)
                        {
                            Content = new JsonContent(new
                            {
                                properties = new
                                {
                                    site = subWeb.Title,
                                    events = response
                                }
                            })
                        };
                    }
                }


            }
            //Put method to broadcast event to all customers
            else if (req.Method == HttpMethod.Put)
            {
                var content = req.Content;
                string JsonContent = content.ReadAsStringAsync().Result;

                var eventInfo = JsonConvert.DeserializeObject<CreateEventModel>(JsonContent);

                using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteUrl, accessToken))
                {                    

                    var subWebs = clientContext.Web.Webs;

                    clientContext.Load(subWebs);
                    clientContext.ExecuteQuery();

                    foreach (var subWeb in subWebs)
                    {

                        clientContext.Load(subWeb);
                        clientContext.ExecuteQuery();

                        var eventsList = subWeb.Lists.GetByTitle("Events");

                        var itemCreateInfo = new ListItemCreationInformation();
                        var newItem = eventsList.AddItem(itemCreateInfo);
                        newItem["Title"] = title;
                        newItem["EventDate"] = eventInfo.StartTime;
                        newItem["EndDate"] = eventInfo.EndTime;
                        newItem["Description"] = eventInfo.Description;
                        newItem.Update();

                        clientContext.ExecuteQuery();

                    }

                    var response = "event posted";
                    var jsonResponse = JsonConvert.SerializeObject(response);

                    return new HttpResponseMessage(HttpStatusCode.Created)
                    {
                        Content = new JsonContent(new
                        {
                            properties = new
                            {
                                message = response,
                                title = title,
                                description = eventInfo.Description,
                                startTime = eventInfo.StartTime,
                                endTime = eventInfo.EndTime,
                            }
                        })
                    };
                }

            }
            else
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Bad request");
            }

        }
    }
}
