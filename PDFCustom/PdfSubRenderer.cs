/*
 *  PdfSUbRenderer.cs
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

using Microsoft.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

namespace PDFCustom
{
    class PdfSubRenderer
    {
        static Type pdfRendererType = null;

        // The built-in renderers we're goning to use later.
        public IRenderingExtension pdfRenderer; //PdfRenderer class isn't public anymore in SSRS 2008 R2

       

        public PdfSubRenderer(string assemblyName)
           
        {

            // Initialize the PDF renderer. it is an internal sealed class but we can still get to it using..Reflection! 
            // Don't worry, there are just 3 lines
            if (pdfRendererType == null)
            {
                // Use a disassembler tool like ILSpy to find The AssemblyName, type and constructor methods for other internal renderers in their respective .dlls
                Assembly IR = Assembly.Load(new AssemblyName(assemblyName));

                //Read the PdfRenderer type from the Assembly
                pdfRendererType = IR.GetType("Microsoft.ReportingServices.Rendering.ImageRenderer.PDFRenderer");
            }

            //Create an instance of type PdfRenderer. 
            //Now, PdfRenderer inherits from IRenderingExtension which is a public interface so cast it.
            pdfRenderer = (IRenderingExtension)pdfRendererType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke(null);
            //phew, no more Reflection from here on out.

           
        }
    }
}
