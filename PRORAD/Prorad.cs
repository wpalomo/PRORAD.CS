
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;

namespace PRORAD
{
    /// <summary>
    /// Módulo principal de procesos de Radio Frecuencia.
    /// </summary>
    public class RF : Base
    {
        //private static int _LINEA_INPUTS = Base._LINEA_INPUTS;
        //private static int _COLUMNA_INPUTS = Base._COLUMNA_INPUTS;

        //----------------------------------------------------
        // Constantes de uso PRIVADO de la aplicación.
        //----------------------------------------------------
        private const int _DATOS_OK = 1;

        private const int _INICIALIZAR = 11;
        private const int _FINALIZAR = 12;

        private const int _LECTURA_OPERARIO = 91;
        private const int _VALIDAR_OPERARIO = 93;
        private const int _LECTURA_TERMINAL = 92;
        private const int _VALIDAR_TERMINAL = 94;
        private const int _DESCONECTAR_OPERARIO	= 95;

        private const int _DESCONECTAR_BBDD = 96;

        private const string _PROGRAMA = "PRORAD.CPP";

        private const string _SECCION_0 = "00";					    // Menú principal.
        private const string _SECCION_E0 = "E0";					// Menú entradas.
        private const string _SECCION_S0 = "S0";					// Menú salidas.
        private const string _SECCION_S1 = "S1";					// Menú salidas - preparación.
        private const string _SECCION_S2 = "S2";					// Menú salidas - reposiciones.
        private const string _SECCION_S3 = "S3";					// Menú salidas - expedición.
        private const string _SECCION_I0 = "I0";					// Menú inventarios.
        private const string _SECCION_M0 = "M0";					// Menú movimientos.
        private const string _SECCION_C0 = "C0";					// Menú consultas.

        // Constantes para menús configurables.
        private const int _INICIALIZAR_MENU = 81;
        private const int _LECTURA_MENU = 82;
        private const int _VALIDAR_MENU = 83;
        private const int _PREVIOUS_MENU = 84;
        private const int _FINALIZAR_MENU = 85;
        private const int _NOOPERACION_MENU = 86;

        //-------------------------------------------------
        // Menú de opciones (GENERAL).
        //-------------------------------------------------
        int display_pantalla(COMPRADIO.PRORAD Iptr, int pantalla)
        {
            int tecla;

            pantalla = 1;
            clrscr();
            DisplayUserScr(Iptr, _PROGRAMA, _SECCION_0);

            do
            {
                gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
                tecla = getkey();
            } while (!(tecla == '1' || tecla == '2' || tecla == '3' || tecla == '4' || tecla == '5' || tecla == 13 || tecla == -12));

            switch (tecla)
            {
                case '1':
                case -1:
                    tecla = 1;
                    break;

                case '2':
                case -2:
                    tecla = 2;
                    break;

                case '3':
                case -3:
                    tecla = 3;
                    break;

                case '4':
                case -4:
                    tecla = 4;
                    break;

                case '5':
                case -5:
                    tecla = 5;
                    break;

                case -12:
                    tecla = 0;
                    break;

                default:
                    tecla = pantalla;
                    break;
            }

            return tecla;
        }

        //-------------------------------------------------
        // Menú de opciones (ENTRADAS).
        //-------------------------------------------------
        int display_pantalla_entradas(COMPRADIO.PRORAD Iptr, int pantalla)
        {
            int tecla;

            clrscr();
            DisplayUserScr(Iptr, _PROGRAMA, _SECCION_E0);

            do
            {
                gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
                tecla = getkey();
            } while (!(tecla == '1' || tecla == '2' || tecla == '3' || tecla == 13 || tecla == -12));

            switch (tecla)
            {
                case '1':
                case -1:
                    tecla = 1;
                    break;

                case '2':
                case -2:
                    tecla = 2;
                    break;

                case '3':
                case -3:
                    tecla = 3;
                    break;

                case -12:
                    tecla = 0;
                    break;

                default:
                    tecla = pantalla;
                    break;
            }

            return tecla;
        }

