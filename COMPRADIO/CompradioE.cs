
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace COMPRADIO
{
    /// <summary>
    /// Funciones para los procesos de Entrada de Material.
    /// </summary>
    public partial class PRORAD
    {
        //-------------------------------------------------------
        // Propiedades de la clase.
        //-------------------------------------------------------
        private Char flgalbp;
        /// <summary></summary>
        public Char FlgAlbP
        {
            get
            {
                flgalbp = Convert.ToChar(Iptr.flgalbp);
                return flgalbp;
            }

            set
            {
                Iptr.flgalbp = System.Convert.ToString(value);
                flgalbp = value;
            }
        }

        //-------------------------------------------------------
        // Entradas - General.
        //-------------------------------------------------------

        /// <summary>
        /// Entradas - Consulta de albaranes.
        /// </summary>
        /// <param name="cstSkipR">
        /// Operación a realizar:
        ///     V - Validar albarán y cargar líneas.
        ///     M - Mostrar el siguiente albarán.
        ///     S - Seleccionar albarán.
        ///     Z - Cerrar proceso de consulta de albaranes.
        /// </param>
        /// <param name="cstAlbaran">Albarán inicial</param>
        /// <returns>Resultado (S/N/C)</returns>

        public char ConsultaAlbaranes(string cstSkipR, string cstAlbaran)
        {
            return System.Convert.ToChar(Iptr.consultaalbaranes(cstSkipR, cstAlbaran));
        }

        /// <summary>
        /// Entradas - Consulta de un albarán.
        /// </summary>
        /// <param name="cstSkipR">
        /// Operación a realizar:
        ///     V - Validar albarán y cargar líneas.
        ///     M - Mostrar la siguiente línea del albarán.
        ///     Z - Cerrar proceso de consulta de detalle del albarán.
        /// </param>
        /// <param name="cstAlbaran">Albarán a consultar</param>
        /// <returns></returns>

        public char ConsultaAlbaran(string cstSkipR, string cstAlbaran)
        {
            return System.Convert.ToChar(Iptr.consultaalbaran(cstSkipR, cstAlbaran));
        }

        //-------------------------------------------------------
        // Entradas - Recuento.
        //-------------------------------------------------------

        /// <summary>
        /// Recuento: Inicializar.
        /// </summary>
        /// <returns>Resultado (S/N/C)</returns>

        public char RecInicializar()
        {
            Char UsrConect = 'S';

            // Sección Compradio.VFP
            // return System.Convert.ToChar(Iptr.recinicializar());

            // Sección Compradio.CS----------------------------------------------
            CodPro = "";
            CodArt = "";
            NumAlb = "";
            CanRec = 0;
            CanRecRF = 0;
            DesArt = "";
            NumPal = "";

            FlgAlbP = 'P';

            return UsrConect;
        }

        /// <summary>
        /// Recuento: Validar el albarán.
        /// </summary>
        /// 
        /// <param name="strAlbaran"> Albarán a validar</param>
        /// 
        /// <returns> Resultado (S/N/C)</returns>

        public char RecValidarAlbaran(string strAlbaran)
        {
            Char UsrConect = 'S';
            string strWhere;

            // Sección Compradio.VFP
            // return System.Convert.ToChar(Iptr.recvalidaralbaran(strAlbaran));

            if(String.IsNullOrEmpty(strAlbaran))
            {
                SetMessage("_RVA-0005", "Debe entrar un valor");
                return 'N';
            }

            if (strAlbaran.TrimEnd().Length > 10)
            {
                SetMessage("_RVA-0001", "Valor muy grande");
                return 'N';
            }

            // Validar el albarán.
            strAlbaran = strAlbaran.Trim().PadLeft(10, '0');
            strWhere = "F18mNumEnt='" + strAlbaran + "'";
            UsrConect = __TFRf.SeekRow("F18M001", SeekWhere: strWhere);

            switch(UsrConect)
            {
                // OK: Guardar propiedades.
                case 'S':
                    NumAlb = strAlbaran;
                    CodPro = System.Convert.ToString(__TFRf.SqlDataRow["F18mCodPro"]);
                    break;

                // Albarán no existe.
                case 'N':
                    SetMessage("_RVA-0002", "Alb. no existe");
                    break;

                // Error en BBDD.
                case 'C':
                    SetMessage("_RVA-0901", "Error BBDD");
                    break;

                // Resto de casos: Error no controlado.
                default:
                    SetMessage("_RVA-????", "Error no controlado");
                    UsrConect = 'N';
                    break;
            }

            return UsrConect;
        }

        /// <summary>
        /// Recuento: Validar el EAN del artículo.
        /// </summary>
        /// 
        /// <param name="strEAN"> Código EAN del artículo.</param>
        /// 
        /// <returns>Resultado (S/N/C)</returns>

        public char RecValidarEAN(string strEAN)
        {
            Char UsrConect = 'S';

            // Sección Compradio.VFP ----------------------------
            UsrConect = System.Convert.ToChar(Iptr.recvalidarean(strEAN));
            UsrError = System.Convert.ToString(Iptr.usrerror);
            UsrErrorC = System.Convert.ToString(Iptr.usrerrorc);

            // Sección Compradio.CS -----------------------------
            UsrConect = _validarEANArt(strEAN, CodPro);

            switch (UsrConect)
            {
                // EAN Ok.
                case 'S':
                    break;

                // No existe ó valor en blanco. - Mensaje ya asignado.
                case 'N':
                    break;

                // Error BBDD.
                case 'C':
                    SetMessage("_VEA-0002", "Error en EAN");
                    break;

                // Opción no controlada.
                default:
                    SetMessage("_RVA-????", "Error no controlado");
                    UsrConect = 'N';
                    break;
            }

            return UsrConect;
        }

        /// <summary>
        /// Recuento: Validar la cantidad recontada.
        /// </summary>
        /// 
        /// <param name="strCantidad">Cantidad recontada a validar.</param>
        /// 
        /// <returns>Resultado (S/N/C)</returns>

        public char RecValidarCnt(string strCantidad)
        {
            return System.Convert.ToChar(Iptr.recvalidarcnt(strCantidad));
        }

        /// <summary>
        /// Recuento: Actualizar.
        /// </summary>
        /// 
        /// <returns>Resultado (S/N/C)</returns>

        public char RecActualizar()
        {
            char UsrConect = 'S';
            string strWhere;
            // Sección Compradio.VFP --------------------------------
            // return System.Convert.ToChar(Iptr.recactualizar());

            // Sección Compradio.CS ---------------------------------
            strWhere = "F18nNumAlb='" + NumAlb + "'";
            if(__TFRf.SeekRow("F18N001", SeekWhere: strWhere)=='S')
                UsrConect = __TFRf.UpdateCurrentRow("F18nCanRec", CanRecRF);

            return UsrConect;
        }

        /// <summary>
        /// Recuento: Finalizar.
        /// </summary>
        /// 
        /// <returns>Resultado (S/N)</returns>

        public char RecFinalizar()
        {
            return System.Convert.ToChar(Iptr.recfinalizar());
        }
    }
}
