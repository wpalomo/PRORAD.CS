
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TF;

namespace COMPRADIO
{
    /// <summary>
    /// Propiedades y funciones de uso interno de la clase.
    /// </summary>
    public partial class PRORAD
    {
        //------------------------------------------------------------------
        // Propiedades públicas de control de la clase.
        //------------------------------------------------------------------
        /// <summary> Acceso a funciones de COMPRADIO.VFP</summary>
        /// 
        public compradio.proradClass Iptr = new compradio.proradClass();

        /// <summary>Acceso a BBDD, uso interno.</summary>
        /// 
        public Tf __TFRf;

        /// <summary> Acceso a fichero XML de configuración (RF).</summary>
        /// 
        public Xml __TFXmlRF;

        /// <summary> Acceso a fichero XML de configuración (SGA).</summary>
        /// 
        public Xml __TFXmlSGA;

        /// <summary> Código general de error. </summary>
        /// 
        public String UsrErrorC
        { get; set; }

        /// <summary> Mensaje general de error. </summary>
        /// 
        public String UsrError
        { get; set; }

        /// <summary> Mensaje extendido de error. </summary>
        public String UsrErrorE
        { get; set; }

        //------------------------------------------------------------------
        // Propiedades públicas de retorno de datos en consultas.
        //------------------------------------------------------------------

        /// <summary> Cantidad recontada.</summary>
        public float CnsCanRec
        {
            get { return (float)Iptr.cnscanrec; }
        }

        /// <summary> Código EAN.</summary>
        public String CnsCodEAN
        {
            get { return (string)Iptr.cnscodean; }
        }

        /// <summary> Descripción artículo.</summary>
        public String CnsDesArt
        {
            get { return (string)Iptr.cnsdesart; }
        }

        /// <summary> Estado albarán.</summary>
        public String CnsEstAlb
        {
            get { return (string)Iptr.cnsestalb; }
        }

        /// <summary> Fecha de entrada de albarán.</summary>
        public DateTime CnsFecEnt
        {
            get { return (DateTime)Iptr.cnsfecent; }
        }

        /// <summary> Número de albarán.</summary>
        public String CnsNumAlb
        {
            get { return (string)Iptr.cnsnumalb; }
        }

        /// <summary> Orden.</summary>
        public int CnsOrden
        {
            get { return System.Convert.ToInt32(Iptr.cnsorden); }
        }

        //------------------------------------------------------------------
        // Propiedades para visualizar datos en pantalla.
        //------------------------------------------------------------------

        private String mensajesrf = "PRORAD";
        /// <summary> Fichero de mensajes para RF en la BBDD</summary>
        public String MensajesRF
        {
            get { return mensajesrf; }
            set { mensajesrf = value; }
        }

        private String mensajessga = "SGA";
        /// <summary> Fichero de mensajes para sga en la BBDD</summary>
        public String MensajesSGA
        {
            get { return mensajessga; }
            set { mensajessga = value; }
        }

        private String scrvalor;
        /// <summary> Valor a visualizar en pantalla.</summary>
        public String ScrValor
        {
            get
            {
                // return (String)Iptr.scrvalor;
                return scrvalor;
            }

            set
            {
                Iptr.scrvalor = value;
                scrvalor = value;
            }
        }

        private String scrmascara;
        /// <summary>Máscara de formateo del valor a visualizar en pantalla.</summary>
        public String ScrMascara
        {
            get
            {
                // return (String)Iptr.scrmascara;
                return scrmascara;
            }

            set
            {
                Iptr.scrmascara = value;
                scrmascara = value;
            }
        }

        private String scrformato;
        /// <summary>Cadena de formateo del valor a visualizar en pantalla.</summary>
        public String ScrFormato
        {
            get
            {
                // return (String)Iptr.scrformato;
                return scrformato;
            }

            set
            {
                Iptr.scrformato = value;
                scrformato = value;
            }
        }

