
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRORAD;

namespace ENTRADA.CNS
{
    public class CONSULTAALBARAN : Base
    {
        private const int _LIN_PAG = 5;

        private const int _CME_LECT_ALBARAN = 0;
        private const int _CME_MOSTRAR_ALBARAN = 1;
        private const int _CME_CARGAR_ALBARAN = 2;
        private const int _CME_INIT_ALBARAN = 3;
        private const int _CME_VALIDAR = 4 ;

        private const int _NO_OPERACION = 21;

        private int nLinea = 0;         // Línea actual.

        // Displays en pantalla.
        public void Pantalla_ConsultaAlbaran(string cstAlbaran,
                                             string cstEAN,
                                             string cstDescri,
                                             string cstCantidad,
                                             string cstLinea)
        {
            if (nLinea == 0)
            {
                // Visualizar la cabecera.
                clrscr();
                gotoxy(0, 0); printf("Contenido de albaranes");

                gotoxy(0, 2); printf("Albaran: %s", cstAlbaran);

                gotoxy(0, 3); printf("NLIN");
                gotoxy(5, 3); printf("EAN");
                gotoxy(24, 3); printf("Cantidad");
            }

            // Visualizar el detalle.
            nLinea += 1;
            gotoxy(0, nLinea * 2 + 2); printf("{0}", cstLinea);
            gotoxy(5, nLinea * 2 + 2); printf("{0}", cstEAN);
            gotoxy(24, nLinea * 2 + 2); printf("{0}", cstCantidad);
            gotoxy(5, nLinea * 2 + 3); printf("{0}", cstDescri.Substring(1, 20));

            return;
        }

