using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web;
using RedditSolution.Models;

namespace RedditSolution.Controllers
{
    public class RedditServicesController : ApiController
    {
        public string Get()
        {
            return "Welcome To Web API";
        }
        public int GetLogin(string username, string password)
        {
            return FindUserToken(username, password);
        }
        public List<RedditList> GetListings(string userToken)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlNodeList xmlnodes;
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath("~/RedditUsers/RedditUsers.xml"), FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);
            xmlnodes = xmldoc.GetElementsByTagName("User");

            StreamReader strRdr = new StreamReader(HttpContext.Current.Server.MapPath("~/RedditDataSource/hot.json"));
            string data = strRdr.ReadToEnd();
            var reddit = JsonConvert.DeserializeObject<dynamic>(data);

            XmlNode xmlNodeFound = null;

            //Find if it is a valid Token
            for (int incr = 0; incr <= xmlnodes.Count - 1; incr++)
            {
                if (userToken == xmlnodes[incr].ChildNodes.Item(3).InnerText)
                {
                    xmlNodeFound = xmlnodes[incr];
                    break;
                }
            }

            if (xmlNodeFound == null)
                return null;

            RedditList lvlistings;
            List<RedditList> listings = new List<Models.RedditList>();

            foreach (var redditList in reddit.data.children)
            {
                lvlistings = new RedditList();
                lvlistings.reddit_id = redditList.data.id;
                lvlistings.permalink = redditList.data.permalink;
                lvlistings.url = redditList.data.url;
                lvlistings.author = redditList.data.author;

                listings.Add(lvlistings);
            }