        private String scrtipo;
        /// <summary>Tipo de campo del valor a visualizar en pantalla.</summary>
        public String ScrTipo
        {
            get
            {
                //return (String)Iptr.scrtipo;
                return scrtipo;
            }

            set
            {
                Iptr.scrtipo = value;
                scrtipo = value;
            }
        }

        private int xcoord;
        /// <summary>Coordenada X del valor a visualizar en pantalla.</summary>
        public int XCoord
        {
            get
            {
                // return System.Convert.ToInt32(Iptr.xcoord);
                return xcoord;
            }

            set
            {
                Iptr.xcoord = value;
                xcoord = value;
            }
        }

        private int ycoord;
        /// <summary>Coordenada Y del valor a visualizar en pantalla.</summary>
        public int YCoord
        {
            get
            {
                // return System.Convert.ToInt32(Iptr.ycoord);
                return ycoord;
            }

            set
            {
                Iptr.ycoord = value;
                ycoord = value;
            }
        }

        private int curlinea;
        /// <summary>Línea actual del valor a visualizar en pantalla.</summary>
        public int CurLinea
        {
            get
            {
                // return System.Convert.ToInt32(Iptr.curlinea);
                return curlinea;
            }

            set
            {
                Iptr.curlinea = value;
                curlinea = value;
            }
        }

        //------------------------------------------------------------------
        // Propiedades del monitor activo.
        //------------------------------------------------------------------

        /// <summary>Monitor</summary>
        /// 
        public string Monitor
        { get; set;  }

        /// <summary>Tipo de monitor</summary>
        /// 
        public char TipoMonitor
        { get; set; }

        private int anchopantalla;
        /// <summary>Ancho caracteres pantalla.</summary>
        public int AnchoPantalla
        {
            get
            {
                // return System.Convert.ToInt32(Iptr.anchopantalla);
                return anchopantalla;
            }

            set
            {
                Iptr.setparam("ANCHOPANTALLA", value);
                anchopantalla = value;
            }
        }

        private int altopantalla;
        /// <summary>Alto caracteres pantalla.</summary>
        public int AltoPantalla
        {
            get
            {
                // return System.Convert.ToInt32(Iptr.altopantalla);
                return altopantalla;
            }

            set
            {
                Iptr.setparam("ALTOPANTALLA", value);
                altopantalla = value;
            }
        }

        private string idioma;
        /// <summary>Idioma de los mensajes.</summary>
        public string Idioma
        {
            get
            {
                // return System.Convert.ToInt32(Iptr.idioma);
                return idioma;
            }

            set
            {
                Iptr.setparam("IDIOMA", value);
                idioma = value;
            }
        }

        private int lineainputs;
        /// <summary>Línea de inputs por pantalla.</summary>
        public int LineaInputs
        {
            get
            {
                // return System.Convert.ToInt32(Iptr.lineainputs);
                return lineainputs;
            }

            set
            {
                Iptr.lineainputs = value;
                lineainputs = value;
            }
        }

        private int columnainputs;
        /// <summary>Columna de inputs por pantalla.</summary>
        public int ColumnaInputs
        {
            get
            {
                // return System.Convert.ToInt32(Iptr.columnainputs);
                return columnainputs;
            }

            set
            {
                Iptr.columnainputs = value;
                columnainputs = value;
            }
        }

        //------------------------------------------------------------------
        // Propiedades para gestión de menús.
        //------------------------------------------------------------------

        private String mnufile = "MENUS";
        /// <summary> Fichero de menús en la BBDD</summary>
        public String MnuFile
        {
            get {return mnufile; }
            set { mnufile = value; }
        }

        private String mnupantallas = "PANTALLAS";
        /// <summary> Fichero de configuración de pantallas en la BBDD</summary>
        public String MnuPantallas
        {
            get { return mnupantallas; }
            set { mnupantallas = value; }
        }

        private String mnuseccion = "  ";
        /// <summary>Sección menú.</summary>
        public String MnuSeccion
        {
            get
            {
                return mnuseccion;
            }

            set
            {
                mnuseccion = value;
            }
        }

