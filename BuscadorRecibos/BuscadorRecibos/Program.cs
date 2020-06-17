using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Resources;
using System.Reflection.Metadata.Ecma335;
using iTextSharp.text.pdf.parser;
using System.Linq;

namespace BuscadorRecibos
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu();


        }
        static void Menu()
        {
            //Menu de usuario 
            Console.WriteLine("Elija una opción('1'=Extraer Metadatos, '2'=Buscar_Recibo y Extraer en PDF, '3'=Salir): ");

            string opcionEntrada = Console.ReadLine();
            int opcion = Int32.Parse(opcionEntrada);
            
            switch(opcion)
            {
                case 1:
                    EscribirDatosTxt();
                    Console.WriteLine("Datos Extraidos y Guardados");
                    Menu();
                    break;
                case 2:
                    leerDatosTxt();                   
                    Menu();
                    break;

                case 3:
                    Console.WriteLine("Cerrando...");
                    break;
            
                default:
                    Console.WriteLine("Elija una opcion valida");
                    Menu();
                    break;

            }
            
             

           
            

        }
        static void EscribirDatosTxt()
        {
            Console.WriteLine("Introduzca una ruta de una carpeta con recibos: ");

            string ruta;
            ruta = Console.ReadLine();
            Rutas recibos = new Rutas();
            var recibos_rutas = recibos.RecuperarArchivos(ruta);

            using (System.IO.StreamWriter archivo = new System.IO.StreamWriter(@"C:\\METADATOS_RECIBOS\\Metadatos.txt"))
            foreach (string ruta_reci in recibos_rutas)
            {
                var datosPdf = DevolverdatosPagina(ruta_reci);         
           

            
            
                foreach (KeyValuePair<string, Array> dato in datosPdf)
                {
                    string pagina = dato.Key;
                    var control = dato.Value.GetValue(0);
                    var periodo = dato.Value.GetValue(1);

                    string linea = control.ToString() + "|" + periodo.ToString() + "|" + pagina + "|" + ruta_reci;


                    archivo.WriteLine(linea);
                    Console.WriteLine(linea);

                }

            }




        }

        static void leerDatosTxt()
        {
            //Lee los datos del archivo de metadatos y devuelve las coincidencias


            Dictionary<string, Array> periodosEncontrados = new Dictionary<string, Array>(); 
            string[] lineas  = System.IO.File.ReadAllLines(@"C:\\METADATOS_RECIBOS\\Metadatos.txt");

            Console.WriteLine("Escriba un numero de control: ");
            string inControl = Console.ReadLine();
            int inControlC = Int32.Parse(inControl);

            Console.WriteLine("Escriba un año: ");
            string inAnno = Console.ReadLine();
            int inAnnoA = Int32.Parse(inAnno);

            Console.WriteLine("Escriba un periodo: ");
            string inPeriodo = Console.ReadLine();
            int inPeriodoP = Int32.Parse(inPeriodo);


            string perAnno = inPeriodoP.ToString("00") + "/" + inAnnoA;

            Console.WriteLine("Indique una carpera de Guardado: ");
            string inRutaGuardado = Console.ReadLine();
            
            // bucle que recorre cada linea del archivo TXT de los metadatos

            for (int l=0; l < lineas.Length; l++)    
            {
                string control = lineas[l].Split("|")[0].Split(":")[1];
                int controlT = Int32.Parse(control);
                string periodoAnno = lineas[l].Split("|")[1].Split(":")[1];
                string periodo = lineas[l].Split("|")[1].Split(":")[1].Split("/")[0];
                string anno = lineas[l].Split("|")[1].Split(":")[1].Split("/")[1];
                string hoja = lineas[l].Split("|")[2];

                string ruta = lineas[l].Split("|")[3];
                

                if (inControlC == controlT & perAnno == periodoAnno)   //compara que los datos del usuario esten en los metadatos 
                {
                    string[] datos = new string[2];
                   

                    datos[0] = ruta;     //se almacenan para futura funcionalidad
                    datos[1] = hoja;

                    periodosEncontrados.Add(control + "|" + periodoAnno, datos);
                    int pagina = Int32.Parse(hoja);

                    ArchivoPdf recibo = new ArchivoPdf(ruta);
                    string rutaGuardado = inRutaGuardado + "\\" + control + "_" + anno + periodo + ".pdf";
                    recibo.ExtraerPaginaPdf(pagina, rutaGuardado);
                    Console.WriteLine("RECIBO GUARDADO EN:" + rutaGuardado);


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


        static void ExtraerRecibo()
        {

        }
    }

}




