
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using System.Security.Cryptography;
using System.Configuration;

namespace COMPRADIO
{
    /// <summary>
    /// Funciones de trabajo de los procesos de RF.
    /// </summary>
    public partial class PRORAD
    {
        //-------------------------------------------------------
        // Funciones generales de RF.
        //-------------------------------------------------------

        /// <summary>
        /// Establecer conexión con la BBDD e inicializar la aplicación.
        /// </summary>
        /// <returns>Resultado (S/N/C)</returns>

        public char InicioConexion()
        {
            //// Valores solo de TEST.
            //string setting1 = Properties.Settings.Default.RadioIniPath;
            string resource1 = Properties.Resources.ResourceBase;

            //string envVar1 = System.Environment.GetEnvironmentVariable("PATH");
            //int nRFIni = ConfigurationManager.AppSettings.Count;
            //nRFIni = ConfigurationManager.ConnectionStrings.Count;

            //string cRFIni = ConfigurationManager.ConnectionStrings[0].ConnectionString;
            //// END valores sólo TEST.

            Char UsrConect = 'S';
            string pathFicheroRF = Properties.Settings.Default.RfXmlPath;
            string pathFicheroSGA = Properties.Settings.Default.SgaXmlFile;

            if (System.IO.File.Exists(pathFicheroRF)==false) pathFicheroRF = System.IO.Directory.GetCurrentDirectory() + "/RADIO.XML";
            if (System.IO.File.Exists(pathFicheroSGA) == false) pathFicheroSGA = System.IO.Directory.GetCurrentDirectory() + "/SGA.XML";

            try
            {
                // Sección Compradio.VFP
                UsrConect = System.Convert.ToChar(Iptr.inicioconexion());
                if (UsrConect != 'S') return UsrConect;

                // Sección Compradio.CS
                // __TFRf = new TF.Tf(Iptr.cadenadeconexion.ToString());            // Toma valor de compradio.Vfp
                __TFRf = new TF.Tf(Properties.Settings.Default.ConnectString);      // Toma valor de configuración.

                __TFXmlRF = new TF.Xml();                                           // Parámetros configuración RF.
                UsrConect = __TFXmlRF.LoadXmlFile(pathFicheroRF);
                if (UsrConect == 'S')
                {
                    __TFXmlSGA = new TF.Xml();
                    UsrConect = __TFXmlSGA.LoadXmlFile(pathFicheroSGA);             // Parámetros configuración SGA.
                }

                switch (UsrConect)
                {
                    // OK.
                    case 'S':
                        break;

                    case 'N':
                        SetMessage("INIC-0001", "No existe XML");
                        break;

                    case 'C':
                        SetMessage("INIC-0002", "Error load XML");
                        break;

                    default:
                        SetMessage("INIC-????", "Error no controlado");
                        UsrConect = 'N';
                        break;
                }

                // Inicializar constantes de configuración de RADIO.XML
                if (_xmlinicializar() == false)
                {
                    // Error de programa.
                    SetMessage("INIC-0004", "Config no válidos");
                    UsrConect = 'N';
                }

                // Inicializa el entorno BBDD.
                __TFRf.Inicializar();

                // Inicializar propiedades de visualización de pantalla.
                _scrinicializar();
                // .....
            }

            catch (ArgumentOutOfRangeException ex)
            {
                // Error de programa.
                SetMessage("INIC-????", ex.Message);
                UsrErrorE = ex.Message;
                UsrConect = 'C';
            }

            catch (Exception ex)
            {
                // Error de programa.
                SetMessage("INIC-0003", "Excepción load XML");
                UsrErrorE = ex.Message;
                UsrConect = 'C';
            }

            return UsrConect;
        }

        /// <summary>
        /// Establecer el operario activo.
        /// </summary>
        /// <param name="CodigoDeUsuario"> Código del usuario que se conecta a la sesión</param>
        /// <returns>Resultado (S/N/C)</returns>