        private String mnunamespace = "  ";
        /// <summary>NameSpace menú.</summary>
        public String MnuNamespace
        {
            get
            {
                return mnunamespace ;
            }

            set
            {
                mnunamespace = value;
            }
        }

        private String mnuclass = "  ";
        /// <summary>Clase menú.</summary>
        public String MnuClase
        {
            get
            {
                return mnuclass;
            }

            set
            {
                mnuclass = value;
            }
        }

        private String mnuassembly = "  ";
        /// <summary>Ensamblado menú.</summary>
        public String MnuEnsamblado
        {
            get
            {
                return mnuassembly;
            }

            set
            {
                mnuassembly = value;
            }
        }

        private String mnuentrypoint = "  ";
        /// <summary>Punto de entrada menú.</summary>
        public String MnuEntryPoint
        {
            get
            {
                return mnuentrypoint;
            }

            set
            {
                mnuentrypoint = value;
            }
        }

        //------------------------------------------------------------------
        // Propiedades públicas de configuración de la clase.
        //------------------------------------------------------------------

        private _Tf.DBEnvironments dbentorno = _Tf.DBEnvironments.SQL;
        /// <summary> Entorno BBDD por defecto </summary>
        public _Tf.DBEnvironments DBENtorno
        {
            get { return dbentorno;  }
            set { dbentorno = value; }
        }

        private string dbversion = "1.0";
        /// <summary> Versión BBDD por defecto </summary>
        public string DBVersion
        {
            get { return dbversion; }
            set { dbversion = value; }
        }

        private _Tf.DBProviders dbprovider = _Tf.DBProviders.ODBC;
        /// <summary> Proveedor BBDD por defecto </summary>
        public _Tf.DBProviders DBProveedor
        {
            get { return dbprovider; }
            set { dbprovider = value; }
        }

        private String codprodef = null;
        /// <summary> Código de propietario por defecto</summary>
        public String CodProD
        {
            get
            { return codprodef; }

            set
            {
                // Validar que exista el propietario.
                char UsrConect = __TFRf.SeekRow("F01P001", SeekWhere: "F01pCodigo='" + value + "'");

                switch (UsrConect)
                {
                    case 'S':
                        Iptr.codpro = value;
                        codprodef = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(String.Format(GetMessage("_COM-0001", "No existe {0}"), value));
                        // break;
                }
            }
        }

        private String sitstke = null;
        /// <summary>SSTK stock disponible.</summary>
        public String SitStkE
        {
            get
            { return sitstke; }

            set
            {
                // Validar que exista la SSTK.
                char UsrConect = __TFRf.SeekRow("F00C001", SeekWhere: "F00cCodStk='" + value + "'");

                switch (UsrConect)
                {
                    case 'S':
                        Iptr.sitstke = value;
                        sitstke = value;
                        break;

                    default:
                        break;
                }
            }
        }

        private String tipmove = null;
        /// <summary>TMOV entrada de material.</summary>
        public String TipMovE
        {
            get
            { return tipmove; }

            set
            {
                // Validar que exista el TMOV.
                char UsrConect = __TFRf.SeekRow("F00B001", SeekWhere: "F00bCodMov='" + value + "'");

                switch (UsrConect)
                {
                    case 'S':
                        Iptr.tipmove = value;
                        tipmove = value;
                        break;

                    default:
                        break;
                }
            }
        }

        private String tipmovx = null;
        /// <summary>TMOV expedición.</summary>
        public String TipMovX
        {
            get
            { return tipmovx; }

            set
            {
                // Validar que exista el TMOV.
                char UsrConect = __TFRf.SeekRow("F00B001", SeekWhere: "F00bCodMov='" + value + "'");

                switch (UsrConect)
                {
                    case 'S':
                        Iptr.tipmovx = value;
                        tipmovx = value;
                        break;

                    default:
                        break;
                }
            }
        }

