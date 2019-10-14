using System;
using System.Collections.Generic;
using System.Text;

namespace GenericSOAPCaller
{
    public class Response<T>
    {
        public string  Message{ get; set; }
        public bool  Status{ get; set; }
        public T  Data { get; set; }
    }
}