        public char ConectarUsuario(string CodigoDeUsuario)
        {
            Char UsrConect;
            string whereOpe = "F05cCodOpe='" + CodigoDeUsuario + "'";

            // Sección Compradio.VFP
            // UsrConect = System.Convert.ToChar(Iptr.conectarusuario(CodigoDeUsuario));

            // Sección Compradio.CS----------------------------------------------
            UsrConect = __TFRf.SeekRow("F05C001", SeekWhere: whereOpe);

            switch (UsrConect)
            {
                // Operario OK. Validar si está conectado.
                case 'S':
                    if (__TFRf.SqlDataRow["F05cCodTer"].ToString().ToUpper() == "CONS")
                    {
                        SetMessage("CONU-0002", "Operario ya conectado");
                        UsrConect = 'N';
                    }
                    else
                    {
                        // Guardar propiedades.
                        CodOpe = __TFRf.SqlDataRow["F05cCodOpe"].ToString();
                        NomOpe = __TFRf.SqlDataRow["F05cNombre"].ToString();
                        GrpOpe = __TFRf.SqlDataRow["F05cGrupo"].ToString();

                        // Actualizar la ficha del operario.
                        UsrConect = __TFRf.UpdateCurrentRow("F05cCodTer", "CONS");
                        string ooo = __TFRf.DataSetToXml();
                    }
                    break;

                // Operario no existe.
                case 'N':
                    SetMessage("CONU-0001", "Operario no existe");
                    break;

                // Error al validar operario.
                case 'C':
                    SetMessage("CONU-0003", "Error BBDD");
                    break;

                // Resto de casos: Error no controlado.
                default:
                    SetMessage("CONU-????", "Error no controlado");
                    UsrConect = 'N';
                    break;
            }
            // Fin Sección Compradio.CS -----------------------------------------

            return UsrConect;
        }

        /// <summary>
        /// Validar propiedades del terminal.
        /// </summary>
        /// <param name="TerminalRF"></param>
        /// <returns>Resultado (S/N/C)</returns>

        public char ValidarTerminal(string TerminalRF)
        {
            Char UsrConect = 'S';
            string WhereRF = "MONITOR='" + TerminalRF + "'";

            // Sección Compradio.VFP
            //  UsrConect = System.Convert.ToChar(Iptr.scrvalidarterminal(TerminalRF));
            // if (UsrConect != 'S') return UsrConect;

            // Sección Compradio.CS----------------------------------------------
            UsrConect = __TFRf.SeekRow("MONITORES", "*", WhereRF);

            switch (UsrConect)
            {
                // Terminal OK.
                case 'S':
                    Monitor = System.Convert.ToString(__TFRf.SqlDataRow["MONITOR"]);
                    TipoMonitor = System.Convert.ToChar(__TFRf.SqlDataRow["TIPO"]);
                    AnchoPantalla = System.Convert.ToInt32(__TFRf.SqlDataRow["ANCHO"]);
                    AltoPantalla = System.Convert.ToInt32(__TFRf.SqlDataRow["ALTO"]);
                    Idioma = System.Convert.ToString(__TFRf.SqlDataRow["IDIOMA"]);
                    LineaInputs = System.Convert.ToInt32(__TFRf.SqlDataRow["LINEA11"]);
                    ColumnaInputs = System.Convert.ToInt32(__TFRf.SqlDataRow["COLUMNA11"]);

                    break;

                // Terminar RF no válido.
                case 'N':
                    SetMessage("SCR-0001", "Terminal no válido");
                    break;

                // Error al validar terminal.
                case 'C':
                    SetMessage("SCR-0003", "Error BBDD");
                    break;

                // Resto de casos: Error no controlado.
                default:
                    SetMessage("SCR-????", "Error no controlado");
                    UsrConect = 'N';
                    break;
            }
            // Fin Sección Compradio.CS -----------------------------------------

            return UsrConect;
        }

        /// <summary>
        /// Validar opción de menú.
        /// </summary>
        /// <param name="programa">Código de programa en MENUS</param>
        /// <param name="seccionMenu">Sección a visualizar en pantalla</param>
        /// <param name="opcionMenu">Opción de menú</param>
        /// <param name="ficheroMenus">Archivo de menús. Def: MENUS</param>
        /// <returns>Resultado (S/N/C)</returns>