        private String tipmovo = null;
        /// <summary>TMOV preparación (Salida origen).</summary>
        public String TipMovO
        {
            get
            { return tipmovo; }

            set
            {
                // Validar que exista el TMOV.
                char UsrConect = __TFRf.SeekRow("F00B001", SeekWhere: "F00bCodMov='" + value + "'");

                switch (UsrConect)
                {
                    case 'S':
                        Iptr.tipmovo = value;
                        tipmovo = value;
                        break;

                    default:
                        break;
                }
            }
        }

        private String tipmovd = null;
        /// <summary>TMOV preparación (Entrada destino).</summary>
        public String TipMovD
        {
            get
            { return tipmovd; }

            set
            {
                // Validar que exista el TMOV.
                char UsrConect = __TFRf.SeekRow("F00B001", SeekWhere: "F00bCodMov='" + value + "'");

                switch (UsrConect)
                {
                    case 'S':
                        Iptr.tipmovd = value;
                        tipmovd = value;
                        break;

                    default:
                        break;
                }
            }
        }

        private String tipmovr = null;
        /// <summary>TMOV preparación (Consolidación).</summary>
        public String TipMovR
        {
            get
            { return tipmovr; }

            set
            {
                // Validar que exista el TMOV.
                char UsrConect = __TFRf.SeekRow("F00B001", SeekWhere: "F00bCodMov='" + value + "'");

                switch (UsrConect)
                {
                    case 'S':
                        Iptr.tipmovr = value;
                        tipmovr = value;
                        break;

                    default:
                        break;
                }
            }
        }

        //------------------------------------------------------------------
        // Propiedades públicas de usuario de la clase.
        //------------------------------------------------------------------

        private String codart = null;
        /// <summary>Código de artículo.</summary>
        public String CodArt
        {
            get
            {
                codart = Iptr.codart.ToString();
                return codart;
            }

            set
            {
                Iptr.codart = value;
                codart = value;
            }
        }

        private String codope;
        /// <summary>Código de operario.</summary>
        public String CodOpe
        {
            get
            {
                // codope = Iptr.codope.ToString();
                return codope;
            }

            set
            {
                Iptr.codope = value;
                codope = value;
            }
        }

        private double canrec;
        /// <summary>Cantidad recontada.</summary>
        public Double CanRec
        {
            get
            {
                canrec = Convert.ToDouble(Iptr.canrec);
                return canrec;
            }

            set
            {
                Iptr.canrec = value;
                canrec = value;
            }
        }

        private double canrecrf;
        /// <summary>Cantidad recontada RF.</summary>
        public Double CanRecRF
        {
            get
            {
                canrecrf = Convert.ToDouble(Iptr.canrecrf);
                return canrecrf;
            }

            set
            {
                Iptr.canrec = value;
                canrec = value;
            }
        }

        private String codpro;
        /// <summary> Código de propietario.</summary>
        public String CodPro
        {
            get
            {
                codpro = Iptr.codpro.ToString();
                return codpro;
            }

            set
            {
                Iptr.codart = value;
                codart = value;
            }
        }

        private String desart;
        /// <summary> Descripción del artículo.</summary>
        public String DesArt
        {
            get
            {
                desart = Iptr.desart.ToString();
                return desart;
            }

            set
            {
                Iptr.desart = value;
                desart = value;
            }
        }

        private String nomope;
        /// <summary> Nombre de operario.</summary>
        public String NomOpe
        {
            get
            {
                // codope = Iptr.nombre.ToString();
                return codope;
            }

            set
            {
                Iptr.nombre = value;
                nomope = value;
            }
        }

        private String grpope;
        /// <summary> Grupo del operario.</summary>
        public String GrpOpe
        {
            get
            {
                return grpope;
            }

            set
            {
                grpope = value;
            }
        }

        private String numalb = null;
        /// <summary> Número de Albarán.</summary>
        /// 
        /// <remarks> Asigna valores a Compradio.Vfp, por compatibilidad</remarks>
        public String NumAlb
        {
            get
            {
                numalb = (string)Iptr.numalb;
                return numalb;
            }

            set
            {
                Iptr.numalb = value;
                numalb = value;
            }
        }

