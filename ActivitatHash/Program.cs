using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ActivitatHash
{
    class Program
    {

        // Variable per emmagatzemar el nom del fitxer amb el que es vol treballar amb el hash
        private string nomFitxerSeleccionat = null;
        // Array amb les extensions acceptades de fitxer, per poder fer la conversió de nom al crear l'arxiu .sha
        private string[] extensionsAcceptades =
        {
            ".txt",
            ".doc",
            ".docx",
        };

        static void Main(string[] args)
        {

            Program launcher = new Program();
            launcher.launcher();

        }


        /* .: 1. COS PRINCIPAL :. */
        private void launcher()
        {

            char opcioMenu = ' ';

            do
            {

                Console.Clear();
                menuPrincipal();
                
                try
                {

                    string usuariInput = null;

                    while (string.IsNullOrEmpty(usuariInput))
                    {
                        usuariInput = Console.ReadLine();
                    }

                    opcioMenu = usuariInput.ToCharArray()[0];

                }
                catch (IOException)
                {

                    opcioMenu = '0';

                }

                switch (opcioMenu)
                {

                    case '1':
                        hashFrase();
                        break;

                    case '2':
                        hashFitxer();
                        break;

                    case '3':
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Introdueix una opció vàlida!");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;

                }


            }
            while (opcioMenu != '3');

        }

        private void menuPrincipal()
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine("------- MENU PRINCIPAL -------");
            Console.WriteLine("------------------------------");
            Console.WriteLine("- 1. Hash de frase           -");
            Console.WriteLine("- 2. Treballar amb un fitxer -");
            Console.WriteLine("- 3. Sortir                  -");
            Console.WriteLine("------------------------------");
        }


        /* .: 2. FER HASH D'UNA FRASE :. */
        private void hashFrase()
        {

            String textIn = null;
            Console.Write("Entra text: ");
            while (string.IsNullOrEmpty(textIn))
            {
                Console.Clear();
                Console.Write("Entra text: ");
                textIn = Console.ReadLine();
            }
            // Convertim l'string a un array de bytes
            byte[] bytesIn = Encoding.UTF8.GetBytes(textIn);
            // Instanciar classe per fer hash

            // fent servir using ja es delimita el seu àmbit i no cal fer dispose
            using (SHA512Managed SHA512 = new SHA512Managed())
            {
                // Calcular hash
                byte[] hashResult = SHA512.ComputeHash(bytesIn);

                // Si volem mostrar el hash per pantalla o guardar-lo en un arxiu de text
                // cal convertir-lo a un string

                String textOut = BitConverter.ToString(hashResult).Replace("-", string.Empty);
                Console.WriteLine("Hash del text {0}", textIn);
                Console.WriteLine(textOut);
                Console.ReadKey();

            }

        }


        /* .: 3. TREBALLAR AMB UN FITXER :.*/
        private void hashFitxer()
        {

            char opcioMenu = ' ';

            do
            {

                Console.Clear();
                menuHashFitxer();

                try
                {

                    string usuariInput = null;

                    while (string.IsNullOrEmpty(usuariInput))
                    {
                        usuariInput = Console.ReadLine();
                    }

                    opcioMenu = usuariInput.ToCharArray()[0];

                }
                catch (IOException)
                {

                    opcioMenu = '0';

                }

                switch (opcioMenu)
                {

                    case '1':
                        seleccioFitxer();
                        break;

                    case '2':
                        crearFitxerNou();
                        break;

                    case '3':
                        if (comprovarExistenciaFitxer())
                            ferHashFitxer();
                        else
                            errorFitxerNoExisteix();
                        break;

                    case '4':
                        if (comprovarExistenciaFitxer() && comprovarHashFitxerCreat())
                            comprovarIntegritatFitxer();
                        else
                            errorFitxerNoExisteix();
                        break;

                    case '5':
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Introdueix una opció vàlida!");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;

                }
            }
            while (opcioMenu != '5');

        }

        private void menuHashFitxer()
        {

            Console.WriteLine("--------------------------------------");
            Console.WriteLine("------ TREBALLAR HASH AMB FITXER -----");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("- 1. Seleccionar fitxer              -");
            Console.WriteLine("- 2. Crear un fitxer nou             -");
            Console.WriteLine("- 3. Fer hash del fitxer             -");
            Console.WriteLine("- 4. Comprovar integritat del fitxer -");
            Console.WriteLine("- 5. Sorir                           -");
            Console.WriteLine("--------------------------------------");

        }


        /* .: 3.1 SELECCIÓ DEL FITXER :. */
        private void seleccioFitxer()
        {

            String textInput = null;
            Console.Write("Introdueix la ruta absoluta i el nom del fitxer: ");

            while (string.IsNullOrEmpty(textInput))
            {
                textInput = Console.ReadLine();
            }

            nomFitxerSeleccionat = textInput;

        }


        /* .: 3.2 CREAR FITXER NOU :. */
        private void crearFitxerNou()
        {

            string nomFitxer = null;
            string contingutFitxer = null;
            Console.Clear();

            try
            {

                Console.WriteLine("Introdueix el nom del fitxer:");
                nomFitxer = Console.ReadLine();

                Console.WriteLine("Introdueix el contingut del fitxer:");
                contingutFitxer = Console.ReadLine();

                File.WriteAllText(nomFitxer, contingutFitxer);
                nomFitxerSeleccionat = nomFitxer;

            }
            catch (IOException e)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("S'ha produit un error! No s'ha pogut creat el fitxer.");
                Console.WriteLine(e);
                Console.ResetColor();

            }

        }


        /* .: 3.3 FER HASH DEL FITXER SELECCIONAT :. */
        private void ferHashFitxer()
        {

            string textFitxer = null;

            try
            {

                // Lectura del fitxer i transformació a array de bytes
                textFitxer = File.ReadAllText(nomFitxerSeleccionat, System.Text.Encoding.UTF8);
                byte[] bytesTextFitxer = Encoding.UTF8.GetBytes(textFitxer);

                // Fer hash del resultat i convertir a string
                SHA512Managed SHA512 = new SHA512Managed();
                byte[] hashResultat = SHA512.ComputeHash(bytesTextFitxer);
                String textOut = BitConverter.ToString(hashResultat).Replace("-", string.Empty);

                // Creació del fitxer amb el resum
                File.WriteAllText(string.Concat(convertirNom(nomFitxerSeleccionat), ".sha"), textOut);

                // Missatge de creació satisfactori
                Console.WriteLine("S'ha generat el hash del fitxer {0} correctament", nomFitxerSeleccionat);
                Console.ReadKey();

            }
            catch (IOException e)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("S'ha produit un error! No s'ha pogut generar el hash del fitxer {0}.", nomFitxerSeleccionat);
                Console.WriteLine(e);
                Console.ResetColor();

            }

        }


        /* .: 3.4 COMPROVAR INTEGRITAT DEL FITXER SELECCIONAT :. */
        private void comprovarIntegritatFitxer()
        {

            string textFitxer = null;
            string hashFitxer = null;
            Boolean integritat = false;

            try
            {

                // Lectura del contingut dels dos fitxers, hash i originals
                textFitxer = File.ReadAllText(nomFitxerSeleccionat, System.Text.Encoding.UTF8);
                hashFitxer = File.ReadAllText(string.Concat(convertirNom(nomFitxerSeleccionat), ".sha"), System.Text.Encoding.UTF8);

                // Conversió de l'original a array de bytes
                byte[] bytesTextFitxer = Encoding.UTF8.GetBytes(textFitxer);

                // Fer hash del fitxer original i passar a string
                SHA512Managed SHA512 = new SHA512Managed();
                byte[] hashResultat = SHA512.ComputeHash(bytesTextFitxer);
                String hashFitxerLlegit = BitConverter.ToString(hashResultat).Replace("-", string.Empty);

                // Revisa si el hash creat és igual que el hash del fitxer ja creat
                if (hashFitxer.Equals(hashFitxerLlegit))
                    integritat = true;

                // Depenent la resposta, es mostra un missatge o un altre
                if (integritat)
                {
                    Console.WriteLine("Hi ha integritat en el fitxer {0}", nomFitxerSeleccionat);
                }
                else
                {
                    Console.WriteLine("No hi ha integritat en el fitxer {0}", nomFitxerSeleccionat);
                }

                Console.ReadKey();

            }
            catch (IOException e)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("S'ha produit un error! No s'ha pogut comparar la integritat del fitxer {0}.", nomFitxerSeleccionat);
                Console.WriteLine(e);
                Console.ResetColor();

            }

        }


        /* .: 3.5 ERROR FITXER NO EXISTEIX :. */
        private void errorFitxerNoExisteix()
        {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("El fitxer seleccionat no existeix!");
            Console.ResetColor();
            Console.ReadKey();

        } 
        
        
        /* .: 4. FUNCIONALITATS :. */
        // Revisa si el fitxer seleccionat existeix i si s'ha inicialitzat la variable amb el nom del fitxer, així evitem possibles crashes. Retorna un boolean
        private Boolean comprovarExistenciaFitxer()
        {
            Boolean existeix = false;

            if (nomFitxerSeleccionat != null)
            {
                if (File.Exists(nomFitxerSeleccionat))
                    existeix = true;
            }

            return existeix;
        }

        // Revisa si s'ha creat el fitxer hash del fitxer seleccionat
        private Boolean comprovarHashFitxerCreat()
        {
            Boolean existeix = false;

            if (nomFitxerSeleccionat != null)
            {
                string nomFitxerHash = string.Concat(convertirNom(nomFitxerSeleccionat), ".sha");

                if (File.Exists(nomFitxerHash))
                    existeix = true;

            }

            return existeix;
        }

        // Treu, del nom que li arriba per paràmetre, l'extensió, si es troba en l'array de les extensions acceptades
        private string convertirNom(string nomFitxer)
        {

            List<char> lletresExtensions = new List<char>();

            for (int extensio = 0; extensio < extensionsAcceptades.Length; extensio++)
            {

                for (int lletra = 0; lletra < extensionsAcceptades[extensio].Length; lletra++)
                {
                    lletresExtensions.Add(extensionsAcceptades[extensio].ToCharArray()[lletra]);
                }

            }

            return nomFitxer.TrimEnd(lletresExtensions.ToArray());
        }


    }
}
