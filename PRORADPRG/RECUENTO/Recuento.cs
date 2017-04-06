
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRORAD;
using ENTRADA.CNS;

namespace ENTRADAS
{
    public class RECUENTO : Base
    {
        private const int _LECT_ALBARAN = 1;
        private const string _LECT_ALBARAN_1 = "ERA-0001";
        private const string _LECT_ALBARAN_2 = "ERA-1001";

        private const int _VALIDAR_ALBARAN = 2;
        private const int _LECT_CODEAN = 3;
        private const string _LECT_CODEAN_1 = "ERA-0002";

        private const int _LECT_CANTID = 4;
        private const string _LECT_CANTID_1 = "ERA-0003";
        private const string _LECT_CANTID_2 = "ERA-1003";
        private const string _LECT_CANTID_3 = "ERA-2003";

        private const int _MOSTRAR_ALBARAN = 5;
        private const int _DATOS_OK = 6;
        private const string _DATOS_OK_1 = "ERA-0011";

        private const int _ACTUALIZAR = 7;

        private const int _INICIALIZAR = 21;
        private const int _FINALIZAR = 22;
        private const int _NO_OPERACION = 23;

        private const int _CONS_ALBARANES = 31;
        private const int _PONER_CERO = 32;

        private const string _PROGRAMA = "RECUENTO.CPP";

        // Displays en pantalla.
        public void Pantalla_RecuentoMaterial(params object[] parametrosE)
        {
            COMPRADIO.PRORAD Iptr = (COMPRADIO.PRORAD)parametrosE[0];
            string cstSeccion = (string)parametrosE[1];

            clrscr();

            // Visualizar campos parametrizados.
            DisplayUserScr(Iptr, _PROGRAMA, cstSeccion);
            clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0);

            return;
        }