        // Proceso de consulta del detalle de UN albarán.
        public void ConsultaAlbaran(params object[] parametrosC)
        {
            COMPRADIO.PRORAD Iptr = (COMPRADIO.PRORAD)parametrosC[0];

            string cstAlbaran = "";
            string cstEAN;
            string cstLinea;
            string cstDescri;
            string cstCantidad;
            string cstSkipR = "";

            string cstLectura = "";
            int ret, opc;
            char RetVal;

            opc = _CME_INIT_ALBARAN;

            try
            {
                do
                {
                    switch (opc)
                    {
                        // Entrar nº de albarán.
                        case _CME_LECT_ALBARAN:
                            cstAlbaran = "";
                            cstEAN = "";
                            cstDescri = "";
                            cstCantidad = "";
                            cstAlbaran = "";
                            cstLinea = "";

                            nLinea = 0;
                            cstLectura = "";

                            Pantalla_ConsultaAlbaran(cstAlbaran, cstEAN, cstDescri, cstCantidad, cstLinea);

                            clreol(0, _LINEA_INPUTS + 0); gotoxy(0, _LINEA_INPUTS + 0); printf("Introduzca albaran");
                            clreol(0, _LINEA_INPUTS + 1); gotoxy(0, _LINEA_INPUTS + 1); printf("<F2:Consulta ESC:Salir>");
                            clreol(0, _LINEA_INPUTS + 2); gotoxy(0, _LINEA_INPUTS + 2); ret = leer_char(ref cstLectura, 10);

                            cstAlbaran = cstLectura;

                            switch (ret)
                            {
                                case _TECLA_ESC:
                                    opc = _NO_OPERACION;
                                    break;

                                // Validar y cargar detalle albarán
                                case _TECLA_0:
                                    cstSkipR = "V";
                                    opc = _CME_VALIDAR;
                                    break;

                                //// Consulta y selección de albaranes.
                                //case _TECLA_F2:
                                //    ConsultaAlbaranes(Iptr);

                                //    // Recuperar albarán actual, si hay.
                                //    Iptr->get_cnsnumalb(vLectAlbaran);
                                //    vLectAlbaran->ChangeType(VT_BSTR);
                                //    cstSkipR = "V";
                                //    vSkipR->SetString((const char*)cstSkipR);
                                //    opc = _CME_VALIDAR;
                                //    break;

                                // Resto de teclas: Ignorar
                                default:
                                    opc = _CME_LECT_ALBARAN;
                                    break;
                            }
                            break;

                        // Mostrar la siguiente línea de albarán.
                        case _CME_MOSTRAR_ALBARAN:
                            cstSkipR = "M";
                            RetVal = System.Convert.ToChar(Iptr.ConsultaAlbaran(cstAlbaran, cstSkipR));

                            switch (RetVal)
                            {
                                // EOF
                                case 'O':
                                    // No hay más líneas
                                    UserError(Iptr);
                                    opc = _CME_LECT_ALBARAN;
                                    break;

                                // OK
                                case 'S':
                                    // Recuperar código de barras.
                                    cstEAN = Iptr.CnsCodEAN;

                                    // Recuperar descripción del artículo.
                                    cstDescri = Iptr.CnsDesArt.ToString();

                                    // Recuperar cantidad recontada
                                    cstCantidad = Iptr.CnsCanRec.ToString();

                                    // Recuperar línea relativa.
                                    cstLinea = Iptr.CnsOrden.ToString();

                                    // Visualizar línea actual
                                    Pantalla_ConsultaAlbaran(cstAlbaran, cstEAN, cstDescri, cstCantidad, cstLinea);

                                    if (nLinea >= _LIN_PAG)
                                    {
                                        // Siguiente pantalla
                                        clreol(0, _LINEA_INPUTS + 1);
                                        gotoxy(0, _LINEA_INPUTS + 1);
                                        printf("Pulse una tecla ..."); getkey();

                                        // Control visualización
                                        nLinea = 0;
                                    }

                                    // Cargar más líneas
                                    opc = _CME_MOSTRAR_ALBARAN;
                                    break;

                                // Error
                                case 'E':
                                    // Error de programa
                                    UserError(Iptr);
                                    opc = _NO_OPERACION;
                                    break;

                                // Resto de casos: Error.
                                default:
                                    // Error
                                    clreol(0, 14); gotoxy(0, 14);
                                    printf("Resultado indefinido. Pulse tecla.");
                                    getkey();
                                    opc = _NO_OPERACION;
                                    break;
                            }
                            break;

                        // Tomar datos del albarán activo.
                        case _CME_CARGAR_ALBARAN:
                            // Recuperar albarán actual, si hay.
                            cstAlbaran = (string)Iptr.NumAlb;
                            cstSkipR = "V";
                            opc = _CME_VALIDAR;
                            break;

                        // Validar el albarán.
                        // Se viene desde _CME_CARGAR_ALBARAN y _CME_LECT_ALBARAN(TECLA_0)
                        case _CME_VALIDAR:
                            RetVal = System.Convert.ToChar(Iptr.ConsultaAlbaran(cstAlbaran, cstSkipR));

                            switch (RetVal)
                            {
                                // Error
                                case 'E':
                                    // Error. Albarán no existe
                                    UserError(Iptr);
                                    opc = _CME_LECT_ALBARAN;
                                    break;

                                // OK
                                case 'S':
                                    // Albarán OK
                                    nLinea = 0;
                                    opc = _CME_MOSTRAR_ALBARAN;
                                    break;

                                // NOK
                                case 'N':
                                    // Albarán en blanco
                                    opc = _CME_LECT_ALBARAN;
                                    break;

                                // Resto de casos: Tomar como OK
                                default:
                                    // Albarán OK
                                    nLinea = 0;
                                    opc = _CME_MOSTRAR_ALBARAN;
                                    break;
                            }
                            break;

                        // Inicializar valores.
                        case _CME_INIT_ALBARAN:
                            cstAlbaran = "";
                            cstEAN = "";
                            cstCantidad = "";
                            cstAlbaran = "";

                            nLinea = 0;
                            opc = _CME_CARGAR_ALBARAN;
                            break;

                        default:
                            opc = _NO_OPERACION;
                            break;
                    }
                } while (opc != _NO_OPERACION);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }

            finally
            {
                // Cerrar proceso de consulta.
                RetVal = Convert.ToChar(Iptr.ConsultaAlbaran(cstLectura, "Z"));
            }
            return;
        }

    }
}