        //-------------------------------------------------
        // Menú. Versión standard.
        //-------------------------------------------------

        /// <summary>
        /// Menú general de opciones - Formato fijo.
        /// </summary>
        /// <param name="parametros">Array con los parámetros</param>
        /// <seealso cref="DisplayPantallaPro(object[])"/>
        /// <seealso cref="_tmain"/>
        
        private void menu(params object[] parametros)
        {
            int ret, ret1;
            ret = -1;
            object oPrg;

            COMPRADIO.PRORAD Iptr = (COMPRADIO.PRORAD)parametros[0];

            do
            {
                switch (ret)
                {
                    // Entradas
                    case 1:
                        ret1 = display_pantalla_entradas(Iptr, ret);

                        switch (ret1)
                        {
                            case 0:
                                ret = -1;
                                break;

                            case 1:
                                oPrg = new ENTRADAS.RECUENTO();
                                // oPrg = Convert.ChangeType(oPrg, typeof(ENTRADAS.RECUENTO));
                                ((ENTRADAS.RECUENTO)oPrg).RecuentoMaterial(Iptr);
                                oPrg = null;
                                break;

                            case 2:
                                oPrg = new SALIDAS.SALIDAP();
                                //RecuentoPalet(Iptr);
                                break;

                            case 3:
                                //UbicarPalet(Iptr);
                                break;

                            default:
                                ret1 = display_pantalla_entradas(Iptr, ret);
                                break;
                        }
                        break;

                    // NOP
                    default:
                        ret = display_pantalla(Iptr, 1);
                        break;
                }
            } while (ret != 0);

            return;
        }

        //-------------------------------------------------
        // Menú. Versión configurable.
        //-------------------------------------------------

        /// <summary>
        /// Menú general de opciones - Formato configurable.
        /// </summary>
        /// 
        /// <param name="parametros"> Array de parámetros</param>
        /// 
        /// <remarks>
        ///     parametros[0] - Objeto COMPRADIO.
        /// </remarks>
        /// 
        /// <seealso cref="menu(object[])"/>
        /// <seealso cref="_tmain"/>
        /// 
        /// <returns></returns>

