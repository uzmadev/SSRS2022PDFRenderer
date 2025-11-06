/*
 *  PdfHelper.cs
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

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;


namespace PDFCustom
{
    class PdfHelper
    {
        /// <summary>
        /// Signs the input PDF without Image. With the Certificate provided as parameter and set permissions to the pdf.
        /// </summary>
        /// <param name="intermediatePDFBuffer">Containing PDF Data</param>
        /// <param name="certificatePath">Path of the Signature Certificate passed in the configuration</param>
        /// <param name="certificatePassword">Password of the certificate </param>
        /// <param name="ownerPassword"> password to edit permissions</param>
        /// <param name="permissions"> permissions for pdf.</param>
        /// <returns>byte array which contains Signed PDF without Image</returns>
 
        public byte[] SignPdfWithoutImage(byte[] intermediatePDFBuffer, string certificatePath, string certificatePassword,
                                      string ownerPassword, int permissions)
        {
            try
            {
                Pkcs12Store store;
                //Open the certificate.
                FileStream fs = new FileStream(certificatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //search the certificate in certificate Store (certmgr.msc)
                UtilMethod.LogMessageToFile("Searching for certificate");
                try
                {
                    store = new Pkcs12Store(fs, certificatePassword.ToCharArray());
                }
                catch (Exception ex)
                {

                    UtilMethod.LogMessageToFile(ex);
                    return null;
                }
                finally
                {
                    fs.Close();
                    fs = null;
                }
                string alias = null;
                ICollection<X509Certificate> chain = new List<X509Certificate>();
                // searching for private key
                UtilMethod.LogMessageToFile("Searching for private key");
                foreach (string al in store.Aliases)
                    if (store.IsKeyEntry(al) && store.GetKey(al).Key.IsPrivate)
                    {
                        alias = al;
                        break;
                    }
                AsymmetricKeyEntry pk = store.GetKey(alias);
                foreach (X509CertificateEntry c in store.GetCertificateChain(alias))
                    chain.Add(c.Certificate);
                RsaPrivateCrtKeyParameters parameters = pk.Key as RsaPrivateCrtKeyParameters;
                //Read the PDF 
                UtilMethod.LogMessageToFile("Reading PDF inside Signature without Image function");
                PdfReader Reader = new PdfReader(intermediatePDFBuffer);
                //Output MemoryStream which will contain the signed PDF
                MemoryStream tryStream = new MemoryStream();
                // essential for signing the document.
                UtilMethod.LogMessageToFile("Next is to try create stamper");
                PdfStamper stamper = PdfStamper.CreateSignature(Reader, tryStream, '\0');
                iTextSharp.text.Rectangle size = Reader.GetPageSizeWithRotation(1);

                stamper.SetEncryption(true, null, ownerPassword, permissions);

                // Creating the appearance
                UtilMethod.LogMessageToFile("Signature Appearance");
                PdfSignatureAppearance appearance = stamper.SignatureAppearance;


                // Creating the signature
                UtilMethod.LogMessageToFile("Creating Signature ");
                IExternalSignature pks = new PrivateKeySignature(pk.Key, DigestAlgorithms.SHA256);

                //Signing the PDF.
                UtilMethod.LogMessageToFile("Signing the PDF start");
                MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, CryptoStandard.CMS);
                UtilMethod.LogMessageToFile("Signing the PDF complete");
                // Release resources.
                UtilMethod.LogMessageToFile("Releasing Resources");
                Reader.Close();
                Reader = null;
                tryStream.Close();

                return tryStream.ToArray();
            }
            catch (Exception ex)
            {
                UtilMethod.LogMessageToFile(ex);
                throw ex;
            }

        }



        ///  makes a pdf digitally signed and password protected.
        /// </summary>
        /// <param name="pdfBuffer"> pdf data in buffer.</param>
       /// <param name="certificatePath">path of the certificate</param>
       /// <param name="certificatePassword"> password of certificate</param>
       /// <param name="openPassword">password to open pdf.</param>
       /// <param name="ownerPassword"> password to edit permissions</param>
       /// <param name="signatureVisibility">should signature image be visible? true or false </param>
       /// <param name="imageLocation"> signature visibility when true pass the signature image location</param>
       /// <param name="permissions"> permissions for pdf.</param>
       /// <returns>digitally signed and password protected pdf</returns>
        public byte[] PasswordProtectPDFwithDigitallySigned(byte[] pdfBuffer, string certificatePath, string certificatePassword,
                                      string openPassword, string ownerPassword,bool signatureVisibility, 
                                      string imageLocation, int permissions)
        {
            try
            {
                UtilMethod.LogMessageToFile("In PasswordProtectPDFwithDigitallySigned ");
                Pkcs12Store store;
                //Open the certificate.
                FileStream fs = new FileStream(certificatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //search the certificate in certificate Store (certmgr.msc)
                UtilMethod.LogMessageToFile("Searching for certificate");
                try // If password is incorrect.
                {
                    store = new Pkcs12Store(fs, certificatePassword.ToCharArray());
                }
                catch (Exception ex)
                {

                    UtilMethod.LogMessageToFile(ex);
                    return null;
                }
                finally
                {
                    fs.Close();
                    fs = null;
                }
                string alias = null;
                ICollection<X509Certificate> chain = new List<X509Certificate>();
                // searching for private key
                UtilMethod.LogMessageToFile("Searching for private key");
                foreach (string al in store.Aliases)
                    if (store.IsKeyEntry(al) && store.GetKey(al).Key.IsPrivate)
                    {
                        alias = al;
                        break;
                    }
                AsymmetricKeyEntry pk = store.GetKey(alias);
                foreach (X509CertificateEntry c in store.GetCertificateChain(alias))
                    chain.Add(c.Certificate);
                RsaPrivateCrtKeyParameters parameters = pk.Key as RsaPrivateCrtKeyParameters;
                //Read the PDF 
                UtilMethod.LogMessageToFile("Reading PDF inside Signature without Image function");
                PdfReader Reader = new PdfReader(pdfBuffer);
                //Output MemoryStream which will contain the signed PDF
                MemoryStream tryStream = new MemoryStream();
                // essential for signing the document.
                UtilMethod.LogMessageToFile("Next is to try create stamper");
                PdfStamper stamper = PdfStamper.CreateSignature(Reader, tryStream, '\0');
                // setting encryption
                stamper.SetEncryption(true, openPassword, ownerPassword, permissions);
                iTextSharp.text.Rectangle size = Reader.GetPageSizeWithRotation(1);

                // Creating the appearance
                UtilMethod.LogMessageToFile("Signature Appearance");
                PdfSignatureAppearance appearance = stamper.SignatureAppearance;

                if (signatureVisibility && !string.IsNullOrEmpty(imageLocation) && File.Exists(imageLocation))
                {
                    float lly = size.Height / 5;
                    float llx = size.Width - 230f;
                    float ury = size.Height / 2;
                    float urx = size.Width - 10f;
                    appearance.Image = Image.GetInstance(imageLocation);
                    appearance.Image.SetAbsolutePosition(0, 0);
                    appearance.Image.Bottom = appearance.Image.Bottom - 20f;
                    appearance.Image.ScaleToFit(70, 70);
                    appearance.Image.SetAbsolutePosition(urx, ury - 100f);
                    appearance.Reason = "My reason for signing";
                    //appearance.Image.Transparency.SetValue(30, 0); 
                    appearance.Location = "The middle of nowhere";
                    appearance.SetVisibleSignature(new iTextSharp.text.Rectangle(llx, lly, urx, ury), Reader.NumberOfPages, "sig");
                }

                // Creating the signature
                UtilMethod.LogMessageToFile("Creating Signature ");
                IExternalSignature pks = new PrivateKeySignature(pk.Key, DigestAlgorithms.SHA256);

                //Signing the PDF.
                UtilMethod.LogMessageToFile("Signing the PDF start");
                MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, CryptoStandard.CMS);
                UtilMethod.LogMessageToFile("Signing the PDF complete");
                // Release resources.
                UtilMethod.LogMessageToFile("Releasing Resources");
                Reader.Close();
                Reader = null;
                tryStream.Close();

                return tryStream.ToArray();
            }
            catch (Exception ex)
            {
                UtilMethod.LogMessageToFile(ex);
                throw ex;
            }
        }


        public byte[] SignPdfWithImage(byte[] intermediatePDFBuffer, string certificatePath, string certificatePassword,
                                        string imgLocation, string ownerPassword, int permissions)
        {
            try
            {
                UtilMethod.LogMessageToFile("Entered SignPdfWithImage");
                FileStream fs = new FileStream(certificatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                Pkcs12Store store;
                try
                {
                     store = new Pkcs12Store(fs, certificatePassword.ToCharArray());
                }catch(Exception ex)
                {
                    UtilMethod.LogMessageToFile(ex);
                    return null;
                }
                finally
                {
                    fs.Close();
                    fs = null;
                }
             
                string alias = null;
                ICollection<X509Certificate> chain = new List<X509Certificate>();
                // searching for private key
                foreach (string al in store.Aliases)
                    if (store.IsKeyEntry(al) && store.GetKey(al).Key.IsPrivate)
                    {
                        alias = al;
                        break;
                    }
                AsymmetricKeyEntry pk = store.GetKey(alias);
                foreach (X509CertificateEntry c in store.GetCertificateChain(alias))
                    chain.Add(c.Certificate);
                RsaPrivateCrtKeyParameters parameters = pk.Key as RsaPrivateCrtKeyParameters;
                PdfReader Reader = new PdfReader(intermediatePDFBuffer);
                MemoryStream tryStream = new MemoryStream();

                PdfStamper stamper = PdfStamper.CreateSignature(Reader, tryStream, '\0');
                iTextSharp.text.Rectangle size = Reader.GetPageSizeWithRotation(1);
                float lly = size.Height / 5;
                float llx = size.Width - 230f;
                float ury = size.Height / 2;
                float urx = size.Width - 10f;

                // Creating the appearance
                PdfSignatureAppearance appearance = stamper.SignatureAppearance;
                stamper.SetEncryption(true, null, ownerPassword, permissions);

                // iTextSharp.text.Image watermark = iTextSharp.text.Image.GetInstance(imgLocation, true);
                // appearance.SignatureGraphic = Image.GetInstance(imgLocation);
                appearance.Image = Image.GetInstance(imgLocation);
                appearance.Image.SetAbsolutePosition(0, 0);
                appearance.Image.Bottom = appearance.Image.Bottom - 20f;
                appearance.Image.ScaleToFit(70, 70);
                appearance.Image.SetAbsolutePosition(urx, ury - 100f);
                appearance.Reason = "My reason for signing";
                //appearance.Image.Transparency.SetValue(30, 0); 
                appearance.Location = "The middle of nowhere";
                appearance.SetVisibleSignature(new iTextSharp.text.Rectangle(llx, lly, urx, ury), Reader.NumberOfPages, "sig");

                // Creating the signature
                IExternalSignature pks = new PrivateKeySignature(pk.Key, DigestAlgorithms.SHA256);
                MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, CryptoStandard.CMS);

                Reader.Close();
                Reader = null;
                tryStream.Close();

                return tryStream.ToArray();
            }
            catch (Exception ex)
            {
                UtilMethod.LogMessageToFile("Error:" + ex);
                UtilMethod.LogMessageToFile("Source:" + ex.Source);
                UtilMethod.LogMessageToFile(ex.StackTrace);
                throw;
            }

        }


        /// <summary>
        /// Encrypts the pdf with passed permissions.
        /// </summary>
        /// <param name="pdf">byte array containing pdf Data.</param>
        /// <param name="openPassword">password for protecting the PDF.</param>
        /// <param name="ownerPassword">edit password is the owner password by which user can change PDF restriction</param>
        /// <param name="permissions">permissions for the pdf.</param>
        /// <returns>PDF with restrictions and (optional) password protected.</returns>
        
        public byte[] GetEncryptedPDF(byte[] pdf, string openPassword, string ownerPassword,
                                      int permissions, bool passwordProtected)
        {
            try
            {
                UtilMethod.LogMessageToFile("Inside password protected with permissions");
                // applying restrictions using bitwise OR operator.


                // read the input pdf
                UtilMethod.LogMessageToFile("Reading pdf inside password protected with permissions");
                PdfReader reader = new PdfReader(pdf);
                // do not allow unethical reading
                PdfReader.unethicalreading = true;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    if (passwordProtected && !string.IsNullOrEmpty(openPassword))
                        //makes the pdf password protected with restrictions
                        PdfEncryptor.Encrypt(reader, memoryStream, true, openPassword, ownerPassword, permissions);
                    else
                        // apply restrictions to the pdf
                        PdfEncryptor.Encrypt(reader, memoryStream, true, null, ownerPassword, permissions);
                    reader.Close();
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                UtilMethod.LogMessageToFile(ex);
                throw ex;
            }
        }
   
        /// <summary>
        /// Sets permissions for pdf.
        /// </summary>
        /// <param name="allowDocumentAssembly"Assembly means merging, extracting, etc the PDF pages> Assembly means merging, extracting, etc the PDF pages</param>
        /// <param name="allowContentCopying">Copy means right-click copying the content of the PDF to the system</param>
        /// <param name="allowFillingFormFields">Can insert data into any AcroForms in the PDF</param>
        /// <param name="allowCommenting">Modification of any annotations</param>
        /// <param name="allowModifyContents">Modification of any content in the PDF</param>
        /// <param name="allowPrinting">Allows printing at any quality level</param>
        /// <param name="allowContentCopyingForAccessibility">Allows the content to be parsed/repurposed for screen readers like Read Out Loud functionality</param>
        /// <returns>integer with all permissions</returns>
        public int SetPermissions(bool allowDocumentAssembly, bool allowContentCopying, bool allowFillingFormFields,
             bool allowCommenting, bool allowModifyContents,
                                      bool allowPrinting, bool allowContentCopyingForAccessibility)
        {
            UtilMethod.LogMessageToFile("Inside Set Permissions");
            int permissions = 0;
            if (allowDocumentAssembly)
                permissions = permissions | PdfWriter.ALLOW_ASSEMBLY;
            if (allowContentCopying)
                permissions = permissions | PdfWriter.ALLOW_COPY;
            if (allowFillingFormFields)
                permissions = permissions | PdfWriter.ALLOW_FILL_IN;
            if (allowCommenting)
                permissions = permissions | PdfWriter.ALLOW_MODIFY_ANNOTATIONS;
            if (allowModifyContents)
                permissions = permissions | PdfWriter.ALLOW_MODIFY_CONTENTS;
            if (allowPrinting)
                permissions = permissions | PdfWriter.ALLOW_PRINTING;
            if (allowContentCopyingForAccessibility)
                permissions = permissions | PdfWriter.ALLOW_SCREENREADERS;

            return permissions;
        }


    }
}
