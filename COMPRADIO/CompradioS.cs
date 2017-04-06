
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace COMPRADIO
{
    /// <summary>
    /// FUnciones generales para los procesos de Salida de Material.
    /// </summary>
    public partial class PRORAD
    {
        //-------------------------------------------------------
        // Propiedades de la clase.
        //-------------------------------------------------------
        private char flgalbn;
        /// <summary></summary>
        public char FlgAlbN
        {
            get
            {
                return flgalbn;
            }

            set
            {
                flgalbn = value;
            }
        }

        //-------------------------------------------------------
        // Salidas - General.
        //-------------------------------------------------------

        /// <summary>
        /// Salidas - Foo.
        /// </summary>
        /// <returns></returns>
        // 
        public bool Foo()
        {
            bool retorno = true;
            return retorno;
        }

        //-------------------------------------------------------
        // Salidas - Preparación Palet.
        //-------------------------------------------------------

        /// <summary>
        /// Salida Palet: Inicializar.
        /// </summary>
        /// <returns>Resultado (S/N/C)</returns>

        public char SalInicializar()
        {
            Char UsrConect = 'S';

            // Sección Compradio.VFP
            UsrConect = System.Convert.ToChar(Iptr.salinicializar());

            // Sección Compradio.CS----------------------------------------------

            return UsrConect;
        }

        /// <summary>
        /// Salida Palet: Cargar siguiente palet a preparar.
        /// </summary>
        /// <returns>Resultado (S/N/C)</returns>

        public char SalPeticionUbicacion()
        {
            Char UsrConect = 'S';

            // Sección Compradio.VFP
            UsrConect = System.Convert.ToChar(Iptr.salpeticionubicacion());
            UsrError = System.Convert.ToString(Iptr.usrerror);
            UsrErrorC = System.Convert.ToString(Iptr.usrerrorc);

            // Sección Compradio.CS----------------------------------------------

            return UsrConect;
        }

        /// <summary>
        /// Salida Palet: Validar la ubicación origen.
        /// </summary>
        /// <param name="codigoUbicacion">Código de ubicación</param>
        /// <param name="dcUbicacion">Dígito de control de la ubicación</param>
        /// <returns>Resultado (S/N/C)</returns>

        public char SalValidarOrigen(string codigoUbicacion, string dcUbicacion)
        {
            Char UsrConect = 'S';

            // Sección Compradio.VFP
            UsrConect = System.Convert.ToChar(Iptr.salvalidarorigen(codigoUbicacion, dcUbicacion));
            UsrError = System.Convert.ToString(Iptr.usrerror);
            UsrErrorC = System.Convert.ToString(Iptr.usrerrorc);

            // Sección Compradio.CS----------------------------------------------

            return UsrConect;
        }

        /// <summary>
        /// Salida Palet: Validar el palet / SSCC.
        /// </summary>
        /// <param name="numeroPalet">número de palet / SSCC a validar</param>
        /// <returns>Resultado</returns>

        public char SalValidarPalet(string numeroPalet)
        {
            Char UsrConect = 'S';

            // Sección Compradio.VFP
            UsrConect = System.Convert.ToChar(Iptr.salvalidarpalet(numeroPalet));
            UsrError = System.Convert.ToString(Iptr.usrerror);
            UsrErrorC = System.Convert.ToString(Iptr.usrerrorc);

            // Sección Compradio.CS----------------------------------------------

            return UsrConect;
        }

        /// <summary>
        /// Salida Palet: Actualizar.
        /// </summary>
        /// <returns>Resultado</returns>

        public char SalActualizar()
        {
            Char UsrConect = 'S';

            // Sección Compradio.VFP
            UsrConect = System.Convert.ToChar(Iptr.salactualizar());
            UsrError = System.Convert.ToString(Iptr.usrerror);
            UsrErrorC = System.Convert.ToString(Iptr.usrerrorc);

            // Sección Compradio.CS----------------------------------------------

            return UsrConect;
        }

        /// <summary>
        /// Salida Palet: Finalizar.
        /// </summary>
        /// <returns>Resultado (S/N/C)</returns>

        public char SalFinalizar()
        {
            return System.Convert.ToChar(Iptr.salfinalizar());
        }
    }
}