        public int DisplayPantallaPro(params object[] parametros)
        {
            COMPRADIO.PRORAD Iptr = (COMPRADIO.PRORAD)parametros[0];

            int opc;
            int tecla;
            char cRetVal;

            CString cstSeccion = "";
            Char cstMenu = ' ';
            CString cstComando = "";
            CString cstEntry = "";

            opc = _INICIALIZAR_MENU;

            do
            {
                switch (opc)
                {
                    case _INICIALIZAR_MENU:
                        cstSeccion = _SECCION_0;
                        opc = _LECTURA_MENU;
                        break;

                    // Mostrar el menú activo.
                    case _LECTURA_MENU:
                        clrscr();
                        DisplayUserScr(Iptr, _PROGRAMA, cstSeccion, "MENUS");

                        cstMenu = ' ';
                        gotoxy(0, _LINEA_INPUTS);
                        do
                        {
                            gotoxy(_COLUMNA_INPUTS, _LINEA_INPUTS);
                            tecla = getkey();
                        } while (tecla == -1);

                        switch (tecla)
                        {
                            // Ir al nivel anterior, ó exit.
                            case _TECLA_ESC:
                                opc = _PREVIOUS_MENU;
                                break;

                            // Validar la opción de menú.
                            default:
                                cstMenu = System.Convert.ToChar(tecla);
                                opc = _VALIDAR_MENU;
                                break;
                        }
                        break;

                    // Validar el menú
                    case _VALIDAR_MENU:
                        cRetVal = System.Convert.ToChar(Iptr.ValidarMenu(_PROGRAMA, cstSeccion, cstMenu));

                        switch (cRetVal)
                        {
                            // OK: Ejecutar la función seleccionada.
                            case 'F':
                                cstComando = Iptr.MnuNamespace + "." + Iptr.MnuClase + ", " + Iptr.MnuEnsamblado;
                                cstEntry = Iptr.MnuEntryPoint;

                                System.Object[] pmo = { Iptr, new System.Object[] { } };
                                Type t = System.Type.GetType(cstComando);
                                if(t==null)
                                {
                                    UserError("_SYS-0010", Iptr.GetMessage("_SYS-0010", "Llamada es NULL", Iptr.MensajesRF));
                                    opc = _LECTURA_MENU;
                                    break;
                                }

                                object ot = System.Activator.CreateInstance(t);
                                ot.GetType().GetMethod(cstEntry).Invoke(ot, pmo);

                                opc = _LECTURA_MENU;
                                break;

                            // OK: Displayar el menú seleccionado.
                            case 'M':
                                cstSeccion = Iptr.MnuSeccion;

                                opc = _LECTURA_MENU;
                                break;

                            // Salir.
                            case 'Q':
                                opc = _FINALIZAR_MENU;
                                break;

                            // Opción incorrecta.
                            case 'N':
                                UserError(Iptr);
                                opc = _LECTURA_MENU;
                                break;

                            // Error.
                            case 'C':
                                UserError(Iptr);
                                opc = _FINALIZAR_MENU;
                                break;

                            // Resto de casos: ignorar.
                            default:
                                opc = _LECTURA_MENU;
                                break;
                        }
                        break;

                    // Ir al nivel anterior de menú.
                    case _PREVIOUS_MENU:
                        cstSeccion = System.Convert.ToString(Iptr.MnuSeccion);
                        cRetVal = System.Convert.ToChar(Iptr.PreviousMenu(_PROGRAMA, cstSeccion, 'Q'));

                        switch (cRetVal)
                        {
                            // OK: Display menú.
                            case 'S':
                                cstSeccion = (string)Iptr.MnuSeccion;
                                opc = _LECTURA_MENU;
                                break;

                            // Salir.
                            case 'Q':
                                opc = _FINALIZAR_MENU;
                                break;

                            // Opción incorrecta
                            case 'N':
                                UserError(Iptr);
                                opc = _LECTURA_MENU;
                                break;

                            // Error
                            case 'C':
                                UserError(Iptr);
                                opc = _FINALIZAR_MENU;
                                break;

                            // Resto de casos: ignorar
                            default:
                                opc = _LECTURA_MENU;
                                break;
                        }
                        break;

                    // Finalizar tratamiento de menú
                    case _FINALIZAR_MENU:
                        opc = _NOOPERACION_MENU;
                        break;
                }
            } while (opc != _NOOPERACION_MENU);

            return 0;
        }

        //-------------------------------------------------
        // Función principal.
        //-------------------------------------------------

        /// <summary>
        /// Punto de entrada al programa de RF.
        /// </summary>
        /// <seealso cref="menu(object[])"/>
        /// <seealso cref="DisplayPantallaPro(object[])"/>
        /// <returns></returns>

