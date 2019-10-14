using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GenericSOAPCaller
{

    //: class , new() this says TResponse is a class and new() says it has a default contructor. 
    //if new() is not there, when it ants to create an instance of T, it would throw up
    public class SOAPService<TResponse> : ISOAPService<TResponse> where TResponse : class, new()
    {
        string _xmlDocument;
        string _responseXMLTagName;
        string _url;
        string _soapNameSpace;
        string _soapAction;

        public SOAPService(string url, string xmlDocument, string responseXMLTagName, string soapNameSpace, string soapAction = "")
        {
            _xmlDocument = xmlDocument;
            _url = url;
            _responseXMLTagName = responseXMLTagName;
            _soapNameSpace = soapNameSpace;
            _soapAction = soapAction;
        }

        public Response<TResponse> Execute()
        {
            var xmlMapper = xmlMapperBuilder();

            var response = xmlMapper.Execute<TResponse>();

            return new Response<TResponse>
            {
                Message = xmlMapper.ErrorMessage,
                Status = response != null,
                Data = response
            };
        }

        private XMLMapper xmlMapperBuilder()
        {
            //add validation of these required variables

            var xmlMapper = new XMLMapper();
            xmlMapper.restUrl = _url;
            xmlMapper.soapAction = _soapAction;
            xmlMapper.xmldoc = _xmlDocument;
            xmlMapper.soapNameSpace = _soapNameSpace;
            xmlMapper.responseMessageTakName = _responseXMLTagName;
            return xmlMapper;
        }

        public async Task<Response<TResponse>> ExecuteAsync()
        {

            var xmlMapper = xmlMapperBuilder();

            var response = await xmlMapper.ExecuteAsync<TResponse>();

            return await Task.FromResult(new Response<TResponse>()
            {
                Message = xmlMapper.ErrorMessage,
                Status = response != null,
                Data = response
            });
        }

        public Response<TResponse> ConvertXMLStringToObject(string xmlDoc)
        {
            if(!xmlDoc.StartsWith("<"))
            {
                return new Response<TResponse>()
                {
                    Message = "Ensure this is a valid XML string" 
                };
            }

            var xmlMapper = xmlMapperBuilder();

            var response = xmlMapper.XMLSTringToObject<TResponse>(xmlDoc);

            return new Response<TResponse>()
            {
                Message = xmlMapper.ErrorMessage,
                Status = response != null,
                Data = response
            };
        }
    }
}
