using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace GenericSOAPCaller.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            ProcessFile_1();
            //ProcessFile_2();
        }

        private static XmlDocument ReturnResponseXmlDoc(string xmlString)
        {
            XmlDocument toReturn;
            try
            {
                toReturn = new XmlDocument();
                toReturn.LoadXml(xmlString);
                return toReturn;
            }
            catch (Exception ex)
            { 
                return null;
            }
        }

        private static void ProcessFile_2()
        {
            var file = File.ReadAllText("xmlFileList.txt");

            var result = ReturnResponseXmlDoc(file);

            XMLMapper mapper = new XMLMapper();
            var result2 =  mapper.ExecuteAsync<List<GetStatusReturn>>().Result

            /*
            var url = "http://sandbox.w3sghana.com:8500/dr3/dataexchange/main/partner/index.cfc";

            var respXMLTag = "GetStatusReturn";

            var nameSpace = "dr.ecobank.com";

            var obj = new SOAPService<GetStatusReturn>(url, file, respXMLTag, nameSpace);

            obj.ConvertXMLStringToObject(file);

            //SYNC CALL
            var result = obj.Execute();

            //ASYNC CALL
            var result2 = obj.ExecuteAsync().Result;

            if (!result.Status)
            {
                Console.WriteLine(result.Message);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("The status is " + result.Data);
            */

            //Console.ReadLine();
        }


        private static void ProcessFile_1()
        {
            var file = File.ReadAllText("xmlFile.txt");

            var url = "http://sandbox.w3sghana.com:8500/dr3/dataexchange/main/partner/index.cfc";

            var respXMLTag = "GetStatusReturn";

            var nameSpace = "dr.ecobank.com";

            var obj = new SOAPService<GetStatusReturn>(url, file, respXMLTag, nameSpace);

            obj.ConvertXMLStringToObject(file);

            //SYNC CALL
            var result = obj.Execute();

            //ASYNC CALL
            var result2 = obj.ExecuteAsync().Result;

            if (!result.Status)
            {
                Console.WriteLine(result.Message);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("The status is " + result.Data);


            Console.ReadLine();
        }

        public class GetStatusReturn
        {
            public string ChannelRef { get; set; }
            public string StatusCode { get; set; }
            public string StatusDescription { get; set; }
            public string TransactionRef { get; set; }
        }

        public class Section
        { 
            public Headers Headers { get; set; }  
        }

        public class Headers
        {
            public string P1 { get; set; }
            public string P2 { get; set; } 
        }
    }
}