        public char ValidarMenu(string programa, string seccionMenu, Char opcionMenu, string ficheroMenus = "MENUS")
        {
            Char UsrConect = 'S';
            string strWhere = "";

            strWhere += "GRUPO='" + GrpOpe + "' And ";
            strWhere += "MONITOR='" + Monitor + "' And ";
            strWhere += "IDIOMA='" + Idioma + "' And ";
            strWhere += "PROGRAMA='" + programa + "' And ";
            strWhere += "SECCION='" + seccionMenu + "'";

            strWhere += opcionMenu == ' ' ? "" : " And OPCION='" + opcionMenu + "'";

            UsrConect = __TFRf.SeekRow(ficheroMenus, SeekWhere: strWhere);

            switch (UsrConect)
            {
                // Opción de menú OK - Validar su tipo.
                case 'S':
                    UsrConect = System.Convert.ToChar(__TFRf.SqlDataRow["TIPOM"]);

                    switch (UsrConect)
                    {
                        // Es un nuevo menú.
                        case 'M':
                            MnuSeccion = opcionMenu == ' ' ? (string)__TFRf.SqlDataRow["SECCION"] : (string)__TFRf.SqlDataRow["NEXT"];
                            break;

                        // Es una función.
                        case 'F':
                            MnuNamespace = System.Convert.ToString(__TFRf.SqlDataRow["NAMESPACE"]).TrimEnd();
                            MnuClase = System.Convert.ToString(__TFRf.SqlDataRow["CLASE"]).TrimEnd();
                            MnuEnsamblado = System.Convert.ToString(__TFRf.SqlDataRow["ENSAMBLADO"]).TrimEnd();
                            MnuEntryPoint = System.Convert.ToString(__TFRf.SqlDataRow["ENTRYPOINT"]).TrimEnd();
                            break;

                        // Comando salir.
                        case 'Q':
                            break;

                        // Opción no controlada.
                        default:
                            SetMessage("SCR-0012", "Tipo no válido");
                            UsrConect = 'N';
                            break;
                    }
                    break;

                // Opción de menú no encontrada.
                case 'N':
                    SetMessage("SCR-0011", "Opción no válida");
                    break;

                // Error BBDD al validar opción de menú.
                case 'C':
                    SetMessage("SCR-0003", "Error BBDD");
                    break;

                // Resto de casos: Error no controlado.
                default:
                    SetMessage("SCR-????", "Error no controlado");
                    UsrConect = 'N';
                    break;
            }

            return UsrConect;
        }

        /// <summary>
        /// Ir al menú anterior.
        /// </summary>
        /// <param name="programa">Código de programa en MENUS</param>
        /// <param name="seccionMenu">Sección a visualizar en pantalla</param>
        /// <param name="opcionMenu">Opción de menú</param>
        /// <param name="cstFichero">Archivo de menús. Def: MENUS</param>
        /// <returns>Resultado (S/N/C)</returns>
        // 
        public char PreviousMenu(string programa, string seccionMenu, Char opcionMenu, string cstFichero = "MENUS")
        {
            Char UsrConect = 'S';
            string strWhere = "";

            strWhere += "MONITOR='" + Monitor + "' And ";
            strWhere += "IDIOMA='" + Idioma + "' And ";
            strWhere += "PROGRAMA='" + programa + "' And ";
            strWhere += "SECCION='" + seccionMenu + "'";

            strWhere += opcionMenu == ' ' ? "" : " And TIPOM='" + opcionMenu + "'";

            UsrConect = __TFRf.SeekRow(cstFichero, SeekWhere: strWhere);

            switch (UsrConect)
            {
                // Opción de menú OK - Validar su tipo.
                case 'S':
                    if (String.IsNullOrWhiteSpace((string)__TFRf.SqlDataRow["PREVIOUS"]) == false)
                        MnuSeccion = System.Convert.ToString(__TFRf.SqlDataRow["PREVIOUS"]);
                    else
                        UsrConect = 'Q';

                    break;

                // Opción de menú no encontrada: Salir.
                case 'N':
                    UsrConect = 'Q';
                    break;

                // Error BBDD al validar opción de menú.
                case 'C':
                    SetMessage("SCR-0003", "Error BBDD");
                    break;

                // Resto de casos: Salir.
                default:
                    UsrConect = 'Q';
                    break;
            }

            return UsrConect;
        }

