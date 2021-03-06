﻿
using System.Text;
using System.IO;
using System.Collections.Generic;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;


namespace BuscadorRecibos
{
    class ArchivoPdf
    /*Clase que lee y crea archivos pdf*/
    {
        private string rutaArchivo;
        public ArchivoPdf(string ruta)
        {
            this.rutaArchivo = ruta;
        }



        public IDictionary<int, string> LeerPdf()
        //Metodo que lee y devuelve el contenido de un archivo Pdf
        {

            Dictionary<int, string> datosPdf = new Dictionary<int, string>();


            PdfReader archivo = new PdfReader(this.rutaArchivo);
            string strText = string.Empty;

            for (int pagina = 1; pagina <= archivo.NumberOfPages; pagina++)
            {
                ITextExtractionStrategy conte = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                PdfReader lectura = new PdfReader(this.rutaArchivo);
                string s = PdfTextExtractor.GetTextFromPage(lectura, pagina, conte);

                s = System.Text.Encoding.UTF8.GetString(ASCIIEncoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF8,
                                            System.Text.Encoding.Default.GetBytes(s)));

                strText = strText + s;
                datosPdf.Add(pagina, strText);
                lectura.Close();
            }

            return (datosPdf);

        }


        public void ExtraerPaginaPdf(int pagina, string rutaGuardado)
        //Metodo que extrae una pagina de un archivo pdf
        {

            PdfReader archivo = new PdfReader(this.rutaArchivo);
            using (FileStream fs = new FileStream(rutaGuardado, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (Document doc = new Document())
                {
                    using (PdfWriter archivoWrite = PdfWriter.GetInstance(doc, fs))
                    {
                        doc.Open();
                        doc.NewPage();
                        archivoWrite.DirectContent.AddTemplate(archivoWrite.GetImportedPage(archivo, pagina), 0, 0);
                        doc.Close();
                    }
                }
            }
        }
    }
}