            return listings;
        }
        public List<RedditUserFavorites> GetFavorites(string userToken)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlNodeList xmlnodes;
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath("~/RedditUsers/RedditUsers.xml"), FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);
            xmlnodes = xmldoc.GetElementsByTagName("User");

            StreamReader strRdr = new StreamReader(HttpContext.Current.Server.MapPath("~/RedditDataSource/hot.json"));
            string data = strRdr.ReadToEnd();
            var reddit = JsonConvert.DeserializeObject<dynamic>(data);

            XmlNode xmlNodeFound, xmlTags;
            xmlNodeFound = null;
            //Find if it is a valid Token
            for (int incr = 0; incr <= xmlnodes.Count - 1; incr++)
            {
                if (userToken == xmlnodes[incr].ChildNodes.Item(3).InnerText)
                {
                    xmlNodeFound = xmlnodes[incr];
                    break;
                }
            }

            if (xmlNodeFound == null)
                return null;

            RedditUserFavorites lvFavorite;
            List<RedditUserFavorites> userFavorites = new List<RedditUserFavorites>();

            xmlTags = xmlNodeFound.ChildNodes.Item(4).ChildNodes.Item(0);

            foreach (var redditList in reddit.data.children)
            {
                for (int index = 0; index < xmlTags.ChildNodes.Count; index++)
                {
                    if (xmlTags.ChildNodes[index].InnerText == Convert.ToString(redditList.data.id))
                    {
                        lvFavorite = new RedditUserFavorites();
                        lvFavorite.reddit_id = redditList.data.id;
                        lvFavorite.permalink = redditList.data.permalink;
                        lvFavorite.url = redditList.data.url;
                        lvFavorite.author = redditList.data.author;
                        lvFavorite.tag = xmlTags.ChildNodes[index].Name;

                        userFavorites.Add(lvFavorite);
                    }
                }

                //If all the Favorited items with Tags are captured, exit the loop
                if (xmlTags.ChildNodes.Count == userFavorites.Count)
                    break;
            }

            return userFavorites;
        }
        public int GetRegisteredToken(string username, string password)
        {
            int validUserToken = 0;
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                XmlNodeList xmlnodes;
                xmldoc.Load(HttpContext.Current.Server.MapPath("~/RedditUsers/RedditUsers.xml"));
                xmlnodes = xmldoc.GetElementsByTagName("User");

                int intUserCount = 1;
                for (int incr = 0; incr <= xmlnodes.Count - 1; incr++)
                {
                    if (username == xmlnodes[incr].ChildNodes.Item(1).InnerText)
                    {
                        validUserToken = Convert.ToInt32(xmlnodes[incr].ChildNodes.Item(3).InnerText);
                        break;
                    }
                    intUserCount++;
                }

                //If User is not found in the dataset, then register the user. Otherwise, return the existing Token
                if (validUserToken != 0)
                {
                    return validUserToken;
                }
                else
                {
                    XmlElement elmntUsers = (XmlElement)xmldoc.SelectSingleNode("//Users");
                    XmlElement elmntUserNew = xmldoc.CreateElement("User");

                    XmlElement elmntUserID = xmldoc.CreateElement("UserId");//Sequential ID incremented based on last User ID
                    elmntUserID.InnerText = intUserCount.ToString();
                    elmntUserNew.AppendChild(elmntUserID);

                    XmlElement elmntUserName = xmldoc.CreateElement("UserName");
                    elmntUserName.InnerText = username;
                    elmntUserNew.AppendChild(elmntUserName);

                    XmlElement elmntUserPass = xmldoc.CreateElement("Password");
                    elmntUserPass.InnerText = password;
                    elmntUserNew.AppendChild(elmntUserPass);

                    Random rangen = new Random();
                    int randNum = rangen.Next(1000000);
                    string sixDigitNumber = randNum.ToString("D6");
                    validUserToken = Convert.ToInt32(sixDigitNumber);

                    XmlElement elmntUserToken = xmldoc.CreateElement("UserToken");
                    elmntUserToken.InnerText = sixDigitNumber;
                    elmntUserNew.AppendChild(elmntUserToken);

                    XmlElement elmntTags = xmldoc.CreateElement("Tags");

                    XmlElement elmntRFavs = xmldoc.CreateElement("RFavs");
                    elmntRFavs.AppendChild(elmntTags);

                    elmntUserNew.AppendChild(elmntRFavs);

                    elmntUsers.AppendChild(elmntUserNew);
                    xmldoc.Save(HttpContext.Current.Server.MapPath("~/RedditUsers/RedditUsers.xml"));

                    return validUserToken;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        private int FindUserToken(string username, string password)
        {
            int validUserToken = 0;
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                XmlNodeList xmlnode;

                FileStream fs = new FileStream(HttpContext.Current.Server.MapPath("~/RedditUsers/RedditUsers.xml"), FileMode.Open, FileAccess.Read);

                xmldoc.Load(fs);
                xmlnode = xmldoc.GetElementsByTagName("User");

                for (int incr = 0; incr <= xmlnode.Count - 1; incr++)
                {
                    if (username == xmlnode[incr].ChildNodes.Item(1).InnerText &&
                        password == xmlnode[incr].ChildNodes.Item(2).InnerText)
                    {
                        validUserToken = Convert.ToInt32(xmlnode[incr].ChildNodes.Item(3).InnerText);
                        break;
                    }
                }

                return validUserToken;
            }
            catch (Exception)
            {
                return validUserToken;
            }
        }
        public void GetSavedResponse(string accessToken, string reddit_id, string tag)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlNodeList xmlnodes;
            xmldoc.Load(HttpContext.Current.Server.MapPath("~/RedditUsers/RedditUsers.xml"));
            xmlnodes = xmldoc.GetElementsByTagName("User");

            XmlNode xmlNodeFound, xmlTags;
            xmlNodeFound = null;
            for (int incr = 0; incr <= xmlnodes.Count - 1; incr++)
            {
                if (accessToken == xmlnodes[incr].ChildNodes.Item(3).InnerText)
                {
                    xmlNodeFound = xmlnodes[incr];
                    break;
                }
            }

            if (xmlNodeFound != null)
            {
                xmlTags = xmlNodeFound.ChildNodes.Item(4).ChildNodes.Item(0);

                XmlElement xmlelmntNewTag = xmldoc.CreateElement(tag);
                xmlelmntNewTag.InnerText = reddit_id;

                xmlTags.AppendChild(xmlelmntNewTag);
                xmldoc.Save(HttpContext.Current.Server.MapPath("~/RedditUsers/RedditUsers.xml"));
            }
        }
    }
}