        /// <summary>
        /// Desconectar el operario activo de la aplicación.
        /// </summary>
        /// <returns>Resultado (S/N)</returns>

        public char DesconectarUsuario()
        {
            Char UsrConect = 'S';
            string whereOpe = "F05cCodOpe='" + CodOpe + "'";

            // Sección Compradio.VFP
            // return System.Convert.ToChar(Iptr.desconectarusuario());

            // Sección Compradio.CS----------------------------------------------
            if (__TFRf.SeekRow("F05C001", "*", whereOpe, "", "") == 'S')
                UsrConect = __TFRf.UpdateCurrentRow("F05cCodTer", "CONN");

            return UsrConect;
        }

        /// <summary>
        /// Finalizar la aplicación.
        /// </summary>
        /// 
        /// <returns>Resultado (S/N)</returns>

        public char FinConexion()
        {
            // Sólo a efectos de Test y formación.
            using (System.Data.Odbc.OdbcConnection cn = new System.Data.Odbc.OdbcConnection(Iptr.cadenadeconexion.ToString()))
            {
                // TEST acceso a BBDD.
                cn.Open();
                string strSQL = "Update F05C001 Set F05cCodTer='CONN' Where F05cCodOpe='0000'";
                using (System.Data.Odbc.OdbcCommand cb = new System.Data.Odbc.OdbcCommand(strSQL, cn))
                {
                    int aaa = cb.ExecuteNonQuery();
                }
            }
            // Fin Test acceso a BBDD.

            return System.Convert.ToChar(Iptr.finconexion());
        }

        /// <summary>
        /// Visualizar el siguiente dato en pantalla.
        /// </summary>
        /// 
        /// <remarks> Utiliza _scrformatlinea para formatear el valor a visualizar</remarks>
        /// 
        /// <seealso cref="_scrformatlinea(DataTableReader)"/>
        /// 
        /// <param name="cstPrograma">Código de programa en MENUS</param>
        /// <param name="cstSeccion">Sección a visualizar en pantalla</param>
        /// <param name="cstFichero">Archivo de menús. Def: PANTALLAS</param>
        /// 
        /// <returns>Resultado (S/N/C)</returns>

        public char ScrGenerarNextLinea(string cstPrograma, string cstSeccion, string cstFichero = "PANTALLAS")
        {
            Char UsrConect = 'S';
            string strWhere = "";

            // Sección Compradio.VFP
            // return System.Convert.ToChar(Iptr.scrgenerarnxtlinea(cstPrograma, cstSeccion));

            // Sección Compradio.CS----------------------------------------------
            switch (CurLinea)
            {
                // Cargar los datos.
                case -1:
                    CurLinea = 0;

                    strWhere += "MONITOR='" + Monitor + "' And ";
                    strWhere += "IDIOMA='" + Idioma + "' And ";
                    strWhere += "PROGRAMA='" + cstPrograma + "' And ";
                    strWhere += "SECCION='" + cstSeccion + "'";

                    UsrConect = __TFRf.SeekRow(cstFichero, SeekWhere: strWhere);
                    UsrConect = (UsrConect == 'S' ? 'X' : UsrConect);
                    break;

                // Obtener el siguiente valor a visualizar.
                default:
                    if (__TFRf.SqlTableReader.Read())
                    {
                        XCoord = (int)__TFRf.SqlTableReader["XCOORD"];
                        YCoord = (int)__TFRf.SqlTableReader["YCOORD"];
                        CurLinea = (int)__TFRf.SqlTableReader["XCOORD"];

                        UsrConect = _scrformatlinea(__TFRf.SqlTableReader);
                    }
                    else
                    {
                        XCoord = 0;
                        YCoord = 0;
                        CurLinea = -1;
                        UsrConect = 'O';
                    }

                    break;
            }

            return UsrConect;
        }