        private String numpal;
        /// <summary> Número de palet.</summary>
        /// 
        /// <remarks> Asigna valores a Compradio.Vfp, por compatibilidad</remarks>
        public String NumPal
        {
            get
            {
                numpal = Iptr.numpal.ToString();
                return numpal;
            }

            set
            {
                Iptr.numpal = value;
                numpal = value;
            }
        }

        //------------------------------------------------------------------
        // Funciones privadas de uso general de la aplicación.
        //------------------------------------------------------------------

        /// <summary>
        /// Validar el código EAN de un artículo. 
        /// </summary>
        /// 
        /// <remarks> Recibe, opcionalmente, el propietario, para validar por código interno.</remarks>
        /// 
        /// <param name="codigoEAN"> Código EAN del artículo a validar</param>
        /// <param name="CodigoPropietario"> Código de propietario</param>
        /// 
        /// <returns> Resultado (S/N/C)</returns>

        internal Char _validarEANArt(string codigoEAN, string CodigoPropietario = null)
        {
            Char UsrConect = 'S';
            string strWhere;

            if(String.IsNullOrEmpty(codigoEAN)==true)
            {
                SetMessage("_VEA-0001", "EAN en blanco");
                UsrConect = 'N';
                return UsrConect;
            }

            strWhere = codigoEAN;

            return UsrConect;
        }

        //------------------------------------------------------------------
        // Funciones públicas de uso general de la aplicación.
        //------------------------------------------------------------------

        /// <summary>
        /// Asignar mensajes de error. Forma general.
        /// </summary>
        /// 
        /// <param name="parametros">
        /// Array de parámetros del mensaje:
        ///     - Codigo del mensaje.
        ///     - Mensaje por defecto.
        ///     - Fichero de mensajes. Def: PRORAD.
        /// </param>
        /// 
        /// <remarks>
        /// Ejemplo de paso de parámetros recibidos en forma de array.
        /// </remarks>
        /// 
        /// <seealso cref="GetMessage(string, string, string)"/>

        public void SetMessage(params object[] parametros)
        {
            char UsrConect;

            string strMessageCode = (string)parametros[0];
            string strDefaultMessage = parametros.Length >=2 ? (string)parametros[1] : "";
            string strDefaultFile = parametros.Length >= 3 ? (string)parametros[2] : MensajesRF;

            // Obtener la descripción del mensaje.
            UsrErrorC = strMessageCode;
            UsrError = String.IsNullOrEmpty(strDefaultMessage) ? "" : strDefaultMessage;

            UsrConect = __TFRf.SeekRow(strDefaultFile, SeekWhere: "CODIGO='" + strMessageCode + "'");
            if (UsrConect == 'S')
            {
                UsrError = __TFRf.SqlDataRow["TEXTOE"].ToString();
            }

            return;
        }

        /// <summary>
        /// Obtener los textos de mensajes de error. Forma general.
        /// </summary>
        /// 
        /// <param name="messageCode"> Código de mensaje</param>
        /// <param name="defaultMessage"> Mensaje por defecto</param>
        /// <param name="messageFile"> Fichero de mensajes. Def: MensajesRF</param>
        /// 
        /// <remarks>
        /// Ejemplo de paso de parámetros de entrada de forma standard.
        /// </remarks>
        /// 
        /// <seealso cref="SetMessage(object[])"/>
        /// 
        /// <returns>Texto del mensaje</returns>

        public string GetMessage(string messageCode, string defaultMessage = "", string messageFile = null)
        {
            char UsrConect;

            string returnedMessage = defaultMessage.TrimEnd();


            // Obtener la descripción del mensaje.
            UsrConect = __TFRf.SeekRow(messageFile==null ? MensajesRF : messageFile, SeekWhere: "CODIGO='" + messageCode + "'");
            if (UsrConect == 'S')
            {
                returnedMessage = __TFRf.SqlDataRow["TEXTOE"].ToString().TrimEnd();
            }

            return returnedMessage;
        }
    }
}
