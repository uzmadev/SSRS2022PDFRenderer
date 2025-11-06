/*
 *  CustomPDFProvider.cs
 *  Part of SqlServer2022-RdlcPdfRenderer
 *
 *  Copyright (C) 2025 Uzma Ashraf
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Affero General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Affero General Public License for more details.
 *
 *  You should have received a copy of the GNU Affero General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Linq;
using System.Text;
using Microsoft.ReportingServices.OnDemandReportRendering;
using System.IO;
using System.Security;

[assembly: SecurityRules(SecurityRuleSet.Level1)]
[assembly: AllowPartiallyTrustedCallers]

namespace PDFCustom
{
    class CustomPDFProvider : IRenderingExtension
    {
        #region VARIABLES
        private string _name;
        private string _extension;
        private Encoding _encoding;
        private string _mimeType;
        private bool _willSeek;
        private Microsoft.ReportingServices.Interfaces.StreamOper _operation;
        private Stream intermediateStream;
        #endregion

        public Stream IntermediateCreateAndRegisterStream(
           string name,
           string extension,
           Encoding encoding,
           string mimeType,
           bool willSeek,
           Microsoft.ReportingServices.Interfaces.StreamOper operation)
        {
            _name = name;
            _encoding = encoding;
            _extension = extension;
            _mimeType = mimeType;
            _operation = operation;
            _willSeek = willSeek;

            // Create and return a new MemoryStream,
            // which will contain the results of the PDF renderer.
            intermediateStream = new MemoryStream();

            return intermediateStream;
        }
        public void GetRenderingResource(Microsoft.ReportingServices.Interfaces.CreateAndRegisterStream createAndRegisterStreamCallback, System.Collections.Specialized.NameValueCollection deviceInfo)
        {
        }

        public bool Render(Microsoft.ReportingServices.OnDemandReportRendering.Report report,
            System.Collections.Specialized.NameValueCollection reportServerParameters,
            System.Collections.Specialized.NameValueCollection deviceInfo,
            System.Collections.Specialized.NameValueCollection clientCapabilities,
            ref System.Collections.Hashtable renderProperties,
            Microsoft.ReportingServices.Interfaces.CreateAndRegisterStream createAndRegisterStream)
        {

            #region Local Variables
            string certificatePath = null;
            string certificatePassword = null;
            string signatureImageLocation = null;
            string ownerPassword = null;
            bool passwordProtected = false;
            bool allowPrint = false;
            bool allowDocumentAssembly = false;
            bool allowContentCopying = false;
            bool allowContentCopyingForAccessibility = false;
            bool allowCommenting = false;
            bool allowFillingFormFields = false;
            bool allowModifyContents = false;
            bool signatureVisibility = false;
            string loggingFileLocation = null;
            string passwordParameterName = null;
            string assemblyName = null;
            byte[] finalpdf;
            string openPassword = null;

            #endregion




            #region Retrieve data From Config

            #region Logging Configuration

            if (deviceInfo["LoggingFileLocation"] != null)
                loggingFileLocation = deviceInfo["LoggingFileLocation"];

            if (!string.IsNullOrEmpty(loggingFileLocation))
            {
                UtilMethod.folderLocation = System.IO.Path.GetDirectoryName(loggingFileLocation);
                UtilMethod.fileName = System.IO.Path.GetFileNameWithoutExtension(loggingFileLocation);
                UtilMethod.extension = System.IO.Path.GetExtension(loggingFileLocation);
            }
            else
            {
                UtilMethod.folderLocation = UtilMethod.fileName = null;
            }

            #endregion

            if (deviceInfo["CertificatePath"] != null)
            {
                certificatePath = deviceInfo["CertificatePath"];
                UtilMethod.LogMessageToFile("Has CertificatePath tag");
            }
            else
            {
                UtilMethod.LogMessageToFile("No CertificatePath tag");
            }

            if (deviceInfo["CertificatePassword"] != null)
            {
                certificatePassword = deviceInfo["CertificatePassword"];
                UtilMethod.LogMessageToFile("Has CertificatePassword tag");
            }

            else
            {
                UtilMethod.LogMessageToFile("No CertificatePassword tag");
            }


            if (deviceInfo["SignatureImageLocation"] != null)
            {
                signatureImageLocation = deviceInfo["SignatureImageLocation"];
                UtilMethod.LogMessageToFile("Has SignatureImageLocation tag");
            }
            else
            {
                UtilMethod.LogMessageToFile("No SignatureImageLocation tag");
            }

            if (deviceInfo["AllowPrint"] != null)
            {
                allowPrint = Convert.ToBoolean(deviceInfo["AllowPrint"]);
                UtilMethod.LogMessageToFile("Has AllowPrint tag");
            }
            else
            {
                UtilMethod.LogMessageToFile("No AllowPrint tag");
            }

            if (deviceInfo["AllowDocumentAssembly"] != null)
            {
                allowDocumentAssembly = Convert.ToBoolean(deviceInfo["AllowDocumentAssembly"]);
                UtilMethod.LogMessageToFile("Has AllowDocumentAssembly tag");

            }
            else
            {
                UtilMethod.LogMessageToFile("No AllowDocumentAssembly tag");
            }

            if (deviceInfo["AllowContentCopying"] != null)
            {
                allowContentCopying = Convert.ToBoolean(deviceInfo["AllowContentCopying"]);
                UtilMethod.LogMessageToFile("Has AllowContentCopying tag");
            }

            else
            {
                UtilMethod.LogMessageToFile("No AllowContentCopying tag");
            }

            if (deviceInfo["AllowContentCopyingForAccessibility"] != null)
            {
                allowContentCopyingForAccessibility = Convert.ToBoolean(deviceInfo["AllowContentCopyingForAccessibility"]);
                UtilMethod.LogMessageToFile("Has AllowContentCopyingForAccessibility tag");

            }
            else
            {
                UtilMethod.LogMessageToFile("No AllowContentCopyingForAccessibility tag");
            }

            if (deviceInfo["AllowCommenting"] != null)
            {
                allowCommenting = Convert.ToBoolean(deviceInfo["AllowCommenting"]);
                UtilMethod.LogMessageToFile("Has AllowCommenting tag");
            }
            else
            {
                UtilMethod.LogMessageToFile("No AllowCommenting tag");
            }

            if (deviceInfo["AllowModifyContents"] != null)
            {
                allowModifyContents = Convert.ToBoolean(deviceInfo["AllowModifyContents"]);
                UtilMethod.LogMessageToFile("Has AllowModifyContents tag");
            }
            else
            {
                UtilMethod.LogMessageToFile("No AllowModifyContents tag");
            }

            if (deviceInfo["AllowFillingFormFields"] != null)
            {
                allowFillingFormFields = Convert.ToBoolean(deviceInfo["AllowFillingFormFields"]);
                UtilMethod.LogMessageToFile("Has AllowFillingFormFields tag");
            }
            else
            {
                UtilMethod.LogMessageToFile("No AllowFillingFormFields tag");
            }

            if (deviceInfo["SignatureVisiblity"] != null)
            {
                signatureVisibility = Convert.ToBoolean(deviceInfo["SignatureVisiblity"]);
                UtilMethod.LogMessageToFile("Has SignatureVisiblity tag");
            }
            else
            {
                UtilMethod.LogMessageToFile("No SignatureVisiblity tag");

            }

            if (deviceInfo["PasswordProtected"] != null)
            {
                passwordProtected = Convert.ToBoolean(deviceInfo["PasswordProtected"]);
                UtilMethod.LogMessageToFile("Has PasswordProtected tag");
            }
            else
            {
                UtilMethod.LogMessageToFile("No PasswordProtected tag");

            }
            if (deviceInfo["PDFOwnerPassword"] != null)
            {
                ownerPassword = deviceInfo["PDFOwnerPassword"];
                UtilMethod.LogMessageToFile("Has PDFOwnerPassword tag");
            }
            else
            {
                UtilMethod.LogMessageToFile("No PDFOwnerPassword tag");

            }


            if (deviceInfo["PasswordParameterName"] != null)
            {
                passwordParameterName = deviceInfo["PasswordParameterName"];
                UtilMethod.LogMessageToFile("Has PasswordParameterName tag");
            }
            else
            {
                UtilMethod.LogMessageToFile("No PasswordParameterName tag");
            }

            if (deviceInfo["AssemblyName"] != null)
            {
                assemblyName = deviceInfo["AssemblyName"];
                UtilMethod.LogMessageToFile("Has AssemblyName tag");
            }
            else
            {
                UtilMethod.LogMessageToFile("No AssemblyName tag");
            }

            UtilMethod.LogMessageToFile("reportParamter Count : " + (report.Parameters != null ? "" + report.Parameters.Count : "0"));
            if (!string.IsNullOrEmpty(passwordParameterName) && report.Parameters != null)
            {
                var reportParam = report.Parameters.FirstOrDefault(x => x.Name == passwordParameterName);
                if (reportParam != null)
                {
                    openPassword = reportParam.Instance.Value.ToString();
                }
                else
                {
                    UtilMethod.LogMessageToFile("Parameter Does not exist");
                }
            }



            if (string.IsNullOrEmpty(assemblyName))
                assemblyName = "Microsoft.ReportingServices.ImageRendering, Version=2022.1.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
                //assemblyName = "Microsoft.ReportingServices.ImageRendering, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

            #endregion

            UtilMethod.LogMessageToFile("Render Called");
            UtilMethod.LogMessageToFile("Before Base Class Render");


            PdfSubRenderer subR = new PdfSubRenderer(assemblyName);
            subR.pdfRenderer.Render(
              report,
              reportServerParameters,
              deviceInfo,
              clientCapabilities,
              ref renderProperties,
              new Microsoft.ReportingServices.Interfaces.CreateAndRegisterStream(IntermediateCreateAndRegisterStream)
          );
            UtilMethod.LogMessageToFile("After Base Class Render");
            Stream outputStream =
                createAndRegisterStream(_name, _extension, _encoding, _mimeType, _willSeek, _operation);

            intermediateStream.Position = 0;
            // transfer data from intermediateStream to an auxiliary Stream,
            // intermediate Stream data gets corrupted if directly used.
            MemoryStream tempStream = new MemoryStream();
            byte[] tempbuffer = new byte[32768];
            while (true)
            {
                int read = intermediateStream.Read(tempbuffer, 0, tempbuffer.Length);
                if (read <= 0) break;
                tempStream.Write(tempbuffer, 0, read);
            }






            PdfHelper pdfWorker = new PdfHelper();
            // Seeting PDF permissions
            int permissions = pdfWorker.SetPermissions(allowDocumentAssembly, allowContentCopying, allowFillingFormFields
                   , allowCommenting, allowModifyContents, allowPrint, allowContentCopyingForAccessibility);


            if (!string.IsNullOrEmpty(ownerPassword))
            {
                // Digitally Signed and Password Protected
                if (passwordProtected && !string.IsNullOrEmpty(openPassword) && !string.IsNullOrEmpty(ownerPassword))
                {
                    if (!string.IsNullOrEmpty(certificatePassword) && !string.IsNullOrEmpty(certificatePath))
                    {
                        if (File.Exists(certificatePath))
                        {
                            UtilMethod.LogMessageToFile("Certificate Path Exists.");

                            finalpdf = pdfWorker.PasswordProtectPDFwithDigitallySigned(tempStream.ToArray(),
                                                       certificatePath, certificatePassword, openPassword, ownerPassword,
                                                       signatureVisibility, signatureImageLocation, permissions);
                        }
                        else
                        {
                            UtilMethod.LogMessageToFile("Certficate Path is Invalid");
                            finalpdf = pdfWorker.GetEncryptedPDF(tempStream.ToArray(), openPassword, ownerPassword, permissions,
                                                                 passwordProtected);
                        }
                        if (finalpdf == null) // if password is incorrect and sends null so make the pdf only password protected.
                        {
                            UtilMethod.LogMessageToFile("final pdf is received null in Digitally Signed and Password Protected ");
                            finalpdf = pdfWorker.GetEncryptedPDF(tempStream.ToArray(), openPassword, ownerPassword, permissions,
                                                                 passwordProtected);
                        }
                    }
                    else // if certificate password and path is null or empty then  only password protected
                    {
                        UtilMethod.LogMessageToFile("certificate password and path is null or empty");
                        finalpdf = pdfWorker.GetEncryptedPDF(tempStream.ToArray(), openPassword, ownerPassword, permissions,
                                                                 passwordProtected);
                    }
                }
                //Only digitally signed.
                else if (!string.IsNullOrEmpty(certificatePassword) && !string.IsNullOrEmpty(certificatePath) && !string.IsNullOrEmpty(ownerPassword) && File.Exists(certificatePath))
                {
                    UtilMethod.LogMessageToFile("Only digitally signed.");
                    // if signature visibilty is true and signature image does exist.
                    if (signatureVisibility && string.IsNullOrEmpty(signatureImageLocation) && File.Exists(signatureImageLocation))
                        finalpdf = pdfWorker.SignPdfWithImage(tempStream.ToArray(), certificatePath, certificatePassword, signatureImageLocation,
                                                    ownerPassword, permissions);
                    else
                    { // make the pdf digitally signed and no visible signatures.
                        UtilMethod.LogMessageToFile("visible signatures are false");
                        finalpdf = pdfWorker.SignPdfWithoutImage(tempStream.ToArray(), certificatePath, certificatePassword, ownerPassword, permissions);
                    }

                    if (finalpdf == null) // in case of certificate password is incorrect, then
                        finalpdf = tempStream.ToArray();

                }

                else // precaution check, if everything is missing except owner password.
                {
                    UtilMethod.LogMessageToFile("Neither password protected worked nor digitally signed");
                    finalpdf = tempStream.ToArray();
                }
            }
            else // if owner password is missing.
            {
                UtilMethod.LogMessageToFile("Owner Password is missing copying base class pdf data to final stream.");
                finalpdf = tempStream.ToArray();
            }

            outputStream.Write(finalpdf, 0, finalpdf.ToArray().Length);

            return false;
        }

        public bool RenderStream(string streamName, Microsoft.ReportingServices.OnDemandReportRendering.Report report, System.Collections.Specialized.NameValueCollection reportServerParameters, System.Collections.Specialized.NameValueCollection deviceInfo, System.Collections.Specialized.NameValueCollection clientCapabilities, ref System.Collections.Hashtable renderProperties, Microsoft.ReportingServices.Interfaces.CreateAndRegisterStream createAndRegisterStream)
        {
            return false;
        }

        public string LocalizedName
        {
            get { return "Custom Render"; }
        }

        public void SetConfiguration(string configuration)
        {
            //UtilMethod.LogMessageToFile("SetConfiguration Called");
        }
    }
}
