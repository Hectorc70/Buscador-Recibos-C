using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace BuscadorRecibos
{
    class Program
    {
        static void Main(string[] args)
        {
            EscribirDatosTxt();
            Console.WriteLine("Datos Escritos");
        }

        static void EscribirDatosTxt()
        {
            string ruta = @"C:\Users\Hector\Documents\ARCHIVOS_PARA_PRUEBAS\recibos\01\ORDINARIA\PDF\RECIBOS\Base4_Cheques_201901.pdf";
            var datosPdf = DevolverdatosPagina(ruta);

            using (System.IO.StreamWriter archivo = new System.IO.StreamWriter(@"C:\\METADATOS_RECIBOS\\Metadatos.txt"))
            {
                foreach (KeyValuePair<string, Array> dato in datosPdf)
                {
                    string pagina = dato.Key;
                    var control = dato.Value.GetValue(0);
                    var periodo = dato.Value.GetValue(1);

                    string linea = control.ToString() + "|" + periodo.ToString() + "|" + pagina + "|" + ruta;


                    archivo.WriteLine(linea);
                    Console.WriteLine(linea);

                }

            }



        }


        static IDictionary<string, Array> DevolverdatosPagina(string ruta)
        {
            Dictionary<string, Array> datosPagina = new Dictionary<string, Array>();
            string[] patrones = { @"No. CONTROL: \d{8}?", @"PERIODO:\d{2}?/\d{4}?" };


            ArchivoPdf recibo = new ArchivoPdf(ruta);
            var pdf = recibo.LeerPdf();





            foreach (KeyValuePair<int, string> paginaRecibo in pdf) //recorre las hojas del archivo
            {
                string numPagina = paginaRecibo.Key.ToString();
                string textoReci = paginaRecibo.Value;
                //List<string> datos = new List<string>();
                string[] datos = new string[2];
                int contador = -1;
                for (int p = 0; p < patrones.Length; p++)           // recorre los patrones dados
                {
                    string patron = patrones[p];
                    Regex busqueda = new Regex(patron);
                    bool resultado = busqueda.IsMatch(textoReci);
                    contador += 1;
                    if (resultado)
                    {
                        MatchCollection coincidencias = Regex.Matches(textoReci, patron);


                        foreach (Match coincidencia in coincidencias)
                        {

                            datos[contador] = coincidencia.ToString();        //agrega el texto encontrado en la pagina
                            Console.WriteLine(coincidencia.ToString());
                        }


                    }
                }

                datosPagina.Add(numPagina, datos); //va agregando la pagina: control y periodo




            }

            return datosPagina;

        }


    }

}