        // Proceso de recuento de material.
        public void RecuentoMaterial(params object[] parametrosE)
        {
            COMPRADIO.PRORAD Iptr = (COMPRADIO.PRORAD)parametrosE[0];

            CString cstCodEAN = new CString();
            CString cstCantid = new CString();
            CString cstAlbaran = new CString();

            string cstLectura = null;

            int ret = 0, opc;
            char RetVal;

            CONSULTAALBARANES oCns = null;

            opc = _INICIALIZAR;

            try
            {
                do
                {
                    switch (opc)
                    {
                        case _INICIALIZAR:
                            cstCodEAN = "";
                            cstCantid = "";
                            cstAlbaran = "";

                            cstLectura = "";

                            // Inicializar estado recuento de material por albarán.
                            RetVal = System.Convert.ToChar(Iptr.RecInicializar());

                            switch (RetVal)
                            {
                                // OK
                                case 'S':
                                    opc = _LECT_ALBARAN;
                                    break;

                                // Resto de casos: Tomar como error.
                                default:
                                    // NOK
                                    opc = _FINALIZAR;
                                    break;
                            }
                            break;

                        // Pedir nº de albarán a recontar
                        case _LECT_ALBARAN:
                            cstAlbaran = "";

                            Pantalla_RecuentoMaterial(Iptr, _SECCION_1);

                            clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf(Iptr.GetMessage(_LECT_ALBARAN_1, "Introduzca albaran"));
                            clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1); printf(Iptr.GetMessage(_LECT_ALBARAN_2, "<F2:Consulta>"));
                            clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2);

                            ret = leer_char(ref cstLectura, 10);
                            cstAlbaran = cstLectura;

                            switch (ret)
                            {
                                // Abandonar programa
                                case _TECLA_ESC:
                                    opc = _FINALIZAR;
                                    break;

                                // Consulta de albaranes
                                case _TECLA_F2:
                                    opc = _CONS_ALBARANES;
                                    break;

                                case _TECLA_0:
                                    opc = _VALIDAR_ALBARAN;
                                    break;

                                // Resto de teclas: ignorar.
                                default:
                                    opc = _LECT_ALBARAN;
                                    break;
                            }
                            break;

                        // Validar el nº de albarán
                        // Se viene desde _LECT_ALBARAN (TECLA_0)  y _CONS_ALBARAN
                        case _VALIDAR_ALBARAN:
                            RetVal = Iptr.RecValidarAlbaran(cstAlbaran);

                            switch (RetVal)
                            {
                                // OK
                                case 'S':
                                    opc = _LECT_CODEAN;
                                    break;

                                // NOK
                                case 'N':
                                    UserError(Iptr);
                                    opc = _LECT_ALBARAN;
                                    break;

                                // Error en BBDD.
                                case 'C':
                                    UserError(Iptr);
                                    opc = _FINALIZAR;
                                    break;

                                // Resto de casos: Tomar como OK
                                default:
                                    // Albarán OK
                                    opc = _LECT_CODEAN;
                                    break;
                            }
                            break;

                        // Solicitamos código de barras del artículo.
                        case _LECT_CODEAN:
                            cstCodEAN = "";
                            cstCantid = "";

                            Pantalla_RecuentoMaterial(Iptr, _SECCION_1);

                            clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf(Iptr.GetMessage(_LECT_CODEAN_1, "Introduzca EAN"));
                            clreol(0, _LINEA_INPUTS + 1);
                            clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2);

                            ret = leer_char(ref cstLectura, 20);
                            cstCodEAN = cstLectura;

                            switch (ret)
                            {
                                // Ir a pedir albarán.
                                case _TECLA_ESC:
                                    opc = _LECT_ALBARAN;
                                    break;

                                case _TECLA_0:
                                    RetVal = System.Convert.ToChar(Iptr.RecValidarEAN(cstCodEAN));

                                    switch (RetVal)
                                    {
                                        // OK
                                        case 'S':
                                            opc = _LECT_CANTID;
                                            break;

                                        // NOK
                                        case 'N':
                                            UserError(Iptr);
                                            opc = _LECT_CODEAN;
                                            break;

                                        // Error en BBDD.
                                        case 'C':
                                            UserError(Iptr);
                                            opc = _FINALIZAR;
                                            break;

                                        // Resto de casos: Tomar como OK
                                        default:
                                            // Albarán OK
                                            opc = _LECT_CANTID;
                                            break;
                                    }
                                    break;

                                // Resto de teclas: ignorar.
                                default:
                                    opc = _LECT_CODEAN;
                                    break;
                            }
                            break;

                        // Entrar la cantidad recontada.
                        case _LECT_CANTID:
                            cstCantid = "";

                            Pantalla_RecuentoMaterial(Iptr, _SECCION_1);

                            clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf(Iptr.GetMessage(_LECT_CANTID_1, "Introduzca cantidad"));
                            clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1); printf(Iptr.GetMessage(_LECT_CANTID_2, "<F2:Poner a cero>"));
                            clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2);

                            ret = leer_char(ref cstLectura, 10);
                            cstCantid = cstLectura;

                            // Tratamiento lectura
                            switch (ret)
                            {
                                // Ir a pedir EAN
                                case _TECLA_ESC:
                                    opc = _LECT_CODEAN;
                                    break;

                                // Poner a cero cantidad recontada de la línea
                                case _TECLA_F2:
                                    opc = _PONER_CERO;
                                    break;

                                case _TECLA_0:
                                    RetVal = Iptr.RecValidarCnt((string)cstCantid);

                                    switch (RetVal)
                                    {
                                        // OK
                                        case 'S':
                                            // Albarán OK
                                            opc = _DATOS_OK;
                                            break;

                                        // NOK
                                        case 'N':
                                            UserError(Iptr);
                                            opc = _LECT_CANTID;
                                            break;

                                        // Cantidad > cantidad pedida
                                        case 'P':
                                            UserError(Iptr);

                                            clreol(0, _LINEA_INPUTS + 1);
                                            ret = preguntar(Iptr.GetMessage(_LECT_CANTID_3, "Aceptar cantidad (S/N)"), _LINEA_INPUTS + 1);

                                            switch (ret)
                                            {
                                                // Aceptar cantidad recontada > cantidad pedida
                                                case _TECLA_SI:
                                                    opc = _DATOS_OK;
                                                    break;

                                                // Por defecto NO acepta la cantidad
                                                default:
                                                    opc = _LECT_CANTID;
                                                    break;
                                            }
                                            break;

                                        // Error en BBDD
                                        case 'C':
                                            UserError(Iptr);
                                            opc = _FINALIZAR;
                                            break;

                                        // Resto de casos: Tomar como OK
                                        default:
                                            // Albarán OK
                                            opc = _DATOS_OK;
                                            break;
                                    }
                                    break;

                                // Resto de teclas: Ignorar.
                                default:
                                    opc = _LECT_CANTID;
                                    break;
                            }
                            break;

                        // Actualizar cantidad recontada en la línea del albarán
                        case _DATOS_OK:
                            Pantalla_RecuentoMaterial(Iptr, _SECCION_1);

                            clreol(0, _LINEA_INPUTS + 1);
                            ret = preguntar(Iptr.GetMessage(_DATOS_OK_1, "Datos OK (S/N)"), _LINEA_INPUTS + 1);

                            switch (ret)
                            {
                                // Grabar datos
                                case _TECLA_SI:
                                    opc = _ACTUALIZAR;
                                    break;

                                // Resto teclas: Ignorar
                                default:
                                    opc = _LECT_CANTID;
                                    break;
                            }
                            break;

                        // Actualizar cantidad y estado albarán de entrada.
                        case _ACTUALIZAR:
                            RetVal = System.Convert.ToChar(Iptr.RecActualizar());

                            switch (RetVal)
                            {
                                // OK
                                case 'S':
                                    // Recuento actualizado
                                    opc = _LECT_CODEAN;
                                    break;

                                // NOK
                                case 'N':
                                    UserError(Iptr);
                                    opc = _LECT_CANTID;
                                    break;

                                // Error en BBDD
                                case 'C':
                                    UserError(Iptr);
                                    opc = _FINALIZAR;
                                    break;

                                // Resto de casos: Tomar como OK
                                default:
                                    // Recuento actualizado
                                    opc = _LECT_CODEAN;
                                    break;
                            }
                            break;

                        // Consulta de albaranes.
                        case _CONS_ALBARANES:
                            oCns = new CONSULTAALBARANES();
                            oCns.ConsultaAlbaranes(Iptr);
                            oCns = null;

                            // Recuperar albarán actual, si hay.
                            cstAlbaran = (string)Iptr.CnsNumAlb;
                            opc = _VALIDAR_ALBARAN;
                            break;

                        // Cerrar estado recuento de material.
                        case _FINALIZAR:
                            opc = _NO_OPERACION;
                            break;

                        default:
                            opc = _NO_OPERACION;
                            break;
                    }
                }while (opc != _NO_OPERACION);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }

            finally
            {
                RetVal = System.Convert.ToChar(Iptr.RecFinalizar());
            }

            return;
        }

        public void IProradEntry(COMPRADIO.PRORAD Iptr, params System.Object [] parametros)
        {
            RecuentoMaterial(Iptr, parametros);
            return;
        }
    }
}