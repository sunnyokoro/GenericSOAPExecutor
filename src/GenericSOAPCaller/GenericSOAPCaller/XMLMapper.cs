using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace GenericSOAPCaller
{
    public class XMLMapper
    {
        public String xmldoc { get; set; }
        public String restUrl { get; set; }
        public String soapAction { get; set; }
        public String soapNameSpace { get; set; }
        public String responseMessageTakName { get; set; } //the class that holds the response messsage, the one we are interested in reading   

        public string ErrorMessage { get; set; } = "";

        public async Task<T> ExecuteAsync<T>() where T : class, new()
        {
            try
            {
                var xmlfulldoc = new StringBuilder();
                string xmlValue = string.Empty;
                var xmlreq = xmldoc.ToString();

                var client = new RestClient(restUrl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("soapaction", soapAction);
                request.AddHeader("content-type", "text/xml");

                request.AddParameter("text/xml", xmlreq, ParameterType.RequestBody);
                ErrorMessage = "";
                //service call
                //Log.Information($"LoadData:: {MethodName} Request: {xmlreq}");
                var response = Task.Run(() => client.Execute(request)).Result; //run async
                // Log.Information($"LoadData:: {MethodName} Response {response.Content}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var xmlDoc = await Task.Run(() => ReturnResponseXmlDoc(response.Content));

                    if (xmlDoc == null)
                    {
                        return default(T);
                    }
                      
                    return await Task.Run(()=> DeserializeXMLDoc<T>(xmlDoc, responseMessageTakName, soapNameSpace));
                }

                ErrorMessage = response.ErrorMessage;
                //Log.Information($"LoadData:: {MethodName} Message {response.ErrorMessage}");
                return default(T);
            }

            catch (Exception ex)
            {
                // Log.Error(ex, ex.Message);
                ErrorMessage = ex.Message;
                return default(T);
            }
        }

        public T Execute<T>() where T : class, new()
        {
            try
            {
                var xmlfulldoc = new StringBuilder();
                string xmlValue = string.Empty;
                var xmlreq = xmldoc.ToString();

                var client = new RestClient(restUrl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("soapaction", soapAction);
                request.AddHeader("content-type", "text/xml");

                request.AddParameter("text/xml", xmlreq, ParameterType.RequestBody);
                ErrorMessage = "";
                //service call
                //Log.Information($"LoadData:: {MethodName} Request: {xmlreq}");
                var response = Task.Run(() => client.Execute(request)).Result; //run async
                // Log.Information($"LoadData:: {MethodName} Response {response.Content}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var xmlDoc =   ReturnResponseXmlDoc(response.Content);

                    if (xmlDoc == null)
                    {
                        return default(T);
                    }

                    var nameSpace = soapNameSpace;

                    return DeserializeXMLDoc<T>(xmlDoc, responseMessageTakName, nameSpace);
                }

                ErrorMessage = response.ErrorMessage;
                //Log.Information($"LoadData:: {MethodName} Message {response.ErrorMessage}");
                return default(T);
            }

            catch (Exception ex)
            {
                // Log.Error(ex, ex.Message);
                ErrorMessage = ex.Message;
                return default(T);
            }
        }

        public T XMLSTringToObject<T>(string xmlDoc) where T : class, new()
        {
            var doc = ReturnResponseXmlDoc(xmlDoc);

            if (xmlDoc == null)
            {
                return default(T);
            }

            var nameSpace = soapNameSpace;

            return DeserializeXMLDoc<T>(doc, responseMessageTakName, nameSpace);
        }

        private XmlDocument ReturnResponseXmlDoc(string xmlResponseString)
        {
            XmlDocument toReturn;
            try
            {
                toReturn = new XmlDocument();
                toReturn.LoadXml(xmlResponseString);
                return toReturn;
            }
            catch (Exception ex)
            {
                //Log.Error(ex, ex.Message);
                ErrorMessage = "There was an error loading the xml string.";
                return null;
            }
        }

        private TResponse DeserializeXMLDoc<TResponse>(XmlDocument xmlResponse, string responseClass, string nameSpace) where TResponse : class, new()
        {
            try
            {
                var respNode = xmlResponse.GetElementsByTagName(responseClass, nameSpace)[0];
                List<XElement> elems = new List<XElement>();
                try
                {
                    foreach (XmlNode node in respNode.ChildNodes)
                    {
                        using (XmlNodeReader nodeReader = new XmlNodeReader(node))
                        {
                            nodeReader.MoveToContent();
                            XElement xRoot = XElement.Load(nodeReader);
                            elems.Add(xRoot);
                        }
                    }
                }
                catch (Exception ex)
                {

                    ErrorMessage = "Ensure your ResponseXMLTag and Namespace are correct ";
                    return default(TResponse);
                }

                //PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(TResponse));
                //TResponse response = (TResponse)Activator.CreateInstance(typeof(TResponse));
                 
                //this is possible cos we used added a restriction on type TResponse and added new()
                TResponse response = new TResponse();
                var properties = response.GetType().GetProperties();

                try
                {
                    foreach (var prop in properties)
                    {
                        var elem = elems.FirstOrDefault(a => a.Name.LocalName == prop.Name);
                        if (elem != null)
                        {
                            //prop.SetValue(xmlResponse, elem.Value); 
                            prop.SetValue(response, elem.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = "An error occured  while reading XML response tags";
                    return default(TResponse);
                }

                return response;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return default(TResponse);
            }
        }



    }
}
