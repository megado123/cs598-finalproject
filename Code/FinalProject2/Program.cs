using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;



namespace FinalProject2
{
    class Program
    {
        static void Main(string[] args)
        {
            //populate variables for file locations
            string fileAlocal = @"C:\Users\a05v6zz\Documents\University of Illinois\CS 598 - Data Curration\Final Project\Original_Consumer_Complaints_FileA.xml";
            string fileA = @"https://d18ky98rnyall9.cloudfront.net/PCUSZ6AVEeiH_Q6QLzoKjg_3c8e8290a01511e898f681e1a8f1a50f_Consumer_Complaints_FileA.xml?Expires=1543708800&Signature=PeSfTSPC8Fas2WrlOvalVwSkTXhA4pUjb8zQzPFcqH7CNFiG8GKxhB7QbsjLrXAK8dSu8-ZcW-mf9w7LWuZukbNn7SYVmNlbXZjlUQJv7CzzUfjy4PfRWThHFc-Ounyckhl5~LXPhwz-w9vwwwrI22Wvq~D3cLJ1ZfTP646DNng_&Key-Pair-Id=APKAJLTNE6QMUY6HBC5A";

            string fileA_RemovedAttributes = @"C:\Users\a05v6zz\Documents\University of Illinois\CS 598 - Data Curration\Final Project\Consumer_Complaints_FileA_NoAttributesv2.xml";
            string fileBlocal = @"C:\Users\a05v6zz\Documents\University of Illinois\CS 598 - Data Curration\Final Project\Original_Consumer_Complaints_FileB.xml";
            string fileB = @"https://d18ky98rnyall9.cloudfront.net/PCWuqKAVEeiH_Q6QLzoKjg_3c454490a01511e8928e63caf432cec5_Consumer_Complaints_FileB.xml?Expires=1543708800&Signature=MHLTF81PYI8in~eelklxXB--S8BNXEMR-h32abffaEydKxCDxcENjbI-3NXcAvLpI-ObLde7Wp1yDJscwkTsyixYIyGUY5Etcr1bNlGzgOT73fr2QuLzJyEoCKBzC0NsUJsmgFgFkVclIoJ7~~wHWzhw75JFEyl-Rx79NVLQrAQ_&Key-Pair-Id=APKAJLTNE6QMUY6HBC5A";

            string fileB_RemovedAttributes = @"C:\Users\a05v6zz\Documents\University of Illinois\CS 598 - Data Curration\Final Project\Consumer_Complaints_FileB_NoAttributesv2.xml";
            string transform = @"C:\Users\a05v6zz\source\repos\FinalProject2\FinalProject2\DataTransformplayground2.xslt";


            //this method requires local files - returns the MD5 checksum
            var valueA = checkMD5(fileAlocal);
            var valueB = checkMD5(fileBlocal);

    

            //Step 1: Apply Generic Transform
            ApplyTransform(fileA, fileA_RemovedAttributes, transform);
            ApplyTransform(fileB, fileB_RemovedAttributes, transform);

            //Step 2: Clean through Json & create a string and file.
            //taking XML files and saving as json files - not required to save as file, but done for completeness
            string fileJsonA = @"c:\temp\FileAplayground.json";
            string fileJsonB = @"c:\temp\FileBplayground.json";

            string jsonA = GenerateJsonObjectFromFile(fileA_RemovedAttributes, fileJsonA);
            string jsonB = GenerateJsonObjectFromFile(fileB_RemovedAttributes, fileJsonB);


            //clean JsonA string
            var myclassA = Newtonsoft.Json.JsonConvert.DeserializeObject<FinalProjectA.RootObject>(jsonA);
            myclassA.consumerComplaints.complaint = myclassA.consumerComplaints.complaint.OrderBy(c => c.id).ToList();

            //clean JsonB string
            var myclassB = Newtonsoft.Json.JsonConvert.DeserializeObject<FinalProjectB.RootObject>(jsonB);
            myclassB.consumerComplaints.complaint = myclassB.consumerComplaints.complaint.OrderBy(c => c.id).ToList();

            //mapping for fileA
            foreach (FinalProjectA.Complaint c in myclassA.consumerComplaints.complaint)
            {
                string submitted = c.submitted.via;
                c.submissionType = submitted;
                c.submitted.via = null;

                foreach (FinalProjectA.Event e in c.@event)
                {
                    if (e.type == "received")
                    {
                        c.receivedDate = e.date;
                    }
                    else
                        if (e.type == "sentToCompany")
                    {
                        c.sentToCompanyDate = e.date;
                    }
                }
            }
            jsonA = Newtonsoft.Json.JsonConvert.SerializeObject(myclassA, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(fileJsonA, jsonA);
            WriteToXmlFileFileA(@"c:\temp\FileA.xml", myclassA.consumerComplaints);


            //mapping for fileB
            foreach (FinalProjectB.Complaint c in myclassB.consumerComplaints.complaint)
            {
                if (c.response.timely == "no")
                    c.response.timely = "N";
                else
                    c.response.timely = "Y";

                foreach (FinalProjectB.Event e in c.@event)
                {
                    if (e.type == "received")
                    {
                        c.receivedDate = e.date;
                    }
                    else
                        if (e.type == "sentToCompany")
                        {
                            c.sentToCompanyDate = e.date;
                        }
                }
            }
            jsonB = Newtonsoft.Json.JsonConvert.SerializeObject(myclassB, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(fileJsonB, jsonB);
            WriteToXmlFileFileB(@"c:\temp\FileB.xml", myclassB.consumerComplaints);


            var valueA2 = checkMD5(@"c:\temp\FileA.xml");
            var valueB2 = checkMD5(@"c:\temp\FileB.xml");

        }

        /// <summary>
        /// Method to wrote XML to file removing namespace, providing indentation, entitizing character return
        /// and ensuring UTF8 Encoding
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        public static void WriteToXmlFileFileA(string filePath, FinalProjectA.ConsumerComplaints obj)
        {
        
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(); ns.Add("", "");
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineHandling = NewLineHandling.Entitize,
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
            };

            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {

                new XmlSerializer(typeof(FinalProjectA.ConsumerComplaints)).Serialize(writer, obj, ns);
            }

        }

        /// <summary>
        /// Method to wrote XML to file removing namespace, providing indentation, entitizing character return
        /// and ensuring UTF8 Encoding
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        public static void WriteToXmlFileFileB(string filePath, FinalProjectB.ConsumerComplaints obj)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces(); ns.Add("", "");
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineHandling = NewLineHandling.Entitize,
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,


            };

            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {

                new XmlSerializer(typeof(FinalProjectB.ConsumerComplaints)).Serialize(writer, obj, ns);
            }
        }
   
