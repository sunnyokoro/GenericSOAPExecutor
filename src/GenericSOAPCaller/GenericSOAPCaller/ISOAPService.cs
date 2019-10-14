using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GenericSOAPCaller
{
    //: class , new() this says TResponse is a class and new() says it has a default contructor. 
    //if new() is not there, when it ants to create an instance of T, it would throw up
    public interface ISOAPService<TResponse> where TResponse : class , new()
    {
        Task<Response<TResponse>> ExecuteAsync();

        Response<TResponse> Execute();

        Response<TResponse> ConvertXMLStringToObject(string xmlDoc);


    }

}