        public int _tmain()
        {
            int opc, ret, siguiente_operacion = 0;
            int nRetCode = 0;
            char RetVal = ' ';

            string lectura_operario = null;
            string lectura_terminal = null;

            // compradio.proradClass comInstance = new compradio.proradClass();
            COMPRADIO.PRORAD comInstanceCS = new COMPRADIO.PRORAD();

            try
            {
                opc = _INICIALIZAR;

                do
                {
                    switch (opc)
                    {
                        case _INICIALIZAR:
                            // Conectar la BBDD
                            clrscr();
                            gotoxy(0, 14);
                            Console.WriteLine("Conectando ...");

                            // RetVal = System.Convert.ToChar(comInstance.inicioconexion());
                            RetVal = comInstanceCS.InicioConexion();

                            switch (RetVal)
                            {
                                // OK
                                case 'S':
                                    opc = _LECTURA_OPERARIO;
                                    break;

                                // Error inicializar.
                                case 'N':
                                    UserError(comInstanceCS.UsrErrorC, comInstanceCS.UsrError);
                                    nRetCode = 2;
                                    opc = _FINALIZAR;
                                    break;

                                // Error general.
                                case 'C':
                                    UserError(comInstanceCS.UsrErrorC, comInstanceCS.UsrError);
                                    nRetCode = 2;
                                    opc = _FINALIZAR;
                                    break;

                                // Resto de casos: OK
                                default:
                                    opc = _LECTURA_OPERARIO;
                                    break;
                            }
                            break;

                        // Entrar código de operario.
                        case _LECTURA_OPERARIO:
                            clrscr();
                            gotoxy(0, 1); printf(comInstanceCS.GetMessage("INI-0005", "PRORAD.2").TrimEnd());
                            gotoxy(0, 4); printf(comInstanceCS.GetMessage("INI-0006", "Operario: ").TrimEnd());

                            lectura_operario = "";
                            gotoxy(10, 4);
                            ret = leer_char(ref lectura_operario, 4);

                            switch (ret)
                            {
                                // Salir del programa
                                case _TECLA_ESC:
                                    opc = _FINALIZAR;
                                    break;

                                // Pedir terminal
                                case _TECLA_0:
                                    opc = _VALIDAR_OPERARIO;
                                    break;

                                // Resto de casos: ignorar
                                default:
                                    opc = _LECTURA_OPERARIO;
                                    break;
                            }
                            break;

                        // Entrar tipo terminal
                        case _LECTURA_TERMINAL:
                            clrscr();
                            gotoxy(0, 1); printf(comInstanceCS.GetMessage("INI-0005", "PRORAD.2").TrimEnd());
                            gotoxy(0, 4); printf(comInstanceCS.GetMessage("INI-9006", "Operario: {0}").TrimEnd(), lectura_operario);
                            gotoxy(0, 5); printf(comInstanceCS.GetMessage("INI-0007", "Terminal: ").TrimEnd());

                            lectura_terminal = "";
                            gotoxy(10, 5);
                            ret = leer_char(ref lectura_terminal, 2);

                            switch (ret)
                            {
                                // Ir a pedir operario.
                                case _TECLA_ESC:
                                    siguiente_operacion = _LECTURA_OPERARIO;
                                    opc = _DESCONECTAR_OPERARIO;
                                    break;

                                // Realizar las validaciones
                                case _TECLA_0:
                                    opc = _VALIDAR_TERMINAL;
                                    break;

                                // Resto de casos: ignorar
                                default:
                                    opc = _LECTURA_TERMINAL;
                                    break;
                            }
                            break;

                        // Validar operario
                        case _VALIDAR_OPERARIO:
                            // RetVal = System.Convert.ToChar(comInstance.conectarusuario(lectura_operario));

                            RetVal = comInstanceCS.ConectarUsuario(lectura_operario);

                            switch (RetVal)
                            {
                                // OK: Mostar menú de opciones.
                                case 'S':
                                    opc = _LECTURA_TERMINAL;
                                    break;

                                // Valores incorrectos.
                                case 'N':
                                    UserError(comInstanceCS.UsrErrorC, comInstanceCS.UsrError);
                                    opc = _LECTURA_OPERARIO;
                                    break;

                                // Error
                                case 'C':
                                    UserError(comInstanceCS.UsrErrorC, comInstanceCS.UsrError);
                                    nRetCode = 3;
                                    opc = _FINALIZAR;
                                    break;

                                // Resto de casos: ignorar
                                default:
                                    opc = _LECTURA_OPERARIO;
                                    break;
                            }
                            break;

                        // Validar terminal.
                        case _VALIDAR_TERMINAL:
                            // RetVal = System.Convert.ToChar(comInstance.scrvalidarterminal(lectura_terminal));
                            RetVal = comInstanceCS.ValidarTerminal(lectura_terminal);

                            switch (RetVal)
                            {
                                // OK: Mostar menú de opciones
                                case 'S':
                                    opc = _DATOS_OK;
                                    break;

                                // Valores incorrectos
                                case 'N':
                                    UserError(comInstanceCS.UsrErrorC, comInstanceCS.UsrError);
                                    opc = _LECTURA_TERMINAL;
                                    break;

                                // Error
                                case 'C':
                                    UserError(comInstanceCS.UsrErrorC, comInstanceCS.UsrError);
                                    nRetCode = 4;
                                    opc = _FINALIZAR;
                                    break;

                                // Resto de casos: ignorar
                                default:
                                    opc = _LECTURA_TERMINAL;
                                    break;
                            }
                            break;

                        // Desconectar operario activo
                        case _DESCONECTAR_OPERARIO:
                            // RetVal = System.Convert.ToChar(comInstance.desconectarusuario());
                            RetVal = comInstanceCS.DesconectarUsuario();

                            switch (RetVal)
                            {
                                // Siguiente operación a realizar:
                                //	- Pedir operario
                                //	- Cerrar BBDD
                                case 'S':
                                    opc = siguiente_operacion;
                                    break;

                                // Error
                                case 'C':
                                    UserError(comInstanceCS.UsrErrorC, comInstanceCS.UsrError);
                                    nRetCode = 5;
                                    opc = _FINALIZAR;
                                    break;

                                // Resto valores: Error
                                default:
                                    UserError(comInstanceCS.UsrErrorC, comInstanceCS.UsrError);
                                    nRetCode = 6;
                                    opc = _FINALIZAR;
                                    break;
                            }
                            break;

                        // Display del menú general
                        case _DATOS_OK:
                            // Cargar la línea inicial de inputs.
                            _LINEA_INPUTS = comInstanceCS.LineaInputs;

                            // Cargar la columna de inputs.
                            // _COLUMNA_INPUTS = System.Convert.ToInt32(comInstanceCS.ColumnaInputs);
                            _COLUMNA_INPUTS = comInstanceCS.ColumnaInputs;

                            // Llamada al menú principal.
                            menu(comInstanceCS);
                            //DisplayPantallaPro(comInstanceCS);

                            nRetCode = 0;
                            opc = _LECTURA_TERMINAL;
                            break;

                        // Finalizar programa
                        case _FINALIZAR:
                            opc = _DESCONECTAR_BBDD;
                            break;

                        // Cerrar BBDD
                        case _DESCONECTAR_BBDD:
                            // RetVal = System.Convert.ToChar(comInstance.finconexion());
                            RetVal = comInstanceCS.FinConexion();

                            switch (RetVal)
                            {
                                // Valores incorrectos
                                case 'N':
                                    UserError(comInstanceCS.UsrErrorC, comInstanceCS.UsrError);
                                    break;

                                // Error
                                case 'C':
                                    UserError(comInstanceCS.UsrErrorC, comInstanceCS.UsrError);
                                    break;
                            }

                            opc = _FINALIZAR;
                            break;

                        default:
                            opc = _FINALIZAR;
                            break;
                    }
                } while (opc != _FINALIZAR);
            }

            catch (Exception ex)
            {
                string cadena = DateTime.Now.ToString() + "|" + ex.Message + "|" + ex.StackTrace + System.Environment.NewLine;

                System.IO.FileStream fs = new System.IO.FileStream("PRORAD.LOG", System.IO.FileMode.Append);
                fs.Write(ASCIIEncoding.ASCII.GetBytes(cadena), 0, cadena.Length);
                fs.Close();

                nRetCode = ex.HResult;
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }

            finally
            {
                RetVal = comInstanceCS.DesconectarUsuario();
                RetVal = comInstanceCS.FinConexion();
            }

            return nRetCode;
        }
    }

    //-------------------------------------------------
    // Entrada al programa.
    //-------------------------------------------------

    /// <summary>
    /// Clase base de gestión de RF.
    /// </summary>
    class Prorad
    {
        /// <summary>
        /// Entrada al programa.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                //Console.WriteLine("Hello World !!");
                //Console.ReadKey();
                RF oRF = new RF();
                int opc = oRF._tmain();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }

            finally
            {
            }

            return;

        }
    }
}