        /// <summary>
        /// Performing MD5 checksum
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string checkMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    //represent the hash as a string, you could convert it to hex using BitConverter
                    var hash = md5.ComputeHash(stream);
                    return System.BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private static void ApplyTransform(string inputFile, string outputFile, string transform)
        {
            var myXslTrans = new System.Xml.Xsl.XslCompiledTransform();
            System.Xml.XmlReaderSettings settings = new System.Xml.XmlReaderSettings();
            //send transform to handle the DTD
            settings.DtdProcessing = DtdProcessing.Parse;

            //load the xslt tranform file
            myXslTrans.Load(transform);

            //tranforming file
            var input = XmlReader.Create(inputFile, settings);
            var writer = XmlWriter.Create(outputFile);
            myXslTrans.Transform(input, writer);
            input.Dispose();
            writer.Dispose();
        }

        /// <summary>
        /// Generate JsonObject from an XML File, Ouptut file is not required for this implmentation
        /// but could be considered valuable as part of the workflow
        /// </summary>
        /// <param name="inputXMLfile"></param>
        /// <param name="outputJsonFile"></param>
        /// <returns></returns>
        private static string GenerateJsonObjectFromFile(string inputXMLfile, string outputJsonFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(inputXMLfile);
            XmlNode rootNode = doc.DocumentElement;
        
            string json = JsonConvert.SerializeXmlNode(rootNode, Newtonsoft.Json.Formatting.Indented, false);
            System.IO.File.WriteAllText(outputJsonFile, json);
            return json;

        }
    
    }
}