        /// <summary>
        /// Validar la configuración inicial de RF.
        /// </summary>
        /// <returns>Resultado (S/N)</returns>

        private char _inicioconexion()
        {
            char UsrConect = 'S';

            // Asignar propiedades.

            return UsrConect;
        }

        /// <summary>
        /// Formatear propiedades de visualización en pantalla.
        /// </summary>
        /// <remarks> Utilizado desde ScrGenerarNextLinea</remarks>
        /// <seealso cref="ScrGenerarNextLinea(string, string, string)"/>
        /// <param name="dr">DataRow con las propiedades a visualizar</param>
        /// <returns>Resultado (S/N/C)</returns>

        private char _scrformatlinea(DataTableReader dr)
        {
            Char UsrConect = 'S';

            string strPropertyName;
            object strPropertyValue;

            ScrFormato = dr["FORMATO"].ToString().TrimEnd();
            ScrMascara = dr["MASCARA"].ToString().TrimEnd();
            ScrTipo = dr["TIPO"].ToString();
            ScrValor = dr["TEXTO"].ToString().TrimEnd();

            if (String.IsNullOrEmpty(ScrValor)==false)
            {
                strPropertyName = "";
                strPropertyValue = ScrValor;
            }
            else
            {
                strPropertyName = dr["PROPIEDAD"].ToString().TrimEnd();

                try
                {
                    strPropertyValue = this.GetType().GetProperty(strPropertyName).GetValue(this, null);
                }
                catch
                {
                    // La propiedad no existe.
                    strPropertyValue = strPropertyName +  " Not Found ";
                }
            }

            ScrValor = strPropertyValue.ToString();

            return UsrConect;
        }

        /// <summary>
        /// Inicializar propiedades de configuración de la aplicación.
        /// </summary>
        /// 
        /// <remarks> Lee configuración de archivo XML</remarks>
        /// 
        /// <returns> Resultado (true / false)</returns>

        private bool _xmlinicializar()
        {
            bool retorno = false;

            // Valores por defecto.
            CodProD = __TFXmlSGA.XmlSeekSingleKeyValue("/SGA/COMMON/DEFAULT/CODPRODEF");            // Propietario por defecto.
            retorno = !String.IsNullOrEmpty(CodProD);

            // Constantes generales.
            retorno = (retorno == true && (SitStkE = __TFXmlSGA.XmlSeekSingleKeyValue("/SGA/COMMON/DEFAULT/SITSTKE"))!= null ? true : false);
            retorno = (retorno == true && (TipMovE = __TFXmlSGA.XmlSeekSingleKeyValue("/SGA/COMMON/DEFAULT/TIPMOVE")) != null ? true : false);
            retorno = retorno == true && (TipMovO = __TFXmlSGA.XmlSeekSingleKeyValue("/SGA/COMMON/DEFAULT/TIPMOVO"))!= null ? true : false;
            retorno = retorno == true && (TipMovD = __TFXmlSGA.XmlSeekSingleKeyValue("/SGA/COMMON/DEFAULT/TIPMOVD"))!= null ? true : false;
            retorno = retorno == true && (TipMovX = __TFXmlSGA.XmlSeekSingleKeyValue("/SGA/COMMON/DEFAULT/TIPMOVX"))!= null ? true : false;
            retorno = retorno == true && (TipMovR = __TFXmlSGA.XmlSeekSingleKeyValue("/SGA/COMMON/DEFAULT/TIPMOVR"))!= null ? true : false;

            return retorno;
        }

        /// <summary>
        /// Inicializar propiedades de visualización en pantalla.
        /// </summary>
        /// <seealso cref="ScrGenerarNextLinea(string, string, string)"/>
        /// <seealso cref="_scrformatlinea(DataTableReader)"/>

        private void _scrinicializar()
        {
            XCoord = 0;
            YCoord = 0;
            CurLinea = -1;
            ScrValor = "";
            ScrMascara = "";
            ScrFormato = "";
            ScrTipo = "";

            return;
        }
    }
}
